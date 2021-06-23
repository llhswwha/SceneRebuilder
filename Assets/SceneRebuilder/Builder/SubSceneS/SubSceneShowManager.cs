using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneShowManager : MonoBehaviour
{
    public static SubSceneShowManager Instance;

    public SubSceneManager sceneManager;
    //public SubScene_Out0[] scenes_Out0;//
    public SubScene_Out1[] scenes_Out1;
    public SubScene_In[] scenes_In;

    public List<SubScene_Out0> scenes_Out0_Part = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_Tree = new List<SubScene_Out0>();

    public Camera[] cameras;

    [ContextMenu("Init")]
    public void Init()
    {
        sceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(true);

        var scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(true);
        foreach(var s in scenes_Out0)
        {
            if(s.contentType==SceneContentType.Part)
            {
                scenes_Out0_Part.Add(s);
            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_Out0_Tree.Add(s);
            }
        }
        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(true);

        cameras = GameObject.FindObjectsOfType<Camera>();
    }

    public bool IsAutoLoad = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();

        if(IsAutoLoad)
            sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());//1.启动时自动加载模型

        if(AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.ShowNodeDistance = this.DisOfVisible;
            AreaTreeNodeShowManager.Instance.HideNodeDistance = this.DisOfHidden;
        }
    }

    public float DisOfVisible = 1600;//40
    public float DisOfLoad = 2500;//50
    public float DisOfHidden = 3600;//60
    public float DisOfUnLoad = 4900;//70

    public float MinDisSqrtToCam = 0;

    public float MaxDisSqrtToCam = 0;

    public float MinDisToCam = 0;

    public float MaxDisToCam = 0;

    public float AvgDisToCam = 0;

    public double TimeOfDis = 0;

    public SubScene_Base MinDisScene;

    public List<SubScene_Base> visibleScenes = new List<SubScene_Base>();
    public List<SubScene_Base> loadScenes = new List<SubScene_Base>();
    public List<SubScene_Base> hiddenScenes = new List<SubScene_Base>();
    public List<SubScene_Base> unloadScenes = new List<SubScene_Base>();

    void CalculateDistance(List<SubScene_Base> scenes)
    {
        DateTime start = DateTime.Now;

        MaxDisSqrtToCam = 0;

        MinDisSqrtToCam = float.MaxValue;

        float sumDis = 0;

        visibleScenes = new List<SubScene_Base>();
        hiddenScenes = new List<SubScene_Base>();
        loadScenes = new List<SubScene_Base>();
        unloadScenes = new List<SubScene_Base>();

        foreach (var scene in scenes)
        {
            float disToCams = 0;
            foreach (var cam in cameras)
            {
                Vector3 cP = cam.transform.position;
                float dis = scene.bounds.SqrDistance(cP);
                if (dis > disToCams)
                {
                    disToCams = dis;
                }
            }

            if (disToCams > MaxDisSqrtToCam)
            {
                MaxDisSqrtToCam = disToCams;
            }
            if (disToCams < MinDisSqrtToCam)
            {
                MinDisSqrtToCam = disToCams;
                MinDisScene = scene;
            }
            sumDis += disToCams;

            if (disToCams <= DisOfLoad)
            {
                loadScenes.Add(scene);
            }
            if (disToCams <= DisOfVisible)
            {
                visibleScenes.Add(scene);
            }
            if (disToCams > DisOfUnLoad)
            {
                unloadScenes.Add(scene);
            }
            if (disToCams > DisOfHidden)
            {
                hiddenScenes.Add(scene);
            }
            scene.DisToCam = disToCams;
        }

        AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
        MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
        MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

        foreach (var scene in visibleScenes)
        {
            scene.ShowObjects();
        }
        foreach (var scene in hiddenScenes)
        {
            scene.HideObjects();
        }
        foreach (var scene in loadScenes)
        {
            scene.LoadSceneAsync(null);
        }
        foreach (var scene in unloadScenes)
        {
            scene.UnLoadSceneAsync();
        }


        TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
    }

    // Update is called once per frame
    void Update()
    {
        List<SubScene_Base> subScenes = new List<SubScene_Base>();
        foreach(var scene in scenes_In)
        {
            subScenes.Add(scene);
        }
        foreach (var scene in scenes_Out1)
        {
            subScenes.Add(scene);
        }
        CalculateDistance(subScenes);
    }
}

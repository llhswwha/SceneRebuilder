using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneShowManager : MonoBehaviour
{
    public SubSceneManager sceneManager;
    public SubScene_Out0[] scenes_Out0;
    public SubScene_Out1[] scenes_Out1;
    public SubScene_In[] scenes_In;

    public Camera[] cameras;

    [ContextMenu("Init")]
    public void Init()
    {
        sceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(true);
        scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(true);
        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(true);

        cameras = GameObject.FindObjectsOfType<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
        sceneManager.LoadScenesEx(scenes_Out0);//1.启动时自动加载模型
    }

    public float DisOfVisible = 1600;//40
    public float DisOfLoad = 2500;//50
    public float DisOfHidden = 3600;//60
    public float DisOfUnLoad = 4900;//70

    public float MinDisToCam = 0;

    public float MaxDisToCam = 0;

    public float AvgDisToCam = 0;

    public double TimeOfDis = 0;

    public List<SubScene_Base> visibleScenes = new List<SubScene_Base>();
    public List<SubScene_Base> hiddenScenes = new List<SubScene_Base>();
    public List<SubScene_Base> loadScenes = new List<SubScene_Base>();
    public List<SubScene_Base> unloadScenes = new List<SubScene_Base>();

    void CalculateDistance()
    {
        DateTime start = DateTime.Now;

        MaxDisToCam = 0;

        MinDisToCam = float.MaxValue;

        float sumDis = 0;

        visibleScenes = new List<SubScene_Base>();
        hiddenScenes = new List<SubScene_Base>();
        loadScenes = new List<SubScene_Base>();
        unloadScenes = new List<SubScene_Base>();

        foreach (var scene in scenes_In)
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

            if (disToCams > MaxDisToCam)
            {
                MaxDisToCam = disToCams;
            }
            if (disToCams < MinDisToCam)
            {
                MinDisToCam = disToCams;
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

        AvgDisToCam = sumDis / scenes_In.Length;

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
        CalculateDistance();
    }
}

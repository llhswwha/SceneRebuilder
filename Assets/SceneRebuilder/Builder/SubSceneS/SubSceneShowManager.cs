using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneShowManager : SingletonBehaviour<SubSceneShowManager>
{
    //public static SubSceneShowManager Instance;

    public SubSceneManager sceneManager;
    public SubScene_Out0[] scenes_Out0;
    public SubScene_Out1[] scenes_Out1;
    public SubScene_In[] scenes_In;

    public List<SubScene_In> scenes_In_Part = new List<SubScene_In>();
    public List<SubScene_In> scenes_In_Tree = new List<SubScene_In>();

    public List<SubScene_Out0> scenes_Out0_Part = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_Tree = new List<SubScene_Out0>();
    //public List<SubScene_Out0> scenes_Out0_TreeNode = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Hidden = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Shown = new List<SubScene_Out0>();
    public List<SubScene_Base> scenes_TreeNode_Hidden = new List<SubScene_Base>();
    public List<SubScene_Base> scenes_TreeNode_Shown = new List<SubScene_Base>();
    public Camera[] cameras;

    [ContextMenu("GetSceneCountInfoEx")]
    public string GetSceneCountInfoEx()
    {
        return this.GetSceneCountInfo()+"\n"+this.GetShowSceneCountInfo();
    }

    public bool IncludeInactive = false;

    [ContextMenu("GetSceneCountInfo")]
    public string GetSceneCountInfo()
    {

        var alls=GameObject.FindObjectsOfType<SubScene_Base>(IncludeInactive);
        var allsInfo=GetSceneRenderInfo(alls);
        var ins=GameObject.FindObjectsOfType<SubScene_In>(IncludeInactive);
        var insInfo=GetSceneRenderInfo(ins);
        var out0s=GameObject.FindObjectsOfType<SubScene_Out0>(IncludeInactive);
        var out0sInfo=GetSceneRenderInfo(out0s);
        var out1s=GameObject.FindObjectsOfType<SubScene_Out1>(IncludeInactive);
        var out1sInfo=GetSceneRenderInfo(out1s);
        
        // List<SubScene_Out0> out0s_tree
        
        string log=$"all:{alls.Length}(v:{allsInfo[0]:F0},r:{allsInfo[1]:F0}) | out0:{out0s.Length}(v:{out0sInfo[0]:F0},r:{out0sInfo[1]:F0}) out1:{out1s.Length}(v:{out1sInfo[0]:F0},r:{out1sInfo[1]:F0}) | ins:{ins.Length}(v:{insInfo[0]:F0},r:{insInfo[1]:F0})";
        Debug.Log("GetSceneCountInfo "+log);
        return log;
    }

    [ContextMenu("GetShowSceneCountInfo")]
    public string GetShowSceneCountInfo()
    {
        List<SubScene_Out0> out0TreeAndPart=new List<SubScene_Out0>();
        out0TreeAndPart.AddRange(scenes_Out0_Tree);
        out0TreeAndPart.AddRange(scenes_Out0_Part);

        var scenes_Out0_Info=GetSceneRenderInfo(out0TreeAndPart);
        var scenes_In_Info=GetSceneRenderInfo(scenes_In);
        var scenes_Out1_Info=GetSceneRenderInfo(scenes_Out1);
        var scenes_Out0_TreeNode_Shown_Info=GetSceneRenderInfo(scenes_Out0_TreeNode_Shown);
        var scenes_Out0_TreeNode_Hidden_Info=GetSceneRenderInfo(scenes_Out0_TreeNode_Hidden);

        string log= $"out0:{out0TreeAndPart.Count}(v:{scenes_Out0_Info[0]:F0},r:{scenes_Out0_Info[1]:F0})"
        +$" out0_node_show:{scenes_Out0_TreeNode_Shown.Count}(v:{scenes_Out0_TreeNode_Shown_Info[0]:F0},r:{scenes_Out0_TreeNode_Shown_Info[1]:F0})"
        +$"\nin:{scenes_In.Length}(v:{scenes_In_Info[0]:F0},r:{scenes_In_Info[1]:F0})"
        +$" out1:{scenes_Out1.Length}(v:{scenes_Out1_Info[0]:F0},r:{scenes_Out1_Info[1]:F0})"
        +$" out0_node_hide:{scenes_Out0_TreeNode_Hidden.Count}(v:{scenes_Out0_TreeNode_Hidden_Info[0]:F0},r:{scenes_Out0_TreeNode_Hidden_Info[1]:F0})";
        Debug.Log("GetShowSceneCountInfo "+log);
        return log;
    }

    public float[] GetSceneRenderInfo<T>(IEnumerable<T> scenes) where T : SubScene_Base
    {
        float vCount=0;
        float rCount=0;
        foreach(T scene in scenes){
            vCount+=scene.vertexCount;
            rCount+=scene.rendererCount;
        }
        return new float[]{vCount,rCount};
    }
    
    public string GetSceneInfo()
    {
        return $"visible:{visibleScenes.Count},loaded:{loadScenes.Count-WaitingScenes_ToLoad.Count}/{loadScenes.Count},hidden:{hiddenScenes.Count},unloaded:{unloadScenes.Count-WaitingScenes_ToUnLoad.Count}/{unloadScenes.Count}";
    }

    public string GetDisInfo()
    {
        return $"Min:{MinDisToCam:F0},Max:{MaxDisToCam:F0},MinSqrt:{MinDisSqrtToCam:F0},MaxSqrt:{MaxDisSqrtToCam:F0},time:{TimeOfDis:F1}";
    }

    [ContextMenu("Init")]
    public void Init()
    {
        sceneManager = GameObject.FindObjectOfType<SubSceneManager>(IncludeInactive);
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(IncludeInactive);
        scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(IncludeInactive);

        scenes_Out0_Part.Clear();
        scenes_Out0_Tree.Clear();
        // scenes_Out0_TreeNode.Clear();
        
        foreach(var s in scenes_Out0)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_Out0_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_Out0_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }

        scenes_In_Part.Clear();
        scenes_In_Tree.Clear();
        foreach(var s in scenes_In)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_In_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_In_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }


        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(IncludeInactive);

        cameras = GameObject.FindObjectsOfType<Camera>();

        List<ModelAreaTree> HiddenTrees = new List<ModelAreaTree>();
        List<ModelAreaTree> ShownTrees = new List<ModelAreaTree>();
        var ts = GameObject.FindObjectsOfType<ModelAreaTree>(IncludeInactive);

        scenes_Out0_TreeNode_Hidden.Clear();
        scenes_Out0_TreeNode_Shown.Clear();
        foreach (ModelAreaTree t in ts)
        {
            if (t.IsHidden && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                //HiddenTreesVertexCount += t.VertexCount;
                scenes_TreeNode_Hidden.AddRange(t.GetComponentsInChildren<SubScene_Base>(IncludeInactive));
                scenes_Out0_TreeNode_Hidden.AddRange(t.GetComponentsInChildren<SubScene_Out0>(IncludeInactive));
            }
            else if (t.IsHidden == false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                //ShownTreesVertexCount += t.VertexCount;

                scenes_TreeNode_Shown.AddRange(t.GetComponentsInChildren<SubScene_Base>(IncludeInactive));
                scenes_Out0_TreeNode_Shown.AddRange(t.GetComponentsInChildren<SubScene_Out0>(IncludeInactive));
            }
        }
    }

    public bool IsAutoLoad = false;

    private void Awake()
    {
        //Instance = this;
        Init();
    }

    public List<SubScene_Base> WaitingScenes = new List<SubScene_Base>();//Waiting To Finish Load

    public List<SubScene_Base> WaitingScenes_ToLoad = new List<SubScene_Base>();
    public SubScene_Base LoadingScene = null;
    public List<SubScene_Base> WaitingScenes_ToUnLoad = new List<SubScene_Base>();
    public SubScene_Base UnLoadingScene = null;
    public bool IsUpdateTreeNodeByDistance = false;
    public bool IsUpdateDistance = true;
    public bool IsEnableLoad = false; 
    public bool IsEnableUnload=false;
    public bool IsEnableHide = false; 
    public bool IsEnableShow=false;

    public void LoadStartScens(Action<float,bool> onComplete=null)
    {
        List<SubScene_Out0> scenes=new List<SubScene_Out0>();
        // scenes.AddRange(scenes_Out0_Tree);
        scenes.AddRange(scenes_Out0_Part);
        scenes.AddRange(scenes_Out0_TreeNode_Shown);
        LoadStartScens_Innder(scenes, onComplete);
    }

    public void LoadStartScens_All(Action<float, bool> onComplete = null)
    {
        List<SubScene_Out0> scenes = new List<SubScene_Out0>();
        BuildingModelManager.Instance.ShowDetail();
        // scenes.AddRange(scenes_Out0_Tree);
        scenes.AddRange(scenes_Out0_Part);
        scenes.AddRange(scenes_Out0_TreeNode_Shown);
        scenes.AddRange(scenes_Out0_TreeNode_Hidden);
        LoadStartScens_Innder(scenes, onComplete);
    }

    private void LoadStartScens_Innder<T>(List<T> scenes,Action<float, bool> onComplete = null) where T :SubScene_Base
    {
        if (scenes.Count > 0)
        {
            AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
            sceneManager.LoadScenesEx(scenes.ToArray(), (p, r) =>
            {
                WaitingScenes.AddRange(scenes);
                AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
                if (onComplete != null) onComplete(p, r);
            });
        }
    }

    public void LoadShownTreeNodes()
    {
        LoadScenes(scenes_Out0_TreeNode_Shown, null);
    }

    public void LoadHiddenTreeNodes(Action<float, bool> onComplete = null)
    {
        BuildingModelManager.Instance.ShowDetail();
        LoadScenes(scenes_Out0_TreeNode_Hidden, onComplete);
    }

    public void LoadOut0BuildingScenes()
    {
        List<SubScene_Out0> scene_Out0s=new List<SubScene_Out0>();
        scene_Out0s.AddRange(scenes_Out0_Tree);
        scene_Out0s.AddRange(scenes_Out0_Part);
        // scene_Out0s.AddRange(scenes_Out0_TreeNode_Shown);
        if(scene_Out0s.Count>0){
            AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
            sceneManager.LoadScenesEx(scene_Out0s.ToArray(), (p,r) =>
                {
                    WaitingScenes.AddRange(scene_Out0s);
                    AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
                });
        }
    }

    public void LoadScenes<T>(List<T> scenes,Action<float,bool> finished) where T : SubScene_Base
    {
        sceneManager.LoadScenesEx(scenes.ToArray(), (p,r) =>
                {
                    WaitingScenes.AddRange(scenes);
                    if(finished!=null){
                        finished(p,r);
                    }
                });
    }

    // public void LoadOut0TreeNodeSceneTop1()
    // {
    //     //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
    //     //sceneManager.LoadScenesEx(scenes_Out0_Tree.ToArray());
    //     sceneManager.LoadScenesEx(scenes_Out0_TreeNode_Shown.ToArray(), () =>
    //     {
    //             //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = true;
    //             WaitingScenes.AddRange(scenes_Out0_TreeNode_Shown);
    //             //AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
    //     });
    // }

    public void LoadOut0TreeNodeSceneTopN(int index)
    {
        scenes_Out0_TreeNode_Shown.Sort((a,b)=>b.vertexCount.CompareTo(a.vertexCount));
        var scene=scenes_Out0_TreeNode_Shown[index];
        Debug.Log($"LoadOut0TreeNodeSceneTopN index:{index} scene:{scene.name} vertexCount:{scene.vertexCount}");
        scene.LoadSceneAsync(null);
    }

    public void LoadOut0TreeNodeSceneTopBiggerN(int index)
    {
        scenes_Out0_TreeNode_Shown.Sort((a,b)=>b.vertexCount.CompareTo(a.vertexCount));
        List<SubScene_Base> topsScenes=new List<SubScene_Base>();
        float vertexCount=0;
        for(int i=index;i<scenes_Out0_TreeNode_Shown.Count;i++)
        {
             var scene=scenes_Out0_TreeNode_Shown[i];
            topsScenes.Add(scene);
            vertexCount+=scene.vertexCount;
        }
        // sceneManager.LoadScenesEx(topsScenes.ToArray(), (p,r) =>
        // {
        //         WaitingScenes.AddRange(topsScenes);
        // });

        LoadScenes(topsScenes,(p,r)=>{
            Debug.Log($"LoadOut0TreeNodeSceneTopN2 index:{index} scene:{topsScenes.Count} vertexCount:{vertexCount}");
        });

        Debug.Log($"LoadOut0TreeNodeSceneTopN1 index:{index} scene:{topsScenes.Count} vertexCount:{vertexCount}");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();

        if (IsAutoLoad)
        {
            LoadStartScens();
        }

        if (AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.ShowNodeDistance = this.DisOfVisible;
            AreaTreeNodeShowManager.Instance.HideNodeDistance = this.DisOfHidden;
        }

        BuildingModelManager.Instance.ShowDetail();

        CheckSceneIndex();

        IdDictionary.InitInfos();
    }

    private void CheckSceneIndex()
    {
        DateTime start = DateTime.Now;
        var alls=GameObject.FindObjectsOfType<SubScene_Base>(true);
        foreach(var s in alls){
            if(s.sceneArg.index<=0){
                Debug.LogError($"SubSceneShowManager.CheckSceneIndex index<=0 sName:{s.name} index:{s.sceneArg.index}");
            }
        }
        Debug.Log($"CheckSceneIndex Time:{(DateTime.Now - start).ToString()}");
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

    public double TimeOfLoad = 0;

    public SubScene_Base MinDisScene;


    public List<SubScene_Base> visibleScenes = new List<SubScene_Base>();
    public List<SubScene_Base> loadScenes = new List<SubScene_Base>();
    public List<SubScene_Base> hiddenScenes = new List<SubScene_Base>();
    public List<SubScene_Base> unloadScenes = new List<SubScene_Base>();

   void LoadUnloadScenes()
    {
        //if (EnableLoadUnload == false) return;
        DateTime start = DateTime.Now;
        if(IsEnableShow)
            foreach (var scene in visibleScenes)
            {
                scene.ShowObjects();
            }
        if(IsEnableHide)
            foreach (var scene in hiddenScenes)
            {
                scene.HideObjects();
            }
        if(IsEnableLoad)
            //var waittingScenes=loadScenes.Where(i=>i)
            foreach (var scene in loadScenes)
            {
                if (scene.IsLoading || scene.IsLoaded)
                {
                    // Debug.LogWarning($"[LoadUnloadScenes.Load] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
                    continue;
                }
                //scene.LoadSceneAsync(null);
                if(!WaitingScenes_ToLoad.Contains(scene))
                    WaitingScenes_ToLoad.Add(scene);
            }
        if(IsEnableUnload)
            foreach (var scene in unloadScenes)
            {
                if (scene.IsLoaded==false)
                {
                    // Debug.LogWarning($"[LoadUnloadScenes.Unload] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
                    continue;
                }
                //scene.UnLoadSceneAsync();
                if (!WaitingScenes_ToUnLoad.Contains(scene))
                    WaitingScenes_ToUnLoad.Add(scene);
            }
        TimeOfLoad = (DateTime.Now - start).TotalMilliseconds;
    }

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

            if (disToCams <= DisOfLoad)
            {
                loadScenes.Add(scene);
            }

            if(scene.boundsGo==null){
                // Debug.LogError("SubSceneShowManager.CalculateDistance scene.boundsGo==null :"+scene);
            }
            else{
                if (disToCams <= DisOfVisible)
                {
                    scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.2f);
                }
                else if (disToCams <= DisOfLoad)
                {
                    scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 0.5f, 0, 0.2f);
                }
                else if (disToCams <= DisOfHidden)
                {
                    scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 0, 0.2f);
                }
                else if (disToCams <= DisOfUnLoad)
                {
                    scene.boundsGo.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.2f);
                }
            }

            scene.DisToCam = disToCams;
        }

        AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
        MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
        MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

        TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (WaitingScenes.Count > 0)
        {
            Debug.Log("CheckWaittingScenes 1:"+ WaitingScenes.Count);
            for(int i = 0; i < WaitingScenes.Count; i++)
            {
                var scene = WaitingScenes[i];
                if(scene.IsLoaded==false)//加载失败的情况，比如是inactive的情况
                {
                     WaitingScenes.RemoveAt(i);
                    i--;
                }
                else if (scene.GetSceneObjectCount()>0)
                {
                    WaitingScenes.RemoveAt(i);
                    i--;
                }
                else
                {
                    scene.CheckGetSceneObjects();
                }
            }
            Debug.Log("CheckWaittingScenes 2:" + WaitingScenes.Count);
        }

        if (WaitingScenes_ToLoad.Count > 0)
        {
            if (LoadingScene == null || (LoadingScene != null && LoadingScene.IsLoaded == true) )
            {
                LoadingScene = WaitingScenes_ToLoad[0];
                WaitingScenes_ToLoad.RemoveAt(0);
                LoadingScene.LoadSceneAsync((b, s) =>
                {
                    if (b)
                    {
                        WaitingScenes.Add(s);
                        LoadingScene = null;
                    }
                });
            }
        }

        if (WaitingScenes_ToUnLoad.Count > 0)
        {
            if (UnLoadingScene == null || (UnLoadingScene != null && UnLoadingScene.IsLoaded == false))
            {
                UnLoadingScene = WaitingScenes_ToUnLoad[0];
                WaitingScenes_ToUnLoad.RemoveAt(0);
                UnLoadingScene.UnLoadGos();
                UnLoadingScene.ShowBounds();
                UnLoadingScene = null;
            }
        }

        if (IsUpdateDistance)
        {
            List<SubScene_Base> subScenes = new List<SubScene_Base>();
            //foreach (var scene in scenes_In)
            //{
            //    subScenes.Add(scene);
            //}
            //foreach (var scene in scenes_Out1)
            //{
            //    subScenes.Add(scene);
            //}
            foreach (var scene in scenes_Out0_TreeNode_Hidden)
            {
                subScenes.Add(scene);
            }
            CalculateDistance(subScenes);
            LoadUnloadScenes();
        }
    }
}

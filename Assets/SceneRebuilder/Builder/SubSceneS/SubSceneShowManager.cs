using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
public class SubSceneShowManager : SingletonBehaviour<SubSceneShowManager>
{
    public void SetEnable(bool isEnable)
    {
        this.enabled = isEnable;
        if (isEnable)
        {
            IsEnableHide = true;
            IsEnableShow = true;
            IsEnableLoad = true;
            IsEnableUnload = true;
            IsUpdateDistance = true;
            IsEnableIn = true;
            IsEnableOut1 = true;
            IsEnableLOD = true;
        }
    }

    //public static SubSceneShowManager Instance;

    public SubSceneManager sceneManager;
    public List<SubScene_Out0> scenes_Out0 = new List<SubScene_Out0>();
    public List<SubScene_Out1> scenes_Out1 = new List<SubScene_Out1>();
    public List<SubScene_In> scenes_In = new List<SubScene_In>();
    public List<SubScene_LODs> scenes_LODs = new List<SubScene_LODs>();
    public List<SubScene_GPUI> scenes_GPUI = new List<SubScene_GPUI>();
    public List<SubScene_In> scenes_In_Part = new List<SubScene_In>();
    public List<SubScene_In> scenes_In_Tree = new List<SubScene_In>();

    public List<SubScene_Out0> scenes_Out0_Part = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_Tree = new List<SubScene_Out0>();
    //public List<SubScene_Out0> scenes_Out0_TreeNode = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Hidden = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Shown = new List<SubScene_Out0>();
    public SubSceneBag scenes_TreeNode_Hidden = new SubSceneBag();
    public SubSceneBag scenes_TreeNode_Shown = new SubSceneBag();
    public List<Camera> cameras = new List<Camera>();

    [ContextMenu("GetSceneCountInfoEx")]
    public string GetSceneCountInfoEx()
    {
        return this.GetSceneCountInfo()+"\n"+this.GetShowSceneCountInfo();
    }

    public static bool IncludeInactive = true;

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
        +$"\nin:{scenes_In.Count}(v:{scenes_In_Info[0]:F0},r:{scenes_In_Info[1]:F0})"
        +$" out1:{scenes_Out1.Count}(v:{scenes_Out1_Info[0]:F0},r:{scenes_Out1_Info[1]:F0})"
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
        return $"[V:{visibleScenes.Count},L:{loadScenes.Count-WaitingScenes_ToLoad.Count}/{loadScenes.Count},W:{WaitingScenes_ToLoad.Count}][H:{hiddenScenes.Count},UL:{unloadScenes.Count-WaitingScenes_ToUnLoad.Count}/{unloadScenes.Count},W:{WaitingScenes_ToUnLoad.Count}]";
    }

    public string GetDisInfo()
    {
        return $"Min:{MinDisToCam:F0},Max:{MaxDisToCam:F0},MinSqrt:{MinDisSqrtToCam:F0},MaxSqrt:{MaxDisSqrtToCam:F0},time:{TimeOfDis:F1}";
    }

    [ContextMenu("Init")]
    public void InitScenes()
    {
        sceneManager = SubSceneManager.Instance;
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(IncludeInactive).ToList();
        scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(IncludeInactive).ToList();
        scenes_LODs = GameObject.FindObjectsOfType<SubScene_LODs>(IncludeInactive).ToList();
        scenes_GPUI= GameObject.FindObjectsOfType<SubScene_GPUI>(IncludeInactive).ToList();

        Debug.Log($"SubSceneShowManager.InitScenes IncludeInactive:{IncludeInactive} In:{scenes_In.Count} Out0:{scenes_Out0.Count} LODs:{scenes_LODs.Count} GPUI:{scenes_GPUI.Count}");

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
            else if (s.contentType == SceneContentType.Tree)
            {
                scenes_Out0_Tree.Add(s);
            }
            else
            {

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


        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(IncludeInactive).ToList();



        List<ModelAreaTree> HiddenTrees = new List<ModelAreaTree>();
        List<ModelAreaTree> ShownTrees = new List<ModelAreaTree>();
        var ts = GameObject.FindObjectsOfType<ModelAreaTree>(IncludeInactive);

        scenes_Out0_TreeNode_Hidden.Clear();
        scenes_Out0_TreeNode_Shown.Clear();
        foreach (ModelAreaTree t in ts)
        {
            if (t.GetIsHidden() && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                //HiddenTreesVertexCount += t.VertexCount;

                scenes_TreeNode_Hidden.AddRange(t.GetComponentsInChildren<SubScene_Base>(IncludeInactive));
                scenes_Out0_TreeNode_Hidden.AddRange(t.GetComponentsInChildren<SubScene_Out0>(IncludeInactive));
            }
            else if (t.GetIsHidden() == false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                //ShownTreesVertexCount += t.VertexCount;

                scenes_TreeNode_Shown.AddRange(t.GetComponentsInChildren<SubScene_Base>(IncludeInactive));
                scenes_Out0_TreeNode_Shown.AddRange(t.GetComponentsInChildren<SubScene_Out0>(IncludeInactive));
            }
        }
    }

    internal void RemoveScenes(SubScene_Base[] ss)
    {
        int count1 = scenes_Out0.Count;
        int count2 = scenes_Out1.Count;
        int count3 = scenes_In.Count;
        int count4 = scenes_LODs.Count;
        foreach (var s in ss)
        {
            if (s is SubScene_Out0)
            {
                scenes_Out0.Remove(s as SubScene_Out0);
            }
            else if (s is SubScene_Out1)
            {
                scenes_Out1.Remove(s as SubScene_Out1);
            }
            else if (s is SubScene_In)
            {
                scenes_In.Remove(s as SubScene_In);
            }
            else if (s is SubScene_LODs)
            {
                scenes_LODs.Remove(s as SubScene_LODs);
            }
            //else if (s is SubScene_GPUI)
            //{
            //    scenes_GPUI.Remove(s as SubScene_GPUI);
            //}
        }
        int count12 = scenes_Out0.Count;
        int count22 = scenes_Out1.Count;
        int count32 = scenes_In.Count;
        int count42 = scenes_LODs.Count;

        Debug.Log($"SubSceneShowManager.RemoveScenes ss:{ss.Length} {count1}>{count12};{count2}>{count22};{count3}>{count32};{count4}>{count42}");
    }

    internal void AddScenes(SubScene_Base[] ss)
    {
        int count1 = scenes_Out0.Count;
        int count2 = scenes_Out1.Count;
        int count3 = scenes_In.Count;
        int count4 = scenes_LODs.Count;
        foreach (var s in ss)
        {
            if (s is SubScene_Out0)
            {
                scenes_Out0.Add(s as SubScene_Out0);
            }
            else if (s is SubScene_Out1)
            {
                scenes_Out1.Add(s as SubScene_Out1);
            }
            else if (s is SubScene_In)
            {
                scenes_In.Add(s as SubScene_In);
            }
            else if (s is SubScene_LODs)
            {
                scenes_LODs.Add(s as SubScene_LODs);
            }
            else if (s is SubScene_GPUI)
            {
                scenes_GPUI.Add(s as SubScene_GPUI);
            }
        }
        int count12 = scenes_Out0.Count;
        int count22 = scenes_Out1.Count;
        int count32 = scenes_In.Count;
        int count42 = scenes_LODs.Count;

        Debug.Log($"SubSceneShowManager.AddScenes ss:{ss.Length} {count1}>{count12};{count2}>{count22};{count3}>{count32};{count4}>{count42}");
    }

    public void InitCameras()
    {
        int cameraCount = 0;
        foreach(var c in cameras)
        {
            if (c == null) continue;
            if (c.gameObject.activeInHierarchy == false) continue;
            cameraCount++;
        }
        if (cameraCount == 0)
        {
            var cs = GameObject.FindObjectsOfType<Camera>();
            cameras = new List<Camera>();
            foreach (var c in cs)
            {
                if (c == null) continue;
                if (c.gameObject.activeInHierarchy == false) continue;
                if (c.name.Contains("UI") || c.name.Contains("RTE"))
                {
                    continue;
                }
                cameras.Add(c);
            }
        }
        
    }

    public bool IsAutoLoad = false;

    private void Awake()
    {
        //Instance = this;
        InitScenes();
    }

    public List<SubScene_Base> WaitingScenes = new List<SubScene_Base>();//Waiting To Finish Load
    public DictList<SubScene_Base> WaitingScenes_ToLoad_Posted = new DictList<SubScene_Base>();
    public DictList<SubScene_Base> WaitingScenes_ToLoad = new DictList<SubScene_Base>();
    public SubScene_Base LoadingScene = null;
    public DictList<SubScene_Base> WaitingScenes_ToUnLoad = new DictList<SubScene_Base>();
    public SubScene_Base UnLoadingScene = null;

    public bool IsUpdateTreeNodeByDistance = false;
    public bool IsUpdateDistance = true;

    public bool IsEnableLoad = true;
    public bool IsEnableUnload = true;
    public bool IsEnableHide = true;
    public bool IsEnableShow = true;

    bool isLoadUserBuildings = false;

    public void LoadUserBuildings(Action<SceneLoadProgress> onComplete = null)
    {
        isLoadUserBuildings = true;
        BuildingController[] deps = GameObject.FindObjectsOfType<BuildingController>();//??????????????????????????????
        BuildingScenesLoadManager.Instance.LoadUserBuildings(deps, onComplete);
    }

    public void LoadStartScenes(Action<SceneLoadProgress> onComplete=null)
    {
        List<SubScene_Out0> scenes = new List<SubScene_Out0>();
        // scenes.AddRange(scenes_Out0_Tree);
        scenes.AddRange(scenes_Out0_Part);
        scenes.AddRange(scenes_Out0_TreeNode_Shown);
        LoadStartScens_Innder(scenes, onComplete);
    }

    public void LoadHiddenTreeNodes(Action<SceneLoadProgress> onComplete = null)
    {
        if (isLoadUserBuildings)
        {
            if (onComplete != null)
            {
                onComplete(new SceneLoadProgress(null, 1, true));
            }
        }
        else
        {
            BuildingModelManager.Instance.ShowDetail();
            LoadScenes(scenes_Out0_TreeNode_Hidden, onComplete, "LoadHiddenTreeNodes");
        }
    }

    public void LoadStartScens_All(Action<SceneLoadProgress> onComplete = null)
    {
        List<SubScene_Out0> scenes = new List<SubScene_Out0>();
        BuildingModelManager.Instance.ShowDetail();
        // scenes.AddRange(scenes_Out0_Tree);
        scenes.AddRange(scenes_Out0_Part);
        scenes.AddRange(scenes_Out0_TreeNode_Shown);
        scenes.AddRange(scenes_Out0_TreeNode_Hidden);
        LoadStartScens_Innder(scenes, onComplete);
    }

    public void LoadStartAndHiddenScenes(Action<SceneLoadProgress> onComplete1, Action<SceneLoadProgress> onComplete2)
    {
        LoadStartScenes((p1) =>
        {
            //Debug.LogError($"LoadStartScens progress:{p1.progress} isFinished:{p1.isAllFinished}");
            if (onComplete1 != null)
            {
                onComplete1(p1);
            }
            if(p1.isAllFinished)
                LoadHiddenTreeNodes((p2) =>
                {
                    //Debug.LogError($"LoadHiddenTreeNodes progress:{p2.progress} isFinished:{p2.isAllFinished}");
                    if (onComplete2 != null)
                    {
                        onComplete2(p2);
                    }
                });
        });
    }

    private void LoadStartScens_Innder<T>(List<T> scenes,Action<SceneLoadProgress> onComplete = null) where T :SubScene_Base
    {
        Debug.Log($"LoadStartScens_Innder scenes:{scenes.Count} onComplete:{onComplete}");
        if (scenes.Count > 0)
        {
            AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
            sceneManager.LoadScenesEx(scenes.ToArray(), (p) =>
            {
                WaitingScenes.AddRange(scenes);
                AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
                if (onComplete != null) onComplete(p);
            }, "LoadStartScens_Innder");
        }
        else
        {
            onComplete(new SceneLoadProgress(null, 1, true));
        }
    }

    public void LoadShownTreeNodes()
    {
        LoadScenes(scenes_Out0_TreeNode_Shown, null, "LoadShownTreeNodes");
    }

    public void LoadOut0BuildingScenes()
    {
        List<SubScene_Out0> scene_Out0s=new List<SubScene_Out0>();
        scene_Out0s.AddRange(scenes_Out0_Tree);
        scene_Out0s.AddRange(scenes_Out0_Part);
        // scene_Out0s.AddRange(scenes_Out0_TreeNode_Shown);
        if(scene_Out0s.Count>0){
            AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;
            sceneManager.LoadScenesEx(scene_Out0s.ToArray(), (p) =>
                {
                    WaitingScenes.AddRange(scene_Out0s);
                    AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = IsUpdateTreeNodeByDistance;
                }, "LoadOut0BuildingScenes");
        }
    }

    public void LoadScenes<T>(List<T> scenes,Action<SceneLoadProgress> finished,string tag) where T : SubScene_Base
    {
        sceneManager.LoadScenesEx(scenes.ToArray(), (p) =>
                {
                    WaitingScenes.AddRange(scenes);
                    if(finished!=null){
                        finished(p);
                    }
                }, tag);
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

        LoadScenes(topsScenes,(p)=>{
            Debug.Log($"LoadOut0TreeNodeSceneTopN2 index:{index} scene:{topsScenes.Count} vertexCount:{vertexCount}");
        }, "LoadOut0TreeNodeSceneTopBiggerN");

        Debug.Log($"LoadOut0TreeNodeSceneTopN1 index:{index} scene:{topsScenes.Count} vertexCount:{vertexCount}");
    }

    public void StopLoadScene(GameObject target=null)
    {
        Debug.Log("StopLoadScene:"+ target);
        SetIsUpdateDistance(false);
        ClearList();
    }

    private void SetIsUpdateDistance(bool v)
    {
        Debug.Log($"SetIsUpdateDistance v:{v}");
        IsUpdateDistance = v;
        LODManager.Instance.IsEnableLoadLod0 = v;
        //LODManager.Instance.IsEnableUpdate = v;
    }

    private void ClearList()
    {
        WaitingScenes_ToLoad.Clear();
        WaitingScenes_ToUnLoad.Clear();
        visibleScenes.Clear();
        hiddenScenes.Clear();
        loadScenes.Clear();
        unloadScenes.Clear();
    }

    public void StartLoadScene(GameObject target)
    {
        //Debug.Log("StartLoadScene:" + target);
        //IsUpdateDistance = true;
        //LoadScenesOfTarget(target);

        if (target == null)
        {
            SetIsUpdateDistance(true); ;//1
            Debug.LogWarning($"SubSceneShowManager.StartLoadScene Warning! 1 target == null");
            return;
        }

        loadScenesCount++;
        BuildingModelInfo buildingModelInfo = target.GetComponent<BuildingModelInfo>();
        List<SubScene_Base> subScenes1 = null;
        if (buildingModelInfo != null)
        {
            subScenes1 = buildingModelInfo.GetFloorLoadScenes();
        }
        else
        {
            subScenes1 = target.GetComponentsInChildren<SubScene_Base>(true).ToList();
        }
        
        SortClearScenes(subScenes1);

        if (subScenes1.Count == 0)
        {
            SetIsUpdateDistance(true); ;//2
            Debug.LogWarning($"SubSceneShowManager.StartLoadScene Warning! 2 subScenes1.Count == 0 target:{target.name} path:{target.transform.GetPath()}  LoadedAll:{SubSceneManager.Instance.WattingForLoadedAll.Count} LoadedCurrent:{SubSceneManager.Instance.WattingForLoadedCurrent.Count}");
            return;
        }

        var subScenes = subScenes1;
        Debug.Log($"SubSceneShowManager.StartLoadScene[{loadScenesCount}] Start! target:{target.name} subScenes:{subScenes.Count} LoadedAll:{SubSceneManager.Instance.WattingForLoadedAll.Count} LoadedCurrent:{SubSceneManager.Instance.WattingForLoadedCurrent.Count}");

        if (IsEnableLoad)
        {
            LoadScenesOfTarget(subScenes, (p) =>
            {
                if (p.IsFinishOrTimeout())
                {
                    SetIsUpdateDistance(true); //3
                    Debug.Log($"SubSceneShowManager.StartLoadScene[{loadScenesCount}] Finished! target:{target.name} subScenes:{subScenes.Count} LoadedAll:{SubSceneManager.Instance.WattingForLoadedAll.Count} LoadedCurrent:{SubSceneManager.Instance.WattingForLoadedCurrent.Count}");
                }
            });
        }
        else
        {
            Debug.LogError("SubSceneShowManager.StartLoadScene[{loadScenesCount}] IsEnableLoad=false");
        }
    }

    private void SortClearScenes(List<SubScene_Base> ss)
    {
        int removeCount1 = ss.RemoveAll(i => i.IsLoaded || i.IsLoading);
        //ss.Sort((a, b) => a.vertexCount.CompareTo(b.vertexCount));
        //int removeCount2 = ss.RemoveAll(i => i.gameObject.activeInHierarchy == false);
        ss.Sort((a, b) => b.GetBoundsVolume().CompareTo(a.GetBoundsVolume()));
    }

    public int loadScenesCount=0;

    //public void LoadScenesOfTarget(GameObject target)
    //{
        
    //    if (target == null)
    //    {
    //        Debug.LogWarning($"SubSceneShowManager.StartLoadScene target == null");
    //        return;
    //    }
    //    loadScenesCount++;
    //    SubScene_Base[] subScenes = target.GetComponentsInChildren<SubScene_Base>(true);
    //    Debug.Log($"SubSceneShowManager.LoadScenes[{loadScenesCount}] target:{target.name} subScenes:{subScenes.Length} LoadedAll:{SubSceneManager.Instance.WattingForLoadedAll.Count} LoadedCurrent:{SubSceneManager.Instance.WattingForLoadedCurrent.Count}");

    //    LoadScenesOfTarget(subScenes,null);
    //}

    public float DistanceOfFPS = 0.3f;

    public void StartLoadSceneFPS(DepNode target)
    {
        StartLoadScene(DistanceOfFPS, target.gameObject);//???????????????0.3????????????
    }

    public void StartLoadSceneFPS(GameObject target)
    {
        StartLoadScene(DistanceOfFPS, target);//???????????????0.3????????????
    }

    /// <summary>
    /// ???????????????????????????
    /// </summary>
    public void EndLoadSceneFPS(GameObject target)
    {
        StartLoadScene(1, target);
    }

    public void StartLoadScene(float p, GameObject target)
    {
        //IsUpdateDistance = true;
        //Debug.Log($"StartLoadScene p:{p}");
        StartLoadScene(target);
        SetCamaraDistance(p);
    }

    public void SetCamaraDistance(float p)
    {
        var setting = SystemSettingHelper.sceneLoadSetting;

        this.DisOfVisible = setting.DisOfVisible* p;
        this.DisOfLoad = setting.DisOfLoad * p;
        this.DisOfHidden = setting.DisOfHidden * p;
        this.DisOfUnLoad = setting.DisOfUnLoad * p;
    }

    private void LoadSetting(Action onComplete = null)
    {
        
        //this.IsUpdateDistance = false;
        SystemSettingHelper.GetSystemSetting(() =>
        {
            var setting = SystemSettingHelper.sceneLoadSetting;
            Debug.Log($"SubSceneShowManager.LoadSetting setting:{setting}");
            this.DelayOfLoad = setting.DelayOfLoad;
            this.DelayOfUnLoad = setting.DelayOfUnLoad;
            this.WaittingInterval = setting.WaittingInterval;
            

            this.AngleOfVisible = setting.AngleOfVisible;
            this.AngleOfLoad = setting.AngleOfLoad;
            this.AngleOfHidden = setting.AngleOfHidden;
            this.AngleOfUnLoad = setting.AngleOfUnLoad;

            this.DisOfVisible = setting.DisOfVisible;
            this.DisOfLoad = setting.DisOfLoad;
            this.DisOfHidden = setting.DisOfHidden;
            this.DisOfUnLoad = setting.DisOfUnLoad;

            this.IsEnableLoad = setting.IsEnableLoad;
            this.IsEnableUnload = setting.IsEnableUnload;
            this.IsEnableHide = setting.IsEnableHide;
            this.IsEnableShow = setting.IsEnableShow;

            this.enabled = setting.IsEnable;
            //this.IsUpdateDistance = setting.IsEnable;

            if (setting.IsEnable)
            {
                onComplete();
            }
        });
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSetting(()=>
        {
            InitOnStart();
        });

        //SceneEvents.FloorFocusStartAction += SceneEvents_FloorFocusStartAction;
        //SceneEvents.FloorFocusCompleteAction += SceneEvents_FloorFocusCompleteAction;

        StartUpdateSubScenes();
    }

    private void SceneEvents_FloorFocusCompleteAction(FloorController obj)
    {
        //StartLoadScene(obj.gameObject);
    }

    private void SceneEvents_FloorFocusStartAction(FloorController obj)
    {
        StopLoadScene(obj.gameObject);
        StartLoadScene(obj.gameObject);//???????????????????????????????????????????????????????????????????????????????????????????????????????????????
    }

    private void InitOnStart()
    {
        InitCameras();

        if (IsAutoLoad)
        {
            LoadStartScenes();
        }

        if (AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.ShowNodeDistance = this.DisOfVisible;
            AreaTreeNodeShowManager.Instance.HideNodeDistance = this.DisOfHidden;
        }

        BuildingModelManager.Instance.ShowDetail();

#if UNITY_EDITOR
        SubSceneManager.Instance.CheckSceneIndex();
#endif
        //IdDictionary.InitInfos();

        StartCoroutine(PostScenesToLoad());
    }

    //public static void CheckSceneIndex()
    //{
    //    DateTime start = DateTime.Now;
    //    var alls=GameObject.FindObjectsOfType<SubScene_Base>(true);
    //    foreach(var s in alls){
    //        if(s.sceneArg.index<=0){
    //            BuildingModelInfo modelInfo = s.GetComponentInParent<BuildingModelInfo>();
    //            if (modelInfo != null)
    //            {
    //                Debug.LogError($"SubSceneShowManager.CheckSceneIndex index<=0 sName:{modelInfo.name}->{s.name} index:{s.sceneArg.index}");
    //            }
    //            else
    //            {
    //                Debug.LogError($"SubSceneShowManager.CheckSceneIndex index<=0 sName:NULL->{s.name} index:{s.sceneArg.index}");
    //            }

    //        }
    //    }
    //    Debug.Log($"CheckSceneIndex Time:{(DateTime.Now - start).ToString()}");
    //}

    public float DelayOfLoad = 2;

    public float DelayOfUnLoad = 5;

    public float AngleOfVisible = 60;
    public float AngleOfLoad = 75;
    public float AngleOfHidden = 80;
    public float AngleOfUnLoad = 90;

    public float DisOfVisible = 2500;//50
    public float DisOfLoad = 4900;//70
    public float DisOfHidden = 8100;//90
    public float DisOfUnLoad = 12100;//110

    public float MinDisSqrtToCam = 0;

    public float MaxDisSqrtToCam = 0;

    public float MinDisToCam = 0;

    public float MaxDisToCam = 0;

    public float AvgDisToCam = 0;

    public double TimeOfDis = 0;

    public double TimeOfLoad = 0;

    //public double TimeOfLoad1 = 0;
    //public double TimeOfLoad2 = 0;
    //public double TimeOfLoad3 = 0;
    //public double TimeOfLoad4 = 0;

    public double TimeOfUpdate = 0;

    public bool IsLogTime = false;

    public SubScene_Base MinDisScene;


    public DictList<SubScene_Base> visibleScenes = new DictList<SubScene_Base>();
    public DictList<SubScene_Base> loadScenes = new DictList<SubScene_Base>();
    public DictList<SubScene_Base> hiddenScenes = new DictList<SubScene_Base>();
    public DictList<SubScene_Base> unloadScenes = new DictList<SubScene_Base>();

    private void AddToWaitingScenes(IEnumerable<SubScene_Base> scenes, DictList<SubScene_Base> waittingList)
    {
        foreach (var scene in scenes)
        {
            if (scene == null) continue;
            if (scene.gameObject.activeInHierarchy == false) continue;
            if (scene.CanLoad())
            {
                // Debug.LogWarning($"[LoadUnloadScenes.Load] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
                //continue;
                waittingList.Add(scene);
            }
            //scene.LoadSceneAsync(null);
                
        }
    }

    private void AddToWaitingScenes_UnLoad(IEnumerable<SubScene_Base> scenes, DictList<SubScene_Base> waittingList)
    {
        if (IsEnableUnload)
            foreach (var scene in scenes)
            {
                if (scene.CanUnload())
                {
                    // Debug.LogWarning($"[LoadUnloadScenes.Unload] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
                    //scene.UnLoadSceneAsync();
                    //if (!waittingList.Contains(scene))
                        waittingList.Add(scene);
                }
               
            }
    }

    public void AddToWaitingScenes_ToLoad(IEnumerable<SubScene_Base> scenes)
    {
        if (IsEnableLoad)
            AddToWaitingScenes(scenes, WaitingScenes_ToLoad);
    }

    public bool isShowLog = false;

    private bool IsShowLog()
    {
#if UNITY_EDITOR
        return isShowLog;
#endif
        return false;
    }

    public void LoadScenesOfTarget(IEnumerable<SubScene_Base> scenes, Action<SceneLoadProgress> finished)
    {
        TimeTest.Start("??????????????????");
        //IsEnableHide = false;
        //IsEnableUnload = false;
        int count = 0;
        foreach (var scene in scenes)
        {
            count++;
            scene.LifeTimeSpane = DelayOfLoad + 1;
            if (WaitingScenes_ToUnLoad.Contains(scene))
            {
                if (IsShowLog())
                {
                    Debug.Log($"AddToWaitingScenes_ToLoadEx[{count}] WaitingScenes_ToUnLoad.Contains(scene) scene:{scene}");
                }
                WaitingScenes_ToUnLoad.Remove(scene);
            }

            if (visibleScenes.Contains(scene))
            {
                if (IsShowLog())
                {
                    Debug.Log($"AddToWaitingScenes_ToLoadEx[{count}] visibleScenes.Contains(scene) scene:{scene}");
                }
            }
            else
            {
                visibleScenes.Add(scene);
            }

            if (hiddenScenes.Contains(scene))
            {
                hiddenScenes.Remove(scene);
                if (IsShowLog())
                {
                    Debug.Log($"AddToWaitingScenes_ToLoadEx[{count}] hiddenScenes.Contains(scene) scene:{scene}");
                }
            }
            else
            {

            }
        }

        DoLoadScenes(scenes.ToList(), finished, "LoadScenesOfTarget");

        //if (IsEnableLoad)
        //    AddToWaitingScenes(scenes, WaitingScenes_ToLoad);
    }

    void LoadUnloadScenes()
    {
        //if (EnableLoadUnload == false) return;
        DateTime start = DateTime.Now;
        if (IsEnableShow)
        {
            //List<SubScene_GPUI> gpuiScens = new List<SubScene_GPUI>();
            foreach (var scene in visibleScenes.Items)
            {
                scene.ShowObjects();
            }

        }
        //TimeOfLoad1 = (DateTime.Now - start).TotalMilliseconds;

        if (IsEnableHide)
            foreach (var scene in hiddenScenes.Items)
            {
                scene.HideObjects();
            }
        //TimeOfLoad2 = (DateTime.Now - start).TotalMilliseconds;

        //if(IsEnableLoad)
        //    //var waittingScenes=loadScenes.Where(i=>i)
        //    foreach (var scene in loadScenes)
        //    {
        //        if (scene.IsLoading || scene.IsLoaded)
        //        {
        //            // Debug.LogWarning($"[LoadUnloadScenes.Load] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
        //            continue;
        //        }
        //        //scene.LoadSceneAsync(null);
        //        if(!WaitingScenes_ToLoad.Contains(scene))
        //            WaitingScenes_ToLoad.Add(scene);
        //    }
        //if (IsEnableLoad)
        //    AddToWaitingScenes(loadScenes, WaitingScenes_ToLoad);
        AddToWaitingScenes_ToLoad(loadScenes.Items);//WaitingScenes_ToLoad
        //TimeOfLoad3 = (DateTime.Now - start).TotalMilliseconds;

        //if (IsEnableUnload)
        //    foreach (var scene in unloadScenes)
        //    {
        //        if (scene.IsLoaded==false)
        //        {
        //            // Debug.LogWarning($"[LoadUnloadScenes.Unload] scene:{scene.GetSceneName()}, IsLoading:{scene.IsLoading} || IsLoaded:{scene.IsLoaded}");
        //            continue;
        //        }
        //        //scene.UnLoadSceneAsync();
        //        if (!WaitingScenes_ToUnLoad.Contains(scene))
        //            WaitingScenes_ToUnLoad.Add(scene);
        //    }
        //if (IsEnableUnload)
        //    AddToWaitingScenes(unloadScenes.Items, WaitingScenes_ToUnLoad);
        //TimeOfLoad4 = (DateTime.Now - start).TotalMilliseconds;
        AddToWaitingScenes_UnLoad(unloadScenes.Items, WaitingScenes_ToUnLoad);

        if (IsShowLog())
        {
            foreach(var item in unloadScenes.Items)
            {
                if (loadScenes.Contains(item))
                {
                    Debug.LogError($"LoadUnloadScenes Error1 loadScenes.Contains unloadScene scene:{item.name} path:{item.transform.GetPath()}");
                }
            }
            foreach (var item in loadScenes.Items)
            {
                if (unloadScenes.Contains(item))
                {
                    Debug.LogError($"LoadUnloadScenes Error2 unloadScenes.Contains loadScene scene:{item.name} path:{item.transform.GetPath()}");
                }
            }
        }

        TimeOfLoad = (DateTime.Now - start).TotalMilliseconds;
    }

    bool IsInFloorMode()
    {
        if(FactoryDepManager.currentDep!=null&&FactoryDepManager.currentDep is FloorController)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int GetActiveCamaraCount()
    {
        int cCount = 0;
        foreach (var cam in cameras)
        {
            if (cam == null) continue;
            if (cam.isActiveAndEnabled == false) continue;
            if (cam.gameObject.activeInHierarchy == false) continue;
            cCount++;
        }
        //if (cCount == 0)
        //{
        //    Debug.LogError("CalculateDistance cameras Count ==0 ");
        //}
        return cCount;
    }

    private List<Camera> GetActiveCamaras()
    {
        List<Camera> cs = new List<Camera>();
        foreach (var cam in cameras)
        {
            if (cam == null) continue;
            if (cam.isActiveAndEnabled == false) continue;
            if (cam.gameObject.activeInHierarchy == false) continue;
            cs.Add(cam);
        }
        //if (cCount == 0)
        //{
        //    Debug.LogError("CalculateDistance cameras Count ==0 ");
        //}
        return cs;
    }

    private List<Transform> GetActiveCamaraTransforms()
    {
        List<Transform> cs = new List<Transform>();
        foreach (var cam in cameras)
        {
            if (cam == null) continue;
            if (cam.isActiveAndEnabled == false) continue;
            if (cam.gameObject.activeInHierarchy == false) continue;
            cs.Add(cam.transform);
        }
        //if (cCount == 0)
        //{
        //    Debug.LogError("CalculateDistance cameras Count ==0 ");
        //}
        return cs;
    }

    public bool IsEnableAngleCheck = true;

    public bool IsEnableBoundsDis = true;

    private void GetSceneDisAndAngle(SubScene_Base scene,List<Transform> cams)
    {
        float disToCams = float.MaxValue;
        float angleOfCams = 0;
        foreach (Transform cam in cams)
        {
            float camAngle = 0;
            if (IsEnableAngleCheck)
            {
                camAngle = scene.GetCameraAngle(cam);
            }
            float dis = 0;
            Vector3 cP = cam.position;
            if (IsEnableBoundsDis)
            {
                dis = scene.bounds.SqrDistance(cP);
            }
            else
            {
                dis = Vector3.Distance(scene.transform.position, cP);
            }

            if (dis < disToCams)
            {
                disToCams = dis;
                angleOfCams = camAngle;
            }
        }
        scene.DisToCam = disToCams;
        scene.AngleToCam = angleOfCams;

        if (disToCams > MaxDisSqrtToCam)
        {
            MaxDisSqrtToCam = disToCams;
        }
        if (disToCams < MinDisSqrtToCam)
        {
            MinDisSqrtToCam = disToCams;
            MinDisScene = scene;
        }
        
    }

    private void AddSceneToList(SubScene_Base scene)
    {
        var disToCams = scene.DisToCam;
        var angleOfCams = scene.AngleToCam;
        //sumDis += disToCams;

        bool isLoad = false;
        bool isVisible = false;
        float disP = 1f;
        if(scene is SubScene_GPUI)
        {
            disP = 1.5f;
        }
        if (disToCams <= DisOfLoad * disP)
        {
            if (angleOfCams < AngleOfLoad)
            {
                //loadScenes.Add(scene);
                scene.SetLifeTime(0);

                if (scene.LifeTimeSpane > DelayOfLoad)
                {

                    if (scene.CanLoad())
                    {
                        if (scene.name.Contains("Node_2_42_346_13w_Renderers"))//MinHang>Building>Factory>????????????>????????????>????????????_Out0_SmallTree>RootNode>Node_0_2_6782_374w>Node_1_39_4485_231w>Node_2_42_346_13w>Node_2_42_346_13w_Renderers
                        {
                            Debug.Log($"[SubSceneManager.LoadScenes]AddSceneToList  Node_2_42_346_13w_Renderers name:{scene.name} disToCams:{disToCams}({DisOfLoad * disP}) angleOfCams:{angleOfCams}({AngleOfLoad}) LifeTimeSpane:{scene.LifeTimeSpane}({DelayOfLoad}) path:{scene.transform.GetPath()}");
                        }
                        loadScenes.Add(scene);
                    }
                    else
                    {

                    }
                    isLoad = true;
                }
            }
        }
        //else
        //{
        //    scene.ClearLifeTime();
        //}

        if (disToCams <= DisOfVisible * disP)
        {
            if (angleOfCams < AngleOfVisible)
            {
                visibleScenes.Add(scene);
                isVisible = true;
            }
        }

        if(isLoad==false && isVisible == false)
        {
            if (scene.CanUnload())
            {
                if (disToCams > DisOfUnLoad * disP)
                {
                    //unloadScenes.Add(scene);
                    scene.SetLifeTime(1);
                    if (scene.LifeTimeSpane > DelayOfUnLoad)
                    {
                        if (!IsInFloorMode()) unloadScenes.Add(scene);
                    }

                }
                else
                {
                    if (angleOfCams > AngleOfUnLoad && disToCams > DisOfLoad * disP)
                    {
                        //unloadScenes.Add(scene);
                        scene.SetLifeTime(1);
                        if (scene.LifeTimeSpane > DelayOfUnLoad)
                        {
                            if (!IsInFloorMode()) unloadScenes.Add(scene);
                        }
                    }
                }
            }

            if (disToCams > DisOfHidden || angleOfCams > DisOfHidden)
            {
                if (!IsInFloorMode()) hiddenScenes.Add(scene);
            }
        }

        //if (disToCams <= DisOfLoad)
        //{
        //    loadScenes.Add(scene);
        //}
    }

    private SubSceneBag GetSubScenes()
    {
        SubSceneBag subScenes = new SubSceneBag();
        //foreach (var scene in scenes_Out0_TreeNode_Hidden)
        //{
        //    if (scene == null) continue;
        //    if (scene.gameObject.activeInHierarchy == false)
        //    {
        //        //if (scene.name.Contains("Node_2_42_346_13w_Renderers"))//MinHang>Building>Factory>????????????>????????????>????????????_Out0_SmallTree>RootNode>Node_0_2_6782_374w>Node_1_39_4485_231w>Node_2_42_346_13w>Node_2_42_346_13w_Renderers
        //        //{
        //        //    Debug.Log($"[SubSceneManager.LoadScenes]GetSubScenes scenes_Out0_TreeNode_Hidden Node_2_42_346_13w_Renderers name:{scene.name} ({DisOfLoad}) LifeTimeSpane:{scene.LifeTimeSpane}({DelayOfLoad}) path:{scene.transform.GetPath()}");
        //        //}
        //        scene.ClearLifeTime();
        //        continue;
        //    }
        //    subScenes.Add(scene);
        //}

        AddSubScenes(scenes_Out0_TreeNode_Hidden, subScenes);

        if (IsEnableLOD)
        {
            AddSubScenes(scenes_LODs, subScenes);
        }

        //if (IsEnableLOD)
        {
            AddSubScenes(scenes_GPUI, subScenes);
        }

        if (IsEnableOut1)
        {
            AddSubScenes(scenes_Out1, subScenes);
        }
        if (IsEnableIn)
        {
            AddSubScenes(scenes_In, subScenes);
        }
        return subScenes;
    }

    private void AddSubScenes<T>(List<T> scensFrom, SubSceneBag subScenes) where T : SubScene_Base
    {
        foreach (var scene in scensFrom)
        {
            if (scene == null) continue;
            if (scene.gameObject.activeInHierarchy == true)
            {
                subScenes.Add(scene);
            }
            else if (scene.transform.parent != null && scene.transform.parent.gameObject.activeInHierarchy == true)
            {
                subScenes.Add(scene);
            }
            else
            {
                scene.ClearLifeTime();
            }
        }
    }

    void CalculateDistance(List<SubScene_Base> scenes)
    {
        DateTime start = DateTime.Now;

        List<Transform> cams = GetActiveCamaraTransforms();
        int cCount = cams.Count;
        if (cCount == 0)
        {
            Debug.LogError("CalculateDistance cameras Count ==0 ");
            return;
        }

        ClearSceneList();
        float sumDis = 0;

        for (int i = 0; i < scenes.Count; i++)
        {
            SubScene_Base scene = scenes[i];
            if (scene == null) continue;
            GetSceneDisAndAngle(scene, cams);
            sumDis += scene.DisToCam;
            SetSceneByDistance(scene);
        }

        AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
        MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
        MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

        TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
    }

    IEnumerator CalculateDistanceCoroutine(List<SubScene_Base> scenes)
    {
        DateTime start = DateTime.Now;

        List<Transform> cams = GetActiveCamaraTransforms();
        int cCount = cams.Count;
        if (cCount > 0)
        {
            ClearSceneList();
            float sumDis = 0;

            for (int i = 0; i < scenes.Count; i++)
            {
                SubScene_Base scene = scenes[i];
                if (scene == null) continue;
                GetSceneDisAndAngle(scene, cams);
                sumDis += scene.DisToCam;
                SetSceneByDistance(scene);
                if (i % UpdateCoroutineSize == 0)
                {
                    yield return null;
                }
            }

            AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
            MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
            MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

            TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
        }
        else
        {
            Debug.LogError("CalculateDistance cameras Count ==0 ");
        }
        yield return null;
    }

    private void ClearSceneList()
    {
        MaxDisSqrtToCam = 0;
        MinDisSqrtToCam = float.MaxValue;

        visibleScenes = new DictList<SubScene_Base>();
        hiddenScenes = new DictList<SubScene_Base>();
        loadScenes = new DictList<SubScene_Base>();
        unloadScenes = new DictList<SubScene_Base>();
    }

    private void SetSceneByDistance(SubScene_Base scene)
    {
        AddSceneToList(scene);
#if UNITY_EDITOR
        ChangeSceneBoundsColor(scene, scene.DisToCam);
#else
            SetSceneActive(scene, scene.DisToCam);
#endif
    }

    private void CalculateDistanceByJobs(List<SubScene_Base> scenes)
    {
        DateTime start = DateTime.Now;

        ClearSceneList();
        float sumDis = 0;

        //JobList<SceneDistanceJob> jobs = InitJobs(scenes);
        //if (jobs == null)
        //{
        //    return;
        //}

        JobList<SceneDistanceJob> jobs = new JobList<SceneDistanceJob>(JobListSize);
        var camTs = GetActiveCamaraTransforms();
        int cCount = camTs.Count;
        if (cCount == 0)
        {
            Debug.LogError("InitJobs cameras Count ==0 ");
            return;
        }

        //Debug.Log($"CalculateDistanceByJobs Camera Count:{cCount}");

        NativeArray<Vector3> camPosList0 = new NativeArray<Vector3>(camTs.Count, Allocator.Persistent);
        NativeArray<Vector3> camForwardList0 = new NativeArray<Vector3>(camTs.Count, Allocator.Persistent);
        for (int i = 0; i < camTs.Count; i++)
        {
            Transform cam = (Transform)camTs[i];
            camPosList0[i] = cam.position;
            camForwardList0[i] = cam.forward;
        }

        //for (int i = 0; i < scenes.Count; i++)
        //{
        //    NativeArray<Vector3> camPosList1 = new NativeArray<Vector3>(camPosList0, Allocator.TempJob);
        //    NativeArray<Vector3> camForwardList1 = new NativeArray<Vector3>(camForwardList0, Allocator.TempJob);
        //    SubScene_Base scene = scenes[i];
        //    SceneDistanceJob job = new SceneDistanceJob()
        //    {
        //        sceneId = i,
        //        camPosList = camPosList1,
        //        camForwardList = camForwardList1,
        //        sceneBounds = scene.bounds,
        //        scenePos = scene.transform.position
        //    };
        //    jobs.Add(job);
        //}
        //jobs.CompleteAllPage();


        NativeArray<Bounds> sceneBounds0 = new NativeArray<Bounds>(scenes.Count, Allocator.Persistent);
        NativeArray<Vector3> scenePos0 = new NativeArray<Vector3>(scenes.Count, Allocator.Persistent);
        NativeArray<float> disToCam0 = new NativeArray<float>(scenes.Count, Allocator.Persistent);
        NativeArray<float> angleToCam0 = new NativeArray<float>(scenes.Count, Allocator.Persistent);

        for (int i = 0; i < scenes.Count; i++)
        {
            SubScene_Base scene = scenes[i];
            sceneBounds0[i] = scene.bounds;
            scenePos0[i] = scene.transform.position;
            disToCam0[i] = 0;
            angleToCam0[i] = 0;
        }
        ScenesDistanceJob.Reset();
        ScenesDistanceJob job1 = new ScenesDistanceJob()
        {
            Count=scenes.Count,
            camPosList = camPosList0,
            camForwardList = camForwardList0,
            sceneBounds = sceneBounds0,
            scenePos = scenePos0,
            disList = disToCam0,
            angleList = angleToCam0
        };
        job1.Schedule().Complete();
        for (int i = 0; i < scenes.Count; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.DisToCam = job1.disList[i];
            scene.AngleToCam = job1.angleList[i];
            SetSceneByDistance(scene);
            sumDis += scene.DisToCam;
        }
        jobs.CompleteAllPage();
        jobs.Dispose();

        camPosList0.Dispose();
        camForwardList0.Dispose();
        sceneBounds0.Dispose();
        scenePos0.Dispose();
        disToCam0.Dispose();
        angleToCam0.Dispose();

        AvgDisToCam = Mathf.Sqrt(sumDis / scenes.Count);
        MinDisToCam = Mathf.Sqrt(MinDisSqrtToCam);
        MaxDisToCam = Mathf.Sqrt(MaxDisSqrtToCam);

        TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
    }

    public bool IsUseJob = false;

    public int JobListSize = 100;

    //private JobList<SceneDistanceJob> InitJobs(List<SubScene_Base> scenes)
    //{
    //    JobList<SceneDistanceJob> jobs = new JobList<SceneDistanceJob>(JobListSize);
    //    var camTs = GetActiveCamaraTransforms();
    //    int cCount = camTs.Count;
    //    if (cCount == 0)
    //    {
    //        Debug.LogError("InitJobs cameras Count ==0 ");
    //        return null;
    //    }

    //    NativeArray<Vector3> camPosList0 = new NativeArray<Vector3>(camTs.Count, Allocator.Persistent);
    //    NativeArray<Vector3> camForwardList0 = new NativeArray<Vector3>(camTs.Count, Allocator.Persistent);
    //    for (int i = 0; i < camTs.Count; i++)
    //    {
    //        Transform cam = (Transform)camTs[i];
    //        camPosList0[i] = cam.position;
    //        camForwardList0[i] = cam.forward;
    //    }

    //    for (int i = 0; i < scenes.Count; i++)
    //    {
    //        NativeArray<Vector3> camPosList1 = new NativeArray<Vector3>(camPosList0, Allocator.TempJob);
    //        NativeArray<Vector3> camForwardList1 = new NativeArray<Vector3>(camForwardList0, Allocator.TempJob);
    //        SubScene_Base scene = scenes[i];
    //        SceneDistanceJob job = new SceneDistanceJob()
    //        {
    //            sceneId = i,
    //            camPosList = camPosList1,
    //            camForwardList = camForwardList1,
    //            sceneBounds = scene.bounds,
    //            scenePos = scene.transform.position
    //        };
    //        jobs.Add(job);
    //    }
    //    return jobs;
    //}

    private void ChangeSceneBoundsColor(SubScene_Base scene, float disToCams)
    {
        if (scene.boundsGo == null)
        {
            // Debug.LogError("SubSceneShowManager.CalculateDistance scene.boundsGo==null :"+scene);
        }
        else
        {
            if (disToCams <= DisOfVisible)
            {
                scene.gameObject.SetActive(true);
                MeshRenderer mr = scene.boundsGo.GetComponent<MeshRenderer>();
                if (mr) mr.material.color = new Color(1, 0, 0, 0.2f);
            }
            else if (disToCams <= DisOfLoad)
            {
                scene.gameObject.SetActive(true);
                MeshRenderer mr = scene.boundsGo.GetComponent<MeshRenderer>();
                if (mr) mr.material.color = new Color(1, 0.5f, 0, 0.2f);
            }
            else if (disToCams <= DisOfHidden)
            {
                scene.gameObject.SetActive(true);
                MeshRenderer mr = scene.boundsGo.GetComponent<MeshRenderer>();
                if (mr) mr.material.color = new Color(1, 1, 0, 0.2f);
            }
            else if (disToCams <= DisOfUnLoad)
            {
                scene.gameObject.SetActive(true);
                MeshRenderer mr = scene.boundsGo.GetComponent<MeshRenderer>();
                if (mr) mr.material.color = new Color(1, 1, 1, 0.2f);
            }
        }
    }

    private void SetSceneActive(SubScene_Base scene, float disToCams)
    {
        if (disToCams <= DisOfVisible)
        {
            scene.gameObject.SetActive(true);
        }
        else if (disToCams <= DisOfLoad)
        {
            scene.gameObject.SetActive(true);
        }
        else if (disToCams <= DisOfHidden)
        {
            scene.gameObject.SetActive(true);
        }
        else if (disToCams <= DisOfUnLoad)
        {
            scene.gameObject.SetActive(true);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    //UpdateSubScenes();
    //}

    public void StartUpdateSubScenes()
    {
        StartCoroutine(UpdateSubScenes_Coroutine());
    }

    //private void UpdateSubScenes()
    //{
    //    DateTime start = DateTime.Now;
    //    RemoveWaitingScenes();
    //    //LoadWaitingScenes();
    //    UnLoadWaitingScenes();
    //    SubSceneBag subScenes = new SubSceneBag();
    //    if (IsUpdateDistance)
    //    {
    //        subScenes = GetSubScenes();
    //        if (IsUseJob)
    //        {
    //            CalculateDistanceByJobs(subScenes);
    //        }
    //        else
    //        {
    //            CalculateDistance(subScenes);
    //        }
    //        LoadUnloadScenes();
    //    }
    //    TimeOfUpdate = (DateTime.Now - start).TotalMilliseconds;
    //    if (IsLogTime)
    //    {
    //        //Debug.Log($"Update subScenes:{subScenes.Count} Update:{TimeOfUpdate:F2}=Dis:{TimeOfDis:F2} Load:{TimeOfLoad:F2}({TimeOfLoad1:F2}+{TimeOfLoad2:F2}+{TimeOfLoad3:F2}+{TimeOfLoad4:F2})");
    //        Debug.Log($"UpdateSubScenes subScenes:{subScenes.Count} Update:{TimeOfUpdate:F2}=Dis:{TimeOfDis:F2} Load:{TimeOfLoad:F2}");
    //    }
    //}

    private IEnumerator UpdateSubScenesCoroutine()
    {
        DateTime start = DateTime.Now;

        RemoveWaitingScenes();

        //LoadWaitingScenes();

        //UnLoadWaitingScenes();

        if (WaitingScenes_ToUnLoad.Count > 0)
        {
            var list = WaitingScenes_ToUnLoad.NewList();
            WaitingScenes_ToUnLoad.Clear();

            if (IsShowLog())
            {
                Debug.Log($"UnLoadWaitingScenes WaitingScenes_ToUnLoad :{list.Count}");
            }
            for (int i = 0; i < list.Count; i++)
            {
                var scene = list[0];
                //UnLoadingScene.UnLoadGos();
                yield return scene.UnLoadSceneAsync_Coroutine(true);
                scene.ShowBounds();
            }
        }

        SubSceneBag subScenes = new SubSceneBag();
        if (IsUpdateDistance)
        {
            subScenes = GetSubScenes();
            if (IsUseJob)
            {
                CalculateDistanceByJobs(subScenes);
            }
            else
            {
                yield return CalculateDistanceCoroutine(subScenes);
            }
            LoadUnloadScenes();
        }

        TimeOfUpdate = (DateTime.Now - start).TotalMilliseconds;
        if (IsLogTime)
        {
            //Debug.Log($"Update subScenes:{subScenes.Count} Update:{TimeOfUpdate:F2}=Dis:{TimeOfDis:F2} Load:{TimeOfLoad:F2}({TimeOfLoad1:F2}+{TimeOfLoad2:F2}+{TimeOfLoad3:F2}+{TimeOfLoad4:F2})");
            Debug.Log($"UpdateSubScenes subScenes:{subScenes.Count} Update:{TimeOfUpdate:F2}=Dis:{TimeOfDis:F2} Load:{TimeOfLoad:F2}");
        }
        yield return null;
    }

    public float UpdateInterval = 0.1f;
    public int UpdateCoroutineSize = 50;

    IEnumerator UpdateSubScenes_Coroutine()
    {
        while (true)
        {
            //UpdateSubScenes();
            yield return UpdateSubScenesCoroutine();
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    private void RemoveWaitingScenes()
    {
        if (WaitingScenes.Count > 0)
        {
            //Debug.Log("CheckWaittingScenes 1:"+ WaitingScenes.Count);
            for (int i = 0; i < WaitingScenes.Count; i++)
            {
                var scene = WaitingScenes[i];
                if (scene.IsLoaded == false)//?????????????????????????????????inactive?????????
                {
                    WaitingScenes.RemoveAt(i);
                    i--;
                }
                else if (scene.GetSceneObjectCount() > 0)
                {
                    WaitingScenes.RemoveAt(i);
                    i--;
                }
                else
                {
                    scene.CheckGetSceneObjects();
                }
            }
            //Debug.Log("CheckWaittingScenes 2:" + WaitingScenes.Count);
        }
    }

    private void LoadWaitingScenes()
    {
        if (WaitingScenes_ToLoad.Count > 0)
        {
            //if (LoadingScene == null || (LoadingScene != null && LoadingScene.IsLoaded == true) )
            //{
            //    LoadingScene = WaitingScenes_ToLoad[0];
            //    WaitingScenes_ToLoad.RemoveAt(0);
            //    LoadingScene.LoadSceneAsync((b, s) =>
            //    {
            //        if (b)
            //        {
            //            WaitingScenes.Add(s);
            //            LoadingScene = null;
            //        }
            //    });
            //}

            var ss = WaitingScenes_ToLoad.NewList();
            WaitingScenes_ToLoad.Clear();
            DoLoadScenes(ss, null, "LoadSceneAsync");

        }
    }

    //private void UnLoadWaitingScenes()
    //{
    //    if (WaitingScenes_ToUnLoad.Count > 0)
    //    {
    //        if (IsShowLog())
    //        {
    //            Debug.Log($"UnLoadWaitingScenes WaitingScenes_ToUnLoad :{WaitingScenes_ToUnLoad.Count}");
    //        }
            
    //        if (UnLoadingScene == null || (UnLoadingScene != null && UnLoadingScene.IsLoaded == false))
    //        {
    //            UnLoadingScene = WaitingScenes_ToUnLoad[0];
    //            WaitingScenes_ToUnLoad.RemoveAt(0);
    //            //UnLoadingScene.UnLoadGos();
    //            UnLoadingScene.UnLoadSceneAsync(true);
    //            UnLoadingScene.ShowBounds();
    //            UnLoadingScene = null;
    //        }
    //    }
    //}

    public float WaittingInterval = 1f;

    public IEnumerator PostScenesToLoad()
    {
        while (true)
        {
            if (WaitingScenes_ToLoad.Count > 0)
            {
                var ss0 = WaitingScenes_ToLoad.NewList();
                var ss = WaitingScenes_ToLoad.NewList();
                WaitingScenes_ToLoad.Clear();

                int count1 = ss.Count;
                var loadedScenes = ss.Select(i => i.IsLoaded || i.IsLoading).ToList();

                int removeCount1=ss.RemoveAll(i => i.IsLoaded || i.IsLoading);
                int removeCount2 = ss.RemoveAll(i => i.gameObject.activeInHierarchy==false);
                int removeCount3 = ss.RemoveAll(i => WaitingScenes_ToLoad_Posted.Contains(i));
                ss.Sort((a, b) => b.GetBoundsVolume().CompareTo(a.GetBoundsVolume()));
                int count2 = ss.Count;

                SubScene_Base inactiveScene = null;
                if (removeCount2 > 0)
                {
                    foreach(var scene in ss0)
                    {
                        if (scene.gameObject.activeInHierarchy == false)
                        {
                            inactiveScene = scene;
                        }
                    }
                }

                if(IsShowLog())
                {
                    if (count2 > 0)
                    {
                        Debug.Log($"SubSceneShowManager.PostScenesToLoad count1:{count1} count2:{count2} remove1:{removeCount1} remove2:{removeCount2} remove3:{removeCount3} inactiveScene:{TransformHelper.GetPathWithActive(inactiveScene)}");
                    }
                    else
                    {
                        Debug.LogWarning($"SubSceneShowManager.PostScenesToLoad count1:{count1} count2:{count2} remove1:{removeCount1} remove2:{removeCount2} remove3:{removeCount3} inactiveScene:{TransformHelper.GetPathWithActive(inactiveScene)}");
                    }
                }

                DoLoadScenes(ss, null,"PostScenesToLoad");
            }
            yield return new WaitForSeconds(WaittingInterval);
        }
    }

    private void DoLoadScenes(List<SubScene_Base> ss, Action<SceneLoadProgress> finished, string tag)
    {
        if (ss == null)
        {
            Debug.LogError($"DoLoadScenes ss == null");
            return;
        }
        if (ss.Count > 0)
        {
            //SubSceneManager.Instance.LoadScenesEx(ss.ToArray(), p =>
            //{
            //    if (p.scene != null)
            //        p.scene.HideObjects();
            //});
            WaitingScenes_ToLoad_Posted.AddRange(ss);
            LoadScenes(ss, p =>
            {
                if (p.scene != null)
                    p.scene.HideObjects();
                if (finished != null)
                {
                    finished(p);
                }
                WaitingScenes_ToLoad_Posted.Remove(p.scene);
            }, tag);
        }
        else
        {
            //Debug.LogError($"DoLoadScenes ss.Count==0");
        }
    }

    public bool IsEnableIn = true;

    public bool IsEnableOut1 = true;

    public bool IsEnableLOD = true;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MeshJobs;
using static PrefabInstanceBuilder;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuildingModelInfo : SubSceneCreater
{
    public static List<BuildingModelInfo> staticBuildings = null;

    public static List<BuildingModelInfo> GetStaticBuildings()
    {
        if (staticBuildings == null)
        {
            List<BuildingModelInfo> modelList = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
            List<BuildingController> buildingControls1 = GameObject.FindObjectsOfType<BuildingController>(true).ToList();
            foreach (var bc in buildingControls1)
            {
                BuildingModelInfo[] models = bc.GetComponentsInChildren<BuildingModelInfo>(true);
                foreach (var model in models)
                {
                    modelList.Remove(model);
                }
            }
            staticBuildings = modelList;
        }
        return staticBuildings;
    }

    public static void HideStaticBuildings()
    {
        GetStaticBuildings();
        foreach(var b in staticBuildings)
        {
            b.gameObject.SetActive(false);
        }
    }

    public static void ShowStaticBuildings()
    {
        GetStaticBuildings();
        foreach (var b in staticBuildings)
        {
            b.gameObject.SetActive(true);
        }
    }

    public override string ToString()
    {
        try
        {
            if (this.gameObject == null)
            {
                return "NULL1";
            }
            return this.name;
        }
        catch (Exception ex)
        {
            return "NULL2";
        }
    }

    [ContextMenu("MoveStaticToOut0")]
    public void MoveStaticToOut0()
    {
        if (OutPart0 == null)
        {
            Debug.LogError("MoveDoorToOut0 info.OutPart0 == null");
            return;
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if (child.name.Contains("Static"))
            {
                //OutRoots.Add(child.gameObject);
                child.transform.SetParent(OutPart0.transform);
                Debug.LogError("MoveDoorToOut0 True");
                return;
            }
        }
        Debug.LogError("MoveStaticToOut0");
    }

    [ContextMenu("MoveStaticToLODs")]
    public void MoveStaticToLODs()
    {
        if (LODPart == null)
        {
            Debug.LogError("MoveStaticToLODs info.OutPart0 == null");
            return;
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if (child.name.Contains("Static"))
            {
                //OutRoots.Add(child.gameObject);
                child.transform.SetParent(LODPart.transform);
                Debug.LogError("MoveStaticToLODs True");
                return;
            }
        }
        Debug.LogError("MoveStaticToLODs");
    }

    [ContextMenu("MoveSmallOut0ToIn")]
    public void MoveSmallOut0ToIn()
    {
        if (OutPart0 == null)
        {
            Debug.LogError("MoveDoorToOut0 info.OutPart0 == null");
            return;
        }

        BigSmallListInfo bsList= GetBigSmallInfo_Out0();

        for (int i = 0; i < bsList.smallModels.Count; i++)
        {
            var child = bsList.smallModels[i];
            child.transform.SetParent(InPart.transform);
        }
        Debug.LogError($"MoveSmallOut0ToIn BigSmallListInfo:{bsList}");
    }

    [ContextMenu("GetTreeCount")]
    public BuildingModelState GetState()
    {
        return new BuildingModelState(this);
    }

    [ContextMenu("TestGetState")]
    private void TestGetState()
    {
        Debug.Log(GetState());
    }

    [ContextMenu("TestGetTreeCount")]
    private void TestGetTreeCount()
    {
        Debug.Log(GetTreeCount());
    }

    [ContextMenu("GetTreeCount")]
    public int GetTreeCount()
    {
        //if (trees == null) return 0;
        int i = 0;
        foreach(var t in GetTrees())
        {
            if (t != null)
            {
                i++;
            }
        }
        return i;
    }

    public bool IsModelSceneFinish()
    {
        int pC = GetPartCount();
        if (pC > 0) return false;
        var tc = GetTreeCount();
        if (tc > 0) return false;
        if (SceneList == null || SceneList.sceneCount == 0) return false;
        return true;
    }

    public DoorsRootList CombineDoors()
    {
        this.Unpack();
        var rootList = DoorManager.UpdateDoors(this.gameObject, null);
        Debug.Log($"CombineDoors roots:{rootList.Count} name:{this.name}");
        if (rootList.Count == 1)
        {
            rootList[0].transform.SetParent(this.transform);
            return rootList;
        }
        if (rootList.Count ==2)
        {
            DoorsRoot inDoors = null;
            DoorsRoot outDoors = null;
            foreach (var root in rootList)
            {
                if(root.name.ToLower()=="doors")
                {
                    inDoors = root;
                }
                else
                {
                    outDoors = root;
                }
            }
            if(inDoors==null)
            {
                Debug.LogError($"CombineDoors roots:{rootList.Count} name:{this.name} inDoors==null");
                return rootList;
            }
            if (outDoors == null)
            {
                Debug.LogError($"CombineDoors roots:{rootList.Count} name:{this.name} outDoors==null");
                return rootList;
            }
            Debug.Log($"CombineDoors inDoors:{inDoors.name}({inDoors.transform.childCount}) outDoors:{outDoors.name}({outDoors.transform.childCount})");
            //for (int i=0;i<inDoors.transform.childCount;i++)
            //{
            //    var door = inDoors.transform.GetChild(i);
            //    door.SetParent(outDoors.transform);
            //}
            foreach(var door in inDoors.Doors)
            {
                door.transform.SetParent(outDoors.transform);
            }
            if (inDoors.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(inDoors.gameObject);
            }
            outDoors.transform.SetParent(this.transform);
        }
        Debug.LogError($"CombineDoors roots:{rootList.Count} name:{this.name} ");
        return rootList;
    }

    internal SubSceneBag GetSubScenes_Out0B()
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_Out0_BigTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes_Out0B Model:{this.name} trees:{trees.Count} bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes_Out0S()
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_Out0_SmallTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes_Out0S Model:{this.name} trees:{trees.Count} bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes_Out1()
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_OutTree1"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out1>(true);
                    bag.AddRange(allScenes);
                }
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes_Out1 Model:{this.name} trees:{trees.Count} bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes_In()
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_InTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_In>(true);
                    bag.AddRange(allScenes);
                }
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes_In Model:{this.name} trees:{trees.Count} bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes_LOD()
    {
        SubSceneBag bag = new SubSceneBag();
        if (LODPart != null)
        {
            {
                var allScenes = LODPart.GetComponentsInChildren<SubScene_LODs>(true);
                bag.AddRange(allScenes);
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes_LOD Model:{this.name}  bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes()
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_Out0_BigTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }
            if (tree.name.Contains("_Out0_SmallTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }
            if (tree.name.Contains("_InTree"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_In>(true);
                    bag.AddRange(allScenes);
                }
            }
            if (tree.name.Contains("_OutTree1"))
            {
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out1>(true);
                    bag.AddRange(allScenes);
                }
            }
        }

        if (LODPart != null)
        {
            {
                var allScenes = LODPart.GetComponentsInChildren<SubScene_LODs>(true);
                bag.AddRange(allScenes);
            }
        }
        Debug.Log($"BuildingModelInfo.GetSubScenes1 Model:{this.name} trees:{trees.Count} bag:{bag.Count}");
        return bag;
    }

    internal SubSceneBag GetSubScenes(BuildingSceneLoadItem floorLoadSetting)
    {
        SubSceneBag bag = new SubSceneBag();
        var trees = GetTreeList();
        foreach(var tree in trees)
        {
            if (tree.name.Contains("_Out0_BigTree"))
            {
                if (floorLoadSetting.Out0RenderersB)
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }
            if (tree.name.Contains("_Out0_SmallTree"))
            {
                if (floorLoadSetting.Out0RenderersS)
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out0>(true);
                    bag.AddRange(allScenes);
                }
            }

            if (floorLoadSetting.InRenderers)
            {
                if (tree.name.Contains("_InTree"))
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_In>(true);
                    bag.AddRange(allScenes);
                }
                if (tree.name.Contains("_In_BigTree"))
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_In>(true);
                    bag.AddRange(allScenes);
                }
            }

            if (tree.name.Contains("_OutTree1"))
            {
                if (floorLoadSetting.Out1Renderers)
                {
                    var allScenes = tree.GetComponentsInChildren<SubScene_Out1>(true);
                    bag.AddRange(allScenes);
                }
            }
            if (tree.name.Contains("_GPUI_SmallTree"))
            {
                var allScenes = tree.GetComponentsInChildren<SubScene_GPUI>(true);
                bag.AddRange(allScenes);
            }
            if (tree.name.Contains("_GPUI_BigTree"))
            {
                var allScenes = tree.GetComponentsInChildren<SubScene_GPUI>(true);
                bag.AddRange(allScenes);
            }
        }

        if (LODPart != null)
        {
            if (floorLoadSetting.LODs)
            {
                var allScenes = LODPart.GetComponentsInChildren<SubScene_LODs>(true);
                bag.AddRange(allScenes);
            }
        }

        foreach(var scene in bag)
        {
            scene.gameObject.SetActive(true);
        }

        //if (GPUIPart != null)
        //{
        //    var allScenes = GPUIPart.GetComponentsInChildren<SubScene_Base>(true);
        //    bag.AddRange(allScenes);

        //    Debug.Log($"BuildingModelInfo.GetSubScenes1 GPUIPart != null scenes:{allScenes.Length}");
        //}

        //Debug.Log($"BuildingModelInfo.GetSubScenes1 Model:{this.name} trees:{trees.Count} scenes:{bag.Count} setting:{floorLoadSetting}");
        return bag;
    }

    public Transform[] GetChildren()
    {
        List<Transform> ts = new List<Transform>();
        for(int i=0;i<transform.childCount;i++)
        {
            ts.Add(transform.GetChild(i));
        }
        return ts.ToArray();
    }

    public void DeleteOthersOfDoor()
    {
        Unpack();
        var rootList = DoorManager.UpdateDoors(this.gameObject,null);
        rootList.SetParent(null);

        var children = this.GetChildren();
        foreach(var child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        rootList.SetParent(this.transform);
    }

    public int GetPartCount()
    {
        int pC = 0;
        if (InPart != null)
        {
            pC++;
        }
        if (OutPart0 != null)
        {
            pC++;
        }
        if (OutPart1 != null)
        {
            pC++;
        }
        if (LODPart != null)
        {
            pC++;
        }
        return pC;
    }

    public void FindInDoors()
    {
        BuildingModelTool tool=this.GetComponent<BuildingModelTool>();
        if(tool==null){
            tool=this.gameObject.AddComponent<BuildingModelTool>();
        }
        tool.FindDoorsInBounds95();
    }

    // public int GetSceneCount()
    // {
    //     if (SceneList == null)
    //     {
    //         return 0;
    //     }
    //     else
    //     {
    //         return SceneList.sceneCount;
    //     }
    // }

    public string GetInfoName()
    {
        // //int m = 0;
        
        // var tc = GetTreeCount();
        // //string infoName = "Mod";

        // int pC = GetPartCount();

        // string p = "";
        // if (pC>0)
        // {
        //     p = $"|P{pC}";
        // }
        // if (SceneList != null)
        // {
            
        //     if (tc > 0)
        //     {
        //         //return "Model(S)(T)";
        //         if (SceneList.sceneCount > 0)
        //         {
        //             return $"Mod|S{SceneList.sceneCount}|T{tc}{p}";
        //         }
        //         else
        //         {
        //             return $"Mod|T{tc}{p}";
        //         }
        //     }
        //     else
        //     {
        //         if (SceneList.sceneCount > 0)
        //         {
        //             return $"Mod|S{SceneList.sceneCount}{p}";
        //         }
        //     }
        // }
        // else
        // {
        //     if (tc > 0)
        //     {
        //         //return "Model(T)";
        //         return $"Mod|T{tc}{p}";
        //     }
        // }
        
        return $"Mod{GetInfoText()}";
    }

    public DoorsRootList doorRoots;

    public void UpdateDoors()
    {
        DoorManager.Instance.LocalTarget = this.gameObject;
        doorRoots = DoorManager.Instance.UpdateDoors();
        //doorRoots=CombineDoors();
    }

    public string GetInfoText()
    {
        //int m = 0;
        
        var tc = GetTreeCount();
        //string infoName = "Mod";

        int pC = GetPartCount();

        string p = "";
        if (pC>0)
        {
            p = $"|P{pC}";
        }
        if (SceneList != null)
        {
            
            if (tc > 0)
            {
                //return "Model(S)(T)";
                if (SceneList.sceneCount > 0)
                {
                    return $"{p}|S{SceneList.sceneCount}|T{tc}";
                }
                else
                {
                    return $"{p}|T{tc}";
                }
            }
            else
            {
                if (SceneList.sceneCount > 0)
                {
                    return $"{p}|S{SceneList.sceneCount}";
                }
            }
        }
        else
        {
            if (tc > 0)
            {
                //return "Model(T)";
                return $"{p}|T{tc}";
            }
        }
        
        return p;
    }

    private List<MeshRendererInfo> MeshRenderers = null;

    public List<MeshRendererInfo> GetMeshRenderers(bool isUpdate)
    {
        if (MeshRenderers==null || isUpdate)
        {
            MeshRendererInfo.InitRenderers(this.gameObject);
            MeshRenderers = new List<MeshRendererInfo>();
            if (InPart == null && OutPart0 == null && OutPart1 == null)
            {
                MeshRenderers.AddRange(this.GetComponentsInChildren<MeshRendererInfo>(true));
            }
            else
            {
                if (InPart)
                    MeshRenderers.AddRange(InPart.GetComponentsInChildren<MeshRendererInfo>(true));
                if (OutPart0)
                    MeshRenderers.AddRange(OutPart0.GetComponentsInChildren<MeshRendererInfo>(true));
                if (OutPart1)
                    MeshRenderers.AddRange(OutPart1.GetComponentsInChildren<MeshRendererInfo>(true));
            }

            MeshRenderers = MeshRenderers.Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
        }
        return MeshRenderers;
    }

    public GameObject InPart;

    public GameObject OutPart0;
    public GameObject OutPart1;

    public GameObject LODPart;

    public GameObject GPUIPart;

    //public MeshVertexInfo InVertexInfo;

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;
    public float LODVertexCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;
    public int LODRendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    private float _inBigVertextCount = 0;
    private float _inSmallVertextCount = 0;
    private int _inBigRendererCount = 0;
    private int _inSmallRendererCount = 0;

    public float InBigVertextCount
    {
        get
        {
            return _inBigVertextCount;
        }
    }
    public float InSmallVertextCount
    {
        get
        {
            return _inSmallVertextCount;
        }
    }
    public int InBigRendererCount
    {
        get
        {
            return _inBigRendererCount;
        }
    }
    public int InSmallRendererCount
    {
        get
        {
            return _inSmallRendererCount;
        }
    }

    public float Out0BigVertextCount = 0;
    public float Out0SmallVertextCount = 0;
    public int Out0BigRendererCount = 0;
    public int Out0SmallRendererCount = 0;

    [ContextMenu("TestLoadAndSwitchToRenderers")]
    public void TestLoadAndSwitchToRenderers()
    {
       LoadAndSwitchToRenderers((p)=>{
           Debug.LogError($"TestLoadAndSwitchToRenderers progress:{p.progress} isFinished:{p.isAllFinished}");
       });
    }

    public void LoadAndSwitchToRenderers(Action<SceneLoadProgress> loadSceneProgressChanged)
    {
        DateTime start=DateTime.Now;
        UpdateTrees();
        var nodes=new List<AreaTreeNode>();
        foreach (var tree in GetTrees())
        {
            nodes.AddRange(tree.TreeLeafs);
        }
        var scenes=new SubSceneBag();
        for(int i=0;i<nodes.Count;i++){
            var node=nodes[i];
            var rendererScene=node.GetCombinedLinkedScene();
            if(rendererScene!=null){
                scenes.Add(rendererScene);
            }
        }
        SubSceneShowManager.Instance.LoadScenes(scenes,(p)=>{
            Debug.LogError($"SwitchToCombined2 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");

            if(p.isAllFinished){
                for(int i=0;i<nodes.Count;i++){
                    var node=nodes[i];
                    node.SwitchToRenderers();
                }
            }
            if(loadSceneProgressChanged!=null){
                loadSceneProgressChanged(p);
            }

            Debug.LogError($"SwitchToCombined3 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");
        }, "LoadAndSwitchToRenderers");
        Debug.LogError($"SwitchToCombined1 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");
    }

    public string PrefabPath = null;

#if UNITY_EDITOR
    public void EditorSavePrefab()
    {
        BuildingModelState state = this.GetState();
        if(state.CanLoadScenes)
        {
            PrefabPath = SubSceneManager.Instance.EditorSavePrefabPath(this.gameObject);
        }
        else
        {
            Debug.LogError("BuildingModelInfo.EditorSavePrefab CanLoadScenes==false :"+this.name);
        }
        
    }

    public void EditorLoadPrefab()
    {
        if (PrefabPath != null)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(go,this.transform.parent) as GameObject;
            if (prefabInstance == null)
            {
                Debug.LogError("EditorLoadPrefab prefabInstance == null:"+this.name);
                return;
            }
            BuildingModelInfo newInfo = prefabInstance.GetComponent<BuildingModelInfo>();
            newInfo.PrefabPath = this.PrefabPath;

            prefabInstance.name = this.name;
            GameObject.DestroyImmediate(this.gameObject);

            EditorHelper.SelectObject(prefabInstance);
        }
    }

    public void ResetModel(Action<ProgressArg> progressChanged)
    {
        this.EditorLoadNodeScenes(progressChanged);
        this.DestroyScenes();
        this.ClearTrees();
    }

    public void ResaveScenes(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;
        ProgressArg p1 = new ProgressArg("ResaveScenes", 0, 2);
        if (progressChanged != null)
        {
            progressChanged(p1);
        }
        this.EditorLoadNodeScenes(sub=>
        {
            p1.AddSubProgress(sub);
            if (progressChanged != null)
            {
                progressChanged(p1);
            }
        });
        this.DestroyScenes();
        this.ClearTrees();

        //UpdateDoors();
        doorRoots = CombineDoors();

        ProgressArg p2 = new ProgressArg("ResaveScenes", 1, 2);
        OneKey_TreeNodeScene(sub=>
        {
            p2.AddSubProgress(sub);
            if (progressChanged != null)
            {
                progressChanged(p2);
            }
        });
        Debug.LogWarning($"ResaveScenes \t{(DateTime.Now - start).ToString()}");
    }


#endif
    internal List<SubScene_Base> GetFloorLoadScenes()
    {
        //In_BigTree,_In_SmallTree,_Out0_BigTree,_Out0_SmallTree,_OutTree1
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        var trees = GetTrees();
        foreach (var tree in trees)
        {
            if (tree.name.Contains("_In_SmallTree"))
            {
                continue;
            }
            var treeScenes = tree.GetComponentsInChildren<SubScene_Base>(true);
            scenes.AddRange(treeScenes);
        }
        if (LODPart)
        {
            //var treeScenes = LODPart.GetComponentsInChildren<SubScene_Base>(true);
            //scenes.AddRange(treeScenes);//LOD的加载由LODManager管理

            var lodScene = LODPart.GetComponent<SubScene_Base>();//加载的是LODs的这个子场景
            if (lodScene != null && lodScene.IsLoaded == false)
            {
                scenes.Add(lodScene);
            }
            Debug.LogError($"GetFloorLoadScenes({this.name}) lodScene:{lodScene}");
        }
        Debug.LogError($"GetFloorLoadScenes({this.name}) scenes:{scenes.Count}");
        return scenes;
    }

    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        DateTime start=DateTime.Now;
         Debug.Log("BuildingModelInfo.SwitchToCombined");
        UpdateTrees();
        foreach(var t in GetTrees())
        {
            t.SwitchToCombined();
        }

        Debug.LogWarning($"SwitchToCombined \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("SwitchToRenderers")]
    public void SwitchToRenderers()
    {
        DateTime start=DateTime.Now;
         Debug.Log("BuildingModelInfo.SwitchToRenderers");
        UpdateTrees();
        foreach(var t in GetTrees())
        {
            t.SwitchToRenderers();
        }

        Debug.LogWarning($"SwitchToRenderers \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        UpdateTrees();
        IdDictionary.InitInfos();
        foreach (var t in GetTrees())
        {
            if (t.IsSceneLoaded() == false)
            {
                Debug.LogError($"ClearTrees IsSceneLoaded() == false tree:{t}");
                continue;
            }
            t.RecoverParent(this.transform);
            EditorHelper.UnpackPrefab(t.gameObject);
            GameObject.DestroyImmediate(t.gameObject);
        }
        //this.ShowRenderers();
        HideDetail();
    }

    public void DestroyBoundBox()
    {
        UpdateTrees();
        foreach (var t in GetTrees())
        {
            t.DestroyBoundBox();
        }
    }

    [ContextMenu("MoveRenderers")]
    public void MoveRenderers()
    {
        UpdateTrees();
        foreach (var t in GetTrees())
        {
            t.MoveRenderers();
        }
    }

    //[ContextMenu("InitRenderers")]
    //public void InitRenderers()
    //{
    //    UpdateTrees();
    //    foreach (var t in GetTrees())
    //    {
    //        t.InitRenderers();
    //    }
    //}

    [ContextMenu("RecoverParent")]
    public void RecoverParent()
    {
        IdDictionary.InitInfos();
        foreach (var t in GetTrees())
        {
            t.RecoverParent(this.transform);
        }
    }


    [ContextMenu("SaveTreeRendersId")]
    public override void SaveTreeRendersId()
    {
        Debug.Log("BuildingModelInfo.SaveTreeRendersId");
        UpdateTrees();
        foreach(var t in GetTrees())
        {
            t.SaveRenderersId();
        }
    }

    [ContextMenu("LoadTreeRenderers")]
    public void LoadTreeRenderers()
    {
        UpdateTrees();
        //IdDictionary.InitRenderers(t.gameObject);
        foreach (var t in GetTrees())
        {
            t.LoadRenderers();
        }
    }

    public void LoadTreeRenderers(List<SubScene_Base> scenes)
    {
        //UpdateTrees();
        ////IdDictionary.InitRenderers(t.gameObject);
        //foreach (var t in GetTrees())
        //{
        //    t.LoadRenderers();
        //}

        //foreach(var scene in scenes)
        //{
        //    var ts = scene.GetTrees();
        //    foreach (var t in ts)
        //    {
        //        t.LoadRenderers();
        //    }
        //}
    }

    //[ContextMenu("GetTrees")]
    public void UpdateTrees()
    {
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
    }

    //[ContextMenu("* CreateTrees")]
    public ModelAreaTree[] CreateTreesInner(Action<ProgressArg> progressChanged)
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        //ShowRenderers();
        return CreateTreesCore(progressChanged);
    }

    public ModelAreaTree[] CreateTreesInnerEx(bool isOut0BS, Action<ProgressArg> progressChanged)
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        //ShowRenderers();

        if (this.OutPart0 == null)
        {
            InitInOut(false);
        }

        ModelAreaTree[] ts = null;
        if (this.OutPart1 == null && this.InPart == null)
        {
            ts= CreateTrees_BigSmall_Core_Out0(progressChanged);
        }
        else
        {
            if (isOut0BS == false)
            {
                ts= CreateTreesCore(progressChanged);
            }
            else
            {
                ts= CreateTreesCoreBS(progressChanged);
            }
        }
        trees = ts;

        return ts;
    }
    
    //public ModelAreaTree[] CreateTreesInnerBS()
    //{
    //    ClearTrees();

    //    UpackPrefab_One(this.gameObject);

    //    ShowRenderers();
    //    return CreateTreesCoreBS(null);
    //}

    public ModelAreaTree[] CreateTreesCore(Action<ProgressArg> progressChanged)
    {
        if (OutPart0 == null)
        {
            Debug.LogWarning("CreateTreesCore OutPart0 == null");
        }

        if (InPart == null && OutPart0 == null && OutPart1 == null)
        {
            InitInOut(false);
        }


        List<ModelAreaTree> ts = new List<ModelAreaTree>();

        ProgressArg p1 = new ProgressArg("CreateTreesCore", 0, 3,"InTree");
        DisplayProgressBar("InTree", progressChanged, p1);

        var tree1 = CreateTree(InPart, "InTree", p =>
        {
            p1.AddSubProgress(p);
            DisplayProgressBar("InTree", progressChanged, p1);
        });
        tree1.HideLeafNodes();
        if (tree1 != null)
        {
            ts.Add(tree1);
        }

        ProgressArg p2 = new ProgressArg("CreateTreesCore",1, 3, "OutTree0");
        DisplayProgressBar("OutTree0", progressChanged, p2);

        var tree2 = CreateTree(OutPart0, "OutTree0", p =>
        {
            p2.AddSubProgress(p);
            DisplayProgressBar("OutTree0", progressChanged, p2);
        });

        if (tree2 != null)
        {
            ts.Add(tree2);
        }

        ProgressArg p3 = new ProgressArg("CreateTreesCore", 2, 3, "OutTree1");
        DisplayProgressBar("OutTree1", progressChanged, p3);
        var tree3 = CreateTree(OutPart1, "OutTree1", p =>
        {
            p3.AddSubProgress(p);
            DisplayProgressBar("OutTree1", progressChanged, p3);
        });
        tree3.HideLeafNodes();
        if (tree3 != null)
        {
            ts.Add(tree3);
        }
        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("CreateTreesCore",3, 3, "End"));
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("OutTree1", new ProgressArg("CreateTreesCore", 3, 3, "End"));
            ProgressBarHelper.ClearProgressBar();
        }

        return ts.ToArray();
    }

    private static void DisplayProgressBar(string title,Action<ProgressArg> progressChanged, ProgressArg p1)
    {
        if (progressChanged != null)
        {
            progressChanged(p1);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar(title, p1);
        }
    }

    public ModelAreaTree[] CreateTreesCoreBS(Action<ProgressArg> progressChanged)
    {
        if (InPart == null && OutPart0 == null && OutPart1 == null)
        {
            InitInOut(false);
        }

        List<ModelAreaTree> ts = new List<ModelAreaTree>();

        ProgressArg p1 = new ProgressArg("CreateTreesCoreBS", 0, 3, "InTree");
        DisplayProgressBar("InTree", progressChanged, p1);

        //var tree1 = CreateTree(InPart, "InTree", p => {
        //    p1.AddSubProgress(p);
        //    DisplayProgressBar("InTree", progressChanged, p1);
        //});
        //tree1.HideLeafNodes();
        //if (tree1 != null)
        //{
        //    ts.Add(tree1);
        //}
        ////var tree2 = CreateTree(OutPart0, "OutTree0");
        ////trees[1] = tree2;
        var tbsIn = CreateTrees_BigSmall_Core_In(p =>
        {
            p1.AddSubProgress(p);
            DisplayProgressBar("InTree", progressChanged, p1);
        });
        if (tbsIn != null)
            foreach (var t in tbsIn)
            {
                if (t != null)
                {
                    ts.Add(t);
                }
            }

        ProgressArg p2 = new ProgressArg("CreateTreesCoreBS", 1, 3, "OutTree0");
        DisplayProgressBar("OutTree0", progressChanged, p2);

        var tbs = CreateTrees_BigSmall_Core_Out0(p =>
         {
             p2.AddSubProgress(p);
             DisplayProgressBar("OutTree0", progressChanged, p2);
         });
        if(tbs!=null)
            foreach (var t in tbs)
            {
                if (t != null)
                {
                    ts.Add(t);
                }
            }

        ProgressArg p3 = new ProgressArg("CreateTreesCoreBS", 2, 3, "OutTree1");
        DisplayProgressBar("OutTree1", progressChanged, p3);
        var tree3 = CreateTree(OutPart1, "OutTree1", p => {
            p3.AddSubProgress(p);
            DisplayProgressBar("OutTree1", progressChanged, p3);
        });
        tree3.HideLeafNodes();
        if (tree3 != null)
        {
            ts.Add(tree3);
        }


        var tbsGPUI = CreateTrees_BigSmall_Core_GPUI(p =>
        {
            p1.AddSubProgress(p);
            DisplayProgressBar("GPUITree", progressChanged, p1);
        });
        if (tbsGPUI != null)
            foreach (var t in tbsGPUI)
            {
                if (t != null)
                {
                    ts.Add(t);
                }
            }

        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("CreateTreesCoreBS", 3, 3, "End"));
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", new ProgressArg("CreateTreesCoreBS", 3, 3, "End"));
            ProgressBarHelper.ClearProgressBar();
        }
        
        return ts.ToArray();
    }

    //[ContextMenu("* InitInOut")]
    //public void InitInOut()
    //{
    //    InitInOut(true);
    //}

    [ContextMenu("ClearInOut")]
    public void ClearInOut()
    {
        //ClearInOut(true);

        ClearPart(InPart);
        ClearPart(OutPart0);
        ClearPart(OutPart1);

        BuildingModelTool tool=this.GetComponent<BuildingModelTool>();
        if(tool!=null){
            GameObject.DestroyImmediate(tool);
        }
    }

    [ContextMenu("CreateTestWalls")]
    public void CreateTestWalls()
    {
        GetInOutParts();

        Vector3 size = this.gameObject.GetSize();

        GameObject wall1 = CreateWall("1");
        wall1.transform.localScale = new Vector3(0.1f, 1f, 1f);
        Vector3 size1 = wall1.gameObject.GetSize();
        wall1.transform.Translate(new Vector3(size.x / 2, 0, 0));
        wall1.transform.SetParent(OutPart0.transform);

        GameObject wall2 = CreateWall("2");
        wall2.transform.localScale = new Vector3(0.1f, 1f, 1f);
        Vector3 size2 = wall2.gameObject.GetSize();
        wall2.transform.Translate(new Vector3(-size.x/2, 0, 0));
        wall2.transform.SetParent(OutPart0.transform);

        GameObject wall3 = CreateWall("3");
        wall3.transform.localScale = new Vector3(1f, 1f, 0.1f);
        Vector3 size3 = wall3.gameObject.GetSize();
        wall3.transform.Translate(new Vector3(0, 0, size.z / 2));
        wall3.transform.SetParent(OutPart0.transform);

        GameObject wall4 = CreateWall("4");
        wall4.transform.localScale = new Vector3(1f, 1f, 0.1f);
        Vector3 size4 = wall4.gameObject.GetSize();
        wall4.transform.Translate(new Vector3(0, 0, -size.z / 2));
        wall4.transform.SetParent(OutPart0.transform);

        Debug.Log($"CreateTestWalls ");
    }

    private GameObject CreateWall(string tag)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall_"+tag;
        wall.transform.SetParent(this.transform);
        wall.transform.localPosition = Vector3.zero;
        wall.transform.localScale = Vector3.one;
        wall.transform.localRotation = Quaternion.identity;

       
        return wall;
    }

    public void InitInOut(bool isShowOut0Log=false, bool isDefaultOut0 = true)
    {
        DateTime start = DateTime.Now;
        //Debug.Log("InitInOut:"+this.name);

        if (this.transform.childCount == 0)
        {
            Debug.LogWarning("InitInOut this.transform.childCount == 0");
            return;
        }
        UpackPrefab_One(this.gameObject);

        GetInOutParts(isDefaultOut0);

        GetInOutVertextCount(isShowOut0Log);

        HideDetail();

        //var manager=GameObject.FindObjectOfType<>

        GetBigSmallInfo_Out0();
        GetBigSmallInfo_In();

        RendererId.InitIds(this.gameObject);

         //Debug.Log($"InitInOut time:{(DateTime.Now - start)}");
    }

    [ContextMenu("CenterPivot")]
    public void CenterPivot()
    {
        var start=DateTime.Now;
        MeshHelper.CenterPivot(InPart);
        MeshHelper.CenterPivot(OutPart0);
        MeshHelper.CenterPivot(OutPart1);
        MeshHelper.CenterPivot(this.gameObject);
        Debug.LogError($"BuildingModelInfo.CenterPivot Time:{(DateTime.Now - start).ToString()}");
    }


    // private static AcRTAlignJobSetting JobSetting;

    private BigSmallListInfo GetBigSmallInfo_Out0()
    {
        if(OutPart0==null){
            Debug.LogError($"GetBigSmallInfo_Out0 OutPart0==null model:{this.name}");
            return null;
        }
        // JobSetting =GameObject.FindObjectOfType<AcRTAlignJobSetting>(true);
        // if (JobSetting == null)
        // {
        //     Debug.LogError("GetSmallBigInfo JobSetting == null");
        //     return;
        // }
        RendererManager.Instance.SetDetailRenderers(OutPart0.GetComponentsInChildren<MeshRenderer>(true));
        var info_out=new BigSmallListInfo(OutPart0, isIgnoreGPU);
        Out0BigRendererCount = info_out.bigModels.Count;
        Out0BigVertextCount = info_out.sumVertex_Big;
        Out0SmallRendererCount = info_out.smallModels.Count;
        Out0SmallVertextCount = info_out.sumVertex_Small;
        return info_out;
    }

    private BigSmallListInfo GetBigSmallInfo_GPUI()
    {
        if (GPUIPart == null)
        {
            //Debug.LogError($"GetBigSmallInfo_GPUI OutPart0==null model:{this.name}");
            return null;
        }
        // JobSetting =GameObject.FindObjectOfType<AcRTAlignJobSetting>(true);
        // if (JobSetting == null)
        // {
        //     Debug.LogError("GetSmallBigInfo JobSetting == null");
        //     return;
        // }
        RendererManager.Instance.SetDetailRenderers(GPUIPart.GetComponentsInChildren<MeshRenderer>(true));
        var info_out = new BigSmallListInfo(GPUIPart, false);
        Out0BigRendererCount = info_out.bigModels.Count;
        Out0BigVertextCount = info_out.sumVertex_Big;
        Out0SmallRendererCount = info_out.smallModels.Count;
        Out0SmallVertextCount = info_out.sumVertex_Small;
        return info_out;
    }

    private BigSmallListInfo GetBigSmallInfo_In()
    {
        if (InPart == null)
        {
            Debug.LogError($"GetBigSmallInfo_In InPart==null model:{this.name}");
            return null;
        }
        // JobSetting =GameObject.FindObjectOfType<AcRTAlignJobSetting>(true);
        // if (JobSetting == null)
        // {
        //     Debug.LogError("GetSmallBigInfo JobSetting == null");
        //     return;
        // }
        //RendererManager.Instance.SetDetailRenderers(InPart.GetComponentsInChildren<MeshRenderer>(true));
        var info_out = new BigSmallListInfo(InPart, isIgnoreGPU);
        _inBigRendererCount = info_out.bigModels.Count;
        _inBigVertextCount = info_out.sumVertex_Big;
        _inSmallRendererCount = info_out.smallModels.Count;
        _inSmallVertextCount = info_out.sumVertex_Small;
        return info_out;
    }

    public void HideBigModels_Out0()
    {
        BigSmallListInfo info = GetBigSmallInfo_Out0();
        info.SetBigModelsVisible(false);
    }

    public void HideSmallModels_Out0()
    {
        BigSmallListInfo info = GetBigSmallInfo_Out0();
        info.SetSmallModelsVisible(false);
    }

    public void ShowBigModels_Out0()
    {
        BigSmallListInfo info = GetBigSmallInfo_Out0();
        info.SetBigModelsVisible(true);
    }

    public void ShowSmallModels_Out0()
    {
        BigSmallListInfo info = GetBigSmallInfo_Out0();
        info.SetSmallModelsVisible(true);
    }

    public void HideBigModels_In()
    {
        BigSmallListInfo info = GetBigSmallInfo_In();
        info.SetBigModelsVisible(false);
    }

    public void HideSmallModels_In()
    {
        BigSmallListInfo info = GetBigSmallInfo_In();
        info.SetSmallModelsVisible(false);
    }

    public void ShowBigModels_In()
    {
        BigSmallListInfo info = GetBigSmallInfo_In();
        info.SetBigModelsVisible(true);
    }

    public void ShowSmallModels_In()
    {
        BigSmallListInfo info = GetBigSmallInfo_In();
        info.SetSmallModelsVisible(true);
    }

    private void GetInOutParts(bool isDefaultOut0=true)
    {
        InPart=null;
        OutPart0=null;
        OutPart1=null;
        LODPart = null;
        GPUIPart = null;
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == "In")
            {
                InPart = child.gameObject;
            }
            if (child.name == "Out0" || child.name == "Out")
            {
                OutPart0 = child.gameObject;
            }
            if (child.name == "Out1")
            {
                OutPart1 = child.gameObject;
            }
            if (child.name == "GPUI")
            {
                GPUIPart = child.gameObject;
                HideGPUIRenderers();
            }
            if (child.name.EndsWith("LODs"))
            {
                LODPart = child.gameObject;
            }
            if (child.name.EndsWith("LOD"))
            {
                child.name = child.name.Replace("LOD","LODs");
                LODPart = child.gameObject;
            }
            children.Add(child);
        }



        if (OutPart1 == null && OutPart0 == null && InPart == null)
        {
            if (isDefaultOut0)
            {
                Debug.Log("OutPart1 == null && OutPart0 == null && InPart == null Default Out0");
                GameObject outRoot = InitPart("Out0");
                OutPart0 = outRoot;
                foreach (var child in children)
                {
                    child.SetParent(outRoot.transform);
                }
            }
            else
            {
                Debug.Log("OutPart1 == null && OutPart0 == null && InPart == null Default In");
                GameObject outRoot = InitPart("In");
                InPart = outRoot;
                foreach (var child in children)
                {
                    child.SetParent(outRoot.transform);
                }
            }

            // InPart = InitPart("In");
            // OutPart1 = InitPart("Out1");
        }

        if (InPart == null)
        {
            InPart = InitPart("In");
        }
        if (OutPart1 == null)
        {
            OutPart1 = InitPart("Out1");
        }
        if (OutPart0 == null)
        {
            OutPart0 = InitPart("Out0");
        }


        if (OutPart1 == null && OutPart0 != null && InPart == null && OutPart0.transform.childCount == 0)
        {
            foreach (var child in children)
            {
                if (child.gameObject == OutPart0) continue;
                child.SetParent(OutPart0.transform);
            }
        }
    }


    public ModelAreaTree[] trees;

    public ModelAreaTree[] GetTrees()
    {
        if (trees == null)
        {
            UpdateTrees();
        }
        return trees;
    }

    public List<ModelAreaTree> GetTreeList()
    {
        TreeNodeCount = 0;

        if (trees == null)
        {
            trees= this.GetComponentsInChildren<ModelAreaTree>(true);
        }
        return trees.Where(i=>i!=null).ToList();
    }

    private int TreeNodeCount = 0;

    public int GetTreeNodeCount()
    {
        if (TreeNodeCount==0)
        {
            GetNodeList();
        }
        return TreeNodeCount;
    }

    public List<AreaTreeNode> GetNodeList()
    {
        List<AreaTreeNode> nodes = new List<AreaTreeNode>();
        foreach(var t in GetTrees())
        {
            if (t == null) continue;
            nodes.AddRange(t.TreeLeafs);
        }
        TreeNodeCount = nodes.Count;
        return nodes;
    }

    //[ContextMenu("* CreateTreesEx")]
    public void CreateTreesEx()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        trees = CreateTreesInnerEx(false,null);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTreesBSEx")]
    public void CreateTreesBSEx()
    {
        CreateTreesBSEx(false,null);
    }

    public void CreateTreesBSEx(bool isLod, Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        BuildingModelInfo[] subModels = this.GetComponentsInChildren<BuildingModelInfo>(true);
        if (subModels.Length > 1)
        {
            Debug.Log($"CreateTreesBSEx subModels.Length > 1 this:{this.name} subModels:{subModels.Length}");
            return;
        }
        AreaTreeManager.Instance.ClearCount();

        AreaTreeManager.Instance.IsByLOD = isLod;

        InitInOut(false);

        trees = CreateTreesInnerEx(true, p => {
            if (progressChanged != null)
            {
                progressChanged(p);
            }
            else
            {
                ProgressBarHelper.DisplayProgressBar("CreateTreesBSEx", p);
            }
        });
        AreaTreeManager.Instance.AddTrees(trees);
        if(progressChanged==null)
            ProgressBarHelper.ClearProgressBar(); 

        HideDetail();
        Debug.LogWarning($"CreateTreesBSEx {(DateTime.Now - start).ToString()}");
    }

    private DictList<SubScene_Base> scenes = new DictList<SubScene_Base>();

    internal bool ContainsScene(SubScene_Base scene)
    {
        try
        {
            if (scene == null) return false;
            if (scenes.Count == 0)
            {
                scenes = new DictList<SubScene_Base>(this.GetComponentsInChildren<SubScene_Base>(true));
            }
            return scenes.Contains(scene);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ContainsScene scene:{scene}");
            return false;
        }

    }

    [ContextMenu("* CreateTreesByLOD")]
    public void CreateTreesByLOD()
    {
        CreateTreesBSEx(true,null);
    }

    //[ContextMenu("* CreateTrees")]
    public void CreateTrees()
    {
        DateTime start = DateTime.Now;
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        trees = CreateTreesInner(null);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
        Debug.LogError($"CreateTreesBSEx {(DateTime.Now - start).ToString()}");
    }

    ////[ContextMenu("* CreateTreesBS")]
    //public void CreateTrees_BS()
    //{

    //    AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
    //    CreateTrees_BigSmall_Core_Out0(null);
    //}

    [NonSerialized]
    public bool isIgnoreGPU = true;

    public ModelAreaTree[] CreateTrees_BigSmall_Core_GPUI(Action<ProgressArg> progressChanged)
    {
        if (GPUIPart)
        {
            SubScene_Base scene = GPUIPart.GetComponent<SubScene_Base>();
            if (scene != null)
            {
                GameObject.DestroyImmediate(scene);
            }
        }
        return CreateTrees_BigSmall_Core(GPUIPart, progressChanged, false);
    }

    public ModelAreaTree[] CreateTrees_BigSmall_Core_In(Action<ProgressArg> progressChanged)
    {
        return CreateTrees_BigSmall_Core(this.InPart, progressChanged, isIgnoreGPU);
    }

    public ModelAreaTree[] CreateTrees_BigSmall_Core_Out0(Action<ProgressArg> progressChanged)
    {
        //if(this.OutPart0==null)
        //{
        //    return null;
        //}
        ////trees = CreateTreesInner();
        ////Debug.Log("CreateTrees_BigSmall");
        //AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //if (treeManager)
        //{
        //    treeManager.Target = this.OutPart0;
        //    var ts= treeManager.CreateOne_BigSmall_Core(this.transform, this.OutPart0, progressChanged, isIgnoreGPU);
        //    foreach(var tree in ts)
        //    {
        //        if (tree == null) continue;
        //        tree.name = this.name + "_" + tree.name;
        //    }
        //    return ts;
        //}
        //else
        //{
        //    Debug.LogError("treeManager==null");
        //    return null;
        //}
        return CreateTrees_BigSmall_Core(this.OutPart0, progressChanged, isIgnoreGPU);
    }

    public ModelAreaTree[] CreateTrees_BigSmall_Core(GameObject target,Action<ProgressArg> progressChanged, bool isIgnoreGPU)
    {
        if (target == null)
        {
            return null;
        }
        //trees = CreateTreesInner();
        //Debug.Log("CreateTrees_BigSmall");
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
        {
            treeManager.Target = target;
            var ts = treeManager.CreateOne_BigSmall_Core(this.transform, target, progressChanged, isIgnoreGPU);
            foreach (var tree in ts)
            {
                if (tree == null) continue;
                tree.name = this.name + "_" + tree.name;
            }
            return ts;
        }
        else
        {
            Debug.LogError("treeManager==null");
            return null;
        }
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        var renderers = this.GetComponentsInChildren<MeshRenderer>(true) ;
        MeshHelper.ShowAllRenderers(renderers, 5,this.name);
    }

    //[ContextMenu("ShowAll")]
    public void ShowAll()
    {
        if (InPart)
        {
            InPart.SetActive(true);
        }
        if (OutPart0)
        {
            OutPart0.SetActive(true);
        }
        if (OutPart1)
        {
            OutPart1.SetActive(true);
        }
    }

    

    public static void UpackPrefab_One(GameObject go)
    {
#if UNITY_EDITOR
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
#endif
    }

    public void Unpack()
    {
        UpackPrefab_One(this.gameObject);
    }

 

    private ModelAreaTree CreateTree(GameObject target,string treeName, Action<ProgressArg> progressChanged)
    {
        if (target == null) return null;
        
        AreaTreeManager treeManager = AreaTreeHelper.InitCubePrefab();

        GameObject treeGo1 = new GameObject(this.name+"_"+treeName);
        treeGo1.transform.position = target.transform.position;
        treeGo1.transform.SetParent(this.transform);
        ModelAreaTree tree1 = treeGo1.AddComponent<ModelAreaTree>();
        //tree1.Target = target;
        tree1.SetRenderers(target);
        if (treeManager)
        {
            tree1.nodeSetting = treeManager.nodeSetting;
        }

        tree1.GenerateMesh(progressChanged);

        treeGo1.SetActive(target.activeInHierarchy);

        tree1.IsHidden = !target.activeInHierarchy;
        return tree1;
    }

    private void ClearPart(GameObject partObj)
    {
        if(partObj!=null){
            while(partObj.transform.childCount>0){
                Transform child=partObj.transform.GetChild(0);
                child.SetParent(this.transform);
            }
            GameObject.DestroyImmediate(partObj);
        }

    }

    public GameObject InitPart(string n)
    {
        GameObject outRoot = new GameObject(n);
        outRoot.transform.position = this.transform.position;
        outRoot.transform.SetParent(this.transform);
        return outRoot;
    }

    public GameObject InitSubPart(string n,Transform p)
    {
        GameObject outRoot = new GameObject(n);
        outRoot.transform.position = p.position;
        outRoot.transform.SetParent(p);
        return outRoot;
    }

    private void GetInOutVertextCount(bool isShowOut0Log)
    {
        InVertextCount = GetChildrenVertextCount(InPart, out InRendererCount,false);
        Out0VertextCount = GetChildrenVertextCount(OutPart0,out Out0RendererCount, isShowOut0Log);
        Out1VertextCount = GetChildrenVertextCount(OutPart1,out Out1RendererCount, false);
        LODVertexCount = GetChildrenVertextCount(LODPart, out LODRendererCount, false);

        AllVertextCount = InVertextCount + Out0VertextCount + Out1VertextCount+ LODVertexCount;
        AllRendererCount = InRendererCount + Out0RendererCount + Out1RendererCount+ LODRendererCount;
    }

    class VertexCountInfo
    {
        public string name;

        public float count;

        public VertexCountInfo(float c,string n)
        {
            count = c;
            name = n;
        }

        public override string ToString()
        {
            return $"{count:F1}    {name}";
        }
    }

    private float GetChildrenVertextCount(GameObject go, out int renderCount, bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        List<VertexCountInfo> logs = new List<VertexCountInfo>();
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            int childRendererCount = 0;
            float childVertexCount = GetVertextCount(child.gameObject, out childRendererCount, showLog);
            count += childVertexCount;
            renderCount += childRendererCount;

            if (childVertexCount > 1)
            {
                //logs.Add($"{childVertexCount:F1}    {go.name}->{child.name}");
                logs.Add(new VertexCountInfo(childVertexCount, $"{go.name}->{child.name}"));
                //Debug.LogError($"GetChildrenVertextCount[{i}]    {childVertexCount:F3}    {go.name}->{child.name}");
            }
        }

        if(showLog)
        {
            logs.Sort((a, b) => a.count.CompareTo(b.count));

            for (int i = 0; i < logs.Count; i++)
            {
                Debug.LogError($"GetChildrenVertextCount[{i}]    {logs[i]}");
            }
        }

        return count;
    }

    private float GetVertextCount(GameObject go,out int renderCount,bool showLog)
    {
        renderCount = 0;
        if (go == null)
        {
            return 0;
        }
        float count = 0;
        var filters = go.GetComponentsInChildren<MeshFilter>(true).ToList().FindAll(i=>i!=null&&i.sharedMesh!=null);
        //filters.Sort((a, b) => b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount));
        filters.Sort((a, b) => a.sharedMesh.vertexCount.CompareTo(b.sharedMesh.vertexCount));

        for (int i = 0; i < filters.Count; i++)
        {
            MeshFilter filter = filters[i];
            if (filter == null) continue;
            if (filter.sharedMesh == null) continue;
            float vertextCount = filter.sharedMesh.vertexCount / 10000.0f;
            count += vertextCount;
            if (showLog)
            {
                if (vertextCount > 5f)
                {
                    Debug.LogWarning($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
                else if (vertextCount > 1f)
                {
                    Debug.Log($"GetVertextCount[{i}]    {vertextCount:F3}    {go.name}->{filter.name}|{filter.transform.parent}");
                }
            }

        }
        renderCount = filters.Count;
        return count;
    }

 

    private void Awake()
    {
        //InitInOut();
    }

    //[ContextMenu("HideIn")]
    //public void HideIn()
    //{
    //    if (InPart)
    //        InPart.SetActive(false);
    //}

    //[ContextMenu("ShowIn")]
    //public void ShowIn()
    //{
    //    if (InPart)
    //        InPart.SetActive(true);
    //}

    public bool IsDetailVisible = false;

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        IsDetailVisible = false;
        //if(InPart)
        //    InPart.SetActive(false);
        //if(OutPart1)
        //    OutPart1.SetActive(false);
        //if(OutPart0)
        //    OutPart0.SetActive(true);

        
    }

    [ContextMenu("HideGPUIRenderers")]
    public void HideGPUIRenderers()
    {
        if (GPUIPart)
        {
            //GPUIPart.SetActive(false);
            MeshRenderer[] meshRenderers = GPUIPart.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = false;
                renderer.gameObject.SetActive(false);
            }
        }
    }

    [ContextMenu("ShowGPUIRenderers")]
    public void ShowGPUIRenderers()
    {
        if (GPUIPart)
        {
            //GPUIPart.SetActive(false);
            MeshRenderer[] meshRenderers = GPUIPart.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = true;
                renderer.gameObject.SetActive(true);
            }
        }
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        if(InPart)
            InPart.SetActive(true);
        if(OutPart1)
            OutPart1.SetActive(true);
        if (trees == null)
        {
            UpdateTrees();
        }
        foreach(var tree in GetTrees()){
            if(tree==null)continue;
            tree.gameObject.SetActive(true);
        }
    }

    private void DestroyOldPartScenes(SceneContentType contentType)
    {
        var components = SubScene_List.GetBaseScenes(this.gameObject);//In Out0 Out1
        foreach (var c in components)
        {
            if(c.contentType==contentType)
                GameObject.DestroyImmediate(c);
        }
    }

    //[ContextMenu("LoadScenes_Part")]
    //public void LoadScenes_Part()
    //{
    //    var singleScene = gameObject.GetComponent<SubScene_Single>();
    //    if (singleScene) singleScene.UnLoadGosM();

    //    var partScenes = gameObject.GetComponentsInChildren<SubScene_Part>(true);
    //    SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
    //    subSceneManager.LoadScenesEx(partScenes,null);

    //    //SceneState = "LoadScenes_Part";
    //}

    //private void UnloadPartScenes()
    //{
    //    var partScenes = gameObject.GetComponentsInChildren<SubScene_Part>(true);
    //    foreach (var scene in partScenes)
    //    {
    //        scene.UnLoadGosM();
    //        scene.DestroyBoundsBox();
    //    }
    //}

    //[ContextMenu("LoadScene")]
    //public void LoadScene()
    //{
    //    //UnloadPartScenes();

    //    var scenes = gameObject.GetComponentsInChildren<SubScene_Single>(true);
    //    SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
    //    subSceneManager.LoadScenesEx(scenes,null);
    //}

#if UNITY_EDITOR

    //[ContextMenu("OneKey_TreePartScene")]
    //public void OneKey_TreePartScene()
    //{
    //    OneKey_TreePartScene(p =>
    //    {
    //        ProgressBarHelper.DisplayProgressBar("OneKey_TreePartScene", $"Progress:{p:P2}", p);
    //    });
    //    ProgressBarHelper.ClearProgressBar();
    //}

    public void OneKey_TreePartScene(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;
        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("OneKey_TreePartScene", 0,3,"Start"));
        }
        InitInOut();

        ShowDetail();

        ProgressArg p1 = new ProgressArg("OneKey_TreePartScene", 1, 3,"CreateTree");
        if (progressChanged != null)
        {
            progressChanged(p1);
        }
        CreateTreesInnerEx(true, subP =>
        {
            p1.AddSubProgress(subP);
            if (progressChanged != null)
            {
                progressChanged(p1);
            }
        });

        ProgressArg p2 = new ProgressArg("OneKey_TreePartScene", 2, 3,"SaveNodes");
        if (progressChanged != null)
        {
            progressChanged(p2);
        }
        EditorCreateNodeScenes(subP=>
        {
            p2.AddSubProgress(subP);
            if (progressChanged != null)
            {
                progressChanged(p2);
            }
        });
        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("OneKey_TreePartScene", 3, 3,"End"));
        }
        Debug.LogError($"BuildingModelInfo.OneKey_TreePart Time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("* OneKey_TreeNodeScene")]
    public void OneKey_TreeNodeScene()
    {
        OneKey_TreeNodeScene(null);
        ProgressBarHelper.ClearProgressBar();
    }

    public void OneKey_TreeNodeScene(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;
        ProgressArg p1 = new ProgressArg("OneKey_TreeNodeScene", 0, 3,"Start");
        if (progressChanged != null)
        {
            progressChanged(p1);
        }
        InitInOut(false);

        ProgressArg p2 = new ProgressArg("OneKey_TreeNodeScene", 1, 3,"CreateTreeBS");
        if (progressChanged != null)
        {
            progressChanged(p2);
        }
        CreateTreesBSEx(false,subP=>
        {
            p2.AddSubProgress(subP);
            if (progressChanged != null)
            {
                progressChanged(p2);
            }
        });
        //CreateTreesInnerEx(true, null);

        ProgressArg p3 = new ProgressArg("OneKey_TreeNodeScene", 2, 3,"SaveScene");
        if (progressChanged != null)
        {
            progressChanged(p3);
        }
        //EditorCreateScenes_TreeWithPart((subP, i, c) =>
        //{
        //    if (progressChanged != null)
        //    {
        //        progressChanged((2 + subP) / 3f);
        //    }
        //});
        EditorCreateNodeScenes((p)=>
        {
            p3.AddSubProgress(p);
            if (progressChanged != null)
            {
                progressChanged(p3);
            }
        });
        ProgressArg p4 = new ProgressArg("OneKey_TreeNodeScene", 3, 3,"End");
        if (progressChanged != null)
        {
            progressChanged(p4);
        }
        Debug.LogError($"BuildingModelInfo.OneKey_TreeNodeScene Time:{(DateTime.Now - start).ToString()}");
    }

    public SceneContentType contentType;

    #region CreateScenes

    public void EditorCreateScenesEx(SceneContentType contentType, Action<ProgressArg> progressChanged)
    {
        this.contentType = contentType;
        if (contentType == SceneContentType.TreeWithPart)
        {
            EditorCreateScenes_TreeWithPart(progressChanged);
        }
        else
        {
            EditorCreateScenes();
        }
    }

    [ContextMenu("* EditorCreateScenes")]
    public void EditorCreateScenes()
    {
        DateTime start = DateTime.Now;

        SaveTreeRendersId();

        DestroyScenes();

        InitInOut(false);
        var scenes=CreatePartScene(contentType);
        EditorCreateScenes(scenes,null);

        //SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes time:{(DateTime.Now - start)}");
    }

    //[ContextMenu("* EditorCreateScenes_TreeWithPart")]
    public void EditorCreateScenes_TreeWithPart()
    {
        EditorCreateScenes_TreeWithPart(null);
    }

    public void EditorCreateScenes_TreeWithPart(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        SaveTreeRendersId();

        DestroyScenes();

        InitInOut(false);

        SubSceneBag scenes = new SubSceneBag();
        scenes.AddRange(CreatePartScene(SceneContentType.Tree));
        scenes.AddRange(CreatePartScene(SceneContentType.Part));
        EditorCreateScenes(scenes, progressChanged);


        //SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes_TreeWithPart time:{(DateTime.Now - start)},progressChanged:{progressChanged}");
    }

   

    public SubSceneBag CreatePartScene(SceneContentType contentType)
    {
        SubSceneBag scenes = new SubSceneBag();
        AreaTreeHelper.InitCubePrefab();
        if (contentType == SceneContentType.Single)
        {
            var scene=SubSceneHelper.EditorCreateScene<SubScene_Single>(this.gameObject, SceneContentType.Single, false,null);
            scenes.Add(scene);
        }
        //else if (contentType == SceneContentType.TreePart)
        //{
        //    string dirPath = SubSceneManager.Instance.GetSceneDir(contentType);
        //    EditorCreatePartScenes(dirPath, true);
        //}
        else
        {
            string dirPath = SubSceneManager.Instance.GetSceneDir(contentType);
            scenes.AddRange(EditorCreatePartScenesEx(dirPath, true, contentType));
        }
        return scenes;
    }

    internal SubSceneBag EditorCreatePartScenesEx(string dir, bool isOverride, SceneContentType contentType)
    {
        //DestroyOldBounds();
        DestroyOldPartScenes(contentType);

        SubSceneBag senes = new SubSceneBag();
        UpdateTrees();
        if (InPart)
        {
            var scene = CreatePartSceneEx(InPart, contentType, "_In_" + contentType, GetTrees(), dir, isOverride, gameObject.AddComponent<SubScene_In>());
            senes.Add(scene);
        }
        if (OutPart0)
        {
            var scene = CreatePartSceneEx(OutPart0, contentType, "_Out0_" + contentType, GetTrees(), dir, isOverride, gameObject.AddComponent<SubScene_Out0>());
            senes.Add(scene);
        }
        if (OutPart1)
        {
            var scene = CreatePartSceneEx(OutPart1, contentType, "_Out1_" + contentType, GetTrees(), dir, isOverride, gameObject.AddComponent<SubScene_Out1>());
            senes.Add(scene);
        }
        return senes;
    }

    public SubScene_Base CreatePartSceneEx(GameObject go, SceneContentType contentType, string nameAf, ModelAreaTree[] trees, string path, bool isOverride, SubScene_Base ss)
    {
        if (go)
        {
            ss.contentType = contentType;

            List<GameObject> gos = new List<GameObject>();

            if (contentType == SceneContentType.Part || contentType == SceneContentType.TreeAndPart)
                gos.Add(go);

            //if (trees != null && (contentType == SceneContentType.Tree || contentType == SceneContentType.TreeAndPart))
            //    foreach (var tree in GetTrees())
            //    {
            //        if (tree == null) continue;

            //        if (tree.Target == go)
            //        {
            //            gos.Add(tree.gameObject);
            //        }
            //    }

            string scenePath = $"{path}{this.name}{nameAf}.unity";
            ss.SetArg(scenePath, isOverride,gos);
            ss.Init();

            //ss.SaveScene();
            //ss.ShowBounds();
            return ss;
        }
        else
        {
            Debug.LogError("CreatePartSceneEx go==null");
        }
        return null;
    }
    //public SubScene_Base CreatePartScene(GameObject go, string nameAf, ModelAreaTree[] trees, string path, bool isOverride, SubScene_Base ss)
    //{
    //    if (go)
    //    {
    //        List<GameObject> gos = new List<GameObject>();
    //        gos.Add(go);

    //        if (trees != null)
    //            foreach (var tree in trees)
    //            {
    //                if (tree == null) continue;

    //                if (tree.Target == go)
    //                {
    //                    gos.Add(tree.gameObject);
    //                }
    //            }

    //        ss.contentType = SceneContentType.TreeAndPart;
    //        string scenePath = $"{path}{this.name}{nameAf}.unity";
    //        ss.SetArg(scenePath, isOverride,gos);
    //        ss.Init();
    //        //ss.SaveScene();
    //        //ss.ShowBounds();
    //        return ss;
    //    }
    //    return null;
    //}
    
    #endregion



    [ContextMenu("* EditorLoadScenes")]
    private void EditorLoadScenes()
    {
        EditorLoadScenes(null);
    }


    public void EditorLoadScenesByContentType(SceneContentType contentType, Action<ProgressArg> progressChanged)
    {
        this.contentType = contentType;
        if (contentType == SceneContentType.TreeWithPart)
        {
            EditorLoadScenes_TreeWithPart(progressChanged);
        }
        else
        {
            EditorLoadScenes(progressChanged);
        }
    }

    public override void EditorLoadScenes(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        //EditorLoadScenes(contentType);

        var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { contentType });
        SubSceneHelper.EditorLoadScenes(scenes.ToArray(), progressChanged);

        this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
        LoadTreeRenderers();

        Debug.LogError($"EditorLoadScenes time:{(DateTime.Now - start)} scenes:{scenes.Count}");
    }

    private void EditorLoadScenes_TreeWithPart(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { SceneContentType.Part, SceneContentType.Tree });

        SubSceneHelper.EditorLoadScenes(scenes.ToArray(), progressChanged);
        LoadTreeRenderers(scenes);
        InitInOut();

        Debug.LogError($"EditorLoadScenes_TreeWithPart time:{(DateTime.Now - start)}");
    }

    //[ContextMenu("EditorLoadScenes_TreeWithPart")]
    public void EditorLoadScenes_TreeWithPart()
    {
        EditorLoadScenes_TreeWithPart(null);
    } 

    //[ContextMenu("EditorSaveScenes_Part")]
    //public void EditorSaveScenes_Part()
    //{
    //    var scenes = gameObject.GetComponentsInChildren<SubScene_Part>(true);
    //    foreach (var scene in scenes)
    //    {
    //        scene.EditorSaveScene();
    //    }
    //    //SceneState = "EditSaveScenes_Part";
    //}

    [ContextMenu("* EditorSaveScenes")]
    public void EditorSaveScenes()
    {
        var scenes = SubScene_List.GetBaseScenes(gameObject);
        foreach (var scene in scenes)
        {
            scene.IsLoaded = true;
            scene.EditorSaveScene();
        }
        //SceneState = "EditSaveScenes";
    }

    [ContextMenu("* EditorCreateNodeScenes")]
    public void EditorCreateNodeScenes()
    {
        EditorCreateNodeScenes(null);
        EditorHelper.ClearOtherScenes();
    }

    public bool IsScenesFolderExists()
    {
        foreach (var t in GetTrees())
        {
            if (t.IsScenesFolderExists() == false)
            {
                return false;
            }
        }
        return true;
    }

    public void SelectScenesFolder()
    {
        //string dir = SubSceneManager.Instance.GetSceneDir(SceneContentType.TreeNode, this.name);
        //Debug.Log(dir);
        //string folderPath = "Assets/" + dir;
        //Debug.Log(folderPath);
        //UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(folderPath, typeof(UnityEngine.Object));
        //Debug.Log(asset);
        //if (asset != null)
        //{
        //    EditorHelper.SelectObject(asset);
        //}
        //string dirPath = Application.dataPath + "/" + dir;
        //Debug.Log(dirPath);
        //Debug.Log(System.IO.Directory.Exists(dirPath));

        List<UnityEngine.Object> folderAssets = new List<UnityEngine.Object>();
        foreach(var t in GetTrees())
        {
            var folder = t.GetScenesFolder();
            if (folder != null)
            {
                folderAssets.Add(folder);
            }
        }
        EditorHelper.SelectObjects(folderAssets);
    }

    public void DeleteScenesFolder()
    {
        foreach (var t in GetTrees())
        {
            t.DeleteScenesFolder(false);
        }
        this.DestroyScenes();
        EditorHelper.RefreshAssets();
    }

    [ContextMenu("InitRenderers")]
    public List<RendererId> InitRenderers()
    {
        List<RendererId> idsAll = new List<RendererId>();
        trees = null;
        var ts = GetTrees();
        for (int i = 0; i < ts.Length; i++)
        {
            var tree = ts[i];
            var rs=tree.InitRenderers();
            idsAll.AddRange(rs);
        }
        IdDictionary.SaveChildrenIds(idsAll,this.gameObject);
        return idsAll;
    }

    public void EditorCreateNodeScenes(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        GetInOutParts();

        ShowDetail();
        InitRenderers();
        var ts = GetTrees();
        for (int i = 0; i < ts.Length; i++)
        {
            var tree = ts[i];
            if (tree == null) continue;
            //float progress = (float)i / ts.Length;
            //float percents = progress * 100;
            ProgressArg p1 = new ProgressArg("EditorCreateNodeScenes", i, ts.Length, tree);
            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    break;
                }
            }
            else
            {
                progressChanged(p1);
            }

            tree.EditorCreateNodeScenes(p =>
            {
                p1.AddSubProgress(p);
                //float progress2 = (float)(i + p) / ts.Length;
                //float percents2 = progress2 * 100;
                //ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorCreateNodeScenes ", $"Progress2 {(i + p):F2}/{trees.Length} {percents2:F2}%", progress2);

                if (progressChanged == null)
                {
                    ProgressBarHelper.DisplayCancelableProgressBar(p1);
                }
                else
                {
                    progressChanged(p1);
                }
            });
        }

        EditorCreateLODsScene();

        //EditorCreateGPUIScene();

        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("EditorCreateNodeScenes", ts.Length, ts.Length));
        }

        UpdateSceneList();

        //EditorSavePrefab();
        if (progressChanged == null) Debug.Log($"BuildingModelInfo.EditorCreateNodeScenes time:{(DateTime.Now - start)} model:{this.name}");
    }

    [ContextMenu("EditorCreateLODsScene")]
    public bool EditorCreateLODsScene()
    {
        if (LODPart != null && LODPart.transform.childCount > 0)
        {
            SubScene_Ref.BeforeCreateScene(LODPart);
            SubScene_LODs lodsScene = SubSceneHelper.CreateSubScene<SubScene_LODs>(LODPart, LODPart, SceneContentType.TreeNode);
            lodsScene.contentType = SceneContentType.LODs;
            return true;
        }
        return false;
    }

    //[ContextMenu("EditorCreateGPUIScene")]
    //public bool EditorCreateGPUIScene()
    //{
    //    if (GPUIPart != null && GPUIPart.transform.childCount > 0)
    //    {
    //        //SubScene_Ref.BeforeCreateScene(GPUIPart);
    //        SubScene_Single lodsScene = SubSceneHelper.CreateSubScene<SubScene_Single>(GPUIPart, GPUIPart, SceneContentType.TreeNode);
    //        lodsScene.contentType = SceneContentType.GPUI;
    //        return true;
    //    }
    //    return false;
        
    //}

    [ContextMenu("* EditorLoadNodeScenes")]
    public void EditorLoadNodeScenes()
    {
        EditorLoadNodeScenes(null);
    }

    public void OneKeyLoadScene(bool isInitIds = true)
    {
        EditorLoadNodeScenesEx(isInitIds);
        DestroyScenesEx();
        ClearTrees();
    }

    public void OneKeySaveScene()
    {
        AreaTreeManager.Instance.isCombine = false;
        InitInOut();
        CreateTreesBSEx();
        EditorCreateNodeScenes();
    }


    public void EditorLoadNodeScenesEx(bool isInitIds = true)
    {
        IdDictionary.InitInfos();
        EditorLoadNodeScenes(null, isInitIds);
    }

    //[ContextMenu("* EditorLoadNodeScenes")]
    public void EditorLoadNodeScenes(Action<ProgressArg> progressChanged,bool isInitIds=true)
    {
        Unpack();
        DateTime start = DateTime.Now;
        if (isInitIds)
        {
            IdDictionary.InitInfos();
        }

        EditorLoadLODsScene();

        EditorLoadGPUIScene();

        var ts = GetTrees();
        for (int i = 0; i < ts.Length; i++)
        {
            var tree = ts[i];
            if (tree == null) continue;
            ProgressArg p1 = new ProgressArg("EditorLoadNodeScenes", i, ts.Length, tree);

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    break;
                }
            }
            else
            {
                progressChanged(p1);
            }


            tree.EditorLoadNodeScenes(p =>
            {
                p1.AddSubProgress(p);

                //float progress2 = (float)(i + p) / ts.Length;
                //float percents2 = progress2 * 100;

                if (progressChanged == null)
                {
                    ProgressBarHelper.DisplayCancelableProgressBar(p1);
                }
                else
                {
                    progressChanged(p1);
                }
            });
        }



        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("EditorLoadNodeScenes", ts.Length, ts.Length));
        }

        Debug.LogWarning($"BuildingModelInfo.EditorLoadNodeScenes time:{(DateTime.Now - start)}");
    }

    [ContextMenu("EditorLoadLODsScene")]
    public bool EditorLoadLODsScene()
    {
        if (LODPart)
        {
            SubSceneHelper.EditorLoadScenes(LODPart, null);
            SubScene_Ref.AfterLoadScene(LODPart);
            return true;
        }
        return false;
    }

    [ContextMenu("EditorLoadGPUIScene")]
    public bool EditorLoadGPUIScene()
    {
        if (GPUIPart)
        {
            SubSceneHelper.EditorLoadScenes(GPUIPart, null);
            //SubScene_Ref.AfterLoadScene(GPUIPart);
            return true;
        }
        return false;
    }

    public int EditorDeleteOtherRepleatScenes()
    {
        int count = 0;
        var scenes = this.GetComponentsInChildren<SubScene_Out0>(true);
        if (scenes.Length > 0)
        {
            var arg = scenes[0].GetSceneArg();
            string scenePath1 = arg.path;
            string sceneAssetPath = arg.GetSceneAssetPath();
            string sceneFilePath = arg.GetSceneFilePath();

            //Debug.Log($"ScenePath:{scenePath1}");
            //Debug.Log($"AssetPath:{sceneAssetPath}");
            //Debug.Log($"DataPath:{Application.dataPath}");
            //Debug.Log(sceneFilePath + "|" + System.IO.File.Exists(sceneFilePath));

            FileInfo fileInfo = new FileInfo(sceneFilePath);
            DirectoryInfo dirInfo = fileInfo.Directory;
            //Debug.Log($"DirInfo:{dirInfo.FullName}");
            if (dirInfo.Name.Contains("Tree"))
            {
                DirectoryInfo dirInfo2 = dirInfo.Parent;
                //Debug.Log($"DirInfo2:{dirInfo2.FullName}");
                string dirPath = dirInfo2.FullName;
                string dir1 = dirPath.Replace("\\", "/");
                //Debug.Log($"dir1:{dir1}");
                string assetDir2Path = "Assets" + dir1.Replace(Application.dataPath, "") + "/";
                //Debug.Log($"AssetDir2:{assetDir2Path}");

                DirectoryInfo dirInfo3 = dirInfo2.Parent;

                int id = dirPath.IndexOf('[');
                int id2 = dirPath.IndexOf(']');
                string sameDir = dirPath.Substring(0, id+1);
                string insIdS = dirPath.Substring(id + 1, id2 - id - 1);
                RendererId rid = RendererId.GetRId(this);
                //Debug.Log($"sameDir:{sameDir}");
                //Debug.Log($"insId:{insIdS}");
                int insId = 0;
                if (int.TryParse(insIdS, out insId))
                {
                    if (rid.insId != insId)
                    {
                        Debug.LogError($"building:{this.name} rid.insId != insId {rid.insId} -> {insId}");
                        rid.insId = insId;
                    }
                    else
                    {
                        //Debug.LogWarning("rid.insId == insId");
                    }
                }

                DirectoryInfo[] subDirs = dirInfo3.GetDirectories();
               
                foreach (var subDir in subDirs)
                {
                    string subDirPath = subDir.FullName;
                    if (subDirPath == dirPath)
                    {
                        continue;
                    }
                    try
                    {

                        if (subDirPath.StartsWith(sameDir))
                        {
                            count++;
                            //subDir.Delete(true);
                            string dir3 = subDirPath.Replace("\\", "/");
                            //Debug.Log($"dir1:{dir1}");
                            string assetDir3Path = "Assets" + dir3.Replace(Application.dataPath, "") + "/";

                            //Debug.LogWarning($"DeleteDir[{count}] asset:{assetDir3Path} fullPath:{subDirPath}");

                            AssetDatabase.DeleteAsset(assetDir3Path);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"DeleteDirException({this.name})[{count}]:{subDirPath} ex:{ex.Message}");
                    }

                }

                AssetDatabase.Refresh();
            }
            else
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
                Debug.Log(sceneAsset);
                if (sceneAsset != null)
                    EditorHelper.SelectObject(sceneAsset);
            }
        }
        else
        {

        }
        return count;
    }
#endif
}

public class BuildingModelState
{
    public int sceneCount ;
    public int unloadedSceneCount ;
    public bool isAllLoaded ;
    public int treeCount ;
    public int partCount ;

    public bool HaveUnloadedScenes;

    public bool CanCreateTrees;

    public bool CanRemoveTrees;

    public bool CanCreateScenes;

    public bool CanRemoveScenes;

    public bool CanLoadScenes;

    public bool CanUnloadScenes;

    public BuildingModelState(BuildingModelInfo b)
    {
        sceneCount = b.GetSceneCount();
        unloadedSceneCount = b.SceneList.GetUnloadedScenes().Count;
        isAllLoaded = b.IsSceneLoaded();

        treeCount = b.GetTreeCount();
        partCount = b.GetPartCount();

        CanCreateTrees = isAllLoaded == true && partCount > 0 && treeCount == 0;
        CanRemoveTrees = isAllLoaded == true && treeCount > 0;
        CanCreateScenes = isAllLoaded == true && treeCount > 0 && sceneCount == 0;
        CanRemoveScenes = isAllLoaded == true && sceneCount > 0;
        CanLoadScenes = isAllLoaded == false && sceneCount > 0;
        CanUnloadScenes = isAllLoaded == true && sceneCount > 0;

        HaveUnloadedScenes = isAllLoaded == false && sceneCount > 0 && unloadedSceneCount > 0 && unloadedSceneCount < sceneCount;
    }

    public bool CanGetInfo()
    {
        return isAllLoaded == true && sceneCount == 0 && treeCount == 0;
    }

    public bool CanFindDoors()
    {
        return partCount > 0 && (isAllLoaded == true || sceneCount == 0 || treeCount == 0);
    }

    public bool CanOneKey()
    {
        return treeCount == 0 && sceneCount == 0;
    }

    public bool CanReset()
    {
        return sceneCount > 0 && treeCount > 0;
    }

    public override string ToString()
    {
        return $"loaded:{isAllLoaded},part:{partCount} tree:{treeCount},scene:{sceneCount},unloaded:{unloadedSceneCount}";
    }
}
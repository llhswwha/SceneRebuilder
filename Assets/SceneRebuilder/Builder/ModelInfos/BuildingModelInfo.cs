using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuildingModelInfo : SubSceneCreater
{

    public int GetTreeCount()
    {
        if (trees == null) return 0;
        int i = 0;
        foreach(var t in trees)
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
                    return $"Mod|S{SceneList.sceneCount}|T{tc}{p}";
                }
                else
                {
                    return $"Mod|T{tc}{p}";
                }
            }
            else
            {
                if (SceneList.sceneCount > 0)
                {
                    return $"Mod|S{SceneList.sceneCount}{p}";
                }
            }
        }
        else
        {
            if (tc > 0)
            {
                //return "Model(T)";
                return $"Mod|T{tc}{p}";
            }
        }
        
        return $"Mod{p}";
    }

    public GameObject InPart;

    public GameObject OutPart0;
    public GameObject OutPart1;

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    public float Out0BigVertextCount = 0;
    public float Out0SmallVertextCount = 0;
    public int Out0BigRendererCount = 0;
    public int Out0SmallRendererCount = 0;

    [ContextMenu("TestLoadAndSwitchToRenderers")]
    public void TestLoadAndSwitchToRenderers()
    {
       LoadAndSwitchToRenderers((p,r)=>{
           Debug.LogError($"TestLoadAndSwitchToRenderers progress:{p} isFinished:{r}");
       });
    }

    public void LoadAndSwitchToRenderers(Action<float,bool> finished)
    {
        DateTime start=DateTime.Now;
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        var nodes=new List<AreaTreeNode>();
        foreach (var tree in trees)
        {
            nodes.AddRange(tree.TreeLeafs);
        }
        var scenes=new List<SubScene_Base>();
        for(int i=0;i<nodes.Count;i++){
            var node=nodes[i];
            var rendererScene=node.GetRendererScene();
            if(rendererScene!=null){
                scenes.Add(rendererScene);
            }
        }
        SubSceneShowManager.Instance.LoadScenes(scenes,(p,r)=>{
            Debug.LogError($"SwitchToCombined2 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");

            if(r){
                for(int i=0;i<nodes.Count;i++){
                    var node=nodes[i];
                    node.SwitchToRenderers();
                }
            }
            if(finished!=null){
                finished(p,r);
            }

            Debug.LogError($"SwitchToCombined3 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");
        });
        Debug.LogError($"SwitchToCombined1 nodes:{nodes.Count} scenes:{scenes.Count}\t{(DateTime.Now-start).ToString()}");
    }

    
    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        DateTime start=DateTime.Now;
         Debug.Log("BuildingModelInfo.SwitchToCombined");
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach(var t in trees)
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
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach(var t in trees)
        {
            t.SwitchToRenderers();
        }

        Debug.LogWarning($"SwitchToRenderers \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        var oldTrees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach (var oldT in oldTrees)
        {
            GameObject.DestroyImmediate(oldT.gameObject);
        }
        this.ShowRenderers();
    }
    

    [ContextMenu("SaveTreeRendersId")]
    public override void SaveTreeRendersId()
    {
        Debug.Log("BuildingModelInfo.SaveTreeRendersId");
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach(var t in trees)
        {
            t.SaveRenderersId();
        }
    }

    [ContextMenu("LoadTreeRenderers")]
    public void LoadTreeRenderers()
    {
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        //IdDictionary.InitRenderers(t.gameObject);
        foreach (var t in trees)
        {
            t.LoadRenderers();
        }
    }

    public void LoadTreeRenderers(List<SubScene_Base> scenes)
    {
        //trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        ////IdDictionary.InitRenderers(t.gameObject);
        //foreach (var t in trees)
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
    public void GetTrees()
    {
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
    }

    //[ContextMenu("* CreateTrees")]
    public ModelAreaTree[] CreateTreesInner(Action<float> progressChanged)
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        ShowRenderers();
        return CreateTreesCore(progressChanged);
    }

    public ModelAreaTree[] CreateTreesInnerEx(bool isOut0BS, Action<float> progressChanged)
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        ShowRenderers();

        if (this.OutPart0 == null)
        {
            InitInOut(false);
        }

        ModelAreaTree[] ts = null;
        if (this.OutPart1 == null && this.InPart == null)
        {
            ts= CreateTrees_BigSmall_Core(progressChanged);
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

    public ModelAreaTree[] CreateTreesCore(Action<float> progressChanged)
    {
        if (OutPart0 == null)
        {
            Debug.LogWarning("CreateTreesCore OutPart0 == null");
        }

        if(InPart==null && OutPart0==null&& OutPart1 == null)
        {
            InitInOut(false);
        }
        

        List<ModelAreaTree> ts = new List<ModelAreaTree>();

        if (progressChanged != null)
        {
            progressChanged(0 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "InTree", 0 / 3f);
        }
        var tree1 = CreateTree(InPart, "InTree",p=> { 

        });
        if (tree1 != null)
        {
            ts.Add(tree1);
        }

        if (progressChanged != null)
        {
            progressChanged(1 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree0", 1 / 3f);
        }

        var tree2 = CreateTree(OutPart0, "OutTree0", p => {

        });
        if (tree2 != null)
        {
            ts.Add(tree2);
        }

        if (progressChanged != null)
        {
            progressChanged(2 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree1", 2 / 3f);
        }
        var tree3 = CreateTree(OutPart1, "OutTree1", p => {

        });
        if (tree3 != null)
        {
            ts.Add(tree3);
        }
        if (progressChanged != null)
        {
            progressChanged(3 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree1", 3 / 3f);
            ProgressBarHelper.ClearProgressBar();
        }

        return ts.ToArray();
    }

    public ModelAreaTree[] CreateTreesCoreBS(Action<float> progressChanged)
    {
        if (InPart == null && OutPart0 == null && OutPart1 == null)
        {
            InitInOut(false);
        }

        if (progressChanged != null)
        {
            progressChanged(0 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "InTree", 0 / 3f);
        }
        

        List<ModelAreaTree> ts = new List<ModelAreaTree>();
        var tree1 = CreateTree(InPart, "InTree", p => {
            //Debug.Log($"CreateTreesCoreBS subProgress:{p},progress:{(0 + p) / 3f}");
            if (progressChanged != null)
            {
                progressChanged((0 + p) / 3f);
            }
            else
            {
                ProgressBarHelper.DisplayProgressBar("CreateTree", $"InTree progress:{((0 + p) / 3f):P1}", (0 + p) / 3f);
            }
        });
        if (tree1 != null)
        {
            ts.Add(tree1);
        }

        //var tree2 = CreateTree(OutPart0, "OutTree0");
        //trees[1] = tree2;

        if (progressChanged != null)
        {
            progressChanged(1 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree0", 1 / 3f);
        }

        var tbs = CreateTrees_BigSmall_Core(p =>
         {
             //0,0.5,1
             if (progressChanged != null)
             {
                 progressChanged((1 + p) / 3f);
             }
             else
             {
                 ProgressBarHelper.DisplayProgressBar("CreateTree", $"OutTree0 progress:{((1 + p) / 3f):P1}", (1 + p) / 3f);
             }
         });
        if(tbs!=null)
            foreach (var t in tbs)
            {
                if (t != null)
                {
                    ts.Add(t);
                }
            }

        if (progressChanged != null)
        {
            progressChanged(2 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree1", 2 / 3f);
        }
        var tree3 = CreateTree(OutPart1, "OutTree1", p => {
            if (progressChanged != null)
            {
                progressChanged((1 + p) / 3f);
            }
            else
            {
                ProgressBarHelper.DisplayProgressBar("CreateTree", $"OutTree1 progress:{((2 + p) / 3f):P1}", (2 + p) / 3f);
            }
        });
        if (tree3 != null)
        {
            ts.Add(tree3);
        }

        if (progressChanged != null)
        {
            progressChanged(3 / 3f);
        }
        else
        {
            ProgressBarHelper.DisplayProgressBar("CreateTree", "OutTree1", 3 / 3f);
            ProgressBarHelper.ClearProgressBar();
        }
        
        return ts.ToArray();
    }

    [ContextMenu("* InitInOut")]
    public void InitInOut()
    {
        InitInOut(true);
    }

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



    public void InitInOut(bool isShowOut0Log)
    {
        Debug.Log("InitInOut:"+this.name);
        if (this.transform.childCount == 0)
        {
            Debug.LogWarning("InitInOut this.transform.childCount == 0");
            return;
        }
        UpackPrefab_One(this.gameObject);

        GetInOutParts();

        GetInOutVertextCount(isShowOut0Log);

        HideDetail();

        //var manager=GameObject.FindObjectOfType<>

        GetBigSmallInfo();


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

    private static AcRTAlignJobSetting JobSetting;

    private void GetBigSmallInfo()
    {
        if(OutPart0==null){
            Debug.LogError("GetSmallBigInfo OutPart0==null");
            return;
        }
        JobSetting =GameObject.FindObjectOfType<AcRTAlignJobSetting>(true);
        if (JobSetting == null)
        {
            Debug.LogError("GetSmallBigInfo JobSetting == null");
            return;
        }
        var meshFilters = OutPart0.GetComponentsInChildren<MeshFilter>(true);
        var info=PrefabInstanceBuilder.GetBigSmallRenderers(meshFilters, JobSetting.MaxModelLength);

        Out0BigRendererCount = info.bigModels.Count;
        Out0BigVertextCount = info.sumVertex_Big;
        Out0SmallRendererCount = info.smallModels.Count;
        Out0SmallVertextCount = info.sumVertex_Small;
        
    }

    private void GetInOutParts()
    {
        InPart=null;
        OutPart0=null;
        OutPart1=null;
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
            children.Add(child);
        }

        if (OutPart1 == null && OutPart0 == null && InPart == null)
        {
            Debug.Log("OutPart1 == null && OutPart0 == null && InPart == null");
            GameObject outRoot = InitPart("Out0");
            OutPart0 = outRoot;
            foreach (var child in children)
            {
                child.SetParent(outRoot.transform);
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
        DateTime start = DateTime.Now;

        InitInOut();

        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //trees = CreateTreesInnerEx(true,null);

        trees = CreateTreesInnerEx(true,p=>{
            ProgressBarHelper.DisplayProgressBar("CreateTreesBSEx",$"Progress {p:P1}",p);
        });
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"CreateTreesBSEx {(DateTime.Now - start).ToString()}");
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

    //[ContextMenu("* CreateTreesBS")]
    public void CreateTrees_BS()
    {

        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        CreateTrees_BigSmall_Core(null);
    }

    public ModelAreaTree[] CreateTrees_BigSmall_Core(Action<float> progressChanged)
    {
        if(this.OutPart0==null)
        {
            return null;
        }
        //trees = CreateTreesInner();
        Debug.Log("CreateTrees_BigSmall");
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
        {
            treeManager.Target = this.OutPart0;
            var ts= treeManager.CreateOne_BigSmall_Core(this.transform, this.OutPart0, progressChanged);
            foreach(var tree in ts)
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
        var renderers = this.GetComponentsInChildren<Renderer>(true) ;
        foreach(var render in renderers)
        {
            render.gameObject.SetActive(true);
            render.enabled = true;
        }
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

 

    private ModelAreaTree CreateTree(GameObject target,string treeName, Action<float> progressChanged)
    {
        if (target == null) return null;
        
        AreaTreeManager treeManager = AreaTreeHelper.InitCubePrefab();

        GameObject treeGo1 = new GameObject(this.name+"_"+treeName);
        treeGo1.transform.position = target.transform.position;
        treeGo1.transform.SetParent(this.transform);
        ModelAreaTree tree1 = treeGo1.AddComponent<ModelAreaTree>();
        tree1.Target = target;
        if (treeManager)
        {
            tree1.nodeSetting = treeManager.nodeSetting;
        }
            //tree1.GenerateTree();

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
        AllVertextCount = InVertextCount + Out0VertextCount + Out1VertextCount;
        AllRendererCount = InRendererCount + Out0RendererCount + Out1RendererCount;
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
        if(InPart)
            InPart.SetActive(false);
        if(OutPart1)
            OutPart1.SetActive(false);

        if(OutPart0)
            OutPart0.SetActive(true);
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        if(InPart)
            InPart.SetActive(true);
        if(OutPart1)
            OutPart1.SetActive(true);
        foreach(var tree in trees){
            if(tree==null)continue;
            tree.gameObject.SetActive(true);
        }
    }

    private void DestroyOldPartScenes(SceneContentType contentType)
    {
        var components = this.GetComponentsInChildren<SubScene_Base>(true);//In Out0 Out1
        foreach (var c in components)
        {
            if(c.contentType==contentType)
                GameObject.DestroyImmediate(c);//���´�������֮ǰ��ɾ��
        }
    }

    // [ContextMenu("ShowSceneBounds")]
    // public void ShowSceneBounds()
    // {
    //     var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
    //     foreach (var scene in scenes)
    //     {
    //         scene.ShowBounds();
    //     }
    // }

    // [ContextMenu("UnLoadScenes")]
    // public void UnLoadScenes()
    // {
    //     var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
    //     foreach(var scene in scenes)
    //     {
    //         scene.UnLoadGosM();
    //         scene.ShowBounds();
    //     }
    // }

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

    //[ContextMenu("DestroyScenes")]
    //public void DestroyScenes()
    //{
    //    var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
    //    foreach (var scene in scenes)
    //    {
    //        GameObject.DestroyImmediate(scene);
    //    }
    //}

    //[ContextMenu("DestroyModels")]
    //public void DestroyModels()
    //{
    //    var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
    //    foreach (var scene in scenes)
    //    {
    //        scene.UnLoadGosM();
    //    }
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

    public void OneKey_TreePartScene(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;
        if (progressChanged != null)
        {
            progressChanged(0 / 3f);
        }
        InitInOut();

        ShowDetail();

        if (progressChanged != null)
        {
            progressChanged(1 / 3f);
        }
        CreateTreesInnerEx(true, subP =>
        {
            if (progressChanged != null)
            {
                progressChanged((1 + subP) / 3f);
            }
        });
        if (progressChanged != null)
        {
            progressChanged(2 / 3f);
        }
        EditorCreateNodeScenes();
        if (progressChanged != null)
        {
            progressChanged(3 / 3f);
        }
        Debug.LogError($"BuildingModelInfo.OneKey_TreePart Time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("* OneKey_TreeNodeScene")]
    public void OneKey_TreeNodeScene()
    {
        OneKey_TreeNodeScene(null);
        ProgressBarHelper.ClearProgressBar();
    }

    public void OneKey_TreeNodeScene(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;
        if (progressChanged != null)
        {
            progressChanged(0 / 3f);
        }
        InitInOut();
        if (progressChanged != null)
        {
            progressChanged(1 / 3f);
        }
        CreateTreesBSEx();
        //CreateTreesInnerEx(true, null);
        if (progressChanged != null)
        {
            progressChanged(2 / 3f);
        }
        //EditorCreateScenes_TreeWithPart((subP, i, c) =>
        //{
        //    if (progressChanged != null)
        //    {
        //        progressChanged((2 + subP) / 3f);
        //    }
        //});
        EditorCreateNodeScenes();
        if (progressChanged != null)
        {
            progressChanged(3 / 3f);
        }
        Debug.LogError($"BuildingModelInfo.OneKey_TreeNodeScene Time:{(DateTime.Now - start).ToString()}");
    }

    public SceneContentType contentType;

    #region CreateScenes

    public void EditorCreateScenesEx(SceneContentType contentType, Action<float,int,int> progressChanged)
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

        SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes time:{(DateTime.Now - start)}");
    }

    //[ContextMenu("* EditorCreateScenes_TreeWithPart")]
    public void EditorCreateScenes_TreeWithPart()
    {
        EditorCreateScenes_TreeWithPart(null);
    }

    public void EditorCreateScenes_TreeWithPart(Action<float,int,int> progressChanged)
    {
        DateTime start = DateTime.Now;

        SaveTreeRendersId();

        DestroyScenes();

        InitInOut(false);

        List<SubScene_Base> scenes = new List<SubScene_Base>();
        scenes.AddRange(CreatePartScene(SceneContentType.Tree));
        scenes.AddRange(CreatePartScene(SceneContentType.Part));
        EditorCreateScenes(scenes, progressChanged);


        SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes_TreeWithPart time:{(DateTime.Now - start)},progressChanged:{progressChanged}");
    }

   

    public List<SubScene_Base> CreatePartScene(SceneContentType contentType)
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        AreaTreeHelper.InitCubePrefab();
        if (contentType == SceneContentType.Single)
        {
            var scene=SubSceneHelper.EditorCreateScene<SubScene_Single>(this.gameObject, SceneContentType.Single, false,"");
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

    internal List<SubScene_Base> EditorCreatePartScenesEx(string dir, bool isOverride, SceneContentType contentType)
    {
        //DestroyOldBounds();
        DestroyOldPartScenes(contentType);

        List<SubScene_Base> senes = new List<SubScene_Base>();
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
        if (InPart)
        {
            var scene = CreatePartSceneEx(InPart, contentType, "_In_" + contentType, trees, dir, isOverride, gameObject.AddComponent<SubScene_In>());
            senes.Add(scene);
        }
        if (OutPart0)
        {
            var scene = CreatePartSceneEx(OutPart0, contentType, "_Out0_" + contentType, trees, dir, isOverride, gameObject.AddComponent<SubScene_Out0>());
            senes.Add(scene);
        }
        if (OutPart1)
        {
            var scene = CreatePartSceneEx(OutPart1, contentType, "_Out1_" + contentType, trees, dir, isOverride, gameObject.AddComponent<SubScene_Out1>());
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

            if (trees != null && (contentType == SceneContentType.Tree || contentType == SceneContentType.TreeAndPart))
                foreach (var tree in trees)
                {
                    if (tree == null) continue;

                    if (tree.Target == go)
                    {
                        gos.Add(tree.gameObject);
                    }
                }

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
    public SubScene_Base CreatePartScene(GameObject go, string nameAf, ModelAreaTree[] trees, string path, bool isOverride, SubScene_Base ss)
    {
        if (go)
        {
            List<GameObject> gos = new List<GameObject>();
            gos.Add(go);

            if (trees != null)
                foreach (var tree in trees)
                {
                    if (tree == null) continue;

                    if (tree.Target == go)
                    {
                        gos.Add(tree.gameObject);
                    }
                }

            ss.contentType = SceneContentType.TreeAndPart;
            string scenePath = $"{path}{this.name}{nameAf}.unity";
            ss.SetArg(scenePath, isOverride,gos);
            ss.Init();
            //ss.SaveScene();
            //ss.ShowBounds();
            return ss;
        }
        return null;
    }
    
    #endregion



    [ContextMenu("* EditorLoadScenes")]
    private void EditorLoadScenes()
    {
        EditorLoadScenes(null);
    }


    public void EditorLoadScenesByContentType(SceneContentType contentType, Action<float> progressChanged)
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

    private void EditorLoadScenes(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;

        //EditorLoadScenes(contentType);

        var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { contentType });
        EditorLoadScenes(scenes.ToArray(), progressChanged);

        this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
        LoadTreeRenderers();

        Debug.LogError($"EditorLoadScenes time:{(DateTime.Now - start)}");
    }

    private void EditorLoadScenes_TreeWithPart(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;

        var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { SceneContentType.Part, SceneContentType.Tree });

        EditorLoadScenes(scenes.ToArray(), progressChanged);
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
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
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
    }

    public void EditorCreateNodeScenes(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;
        ShowDetail();
        for (int i = 0; i < trees.Length; i++)
        {
            var tree = trees[i];
            if (tree == null) continue;
            float progress = (float)i / trees.Length;
            float percents = progress * 100;
            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorCreateNodeScenes", $"Progress1 {i}/{trees.Length} {percents:F2}%", progress))
                {
                    break;
                }
            }
            else
            {
                progressChanged(progress);
            }

            tree.EditorCreateNodeScenes(p =>
            {
                float progress2 = (float)(i + p) / trees.Length;
                float percents2 = progress2 * 100;
                //ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorCreateNodeScenes", $"Progress2 {(i + p):F2}/{trees.Length} {percents2:F2}%", progress2);

                if (progressChanged == null)
                {
                    ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorCreateNodeScenes", $"Progress2 {(i + p):F2}/{trees.Length} {percents2:F2}%", progress2);
                }
                else
                {
                    progressChanged(progress2);
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
            progressChanged(1);
        }

        if (progressChanged == null) Debug.LogError($"BuildingModelInfo.EditorCreateNodeScenes time:{(DateTime.Now - start)}");
    }

    [ContextMenu("* EditorLoadNodeScenes")]
    public void EditorLoadNodeScenes()
    {
        EditorLoadNodeScenes(null);
    }

    public void EditorLoadNodeScenesEx()
    {
        IdDictionary.InitInfos();
        EditorLoadNodeScenes(null);
    }

    //[ContextMenu("* EditorLoadNodeScenes")]
    public void EditorLoadNodeScenes(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;
        IdDictionary.InitInfos();
        for (int i = 0; i < trees.Length; i++)
        {
            var tree = trees[i];
            if (tree == null) continue;
            float progress = (float)i / trees.Length;
            float percents = progress * 100;

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorLoadNodeScenes", $"Progress1 {i}/{trees.Length} {percents:F2}%", progress))
                {
                    break;
                }
            }
            else
            {
                progressChanged(progress);
            }


            tree.EditorLoadNodeScenes(p =>
            {
                float progress2 = (float)(i + p) / trees.Length;
                float percents2 = progress2 * 100;

                if (progressChanged == null)
                {
                    ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorLoadNodeScenes", $"Progress2 {(i + p):F2}/{trees.Length} {percents2:F2}%", progress2);
                }
                else
                {
                    progressChanged(progress2);
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
            progressChanged(1);
        }

        Debug.LogWarning($"BuildingModelInfo.EditorLoadNodeScenes time:{(DateTime.Now - start)}");
    }
#endif
}

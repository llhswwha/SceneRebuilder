using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuildingModelInfo : MonoBehaviour
{
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

    private void ClearTrees()
    {
        var oldTrees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach (var oldT in oldTrees)
        {
            GameObject.DestroyImmediate(oldT.gameObject);
        }
    }

    //[ContextMenu("* CreateTrees")]
    public ModelAreaTree[] CreateTreesInner()
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        ShowRenderers();
        return CreateTreesCore();
    }

    public ModelAreaTree[] CreateTreesInnerEx(bool isOut0BS)
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        ShowRenderers();

        if (this.OutPart0 == null)
        {
            InitInOut();
        }

        if (this.OutPart1 == null && this.InPart == null)
        {
            return CreateTrees_BigSmall_Core();//没有In的状态下直接把Out0分成Small和Big
        }
        else
        {
            if (isOut0BS == false)
            {
                return CreateTreesCore();
            }
            else
            {
                return CreateTreesCoreBS();
            }
        }

        //return null;
    }

    public ModelAreaTree[] CreateTreesInnerBS()
    {
        ClearTrees();

        UpackPrefab_One(this.gameObject);

        ShowRenderers();
        return CreateTreesCoreBS();
    }

    public ModelAreaTree[] CreateTreesCore()
    {
        if (OutPart0 == null)
        {
            Debug.LogWarning("CreateTreesCore OutPart0 == null");
        }

        if(InPart==null && OutPart0==null&& OutPart1 == null)
        {
            InitInOut();
        }
        

        ModelAreaTree[] trees = new ModelAreaTree[3];
        var tree1 = CreateTree(InPart, "InTree");
        trees[0] = tree1;
        var tree2 = CreateTree(OutPart0, "OutTree0");
        trees[1] = tree2;
        var tree3 = CreateTree(OutPart1, "OutTree1");
        trees[2] = tree3;
        return trees;
    }

    public ModelAreaTree[] CreateTreesCoreBS()
    {
        if (InPart == null && OutPart0 == null && OutPart1 == null)
        {
            InitInOut();
        }

        ModelAreaTree[] trees = new ModelAreaTree[3];
        var tree1 = CreateTree(InPart, "InTree");
        trees[0] = tree1;

        //var tree2 = CreateTree(OutPart0, "OutTree0");
        //trees[1] = tree2;

        CreateTrees_BigSmall_Core();

        var tree3 = CreateTree(OutPart1, "OutTree1");
        trees[2] = tree3;
        return trees;
    }

    [ContextMenu("* InitInOut")]
    public void InitInOut()
    {
        UpackPrefab_One(this.gameObject);

        GetInOutParts();

        GetInOutVertextCount();

        HideDetail();

        //var manager=GameObject.FindObjectOfType<>
    }

    private void GetInOutParts()
    {
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
            GameObject outRoot = InitPart("Out0");
            OutPart0 = outRoot;
            foreach (var child in children)
            {
                child.SetParent(outRoot.transform);
            }
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


    [ContextMenu("* CreateTreesEx")]
    public void CreateTreesEx()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        var trees = CreateTreesInnerEx(false);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTreesBSEx")]
    public void CreateTreesBSEx()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        var trees = CreateTreesInnerEx(true);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTrees")]
    public void CreateTrees()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        var trees = CreateTreesInner();
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTreesBS")]
    public void CreateTrees_BS()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        CreateTrees_BigSmall_Core();
    }

    public ModelAreaTree[] CreateTrees_BigSmall_Core()
    {
        if(this.OutPart0==null)
        {
            return null;
        }
        //var trees = CreateTreesInner();
        Debug.Log("CreateTrees_BigSmall");
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
        {
            treeManager.Target = this.OutPart0;
            var trees= treeManager.CreateOne_BigSmall_Core(this.transform, this.OutPart0);
            foreach(var tree in trees)
            {
                if (tree == null) continue;
                tree.name = this.name + "_" + tree.name;
            }
            return trees;
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

    [ContextMenu("ShowAll")]
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

 

    private ModelAreaTree CreateTree(GameObject target,string treeName)
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
            //tree1.GenerateTree();//没有合并

        tree1.GenerateMesh();//合并

        treeGo1.SetActive(target.activeInHierarchy);//该隐藏的继续隐藏

        tree1.IsHidden = !target.activeInHierarchy;//动态隐藏
        return tree1;
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

    private void GetInOutVertextCount()
    {
        InVertextCount = GetChildrenVertextCount(InPart, out InRendererCount,false);
        Out0VertextCount = GetChildrenVertextCount(OutPart0,out Out0RendererCount, true);
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
        logs.Sort((a,b)=>a.count.CompareTo(b.count));

        for (int i = 0; i < logs.Count; i++)
        {
            Debug.LogError($"GetChildrenVertextCount[{i}]    {logs[i]}");
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

    [ContextMenu("FindInGos")]
    public void FindInGos()
    {

    }

    [ContextMenu("HideIn")]
    public void HideIn()
    {
        if (InPart)
            InPart.SetActive(false);
    }

    [ContextMenu("ShowIn")]
    public void ShowIn()
    {
        if (InPart)
            InPart.SetActive(true);
    }

    public bool IsDetailVisible = false;

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        IsDetailVisible = false;
        if(InPart)
            InPart.SetActive(false);
        if(OutPart1)
            OutPart1.SetActive(false);
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        if(InPart)
            InPart.SetActive(true);
        if(OutPart1)
            OutPart1.SetActive(true);
    }

    private void DestroyOldPartScenes()
    {
        var components = this.GetComponentsInChildren<SubScene_Part>();//In Out0 Out1
        foreach (var c in components)
        {
            GameObject.DestroyImmediate(c);//重新创建，把之前的删除
        }
    }

    //public void DestroyOldBounds()
    //{
    //    var components = this.GetComponentsInChildren<BoundsBox>();//In Out0 Out1
    //    foreach (var c in components)
    //    {
    //        if (c == null) continue;
    //        GameObject.DestroyImmediate(c.gameObject);//重新创建，把之前的删除
    //    }
    //}

    //public string SceneState = "";

    [ContextMenu("LoadScenes_InOut")]
    public void LoadScenes_InOut()
    {
        var singleScene = gameObject.GetComponent<SubScene_Single>();
        if (singleScene) singleScene.UnLoadGosM();

        var partScenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.LoadScenesEx(partScenes);

        //SceneState = "LoadScenes_InOut";
    }

    private void UnloadPartScenes()
    {
        var partScenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        foreach (var scene in partScenes)
        {
            scene.UnLoadGosM();
            scene.DestroyBoundsBox();
        }
    }

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        //UnloadPartScenes();

        var scenes = gameObject.GetComponentsInChildren<SubScene_Single>();
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.LoadScenesEx(scenes);
    }

#if UNITY_EDITOR

    [ContextMenu("EditorRemoveScene")]
    public void EditorRemoveScene()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Single>();
        foreach (var scene in scenes)
        {
            GameObject.DestroyImmediate(scene);
        }
    }

    public static GameObject CreateEmptySceneGo(string sceneName,Transform t)
    {
        GameObject goScene = new GameObject($"{sceneName}_Scene");
        int index = t.GetSiblingIndex();
        //Debug.Log("index:"+index);
        //goScene.transform.SetSiblingIndex(index);
        goScene.transform.position = t.position;
        goScene.transform.rotation = t.rotation;
        goScene.transform.parent = t.parent;
        goScene.transform.localScale = t.localScale;
        return goScene;
    }

    [ContextMenu("EditorCreateSceneGO")]
    public void EditorCreateSceneGO()
    {
        string sceneName = this.name;

        GameObject goScene = CreateEmptySceneGo(sceneName, this.transform);
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        SubScene_Single ss = goScene.AddComponent<SubScene_Single>();

        ss.gos = SubSceneHelper.GetChildrenGos(this.transform);
        ss.Init();

        string path= subSceneManager.GetScenePath(sceneName, false);
        ss.SetPath(path);

        SubSceneHelper.SaveChildrenToScene(path, this.transform, subSceneManager.IsOverride);

        ss.ShowBounds();
    }

    [ContextMenu("EditorCreateScene")]
    public void EditorCreateScene()
    {
        GameObject go = this.gameObject;

        SubScene_Single ss1 = go.GetComponent<SubScene_Single>();
        if (ss1 != null)
        {
            Debug.LogWarning("已经存在SubScene_Single，调用EditorSaveScenes，保存场景");
            EditorSaveScenes();
            return;
        }

        UpackPrefab_One(go);

        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();

        SubScene_Single ss = go.AddComponent<SubScene_Single>();
        ss.Init();
        string path = subSceneManager.GetScenePath(go.name, false);
        SubSceneHelper.SaveChildrenToScene(path, this.transform, subSceneManager.IsOverride);
        ss.ShowBounds();
    }

    [ContextMenu("EditorCreatePartScenes")]
    private void EditorCreatePartScenes()
    {
        AreaTreeHelper.InitCubePrefab();



        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        string dir = subSceneManager.GetSceneDir(true);
        this.EditorCreatePartScenes(dir, subSceneManager.IsOverride);


        //SceneState = "SaveScenes_InOut";

        //DestroyOldBounds();
    }

    [ContextMenu("EditorLoadScenes_InOut")]
    public void EditorLoadScenes_InOut()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        foreach (var scene in scenes)
        {
            scene.EditorLoadScene();
        }

        this.InitInOut();

        //SceneState = "EditLoadScenes_InOut";
    }
    [ContextMenu("EditorSaveScenes_InOut")]
    public void EditorSaveScenes_InOut()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        foreach (var scene in scenes)
        {
            scene.EditorSaveScene();
        }

        //SceneState = "EditSaveScenes_InOut";
    }
    [ContextMenu("EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        UnloadPartScenes();

        var scenes = gameObject.GetComponentsInChildren<SubScene_Single>();
        foreach(var scene in scenes)
        {
            scene.EditorLoadScene();
        }

        this.InitInOut();

        //SceneState = "EditLoadScenes";
    }
    [ContextMenu("EditorSaveScenes")]
    public void EditorSaveScenes()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Single>();
        foreach (var scene in scenes)
        {
            scene.IsLoaded = true;
            scene.EditorSaveScene();
        }

        //SceneState = "EditSaveScenes";
    }


 


    internal void EditorCreatePartScenes(string dir, bool isOverride)
    {
        //DestroyOldBounds();
        DestroyOldPartScenes();//重新创建，把之前的删除

        //SubScene_Single subScene_Single = this.GetComponent<SubScene_Single>();
        //if (subScene_Single)
        //{
        //    subScene_Single.EditorLoadScene();
        //}

        InitInOut();

        var trees = this.GetComponentsInChildren<ModelAreaTree>(true);

        if (InPart)
            CreatePartScene(InPart, "_In", trees, dir, isOverride, gameObject.AddComponent<SubScene_In>());
        if (OutPart0)
            CreatePartScene(OutPart0, "_Out0", trees, dir, isOverride, gameObject.AddComponent<SubScene_Out0>());
        if (OutPart1)
            CreatePartScene(OutPart1, "_Out1", trees, dir, isOverride, gameObject.AddComponent<SubScene_Out1>());


        //if (subScene_Single)
        //{
        //    subScene_Single.IsLoaded = false;
        //}

        //SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //subSceneManager.ClearOtherScenes();
    }

    public SubScene_Base CreatePartScene(GameObject go,string nameAf, ModelAreaTree[] trees,string dir,bool isOverride, SubScene_Base ss)
    {
        if (go)
        {
            List<GameObject> gos = new List<GameObject>();
            gos.Add(go);
            foreach (var tree in trees)
            {
                if (tree == null) continue;
                
                if (tree.Target == go)
                {
                    gos.Add(tree.gameObject);
                }
            }

            //SubScene_Single ss = gameObject.AddComponent<SubScene_Single>();
            //ss.sceneType = sceneType;
            ss.gos = gos;
            ss.Init();
            string path = $"{dir}{this.name}{nameAf}.unity";
            ss.SaveScene(path, isOverride);
            ss.ShowBounds();
            return ss;
        }
        return null;
    }

#endif
}

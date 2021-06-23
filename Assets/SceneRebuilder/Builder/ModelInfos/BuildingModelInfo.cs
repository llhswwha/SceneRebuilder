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
    public string GetInfoName()
    {
        var tc = GetTreeCount();
        if (SceneList != null)
        {
            
            if (tc > 0)
            {
                //return "Model(S)(T)";
                if (SceneList.sceneCount > 0)
                {
                    return $"Model(S{SceneList.sceneCount})(T{tc})";
                }
                else
                {
                    return $"Model(T{tc})";
                }
            }
            else
            {
                if (SceneList.sceneCount > 0)
                {
                    return $"Model(S{SceneList.sceneCount})";
                }
            }
        }
        else
        {
            if (tc > 0)
            {
                //return "Model(T)";
                return $"Model(T{tc})";
            }
        }
        
        return "Model";
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

    private void ClearTrees()
    {
        var oldTrees = this.GetComponentsInChildren<ModelAreaTree>(true);
        foreach (var oldT in oldTrees)
        {
            GameObject.DestroyImmediate(oldT.gameObject);
        }
    }

    [ContextMenu("SaveTreeRendersId")]
    public void SaveTreeRendersId()
    {
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
        foreach (var t in trees)
        {
            t.LoadRenderers();
        }
    }

    [ContextMenu("GetTrees")]
    public void GetTrees()
    {
        trees = this.GetComponentsInChildren<ModelAreaTree>(true);
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
            InitInOut(false);
        }

        ModelAreaTree[] ts = null;
        if (this.OutPart1 == null && this.InPart == null)
        {
            ts= CreateTrees_BigSmall_Core();//没有In的状态下直接把Out0分成Small和Big
        }
        else
        {
            if (isOut0BS == false)
            {
                ts= CreateTreesCore();
            }
            else
            {
                ts= CreateTreesCoreBS();
            }
        }
        trees = ts;

        return ts;
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
            InitInOut(false);
        }
        

        List<ModelAreaTree> ts = new List<ModelAreaTree>();
        var tree1 = CreateTree(InPart, "InTree");
        if (tree1 != null)
        {
            ts.Add(tree1);
        }
        var tree2 = CreateTree(OutPart0, "OutTree0");
        if (tree2 != null)
        {
            ts.Add(tree2);
        }
        var tree3 = CreateTree(OutPart1, "OutTree1");
        if (tree3 != null)
        {
            ts.Add(tree3);
        }
        return ts.ToArray();
    }

    public ModelAreaTree[] CreateTreesCoreBS()
    {
        if (InPart == null && OutPart0 == null && OutPart1 == null)
        {
            InitInOut(false);
        }

        List<ModelAreaTree> ts = new List<ModelAreaTree>();
        var tree1 = CreateTree(InPart, "InTree");
        if (tree1 != null)
        {
            ts.Add(tree1);
        }

        //var tree2 = CreateTree(OutPart0, "OutTree0");
        //trees[1] = tree2;

       var tbs= CreateTrees_BigSmall_Core();
        foreach(var t in tbs)
        {
            if (t != null)
            {
                ts.Add(t);
            }
        }

        var tree3 = CreateTree(OutPart1, "OutTree1");
        if (tree3 != null)
        {
            ts.Add(tree3);
        }
        return ts.ToArray();
    }

    [ContextMenu("* InitInOut")]
    private void InitInOut()
    {
        InitInOut(true);
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
            Debug.Log("OutPart1 == null && OutPart0 == null && InPart == null");
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


    public ModelAreaTree[] trees;

    [ContextMenu("* CreateTreesEx")]
    public void CreateTreesEx()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        trees = CreateTreesInnerEx(false);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTreesBSEx")]
    public void CreateTreesBSEx()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        trees = CreateTreesInnerEx(true);
        if (treeManager)
        {
            treeManager.AddTrees(trees);
        }
    }

    [ContextMenu("* CreateTrees")]
    public void CreateTrees()
    {
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        trees = CreateTreesInner();
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
        //trees = CreateTreesInner();
        Debug.Log("CreateTrees_BigSmall");
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
        {
            treeManager.Target = this.OutPart0;
            var ts= treeManager.CreateOne_BigSmall_Core(this.transform, this.OutPart0);
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

    private void DestroyOldPartScenes(SceneContentType contentType)
    {
        var components = this.GetComponentsInChildren<SubScene_Base>();//In Out0 Out1
        foreach (var c in components)
        {
            if(c.contentType==contentType)
                GameObject.DestroyImmediate(c);//重新创建，把之前的删除
        }
    }

    [ContextMenu("DestroyOldPartScenes")]
    private void DestroyOldPartScenes()
    {
        //Debug.Log("DestroyOldPartScenes");
        if (SceneList == null)
        {
            SceneList = this.GetComponentInChildren<SubScene_List>();
        }
        if (SceneList != null)
        {
            SceneList.Clear();
        }
        else
        {
            var components = this.GetComponentsInChildren<SubScene_Base>();//In Out0 Out1
            foreach (var c in components)
            {
                //if (c.contentType == contentType)
                GameObject.DestroyImmediate(c);//重新创建，把之前的删除
            }
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

    [ContextMenu("LoadScenes_Part")]
    public void LoadScenes_Part()
    {
        var singleScene = gameObject.GetComponent<SubScene_Single>();
        if (singleScene) singleScene.UnLoadGosM();

        var partScenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.LoadScenesEx(partScenes);

        //SceneState = "LoadScenes_Part";
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

    [ContextMenu("DestroyScenes")]
    public void DestroyScenes()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        foreach (var scene in scenes)
        {
            GameObject.DestroyImmediate(scene);
        }
    }

    [ContextMenu("DestroyModels")]
    public void DestroyModels()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        foreach (var scene in scenes)
        {
            scene.UnLoadGosM();
        }
    }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        foreach (var scene in scenes)
        {
            scene.ShowBounds();
        }
    }

#if UNITY_EDITOR

    public SubScene_List SceneList = null;

    private void InitSceneListGO()
    {
        if (SceneList == null)
        {
            GameObject go = new GameObject("SubScenes");
            go.transform.position = this.transform.position;
            go.transform.SetParent(this.transform);
            SceneList = go.AddComponent<SubScene_List>();
            SceneList.Init();
        }
    }
    /// <summary>
    /// 专门创建一个SceneList分别显示SubScene的信息
    /// </summary>
    [ContextMenu("EditorMoveScenes")]
    public void EditorMoveScenes()
    {
        InitSceneListGO();

        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        foreach (var scene in scenes)
        {
            SceneList.AddScene(scene);
            GameObject.DestroyImmediate(scene);
        }
    }

    public SceneContentType contentType;

    #region CreateScenes
    public static GameObject CreateEmptySceneGo(string sceneName, Transform t)
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

    //private T AddSubSceneComponent<T>() where T : SubScene_Base
    //{
    //    InitSceneListGO();

    //    GameObject subSceneGo = new GameObject(scene.GetSceneNameEx());
    //    subSceneGo.transform.position = this.transform.position;
    //    subSceneGo.transform.SetParent(SceneListGo.transform);
    //}

    [ContextMenu("EditorCreateSceneGO")]
    public void EditorCreateSceneGO()
    {
        string sceneName = this.name;

        GameObject goScene = CreateEmptySceneGo(sceneName, this.transform);
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        SubScene_Single ss = goScene.AddComponent<SubScene_Single>();

        ss.gos = SubSceneHelper.GetChildrenGos(this.transform);
        ss.Init();

        string path = subSceneManager.GetScenePath(sceneName, SceneContentType.Single);
        ss.SetPath(path);

        SubSceneHelper.SaveChildrenToScene(path, this.transform, subSceneManager.IsOverride);

        ss.ShowBounds();
    }

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

        DestroyOldPartScenes();

        InitInOut(false);
        var scenes=CreatePartScene(contentType);
        EditorCreateScenes(scenes,null);

        SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes time:{(DateTime.Now - start)}");
    }

    [ContextMenu("* EditorCreateScenes_TreeWithPart")]
    private void EditorCreateScenes_TreeWithPart()
    {
        EditorCreateScenes_TreeWithPart(null);
    }

    public void EditorCreateScenes_TreeWithPart(Action<float,int,int> progressChanged)
    {
        DateTime start = DateTime.Now;

        SaveTreeRendersId();

        DestroyOldPartScenes();

        InitInOut(false);

        List<SubScene_Base> scenes = new List<SubScene_Base>();
        scenes.AddRange(CreatePartScene(SceneContentType.Tree));//这里会保存RendererId的关联关系
        scenes.AddRange(CreatePartScene(SceneContentType.Part));
        EditorCreateScenes(scenes, progressChanged);

        LoadTreeRenderers();//关联回Tree中的Renderers

        SubSceneManager.Instance.ClearOtherScenes();
        EditorMoveScenes();

        Debug.LogError($"EditorCreateScenes_TreeWithPart time:{(DateTime.Now - start)},progressChanged:{progressChanged}");
    }

    public void EditorCreateScenes(List<SubScene_Base> scenes, Action<float,int,int> progressChanged)
    {
        int count = scenes.Count;
        Debug.Log("EditorCreateScenes:" + count);
        for (int i = 0; i < count; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.IsLoaded = true;
            scene.SaveScene();
            scene.ShowBounds();

            float progress = (float)i / count;
            float percents = progress * 100;
            if (progressChanged != null)
            {
                progressChanged(progress,i,count);
            }
            else
            {
                Debug.Log($"EditorCreateScenes progress:{progress:F2},percents:{percents:F2}");
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateScenes", $"{i}/{count} {percents:F2}% of 100%", progress))
                {
                    break;
                }
            }
            //System.Threading.Thread.Sleep(1000);
        }
        if(progressChanged==null)
            ProgressBarHelper.ClearProgressBar();
    }

    public List<SubScene_Base> CreatePartScene(SceneContentType contentType)
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        AreaTreeHelper.InitCubePrefab();
        if (contentType == SceneContentType.Single)
        {
            var scene=SubSceneHelper.EditorCreateScene(this.gameObject,false);
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
        DestroyOldPartScenes(contentType);//重新创建，把之前的删除
        //InitInOut(false);//放到外面去

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

            ss.gos = gos;
            ss.Init();
            string scenePath = $"{path}{this.name}{nameAf}.unity";
            ss.SetArg(scenePath, isOverride);
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
            ss.gos = gos;
            ss.Init();
            string scenePath = $"{path}{this.name}{nameAf}.unity";
            ss.SetArg(scenePath, isOverride);
            //ss.SaveScene();
            //ss.ShowBounds();
            return ss;
        }
        return null;
    }

    internal void EditorCreatePartScenes(string dir, bool isOverride)
    {
        //DestroyOldBounds();
        DestroyOldPartScenes();//重新创建，把之前的删除

        InitInOut(false);

        trees = this.GetComponentsInChildren<ModelAreaTree>(true);

        if (InPart)
        {
            CreatePartScene(InPart, "_In", trees, dir, isOverride, gameObject.AddComponent<SubScene_In>());
        }
        if (OutPart0)
        {
            CreatePartScene(OutPart0, "_Out0", trees, dir, isOverride, gameObject.AddComponent<SubScene_Out0>());
        }
        if (OutPart1)
        {
            CreatePartScene(OutPart1, "_Out1", trees, dir, isOverride, gameObject.AddComponent<SubScene_Out1>());
        }
    }

    
    #endregion

    public void EditorLoadScenesEx(SceneContentType contentType)
    {
        this.contentType = contentType;
        if (contentType == SceneContentType.TreeWithPart)
        {
            EditorLoadScenes_TreeWithPart();
        }
        else
        {
            EditorLoadScenes();
        }
    }

    [ContextMenu("EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        DateTime start = DateTime.Now;

        //EditorLoadScenes(contentType);

        var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { contentType });//按照实际使用中也是先呈现Tree，再按需加载Part的
        EditorLoadScenes(scenes.ToArray());

        this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
        LoadTreeRenderers();

        Debug.LogError($"EditorLoadScenes time:{(DateTime.Now - start)}");
    }

    [ContextMenu("EditorLoadScenes_TreeWithPart")]
    public void EditorLoadScenes_TreeWithPart()
    {
        DateTime start = DateTime.Now;

        //EditorLoadScenes(SceneContentType.Tree);
        //EditorLoadScenes(SceneContentType.Part);

        var scenes=GetSubScenesOfTypes(new List<SceneContentType>() { SceneContentType.Tree, SceneContentType.Part });//按照实际使用中也是先呈现Tree，再按需加载Part的
        EditorLoadScenes(scenes.ToArray());
        LoadTreeRenderers();
        InitInOut();//这个是和Part有关的。

        Debug.LogError($"EditorLoadScenes_TreeWithPart time:{(DateTime.Now - start)}");
    }

    private List<SubScene_Base> GetSubScenesOfTypes(List<SceneContentType> types)
    {
        List<SubScene_Base> list = new List<SubScene_Base>();
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            if (types.Contains(scene.contentType))
            {
                list.Add(scene);
            }
        }
        return list;
    }

    public void EditorLoadScenes(SceneContentType ct)
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        EditorLoadScenes(scenes);
    }

    public void EditorLoadScenes(SubScene_Base[] scenes)
    {
        Debug.Log("EditorLoadScenes:"+scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.IsLoaded = false;
            scene.EditorLoadScene();

            float progress = (float)i / scenes.Length;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"{i}/{scenes.Length} {percents:F2}% of 100%", progress))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        //this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
    }

    [ContextMenu("EditorSaveScenes_Part")]
    public void EditorSaveScenes_Part()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Part>();
        foreach (var scene in scenes)
        {
            scene.EditorSaveScene();
        }

        //SceneState = "EditSaveScenes_Part";
    }

    //[ContextMenu("EditorLoadScenes")]
    //public void EditorLoadScenes()
    //{
    //    UnloadPartScenes();

    //    var scenes = gameObject.GetComponentsInChildren<SubScene_Single>();
    //    foreach(var scene in scenes)
    //    {
    //        scene.EditorLoadScene();
    //    }

    //    this.InitInOut();

    //    //SceneState = "EditLoadScenes";
    //}

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

 

#endif
}

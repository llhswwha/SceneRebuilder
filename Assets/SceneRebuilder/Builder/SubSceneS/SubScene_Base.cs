using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SubScene_Base : MonoBehaviour
{
    public SubScene_Base LinkedScene;

    public SubSceneArg sceneArg;

    public Transform sceneParent;

    public Transform GetSceneParent()
    {
        if (sceneParent != null)
        {
            return sceneParent;
        }
        return this.transform;
    }

    public bool IsLoaded = false;

    public bool GetIsLoaded()
    {
        return IsLoaded;
    }

    public List<GameObject> gos
    {
        get
        {
            if (sceneArg == null)
            {
                return new List<GameObject>();
            }
            else
            {
                return sceneArg.objs;
            }
        }
    }

    public bool HaveGos()
    {
        bool r = false;
        if(gos==null)return false;
        foreach(var g in gos)
        {
            if (g != null)
            {
                r = true;
            }
        }
        return r;
    }

    public bool IsLoading = false;

    //public SubSceneType sceneType;

    public SceneContentType contentType;

    public string sceneName = "";

    //public string scenePath;

    public void SetPath(string path)
    {
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }
        sceneArg.path = path;
        GetSceneName();
    }

    public Scene scene;

    public Bounds bounds;

    public Vector3 center;

    public float radius;

    //public List<GameObject> gos = new List<GameObject>();

    public void SetObjects(List<GameObject> goList)
    {
        //gos = goList;
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }

        if (sceneArg != null)
        {
            sceneArg.objs = goList;
        }

        //SetRendererParent();
    }

    public int childCount;

    public int rendererCount;

    public float vertexCount = 0;


    public bool IsSetParent = true;

    public bool IsAutoLoad = false;

    public event Action<float,SubScene_Base> ProgressChanged;

    protected void OnProgressChanged(float progress)
    {
        this.loadProgress = progress;
        if (ProgressChanged != null)
        {
            ProgressChanged(progress,this);
        }
    }

    public event Action<SubScene_Base> LoadFinished;

    protected void OnLoadedFinished()
    {

        OnProgressChanged(1);

        if (LoadFinished != null)
        {
            LoadFinished(this);
        }
    }

    internal string GetSceneInfo()
    {
        //return $"r:{rendererCount}\tv:{vertexCount:F1}w\t[{GetSceneName()}] ";
        return $"{GetSceneName()} r:{rendererCount} v:{vertexCount:F0}w ";
    }

    internal string GetSceneNameEx()
    {
        return $"[{contentType}]{GetSceneName()} r:{rendererCount} v:{vertexCount:F0}w ";
    }

    public SceneLoadArg GetSceneArg()
    {
        SceneLoadArg arg=new SceneLoadArg();
        arg.name=GetSceneName();
        arg.path=sceneArg.path;
        arg.index=sceneArg.index;
        return arg;
    }

    public string GetSceneName()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            try
            {
                string[] parts = sceneArg.path.Split(new char[] { '.', '\\', '/' });
                sceneName = parts[parts.Length - 2];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetSceneName  obj:{this},path:{sceneArg.path},Exception:{ex}");
            }
            
        }
        return sceneName;
    }


    void Start()
    {
        if (IsAutoLoad)
        {
            LoadSceneAsync(null);
        }
    }

    public GameObject boundsGo;

    protected virtual string BoundsName
    {
        get
        {
            return $"{this.name}_Bounds_{contentType}";
            //return "123";
        }
    }

    public int cubePrefabId = 0;

    internal void ShowBounds(Transform boxP)
    {
        //Debug.Log("ShowBounds:"+BoundsName+"|"+ contentType);
        //DestroyBoundsBox();
        //boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, transform);

        if (boundsGo == null)
        {
            boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, boxP, cubePrefabId);
        }
        else
        {
            boundsGo.SetActive(true);
        }
    }

    internal void ShowBounds()
    {
        ShowBounds(this.transform);
    }

    public void HideBoundsBox()
    {
        if (boundsGo)
        {
            boundsGo.SetActive(false);
        }
    }

    public virtual void DestroyBoundsBox()
    {
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
    }

    [ContextMenu("DestoryChildren")]
    public void DestoryChildren()
    {
        var children = SubSceneHelper.GetChildrenGos(GetSceneParent());
        foreach (var go in children)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        gos.Clear();

        IsLoaded = false;
        IsLoading = false;
    }

    internal List<ModelAreaTree> GetTrees()
    {
        List<ModelAreaTree> ts = new List<ModelAreaTree>();
        foreach(var go in gos)
        {
            ts.AddRange(go.GetComponentsInChildren<ModelAreaTree>());
        }
        return ts;
    }

    public float DisToCam;

    internal void HideObjects()
    {
        if (IsVisible==false) return;
        IsVisible = false;
        foreach (var go in gos)
        {
            if (go == null) continue;
            go.SetActive(false);
        }
    }

    //public bool IsFirst

    public bool IsVisible = true;
    internal void ShowObjects()
    {
        if (IsVisible) return;
        IsVisible = true;
        foreach (var go in gos)
        {
            if (go == null) continue;
            go.SetActive(true);
        }
    }

    [ContextMenu("UnLoadGosM")]
    public void UnLoadGosM()
    {
        //if (gos != null)
        //{
        //    foreach (var go in gos)
        //    {
        //        if (go == null) continue;
        //        GameObject.DestroyImmediate(go);
        //    }
        //    gos.Clear();
        //}


        if (sceneArg != null && sceneArg.objs != null)
        {
            foreach (var obj in sceneArg.objs)
            {
                if (obj == null) continue;
                GameObject.DestroyImmediate(obj);
            }
            //sceneArg.objs = null;
            sceneArg.objs.Clear();
        }

        IsLoaded = false;
        IsLoading = false;
    }

    [ContextMenu("UnLoadGos")]
    public void UnLoadGos()
    {
        foreach (var go in gos)
        {
            if (go == null) continue;
            GameObject.Destroy(go);
        }
        gos.Clear();

        if(sceneArg!=null&& sceneArg.objs != null)
        {
            foreach (var obj in sceneArg.objs)
            {
                if (obj == null) continue;
                GameObject.Destroy(obj);
            }
            sceneArg.objs = null;
        }


        IsLoaded = false;
        IsLoading = false;
    }

    //[ContextMenu("ReLoadScene")]
    //public void ReLoadScene()
    //{
    //    UnLoadGosM();
    //    LoadScene();
    //}

    //[ContextMenu("LoadScene")]
    //public void LoadScene()
    //{
    //    var gs= EditorHelper.LoadScene(GetSceneArg(), IsSetParent ? GetSceneParent() : null).ToList();
    //    SetObjects(gs);
    //    SetRendererParent();
    //    IsLoaded = true;
    //}

    //[ContextMenu("TestLoadSceneAsync")]
    //public void TestLoadSceneAsync()
    //{
    //    LoadSceneAsync();
    //    LoadSceneAsync();
    //}

    public IEnumerator LoadSceneAsyncCoroutine(Action<bool,SubScene_Base> callback)
    {
        if (IsLoading || IsLoaded)
        {
            Debug.LogWarning($"[SubScene_Base.LoadSceneAsyncCoroutine] scene:{GetSceneName()}, IsLoading:{IsLoading} || IsLoaded:{IsLoaded}");
            if (callback != null)
            {
                callback(false,this);
            }
            yield return null;
        }
        else
        {
            DateTime start = DateTime.Now;

            IsLoading = true;
            OnProgressChanged(0);
            yield return EditorHelper.LoadSceneAsync(GetSceneArg(), progress =>
            {
                OnProgressChanged(progress);
                //Debug.Log("progress:" + progress);
            }, s =>
            {
                if (IsSetParent)
                    GetSceneObjects();
                IsLoaded = true;
                IsLoading = false;

                if (callback != null)
                {
                    callback(true,this);
                }


                //WriteLog($"Load name:{GetSceneName()},time:{(DateTime.Now - start).ToString()},progress:{loadProgress}");
                //WriteLog($"Load[ {GetSceneName()} ]: {(DateTime.Now - start).ToString()}");
                WriteLog($"Load name:{GetSceneName()}",$"{(DateTime.Now - start).ToString()}|r:{rendererCount}|v:{GetVertexCountText()}w");
                OnLoadedFinished();
            }, IsSetParent);
        }
    }

    public string GetVertexCountText()
    {
        if(vertexCount<0.1){
            return vertexCount.ToString("F3");
        }
        else if(vertexCount<1){
            return vertexCount.ToString("F2");
        }
        else if(vertexCount<10){
            return vertexCount.ToString("F1");
        }
        else
        {
            return vertexCount.ToString("F0");
        }
    }

    public string Log = "";

    private void WriteLog(string tag,string log)
    {
        Log = log;
        Debug.Log($"[{tag}]{log}");
    }

    [ContextMenu("LoadSceneAsync")]
    public void TestLoadSceneAsync()
    {
        StartCoroutine(LoadSceneAsyncCoroutine(null));
    }

    //[ContextMenu("LoadSceneAsync")]
    public void LoadSceneAsync(Action<bool,SubScene_Base> callback)
    {
        if (IsLoading || IsLoaded)
        {
            Debug.LogWarning($"[SubScene_Base.LoadSceneAsync] scene:{GetSceneName()}, IsLoading:{IsLoading} || IsLoaded:{IsLoaded}");
            if (callback != null)
            {
                callback(false,this);
            }
            return;
        }

        //StartCoroutine(EditorHelper.LoadSceneAsync(GetSceneName(), progress=>
        //{
        //    loadProgress = progress;
        //    Debug.Log("progress:"+ progress);
        //},s=>
        //{
        //    if(IsSetParent)
        //        GetSceneObjects();
        //    IsLoaded = true;
        //    IsLoading = false;
        //}, IsSetParent)
        //);

        if (this.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning("[SubScene_Base.LoadSceneAsync]this.gameObject.activeInHierarchy == false :" + name);
            //if (callback != null)
            //{
            //    callback(false);
            //}
            //return;
            HideBoundsBox();
            this.gameObject.SetActive(true);
        }
        if (this.gameObject.activeInHierarchy == true)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(callback));
        }
        else{
            if(callback!=null){
                callback(false,this);
            }
             Debug.LogError("[SubScene_Base.LoadSceneAsync]this.gameObject.activeInHierarchy == false :" + name);
        }
    }
    [ContextMenu("UnLoadSceneAsync2")]
    public void TestUnLoadSceneAsync()
    {
        UnLoadSceneAsync();
        UnLoadSceneAsync();
    }

    [ContextMenu("UnLoadSceneAsync")]
    public void UnLoadSceneAsync()
    {
        if (IsLoaded == false)
        {
            //Debug.LogWarning("IsLoaded==false :" + GetSceneName());
            return;
        }
        //DestoryGosImmediate();
        UnLoadGos();
        StartCoroutine(EditorHelper.UnLoadSceneAsync(GetSceneArg(), progress =>
        {
            loadProgress = progress;
            Debug.Log("progress:" + progress);
        }, () =>
        {
            //GetSceneObjects();
            IsLoaded = false;
        }));
    }

    public float loadProgress;



    [ContextMenu("GetSceneObjects")]
    public void GetSceneObjects()
    {
        //DestroyBoundsBox();
        HideBoundsBox();

        var gs = EditorHelper.GetSceneObjects(GetSceneArg(), GetSceneParent()).ToList();
        SetObjects(gs);
        SetRendererParent();
        InitVisible();

        AreaTreeNode node=this.transform.parent.GetComponent<AreaTreeNode>();
        if(node!=null)
            node.LoadRenderers(this.gameObject);
    }

     [ContextMenu("SetRendererParent")]
    public void SetRendererParent()
    {
        RendererId[] rIds=this.GetComponentsInChildren<RendererId>(true);
        foreach(var rI in rIds){
            rI.SetParent();
        }
    }

     [ContextMenu("SetRendererParent_Ids")]
    private void SetRendererParentEx()
    {
        IdDictionary.InitInfos();
        SetRendererParent();
    }

    public int GetSceneObjectCount()
    {
        int i = 0;
        foreach(var g in gos)
        {
            if (g != null) i++;
        }
        return i;
    }

    //public void Update()
    //{
    //    //if (IsLoaded == true && gos.Count == 0 && IsSetParent)
    //    //{
    //    //    GetSceneObjects();
    //    //}
    //}

    public void CheckGetSceneObjects()
    {
        //Debug.LogError($"CheckGetSceneObjects {this.name} IsLoaded:{IsLoaded},count:{gos.Count},IsSetParent:{IsSetParent}");
        if (IsLoaded == true && gos.Count == 0 && IsSetParent)
        {
            GetSceneObjects();
            Debug.LogError($"CheckGetSceneObjects {this.name} UnLoadSceneAsync1!!!");

            //if (this.gameObject.activeSelf == false)
            //{
            //    HideBoundsBox();
            //    this.gameObject.SetActive(true);
            //}

            StartCoroutine(EditorHelper.UnLoadSceneAsync(GetSceneArg(), null,null));
        }
        else
        {
            //Debug.LogError($"CheckGetSceneObjects {this.name} UnLoadSceneAsync2!!!");
            //this.UnLoadSceneAsync();
        }
    }

#if UNITY_EDITOR

    [ContextMenu("EditorCreateScene")]
    public void EditorCreateScene()
    {
        SubSceneManager subSceneManager = SubSceneManager.Instance;
        string path = subSceneManager.GetScenePath(this.name, SceneContentType.Single,"");
        //if (sceneArg == null)
        //{
        //    sceneArg = new SubSceneArg();
        //}
        //if (sceneArg.objs.Length == 0)
        //{

        //}
        SubSceneHelper.EditorCreateScene(this.gameObject, path, subSceneManager.IsOverride, true,this);
    }

    [ContextMenu("EditorReLoadScene")]
    public void EditorReLoadScene()
    {
        UnLoadGosM();
        EditorLoadSceneEx();
    }

    [ContextMenu("EditorLoadScene")]
    public void EditorLoadScene()
    {
        IsLoaded = true;
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
        var gs = EditorHelper.EditorLoadScene(scene, sceneArg.path, IsSetParent ? GetSceneParent() : null).ToList();
        SetObjects(gs);

        InitIdDict();

        SetRendererParent();
    }

    [ContextMenu("EditorLoadSceneEx")]
    public void EditorLoadSceneEx()
    {
        if (IsLoaded == true)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==true :" + GetSceneName());
            return;
        }


        EditorLoadScene();
    }

    [ContextMenu("EditorLoadLinkedScene")]
    public void EditorLoadLinkedScene()
    {
        if (LinkedScene != null)
        {
            LinkedScene.EditorLoadSceneEx();
        }
        else
        {
            Debug.LogError($"SubScene_Base.EditorLoadLinkedScene LinkedScene==null scene:{this.name}");
        }
    }


    private void LoadTreeRenderers()
    {
        var trees = GetTrees();
        foreach(var tree in trees)
        {
            tree.LoadRenderers();
        }
    }


    internal void InitIdDict()
    {
        Debug.Log($"SubScene_Base.InitIdDict name:{this.name} type:{contentType} linked:{LinkedScene}");
        IdDictionary.InitGos(gos, sceneName);
        if (LinkedScene != null)
        {
            if (this.contentType == SceneContentType.Tree)
            {
                if (LinkedScene.IsLoaded)
                {
                    this.LoadTreeRenderers();
                }
                else
                {
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name}");
                }
            }
            else if (this.contentType == SceneContentType.Part)
            {
                if (LinkedScene.IsLoaded)
                {
                    LinkedScene.LoadTreeRenderers();
                }
                else
                {
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name}");
                }
            }
           
            else if (this.contentType == SceneContentType.TreeAndPart)
            {
                LoadTreeRenderers();
            }
            else
            {
               
                if (LinkedScene.IsLoaded)
                {
                    LinkedScene.LoadTreeRenderers();
                    this.LoadTreeRenderers();
                }
                else
                {
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name} linkedScene:{LinkedScene.name}");
                }
            }
        }
        else
        {
            Debug.LogError($"SubScene_Base.InitIdDict LinkedScene==null scene:{this.name}");
        }
    }

    [ContextMenu("EditorSaveScene")]
    public void EditorSaveScene()
    {
        if (IsLoaded == false)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==false :" + GetSceneName());
            return;
        }
        IsLoaded = false;

        if (this is SubScene_Single)
        {
            var gs = SubSceneHelper.GetChildrenGos(GetSceneParent());
            SetObjects(gs);
        }

        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();

        //Scene scene=EditorHelper.CreateScene(scenePath, true, subSceneManager.IsOpenSubScene, gos.ToArray());
        Scene scene = SubSceneHelper.CreateScene(sceneArg);
        gos.Clear();
        IsLoaded = false;

        AreaTreeHelper.InitCubePrefab();
        ShowBounds();
        bool r1 = UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
        Debug.Log("r1:" + r1);
    }

    //public void SaveScene(string path, bool isOverride)
    //{
    //    SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
    //    sceneArg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, gos.ToArray());
    //    scene = SubSceneHelper.CreateScene(sceneArg);
    //    //SetPath(path);
    //    GetSceneName();
    //}

    public void SaveScene(SubSceneArg arg)
    {
        scene = SubSceneHelper.CreateScene(arg);
        this.IsLoaded=false;
    }

    public void SaveScene()
    {
        scene = SubSceneHelper.CreateScene(sceneArg);
        this.IsLoaded=false;
    }

    public void SetArg(string path, bool isOverride, List<GameObject> gs)
    {
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        if (sceneArg == null)
        {
            sceneArg = new SubSceneArg();
        }
        sceneArg.path = path;
        sceneArg.isOveride = isOverride;
        sceneArg.isOpen= subSceneManager.IsOpenSubScene;
        sceneArg.objs = gs;
        GetSceneName();
    }

#endif



    internal void Init()
    {
        if (gos.Count == 0)
        {
            Debug.LogError($"SubScene.Init gos.Count == 0 subScene:{this.name} contentType:{contentType}");
        }
        //if (gos.Count == 0)
        //{
        //    var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        //    InitRenderersInfo(renderers);
        //}
        //else
        {
            List<MeshRenderer> renderers = GetSceneRenderers();
            InitRenderersInfo(renderers.ToArray());
        }

        InitVisible();

        this.sceneParent = this.transform;
    }

    public List<GameObject> GetLoadedSceneGos()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (var go in gos)
        {
            if (go == null) continue;
            if (!objs.Contains(go))
            {
                objs.Add(go);
            }
        }
        if(sceneArg!=null)
            foreach (var go in sceneArg.objs)
            {
                if (go == null) continue;
                if (!objs.Contains(go))
                {
                    objs.Add(go);
                }
            }
        if (objs.Count == 0 && sceneParent!=null)
        {
            for(int i=0;i<sceneParent.childCount;i++)
            {
                var child = sceneParent.GetChild(i);
                objs.Add(child.gameObject);
            }
        }
        return objs;
    }

    public List<MeshRenderer> GetSceneRenderers()
    {
        List<GameObject> objs = GetLoadedSceneGos();
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var go in objs)
        {
            if (go == null) continue;
            renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true));
        }
        //Debug.Log($"SubScene_Base.GetSceneRenderers name:{this.name} gos:{objs.Count},renderers:{renderers.Count}");
        return renderers;
    }

    private void InitVisible()
    {
        IsVisible = true;
        foreach (var go in gos)
        {
            if (go.activeInHierarchy == false)
            {
                IsVisible = false;
                break;
            }
        }
    }

    private void InitRenderersInfo(MeshRenderer[] renderers)
    {
        rendererCount = renderers.Length;
        bounds = ColliderHelper.CaculateBounds(renderers);
        center = bounds.center;
        //radius=bounds.
        vertexCount = GetVertexCount(renderers);
        //Debug.Log($"SubScene_Base.Init name:{this.name} renderers:{renderers.Length} bounds:{bounds} center:{center}");
    }

    public float GetVertexCount(MeshRenderer[] renderers)
    {
        int count = 0;
        foreach (var renderer in renderers)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null) continue;
            count += meshFilter.sharedMesh.vertexCount;
        }
        float vertexCount = count / 10000.0f;
        return vertexCount;
    }

    [ContextMenu("GetClostPoint")]
    public void GetClostPoint()
    {
        Camera cam = GameObject.FindObjectOfType<Camera>();
        var pos = cam.transform.position;
        var clo = this.bounds.ClosestPoint(pos);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = clo;

        var dis = Vector3.Distance(clo, pos);
        Debug.Log("dis:" + dis+"|"+(dis*dis));
        Debug.DrawLine(clo, pos, Color.red, 10);
    }

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        var renderers = GetSceneRenderers();
        foreach(var renderer in renderers)
        {
            //AutomaticLODHelper.CreateLOD(renderer.gameObject, null, null, null, true, true);
            LODManager.Instance.CreateLOD(renderer.gameObject);
        }
    }

    public void OnDestroy()
    {
        Debug.Log("SubScene.OnDestroy:"+this.name);
    }
}



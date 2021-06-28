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
    public SubScene_Base LinkedScene;//���Ӧ�û�һֱ�����������У���InstanceId���ˡ�

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

    public bool HaveGos()
    {
        bool r = false;
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

    public List<GameObject> gos = new List<GameObject>();

    public void SetObjects(List<GameObject> goList)
    {
        gos = goList;
        if (sceneArg != null)
        {
            sceneArg.objs = goList.ToArray();
        }
    }

    public int childCount;

    public int rendererCount;

    public float vertexCount = 0;


    public bool IsSetParent = true;

    public bool IsAutoLoad = false;

    public event Action<float> ProgressChanged;

    protected void OnProgressChanged(float progress)
    {
        this.loadProgress = progress;
        if (ProgressChanged != null)
        {
            ProgressChanged(progress);
        }
    }

    public event Action AllLoaded;

    protected void OnAllLoaded()
    {

        OnProgressChanged(1);

        if (AllLoaded != null)
        {
            AllLoaded();
        }
    }

    internal string GetSceneInfo()
    {
        return $"r:{rendererCount}\tv:{vertexCount:F1}w\t[{GetSceneName()}] ";
    }

    internal string GetSceneNameEx()
    {
        return $"[{contentType}]{GetSceneName()} r:{rendererCount} v:{vertexCount:F0}w ";
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

    internal void ShowBounds(Transform boxP)
    {
        //Debug.Log("ShowBounds:"+BoundsName+"|"+ contentType);
        //DestroyBoundsBox();
        //boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, transform);

        if (boundsGo == null)
        {
            boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, boxP);
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
        foreach (var go in gos)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        gos.Clear();

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

        IsLoaded = false;
        IsLoading = false;
    }

    [ContextMenu("ReLoadScene")]
    public void ReLoadScene()
    {
        UnLoadGosM();
        LoadScene();
    }

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        gos = EditorHelper.LoadScene(GetSceneName(), IsSetParent ? GetSceneParent() : null).ToList();
        IsLoaded = true;
    }
    //[ContextMenu("TestLoadSceneAsync")]
    //public void TestLoadSceneAsync()
    //{
    //    LoadSceneAsync();
    //    LoadSceneAsync();
    //}

    public IEnumerator LoadSceneAsyncCoroutine(Action<bool> callback)
    {
        if (IsLoading || IsLoaded)
        {
            Debug.LogWarning("IsLoading || IsLoaded :" + GetSceneName());
            if (callback != null)
            {
                callback(false);
            }
            yield return null;
        }
        else
        {
            DateTime start = DateTime.Now;

            IsLoading = true;
            OnProgressChanged(0);
            yield return EditorHelper.LoadSceneAsync(GetSceneName(), progress =>
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
                    callback(true);
                }


                //WriteLog($"Load name:{GetSceneName()},time:{(DateTime.Now - start).ToString()},progress:{loadProgress}");
                WriteLog($"Load {GetSceneName()} : {(DateTime.Now - start).ToString()}");
                OnAllLoaded();
            }, IsSetParent);
        }
    }

    public string Log = "";

    private void WriteLog(string log)
    {
        Log = log;
        Debug.LogError(Log);
    }

    [ContextMenu("TestLoadSceneAsync")]
    public void TestLoadSceneAsync()
    {
        StartCoroutine(LoadSceneAsyncCoroutine(null));
    }

    //[ContextMenu("LoadSceneAsync")]
    public void LoadSceneAsync(Action<bool> callback)
    {


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
            Debug.LogError("this.gameObject.activeInHierarchy == false :"+name);
            if (callback != null)
            {
                callback(false);
            }
            return;
        }
        StartCoroutine(LoadSceneAsyncCoroutine(callback));
    }
    [ContextMenu("TestUnLoadSceneAsync")]
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
        StartCoroutine(EditorHelper.UnLoadSceneAsync(GetSceneName(), progress =>
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

        gos = EditorHelper.GetSceneObjects(GetSceneName(), GetSceneParent()).ToList();

        InitVisible();
    }

    //public void Update()
    //{
    //    if(IsLoaded==true && gos.Count == 0 && IsSetParent)
    //    {
    //        GetSceneObjects();
    //    }
    //}

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
        EditorLoadScene();
    }

    [ContextMenu("EditorLoadScene")]
    public void EditorLoadScene()
    {
        if (IsLoaded == true)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==true :" + GetSceneName());
            return;
        }
        IsLoaded = true;

        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
        gos = EditorHelper.EditorLoadScene(scene, sceneArg.path, IsSetParent ? GetSceneParent() : null).ToList();

        InitIdDict();
    }

    [ContextMenu("EditorLoadLinkedScene")]
    public void EditorLoadLinkedScene()
    {
        if (LinkedScene != null)
        {
            LinkedScene.EditorLoadScene();
        }
        else
        {
            Debug.LogError($"SubScene_Base.EditorLoadLinkedScene LinkedScene==null scene:{this.name}");
        }
    }

    /// <summary>
    /// ��ȡ�ϲ���������ԭģ�͵���Ϸ����
    /// </summary>
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
        IdDictionay.InitGos(gos, sceneName);
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
                    Debug.LogError($"SubScene_Base.InitIdDict LinkedScene.IsLoaded==false scene:{this.name}");
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
            gos = SubSceneHelper.GetChildrenGos(GetSceneParent());
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
    }

    public void SaveScene()
    {
        scene = SubSceneHelper.CreateScene(sceneArg);
    }

    public void SetArg(string path, bool isOverride)
    {
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        sceneArg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, gos.ToArray());
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
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            //foreach (var go in gos)
            //{
            //    if (go == null) continue;
            //    renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true));
            //}
            //Debug.Log($"SubScene_Base.Init name:{this.name} gos:{gos.Count},renderers:{renderers.Count}");

            foreach (var go in sceneArg.objs)
            {
                if (go == null) continue;
                renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true));
            }
            Debug.Log($"SubScene_Base.Init name:{this.name} sceneArg.objs:{sceneArg.objs.Length},renderers:{renderers.Count}");

            InitRenderersInfo(renderers.ToArray());
        }

        InitVisible();

        this.sceneParent = this.transform;
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
        Debug.Log($"SubScene_Base.Init name:{this.name} renderers:{renderers.Length} bounds:{bounds} center:{center}");
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

    public void OnDestroy()
    {
        Debug.Log("SubScene.OnDestroy:"+this.name);
    }
}



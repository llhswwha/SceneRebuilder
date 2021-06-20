using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubScene_Base : MonoBehaviour
{
    public bool IsLoaded = false;

    public bool IsLoading = false;

    //public SubSceneType sceneType;

    public string sceneName = "";

    public string scenePath;

    public Scene scene;

    public Bounds bounds;

    public List<GameObject> gos = new List<GameObject>();

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

    public string GetRalativePath()
    {
        //string rPath = scenePath;
        //if(rPath.Contains(":"))
        //{
        //    rPath = EditorHelper.PathToRelative(rPath);
        //}
        //return rPath;

        string rPath = EditorHelper.PathToRelative(scenePath);
        return rPath;
    }


    public string GetSceneName()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            try
            {
                string[] parts = scenePath.Split(new char[] { '.', '\\', '/' });
                sceneName = parts[parts.Length - 2];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetSceneName  obj:{this},path:{scenePath},Exception:{ex}");
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
            return this.name + "_Bounds";
        }
    }


    internal void ShowBounds()
    {
        DestroyBoundsBox();
        boundsGo = AreaTreeHelper.CreateBoundsCube(bounds, BoundsName, transform);
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
        var children = GetChildrenGos();
        foreach (var go in children)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        gos.Clear();

        IsLoaded = false;
        IsLoading = false;
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
        gos = EditorHelper.LoadScene(GetSceneName(), IsSetParent ? this.transform : null).ToList();
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
                Debug.Log("progress:" + progress);
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
            Debug.LogWarning("IsLoaded==false :" + GetSceneName());
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
        DestroyBoundsBox();

        gos = EditorHelper.GetSceneObjects(GetSceneName(), this.transform).ToList();
    }

    //public void Update()
    //{
    //    if(IsLoaded==true && gos.Count == 0 && IsSetParent)
    //    {
    //        GetSceneObjects();
    //    }
    //}

#if UNITY_EDITOR

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
        gos = EditorHelper.EditorLoadScene(scene, scenePath, IsSetParent ? this.transform : null).ToList();
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
            gos = GetChildrenGos();//获取新的全部子物体，以便下面更新场景
        }

        Scene scene=EditorHelper.CreateScene(scenePath, true, gos.ToArray());
        gos.Clear();
        IsLoaded = false;

        AreaTreeHelper.InitCubePrefab();
        ShowBounds();
        bool r1 = UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);//关闭场景，不关闭无法覆盖
        Debug.Log("r1:" + r1);
    }

    //[ContextMenu("SaveScene")]
    public void SaveChildrenToScene(string path, bool isOverride)
    {

        var children = GetChildrenGos();
        scene = EditorHelper.CreateScene(path, isOverride, children.ToArray());
        scenePath = path;

    }

    public void SaveScene(string path, bool isOverride)
    {
        scene = EditorHelper.CreateScene(path, isOverride, gos.ToArray());
        scenePath = path;
        GetSceneName();
    }

#endif

    public List<GameObject> GetChildrenGos()
    {
        List<GameObject> rootChildren = new List<GameObject>();
        childCount = this.transform.childCount;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            rootChildren.Add(this.transform.GetChild(i).gameObject);
        }
        return rootChildren;
    }

    internal void Init()
    {
        if (gos.Count == 0)
        {
            var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
            InitRenderersInfo(renderers);
        }
        else
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            foreach(var go in gos)
            {
                if (go == null) continue;
                renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true));
            }
            InitRenderersInfo(renderers.ToArray());
        }
    }

    private void InitRenderersInfo(MeshRenderer[] renderers)
    {
        rendererCount = renderers.Length;
        bounds = ColliderHelper.CaculateBounds(renderers);
        vertexCount = GetVertexCount(renderers);
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
}

public enum SubSceneType
{
    Single, Part, Base,In,Out0,Out1
}
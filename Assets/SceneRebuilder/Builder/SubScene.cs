using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class SubScene : MonoBehaviour
{
    public Bounds bounds;

    public List<GameObject> gos = new List<GameObject>();

    public int childCount;

    public int rendererCount;

    public Scene scene;

    public string scenePath;

    public string GetRalativePath()
    {
        //string rPath = scenePath;
        //if(rPath.Contains(":"))
        //{
        //    rPath = EditorHelper.PathToRelative(rPath);
        //}
        //return rPath;

        string rPath= EditorHelper.PathToRelative(scenePath);
        return rPath;
    }

    public string sceneName = "";

    public string GetSceneName()
    {
        if(string.IsNullOrEmpty(sceneName))
        {
            string[] parts = scenePath.Split(new char[] { '.', '\\', '/' });
            sceneName = parts[parts.Length - 2];
        }
        return sceneName;
    }

    public bool IsSetParent = true;

    public bool IsAutoLoad = true;

    void Start()
    {
        if (IsAutoLoad)
        {
            LoadSceneAsync();
        }
    }

    public GameObject boundsGo;

    internal void ShowBounds()
    {
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
        boundsGo=AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform);
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

    [ContextMenu("DestoryGosImmediate")]
    public void DestoryGosImmediate()
    {
        foreach(var go in gos)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        gos.Clear();

        IsLoaded = false;
        IsLoading = false;
    }

    [ContextMenu("DestoryGos")]
    public void DestoryGos()
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
        DestoryGosImmediate();
        LoadScene();
    }

    public bool IsLoaded = false;

    public bool IsLoading = false;

    [ContextMenu("LoadScene")]
    public void LoadScene()
    {
        gos = EditorHelper.LoadScene(GetSceneName(), IsSetParent ? this.transform : null).ToList();
        IsLoaded = true;
    }
    [ContextMenu("TestLoadSceneAsync")]
    public void TestLoadSceneAsync()
    {
        LoadSceneAsync();
        LoadSceneAsync();
    }

    public IEnumerator LoadSceneAsyncCoroutine()
    {
        if (IsLoading || IsLoaded)
        {
            Debug.LogWarning("IsLoading || IsLoaded :" + GetSceneName());
            yield return null;
        }
        else
        {
            IsLoading = true;
            loadProgress = 0;
            yield return EditorHelper.LoadSceneAsync(GetSceneName(), progress =>
            {
                loadProgress = progress;
                Debug.Log("progress:" + progress);
            }, s =>
            {
                if (IsSetParent)
                    GetSceneObjects();
                IsLoaded = true;
                IsLoading = false;
            }, IsSetParent);
        }
    }

    [ContextMenu("LoadSceneAsync")]
    public void LoadSceneAsync()
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

        StartCoroutine(LoadSceneAsyncCoroutine());
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
        if(IsLoaded==false )
        {
            Debug.LogWarning("IsLoaded==false :" + GetSceneName());
            return;
        }
        //DestoryGosImmediate();
        DestoryGos();
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
        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }

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
        DestoryGosImmediate();
        EditorLoadScene();
    }

    [ContextMenu("EditorLoadScene")]
    public void EditorLoadScene()
    {
        if (IsLoaded==true)
        {
            Debug.LogWarning("EditorLoadScene IsLoaded==true :" + GetSceneName());
            return;
        }
        IsLoaded = true;

        if (boundsGo)
        {
            GameObject.DestroyImmediate(boundsGo);
        }
        gos = EditorHelper.EditorLoadScene(scene, scenePath, IsSetParent ? this.transform : null).ToList() ;

       
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


        EditorHelper.CreateScene(scenePath, true, gos.ToArray());
        gos.Clear();
        IsLoaded = false;
    }

    //[ContextMenu("SaveScene")]
    public void SaveChildrenToScene(string path,bool isOverride)
    {

        var children = GetChildrenGos();
        scene=EditorHelper.CreateScene(path, isOverride, children.ToArray());
        scenePath = path;
#endif
    }

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
        var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        rendererCount = renderers.Length;

        bounds = ColliderHelper.CaculateBounds(renderers);
        //ShowBounds();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManager : MonoBehaviour
{
    private static SubSceneManager _instance;
    public static SubSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance= GameObject.FindObjectOfType<SubSceneManager>();
            }
            if (_instance == null)
            {
                GameObject go = new GameObject("SubSceneManager");
                _instance = go.AddComponent<SubSceneManager>();
            }
            return _instance;
        }
    }

    public SubScene_Base[] subScenes;

    public string RootDir = "SubScenes";

    public string SceneDir = "MinHangBuildings";

    public string SceneName = "abc";

    public bool IsOverride = true;

    public SceneContentType contentType;

    public List<GameObject> gos = new List<GameObject>();

    public GameObject root = null;

    public bool IsSetParent = true;

    public bool IsAutoLoad = false;

    public bool IsOneCoroutine = true;


    //public bool IsPartScene = false;

    public bool IsClearOtherScenes = true;

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

#if UNITY_EDITOR

    [ContextMenu("* EditorCreateBuildingScenes")]
    private void EditorCreateBuildingScenes()
    {
        AreaTreeHelper.InitCubePrefab();

        DateTime start = DateTime.Now;
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>();
        for (int i = 0; i < buildings.Length; i++)
        {
            var item = buildings[i];

            float progress = (float)i / buildings.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes", $"Progress1 {i}/{subScenes.Length} {percents:F1}% {item.name}", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }

            //if (IsPartScene)
            //{
            //    string dir = GetSceneDir(SceneContentType.Part);
            //    item.EditorCreatePartScenes(dir, IsOverride);
            //}
            //else
            //{
            //    SubSceneHelper.EditorCreateScene<SubScene_Single>(item.gameObject, GetScenePath(item.name, SceneContentType.Single),IsOverride);
            //}

            item.EditorCreateScenesEx(this.contentType,(subProgress,si,c)=>
            {
                //子进度

                //Debug.Log($"EditorCreateBuildingScenes subProgress:{subProgress} || {i}/{subScenes.Length} {percents:F2}% of 100% \t{item.name}");
                float progress = (float)(i+subProgress) / buildings.Length;
                float percents = progress * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes ", $"Progress2 {(i + subProgress):F1}/{subScenes.Length} {percents:F1}% {item.name}", progress+ subProgress))
                {
                    //ProgressBarHelper.ClearProgressBar();
                    //break;
                }
            });
        }
        ProgressBarHelper.ClearProgressBar();

        SetBuildings();
        SetSetting();

        if (IsClearOtherScenes)
        {
            ClearOtherScenes();
        }

        EditorHelper.RefreshAssets();

        WriteLog($"EditorSaveScenes count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    //[ContextMenu("EditorLoadScenes_Part")]
    //public void EditorLoadScenes_Part()
    //{
    //    subScenes = GameObject.FindObjectsOfType<SubScene_Part>(true);
    //    EditorLoadScenes(subScenes);
    //}

    //[ContextMenu("EditorLoadScenes")]
    //public void EditorLoadScenes()
    //{
    //    subScenes = GetSubScenes();
    //    EditorLoadScenes(subScenes);
    //}

    [ContextMenu("* EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        AreaTreeHelper.InitCubePrefab();

        DateTime start = DateTime.Now;
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>();
        for (int i = 0; i < buildings.Length; i++)
        {
            var item = buildings[i];
            float progress = (float)i / buildings.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes", $"{i}/{subScenes.Length} {percents:F2}% of 100% \t{item.name}", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }

            //if (IsPartScene)
            //{
            //    string dir = GetSceneDir(SceneContentType.Part);
            //    item.EditorCreatePartScenes(dir, IsOverride);
            //}
            //else
            //{
            //    SubSceneHelper.EditorCreateScene<SubScene_Single>(item.gameObject, GetScenePath(item.name, SceneContentType.Single),IsOverride);
            //}
            item.EditorLoadScenesEx(this.contentType);
        }
        ProgressBarHelper.ClearProgressBar();

        SetBuildings();
        SetSetting();

        if (IsClearOtherScenes)
        {
            ClearOtherScenes();
        }

        WriteLog($"EditorSaveScenes count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    public static void EditorLoadScenes<T>(T[] scenes) where T : SubScene_Base
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base item = scenes[i];
            float progress = (float)i / scenes.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"{item.GetSceneName()}\t{i}/{scenes.Length} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            item.EditorLoadScene();
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"EditorLoadScenes count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("EditorSaveScenes")]
    public void EditorSaveScenes()
    {
        DateTime start = DateTime.Now;
        subScenes = GetSubScenes();
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene_Base item = subScenes[i];
            float progress = (float)i / subScenes.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes", $"{item.GetSceneName()}\t{i}/{subScenes.Length} {percents:F2}% of 100%", progress))
            {
                break;
            }
            item.EditorSaveScene();
        }
        ProgressBarHelper.ClearProgressBar();

        ClearOtherScenes();

        WriteLog($"EditorSaveScenes count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    

    public void SetBuildings()
    {
        if (contentType == SceneContentType.Part)
        {
            SetBuildings_Parts();
        }
        else
        {
            SetBuildings_Single();
        }
    }


    [ContextMenu("SetBuildings_Single")]
    public void SetBuildings_Single()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        SetBuildings(subScenes);
    }

    [ContextMenu("SetBuildings_Parts")]
    public void SetBuildings_Parts()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Part>(true);
        SetBuildings(subScenes);
    }

    [ContextMenu("SetBuildings_All")]
    public void SetBuildings_All()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
        SetBuildings(subScenes);
    }

    public static void SetBuildings<T>(T[] scenes) where T : SubScene_Base
    {
        Debug.Log($"scenes:{scenes.Length}");
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[scenes.Length + 1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];
            string path = item.sceneArg.GetRalativePath();
            Debug.Log("path:" + path);
            buildingScenes[i + 1] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = buildingScenes;

        Debug.Log("SetBuildings:" + scenes.Length);
    }

    //[ContextMenu("TestCreateDir")]
    //public void TestCreateDir()
    //{

    //    EditorHelper.CreateDir(RootDir, SceneDir);

    //}

    //public int Count = 1;

    //[ContextMenu("TestCreateScene")]
    //public void TestCreateScene()
    //{
    //    string scenePath = GetScenePath(SceneName);
    //    List<GameObject> newGos = new List<GameObject>();
    //    for (int i = 0; i<Count;i++)
    //    {
    //        newGos.Add(new GameObject($"go_{Count}_{i}"));
    //    }
    //    Count++;
    //    EditorHelper.CreateScene( scenePath, IsOverride, newGos.ToArray());
    //}

    //[ContextMenu("TestCloseScene")]
    //public void TestCloseScene()
    //{
    //    string scenePath = GetScenePath(SceneName);

    //    Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(scenePath);
    //    Debug.Log("scene IsValid:" + scene.IsValid());
    //    if (scene.IsValid() == true)//打开
    //    {
    //        bool r=UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);//关闭场景，不关闭无法覆盖
    //        Debug.Log("r:" + r);

    //    }
    //}

    public Scene newScene;

    public string ScenePath;

    //public SubScene_Single subScene;

    //[ContextMenu("TestSaveGos")]
    //public void TestSaveGos()
    //{
    //    //string scenePath = GetScenePath(SceneName);
    //    newScene= CreateScene(gos.ToArray());
    //}

    //[ContextMenu("TestSubScene")]
    //public void TestSubScene()
    //{
    //    if (root)
    //    {
    //        SubScene ss=root.AddComponent<SubScene>();
    //        ss.Init();
    //        string path = GetScenePath(root.name);
    //        ss.SaveChildrenToScene(path, IsOverride);
    //    }
    //}

    //[ContextMenu("LoadAllScenes")]
    //public void LoadAllScenes()
    //{
    //    if (root)
    //    {
    //        SubScene_Single ss = root.AddComponent<SubScene_Single>();
    //        ss.Init();
    //        string path = GetScenePath(root.name);
    //        ss.SaveChildrenToScene(path, IsOverride);
    //    }
    //}

    [ContextMenu("ClearOtherScenes")]
    public void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    public bool IsOpenSubScene = false;

    public Scene CreateScene(params GameObject[] objs)
    {
        ScenePath = GetScenePath(SceneName, SceneContentType.Single);
        return EditorHelper.CreateScene(ScenePath, IsOverride, IsOpenSubScene, objs);
    }

    //public Scene CreateScene(string sceneName,params GameObject[] objs)
    //{
    //    string scenePath = GetScenePath(sceneName);
    //    return EditorHelper.CreateScene(scenePath, IsOverride, objs);
    //}

#endif


    [ContextMenu("AutoLoadScenes")]
    public void AutoLoadScenes()
    {
        var outScenes = GetSubScenes(SubSceneType.Out0);
        LoadScenesEx(outScenes);
    }

    [ContextMenu("LoadScenesEx")]
    public void LoadScenesEx()
    {
        subScenes = GetSubScenes();
        LoadScenesEx(subScenes);
    }

    public void LoadScenesEx<T>(T[] scenes) where T : SubScene_Base
    {
        if (IsOneCoroutine)
        {
            LoadScenesAsyncEx(scenes);
        }
        else
        {
            LoadScenesAsync(scenes);
        }
    }
    

    void Start()
    {
        if (IsAutoLoad)
        {
            LoadScenesEx();
        }
    }

    [ContextMenu("GetSceneInfos")]
    public void GetSceneInfos()
    {
        Debug.Log("GetSceneInfos");
        subScenes = GetSubScenes();
        foreach (var item in subScenes)
        {
            item.Init();
        }
        SortScenes();
    }

    [ContextMenu("SortScenes")]
    private void SortScenes()
    {
        var list = GetSubScenes().ToList();
        list.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        subScenes = list.ToArray();
    }


    [ContextMenu("SetSetting")]
    public void SetSetting()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
        foreach (var item in subScenes)
        {
            item.IsSetParent = this.IsSetParent;
            item.IsAutoLoad = this.IsAutoLoad;
        }

        Debug.Log("SetSetting:"+subScenes.Length);
    }

    public string GetScenePath(string sceneName, SceneContentType dir)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        //return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";
        

        //if (dir==SubSceneDir.Part)
        //{
        //    return $"{RootDir}/{SceneDir}_Part/{sceneName}.unity";
        //}
        //else if (dir == SubSceneDir.Tree)
        //{
        //    return $"{RootDir}/{SceneDir}_Tree/{sceneName}.unity";
        //}
        //else if (dir == SubSceneDir.TreePart)
        //{
        //    return $"{RootDir}/{SceneDir}_TreePart/{sceneName}.unity";
        //}
        //else
        //{
        //    return $"{RootDir}/{SceneDir}/{sceneName}.unity";
        //}

        return $"{RootDir}/{SceneDir}_{dir}/{sceneName}.unity";
    }

    public string GetSceneDir(SceneContentType dir)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        //return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";

        //if (isPart)
        //{
        //    return $"{RootDir}/{SceneDir}_Parts/";
        //}
        //else
        //{
        //    return $"{RootDir}/{SceneDir}/";
        //}

        return $"{RootDir}/{SceneDir}_{dir}/";
    }

    [ContextMenu("ClearScenes")]
    public void ClearScenes()
    {

        var ssList = GameObject.FindObjectsOfType<SubScene_List>(true);
        foreach (var item in ssList)
        {
            item.Clear();
        }
        var ss = GameObject.FindObjectsOfType<SubScene_Base>(true);
        foreach (var item in ss)
        {
            //item.LoadScene();
            GameObject.DestroyImmediate(item);
        }

        Debug.Log($"ClearScenes list:{ssList.Length},ss:{ss.Length}");
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        subScenes = GetSubScenes();
        foreach (var item in subScenes)
        {
            item.LoadScene();
        }
    }

    [ContextMenu("LoadScenesAsync")]
    public void LoadScenesAsync()
    {
        subScenes = GetSubScenes();
        LoadScenesAsync(subScenes);
    }

    public void LoadScenesAsync<T>(T[] scenes) where T : SubScene_Base
    {
        DateTime start = DateTime.Now;
        OnProgressChanged(0);
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        int count = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];
            item.LoadSceneAsync(b =>
            {
                count++;
                var progress = (count + 0.0f) / scenes.Length;
                OnProgressChanged(progress);
                if (count == scenes.Length)
                {
                    OnAllLoaded();
                    WriteLog($"LoadScenesAsync  count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
                }
            });//多个协程，乱序
        }
    }

    [ContextMenu("LoadScenesAsyncEx")]
    public void LoadScenesAsyncEx()
    {
        subScenes = GetSubScenes();
        LoadScenesAsyncEx(subScenes);//一个协程顺序
    }

    public void LoadScenesAsyncEx<T>(T[] scenes) where T : SubScene_Base
    {
        StartCoroutine(LoadAllScenesCoroutine(scenes));//一个协程顺序
    }

    public float loadProgress = 0;

    IEnumerator LoadAllScenesCoroutine<T>(T[] scenes) where T : SubScene_Base
    {
        //loadProgress = 0;
        OnProgressChanged(0);
        DateTime start = DateTime.Now;
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            var subScene = scenes[i];
            var progress = (i+0.0f) / scenes.Length;
            OnProgressChanged(progress);
            Debug.Log($"loadProgress:{loadProgress},scene:{subScene.GetSceneName()}");
            
            yield return subScene.LoadSceneAsyncCoroutine(null);
        }
        WriteLog($"LoadScenesAsyncEx  count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
        OnAllLoaded();
        yield return null;
    }

    [ContextMenu("DestoryChildren")]
    public void DestoryChildren()
    {
        subScenes = GetSubScenes();
        foreach (var item in subScenes)
        {
            item.DestoryChildren();
        }
    }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        AreaTreeHelper.InitCubePrefab();

        subScenes = GetSubScenes();
        foreach (var item in subScenes)
        {
            item.ShowBounds();
        }
    }

    [ContextMenu("DestoryGosImmediate")]
    public void DestoryGosImmediate()
    {
        subScenes = GetSubScenes();
        foreach (var item in subScenes)
        {
            item.UnLoadGosM();
        }
    }

    [ContextMenu("UnLoadSceneAsync")]
    public void UnLoadScenesAsync()
    {
        subScenes = GetSubScenes();
        UnLoadScenesAsync(subScenes);
    }

    public static SubScene_Base[] ToBaseScene<T>(T[] ss) where T : SubScene_Base
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        foreach (var s in ss)
        {
            scenes.Add(s);
        }
        return scenes.ToArray();
    }

    public static SubScene_Base[] GetSubScenes(SubSceneType sceneType)
    {
        SubScene_Base[] result = null;
        if (sceneType == SubSceneType.In)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_In>();
            result= ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Out0)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out0>();
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Out1)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out1>();
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Part)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Part>();
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Single)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Single>();
            result = ToBaseScene(scenes);
        }
        else //全部
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Base>();
            result = ToBaseScene(scenes);
        }

        //txtResult.text = $"type:{sceneType},count:{result.Length}";
        return result;
    }

    /// <summary>
    /// 根据IsPartScene获取不同的类型
    /// </summary>
    /// <returns></returns>
    public SubScene_Base[] GetSubScenes()
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        if (contentType==SceneContentType.Part)
        {
            var ss = GameObject.FindObjectsOfType<SubScene_Part>(true);
            foreach(var s in ss)
            {
                scenes.Add(s);
            }
        }
        else
        {
            var ss = GameObject.FindObjectsOfType<SubScene_Single>(true);
            foreach (var s in ss)
            {
                scenes.Add(s);
            }
        }
        return scenes.ToArray();
    }

    public void UnLoadScenesAsync<T>(T[] scenes) where T : SubScene_Base
    {
        foreach (var item in scenes)
        {
            item.UnLoadSceneAsync();
        }
    }




    public string Log = "";

    private void WriteLog(string log)
    {
        Log = log;
        Debug.LogError(Log);
    }
}

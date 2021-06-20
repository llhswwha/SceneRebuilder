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
    public SubScene_Base[] subScenes;

    public string RootDir = "SubScenes";

    public string SceneDir = "MinHangBuildings";

    public string SceneName = "abc";

    public bool IsOverride = true;

    public List<GameObject> gos = new List<GameObject>();

    public GameObject root = null;

    public bool IsSetParent = true;

    public bool IsAutoLoad = true;

    public bool IsOneCoroutine = true;

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

    [ContextMenu("LoadScenesEx")]
    public void LoadScenesEx()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
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
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.Init();
        }
        SortScenes();
    }

    [ContextMenu("SortScenes")]
    private void SortScenes()
    {
        var list = GameObject.FindObjectsOfType<SubScene_Single>(true).ToList();
        list.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        subScenes = list.ToArray();
    }


    [ContextMenu("SetSetting")]
    public void SetSetting()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.IsSetParent = this.IsSetParent;
            item.IsAutoLoad = this.IsAutoLoad;
        }
    }

    private string GetScenePath(string sceneName)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        //return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";
        

        if (IsPartScene)
        {
            return $"{RootDir}/{SceneDir}_Parts/{sceneName}.unity";
        }
        else
        {
            return $"{RootDir}/{SceneDir}/{sceneName}.unity";
        }
    }

    public string GetSceneDir(bool isPart)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        //return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";
        if (isPart)
        {
            return $"{RootDir}/{SceneDir}_Parts/";
        }
        else
        {
            return $"{RootDir}/{SceneDir}/";
        }
    }

    [ContextMenu("RemoveSubScenes")]
    public void RemoveSubScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            //item.LoadScene();
            GameObject.DestroyImmediate(item);
        }
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.LoadScene();
        }
    }

    [ContextMenu("LoadScenesAsync")]
    public void LoadScenesAsync()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
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
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
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
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.DestoryChildren();
        }
    }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        AreaTreeHelper.InitCubePrefab();

        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.ShowBounds();
        }
    }

    [ContextMenu("DestoryGosImmediate")]
    public void DestoryGosImmediate()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.UnLoadGosM();
        }
    }

    [ContextMenu("UnLoadSceneAsync")]
    public void UnLoadScenesAsync()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        foreach (var item in subScenes)
        {
            item.UnLoadSceneAsync();
        }
    }

#if UNITY_EDITOR

    private void GetSubScenes()
    {

    }

    [ContextMenu("EditorLoadScenes_Part")]
    public void EditorLoadScenes_Part()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Part>(true);
        EditorLoadScenes(subScenes);
    }

    [ContextMenu("EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        EditorLoadScenes(subScenes);
    }

    public static void EditorLoadScenes<T>(T[] scenes) where T :SubScene_Base
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base item = scenes[i];
            float progress = (float)i / scenes.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"{item.GetSceneName()}\t{i}/{scenes.Length} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            item.EditorLoadScene();
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"EditorLoadScenes count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    public string Log = "";

    private void WriteLog(string log)
    {
        Log = log;
        Debug.LogError(Log);
    }



    [ContextMenu("EditorSaveScenes")]
    public void EditorSaveScenes()
    {
        DateTime start = DateTime.Now;
        subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene_Base item = subScenes[i];
            float progress = (float)i / subScenes.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes", $"{item.GetSceneName()}\t{i}/{subScenes.Length} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            item.EditorSaveScene();
        }
        ProgressBarHelper.ClearProgressBar();

        ClearOtherScenes();

        WriteLog($"EditorSaveScenes count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    public bool IsPartScene = false;

    public bool IsClearOtherScenes = true;

    [ContextMenu("EditorCreateBuildingScenes")]
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

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorSaveScenes", $"{item.name}\t{i}/{subScenes.Length} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }

            if (IsPartScene)
            {
                string dir = GetSceneDir(true);
                item.EditorCreatePartScenes(dir, IsOverride);
            }
            else
            {
                CreateSubScene(item.gameObject);
            }
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

    [ContextMenu("SetBuildings")]
    public void SetBuildings()
    {
        var subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        Debug.Log($"scenes:{subScenes.Length}");
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[subScenes.Length+1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene_Single item = subScenes[i];
            string path = item.GetRalativePath();
            Debug.Log("path:" + path);
            buildingScenes[i+1] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = buildingScenes;
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



    private SubScene_Single CreateSubScene(GameObject go)
    {
        UpackPrefab_One(go);

        SubScene_Single ss = go.AddComponent<SubScene_Single>();
        ss.Init();
        string path = GetScenePath(go.name);
        ss.SaveChildrenToScene(path, IsOverride);
        ss.ShowBounds();
        return ss;
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

    [ContextMenu("LoadAllScenes")]
    public void LoadAllScenes()
    {
        if (root)
        {
            SubScene_Single ss = root.AddComponent<SubScene_Single>();
            ss.Init();
            string path = GetScenePath(root.name);
            ss.SaveChildrenToScene(path, IsOverride);
        }
    }

    [ContextMenu("ClearOtherScenes")]
    public void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    public Scene CreateScene(params GameObject[] objs)
    {
        ScenePath = GetScenePath(SceneName);
        return EditorHelper.CreateScene(ScenePath, IsOverride, objs);
    }

    //public Scene CreateScene(string sceneName,params GameObject[] objs)
    //{
    //    string scenePath = GetScenePath(sceneName);
    //    return EditorHelper.CreateScene(scenePath, IsOverride, objs);
    //}

#endif
}

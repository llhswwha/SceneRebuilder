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
    public SubScene[] subScenes;

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

    public void LoadScenesEx()
    {
        if (IsOneCoroutine)
        {
            LoadScenesAsyncEx();
        }
        else
        {
            LoadScenesAsync();
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
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.Init();
        }
        SortScenes();
    }

    [ContextMenu("SortScenes")]
    private void SortScenes()
    {
        var list = GameObject.FindObjectsOfType<SubScene>(true).ToList();
        list.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        subScenes = list.ToArray();
    }


    [ContextMenu("SetSetting")]
    public void SetSetting()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
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
        return $"{RootDir}/{SceneDir}/{sceneName}.unity";
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.LoadScene();
        }
    }

    [ContextMenu("LoadScenesAsync")]
    public void LoadScenesAsync()
    {
        DateTime start = DateTime.Now;
        OnProgressChanged(0);
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        int count = 0;
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene item = subScenes[i];
            item.LoadSceneAsync(b=>
            {
                count++;
                var progress = (count + 0.0f) / subScenes.Length;
                OnProgressChanged(progress);
                if(count== subScenes.Length)
                {
                    OnAllLoaded();
                    WriteLog($"LoadScenesAsync  count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
                }
            });//多个协程，乱序
        }
    }

    [ContextMenu("LoadScenesAsyncEx")]
    public void LoadScenesAsyncEx()
    {
        StartCoroutine(LoadAllScenesCoroutine());//一个协程顺序
    }

    public float loadProgress = 0;

    IEnumerator LoadAllScenesCoroutine()
    {
        //loadProgress = 0;
        OnProgressChanged(0);
        DateTime start = DateTime.Now;
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            var subScene = subScenes[i];
            var progress = (i+0.0f) / subScenes.Length;
            OnProgressChanged(progress);
            Debug.Log($"loadProgress:{loadProgress},scene:{subScene.GetSceneName()}");
            
            yield return subScene.LoadSceneAsyncCoroutine(null);
        }
        WriteLog($"LoadScenesAsyncEx  count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
        OnAllLoaded();
        yield return null;
    }

    [ContextMenu("DestoryChildren")]
    public void DestoryChildren()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.DestoryChildren();
        }
    }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        AreaTreeManager areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (areaTreeManager)
        {
            AreaTreeHelper.CubePrefab = areaTreeManager.CubePrefab;
        }

        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.ShowBounds();
        }
    }

    [ContextMenu("DestoryGosImmediate")]
    public void DestoryGosImmediate()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.DestoryGosImmediate();
        }
    }

    [ContextMenu("UnLoadSceneAsync")]
    public void UnLoadScenesAsync()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        foreach (var item in subScenes)
        {
            item.UnLoadSceneAsync();
        }
    }

#if UNITY_EDITOR

    [ContextMenu("EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        
        DateTime start = DateTime.Now;

        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene item = subScenes[i];
            float progress = (float)i / subScenes.Length;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"{item.GetSceneName()}\t{i}/{subScenes.Length} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            item.EditorLoadScene();
        }

        ProgressBarHelper.ClearProgressBar();

        WriteLog($"EditorLoadScenes count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
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
        subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene item = subScenes[i];
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

    [ContextMenu("SetBuildings")]
    public void SetBuildings()
    {
        var subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        Debug.Log($"scenes:{subScenes.Length}");
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[subScenes.Length+1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < subScenes.Length; i++)
        {
            SubScene item = subScenes[i];
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

    public SubScene subScene;

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

    [ContextMenu("CreateBuildingScenes")]
    public void CreateBuildingScenes()
    {
        AreaTreeManager areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if(areaTreeManager)
        {
            AreaTreeHelper.CubePrefab = areaTreeManager.CubePrefab;
        }

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


            //item.SaveScenes(path, IsOverride);

            CreateSubScene(item.gameObject);
        }
        ProgressBarHelper.ClearProgressBar();

        SetBuildings();
        SetSetting();

        WriteLog($"EditorSaveScenes count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    private SubScene CreateSubScene(GameObject go)
    {
        UpackPrefab_One(go);

        SubScene ss = go.AddComponent<SubScene>();
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
            SubScene ss = root.AddComponent<SubScene>();
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

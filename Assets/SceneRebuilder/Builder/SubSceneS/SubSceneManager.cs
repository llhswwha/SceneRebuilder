using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManager : SingletonBehaviour<SubSceneManager>
{
    //private static SubSceneManager _instance;
    //public static SubSceneManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance= GameObject.FindObjectOfType<SubSceneManager>();
    //        }
    //        if (_instance == null)
    //        {
    //            GameObject go = new GameObject("SubSceneManager");
    //            _instance = go.AddComponent<SubSceneManager>();
    //        }
    //        return _instance;
    //    }
    //}

    public SubScene_Base[] subScenes;

    public List<SubScene_Base> GetScenes()
    {
        return subScenes.ToList().Where(s => s != null).ToList();
    }

    public string RootDir = "SubScenes";

    public string SceneDir = "MinHangBuildings";


    public bool IsDirId = false;

    public bool IsDirHaveParent = false;

    public string SceneName = "abc";

    public bool IsOverride = true;

    public SceneContentType contentType;

    public List<GameObject> gos = new List<GameObject>();

    public GameObject root = null;

    public bool IsSetParent = true;

    public bool IsAutoLoad = false;

    //public bool IsOneCoroutine = true;


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

    [ContextMenu("* OneKey")]
    public void OneKey()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        OneKey(buildings);
    }

    public void OneKey(BuildingModelInfo[] buildings)
    {
        AreaTreeHelper.InitCubePrefab();

        DateTime start = DateTime.Now;
        int count = buildings.Length;
        for (int i = 0; i < count; i++)
        {
            var item = buildings[i];
            if (item == null) continue;
            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("OneKey", $"Progress1 {i}/{count} {percents:F1}% {item.name}", progress))
            {
                break;
            }

            item.OneKey_TreePartScene((subProgress) =>
            {
                //Debug.Log($"EditorCreateBuildingScenes subProgress:{subProgress} || {i}/{subScenes.Length} {percents:F2}% of 100% \t{item.name}");
                float progress = (float)(i + subProgress) / count;
                float percents = progress * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("OneKey ", $"Progress2 {(i + subProgress):F1}/{count} {percents:F1}% {item.name}", progress))
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

        WriteLog("OneKey",$"count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("* EditorCreateBuildingScenes")]
    public void EditorCreateBuildingScenes()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        EditorCreateBuildingScenes(buildings);
    }

    public void EditorCreateBuildingScenes(BuildingModelInfo[] buildings)
    {
        AreaTreeHelper.InitCubePrefab();
        DateTime start = DateTime.Now;
        int count = buildings.Length;
        for (int i = 0; i < count; i++)
        {
            var item = buildings[i];
            if(item==null)continue;
            float progress = (float)i / count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateBuildingScenes", $"Progress1 {i}/{count} {percents:F1}% {item.name}", progress))
            {
                break;
            }
            // item.EditorCreateScenesEx(this.contentType,(subProgress,si,c)=>
            // {
            //     float progress = (float)(i+subProgress) / count;
            //     float percents = progress * 100;
            //     if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateBuildingScenes ", $"Progress2 {(i + subProgress):F1}/{count} {percents:F1}% {item.name}", progress))
            //     {
            //         //ProgressBarHelper.ClearProgressBar();
            //         //break;
            //     }
            // });
            item.EditorCreateNodeScenes((subProgress)=>
            {
                float progress = (float)(i+subProgress) / count;
                float percents = progress * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateBuildingScenes ", $"Progress2 {(i + subProgress):F1}/{count} {percents:F1}% {item.name}", progress))
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
        WriteLog("EditorCreateBuildingScenes",$"count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("* EditorLoadScenes")]
    public void EditorLoadScenes()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        EditorLoadScenes(buildings);
    }

    public void EditorLoadScenes(BuildingModelInfo[] buildings)
    {
        AreaTreeHelper.InitCubePrefab();

        DateTime start = DateTime.Now;
        int count = buildings.Length;
        for (int i = 0; i < count; i++)
        {
            var item = buildings[i];
            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"Progress1 {i}/{count} {percents:F2}% of 100%  {item.name}", progress))
            {
                break;
            }

            item.EditorLoadNodeScenes(p =>
            {
                float progress = (float)(i + p) / count;
                float percents = progress * 100;

                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"Progress2 {(i + p):F1}/{count} {percents:F2}% of 100%  {item.name}", progress))
                {
                    return;
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

        WriteLog("EditorSaveScenes",$"count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("* EditorUnLoadScenes")]
    public void EditorUnLoadScenes()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        EditorUnLoadScenes(buildings);
    }

    public void EditorUnLoadScenes(BuildingModelInfo[] buildings)
    {
        AreaTreeHelper.InitCubePrefab();

        DateTime start = DateTime.Now;
        int count = buildings.Length;
        for (int i = 0; i < count; i++)
        {
            var item = buildings[i];
            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorUnLoadScenes", $"Progress1 {i}/{count} {percents:F2}% of 100%  {item.name}", progress))
            {
                break;
            }
            item.UnLoadScenes();
        }
        ProgressBarHelper.ClearProgressBar();

        WriteLog("EditorUnLoadScenes",$"count:{buildings.Length},\t time:{(DateTime.Now - start).ToString()}");
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

        WriteLog("EditorSaveScenes",$"count:{subScenes.Length},\t time:{(DateTime.Now - start).ToString()}");
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

    public void UpdateScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
    }

    [ContextMenu("SetBuildings_All")]
    public void SetBuildings_All()
    {
        UpdateScenes();
        SetBuildings(subScenes);
    }

    [ContextMenu("ClearBuildings")]
    public void ClearBuildings()
    {
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        EditorBuildSettings.scenes = buildingScenes;
    }

    public static void SetBuildings<T>(T[] scenes) where T : SubScene_Base
    {
        Debug.Log($"SetBuildings scenes:{scenes.Length}");
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[scenes.Length + 1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];
            string path = item.sceneArg.GetRalativePath();
            Debug.Log("path:" + path);
            buildingScenes[i + 1] = new EditorBuildSettingsScene(path, true);
            item.sceneArg.index=i+1;
        }
        EditorBuildSettings.scenes = buildingScenes;

        //Debug.Log("SetBuildings:" + scenes.Length);
    }

    public Scene newScene;

    public string ScenePath;

    [ContextMenu("ClearOtherScenes")]
    public void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    public bool IsOpenSubScene = false;

#endif


    [ContextMenu("AutoLoadScenes")]
    public void AutoLoadScenes()
    {
        var outScenes = GetSubScenes(SubSceneType.Out0);
        LoadScenesEx(outScenes,null);
    }

    [ContextMenu("LoadScenesEx")]
    public void LoadScenesEx()
    {
        subScenes = GetSubScenes();
        LoadScenesEx(subScenes,null);
    }

    public List<SubScene_Base> WattingForLoadedAll = new List<SubScene_Base>();
    public List<SubScene_Base> WattingForLoadedCurrent = new List<SubScene_Base>();

    public int LoadingSceneMaxCount = 1;
    
    public void LoadScenesEx<T>(T[] scenes, Action<float,bool> finishedCallback) where T : SubScene_Base
    {
        //if (IsOneCoroutine)
        //{
        //    LoadScenesAsyncEx(scenes, finishedCallbak);
        //}
        //else
        //{
        //    LoadScenesAsync(scenes, finishedCallbak);
        //}

        if (LoadingSceneMaxCount == 1)
        {
            LoadScenesAsyncEx(scenes, finishedCallback);
        }
        else if(LoadingSceneMaxCount <= 0)
        {
            LoadScenesAsync(scenes, finishedCallback);
        }
        else
        {
            StartCoroutine(LoadScenesByBag(scenes, finishedCallback));
        }
    }

    IEnumerator LoadScenesByBag<T>(T[] scenes, Action<float,bool> finishedCallback) where T : SubScene_Base
    {
        var start = DateTime.Now;
        WattingForLoadedAll.AddRange(scenes);
        int count = 0;
        while (WattingForLoadedAll.Count>0)
        {
            if(WattingForLoadedCurrent.Count< LoadingSceneMaxCount)
            {
                var scene = WattingForLoadedAll[0];
                WattingForLoadedAll.RemoveAt(0);
                WattingForLoadedCurrent.Add(scene);
                Debug.Log($"LoadScenesByBag Start scene:{scene.GetSceneName()},currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");
                scene.LoadSceneAsync((b, s) =>
                {
                    WattingForLoadedCurrent.Remove(s);
                    Debug.Log($"LoadScenesByBag End scene:{s.GetSceneName()},currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");

                    count++;
                    var progress = (count + 0.0f) / scenes.Length;
                    WriteLog("LoadScenesByBag",$"count:{scenes.Length} index:{count} progress:{progress:F3} time:{(DateTime.Now - start).ToString()}");
                    OnProgressChanged(progress);
                    if (count == scenes.Length)
                    {
                        if (finishedCallback != null)
                        {
                            finishedCallback(1,true);
                        }
                        WriteLog("LoadScenesByBag",$"count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
                        OnAllLoaded();
                    }
                    else
                    {
                        if (finishedCallback != null)
                        {
                            finishedCallback(progress,false);
                        }
                    }
                });
            }
            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log($"LoadSceneAsync Finished currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");
        yield return null;
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

    public string GetScenePath(string sceneName, SceneContentType contentType, GameObject go)
    {
        if (go==null)
        {
            return $"{RootDir}/{SceneDir}/{contentType}/{sceneName}.unity";
        }
        else
        {
            //return $"{RootDir}/{SceneDir}/{contentType}/{dir}/{sceneName}.unity";
            if (go.transform.parent != null && IsDirHaveParent)
            {
                var p = go.transform.parent.gameObject;
                if (IsDirId)
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{p.name}[{p.GetInstanceID()}]/{go.name}/{sceneName}.unity";
                }
                else
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{p.name}/{go.name}/{sceneName}.unity";
                }
            }
            else
            {
                if (IsDirId)
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}[{go.GetInstanceID()}]/{sceneName}.unity";
                }
                else
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}/{sceneName}.unity";
                }
            }
        }
    }

    //public string GetSceneDir(SceneContentType contentType, string dir)
    //{
    //    if (string.IsNullOrEmpty(dir))
    //    {
    //        return $"{RootDir}/{SceneDir}/{contentType}";
    //    }
    //    else
    //    {
    //        return $"{RootDir}/{SceneDir}/{contentType}/{dir}";
    //    }
    //}

    public string GetSceneDir(SceneContentType contentType, GameObject go)
    {
        if (go==null)
        {
            return $"{RootDir}/{SceneDir}/{contentType}";
        }
        else
        {
            //if(IsDirId)
            //{
            //    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}[{go.GetInstanceID()}]";
            //}
            //else
            //{
            //    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}";
            //}
            if (go.transform.parent != null && IsDirHaveParent)
            {
                var p = go.transform.parent.gameObject;
                if (IsDirId)
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{p.name}[{p.GetInstanceID()}]/{go.name}";
                }
                else
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{p.name}/{go.name}";
                }
            }
            else
            {
                if (IsDirId)
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}[{go.GetInstanceID()}]";
                }
                else
                {
                    return $"{RootDir}/{SceneDir}/{contentType}/{go.name}";
                }
            }
        }
    }

    public string GetSceneDir(SceneContentType dir)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        //return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";

        return $"{RootDir}/{SceneDir}_{dir}/";
    }

    public string GetRootDir()
    {
        return $"{RootDir}/{SceneDir}/";
    }

    public DirectoryInfo GetRootDirInfo()
    {
        string path= $"{Application.dataPath}/{RootDir}/{SceneDir}/";
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        Debug.Log($"path:{path}");
        return dirInfo;
    }

    public FileInfo[] GetSceneFiles()
    {
        DirectoryInfo rootDir = GetRootDirInfo();
        FileInfo[] files= rootDir.GetFiles("*.unity", SearchOption.AllDirectories);
        Debug.Log($"files:{files.Length}");
        return files;
    }

    public class SceneFile
    {
        public string scenePath1 ;
        public string sceneAssetPath ;
        public string sceneFilePath ;

        public bool isActive = false;

        public bool isExist = false;

        public float fileLength = 0;

        public SceneFile(SubScene_Base item)
        {
            var arg = item.GetSceneArg();
            scenePath1 = arg.path;
            sceneAssetPath = arg.GetSceneAssetPath();
            sceneFilePath = arg.GetSceneFilePath();
            isActive = true;

            //isExist = File.Exists(sceneFilePath);

            FileInfo fileInfo = new FileInfo(sceneFilePath);
            isExist = fileInfo.Exists;
            fileLength = fileInfo.Length / (1024f * 1024f);

            sceneFilePath = fileInfo.FullName;
        }

        public SceneFile(FileInfo item)
        {
            sceneFilePath = item.FullName;
            sceneAssetPath = EditorHelper.PathToRelative(sceneFilePath);
            scenePath1 = sceneAssetPath.Replace("Assets", "");

            FileInfo fileInfo = new FileInfo(sceneFilePath);
            isExist = fileInfo.Exists;
            fileLength = fileInfo.Length / (1024f * 1024f);

            sceneFilePath = fileInfo.FullName;
        }

        public void SelectAsset()
        {
            Debug.Log(scenePath1);
            Debug.Log(Application.dataPath);
            Debug.Log(sceneFilePath + "|" + System.IO.File.Exists(sceneFilePath));
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            Debug.Log(sceneAsset);
            //var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
            EditorHelper.SelectObject(sceneAsset);
        }

        public void DeleteAsset()
        {
            //SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            AssetDatabase.DeleteAsset(sceneAssetPath);
            //AssetDatabase.Refresh();
        }

        public override string ToString()
        {
            return $"[{isExist}][{isActive}][{fileLength:F2} M] {scenePath1}";
        }
    }

    public List<SceneFile> GetActiveSceneFiles()
    {
        List<SceneFile> sceneFiles1 = new List<SceneFile>();
        var scenes = GetScenes();
        foreach (var scene in scenes)
        {
            SceneFile sceneFile = new SceneFile(scene);
            sceneFiles1.Add(sceneFile);
        }
        return sceneFiles1;
    }

    public List<SceneFile> GetAllSceneFiles()
    {
        List<SceneFile> sceneFiles2 = new List<SceneFile>();
        DirectoryInfo rootDir = GetRootDirInfo();
        FileInfo[] files = rootDir.GetFiles("*.unity", SearchOption.AllDirectories);
        Debug.Log($"files:{files.Length}");
        foreach (var file in files)
        {
            SceneFile sceneFile = new SceneFile(file);
            sceneFiles2.Add(sceneFile);
        }
        return sceneFiles2;
    }

    public List<SceneFile> GetSceneFilesEx()
    {
        List<SceneFile> sceneFiles = new List<SceneFile>();
        List<SceneFile> sceneFiles1 = GetActiveSceneFiles();
        Dictionary<string, SceneFile> dict = new Dictionary<string, SceneFile>();
        foreach(var scene in sceneFiles1)
        {
            dict.Add(scene.sceneFilePath, scene);
        }
        List<SceneFile> sceneFiles2 = GetAllSceneFiles();
        foreach(var scene in sceneFiles2)
        {
            scene.isActive = dict.ContainsKey(scene.sceneFilePath);
        }

        return sceneFiles2;
    }

    public void DeleteInActiveScenes()
    {
        float fileLength = 0;
        List<SceneFile> sceneFiles = GetSceneFilesEx();
        int count = 0;
        for (int i = 0; i < sceneFiles.Count; i++)
        {
            float progress = (float)i / sceneFiles.Count;
            if(ProgressBarHelper.DisplayCancelableProgressBar("DeleteInActiveScenes", $"{i}/{sceneFiles.Count} {progress:P1}",progress))
            {
                break;
            }
            SceneFile scene = sceneFiles[i];
            if (scene.isActive==false)
            {
                scene.DeleteAsset();
                fileLength += scene.fileLength;
                count++;
            }
        }

        Debug.Log($"DeleteInActiveScenes all:{sceneFiles.Count} remove:{count} size:{fileLength}");
        AssetDatabase.Refresh();

        ProgressBarHelper.ClearProgressBar();
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

    //[ContextMenu("LoadScenes")]
    //public void LoadScenes()
    //{
    //    subScenes = GetSubScenes();
    //    foreach (var item in subScenes)
    //    {
    //        item.LoadScene();
    //    }
    //}

    [ContextMenu("LoadScenesAsync")]
    public void LoadScenesAsync()
    {
        subScenes = GetSubScenes();
        LoadScenesAsync(subScenes,null);
    }

    public void LoadScenesAsync<T>(T[] scenes, Action<float,bool> finishedCallbak) where T : SubScene_Base
    {
        DateTime start = DateTime.Now;
        OnProgressChanged(0);
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        int count = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];

            //WriteLog($"LoadScenesAsync Start count:{scenes.Length} index:{i} scene:{item.name}");

            item.LoadSceneAsync((b,s) =>
            {
                count++;
                var progress = (count + 0.0f) / scenes.Length;
                WriteLog("LoadScenesAsync",$"count:{scenes.Length} index:{count} progress:{progress} ");

                OnProgressChanged(progress);


                if (count == scenes.Length)
                {
                    if (finishedCallbak != null)
                    {
                        finishedCallbak(1,true);
                    }
                    WriteLog("LoadScenesAsync",$"count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
                    OnAllLoaded();
                }
                else{
                    if (finishedCallbak != null)
                    {
                        finishedCallbak(1,false);
                    }
                }
            });
        }
    }

    [ContextMenu("LoadScenesAsyncEx")]
    public void LoadScenesAsyncEx()
    {
        subScenes = GetSubScenes();
        LoadScenesAsyncEx(subScenes,null);
    }

    public void LoadScenesAsyncEx<T>(T[] scenes, Action<float,bool> finishedCallbak) where T : SubScene_Base
    {
        StartCoroutine(LoadAllScenesCoroutine(scenes, finishedCallbak));
    }

    public float loadProgress = 0;

    IEnumerator LoadAllScenesCoroutine<T>(T[] scenes,Action<float,bool> finishedCallbak) where T : SubScene_Base
    {
        //loadProgress = 0;
        OnProgressChanged(0);
        DateTime start = DateTime.Now;
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            var subScene = scenes[i];
            var progress = (i+0.0f) / scenes.Length;
            WriteLog("LoadAllScenesCoroutine",$"count:{scenes.Length} index:{i} progress:{progress} ");

            OnProgressChanged(progress);
            if(finishedCallbak!=null){
                finishedCallbak(progress,false);
            }
            //Debug.Log($"loadProgress:{loadProgress},scene:{subScene.GetSceneName()}");
            
            yield return subScene.LoadSceneAsyncCoroutine(null);
        }
        WriteLog("LoadAllScenesCoroutine",$"count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");

        if (finishedCallbak != null) finishedCallbak(1,true);
        OnAllLoaded();
        
        yield return null;
    }

    //[ContextMenu("DestoryChildren")]
    //public void DestoryChildren()
    //{
    //    subScenes = GetSubScenes();
    //    foreach (var item in subScenes)
    //    {
    //        item.DestoryChildren();
    //    }
    //}

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
            var scenes = GameObject.FindObjectsOfType<SubScene_In>(true);
            result= ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Out0)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out0>(true);
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Out1)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out1>(true);
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Part)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Part>(true);
            result = ToBaseScene(scenes);
        }
        else if (sceneType == SubSceneType.Single)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
            result = ToBaseScene(scenes);
        }
        else
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
            result = ToBaseScene(scenes);
        }

        //txtResult.text = $"type:{sceneType},count:{result.Length}";
        return result;
    }



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

    // private void WriteLog(string log)
    // {
    //     Log = log;
    //     Debug.Log(Log);
    // }

    private void WriteLog(string tag,string log)
    {
        Log = log;
        Debug.Log($"[{tag}]{log}");
    }
}



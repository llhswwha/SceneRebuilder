using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    public SubSceneBag GetScenes()
    {
        if (subScenes == null) return new SubSceneBag();
        return new SubSceneBag(subScenes);
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
            //float progress = (float)i / count;
            //float percents = progress * 100;
            var p1 = new ProgressArg("OneKey", i, count, item);

            if (ProgressBarHelper.DisplayCancelableProgressBar("OneKey", p1))
            {
                break;
            }

            item.OneKey_TreePartScene((subProgress) =>
            {
                p1.AddSubProgress(subProgress);

                //Debug.Log($"EditorCreateBuildingScenes subProgress:{subProgress} || {i}/{subScenes.Length} {percents:F2}% of 100% \t{item.name}");
                //float progress2 = (float)(i + subProgress) / count;
                //float percents2 = progress2 * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("OneKey ", p1))
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

    //public void SavePrefabs()
    //{
    //    var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
    //    BuildingModelInfoList.SavePrefabs(buildings);
    //}

    public void EditorCreateBuildingScenes(BuildingModelInfo[] buildings)
    {
        AreaTreeHelper.InitCubePrefab();
        DateTime start = DateTime.Now;
        int count = buildings.Length;
        for (int i = 0; i < count; i++)
        {
            var item = buildings[i];
            if(item==null)continue;
            //float progress = (float)i / count;
            //float percents = progress * 100;
            var p1 = new ProgressArg("EditorCreateBuildingScenes", i, count, item);
            if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateBuildingScenes", p1))
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
                p1.AddSubProgress(subProgress);
                //float progress2 = (float)(i+subProgress) / count;
                //float percents2 = progress2 * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateBuildingScenes ", p1))
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
            //float progress = (float)i / count;
            //float percents = progress * 100;
            var p1 = new ProgressArg("EditorLoadScenes", i, count, item);

            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }

            item.EditorLoadNodeScenes(p =>
            {
                p1.AddSubProgress(p);
                //float progress2 = (float)(i + p) / count;
                //float percents2 = progress2 * 100;

                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
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
        //if (contentType == SceneContentType.Part)
        //{
        //    SetBuildings_Parts();
        //}
        //else
        //{
        //    SetBuildings_Single();
        //}

        SetBuildings_All();
    }

    //[ContextMenu("SetBuildings_Single")]
    //public void SetBuildings_Single()
    //{
    //    subScenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
    //    SetBuildings(subScenes);
    //}

    //[ContextMenu("SetBuildings_Parts")]
    //public void SetBuildings_Parts()
    //{
    //    subScenes = GameObject.FindObjectsOfType<SubScene_Part>(true);
    //    SetBuildings(subScenes);
    //}

   

    [ContextMenu("SetBuildings_All")]
    public void SetBuildings_All()
    {
        
        //UpdateScenes();
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
        Debug.Log($"SetBuildings_All scenes:{subScenes.Length} includeInactive:{includeInactive}");
        SubSceneHelper.SetBuildings(subScenes);
    }



    

    public Scene newScene;

    public string ScenePath;

    [ContextMenu("ClearOtherScenes")]
    private void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    public bool IsOpenSubScene = false;

    public void CheckSceneIndex()
    {
        SubSceneHelper.CheckSceneIndex(includeInactive);
    }

#endif



    public static bool includeInactive = true;
    public SubScene_Base[] UpdateScenes()
    {
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
        return subScenes;
    }


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
    
    public void LoadScenesEx<T>(T[] scenes, Action<SceneLoadProgress> finishedCallback) where T : SubScene_Base
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

    public bool IsFilterLoadedScene = true;

    IEnumerator LoadScenesByBag<T>(T[] scenes, Action<SceneLoadProgress> finishedCallback) where T : SubScene_Base
    {
        var start = DateTime.Now;
        //WattingForLoadedAll.AddRange(scenes);
        int totalScenesCount = 0;
        foreach (var scene in scenes)
        {
            if (IsFilterLoadedScene && scene.IsLoaded) continue;
            if (WattingForLoadedAll.Contains(scene))
            {
                Debug.LogError($"LoadScenesByBag WattingForLoadedAll.Contains(scene) scene:{scene.GetSceneName()}");
                continue;
            }
            WattingForLoadedAll.Add(scene);
            totalScenesCount++;
        }

        int count = 0;
        SceneLoadProgress loadProgress = new SceneLoadProgress();
        if (WattingForLoadedAll.Count == 0)
        {
            if (finishedCallback != null)
            {
                loadProgress.SetInfo(null, 1, true);
                finishedCallback(loadProgress);
            }
        }
        bool isFinishedCallBack = false;
        DateTime startLoadTime = DateTime.Now;
        SubScene_Base lastScene = null;
        Debug.Log($"LoadScenesByBag scenes:{scenes.Length} WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount} bool isLoadScene  :{WattingForLoadedCurrent.Count < LoadingSceneMaxCount}");

        //if(WattingForLoadedAll.Count > 0 && WattingForLoadedCurrent.Count == 0)
        //{
        //    var scene = WattingForLoadedAll[0];
        //    lastScene = scene;
        //    WattingForLoadedAll.RemoveAt(0);
        //    WattingForLoadedCurrent.Add(scene);
        //}

        //while (WattingForLoadedAll.Count > 0 && WattingForLoadedCurrent.Count > 0)
        while (WattingForLoadedAll.Count > 0 || WattingForLoadedCurrent.Count > 0)
        {
            //Debug.Log($"LoadScenesByBag0 WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount} bool isLoadScene:{WattingForLoadedCurrent.Count < LoadingSceneMaxCount}");

            try
            {
                if (WattingForLoadedCurrent.Count < LoadingSceneMaxCount)
                //if (WattingForLoadedCurrent.Count < LoadingSceneMaxCount && WattingForLoadedAll.Count > 0)
                {
                    startLoadTime = DateTime.Now;
                    //Debug.Log($"LoadScenesByBag1 WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount} bool isLoadScene:{WattingForLoadedCurrent.Count < LoadingSceneMaxCount}");
                    if (WattingForLoadedAll.Count > 0)
                    {
                        var scene = WattingForLoadedAll[0];
                        lastScene = scene;
                        WattingForLoadedAll.RemoveAt(0);
                        WattingForLoadedCurrent.Add(scene);

                        //Debug.Log($"LoadScenesByBag2 WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount} bool isLoadScene:{WattingForLoadedCurrent.Count < LoadingSceneMaxCount}");

#if UNITY_EDITOR
                        Debug.Log($"LoadScenesByBag Start scene:{scene.GetSceneName()},currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");
#endif
                        scene.LoadSceneAsync((b, s) =>
                        {
                            //WattingForLoadedAll.RemoveAt(0);
                            WattingForLoadedCurrent.Remove(s);
#if UNITY_EDITOR
                            Debug.Log($"LoadScenesByBag End scene:{s.GetSceneName()},currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");
#endif

                            count++;
                            var progress = (count + 0.0f) / totalScenesCount;
                            WriteLog("LoadScenesByBag", $"count:{totalScenesCount} index:{count} progress:{progress:F3} time:{(DateTime.Now - start).ToString()}");
                            OnProgressChanged(progress);
                            if (count == totalScenesCount)
                            {
                                if (isFinishedCallBack == false)
                                {
                                    isFinishedCallBack = true;
                                    if (finishedCallback != null)
                                    {
                                        loadProgress.SetInfo(s, 1, true);
                                        finishedCallback(loadProgress);
                                    }
                                    WriteLog("LoadScenesByBag", $"Finished1 count:{totalScenesCount},\t time:{(DateTime.Now - start).ToString()}");
                                    OnAllLoaded();
                                }
                                else
                                {
                                    WriteLog("LoadScenesByBag", $"Finished12(Error)!!! count:{totalScenesCount},\t time:{(DateTime.Now - start).ToString()}");
                                }

                            }
                            else
                            {
                                if (finishedCallback != null)
                                {
                                    loadProgress.SetInfo(s, progress, false);
                                    finishedCallback(loadProgress);
                                }
                            }
                        });
                    }
                    else
                    {

                    }


                }
                else
                {
                    float time = (float)(DateTime.Now - startLoadTime).TotalMilliseconds;
#if UNITY_EDITOR
                    if (lastScene != null)
                    {
                        Debug.Log($"LoadScenesByBag Waiting time:{time}ms count:{count} totalCount:{totalScenesCount} scene:{lastScene.GetSceneName()} WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount}");
                    }
                    else
                    {
                        Debug.Log($"LoadScenesByBag Waiting time:{time}ms count:{count} totalCount:{totalScenesCount} scene:NULL WattingForLoadedAll:{WattingForLoadedAll.Count} WattingForLoadedCurrent:{WattingForLoadedCurrent.Count} LoadingSceneMaxCount:{LoadingSceneMaxCount}");
                    }
                    
#endif

                    //Debug.Log($"LoadScenesByBag Waiting time:{time} scene:{lastScene.GetSceneName()},currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count}");


                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadScenesByTag Exception:{ex}");
            }
            yield return new WaitForSeconds(0.02f);
        }

        if (isFinishedCallBack == false)
        {
            isFinishedCallBack = true;
            if (finishedCallback != null)
            {
                loadProgress.SetInfo(lastScene, 1, true);
                finishedCallback(loadProgress);
            }
            WriteLog("LoadScenesByBag", $"Finished2 count:{count} totalScenesCount:{totalScenesCount},\t time:{(DateTime.Now - start).ToString()}");
            OnAllLoaded();
        }
       
        Debug.Log($"[SubSceneManager.LoadScenesByBag] Finished currentList:{WattingForLoadedCurrent.Count} allList:{WattingForLoadedAll.Count} time:{(DateTime.Now - start).ToString()}");
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
        subScenes = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
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
#if UNITY_EDITOR
    public GameObject EditorSavePrefab(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("SavePrefab go == null");
            return null;
        }
        EditorHelper.UnpackPrefab(go);
        string prefabPath = $"Assets/{RootDir}/{SceneDir}/Prefabs/{go.name}[{go.GetInstanceID()}].prefab";
        EditorHelper.makeParentDirExist(prefabPath);
        GameObject assetObj= PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.UserAction);
        Debug.Log($"SavePrefab go:{go.name} asset:{assetObj} path:{prefabPath}");
        return assetObj;
    }

    public string EditorSavePrefabPath(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("SavePrefab go == null");
            return null;
        }
        EditorHelper.UnpackPrefab(go);
        string prefabPath = $"Assets/{RootDir}/{SceneDir}/Prefabs/{go.name}[{go.GetInstanceID()}].prefab";
        EditorHelper.makeParentDirExist(prefabPath);
        GameObject assetObj = PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.UserAction);
        Debug.Log($"SavePrefab go:{go.name} asset:{assetObj} path:{prefabPath}");
        return prefabPath;
    }
#endif

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

        public GameObject go;

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

            go = item.gameObject;
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

#if UNITY_EDITOR
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
#endif

        public override string ToString()
        {
            return $"[{isExist}][{isActive}][{fileLength:F2} M] {scenePath1}";
        }
    }

    

    public List<SceneFile> GetActiveSceneFiles()
    {
        List<SceneFile> sceneFiles1 = new List<SceneFile>();
        //var scenes = GetScenes();
        var scenes = UpdateScenes();
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetActiveSceneFiles", i, scenes.Length, scene));
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

    public List<GameObject> ErrorSceneGos = new List<GameObject>();

    public List<SceneFile> GetSceneFilesEx()
    {
        ErrorSceneGos = new List<GameObject>();
        List<SceneFile> sceneFiles = new List<SceneFile>();
        List<SceneFile> sceneFiles1 = GetActiveSceneFiles();
        Dictionary<string, SceneFile> dict = new Dictionary<string, SceneFile>();
        for (int i = 0; i < sceneFiles1.Count; i++)
        {
            SceneFile scene = sceneFiles1[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetSceneFilesEx1", i, sceneFiles1.Count, scene));
            if (dict.ContainsKey(scene.sceneFilePath))
            {
                SceneFile scene2=dict[scene.sceneFilePath];
                Debug.LogError($"dict.ContainsKey1 scene1:{scene2.go} scene2:{scene.go} path:{scene.sceneFilePath}");

                ErrorSceneGos.Add(scene2.go);
            }
            else{
                dict.Add(scene.sceneFilePath, scene);
            }
            
        }
        List<SceneFile> sceneFiles2 = GetAllSceneFiles();
        for (int i = 0; i < sceneFiles2.Count; i++)
        {
            SceneFile scene = sceneFiles2[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetSceneFilesEx2", i, sceneFiles1.Count, scene));
            scene.isActive = dict.ContainsKey(scene.sceneFilePath);
        }

        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"sceneFiles1:{sceneFiles1.Count} dict:{dict.Count} sceneFiles2:{sceneFiles2.Count}");
        return sceneFiles2;
    }

    public int maxDeleteCount = 100;

    public void DeleteInActiveScenes()
    {
#if UNITY_EDITOR
        DateTime start = DateTime.Now;

        float fileLength = 0;
        List<SceneFile> sceneFiles = GetSceneFilesEx();
        int count = 0;
        int maxCount = sceneFiles.Count;
        //if(maxDeleteCount>0&& maxDeleteCount<maxCount)
        //{
        //    maxCount = maxDeleteCount;
        //}

        StringBuilder deleteFilePaths = new StringBuilder();
        for (int i = 0; i < sceneFiles.Count; i++)
        {
            float progress = (float)i / sceneFiles.Count;
            if(ProgressBarHelper.DisplayCancelableProgressBar("DeleteInActiveScenes", $"{i}/{sceneFiles.Count} {progress:P1}",progress))
            {
                break;
            }
            SceneFile scene = sceneFiles[i];
            if (scene.isActive == false)
            {
                //scene.DeleteAsset();
                fileLength += scene.fileLength;
                count++;
                if(count> maxDeleteCount)
                {
                    break;
                }

                deleteFilePaths.AppendLine(scene.sceneFilePath);
            }
        }

        Debug.Log($"DeleteInActiveScenes time:{DateTime.Now-start} all:{sceneFiles.Count} remove:{count} size:{fileLength}");

        //EditorHelper.RefreshAssets();

        ProgressBarHelper.ClearProgressBar();
        string path = Application.dataPath + "/DeleteScenes.txt";
        FileInfo fileInfo = new FileInfo(path);
        Debug.Log($"path:{fileInfo.FullName} content:\n{deleteFilePaths.ToString()}");
        File.WriteAllText(fileInfo.FullName, deleteFilePaths.ToString());
#endif
    }

    [ContextMenu("ClearScenes")]
    public void ClearScenes()
    {

        var ssList = GameObject.FindObjectsOfType<SubScene_List>(true);
        foreach (var item in ssList)
        {
            item.Clear();
        }
        var ss = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
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

    public void LoadScenesAsync<T>(T[] scenes, Action<SceneLoadProgress> finishedCallbak) where T : SubScene_Base
    {
        DateTime start = DateTime.Now;
        OnProgressChanged(0);
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        int count = 0;
        SceneLoadProgress progressInfo = new SceneLoadProgress();
        if (scenes.Length == 0)
        {
            if (finishedCallbak != null)
            {
                progressInfo.SetInfo(null, 1, true);
                finishedCallbak(progressInfo);
            }
        }
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
                        progressInfo.SetInfo(null, 1, true);
                        finishedCallbak(progressInfo);
                    }
                    WriteLog("LoadScenesAsync",$"count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");
                    OnAllLoaded();
                }
                else{
                    if (finishedCallbak != null)
                    {
                        progressInfo.SetInfo(s, progress, false);
                        finishedCallbak(progressInfo);
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

    public void LoadScenesAsyncEx<T>(T[] scenes, Action<SceneLoadProgress> finishedCallbak) where T : SubScene_Base
    {
        StartCoroutine(LoadAllScenesCoroutine(scenes, finishedCallbak));
    }

    public float loadProgress = 0;

    
    IEnumerator LoadAllScenesCoroutine<T>(T[] scenes,Action<SceneLoadProgress> finishedCallbak) where T : SubScene_Base
    {
        //loadProgress = 0;
        OnProgressChanged(0);
        DateTime start = DateTime.Now;
        //subScenes = GameObject.FindObjectsOfType<SubScene>(true);
        SceneLoadProgress progressInfo = new SceneLoadProgress();
        for (int i = 0; i < scenes.Length; i++)
        {
            var subScene = scenes[i];
            //Debug.Log($"loadProgress:{loadProgress},scene:{subScene.GetSceneName()}");
            
            yield return subScene.LoadSceneAsyncCoroutine(null);

            var progress = (i + 0.0f) / scenes.Length;
            WriteLog("LoadAllScenesCoroutine", $"count:{scenes.Length} index:{i} progress:{progress} ");
            progressInfo.SetInfo(subScene, progress, false);
            OnProgressChanged(progress);
            if (finishedCallbak != null)
            {
                finishedCallbak(progressInfo);
            }
        }
        WriteLog("LoadAllScenesCoroutine",$"count:{scenes.Length},\t time:{(DateTime.Now - start).ToString()}");

        progressInfo.SetInfo(null, 1, true);

        if (finishedCallbak != null) finishedCallbak(progressInfo);
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

    [ContextMenu("UnLoadScene")]
    public void UnLoadScenes()
    {
        subScenes = GetSubScenes();
        UnLoadScenes(subScenes);
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
        else if (sceneType == SubSceneType.LODs)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_LODs>(true);
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
        //List<SubScene_Base> scenes = new List<SubScene_Base>();
        //if (contentType==SceneContentType.Part)
        //{
        //    var ss = GameObject.FindObjectsOfType<SubScene_Part>(true);
        //    foreach(var s in ss)
        //    {
        //        scenes.Add(s);
        //    }
        //}
        //else
        //{
        //    var ss = GameObject.FindObjectsOfType<SubScene_Single>(true);
        //    foreach (var s in ss)
        //    {
        //        scenes.Add(s);
        //    }
        //}
        //return scenes.ToArray();
        UpdateScenes();
        return subScenes;
    }

    public void UnLoadScenesAsync<T>(T[] scenes) where T : SubScene_Base
    {
        foreach (var item in scenes)
        {
            item.UnLoadSceneAsync();
        }
    }

    public void UnLoadScenes<T>(T[] scenes) where T : SubScene_Base
    {
        foreach (var item in scenes)
        {
            item.UnLoadGosM();
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
        //Debug.Log($"[{tag}]{log}");
    }
}

public class SceneLoadProgress
{
    public SubScene_Base scene;
    public float progress;
    public bool isAllFinished;
    public SceneLoadProgress()
    {

    }
    public SceneLoadProgress(SubScene_Base scene, float progress, bool isAllFinished)
    {
        SetInfo(scene, progress, isAllFinished);
    }

    public void SetInfo(SubScene_Base scene, float progress, bool isAllFinished)
    {
        this.scene = scene;
        this.progress = progress;
        this.isAllFinished = isAllFinished;
    }

    public override string ToString()
    {
        return $"scene:{scene} progress:{progress} isFinished:{isAllFinished}";
    }
}




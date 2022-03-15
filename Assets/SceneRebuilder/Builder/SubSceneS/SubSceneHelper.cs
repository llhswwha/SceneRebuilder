using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class SubSceneHelper
{
#if UNITY_EDITOR
    [ContextMenu("ClearBuildings")]
    public static void ClearBuildings()
    {
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[1];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        EditorBuildSettings.scenes = buildingScenes;
    }

    public static void SetBuildings()
    {
        SetBuildings<SubScene_Base>(true);
    }

    public static void SetBuildings<T>(bool includeInactive) where T : SubScene_Base
    {
        var subScenes = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
        SubSceneHelper.SetBuildings(subScenes);
    }

    public static void SetBuildingsWithNavmesh<T>(bool includeInactive) where T : SubScene_Base
    {
        var subScenes = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
        SubSceneHelper.SetBuildingWithNavmeshScene(subScenes);
    }

    public static void SetBuildings<T>(T[] scenes) where T : SubScene_Base
    {
        Debug.Log($"SetBuildings scenes:{scenes.Length}");

        List<EditorBuildSettingsScene> buildScenes1 = new List<EditorBuildSettingsScene>();
        int sceneId = 1;
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];

            EditorHelper.UnpackPrefab(item.gameObject);

            string path = item.sceneArg.GetRalativePath();
            //Debug.Log("path:" + path);
            EditorBuildSettingsScene scene= new EditorBuildSettingsScene(path, true);
            if (File.Exists(path) == false)
            {
                Debug.LogError($"scene[{i + 1}] scene:{scene} path:{path} enabled:{scene.enabled} Exists:{File.Exists(path)}");
            }
            else
            {
                //buildingScenes[sceneId] = scene;
                buildScenes1.Add(scene);
                item.sceneArg.index = sceneId;
                sceneId++;
            }
        }

        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[buildScenes1.Count + 1];

        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < buildScenes1.Count; i++)
        {
            buildingScenes[i + 1] = buildScenes1[i];
        }

        EditorBuildSettings.scenes = buildingScenes;

        Debug.Log($"SetBuildings totalScenes:{scenes.Length} sceneCount:{sceneId-1}");
    }

    public static void SetBuildingWithNavmeshScene<T>(T[] scenes) where T : SubScene_Base
    {
        //string navmeshPath = string.Format("{0}{1}", Application.dataPath, @"\Scenes\MinHang\MHNavmesh.unity");
        string navmeshPath = @"Assets\Scenes\MinHang\MHNavmesh.unity";
        if (!File.Exists(navmeshPath))
        {
            Debug.LogErrorFormat("Path:{0} not exist!", navmeshPath);
            return;
        }
        EditorBuildSettingsScene[] buildingScenes = new EditorBuildSettingsScene[scenes.Length + 2];
        buildingScenes[0] = new EditorBuildSettingsScene(EditorSceneManager.GetActiveScene().path, true);
        for (int i = 0; i < scenes.Length; i++)
        {
            T item = scenes[i];

            EditorHelper.UnpackPrefab(item.gameObject);

            string path = item.sceneArg.GetRalativePath();
            //Debug.Log("path:" + path);
            buildingScenes[i + 1] = new EditorBuildSettingsScene(path, true);
            item.sceneArg.index = i + 1;


        }
        buildingScenes[buildingScenes.Length - 1] = new EditorBuildSettingsScene(navmeshPath, true);
        EditorBuildSettings.scenes = buildingScenes;

        Debug.Log("SetBuildings:" + scenes.Length);
    }

    public static void CheckSceneIndex(bool includeInactive)
    {
        DateTime start = DateTime.Now;
        var alls = GameObject.FindObjectsOfType<SubScene_Base>(includeInactive);
        for (int i = 0; i < alls.Length; i++)
        {
            SubScene_Base s = alls[i];
            try
            {
                SceneLoadArg arg = s.GetSceneArg();
                if (arg.index <= 0)
                {
                    BuildingModelInfo modelInfo = s.GetComponentInParent<BuildingModelInfo>();
                    if (modelInfo != null)
                    {
                        Debug.LogError($"SubSceneShowManager.CheckSceneIndex index<=0 sName:{modelInfo.name}->{s.name} index:{s.sceneArg.index}");
                    }
                    else
                    {
                        Debug.LogError($"SubSceneShowManager.CheckSceneIndex index<=0 sName:NULL->{s.name} index:{s.sceneArg.index}");
                    }
                }
                else
                {
                    //EditorSceneManager.GetSceneByBuildIndex(arg.index);
                    //Scene scene = SceneManager.GetSceneByBuildIndex(arg.index);

                    //Scene scene = EditorSceneManager.GetSceneByBuildIndex(arg.index);
                    //if (arg.name != scene.name)
                    //{
                    //    Debug.LogError($"GetSceneByBuildIndex Error [arg:{arg}] scene:{scene.name} {scene.path} {scene.isLoaded}");
                    //}
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetSceneByBuildIndex Exception:{ex}");
            }
        }
        Debug.Log($"CheckSceneIndex Time:{(DateTime.Now - start).ToString()} scenes:{alls.Length}");
    }
#endif

    public static SubSceneBag GetScenes<T>(List<T> list) where T :Component
    {
        SubSceneBag scenes = new SubSceneBag();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item == null) continue;
            var ss = SubScene_List.GetBaseScenes(item.gameObject);
            scenes.AddRange(ss);
        }
        return scenes;
    }

    public static void LinkScenes(SubScene_Base[] scenes)
    {
        int count = scenes.Length;
        Debug.Log("LinkScenes:" + count);
        for (int i = 0; i < count; i++)
        {
            SubScene_Base scene = scenes[i];
            if (scene.LinkedScene == null)
            {
                for (int j = i + 1; j < count; j++)
                {
                    SubScene_Base scene2 = scenes[j];
                    Debug.Log($"LinkScenes sceneType:{scene.GetType()} sceneType2:{scene2.GetType()} {scene.GetType() == scene2.GetType()}");
                    if (scene.GetType() == scene2.GetType())
                    {
                        scene.LinkedScene = scene2;
                        scene2.LinkedScene = scene;
                        break;
                    }
                }
            }
        }
    }

    public static List<GameObject> GetChildrenGos(Transform parent)
    {
        List<GameObject> rootChildren = new List<GameObject>();
        //childCount = parent.childCount;
        for (int i = 0; i < parent.childCount; i++)
        {
            rootChildren.Add(parent.GetChild(i).gameObject);
        }
        return rootChildren;
    }

#if UNITY_EDITOR

    public static void EditorLoadScenes(GameObject root, Action<ProgressArg> progressChanged)
    {
        if (root == null)
        {
            return;
        }
        SubScene_Base[] scenes = SubScene_List.GetBaseScenes(root);
        EditorLoadScenes(scenes, progressChanged);
    }

    public static void EditorLoadScenes(SubSceneBag scenes, Action<ProgressArg> progressChanged)
    {
        EditorLoadScenes(scenes.ToArray(), progressChanged);
    }

    public static void EditorLoadScenes(SubScene_Base[] scenes, Action<ProgressArg> progressChanged)
    {
        Debug.Log("EditorLoadScenes:" + scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.IsLoaded = false;
            scene.EditorLoadSceneEx();
            ProgressArg p = new ProgressArg("EditorLoadScenes", i, scenes.Length, scene);
            if (progressChanged != null)
            {
                progressChanged(p);
            }
            else
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p))
                {
                    break;
                }
            }
        }
        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("EditorLoadScenes", scenes.Length, scenes.Length));
        }
        else
        {
            ProgressBarHelper.ClearProgressBar();
        }

        //this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
    }

    //[ContextMenu("SaveScene")]
    public static Scene SaveChildrenToScene(string path, Transform target, bool isOverride)
    {
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //var children = GetChildrenGos(target);
        SubSceneArg arg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene,true, target.gameObject);
        return CreateScene(arg);
        //scenePath = path;
    }

    public static Scene CreateScene(SubSceneArg arg)
    {
        return EditorHelper.CreateScene(arg.path, arg.isOveride, arg.isOpen, arg.objs.ToArray());
        //scenePath = path;
    }

    public static T EditorCreateScene<T>(GameObject go, SceneContentType contentType,bool isSave,GameObject dir) where T : SubScene_Base
    {
        if (go == null)
        {
            Debug.LogError("SubSceneHelper.EditorCreateScene go==null");
            return null;
        }
        string path = "";
        if (contentType==SceneContentType.TreeNode)
        {
            path = SubSceneManager.Instance.GetScenePath($"{go.name}", contentType, dir);
        }
        else
        {
            path = SubSceneManager.Instance.GetScenePath($"{go.name}[{go.GetInstanceID()}]", contentType, dir);
        }
        T scene= EditorCreateScene<T>(go, path, SubSceneManager.Instance.IsOverride, isSave, false);
        scene.contentType = contentType;
        return scene;
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride, bool isSave, bool isOnlyChildren) where T : SubScene_Base
    {
        //UpackPrefab_One(go);

        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        ////string path = GetScenePath(go.name, isPart);
        //ss.SetPath(path);
        //ss.Init();
        ////
        //SubSceneHelper.SaveChildrenToScene(path, go.transform, isOverride);
        //ss.ShowBounds();
        //return ss;

        return EditorCreateScene<T>(go, path, isOverride, isSave, isOnlyChildren,null);
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride, bool isSave, bool isOnlyChildren,T ss) where T : SubScene_Base
    {
        UpackPrefab_One(go);

        if (ss == null)
        {
            ss = go.AddComponent<T>();
        }
        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        //string path = GetScenePath(go.name, isPart);

        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //if(ss.sceneArg==null|| ss.sceneArg.objs==null|| ss.sceneArg.objs.Count==0)
            ss.sceneArg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, isOnlyChildren, go);

        //ss.SetPath(path);
        ss.Init();

        //SubSceneHelper.SaveChildrenToScene(path, go.transform, isOverride);
        if (isSave)
        {
            ss.SaveScene();
            ss.ShowBounds();
        }


        return ss;
    }

    public static void CreateSubScenes<T>(GameObject[] gameObjects) where T : SubScene_Base
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject obj = gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CreateSubScenes", i, gameObjects.Length, obj)))
            {
                break;
            }
            CreateSubScene<T>(obj);
        }
        EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();
    }

    public static T CreateSubScene<T>(GameObject obj) where T : SubScene_Base
    {
        if (obj == null) return null;
        T subScene = obj.GetComponent<T>();
        if (subScene == null)
        {
            subScene = obj.AddComponent<T>();
            subScene.IsLoaded = true;
        }
        subScene.EditorCreateScene(true);
        return subScene;
    }

    public static void EditorCreateScenes(List<SubScene_Base> scenes, Action<ProgressArg> progressChanged)
    {
        int count = scenes.Count;
        //Debug.Log("EditorCreateScenes:" + count);
        for (int i = 0; i < count; i++)
        {
            SubScene_Base scene = scenes[i];

            if (scene.gos.Count == 0)
            {
                Debug.LogError($"EditorCreateScenes scene.gos.Count == 0 Scene:{scene.name}");
                GameObject.DestroyImmediate(scene);
                continue;
            }
            //scene.IsLoaded = true;
            scene.SaveScene();
            scene.ShowBounds();

            //float progress = (float)i / count;
            //float percents = progress * 100;
            ProgressArg p1 = new ProgressArg("EditorCreateScenes", i, count, scene);
            if (progressChanged != null)
            {
                progressChanged(p1);
            }
            else
            {
                Debug.Log($"EditorCreateScenes {p1}");
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    break;
                }
            }
            //System.Threading.Thread.Sleep(1000);
        }

        EditorHelper.ClearOtherScenes();
        //EditorHelper.RefreshAssets();

        if (progressChanged == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("EditorCreateScenes",count,count));
        }
    }

    public static void UpackPrefab_One(GameObject go)
    {

        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }

    }
#endif

}

public enum SubSceneType
{
    Single, Part, Base, In, Out0, Out1,LODs
}

public enum SceneContentType
{
    Single,Part,Tree,TreeAndPart,TreeWithPart,TreeNode,LOD0,LODGroup,LODs,Ref
}

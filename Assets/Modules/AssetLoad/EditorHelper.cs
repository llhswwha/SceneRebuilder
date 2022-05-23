//using Base.Common;
using CommonUtils;
using Jacovone.AssetBundleMagic;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class EditorHelper
{
    public static void ForEachEx<T>(this List<T> buildings, string actionName, System.Action<T> actionContent)
    {
        System.DateTime start = System.DateTime.Now;
        for (int i = 0; i < buildings.Count; i++)
        {
            float progress = (float)i / buildings.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar(actionName, $"{i}/{buildings.Count} {percents:F2}% of 100%", progress))
            {
                break;
            }
            //buildings[i].LoadPrefab();
            actionContent(buildings[i]);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"ForEachEx1 {actionName} Buildings:{buildings.Count},Time:{(System.DateTime.Now - start).TotalMilliseconds}ms");
    }

    public static void ForEachEx<T>(this T[] buildings, string actionName, System.Action<T> actionContent)
    {
        System.DateTime start = System.DateTime.Now;
        for (int i = 0; i < buildings.Length; i++)
        {
            float progress = (float)i / buildings.Length;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar(actionName, $"{i}/{buildings.Length} {percents:F2}% of 100%", progress))
            {
                break;
            }
            //buildings[i].LoadPrefab();
            actionContent(buildings[i]);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"ForEachEx2 {actionName} Buildings:{buildings.Length},Time:{(System.DateTime.Now - start).TotalMilliseconds}ms");
    }

    

    public static void UnpackPrefab(GameObject go)
    {
#if UNITY_EDITOR
        UnpackPrefab(go, PrefabUnpackMode.Completely);
#endif
    }
    
    public static void Destroy(GameObject go)
    {
        UnpackPrefab(go);
        GameObject.DestroyImmediate(go);
    }



#if UNITY_EDITOR

    public static void UnpackPrefab(GameObject go, PrefabUnpackMode unpackMode)
    {
        if (go == null) return;
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, unpackMode, InteractionMode.UserAction);
        }
    }

    public static string GetMeshAssetDir(GameObject source)
    {
        if (source != null)
        {
            MeshFilter[] meshFilters = source.GetComponentsInChildren<MeshFilter>(true);
            foreach (var meshFilter in meshFilters)
            {
                if (UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                    string parentDir = EditorHelper.getParentPath(path);
                    Debug.Log($"GetMeshAssetDir sharedMesh:{meshFilter.sharedMesh}\npath:{path}\nparentDir:{parentDir}");
                    //isAllCreated = false;

                    //return false;

                    return parentDir+ "/MeshAssets";
                }
            }
        }

        return "Assets/Models/MeshAssets";
    }

    //public static string GetMeshPath(GameObject go)
    //{
    //    MeshFilter mf = go.GetComponent<MeshFilter>();
    //    if (mf == null) return "";
    //    string path = UnityEditor.AssetDatabase.GetAssetPath(mf.sharedMesh);
    //    return path;
    //}

    public static string SaveMeshAssetResource(GameObject go)
    {
        MeshFilter mf = go.GetComponent<MeshFilter>();
        return SaveMeshAssetResource(mf);
    }

        public static string SaveMeshAssetResource(MeshFilter meshFilter)
    {
        ////Mesh mesh = meshFilter.sharedMesh;
        //if (UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
        //{
        //    string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        //    string parentDir = EditorHelper.getParentPath(path);
        //    Debug.Log($"SaveMeshAsset sharedMesh:{meshFilter.sharedMesh}\npath:{path}\nparentDir:{parentDir}");
        //    //isAllCreated = false;
        //    //return false;
        //    //return parentDir + "/MeshAssets";
        //    return path;
        //}
        //else
        //{
        //    string dir= "Assets/Models/MeshAssets/Prefabs";

        //    string assetName = $"{meshFilter.sharedMesh.name}_{meshFilter.sharedMesh.GetInstanceID()}";

        //    Debug.Log($"SaveMeshAsset sharedMesh:{meshFilter.sharedMesh}\nassetName:{assetName}");

        //    SaveMeshAsset(dir, assetName, meshFilter);

        //    string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        //    return path;
        //}

        if (meshFilter == null)
        {
            Debug.LogError("meshFilter == null");
            return "";
        }

        if (meshFilter.sharedMesh == null)
        {
            Debug.LogError("meshFilter.sharedMesh == null:"+meshFilter);
            return "";
        }

        //string dir = "Assets/Resources/MeshAssets/Prefabs";

        //string assetName = $"{meshFilter.sharedMesh.name}_{meshFilter.sharedMesh.GetInstanceID()}";
        string assetName = RendererId.GetId(meshFilter.gameObject);

        Debug.Log($"SaveMeshAsset sharedMesh:{meshFilter.sharedMesh}\nassetName:{assetName}");

        return SaveMeshAsset(resourcesMeshDir_Full, assetName,true, meshFilter);

        //string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        //return $"{dir}/{assetName}";
    }

    public static void SaveMeshAsset(GameObject go)
    {
        string dir = GetMeshAssetDir(go); ;
        SaveMeshAsset(dir, go);
    }

    public static void SaveMeshAsset(GameObject source, GameObject go)
    {
        string dir = GetMeshAssetDir(source); ;
        SaveMeshAsset(dir, go);
    }

    public static void SaveMeshAsset(string dir, GameObject go)
    {
        MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
        SaveMeshAsset(dir, go, meshFilters);
    }

    private static bool IsAllMeshCreated(MeshFilter[] meshFilters)
    {
        //bool isAllCreated = false;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter meshFilter = meshFilters[i];

            if (!UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                Debug.Log($"SaveMeshAsset[{i}] sharedMesh:{meshFilter.sharedMesh},path:{path}");
                //isAllCreated = false;

                return false;
            }
            else
            {
                
            }
        }
        return true;
    }

    public static void SaveMeshAsset(string dir, GameObject go, params MeshFilter[] meshFilters)
    {
        string assetName = go.name + go.GetInstanceID();
        SaveMeshAsset(dir, assetName,false, meshFilters);
    }

    public static string SaveMeshAsset(string dir, string assetName, bool isCreateNew,params MeshFilter[] meshFilters)
    {
        makeParentDirExist(dir+"/");

        //string assetName = go.name + go.GetInstanceID();
        assetName = assetName.Replace("/", "_");//文件名中不能有/
        assetName = assetName.Replace("\\", "_");//文件名中不能有/
        assetName = assetName.Replace(":", "_");//文件名中不能有/
        assetName = assetName.Replace("*", "_");//文件名中不能有/
        assetName = assetName.Replace("?", "_");//文件名中不能有/
        assetName = assetName.Replace("<", "_");//文件名中不能有/
        assetName = assetName.Replace(">", "_");//文件名中不能有/
        assetName = assetName.Replace("|", "_");//文件名中不能有/

        string meshPath = dir + "/" + assetName + ".asset";
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning($"SaveMeshAsset meshFilters.Length == 0 assetName:{assetName} meshFilters:{meshFilters.Length}");
            return meshPath;
        }

        if (isCreateNew == false)
        {
            bool isAllCreated = IsAllMeshCreated(meshFilters);
            if (isAllCreated)
            {
                Debug.LogWarning($"SaveMeshAsset isAllCreated:{isAllCreated} assetName:{assetName} meshFilters:{meshFilters.Length}");
                return meshPath;
            }
        }

        if (meshFilters.Length ==1)
        {
            MeshFilter meshFilter = meshFilters[0];
            //Debug.LogError("meshFilter.sharedMesh :" + meshFilter.sharedMesh);
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogError("meshFilter.sharedMesh == null");
                return meshPath;
            }
            Mesh mesh = meshFilter.sharedMesh;
            if (isCreateNew)
            {
                //mesh = meshFilter.mesh;
                mesh = GameObject.Instantiate(meshFilter.sharedMesh);
                mesh.name = meshFilter.sharedMesh.name;
            }
            EditorHelper.SaveAsset(mesh, meshPath, false, isCreateNew);
        }
        else
        {
            Mesh meshAsset = new Mesh();
            //string assetName = tree.Target.name+"_"+gameObject.name + gameObject.GetInstanceID();

            EditorHelper.SaveAsset(meshAsset, meshPath, false, isCreateNew);

            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter meshFilter = meshFilters[i];
                //Debug.LogError("meshFilter.sharedMesh :" + meshFilter.sharedMesh);
                if (meshFilter.sharedMesh == null)
                {
                    Debug.LogError("meshFilter.sharedMesh == null");
                    continue;
                }
                Mesh mesh = meshFilter.sharedMesh;
                if (isCreateNew)
                {
                    //mesh = meshFilter.mesh;
                    mesh = GameObject.Instantiate(meshFilter.sharedMesh);
                    mesh.name = meshFilter.sharedMesh.name;
                }
                EditorHelper.SaveAsset(mesh, meshPath, true, isCreateNew);
            }
        }

        return meshPath;
    }

    public static bool SaveAsset(Object assetObj, string strFile, bool bAssetAlreadyCreated, bool isCreateNew)
    {
#if UNITY_EDITOR
        if (bAssetAlreadyCreated == false && UnityEditor.AssetDatabase.Contains(assetObj) == false)
        {
            Debug.Log($"CreateAsset1:{strFile}");
            UnityEditor.AssetDatabase.CreateAsset(assetObj, strFile);
            bAssetAlreadyCreated = true;
        }
        else
        {
            if (isCreateNew == false)
            {
                if (UnityEditor.AssetDatabase.Contains(assetObj) == false)
                {
                    Debug.Log($"AddObjectToAsset1:{strFile}");
                    UnityEditor.AssetDatabase.AddObjectToAsset(assetObj, strFile);
                    UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(assetObj));
                }
            }
            else
            {
                if (bAssetAlreadyCreated == false)
                {
                    Debug.Log($"CreateAsset2:{strFile}");
                    UnityEditor.AssetDatabase.CreateAsset(assetObj, strFile);
                    bAssetAlreadyCreated = true;
                }
                else
                {
                    Debug.Log($"AddObjectToAsset2:{strFile}");
                    UnityEditor.AssetDatabase.AddObjectToAsset(assetObj, strFile);
                    UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(assetObj));
                }
                //if (UnityEditor.AssetDatabase.Contains(assetObj) == false)
                {
                    
                }
            }
            
        }
#endif
        return bAssetAlreadyCreated;
    }


    public static void ClearOtherScenes()
    {
        //EditorSceneManager.
        //Debug.Log("ClearOtherScenes sceneCount:" + EditorSceneManager.sceneCount);
        Scene[] allScenes = SceneManager.GetAllScenes();
        for (int i = 1; i < allScenes.Length; i++)
        {
            float progress = (float)i / allScenes.Length;
            float percents = progress * 100;

            //if (ProgressBarHelper.DisplayCancelableProgressBar("ClearOtherScenes", $"{i}/{allScenes.Length} {percents:F2}% of 100%", progress))
            //{
            //    break;
            //}
            Scene scene = allScenes[i];
            EditorSceneManager.CloseScene(scene, true);
        }

        //ProgressBarHelper.ClearProgressBar();
    }

    public static string GetSceneName(string path)
    {
        try
        {
            string[] parts = path.Split(new char[] { '.', '\\', '/' });
            string sceneName = parts[parts.Length - 2];
            return sceneName;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"GetSceneName  path:{path},Exception:{ex}");
            return path;
        }
    }

    public static GameObject[] EditorLoadScene(Scene scene, string path, Transform parent)
    {
        string scenePath = EditorHelper.GetScenePath(path);
        string sceneName = GetSceneName(scenePath);
        Debug.Log($"LoadScene IsValid:{scene.IsValid()}, \tpath:{path}\nscenePath:{scenePath}\n{System.IO.File.Exists(scenePath)}");
        if (scene.IsValid() == false)//没有打开场景 > 打开
        {
            try
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"EditorLoadScene scenePath:{scenePath} Exception:{ex} ");
                return new GameObject[0];
            }
        }
        var objs = scene.GetRootGameObjects();
        if (parent)
        {
            //int idCount = 0;
            foreach (var obj in objs)
            {
                obj.transform.SetParent(parent);

                //var ids = obj.GetComponentsInChildren<RendererId>(true);//场景中的对象在场景创建时就应该保存Id信息的
                //idCount += ids.Length;
            }
            //Debug.LogError($"EditorLoadScene sceneName:{sceneName} objs:{objs.Length},idCount:{idCount}");

            EditorSceneManager.CloseScene(scene, true);

            IdDictionary.InitGos(objs, sceneName);
        }
        return objs;
    }

    private static Scene CreateScene(string scenePath,bool isOpen, params GameObject[] objs)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(activeScene);

        MoveGosToScene(scene, objs);

        bool result = EditorSceneManager.SaveScene(scene, scenePath);
        if(result==false)
        {
            Debug.LogError("CreateScene SaveSceneResult1:" + result);
        }

        //RefreshAssets();

        if (isOpen == false)
        {
            EditorSceneManager.CloseScene(scene, true);//保存后关闭
        }
        return scene;
    }

    public static void RefreshAssets()
    {
        Debug.LogError("RefreshAssets");
        AssetDatabase.Refresh();
    }

    //兼容主项目用的接口
    public static Scene CreateScene(GameObject objs, string scenePath)
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(activeScene);

        MoveGosToScene(scene, objs);

        bool result = EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("CreateScene  SaveSceneResult1:" + result);
        RefreshAssets();
        return scene;
    }

    public static Scene CreateScene(string path, bool isOveride,bool isOpen, params GameObject[] objs)
    {
        string scenePath = GetScenePath(path);

        if (objs.Length == 0)
        {
            Debug.LogError($"CreateScene1 objs.Length == 0 objs:{objs.Length},\tpath:{path},\nscenePath:{scenePath}");
            return new Scene();
        }

        if (objs.Length > 0)
        {
            Debug.Log($"CreateScene1 objs:{objs.Length},obj1:{objs[0]},\tpath:{path},\nscenePath:{scenePath}");
        }
        else
        {
            Debug.LogWarning($"CreateScene1 objs:{objs.Length},\tpath:{path},\nscenePath:{scenePath}");
        }
        

        FileInfo file = new FileInfo(scenePath);
        //Debug.Log($"CreateScene2 file:{file.FullName},\ndir:{file.Directory.FullName}\t{file.Directory.Exists}");
        if (file.Directory.Exists == false)
        {
            var path1 = file.FullName;
            var relativeDirPath = PathToRelative(path1);
            var relativeDirPath2 = relativeDirPath.Replace("\\", "/");
            Debug.Log($"CreateScene relativeDirPath:{relativeDirPath}\nrelativeDirPath2:{relativeDirPath2}");

            makeParentDirExist(relativeDirPath2);//创建文件夹
        }

        bool isExist = File.Exists(scenePath);
        //Debug.Log("isExist:" + isExist);

        //var asset = AssetDatabase.LoadMainAssetAtPath(scenePath);
        //Debug.Log("asset:" + asset);

        if (isExist == false)//场景不存在
        {
            return CreateScene(scenePath, isOpen,objs);
        }
        else
        {
            if (isOveride)//重新覆盖
            {
                Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                //Debug.Log("scene IsValid:" + scene.IsValid());
                if (scene.IsValid() == true)//打开
                {
                    bool r1 = EditorSceneManager.CloseScene(scene, true);//关闭场景，不关闭无法覆盖
                    Debug.Log("r1:" + r1);
                }
                return CreateScene(scenePath, isOpen, objs);
            }
            else
            {
                Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
                if (scene.IsValid() == false)//没有打开
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);//打开
                }
                MoveGosToScene(scene, objs);
                bool result2 = EditorSceneManager.SaveScene(scene, scenePath);
                Debug.Log("SaveSceneResult3:" + result2);
                //RefreshAssets();

                if (isOpen == false)
                {
                    EditorSceneManager.CloseScene(scene, true);//保存后关闭子场景
                }
                return scene;
            }
        }
    }

    public static void SelectObject(Object obj)
    {
        //EditorGUIUtility.Object
        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
        EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
    }

    public static void SelectObjects<T>(List<T> objs) where T : Object
    {
        SelectObjects(objs.ToArray());
    }

    public static void SelectObjects<T>(T[] objs) where T : Object
    {
        //EditorGUIUtility.Object
        if (objs.Length == 1)
        {
            //EditorGUIUtility.PingObject(objs[0]);
            SelectObject(objs[0]);
        }
        else
        {
            Selection.objects = objs;
            EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
        }
    }

    public static void SelectScriptObjects<T>(List<T> objs) where T : MonoBehaviour
    {
        List<GameObject> gos = new List<GameObject>();
        foreach(var obj in objs)
        {
            gos.Add(obj.gameObject);
        }
        SelectObjects(gos);
    }

    public static void SelectObjects(List<GameObject> objs)
    {
        SelectObjects(objs.ToArray());
    }

    public static void SelectObjects(GameObject[] objs)
    {
        Selection.objects = objs;
        EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
    }



    public static void SelectObjects<T>(IEnumerable<T> components) where T :Component
    {
        List<GameObject> objs = new List<GameObject>();
        foreach(var com in components)
        {
            if (com == null) continue;
            objs.Add(com.gameObject);
        }
        SelectObjects(objs.ToArray());
    }

    /// <summary>
    /// 得到模型所在路径
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetMeshPath(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("GetMeshPath failed...obj==null");
            return "";
        }
        var meshFilter = obj.GetComponentInChildren<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("GetMeshPath failed...meshFilter==null");
            return "";
        }
        var mesh = meshFilter.sharedMesh;
        var path = AssetDatabase.GetAssetPath(mesh);
        return path;
    }

    public static void SetAssetBundleName(string path, string assetName)
    {
        string relativePath = PathToRelative(path, null);
        //Object subObj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
        AssetImporter importer1 = AssetImporter.GetAtPath(relativePath);
        if (importer1 == null)
        {
            Debug.LogError("importer1 == null:" + relativePath);
        }
        else
        {
            importer1.assetBundleName = assetName;
        }
    }

    public static void makePrefab(string path, GameObject obj)
    {
        makeParentDirExist(path);
        Object prefab = PrefabUtility.CreateEmptyPrefab(path);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

    public static void makeParentDirExist(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"makeParentDirExist pDir == path");
            return;
        }
        string pDir = getParentPath(path);
        //Debug.Log($"makeParentDirExist path:{path}\npDir:{pDir}");
        if (pDir == path)
        {
            Debug.LogError($"makeParentDirExist pDir == path");
            return;
        }
        if (!AssetDatabase.IsValidFolder(pDir))
        {
            makeParentDirExist(pDir);
            AssetDatabase.CreateFolder(getParentPath(pDir), getParentName(path));
            //RefreshAssets();
        }
        else
        {
            return;
        }
    }

    public static void CreateDir(string rootDir, string newDir)
    {
        try
        {
            //string rootDir = RootDir;
            //string newDir = SceneDir;

            string parentDir = $"Assets/{rootDir}";
            if (string.IsNullOrEmpty(rootDir))
            {
                parentDir = "Assets";
            }

            Debug.Log("parentDir:" + parentDir);
            string dirPath = $"{parentDir}/{newDir}";
            Debug.Log("dirPath:" + dirPath);
            //UnityEditor.AssetDatabase
            string fullPath = $"{Application.dataPath}/{parentDir}/{newDir}";
            Debug.Log("fullPath:" + fullPath);

            bool isValidFolder = UnityEditor.AssetDatabase.IsValidFolder(dirPath);
            Debug.Log("isValidFolder:" + isValidFolder);

            if (isValidFolder == false)
            {
                //string parentDir = "Assets/Models/Instances/Trees";
                string guid = UnityEditor.AssetDatabase.CreateFolder(parentDir, newDir);
                Debug.Log("guid:" + guid);
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("path:" + path);
                RefreshAssets();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    public static string GetPrefabPath(GameObject obj)
    {
        var meshPath = EditorHelper.GetMeshPath(obj);
        //Debug.Log("meshPath:" + meshPath);//Assets/Models/SiHuiFactory/J1_Devices/主厂房设备8/主厂房设备8.FBX
        var dirPath = EditorHelper.GetDirPath(meshPath);
        //Debug.Log("dirPath:" + dirPath);
        //2.使用当前游戏物体创建预设，在该模型位置
        var prefatPath = dirPath + obj.name + ".prefab";
        return prefatPath;
    }

    public static string GetModelName(GameObject obj)
    {
        var path = EditorHelper.GetMeshPath(obj);//Assets/Models/SiHuiFactory/J1_Devices/主厂房设备8/主厂房设备8.FBX
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("string.IsNullOrEmpty(path):" + obj);
            return path;
        }
        var newPath = path;
        try
        {

            int id = path.LastIndexOf("/");
            if (id != -1)
            {
                newPath = path.Substring(id + 1);
            }
            int id2 = newPath.LastIndexOf(".");
            newPath = newPath.Substring(0, id2);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("path:" + path + "\n" + ex);
        }
        return newPath;
    }

    #region CopyComponent
    public static T CopyComponent<T>(GameObject fromObj, GameObject targetObj) where T : Component
    {
        T component = fromObj.GetComponent<T>();
        if (component != null)
        {
            CopyComponent(targetObj, component);
            T newComponent = targetObj.GetComponent<T>();
            return newComponent;
        }
        return null;
    }


    public static List<T> CopyComponents<T>(GameObject fromObj, GameObject targetObj) where T : Component
    {
        List<T> componentsNew = new List<T>();
        T[] components = fromObj.GetComponentsInChildren<T>();
        foreach (var component in components)
        {
            var cName1 = component.name;
            var cName2 = cName1 + "_Simple";
            var targetObjNew = targetObj.transform.GetChildByName(cName1);
            if (targetObjNew == null)
            {
                if (targetObj.name == cName1 || targetObj.name == cName2)
                {
                    targetObjNew = targetObj.transform;
                }
                else
                {
                    Debug.LogError("CopyComponents. targetObjNew == null : " + component.name);
                    continue;
                }
            }
            CopyComponent(targetObjNew.gameObject, component);
            T newComponent = targetObjNew.GetComponent<T>();
            componentsNew.Add(newComponent);
        }
        return componentsNew;
    }

    public static void CopyAllComponents(GameObject fromObj, GameObject targetObj, bool isClearAllOldComponents, params System.Type[] notCopyComponents)
    {
        if (fromObj == targetObj) return;
        if (isClearAllOldComponents)
        {
            RemoveComponents(targetObj,null);
        }

        Component[] components = fromObj.GetComponents<Component>();
        foreach (var component in components)
        {
            if (IsType(component.GetType(), notCopyComponents)) continue;

            if (component is MonoBehaviour)
            {
                CopyComponent(targetObj, component);
            }
            if (component is Collider)
            {
                if (component is MeshCollider)
                {
                    targetObj.AddComponent<MeshCollider>();
                }
                else
                {
                    CopyComponent(targetObj, component);
                }

            }
        }
    }

    public static void CopyComponent(GameObject targetObject, Component newComponent)
    {
        UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
        Component oldComponent = targetObject.GetComponent(newComponent.GetType());
        if (oldComponent != null)
        {
            if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
            {
                //Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Success");
            }
            else
            {
                Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Failed");
            }
        }
        else
        {
            if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
            {
                //Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Success");
            }
            else
            {
                Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Failed");
            }
        }
    }
    #endregion

#endif

    public static bool IsType(System.Type type1, System.Type[] typeList)
    {
        if (typeList == null) return false;
        foreach (var t in typeList)
        {
            if (t.IsAssignableFrom(type1))
            {
                return true;
            }
        }
        return false;
    }

    public static void RemoveAllComponents(GameObject targetObj, params System.Type[] notRemoveComponents)
    {
        Component[] components = targetObj.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component is Transform) continue;
            if (IsType(component.GetType(), notRemoveComponents)) continue;
            GameObject.DestroyImmediate(component);
        }
    }

    public static void RemoveComponents(GameObject targetObj, params System.Type[] notRemoveComponents)
    {
        Component[] components = targetObj.GetComponents<Component>();
        foreach (var component in components)
        {
            if (IsType(component.GetType(), notRemoveComponents)) continue;
            if (component is MonoBehaviour)
            {
                GameObject.DestroyImmediate(component);
            }
            if (component is Collider)
            {
                GameObject.DestroyImmediate(component);

            }
        }
    }

    public static Scene GetSceneByBuildIndex(SceneLoadArg arg,string tag)
    {
        if (arg.index <= 0)
        {
            Debug.LogError($"GetByIndex({tag}) arg.index<=0 sName:{arg.name} index:{arg.index}");
            return new Scene();
        }
        else
        {
            try
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(arg.index);
#if UNITY_EDITOR
                //Debug.Log($"GetByIndex({tag}) [arg:{arg}] scene:{scene.name}");
                if (arg.name != scene.name)
                {
                    Debug.Log($"GetByIndex({tag}) Error [arg:{arg.name}] scene:{scene.name}");
                }
#endif
                return scene;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"GetByIndex({tag}) Error [arg:{arg.name}] Exception:{ex}");
                return new Scene();
            }
            
        }
    }

    public static IEnumerator LoadSceneAsync(SubScene_Base sceneObj,SceneLoadArg arg, System.Action<float> progressChanged, System.Action<Scene> finished,bool isAutoUnload=false)
    {
        System.DateTime start = System.DateTime.Now;
        //Debug.Log("LoadSceneAsync:" + sName);
        if(arg.index<=0){
            Debug.LogError($"EditorHelper.LoadSceneAsync arg.index<=0 sName:{arg.name} index:{arg.index} sceneObj:{TransformHelper.GetPath(sceneObj)}");

            Scene scene = GetSceneByBuildIndex(arg, "LoadSceneAsync1");
            if (finished != null)
            {
                finished(scene);
            }
            yield return null;
        }
        else{
            AsyncOperation async = null;
            try
            {
                async = SceneManager.LoadSceneAsync(arg.index, LoadSceneMode.Additive);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"EditorHelper.LoadSceneAsync1 arg:{arg} Exception:{ex}");
            }
            
            if (async == null)
            {
                Debug.LogError($"EditorHelper.LoadSceneAsync2 async == null sName:{arg.name}");

                Scene scene = GetSceneByBuildIndex(arg, "LoadSceneAsync2");
                if (finished != null)
                {
                    finished(scene);
                }
                yield return null;
            }
            else
            {
                //async.allowSceneActivation = false;
                while (!async.isDone)
                {
                    if (progressChanged != null)
                    {
                        progressChanged(async.progress);
                    }
                    yield return null;
                }
                if (progressChanged != null)
                {
                    progressChanged(async.progress);
                }


                //SceneManager.GetSceneByBuildIndex
                //Scene scene = SceneManager.GetSceneByName(arg.name);
                // Scene scene = SceneManager.GetSceneByPath(arg.path);
                Scene scene = GetSceneByBuildIndex(arg, "LoadSceneAsync3");
                var objs = scene.GetRootGameObjects();
                if (objs.Length == 0)
                {
                    Debug.LogError($"LoadSceneAsync[{arg.name}] objs:{objs.Length} time:{(System.DateTime.Now - start).ToString()} objs.Length == 0");
                }
                else
                {
                    //Debug.Log($"LoadSceneAsync[{arg.name}] objs:{objs.Length} time:{(System.DateTime.Now - start).ToString()}");
                }
                
                if (finished != null)
                {
                    finished(scene);
                }

                //Debug.Log($"LoadSceneAsync[{sName}] time:{(System.DateTime.Now - start).ToString()}");

                if (isAutoUnload)
                {
                    yield return UnLoadSceneAsync(arg, null, null);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    public static IEnumerator UnLoadSceneAsync(SceneLoadArg arg, System.Action<float> progressChanged, System.Action finished)
    {
        System.DateTime start = System.DateTime.Now;
        //Debug.Log("UnLoadSceneAsync:" + sName);
        Scene scene = GetSceneByBuildIndex(arg, "UnLoadSceneAsync");
        if(scene.IsValid())
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync(arg.index, UnloadSceneOptions.None);
            //UnloadSceneOptions.

            //async.allowSceneActivation = false;
            while (async!=null && !async.isDone)
            {
                if (progressChanged != null)
                {
                    progressChanged(async.progress);
                }
                yield return null;
            }

            //yield return Resources.UnloadUnusedAssets();

            //System.GC.Collect();

            if (async != null)
            {
                if (progressChanged != null)
                {
                    progressChanged(async.progress);
                }
            }
            else
            {
                if (progressChanged != null)
                {
                    progressChanged(1);
                }
            }
        }
        //var objs = scene.GetRootGameObjects();
        if (finished != null)
        {
            finished();
        }


        //yield return Resources.UnloadUnusedAssets();
        //System.GC.Collect();
        //从内存卸载，没有的话，第二次开始不会从硬盘读取了。

        //Debug.Log($"UnLoadSceneAsync[{arg.name}] time:{(System.DateTime.Now - start).ToString()}");
        yield return null;
    }

    public static GameObject[] LoadScene(SceneLoadArg arg, Transform parent)
    {
        //SceneManager.LoadScene(arg, LoadSceneMode.Additive);
        SceneManager.LoadScene(arg.index, LoadSceneMode.Additive);
        return GetSceneObjects(arg, parent); ;
    }

    public static GameObject[] GetSceneObjects(SceneLoadArg arg, Transform parent)
    {
        string scenePath=arg.path;
        Scene scene = GetSceneByBuildIndex(arg, "GetSceneObjects");

        //Debug.Log($"GetSceneObjects scene:{scenePath},isLoaded:{scene.isLoaded},isValid:{scene.IsValid()}");

        if(scene.IsValid()){
            var objs = scene.GetRootGameObjects();
            if (parent)
            {
                if (objs.Length == 0)
                {
                    Debug.LogError($"GetSceneObjects scene:{scenePath},isLoaded:{scene.isLoaded},isValid:{scene.IsValid()}  objs.Length == 0");
                }
                foreach (var obj in objs)
                {
                    if(obj.name.Contains("WebMsgReciver"))
                    {
                        Debug.LogError($"EditorHelper.GetSceneObjects WebMsgReciver scene:{scene} index:{arg}");
                    }
                    obj.transform.SetParent(parent);
                }
                //EditorSceneManager.CloseScene(scene, true);

                IdDictionary.InitGos(objs,scenePath);
            }
            return objs;
        }
        else
        {
            Debug.LogError($"GetSceneObjects scene:{scenePath},isLoaded:{scene.isLoaded},isValid:{scene.IsValid()}");
            return new GameObject[1]{new GameObject("GetSceneObjectsError:"+scenePath)};
        }

    }

    

    private static void MoveGosToScene(Scene scene, params GameObject[] objs)
    {
        foreach(var obj in objs)
        {
            if (obj == null) continue;
            obj.transform.parent = null;
            SceneManager.MoveGameObjectToScene(obj, scene);
        }

    }

    public static string GetScenePath(string path)
    {
        if(string.IsNullOrEmpty(path))
        {
            return path;
        }

        string scenePath = path;
        if (path.Contains(":"))//绝对路径
        {

        }
        else //相对路径
        {
            scenePath = $"{Application.dataPath}/{path}";
        }
        return scenePath;
    }

    

    public static int GetVertexs(GameObject obj)
    {
        int vertexs = 0;
        MeshFilter[] mrs = obj.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (mrs[i].sharedMesh == null) continue;
            vertexs += mrs[i].sharedMesh.vertexCount;
        }
        return vertexs;
    }

    public static string GetDirPath(string path)//Assets/Models/SiHuiFactory/J1_Devices/主厂房设备8/主厂房设备8.FBX
    {
        int id = path.LastIndexOf("/");
        if (id == -1)
        {
            return path;
        }
        else
        {
            string dirPath = path.Substring(0, id + 1);
            return dirPath;
        }
    }

    public static string PathToRelative(string path, GameObject obj=null)
    {
        if (path.StartsWith("Assets"))//已经是相对路径
        {
            return path;
        }
        if(path.Contains(":") && path.Length >= Application.dataPath.Length + 1)
        //if ()
        {
            string relativePath = "Assets\\" + path.Substring(Application.dataPath.Length + 1);//这里必须加上Assets\\
            return relativePath;
        }
        else
        {
            //Debug.LogError("PathToRelative:" + path + "," + obj);
            string relativePath = "Assets\\" + path;//这里必须加上Assets\\
            return relativePath;
            //return path;
        }
    }

    static string resourcesMeshDir_Full = "Assets/Resources/MeshAssets/Prefabs";

    static string resourcesMeshDir = "MeshAssets/Prefabs";

    public static Mesh LoadResoucesMesh(string id)
    {
        //string meshPath = resourcesMeshDir + "/" + id + ".asset";
        string meshPath = resourcesMeshDir + "/" + id + "";
        Object obj = Resources.Load(meshPath);
        Mesh mesh = obj as Mesh;


        if (mesh != null)
        {
            //Debug.Log($"LoadResoucesMesh id:{id} meshPath:{meshPath} obj:{obj} mesh:{mesh} vc:{mesh.vertices.Length}");
        }
        else
        {
            Debug.LogError($"LoadResoucesMesh id:{id} meshPath:{meshPath} obj:{obj} mesh:{mesh}");
        }

        //var objs = Resources.LoadAll(resourcesMeshDir);
        //Debug.Log($"LoadResoucesMesh objs:{objs.Length}");
        //for (int i = 0; i < objs.Length; i++)
        //{
        //    Debug.Log($"obj[{i}]:{objs[i]}");
        //}

        return mesh;
    }

    #region makePrefab


    public static string getParentPath(string path, char splitFlag = '/')
    {
        string[] dirs = path.Split(splitFlag);
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < dirs.Length - 1; ++i)
        {
            str.Append(dirs[i]).Append(splitFlag);
        }
        //cut down the last splitFlag
        string result = str.ToString();
        if (result.EndsWith(splitFlag.ToString()))
        {
            result = result.Substring(0, result.Length - 1);
        }
        return result;
    }

    public static string getParentName(string path, char splitFlag = '/')
    {
        string[] dirs = path.Split(splitFlag);
        if (dirs.Length < 2)
        {
            return "";
        }
        else
        {
            return dirs[dirs.Length - 2];
        }
    }

    
    #endregion

    


}


//using Base.Common;
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
#if UNITY_EDITOR


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
        Debug.Log($"LoadScene IsValid:{scene.IsValid()}, \tpath:{path}\nscenePath:{scenePath}");
        if (scene.IsValid() == false)//没有打开场景 > 打开
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }
        var objs = scene.GetRootGameObjects();
        if (parent)
        {
            int idCount = 0;
            foreach (var obj in objs)
            {
                obj.transform.SetParent(parent);

                var ids = obj.GetComponentsInChildren<RendererId>(true);//场景中的对象在场景创建时就应该保存Id信息的
                idCount += ids.Length;
            }
            Debug.LogError($"EditorLoadScene sceneName:{sceneName} objs:{objs.Length},idCount:{idCount}");

            EditorSceneManager.CloseScene(scene, true);

            IdDictionay.InitRenderers(objs, sceneName);
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
        AssetDatabase.Refresh();
    }

    //public static Scene CreateScene(GameObject objs, string scenePath)
    //{
    //    Scene activeScene = EditorSceneManager.GetActiveScene();
    //    Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
    //    EditorSceneManager.SetActiveScene(activeScene);

    //    MoveGosToScene(scene, objs);

    //    bool result = EditorSceneManager.SaveScene(scene, scenePath);
    //    Debug.Log("CreateScene  SaveSceneResult1:" + result);
    //    RefreshAssets();
    //    return scene;
    //}

    public static Scene CreateScene(string path, bool isOveride,bool isOpen, params GameObject[] objs)
    {
        string scenePath = GetScenePath(path);
        Debug.Log($"CreateScene1 objs:{objs.Length},\tpath:{path},\nscenePath:{scenePath}");

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

    public static void SelectObject(GameObject obj)
    {
        EditorGUIUtility.PingObject(obj);
        Selection.activeGameObject = obj;
        EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
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
        }
        var meshFilter = obj.GetComponentInChildren<MeshFilter>();
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
        Debug.Log($"makeParentDirExist path:{path}\npDir:{pDir}");
        if (pDir == path)
        {
            Debug.LogError($"makeParentDirExist pDir == path");
            return;
        }
        if (!AssetDatabase.IsValidFolder(pDir))
        {
            makeParentDirExist(pDir);
            AssetDatabase.CreateFolder(getParentPath(pDir), getParentName(path));
            RefreshAssets();
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
                AssetDatabase.Refresh();
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

    public static IEnumerator LoadSceneAsync(string sName, System.Action<float> progressChanged, System.Action<Scene> finished,bool isAutoUnload=false)
    {
        System.DateTime start = System.DateTime.Now;
        //Debug.Log("LoadSceneAsync:" + sName);
        AsyncOperation async = SceneManager.LoadSceneAsync(sName, LoadSceneMode.Additive);
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

        Scene scene = SceneManager.GetSceneByName(sName);
        //var objs = scene.GetRootGameObjects();
        if (finished != null)
        {
            finished(scene);
        }

        Debug.Log($"LoadSceneAsync[{sName}] time:{(System.DateTime.Now - start).ToString()}");

        if (isAutoUnload)
        {
            yield return UnLoadSceneAsync(sName, null, null);
        }
        else
        {
            yield return null;
        }
    }

    public static IEnumerator UnLoadSceneAsync(string sName, System.Action<float> progressChanged, System.Action finished)
    {
        System.DateTime start = System.DateTime.Now;
        //Debug.Log("UnLoadSceneAsync:" + sName);
        Scene scene = SceneManager.GetSceneByName(sName);
        if(scene.IsValid())
        {
            AsyncOperation async = SceneManager.UnloadSceneAsync(sName, UnloadSceneOptions.None);
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

            if (progressChanged != null)
            {
                progressChanged(async.progress);
            }
        }

        //Scene scene = SceneManager.GetSceneByName(sName);
        //var objs = scene.GetRootGameObjects();
        if (finished != null)
        {
            finished();
        }


        //yield return Resources.UnloadUnusedAssets();
        //System.GC.Collect();
        //从内存卸载，没有的话，第二次开始不会从硬盘读取了。

        Debug.Log($"UnLoadSceneAsync[{sName}] time:{(System.DateTime.Now - start).ToString()}");
        yield return null;
    }

    public static GameObject[] LoadScene(string sceneName, Transform parent)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        return GetSceneObjects(sceneName, parent); ;
    }

    public static GameObject[] GetSceneObjects(string sceneName, Transform parent)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        
        
        var objs = scene.GetRootGameObjects();
        Debug.Log($"LoadScene scene:{sceneName},isLoaded:{scene.isLoaded},isValid:{scene.IsValid()},objs:{objs.Length}");
        if (parent)
        {
            foreach (var obj in objs)
            {
                obj.transform.SetParent(parent);
            }
            //EditorSceneManager.CloseScene(scene, true);

            IdDictionay.InitRenderers(objs,sceneName);
        }
        return objs;
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


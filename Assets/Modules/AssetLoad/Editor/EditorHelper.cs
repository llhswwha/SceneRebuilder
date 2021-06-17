using Base.Common;
using Jacovone.AssetBundleMagic;
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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class EditorHelper  {



    public static void ClearOtherScenes()
    {
        //EditorSceneManager.
        Debug.Log("sceneCount:" + EditorSceneManager.sceneCount);
        Scene[] allScenes = SceneManager.GetAllScenes();
        for (int i = 1; i < allScenes.Length; i++)
        {
            Scene scene = allScenes[i];
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    public static Scene CreateScene(GameObject obj, string scenePath)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        //scene.title
        Debug.Log("scenePath:" + scenePath);
        obj.transform.parent = null;
        SceneManager.MoveGameObjectToScene(obj, scene);
        bool result = EditorSceneManager.SaveScene(scene, scenePath);
        return scene;
    }

    public static void SelectObject(GameObject obj)
    {
        EditorGUIUtility.PingObject(obj);
        Selection.activeGameObject = obj;
        EditorApplication.ExecuteMenuItem("Edit/Frame Selected");
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

    public static string GetDirPath(string path)//Assets/Models/SiHuiFactory/J1_Devices/主厂房设备8/主厂房设备8.FBX
    {
        int id = path.LastIndexOf("/");
        if (id == -1)
        {
            return path;
        }
        else
        {
            string dirPath = path.Substring(0, id+1);
            return dirPath;
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
            Debug.LogError("string.IsNullOrEmpty(path):"+obj);
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
            Debug.LogError("path:"+path+"\n"+ex);
        }
        return newPath;
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

    public static string PathToRelative(string path, GameObject obj)
    {
        if (path.StartsWith("Assets"))//已经是相对路径
        {
            return path;
        }
        if (path.Length >= Application.dataPath.Length + 1)
        {
            string relativePath = "Assets\\" + path.Substring(Application.dataPath.Length + 1);//这里必须加上Assets\\
            return relativePath;
        }
        else
        {
            Debug.LogError("PathToRelative:" + path + "," + obj);
            return path;
        }
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
                Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Success");
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
                Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Success");
            }
            else
            {
                Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Failed");
            }
        }
    }
    #endregion

    #region makePrefab
    public static void makePrefab(string path, GameObject obj)
    {
        makeParentDirExist(path);
        Object prefab = PrefabUtility.CreateEmptyPrefab(path);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

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

    public static void makeParentDirExist(string path)
    {
        string pDir = getParentPath(path);

        if (!AssetDatabase.IsValidFolder(pDir))
        {
            makeParentDirExist(pDir);
            AssetDatabase.CreateFolder(getParentPath(pDir), getParentName(path));
            AssetDatabase.Refresh();
        }
        else
        {
            return;
        }
    }
    #endregion
}

#endif
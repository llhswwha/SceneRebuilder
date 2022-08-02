using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using CommonUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class InnerEditorHelper
{
    public static void SetExternalMaterial(GameObject prefab,object logTag)
    {
#if UNITY_EDITOR
        MeshRenderer mr = prefab.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            return;
        }
        bool isChanged = false;
        Material[] newMats = mr.sharedMaterials;
        for (int i = 0; i < mr.sharedMaterials.Length; i++)
        {
            Material mat = mr.sharedMaterials[i];
            string matPath = AssetDatabase.GetAssetPath(mat);
            //Debug.Log($"SetPrefabsMaterial[{i1}][{i}] {prefab.name}|{mat.name}|{matPath}");
            if (matPath.EndsWith(".fbx"))
            {
                string matPathNew = "";
                string[] pathParts = matPath.Split('/');
                for (int j = 0; j < pathParts.Length - 1; j++)
                {
                    matPathNew += pathParts[j] + "/";
                }
                matPathNew += $"Materials/{mat.name}.mat";
                Material matNew = AssetDatabase.LoadAssetAtPath<Material>(matPathNew);
                if (matNew != null)
                {
                    //mr.sharedMaterials[i] = matNew;
                    newMats[i] = matNew;
                    Debug.LogWarning($"SetExternalMaterial[{logTag}][{i}]2 {prefab.name}|{mat.name}\n{matPath}\n{matPathNew}\n{matNew}");
                    isChanged = true;
                }
                else
                {
                    Debug.LogError($"SetExternalMaterial[{logTag}][{i}]3 {prefab.name}|{mat.name}\n{matPath}\n{matPathNew}\n{matNew}");
                }
            }
            else
            {
                Debug.Log($"SetExternalMaterial[{logTag}][{i}]1 {prefab.name}|{mat.name}|{matPath}");
            }
        }
        if (isChanged)
        {
            mr.sharedMaterials = newMats;
            InnerEditorHelper.ChangePrefabMaterials(prefab.gameObject, newMats);

            //EditorSavePrefabPath(prefab.gameObject, true);
        }
#endif
    }

    public static void ChangePrefabMaterials(GameObject prefabObject, Material[] mats)
    {
#if UNITY_EDITOR
        PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefabObject);
        //Debug.Log($"ChangePrefabMaterials prefabObject:{prefabObject} prefabType:{prefabType}");
        if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
        {
            //GameObject prefabContents=PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;

            string prefabPath = AssetDatabase.GetAssetPath(prefabObject);
            if (string.IsNullOrEmpty(prefabPath))
            {
                //Debug.LogError($"ChangePrefabMaterials prefabObject:{prefabObject} prefabType:{prefabType} prefabPath:{prefabPath}");
                return;
            }
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(prefabPath);

            MeshRenderer mr = prefabContents.GetComponent<MeshRenderer>();
            mr.sharedMaterials = mats;

            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);

            Debug.Log($"ChangePrefabMaterials prefabObject:{prefabObject} prefabType:{prefabType}");
        }
        else
        {
            Debug.LogError($"ChangePrefabMaterials prefabObject:{prefabObject} prefabType:{prefabType}");
        }
#endif
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
#if UNITY_EDITOR
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
#endif
    }

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
            Debug.LogError("meshFilter.sharedMesh == null:" + meshFilter);
            return "";
        }

        //string dir = "Assets/Resources/MeshAssets/Prefabs";

        //string assetName = $"{meshFilter.sharedMesh.name}_{meshFilter.sharedMesh.GetInstanceID()}";
        string assetName = InnerRendererId.GetId(meshFilter.gameObject);

        Debug.Log($"SaveMeshAsset sharedMesh:{meshFilter.sharedMesh}\nassetName:{assetName}");

        return SaveMeshAsset(resourcesMeshDir_Full, assetName, true, meshFilter);

        //string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        //return $"{dir}/{assetName}";
    }

    public static string GetMeshAssetDir(GameObject source)
    {
        if (source != null)
        {
            MeshFilter[] meshFilters = source.GetComponentsInChildren<MeshFilter>(true);
            foreach (var meshFilter in meshFilters)
            {
#if UNITY_EDITOR
                if (UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                    string parentDir = getParentPath(path);
                    Debug.Log($"GetMeshAssetDir sharedMesh:{meshFilter.sharedMesh}\npath:{path}\nparentDir:{parentDir}");
                    //isAllCreated = false;

                    //return false;

                    return parentDir + "/MeshAssets";
                }
#endif
            }
        }

        return "Assets/Models/MeshAssets";
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
#if UNITY_EDITOR
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
#endif
        }
        return true;
    }

    public static void SaveMeshAsset(string dir, GameObject go, params MeshFilter[] meshFilters)
    {
        string assetName = go.name + go.GetInstanceID();
        SaveMeshAsset(dir, assetName, false, meshFilters);
    }

    public static string SaveMeshAsset(string dir, string assetName, bool isCreateNew, params MeshFilter[] meshFilters)
    {
        makeParentDirExist(dir + "/");

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

        if (meshFilters.Length == 1)
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
            SaveAsset(mesh, meshPath, false, isCreateNew);
        }
        else
        {
            Mesh meshAsset = new Mesh();
            //string assetName = tree.Target.name+"_"+gameObject.name + gameObject.GetInstanceID();

            SaveAsset(meshAsset, meshPath, false, isCreateNew);

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
                SaveAsset(mesh, meshPath, true, isCreateNew);
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


    public static List<T> CopyComponentsInChildren<T>(GameObject fromObj, GameObject targetObj) where T : Component
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

    public static T[] ClearAndCopyComponents<T>(GameObject fromObj, GameObject targetObj) where T : Component
    {
        T[] components1 = targetObj.GetComponents<T>();
        foreach(var com in components1)
        {
            GameObject.DestroyImmediate(com);
        }
        T[] components2 = fromObj.GetComponents<T>();
        foreach (var component in components2)
        {
            CopyComponent(targetObj, component);
        }
        T[] components3 = targetObj.GetComponents<T>();
        return components3;
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

    public static void CopyAllScriptsAndCollider(GameObject fromObj, GameObject targetObj)
    {
        if (fromObj == targetObj) return;
        Component[] components = fromObj.GetComponents<Component>();
        foreach (var component in components)
        {
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
#if UNITY_EDITOR
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
#endif
    }
    #endregion

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

    public static void UnpackPrefab(GameObject go)
    {
#if UNITY_EDITOR
        UnpackPrefab(go, PrefabUnpackMode.Completely);
#endif
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
#endif
}

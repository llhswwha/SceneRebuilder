using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorTool : MonoBehaviour
{
    [MenuItem("EditorTool/FocusSelection")]
    private static void FocusSelection()
    {
        EditorHelper.SelectObject(Selection.activeGameObject);
    }
        [MenuItem("EditorTool/Models/Delete")]
    private static void Delete()
    {
        print("Delete");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            Delete(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    private static void Delete(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        print("Delete Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //print("ChangeModel file:" + filePath);

            string relativePath = "Assets\\" + filePath.Substring(Application.dataPath.Length + 1);

            if (regex.IsMatch(relativePath))
            {
                string name = file.Name;
                string[] parts = name.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                //print("name:" + parts.Length + "|" + name);
                if (parts.Length == 6)
                {
                    string newName = string.Format("{0}({1})({2}){3}", parts[0], parts[1], parts[2], parts[5]);
                    print("name:" + newName + "<->" + name);
                    string assetPath = relativePath.Replace("\\", "/");
                    AssetDatabase.DeleteAsset(assetPath);//重复的
                }

                ////print(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                //if (subObj != null)
                //{

                //    modelObjects.Add(subObj);
                //}
            }
        }

        var time = DateTime.Now - start;

        print(string.Format("Rename 用时:{0}", time));
    }

    [MenuItem("EditorTool/Models/Rename")]
    private static void Rename()
    {
        print("Rename");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            Rename(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    private static void Rename(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        print("Rename Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //print("ChangeModel file:" + filePath);

            string relativePath = "Assets\\" + filePath.Substring(Application.dataPath.Length + 1);

            if (regex.IsMatch(relativePath))
            {
                string name = file.Name;
                string[] parts = name.Split(new char[]{ '(',')'},StringSplitOptions.RemoveEmptyEntries);
                //print("name:" + parts.Length + "|" + name);
                if (parts.Length == 6)
                {
                    string newName = string.Format("{0}({1})({2}){3}", parts[0], parts[1], parts[2], parts[5]);
                    print("name:" + newName + "<->" + name);
                    //string newPath = relativePath.Replace(name, newName);
                    //print("newPath:" + newPath + "<->" + relativePath);
                    string assetPath = relativePath.Replace("\\", "/");
                    //print("assetPath:" + assetPath);
                    //string assetPathNew = newPath.Replace("\\", "/");
                    //print("assetPathNew:" + assetPathNew);
                    string r=AssetDatabase.RenameAsset(assetPath, newName);
                    print("r:" + r );
                    if (r != "")
                    {
                        if (newName.Contains("_lod"))
                        {
                            break;
                        }
                        else
                        {
                            AssetDatabase.DeleteAsset(assetPath);//重复的
                            
                        }
                    }
                    //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                    //print("subObj:" + subObj);
                    //string assetpath = AssetDatabase.GetAssetPath(subObj);
                    //print("assetpath:" + assetpath);
                    //break;
                }

                ////print(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                //if (subObj != null)
                //{

                //    modelObjects.Add(subObj);
                //}
            }
        }

        var time = DateTime.Now - start;

        print(string.Format("Rename 用时:{0}", time));
    }

    [MenuItem("EditorTool/Models/CreatePrefab")]
    private static void CreatePrefab()
    {
        print("CreatePrefab");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreatePrefab(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    [MenuItem("EditorTool/Models/CreatePrefab(Test)")]
    private static void CreatePrefabTest()
    {
        print("CreatePrefab");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设

        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreatePrefab(dir, true);
        }
        EditorHelper.RefreshAssets();
    }

    private static List<GameObject> GetGameObject(DirectoryInfo dir,string filter)
    {
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(filter); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //print("ChangeModel file:" + filePath);

            int index = filePath.IndexOf(Application.dataPath);
            string relativePath = filePath.Substring(Application.dataPath.Length + 1);
            relativePath = "Assets\\" + relativePath;
            if (regex.IsMatch(relativePath))
            {
                //print(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                if (subObj != null)
                {
                    
                    modelObjects.Add(subObj);
                }
            }
        }
        return modelObjects;
    }

    private static void CreatePrefab(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        print("CreatePrefab Dir:" + dir);
        List<GameObject> modelObjects = GetGameObject(dir, ".(3DS|3ds|fbx|FBX)$");

        Dictionary<string, List<GameObject>> groups = new Dictionary<string, List<GameObject>>();
        foreach (GameObject obj in modelObjects)
        {
            string name = obj.name;
            int id = name.LastIndexOf(')');
            string groupName = name.Substring(0, id + 1);

            List<GameObject> list = null;
            if (!groups.ContainsKey(groupName))
            {
                list = new List<GameObject>();
                groups.Add(groupName, list);
            }
            else
            {
                list = groups[groupName];
            }

            list.Add(obj);
        }

        GameObject root = new GameObject();
        root.name = "PrefabRoot";

        Material[] lodMaterials = Resources.LoadAll<Material>("Materials/LODs");

        foreach (var groupKeyValue in groups)
        {
            string name = groupKeyValue.Key;
            var list = groupKeyValue.Value;

            for (int i = 0; i < list.Count; i++)
            {
                GameObject item = list[i];
                GameObject model = GameObject.Instantiate(item);
                model.transform.SetParent(root.transform);
                model.transform.position = Vector3.zero;
                model.name = item.name;
                
                if (isTest)
                {
                    if (item.name.ToLower().Contains("_lod1"))
                    {
                        model.name = name + "(test)_lod1";
                    }
                    else if (item.name.ToLower().Contains("_lod2"))
                    {
                        model.name = name + "(test)_lod2";
                    }
                    else if (item.name.ToLower().Contains("_lod3"))
                    {
                        model.name = name + "(test)_lod3";
                    }
                    else
                    {
                        model.name = name + "(test)";
                    }


                    //model.name += "[test]";
                    if (item.name.ToLower().Contains("_lod1"))
                    {
                        SetMaterial(model, lodMaterials[1]);
                    }
                    else if (item.name.ToLower().Contains("_lod2"))
                    {
                        SetMaterial(model, lodMaterials[2]);
                    }
                    else if (item.name.ToLower().Contains("_lod3"))
                    {
                        SetMaterial(model, lodMaterials[3]);
                    }
                    else
                    {
                        SetMaterial(model, lodMaterials[0]);
                    }
                }
                SavePrefab(model, "UnitsPrefab");
            }
        }

        var time = DateTime.Now - start;

        print(string.Format("CreatePrefab 用时:{0}", time));
    }

    private static void SetMaterial(GameObject obj,Material newMaterial)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            //for (int k = 0; k < renderer.sharedMaterials.Length; k++)
            //{
            //    renderer.sharedMaterials[k] = newMaterial;
            //}
            //没有效果

            //for (int k = 0; k < renderer.materials.Length; k++)
            //{
            //    renderer.materials[k] = newMaterial;
            //}
            //没运行时 会报错，运行了 也没有效果

            renderer.material = newMaterial;
        }
    }

    private static void SavePrefab(GameObject obj,string dirName)
    {
        string dir =Application.dataPath+ "/Resources/RvtScenes/"+ currentModelSceneName + "/Models/" + dirName + "/";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //Debug.LogError("dir:"+ dir);
        string localPath = "Assets/Resources/RvtScenes/"+ currentModelSceneName + "/Models/"+ dirName + "/" + obj.name + ".prefab";
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath); //自动重命名
        // Create the new Prefab.
        //string filePath = Application.dataPath + "/Resources/RvtScenes/滁州惠科中心/Models/" + dirName + "/" + obj.name + ".prefab";
        //print("filePath:" + filePath);
        //if (File.Exists(filePath))
        //{
        //    File.Delete(filePath);//避免自动修改文件名
        //}

        PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.UserAction);
    }



    [MenuItem("EditorTool/Models/CreateLODGroup")]
    private static void CreateLODGroup()
    {
        print("CreateLODGroup");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreateLODGroup(dir,4);

        }
        EditorHelper.RefreshAssets();
    }

    [MenuItem("EditorTool/Models/CreateLODGroup2")]
    private static void CreateLODGroup2()
    {
        print("CreateLODGroup2");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreateLODGroup(dir, 3);
        }
        EditorHelper.RefreshAssets();
    }

    private static Renderer[] CreateAndGetRenderers(GameObject pre,GameObject parent)
    {
        Renderer[] renderers = new Renderer[1];
        GameObject model = GameObject.Instantiate(pre);
        model.transform.SetParent(parent.transform);
        model.transform.position = Vector3.zero;
        model.name = pre.name;
        Renderer render = model.GetComponent<Renderer>();
        //print(string.Format("{0},{1}", i, render));
        renderers[0] = render;
        return renderers;
    }

    private static void CreateLODGroup(DirectoryInfo dir,int lodCount)
    {
        var start = DateTime.Now;
        print("CreateLODGroup Dir:" + dir);
        List<GameObject> modelObjects = GetGameObject(dir, ".(3DS|3ds|fbx|FBX|prefab|Prefab)$");

        Dictionary<string, List<GameObject>> groups = new Dictionary<string, List<GameObject>>();
        foreach(GameObject obj in modelObjects)
        {
            string name = obj.name;
            //if (!name.ToLower().Contains("lod"))
            //{
            //    continue;
            //}
            int id = name.LastIndexOf(')');

            string groupName = name.Substring(0, id+1);
            List<GameObject> list = null;
            if (!groups.ContainsKey(groupName))
            {
                list = new List<GameObject>();
                groups.Add(groupName, list);            
            }
            else
            {
                list = groups[groupName];
            }
            list.Add(obj);
        }

        GameObject root = new GameObject();
        root.name = "LODRoot";

        foreach (var groupKeyValue in groups)
        {
            string name = groupKeyValue.Key;
            var list = groupKeyValue.Value;

            print("CreateLODGroup type name:" + name);

            GameObject obj = new GameObject();
            obj.name = name + "_lod";
            obj.transform.SetParent(root.transform);
            LODGroup lodGroup = obj.AddComponent<LODGroup>();

             if (list.Count == 4)
            {
                LOD[] lods = new LOD[lodCount];

                float[] lodHeightList = new float[] { 0.1f, 0.05f, 0.01f, 0.0001f };

                if (lodCount == 3)
                {
                    lodHeightList = new float[] { 0.1f, 0.05f, 0.0001f };
                    obj.name = name + "_lod_low";

                    for (int i = 1, j = 0; i < list.Count; i++, j++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);

                        LOD lod = new LOD(lodHeightList[j], renderers);
                        lods[j] = lod;
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);
                        LOD lod = new LOD(lodHeightList[i], renderers);
                        lods[i] = lod;
                    }
                }
                lodGroup.SetLODs(lods);
            }
            if (list.Count == 3)
            {
                //lodCount--;
                int newLodCount = lodCount - 1;////4->3 3->2

                LOD[] lods = new LOD[newLodCount];

                float[] lodHeightList = new float[] { 0.1f, 0.05f, 0.0001f };

                if (newLodCount == 2)
                {
                    lodHeightList = new float[] { 0.1f, 0.0001f };
                    obj.name = name + "_lod_low";

                    for (int i = 1, j = 0; i < list.Count; i++, j++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);

                        LOD lod = new LOD(lodHeightList[j], renderers);
                        lods[j] = lod;
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);
                        LOD lod = new LOD(lodHeightList[i], renderers);
                        lods[i] = lod;
                    }
                }
                lodGroup.SetLODs(lods);
            }

            

            
            lodGroup.RecalculateBounds();

            SavePrefab(obj,"UnitsLOD");
        }

        var time = DateTime.Now - start;

        print(string.Format("CreateLODGroup 用时:{0}", time));
    }

    [MenuItem("EditorTool/Models/GetModelInfo")]//清空AssetBunldeName
    private static void GetChangeModelMenu()
    {
        print("ChangeModelMenu");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            FindModelInDir(dir, false,0,false);
        }
        EditorHelper.RefreshAssets();
    }
    private static void SetChangeModelInner(bool isSet, int maxCount, bool isSetMat)
    {
        print("ChangeModelMenu");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            FindModelInDir(dir, isSet, maxCount, isSetMat);
        }
        EditorHelper.RefreshAssets();
    }


    [MenuItem("EditorTool/Models/ChangeModel(100)")]//清空AssetBunldeName
    private static void SetChangeModel100()
    {
        SetChangeModelInner(true, 100, true);
    }

    [MenuItem("EditorTool/Models/ChangeModel(1000)")]//清空AssetBunldeName
    private static void SetChangeModel1000()
    {
        SetChangeModelInner(true,1000,true);
    }

    [MenuItem("EditorTool/Models/ChangeModel(All)")]
    private static void SetChangeModelAll()
    {
        SetChangeModelInner(true, 0, true);
    }

    [MenuItem("EditorTool/Models/ChangeModel(Selection)")]
    private static void SetChangeModelSelection()
    {
        SetChangeModelSelectionInner(true);
    }

    [MenuItem("EditorTool/Models/EnableReadWrite(100)")]//清空AssetBunldeName
    private static void EnableReadWrite100()
    {
        SetChangeModelInner(true, 100, false);
    }

    [MenuItem("EditorTool/Models/EnableReadWrite(1000)")]//清空AssetBunldeName
    private static void EnableReadWrite1000()
    {
        SetChangeModelInner(true, 1000, false);
    }

    [MenuItem("EditorTool/Models/EnableReadWrite(All)")]
    private static void EnableReadWriteAll()
    {
        SetChangeModelInner(true, 0, false);
    }

    [MenuItem("EditorTool/Models/EnableReadWrite(Selection)")]
    private static void EnableReadWriteSelection()
    {
        SetChangeModelSelectionInner(false);
    }

    private static void SetChangeModelSelectionInner(bool isSetMat)
    {
        var start = DateTime.Now;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        print("selectedAssets:" + selectedAssets.Length + "|" + selectedAssets);

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            var assetObj = selectedAssets[i];
            float progress = (float)i / selectedAssets.Length;
            float percent = progress * 100;
            if (EditorUtility.DisplayCancelableProgressBar("SetChangeModelSelection", $"{i}/{selectedAssets.Length} {percent}% of 100%", progress))
            {

                break;
            }

            string sPath = AssetDatabase.GetAssetPath(assetObj);
            var atImporter = ModelImporter.GetAtPath(sPath);
            if (atImporter == null)
            {
                Debug.LogError($"atImporter == null");
            }
            else if (atImporter is ModelImporter)
            {
                ModelImporter modelImporter = (ModelImporter)atImporter;

                Debug.Log($"[{i}] {assetObj} \t{sPath} \t{modelImporter} \t{modelImporter.isReadable} \t{modelImporter.materialLocation}");

                //if (modelImporter.isReadable == false || modelImporter.materialLocation == ModelImporterMaterialLocation.InPrefab)
                //{
                //    modelImporter.isReadable = true;
                //    //importer.
                //    if (isSetMat)
                //    {
                //        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                //    }
                //    AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                //}
                bool isChanged = false;
                if (modelImporter.isReadable == false)
                {
                    modelImporter.isReadable = true;
                    isChanged = true;
                }
                if (isSetMat)
                {
                    if (modelImporter.materialLocation == ModelImporterMaterialLocation.InPrefab)
                    {
                        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                        isChanged = true;
                    }
                }
                if (isChanged)
                {
                    AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                }
            }
            else
            {
                Debug.LogError($"atImporter:{atImporter.GetType()} path:{sPath} obj:{assetObj}");
            }

        }
        EditorUtility.ClearProgressBar();
        Debug.Log($"SetChangeModelSelection {(DateTime.Now - start).ToString()}");
    }

    [MenuItem("EditorTool/Models/GPUI")]
    private static void SetModelGPUI()
    {
        var start=DateTime.Now;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        print("selectedAssets:"+ selectedAssets.Length+"|"+ selectedAssets);

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            var assetObj=selectedAssets[i];
            float progress=(float)i/selectedAssets.Length;
            float percent=progress*100;
            if(EditorUtility.DisplayCancelableProgressBar("SetChangeModelSelection",$"{i}/{selectedAssets.Length} {percent}% of 100%",progress))
            {

                break;
            }

            string sPath = AssetDatabase.GetAssetPath(assetObj);
            ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(sPath);

            Debug.Log($"[{i}] {assetObj} \t{sPath} \t{importer} \t{importer.isReadable} \t{importer.materialLocation}");

            if (importer.isReadable==false || importer.materialLocation== ModelImporterMaterialLocation.InPrefab)
            {
                importer.isReadable = true;
                //importer.
                importer.materialLocation = ModelImporterMaterialLocation.External;
                AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log($"SetChangeModelSelection {(DateTime.Now-start).ToString()}");
    }

    private static string currentModelSceneName = "XXXX";

    /// <summary>
    /// 获取选中文件夹
    /// </summary>
    /// <returns></returns>
    private static List<DirectoryInfo> GetDirObjs()
    {
        currentModelSceneName = "XXXX";
        print("GetDirObjs");
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        print("selectedAssets:"+ selectedAssets.Length+"|"+ selectedAssets);
        List<DirectoryInfo> dirObjs = new List<DirectoryInfo>();
        foreach (Object obj in selectedAssets)
        {
            //print("obj:" + obj);
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //print("sourcePath:" + sourcePath);

            if (Directory.Exists(sourcePath)) //判断是否是文件夹
            {
                //var dir = GetDirInfo(dirObj);
                dirObjs.Add(new DirectoryInfo(sourcePath));
                print("add Dir1:" + sourcePath);
            }
        }
        if (dirObjs.Count == 0)//选中的是文件(模型)
        {
            foreach (Object obj in selectedAssets)
            {
                //print("obj:" + obj);
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                //print("sourcePath:" + sourcePath);

                if (File.Exists(sourcePath)) //判断是否是文件夹
                {
                    FileInfo fi = new FileInfo(sourcePath);
                    if (!dirObjs.Contains(fi.Directory))
                    {
                        dirObjs.Add(fi.Directory);

                        print("add Dir2:" + fi.Directory);
                    }

                    if (fi.Directory.Name == "Units")
                    {
                        currentModelSceneName = fi.Directory.Parent.Parent.Name;
                    }
                    
                }
            }
        }
        return dirObjs;
    }
    /// <summary>
    /// 设置文件夹Asset名称
    /// </summary>
    /// <param name="dir"></param>
    private static void FindModelInDir(DirectoryInfo dir,bool isSet,int maxCount, bool isSetMat)
    {
        Debug.Log(string.Format("SetDirAssetName:{0}",dir.FullName));
        DirectoryInfo[] subDirs = dir.GetDirectories();
        //if (subDirs.Length == 1 && subDirs[0].Name == "Materials")
        //{
        //    ChangeModel(dir);
        //}
        //else
        //{
            ChangeModel(dir, isSet, maxCount, isSetMat);
            //foreach (DirectoryInfo subDir in subDirs)
            //{
            //    FindModelInDir(subDir);
            //}
        //}
    }
    /// <summary>
    /// 获取文件夹信息
    /// </summary>
    /// <param name="dirObj"></param>
    /// <returns></returns>
    private static DirectoryInfo GetDirInfo(Object dirObj)
    {
        string sourcePath = AssetDatabase.GetAssetPath(dirObj);
        DirectoryInfo dir = new DirectoryInfo(sourcePath); //获取文件夹
        return dir;
    }

    private static void ChangeModel(DirectoryInfo dir,bool isSetMatAndReadWrite,int maxCount,bool isSetMat)
    {
        var start = DateTime.Now;
        print("ChangeModel Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        List<Object> subObjects = new List<Object>();
        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<ModelImporter> models = new List<ModelImporter>();
        List<string> pathList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //print("ChangeModel file:" + filePath);

            int index = filePath.IndexOf(Application.dataPath);
            string relativePath = filePath.Substring(Application.dataPath.Length + 1);
            relativePath = "Assets\\" + relativePath;
            if (regex.IsMatch(relativePath))
            {
                print(string.Format("GetModelInfo ({0}/{1}) file :{2}",i,files.Length,filePath));
                Object subObj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
                string sPath = AssetDatabase.GetAssetPath(subObj);
                ModelImporter importer = ModelImporter.GetAtPath(sPath) as ModelImporter;
                if (importer != null)
                {
                    //if (importer.globalScale == 0.1f) importer.globalScale = 0.7f;
                    //else if (importer.globalScale == 0.3f) importer.globalScale = 3.3f;
                    if (importer.isReadable==false || importer.materialLocation== ModelImporterMaterialLocation.InPrefab)
                    {
                        models.Add(importer);
                        pathList.Add(sPath);

                        //importer.isReadable = true;
                        ////importer.
                        //importer.materialLocation = ModelImporterMaterialLocation.External;
                        //AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                    }
                }
                else
                {

                }

                //MeshImporter.
            }
        }

        print(string.Format("models:{0}", models.Count));
        if (isSetMatAndReadWrite)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (maxCount > 0 && i <= maxCount) break;
                var importer = models[i];
                var sPath = pathList[i];
                print(string.Format("SetModelInfo({0}/{1}) file :{2}", i, files.Length, sPath));

                importer.isReadable = true;
                //importer.
                if (isSetMat)
                {
                    importer.materialLocation = ModelImporterMaterialLocation.External;
                }
                AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
            }
        }

        var time = DateTime.Now - start;

        print(string.Format("ChangeModel 用时:{0}", time));
    }

    [MenuItem("EditorTool/Models/LoadModels")]//清空AssetBunldeName
    private static void LoadModels()
    {
        print("LoadModels");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            print("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            ////var dir = GetDirInfo(dirObj);
            //FindModelInDir(dir);

            
        }
       
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("EditorTool/Collider/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("EditorTool/Collider/AddBoxCollider_IsTrigger")]
    public static void AddBoxCollider_IsTrigger()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("EditorTool/Collider/AddBoxCollider_IsTrigger_NotRemoveChild")]
    public static void AddBoxCollider_IsTrigger_NotRemoveChild()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform, false);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 添加所有的MeshCollider
    /// </summary>
    [MenuItem("EditorTool/Collider/AddAllMeshCollider")]
    public static void AddAllMeshCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        AddAllMeshCollider(parent);
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        var meshFilters = parent.gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }
}


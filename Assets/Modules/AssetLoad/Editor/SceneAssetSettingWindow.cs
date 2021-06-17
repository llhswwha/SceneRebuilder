using Base.Common;
using Jacovone.AssetBundleMagic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class SceneAssetSettingWindow : EditorWindow {

    [XmlType("ObjectSceneName")]
    public class ObjectSceneName
    {
        [XmlAttribute]
        public string ObjectName { get; set; }

        [XmlAttribute]
        public string SceneName { get; set; }
    }

    [XmlRoot("ObjectSceneNameList")]
    public class ObjectSceneNameList : List<ObjectSceneName>
    {

    }


	// Use this for initialization
    [MenuItem("Tools/ModelAsset/BuildingSetting...")]
	static void CreateWindow () {
        var window = (SceneAssetSettingWindow)EditorWindow.GetWindow(typeof(SceneAssetSettingWindow));
        window.Show();
	}

    private Vector2 scrollPos;

    private Dictionary<string, SceneAssetInfo> infos = new Dictionary<string, SceneAssetInfo>();

    private Dictionary<MonoBehaviour, int> vertexCountDic = new Dictionary<MonoBehaviour, int>();

    private int GetVertexCount(MonoBehaviour b)
    {
        if (vertexCountDic.ContainsKey(b))
        {
            return vertexCountDic[b];
        }
        int count = GetVertexs(b.gameObject);
        vertexCountDic[b] = count;
        return count;
    }

    //private int SortBuilding(BuildingController a, BuildingController b)
    //{
    //    var result = 0;
    //    if (IsSortByVertextCount)
    //    {
    //        var count1 = GetVertexCount(a);
    //        var count2 = GetVertexCount(b);
    //        result = count2.CompareTo(count1);
    //    }
    //    else if (IsSortByName)
    //    {
    //        result = a.gameObject.name.CompareTo(b.gameObject.name);
    //    }
    //    else
    //    {
    //        var parent1 = a.gameObject.transform.parent;
    //        var parent2 = b.gameObject.transform.parent;
    //        if (parent1 != null && parent2 != null)
    //        {
    //            var result1 = parent1.name.CompareTo(parent2.name);
    //            if (result1 == 0)
    //            {
    //                result = a.gameObject.name.CompareTo(b.gameObject.name);
    //            }
    //            else
    //            {
    //                result = result1;
    //            }
    //        }
    //        else
    //        {
    //            result = a.gameObject.name.CompareTo(b.gameObject.name);
    //        }
    //    }
    //    return result;
    //}

    public static int GetVertexs(GameObject obj)
    {
        return EditorHelper.GetVertexs(obj);
    }

    public static void SelectObject(GameObject obj)
    {
        EditorHelper.SelectObject(obj);
    }

    private SceneAssetInfo GetSceneAssetInfo(BuildingController building)
    {
        var objName = building.gameObject.name;
        SceneAssetInfo info = null;
        if (infos.ContainsKey(objName))
        {
            info = infos[objName];
        }
        else
        {
            info = new SceneAssetInfo(building);
            infos[objName] = info;
        }
        return info;
    }

    private SceneAssetInfo GetSceneAssetInfo(BuildingAssetInfo building)
    {
        var objName = building.gameObject.name;
        SceneAssetInfo info = null;
        if (infos.ContainsKey(objName))
        {
            info = infos[objName];
        }
        else
        {
            info = new SceneAssetInfo(building);
            infos[objName] = info;
        }
        return info;
    }

    public ColumnSortComponent sortComponent = new ColumnSortComponent();


    //int idColumnWidth = 20;
    //int sceneColumnWidth = 50;
    //int countColumnWidth = 60;
    //int parentColumnWidth = 150;
    //int nameColumnWidth = 70;
    //int nodeColumnWidth = 120;

    string loadDistanceBiasText = "0.6";
    string unloadDistanceBiasText = "1.2";

    float loadDistanceBias = 0.6f;
    float unloadDistanceBias = 1.2f;

    private string GetScenePath(string sceneName)
    {
        return Application.dataPath + "/Modules/AssetLoad/AssetScenes/AutoCreate/" + sceneName + ".unity";
    }

    public Dictionary<string, GameObject> selectedObjects = new Dictionary<string, GameObject>();

    // Update is called once per frame
    void OnGUI()
    {
        sortComponent.OnGUI();

        EditorGUILayout.BeginHorizontal();
        loadDistanceBiasText = GUILayout.TextField(loadDistanceBiasText, GUILayout.Width(50));
        unloadDistanceBiasText = GUILayout.TextField(unloadDistanceBiasText, GUILayout.Width(50));
        loadDistanceBias = float.Parse(loadDistanceBiasText);
        unloadDistanceBias = float.Parse(unloadDistanceBiasText);
        EditorGUILayout.EndHorizontal();

        FactoryDepManager root = GameObject.FindObjectOfType<FactoryDepManager>();
        if (root)
        {
            var allCount = GetVertexs(root.gameObject);
            GUILayout.Label("AllCount:" + allCount, EditorStyles.boldLabel);
        }
       
        GUILayout.Label("BuildingList:", EditorStyles.boldLabel);

        
        

        GUI_InitTableComponent();

        var heightOffset = 180;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - heightOffset));//开始滚动条
        //var buildings = Resources.FindObjectsOfTypeAll<BuildingController>().ToList();
        var buildings = new List<BuildingAssetInfo>();
        var list1 = GameObject.FindObjectsOfType<BuildingAssetInfo>().ToList();
        list1.Sort(sortComponent.SortMethod);
        for (int i = 0; i < list1.Count; i++)
        {
            var building = list1[i];
            var meshRenderer = building.gameObject.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null) continue;
            buildings.Add(building);
        }

        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            var obj = building.gameObject;

            //GUILayout.Toggle(meshRenderer != null, "MeshRenderer", GUILayout.Width(100));

            var objName = obj.name;
            var parent = obj.transform.parent;
            var info = GetSceneAssetInfo(building);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("{0:00}", i + 1), tableComponent.GetGUIWidth("ID"));
            
            info.SceneName = GUILayout.TextField(info.SceneName, tableComponent.GetGUIWidth("Scene"));
            var sceneName = info.SceneName;
            var vertexCount = GetVertexs(obj);
            GUILayout.Label(vertexCount + "", tableComponent.GetGUIWidth("Count"));
            if (parent != null)
            {
                if (GUILayout.Button(parent.name, tableComponent.GetGUIWidth("Parent")))
                {
                    SelectObject(parent.gameObject);
                }
            }
            else
            {
                if (GUILayout.Button("NULL", tableComponent.GetGUIWidth("Parent")))
                {

                }
            }
            if (GUILayout.Button(obj.name, tableComponent.GetGUIWidth("Name")))
            {
                SelectObject(obj);
            }

            //GUILayout.Label(building.NodeName, EditorStyles.label, tableComponent.GetGUIWidth("Node"));

            if (GUILayout.Button("Create"))
            {
                var scenePath = GetScenePath(sceneName);
                var cube = ReplaceBuildingCube(obj, info);
                //obj.SetActive(false);
                SelectObject(cube);
                var scene = EditorHelper.CreateScene(obj, scenePath);
                AssetDatabase.Refresh();//刷新编辑器
                EditorHelper.SetAssetBundleName(scenePath, info.AssetName);
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            GameObject assetModel = null;
            if (!selectedObjects.ContainsKey(sceneName))
            {
                assetModel = FindModelAsset(sceneName);
                selectedObjects.Add(sceneName, assetModel);
            }
            else
            {
                assetModel = selectedObjects[sceneName];
            }
            var meshPath = EditorHelper.GetMeshPath(obj);

          
            var modelPath = AssetDatabase.GetAssetPath(assetModel);
            GUILayout.Toggle(meshPath== modelPath, "独立文件", GUILayout.Width(60));

            var selectedObj= (GameObject)EditorGUILayout.ObjectField(assetModel, typeof(GameObject), false, GUILayout.Width(110));
            selectedObjects[sceneName] = selectedObj;

            GameObject simpleOjb = GetSimpleObject(obj);
            EditorGUILayout.ObjectField(simpleOjb, typeof(GameObject), false, GUILayout.Width(110));

            if (GUILayout.Button("ReplaceObj"))
            {
                ReplaceBuildingObject(obj, info, sceneName, selectedObj, "");
            }

            if (GUILayout.Button("ReplaceCube"))
            {
                var cube = ReplaceBuildingCube(obj, info);
                obj.SetActive(false);
                SelectObject(cube);
            }

            
            if (simpleOjb == null)
            {
                if (GUILayout.Button("", GUILayout.Width(100)))
                {

                }
            }
            else
            {
                var simpleVertexCount = GetVertexs(simpleOjb);
                if (GUILayout.Button(simpleVertexCount+"", GUILayout.Width(100)))
                {
                    //assetModel = simpleOjb;
                    ReplaceBuildingObject(obj, info, sceneName, simpleOjb,"_Simple");
                }
            }


            if (GUILayout.Button("Path"))
            {
                var path1 = AssetDatabase.GetAssetPath(obj);
                Debug.Log("path1:" + path1);
                var path2 = EditorHelper.GetMeshPath(obj);
                Debug.Log("path2:" + path2);
                //var path3 = AssetDatabase.GetAssetPath(assetModel);
                //Debug.Log("path3:" + path3);

                Debug.Log("GetDependencies");
                var dependencies = AssetDatabase.GetDependencies(path2);
                foreach (var item in dependencies)
                {
                    Debug.Log("" + item);
                    //var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                }

                //Object parentObject = EditorUtility.GetPrefabParent(obj);
                //string path4 = AssetDatabase.GetAssetPath(parentObject);
                //Debug.Log("prefab path4:" + path4);

                ////Path.
                ////Projector

                //GameObject simpleOjb2 = GetSimpleObject(obj);
            }
            if (GUILayout.Button("Resource"))
            {
                //var meshPath = GetMeshPath(obj);
                Log.Info("meshPath:" + meshPath);
                
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateAll"))
        {
            Debug.Log("CreateAll");
            for (int i = 0; i < buildings.Count; i++)
            {
                var building = buildings[i];
                var obj = building.gameObject;
                var info = GetSceneAssetInfo(building);
                var scenePath = GetScenePath(info.SceneName);
                var cube = ReplaceBuildingCube(obj, info);
                var scene = EditorHelper.CreateScene(obj, scenePath);
                AssetDatabase.Refresh();//刷新编辑器
                EditorHelper.SetAssetBundleName(scenePath, info.AssetName);
                EditorSceneManager.CloseScene(scene, true);
            }
            SaveInfo();
        }
        if (GUILayout.Button("ReplaceBoxAll"))
        {
            ReplaceBoxAll(buildings);
        }
        if (GUILayout.Button("ReplaceSimpleAll"))
        {
            //ReplaceBoxAll(buildings);
            ReplaceSimpleAll(buildings);
            //SaveInfo();
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("ClearScene"))
        {
            EditorHelper.ClearOtherScenes();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("LoadInfo"))
        {
            LoadInfo();
        }
        if (GUILayout.Button("SaveInfo"))
        {
            SaveInfo();
        }
        if (GUILayout.Button("Reset"))
        {
            infos = new Dictionary<string, SceneAssetInfo>();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetChunkManager"))
        {
            SetChunkManager(buildings);
        }
        if (GUILayout.Button("SetFromHttp"))
        {
            ChunkManager cm = GameObject.FindObjectOfType<ChunkManager>();
            if (cm)
            {
                foreach (var chunk in cm.chunks)
                {
                    foreach (var bundle in chunk.bundleList)
                    {
                        bundle.fromFile = false;
                        bundle.checkVersion = true;
                    }
                }
            }
        }
        if (GUILayout.Button("SetFromFile"))
        {
            ChunkManager cm = GameObject.FindObjectOfType<ChunkManager>();
            if (cm)
            {
                foreach (var chunk in cm.chunks)
                {
                    foreach (var bundle in chunk.bundleList)
                    {
                        bundle.fromFile = true;
                        bundle.checkVersion = false;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ReplaceSimpleAll(List<BuildingAssetInfo> buildings)
    {
        Debug.Log("ReplaceSimpleAll");
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            var obj = building.gameObject;
            GameObject simpleOjb = GetSimpleObject(obj);
            if (simpleOjb != null)
            {
                var info = GetSceneAssetInfo(building);
                var sceneName = info.SceneName;
                ReplaceBuildingObject(obj, info, sceneName, simpleOjb, "_Simple");
                GameObject.DestroyImmediate(obj);
            }
        }
    }

    private void ReplaceBuildingObject(GameObject obj, SceneAssetInfo info, string sceneName, GameObject selectedObj,string nameAfter)
    {
        if (selectedObj != null)
        {
            var path = AssetDatabase.GetAssetPath(selectedObj);
            info.ModelPath = path;
            GameObject newObj = GameObject.Instantiate(selectedObj);
            newObj.name = sceneName + nameAfter;
            newObj.transform.position = obj.transform.position;
            newObj.transform.parent = obj.transform.parent;

            var buildingControllers = obj.GetComponentsInChildren<BuildingController>();
            if (buildingControllers.Length ==1)
            {
                var buildingControllersNew = CopyBuildingComponents(obj, newObj, true);//拷贝大楼控制脚本
                CopyFloorControllers(obj, newObj, buildingControllersNew[0]);
                CreateBuildingBox(newObj, info);
            }
            else if (buildingControllers.Length > 1)//D1-D5，J6J11的情况
            {

                foreach (var buildingController in buildingControllers)
                {
                    var newController = newObj.transform.FindChildByName(buildingController.name);
                    if (newController == null)
                    {
                        Debug.LogError("newController == null :"+ buildingController.name);
                        continue;
                    }
                    var buildingControllersNew = CopyBuildingComponents(buildingController.gameObject, newController.gameObject,true);//拷贝大楼控制脚本
                    CopyFloorControllers(buildingController.gameObject, newController.gameObject, buildingControllersNew[0]);
                    BuildingBox box=CreateBuildingBox(newController.gameObject, info);
                    box.IsPart = true;
                }
            }
            else
            {
                Debug.LogError("没有BuildingController:"+ obj);
            }

            obj.SetActive(false);
            SelectObject(newObj);
        }
        else
        {
            Debug.LogError("selectedObj == null");
        }
    }

    private static void CopyFloorControllers(GameObject obj, GameObject newObj, BuildingController buildingController)
    {
        buildingController.ChildNodes = new List<DepNode>();
        var floors = obj.GetComponentsInChildren<FloorController>();
        foreach (var floor in floors)
        {
            var floorObj = floor.gameObject;
            var floorName = floorObj.name;
            var newFloor = newObj.transform.GetChildByName(floorName);
            if (newFloor)
            {
                var floorController = EditorHelper.CopyComponent<FloorController>(floorObj, newFloor.gameObject);//拷贝楼层控制脚本
                buildingController.ChildNodes.Add(floorController);
                floorController.ParentNode = buildingController;

                var floorCube = newFloor.transform.GetChildByName("FloorCube");
                if (floorCube != null)
                {
                    floorController.floorCube = floorCube.gameObject.GetComponent<FloorCubeInfo>();
                }
                var collider = EditorHelper.CopyComponent<BoxCollider>(floorObj, newFloor.gameObject);//拷贝楼层BoxCollider

                //BoxCollider collider = newFloor.gameObject.AddCollider(false);
                //collider.enabled = false;
            }
            else
            {
                Debug.LogError("newFloor == null : " + floorName);
            }
        }
    }

    private GameObject GetSimpleObject(GameObject obj)
    {
        GameObject simpleObj = null;
        var meshPath = EditorHelper.GetMeshPath(obj);
        //Debug.Log("dataPath:" + Application.dataPath);
        //Debug.Log("meshPath:" + meshPath);
        string path = Application.dataPath.Replace("Assets", "") + meshPath;
        //Debug.Log("path:" + path);
        FileInfo fileInfo = new FileInfo(path);
        //Debug.Log("dir:" + fileInfo.Directory.FullName);
        var dirPath = fileInfo.Directory.FullName;
        var parts = dirPath.Split('_');

        string simpleDir = parts[0] + "_Simple";
        if (Directory.Exists(simpleDir))
        {
            var dir2 = EditorHelper.PathToRelative(simpleDir, obj);
            //Debug.Log("dir2:" + dir2);
            var guids = AssetDatabase.FindAssets("t:Prefab", new string[] { dir2 });//先找预设
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(assetPath);
                simpleObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                break;
            }
            if (simpleObj == null)
            {
                var guids2 = AssetDatabase.FindAssets("t:model", new string[] { dir2 });//再找模型
                foreach (var guid in guids2)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    //Debug.Log(assetPath);
                    simpleObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    break;
                }
            }
        }
        else
        {
            var dir2 = EditorHelper.PathToRelative(fileInfo.Directory.FullName, obj);
            //Debug.Log("dir2:" + dir2);
            var guids = AssetDatabase.FindAssets("t:model", new string[] { dir2 });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(assetPath);
                if (assetPath.Contains("简") || assetPath.ToLower().Contains("_simple"))//简单、简易
                {
                    simpleObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    break;
                }
            }
        }
        
        return simpleObj;
    }

    private List<GameObject> GetAllAssetModels()
    {
        List<GameObject> assets = new List<GameObject>();
        Debug.Log("Find");
        var dir = "Assets\\Models\\SiHuiFactory";
        var guids = AssetDatabase.FindAssets("t:model", new string[] { dir });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(path);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Debug.Log(asset);
            assets.Add(asset);
        }
        return assets;
    }

    List<GameObject> allModelAssets;

    private GameObject FindModelAsset(string sceneName)
    {
        if(allModelAssets==null|| allModelAssets.Count == 0)
        {
            allModelAssets = GetAllAssetModels();
        }
        var parts = sceneName.Split('#');
        var name = parts[0].ToLower();
        var asset = allModelAssets.FirstOrDefault(i => i.name.ToLower().StartsWith(name));
        return asset;
    }

    private void SetChunkManager(List<BuildingController> buildings)
    {
        Debug.Log("SetChunkManager");

        LoadInfo();
        if (buildings.Count > 0)
        {
            ReplaceAll(buildings);
        }

        ChunkManager cm = GameObject.FindObjectOfType<ChunkManager>();
        if (cm)
        {
            var buildingBoxes = GameObject.FindObjectsOfType<BuildingBox>();
            List<ChunkManager.Chunk> chunkList = new List<ChunkManager.Chunk>();

            for (int i = 0; i < buildingBoxes.Length; i++)
            {
                var box = buildingBoxes[i];
                var info = box.info;
                if (info == null)
                {
                    Debug.LogWarning("info==null:"+box);
                    continue;
                }
                //if (info.SceneName.ToLower() != "u3")
                //{
                //    continue;
                //}
                var chunk = new ChunkManager.Chunk();
                chunk.sceneName = info.SceneName;
                chunk.center = info.Position;
                chunk.loadDistance = info.LoadDistance;
                chunk.unloadDistance = info.UnloadDistance;

                if (chunk.onLoad == null)
                    chunk.onLoad = new OnLoadUnityEvent();
                //chunk.onLoad.AddListener(box.Hide);
                UnityEventTools.AddPersistentListener(chunk.onLoad, box.Hide);
                if (chunk.onUnload == null)
                    chunk.onUnload = new OnLoadUnityEvent();
                //chunk.onUnload.AddListener(box.Show);
                UnityEventTools.AddPersistentListener(chunk.onUnload, box.Show);

                ChunkManager.BundleDef bundle = new ChunkManager.BundleDef();
                bundle.bundleName = info.AssetName.ToLower();//这里要加ToLower()，不然会出错
                bundle.fromFile = true;
                bundle.checkVersion = false;
                chunk.bundleList = new ChunkManager.BundleDef[] { bundle };
                Debug.Log("bundle:" + bundle);
                Debug.Log("chunk:" + chunk);
                chunkList.Add(chunk);
            }
            cm.chunks = chunkList.ToArray();

            Debug.Log("chunks:--------------" + cm.chunks.Length);
            for (int i = 0; i < cm.chunks.Length; i++)
            {
                var chunk = cm.chunks[i];
                Debug.Log("chunk:" + chunk);
                foreach (var item in chunk.bundleList)
                {
                    Debug.Log("bundle:" + item);
                }
            }
        }
    }

    private void SetChunkManager(List<BuildingAssetInfo> buildings)
    {
        Debug.Log("SetChunkManager");

        LoadInfo();
        if (buildings.Count > 0)
        {
            //ReplaceBoxAll(buildings);
            ReplaceSimpleAll(buildings);
        }

        ChunkManager cm = GameObject.FindObjectOfType<ChunkManager>();
        if (cm)
        {
            var buildingBoxes = GameObject.FindObjectsOfType<BuildingBox>();
            List<ChunkManager.Chunk> chunkList = new List<ChunkManager.Chunk>();

            for (int i = 0; i < buildingBoxes.Length; i++)
            {
                var box = buildingBoxes[i];
                var info = box.info;
                if (info == null)
                {
                    Debug.LogWarning("info==null:" + box);
                    continue;
                }
                //if (info.SceneName.ToLower() != "u3")
                //{
                //    continue;
                //}
                var chunk = new ChunkManager.Chunk();
                chunk.sceneName = info.SceneName;
                chunk.center = info.Position;
                chunk.loadDistance = info.LoadDistance;
                chunk.unloadDistance = info.UnloadDistance;

                if (chunk.onLoad == null)
                    chunk.onLoad = new OnLoadUnityEvent();
                //chunk.onLoad.AddListener(box.Hide);
                UnityEventTools.AddPersistentListener(chunk.onLoad, box.Hide);
                if (chunk.onUnload == null)
                    chunk.onUnload = new OnLoadUnityEvent();
                //chunk.onUnload.AddListener(box.Show);
                UnityEventTools.AddPersistentListener(chunk.onUnload, box.Show);

                ChunkManager.BundleDef bundle = new ChunkManager.BundleDef();
                bundle.bundleName = info.AssetName.ToLower();//这里要加ToLower()，不然会出错
                bundle.fromFile = true;
                bundle.checkVersion = false;
                chunk.bundleList = new ChunkManager.BundleDef[] { bundle };
                Debug.Log("bundle:" + bundle);
                Debug.Log("chunk:" + chunk);
                chunkList.Add(chunk);
            }
            cm.chunks = chunkList.ToArray();

            Debug.Log("chunks:--------------" + cm.chunks.Length);
            for (int i = 0; i < cm.chunks.Length; i++)
            {
                var chunk = cm.chunks[i];
                Debug.Log("chunk:" + chunk);
                foreach (var item in chunk.bundleList)
                {
                    Debug.Log("bundle:" + item);
                }
            }
        }
    }

    public class OnLoadUnityEvent : UnityEvent
    {
        //protected override MethodInfo FindMethod_Impl(string name, object targetObj)
        //{
        //    Debug.Log("OnLoadUnityEvent.FindMethod_Impl:"+name+"|"+targetObj);
        //    return base.FindMethod_Impl(name, targetObj);
        //}
    }

    TableComponent tableComponent = new TableComponent();

    private void GUI_InitTableComponent()
    {
        int idColumnWidth = 20;
        int sceneColumnWidth = 50;
        int countColumnWidth = 60;
        int parentColumnWidth = 150;
        int nameColumnWidth = 70;
        int nodeColumnWidth = 120;

        tableComponent.Clear();
        tableComponent.AddColumn("ID", idColumnWidth);
        tableComponent.AddColumn("Scene", sceneColumnWidth);
        tableComponent.AddColumn("Count", countColumnWidth);
        tableComponent.AddColumn("Parent", parentColumnWidth);
        tableComponent.AddColumn("Name", nameColumnWidth);
        tableComponent.AddColumn("Node", nodeColumnWidth);
        tableComponent.OnGUI();
    }

    private void LoadInfo()
    {
        var infoPath = Application.dataPath + "/SceneAssetInfoList.xml";
        SceneAssetInfoList list = SerializeHelper.LoadFromFile<SceneAssetInfoList>(infoPath);
        infos = new Dictionary<string, SceneAssetInfo>();
        foreach (var item in list)
        {
            infos.Add(item.ObjectName, item);
        }
    }

    private void SaveInfo()
    {
        var infoPath = Application.dataPath + "/SceneAssetInfoList.xml";
        Debug.Log("infoPath:" + infoPath);
        SceneAssetInfoList list = new SceneAssetInfoList();
        foreach (var key in infos.Keys)
        {
            var info = infos[key];
            list.Add(info);
        }
        SerializeHelper.Save(list, infoPath);
    }

    private void ReplaceAll(List<BuildingController> buildings)
    {
        Debug.Log("ReplaceAll");
        for (int i = 0; i < buildings.Count; i++)
        {

            var building = buildings[i];
            //Debug.Log("Replace:" + building);
            var obj = building.gameObject;
            var info = GetSceneAssetInfo(building);
            ReplaceBuildingCube(obj, info);
            //obj.SetActive(false);
            GameObject.DestroyImmediate(obj);
        }
        SaveInfo();
    }

    private void ReplaceBoxAll(List<BuildingAssetInfo> buildings)
    {
        Debug.Log("ReplaceBoxAll");
        for (int i = 0; i < buildings.Count; i++)
        {

            var building = buildings[i];
            //Debug.Log("Replace:" + building);
            var obj = building.gameObject;
            var info = GetSceneAssetInfo(building);
            ReplaceBuildingCube(obj, info);
            //obj.SetActive(false);
            GameObject.DestroyImmediate(obj);
        }
        SaveInfo();
    }

    private static float GetMax(float x,float y,float z)
    {
        if (x >= y && x >= z)
        {
            return x;
        }
        if (y >= x && y >= z)
        {
            return y;
        }
        if (z >= x && z >= y)
        {
            return z;
        }
        return 0;
    }

    private BoxCollider CreateNewBoxCollider(GameObject obj)
    {
        var boxColliders = obj.GetComponents<BoxCollider>();
        if (boxColliders!=null)
        {
            foreach (var item in boxColliders)
            {
                DestroyImmediate(item);//删除原来的碰撞体
            }
        }
        var boxCollider = obj.AddCollider(false);
        return boxCollider;
    }

    private BuildingBox CreateBuildingBox(GameObject obj, SceneAssetInfo info)
    {
        BuildingBox box = obj.AddMissingComponent<BuildingBox>();
        box.info = info;
        box.SceneName = info.SceneName;
        box.AssetName = info.AssetName;

        CreateBuildingAssetInfo(obj, info);
        return box;
    }

    private BuildingAssetInfo CreateBuildingAssetInfo(GameObject obj, SceneAssetInfo info)
    {
        BuildingAssetInfo box = obj.AddMissingComponent<BuildingAssetInfo>();
        //box.info = info;
        box.SceneName = info.SceneName;
        box.AssetName = info.AssetName;
        return box;
    }

    private GameObject ReplaceBuildingCube(GameObject buildingObj, SceneAssetInfo info)
    {
        GameObject buildingCube = null;
        {
            var boxCollider = CreateNewBoxCollider(buildingObj);
            var pos = buildingObj.transform.position;
            info.Position = pos + boxCollider.center;
            info.Center = boxCollider.center;
            var bSize = boxCollider.size;
            var scale = buildingObj.transform.localScale;
            var size = new Vector3(bSize.x * scale.x, bSize.y * scale.y, bSize.z * scale.z);
            info.Size = size;
            info.LoadDistance = GetMax(size.x, size.y, size.z) * loadDistanceBias;
            info.UnloadDistance = info.LoadDistance * unloadDistanceBias;

            buildingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buildingCube.name = info.ObjectName + "_Building";
            //buildingCube.transform.position = pos;
            buildingCube.transform.position = pos + boxCollider.center;
            buildingCube.transform.localScale = size;
            buildingCube.transform.parent = buildingObj.transform.parent;

            CreateBuildingBox(buildingCube, info);
        }

        var buildingControllers = CopyBuildingComponents(buildingObj, buildingCube,false);
        if (buildingControllers.Count > 0)
        {
            var buildingController = buildingControllers[0];
            {
                buildingController.ChildNodes = new List<DepNode>();
                var floors = buildingObj.GetComponentsInChildren<FloorController>();
                foreach (var floor in floors)
                {
                    var floorObj = floor.gameObject;
                    var boxCollider = CreateNewBoxCollider(floorObj);
                    var bSize = boxCollider.size;
                    var scale = floorObj.transform.localScale;
                    var size = new Vector3(bSize.x * scale.x, bSize.y * scale.y, bSize.z * scale.z);
                    var floorCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    floorCube.name = floorObj.name + "_Floor";
                    floorCube.transform.position = floorObj.transform.position;
                    floorCube.transform.localScale = size;
                    floorCube.transform.parent = buildingCube.transform;
                    var floorController = EditorHelper.CopyComponent<FloorController>(floorObj, floorCube);
                    buildingController.ChildNodes.Add(floorController);
                    floorController.ParentNode = buildingController;

                    var cubeCollider = floorCube.GetComponent<BoxCollider>();
                    cubeCollider.enabled = false;
                }
            }
        }

        return buildingCube;
    }

    private static List<BuildingController> CopyBuildingComponents(GameObject buildingObj, GameObject buildingCube,bool copyBoxCollider)
    {
        Debug.Log("CopyBuildingComponents " + buildingObj + "->" + buildingCube);
        //1.复制BuildingController
        //var buildingController = CopyComponent<BuildingController>(buildingObj, buildingCube);
        //if (buildingController!=null && buildingController.ParentNode != null)
        //{
        //    buildingController.ParentNode.ChildNodes.Add(buildingController);
        //}
        var buildingControllers = EditorHelper.CopyComponents<BuildingController>(buildingObj, buildingCube);
        foreach (var buildingController in buildingControllers)
        {
            if (buildingController.ParentNode == null)
            {
                buildingController.ParentNode = buildingController.transform.parent.GetComponent<DepNode>();
            }
            if (buildingController.ParentNode == null)//D1-D4不一样，下面有几个子物体
            {
                buildingController.ParentNode = buildingController.transform.parent.parent.GetComponent<DepNode>();
            }
            if (buildingController.ParentNode != null)
            {
                buildingController.ParentNode.ChildNodes.Add(buildingController);
            }
            else 
            {
                Debug.LogError("buildingController.ParentNode == null:"+ buildingController);
            }
        }

        //2.复制Collider
        MeshCollider meshCollider = EditorHelper.CopyComponent<MeshCollider>(buildingObj, buildingCube);
        if (meshCollider != null)
        {
            MeshFilter meshFilter = buildingCube.GetComponent<MeshFilter>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        if (copyBoxCollider)
        {
            //2.复制BoxCollider
            BoxCollider boxCollider = EditorHelper.CopyComponent<BoxCollider>(buildingObj, buildingCube);
            if (meshCollider != null)
            {
                BoxCollider meshFilter = buildingCube.GetComponent<BoxCollider>();
                //meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }

        //BoxCollider boxCollider = CopyComponent<BoxCollider>(buildingObj, buildingCube);

        //3.复制BuidlingInfoTarget
        BuidlingInfoTarget infoTarget = buildingObj.GetComponentInChildren<BuidlingInfoTarget>();
        if (infoTarget != null)
        {
            var newInfoTarget = GameObject.Instantiate(infoTarget);
            newInfoTarget.transform.parent = buildingCube.transform;
            CopyBasicInfo(newInfoTarget.gameObject,infoTarget.gameObject);
        }

        //4.复制BuildingFloorTweening
        BuildingFloorTweening tweening = EditorHelper.CopyComponent<BuildingFloorTweening>(buildingObj, buildingCube);
        if (tweening != null)
        {
            for (int i = 0; i < tweening.FloorList.Count; i++)
            {
                GameObject floor = tweening.FloorList[i];
                if (floor == null) continue;
                var floorName = floor.name;
                var floorNew = buildingCube.transform.GetChildByName(floorName);
                if (floorNew != null)
                {
                    tweening.FloorList[i] = floorNew.gameObject;
                }
                else
                {
                    if (floorName == "H3_Top")
                    {
                        var floorNew2 = buildingCube.transform.GetChildByName("H3_F2");//模型改名了
                        if (floorNew2 != null)
                        {
                            tweening.FloorList[i] = floorNew2.gameObject;
                        }
                        else
                        {
                            Debug.LogError("复制BuildingFloorTweening floorNew == null : H3_F2");
                        }
                    }
                    
                    else
                    {
                        Debug.LogError("复制BuildingFloorTweening floorNew == null : " + floorName);
                    }
                }
            }
        }

        //5.复制FloorCube
        List<Transform>  floorCubes=CopyChildren(buildingObj, buildingCube, "FloorCube");
        foreach (var floorCube in floorCubes)
        {
            BoxCollider collider = floorCube.GetComponent<BoxCollider>();
            if (collider)
            {
                collider.enabled = false;
            }
        }

        //6.设置地板的MeshCollider和Layer
        var floors = buildingObj.transform.GetChildrenByLayer("Floor");
        foreach (var floor in floors)
        {
            if (floor.name == "FloorCube") continue;
            if (floor.parent.name == "SameLocationFloor") continue;
            var floorNew = buildingCube.transform.GetChildByName(floor.name);
            if (floorNew == null)
            {
                if (floor.name == "H2_FW6")
                {
                    var floorName = "H2_F2_FW6";
                    var floorNew2 = buildingCube.transform.GetChildByName(floorName);
                    if (floorNew2 != null)
                    {
                        //var floorMeshCollider = floorNew2.gameObject.AddComponent<MeshCollider>();
                        floorNew2.gameObject.layer = floor.gameObject.layer;
                    }
                    else
                    {
                        Debug.LogError("设置地板的MeshCollider和Layer floorNew == null : " + floor.name + "," + buildingObj + "|" + floorName);
                    }
                }
                else
                {
                    Debug.LogError("设置地板的MeshCollider和Layer floorNew == null : " + floor.name + "," + buildingObj);
                }
            }
            else
            {
                //var floorMeshCollider = floorNew.gameObject.AddComponent<MeshCollider>();
                floorNew.gameObject.layer = floor.gameObject.layer;
            }

            //MeshFilter meshFilter = floorNew.gameObject.GetComponent<MeshFilter>();
            //floorMeshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        //7.主厂房 拷贝SameLocationFloor
        CopyChildren(buildingObj, buildingCube, "SameLocationFloor",true);

        //8.复制静态设备脚本
        if(buildingObj.GetComponent<FacilityDevController>()!=null)
        {
            buildingCube.AddMissingComponent<FacilityDevController>();
        }

        return buildingControllers;
    }

    private static List<Transform> CopyChildren(GameObject buildingObj, GameObject buildingCube,string childrenName,bool isPosZero=false)
    {
        List<Transform> newItems = new List<Transform>();
        List<Transform> items = buildingObj.transform.FindChildrenByName(childrenName);
        foreach (var cube in items)
        {
            var newParent = buildingCube.transform.GetChildByName(cube.parent.name);
            if (newParent != null)
            {
                var newCube = GameObject.Instantiate(cube);
                newCube.name = cube.name;
                newCube.parent = newParent;
                newCube.transform.position = cube.position;
                if (isPosZero)
                {
                    newCube.localPosition = Vector3.zero;
                }
                newItems.Add(newCube);
            }
            else
            {
                Debug.LogError("newParent == null : " + cube.parent.name);
            }
        }
        return newItems;
    }

    /// <summary>
    /// 拷贝基本信息（localPos,localAngle,localScale）
    /// </summary>
    /// <param name="newObj"></param>
    /// <param name="oldObj"></param>
    private static void CopyBasicInfo(GameObject newObj,GameObject oldObj)
    {
        newObj.transform.name = oldObj.transform.name;
        newObj.transform.localPosition = oldObj.transform.localPosition;
        newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles;
        newObj.transform.localScale = oldObj.transform.localScale;
    }
}
#endif
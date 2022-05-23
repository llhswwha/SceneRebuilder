using Base.Common;
using GPUInstancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class BuildingScenesLoadManager : MonoBehaviour
{
    private static BuildingScenesLoadManager _instance;

    public static BuildingScenesLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BuildingScenesLoadManager>(); 
            }
            //if (_instance == null)
            //{
            //    GameObject insGo = new GameObject(typeof(T).Name);
            //    _instance = insGo.AddComponent<T>();
            //}
            return _instance;
        }
    }

    //public bool IsLoadXmlOnStart = true;
    public bool IsLoadSceneOnStart = true;
    public bool IsLoadUserBuildings = false;
    public List<string> UserBulindgs = new List<string>();
    //public bool IsLoadByDistance = true;

    private void LoadSettingXmlEx()
    {
        LoadSettingXml();

        //IsLoadByDistance = Setting.IsLoadByDistance;
        IsLoadSceneOnStart = Setting.IsLoadSceneOnStart;
        IsLoadUserBuildings = Setting.IsLoadUserBuildings;
        UserBulindgs = Setting.UserBulindgs;
        SubSceneShowManager.Instance.IsEnableUnload = Setting.IsEnableUnload;
    }

    void Start()
    {
        LoadSettingXmlEx();

        if (IsLoadSceneOnStart)
        {
            if (IsLoadUserBuildings)
            {
                UserBulindgs.RemoveAll(i => string.IsNullOrEmpty(i));
                if (UserBulindgs.Count > 0)
                {
                    LoadUserBuildings();
                }
                else
                {
                    //RemoveAllBuildings
                }
            }
            else
            {
                LoadScenesBySetting();
            }
        }
        else
        {

        }
        

        SubSceneShowManager.Instance.IsUpdateTreeNodeByDistance = false;
        AreaTreeNodeShowManager.Instance.IsUpdateTreeNodeByDistance = false;

        //SubSceneShowManager.Instance.SetEnable(IsLoadByDistance);
    }

    public string TestTargetName = "";

    public void LoadDepNodes(List<DepNode> depNodes)
    {

    }

    [ContextMenu("InitUserBuildings")]
    public void InitUserBuildings()
    {
        Setting.UserBulindgs.Clear();
        var bs = GameObject.FindObjectsOfType<BuildingController>();
        
        foreach(var b in bs)
        {
            Setting.UserBulindgs.Add(b.name);
        }
    }

    [ContextMenu("ClearUserBuildings")]
    public void ClearUserBuildings()
    {
        Setting.UserBulindgs.Clear();
    }

    [ContextMenu("TestLoadBuildings")]
    public void LoadUserBuildings()
    {
        string[] bs = UserBulindgs.ToArray();
        LoadBuildings(bs, (p) =>
        {
            Debug.LogError($"TestLoadBuildings Progress:{p}");
        });
    }

    public void LoadBuildings(IEnumerable<string> depNodes, Action<SceneLoadProgress> finishedCallbak)
    {
        List<BuildingController> enableBuildings = new List<BuildingController>();
        var buildings = GameObject.FindObjectsOfType<BuildingController>(true).ToList();
        foreach (var dep in depNodes)
        {
            if (string.IsNullOrEmpty(dep)) continue;
            BuildingController b = FindBuildingByName(buildings, dep);
            if (b != null)
            {
                buildings.Remove(b);
                enableBuildings.Add(b);
            }
            else
            {
                Debug.LogError($"LoadBuildings Not Found Building:{dep}");
            }
        }

        LoadBuildingScenes(buildings, enableBuildings, finishedCallbak);
    }

    public void LoadBuildingScenes(List<BuildingController> disableBuildings, List<BuildingController> enableBuildings, Action<SceneLoadProgress> finishedCallbak)
    {
        List<BuildingModelInfo> modelList = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
        foreach (var b in disableBuildings)
        {
            BuildingModelInfo[] ms = b.GetComponentsInChildren<BuildingModelInfo>(true);
            foreach(var m in ms)
            {
                modelList.Remove(m);
            }
        }
        foreach (var b in enableBuildings)
        {
            BuildingModelInfo[] ms = b.GetComponentsInChildren<BuildingModelInfo>(true);
            foreach (var m in ms)
            {
                modelList.Remove(m);
            }
        }


        Debug.LogError($"LoadBuildingScenes disableBuildings:{disableBuildings.Count} enableBuildings:{enableBuildings.Count}");
        string s1 = "";
        foreach(var b in disableBuildings)
        {
            s1 += b.name + ";";
        }
        string s2 = "";
        foreach (var b in enableBuildings)
        {
            s2 += b.name + ";";
        }

        List<BuildingController> staticBuilidings = new List<BuildingController>();
        foreach (var b in disableBuildings)
        {
            var bs = Setting.GetBuilding(b);
            if (bs == null)
            {
                Debug.LogError($"LoadBuildingScenes GetBuilding == null :{b.name}");
                continue;
            }
            if (bs.IsStatic)
            {
                staticBuilidings.Add(b);
                continue;
            }
            GameObject.DestroyImmediate(b.gameObject);
        }
        string s3 = "";
        foreach (var b in staticBuilidings)
        {
            s3 += b.name + ";";
        }

        string s4 = "";
        foreach (var b in modelList)
        {
            s4 += b.name + ";";
        }

        Debug.LogError($"LoadBuildingScenes IsLoadUserBuildings:{IsLoadUserBuildings} disableBuildings({disableBuildings.Count}):{s1}\n enableBuildings({enableBuildings.Count}):{s2}\n staticBuilidings({staticBuilidings.Count}):{s3}\nmodelList({modelList.Count}):{modelList}");

        if (IsLoadUserBuildings)//Test
        {
#if UNITY_EDITOR
            staticBuilidings.Clear();
            modelList.Clear();
#endif
        }

        if (Setting.Enabled == false)
        {
            Debug.LogError("LoadBuildingScenes Setting.Enabled == false DistroyAllBuildings!!!");
            foreach (var b in enableBuildings)
            {
                GameObject.DestroyImmediate(b.gameObject);
            }
            foreach (var b in staticBuilidings)
            {
                GameObject.DestroyImmediate(b.gameObject);
            }

            if (finishedCallbak != null)
            {
                finishedCallbak(new SceneLoadProgress(null, 1, true));
            }
        }
        else
        {
            SubSceneBagList allScenes3 = GetBuildingScenes(modelList);
            SubSceneBag sceneList3 = allScenes3.GetAllScenes();

            SubSceneBagList allScenes1 = GetBuildingScenes(staticBuilidings);
            SubSceneBag sceneList1 = allScenes1.GetAllScenes();
            sceneList1.Sort((a, b) => b.GetBoundsVolume().CompareTo(a.GetBoundsVolume()));
            SubSceneBagList allScenes2 = GetBuildingScenes(enableBuildings);
            SubSceneBag sceneList2 = allScenes2.GetAllScenes();
            sceneList2.Sort((a, b) => b.GetBoundsVolume().CompareTo(a.GetBoundsVolume()));

            sceneList1.AddRange(sceneList3);
            sceneList1.AddRange(sceneList2);

            //var ss = sceneList2.ToArray();
            var ss = sceneList1.ToArray();

            SubSceneShowManager.Instance.RemoveScenes(ss);
            Debug.Log($"LoadScenesBySetting buildings:{disableBuildings.Count()} enableBuildings:{enableBuildings.Count()} bags1:{allScenes1.Count} bags2:{allScenes2.Count} scenes:{ss.Length}");
            if (IsEnableLoad)
            {
                SubSceneManager.Instance.LoadScenesEx(ss, (p)=>
                {
                    //if (p.isAllFinished)
                    //{
                    //    LODManager.Instance.UpdateLODs();
                    //}

                    StartGPUInstance(p);
                    if (finishedCallbak != null)
                    {
                        finishedCallbak(p);
                    }
                });
            }
            else
            {
                SceneLoadProgress p = new SceneLoadProgress(null, 1, true);
                StartGPUInstance(p);
                if (finishedCallbak != null)
                {
                    finishedCallbak(p);
                }
            }
        }


    }

    private void StartGPUInstance(SceneLoadProgress p)
    {
        if (p.isAllFinished == false)
        {
            return;
        }
        if (IsEnableGPU)
        {
            GPUInstancerPrototype.IsShowLog = false;
            GPUInstanceTest.Instance.StartGPUInstance();
        }
    }

    private SubSceneBagList GetBuildingScenes(List<BuildingModelInfo> enableBuildings)
    {
        SubSceneBagList allScenes = new SubSceneBagList();

        for (int i = 0; i < enableBuildings.Count; i++)
        {
            BuildingModelInfo b = enableBuildings[i];
            if (b == null) continue;
            //if (string.IsNullOrEmpty(b.name))
            //{
            //    b.NodeName = b.name;
            //}
            if (IsShowLog)
                Debug.LogError($"LoadBuildingScenes building[{i + 1}]:{b}");
            BuildingSceneLoadItemCollection bulldingSetting = Setting.GetBuilding(b.name);
            if (bulldingSetting == null || bulldingSetting.IsEnable == false)
            {
                GameObject.DestroyImmediate(b.gameObject);
            }
            else
            {
                BuildingModelInfo[] models = b.GetComponentsInChildren<BuildingModelInfo>(true);
                for (int i1 = 0; i1 < models.Length; i1++)
                {
                    BuildingModelInfo model = models[i1];
                    if (model == null) continue;
                    if (IsShowLog)
                        Debug.LogError($"LoadBuildingScenes building[{i + 1}]:{b} floor[{i1 + 1}]:{model}");
                    //allScenes.Add(model.GetSubScenes_Out0B());
                    BuildingSceneLoadItem floorSetting = bulldingSetting.GetChild(model.name);
                    if (floorSetting == null || floorSetting.IsEnable == false)
                    {
                        GameObject.DestroyImmediate(model.gameObject);
                    }
                    else
                    {
                        SubSceneBag subScenes = model.GetSubScenes(floorSetting);
                        allScenes.Add(subScenes);
                    }

                }
            }
        }
        return allScenes;
    }

    private SubSceneBagList GetBuildingScenes(List<BuildingController> enableBuildings)
    {
        SubSceneBagList allScenes = new SubSceneBagList();

        for (int i = 0; i < enableBuildings.Count; i++)
        {
            BuildingController b = enableBuildings[i];
            if (b == null) continue;
            if (string.IsNullOrEmpty(b.NodeName))
            {
                b.NodeName = b.name;
            }
            if (IsShowLog)
                Debug.LogError($"LoadBuildingScenes building[{i + 1}]:{b}");
            BuildingSceneLoadItemCollection bulldingSetting = Setting.GetBuilding(b.NodeName);
            if (bulldingSetting == null || bulldingSetting.IsEnable == false)
            {
                GameObject.DestroyImmediate(b.gameObject);
            }
            else
            {
                BuildingModelInfo[] models = b.GetComponentsInChildren<BuildingModelInfo>(true);
                for (int i1 = 0; i1 < models.Length; i1++)
                {
                    BuildingModelInfo model = models[i1];
                    if (model == null) continue;
                    if (IsShowLog)
                        Debug.LogError($"LoadBuildingScenes building[{i + 1}]:{b} floor[{i1 + 1}]:{model}");
                    //allScenes.Add(model.GetSubScenes_Out0B());
                    BuildingSceneLoadItem floorSetting = bulldingSetting.GetChild(model.name);
                    if (floorSetting == null || floorSetting.IsEnable == false)
                    {
                        GameObject.DestroyImmediate(model.gameObject);
                    }
                    else
                    {
                        SubSceneBag subScenes = model.GetSubScenes(floorSetting);
                        allScenes.Add(subScenes);
                    }

                }
            }
        }
        return allScenes;
    }

    public bool IsEnableLoad = true;

    public void LoadBuildingScene(DepNode building)
    {

    }

    public void LoadUserBuildings(IEnumerable<BuildingController> depNodes, Action<SceneLoadProgress> finishedCallbak)
    {
        Debug.LogError($"LoadUserBuildings IsLoadUserBuildings:{IsLoadUserBuildings} UserBulindgs:{UserBulindgs.Count} depNodes:{depNodes.Count()}");

        if (IsLoadUserBuildings)
        {
            UserBulindgs.RemoveAll(i => string.IsNullOrEmpty(i));
            LoadBuildings(UserBulindgs, finishedCallbak);
        }
        else
        {
            List<BuildingController> enableBuildings = new List<BuildingController>();
            var buildings = GameObject.FindObjectsOfType<BuildingController>(true).ToList();
            foreach (var dep in depNodes)
            {
                if (buildings.Contains(dep))
                {
                    buildings.Remove(dep);
                    enableBuildings.Add(dep);
                }
                else
                {
                    Debug.LogError($"LoadBuildings Not Found Building:{dep}");
                }
            }

            LoadBuildingScenes(buildings, enableBuildings, finishedCallbak);
        }

        //if (finishedCallbak != null)
        //{
        //    finishedCallbak(new SceneLoadProgress(null,1,true));
        //}

    }

    public void LoadBuildings(IEnumerable<BuildingController> depNodes, Action<SceneLoadProgress> finishedCallbak)
    {
        List<BuildingController> enableBuildings = new List<BuildingController>();
        var buildings = GameObject.FindObjectsOfType<BuildingController>(true).ToList();
        foreach (var dep in depNodes)
        {
            if (buildings.Contains(dep))
            {
                buildings.Remove(dep);
                enableBuildings.Add(dep);
            }
            else
            {
                Debug.LogError($"LoadBuildings Not Found Building:{dep}");
            }
        }

        LoadBuildingScenes(buildings, enableBuildings, finishedCallbak);
    }

    public BuildingController FindBuildingByName(List<BuildingController> buildings, string name)
    {
        foreach (var b in buildings)
        {
            if (b.NodeName == name || b.name == name)
            {
                return b;
            }
        }
        return null;
    }

    public void LoadFloors()
    {

    }

    [ContextMenu("CreateScenes")]
    public void CreateScenes()
    {
        var trees = ModelTarget.GetComponentsInChildren<ModelAreaTree>();
        for (int i = 0; i < trees.Length; i++)
        {
            //trees[i].EditorCreateScenes();
        }
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.LoadScenes ModelTarget == null");
            return;
        }
        var trees = ModelTarget.GetComponentsInChildren<ModelAreaTree>();
        for (int i = 0; i < trees.Length; i++)
        {
            //trees[i].EditorCreateScenes();
        }
    }

    public bool IsShowLog = false;

    [ContextMenu("LoadScenesBySetting")]
    public void LoadScenesBySetting()
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.LoadScenesBySetting ModelTarget == null");
            return;
        }
        if (Setting.Enabled == false)
        {
            Debug.LogError($"BuildingScenesLoadManager.InitSettingByScene Setting.Enabled == false");
            return;
        }

        DepNode[] bcs = ModelTarget.GetComponentsInChildren<DepNode>(true);
        Dictionary<string, DepNode> depDict = new Dictionary<string, DepNode>();
        foreach (var bc in bcs)
        {
            if (string.IsNullOrEmpty(bc.NodeName))
            {
                bc.NodeName = bc.name;
            }
            depDict.Add(bc.NodeName, bc);
        }

        BuildingModelInfo[] models = ModelTarget.GetComponentsInChildren<BuildingModelInfo>(true);
        Dictionary<string, BuildingModelInfo> modelDict = new Dictionary<string, BuildingModelInfo>();
        foreach (var bc in models)
        {
            modelDict.Add(bc.name, bc);
        }

        Debug.Log($"LoadScenesBySetting depDict:{depDict.Count}");


        SubSceneBagList allScenes = new SubSceneBagList();
        for (int i = 0; i < Setting.Items.Count; i++)
        {
            BuildingSceneLoadItemCollection buildingloadItem = Setting.Items[i];
            if(IsShowLog)
                Debug.LogError($"LoadScenesBySetting building[{i+1}]:{buildingloadItem}");
            if (depDict.ContainsKey(buildingloadItem.Name) == false)
            {
                Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(buildingloadSetting.Name) == false building:{buildingloadItem}");
                continue;
            }

            DepNode buildingDep = depDict[buildingloadItem.Name];
            BuildingController buildingController = buildingDep as BuildingController;
            if (buildingController == null)
            {
                Debug.LogError($"LoadScenesBySetting buildingController==null buildingDep:{buildingDep}");
                continue;
            }

            if (buildingloadItem.IsEnable == false)
            {
                GameObject.DestroyImmediate(buildingController.gameObject);
                continue;
            }

            if (buildingloadItem.Children != null && buildingloadItem.Children.Count > 0)
            {
                for (int i1 = 0; i1 < buildingloadItem.Children.Count; i1++)
                {
                    BuildingSceneLoadItem floorLoadItem = buildingloadItem.Children[i1];
                    if (IsShowLog)
                        Debug.LogError($"LoadScenesBySetting building[{i + 1}]:{buildingloadItem.Name} floor[{i1+1}]:{floorLoadItem}");

                    if (modelDict.ContainsKey(floorLoadItem.Name) == false)
                    {
                        Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(floorLoadSetting.Name) == false floor:{floorLoadItem}");
                        continue;
                    }

                    BuildingModelInfo floorModelInfo = modelDict[floorLoadItem.Name];
                    //FloorController floorController = floorDep as FloorController;
                    //if (floorController == null)
                    //{
                    //    Debug.LogError($"LoadScenesBySetting floorController == null floorDep:{floorDep}");
                    //    continue;
                    //}

                    //BuildingModelInfo floorModelInfo = floorController.GetComponent<BuildingModelInfo>();
                    if (floorModelInfo == null)
                    {
                        Debug.LogError($"LoadScenesBySetting floorModelInfo == null building:{floorLoadItem.Name}");
                        continue;
                    }
                    else
                    {
                        if (floorLoadItem.IsEnable == false)
                        {
                            GameObject.DestroyImmediate(floorModelInfo.gameObject);
                        }
                        else
                        {
                            //LoadFloorScenes
                            //LoadSubScenes(floorController)
                            SubSceneBag subScenes = floorModelInfo.GetSubScenes(floorLoadItem);
                            allScenes.Add(subScenes);
                        }

                    }
                }
            }
            else
            {
                Debug.LogError($"LoadScenesBySetting buildingloadItem.Children is Empty buildingDep:{buildingDep}");

                //BuildingModelInfo floorModelInfo = buildingController.GetComponent<BuildingModelInfo>();
                //if (floorModelInfo == null)
                //{
                //    Debug.LogError($"LoadScenesBySetting floorModelInfo == null buildingController:{buildingController}");
                //    continue;
                //}

                //SubSceneBag subScenes = floorModelInfo.GetSubScenes(buildingloadItem);
                //allScenes.Add(subScenes);
            }
        }

        var ss = allScenes.GetAllScenesArray();
        SubSceneShowManager.Instance.RemoveScenes(ss);
        Debug.Log($"LoadScenesBySetting1 depDict:{depDict.Count} bags:{allScenes.Count} scenes:{ss.Length}");

        if (IsLoadScene)
        {
            SubSceneManager.Instance.LoadScenesAsyncEx(ss, (p) =>
            {
                StartGPUInstance(p);
            });
        }
    }

    private void Awake()
    {
        if (IsEnableGPU)
        {
            GPUInstanceTest.Instance.IsStartGPUInstance = false;
            GPUInstanceTest.Instance.gameObject.SetActive(true);
        }
        else
        {
            GPUInstanceTest.Instance.gameObject.SetActive(false);
        }
    }

    public bool IsLoadScene = true;

    public bool IsEnableGPU = false;

    public void LoadSubScenes(BuildingModelInfo root, BuildingSceneLoadItem loadSetting)
    {

    }

    [ContextMenu("TestLoadTarget")]
    public void TestLoadTarget()
    {
        DepNode[] bcs = ModelTarget.GetComponentsInChildren<DepNode>(true);
        Dictionary<string, DepNode> depDict = new Dictionary<string, DepNode>();
        foreach (var bc in bcs)
        {
            depDict.Add(bc.NodeName, bc);
        }
    }

    public GameObject ModelTarget;

    [ContextMenu("UpdateSettingByScene")]
    public void UpdateSettingByScene()
    {
        InitSettingByScene_Core(false);
    }

    [ContextMenu("InitSettingByScene")]
    public void InitSettingByScene()
    {
        InitSettingByScene_Core(true);
    }

    public void InitSettingByScene_Core(bool isNew)
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.InitSettingByScene ModelTarget == null");
            return;
        }

        List<BuildingModelInfo> modelList = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
        int allCount = modelList.Count;
        BuildingController[] buildingControls1 = ModelTarget.GetComponentsInChildren<BuildingController>(true);
        if (isNew || Setting == null)
        {
            Setting = new BuildingSceneLoadSetting();
        }

        foreach (var bc in buildingControls1)
        {
            BuildingModelInfo[] models = bc.GetComponentsInChildren<BuildingModelInfo>(true);
            foreach (var model in models)
            {
                modelList.Remove(model);
            }
        }
        for (int i = 0; i < modelList.Count; i++)
        {
            BuildingModelInfo model = modelList[i];
            Debug.Log($"model[{i}]:{model.name}");
            if (model.transform.GetComponent<BuildingController>() == null) continue;//数据库中没信息的建筑，当环境放在场景中，不需要再添加controller wk
            BuildingController bc = model.gameObject.AddMissingComponent<BuildingController>();
            if (string.IsNullOrEmpty(bc.NodeName))
            {
                bc.NodeName = bc.name;
            }
        }
        BuildingController[] buildingControls2 = ModelTarget.GetComponentsInChildren<BuildingController>(true);
        //Setting = new BuildingSceneLoadSetting();
        foreach (var bc in buildingControls2)
        {
            BuildingSceneLoadItemCollection bItem = Setting.AddItem(bc, isNew);
            BuildingModelInfo[] models = bc.GetComponentsInChildren<BuildingModelInfo>(true);
            foreach (var model in models)
            {
                bItem.AddItem(model, isNew);
                modelList.Remove(model);
            }
        }

        for (int i = 0; i < modelList.Count; i++)
        {
            BuildingModelInfo b = modelList[i];
            BuildingSceneLoadItemCollection bItem = Setting.AddItem(b, isNew);
            bItem.IsStatic = true;
            bItem.AddItem(b, isNew);
        }

        //string xml=XmlSerializableHelper.
        Debug.Log($"InitSettingByScene({isNew}) buildingControls1:{buildingControls1.Length} buildingControls2:{buildingControls2.Length} modelList:{modelList.Count} allCount:{allCount}");
    }

    //public string XmlFilePath = "d:\\BuildingSceneLoadSetting.xml";

#if UNITY_WEBGL
        public static string XmlFilePath = "\\BuildingSceneLoadSetting.XML";
#else
    public static string XmlFilePath = "\\..\\BuildingSceneLoadSetting.XML";
#endif

    public string GetXmlFilePath()
    {
        string path = Application.dataPath + XmlFilePath;
        return path;
    }

    [ContextMenu("SaveXml")]
    public void SaveXml()
    {
        string xml = SerializeHelper.GetXmlText(Setting);
        Debug.Log($"SaveXml xml:{xml}");
        File.WriteAllText(GetXmlFilePath(), xml);
    }

    [ContextMenu("LoadXml")]
    public void LoadSettingXml()
    {
        string path = GetXmlFilePath();
        if (File.Exists(path) == false)
        {
            Debug.LogError($"LoadXml FileNotFound path:{path}");
            return;
        }
        string xml = File.ReadAllText(path);
        Debug.Log($"LoadXml xml:{xml}");
        BuildingSceneLoadSetting setting = SerializeHelper.LoadFromText<BuildingSceneLoadSetting>(xml);
        Setting = setting;
        //Setting.SetAllEnable(true);
        Debug.Log($"LoadXml setting:{setting} xml:{xml}");
    }

    [ContextMenu("SetAllEnable")]
    public void SetAllEnable()
    {
        if (Setting != null)
        {
            Setting.SetAllEnable(true);
        }
    }

    [ContextMenu("EnableOut1")]
    public void EnableOut1()
    {
        if (Setting != null)
        {
            Setting.EnableOut1(true);
        }
    }

    [ContextMenu("DisableOut1")]
    public void DisableOut1()
    {
        if (Setting != null)
        {
            Setting.EnableOut1(false);
        }
    }

    [ContextMenu("EnableIn")]
    public void EnableIn()
    {
        if (Setting != null)
        {
            Setting.EnableIn(true);
        }
    }

    [ContextMenu("DisableIn")]
    public void DisableIn()
    {
        if (Setting != null)
        {
            Setting.EnableIn(false);
        }
    }

    [ContextMenu("EnableLOD")]
    public void EnableLOD()
    {
        if (Setting != null)
        {
            Setting.EnableLOD(true);
        }
    }

    [ContextMenu("DisableLOD")]
    public void DisableLOD()
    {
        if (Setting != null)
        {
            Setting.EnableLOD(false);
        }
    }

    public BuildingSceneLoadSetting Setting = new BuildingSceneLoadSetting();
}

[Serializable]
public class BuildingSceneLoadSetting
{
    [XmlAttribute]
    public bool IsLoadUserBuildings = false;

    public List<string> UserBulindgs = new List<string>();//IsLoadUserBuildings==trueʱ�����Խ����ݿ⣬�����õ��б��м��ء�

    //[XmlAttribute]
    //public bool IsLoadByDistance = true;
    [XmlAttribute]
    public bool IsLoadSceneOnStart = false;
    [XmlAttribute]
    public bool Enabled = true;
    [XmlAttribute]
    public bool IsEnableUnload = true;
    

    public List<BuildingSceneLoadItemCollection> Items = new List<BuildingSceneLoadItemCollection>();

    public BuildingSceneLoadItemCollection AddItem(BuildingModelInfo bc, bool isNew)
    {
        BuildingSceneLoadItemCollection bItem = GetBuilding(bc);
        if (isNew || bItem == null)
        {
            bItem = new BuildingSceneLoadItemCollection();
            bItem.Name = bc.name;
            if (string.IsNullOrEmpty(bItem.Name))
            {
                bItem.Name = bc.name;
            }
            this.Items.Add(bItem);

            if (isNew == false)
            {
                Debug.LogError($"BuildingSceneLoadSetting.AddItem BuildingController:{bc.name}");
            }
        }
        return bItem;
    }

    public BuildingSceneLoadItemCollection AddItem(BuildingController bc,bool isNew)
    {
        BuildingSceneLoadItemCollection bItem = GetBuilding(bc);
        if(isNew || bItem == null)
        {
            bItem = new BuildingSceneLoadItemCollection();
            bItem.Name = bc.NodeName;
            if (string.IsNullOrEmpty(bItem.Name))
            {
                bItem.Name = bc.name;
            }
            this.Items.Add(bItem);

            if (isNew == false)
            {
                Debug.LogError($"BuildingSceneLoadSetting.AddItem BuildingController:{bc.name}");
            }
        }
        return bItem;
    }

    //public BuildingSceneLoadItem GetItem(string name)
    //{
    //    return null;
    //}

    //public List<BuildingSceneLoadItem> GetAllItems()
    //{
    //    List<BuildingSceneLoadItem> list = new List<BuildingSceneLoadItem>();
    //    return list;
    //}

    public override string ToString()
    {
        return $"BuildingSceneLoadSetting({Items.Count}) ";
    }

    public void SetAllEnable(bool e)
    {
        foreach (var item in Items)
        {
            item.SetAllEnable(e);
        }
    }

    public void EnableOut1(bool e)
    {
        foreach (var item in Items)
        {
            item.EnableOut1(e);
        }
    }

    public void EnableIn(bool e)
    {
        foreach (var item in Items)
        {
            item.EnableIn(e);
        }
    }

    public void EnableLOD(bool e)
    {
        foreach (var item in Items)
        {
            item.EnableLOD(e);
        }
    }

    internal BuildingSceneLoadItemCollection GetBuilding(BuildingModelInfo bc)
    {
        //foreach (var item in Items)
        //{
        //    if (item.Name == bc.NodeName)
        //    {
        //        return item;
        //    }
        //}
        foreach (var item in Items)
        {
            if (item.Name == bc.name)
            {
                return item;
            }
        }
        return null;
    }

    internal BuildingSceneLoadItemCollection GetBuilding(BuildingController bc)
    {
        foreach (var item in Items)
        {
            if (item.Name == bc.NodeName )
            {
                return item;
            }
        }
        foreach (var item in Items)
        {
            if (item.Name == bc.name)
            {
                return item;
            }
        }
        return null;
    }

    internal BuildingSceneLoadItemCollection GetBuilding(string name)
    {
        foreach(var item in Items)
        {
            if (item.Name == name)
            {
                return item;
            }
        }
        return null;
    }
}

[Serializable]
[XmlType("BuildingSceneLoadItemCollection")]
public class BuildingSceneLoadItemCollection
{
    [XmlAttribute]
    public string Name;
    [XmlAttribute]
    public bool IsEnable = true;
    [XmlAttribute]
    public bool IsStatic = false;


    [XmlElement("BuildingSceneLoadItem")]
    public List<BuildingSceneLoadItem> Children = new List<BuildingSceneLoadItem>();

    public BuildingSceneLoadItem AddItem(BuildingModelInfo model, bool isNew)
    {
        BuildingSceneLoadItem fItem = GetChild(model.name);
        if(isNew || fItem == null)
        {
            fItem = new BuildingSceneLoadItem();
            if (model.LODPart != null)
            {
                fItem.LODs = model.LODPart.isStatic;
            }
            //fItem.Name = fl.NodeName;
            //if (string.IsNullOrEmpty(fItem.Name))
            //{
            fItem.Name = model.name;
            //}
            Children.Add(fItem);

            if (isNew == false)
            {
                Debug.LogError($"BuildingSceneLoadItemCollection.AddItem BuildingModelInfo:{model.name}");
            }
        }
        return fItem;
    }

    public void SetAllEnable(bool e)
    {
        foreach (var item in Children)
        {
            item.SetAllEnable(e);
        }
    }

    public void EnableOut1(bool e)
    {
        foreach (var item in Children)
        {
            item.EnableOut1(e);
        }
    }

    public void EnableIn(bool e)
    {
        foreach (var item in Children)
        {
            item.EnableIn(e);
        }
    }

    public void EnableLOD(bool e)
    {
        foreach (var item in Children)
        {
            item.EnableLOD(e);
        }
    }


    public override string ToString()
    {
        //return $"[Name:{Name} InCombined:{InCombined} Out0Combined:{Out0Combined} Out1Combined:{Out1Combined} InRenderers:{InRenderers} Out0Renderers:{Out0Renderers} Out1Renderers:{Out1Renderers}]";
        return $"[Name:{Name} IsEnable:{IsEnable}]";
    }

    internal BuildingSceneLoadItem GetChild(string name)
    {
        foreach(var child in Children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }
        return null;
    }
}

[Serializable]
[XmlType("BuildingSceneLoadItem")]
public class BuildingSceneLoadItem
{
    public void EnableIn(bool e)
    {
        InRenderers = e;
        foreach (var item in Children)
        {
            item.EnableIn(e);
        }
    }

    public void EnableOut1(bool e)
    {
        Out1Renderers = e;
        foreach (var item in Children)
        {
            item.EnableOut1(e);
        }
    }

    public void EnableLOD(bool e)
    {
        LODs = e;
        foreach (var item in Children)
        {
            item.EnableLOD(e);
        }
    }

    public void SetAllEnable(bool e)
    {
        IsEnable = e;
        //InCombined = e;
        //Out0Combined = e;
        //Out1Combined = e;
        InRenderers = e;
        Out0RenderersB = e;
        Out0RenderersS = e;
        Out1Renderers = e;
        LODs = e;

        foreach (var item in Children)
        {
            item.SetAllEnable(e);
        }
    }

    [XmlAttribute]
    public string Name;
    [XmlAttribute]
    public bool IsEnable;
    //[XmlAttribute]
    //public bool InCombined;
    [XmlAttribute]
    public bool InRenderers;
    //[XmlAttribute]
    //public bool Out0Combined;
    [XmlAttribute]
    public bool Out0RenderersB;
    [XmlAttribute]
    public bool Out0RenderersS;
    //[XmlAttribute]
    //public bool Out1Combined;
    [XmlAttribute]
    public bool Out1Renderers;
    [XmlAttribute]
    public bool LODs;

    public BuildingSceneLoadItem()
    {
        IsEnable = true;
        //Out0Combined = true;
        Out0RenderersB = true;
        //Out0RenderersS = true;
        //SetAllEnable(true);
    }

    [XmlElement("BuildingSceneLoadItem")]
    public List<BuildingSceneLoadItem> Children = new List<BuildingSceneLoadItem>();

    public override string ToString()
    {
        //return $"[Name:{Name} InCombined:{InCombined} Out0Combined:{Out0Combined} Out1Combined:{Out1Combined} InRenderers:{InRenderers} Out0Renderers:{Out0Renderers} Out1Renderers:{Out1Renderers}]";
        return $"[Name:{Name} In:{InRenderers} Out0B:{Out0RenderersB} Out0S:{Out0RenderersS} Out1:{Out1Renderers} LODs:{LODs} ] ";
    }
}
using Base.Common;
using Location.WCFServiceReferences.LocationServices;
using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class InitNavisFileInfoByModel : SingletonBehaviour<InitNavisFileInfoByModel>
{
    //public List<NavisModelRoot> ModelRoots = new List<NavisModelRoot>();

    //[ContextMenu("GetModelRoots")]
    //public void GetModelRootObjects()
    //{
    //    ModelRoots = GameObject.FindObjectsOfType<NavisModelRoot>(true).ToList();

    //}

    //[ContextMenu("RootsBindBim")]
    //public void RootsBindBim()
    //{
    //    for(int i = 0; i < ModelRoots.Count; i++)
    //    {
    //        var root = ModelRoots[i];
    //        root.BindBimInfo();
    //    }
    //}

    public bool IsShowAll = false;

    public bool IsFindClosedFloor = true;

    public GameObject factory;
    public bool AddFactoryToXml;
    // Start is called before the first frame update
    void Start()
    {
        string path = GetNavisFilePath();
        string path2 = GetNavisFilePath_New();
        ThreadManager.Run(() =>
        {
            //InitNavisFileInfoByModel.GetNavisFileInfoEx_New();

            InitNavisFileInfoByModel.GetNavisFileInfoEx(path2);

            InitNavisFileInfoByModel.GetNavisFileInfoEx(path);
            
        }, () => { },
        "");
    }
    [ContextMenu("DestoryFactoryNavisInfo")]
    public void DestoryNavisInfo()
    {
        if (factory == null)
        {
            Debug.LogError("Factory is null,please check!");
            return;
        }
        DestoryNavisInfo(factory);
    }

    public static void DestoryNavisInfo(GameObject root)
    {
        BIMModelInfo[] infos = null;
        if (root != null)
        {
            infos = root.GetComponentsInChildren<BIMModelInfo>(true);
        }
        else
        {
            infos = GameObject.FindObjectsOfType<BIMModelInfo>(true);
        }
        if (infos != null)
        {
            for (int i = infos.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(infos[i]);
            }
        }
        Debug.LogError($"DestoryNavisInfo infos:{infos.Length}");
    }

    public List<BIMModelInfo> bimInfos;

    [ContextMenu("GetBims")]
    public void GetBims()
    {
        bimInfos = new List<BIMModelInfo>();
        foreach (var b in InitNavisFileInfoByModelSetting.Instance.initInfoBuildings)
        {
            if (b == null) continue;
            if (b.activeInHierarchy == false) continue;
            bimInfos.AddRange(b.GetComponentsInChildren<BIMModelInfo>(true));
        }

        bimInfos.Sort();
        Debug.LogError($"GetBims infos:{bimInfos.Count}");
    }

    public void RemoveNotFound()
    {

    }

    int sumChild = 0;
    [ContextMenu("InitNavisFileInfoWithDetail")]
    public void InitNavisFileInfoWithDetail()
    {
        sumChild = 0;
        InitInfo(true);
    }

    [ContextMenu("InitNavisFileInfoWithoutDetail")]
    public void InitInfoWithoutDetail()
    {
        sumChild = 0;
        InitInfo(false);
    }
    private void InitInfo(bool withDetail)
    {
        if (factory == null)
        {
            Debug.LogError("Factory is null,please check!");
            return;
        }
        NavisFileInfo info = new NavisFileInfo();
        info.FileName = "";
        info.Models = new List<ModelItemInfo>();
        if (AddFactoryToXml)
        {
            AddModelItemInfo(factory.transform, info.Models, withDetail);
        }
        else
        {
            foreach (Transform childT in factory.transform)
            {
                AddModelItemInfo(childT, info.Models, withDetail);
            }
        }


        string fileName = withDetail ? "\\..\\NavisFileInfoDetail.xml" : "\\..\\NavisFileInfoNoDetail.xml";
        string path = Application.dataPath + fileName;
        SerializeHelper.Save(info, path);
        Debug.Log("SaveComplete,SumChild:" + sumChild);
    }

    private void AddModelItemInfo(Transform childT, List<ModelItemInfo> modelInfos, bool withDetail)
    {
        BIMModelInfo bimInfo = childT.gameObject.AddMissingComponent<BIMModelInfo>();
        if (string.IsNullOrEmpty(bimInfo.Guid)) bimInfo.Guid = Guid.NewGuid().ToString();
        bimInfo.MName = childT.name;

        ModelItemInfo info = new ModelItemInfo();
        info.UId = bimInfo.Guid;
        info.Name = childT.name;
        info.X = childT.transform.position.x;
        info.Y = childT.transform.position.y;
        info.Z = childT.transform.position.z;



        if (withDetail)
        {
            info.Categories = new List<PropertyCategoryInfo>();
            AddCateGories(childT, ref info.Categories);
        }
        info.Children = new List<ModelItemInfo>();
        modelInfos.Add(info);
        sumChild++;
        foreach (Transform subChild in childT)
        {
            AddModelItemInfo(subChild, info.Children, withDetail);
        }
    }

    private void AddCateGories(Transform child, ref List<PropertyCategoryInfo> listT)
    {
        PropertyCategoryInfo info = new PropertyCategoryInfo();
        info.DisplayName = "模型基础信息";
        info.Properties = new List<DataPropertyInfo>();
        MeshFilter f = child.GetComponent<MeshFilter>();
        if (f != null && f.sharedMesh != null)
        {
            Vector3 length = f.sharedMesh.bounds.size;
            float xlength = length.x * child.lossyScale.x;
            float ylength = length.y * child.lossyScale.y;
            float zlength = length.z * child.lossyScale.z;

            info.Properties.Add(TryGetCateInfo("长", xlength.ToString()));
            info.Properties.Add(TryGetCateInfo("宽", zlength.ToString()));
            info.Properties.Add(TryGetCateInfo("高", ylength.ToString()));

            info.Properties.Add(TryGetCateInfo("VertexCount:", f.sharedMesh.vertexCount.ToString()));
            info.Properties.Add(TryGetCateInfo("SblingIndex:", child.GetSiblingIndex().ToString()));
        }
        else
        {
            info.Properties.Add(TryGetCateInfo("长", child.lossyScale.x.ToString()));
            info.Properties.Add(TryGetCateInfo("宽", child.lossyScale.x.ToString()));
            info.Properties.Add(TryGetCateInfo("高", child.lossyScale.x.ToString()));

            info.Properties.Add(TryGetCateInfo("VertexCount:", "0"));
            info.Properties.Add(TryGetCateInfo("SblingIndex:", child.GetSiblingIndex().ToString()));
        }
        listT.Add(info);
    }

    private DataPropertyInfo TryGetCateInfo(string nameDisplay, string value)
    {
        DataPropertyInfo t = new DataPropertyInfo();
        t.DisplayName = nameDisplay;
        t.Value = value;
        return t;
    }



    //public List<GameObject> initInfoBuildings = new List<GameObject>();
    private int findCount;
    private int totalCount;

    private List<ModelItemInfo> newModelItemInfo;

    public void ClearBuildings()
    {
        InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Clear();
    }

    public void UpdateBuildings()
    {
        ////initInfoBuildings.Clear();
        //var bs1 = InitNavisFileInfoByModelSetting.Instance.initInfoBuildings;
        //InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Clear();
        //foreach (var b in bs1)
        //{
        //    if (b == null) continue;
        //    InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Add(b.gameObject);
        //}

        //BuildingController[] bs = GameObject.FindObjectsOfType<BuildingController>(true);
        //foreach (var b in bs)
        //{
        //    if (InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Contains(b.gameObject)) continue;
        //    InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Add(b.gameObject);
        //}

        InitNavisFileInfoByModelSetting.Instance.UpdateBuildings();
    }

    /*
     * 1.给建筑节点增加BimModelInfo信息，生成NavisFileInfo_NewInfo.xml，用于服务端导入数据库中.
     * 2.生成本地用拓扑树
     * 3.给BimModelInfo绑定ModelId,用于加载精细模型
     */
    [ContextMenu("1.建筑绑定信息，生成数据库用Xml")]
    public void CompareNavisFileInfo()
    {
        if (InitNavisFileInfoByModelSetting.Instance.initInfoBuildings == null)
        {
            Debug.LogError("NeedToCompareObj is null...");
            return;
        }

        UpdateBuildings();

        findCount = 0;
        totalCount = 0;
        string fileName = "\\..\\NavisFileInfo_Basic.xml";
        string path = Application.dataPath + fileName;
        DateTime recordT = DateTime.Now;
        var navisFiles = SerializeHelper.LoadFromFile<NavisFileInfo>(path);
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        if (navisFiles != null && navisFiles.Models != null)
        {
            models.AddRange(navisFiles.Models);
            foreach (var item in navisFiles.Models)
            {
                models.AddRange(GetChildItemInfo(item));
            }
        }

        newModelItemInfo = new List<ModelItemInfo>();

        Dictionary<string, ModelItemInfo> dicT = ToModelDic(models);
        foreach (var building in InitNavisFileInfoByModelSetting.Instance.initInfoBuildings)
        {
            if (building == null) continue;
            FindCompareInfo(building.transform, dicT);
        }

        if (newModelItemInfo.Count > 0)
        {
            NavisFileInfo info = new NavisFileInfo();
            info.FileName = "";
            info.Models = new List<ModelItemInfo>();
            info.Models.AddRange(newModelItemInfo);

            string newModelInfo = "\\..\\NavisFileInfo_NewInfo.xml";
            string pathNew = Application.dataPath + newModelInfo;
            SerializeHelper.Save(info, pathNew);
        }

        Debug.LogErrorFormat("FindComplete,CostTime:{0} findCount:{1} TotalModelCount:{2} TotalNavisInfoCount:{3} NewModelItemCount:{4}"
            , (DateTime.Now - recordT).TotalSeconds, findCount, totalCount, dicT.Count, newModelItemInfo.Count);
    }

    private void FindCompareInfo(Transform parentT, Dictionary<string, ModelItemInfo> modelDicT)
    {
        if (parentT.childCount == 0) return;
        foreach (Transform child in parentT)
        {
            totalCount++;
            if (modelDicT.ContainsKey(child.name))
            {
                BIMModelInfo bimT = child.gameObject.AddMissingComponent<BIMModelInfo>();
                if (bimT)
                {
                    findCount++;
                    ModelItemInfo infoT = modelDicT[child.name];
                    bimT.Guid = infoT.UId;
                    bimT.Position1 = infoT.GetPositon();
                    RendererId renderId = child.GetComponent<RendererId>();
                    if (renderId)
                    {
                        bimT.RenderId = renderId.Id;
                    }
                }
            }
            else
            {
                BIMModelInfo bimT = child.gameObject.AddMissingComponent<BIMModelInfo>();
                if (bimT)
                {
                    bimT.Guid = Guid.NewGuid().ToString();
                    RendererId renderId = child.GetComponent<RendererId>();
                    if (renderId)
                    {
                        bimT.RenderId = renderId.Id;
                    }
                    bimT.Position1 = child.position;

                    ModelItemInfo info = new ModelItemInfo();
                    info.UId = bimT.Guid;
                    info.Name = child.gameObject.name;
                    info.X = child.position.x;
                    info.Y = child.position.y;
                    info.Z = child.position.z;

                    info.Categories = new List<PropertyCategoryInfo>();
                    AddCateGories(child, ref info.Categories);
                    newModelItemInfo.Add(info);
                }
            }
            FindCompareInfo(child, modelDicT);
        }
    }
    private Dictionary<string, ModelItemInfo> ToModelDic(List<ModelItemInfo> models)
    {
        Dictionary<string, ModelItemInfo> dicT = new Dictionary<string, ModelItemInfo>();
        int repeatCount = 0;
        StringBuilder sb = new StringBuilder();
        foreach (var item in models)
        {
            if (!dicT.ContainsKey(item.Name))
            {
                dicT.Add(item.Name, item);
            }
            else
            {
                //Debug.LogError($"重复的模型名称:{item.Name}");
                repeatCount++;
                sb.AppendLine(item.Name);
            }
        }
        Debug.LogError($"ToModelDic dicT:{dicT.Count} repeatCount:{repeatCount} repeatNames:\n{sb.ToString()}");
        return dicT;
    }
    private static List<ModelItemInfo> GetChildItemInfo(ModelItemInfo navisT)
    {
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        if (navisT.Children != null)
        {
            foreach (ModelItemInfo child in navisT.Children)
            {
                if (child.IsZero())
                {
                    continue;
                }
                models.Add(child);
            }
            //models.AddRange(navisT.Children);

            foreach (var item in navisT.Children)
            {
                models.AddRange(GetChildItemInfo(item));
            }
        }
        return models;
    }

    [ContextMenu("2.生成建筑拓扑树")]
    public void SaveNavisFileinfo()
    {
        NavisFileInfo info = new NavisFileInfo();
        info.FileName = "";
        info.Models = new List<ModelItemInfo>();
        foreach (var building in InitNavisFileInfoByModelSetting.Instance.initInfoBuildings)
        {
            if (building == null) continue;
            SaveModelItemInfo(building.transform, info.Models);
        }

        string fileName = "\\..\\NavisFileInfoNoDetail.xml";
        string path = Application.dataPath + fileName;
        SerializeHelper.Save(info, path);
    }

    private void SaveModelItemInfo(Transform childT, List<ModelItemInfo> modelInfos)
    {
        BIMModelInfo bimInfo = childT.gameObject.AddMissingComponent<BIMModelInfo>();
        if (string.IsNullOrEmpty(bimInfo.Guid)) bimInfo.Guid = Guid.NewGuid().ToString();
        bimInfo.MName = childT.name;

        ModelItemInfo info = new ModelItemInfo();
        info.UId = bimInfo.Guid;
        info.RenderId = bimInfo.RenderId;
        info.RenderName = bimInfo.name;
        info.Name = childT.name;
        info.X = bimInfo.Position1.x;
        info.Y = bimInfo.Position1.y;
        info.Z = bimInfo.Position1.z;
        DepNode dep = childT.GetComponentInParent<DepNode>();
        if (dep && dep.TopoNode != null)
        {
            info.AreaId = dep.TopoNode.Id;
        }
        else
        {
            info.AreaId = dep.NodeID;
        }

        info.Children = new List<ModelItemInfo>();
        modelInfos.Add(info);
        foreach (Transform subChild in childT)
        {
            SaveModelItemInfo(subChild, info.Children);
        }
    }

    [ContextMenu("3.绑定RenderId")]
    public void BindingRenderId()
    {
        if (factory != null)
        {
            BIMModelInfo[] infos = factory.GetComponentsInChildren<BIMModelInfo>();
            foreach (var item in infos)
            {
                RendererId renderId = item.transform.GetComponent<RendererId>();
                if (renderId)
                {
                    item.RenderId = renderId.Id;
                }
            }
        }
    }



    #region 设备信息绑定，由VUE生产树模型

    public GameObject modelTest;
    public string modelTestUid;
    [ContextMenu("测试模型信息和实际位置偏差值")]
    public void Test()
    {
        DateTime recordTime = DateTime.Now;
        string fileName = "\\..\\NavisFileInfo_Vue.xml";
        string path = Application.dataPath + fileName;
        var navisFiles = SerializeHelper.LoadFromFile<NavisFileInfo>(path);
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        if (navisFiles != null && navisFiles.Models != null)
        {
            models.AddRange(navisFiles.Models);
            foreach (var item in navisFiles.Models)
            {
                models.AddRange(GetChildItemInfo(item));
            }
        }
        Debug.Log("LoadInfo,CostTime:" + (DateTime.Now - recordTime).TotalSeconds);
        recordTime = DateTime.Now;
        var modelxxx = models.Find(i => i.UId == modelTestUid);
        Debug.Log("models.Find,CostTime:" + (DateTime.Now - recordTime).TotalSeconds);
        if (modelxxx != null)
        {
            Vector3 modelInfoPos = modelxxx.GetPositon();
            Debug.LogFormat("modelInfoPos:{0}  UnityModelPos:{1} offset:{2}"
                , modelInfoPos, modelTest.transform.position, Vector3.Distance(modelInfoPos, modelTest.transform.position));
        }
    }

    public List<string> RootModels = new List<string>();

    //public bool IsFindInCurrentModels = false;

    //public List<string> CurrentModels = new List<string>();

    [ContextMenu("GetRootModels")]
    public void GetRootModels()
    {
        
        navisFile = GetNavisFileInfo();
        if (navisFile != null && navisFile.Models != null)
        {
            GetRootModels(navisFile.Models);
        }
    }

    public void GetRootModels(List<ModelItemInfo> models)
    {
            RootModels.Clear();
            foreach (var model in models)
            {
                RootModels.Add(model.Name);
            }
    }

    public static NavisFileInfo file = null;

    public static NavisFileInfo GetNavisFileInfoEx()
    {
        Debug.Log($"GetNavisFileInfoEx [file:{file}]");
        if (file == null)
        {
            file = GetNavisFileInfo();
        }
        return file;
    }

    public static NavisFileInfo file_new = null;//专业目录树

    public static NavisFileInfo GetNavisFileInfoEx_New()
    {
        Debug.Log($"GetNavisFileInfoEx_New [file:{file_new}]");
        if (file_new == null)
        {
            file_new = GetNavisFileInfo_New();
        }
        return file_new;
    }

    public static Dictionary<string, NavisFileInfo> files = new Dictionary<string, NavisFileInfo>();

    public static NavisFileInfo GetNavisFileInfoEx(string path)
    {
        NavisFileInfo navFile = null;
        if (!files.ContainsKey(path))
        {
            navFile = GetNavisFileInfo(path);
            files.Add(path, navFile);
        }
        else
        {
            navFile = files[path];
        }
        Debug.Log($"GetNavisFileInfoEx [file:{navFile}]");
        return navFile;
    }

    public static NavisFileInfo GetNavisFileInfo(string path)
    {
        Debug.Log($"GetNavisFileInfo path:{path}");
        if (File.Exists(path))
        {
            var navisFiles = SerializeHelper.LoadFromFile<NavisFileInfo>(path);
            return navisFiles;
        }
        else
        {
            return null;
        }
    }

    public static string GetNavisFilePath()
    {
        string fileName = "\\..\\NavisFileInfo.xml";
        string path = Application.dataPath + fileName;
        return path;
    }

    /// <summary>
    /// 专业目录树
    /// </summary>
    /// <returns></returns>


    public static NavisFileInfo GetNavisFileInfo()
    {
        string path = GetNavisFilePath();
        return GetNavisFileInfo(path);
    }

    public static NavisFileInfo GetNavisFileInfo_New()
    {
        string path = GetNavisFilePath_New();
        return GetNavisFileInfoEx(path);
    }
    public static string GetNavisFilePath_New()
    {
        string fileName = "\\..\\NavisFileInfo_New.xml";
        string path = Application.dataPath + fileName;
        return path;
    }

    private int posFindCount;


    public Transform testTarget = null;

    [ContextMenu("TestInitBIMModelInfoByPos")]
    public void TestInitBIMModelInfoByPos()
    {
        DateTime recordTime = DateTime.Now;
        List<ModelItemInfo> models = GetAllModelInfos(InitNavisFileInfoByModelSetting.Instance.IsFindInCurrentModels);
        bool r = InitBIMModelInfoByPos_Model2Vue(testTarget, models, InitNavisFileInfoByModelSetting.Instance.MinDistance1);
        Debug.Log($"TestInitBIMModelInfoByPos target:{testTarget} r:{r} CostTime:{(DateTime.Now - recordTime).TotalSeconds}s");
    }

    public List<ModelItemInfo> GetAllModelInfos(bool isFilterModel)
    {
        navisFile = GetNavisFileInfo();
        return GetAllModelInfos(navisFile, isFilterModel);
    }

    public List<ModelItemInfo> GetAllModelInfos(NavisFileInfo navisFiles, bool isFilterModel)
    {
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        if (navisFiles != null && navisFiles.Models != null)
        {
            vueRootModels = navisFiles.Models;
            

            foreach (var item in navisFiles.Models)
            {
                //if (item.X == 0 && item.Y == 0 && item.Z == 0) continue;
                if (isFilterModel && InitNavisFileInfoByModelSetting.Instance.CurrentModels.Count > 0 && InitNavisFileInfoByModelSetting.Instance.CurrentModels.Contains(item.Name) == false) continue;
                //models.Add(item);
                models.AddRange(GetChildItemInfo(item));
            }

            SortVueRootModels();
        }
        return models;
    }


    //public float MinDistance1 = 0.005f;

    //public float MinDistance2 = 0.05f;

    //public float MinDistance3 = 0.15f;

    //public float MinDistance4 = 0.3f;

    //public float MinDistance5 = 0.6f;

    public List<Transform> CompareModelVueInfo_Model2Vue(List<Transform> allTrans, List<ModelItemInfo> models, float minDis, int id)
    {
        DateTime recordTime = DateTime.Now;
        totalCount = 0;
        findCount = 0;
        posFindCount = 0;
        //var allTrans = GetAllChildren();
        Debug.Log($"CompareModelVueInfo_Model2Vue allTrans:{allTrans.Count} models:{models.Count}");
        StringBuilder notFoundModels = new StringBuilder();
        List<Transform> nodeFoundModelList = new List<Transform>();
        for (int i = 0; i < allTrans.Count; i++)
        {
            Transform t = allTrans[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CompareModelVueInfo_" + id, i, allTrans.Count, t));
            if (t == null) continue;
            bool r = InitBIMModelInfoByPos_Model2Vue(t, models, minDis);
            if (r == false)
            {

                nodeFoundModelList.Add(t);
            }
        }
        nodeFoundModelList.Sort((a, b) => a.name.CompareTo(b.name));

        foreach (var t in nodeFoundModelList)
        {
            notFoundModels.AppendLine(t.name);
        }
        Debug.LogErrorFormat($"FindComplete_{id},CostTime:{(DateTime.Now - recordTime).TotalSeconds} nameFindCount:{findCount} posFindCount:{posFindCount} TotalModelCount:{totalCount} TotalNavisInfoCount:{models.Count},notFoundModels:{nodeFoundModelList.Count}\n{notFoundModels}");
        return nodeFoundModelList;
    }


    [ContextMenu("匹配Vue属性树1")]
    public void CompareModelVueInfo_Model2Vue()
    {
        DateTime recordTime = DateTime.Now;
        InitNavisFileInfoByModelSetting setting = InitNavisFileInfoByModelSetting.Instance;
        navisFile = GetNavisFileInfo();
        List<ModelItemInfo> models = GetAllModelInfos(navisFile, setting.IsFindInCurrentModels);
        //Dictionary<string, ModelItemInfo> dicT = ToModelDicByUid(models);
        //foreach (var item in initInfoBuildings)
        //{
        //    if (item == null) continue;
        //    FindCompareInfo_VUE(item.transform, models);
        //}
        var allTrans = GetAllModelTransform();
        allTrans = CompareModelVueInfo_Model2Vue(allTrans, models, setting.MinDistance1, 1);
        allTrans = CompareModelVueInfo_Model2Vue(allTrans, models, setting.MinDistance2, 2);
        allTrans = CompareModelVueInfo_Model2Vue(allTrans, models, setting.MinDistance3, 3);

        var doors = allTrans.FindAll(a => a.name.Contains("Door"));
        var notDoors = allTrans.FindAll(a => !a.name.Contains("Door"));
        doors = CompareModelVueInfo_Model2Vue(doors, models, setting.MinDistance4, 4);
        doors = CompareModelVueInfo_Model2Vue(doors, models, setting.MinDistance5, 5);

        ProgressBarHelper.ClearProgressBar();

        SaveNavisFile();
        Debug.LogErrorFormat($"CompareModelVueInfo_Model2Vue,{ (DateTime.Now - recordTime).TotalSeconds}");
    }

    [ContextMenu("TestGetAllModelInfos")]
    public void TestGetAllModelInfos()
    {
        DateTime recordTime = DateTime.Now;
        navisFile = GetNavisFileInfo();
        List<ModelItemInfo> models = GetAllModelInfos(navisFile, InitNavisFileInfoByModelSetting.Instance.IsFindInCurrentModels);
        Debug.Log($"TestGetAllModelInfos models:{models.Count} CostTime:{(DateTime.Now - recordTime).TotalSeconds}s");
    }

    [ContextMenu("GetCompareList")]
    public void GetCompareList()
    {
        this.GetVueModels(false);
        this.GetAllModelInfos(false);
        this.GetAllModelTransform();
    }

    [ContextMenu("ClearRendererId")]
    public void ClearRendererId()
    {
        DateTime start = DateTime.Now;

        this.GetVueModels(false);

        int rendererIdCount = 0;
        var all = navisFile.GetAllItems();
        foreach (var item in all)
        {
            if (!string.IsNullOrEmpty(item.RenderId))
            {
                rendererIdCount++;
            }
            item.RenderId =null;
            item.RenderName = null;
        }

        SaveNavisFile();

        Debug.Log($"ClearRendererId time:{DateTime.Now - start} rendererIdCount:{rendererIdCount}");
    }

    private Dictionary<string, GameObject> buildingNames = new Dictionary<string, GameObject>();

    public void InitBuildingNames()
    {
        buildingNames.Clear();
        ClearBuildings();
        UpdateBuildings();
        for (int i = 0; i < InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Count; i++)
        {
            GameObject b = InitNavisFileInfoByModelSetting.Instance.initInfoBuildings[i];
            string name = b.name;
            BuildingController bc = b.GetComponent<BuildingController>();
            if (bc != null)
            {
                //bc.NodeName
                if (!string.IsNullOrEmpty(bc.NodeName))
                {
                    name = bc.NodeName;
                }
                
            }
            string[] parts = name.Split('_');
            if (parts.Length == 2)
            {
                name = parts[1];
            }
            name = name.Replace("及厂家设备", "");
            if(bc!=null&& string.IsNullOrEmpty(bc.NodeName))
            {
                bc.NodeName = name;
            }

            if (buildingNames.ContainsKey(name))
            {
                Debug.LogError($"InitBuildingNames[{i}] b:{b.name} name:{name} buildingNames.ContainsKey(name)");
            }
            else
            {
                buildingNames.Add(name, b);
                Debug.Log($"InitBuildingNames[{i}] b:{b.name} name:{name}");
            }
            
        }
    }

    public void FindVueBuildingsByName()
    {
        InitBuildingNames();
        List<ModelItemInfo> notFoundVueBuildings = new List<ModelItemInfo>();

        for (int i = 0; i < vueRootModels.Count; i++)
        {
            ModelItemInfo vue = vueRootModels[i];

            string name = vue.Name;
            string[] parts = name.Split('-');
            if (parts.Length == 2)
            {
                name = parts[1];
            }
            FindVueBuildingsByName(i, vue, name);
        }
        SortVueRootModels();
    }

    private void FindVueBuildingsByName(int i, ModelItemInfo vue, string name)
    {
        if(name=="H级主厂房")
        {
            vue.AreaName = "H级燃机厂房;H级汽机厂房";
            return;
        }
        vue.AreaName = "";
        vue.AreaGos.Clear();
        if (buildingNames.ContainsKey(name))
        {
            vue.AreaName = name;
            vue.AreaGos.Add(buildingNames[name]);
            Debug.Log($"FindVueBuildingsByName[{i}] Find1  name:{name}");
        }
        else
        {
            foreach (var bn in buildingNames.Keys)
            {
                if (bn.Contains(name) || name.Contains(bn))
                {
                    Debug.LogWarning($"FindVueBuildingsByName[{i}] Find2  name:{name}");
                    vue.AreaName += bn + ";";
                    vue.AreaGos.Add(buildingNames[bn]);
                }
            }
            if (string.IsNullOrEmpty(vue.AreaName))
            {
                Debug.LogError($"FindVueBuildingsByName[{i}] not found! name:{name} ");
            }
        }
    }

    public bool IsDebug = false;

    [ContextMenu("GetVueModels")]
    public void GetVueModels(bool isClear)
    {
        DateTime recordTime = DateTime.Now;
        navisFile = GetNavisFileInfo();
        var list1 = GetAllModelInfos(navisFile, true);
        List<ModelItemInfo> list2 = new List<ModelItemInfo>();
        foreach (var item in list1)
        {
            if (item.IsZero()) continue;
            list2.Add(item);

        }
        vueAllModels = list2;
        if (isClear)
        {
            foreach (var vueM in vueAllModels)
            {
                vueM.AreaId = 0;
                vueM.AreaName = null;
                vueM.RenderId = null;
                vueM.RenderName = null;
            }
        }
        Debug.Log($"GetVueModels time:{(DateTime.Now - recordTime)} list1:{list1.Count} list2:{list2.Count}");
    }

    [HideInInspector]
    public List<ModelItemInfo> vueRootModels = new List<ModelItemInfo>();

    public void SortVueRootModels()
    {
        foreach(var m in vueRootModels)
        {
            if (string.IsNullOrEmpty(m.AreaName))
            {
                m.AreaName = "";
            }
        }
        vueRootModels.Sort((a, b) =>
        {
            //if (a == null) return -1;
            int r = a.AreaName.CompareTo(b.AreaName);
            if (r == 0)
            {
                r = a.Name.CompareTo(b.Name);
            }
            return r;
        });
    }

    private List<ModelItemInfo> vueAllModels = new List<ModelItemInfo>();

    public List<ModelItemInfo> GetVueAllModels()
    {
        return vueAllModels;
    }

    //public bool enableDistance2 = false;

    //public bool enableDistance3 = false;

    private NavisFileInfo navisFile = null;

    //private ProgressArg p1 = null;

    public ProgressArg progressArg = null;

    public void BindBimInfo()
    {
        navisFile = GetNavisFileInfo();
        var vueItems = navisFile.GetAllItems();
        if (vueItems.Count == 0)
        {
            Debug.LogError("BindBimInfo vueItems.Count == 0");
            return;
        }
        Dictionary<string, ModelItemInfo> vueDict = new Dictionary<string, ModelItemInfo>();
        for (int i = 0; i < vueItems.Count; i++)
        {
            ModelItemInfo item = vueItems[i];
            ProgressArg p1 = new ProgressArg("BindBimInfo1", i, vueItems.Count, item.Name);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);
            if (string.IsNullOrEmpty(item.UId)) continue;
            if (!vueDict.ContainsKey(item.UId))
            {
vueDict.Add(item.UId, item);
            }
            else
            {
                Debug.LogError($"BindBimInfo1 vueDict.ContainsKey(item.UId) item:{item} UId:{item.UId}");
            }
            if(item.Name== "F级高厂变")
            {
                Debug.LogError("F级高厂变 Model");
            }
        }
        var bims = GameObject.FindObjectsOfType<BIMModelInfo>(true);
        for (int i = 0; i < bims.Length; i++)
        {
            BIMModelInfo bim = bims[i];
            bim.GetRenderId();
            if (bim.name == "F级高厂变")
            {
                Debug.LogError("F级高厂变 BIM");
            }
            ProgressArg p1 = new ProgressArg("BindBimInfo2", i, vueItems.Count, bim.name);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

            if (vueDict.ContainsKey(bim.Guid))
            {
                ModelItemInfo model = vueDict[bim.Guid];
                model.RenderId = bim.RenderId;
                model.RenderName = bim.name;
                DepNode dep = bim.GetComponentInParent<DepNode>();
                if (dep != null)
                {
                    var depName = dep.NodeName;
                    if (dep.ChildNodes!=null && dep.ChildNodes.Count>0)
                    {
                        //if (dep.ChildNodes[0] != null)
                        //{
                        //    depName = dep.ChildNodes[0].NodeName;
                        //}
                        //else
                        //{

                        //}

                        foreach(var child in dep.ChildNodes)
                        {
                            if (child == null) continue;
                            depName = child.NodeName;
                            break;
                        }
                    }
                    model.AreaName = depName;
                    //model.GetPath();

                    var subModels = model.GetAllChildren();
                    foreach(var sub in subModels)
                    {
                        sub.AreaName = depName;
                    }
                }
                else
                {
                    Debug.LogError($"BindBimInfo3 dep==null bim:{bim} rId:{bim.RenderId}");
                }
            }
            else
            {
                Debug.LogError($"BindBimInfo2 !vueDict.ContainsKey(bim.Guid) bim:{bim} guid:{bim.Guid}");
            }
            
        }


        Debug.LogError($"BindBimInfo vueDict:{vueDict.Count} bims:{bims.Length}");

        ProgressBarHelper.ClearProgressBar();

        SaveNavisFile();
    }

    [ContextMenu("匹配Vue属性树2")]
    public void CompareModelVueInfo_Vue2Model()
    {
        DateTime recordTime = DateTime.Now;

        InitNavisFileInfoByModelSetting setting = InitNavisFileInfoByModelSetting.Instance;

        navisFile = GetNavisFileInfo();
        vueAllModels = GetAllModelInfos(navisFile, true);
        var allTrans = GetAllModelTransform();

        vueAllModels = CompareModelVueInfo_Vue2Model(allTrans, vueAllModels, setting.MinDistance1, 1);
        //if (vueAllModels == null) return;

        if (vueAllModels!=null && setting.enableDistance2)
        {
            vueAllModels = CompareModelVueInfo_Vue2Model(allTrans, vueAllModels, setting.MinDistance2, 2);
            //if (vueAllModels == null) return;
        }

        if (vueAllModels != null && setting.enableDistance3)
        {
            vueAllModels = CompareModelVueInfo_Vue2Model(allTrans, vueAllModels, setting.MinDistance3, 3);
            //if (vueAllModels == null) return;
        }

        ProgressBarHelper.ClearProgressBar();

        SaveNavisFile();
        Debug.LogError($"CompareModelVueInfo_Vue2Model,CostTime:{(DateTime.Now - recordTime).TotalSeconds} enableDistance2:{InitNavisFileInfoByModelSetting.Instance.enableDistance2} enableDistance3:{InitNavisFileInfoByModelSetting.Instance.enableDistance3}");
    }

    public void SaveNavisFile()
    {
        SaveNavisFile(navisFile);
    }

    public void TestSave()
    {
        navisFile = GetNavisFileInfo();
        //SaveNavisFile();
        SaveNavisFile(navisFile);
    }

    public void SaveNavisFile(NavisFileInfo navisFiles)
    {
        if (navisFiles == null)
        {
            Debug.LogError("SaveNavisFile navisFiles == null");
            return;
        }
        string newModelInfo = "\\..\\NavisFileInfo.xml";
        string pathNew = Application.dataPath + newModelInfo;
        navisFiles.SetPropertiesExist(false);
        SerializeHelper.Save(navisFiles, pathNew);
        Debug.Log($"SaveNavisFile path:{pathNew}");
    }

    public bool IsSameName = true;

    public List<ModelItemInfo> CompareModelVueInfo_Vue2Model(List<Transform> allTrans, List<ModelItemInfo> models, float minDis, int id)
    {
        DateTime recordTime = DateTime.Now;
        totalCount = 0;
        findCount = 0;
        posFindCount = 0;
        //var allTrans = GetAllChildren();
        Debug.Log($"CompareModelVueInfo_Vue2Model allTrans:{allTrans.Count} models:{models.Count}");
        StringBuilder notFoundModels = new StringBuilder();
        List<ModelItemInfo> nodeFoundModelList = new List<ModelItemInfo>();
        for (int i = 0; i < models.Count; i++)
        {
            ModelItemInfo t = models[i];
            progressArg = new ProgressArg("CompareModelVueInfo_" + id, i, models.Count, t);
            if (ProgressBarHelper.DisplayCancelableProgressBar(progressArg))
            {
                return null;
            }
            if (t == null) continue;
            bool r = InitBIMModelInfoByPos_Vue2Model(t, allTrans, minDis, IsSameName);
            if (r == false)
            {
                nodeFoundModelList.Add(t);
            }
        }
        nodeFoundModelList.Sort((a, b) => a.Name.CompareTo(b.Name));

        foreach (var t in nodeFoundModelList)
        {
            notFoundModels.AppendLine(t.Name);
        }
        Debug.LogError($"FindComplete_{id},CostTime:{(DateTime.Now - recordTime).TotalSeconds} nameFindCount:{findCount} posFindCount:{posFindCount} TotalModelCount:{totalCount} TotalNavisInfoCount:{models.Count},notFoundModels:{nodeFoundModelList.Count}\n{notFoundModels}");
        
        ProgressBarHelper.ClearProgressBar();

        return nodeFoundModelList;
    }

    //public List<string> FilterNames1 = new List<string>() { "In", "Out0", "Out1", "LOD", "LODs" };

    //public List<string> FilterNames2 = new List<string>() { "_F1", "_F2", "_F3", "_F4", "_F5", "_F6" };

    //public List<string> FilterNames3 = new List<string>() { "合成部分" };

    //[ContextMenu("AddDoorABFilter")]
    //public void AddDoorABFilter()
    //{
    //    for (int i = 0; i < 20; i++)
    //    {
    //        if (!FilterNames2.Contains($"_Door{i + 1}A"))
    //        {
    //            FilterNames2.Add($"_Door{i + 1}A");
    //        }
    //        if (!FilterNames2.Contains($"_Door{i + 1}B"))
    //        {
    //            FilterNames2.Add($"_Door{i + 1}B");
    //        }
    //    }
    //}

    //public List<Transform> FilterList(List<Transform> list1, ProgressArgEx p0)
    //{
    //    List<Transform> all = new List<Transform>();
    //    for (int i1 = 0; i1 < list1.Count; i1++)
    //    {
    //        Transform t = list1[i1];
    //        var p1 = ProgressArg.New("FilterList", i1, list1.Count, t.name, p0);
    //        ProgressBarHelper.DisplayCancelableProgressBar(p1);
    //        if (InitNavisFileInfoByModelSetting.Instance.IsFiltered(t))
    //        {
    //            continue;
    //        }
    //        if(IsIncludeStructure && IsStructrue(t.name))
    //        {
    //            continue;
    //        }
    //        all.Add(t);
    //    }
    //    if(p0==null)
    //        ProgressBarHelper.ClearProgressBar();
    //    return all;
    //}

    //public bool IsIncludeStructure = true;//是否包括建筑结构

    //public List<string> structureNameList = new List<string>() { "MemberPartPrismatic", "PHC600AB", "Slab-", "WallPart-" };

    //public bool IsStructrue(string n)
    //{
    //    foreach (var key in structureNameList)
    //    {
    //        if (n.StartsWith(key))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool IsFiltered(Transform t)
    //{
    //    if(t.name== "HorPumpBB1Asm-1-0002")
    //    {
    //        Debug.LogError("HorPumpBB1Asm-1-0002");
    //    }
    //    if (InitNavisFileInfoByModelSetting.Instance.IsFiltered(t.name))
    //    {
    //        return true;
    //    }
    //    MeshRendererInfo info = t.GetComponent<MeshRendererInfo>();
    //    if (info != null)
    //    {
    //        //if (info.IsRendererType(MeshRendererType.LOD) && !info.IsLodN(0))
    //        //{
    //        //    return true;
    //        //}

    //        if (info.IsRendererType(MeshRendererType.LOD))
    //        {
    //            if (info.GetComponent<LODGroup>() == null) return true;
    //        }
    //    }
    //    if (t.childCount == 0 && t.GetComponent<MeshRenderer>() == null)
    //    {
    //        return true;
    //    }
    //    //if (t.GetComponent<MeshRenderer>() == null && t.GetComponent<LODGroup>() == null) return true;
    //    if (MeshHelper.IsEmptyGroup(t, false)) return true;
    //    //if (MeshHelper.IsSameNameGroup(t)) return true;
    //    if (MeshHelper.IsEmptyLODSubGroup(t)) return true;
    //    return false;
    //}



    //private bool IsFiltered(string n)
    //{
    //    if (FilterNames1.Contains(n))
    //    {
    //        return true;
    //    }
    //    foreach (var f in FilterNames2)
    //    {
    //        if (n.EndsWith(f))
    //        {
    //            return true;
    //        }
    //    }
    //    foreach (var f in FilterNames3)
    //    {
    //        if (n.Contains(f))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    private List<Transform> models = new List<Transform>();

    public List<Transform> GetModels()
    {
        return models;
    }

    [ContextMenu("GetAllModelTransform")]
    public List<Transform> GetAllModelTransform()
    {
        List<Transform> all = new List<Transform>();
        Debug.LogError($"GetAllModelTransform initInfoBuildings:{InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Count}");
        //return all;

        Dictionary<Transform, Transform> allDict = new Dictionary<Transform, Transform>();
        
        List<Transform> list1 = new List<Transform>();

        for (int i = 0; i < InitNavisFileInfoByModelSetting.Instance.initInfoBuildings.Count; i++)
        {
            GameObject item = InitNavisFileInfoByModelSetting.Instance.initInfoBuildings[i];
            if (item == null) continue;
            if (item.activeInHierarchy == false) continue;
            Transform[] ts = item.GetComponentsInChildren<Transform>(true);
            list1.AddRange(ts);
        }
        //for (int i1 = 0; i1 < list1.Count; i1++)
        //{
        //    Transform t = list1[i1];
        //    ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetAllModelTransform", i1, list1.Count, t.name));
        //    if (IsFiltered(t))
        //    {
        //        continue;
        //    }
        //    all.Add(t);
        //}

        all = InitNavisFileInfoByModelSetting.Instance.FilterList(list1,null);

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetAllChildren all:{all.Count}");

        models = all;
        return all;
    }

    [ContextMenu("GetItemCount")]
    public void GetItemCount()
    {
        //NavisFileInfo_Full.xml
        string fileName = "\\..\\NavisFileInfo_Full.xml";
        string path = Application.dataPath + fileName;
        var navisFiles = SerializeHelper.LoadFromFile<NavisFileInfo>(path);
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        if (navisFiles != null && navisFiles.Models != null)
        {
            models.AddRange(navisFiles.Models);
            foreach (var item in navisFiles.Models)
            {
                models.AddRange(GetChildItemInfo(item));
            }
        }
        Debug.Log("Total Count:" + models.Count);
    }

    //public void RenderIdCheck(List<ModelItemInfo> modelT)
    //{
    //    var result = modelT.FindAll(i=>!string.IsNullOrEmpty(i.RenderId));
    //    Debug.Log("包含RenderId节点数量："+result==null?"0":result.Count.ToString());
    //}
    private Dictionary<string, ModelItemInfo> ToModelDicByUid(List<ModelItemInfo> models)
    {
        Dictionary<string, ModelItemInfo> dicT = new Dictionary<string, ModelItemInfo>();
        foreach (var item in models)
        {
            if (!string.IsNullOrEmpty(item.UId) && !dicT.ContainsKey(item.UId))
            {
                dicT.Add(item.UId, item);
            }
        }
        return dicT;
    }

    private void FindCompareInfo_VUE(Transform parentT, List<ModelItemInfo> modelList)
    {
        if (parentT.childCount == 0) return;
        foreach (Transform child in parentT)
        {
            totalCount++;
            //if (modelDicT.ContainsKey(child.name))
            //{
            //    BIMModelInfo bimT = child.gameObject.AddMissingComponent<BIMModelInfo>();
            //    if (bimT)
            //    {
            //        findCount++;
            //        ModelItemInfo infoT = modelDicT[child.name];
            //        bimT.Guid = infoT.UId;
            //        bimT.Position1 = new Vector3(infoT.X, infoT.Z, infoT.Y);
            //        RendererId renderId = child.GetComponent<RendererId>();
            //        if (renderId)
            //        {
            //            bimT.RenderId = renderId.Id;
            //        }
            //    }
            //}
            //else
            //{
            InitBIMModelInfoByPos_Model2Vue(child, modelList, InitNavisFileInfoByModelSetting.Instance.MinDistance1);
            //}
            FindCompareInfo_VUE(child, modelList);
        }
    }

    public bool IsDestroyNoFoundBim = true;

    private bool InitBIMModelInfoByPos_Model2Vue(Transform child, List<ModelItemInfo> modelList, float minD)
    {
        BIMModelInfo bimT = child.gameObject.AddMissingComponent<BIMModelInfo>();
        ModelItemInfo infoT = TryGetModelInfoByPos_Model2Vue(modelList, child.gameObject, bimT, minD);
        bool isFound = bimT.Distance < minD;
        bimT.IsFound = isFound;

        if (isFound)
        {
            modelList.Remove(infoT);
        }

        if (isFound)
        {
            posFindCount++;
        }

        bimT.Guid = infoT.UId;
        bimT.Position1 = infoT.GetPositon();
        RendererId renderId = child.GetComponent<RendererId>();
        if (renderId)
        {
            bimT.RenderId = renderId.Id;
            infoT.RenderId = renderId.Id;
            infoT.RenderName = renderId.name;
        }

        DepNode dep = child.GetComponentInParent<DepNode>();
        if (dep && dep.TopoNode != null)
        {
            if (dep.TopoNode != null)
            {
                infoT.AreaName = dep.TopoNode.Name;
            }
            else
            {
                if (string.IsNullOrEmpty(dep.NodeName))
                {
                    dep.NodeName = dep.name;
                }
                infoT.AreaName = dep.NodeName;
            }
        }
        else
        {
            infoT.AreaName = "";
        }


        if (IsDestroyNoFoundBim && isFound == false)
        {
            if (child.GetComponent<BIMModelInfo>() != null)
            {
                child.gameObject.RemoveComponent<BIMModelInfo>();
            }
        }
        return isFound;
    }

    private Dictionary<GameObject, Vector3> go2Center = new Dictionary<GameObject, Vector3>();

    public Vector3 GetCenter(GameObject go)
    {
        if (!go.name.Contains("Door"))
        {
            return go.transform.position;
        }
        if (go2Center.ContainsKey(go))
        {
            return go2Center[go];
        }
        Vector3 c = MeshHelper.GetMinMax(go)[3];
        go2Center.Add(go, c);
        return c;
    }


    private ModelItemInfo TryGetModelInfoByPos_Model2Vue(List<ModelItemInfo> modelDicT, GameObject objT, BIMModelInfo bim, float minD)
    {
        float minDis = float.MaxValue;
        ModelItemInfo minModel = null;
        foreach (var item in modelDicT)
        {
            Vector3 posInfo = item.GetPositon();
            float dis = Vector3.Distance(posInfo, GetCenter(objT));

            if (dis < minDis)
            {
                minDis = dis;
                minModel = item;
            }
            if (dis < minD)
            {
                if (bim != null)
                {
                    bim.Distance = dis;
                    bim.SetModelInfo(item);
                    //bim.MName = item.Name;
                    //bim.MId = item.Id;
                }
                return item;
            }
        }
        if (bim != null)
        {
            bim.Distance = minDis;
            bim.SetModelInfo(minModel);
            //bim.Model = minModel;
            //bim.MName = minModel.Name;
            //bim.MId = minModel.Id;
        }
        return minModel;
    }

    public BIMModelInfo InitBIMModelInfoByPos_Vue2Model(ModelItemInfo child, List<Transform> modelList, float minD, bool isSameName)
    {
        BIMModelInfo bimT = TryGetModelInfoByPos_Vue2Model(modelList, child, minD, isSameName);
        if (bimT == null) return null;
        //bool isFound = bimT.Distance < minD;
        //bimT.IsFound = isFound;

        bool isFound = bimT.IsFound;

        if (isFound)
        {
            modelList.Remove(bimT.transform);
        }

        if (isFound)
        {
            posFindCount++;
        }

        bimT.Guid = child.UId;
        if (string.IsNullOrEmpty(child.UId))
        {
            bimT.Guid = child.Name;
        }

        bimT.Position1 = child.GetPositon();
        RendererId renderId = bimT.GetComponent<RendererId>();
        if (renderId)
        {
            bimT.RenderId = renderId.Id;
            child.RenderId = renderId.Id;
            child.RenderName = renderId.name;
        }

        DepNode dep = bimT.GetComponentInParent<DepNode>();
        if (dep)
        {
            if (dep.TopoNode != null)
            {
                child.AreaName = dep.TopoNode.Name;
            }
            else
            {
                if (string.IsNullOrEmpty(dep.NodeName))
                {
                    dep.NodeName = dep.name;
                }
                child.AreaName = dep.NodeName;
            }
        }
        else
        {
            child.AreaName = "";
        }

        if (IsDestroyNoFoundBim && isFound == false)
        {
            if (bimT.GetComponent<BIMModelInfo>() != null)
            {
                bimT.gameObject.RemoveComponent<BIMModelInfo>();
            }
        }
        return bimT;
    }

    private static BIMModelInfo AddBimModelInfo(Transform item, ModelItemInfo objT, float dis)
    {
        if (item == null)
        {
            Debug.LogError($"AddBimModelInfo item == null objT:{objT}");
            return null;
        }
        BIMModelInfo bim = item.gameObject.AddMissingComponent<BIMModelInfo>();
        bim.Distance = dis;
        bim.SetModelInfo(objT);
        //bim.Model = objT;
        //bim.MName = objT.Name;
        //bim.MId = objT.Id;
        //bim.IsFound = true;
        //bim.IsFound = bim.Distance < minD;
        return bim;
    }



    public bool IsProgressBreak = false;

    private BIMModelInfo TryGetModelInfoByPos_Vue2Model(List<Transform> modelDicT, ModelItemInfo objT, float minD, bool isSameName)
    {
        float minDis = float.MaxValue;
        Transform minModel = null;
        Vector3 posInfo = objT.GetPositon();

        if (isSameName)
        {
            {
                List<Transform> sameNameList = TransformHelper.FindSameNameList(modelDicT, objT.UId);
                if (sameNameList.Count == 1)
                {
                    Transform item = sameNameList[0];
                    float dis = Vector3.Distance(posInfo, item.position);
                    BIMModelInfo bim = AddBimModelInfo(item, objT, dis);
                    bim.IsFound = true;
                    return bim;
                }
                if (sameNameList.Count > 0)
                {
                    modelDicT = sameNameList;
                }
                //else
                //{
                //    return null;
                //}
            }

            {
                List<Transform> sameNameList = TransformHelper.FindSameNameList(modelDicT, objT.Name);
                if (sameNameList.Count == 1)
                {
                    Transform item = sameNameList[0];
                    float dis = Vector3.Distance(posInfo, item.position);
                    BIMModelInfo bim = AddBimModelInfo(item, objT, dis);
                    bim.IsFound = true;
                    return bim;
                }
                if (sameNameList.Count > 0)
                {
                    modelDicT = sameNameList;
                }
                else
                {
                    return null;
                }
            }
        }

        for (int i = 0; i < modelDicT.Count; i++)
        {
            Transform item = modelDicT[i];
            ProgressArg p3 = new ProgressArg("TryGetModelInfoByPos", i, modelDicT.Count, item.name);
            if (progressArg != null)
            {
                progressArg.AddSubProgress(p3);
                p3 = progressArg;
            }
            
            if (ProgressBarHelper.DisplayCancelableProgressBar(p3))
            {
                IsProgressBreak = true;
                ProgressBarHelper.ClearProgressBar();
                return null;
                //break;
            }
            float dis = Vector3.Distance(posInfo, item.position);

            if (dis < minDis)
            {
                minDis = dis;
                minModel = item;
            }
            if (dis < minD)
            {
                BIMModelInfo bim = AddBimModelInfo(item, objT, dis);
                bim.IsFound = bim.Distance < minD;
                //ProgressBarHelper.ClearProgressBar();
                return bim;
            }
        }

        if (minModel == null)
        {
            return null;
        }

        {
            BIMModelInfo bim = AddBimModelInfo(minModel, objT, minDis);
            bim.IsFound = bim.Distance < minD;
            //ProgressBarHelper.ClearProgressBar();
            return bim;
        }

        
    }
    #endregion

    public static void InitBimModel(PhysicalTopology root)
    {
        BimNodeHelper_PhysicalTopology.IsDebug = InitNavisFileInfoByModel.Instance.IsDebug;
        BimNodeHelper_PhysicalTopology.InitBimModel(root);
    }

    public static ModelItemInfo GetModel(PhysicalTopology node)
    {
        return BimNodeHelper_PhysicalTopology.GetModel(node);
    }
    public static ModelItemInfo GetModel(DevInfo node)
    {
        return BimNodeHelper_PhysicalTopology.GetModel(node);
    }


    public static void InitBimModel(AreaNode root)
    {
        BimNodeHelper_AreaNode.InitBimModel(root);
    }

    public static ModelItemInfo GetModel(AreaNode node)
    {
        return BimNodeHelper_AreaNode.GetModel(node);
    }
}



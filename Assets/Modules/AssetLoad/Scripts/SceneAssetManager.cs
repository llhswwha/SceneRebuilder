using System;
using System.Collections.Generic;
using System.Linq;
using Jacovone.AssetBundleMagic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;

public class SceneAssetManager : MonoBehaviour {

    public GameObject ParkObject;

    public static SceneAssetManager Instance;

    public SceneAssetManagerWindow Window;

    public Transform subject;

    public bool isRoam = false;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 显示顶点数
    /// </summary>
    [ContextMenu("ShowVertexs")]
    public int ShowVertexs()
    {
        if (FactoryDepManager.Instance == null)
        {
            Debug.LogError("SceneAssetManager.ShowVertexs FactoryDepManager.Instance == null");
            return 0;
        }
        if (ParkObject == null)
        {
            ParkObject = FactoryDepManager.Instance.gameObject;
        }
        int Vertexs = 0;
        MeshFilter[] mrs = ParkObject.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (mrs[i] == null) continue;
            if (mrs[i].sharedMesh == null) continue;
            Vertexs += mrs[i].sharedMesh.vertexCount;
        }
        //Debug.Log("Vertexs:" + Vertexs);

        SceneVertexs = Vertexs;
        Window.RefreshInfo();
        return Vertexs;
    }

    public List<AssetBundleInfo> loadBoxList = new List<AssetBundleInfo>();

    public List<AssetBundleInfo> unloadBoxList = new List<AssetBundleInfo>();

    public List<AssetBundleInfo> notUnloadBoxList = new List<AssetBundleInfo>();//一直不用卸载的模型

    public void AfterLoadAsset(AssetBundleInfo box)
    {
        loadBoxList.Add(box);
        SetCount();
        ShowVertexs();
    }

    private void SetCount()
    {
        BuildingCount = 0;
        DeviceSetCount = 0;
        foreach (var item in loadBoxList)
        {
            if(item is BuildingBox)
            {
                if (item.IsNotUnload) continue;//不算入
                BuildingCount++;
            }
            else if(item is DeviceAssetInfo)
            {
                DeviceSetCount++;//设备Asset不计算入缓存，和建筑一起卸载的。
            }
        }
    }

    private string logs;

    private void WriteLog(string log)
    {
        //#if UNITY_EDITOR
        if (subject == null)
        {
            subject = Camera.main.transform;
        }
        logs += string.Format("{0}|{1}\n",log,subject.transform.position);
//#endif
        Debug.Log(log);
    }

    [ContextMenu("ShowLog")]
    private void ShowLog()
    {
        logs += "-------------------------\n";
        Debug.Log(logs);
    }

    void OnDestroy()
    {
        ShowLog();
    }

    [ContextMenu("TestJ6J11")]
    private void TestJ6J11()
    {
        /*
         * 
排队卸载模型 :buildings_J6J11
加载模型 :buildings_S3
排队卸载模型 :buildings_S3
实际卸载模型 :buildings_S3
排队卸载模型 :buildings_J6J11
加载模型 :buildings_S3
加载模型 :buildings_H4
实际卸载模型 :buildings_J6J11
加载模型 :buildings_J6J11
排队卸载模型 :buildings_S3
实际卸载模型 :buildings_S3
排队卸载模型 :buildings_H4
实际卸载模型 :buildings_H4
         */
        var b1 = GetBuildingBox("buildings_J6J11");
        InBuildings.Add(b1);
        if (b1.IsLoaded)//已加载
        {
            if (b1.IsLoaded)
            {
                var b2=b1.UnloadAsset("卸载");//卸载
                //b2.Tag = "第一次加载的";
                b1.LoadBuilding((node)=>
                {
                    if (node != null)
                    {
                        node.Tag = "第二次加载";
                    }
                });//加载
            }
        }
        else
        {
            b1.LoadBuilding((node) =>//加载
            {
                if (b1.IsLoaded)
                {
                    b1.UnloadAsset("第一次加载的");//卸载
                    b1.LoadBuilding();//加载
                }
            });
        }

    }

    private BuildingBox GetBuildingBox(string assetName)
    {
        return buildingBox.ToList().Find(i => i.AssetName == assetName);
    }

    public void BeforeUnloadAsset(AssetBundleInfo box,bool isImmediate)
    {
        if (isImmediate)//设备模型马上卸载
        {
            box.UnloadAsset("");//卸载模型
            ShowVertexs();
            loadBoxList.Remove(box);
            SetCount();
        }
        else
        {
            //if (unloadBoxList.Contains(box))
            //{
            //    unloadBoxList.Remove(box);
            //}
            //unloadBoxList.Add(box);


            if (box.IsNotUnload)
            {
                if (!notUnloadBoxList.Contains(box))
                {
                    notUnloadBoxList.Add(box);
                }
            }
            else
            {

                if (!unloadBoxList.Contains(box))
                {
                    unloadBoxList.Add(box);
                    WriteLog("排队卸载模型 :" + box.AssetName);
                }
                else
                {

                }
            }
        }
    }

    private void ScanEmptyItem()
    {
        for (int i = 0; i < loadBoxList.Count; i++)
        {
            var item = loadBoxList[i];
            if (item == null)
            {
                loadBoxList.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < unloadBoxList.Count; i++)
        {
            var item = unloadBoxList[i];
            if (item == null)
            {
                unloadBoxList.RemoveAt(i);
                i--;
            }
        }
    }

    public AssetBundleInfo GetUnloadOne()
    {
        foreach (var item in unloadBoxList)
        {
            Log.Info("GetUnloadOne:"+item.AssetName+","+ item.SceneName);
        }
        if (unloadBoxList.Count == 0) return null;
        //return unloadBoxList.First();
        List<AssetBundleInfo> noJ1 = unloadBoxList.Where(i => i.SceneName != "J1").ToList();//主厂房能不卸载尽量不要卸载
        if (noJ1.Count > 0)
        {
            return noJ1[0];
        }
        return unloadBoxList.First();
    }

    [ContextMenu("RemoveAsset")]
    private void UnloadOneAsset()
    {
        bool EnableUnload = SystemSettingHelper.assetLoadSetting.EnableUnloadFunction;
        if(!EnableUnload)
        {
            return;
        }
        if (unloadBoxList.Count > 0)
        {
            AssetBundleInfo first = GetUnloadOne();
            if (first != null)
            {
                unloadBoxList.Remove(first);
                loadBoxList.Remove(first);

                first.UnloadAsset("");//卸载模型
                WriteLog("实际卸载模型 :" + first.AssetName);
                SetCount();

                //ScanEmptyItem();//主厂房卸载后，里面的1楼2楼的设备也一起卸载的，所以，这里要把1楼2楼的设备的AssetBundleInfo也去掉
                ShowVertexs();
            }
        }
    }

    void Update()
    {
        if (ShouldUnload())
        {
            UnloadOneAsset();
        }

        if (isEnableLoad)
        {
            LoadBuildingByDistance();
            LoadDeviceByDistance();//根据距离动态加载主厂房内设备的精细模型
        }

        GetLoadedBundles();

        //AssetBundleMagic.LoadFileAndSceneProgress.bundlePower = bundlePower;
    }

    //private void Upadte

    [ContextMenu("GetLoadedBundles")]
    private void GetLoadedBundles()
    {
        LoadedBundles.Clear();
        foreach (var item in AssetBundleMagic.Bundles.Keys)
        {
            LoadedBundles.Add(item);
        }
    }

    [ContextMenu("ClearBundles")]
    private void ClearBundles()
    {
        List<string> tmp = new List<string>();
        tmp.AddRange(LoadedBundles);
        foreach (var item in tmp)
        {
            AssetBundleMagic.UnloadBundleEx(item);
        }
    }

    public List<string> LoadedBundles = new List<string>();

    public List<DeviceModelInfo> InDevices = new List<DeviceModelInfo>();

    public List<DeviceModelInfo> LoadedDevices = new List<DeviceModelInfo>();

    public bool EnableLoadDevices = false;

    private void LoadDeviceByDistance()
    {
        if (PersonSubsystemManage.Instance!=null && PersonSubsystemManage.Instance.IsHistorical) return;//处理历史轨迹模式下不用加载精细模型
        if (EnableLoadDevices == false) return;
        //DeviceModelInfo[] deviceModels = GameObject.FindObjectsOfType<DeviceModelInfo>();
        var deviceModels = DeviceModelInfo.Buffer;
        InDevices.Clear();
        foreach (var item in deviceModels)
        {
            if (item.IsInDistance(subject, isRoam))
            {
                if (IsInViewEx(item) == false) continue;//不在视野中
                InDevices.Add(item);
                item.LoadAsset(()=>
                {
                    LoadedDevices.Add(item);
                    ShowVertexs();
                });
            }
        }

        if (LoadedDevices.Count > DeviceCacheCount)
        {
            UnLoadDevicesWhenNotInDistance();
        }
        else
        {
            UnLoadDevicesWhenOutDistance();
        }
    }

    private void UnLoadDevicesWhenOutDistance()
    {
        for (int i = 0; i < LoadedDevices.Count; i++)
        {
            try
            {
                DeviceModelInfo item = LoadedDevices[i];
                if (item.IsOutDistance(subject, isRoam))
                {
                    item.UnloadAsset();
                    LoadedDevices.RemoveAt(i);
                    i--;
                    ShowVertexs();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("SceneAssetManager.UnLoadDevicesWhenOutDistance:"+ex);
            }
        }
    }

    private void UnLoadDevicesWhenNotInDistance()
    {
        for (int i = 0; i < LoadedDevices.Count; i++)
        {
            try
            {
                DeviceModelInfo item = LoadedDevices[i];
                if (!item.IsInDistance(subject, isRoam))
                {
                    item.UnloadAsset();
                    LoadedDevices.RemoveAt(i);
                    i--;
                    ShowVertexs();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("SceneAssetManager.UnLoadDevicesWhenNotInDistance:" + ex);
            }
            
        }
    }

    public bool IsInView(Vector3 worldPos, Camera camera, float min = 0, float max = 1)
    {
        Transform camTransform = camera.transform;
        Vector2 viewPos = camera.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= min && viewPos.x <= max && viewPos.y >= min && viewPos.y <= max)
            return true;
        else
            return false;
    }



    public float minView = 0;
    public float maxView = 1;

    public List<BuildingBox> inViewBoxList = new List<BuildingBox>();
    public List<BuildingBox> notInViewBoxList = new List<BuildingBox>();

    public List<BuildingBox> hitBoxList = new List<BuildingBox>();

    public List<Transform> hitObjects = new List<Transform>();

    private void CheckIsInViewSimple()//判断建筑是否在视野内
    {
        inViewBoxList.Clear();
        notInViewBoxList.Clear();
        foreach (BuildingBox item in buildingBox)
        {
            if (item.transform == null) continue;
            bool inView = IsInView(item);//todo:这里是用中心判断的，假如中心没有在视野内，就算大部分在视野内也不行。
            //bool inView = IsInViewEx(item);//可能有性能问题
            if (inView)
            {
                inViewBoxList.Add(item);
            }
            else
            {
                notInViewBoxList.Add(item);
            }
        }

        //GetHitInfos();
    }

    [ContextMenu("GetHitInfos")]
    private void GetHitInfos()
    {
        hitBoxList.Clear();
        hitObjects.Clear();
        var hitInfos = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);
        foreach (var item in hitInfos)
        {
            BuildingBox box = item.transform.GetComponent<BuildingBox>();
            //if (!hitBoxList.Contains(box))
            //{
            //    hitBoxList.Add(box);
            //}
            if (box != null)
            {
                hitBoxList.Add(box);
            }
            hitObjects.Add(item.transform);
        }
    }

    private bool IsInViewEx(BuildingBox item)
    {
        if (item == null) return false;

        bool inView = false;
        var points=item.GetTestPoints();
        foreach (var point in points)
        {
            var r= IsInView(point, Camera.main, minView, maxView);
            if (r)
            {
                inView = true;
                break;
            }
        }
        return inView;
    }

    private bool IsInViewEx(DeviceModelInfo item)
    {
if (item == null) return false;

        bool inView = false;
        var points=item.GetTestPoints();
        foreach (var point in points)
        {
            var r= IsInView(point, Camera.main, minView, maxView);
            if (r)
            {
                inView = true;
                break;
            }
        }
        return inView;
    }

    public bool AllUseIsInViewEx = false;

    private bool IsInView(BuildingBox item)
    {
        if (item == null) return false;
        if (AllUseIsInViewEx)//测试用
        {
            return IsInViewEx(item);
        }

        bool inView = IsInView(item.transform.position, Camera.main, minView, maxView);
        return inView;
    }

    private void ShowViewInfo(BuildingBox item)
    {
        Vector3 worldPos = item.transform.position;
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
        Debug.Log("ShowViewInfo:" + dot + "|" + viewPos+"|"+item.AssetName);
    }

    public bool OnlyLoadInView = true;

    /// <summary>
    /// 获取判断距离的参考物体
    /// </summary>
    /// <returns></returns>
    private bool GetSubject()
    {
        if (subject == null)
        {
            subject = Camera.main.transform;
        }

        if (subject == Camera.main.transform)
        {
            if (subject.transform.position.y == 0)
            {
                return false;//跳过首页进入时，摄像机的位置还没设置起来
            }
        }
        return true;
    }

    /// <summary>
    /// 判断当前是否应该加载模型
    /// </summary>
    private bool GetIsInPark()
    {
        bool isInPark = false;
        if (FactoryDepManager.currentDep == null)
        {
            Debug.LogError("SceneAssetManager.LoadBuildingByDistance FactoryDepManager.currentDep == null");
            isInPark = false;
        }
        else if (FactoryDepManager.currentDep.TopoNode == null)
        {
            Debug.LogError("SceneAssetManager.LoadBuildingByDistance FactoryDepManager.currentDep.TopoNode == null :" + FactoryDepManager.currentDep.NodeName);//一般意味着初始化有问题
            isInPark = false;
            //#if UNITY_EDITOR
            //            this.enabled = false;//不继续执行
            //#endif
        }
        else
        {
            isInPark = FactoryDepManager.currentDep.TopoNode.Type == AreaTypes.园区;//对焦到某一个园区、楼层状态下，不加载模型。
        }
        return isInPark;
    }

    private void LoadBuildingByDistance()
    {
        //Log.Info("LoadBuildingByDistance");
        if (GetSubject() == false) return;//获取判断距离的参考物体
        DoLoad = GetIsInPark();//判断当前是否应该加载模型
        CheckIsInViewSimple();//获取在视图中的建筑模型
        if (DoLoad == false)
        {
            //Debug.Log("DoLoad == false");
        }
        
        if (PersonSubsystemManage.Instance != null && PersonSubsystemManage.Instance.IsHistorical)
        {
            if (LocationHistoryManager.Instance.IsFocus)//历史轨迹的对焦模式和漫游模式加载行为类似
            {
                isRoam = true;//漫游和非漫游判断距离的方式、参数是不一样的
            }
            else
            {
                //不对焦的透明状态，不用加载详细模型
                return;
            }
        }

        InBuildings.Clear();
        foreach (BuildingBox item in buildingBox)
        {
            if (!isRoam)
            {
                if (!item.gameObject.activeInHierarchy && item.IsLoaded == false)
                    continue;
                //非漫游是，还没加载模型并且隐藏了的不管，对应双击拉近模型时的问题。
            }
            
            if (item.IsInDistance(subject,isRoam))//漫游和非漫游判断距离的方式、参数是不一样的
            {
                InBuildings.Add(item);
                if (DoLoad==false) continue;//测试模型不加载模型


                if (isRoam  //漫游模式
                //历史轨迹模式
                    )
                {
                    if (J1&&J1.IsInBounds(subject))//2019_05_23_cww:特殊处理在主厂房内漫游时不加载外部的建筑。
                    {
                        if(J1_F2 && J1_F2.IsInBounds(subject)) //在二楼，要去集控楼 所以
                        {
                            if (item.SceneName == "J4")//要去集控楼
                            {
                                //加载集控楼建筑
                            }
                            else
                            {
                                continue;//不加载外面建筑
                            }
                        }
                        else
                        {
                            continue;//不加载外面建筑
                        }
                    }
                }

                //if (isRoam)
                {
                    if (item.IsLoaded)//2019_05_15_cww:处理漫游进入主厂房后，走着走着加载主厂房外面的建筑导致把主厂房卸载了的问题。
                    {
                        if (unloadBoxList.Contains(item))
                        {
                            unloadBoxList.Remove(item);
                        }
                    }
                    else
                    {
                        if (OnlyLoadInView)//2019_05_21_cww:只加载在视野中的建筑 处理因为摄像头自动控制距离导致的问题
                        {
                            if (item.IsInBounds(subject))//靠的太近时无法用物体边缘的点判断是否在视野内
                            {

                            }
                            else if(!IsInViewEx(item))//建筑不在视野中
                            {
                                continue;//不加载模型，处理对接一个人员时，摄像头拉远，后面的建筑被加载；导致前面的建筑被卸载，刚好前面的建筑又正好有点远。
                            }
                        }
                    }
                }


                bool isBusyOrLoad = !item.LoadBuilding((nNode) =>
                {
                    if (DevSubsystemManage.Instance)
                    {
                        if (nNode != null)
                        {
                            DevSubsystemManage.Instance.EnlargeBuildingDoorCollider(nNode.gameObject);//调大门的碰撞体
                        }
                        else
                        {
                            //Debug.LogError("LoadBuilding.LoadBuildingInRoam nNode ==null");
                        }
                    }
                }, false);//漫游进入建筑
                if (isBusyOrLoad)
                {
                    //Debug.Log(" isBusyOrLoad :"+item);
                }
                else
                {
                    WriteLog("加载模型 :" + item.AssetName);
                }
            }
            else
            {
                if (DoLoad == false) continue;
                if (item.IsLoaded)
                {
                    item.SetUnload(false);
                    
                }
            }
        }
        //EnableRoamLoad = false;
    }

    public bool DoLoad = false;

    private bool isEnableLoadBack = false;

    public bool isEnableLoad = false;

    public void SetEnableLoadBuilding(bool enable)
    {
        isEnableLoad = enable;
    }

    private int saveCount = 0;

    public void SaveEnableLoadBuilding(bool enable)
    {
        Debug.LogError("SaveEnableLoadBuilding:"+ enable+","+ saveCount);
        if (saveCount==0)
        {
            isEnableLoadBack = isEnableLoad;
            isEnableLoad = enable;
        }
        saveCount++;
        if (saveCount > 1)//摄像头多次连续变化，在定位楼层设备时，会先进入楼层，在对焦设备 。
        {
            Debug.LogError("多次进入 SaveEnableLoadBuilding");
        }
    }

    public void RecoverEnableLoadBuilding()
    {
        Debug.LogError("RecoverEnableLoadBuilding:" +saveCount);
        //再不行，这里直接设置saveCount为0
        saveCount--;
        if (saveCount == 0)
        {
            isEnableLoad = isEnableLoadBack;
        }
        else
        {
            bool isInPark=GetIsInPark();//2019_05_24_cww:处理问题，两次快速点击返回上一层会导致 SaveEnableLoadBuilding两次 RecoverEnableLoadBuilding一次 然后就无法加载模型了
            if (isInPark)
            {
                isEnableLoad = isInPark;
                saveCount = 0;
            }
        }
    }

    public List<BuildingBox> InBuildings = new List<BuildingBox>();

    public bool ShouldUnload()
    {
        //if (Setting == null) return false;
        if (UnloadMode == 0)
        {
            return BuildingCount > CacheCount;
        }
        else if (UnloadMode == 0)
        {
            return SceneVertexs > MaxVertex;
        }
        else
        {
            return false;
        }
    }


    public BuildingBox[] buildingBox;

    public BuildingBox J1;

    public FloorController J1_F2;

    void Start()
    {
        //Setting = SystemSettingHelper.assetLoadSetting;
        ShowVertexs();

        buildingBox = GameObject.FindObjectsOfType<BuildingBox>();

        if(J1==null)
            J1 = buildingBox.ToList().Find(i => i.SceneName == "J1");//J1特殊处理
        if (J1)
        {
            var building = J1.GetComponent<BuildingController>();
            J1_F2 = building.ChildNodes[1] as FloorController;
        }
    }

    void OnGUI()
    {
        //GUI.Label(new Rect(30, 30, 100, 200), "Vertexs:"+ SceneVertexs);
    }

    internal void SetSetting(AssetLoadSetting setting)
    {
        Debug.Log("SceneAssetManager.SetSetting");
        UnloadMode = setting.UnloadMode;
        CacheCount = setting.CacheCount;
        DeviceCacheCount = setting.DeviceCacheCount;
        MaxVertex = setting.MaxVertex;
        LoadFromFile = setting.BuildingFromFile;
        HttpUrl = setting.HttpUrl;

        Window.LoadSetting();
    }

    public int BuildingCount = 0;

    /// <summary>
    /// 主厂房1楼或2楼全部设备
    /// </summary>
    public int DeviceSetCount = 0;

    /// <summary>
    /// 主厂房单独设备模型
    /// </summary>
    public int SimgleDeviceCount
    {
        get
        {
            return LoadedDevices.Count;
        }
    }


    public int DeviceCacheCount = 0;

    public int SceneVertexs = 0;
    /// <summary>
    /// 卸载策略 0:按数量 
    /// todo:1:按顶点数 2:按时间 3:按频率
    /// </summary>
    public int UnloadMode = 0;
    /// <summary>
    /// 缓存数量，卸载资源
    /// </summary>
    public int CacheCount = 0;
    //0:不缓存，加载一个，卸载一个。
    //1:加载A，加载B，卸载A，加载C，卸载B
    //设置为1000也就是只加载，不卸载
    /// <summary>
    /// 最大顶点数量
    /// </summary>
    public int MaxVertex = int.MaxValue;

    public bool LoadFromFile = true;//false的话是从服务端加载模型

    public string HttpUrl = "";

    internal void SetChunkManager(ChunkManager cm)
    {
        //if (Setting == null) return;
        //AssetBundleMagic.Instance.BundlesBaseUrl = "http://192.168.1.16:8000/StreamingAssets";
        AssetBundleMagic.Instance.BundlesBaseUrl = CommunicationObject.currentIp + ":8000/StreamingAssets";
        HttpUrl = AssetBundleMagic.Instance.BundlesBaseUrl;
        Window.LoadSetting();

        if (LoadFromFile == false)//false,从服务端获取
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
}

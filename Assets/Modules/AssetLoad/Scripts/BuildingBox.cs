using Jacovone.AssetBundleMagic;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingBox : AssetBundleInfo
{

    // Use this for initialization
    void Start()
    {
        roomFactory = RoomFactory.Instance;
        mapManager = MapLoadManage.Instance;

        InitDistanceBounds();

        //ShowTestPoints();//测试用

        if(SceneName== "S1")
        {
            IsNotUnload = true;//2019_05_23_cww:特殊处理，因为一直要加载卸载S1，它本身比较大
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        Debug.Log("BuildingBox.Hide:" + this);
        //this.gameObject.SetActive(false);

        if (IsPart)
        {
            this.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        Debug.Log("BuildingBox.Show:" + this);
        //this.gameObject.SetActive(true);

        if (IsPart)
        {
            this.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }

    public SceneAssetInfo info;

    //public string SceneName;

    //public string AssetName;

    DepNode parentDepNode;

    public BuildingController buildingBoxController;//假的只有轮廓的模型

    public BuildingController buildingController;//原来的模型（加载的精细模型）

    [System.NonSerialized]
    PhysicalTopology topoNode;

    RoomFactory roomFactory;

    MapLoadManage mapManager;

    public DepNode focusNode;

    public bool LoadBuilding(Action<DepNode> callback =null,bool isFocus = true, DepNode focusNode=null)
    {
        //Debug.Log("BuildingBox.LoadBuilding:"+this);
        this.focusNode = focusNode;
        //Debug.Log("BuildingBox.LoadBuilding:" + this);
        InitBuildingController();
        return base.LoadAsset(callback, isFocus);
    }

    protected override DepNode LoadRootObject(GameObject obj,bool isFocus=true)
    {
        InitBuildingController();
        return LoadBuildingObject(obj, isFocus);
    }

    protected override void AfterLoad()
    {
        Hide();
        IsLoading = false;
    }

    private void InitBuildingController()
    {
        if (buildingBoxController == null)
        {
            buildingBoxController = gameObject.GetComponent<BuildingController>();
            
            if (buildingBoxController == null)
            {
                Debug.LogError("Name:" + gameObject.name + "，没加BuildingController.cs脚本");
                return;
            }
            buildingBoxController.IsSimple = true;
            topoNode = buildingBoxController.TopoNode;
            buildingBoxController.buildingBox = this;
            parentDepNode = buildingBoxController.ParentNode;
        }
    }

    public override BuildingController UnloadAsset(string tag)//这个由SceneAssetManager调用
    {        
        if (IsLoaded == false)
        {
            //Debug.Log("BuildingBox.UnloadAsset IsLoaded == false:"+this);
            return buildingController;
        }
        IsLoaded = false;
        Debug.Log("BuildingBox.UnloadAsset:" + this);
        if (buildingController == null)
        {
            Show();//显示简化模型的建筑
            ReplaceDepNode(buildingBoxController, tag);
            return buildingController;
        }

        UnloadDevicesAsset();//卸载建筑前先卸载设备
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (collider)
        {
            collider.enabled = true;
        }
        ShowObject();//显示简化模型的建筑
        ReplaceDepNodeWhenUnLoad(tag);//替换缓存节点

        buildingController.SetIsUnload();
        MoveLocationObjects();///移动人员到相应的原来的建筑结点中，因为LocationObject中也在刷新，这个可能没什么意义
        DestroyGameObject();

        UnloadOtherBox();//卸载其他相关模型

        return buildingController;
    }

    private void UnloadOtherBox()
    {
        if (IsPart)
        {
            var boxList = this.transform.parent.GetComponentsInChildren<BuildingBox>();
            foreach (var item in boxList)
            {
                if (item != this)
                {
                    item.UnloadAsset("");//J6J11，J6加载的话，J11也要替换
                }
            }
        }
    }

    private void MoveLocationObjects()
    {
        //这个放到ReplaceDepNode后面，不然LocationObject还会找回到要被删除的节点
        //移动人员到相应的原来的建筑结点中，因为LocationObject中也在刷新，这个可能没什么意义。
        Debug.Log("BuildingBox.UnloadAsset movePersons start");
        var persons = buildingController.GetComponentsInChildren<LocationObject>();
        foreach (var person in persons)
        {
            var personParent = person.transform.parent;
            var newParent = buildingBoxController.transform.GetChildByName(personParent.name);
            person.transform.parent = newParent;
        }
        Debug.Log("BuildingBox.UnloadAsset movePersons end");
    }

    private void ShowObject()
    {
        bool isActivie = buildingController.gameObject.activeInHierarchy;
        if (isActivie)//双击拉近一个其他建筑，加载其他建筑的简化模型时，当前建筑应该是隐藏的，不应该另外特意把简化模型显示出来。
        {
            buildingController.gameObject.SetActive(false);//隐藏原来的模型
            Show();//显示简化模型的建筑
        }
        else
        {
            Debug.LogError("BuildingBox.UnloadAsset isActivie==false");
        }
    }

    private void ReplaceDepNodeWhenUnLoad(string tag)
    {
        parentDepNode.ChildNodes.Remove(buildingController);
        parentDepNode.ChildNodes.Add(buildingBoxController);
        UnloadInit(buildingBoxController, buildingController);

        ReplaceDepNode(buildingBoxController, tag);
    }

    private void DestroyGameObject()
    {
        var obj = buildingController.gameObject;
        if (IsPart)
        {
            obj= obj.transform.parent.gameObject;
        }

        //放到最后删除掉
        //GameObject.DestroyImmediate(buildingController.gameObject);//放到最后删除掉
        //GameObject.Destroy(buildingController.gameObject);//有问题,建筑内的人员会隐藏掉，看不到，但是实际上没有被删除
        GameObject.Destroy(obj, 2f);//加了延迟，人员就正常显示了。看了一下，不显示可能和LocationObject中的locationAreas有关，需要等一下，等移动到新的建筑里重新添加了locationAreas吧。

        //AssetBundleMagic.UnloadBundle(AssetName, true);//不能用false，要卸载掉asset
        //在卸载掉的BuildingController中移除Asset

        AssetBundleMagic.UnloadBundleEx(AssetName);
    }


    public bool IsPart = false;//D1-D4,只是一部分，J6J11

#if UNITY_EDITOR
    private List<BuildingController> history = new List<BuildingController>();
#endif

    private void SetRootParent(GameObject rootObj)
    {
        if (IsPart)
        {
            rootObj.transform.parent = this.transform.parent.parent;
        }
        else
        {
            rootObj.transform.parent = this.transform.parent;
        }
    }

    private DepNode LoadBuildingObject(GameObject rootObj,bool isFocus=true)
    {
        SetRootParent(rootObj);//设置加载的物体的父物体

        //Debug.Log("BuildingBox.LoadBuilding: " + topoNode);
        ReplaceDepNodeWhenLoad(rootObj);//替换缓存中的节点对象
        ReplaceOtherDepNode(rootObj);//J6J11，J6加载的话，J11也要替换

        DoorAccessModelAdd.InitDoorControl(rootObj.transform);//这个要放到callback前面，因为callback里面有漫游时设置门碰撞体的部分。

        DepNode node = FocusNode(isFocus);
        return node;
    }

    private void ReplaceOtherDepNode(GameObject rootObj)
    {
        if (IsPart)
        {
            var boxList = this.transform.parent.GetComponentsInChildren<BuildingBox>();
            foreach (var item in boxList)
            {
                if (item != this)
                {
                    item.InitBuildingController();
                    item.ReplaceDepNodeWhenLoad(rootObj);//J6J11，J6加载的话，J11也要替换
                }
            }
        }
    }

    private void ReplaceDepNodeWhenLoad(GameObject rootObj)
    {
        buildingController = InitBuildingController(rootObj);
        if (parentDepNode != null)
        {
            if (parentDepNode.ChildNodes.Contains(buildingBoxController)) parentDepNode.ChildNodes.Remove(buildingBoxController);
            if (!parentDepNode.ChildNodes.Contains(buildingController)) parentDepNode.ChildNodes.Add(buildingController);
        }
        else
        {
            Debug.LogError("BuildingBox.ReplaceDepNodeWhenLoad:"+this);
        }
        
        ReplaceDepNode(buildingController, "");
        LoadInit(buildingBoxController, buildingController);
    }

    private DepNode FocusNode(bool isFocus = true)
    {
        DepNode node = buildingController.FindRelationNode(focusNode);
        if (isFocus && callback == null)
        {
            RoomFactory.Instance.FocusNode(node);
        }
        else
        {
            if (callback != null)
            {
                callback(node);//外部的回掉函数（一般是聚焦建筑）
            }
        }
        return node;
    }

    private BuildingController InitBuildingController(GameObject rootObj)
    {
        Transform controllerObject = rootObj.transform;
        BuildingController controller = null;
        //if (buildingController == null)//2019_05_22_cww:这个不需要，不然快速卸载->加载时，后续的的物体无法替换进去。正常操作的话倒是不影响。
        {
            controller = controllerObject.GetComponent<BuildingController>();
            if (controller == null)//D1-D4，物体下面有子物体
            {
                controllerObject = controllerObject.FindChildByName(gameObject.name);
                if (controllerObject == null)
                {
                    Debug.LogError("controllerObject == null:" + gameObject.name);
                }
                else
                {
                    controller = controllerObject.GetComponent<BuildingController>();
                }
            }
            if (controller != null)
            {
                BuildingBox boxInAsset = controller.GetComponent<BuildingBox>();
                if (boxInAsset)
                {
                    GameObject.DestroyImmediate(boxInAsset);//有时候不小心把BuildingBox也打包进去了。
                    Debug.LogWarning(" Have BuildingBox In AssetBundle !!!!");
                }

                controller.buildingBox = this;
                controller.ParentNode = parentDepNode;
                controller.AfterOpenFloor += BuildingController_AfterOpenFloor;//展开楼层时加载设备
                controller.SetIsLoaded();
#if UNITY_EDITOR
                if(!history.Contains(controller)) history.Add(controller);
#endif
            }
            else
            {
                Debug.LogError("buildingController == null :" + controllerObject);
            }
        }
        //else{

        //}

        return controller;
    }

    private void BuildingController_AfterOpenFloor(BuildingController obj)
    {
        LoadDevicesAsset();//展开楼层时加载设备
    }

    /// <summary>
    /// 加载时初始化
    /// </summary>
    public void LoadInit(DepNode buildingBoxControllerT, DepNode buildingControllerT)
    {
        LoadInit_Area(buildingBoxControllerT, buildingControllerT);
    }

    /// <summary>
    /// 加载时区域初始化设置
    /// </summary>
    public void LoadInit_Area(DepNode simpledepnode, DepNode complexdepnode)
    {
        if (simpledepnode == null || complexdepnode == null) return;
        complexdepnode.monitorRangeObject = simpledepnode.monitorRangeObject;
        if (complexdepnode.monitorRangeObject != null)
        {
            complexdepnode.monitorRangeObject.SetFollowTarget(complexdepnode.NodeObject);
        }
        //Transform ranges1 = simpledepnode.transform.Find("Ranges");
        ////Transform ranges2 = complexdepnode.transform.Find("Ranges");

        if (simpledepnode)
        {
            int childcount = simpledepnode.transform.childCount;
            for (int i = childcount - 1; i >= 0; i--)
            {
                Transform t = simpledepnode.transform.GetChild(i);
                if (t.tag == "Person")
                {
                    t.SetParent(complexdepnode.transform);
                    //if (complexdepnode.transform.name == "J1_F1")
                    //{
                    //    Debug.LogError("complexdepnode_Person_J1_F1");
                    //}
                }
            }
        }

        if (simpledepnode.ChildNodes != null && complexdepnode.ChildNodes!=null)
        {
            foreach (DepNode simplechild in simpledepnode.ChildNodes)
            {
                if (simplechild == null) continue;
                DepNode complexchild = complexdepnode.ChildNodes.Find((item) => simplechild.NodeName == item.NodeName);
                LoadInit_Area(simplechild, complexchild);
            }
        }
    }

    /// <summary>
    /// 卸载时初始化
    /// </summary>
    public void UnloadInit(DepNode buildingBoxControllerT, DepNode buildingControllerT)
    {
        Unload_Area(buildingBoxControllerT, buildingControllerT);
    }

    /// <summary>
    /// 卸载时区域初始化设置
    /// </summary>
    public void Unload_Area(DepNode simpledepnode, DepNode complexdepnode)
    {
        if (simpledepnode == null || complexdepnode == null) return;
        simpledepnode.monitorRangeObject = complexdepnode.monitorRangeObject;
        if (simpledepnode.monitorRangeObject != null)
        {
            simpledepnode.monitorRangeObject.SetFollowTarget(simpledepnode.NodeObject);
        }
        //Transform ranges1 = complexdepnode.transform.Find("Ranges");
        //Transform ranges2 = simpledepnode.transform.Find("Ranges");

        if (complexdepnode)
        {
            int childcount = complexdepnode.transform.childCount;
            for (int i = childcount - 1; i >= 0; i--)
            {
                Transform t = complexdepnode.transform.GetChild(i);

                if (t.tag == "Person")
                {
                    t.SetParent(simpledepnode.transform);
                }
            }
        }

        if (complexdepnode !=null && complexdepnode.ChildNodes!=null)
            foreach (DepNode child1 in complexdepnode.ChildNodes)
            {
                DepNode child2 = simpledepnode.ChildNodes.Find((item) => item!=null && child1.NodeName == item.NodeName);
                Unload_Area(child2, child1);
            }
    }

    private void ReplaceDepNode(DepNode depNode,string tag)
    {
        if (depNode == null)
        {
            Debug.LogError("BuildingBox.ReplaceDepNode depNode == null ");
            return;
        }
        //Debug.Log("BuildingBox.ReplaceDepNode depNode :"+ depNode+ ",tag:"+ tag);
        if (roomFactory.Contains(depNode))//避免重复操作
        {
            //Debug.Log("BuildingBox.ReplaceDepNode roomFactory.Contains(depNode) :" + depNode);
            return;
        }
        roomFactory.SetTopoNode(depNode, topoNode, true);
        if (topoNode != null && topoNode.Children != null)
            roomFactory.BindingChild(depNode, topoNode.Children.ToList(), true);

        if (mapManager == null)
        {
            mapManager = MapLoadManage.Instance;
        }
        if (mapManager != null)
        {
            mapManager.InitBuildingId();
            mapManager.ReplaceDepNode(depNode);
        }
    
        FactoryDepManager.Instance.ReplaceNode(depNode);
    }


    //public bool haveRoamBounds = false;

    public DistanceChecker distanceChecker;

    [ContextMenu("InitDistanceBounds")]
    private void InitDistanceBounds()
    {
        if (distanceChecker == null)
        {
            distanceChecker = gameObject.AddComponent<DistanceChecker>();
            distanceChecker.InitDistanceBounds();
        }
    }


    public bool IsInDistance(Transform t,bool isRoam)
    {
        return distanceChecker.IsInDistance(t, isRoam);
    }

    public bool IsInBounds(Transform t)
    {
        return distanceChecker.IsInBounds(t);
    }

    private List<DeviceAssetInfo> GetDevicesAssets()
    {
        List<DeviceAssetInfo> deviceAssets = new List<DeviceAssetInfo>();
        GameObject buildingObject = buildingController.gameObject;
        DeviceAssetInfo buildingDevices = buildingObject.GetComponent<DeviceAssetInfo>();
        if (buildingDevices)
        {
            deviceAssets.Add(buildingDevices);
        }
        var floorDevices = buildingObject.FindComponentsInChildren<DeviceAssetInfo>();
        deviceAssets.AddRange(floorDevices);
        Debug.Log("BuildingBox.LoadDevices deviceAssets:" + deviceAssets.Count);
        return deviceAssets;
    }

    [ContextMenu("LoadDevices")]
    public void LoadDevicesAsset()
    {
        if (buildingController == null)
        {
            Debug.LogError("BuildingBox.LoadDevices buildingController == null");
            return;
        }

        List<DeviceAssetInfo> deviceAssets=GetDevicesAssets();
        for (int i = deviceAssets.Count - 1; i >= 0; i--)//先加载一楼
        {
            Debug.Log("BuildingBox.LoadAsset:" + deviceAssets[i]);
            deviceAssets[i].LoadAsset(null);
        }
    }

    [ContextMenu("UnloadDevices")]
    public void UnloadDevicesAsset()
    {
        List<DeviceAssetInfo> deviceAssets = GetDevicesAssets();
        if (deviceAssets != null)
            foreach (var deviceAsset in deviceAssets)
            {
                deviceAsset.SetUnload(true);
            }
    }
    List<Vector3> points = new List<Vector3>();
    [ContextMenu("GetTestPoints")]
    public List<Vector3> GetTestPoints()
    {
        if (points.Count > 0) return points;
        //distanceChecker
        var pos = transform.position;
        points.Add(pos);

        var bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
        var size = bounds.size/2;
        points.Add(pos + new Vector3(-size.x, -size.y, -size.z));
        points.Add(pos + new Vector3(-size.x, -size.y, size.z));
        points.Add(pos + new Vector3(-size.x, size.y, -size.z));
        points.Add(pos + new Vector3(size.x, -size.y, -size.z));
        points.Add(pos + new Vector3(-size.x, size.y, size.z));
        points.Add(pos + new Vector3(size.x, -size.y, size.z));
        points.Add(pos + new Vector3(size.x, size.y, -size.z));
        points.Add(pos + new Vector3(size.x, size.y, size.z));

        points.Add(pos + new Vector3(0, -size.y, -size.z));
        points.Add(pos + new Vector3(0, -size.y, size.z));
        points.Add(pos + new Vector3(0, size.y, -size.z));
        points.Add(pos + new Vector3(0, size.y, size.z));

        points.Add(pos + new Vector3(size.x, 0, -size.z));
        points.Add(pos + new Vector3(size.x, 0, size.z));
        points.Add(pos + new Vector3(size.x, 0, -size.z));
        points.Add(pos + new Vector3(size.x, 0, size.z));

        points.Add(pos + new Vector3(-size.x, -size.y, 0));
        points.Add(pos + new Vector3(size.x, -size.y, 0));
        points.Add(pos + new Vector3(-size.x, size.y, 0));
        points.Add(pos + new Vector3(size.x, size.y, 0));
        return points;
    }



    [ContextMenu("ShowTestPoints")]
    public List<Vector3> ShowTestPoints()
    {
        List<Vector3> points = GetTestPoints();
        foreach (var item in points)
        {
            CreateTestPoint(item);
        }
        return points;
    }

    private void CreateTestPoint(Vector3 pos)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = pos;
        obj.name = pos + "";
        obj.transform.parent = transform;
    }
}

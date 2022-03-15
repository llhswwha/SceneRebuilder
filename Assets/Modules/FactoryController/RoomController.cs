using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : DepNode
{
    ///// <summary>
    ///// 房间碰撞体
    ///// </summary>
    //public BoxCollider depCollider;
    ///// <summary>
    ///// 存放房间设备处
    ///// </summary>
    //private GameObject _roomDevContainer;
    ///// <summary>
    ///// 存放房间设备处
    ///// </summary>
    //public GameObject RoomDevContainer
    //{
    //    get
    //    {
    //        if (_roomDevContainer == null) InitDevContainer();
    //        return _roomDevContainer;
    //    }
    //}
    ///// <summary>
    ///// 设备创建完成
    ///// </summary>
    //private Action OnDevCreateComplete;
    //// Use this for initialization

    //void Awake()
    //{
    //    depType = DepType.Room;
    //}

    ///// <summary>
    ///// 设置监控区域
    ///// </summary>
    ///// <param name="oT"></param>
    //public override void SetMonitorRangeObject(MonitorRangeObject oT)
    //{
    //    monitorRangeObject = oT;
    //    InitDevContainer();
    //}
    //private void InitDevContainer()
    //{
    //    if (_roomDevContainer != null) return;
    //    _roomDevContainer = new GameObject("RoomDevContainer");
    //    _roomDevContainer.transform.parent = transform;
    //    FloorController floor = ParentNode as FloorController;
    //    if (floor)
    //    {
    //        if (monitorRangeObject != null)
    //        {
    //            transform.position = monitorRangeObject.transform.position;
    //        }
    //        Transform parentCotainer = floor.RoomDevContainer.transform;
    //        _roomDevContainer.transform.position = parentCotainer.position;
    //        _roomDevContainer.transform.eulerAngles = parentCotainer.eulerAngles;
    //        _roomDevContainer.transform.localScale = parentCotainer.lossyScale;
    //    }
    //}
    ///// <summary>
    ///// 隐藏机房设备
    ///// </summary>
    //public void HideRoomDev()
    //{
    //    RoomDevContainer.SetActive(false);
    //}
    ///// <summary>
    ///// 显示机房设备
    ///// </summary>
    //public void ShowRoomDev()
    //{
    //    RoomDevContainer.SetActive(true);
    //}
    ///// <summary>
    ///// 打开区域
    ///// </summary>
    ///// <param name="onComplete"></param>
    //public override void OpenDep(Action onComplete = null, bool isFocusT = true)
    //{
    //    HideFacotry();
    //    BuildingController building = ParentNode.ParentNode as BuildingController;
    //    if (building != null) building.LoadRoom(ParentNode, true, floor =>
    //    {
    //        OnDevCreateComplete = onComplete;
    //        //DepNode lastDep = FactoryDepManager.currentDep;
    //        //FactoryDepManager.currentDep = this;
    //        //SceneEvents.OnDepNodeChanged(lastDep, this);

    //        SceneEvents.OnDepNodeChanged(this);
    //        //Todo:摄像头聚焦    
    //        if (isFocusT)
    //        {
    //            FocusOn(() =>
    //            {
    //                AfterRoomFocus(true);
    //            });
    //        }
    //        else
    //        {
    //            AfterRoomFocus(true);
    //        }
    //    });

    //}
    //public override void HideDep(Action onComplete = null)
    //{
    //    FlashingOff();
    //    if (ParentNode != null)
    //    {
    //        ParentNode.HideDep();
    //    }
    //}
    ///// <summary>
    ///// 隐藏厂区建筑物
    ///// </summary>
    //private void HideFacotry()
    //{
    //    FactoryDepManager manager = FactoryDepManager.Instance;
    //    if (manager) manager.HideFacotry();
    //}
    ///// <summary>
    ///// 聚焦房间
    ///// </summary>
    ///// <param name="onFocusFinish"></param>
    //public override void FocusOn(Action onFocusFinish = null)
    //{
    //    AlignTarget alignTargetTemp;
    //    alignTargetTemp = monitorRangeObject != null ? GetTargetInfo(monitorRangeObject.gameObject) : GetTargetInfo(gameObject);
    //    CameraSceneManager camera = CameraSceneManager.Instance;
    //    //FlashingRoom();
    //    camera.FocusTargetWithTranslate(alignTargetTemp, AreaSize, onFocusFinish, () =>
    //     {
    //         if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
    //     });
    //}

    //public override void FocusOff(Action onFocusComplete = null)
    //{
    //    IsFocus = false;
    //    CameraSceneManager.Instance.ReturnToDefaultAlign(onFocusComplete);
    //}
    ///// <summary>
    ///// 房间聚焦之后
    ///// </summary>
    //private void AfterRoomFocus(bool isFlashing)
    //{
    //    FlashingRoom();
    //    CreateRoomDev(OnDevCreateComplete);
    //}
    ///// <summary>
    ///// 创建机房设备
    ///// </summary>
    //public void CreateRoomDev(Action onDevCreateCompleteT = null)
    //{
    //    try
    //    {
    //        if (ParentNode != null)
    //        {
    //            FloorController controller = ParentNode as FloorController;
    //            controller.CreateFloorDev(onDevCreateCompleteT);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("RoomController.CreateRoomdev :" + e.ToString());
    //        if (onDevCreateCompleteT != null) onDevCreateCompleteT();
    //    }
    //}
    ///// <summary>
    ///// 房间闪烁
    ///// </summary>
    ///// <param name="isShow">是否闪烁</param>
    //private void FlashingRoom()
    //{
    //    if (monitorRangeObject == null) return;
    //    if (RoomFactory.Instance)
    //    {
    //        List<DevNode> devs = RoomFactory.Instance.GetDepDevs(this);
    //        DevNode dev = devs.Find(i => i.isAlarm == true);
    //        if (dev != null) return;
    //    }

    //    if (monitorRangeObject.isAlarming) return;//如果机房正处于告警就返回
    //    monitorRangeObject.FlashingOn(Color.green, 2f);
    //    if (IsInvoking("FlashingOff"))
    //    {
    //        CancelInvoke("FlashingOff");
    //        Invoke("FlashingOff", 1.5f);
    //    }
    //    else
    //    {
    //        Invoke("FlashingOff", 1.5f);
    //    }
    //}
    ///// <summary>
    ///// 关闭闪烁
    ///// </summary>
    //private void FlashingOff()
    //{
    //    //房间有消防告警，不关闭高亮闪烁
    //    if(DevAlarmManage.Instance!=null&&NodeID!=0)
    //    {
    //        bool isDepFireAlarm = DevAlarmManage.Instance.IsDepFireAlarm(NodeID);
    //        if (isDepFireAlarm) return;
    //    }
    //    if (monitorRangeObject == null) return;
    //    monitorRangeObject.FlashingOff();
    //}

    ///// <summary>
    ///// 是否在机房区域内
    ///// </summary>
    ///// <param name="devPos"></param>
    ///// <returns></returns>
    //public bool IsInDepField(Vector3 devPos)
    //{
    //    if (depCollider == null) return false;
    //    depCollider.enabled = true;
    //    Bounds bounds = depCollider.bounds;
    //    bool rendererIsInsideTheBox = bounds.Contains(devPos);
    //    depCollider.enabled = false;
    //    return rendererIsInsideTheBox;
    //}

    //public Vector2 angleFocus = new Vector2(40, 180);
    //public float camDistance = 10;
    //[HideInInspector]
    //public Range angleRange = new Range(0, 90);
    //public Range disRange = new Range(2, 30);
    ////拖动区域大小
    //public Vector2 AreaSize = new Vector2(2, 2);
    ///// <summary>
    ///// 获取相机聚焦物体的信息
    ///// </summary>
    ///// <param name="obj"></param>
    ///// <returns></returns>
    //public AlignTarget GetTargetInfo(GameObject obj)
    //{
    //    AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
    //                           camDistance, angleRange, disRange);
    //    return alignTargetTemp;
    //}

    ////public void OnDestroy()
    ////{
    ////    int i = 0;
    ////    //Debug.LogError("RoomController_OnDestroy");
    ////    RoomFactory.Instance.RemoveDepNodeById(NodeID);
    ////}
}

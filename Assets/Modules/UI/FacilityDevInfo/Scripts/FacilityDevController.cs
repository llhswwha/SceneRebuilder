using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using RTEditor;
using Mogoson.CameraExtension;
using Y_UIFramework;

public class FacilityDevController : DevNode
{
	// Use this for initialization
	public override void Start ()
	{
        if (transform.GetComponent <Collider>()!=null )
        {
            DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
            trigger.onClick += OnClick;
            trigger.onDoubleClick += OnDoubleClick;
        }
        SaveInfoToRoomFactory();
    }
    public void OnClick()
    {
        if (!isInDevViewState()) return;
        Debug.Log("FacilityDevController.OnClick");
        if (PersonSubsystemManage.Instance.IsOnEditArea || TrainSubBar.FireState == true) return; //如果此时处于消防模式则点击设备高亮功能消失       
        EditorGizmoSystem gizmoSystem = EditorGizmoSystem.Instance;
        //当其他设备在拖动时，不高亮静态设备
        bool isGizmoUse = gizmoSystem != null && gizmoSystem.IsActiveGizmoReadyForObjectManipulation();
        if (!DevSubsystemManage.IsRoamState&&!isGizmoUse)
        {
            HighlightOn();
        }
        if(!isGizmoUse)ModifyDevInfo();
        if (Info == null||(string.IsNullOrEmpty(Info.Name)&&Info.Id==0))
        {
            Debug.LogError("FacilityDevController.OnClick Info == null");
            return;  //DevInfo为空，会导致切换区域时，界面关闭不掉
        }
        //if (string.IsNullOrEmpty(Info.KKSCode))
        //{
        //    Debug.LogError("FacilityDevController.KKS == null");
        //    return;
        //}
        if (Info.Name == "设备")
        {
            Debug.LogError("FacilityDevController.KKS == null");
            return;
        }
        ShowFollowUI();
        MessageCenter.SendMsg(MsgType.AreaDevTreePanelMsg.TypeName, MsgType.AreaDevTreePanelMsg.SelectDev, Info);
        if (Info != null)
        {
            Debug.Log("Click  ID: " + Info.Id + " DevID: " + Info.DevID);
        }   
    }
    public void OnDoubleClick()
    {
        if (TrainSubBar.FireState == true|| !isInDevViewState()) return; //如果此时处于消防模式则双击聚焦功能失效
        if (gameObject.GetComponent<BuildingController>() != null) return;
        DepNode dep = transform.GetComponent<DepNode>();
        if (dep) return;//既是建筑，又是设备，先响应建筑的双击
        Debug.Log("FacilityDevController.OnDoubleClick");
        if (DevSubsystemManage.IsRoamState) return;
        if (Info == null)
        {
            Debug.LogError("FacilityDevController.OnDoubleClick Info == null");
            //return;//2019_05_14_cww:不还回
        }
         FocusDev();
    }

    private bool isInDevViewState()
    {
        ViewState s= SceneManage.GetInstance().CurrentViewState;

        return s == ViewState.人员定位 || s == ViewState.设备定位;
    }

    /// <summary>
    /// 显示生产信息漂浮UI
    /// </summary>
    public void ShowFollowUI()
    {
        if (FollowUI == null)
        {
            CreateFollowUI();
        }

        bool isEditMode = DevSubsystemManage.Instance != null && DevSubsystemManage.Instance.DevEditorToggle.isOn ? true : false;
        if (FollowUI!=null&& !DevSubsystemManage.IsRoamState&&!isEditMode)
        {
            DeviceFollowUI devInfo = FollowUI.GetComponent<DeviceFollowUI>();
            if(devInfo!=null)devInfo.ShowUI();
        }
    }

    private void SaveInfoToRoomFactory()
    {
        RoomFactory factory = RoomFactory.Instance;
        if(factory)
        {
            factory.SaveStaticDevInfo(this);
        }
    }

    /// <summary>
    /// 聚焦/取消聚焦
    /// </summary>
    private void FocusDev()
    {
        if (!IsFocus) FocusOn();
        else FocusOff();
    }

    public override void FocusOn()
    {
        bool sameArea = IsSameArea();
        if (CurrentFocusDev != null) CurrentFocusDev.FocusOff(false);
        IsFocus = true;
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager)
        {
            if (sameArea)
            {
                AlignTarget target = GetTargetInfo(gameObject);
                manager.FocusTargetWithTranslate(target, new Vector2(20, 20), () =>
                {
                    ChangeBackButtonState(true);
                });
                HighlightOn();
            }
            else
            {
                RoomFactory.Instance.FocusNode(ParentDepNode, () =>
                {
                    AlignTarget target = GetTargetInfo(gameObject);
                    manager.FocusTargetWithTranslate(target, new Vector2(20, 20), () =>
                    {
                        ChangeBackButtonState(true);
                    });
                    HighlightOn();
                });
            }
            CurrentFocusDev = this;
        }
        MessageCenter.SendMsg(MsgType.AreaDevTreePanelMsg.TypeName, MsgType.AreaDevTreePanelMsg.SelectDev,Info);
        if(MHTopoTest_Pipe.Instance&&devName.Contains("泵"))
        {
            MHTopoTest_Pipe.Instance.Show(devName);            
        }
        else
        {
            ShowFollowUI();
        }      
    }
    public override void FocusOff(bool isCameraBack = true)
    {
        base.FocusOff(isCameraBack);
        if (DeviceFollowUI.CurrentMonitor != null) DeviceFollowUI.CurrentMonitor.CloseUI();
        if (MHTopoTest_Pipe.Instance && transform.GetComponentInParent<DepNode>().NodeName.Contains("泵"))
        {
            MHTopoTest_Pipe.Instance.Close();
        }
    }

    private bool isInfoFinded = false;

    ///// <summary>
    ///// 创建漂浮UI
    ///// </summary>
    //public void CreateFollowUI()
    //{
    //    if (Info == null)
    //    {
    //        if (isInfoFinded == false)
    //        {
    //            Info = CommunicationObject.Instance.GetDevByGameObjecName(this.name);
    //            Log.Info("FacilityDevController.CreateFollowUI", string.Format("FindInfo:{0},{1}", this.name, Info));
    //            isInfoFinded = true;
    //        }
            
    //    }
    //    if (ParentDepNode == null)
    //    {
    //        Log.Error("FacilityDevController.CreateFollowUI", "ParentDepNode == null");
    //    }

    //    FollowTargetManage.Instance.CreateDevFollowUI(gameObject, ParentDepNode, this, obj => { followUI = obj; });
    //}
    /// <summary>
    /// 修改设备信息
    /// </summary>
    private void ModifyDevInfo()
    {
        if (!DevSubsystemManage.isDevEdit) return;
        //出现了info不为空，但里面所有值都是空的情况
        CommunicationObject service = CommunicationObject.Instance;
        {
            if(service)
            {
                service.GetDevInfoByModelNameAsync(devName,infoT=> 
                {
                    if(infoT==null)
                    {
                        AddDevInfoToServer(newDevInfo =>
                        {
                            Info = newDevInfo;
                            ShowDevEidtInfo();
                        });
                    }
                    else
                    {
                        if (Info == null || Info.Id == 0) Info = infoT;
                        ShowDevEidtInfo();
                    }

                },error=> 
                {
                    Log.Error("FacilityDevController.Error:"+error);
                });
            }
        }     
    }
    /// <summary>
    /// 显示设备编辑信息
    /// </summary>
    private void ShowDevEidtInfo()
    {
        DevSubsystemManage manager = DevSubsystemManage.Instance;
        if (manager && manager.DevEditorToggle.isOn)
        {
            ClearSelection();
            //DeviceEditUIPanel.Instacne.ShowSingleDev(this);
            UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowSingleDev, this);
        }
    }

    /// <summary>
    /// 把设备信息添加到数据库
    /// </summary>
    /// <param name="onComplete"></param>
    private void AddDevInfoToServer(Action<DevInfo>onComplete)
    {
        CommunicationObject service = CommunicationObject.Instance;
        DevInfo devInfo = GetDevInfo(this);
        service.AddDevInfoAsync(devInfo, devcallBack =>
        {
            if (onComplete != null) onComplete(devcallBack);
        });
    }
    private DevInfo GetDevInfo(FacilityDevController controller)
    {
        GameObject model = controller.gameObject;
        DevInfo dev = new DevInfo();
        dev.DevID = Guid.NewGuid().ToString();
        dev.IP = "";
        dev.CreateTime = DateTime.Now;
        dev.ModifyTime = DateTime.Now;
        dev.Name = "设备";
        //dev.ModelName = model.name;
        dev.ModelName = controller.devName;
        dev.Status = 0;
        dev.ParentId = GetPID(model);
        dev.TypeCode = 20181008;
        dev.UserName = "admin";
        return dev;
    }
    private int? GetPID(GameObject model)
    {
        DepNode parentNode = model.GetComponentInParent<DepNode>();
        if(parentNode is BuildingController||parentNode is DepController)
        {
            parentNode = FactoryDepManager.Instance;
        }
        return parentNode.NodeID;
    }
    /// <summary>
    /// 清除设备选中
    /// </summary>
    private void ClearSelection()
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            selection.ClearSelection(false);
        }
    }
    /// <summary>
    /// 鼠标是否Hover
    /// </summary>
    /// <param name="isEnter"></param>
    public void SetMouseState(bool isEnter)
    {
        if(isEnter)
        {
            if (!DevSubsystemManage.IsRoamState) return;
            HighlightOn();
            DevSubsystemManage.Instance.SetFocusDevInfo(this, true);
            //if (RoamDevInfoPanel.Instance) RoamDevInfoPanel.Instance.ShowDevInfo(Info);
            UIManager.GetInstance().ShowUIPanel(typeof(RoamDevInfoPanel).Name);
            MessageCenter.SendMsg(MsgType.RoamDevInfoPanelMsg.TypeName, MsgType.RoamDevInfoPanelMsg.ShowDevInfo, Info);
        }
        else
        {
            if (!DevSubsystemManage.IsRoamState) return;
            HighLightOff();
            DevSubsystemManage.Instance.SetFocusDevInfo(this, false);
            MessageCenter.SendMsg(MsgType.RoamDevInfoPanelMsg.TypeName, MsgType.RoamDevInfoPanelMsg.ClosePanel, null);
        }
    }

    //void OnMouseEnter()
    //{
    //    if (!DevSubsystemManage.IsRoamState) return;
    //    HighlightOn();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(this, true);
    //    if (RoamDevInfoPanel.Instance) RoamDevInfoPanel.Instance.ShowDevInfo(Info);
    //}
    //void OnMouseExit()
    //{
    //    if (!DevSubsystemManage.IsRoamState) return;
    //    HighLightOff();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(this, false);
    //    if (RoamDevInfoPanel.Instance) RoamDevInfoPanel.Instance.Close();
    //}

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (RoomFactory.Instance.StaticDevList.Contains(this))
        {
            RoomFactory.Instance.StaticDevList.Remove(this);
        }
    }


    float radius = 0;

    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected override AlignTarget GetTargetInfo(GameObject obj)
    {
        if (MHTopoTest_Pipe.Instance && devName.Contains("泵"))
        {
            return new AlignTarget(obj.transform, new Vector2(25, 73),
                                   4, new Range(0, 90), new Range(0, 10));
        }
        else
        {
            if(RoomFactory.Instance.FactoryType==FactoryTypeEnum.ShiDongKou)
            {
                if(devName.Contains("屏"))
                {
                    return new AlignTarget(obj.transform, new Vector2(19, 0),
                                  8f, new Range(0, 90), new Range(0, 15));
                }
                else
                {
                    return new AlignTarget(obj.transform, new Vector2(26, 293),
                                   52.8f, new Range(0, 90), new Range(0, 100));
                }
                
            }
            else
            {
                if (radius == 0)
                {
                    var bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
                    radius = ColliderHelper.GetRadius(bounds.size);
                }

                camDistance = radius * 1.5f;
                disRange = new Range(radius * 0.75f, radius * 10f);
                angleFocus = new Vector2(60, 60);

                AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                                       camDistance, angleRange, disRange);
                return alignTargetTemp;
            }          
        }
    }           
}

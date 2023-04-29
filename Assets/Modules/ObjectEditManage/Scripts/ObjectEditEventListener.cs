using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
using Location.WCFServiceReferences.LocationServices;
using System.Linq;
using Unity.ComnLib.Utils;
using Assets.M_Plugins.Helpers.Utils;
using System;
using Y_UIFramework;
using ModelGroupEdit;

public class ObjectEditEventListener : MonoBehaviour {

    private List<GameObject> selectedObjs = new List<GameObject>();//当前选中设备

    // Use this for initialization
    void Start()
    {
        BindingGizmosEvent();
    }
    /// <summary>
    /// 绑定设备编辑事件
    /// </summary>
    private void BindingGizmosEvent()
    {
        EditorGizmoSystem GizmoSystem = EditorGizmoSystem.Instance;
        if (GizmoSystem == null)
        {
            Debug.LogWarning("EditorGizmoSystem.Instance is null!");
        }
        else
        {
            //移动编辑事件
            GizmoSystem.TranslationGizmo.GizmoDragStart += TranslationGizmosStart;
            GizmoSystem.TranslationGizmo.GizmoDragUpdate += TranslationGizmosUpdate;
            GizmoSystem.TranslationGizmo.GizmoDragEnd += TranslationGizmosEnd;
            //旋转编辑事件
            GizmoSystem.RotationGizmo.GizmoDragStart += RotationGizmosStart;
            //GizmoSystem.RotationGizmo.GizmoDragUpdate += RotationGizmosUpdate;
            GizmoSystem.RotationGizmo.GizmoDragEnd += RotationGizmosEnd;
            //缩放编辑事件
            GizmoSystem.ScaleGizmo.GizmoDragStart += ScaleGizmosStart;
            //GizmoSystem.ScaleGizmo.GizmoDragUpdate += ScaleGizmosUpdate;
            GizmoSystem.ScaleGizmo.GizmoDragEnd += ScaleGizmosEnd;
        }
        if(EditorObjectSelection.Instance)
        {
            //物体选中和取消选中的事件
            EditorObjectSelection.Instance.SelectionChanged += OnEditObjectSelectionChange;
        }

        if(RTEManager.Instance)
        {
            RTEManager.OnSelectionEvent += SelectionChanged;
            RTEManager.GizmoDragEvent += OnGizmoUpdate;
            RTEManager.GizemoDragEndEvent += GizemoDragEndEvent;
        }

    }
    /// <summary>
    /// 编辑物体，选中和取消选中的事件
    /// </summary>
    /// <param name="selectionChangedEventArgs"></param>
    public void OnEditObjectSelectionChange(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection == null) return;
        SelectionChanged(selection.SelectedGameObjects.ToList());
    }

    private void SelectionChanged(List<GameObject> selectedGameObjects)
    {
        if (ObjectAddListPanel.IsEditMode||DevKKSModelPanel.IsEditMode)
        {
            if (IsClickUGUIorNGUI.Instance.isOverUI) return;
            HideDevInfo();
            if (selectedGameObjects.Count == 0)
            {
                //DeviceEditUIPanel.Instacne.SetEmptValue();
                UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
                MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.SetEmptyValue, null);
                SurroundEditMenu_BatchCopy.Instacne.CloseUI();
            }
            else
            {
                ClearDevHighlight();
                selectedObjs = selectedGameObjects;
                List<DevNode> devs = GetDevNode(selectedObjs);
                if(devs!=null&&devs.Count==1&&devs[0].Info!=null)
                {
                    if (ObjectAddListPanel.IsEditMode&&TypeCodeHelper.IsKKSMonitor(devs[0].Info.TypeCode.ToString()))
                    {
                        if (RTEManager.Instance) RTEManager.Instance.TryToClearSelection();
                        return;
                    }
                    if (DevKKSModelPanel.IsEditMode && !TypeCodeHelper.IsKKSMonitor(devs[0].Info.TypeCode.ToString()))
                    {
                        if (RTEManager.Instance) RTEManager.Instance.TryToClearSelection();
                        return;
                    }
                }
                ShowDevInfo(devs);
                //SetBatchCopyState(devs);
            }
        }        
    }

    /// <summary>
    /// 清除静态设备高亮
    /// </summary>
    private void ClearDevHighlight()
    {
        if(HighlightManage.Instance)
        {
            HighlightManage.Instance.HighLightDevOff();
        }
    }
    /// <summary>
    /// 设置批量复制按钮
    /// </summary>
    /// <param name="devList"></param>
    private void SetBatchCopyState(List<DevNode>devList)
    {
        SurroundEditMenu_BatchCopy copyPart = SurroundEditMenu_BatchCopy.Instacne;
        if (copyPart)
        {
            if (devList.Count > 1) copyPart.CloseUI();
            else if (devList.Count == 1)
            {
                DevNode dev = devList[0];
                if (dev is RoomDevController || dev is DepDevController||!TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()))
                {
                    if (ObjectAddListPanel.IsEditMode) copyPart.Open(dev);
                }
                else
                {
                    copyPart.CloseUI();
                }
            }
        }
    }
  
    /// <summary>
    /// 获取设备
    /// </summary>
    /// <param name="devs"></param>
    /// <returns></returns>
    private List<DevNode>GetDevNode(List<GameObject>devs)
    {
        List<DevNode> devList = new List<DevNode>();
        foreach (var item in devs)
        {
            DevNode dev = item.GetComponent<DevNode>();
            if (dev == null) continue;
            devList.Add(dev);
        }
        return devList;
    }
    /// <summary>
    /// 关闭设备编辑界面
    /// </summary>
    private void HideDevInfo()
    {
        //DeviceEditUIPanel.Instacne.Close();
        //DeviceEditUIPanel.Instacne.HideMultiDev();
        MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.Close, null);
        MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.HideMultiDev, null);
    }
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    /// <param name="selectObjList"></param>
    private void ShowDevInfo(List<DevNode> selectObjList)
    {
        if (selectObjList.Count == 1)
        {
            //DeviceEditUIPanel.Instacne.ShowSingleDev(selectObjList[0]);
            UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowSingleDev, selectObjList[0]);
            SelectDevInGroupMode(selectObjList[0]);
        }
        else if (selectObjList.Count > 1)
        {           
            ShowDevFollowUI(selectObjList);
        }       
    }

    /// <summary>
    /// 设备分组模式，选中树节点中的设备
    /// </summary>
    /// <param name="dev"></param>
    private void SelectDevInGroupMode(DevNode dev)
    {
        if (ModelGroupPanel.IsModelGroupMode)
        {
            //panelT.SelectDev(dev);
            MessageCenter.SendMsg(MsgType.ModelGroupPanelMsg.TypeName, MsgType.ModelGroupPanelMsg.SelectDev, dev);
        }
    }

    /// <summary>
    /// 显示设备漂浮UI
    /// </summary>
    /// <param name="devList"></param>
    private void ShowDevFollowUI(List<DevNode>devList)
    {
        if (devList.Count == 1)
        {
            //DeviceEditUIPanel.Instacne.ShowSingleDev(devList[0]);
            UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowSingleDev, devList[0]);
        }
        else if (devList.Count > 1)
        {
            //DeviceEditUIPanel.Instacne.ShowMultiDev(devList);
            UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowMultiDev, devList);
        }
    }

    private CommunicationObject Service;
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="ObjList"></param>
    private void ModifyDevPos(List<GameObject>ObjList)
    {
        if (Service == null)
        {
            Service = CommunicationObject.Instance;
        }
        List<DevPos> posList = new List<DevPos>();
        foreach (var Obj in ObjList)
        {
            DevNode Dev = Obj.GetComponent<DevNode>();
            if (Dev == null) continue;
            DevPos pos = LocationManager.CaculateDevPos(Dev,Obj);
            //Service.ModifyDevPos(pos);  
            posList.Add(pos);
        }
        if (posList.Count != 0)
        {
            Service.ModifyDevPosByListAsync(posList,value=> { Debug.Log("ModifyPosList..."); });
        }
    }
    // /// <summary>
    // /// 更新设备位置信息
    // /// </summary>
    // /// <param name="devInfo"></param>
    // /// <param name="model"></param>
    // private DevPos CaculateDevPos(DevNode devInfo,GameObject model)
    // {
    //     DevPos posInfo = new DevPos();
    //     posInfo.DevID = devInfo.Info.DevID;
    //     Vector3 cadPos = GetCadPos(model,devInfo);
    //     posInfo.PosX = cadPos.x;
    //     posInfo.PosY = cadPos.y;
    //     posInfo.PosZ = cadPos.z;
    //     Vector3 rotation = model.transform.eulerAngles;
    //     posInfo.RotationX = rotation.x;
    //     posInfo.RotationY = rotation.y;
    //     posInfo.RotationZ = rotation.z;
    //     Vector3 scale = model.transform.localScale;
    //     posInfo.ScaleX = scale.x;
    //     posInfo.ScaleY = scale.y;
    //     posInfo.ScaleZ = scale.z;
    //     return posInfo;
    // }
    // /// <summary>
    // /// 获取CAD位置
    // /// </summary>
    // /// <param name="obj"></param>
    // /// <param name="dev"></param>
    // /// <returns></returns>
    // private Vector3 GetCadPos(GameObject obj,DevNode dev)
    // {
    //     //DeviceEdit editPart = DeviceEditUIManager.Instacne.EditPart;
    //     Vector3 cadPos;
    //     bool isLocalPos = !(dev.ParentDepNode==FactoryDepManager.Instance||dev is DepDevController);
    //     if (!isLocalPos)
    //     {
    //         cadPos = LocationManager.UnityToCadPos(obj.transform.position, false);
    //     }
    //     else
    //     {
    //         cadPos = LocationManager.UnityToCadPos(obj.transform.localPosition, true);
    //     }
    //     return cadPos;
    // }
    #region TranslationGizmo
    /// <summary>
    /// 移动开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 移动更新
    /// </summary>
    /// <param name="gizmo"></param>
    //private void TranslationGizmosUpdate(Gizmo gizmo, CameraDevController cameraDev)
    //{
    //    List<GameObject> controlObj = gizmo.ControlledObjects.ToList();
    //    if (controlObj.Count == 1)
    //    {
    //        GameObject modelT = controlObj[0];
    //        DevNode dev = modelT.GetComponent<DevNode>();
    //        if (dev == null||dev.Info==null) return;
    //        dev.Info.Pos = CaculateDevPos(dev, modelT);
    //        DeviceEditUIManager.Instacne.Show(dev);
    //        //保存移动以后位置
    //        //cameraDev.EditCaneraDevPos(cameraDev.Info,null, ps =>
    //        //{

    //        //});
    //        cameraDev.EditCaneraDevPos(cameraDev.Info, null,null);

    //    }
    //}

    private void TranslationGizmosUpdate(Gizmo gizmo)
    {
        List<GameObject> controlObj = gizmo.ControlledObjects.ToList();
        OnGizmoUpdate(controlObj);
    }

    private void OnGizmoUpdate(List<GameObject>objs)
    {        
        if (objs.Count == 1)
        {
            GameObject modelT = objs[0];
            DevNode dev = modelT.GetComponent<DevNode>();
            if (dev == null || dev.Info == null) return;
            dev.Info.Pos = LocationManager.CaculateDevPos(dev, modelT);
            //DeviceEditUIPanel.Instacne.ShowSingleDev(dev);
            //UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowSingleDev, dev);          
        }
    }

    /// <summary>
    /// 移动结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosEnd(Gizmo gizmo)
    {
        List<GameObject> objList = gizmo.ControlledObjects.ToList();
        GizemoDragEndEvent(objList);
    }

    private void GizemoDragEndEvent(List<GameObject> controlObj)
    {
        ModifyDevPos(controlObj);        
        if (controlObj.Count == 1)
        {
            GameObject modelT = controlObj[0];
            DevNode dev = modelT.GetComponent<DevNode>();
            if (dev == null || dev.Info == null) return;
            dev.Info.Pos = LocationManager.CaculateDevPos(dev, modelT);
            //CommunicationObject.Instance.EditCameraInfo(dev, ps => {
            //    Debug.LogError("CameraInfo:" + ps);
            //});
            //修改摄像头位置 设备位置
            CommunicationObject.Instance.EditModifyDevInfoPos(dev, ps => {
                Debug.LogError("CameraInfo:" + ps);
            });
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.UpdateRangeInfo, dev);
        }
    }

    #endregion
    #region RotationGizmo
    /// <summary>
    /// 旋转开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 旋转更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 旋转结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region ScaleGizmo
    /// <summary>
    /// 缩放开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 缩放更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 缩放结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
}

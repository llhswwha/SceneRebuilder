using Battlehub.RTHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RTEditor;
using Y_UIFramework;
/// <summary>
/// 物体编辑管理控制脚本
/// </summary>
public class ObjectsEditManage : MonoBehaviour
{
    public static ObjectsEditManage Instance;
    /// <summary>
    /// 编辑部分（不使用时关闭，减少性能消耗）
    /// </summary>
    public GameObject EditPart;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SceneEvents.DepNodeChanged += OnDepNodeChange;
        //EndEdit();
        //Invoke("EndEdit", 2f);
    }

    /// <summary>
    /// 开始编辑
    /// </summary>
    public void StartEdit()
    {
        if (!EditPart.activeInHierarchy)
            EditPart.SetActive(true);
    }
    /// <summary>
    /// 结束编辑
    /// </summary>
    public void EndEdit()
    {
        if (EditPart.activeInHierarchy)
            EditPart.SetActive(false);
    }

    /// <summary>
    /// 设置Translation，Rotation，Scale，VolumeScale是否可以编辑，是为了在不编辑状态时，按Q,W,E,R等键时不会影响到，开启编辑起始状态
    /// </summary>
    public void SetEditorGizmoSystem(bool isActive)
    {
        //EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Translation, isActive);
        //EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Rotation, isActive);
        //EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Scale, isActive);
        //EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.VolumeScale, isActive);

        //EditorGizmoSystem.Instance.ActiveGizmoType = GizmoType.Translation;

        if(isActive)
        {
            if (RTEManager.Instance && RoomFactory.Instance.RemoteMode != RemoteMode.RenderStreaming)
            {
                //RTEManager.Instance.HideHandles();
                RTEManager.Instance.ShowToolbar();
            }

            if (ModelSelection.Instance)
            {
                ModelSelection.Instance.enabled = false;
                ModelSelection.Instance.RecoveryLastClickRender();
            }
        }
        
    }

    private void OnDepNodeChange(DepNode last, DepNode newDep)
    {
        if (ObjectAddListPanel.IsEditMode)
        {
            ClearSelection();
            //DeviceEditUIPanel manager = DeviceEditUIPanel.Instacne;
            //if(manager)
            //{
            //    manager.Close();
            //    manager.HideMultiDev();
            //    manager.SetEmptValue();
            //}
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.Close, null);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.HideMultiDev, null);
            MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.SetEmptyValue, null);
            //EditorCamera.Instance.SetObjectVisibilityDirty();
            ClearSelection();
        }
    }
    /// <summary>
    /// 清除设备选中
    /// </summary>
    private void ClearSelection()
    {
        //EditorObjectSelection selection = EditorObjectSelection.Instance;
        //if (selection)
        //{
        //    selection.ClearSelection(false);
        //}
        if (RTEManager.Instance) RTEManager.Instance.TryToClearSelection();
    }
    /// <summary>
    /// 设置可选择的Layer
    /// </summary>
    /// <param name="layerList"></param>
    public void SetSelectionLayer(List<int> layerList)
    {
        //RTEditor.EditorObjectSelection editorSelection = RTEditor.EditorObjectSelection.Instance;
        //if (editorSelection)
        //{
        //    foreach (var item in layerList)
        //    {
        //        int layerBits = editorSelection.ObjectSelectionSettings.SelectableLayers;
        //        bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
        //        if (!isSet)
        //        {
        //            editorSelection.ObjectSelectionSettings.SelectableLayers = LayerHelper.SetLayerBit(layerBits, item);
        //        }
        //    }
        //}
        if (RTEManager.Instance == null) return;
        var selectionCom = RTEManager.Instance.GetSelectionComponent();
        if(selectionCom)
        {
            foreach (var item in layerList)
            {
                int layerBits = selectionCom.SelectableLayers;
                bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
                if (!isSet)
                {
                    selectionCom.SelectableLayers = LayerHelper.SetLayerBit(layerBits, item);
                }
            }
        }

    }
    /// <summary>
    /// 取消可选择的Layer
    /// </summary>
    /// <param name="layerList"></param>
    public void CloseSelectionLayer(List<int> layerList)
    {
        //EditorObjectSelection editorSelection = EditorObjectSelection.Instance;
        //if (editorSelection)
        //{
        //    foreach (var item in layerList)
        //    {
        //        int layerBits = editorSelection.ObjectSelectionSettings.SelectableLayers;
        //        bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
        //        if (isSet)
        //        {
        //            editorSelection.ObjectSelectionSettings.SelectableLayers = LayerHelper.ClearLayerBit(layerBits, item);
        //        }
        //    }
        //}
        if (RTEManager.Instance == null) return;
        var selectionCom = RTEManager.Instance.GetSelectionComponent();
        if (selectionCom)
        {
            foreach (var item in layerList)
            {
                int layerBits = selectionCom.SelectableLayers;
                bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
                if (isSet)
                {
                    selectionCom.SelectableLayers = LayerHelper.ClearLayerBit(layerBits, item);
                }
            }
        }
    }
    #region 开启和关闭设备编辑

    private string DepDeviceName = "DepDevice";
    private string RoomDeviceName = "RoomDevice";
    /// <summary>
    /// 开启设备编辑
    /// </summary>
    public void OpenDevEdit()
    {
        SetSelectionLayer(GetDevLayers());
    }
    /// <summary>
    /// 关闭设备编辑
    /// </summary>
    public void CloseDevEdit()
    {
        CloseSelectionLayer(GetDevLayers());
    }
    /// <summary>
    /// 获取设备所在layer
    /// </summary>
    /// <returns></returns>
    private List<int> GetDevLayers()
    {
        List<int> devLayers = new List<int>()
        {
            LayerMask.NameToLayer(DepDeviceName),
            LayerMask.NameToLayer(RoomDeviceName)
        };
        return devLayers;
    }
    
    public void OpenRangeEdit()
    {
        SetSelectionLayer(new List<int> { LayerMask.NameToLayer(Layers.Range)}); ;
    }
    public void CloseRangeEdit()
    {
        CloseSelectionLayer(new List<int> { LayerMask.NameToLayer(Layers.Range) }); ;
    }
    #endregion
}

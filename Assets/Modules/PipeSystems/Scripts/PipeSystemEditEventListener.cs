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
using Battlehub.RTCommon;

//ObjectEditEventListener
public class PipeSystemEditEventListener : SingletonBehaviour<PipeSystemEditEventListener>
{

    private List<GameObject> selectedObjs = new List<GameObject>();//��ǰѡ���豸

    // Use this for initialization
    void Start()
    {
        BindingGizmosEvent();
    }
    /// <summary>
    /// ���豸�༭�¼�
    /// </summary>
    private void BindingGizmosEvent()
    {
        EditorGizmoSystem GizmoSystem = EditorGizmoSystem.Instance;
        if (GizmoSystem == null)
        {
            Debug.LogError("EditorGizmoSystem.Instance is null!");
        }
        else
        {
            //�ƶ��༭�¼�
            GizmoSystem.TranslationGizmo.GizmoDragStart += TranslationGizmosStart;
            GizmoSystem.TranslationGizmo.GizmoDragUpdate += TranslationGizmosUpdate;
            GizmoSystem.TranslationGizmo.GizmoDragEnd += TranslationGizmosEnd;
            //��ת�༭�¼�
            GizmoSystem.RotationGizmo.GizmoDragStart += RotationGizmosStart;
            //GizmoSystem.RotationGizmo.GizmoDragUpdate += RotationGizmosUpdate;
            GizmoSystem.RotationGizmo.GizmoDragEnd += RotationGizmosEnd;
            //���ű༭�¼�
            GizmoSystem.ScaleGizmo.GizmoDragStart += ScaleGizmosStart;
            //GizmoSystem.ScaleGizmo.GizmoDragUpdate += ScaleGizmosUpdate;
            GizmoSystem.ScaleGizmo.GizmoDragEnd += ScaleGizmosEnd;
        }
        if (EditorObjectSelection.Instance)
        {
            //����ѡ�к�ȡ��ѡ�е��¼�
            EditorObjectSelection.Instance.SelectionChanged += OnEditObjectSelectionChange;
        }

        if (RTEManager.Instance)
        {
            RTEManager.OnSelectionEvent += SelectionChanged;
            RTEManager.GizmoDragStartEvent += GizemoDragStartEvent;
            RTEManager.GizmoDragEvent += OnGizmoUpdate;
            RTEManager.GizemoDragEndEvent += GizemoDragEndEvent;
        }

    }
    /// <summary>
    /// �༭���壬ѡ�к�ȡ��ѡ�е��¼�
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
        //if (!ObjectAddListPanel.IsEditMode) return;
        //HideDevInfo();
        //if (selectedGameObjects.Count == 0)
        //{
        //    //DeviceEditUIPanel.Instacne.SetEmptValue();
        //    UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
        //    MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.SetEmptyValue, null);
        //    SurroundEditMenu_BatchCopy.Instacne.CloseUI();
        //}
        //else
        //{
        //    ClearDevHighlight();
        //    selectedObjs = selectedGameObjects;
        //    List<DevNode> devs = GetDevNode(selectedObjs);
        //    ShowDevInfo(devs);
        //    //SetBatchCopyState(devs);
        //}
    }

    /// <summary>
    /// �����̬�豸����
    /// </summary>
    private void ClearDevHighlight()
    {
        if (HighlightManage.Instance)
        {
            HighlightManage.Instance.HighLightDevOff();
        }
    }
    /// <summary>
    /// �����������ư�ť
    /// </summary>
    /// <param name="devList"></param>
    private void SetBatchCopyState(List<DevNode> devList)
    {
        //SurroundEditMenu_BatchCopy copyPart = SurroundEditMenu_BatchCopy.Instacne;
        //if (copyPart)
        //{
        //    if (devList.Count > 1) copyPart.CloseUI();
        //    else if (devList.Count == 1)
        //    {
        //        DevNode dev = devList[0];
        //        if (dev is RoomDevController || dev is DepDevController || !TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()))
        //        {
        //            if (ObjectAddListPanel.IsEditMode) copyPart.Open(dev);
        //        }
        //        else
        //        {
        //            copyPart.CloseUI();
        //        }
        //    }
        //}
    }

    /// <summary>
    /// ��ȡ�豸
    /// </summary>
    /// <param name="devs"></param>
    /// <returns></returns>
    private List<DevNode> GetDevNode(List<GameObject> devs)
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
    /// �ر��豸�༭����
    /// </summary>
    private void HideDevInfo()
    {
        //DeviceEditUIPanel.Instacne.Close();
        //DeviceEditUIPanel.Instacne.HideMultiDev();
        MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.Close, null);
        MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.HideMultiDev, null);
    }
    /// <summary>
    /// ��ʾ�豸�༭����
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
    /// �豸����ģʽ��ѡ�����ڵ��е��豸
    /// </summary>
    /// <param name="dev"></param>
    private void SelectDevInGroupMode(DevNode dev)
    {
        //if (ModelGroupPanel.IsModelGroupMode)
        //{
        //    //panelT.SelectDev(dev);
        //    MessageCenter.SendMsg(MsgType.ModelGroupPanelMsg.TypeName, MsgType.ModelGroupPanelMsg.SelectDev, dev);
        //}
    }

    /// <summary>
    /// ��ʾ�豸Ư��UI
    /// </summary>
    /// <param name="devList"></param>
    private void ShowDevFollowUI(List<DevNode> devList)
    {
        //if (devList.Count == 1)
        //{
        //    //DeviceEditUIPanel.Instacne.ShowSingleDev(devList[0]);
        //    UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
        //    MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowSingleDev, devList[0]);
        //}
        //else if (devList.Count > 1)
        //{
        //    //DeviceEditUIPanel.Instacne.ShowMultiDev(devList);
        //    UIManager.GetInstance().ShowUIPanel(typeof(DeviceEditUIPanel).Name);
        //    MessageCenter.SendMsg(MsgType.DeviceEditUIPanelMsg.TypeName, MsgType.DeviceEditUIPanelMsg.ShowMultiDev, devList);
        //}
    }

    private CommunicationObject Service;
    /// <summary>
    /// ��ȡCADλ��
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dev"></param>
    /// <returns></returns>
    private Vector3 GetCadPos(GameObject obj, DevNode dev)
    {
        //DeviceEdit editPart = DeviceEditUIManager.Instacne.EditPart;
        Vector3 cadPos;
        bool isLocalPos = !(dev.ParentDepNode == FactoryDepManager.Instance || dev is DepDevController);
        if (!isLocalPos)
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.position, false);
        }
        else
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.localPosition, true);
        }
        return cadPos;
    }
    #region TranslationGizmo

    public bool IsGraging=false;
    /// <summary>
    /// �ƶ���ʼ
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosStart(Gizmo gizmo)
    {
        
        Debug.Log($"PipeSystemEditEventListener.TranslationGizmosStart gizmo:{gizmo}");
        List<GameObject> objs = gizmo.ControlledObjects.ToList();
        GizemoDragStartEvent(objs);
    }

    public Vector3 startPos;

    //public Vector3 endPos;

    private void GizemoDragStartEvent(List<GameObject> objs)
    {
        IsGraging=true;
        if (objs.Count == 1)
        {
            GameObject modelT = objs[0];
            if(modelT==null) return;
            startPos=modelT.transform.position;

            var pipePointComponent=modelT.GetComponent<PipePointComponent>();
            if(pipePointComponent){
                if(modelT.transform.parent)
                {
                    Debug.Log($"PipeSystemEditEventListener.TranslationGizmosStart_1 modelT:{modelT} parent:{modelT.transform.parent}");
                    Collider collider=modelT.transform.parent.GetComponent<Collider>();
                    if(collider)
                    {
                        collider.enabled=false;
                    }
                    else{

                    }

                    ExposeToEditor exposeToEditor=modelT.transform.parent.GetComponent<ExposeToEditor>();
                    if(exposeToEditor){
                        GameObject.DestroyImmediate(exposeToEditor);
                    }
                    else{
                        
                    }
                }
                else{
                    Debug.Log($"PipeSystemEditEventListener.TranslationGizmosStart_2 modelT:{modelT}");
                }
            }
            else{
                var pipeSystemComponent=modelT.GetComponent<PipeSystemComponent>();
                if(pipeSystemComponent){
                    Debug.Log($"PipeSystemEditEventListener.TranslationGizmosStart_3 modelT:{modelT}");
                }
                else{
                    Debug.Log($"PipeSystemEditEventListener.TranslationGizmosStart_4 modelT:{modelT}");
                }
            }
        }
        else{

        }
    }

    private void TranslationGizmosUpdate(Gizmo gizmo)
    {
        Debug.Log($"PipeSystemEditEventListener.TranslationGizmosUpdate 1 gizmo:{gizmo}  ");
        List<GameObject> controlObj = gizmo.ControlledObjects.ToList();
        OnGizmoUpdate(controlObj);
    }

    private void OnGizmoUpdate(List<GameObject> objs)
    {
        IsGraging=true;
        if (objs.Count == 1)
        {
            GameObject modelT = objs[0];
            if(modelT==null)return;
            // PipePointComponent pipeCom = modelT.GetComponent<PipePointComponent>();
            // if (pipeCom == null || pipeCom.point == null) return;
            // PipeSystemBuilder.Instance.RefreshPipe(pipeCom);
            var pos=modelT.transform.position;

            var pipePointComponent=modelT.GetComponent<PipePointComponent>();
            if(pipePointComponent && pipePointComponent.point!=null){
                pipePointComponent.RefreshPos(startPos,pos);
            }
            else{

                var pipeSystemComponent=modelT.GetComponent<PipeSystemComponent>();
                if(pipeSystemComponent==null && modelT.transform.childCount>0){
                    pipeSystemComponent=modelT.transform.GetChild(0).GetComponent<PipeSystemComponent>();
                }
                if(pipeSystemComponent){
                    //pipeSystemComponent.EditPos(startPos,endPos);
                    var posNew=pipeSystemComponent.GetNewPos(startPos,pos);
                    MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.SetTransformPos, posNew);
                    Debug.Log($"PipeSystemEditEventListener.OnGizmoUpdate_3 modelT:{modelT}");
                }
                else{
                    Debug.Log($"PipeSystemEditEventListener.OnGizmoUpdate_4 modelT:{modelT}");
                }
            }
        }
        else{
            Debug.Log($"PipeSystemEditEventListener.OnGizmoUpdate_5 ");
        }
    }

    /// <summary>
    /// �ƶ�����
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosEnd(Gizmo gizmo)
    {
        Debug.Log($"PipeSystemEditEventListener.TranslationGizmosEnd gizmo:{gizmo}");
        List<GameObject> objList = gizmo.ControlledObjects.ToList();
        GizemoDragEndEvent(objList);
    }

    private void GizemoDragEndEvent(List<GameObject> controlObj)
    {
        IsGraging=false;
        //ModifyDevPos(controlObj);
        if (controlObj.Count == 1)
        {
            GameObject modelT = controlObj[0];
            if(modelT==null)return;
            var pos=modelT.transform.position;
            //endPos=pos;
            // Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent 1 modelT:{modelT} parent:{modelT.transform.parent}");
            // PipePointComponent pipeCom = modelT.GetComponent<PipePointComponent>();
            // if (pipeCom == null || pipeCom.point == null) return;
            
            var pipePointComponent=modelT.GetComponent<PipePointComponent>();
            if(pipePointComponent && pipePointComponent.point!=null){
                Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent_1 point:{pipePointComponent.point}");
                //PipeSystemBuilder.Instance.RefreshPipe(pipeCom);
                
                pipePointComponent.EditPos(startPos,pos);
                if(modelT.transform.parent)
                {
                    Collider collider=modelT.transform.parent.GetComponent<Collider>();
                    collider.enabled=true;
                    Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent_2 modelT:{modelT} parent:{modelT.transform.parent}");
                }
            }
            else{
                var pipeSystemComponent=modelT.GetComponent<PipeSystemComponent>();
                if(pipeSystemComponent==null && modelT.transform.childCount>0){
                    pipeSystemComponent=modelT.transform.GetChild(0).GetComponent<PipeSystemComponent>();
                }
                if(pipeSystemComponent){
                    pipeSystemComponent.EditPos(startPos,pos);
                    MessageCenter.SendMsg(MsgType.PipeSystemEditPanelMsg.TypeName, MsgType.PipeSystemEditPanelMsg.SaveTransform, pipeSystemComponent.pipeSystem);
                    Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent_3 modelT:{modelT}");
                }
                else{
                    Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent_4 modelT:{modelT}");
                }
            }
        }
        else{
            Debug.Log($"PipeSystemEditEventListener.GizemoDragEndEvent_5");
        }
    }

    #endregion
    #region RotationGizmo
    /// <summary>
    /// ��ת��ʼ
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// ��ת����
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// ��ת����
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosEnd(Gizmo gizmo)
    {
        //ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region ScaleGizmo
    /// <summary>
    /// ���ſ�ʼ
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// ���Ÿ���
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// ���Ž���
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosEnd(Gizmo gizmo)
    {
        //ModifyDevPos(gizmo.ControlledObjects.ToList());
    }
    #endregion
}

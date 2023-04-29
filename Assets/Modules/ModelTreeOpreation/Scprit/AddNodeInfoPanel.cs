using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y_UIFramework;
using NavisPlugins.Infos;
using UnityEngine.EventSystems;

public class AddNodeInfoPanel : MonoBehaviour, IPointerClickHandler
{
    List<PropertyCategoryInfo> infoList;
    void Start()
    {
        
    }
    public void ShowInfo(List<PropertyCategoryInfo> infoList) 
    {
        UIManager.GetInstance().ShowUIPanel(typeof(SmartModelInfoPanel).Name);
        MessageCenter.SendMsg(MsgType.SmartModelInfoPanelMsg.TypeName, MsgType.SmartModelInfoPanelMsg.ShowInfo, infoList);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ShowInfo(infoList);
            //if (ModeTreeEditManager.IsEdit)
            //{
            //    //GetTreeNodeInfo();//获取选中的节点
            //    //SelectNodeFouse();//聚焦节点
            //    //UIManager.GetInstance().ShowUIPanel(typeof(ModelTreeOperatePanel).Name);//节点操作界面
            //    //SetOperationWindow();
            //    ShowInfo();
            //}
            //else
            //    UGUIMessageBox.Show("请右击节点给节点添加属性!");
        }
        //rightClick.Invoke();
    }
    void Update()
    {
        
    }
}

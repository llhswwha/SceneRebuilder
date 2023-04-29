using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class EditTreeNodeNameManager : MonoBehaviour
{
    public Button SureBtn;//确定按钮
    public Button CancelBtn;//取消按钮
    public Text text;
    public InputField NodeNameField;//节点名称输入框

    void Start()
    {
        SureBtn.onClick.AddListener(SureNodeName);
        CancelBtn.onClick.AddListener(CancelNodeName);
        NodeNameField.onEndEdit.AddListener(InputNodeName);
    }
    /// <summary>
    /// 确认修改
    /// </summary>
    public void SureNodeName()
    {
        if(text.text == "") UGUIMessageBox.Show("输入的节点名称不能为空!");
        NodeNameField.text = "";
        UIManager.GetInstance().CloseUIPanels(typeof(EditTreeNodeNamePanel).Name);
    }
    /// <summary>
    /// 取消修改
    /// </summary>
    public void CancelNodeName()
    {
        NodeNameField.text = "";
        UIManager.GetInstance().CloseUIPanels(typeof(EditTreeNodeNamePanel).Name);
    }
    /// <summary>
    /// 输入节点名字
    /// </summary>
    public void InputNodeName(string name)
    {
        if (name == "") return;
        NodeCacheManager.Instance.cacheNode.Item.Name = name;
    }
}

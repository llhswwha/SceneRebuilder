using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class AddInfoPanel : UIBasePanel
{
    public Text Title;
    public Button SureBtn;//确定按钮
    public Button CancelBtn;//取消按钮
    public InputField inputInfoName;//属性名称
    public InputField inputInfoValue;//属性名称 
    private void Awake()
    {
        CurrentUIType.UIPanels_Type = UIPanelType.Fixed;  //普通窗体
        CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.Normal;//普通，可以和其他窗体共存

        SureBtn.onClick.AddListener(SetInfo);
        CancelBtn.onClick.AddListener(CancelSetInfo);
    }
    private void OnEnable()
    {
        if (InfoOperationPanel.state)
            Title.text = "添加属性";
        else
            Title.text = "修改属性";
    }
    /// <summary>
    /// 设置属性的名称和值
    /// </summary>
    public void SetInfo()
    {
        Tips();
        if (InfoOperationPanel.state)
        {
            GameObject go = Instantiate(InfoCacheManager.Instance.CacheInfo.transform.parent.gameObject, InfoCacheManager.Instance.CacheInfo.transform.parent.parent);
            //DestroyImmediate(go.transform.GetChild(1).GetChild(0));
            go.transform.GetChild(1).GetComponent<InputField>().text = inputInfoValue.text;
            go.transform.GetChild(1).gameObject.AddComponent<NewInfo>();
            go.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = inputInfoName.text;
            go.transform.GetChild(1).GetComponent<InputField>().ActivateInputField();
            InfoEditManager.Instance.vb.value = -0.03f;
            //StartCoroutine(DelayTime(0.5f));
        }
        else
        {

            InfoCacheManager.Instance.CacheInfo.transform.GetComponent<InputField>().text = inputInfoValue.text;
            if(InfoCacheManager.Instance.CacheInfo.GetComponent<NewInfo>() != null)
            //Debug.LogError("InfoCacheManager.Instance.CacheInfo.transform.GetChild(3): " + InfoCacheManager.Instance.CacheInfo.transform.GetChild(3));
                InfoCacheManager.Instance.CacheInfo.transform.GetChild(4).GetComponent<Text>().text = inputInfoName.text;
            else
                InfoCacheManager.Instance.CacheInfo.transform.GetChild(3).GetComponent<Text>().text = inputInfoName.text;
        }
        
        
        ResetInputField();
        UIManager.GetInstance().CloseUIPanels(typeof(AddInfoPanel).Name);
    }
    /// <summary>
    /// 取消设置
    /// </summary>
    public void CancelSetInfo()
    {

        ResetInputField();
        UIManager.GetInstance().CloseUIPanels(typeof(AddInfoPanel).Name);
    }
    /// <summary>
    /// 延迟时间
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayTime(float times)
    {
        Debug.LogError("WaitForSeconds");
        yield return new WaitForSeconds(times);
        Debug.LogError("DelayTime");
        
    }
    /// <summary>
    /// 恢复输入框
    /// </summary>
    public void ResetInputField()
    {
        inputInfoName.text = "";
        inputInfoValue.text = "";
    }
    /// <summary>
    /// 提示
    /// </summary>
    public void Tips()
    {
        if (inputInfoName.text == "")
        {
            UGUIMessageBox.Show("属性名称不能为空!"); return;
        }
        if (inputInfoValue.text == "")
        {
            UGUIMessageBox.Show("属性值不能为空!"); return;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Y_UIFramework;

public class InfoOperationPanel : UIBasePanel
{
    public Button AddInfoBtn; 
    public Button EditBtn;
    public Button RemoveBtn;
    public static bool state = false;//修改还是新增

    private void Awake()
    {
        CurrentUIType.UIPanels_Type = UIPanelType.Fixed;  //普通窗体
        CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.ReverseChange;//普通，可以和其他窗体共存

        RegisterMsgListener(MsgType.ModelTreeOperatePanelMsg.TypeName,
        obj =>
        {
            
        });
        AddInfoBtn.onClick.AddListener(AddInfo);
        EditBtn.onClick.AddListener(EditInfo);
        RemoveBtn.onClick.AddListener(RemoveInfo);
    }
    public void AddInfo()
    {
        state = true;
        UIManager.GetInstance().ShowUIPanel(typeof(AddInfoPanel).Name);
        //InfoCacheManager.Instance.CacheInfo.transform.parent
        
        //InfoEditManager.Instance.vb.value = 0;
    }
    public void EditInfo()
    {
        state = false;
        UIManager.GetInstance().ShowUIPanel(typeof(AddInfoPanel).Name);
        //InfoCacheManager.Instance.CacheInfo.transform.GetComponent<InputField>().interactable = true;
        //InfoCacheManager.Instance.CacheInfo.transform.GetComponent<InputField>().ActivateInputField();
        //InfoCacheManager.Instance.CacheInfo.transform.GetComponent<InputField>().onEndEdit.AddListener(delegate { InfoCacheManager.Instance.CacheInfo.transform.GetComponent<InputField>().interactable = false; });
    }
    public void RemoveInfo() 
    {
        Debug.LogError("this.transform.gameObject: " + this.transform.gameObject.name);
        GameObject go = InfoCacheManager.Instance.CacheInfo;
        DestroyImmediate(go);
        InfoCacheManager.Instance.CacheInfo = null;
        //this.transform.gameObject
        //DestroyImmediate(this.transform.gameObject);   
    }
    /// <summary>
    /// 延迟时间
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayTime(float times)
    {
        yield return new WaitForSeconds(times);
        InfoEditManager.Instance.vb.value = 0;
    }
    /// <summary>
    /// 设置属性是否可编辑
    /// </summary>
    //public void SetIsEdit()
    //{
    //    InputField[] list = Content.GetComponentsInChildren<InputField>();
    //    for (int i = 0; i < list.Length; i++)
    //    {
    //        list[i].interactable = isEdit;
    //    }
    //}
    /// <summary>
    /// 关闭操作界面
    /// </summary>
    public void HideWindow()
    {
        this.transform.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            HideWindow();
        }
    }
}

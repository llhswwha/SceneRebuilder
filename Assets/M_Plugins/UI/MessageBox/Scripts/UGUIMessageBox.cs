using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Modules.Context;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 提示框界面
/// </summary>
public class UGUIMessageBox : MonoBehaviour,IMessageBox {

    public static UGUIMessageBox Instance;
    public GameObject window;//窗口
    public Button sureBtn;//确定按钮
    public Button cancelBtn;//取消按钮
    public Button closeBtn;//关闭按钮

    [HideInInspector]
    public Text sureBtnTxt;//确定按钮名称
    [HideInInspector]
    public Text cancelBtnTxt;//取消按钮名称

    public Text txtMsg;//信息框

    private Action SureCallAction;//确定回调
    private Action CancelCallAction;//取消回调
    private Action CloseCallAction;//关闭回调

    void Awake()
    {
        Instance = this;
        Unity.Modules.Context.AppContext.MessageBox = this;
    }

    // Use this for initialization
    void Start () {
        
        sureBtn.onClick.AddListener(SureBtn_OnClick);
        cancelBtn.onClick.AddListener(CancelBtn_OnClick);
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
        sureBtnTxt = sureBtn.GetComponentInChildren<Text>(true);
        cancelBtnTxt = cancelBtn.GetComponentInChildren<Text>(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="msg"></param>
    public static void Show(string msg)
    {
        if (Instance == null)
        {
            Debug.LogError("UGUIMessageBox.Show Instance == null");
            return;
        }
        Instance.ShowMessage(msg);
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMessage(string msg)
    {

        txtMsg.text = msg;
        sureBtnTxt.text = "确定";
        cancelBtnTxt.text = "取消";
        Instance.SureCallAction = null;
        Instance.CancelCallAction = null;
        Instance.CloseCallAction = null;
        SetWindowActive(true);
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="msg"></param>
    public static void Show(string msg,Action sureCallActionT, Action cancelCallActionT)
    {
        if (Instance == null)
        {
            Debug.LogError("UGUIMessageBox.Show Instance == null");
            return;
        }
        Instance.txtMsg.text = msg;
        Instance.SureCallAction = sureCallActionT;
        Instance.CancelCallAction = cancelCallActionT;
        Instance.sureBtnTxt.text = "确定";
        Instance.cancelBtnTxt.text = "取消";
        Instance.SetWindowActive(true);
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="msg"></param>
    public static void Show(string msg,string sureBtnName, string cancelBtnName, Action sureCallActionT, Action cancelCallActionT, Action closeCallActionT)
    {
        if (Instance == null)
        {
            Debug.LogError("UGUIMessageBox.Show Instance == null");
            return;
        }
        Instance.txtMsg.text = msg;
        Instance.SureCallAction = sureCallActionT;
        Instance.CancelCallAction = cancelCallActionT;
        Instance.CloseCallAction = closeCallActionT;
        Instance.sureBtnTxt.text = sureBtnName;
        Instance.cancelBtnTxt.text = cancelBtnName;
        Instance.SetWindowActive(true);
    }


    /// <summary>
    /// 隐藏
    /// </summary>
    public static void Hide()
    {
        if (Instance == null)
        {
            Debug.LogError("UGUIMessageBox.Hide Instance == null");
            return;
        }
        Instance.SetWindowActive(false);
        Instance.SureCallAction = null;
        Instance.CancelCallAction = null;
        Instance.CloseCallAction = null;
    }

    /// <summary>
    /// 显示隐藏窗口
    /// </summary>
    /// <param name="isActive"></param>
    public void SetWindowActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 确定按钮触发事件
    /// </summary>
    public void SureBtn_OnClick()
    {
        //Hide();
        Instance.SetWindowActive(false);

        if (SureCallAction!=null)
        {
            SureCallAction();
        }
        //Instance.SureCallAction = null;
        //Instance.CancelCallAction = null;
        //Instance.CloseCallAction = null;

    }

    /// <summary>
    /// 取消按钮触发事件
    /// </summary>
    public void CancelBtn_OnClick()
    {
        if (CancelCallAction != null)
        {
            CancelCallAction();
        }
        Hide();
    }

    /// <summary>
    /// 关闭按钮触发事件
    /// </summary>
    public void CloseBtn_OnClick()
    {
        if (CloseCallAction != null)
        {
            CloseCallAction();
        }
        Hide();
    }

}

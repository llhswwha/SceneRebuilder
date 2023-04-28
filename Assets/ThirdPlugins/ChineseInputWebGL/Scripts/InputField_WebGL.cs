using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class InputField_WebGL : MonoBehaviour
{
    [HideInInspector]
    public InputField inputField;
    [HideInInspector]
    public RectTransform RectT;

    int selectStartIndex;//选择文本框中文字的起始位置
    int selectEndIndex;//选择文本框中文字的结束位置
    bool isFocus;
    bool isDoFocus;
    void Start()
    {
        inputField = GetComponent<InputField>();
        InputWebGLManage.Instance.Register(gameObject.GetInstanceID(),this);
        RectT = inputField.textComponent.GetComponent<RectTransform>();
        //添加unity输入框回调
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onEndEdit.AddListener(OnEndEdit);
        //添加获得焦点回调
        EventTrigger trigger = inputField.gameObject.GetComponent<EventTrigger>();
        if (null == trigger)
            trigger = inputField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry e = new EventTrigger.Entry();
        e.eventID = EventTriggerType.PointerDown;
        e.callback.AddListener((data) => { OnFocus((PointerEventData)data); });
        trigger.triggers.Add(e);

        DoFocus();
    }

    void Update()
    {
        if (isFocus)
        {
            if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
            {
                if (selectStartIndex != inputField.selectionAnchorPosition || selectEndIndex != inputField.selectionFocusPosition)
                {
                    //print("OnSelectRangeChanged1");
                    //print("StartStr:" + selectStartIndex + "|" + "EndStr:" + selectEndIndex + "|" + "AnchorPos:" + inputField.selectionAnchorPosition + "|" + "FocusPos:" + inputField.selectionFocusPosition);
                    OnSelectRangeChanged(inputField.selectionAnchorPosition, inputField.selectionFocusPosition);
                }
            }
            else
            {
                if (selectStartIndex != inputField.selectionFocusPosition || selectEndIndex != inputField.selectionAnchorPosition)
                {
                    //print("OnSelectRangeChanged2");
                    //print("StartStr:" + selectStartIndex + "|" + "EndStr:" + selectEndIndex + "|" + "AnchorPos:" + inputField.selectionAnchorPosition + "|" + "FocusPos:" + inputField.selectionFocusPosition);
                    OnSelectRangeChanged(inputField.selectionFocusPosition, inputField.selectionAnchorPosition);
                }
            }
        }
    }

    /// <summary>
    /// 克隆完InputField后，马上可以输入（正常文本框，点击聚焦后才可以输入）
    /// 目前用于二维标注的文本标注
    /// </summary>
    /// <param name="b"></param>
    public void SetisDoFocus(bool b)
    {
        isDoFocus = b;
    }

    /// <summary>
    /// 代码触发
    /// </summary>
    public void DoFocus()
    {
        if (isDoFocus)
        {
            isDoFocus = false;
            ExecuteEvents.Execute<IPointerDownHandler>(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);//执行这步才会触发InputField_WebGL的OnFocus方法，网页端才能输入
        }
    }

    #region ugui回调
    private void OnValueChanged(string arg0)
    {

    }
    private void OnFocus(PointerEventData pointerEventData)
    {
        if (isFocus==false)
        {
            SelectAll();
            selectStartIndex = inputField.selectionAnchorPosition;
            selectEndIndex = inputField.selectionFocusPosition;
        }
        isFocus = true;
        #if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
        #endif
        //print("InputShowOP");
        //print("StartStr:" + selectStartIndex + "|" + "EndStr:" + selectEndIndex + "|" + "AnchorPos:" + inputField.selectionAnchorPosition + "|" + "FocusPos:" + inputField.selectionFocusPosition);
        InputShowOP(inputField.text, inputField.selectionAnchorPosition + "|" + inputField.selectionFocusPosition);
    }

    public void OnSelectRangeChanged(int startIndex, int endIndex)
    {
        selectStartIndex = startIndex;
        selectEndIndex = endIndex;
        InputShowOP(inputField.text, selectStartIndex + "|" + selectEndIndex);
    }

    public void InputShowOP(string text, string indexStr)
    {
        string log = "";
        //获取在Canvas下的位置
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float casRecWidth=canvasRect.rect.width;
        float casRecHeight=canvasRect.rect.height;
        int mode = scaler==null?0:(int)scaler.uiScaleMode;
        float X = 0;
        float Y = 0;
        GetPosXY(canvas, transform, ref X, ref Y);
        float width = RectT.rect.width;
        float height = RectT.rect.height;       
        string inputRectStr =mode+"|"+ X + "|" + Y + "|" + width + "|" + height+"|"+casRecWidth+"|"+casRecHeight;
        string fontsize = inputField.textComponent.fontSize.ToString();
        InputWebGLManage.Instance.InputShow(gameObject.GetInstanceID().ToString(), text, fontsize, indexStr, inputRectStr);
    }

    private void OnEndEdit(string str)
    {
        isFocus = false;
    }

    /// <summary>
    /// 获取在Canvas下的位置
    /// </summary>
    public void GetPosXY(Canvas canvas, Transform tran, ref float x, ref float y)
    {
        x += tran.localPosition.x;
        y += tran.localPosition.y;
        if (canvas.transform == tran.parent)
        {
            return;
        }
        else
        {
            GetPosXY(canvas, tran.parent, ref x, ref y);
        }
    }

#endregion

#region WebGL回调
    public void OnInputText(string text,string selectStartIndexStr, string selectEndIndexStr)
    {
        inputField.text = text;

        try
        {
            int selectStartIndexStrT= int.Parse(selectStartIndexStr);
            int selectEndIndexStrT = int.Parse(selectEndIndexStr);
            inputField.selectionAnchorPosition = selectStartIndexStrT;
            inputField.selectionFocusPosition = selectEndIndexStrT;
            selectStartIndex = selectStartIndexStrT;
            selectEndIndex = selectEndIndexStrT;
            //print("SET:" + selectStartIndex + "|" + selectEndIndex);
        }
        catch
        {
            inputField.selectionAnchorPosition = inputField.text.Length;
            inputField.selectionFocusPosition = inputField.text.Length;
        }
        inputField.Select();
        inputField.ForceLabelUpdate();
    }
    public void OnInputEnd()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        //WebGLInput.captureAllKeyboardInput = true;
#endif
        ////inputField.DeactivateInputField();
    }

    public void SelectAll()
    {
        inputField.selectionAnchorPosition = 0;
        inputField.selectionFocusPosition = inputField.text.Length;
        inputField.Select();
        inputField.ForceLabelUpdate();
    }

#endregion

}

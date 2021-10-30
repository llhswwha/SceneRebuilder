using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class DoubleClickEventTrigger_u3d : MonoBehaviour {

    public delegate void VoidDelegate();
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;
    public delegate void VoidDelegate2(GameObject o);
    public VoidDelegate2 on_Click2;
    public VoidDelegate2 on_DoubleClick2;


    public static GameObject ClickedObject;
    public static GameObject DoubleClickedObject;

    protected float TimeInterval = 350;
    private DateTime startTime;
    //private bool firstTime = true;
    private int clickTimes = 0;//点击次数

    //public bool isOverUI = false;

    static public DoubleClickEventTrigger_u3d Get(GameObject go)
    {
        DoubleClickEventTrigger_u3d listener = go.GetComponent<DoubleClickEventTrigger_u3d>();
        if (listener == null) listener = go.AddComponent<DoubleClickEventTrigger_u3d>();
        return listener;
    }

    void Update()
    {
        if (clickTimes == 1)
        {
            bool result = CheckTime();
            if (!result)//时间超出时间范围触发单击事件
            {               
                ClearFlag();
                OnClick_u();
            }
        }
    }

    void OnMouseDown()
    {
        if (IsPointerOverUGUI()) return;
        //GameObjectHelper.CurrentObject = gameObject;

        clickTimes++;
        //print (DateTime.Now+" mousedown:"+transform.parent+"\\"+gameObject);
        if (clickTimes == 1)
        {            
            startTime = DateTime.Now;
            ClickedObject = gameObject;
        }
        else if (clickTimes == 2)
        {
            ClearFlag();
            bool result = CheckTime();
            if (!result) return;
            //print("DoubleClickEvent1:" + time + " < " + limit + "=" + result);
            OnDoubleClick_u();
            DoubleClickedObject = gameObject;

        }
        else if (clickTimes > 2)
        {
            Debug.LogError("clickTimes > 2");
        }
    }

    /// <summary>
    /// 点击第二下的时间是否超出限定时间范围
    /// </summary>
    /// <returns></returns>
    private bool CheckTime()
    {
        TimeSpan time = DateTime.Now - startTime;
        TimeSpan limit = new TimeSpan(0, 0, 0, 0, (int)TimeInterval);        
        bool result = time < limit;
        return result;
    }

    protected void ClearFlag()
    {
        clickTimes = 0;
        //print("ClearFlag_clickTimes");
    }

    //protected IEnumerator ClearFlag()
    //{
    //    yield return new WaitForSeconds(TimeInterval / 1000);
    //    firstTime = true;
    //    //print(DateTime.Now + " ClearFlag:");
    //}

    protected void OnClick_u()
    {
        print("OnClick_u:"+this);
        if (onClick != null) onClick();
        if (on_Click2 != null) on_Click2(gameObject);
        
    }

    protected void OnDoubleClick_u()
    {
        print("OnDoubleClick_u:" + this);
        if (onDoubleClick != null) onDoubleClick();
        if (on_DoubleClick2 != null) on_DoubleClick2(gameObject);
    }

    /// <summary>
    /// 判断鼠标点击3D游戏物体是，是否被UGUI界面遮挡
    /// </summary>
    public bool IsPointerOverUGUI()
    {
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("鼠标当前触摸在UGUI上");
            //isOverUI = true;
            return true;
        }
        else
        {
            //isOverUI = false;
            //Debug.Log("鼠标当前没有触摸在UGUI上");
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// UI界面元素查找类，因为多人开发时，场景上的UI元素绑定有时候会丢失，对于这种情况，采用路径查找
/// </summary>
public static class UIFinder
{
    /// <summary>
    /// 找到巡维开始按钮
    /// </summary>
    /// <returns></returns>
    public static GameObject FindCruiseStartButton()
    {
        return GameObject.Find(Names.CruiseButton);
    }

    /// <summary>
    /// 找到巡维停止按钮
    /// </summary>
    /// <returns></returns>
    public static GameObject FindCruiseStopButton()
    {
        GameObject toolBarRight = GameObject.Find(Names.ToolBarRight);
        if (toolBarRight == null) return null;
        return toolBarRight.transform.Find(Names.BreakCruiseButton).gameObject;
    }
}

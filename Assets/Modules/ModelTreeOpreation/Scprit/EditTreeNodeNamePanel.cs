using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Y_UIFramework;
using UnityEngine.UI;

public class EditTreeNodeNamePanel : UIBasePanel
{
    
    private void Awake()
    {
        CurrentUIType.UIPanels_Type = UIPanelType.Fixed;  //普通窗体
        CurrentUIType.UIPanel_LucencyType = UIPanelTransparentType.Pentrate;//透明，可以穿透
        CurrentUIType.UIPanels_ShowMode = UIPanelShowMode.Normal;//普通，可以和其他窗体共存
    } 
}

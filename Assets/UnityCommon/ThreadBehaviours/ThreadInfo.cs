using System;
using UnityEngine;
using System.Collections;

public class ThreadInfo  {

    public string Name { get; set; }

    public Action MainAction { get; set; }

    public Action UiAction { get; set; }

    public ThreadInfo(Action mainAction, Action uiAction, string name)
    {
        Name = name;
        MainAction = mainAction;
        UiAction = uiAction;
    }
}

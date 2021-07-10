using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class BaseEditor : Editor
{
    protected GUIStyle contentStyle;
    protected int buttonWidth=110;

    protected void NewButton(string text,int width,bool isEnable,Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent==null);
        if (GUILayout.Button(text,contentStyle,GUILayout.Width(width)))
        {
            if(clickEvent!=null){
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();

        // if (GUILayout.Button(text))
        // {
        //     if(clickEvent!=null){
        //         clickEvent();
        //     }
        // }
    }
}

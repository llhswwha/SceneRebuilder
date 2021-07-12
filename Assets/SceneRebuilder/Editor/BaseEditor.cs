using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
public class BaseEditor<T> : Editor where T:class
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

    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        T item = target as T;
        OnToolLayout(item);

        base.OnInspectorGUI();
    }

    public virtual void OnToolLayout(T item)
    {

    }
}

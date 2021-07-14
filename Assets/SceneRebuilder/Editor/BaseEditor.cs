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

    public void NewButton(string text, int width, bool isEnable, Action clickEvent)
    {
        NewButton(text, width, isEnable, contentStyle, clickEvent);
    }

    public static void NewButton(string text,int width,bool isEnable, GUIStyle style, Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent == null);
        if (GUILayout.Button(text, style, GUILayout.Width(width)))
        {
            if (clickEvent != null)
            {
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();

        //if (GUILayout.Button(text))
        //{
            
        //}
    }

    protected T targetT ;

    public virtual void OnEnable()
    {
        targetT = target as T;
    }

    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;
        
        OnToolLayout(targetT);

        base.OnInspectorGUI();
    }

    public virtual void OnToolLayout(T item)
    {

    }
}

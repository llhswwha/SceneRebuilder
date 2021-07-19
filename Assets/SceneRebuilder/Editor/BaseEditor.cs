using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;

[CanEditMultipleObjects]
public class BaseEditor<T> : Editor where T:class
{
    protected FoldoutEditorArg toolbarArg = new FoldoutEditorArg();
    protected FoldoutEditorArg propertyArg = new FoldoutEditorArg();

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

        toolbarArg.caption = "ToolBar";
        toolbarArg.isEnabled = true;
        toolbarArg.isToggle = false;
        toolbarArg.isExpanded = true;

        propertyArg.caption = "Properties";
        propertyArg.isEnabled = true;
        propertyArg.isToggle = false;
    }

    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        EditorUIUtils.SetupStyles();

        EditorUIUtils.ToggleFoldout(toolbarArg, null,null);
        if (toolbarArg.isExpanded && toolbarArg.isEnabled)
        {
            OnToolLayout(targetT);
        }

        EditorUIUtils.ToggleFoldout(propertyArg, null, null);
        if (propertyArg.isExpanded)
        {
            base.OnInspectorGUI();
        }
    }

    public virtual void OnToolLayout(T item)
    {

    }
}

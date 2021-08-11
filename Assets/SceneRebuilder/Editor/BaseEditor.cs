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

    public static bool NewButton(string text,int width,bool isEnable, GUIStyle style, Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent == null);
        bool isClick = GUILayout.Button(text, style, GUILayout.Width(width));
        if (isClick)
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
        return isClick;
    }

    protected T targetT ;

    public virtual void OnEnable()
    {
        targetT = target as T;

        toolbarArg.caption = "ToolBar";
        toolbarArg.isEnabled = true;
        toolbarArg.isToggle = false;
        toolbarArg.isExpanded = true;
        toolbarArg.separator = false;
        toolbarArg.background = true;
        toolbarArg.bold = true;

        propertyArg.caption = "Properties";
        propertyArg.isEnabled = true;
        propertyArg.isToggle = false;
        propertyArg.separator = true;
        propertyArg.background = true;
        propertyArg.bold = true;
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

    public static TO ObjectField<TO>(TO obj) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), false) as TO;
    }
    
}

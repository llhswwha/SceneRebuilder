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
        NewEnabledButton(text, width, isEnable, contentStyle, clickEvent);
    }

    public static bool NewEnabledButton(string text,int width,bool isEnable, GUIStyle style, Action clickEvent)
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


    public TO ObjectField<TO>(TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public TO ObjectField<TO>(string label,TO obj) where TO : UnityEngine.Object
    {
        //GUILayout.Label(label);
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
    }
}

public static class BaseEditorHelper
{
    public static TO ObjectField<TO>(TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }
    public static TO ObjectField<TO>(string label, TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public static TO ObjectField<TO>(string label, float width,TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label,GUILayout.Width(width)))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public static TO ObjectField<TO>(string label,int width1, TO obj, int width2=0) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label,GUILayout.Width(width1)))
        {
            EditorHelper.SelectObject(obj);
        }
        if (width2 == 0)
        {
            return EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
        }
        else
        {
            return EditorGUILayout.ObjectField(obj, typeof(TO), true, GUILayout.Width(width2)) as TO;
        }
        
    }
}

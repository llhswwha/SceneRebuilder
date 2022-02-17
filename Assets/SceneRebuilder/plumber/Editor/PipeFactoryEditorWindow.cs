using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PipeFactoryEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/PipeFactory")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<PipeFactoryEditorWindow>(800, 700, "PipeFactory");
    }

    public PipeFactory target;

    public void Init()
    {
        target = PipeFactory.Instance;
    }

    private void OnGUI()
    {
        if (target == null)
        {
            Init();
        }
        PipeFactoryEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

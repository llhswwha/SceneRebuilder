using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelUpdateManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/ModelUpdate")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<ModelUpdateManagerEditorWindow>(500, 500, "ModelUpdate");
    }

    public ModelUpdateManager target;

    public void Init()
    {
        target = ModelUpdateManager.Instance;
    }

    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }

        ModelUpdateManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

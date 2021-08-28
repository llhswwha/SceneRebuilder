using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshAlignmentManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/MeshAlignment")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<MeshAlignmentManagerEditorWindow>(500, 150, "MeshAlignment");
    }

    public MeshAlignmentManager target;

    public void Init()
    {
        target = MeshAlignmentManager.Instance;
    }
    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);

        MeshAlignmentManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

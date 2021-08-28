using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshCombinerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/MeshCombiner")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<MeshCombinerEditorWindow>(500, 150, "MeshCombiner");
    }

    public MeshCombiner target;

    public void Init()
    {
        target = MeshCombiner.Instance;
    }

    private void OnGUI()
    {
        MeshCombinerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

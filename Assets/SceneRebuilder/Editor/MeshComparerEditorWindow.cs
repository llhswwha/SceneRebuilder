using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshComparerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/MeshComparer")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<MeshComparerEditorWindow>(550, 230, "MeshComparer");
    }

    public MeshComparer target;

    public void Init()
    {
        target = MeshComparer.Instance;
    }

    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);

        MeshComparerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshComparerEditorWindow : EditorWindow
{
    [MenuItem("Window/Tools/MeshComparer")]
    public static void ShowWindow()
    {
        MeshComparerEditorWindow window = (MeshComparerEditorWindow)EditorWindow.GetWindowWithRect(typeof(MeshComparerEditorWindow), new Rect(0, 0, 550, 230), true, "MeshComparer");
        window.Show();
        window.Init();
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

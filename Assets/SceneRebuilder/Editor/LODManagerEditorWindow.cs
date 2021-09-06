using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LODManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/LODManager")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<LODManagerEditorWindow>(700, 550, "LODManager");
    }

    public LODManager target;

    public void Init()
    {
        target = LODManager.Instance;
    }
    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }
        LODManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}


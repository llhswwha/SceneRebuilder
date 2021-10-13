using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloorBoxManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/FloorBoxManager")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<FloorBoxManagerEditorWindow>(550, 230, "FloorBoxManager");
    }

    public FloorBoxManager target;

    public void Init()
    {
        target = FloorBoxManager.Instance;
    }

    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }
        FloorBoxManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

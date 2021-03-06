using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AreaTreeManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/TreeManager")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<AreaTreeManagerEditorWindow>(400,200, "TreeManager");
    }

    public AreaTreeManager target;

    public void Init()
    {
        target = AreaTreeManager.Instance;
    }
    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);

        AreaTreeManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AreaTreeManagerEditorWindow : EditorWindow
{
    [MenuItem("Window/Tools/TreeManager")]
    public static void ShowWindow()
    {
        var window = (AreaTreeManagerEditorWindow)EditorWindow.GetWindowWithRect(typeof(AreaTreeManagerEditorWindow), new Rect(0, 0, 400, 200), true, "TreeManager");
        window.Show();
        window.Init();
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

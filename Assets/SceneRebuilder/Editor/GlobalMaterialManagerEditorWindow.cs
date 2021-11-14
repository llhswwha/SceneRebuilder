using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GlobalMaterialManagerEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/MaterialManager")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<GlobalMaterialManagerEditorWindow>(700, 550, "MaterialManager");
    }

    public GlobalMaterialManager target;

    public void Init()
    {
        target = GlobalMaterialManager.Instance;
    }
    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }
        GlobalMaterialManagerEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

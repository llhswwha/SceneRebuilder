using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavisModelRootListEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/NavisModelRootList")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<NavisModelRootListEditorWindow>(500, 300, "NavisModelRootList");
    }

    public NavisModelRootList target;

    public void Init()
    {
        target = NavisModelRootList.Instance;
    }


    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }
        NavisModelRootListEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

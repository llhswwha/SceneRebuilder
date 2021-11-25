using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InitNavisFileInfoByModelWindow :  EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/InitNavisFileInfo")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<InitNavisFileInfoByModelWindow>(500, 300, "InitNavisFileInfo");
    }

    public InitNavisFileInfoByModel target;

    public void Init()
    {
        target = InitNavisFileInfoByModel.Instance;
    }


    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);
        if (target == null)
        {
            Init();
        }
        InitNavisFileInfoByModelEditor.DrawUI(target);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }

}

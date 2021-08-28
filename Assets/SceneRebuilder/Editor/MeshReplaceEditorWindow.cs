using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshReplaceEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/MeshReplace")]
    public static void ShowWindow()
    {
        //BaseEditorWindow.ShowWindow<MeshReplaceEditorWindow>(500, 200, "MeshReplace");
        BaseEditorWindow.ShowWindow<MeshReplaceEditorWindow>(500, 100 + MeshReplace.Instance.Items.Count * 50, "MeshReplace");
    }

    public MeshReplace target;

    public void Init()
    {
        target = MeshReplace.Instance;
    }
    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);

        MeshReplaceEditor.DrawUI(target);
        //var pos = this.position;
        //pos.height = 100+ target.Items.Count*100;
        //this.position = pos;

        this.minSize = new Vector2(500, 100 + MeshReplace.Instance.Items.Count * 50);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }

}

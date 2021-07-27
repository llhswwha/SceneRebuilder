using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformNode))]
public class TransformNodeEditor : BaseEditor<TransformNode>
{
    public override void OnToolLayout(TransformNode item)
    {
        base.OnToolLayout(item);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Init"))
        {
            item.Init();
        }
        if (GUILayout.Button("Reset"))
        {
            item.Reset();
        }
        if (GUILayout.Button("Recover"))
        {
            item.Recover();
        }
        if (GUILayout.Button("Select"))
        {
            EditorHelper.SelectObject(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("SplitByMaterials"))
        //{
        //    item.SplitByMaterials();
        //}
        //EditorGUILayout.EndHorizontal();
    }
}

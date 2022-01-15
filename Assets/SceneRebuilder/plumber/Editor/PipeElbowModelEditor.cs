using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeElbowModel))]
public class PipeElbowModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeElbowModel targetT = target as PipeElbowModel;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetModelInfo"))
        {
            targetT.GetModelInfo();
        }
        if (GUILayout.Button("RendererModel"))
        {
            targetT.RendererModel();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowTriangles"))
        {
            targetT.ShowTriangles();
        }
        if (GUILayout.Button("ShowSharedPoints"))
        {
            targetT.ShowSharedPoints();
        }
        if (GUILayout.Button("ShowKeyPoints"))
        {
            targetT.ShowKeyPoints();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("ClearGo"))
        {
            targetT.ClearGo();
        }

        base.OnInspectorGUI();
    }
}

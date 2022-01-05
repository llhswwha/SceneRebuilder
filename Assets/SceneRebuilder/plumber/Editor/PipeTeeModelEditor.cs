using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeTeeModel))]
public class PipeTeeModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeTeeModel targetT = target as PipeTeeModel;
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
        if (GUILayout.Button("GetPipeRadius"))
        {
            targetT.GetPipeRadius();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }

        base.OnInspectorGUI();
    }
}

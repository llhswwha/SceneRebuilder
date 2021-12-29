using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeElbowModel))]
public class PipeElbowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeElbowModel targetT = target as PipeElbowModel;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetElbowInfo"))
        {
            targetT.GetModelInfo();
        }
        if (GUILayout.Button("CreateElbow"))
        {
            targetT.CreateElbow();
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

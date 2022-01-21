using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeWeldoletModel))]
public class PipeWeldoletModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeWeldoletModel targetT = target as PipeWeldoletModel;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetModelInfo"))
        {
            targetT.GetModelInfo();
        }
        if (GUILayout.Button("RendererModel"))
        {
            targetT.RendererModel();
        }
        if (GUILayout.Button("Replace"))
        {
            targetT.ReplaceOld();
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

        base.OnInspectorGUI();
    }
}

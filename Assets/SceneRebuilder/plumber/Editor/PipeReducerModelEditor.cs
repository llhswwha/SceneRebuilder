using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeReducerModel))]
public class PipeReducerModelEditor 
    //: PipeLineModelEditor
    //:PipeElbowModelEditor
    : Editor
{
    public override void OnInspectorGUI()
    {
        PipeReducerModel targetT = target as PipeReducerModel;
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

        base.OnInspectorGUI();
    }
}

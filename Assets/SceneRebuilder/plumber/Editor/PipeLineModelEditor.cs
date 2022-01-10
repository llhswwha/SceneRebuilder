using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeLineModel))]
public class PipeLineModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeLineModel targetT = target as PipeLineModel;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ResultGo"))
        {
            EditorHelper.SelectObject(targetT.ResultGo);
        }
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        GUILayout.EndHorizontal();

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
        if (GUILayout.Button("GetModelInfo(Job)"))
        {
            targetT.GetModelInfoJob();
        }
        if (GUILayout.Button("CreateWeld"))
        {
            targetT.CreateWeld();
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

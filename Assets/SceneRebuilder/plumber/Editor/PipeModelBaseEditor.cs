using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PipeModelBaseEditor : Editor
{
    
    public void DrawBaseModelToolBar(PipeModelBase targetT)
    {
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
            targetT.DebugShowTriangles();
        }
        if (GUILayout.Button("ShowSharedPoints"))
        {
            targetT.DebugShowSharedPoints();
        }
        if (GUILayout.Button("ShowKeyPoints"))
        {
            targetT.DebugShowKeyPoints();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("ClearGo"))
        {
            targetT.ClearGo();
        }
        if (GUILayout.Button("ResultGo"))
        {
            EditorHelper.SelectObject(targetT.ResultGo);
        }
        GUILayout.EndHorizontal();
    }
}

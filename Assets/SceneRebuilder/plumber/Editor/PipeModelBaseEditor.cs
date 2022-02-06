using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PipeFactoryEditor;

public class PipeModelBaseEditor : Editor
{
    public PipeGenerateArgEditorValues generateArgEditorValues;

    public void DrawBaseModelToolBar(PipeModelBase targetT)
    {
        GUILayout.BeginHorizontal();

        if (generateArgEditorValues == null)
        {
            generateArgEditorValues = new PipeGenerateArgEditorValues();
        }
        DrawGenerateArg(targetT.generateArg, generateArgEditorValues);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetInfo"))
        {
            targetT.GetModelInfo();
        }
        if (GUILayout.Button("Renderer"))
        {
            targetT.RendererModel();
        }
        //if (GUILayout.Button("RendererLOD3"))
        //{
        //    targetT.RendererModelLOD(3);
        //}
        if (GUILayout.Button("RendererLOD4"))
        {
            targetT.RendererModelLOD(4);
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

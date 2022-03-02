using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoxMeshModel))]
public class BoxMeshModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoxMeshModel targetT = target as BoxMeshModel;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearInfo"))
        {
            targetT.ClearModelInfo();
        }

        if (GUILayout.Button("GetInfo"))
        {
            targetT.GetModelInfo();
        }

        if (GUILayout.Button("GetInfo(Job)"))
        {
            targetT.GetModelInfo_Job();
        }

        if (GUILayout.Button("RendererModel"))
        {
            targetT.RendererModel();
        }

        if (GUILayout.Button("ReplaceModel"))
        {
            //targetT.ReplaceModel();

            targetT.ReplaceOld();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("ClearDebugInfo"))
        {
            targetT.ClearDebugInfoGos();
        }
        if (GUILayout.Button("ClearGo"))
        {
            targetT.ClearGo();
        }
        if (GUILayout.Button("CheckResult"))
        {
            targetT.CheckResult();
        }


        //if (GUILayout.Button("ResultGo"))
        //{
        //    EditorHelper.SelectObject(targetT.ResultGo);
        //}
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

        if (GUILayout.Button("ShowNormalPoints"))
        {
            targetT.DebugShowNormalPoints();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

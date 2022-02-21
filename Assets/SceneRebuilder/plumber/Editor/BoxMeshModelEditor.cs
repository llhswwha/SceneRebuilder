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
            targetT.ReplaceModel();
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
        //if (GUILayout.Button("ClearGo"))
        //{
        //    targetT.ClearGo();
        //}
        //if (GUILayout.Button("ResultGo"))
        //{
        //    EditorHelper.SelectObject(targetT.ResultGo);
        //}
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

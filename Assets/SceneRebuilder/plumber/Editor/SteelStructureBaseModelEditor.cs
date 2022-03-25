using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SteelStructureBaseModel))]
public class SteelStructureBaseModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SteelStructureBaseModel targetT = target as SteelStructureBaseModel;
        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("ShowOBB"))
        //{
        //    targetT.ShowOBB();
        //}
        //if (GUILayout.Button("GetInfo"))
        //{
        //    targetT.GetModelInfo();
        //}
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        //if (GUILayout.Button("ClearInfo"))
        //{
        //    targetT.ClearModelInfo();
        //}

        if (GUILayout.Button("GetInfo"))
        {
            targetT.GetModelInfo();
        }

        if (GUILayout.Button("GetInfo(Job)"))
        {
            targetT.GetModelInfo_Job();
        }

        //if (GUILayout.Button("RendererModel"))
        //{
        //    targetT.RendererModel();
        //}

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

[CustomEditor(typeof(SteelStructureModelH))]
public class SteelStructureModelHEditor : SteelStructureBaseModelEditor
{
}

[CustomEditor(typeof(SteelStructureModelC))]
public class SteelStructureModelCEditor : SteelStructureBaseModelEditor
{
}

[CustomEditor(typeof(SteelStructureModelL))]
public class SteelStructureModelLEditor : SteelStructureBaseModelEditor
{
}

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
            targetT.isShowDebug = true;
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

        DrawSubToolBar();

        base.OnInspectorGUI();
    }

    public virtual void DrawSubToolBar()
    {

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
    public override void DrawSubToolBar()
    {
        SteelStructureModelL targetT = target as SteelStructureModelL;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearData"))
        {
            targetT.ClearData();
        }
        if (GUILayout.Button("Debug_True"))
        {
            targetT.DebugGetModelInfoDebug_True();
        }
        if (GUILayout.Button("Debug_False"))
        {
            targetT.DebugGetModelInfoDebug_False(); 
        }
        GUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(SteelStructureModelCornerBox))]
public class SteelStructureModelCornerBoxEditor : SteelStructureBaseModelEditor
{
    public override void DrawSubToolBar()
    {
        SteelStructureModelCornerBox targetT = target as SteelStructureModelCornerBox;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearData"))
        {
            targetT.ClearData();
        }
        //if (GUILayout.Button("Debug_True"))
        //{
        //    targetT.DebugGetModelInfoDebug_True();
        //}
        //if (GUILayout.Button("Debug_False"))
        //{
        //    targetT.DebugGetModelInfoDebug_False();
        //}
        GUILayout.EndHorizontal();
    }
}

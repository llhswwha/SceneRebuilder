using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeWeldModel))]
public class PipeWeldModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeWeldModel targetT = target as PipeWeldModel;

        //base.DrawBaseModelToolBar(targetT);

        //GUILayout.BeginHorizontal();

        //if (generateArgEditorValues == null)
        //{
        //    generateArgEditorValues = new PipeGenerateArgEditorValues();
        //}
        //DrawGenerateArg(targetT.generateArg, generateArgEditorValues);
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("GetInfo"))
        //{
        //    targetT.GetModelInfo();
        //}
        //GUILayout.Label("RenderOnStart", GUILayout.Width(90));
        //targetT.IsRendererOnStart = EditorGUILayout.Toggle(targetT.IsRendererOnStart, GUILayout.Width(15));
        //if (GUILayout.Button("Renderer"))
        //{
        //    targetT.RendererModel();
        //}
        ////if (GUILayout.Button("RendererLOD3"))
        ////{
        ////    targetT.RendererModelLOD(3);
        ////}
        //if (GUILayout.Button("RendererLOD4"))
        //{
        //    targetT.RendererModelLOD(4);
        //}

        //if (GUILayout.Button("Replace"))
        //{
        //    targetT.ReplaceOld();
        //}
        //if (GUILayout.Button("RemoveComponents"))
        //{
        //    targetT.RemoveAllComponents();
        //    //targetT.gameObject.SetActive(true);
        //}
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("ShowTriangles"))
        //{
        //    targetT.DebugShowTriangles();
        //}
        //if (GUILayout.Button("ShowSharedPoints"))
        //{
        //    targetT.DebugShowSharedPoints();
        //}
        //if (GUILayout.Button("ShowKeyPoints"))
        //{
        //    targetT.DebugShowKeyPoints();
        //}
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("ClearChildren"))
        //{
        //    targetT.ClearChildren();
        //}
        //if (GUILayout.Button("ClearDebugInfo"))
        //{
        //    targetT.ClearDebugInfoGos();
        //}
        //if (GUILayout.Button("ClearGo"))
        //{
        //    targetT.ClearGo();
        //}
        //if (GUILayout.Button("ResultGo"))
        //{
        //    EditorHelper.SelectObject(targetT.ResultGo);
        //}
        //GUILayout.EndHorizontal();

        if (GUILayout.Button("Renderer"))
        {
            targetT.RendererModel();
        }
        if (GUILayout.Button("ClearGo"))
        {
            targetT.ClearGo();
        }
        if (GUILayout.Button("ResultGo"))
        {
            EditorHelper.SelectObject(targetT.ResultGo);
        }

        base.OnInspectorGUI();
    }
}

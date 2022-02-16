using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeLineModel))]
public class PipeLineModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeLineModel targetT = target as PipeLineModel;
        base.DrawBaseModelToolBar(targetT);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("GetModelInfo(Job)"))
        {
            targetT.GetModelInfoJob();
        }
        if (GUILayout.Button("CreateWeld"))
        {
            targetT.CreateWeld();
        }
        if (GUILayout.Button("CreateBoxLine"))
        {
            targetT.CreateBoxLine();
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

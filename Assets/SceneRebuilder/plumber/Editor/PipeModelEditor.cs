using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeLineModel))]
public class PipeModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeLineModel targetT = target as PipeLineModel;
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("CreatePipe"))
        {
            targetT.CreatePipe();
        }
        if (GUILayout.Button("CreateWeld"))
        {
            targetT.CreateWeld();
        }
        base.OnInspectorGUI();
    }
}

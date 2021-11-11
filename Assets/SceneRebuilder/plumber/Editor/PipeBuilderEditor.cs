using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBuilder))]
public class PipeBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeBuilder targetT = target as PipeBuilder;
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("CreateEachPipes"))
        {
            targetT.CreateEachPipes();
        }
        if (GUILayout.Button("CreateOnePipe"))
        {
            targetT.CreateOnePipe();
        }
        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}
        base.OnInspectorGUI();
    }
}
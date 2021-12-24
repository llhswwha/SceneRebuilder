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
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateEachPipes"))
        {
            targetT.CreateEachPipes();
        }
        if (GUILayout.Button("CreateOnePipe1"))
        {
            targetT.UseOnlyEndPoint = false;
            targetT.CreateOnePipe();
        }
        if (GUILayout.Button("CreateOnePipe2"))
        {
            targetT.UseOnlyEndPoint = true;
            targetT.CreateOnePipe();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("TestCreateOnePipe"))
        {
            targetT.TestCreateOnePipe();
        }
        GUILayout.EndHorizontal();

        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}
        base.OnInspectorGUI();
    }
}

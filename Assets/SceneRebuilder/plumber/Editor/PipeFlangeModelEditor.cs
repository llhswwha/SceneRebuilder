using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFlangeModel))]
public class PipeFlangeModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeFlangeModel targetT = target as PipeFlangeModel;
        base.DrawBaseModelToolBar(targetT);

        GUILayout.BeginHorizontal();

        //if (GUILayout.Button("CreateUnitPrefab"))
        //{
        //    targetT.CreatePipeLineUnitPrefab();
        //}

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

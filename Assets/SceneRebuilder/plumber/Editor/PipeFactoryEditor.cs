using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFactory))]
public class PipeFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeFactory targetT = target as PipeFactory;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetPipeParts"))
        {
            targetT.GetPipeParts();
        }
        if (GUILayout.Button("OnlyShowPipe"))
        {
            targetT.OnlyShowPipe();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowAll"))
        {
            targetT.ShowAll();
        }
        if (GUILayout.Button("HidAll"))
        {
            targetT.HidAll();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("GeneratePipe"))
        {
            targetT.GeneratePipe();
        }
        base.OnInspectorGUI();
    }
}

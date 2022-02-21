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
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }

        if (GUILayout.Button("GetInfo"))
        {
            targetT.GetModelInfo();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

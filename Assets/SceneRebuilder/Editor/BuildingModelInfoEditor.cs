using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BuildingModelInfo info = target as BuildingModelInfo;

        if(GUILayout.Button("GetInfo"))
        {
            info.InitInOut();
        }
        if (GUILayout.Button("Combine"))
        {
            info.CreateTreesBSEx();
        }
        if(GUILayout.Button("CreateScene"))
        {
            info.EditorCreateNodeScenes();
        }
        if (GUILayout.Button("LoadScene"))
        {
            info.EditorCreateNodeScenes();
        }
        if (GUILayout.Button("UnloadScene"))
        {
            info.EditorCreateNodeScenes();
        }
    }
}

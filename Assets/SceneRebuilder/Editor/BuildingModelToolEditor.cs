using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingModelTool))]
public class BuildingModelToolEditor : BaseEditor
{
    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        BuildingModelTool tool = target as BuildingModelTool;

        if(GUILayout.Button("SetDoorSetting",contentStyle))
        {
            tool.SetDoorSetting();
        }
        if(GUILayout.Button("RemoveGeometryGroup",contentStyle))
        {
            tool.RemoveGeometryGroup();
        }
        if(GUILayout.Button("RemoveEmptyObjects",contentStyle))
        {
            tool.RemoveEmptyObjects();
        }

        base.OnInspectorGUI();
    }
}

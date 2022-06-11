using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingModelTool))]
public class BuildingModelToolEditor : BaseEditor<BuildingModelTool>
{
    //public override void OnInspectorGUI()
    //{
    //    contentStyle = new GUIStyle(EditorStyles.miniButton);
    //    contentStyle.alignment = TextAnchor.MiddleLeft;

    //    BuildingModelTool tool = target as BuildingModelTool;

    //    if(GUILayout.Button("SetDoorSetting",contentStyle))
    //    {
    //        tool.SetDoorSetting();
    //    }
    //    if(GUILayout.Button("RemoveGeometryGroup",contentStyle))
    //    {
    //        tool.RemoveGeometryGroup();
    //    }
    //    if(GUILayout.Button("RemoveEmptyObjects",contentStyle))
    //    {
    //        tool.RemoveEmptyObjects();
    //    }

    //    base.OnInspectorGUI();
    //}

    public override void OnToolLayout(BuildingModelTool item)
    {
        base.OnToolLayout(item);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SetDoorSetting", contentStyle))
        {
            item.SetDoorSetting();
        }
        if (GUILayout.Button("RemoveGeometryGroup", contentStyle))
        {
            item.RemoveGeometryGroup();
        }
        if (GUILayout.Button("RemoveEmptyObjects", contentStyle))
        {
            item.RemoveEmptyObjects();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("FindDoorsInBounds95", contentStyle))
        {
            item.FindDoorsInBounds95();
        }
        if (GUILayout.Button("FindDoorsInBounds90", contentStyle))
        {
            item.FindDoorsInBounds90();
        }
        if (GUILayout.Button("FindDoorsInBounds85", contentStyle))
        {
            item.FindDoorsInBounds85();
        }
        GUILayout.EndHorizontal();
    }
}

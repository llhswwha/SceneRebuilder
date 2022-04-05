using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingScenesLoadManager))]
public class BuildingScenesLoadManagerEditor : BaseEditor<BuildingScenesLoadManager>
{
    public override void OnToolLayout(BuildingScenesLoadManager item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InitXml"))
        {
            item.InitSettingByScene();
            item.SaveXml();
        }
        if (GUILayout.Button("SaveXml"))
        {
            item.SaveXml();
        }
        if (GUILayout.Button("LoadXml"))
        {
            item.LoadSettingXml();
        }
        EditorGUILayout.EndHorizontal();
    }
}

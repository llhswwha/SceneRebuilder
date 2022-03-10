using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaTreeManager))]
public class AreaTreeManagerEditor : BaseFoldoutEditor<AreaTreeManager>
{
    public static void DrawUI(AreaTreeManager item)
    {
        item.MaxMeshVertexCount=EditorGUILayout.IntField("MaxMeshVertexCount", item.MaxMeshVertexCount);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearDictionary"))
        {
            AreaTreeHelper.ClearDict();
        }
        if (GUILayout.Button("CreateDictionary"))
        {
            item.CreateDictionary();
        }
        EditorGUILayout.EndHorizontal();

        var setting = item.nodeSetting;
        setting.MinLevel = EditorGUILayout.IntField("MinLevel", setting.MinLevel);
        setting.MaxLevel = EditorGUILayout.IntField("MaxLevel", setting.MaxLevel);
        setting.MaxRenderCount = EditorGUILayout.IntField("MaxRenderCount", setting.MaxRenderCount);
        setting.MinRenderCount = EditorGUILayout.IntField("MinRenderCount", setting.MinRenderCount);
        setting.MaxVertexCount = EditorGUILayout.IntField("MaxVertexCount", setting.MaxVertexCount);
    }

    public override void OnToolLayout(AreaTreeManager item)
    {
        base.OnToolLayout(item);
        AreaTreeManagerEditor.DrawUI(item);


    }
}

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
    }

    public override void OnToolLayout(AreaTreeManager item)
    {
        base.OnToolLayout(item);
        AreaTreeManagerEditor.DrawUI(item);


    }
}

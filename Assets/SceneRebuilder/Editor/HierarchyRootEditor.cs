using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HierarchyRoot))]
public class HierarchyRootEditor : BaseEditor<HierarchyRoot>
{
    public override void OnToolLayout(HierarchyRoot item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            item.LoadXml();
        }
        if (GUILayout.Button("Save"))
        {
            item.SaveXml();
        }
        if (GUILayout.Button("Check"))
        {
            item.Check();
        }
        EditorGUILayout.EndHorizontal();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombineMesh))]
public class CombineMeshEditor : BaseEditor<CombineMesh>
{
    public override void OnToolLayout(CombineMesh item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Combine"))
        {
            item.Combine();
        }
        if (GUILayout.Button("Split"))
        {
            item.Split();
        }
        EditorGUILayout.EndHorizontal();
    }
}

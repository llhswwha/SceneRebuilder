using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : BaseEditor<MeshCombiner>
{
    public override void OnToolLayout(MeshCombiner item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Combine"))
        {
            item.Combine();
        }
        if (GUILayout.Button("CombineEx"))
        {
            item.CombineEx();
        }
        if (GUILayout.Button("CombineByMaterial"))
        {
            item.CombineByMaterial();
        }
        //if (GUILayout.Button("Split"))
        //{
        //    item.Split();
        //}
        EditorGUILayout.EndHorizontal();
    }
}

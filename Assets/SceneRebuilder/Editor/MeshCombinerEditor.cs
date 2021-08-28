using System;
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
        MeshCombinerEditor.DrawUI(item);
        if (GUILayout.Button("Window"))
        {
            MeshCombinerEditorWindow.ShowWindow();
        }
    }

    internal static void DrawUI(MeshCombiner item)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Source", GUILayout.Width(60));
        var newSource = BaseEditorHelper.ObjectField(item.sourceRoot);
        item.SetSourceRoot(newSource);

        item.sourceType = (MeshCombineSourceType)EditorGUILayout.EnumPopup(item.sourceType, GUILayout.Width(80));

        MeshCombinerSetting.Instance.IsDestroySource = GUILayout.Toggle(MeshCombinerSetting.Instance.IsDestroySource, "IsDestroy", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Combine"))
        {
            item.CombineEx();
        }
        if (GUILayout.Button("CombineByMat"))
        {
            item.CombineByMaterial();
        }
        if (GUILayout.Button("Clear"))
        {
            item.ClearResult();
        }
        if (GUILayout.Button("Save"))
        {
            item.SaveResult();
        }
        if (GUILayout.Button("Destroy"))
        {
            item.DestroySource();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Split"))
        {
            MeshCombineHelper.SplitByMaterials(item.sourceRoot, false);
        }
        if (GUILayout.Button("Combine"))
        {
            MeshCombineHelper.CombineEx(new MeshCombineArg(item.sourceRoot), MeshCombineMode.OneMesh);
        }
        EditorGUILayout.EndHorizontal();
    }
}

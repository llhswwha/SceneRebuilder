using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshReplace))]
public class MeshReplaceEditor : BaseEditor<MeshReplace>
{
    public override void OnToolLayout(MeshReplace item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Replace"))
        {
            item.Replace();
        }
        if (GUILayout.Button("Clear"))
        {
            item.ClearNewGos();
        }
        if (GUILayout.Button("Apply"))
        {
            item.ApplyNewGos();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectPrefabs"))
        {
            item.SelectPrefabs();
        }
        if (GUILayout.Button("SelectTargets"))
        {
            item.SelectTargets();
        }
        if (GUILayout.Button("SelectNew"))
        {
            item.SelectNewGos();
        }
        EditorGUILayout.EndHorizontal();

        EditorUIUtils.SetupStyles();
        foreach (var subItem in item.Items)
        {
            if (subItem == null) continue;
            
            EditorUIUtils.Separator(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefab", GUILayout.Width(50));

            if(subItem.prefab!=null)
                if (GUILayout.Button(subItem.prefab.name))
                {
                    EditorHelper.SelectObject(subItem.prefab);
                }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Targets", GUILayout.Width(50));
            foreach (var target in subItem.targetList)
            {
                if (target == null) continue;
                if (GUILayout.Button(target.name))
                {
                    EditorHelper.SelectObject(target);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        
    }
}

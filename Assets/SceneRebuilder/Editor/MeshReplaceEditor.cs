using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
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
        MeshReplaceEditor.DrawUI(item);
        if (GUILayout.Button("Window"))
        {
            MeshReplaceEditorWindow.ShowWindow();
        }
    }

    internal static void DrawUI(MeshReplace item)
    {
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

        EditorUIUtils.Separator(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Items:({item.Items.Count})", GUILayout.Width(60));
        if (GUILayout.Button("+"))
        {
            item.Items.Add(new MeshReplaceItem());
            //MeshReplaceEditorWindow.ShowWindow();
        }
        if (GUILayout.Button("-"))
        {
            item.Items.RemoveAt(item.Items.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        foreach (var subItem in item.Items)
        {
            if (subItem == null) continue;

            EditorUIUtils.Separator(1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefab", GUILayout.Width(60));
            if (GUILayout.Button("+|", GUILayout.Width(20)))
            {
                Debug.Log("+" + Selection.activeObject);
                subItem.prefab = Selection.activeObject as GameObject;
            }
            if (subItem.prefab != null)
            {
                if (GUILayout.Button(subItem.prefab.name))
                {
                    EditorHelper.SelectObject(subItem.prefab);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    Debug.Log("-" + subItem.prefab);
                    subItem.prefab = null;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Target({subItem.targetList.Count})", GUILayout.Width(60));

            if (GUILayout.Button("+|", GUILayout.Width(20)))
            {
                Debug.Log("+" + Selection.activeObject);
                //subItem.targetList.Add(Selection.activeObject as GameObject);
                
                foreach(GameObject obj in Selection.objects)
                {
                    subItem.targetList.Add(obj);
                }
            }

            for (int i = 0; i < subItem.targetList.Count; i++)
            {
                GameObject target = subItem.targetList[i];
                if (target == null)
                {
                    subItem.targetList.RemoveAt(i);
                    i--;
                    continue;
                }
                if (GUILayout.Button(target.name))
                {
                    EditorHelper.SelectObject(target);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    Debug.Log("-" + target);
                    subItem.targetList.RemoveAt(i);
                    i--;
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}

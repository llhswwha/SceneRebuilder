using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorBoxManager))]
public class FloorBoxManagerEditor : BaseFoldoutEditor<FloorBoxManager>
{
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnToolLayout(FloorBoxManager item)
    {
        if (item == null) return;
        base.OnToolLayout(item);
        DrawUI(item);
        if (GUILayout.Button("Window"))
        {
            FloorBoxManagerEditorWindow.ShowWindow();
        }
    }

    internal static void DrawUI(FloorBoxManager item)
    {
        EditorGUILayout.BeginHorizontal();
        item.Sources = BaseEditorHelper.ObjectField(item.Sources);
        
        item.IsIn = GUILayout.Toggle(item.IsIn, "IsIn");
        if (GUILayout.Button("SetParent"))
        {
            item.SetParent();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.IsChangeParent = GUILayout.Toggle(item.IsChangeParent, "IsChangeParent");
        if (GUILayout.Button("SetToFloorChild"))
        {
            item.SetToFloorChild();
        }
        if (GUILayout.Button("SetToFloorRenderers"))
        {
            item.SetToFloorRenderers();
        }
        EditorGUILayout.EndHorizontal();

        DrawList(item.Floors, "Floors");

        DrawList(item.List0, "List0");
        DrawList(item.List1, "List1");
        DrawList(item.List2, "List2");
    }

    private static void DrawList(List<TransformFloorParent> list1,string listName)
    {
        FoldoutEditorArg foldoutArg1 = FoldoutEditorArgBuffer.GetGlobalEditorArg(list1, new FoldoutEditorArg(true, true, true, true, true));
        foldoutArg1.caption = $"{listName}({list1.Count})";
        EditorUIUtils.ToggleFoldout(foldoutArg1, (ar) =>
        {
            FoldoutEditorArgBuffer.InitEditorArg(list1);

        }, () =>
        {
        });
        if (foldoutArg1.isExpanded && foldoutArg1.isEnabled)
        {
            foldoutArg1.DrawPageToolbar(list1.Count);
            for (int i = foldoutArg1.GetStartId(); i < list1.Count && i < foldoutArg1.GetEndId(); i++)
            {
                var node = list1[i];
                if (node.go == null) continue;
                var arg = FoldoutEditorArgBuffer.editorArgs[node];
                string title = $"[{i + 1:00}] {node.go.name}";
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, title, $"[{node.GetFloors()}]", false, false, false, node.go.gameObject);
            }
        }
    }

    private static void DrawList(List<GameObject> list1, string listName)
    {
        FoldoutEditorArg foldoutArg1 = FoldoutEditorArgBuffer.GetGlobalEditorArg(list1, new FoldoutEditorArg(true, true, true, true, true));
        foldoutArg1.caption = $"{listName}({list1.Count})";
        EditorUIUtils.ToggleFoldout(foldoutArg1, (ar) =>
        {
            FoldoutEditorArgBuffer.InitEditorArg(list1,new FoldoutEditorArg(true,false,true));

        }, () =>
        {
        });
        if (foldoutArg1.isExpanded && foldoutArg1.isEnabled)
        {
            foldoutArg1.DrawPageToolbar(list1.Count);
            for (int i = foldoutArg1.GetStartId(); i < list1.Count && i < foldoutArg1.GetEndId(); i++)
            {
                var node = list1[i];
                var arg = FoldoutEditorArgBuffer.editorArgs[node];
                string title = $"[{i + 1:00}] {node.name}";
                EditorUIUtils.ObjectFoldout(arg.isExpanded, title, "", false, false, false, node);

                //arg.isExpanded = GUILayout.Toggle(arg.isExpanded, $"[{i + 1:00}] {node.name}");
                //node.SetActive(arg.isEnabled);
            }
        }
    }
}

using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorBoxManager))]
public class FloorBoxManagerEditor : BaseFoldoutEditor<FloorBoxManager>
{
    public static FoldoutEditorArg FloorsArg = new FoldoutEditorArg(true, false, true, true, true);

    public static FoldoutEditorArg List00Arg = new FoldoutEditorArg(true, true, true, true, true);
    public static FoldoutEditorArg List01Arg = new FoldoutEditorArg(true, true, true, true, true);
    public static FoldoutEditorArg List2Arg = new FoldoutEditorArg(true, true, true, true, true);
    public static FoldoutEditorArg List3Arg = new FoldoutEditorArg(true, true, true, true, true);

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
        item.OnlySetOneFloor = GUILayout.Toggle(item.OnlySetOneFloor, "OneFloor");
        item.IsDebug = GUILayout.Toggle(item.IsDebug, "Debug");

        if (GUILayout.Button("SetParent"))
        {
            item.SetParent();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.IsChangeParent = GUILayout.Toggle(item.IsChangeParent, "IsChangeParent");
        item.MinIntersectPercent = EditorGUILayout.FloatField(item.MinIntersectPercent, "MinIntersect");
        if (GUILayout.Button("SetChild", GUILayout.Width(100)))
        {
            item.SetToFloorChild();
        }
        if (GUILayout.Button("SetRenderers", GUILayout.Width(100)))
        {
            item.SetToFloorRenderers();
        }
        EditorGUILayout.EndHorizontal();

        DrawList(FloorsArg, item.Floors, "Floors", () =>
         {
             if (GUILayout.Button("Update "))
             {
                 item.GetFloors();
             }
         });

        DrawList(List00Arg, item.Sources, item.List00, "List00", null);
        DrawList(List01Arg, item.Sources, item.List01, "List01", null);
        DrawList(List2Arg, item.Sources, item.List1, "List1", null);
        DrawList(List3Arg, item.Sources, item.List2, "List2", null);
    }

    private static void DrawList(FoldoutEditorArg listArg, GameObject root, List<TransformFloorParent> list1, string listName, Action toolbarAction)
    {
        FoldoutEditorArg foldoutArg1 = FoldoutEditorArgBuffer.GetGlobalEditorArg(list1, listArg);
        foldoutArg1.caption = $"{listName}({list1.Count})";
        EditorUIUtils.ToggleFoldout(foldoutArg1, (ar) =>
        {
            FoldoutEditorArgBuffer.InitEditorArg(list1);

        }, () =>
        {
            if (toolbarAction != null)
            {
                toolbarAction();
            }
        });
        if (foldoutArg1.isExpanded && foldoutArg1.isEnabled)
        {
            foldoutArg1.DrawPageToolbar(list1.Count);
            for (int i = foldoutArg1.GetStartId(); i < list1.Count && i < foldoutArg1.GetEndId(); i++)
            {
                TransformFloorParent node = list1[i];
                if (node.go == null) continue;
                var arg = FoldoutEditorArgBuffer.editorArgs[node];
                string path = node.go.transform.GetPath(root.transform,">",3);
                //string title = $"[{i + 1:00}] {node.go.transform.parent.name} > {node.go.name}";
                string title = $"[{i + 1:00}] {path}";
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, title, $"[{node.GetFloors()}]", false, false, false, node.go.gameObject);
            }
        }
    }

    private static void DrawList(FoldoutEditorArg listArg, List<GameObject> list1, string listName, Action toolbarAction)
    {
        FoldoutEditorArg foldoutArg1 = FoldoutEditorArgBuffer.GetGlobalEditorArg(list1, listArg);
        foldoutArg1.caption = $"{listName}({list1.Count})";
        EditorUIUtils.ToggleFoldout(foldoutArg1, (ar) =>
        {
            FoldoutEditorArgBuffer.InitEditorArg(list1, new FoldoutEditorArg(true, false, true));

        }, () =>
        {
            if (toolbarAction != null)
            {
                toolbarAction();
            }
        });
        if (foldoutArg1.isExpanded && foldoutArg1.isEnabled)
        {
            foldoutArg1.DrawPageToolbar(list1.Count);
            for (int i = foldoutArg1.GetStartId(); i < list1.Count && i < foldoutArg1.GetEndId(); i++)
            {
                GameObject node = list1[i];
                var arg = FoldoutEditorArgBuffer.editorArgs[node];
                string title = $"[{i + 1:00}] {node.transform.parent.name} > {node.name}";
                EditorUIUtils.ObjectFoldout(arg.isExpanded, title, "", false, false, false, node);

                //arg.isExpanded = GUILayout.Toggle(arg.isExpanded, $"[{i + 1:00}] {node.name}");
                //node.SetActive(arg.isEnabled);
            }
        }
    }
}

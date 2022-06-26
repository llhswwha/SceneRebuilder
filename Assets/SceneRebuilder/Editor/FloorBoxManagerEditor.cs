using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorBoxManager))]
public class FloorBoxManagerEditor : BaseFoldoutEditor<FloorBoxManager>
{
    public static FloorBoxFoldoutEditorArg FloorsArg = new FloorBoxFoldoutEditorArg(true, false, true, true, true);
    public static FloorBoxFoldoutEditorArg BuildingsArg = new FloorBoxFoldoutEditorArg(true, false, true, true, true);

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

        DrawFloorList(item,BuildingsArg, item.Buildings, "Buildings", () =>
        {
            BuildingsArg.OnlyShowInBox = GUILayout.Toggle(BuildingsArg.OnlyShowInBox, "OnlyInBox");
            BuildingsArg.OnlyShowHaveItems = GUILayout.Toggle(BuildingsArg.OnlyShowHaveItems, "OnlyHaveItems");
            if (GUILayout.Button("ShowNotInFloors", GUILayout.Width(150)))
            {
                item.ShowNotInFloorsInBuildings();
            }
            if (GUILayout.Button("Update",GUILayout.Width(100)))
            {
                item.GetFloors();
            }
        });

        DrawFloorList(item, FloorsArg, item.Floors, "Floors", () =>
         {
             FloorsArg.OnlyShowInBox = GUILayout.Toggle(FloorsArg.OnlyShowInBox, "OnlyInBox");
             FloorsArg.OnlyShowHaveItems = GUILayout.Toggle(FloorsArg.OnlyShowHaveItems, "OnlyHaveItems");
             if (GUILayout.Button("Update", GUILayout.Width(100)))
             {
                 item.GetFloors();
             }
         });

        DrawObjectList(item, List00Arg, item.Sources, item.List00, "List00", null);
        DrawObjectList(item, List01Arg, item.Sources, item.List01, "List01", null);
        DrawObjectList(item, List2Arg, item.Sources, item.List1, "List1", null);
        DrawObjectList(item, List3Arg, item.Sources, item.List2, "List2", null);
    }

    private static void DrawObjectList(FloorBoxManager boxMananger, FoldoutEditorArg listArg, GameObject root, List<TransformFloorParent> list1, string listName, Action toolbarAction)
    {
        FoldoutEditorArg foldoutArg1 = FoldoutEditorArgBuffer.GetGlobalEditorArg(list1, listArg);
        foldoutArg1.caption = $"{listName}({list1.Count})";
        EditorUIUtils.ToggleFoldout(foldoutArg1, (ar) =>
        {
            FoldoutEditorArgBuffer.InitEditorArg(list1);

        }, () =>
        {
            //if (toolbarAction != null)
            //{
            //    toolbarAction();
            //}

            if (GUILayout.Button("SelectAll", GUILayout.Width(100)))
            {
                List<GameObject> objs = new List<GameObject>();
                foreach(var item in list1)
                {
                    objs.Add(item.go.gameObject);
                }
                EditorHelper.SelectObjects(objs);
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
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, title, "", false, false, false, node.go.gameObject,()=>
                {
                    if (node.floors.Count > 0)
                    {
                        for (int i = 0; i < node.floors.Count; i++)
                        {
                            IntersectInfo floor = node.floors[i];
                            if (GUILayout.Button($"{floor.name}({floor.percent:P1})", GUILayout.Width(120)))
                            {
                                EditorHelper.SelectObject(floor.IntersectGo.gameObject);
                            }
                        }
                        if (GUILayout.Button("Move", GUILayout.Width(50)))
                        {
                            //IntersectInfo floor = node.floors[0];
                            boxMananger.SetParent(node);
                            EditorHelper.SelectObject(node.go);
                        }
                    }
                    
                });
            }
        }
    }

    private static void DrawFloorList(FloorBoxManager boxMananger,FloorBoxFoldoutEditorArg listArg, List<ModelContainerBox> list1, string listName, Action toolbarAction)
    {
        float minPercet = 0;
        if (listArg.OnlyShowInBox)
        {
            minPercet = boxMananger.MinIntersectPercent;
        }
        if (listArg.OnlyShowHaveItems)
        {
            list1= list1.FindAll(i => i.GetCount(minPercet) > 0);
        }

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
                ModelContainerBox node = list1[i];
                if (node == null)
                {
                    Debug.LogError("node == null");
                    continue;
                }
                if (node.transform == null)
                {
                    Debug.LogError("node.transform == null");
                    continue;
                }
                if (node.transform.parent == null)
                {
                    Debug.LogError("node.transform.parent == null");
                    continue;
                }
                //if (node.Items == null)
                //{
                //    Debug.LogError("node.Items == null");
                //    continue;
                //}

                string title1 = $"[{i + 1:00}] {node.transform.parent.name} > {node.name} ({node.GetCount(minPercet)})";
                FoldoutEditorArg itemArg = FoldoutEditorArgBuffer.GetEditorArg(node, new FoldoutEditorArg(true, false, true));
                itemArg.caption = title1;

                itemArg.level = foldoutArg1.level+1;

                //if (node.Count > 0)
                {
                    List<IntersectInfo> subList = node.GetItemList(minPercet);

                    EditorUIUtils.ToggleFoldout(itemArg, (ar) =>
                    {
                        FoldoutEditorArgBuffer.InitEditorArg(subList, new FoldoutEditorArg(true, false, true));

                    }, () =>
                    {
                        if (GUILayout.Button(">", GUILayout.Width(30)))
                        {
                            EditorHelper.SelectObject(node.go);
                        }
                        if (GUILayout.Button("SelectModels",GUILayout.Width(100)))
                        {
                            List<GameObject> objs = new List<GameObject>();
                            foreach(var item in subList)
                            {
                                objs.Add(item.IntersectGo.gameObject);
                            }
                            EditorHelper.SelectObjects(objs);
                        }
                    });
                    if (itemArg.isExpanded && itemArg.isEnabled)
                    {
                        itemArg.DrawPageToolbar(subList.Count);
                        for (int j = itemArg.GetStartId(); j < subList.Count && j < itemArg.GetEndId(); j++)
                        {
                            IntersectInfo intersectInfo = subList[j];
                            FoldoutEditorArg subItemArg = FoldoutEditorArgBuffer.editorArgs[intersectInfo];
                            string title2 = $"[{j + 1:00}] {intersectInfo.name} ({intersectInfo.percent:P1})";
                            subItemArg.caption = title2;
                            subItemArg.level = itemArg.level + 1;
                            //EditorUIUtils.ObjectFoldout(subItemArg.isExpanded, title2, "", false, false, false, intersectInfo.IntersectGo.gameObject);
                            EditorUIUtils.ObjectFoldout(subItemArg, intersectInfo.IntersectGo.gameObject);

                            //arg.isExpanded = GUILayout.Toggle(arg.isExpanded, $"[{i + 1:00}] {node.name}");
                            //node.SetActive(arg.isEnabled);
                        }
                    }
                }
                //else
                //{
                //    EditorUIUtils.ObjectFoldout(itemArg.isExpanded, title1, "", false, false, false, node.go);
                //}
            }
        }
    }
}

public class FloorBoxFoldoutEditorArg: FoldoutEditorArg
{
    public bool OnlyShowHaveItems = false;

    public bool OnlyShowInBox = false;

    public FloorBoxFoldoutEditorArg():base(true, false, true, true, true)
    {
    }

    public FloorBoxFoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle, bool separator, bool background) : base(isEnabled, isExpanded, isToggle, separator, background)
    {
    }
}

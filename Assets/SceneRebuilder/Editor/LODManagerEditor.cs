using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODManager))]
public class LODManagerEditor : BaseFoldoutEditor<LODManager>
{
    FoldoutEditorArg<LODTwoRenderers> twoListArg = new FoldoutEditorArg<LODTwoRenderers>();

    FoldoutEditorArg lodGroupListArg = new FoldoutEditorArg();

    private string searchKey = "";

    public override void OnEnable()
    {
        base.OnEnable();
        //twoListArg.Items = targetT.twoList;
    }

    public override void OnToolLayout(LODManager lodManager)
    {
        base.OnToolLayout(lodManager);

        //if (GUILayout.Button("CheckLODPositions"))
        //{
        //    item.CheckLODPositions();
        //}
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InActive"))
        {
            lodManager.SetLODActive(false);
        }
        if (GUILayout.Button("Active"))
        {
            lodManager.SetLODActive(true);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("SetRenderersLODInfo"))
        {
            lodManager.SetRenderersLODInfo();
        }

        if (GUILayout.Button("UniformLOD"))
        {
            lodManager.UniformLOD();
        }
        EditorUIUtils.Separator(5);

        if (GUILayout.Button("GetLODDetail"))
        {
            string detail=lodManager.GetRuntimeLODDetail(true);
            Debug.Log($"lod detail:{detail}");
        }
        DrawLODGroupList(lodGroupListArg, lodManager);


        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ZeroDistance:");
        lodManager.zeroDistance = EditorGUILayout.FloatField(lodManager.zeroDistance);
        lodManager.DoCreateGroup = GUILayout.Toggle(lodManager.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Group:");
        lodManager.GroupRoot = EditorGUILayout.ObjectField(lodManager.GroupRoot, typeof(GameObject)) as GameObject;
        GUILayout.Label("LOD:");
        lodManager.LODnRoot = EditorGUILayout.ObjectField(lodManager.LODnRoot, typeof(GameObject)) as GameObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("AppendLod1"))
        {
            lodManager.AppendLod1ToGroup();
        }
        if (GUILayout.Button("AppendLod2"))
        {
            lodManager.AppendLod2ToGroup();
        }
        if (GUILayout.Button("AppendLod3"))
        {
            lodManager.AppendLod3ToGroup();
        }
        if (GUILayout.Button("SetColor"))
        {
            lodManager.SetAppendLod3Color();
        }
        EditorGUILayout.EndHorizontal();

        twoListArg.caption = $"TwoObject List";
        EditorUIUtils.ToggleFoldout(twoListArg, arg =>
        {
            var list = targetT.twoList;
            if (!string.IsNullOrEmpty(searchKey))
            {
                list = list.Where(i => i.renderer_lod1.name.Contains(searchKey)).ToList();
            }
            twoListArg.Items = list;
            arg.caption = $"TwoObject List ({list.Count}) ({lodManager.LODRendererCount0})({lodManager.LODRendererCount1})";
            //arg.info = $"{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
            searchKey=GUILayout.TextField(searchKey);
        });
        if (twoListArg.isEnabled && twoListArg.isExpanded)
        {
            //EditorGUILayout.BeginHorizontal();
            //item.IsOnlyActive = EditorGUILayout.Toggle("Active", item.IsOnlyActive);
            //item.IsOnlyCanSplit = EditorGUILayout.Toggle("CanSplit", item.IsOnlyCanSplit);
            //if (GUILayout.Button("SplitAll", GUILayout.Width(50)))
            //{
            //    item.SplitAll();
            //}
            //EditorGUILayout.EndHorizontal();
            var list = twoListArg.Items;
            if (!string.IsNullOrEmpty(searchKey))
            {
                list = list.Where(i => i.renderer_lod1.name.Contains(searchKey)).ToList();
            }
            InitEditorArg(list);
            twoListArg.DrawPageToolbar(list, (item, i) =>
            {
                //var door = item;
                if (item.renderer_lod1 == null)
                {
                    return;
                }
                var arg = editorArgs[item];
                arg.caption = $"[{i:00}] {item.renderer_lod1.name}({item.vertexCount1}) <{item.dis:F3}|{item.meshDis:F3}> {item.renderer_lod0.name}({item.vertexCount0})";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, item.renderer_lod1, () =>
                {
                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                    {
                        lodManager.AddLOD2(item.renderer_lod0, item.renderer_lod1);
                    }
                    if (GUILayout.Button("P", GUILayout.Width(50)))
                    {
                        //EditorHelper.SelectObject(door.renderer_lod0);
                        item.renderer_lod1.transform.SetParent(item.renderer_lod0.transform);
                    }
                    if (GUILayout.Button("LOD0", GUILayout.Width(50)))
                    {
                        EditorHelper.SelectObject(item.renderer_lod0);
                    }
                    //if (GUILayout.Button("0&1", GUILayout.Width(50)))
                    //{
                    //    EditorHelper.SelectObjects(new MeshRenderer[] { door.renderer_lod0, door.renderer_lod1 });
                    //}

                });
            });
        }

    }
}

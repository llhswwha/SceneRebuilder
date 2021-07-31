using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODManager))]
public class LODManagerEditor : BaseFoldoutEditor<LODManager>
{
    FoldoutEditorArg twoListArg = new FoldoutEditorArg();

    FoldoutEditorArg lodGroupListArg = new FoldoutEditorArg();

    public override void OnToolLayout(LODManager item)
    {
        base.OnToolLayout(item);

        //if (GUILayout.Button("CheckLODPositions"))
        //{
        //    item.CheckLODPositions();
        //}
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InActive"))
        {
            item.SetLODActive(false);
        }
        if (GUILayout.Button("Active"))
        {
            item.SetLODActive(true);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("SetRenderersLODInfo"))
        {
            item.SetRenderersLODInfo();
        }

        if (GUILayout.Button("UniformLOD"))
        {
            item.UniformLOD();
        }
        EditorUIUtils.Separator(5);

        if (GUILayout.Button("GetLODDetail"))
        {
            string detail=item.GetRuntimeLODDetail(true);
            Debug.Log($"lod detail:{detail}");
        }
        DrawLODGroupList(lodGroupListArg, item);


        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ZeroDistance:");
        item.zeroDistance = EditorGUILayout.FloatField(item.zeroDistance);
        item.DoCreateGroup = GUILayout.Toggle(item.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Group:");
        item.GroupRoot = EditorGUILayout.ObjectField(item.GroupRoot, typeof(GameObject)) as GameObject;
        GUILayout.Label("LOD:");
        item.LODnRoot = EditorGUILayout.ObjectField(item.LODnRoot, typeof(GameObject)) as GameObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("AppendLod1"))
        {
            item.AppendLod1ToGroup();
        }
        if (GUILayout.Button("AppendLod2"))
        {
            item.AppendLod2ToGroup();
        }
        if (GUILayout.Button("AppendLod3"))
        {
            item.AppendLod3ToGroup();
        }
        if (GUILayout.Button("SetColor"))
        {
            item.SetAppendLod3Color();
        }
        EditorGUILayout.EndHorizontal();

        twoListArg.caption = $"TwoObject List";
        EditorUIUtils.ToggleFoldout(twoListArg, arg =>
        {
            var doors = item.twoList;
            arg.caption = $"TwoObject List ({doors.Count}) ({item.LODRendererCount0})({item.LODRendererCount1})";
            //arg.info = $"{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
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
            var doors = item.twoList;
            InitEditorArg(doors);
            twoListArg.DrawPageToolbar(doors, (door, i) =>
            {
                if (door.renderer_lod1 == null)
                {
                    return;
                }
                var arg = editorArgs[door];
                arg.caption = $"[{i:00}] {door.renderer_lod1.name}({door.vertexCount1}) <{door.dis:F3}|{door.meshDis:F3}> {door.renderer_lod0.name}({door.vertexCount0})";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, door.renderer_lod1, () =>
                {
                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                    {
                        item.AddLOD2(door.renderer_lod0, door.renderer_lod1);
                    }
                    if (GUILayout.Button("P", GUILayout.Width(50)))
                    {
                        //EditorHelper.SelectObject(door.renderer_lod0);
                        door.renderer_lod1.transform.SetParent(door.renderer_lod0.transform);
                    }
                    if (GUILayout.Button("LOD0", GUILayout.Width(50)))
                    {
                        EditorHelper.SelectObject(door.renderer_lod0);
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

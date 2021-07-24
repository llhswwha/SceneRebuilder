using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODManager))]
public class LODManagerEditor : BaseFoldoutEditor<LODManager>
{
    FoldoutEditorArg twoListArg = new FoldoutEditorArg();

    FoldoutEditorArg lodListArg = new FoldoutEditorArg();

    public override void OnToolLayout(LODManager item)
    {
        base.OnToolLayout(item);

        //if (GUILayout.Button("CheckLODPositions"))
        //{
        //    item.CheckLODPositions();
        //}

        
         if (GUILayout.Button("UniformLOD"))
        {
            item.UniformLOD();
        }

        if (GUILayout.Button("GetRuntimeLODDetail"))
        {
            string detail=item.GetRuntimeLODDetail(true);
            item.lodDetails.Sort((a, b) =>
            {
                return b.vertexCount.CompareTo(a.vertexCount);
            });
            Debug.Log($"lod detail:{detail}");
        }

        lodListArg.caption = $"LOD List";
        EditorUIUtils.ToggleFoldout(lodListArg, arg =>
        {
            var lods = item.lodDetails;
            float[] sumVertex = new float[4];
            for (int i = 0; i < lods.Count; i++)
            {
                LODGroupDetails lod = (LODGroupDetails)lods[i];
                //sumVertex[i] += lod.chi
                for(int j=0;j<lod.childs.Count;j++)
                {
                    sumVertex[j] += lod.childs[j].vertexCount;
                }
            }
            arg.caption = $"LOD List ({lods.Count})";
            string txt = "";
            float v0 = 0;
            for (int i = 0; i < sumVertex.Length; i++)
            {
                float v = sumVertex[i];
                if(i==0)
                {
                    v0 = v;
                }
                txt += $"{v/10000f:F1}({v/v0:P0})|";
            }
            arg.info = txt;
            InitEditorArg(lods);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
        });
        if (lodListArg.isEnabled && lodListArg.isExpanded)
        {
            var lods = item.lodDetails;
            InitEditorArg(lods);
            lodListArg.DrawPageToolbar(lods, (lodDetail, i) =>
            {
                var arg = editorArgs[lodDetail];
                if (lodDetail.group == null) return;
                arg.caption = $"[{i:00}] {lodDetail.group.name}";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, lodDetail.group, () =>
                {
                    float lod0Vertex = 0;
                    for(int i=0;i<lodDetail.childs.Count;i++)
                    {
                        var lodChild = lodDetail.childs[i];
                        if (i == 0)
                        {
                            lod0Vertex = lodChild.vertexCount;
                        }
                        if (GUILayout.Button($"L{i}[{lodChild.GetVertexCountF():F2}][{lodChild.vertexCount/lod0Vertex:P0}][{lodChild.screenRelativeTransitionHeight}]", GUILayout.Width(140)))
                        {
                            //EditorHelper.SelectObject()
                            EditorHelper.SelectObjects(lodChild.renderers);
                        }
                    }
                });
            });
        }

        if (GUILayout.Button("SetRenderersLODInfo"))
        {
            item.SetRenderersLODInfo();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ZeroDistance:");
        item.zeroDistance = EditorGUILayout.FloatField(item.zeroDistance);
        item.DoCreateGroup = GUILayout.Toggle(item.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Group:");
        item.GroupRoot = EditorGUILayout.ObjectField(item.GroupRoot, typeof(GameObject)) as GameObject;
        GUILayout.Label("LOD2:");
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
        EditorGUILayout.EndHorizontal();

        twoListArg.caption = $"TwoObject List";
        EditorUIUtils.ToggleFoldout(twoListArg, arg =>
        {
            var doors = item.twoList;
            arg.caption = $"TwoObject List ({doors.Count})";
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
                arg.caption = $"[{i:00}] {door.renderer_lod1.name} <{door.dis:F3}|{door.meshDis:F3}> {door.renderer_lod0.name}";
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

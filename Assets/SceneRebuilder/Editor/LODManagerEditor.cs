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
        lodManager.compareMode = (LODCompareMode)EditorGUILayout.EnumPopup(lodManager.compareMode);
        lodManager.DoCreateGroup = GUILayout.Toggle(lodManager.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Group:");
        lodManager.GroupRoot = EditorGUILayout.ObjectField(lodManager.GroupRoot, typeof(GameObject)) as GameObject;
        GUILayout.Label("LOD:");
        lodManager.LODnRoot = EditorGUILayout.ObjectField(lodManager.LODnRoot, typeof(GameObject)) as GameObject;

        if (GUILayout.Button("Show0",GUILayout.Width(50)))
        {
            lodManager.GroupRoot.SetActive(true);
            lodManager.LODnRoot.SetActive(false);
        }
        if (GUILayout.Button("Show1", GUILayout.Width(50)))
        {
            lodManager.GroupRoot.SetActive(false);
            lodManager.LODnRoot.SetActive(true);
        }
        if (GUILayout.Button("Show01", GUILayout.Width(60)))
        {
            lodManager.GroupRoot.SetActive(true);
            lodManager.LODnRoot.SetActive(true);
        }

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
                //list = list.Where(i => i != null && i.renderer_lod1 != null && i.renderer_lod1.name.Contains(searchKey)).ToList();
                list = list.Where(i => i.GetCaption().Contains(searchKey)).ToList();
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
            //if (!string.IsNullOrEmpty(searchKey))
            //{
            //    //list = list.Where(i => i.renderer_lod1.name.Contains(searchKey)).ToList();
            //    //list = list.Where(i => i.GetCaption().Contains(searchKey)).ToList();
            //    list = list.Where(i => i.isSameName).ToList();
            //}
            InitEditorArg(list);
            twoListArg.DrawPageToolbar(list, (item, i) =>
            {
                //var door = item;
                if (item.renderer_lod1 == null)
                {
                    return;
                }
                var arg = editorArgs[item];
                arg.caption = $"[{i:00}] {item.GetCaption()}";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, item.renderer_lod1, () =>
                {
                    if (GUILayout.Button("Debug", GUILayout.Width(50)))
                    {
                        //lodManager.AddLOD2(item.renderer_lod0, item.renderer_lod1);
                        float dis1 = Vector3.Distance(item.renderer_lod0.transform.position, item.renderer_lod1.transform.position);
                        float dis2 = LODManager.GetCenterDistance(item.renderer_lod0.gameObject, item.renderer_lod1.gameObject);
                        float dis3 = MeshHelper.GetAvgVertexDistanceEx(item.renderer_lod0.transform, item.renderer_lod1.transform);
                        Debug.Log($"Debug lod0:{item.renderer_lod0.name} lod1:{item.renderer_lod1.name} dis1:{dis1} dis2:{dis2} dis3:{dis3}");
                        var min = lodManager.GetMinInfo(item.renderer_lod1.transform);
                        Debug.Log($"dis:{min.dis} meshDis:{min.meshDis} target:{min.target}");
                    }
                    //if (GUILayout.Button("Add1", GUILayout.Width(40)))
                    //{
                    //    lodManager.AddLOD1(item.renderer_lod0, item.renderer_lod1);
                    //}
                    //if (GUILayout.Button("Add2", GUILayout.Width(40)))
                    //{
                    //    lodManager.AddLOD2(item.renderer_lod0, item.renderer_lod1);
                    //}
                    //if (GUILayout.Button("Add3", GUILayout.Width(40)))
                    //{
                    //    lodManager.AddLOD3(item.renderer_lod0, item.renderer_lod1);
                    //}

                    if (GUILayout.Button("AddLOD", GUILayout.Width(40)))
                    {
                        lodManager.CreateGroup(item);
                    }

                    //if (GUILayout.Button("Color1", GUILayout.Width(50)))
                    //{
                    //    item.SetColor1();
                    //}
                    //if (GUILayout.Button("Color2", GUILayout.Width(50)))
                    //{
                    //    item.SetColor2();
                    //}
                    //if (GUILayout.Button("P", GUILayout.Width(50)))
                    //{
                    //    //EditorHelper.SelectObject(door.renderer_lod0);
                    //    item.renderer_lod1.transform.SetParent(item.renderer_lod0.transform);
                    //}
                    if (GUILayout.Button("LOD0", GUILayout.Width(45)))
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

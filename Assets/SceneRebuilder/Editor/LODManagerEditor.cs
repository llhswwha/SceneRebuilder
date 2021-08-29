using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODManager))]
public class LODManagerEditor : BaseFoldoutEditor<LODManager>
{
    static FoldoutEditorArg<LODTwoRenderers> twoListArg = new FoldoutEditorArg<LODTwoRenderers>();

    static FoldoutEditorArg<LODGroupDetails> lodGroupListArg = new FoldoutEditorArg<LODGroupDetails>();

    private static string searchKey = "";

    public override void OnEnable()
    {
        base.OnEnable();
        //twoListArg.Items = targetT.twoList;
        targetT.ClearTwoList();
    }

    public static void DrawUI(LODManager lodManager)
    {
        //if (GUILayout.Button("CheckLODPositions"))
        //{
        //    item.CheckLODPositions();
        //}

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetRenderersLODInfo"))
        {
            lodManager.SetRenderersLODInfo();
        }

        if (GUILayout.Button("ChangeLODsRelativeHeight"))
        {
            lodManager.ChangeLODsRelativeHeight();
        }

        if (GUILayout.Button("UniformLOD0"))
        {
            lodManager.UniformLOD0();
        }

        EditorGUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        lodManager.LocalTarget = BaseEditorHelper.ObjectField(lodManager.LocalTarget, GUILayout.Width(100));
        lodManager.lodCamera = BaseEditorHelper.ObjectField(lodManager.lodCamera, GUILayout.Width(100));
        if (GUILayout.Button("Update LODs"))
        {
            string detail = lodManager.GetRuntimeLODDetail(true);
            Debug.Log($"lod detail:{detail}");
        }
        //if (GUILayout.Button("Delete LODs",GUILayout.Width(100)))
        //{
        //    string detail = lodManager.GetRuntimeLODDetail(true);
        //    Debug.Log($"lod detail:{detail}");
        //}

        if (GUILayout.Button("InActive", GUILayout.Width(65)))
        {
            lodManager.SetLODActive(false);
        }
        if (GUILayout.Button("Active", GUILayout.Width(65)))
        {
            lodManager.SetLODActive(true);
        }
        if (GUILayout.Button("Delete", GUILayout.Width(65)))
        {
            lodManager.DeleteLODs();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("time:" + lodManager.lodInfoTime);

        DrawLODGroupList(lodGroupListArg, lodManager);

        EditorGUILayout.BeginHorizontal();
        lodManager.includeInactive = GUILayout.Toggle(lodManager.includeInactive, "includeInactive", GUILayout.Width(100));
        if (GUILayout.Button("InitGroupInfos"))
        {
            lodManager.InitGroupInfos();
        }
        if (GUILayout.Button("SetMats"))
        {
            lodManager.SetMats();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("SaveLOD0s"))
        {
            lodManager.SaveLOD0s();
        }

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

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Unpack"))
        {
            EditorHelper.UnpackPrefab(lodManager.GroupRoot);
            EditorHelper.UnpackPrefab(lodManager.LODnRoot);
        }
        if (GUILayout.Button("Show0"))
        {
            lodManager.ShowRoot0();
        }
        if (GUILayout.Button("Show1"))
        {
            lodManager.ShowRoot1();
        }
        if (GUILayout.Button("Show01"))
        {
            lodManager.ShowRoot01();
        }
        if (GUILayout.Button("Hide01"))
        {
            lodManager.HideRoot01();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Compare"))
        {
            lodManager.CompareTwoRoot();
        }
        if (GUILayout.Button("CheckLOD0"))
        {
            lodManager.CheckLOD0();
        }
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
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetColor"))
        {
            lodManager.SetAppendLod3Color();
        }
        if (GUILayout.Button("DeleteSame"))
        {
            lodManager.DeleteSame();
        }
        if (GUILayout.Button("Replace"))
        {
            lodManager.SetAppendLod3Color();
        }
        if (GUILayout.Button("SetName0"))
        {
            lodManager.SetName0();
        }
        if (GUILayout.Button("SetName1"))
        {
            lodManager.SetName1();
        }
        EditorGUILayout.EndHorizontal();

        twoListArg.caption = $"TwoObject List";
        EditorUIUtils.ToggleFoldout(twoListArg, arg =>
        {
            var list = lodManager.twoList;
            if (!string.IsNullOrEmpty(searchKey))
            {
                //list = list.Where(i => i != null && i.renderer_lod1 != null && i.renderer_lod1.name.Contains(searchKey)).ToList();
                list = list.Where(i => i.GetCaption().Contains(searchKey)).ToList();
            }
            twoListArg.Items = list;
            int v0 = 0;
            int v1 = 0;
            list.ForEach(i => { v0 += i.vertexCount0; v1 += i.vertexCount1; });
            arg.caption = $"TwoObject List ({list.Count})";
            arg.info = $"(r0:{lodManager.LODRendererCount0},v0:{MeshHelper.GetVertexCountS(v0)})(r1:{lodManager.LODRendererCount1},v1:{MeshHelper.GetVertexCountS(v1)})";
            InitEditorArg(list);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
            searchKey = GUILayout.TextField(searchKey);
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
                EditorUIUtils.ObjectFoldout(arg, item.renderer_lod1.gameObject, () =>
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

                    if (GUILayout.Button("AddLOD", GUILayout.Width(60)))
                    {
                        lodManager.CreateGroup(item);
                    }
                    if (GUILayout.Button("Align", GUILayout.Width(60)))
                    {
                        item.Align();
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
                        Debug.Log($"v1:{item.renderer_lod0.GetMinLODVertexCount()} v2:{item.vertexCount0}");
                        EditorHelper.SelectObject(item.renderer_lod0.GetMinLODGo());
                    }
                    //if (GUILayout.Button("0&1", GUILayout.Width(50)))
                    //{
                    //    EditorHelper.SelectObjects(new MeshRenderer[] { door.renderer_lod0, door.renderer_lod1 });
                    //}

                });
            });
        }
    }

    public override void OnToolLayout(LODManager lodManager)
    {
        base.OnToolLayout(lodManager);

        

    }
}

using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static MeshHelper;

[CustomEditor(typeof(LODManager))]
public class LODManagerEditor : BaseFoldoutEditor<LODManager>
{
    static FoldoutEditorArg<LODTwoRenderers> twoListArg = new FoldoutEditorArg<LODTwoRenderers>(true,false);

    static FoldoutEditorArg<LODGroupDetails> lodGroupListArg = new FoldoutEditorArg<LODGroupDetails>(true, false);

    private static string searchKey = "";

    private static string searchKeyTxt = "";

    public override void OnEnable()
    {
        base.OnEnable();
        //twoListArg.Items = targetT.twoList;
        targetT.ClearTwoList();
    }

    public static void DrawUI(LODManager lodManager)
    {
        if (lodManager == null) return;
        //if (GUILayout.Button("CheckLODPositions"))
        //{
        //    item.CheckLODPositions();
        //}

        EditorGUILayout.BeginHorizontal();
        lodManager.LocalTarget = BaseEditorHelper.ObjectField("LODRoot:", 100,lodManager.LocalTarget);
        lodManager.lodCamera = BaseEditorHelper.ObjectField("Camera:",100,lodManager.lodCamera);
        if (GUILayout.Button("Update LODs"))
        {
            lodManager.UpdateLODs();
       
            lodGroupListArg.isEnabled = true;
            lodGroupListArg.isExpanded = true;
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
        lodManager.includeInactive = GUILayout.Toggle(lodManager.includeInactive, "Inactive", GUILayout.Width(100));
        if (GUILayout.Button("SetRenderersInfo"))
        {
            lodManager.SetRenderersLODInfo();
            lodManager.UpdateLODs();
        }

        if (GUILayout.Button("UniformLOD0"))
        {
            lodManager.UniformLOD0();
            lodManager.UpdateLODs();
        }
        if (GUILayout.Button("InitGroupInfos"))
        {
            lodManager.InitGroupInfos(true);
            lodManager.UpdateLODs();
        }
        if (GUILayout.Button("SetMats"))
        {
            lodManager.SetMats();
            lodManager.UpdateLODs();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SaveLOD0s"))
        {
            lodManager.SaveLOD0s();
        }
        if (GUILayout.Button("LoadLOD0s"))
        {
            lodManager.LoadLOD0s();
        }
        if (GUILayout.Button("CheckScene"))
        {
            lodManager.CheckLOD0Scenes();
        }
        if (GUILayout.Button("ClearScenes"))
        {
            lodManager.ClearScenes();
        }
        EditorGUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Levels:");
        Vector2 l1 = EditorGUILayout.Vector2Field("Levels1", lodManager.GetLevels1());
        lodManager.SetLevels1(l1);
        Vector3 l2 = EditorGUILayout.Vector3Field("Levels2", lodManager.GetLevels2());
        lodManager.SetLevels2(l2);
        Vector4 l3 = EditorGUILayout.Vector4Field("Levels3", lodManager.GetLevels3());
        lodManager.SetLevels3(l3);
        if (GUILayout.Button("ChangeRelativeHeight"))
        {
            lodManager.ChangeLODsRelativeHeight();
            lodManager.UpdateLODs();
        }

        EditorGUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ZeroDistance:",GUILayout.Width(100));
        lodManager.zeroDistance = EditorGUILayout.FloatField(lodManager.zeroDistance);
        GUILayout.Label("CompareMode:", GUILayout.Width(100));
        lodManager.compareMode = (LODCompareMode)EditorGUILayout.EnumPopup(lodManager.compareMode);
        lodManager.DoCreateGroup = GUILayout.Toggle(lodManager.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        //GUILayout.Label("Root:");
        //lodManager.GroupRoot = BaseEditorHelper.ObjectField(lodManager.GroupRoot);

        GameObject go1= BaseEditorHelper.ObjectField("Root:",55, lodManager.GroupRoot);
        if(go1!= lodManager.GroupRoot)
        {
            lodManager.GroupRoot = go1;
            lodManager.twoList.Clear();
        }

        //GUILayout.Label("NewLOD:");
        //lodManager.LODnRoot = BaseEditorHelper.ObjectField(lodManager.LODnRoot);

        GameObject go2 = BaseEditorHelper.ObjectField("NewLOD:", 70, lodManager.LODnRoot);
        if (go2 != lodManager.LODnRoot)
        {
            lodManager.LODnRoot = go2;
            lodManager.twoList.Clear();
        }

        if (GUILayout.Button("Compare",GUILayout.Width(80)))
        {
            lodManager.CompareTwoRoot();
            twoListArg.isEnabled = true;
            twoListArg.isExpanded = true;
        }

        //if (GUILayout.Button("TestCompare", GUILayout.Width(80)))
        //{
        //    lodManager.TestCompareTwoRoot();
        //    twoListArg.isEnabled = true;
        //    twoListArg.isExpanded = true;
        //}

        GUIStyle btnStyle = new GUIStyle(EditorStyles.miniButton);
        int lodN = lodManager.GetNewLODn();
        NewEnabledButton("AddLOD_"+ lodN, 80, lodManager.twoList.Count>0, btnStyle, ()=> {
            lodManager.AppendLodNToGroup(lodN);
        });

        //if (GUILayout.Button("AddLOD"))
        //{
        //    lodManager.AppendLodNToGroup();
        //}

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

        if (GUILayout.Button("AddLod1"))
        {
            lodManager.AppendLod1ToGroup();
        }
        if (GUILayout.Button("AddLod2"))
        {
            lodManager.AppendLod2ToGroup();
        }
        if (GUILayout.Button("AddLod3"))
        {
            lodManager.AppendLod3ToGroup();
        }
        if (GUILayout.Button("CheckLOD0"))
        {
            lodManager.CheckLOD0();
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
        if (GUILayout.Button("DeleteFilter"))
        {
            lodManager.DeleteFilter(searchKey);
        }
        //if (GUILayout.Button("Replace"))
        //{
        //    lodManager.Replace();
        //}
        if (GUILayout.Button("ReplaceFilter"))
        {
            lodManager.ReplaceFilter(searchKey);
        }
        if (GUILayout.Button("ReplaceLOD1"))
        {
            lodManager.ReplaceLOD1();
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

        DrawListCompareResult(lodManager);
    }

    public static void DrawListCompareResult(LODManager lodManager)
    {
        twoListArg.caption = $"TwoObject List";
        EditorUIUtils.ToggleFoldout(twoListArg, arg =>
        {
            var list = lodManager.twoList;
            if (!string.IsNullOrEmpty(searchKey))
            {
                //list = list.Where(i => i != null && i.renderer_lod1 != null && i.renderer_lod1.name.Contains(searchKey)).ToList();
                list = list.FindList(searchKey);
            }
            twoListArg.Items = list;
            int v0 = 0;
            int v1 = 0;
            list.ForEach(i => { v0 += i.vertexCount0; v1 += i.vertexCount1; });
            arg.caption = $"TwoObject List ({list.Count})";
            arg.info = $"(r0:{list.LODRendererCount0},v0:{MeshHelper.GetVertexCountS(v0)})(r1:{list.LODRendererCount1},v1:{MeshHelper.GetVertexCountS(v1)})";
            InitEditorArg(list);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
            searchKeyTxt = GUILayout.TextField(searchKeyTxt);
            if (GUILayout.Button("Search"))
            {
                //RemoveEditorArg(item.GetDoors());
                //InitEditorArg(item.UpdateDoors());

                searchKey = searchKeyTxt;
            }
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
            twoListArg.DrawPageToolbar(list, (System.Action<LODTwoRenderers, int>)((item, i) =>
            {
                //var door = item;
                if (item.renderer_new == null)
                {
                    return;
                }
                var arg = editorArgs[item];
                arg.caption = $"[{i:00}] {item.GetLODCaption()}";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, item.renderer_new.gameObject, () =>
                {
                    if (GUILayout.Button("Debug", GUILayout.Width(50)))
                    {
                        //lodManager.AddLOD2(item.renderer_lod0, item.renderer_lod1);
                        float dis1 = Vector3.Distance(item.renderer_old.transform.position, item.renderer_new.transform.position);
                        float dis2 = MeshHelper.GetCenterDistance(item.renderer_old.gameObject, item.renderer_new.gameObject);
                        float dis3 = MeshHelper.GetAvgVertexDistanceEx(item.renderer_old.transform, item.renderer_new.transform);
                        Debug.Log($"Debug lod0:{item.renderer_old.name} lod1:{item.renderer_new.name} dis1:{dis1} dis2:{dis2} dis3:{dis3}");
                        var min = lodManager.GetMinInfo(item.renderer_new.transform);
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
                        Debug.Log($"v1:{item.renderer_old.GetMinLODVertexCount()} v2:{item.vertexCount0}");
                        EditorHelper.SelectObject(item.renderer_old.GetMinLODGo());
                    }
                    //if (GUILayout.Button("0&1", GUILayout.Width(50)))
                    //{
                    //    EditorHelper.SelectObjects(new MeshRenderer[] { door.renderer_lod0, door.renderer_lod1 });
                    //}

                });
            }));
        }
    }

    public override void OnToolLayout(LODManager lodManager)
    {
        base.OnToolLayout(lodManager);

        DrawUI(lodManager);

        if (GUILayout.Button("Window"))
        {
            LODManagerEditorWindow.ShowWindow();
        }
    }
}

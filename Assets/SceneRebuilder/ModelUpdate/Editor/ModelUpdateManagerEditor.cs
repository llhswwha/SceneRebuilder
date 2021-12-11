using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MeshHelper;

[CustomEditor(typeof(ModelUpdateManager))]
public class ModelUpdateManagerEditor : BaseFoldoutEditor<ModelUpdateManager>
{
    static FoldoutEditorArg<LODTwoRenderers> twoListArg = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    //private static string searchKey = "";
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_All = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_Door = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_LodDevs = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_Wall = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_MemberPart = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_Welding = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_Piping = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListOldArg_Others = new FoldoutEditorArg<LODTwoRenderers>(true, false);

    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg_MemberPart = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg_WallPart = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg_Welding = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg_Piping = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> modelListNewArg_Others = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    public override void OnToolLayout(ModelUpdateManager item)
    {
        base.OnToolLayout(item);

        ModelUpdateManagerEditor.DrawUI(item);

        if (GUILayout.Button("Window"))
        {
            ModelUpdateManagerEditorWindow.ShowWindow();
        }
    }



    internal static void DrawUI(ModelUpdateManager item)
    {
        if (item == null) return;
        GUILayout.BeginHorizontal();
        item.Model_Old = BaseEditorHelper.ObjectField("ModelOld:", 100, item.Model_Old);
        item.Model_New = BaseEditorHelper.ObjectField("ModelNew:", 100, item.Model_New);
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("ZeroDistance:", GUILayout.Width(100));
        item.zeroDistance = EditorGUILayout.FloatField(item.zeroDistance);
        GUILayout.Label("CompareMode:", GUILayout.Width(100));
        item.compareMode = (LODCompareMode)EditorGUILayout.EnumPopup(item.compareMode);
        //lodManager.DoCreateGroup = GUILayout.Toggle(lodManager.DoCreateGroup, "CreateGroup");
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        item.isIncludeInactive = GUILayout.Toggle(item.isIncludeInactive, "isIncludeInactive");
        item.IsFilterByFiles = GUILayout.Toggle(item.IsFilterByFiles, "IsFilterByFiles");
        if (GUILayout.Button("GetRenders"))
        {
            item.GetModelRenders();
        }
        if (GUILayout.Button("ClearUpdates"))
        {
            item.ClearUpdates();
        }
        GUILayout.Label("MaxCount:", GUILayout.Width(100));
        item.MaxCompareCount = EditorGUILayout.IntField(item.MaxCompareCount);
        if (GUILayout.Button("CompareRenders"))
        {
            item.CompareModels();
        }

        //if (GUILayout.Button("CompareDoors"))
        //{
        //    item.CompareModels_Doors();
        //}

        //if (GUILayout.Button("ReplaceOld"))
        //{
        //    item.ReplaceOld();
        //}
        //if (GUILayout.Button("DeleteNew"))
        //{
        //    item.DeleteNew();
        //}
        GUILayout.EndHorizontal();
        //DrawRendererInfoList("Models_Old", item.ModelRendersWaiting_Old, modelListOldArg);

        
        DrawListCompareResult("Models_Old(All)", item, item.ModelRendersWaiting_Old_All, modelListOldArg_All);
        DrawListCompareResult("Models_Old", item, item.ModelRendersWaiting_Old, modelListOldArg);
        DrawListCompareResult("Models_Old_Door", item, item.ModelRendersWaiting_Old_Door, modelListOldArg_Door);
        DrawListCompareResult("Models_Old_Dev(LOD)", item, item.ModelRendersWaiting_Old_LodDevs, modelListOldArg_LodDevs);
        DrawListCompareResult("Models_Old_Wall", item, item.ModelRendersWaiting_Old_Walls, modelListOldArg_Wall);
        DrawListCompareResult("Models_Old_MemberPart", item, item.ModelRendersWaiting_Old_MemberPart, modelListOldArg_MemberPart);
        DrawListCompareResult("Models_Old_Welding", item, item.ModelRendersWaiting_Old_Welding, modelListOldArg_Welding);
        DrawListCompareResult("Models_Old_Piping", item, item.ModelRendersWaiting_Old_Piping, modelListOldArg_Piping);
        DrawListCompareResult("Models_Old_Other", item, item.ModelRendersWaiting_Old_Others, modelListOldArg_Others);
        EditorUIUtils.Separator(5);
        DrawListCompareResult("Models_New", item, item.ModelRendersWaiting_New, modelListNewArg);
        DrawListCompareResult("Models_New_MemberPart", item, item.ModelRendersWaiting_MemberPart, modelListNewArg_MemberPart);
        DrawListCompareResult("Models_New_WallPart", item, item.ModelRendersWaiting_WallPart, modelListNewArg_WallPart);
        DrawListCompareResult("Models_New_Welding", item, item.ModelRendersWaiting_Welding, modelListNewArg_Welding);
        DrawListCompareResult("Models_New_Piping", item, item.ModelRendersWaiting_Piping, modelListNewArg_Piping);
        DrawListCompareResult("Models_New_Others", item, item.ModelRendersWaiting_NewOthers, modelListNewArg_Others);
        EditorUIUtils.Separator(5);
        DrawListCompareResult("CompareResultList", item, item.twoList, twoListArg);
    }


    

    public static void DrawListCompareResult(string name,ModelUpdateManager lodManager, LODTwoRenderersList twoList, FoldoutEditorArg<LODTwoRenderers> listArg)
    {
        listArg.caption = name;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list = twoList;
            if (!string.IsNullOrEmpty(twoList.searchKey))
            {
                list = list.FindList(twoList.searchKey);
            }
            listArg.Items = list;
            int v0 = 0;
            int v1 = 0;
            list.ForEach(i => { v0 += i.vertexCount0; v1 += i.vertexCount1; });
            arg.caption = $"{name} ({list.Count})->{twoList.GetTargetListInfo()}";
            arg.info = $"({list.LODRendererCount0}|{MeshHelper.GetVertexCountS(v0)})({list.LODRendererCount1}|{MeshHelper.GetVertexCountS(v1)})";
            InitEditorArg(list);
        },
        () =>
        {
            

            twoList.searchKeyInput = GUILayout.TextField(twoList.searchKeyInput, GUILayout.Width(100));
            if (GUILayout.Button("Search"))
            {
                //RemoveEditorArg(item.GetDoors());
                //InitEditorArg(item.UpdateDoors());

                twoList.searchKey = twoList.searchKeyInput;
                Debug.Log("Search:"+ twoList.searchKey);
            }

        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Size", GUILayout.Width(30));
            twoList.isShowSize = EditorGUILayout.Toggle(twoList.isShowSize, GUILayout.Width(50));
            var btnStyle = new GUIStyle(EditorStyles.miniButton);
            btnStyle.margin = new RectOffset(0, 0, 0, 0);
            btnStyle.padding = new RectOffset(0, 0, 0, 0);
            if (GUILayout.Button("Comp", btnStyle, GUILayout.Width(45)))
            {
                twoList.CompareList(lodManager.MaxCompareCount, lodManager.compareMode);
            }
            if (GUILayout.Button("Test", btnStyle, GUILayout.Width(40)))
            {
                twoList.TestInitSharedMesh();
            }
            if (GUILayout.Button("Clear", btnStyle, GUILayout.Width(40)))
            {
                twoList.ClearNew();
            }
            twoList.zeroDistance = EditorGUILayout.FloatField(twoList.zeroDistance, GUILayout.Width(45));
            if (GUILayout.Button("DelNewOld", btnStyle, GUILayout.Width(75)))
            {
                twoList.DeleteNewOld();
            }
            if (GUILayout.Button("DelNew", btnStyle, GUILayout.Width(55)))
            {
                twoList.DeleteNew();
            }
            if (GUILayout.Button("DelOld", btnStyle, GUILayout.Width(55)))
            {
                twoList.DeleteOld();
            }
            if (GUILayout.Button("DelSame", btnStyle, GUILayout.Width(60)))
            {
                twoList.DeleteSame();
            }
            if (GUILayout.Button("RepOld", btnStyle, GUILayout.Width(50)))
            {
                twoList.ReplaceOld();
            }
            if (GUILayout.Button("RepNew", btnStyle, GUILayout.Width(50)))
            {
                twoList.ReplaceNew();
            }
            if (GUILayout.Button("RenaNew", btnStyle, GUILayout.Width(60)))
            {
                twoList.RenameNew();
            }
            if (GUILayout.Button("MatNew", btnStyle, GUILayout.Width(60)))
            {
                twoList.ReplaceMaterialNew();
            }
            if (GUILayout.Button("MatOld", btnStyle, GUILayout.Width(60)))
            {
                twoList.ReplaceMaterialOld();
            }
            //if (GUILayout.Button("AliOld", GUILayout.Width(50)))
            //{
            //    twoList.AlignOld();
            //}
            if (GUILayout.Button("Upda", btnStyle, GUILayout.Width(45)))
            {
                twoList.DoUpdate();
            }
            GUILayout.EndHorizontal();

            var list = listArg.Items;
            InitEditorArg(list);
            listArg.DrawPageToolbar(list, (Action<LODTwoRenderers, int>)((item, i) =>
            {
                //if (item.renderer_lod1 == null)
                //{
                //    return;
                //}
                var arg = editorArgs[item];
                arg.caption = $"[{i+1:00}] {item.GetCompareCaption(twoList.isShowSize)}";
                //arg.info = door.ToString();

                arg.bold = false;
                if (item.dis> twoList.zeroDistance)
                {
                    arg.bold = true;
                }
                if (item.renderer_old == null) return;
                EditorUIUtils.ObjectFoldout(arg, item.renderer_old.gameObject, () =>
                {
                    item.UpdateMode = (UpdateChangedMode)EditorGUILayout.EnumPopup(item.UpdateMode, GUILayout.Width(90));
                    if (GUILayout.Button("Debug", GUILayout.Width(50)))
                    {
                        twoList.CompareOne(item, lodManager.compareMode);

                        //lodManager.AddLOD2(item.renderer_lod0, item.renderer_lod1);
                        if (item.renderer_new != null)
                        {
                            float dis1 = Vector3.Distance(item.renderer_old.transform.position, item.renderer_new.transform.position);
                            float dis2 = MeshHelper.GetCenterDistance(item.renderer_old.gameObject, item.renderer_new.gameObject);
                            float dis3 = MeshHelper.GetAvgVertexDistanceEx(item.renderer_old.transform, item.renderer_new.transform);
                            float dis4 = MeshHelper.GetMinDistance(item.renderer_old.gameObject, item.renderer_new.gameObject);
                            float dis5 = MeshHelper.GetMaxDistance(item.renderer_old.gameObject, item.renderer_new.gameObject);
                            Debug.Log($"Debug lod0:{item.renderer_old.name} lod1:{item.renderer_new.name} dis1:{dis1} dis2:{dis2} dis3:{dis3} dis4:{dis4} dis5:{dis5}");
                           
                        }
                        var min = twoList.GetMinInfo(item.renderer_old.transform, lodManager.compareMode);
                        Debug.Log($"dis:{min.dis} meshDis:{min.meshDis} target:{min.target}");
                    }
                    if (GUILayout.Button("Align", GUILayout.Width(40)))
                    {
                        item.Align();
                    }
                    if (GUILayout.Button("New", GUILayout.Width(35)))
                    {
                        Debug.Log($"v1:{item.renderer_new.GetMinLODVertexCount()} v2:{item.vertexCount1}");
                        EditorHelper.SelectObject(item.renderer_new.GetMinLODGo());
                    }
                    
                });
            }));
        }
    }
}

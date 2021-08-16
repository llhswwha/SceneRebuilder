using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabInstanceBuilder))]
public class PrefabInstanceBuilderEditor : BaseFoldoutEditor<PrefabInstanceBuilder>
{
    private FoldoutEditorArg prefabInfoListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();

        prefabInfoListArg = new FoldoutEditorArg(true, false, true, true, false);
    }

    public override void OnToolLayout(PrefabInstanceBuilder item)
    {
        base.OnToolLayout(item);

        GUILayout.BeginHorizontal();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearIns"))
        {
            item.ClearPrefabs();
        }
        if (GUILayout.Button("HideIns14"))
        {
            item.HideInstances1();
        }
        if (GUILayout.Button("HideIns56"))
        {
            item.HideInstances2();
        }
        if (GUILayout.Button("ShowIns14"))
        {
            item.ShowInstances1();
        }
        if (GUILayout.Button("ShowIns56"))
        {
            item.ShowInstances2();
        }
        if (GUILayout.Button("RemIns14"))
        {
            item.RemoveInstances1();
        }
        if (GUILayout.Button("RemIns56"))
        {
            item.RemoveInstances2();
        }



        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DestoryIns"))
        {
            item.DestoryInstances();
        }
        if (GUILayout.Button("PrefabListCount"))
        {
            item.ShowPrefabListCount();
        }
        if (GUILayout.Button("InsCount"))
        {
            item.ShowInstanceCount();
        }
        //if (GUILayout.Button("PrefabCount"))
        //{
        //    item.ShowPrefabCount();
        //}
        if (GUILayout.Button("MeshSize"))
        {
            item.GetMeshSizeInfo();
        }
        if (GUILayout.Button("VertexCenterInfos"))
        {
            item.GetVertexCenterInfos();
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowRenderers"))
        {
            item.ShowRenderers();
        }
        if (GUILayout.Button("BigSmallRenderers"))
        {
            item.GetBigSmallRenderers();
        }
        if (GUILayout.Button("CombinedRenderers"))
        {
            item.GetCombinedRenderers();
        }

        if (GUILayout.Button("HiddenRenderers"))
        {
            item.GetHiddenRenderers();
        }
        GUILayout.EndHorizontal();

        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("TargetCount"))
        {
            item.GetTargetCount();
        }
        if (GUILayout.Button("CreatePrefab"))
        {
            item.CreatePrefab();
        }
        if (GUILayout.Button("UnpackPrefab"))
        {
            item.UnpackPrefab();
        }
        if (GUILayout.Button("CreateTestModel"))
        {
            item.CreateTestModel();
        }
        if (GUILayout.Button("ResetPrefabs"))
        {
            item.ResetPrefabs();
        }
        if (GUILayout.Button("DestroySimgleOnes"))
        {
            item.DestroySimgleOnes();
        }
        

        GUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);
        GUILayout.BeginHorizontal();
        item.TargetRoots = ObjectField(item.TargetRoots);
        item.IsCopyTargetRoot=GUILayout.Toggle(item.IsCopyTargetRoot, "IsCopy");
        item.TargetRootsCopy = ObjectField(item.TargetRootsCopy);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetThreePointJobs"))
        {
            item.GetThreePointJobs();
        }
        if (GUILayout.Button("MeshAlignJobs"))
        {
            item.MeshAlignJobs();
        }
        if (GUILayout.Button("RTAlignJobs"))
        {
            item.RTAlignJobs();
        }
        if (GUILayout.Button("RTAlignJobs"))
        {
            item.RTAlignJobs();
        }
        if (GUILayout.Button("AcRTAlignJobs"))
        {
            item.AcRTAlignJobs();
        }
        if (GUILayout.Button("* AcRTAlignJobsEx"))
        {
            item.AcRTAlignJobsEx();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("TestRTs"))
        {
            item.TestRTs();
        }
        if (GUILayout.Button("AcRTAlignJobsEx2"))
        {
            item.AcRTAlignJobsEx2();
        }
        if (GUILayout.Button("AcRTAlignJobsEx(TestList)"))
        {
            item.AcRTAlignJobsExTestList();
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowPrefabInfo"))
        {
            item.ShowPrefabInfo();
        }
        if (GUILayout.Button("SortPrefabInfoList"))
        {
            item.SortPrefabInfoList();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateInstance"))
        {
            item.CreateInstance();
        }
        if (GUILayout.Button("CreatePrefabs"))
        {
            item.CreatePrefabs();
        }
        if (GUILayout.Button("CreateInstances"))
        {
            item.CreateInstances();
        }
        if (GUILayout.Button("CreateInstances_LOD"))
        {
            item.CreateInstances_LOD();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("OneKey_Align_Remove_Instance"))
        {
            item.OneKey_Align_Remove_Instance();
        }
        if (GUILayout.Button("OneKey_Align_Remove_Instance_LOD"))
        {
            item.OneKey_Align_Remove_Instance_LOD();
        }

        base.DrawPrefabList(prefabInfoListArg, () => item.PrefabInfoList);

        //base.DrawObjectList(prefabInfoListArg, "PrefabInfoList", () =>
        //{
        //    return item.PrefabInfoList;
        //},
        //(arg,item,i) =>
        //{
        //    arg.caption = $"[{i:00}] {item.Prefab.name}({item.InstanceCount})";
        //    arg.tag = item.Prefab;
        //    arg.isFoldout = true;
        //    arg.background = true;
        //},
        //(item) =>
        // {
        //     //ObjectField(item.Prefab);
        //     //if (GUILayout.Button(item.Prefab.name,GUILayout.Width(100)))
        //     //{
        //     //    EditorHelper.SelectObject(item.Prefab);
        //     //}

        //     //base.DrawObjectList(item.Instances,"")
        // }, (arg, item, i) =>
        // {
        //     //arg.caption = $"[{i:00}] {item.Prefab.name}({item.InstanceCount})";
        //     //arg.tag = item.Prefab;
        //     //arg.isFoldout = true;

        //     base.DrawObjectList(arg, "InstanceList", () =>
        //       {
        //           return item.Instances;
        //       }, (subArg,subItem,subI)=>
        //       {
        //           subArg.level = 2;
        //       }, null, null);
        // });
    }
}

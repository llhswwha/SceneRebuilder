using CodeStage.AdvancedFPSCounter.Editor.UI;
using MeshJobs;
using System;
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

    public static void DrawUI(PrefabInstanceBuilder item)
    {
        if (item == null) return;
        DrawTargetRoot(item);
        DrawAlignTest(item);
        DrawAlignResult(item);
        DrawToolButtons(item);
    }

    public static void DrawTargetRoot(PrefabInstanceBuilder item)
    {
        GUILayout.BeginHorizontal();
        GameObject targetNew= BaseEditorHelper.ObjectField(item.TargetRoots);
        if(item.TargetRoots!=targetNew)
        {
            item.TargetRoots = targetNew;
            if(item.IsCopyTargetRoot==true && item.TargetRootsCopy!=null)
            {
                GameObject.DestroyImmediate(item.TargetRootsCopy);
            }
        }
        
        item.IsCopyTargetRoot = GUILayout.Toggle(item.IsCopyTargetRoot, "IsCopy");
        item.TargetRootsCopy = BaseEditorHelper.ObjectField(item.TargetRootsCopy);
        if (GUILayout.Button("X",GUILayout.Width(30)))
        {
            if (item.TargetRootsCopy != null)
            {
                GameObject.DestroyImmediate(item.TargetRootsCopy);
            }
        }
        if (GUILayout.Button("GetMeshFilters"))
        {
            item.GetMeshFilters();
        }
        GUILayout.EndHorizontal();
    }

    public static void DrawAlignTest(PrefabInstanceBuilder item)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("JobSize:", GUILayout.Width(60));
        item.JobSize = EditorGUILayout.IntField(item.JobSize, GUILayout.Width(60));

        AcRTAlignJobSetting setting = AcRTAlignJobSetting.Instance;

        GUILayout.Label("MinSize:", GUILayout.Width(60));
        setting.CompareSizeMinValue = EditorGUILayout.FloatField(setting.CompareSizeMinValue, GUILayout.Width(40));

        GUILayout.Label("MaxVertex:", GUILayout.Width(60));
        item.MaxVertexCount = EditorGUILayout.IntField(item.MaxVertexCount, GUILayout.Width(50));

        setting.IsSetParent = GUILayout.Toggle(setting.IsSetParent, "Parent", GUILayout.Width(60));//关联相似的模型用于测试，测试好了要关闭。

        GUILayout.Label("TryModes:", GUILayout.Width(60));
        setting.IsTryAngles = GUILayout.Toggle(setting.IsTryAngles, "Angle",GUILayout.Width(60));
        setting.IsTryAngles_Scale = GUILayout.Toggle(setting.IsTryAngles_Scale, "Scale", GUILayout.Width(50));
        setting.IsTryRT = GUILayout.Toggle(setting.IsTryRT, "RT", GUILayout.Width(40));
        setting.IsTryICP = GUILayout.Toggle(setting.IsTryICP, "ICP", GUILayout.Width(100));
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("MeshAlignJobs"))
        {
            item.MeshAlignJobs();
        }
        if (GUILayout.Button("RTAlignJobs"))
        {
            item.RTAlignJobs();
        }
        //if (GUILayout.Button("RTAlignJobs"))
        //{
        //    item.RTAlignJobs();
        //}
        if (GUILayout.Button("* AcRTAlignJobs"))
        {
            item.AcRTAlignJobs();
        }
        if (GUILayout.Button("* AcRTAlignJobsEx"))
        {
            item.AcRTAlignJobsEx();
        }

        if (GUILayout.Button("* TestList"))
        {
            //TreePointJobHelper.IsShowProgress = false;

            //float[] minSize = new float[] { 1.01f, 1.05f, 1.1f, 1.25f, 1.5f, 2, 3, 0 };
            float[] minSize = item.TestMinSizeList;
            string log = "";
            DateTime start0 = DateTime.Now;
            for (int i = 0; i < minSize.Length; i++)
            {
                float s = minSize[i];

                ProgressArg p1 = new ProgressArg("TestList", i, minSize.Length, s);
                JobHandleList.testProgressArg = p1;
                //if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                //{
                //    break;
                //}

                DateTime start = DateTime.Now;
                setting.CompareSizeMinValue = s;
                if (item.TargetRootsCopy != null)
                {
                    GameObject.DestroyImmediate(item.TargetRootsCopy);
                }
                var prefabs=item.AcRTAlignJobsEx();
               
                if (prefabs == null)
                {
                    Debug.LogError("prefabs == null");
                    break;
                }
                log += $"{item.TargetRoots.transform.parent.name}\t{item.TargetRoots.name}\t{AcRTAlignJobContainer.Last.targetCount}\t{item.PrefabInfoList.Count}\t{s}\t{DateTime.Now - start}\t{AcRTAlignJobContainer.Last.totalJobCount}\n";
            }
            JobHandleList.testProgressArg = null;
            //TreePointJobHelper.IsShowProgress = true;
            ProgressBarHelper.ClearProgressBar();

            Debug.LogError($"time:{DateTime.Now - start0}\n{log}");
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetThreePointJobs"))
        {
            item.GetThreePointJobs();
        }
        if (GUILayout.Button("TestRTs"))
        {
            item.TestRTs();
        }
        if (GUILayout.Button("AlignEx2"))
        {
            item.AcRTAlignJobsEx2();
        }
        if (GUILayout.Button("AlignEx(TestList)"))
        {
            item.AcRTAlignJobsExTestList();
        }

        GUILayout.EndHorizontal();
    }

    public static void DrawAlignResult(PrefabInstanceBuilder item)
    {
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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Align_Remove_Instance"))
        {
            item.OneKey_Align_Remove_Instance();
        }
        if (GUILayout.Button("Align_Remove_Instance_LOD"))
        {
            item.OneKey_Align_Remove_Instance_LOD();
        }
        GUILayout.EndHorizontal();
    }

    public static void DrawToolButtons(PrefabInstanceBuilder item)
    {
        EditorUIUtils.Separator(5);
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
            //item.GetBigSmallRenderers();
            var bs = new BigSmallListInfo(item.TargetRoots,true);
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
    }

    public override void OnToolLayout(PrefabInstanceBuilder item)
    {
        base.OnToolLayout(item);

        EditorUIUtils.Separator(5);
        DrawUI(item);
        BaseFoldoutEditorHelper.DrawPrefabList(prefabInfoListArg, () => item.PrefabInfoList);

        if (GUILayout.Button("Window"))
        {
            PrefabInstanceBuilderEditorWindow.ShowWindow();
        }

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

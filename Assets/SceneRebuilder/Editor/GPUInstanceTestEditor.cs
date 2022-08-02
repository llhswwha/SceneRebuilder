using CodeStage.AdvancedFPSCounter.Editor.UI;
using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GPUInstanceTest))]
public class GPUInstanceTestEditor : BaseFoldoutEditor<GPUInstanceTest>
{
    static FoldoutEditorArg gpuiRootListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg prefabListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg managedprefabListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg lodPrefabListArg = new FoldoutEditorArg(true, false);

    public new void OnEnable()
    {
        base.OnEnable();
        targetT.CheckList();
    }

    public override void OnToolLayout(GPUInstanceTest item)
    {
        EditorGUILayout.BeginHorizontal();
        item.MeshTarget = ObjectField("MeshTarget", item.MeshTarget);
        GUILayout.Label($"PrefabCount:{item.PrefabCount}");
        if (GUILayout.Button("GetPreInfo10-300"))
        {
            item.GetPrefabListInfo10_300();
        }
        EditorGUILayout.EndHorizontal();

        DrawObjectList(gpuiRootListArg, "GPUIRoots", item.GPUIRoots, null, null, null);
        DrawObjectList(prefabListArg, "PrefabList", item.PrefabList, null, null, null);
        DrawObjectList(lodPrefabListArg, "LODPrefabList", item.LODPrefabList, null, null, null);

        if (item.ManagedPrefabList.Count == 0)
        {
            item.GetManagedPrefabList();
        }
        DrawObjectList(managedprefabListArg, "ManagedPrefabList", item.ManagedPrefabList, null, null, null);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("MinPreInsCount:",GUILayout.Width(100));
        item.MinPrefabInstanceCount = EditorGUILayout.IntField(item.MinPrefabInstanceCount);
        if (GUILayout.Button("GetPreInfo"))
        {
            item.GetPrefabListInfo();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("OneKeySetPrefabs"))
        {
            item.OneKeySetPrefabs();
        }
        if (GUILayout.Button("OneKeyClear",GUILayout.Width(100)))
        {
            if(EditorUtility.DisplayDialog("确认", "删除Prefabs设置和文件", "确定删除", "不删除"))
            {
                item.OneKeyClearPrefabs();
                Debug.Log("确定删除");
            }
            else
            {
                Debug.Log("不删除");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("0.GetRoots"))
        {
            item.GetGPUIRoots();
        }
        if (GUILayout.Button("1.GetPrefabs"))
        {
            item.GetPrefabMeshes();
            prefabListArg.isEnabled = true;
            prefabListArg.isExpanded = true;
        }

        if (GUILayout.Button("2.SaveMesh"))
        {
            item.SaveMesh();
        }
        if (GUILayout.Button("2.SavePrefab"))
        {
            item.SavePrefabs();
        }
        if (GUILayout.Button("3.InitPrefabs"))
        {
            item.InitPrefabs();
        }
        if (GUILayout.Button("4.ReplaceInstances"))
        {
            item.ReplaceInstances();
        }
        if (GUILayout.Button("4.ReplaceInstancesLOD"))
        {
            item.ReplaceInstancesLODWhenEditor();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearRoots"))
        {
            item.ClearRoots();
        }

        if (GUILayout.Button("ClearPrefabs"))
        {
            if (EditorUtility.DisplayDialog("确认", "清空Prefabs列表", "确定清空", "不清空"))
            {
                item.ClearOldPrefabs();
                Debug.Log("确定清空");
            }
            else
            {
                Debug.Log("不清空");
            }
        }
        if (GUILayout.Button("DeleteSame"))
        {
            item.DeleteSamePrefabs();
        }
        if (GUILayout.Button("DeletePrefabs"))
        {
            if (EditorUtility.DisplayDialog("确认", "删除Prefabs文件", "确定删除", "不删除"))
            {
                item.DeleteOtherPrefabs();
                Debug.Log("确定删除");
            }
            else
            {
                Debug.Log("不删除");
            }
        }
        if (GUILayout.Button("OpenDir",GUILayout.Width(70)))
        {
            item.SelectPrefabFile();
        }
        if (GUILayout.Button("DeletePrototypeData"))
        {
            if (EditorUtility.DisplayDialog("确认", "删除PrototypeData文件", "确定删除", "不删除"))
            {
                item.DeletePrototypeData();
                Debug.Log("确定删除");
            }
            else
            {
                Debug.Log("不删除");
            }
        }
        if (GUILayout.Button("OpenDir", GUILayout.Width(70)))
        {
            item.SelectPrototypeDataFile();
        }
        EditorGUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        if (item.prefabManager)
        {
            item.prefabManager.IsEnableUpdate = GUILayout.Toggle(item.prefabManager.IsEnableUpdate, "Update", GUILayout.Width(70));
            //item.prefabManager.IsEnableLateUpdate = GUILayout.Toggle(item.prefabManager.IsEnableLateUpdate, "LateUpdate", GUILayout.Width(90));
            item.prefabManager.IsEnableUpdateTreeMPB = GUILayout.Toggle(item.prefabManager.IsEnableUpdateTreeMPB, "TreeMPB", GUILayout.Width(80));
            item.prefabManager.IsEnableUpdateBuffers = GUILayout.Toggle(item.prefabManager.IsEnableUpdateBuffers, "Buffers", GUILayout.Width(80));
            //item.prefabManager.IsUpdateGPUBuffers = GUILayout.Toggle(item.prefabManager.IsUpdateGPUBuffers, "GPUBuffers", GUILayout.Width(80));
            //item.prefabManager.IsGPUIDrawMeshInstancedIndirect = GUILayout.Toggle(item.prefabManager.IsGPUIDrawMeshInstancedIndirect, "DrawMeshInstanced", GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.isShowLog = GUILayout.Toggle(item.isShowLog, "ShowLog", GUILayout.Width(80));
        item.AlwaysGUPI = GUILayout.Toggle(item.AlwaysGUPI, "AlwaysGUPI", GUILayout.Width(120));
        GPUInstancerPrefab.AlwaysGUPI = item.AlwaysGUPI;
        item.IsAutoHideShowRoot = GUILayout.Toggle(item.IsAutoHideShowRoot, "AutoHideShow", GUILayout.Width(120));
        item.EnableAutoHide = GUILayout.Toggle(item.EnableAutoHide, "AutoHide", GUILayout.Width(120));
        item.EnableAutoShow = GUILayout.Toggle(item.EnableAutoShow, "AutoShow", GUILayout.Width(120));
        item.IsAsync = GUILayout.Toggle(item.IsAsync, "Async", GUILayout.Width(65));
        item.runInThreads = GUILayout.Toggle(item.runInThreads, "InThread", GUILayout.Width(80));
        item.IsCreatePrefabsWhenRun = GUILayout.Toggle(item.IsCreatePrefabsWhenRun, "CreatePrefabs", GUILayout.Width(120));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.IsAutoGPUI = GUILayout.Toggle(item.IsAutoGPUI, "AutoStart", GUILayout.Width(80));
        if (GUILayout.Button("StartGPUI"))
        {
            item.StartGPUInstanceOfTargets();
        }
        if (GUILayout.Button("StopGPUI"))
        {
            //item.AlwaysGUPI = false;
            //GPUInstancerPrefab.AlwaysGUPI = false;
            item.StopGPUInstanceOfGPUIRoots();
        }
        if (GUILayout.Button("RemoveOneInstance"))
        {
            item.AlwaysGUPI = false;
            GPUInstancerPrefab.AlwaysGUPI = false;
            item.RemoveOneInstance();
        }
        if (GUILayout.Button("RemoveOnePrefab"))
        {
            item.AlwaysGUPI = false;
            GPUInstancerPrefab.AlwaysGUPI = false;
            item.RemoveOnePrefab();
        }
        if (GUILayout.Button("RemoveOnePrefabLOD"))
        {
            item.AlwaysGUPI = false;
            GPUInstancerPrefab.AlwaysGUPI = false;
            item.RemoveOnePrefabLOD();
        }

        EditorGUILayout.EndHorizontal();
        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("CoroutineSize:", GUILayout.Width(100));
        item.CoroutineSize = EditorGUILayout.IntField(item.CoroutineSize, GUILayout.Width(50));
        if (GUILayout.Button("RemoveCoroutine"))
        {
            item.RemovePrefabsInstancesCoroutine();
        }
        if (GUILayout.Button("AddCoroutine"))
        {
            item.AddPrefabsInstancesCoroutine();
        }
        if (GUILayout.Button("RemoveAsync"))
        {
            item.RemovePrefabInstancesAsync(null);
        }
        if (GUILayout.Button("AddAsync"))
        {
            item.AddGPUIPrefabInstancesAsync(null);
        }
        //if (GUILayout.Button("StartGPUIOfMeshTarget"))
        //{
        //    item.StartGPUInstanceOfMeshTarget();
        //}
        //if (GUILayout.Button("StartGPUIOfGPUIRoots"))
        //{
        //    item.StartGPUInstanceOfGPUIRoots();
        //}
        //if (GUILayout.Button("StopGPUIOfGPUIRoots"))
        //{
        //    item.StopGPUInstanceOfGPUIRoots();
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HideNotInPrefabs"))
        {
            item.HideNotInPrefabs();
        }
        if (GUILayout.Button("HideInPrefabs"))
        {
            item.HideInPrefabs();
        }
        if (GUILayout.Button("ShowAll"))
        {
            item.ShowAll();
        }
        if (GUILayout.Button("UnpackAll"))
        {
            item.UnpackAll();
        }
        if (GUILayout.Button("ClearScripts"))
        {
            item.ClearPrefabScrips();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.MoveTargetRoot = ObjectField("MoveTargetRoot", item.MoveTargetRoot);
        if (GUILayout.Button("MoveToGPUI"))
        {
            item.MovePrefabInstances();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GetPrefabsInScene"))
        {
            item.GetPrefabsInScene();
        }
        if (GUILayout.Button("InitPreInstances"))
        {
            item.InitPrefabInstances();
        }
        if (GUILayout.Button("ClearPreInstances"))
        {
            item.ClearPrefabInstances();
        }
        if (GUILayout.Button("ShowPrototypes"))
        {
            item.ShowPrototypeList();
        }
        if (GUILayout.Button("DistroyInactive"))
        {
            item.DistroyInactive();
        }
        if (GUILayout.Button("DistroyInBuild"))
        {
            item.DistroyInBuild();
        }
        //if (GUILayout.Button("Disable"))
        //{
        //    item.DisableManager();
        //}
        //if (GUILayout.Button("Enable"))
        //{
        //    item.EnableManager();
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.TestTargetGPUIModel = ObjectField("TestPrefab", item.TestTargetGPUIModel);
        if (GUILayout.Button("GPUIOn"))
        {
            item.GPUIOn();
        }
        if (GUILayout.Button("GPUIOff"))
        {
            item.GPUIOff();
        }
        if (GUILayout.Button("TransparentOn"))
        {
            item.TransparentOn();
        }
        if (GUILayout.Button("TransparentOff"))
        {
            item.TransparentOff();
        }
        if (GUILayout.Button("HightlightOn"))
        {
            item.HighlightOn();
        }
        if (GUILayout.Button("HighlightOff"))
        {
            item.HighlightOff();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("TransparentOnEx"))
        {
            item.TransparentOnEx();
        }
        if (GUILayout.Button("TransparentOffEx"))
        {
            item.TransparentOffEx();
        }

        if (GUILayout.Button("HighlightOnEx"))
        {
            item.HighlightOnEx();
        }
        if (GUILayout.Button("HighlightOffEx"))
        {
            item.HighlightOffEx();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("FindWalls"))
        {
            item.FindWalls();
        }
        if (GUILayout.Button("FindWindows"))
        {
            item.FindWindows();
        }
        if (GUILayout.Button("SetdWallsGPUIRoot"))
        {
            item.SetdWallsGPUIRoot();
        }
        if (GUILayout.Button("SaveToSetting"))
        {
            item.SaveToSetting();
        }
        if (GUILayout.Button("LoadSetting"))
        {
            item.LoadSetting();
        }
        if (GUILayout.Button("SortSetting"))
        {
            item.SortSetting();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("DeleteRootsAndLODs"))
        {
            item.DeleteRootsAndLODs();
        }
        if (GUILayout.Button("DeleteDoors"))
        {
            item.DeleteDoors();
        }
        if (GUILayout.Button("InstantiatePrefabs"))
        {
            item.InstantiatePrefabs();
        }
        if (GUILayout.Button("SetExternalMaterial"))
        {
            item.SetExternalMaterial();
        }
        if (GUILayout.Button("ReplaceMaterials"))
        {
            item.ReplaceMaterials();
        }
        //if (GUILayout.Button("SetTransparent"))
        //{
        //    item.SetMaterialTransparent();
        //}
        EditorGUILayout.EndHorizontal();
    }
}

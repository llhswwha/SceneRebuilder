using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorManager))]
public class DoorManagerEditor : BaseFoldoutEditor<DoorManager>
{
    FoldoutEditorArg doorRootListArg = new FoldoutEditorArg();

    FoldoutEditorArg doorListArg = new FoldoutEditorArg();

    FoldoutEditorArg doorPartListArg = new FoldoutEditorArg();
    FoldoutEditorArg prefabListArg = new FoldoutEditorArg();

    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();

        doorRootListArg=new FoldoutEditorArg(true, false, true, true, false);
        doorListArg = new FoldoutEditorArg(true, false, true, true, false);
        doorPartListArg = new FoldoutEditorArg(true, false, true, true, false);
        prefabListArg = new FoldoutEditorArg(true, false, true, true, false);
        sharedMeshListArg = new FoldoutEditorArg(true, false, true, true, false);

        doorPartListArg.isEnabled = true;
        targetT.LocalTarget = null;
    }

    public override void OnToolLayout(DoorManager item)
    {
        base.OnToolLayout(item);

        if (GUILayout.Button("Update"))
        {
            item.UpdateDoors(p=> 
            {
                ProgressBarHelper.DisplayCancelableProgressBar(p);
            });

            sharedMeshListArg.tag = item.GetSharedMeshList();

            ProgressBarHelper.ClearProgressBar();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CombineDoors"))
        {
            item.CombineDoors();
        }
        if (GUILayout.Button("DeleteOthers"))
        {
            item.DeleteOthersOfDoor();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetPrefabs"))
        {
            item.GetPrefabs();
        }
        if (GUILayout.Button("Apply"))
        {
            item.ApplyReplace();
        }
        if (GUILayout.Button("Revert"))
        {
            item.RevertReplace();
        }
        if (GUILayout.Button("ShowOri"))
        {
            item.ShowOri();
        }
        if (GUILayout.Button("ShowNew"))
        {
            item.ShowNew();
        }
        //if (GUILayout.Button("CombinePrefab"))
        //{
        //    item.CombinePrefab(item.IsAlign, item.IsReplace);
        //}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SetDoorPivot"))
        {
            item.SetDoorPivot();
        }
        if (GUILayout.Button("Split"))
        {
            item.Split();
        }
        if (GUILayout.Button("SetLOD"))
        {
            item.SetLOD();
        }
        if (GUILayout.Button("CopyPart"))
        {
            item.CopyPart();
        }

        if (GUILayout.Button("Prepare"))
        {
            item.Prepare();
        }
        if (GUILayout.Button("InitNodes"))
        {
            MeshNode.InitNodes(item.gameObject);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Align"))
        {
            item.AcRTAlignJobs();
        }
        if (GUILayout.Button("AlignEx"))
        {
            item.AcRTAlignJobsEx();
        }
        GUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);
        if (GUILayout.Button("RecoverDoors"))
        {
            item.RecoverDoors();
        }
        item.OldTarget = ObjectFieldH("OldTarget", item.OldTarget);
        item.LocalTarget = ObjectFieldH("LocalTarget", item.LocalTarget);

        DrawDoorsRootList(doorRootListArg, item);

        DrawDoorList(doorListArg, item.GetDoors(), false);

        DrawDoorPartList(doorPartListArg, item);

        DrawPrefabList(prefabListArg, () => item.prefabs);

        DrawSharedMeshListEx(sharedMeshListArg,()=> item.GetSharedMeshList());
    }
}

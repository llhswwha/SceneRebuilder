using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorsRoot))]
public class DoorsRootEditor : BaseFoldoutEditor<DoorsRoot>
{
    FoldoutEditorArg doorListArg = new FoldoutEditorArg();

    FoldoutEditorArg doorPartListArg = new FoldoutEditorArg();

    FoldoutEditorArg prefabListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();

        doorListArg = new FoldoutEditorArg(true, false, true, true, false);
        doorPartListArg = new FoldoutEditorArg(true, false, true, true, false);
        prefabListArg = new FoldoutEditorArg(true, false, true, true, false);

        doorPartListArg.isEnabled = true;
        DoorManager.Instance.LocalTarget = targetT.gameObject;
        DoorManager.Instance.UpdateDoors();
    }

    public override void OnToolLayout(DoorsRoot item)
    {
        base.OnToolLayout(item);

        //GUILayout.BeginHorizontal();
        //item.IsAlign=GUILayout.Toggle(item.IsAlign, "IsAlign");
        //item.IsReplace = GUILayout.Toggle(item.IsReplace, "IsReplace");
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetPrefab"))
        {
            item.SetDoorShared();
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
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            item.prefabs.Clear();
        }
        if (GUILayout.Button("DestroyIns"))
        {
            item.DestroyInstances();
        }
        if (GUILayout.Button("Reset"))
        {
            item.ResetPrefabs();
        }

        if (GUILayout.Button("CombinePrefab"))
        {
            item.CombinePrefab();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Split"))
        {
            item.Split();
        }
        if (GUILayout.Button("CopyPart"))
        {
            item.CopyPart();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("AcRTAlignJobs"))
        {
            item.AcRTAlignJobs();
        }
        if (GUILayout.Button("AcRTAlignJobsEx"))
        {
            item.AcRTAlignJobsEx();
        }
        GUILayout.EndHorizontal();

        DrawDoorList(doorListArg, item, false);

        DrawDoorPartList(doorPartListArg, DoorManager.Instance);

        DrawPrefabList(prefabListArg, () => item.prefabs);
    }
}

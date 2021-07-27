using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorManager))]
public class DoorManagerEditor : BaseFoldoutEditor<DoorManager>
{
    FoldoutEditorArg doorListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        doorListArg.isEnabled = true;
        targetT.LocalTarget = null;
    }

    public override void OnToolLayout(DoorManager item)
    {
        base.OnToolLayout(item);
        //if(GUILayout.Button("GetDoors"))
        //{
        //    item.GetDoors();
        //}

        //doorListArg.caption = $"Door List";
        //EditorUIUtils.ToggleFoldout(doorListArg, arg =>
        //{
        //    var doors = item.doorInfos;
        //    arg.caption = $"Door List ({doors.Count})";
        //    arg.info = $"{doors.VertexCount_Show/10000f:F0}/{doors.VertexCount/10000f:F0}";
        //    InitEditorArg(doors);
        //},
        //() =>
        //{
        //    if (GUILayout.Button("Update"))
        //    {
        //        RemoveEditorArg(item.doorInfos);
        //        InitEditorArg(item.GetDoors());
        //    }
        //});
        //if(doorListArg.isEnabled&& doorListArg.isExpanded)
        //{
        //    doorListArg.DrawPageToolbar(item.doorInfos, (door,i) =>
        //    {
        //        var arg = editorArgs[door];
        //        arg.caption = $"[{i + 1:00}] {door.GetTitle()}";
        //        arg.info = door.ToString();
        //        EditorUIUtils.ObjectFoldout(arg, door.DoorGo, () =>
        //        {
        //            if (GUILayout.Button("Split", GUILayout.Width(50)))
        //            {
        //                Debug.Log($"Split:{door.GetTitle()}");
        //                GameObject result = MeshCombineHelper.SplitByMaterials(door.DoorGo);
        //            }
        //        });
        //    });
        //}

        DrawDoorList(doorListArg, item);
    }
}

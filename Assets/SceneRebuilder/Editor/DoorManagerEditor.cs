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

    public override void OnEnable()
    {
        base.OnEnable();

        doorRootListArg=new FoldoutEditorArg(true, false, true, true, false);
        doorListArg = new FoldoutEditorArg(true, false, true, true, false);
        doorPartListArg = new FoldoutEditorArg(true, false, true, true, false);

        doorPartListArg.isEnabled = true;
        targetT.LocalTarget = null;
    }

    public override void OnToolLayout(DoorManager item)
    {
        base.OnToolLayout(item);

        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("GetPrefab"))
        //{
        //    item.SetDoorShared(item.IsAlign, item.IsReplace);
        //}
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

        EditorUIUtils.Separator(5);

        item.LocalTarget = ObjectField(item.LocalTarget);

        DrawDoorsRootList(doorRootListArg, item);
        DrawDoorPartList(doorPartListArg, item);
    }
}

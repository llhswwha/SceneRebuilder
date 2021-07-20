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
    }

    public override void OnToolLayout(DoorManager item)
    {
        base.OnToolLayout(item);
        if(GUILayout.Button("GetDoors"))
        {
            item.GetDoors();
        }

        doorListArg.caption = $"Door List";
        EditorUIUtils.ToggleFoldout(doorListArg, arg =>
        {
            arg.caption = $"Door List ({item.Doors.Count})";
            arg.info = $"{item.VertexCount_Show/10000f:F0}/{item.VertexCount/10000f:F0}";
            InitEditorArg(item.Doors);
        },
        () =>
        {

        });
        if(doorListArg.isEnabled&& doorListArg.isExpanded)
        {
            foreach (var door in item.Doors)
            {
                var arg = editorArgs[door];
                arg.caption = door.GetTitle();
                arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, door.DoorGo, ()=>
                {
                    if (GUILayout.Button("Split",GUILayout.Width(50)))
                    {
                        Debug.Log($"Split:{door.GetTitle()}");
                        GameObject result=MeshCombineHelper.SplitByMaterials(door.DoorGo);
                    }
                });
            }
        }
    }
}

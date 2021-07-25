using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODGroupInfo))]
public class LODGroupInfoEditor : BaseFoldoutEditor<LODGroupInfo>
{
    FoldoutEditorArg lodListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        targetT.GetLODs();

        lodListArg = new FoldoutEditorArg(true,true,true,true,true);
    }

    public override void OnToolLayout(LODGroupInfo item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GetLODs"))
        {
            item.GetLODs();
        }
        if (GUILayout.Button("SetLODs"))
        {
            item.SetLODs();
        }
        EditorGUILayout.EndHorizontal();

        lodListArg.caption = $"LOD List";
        EditorUIUtils.ToggleFoldout(lodListArg, arg =>
        {
            var lods = item.LodInfos;
            arg.caption = $"LOD List ({lods.Count})";
            //string txt = "";
            //float v0 = 0;
            //float sv = 0;
            //for (int i = 0; i < lods.Count; i++)
            //{
            //    var v = lods[i];
            //    if (i == 0)
            //    {
            //        v0 = v;
            //    }
            //    if (i <= 1)
            //    {
            //        txt += $"{v / 10000f:F0}({v / v0:P0})|";
            //    }
            //    else
            //    {
            //        txt += $"{v / 10000f:F1}({v / v0:P0})|";
            //    }

            //    sv += v;
            //}
            //arg.info = $"({sv / 10000f:F0}){txt}";
            InitEditorArg(lods);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
        });
        if (lodListArg.isEnabled && lodListArg.isExpanded)
        {
            var lods = item.LodInfos;
            InitEditorArg(lods);
            lodListArg.DrawPageToolbar(lods, (lod, i) =>
            {
                var arg = editorArgs[lod];
                arg.caption = $"[{i:00}] {lod.GetName()}";
                arg.info = $"{lod.vertextCount}";
                var renderer = lod.GetRenderer();
                EditorUIUtils.ObjectFoldout(arg, renderer, () =>
                {
                    lod.screenRelativeTransitionHeight=EditorGUILayout.FloatField(lod.screenRelativeTransitionHeight,GUILayout.Width(50));
                });
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;

[CustomEditor(typeof(RendererManager))]
public class RendererManagerEditor : BaseFoldoutEditor<RendererManager>
{
    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();

        sharedMeshListArg = GetGlobalEditorArg(targetT,new FoldoutEditorArg(true, true, true, true, true));
    }

    public override void OnToolLayout(RendererManager item)
    {
        base.OnToolLayout(item);

        if (GUILayout.Button("InitRenderers_All"))
        {
            item.InitRenderers_All();
        }

        if (GUILayout.Button("InitIds"))
        {
            item.InitIds();
        }

        if (GUILayout.Button("CheckRendererParent"))
        {
            item.CheckRendererParent();
        }
        if (GUILayout.Button("SetDetailRenderers"))
        {
            item.SetDetailRenderers();
        }

        EditorUIUtils.Separator(5);

        if (GUILayout.Button("UpdateMeshList"))
        {
            sharedMeshListArg.tag = item.GetSharedMeshList();
        }
        DrawSharedMeshListEx(sharedMeshListArg);
    }
}

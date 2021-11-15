using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;

[CustomEditor(typeof(RendererManager))]
public class RendererManagerEditor : BaseFoldoutEditor<RendererManager>
{
    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg(true,false);

    FoldoutEditorArg weldingRendererListArg = new FoldoutEditorArg(true, false);

    public override void OnEnable()
    {
        base.OnEnable();

        sharedMeshListArg = GetGlobalEditorArg(targetT,new FoldoutEditorArg(true, true, true, true, true));
    }

    public override void OnToolLayout(RendererManager item)
    {
        base.OnToolLayout(item);
        GUILayout.Label(item.TestId);
        if (GUILayout.Button("TestGetGo"))
        {
            item.TestGetGo();
        }
        item.TestGo=ObjectField(item.TestGo);
        EditorUIUtils.Separator(5);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("InitRenderers_All"))
        {
            item.InitRenderers_All();
        }

        if (GUILayout.Button("InitIds"))
        {
            item.InitIds();
        }
        if (GUILayout.Button("ShowAll"))
        {
            item.ShowAll();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CheckRendererParent"))
        {
            item.CheckRendererParent();
        }
        if (GUILayout.Button("SetDetailRenderers"))
        {
            item.SetDetailRenderers();
        }
        GUILayout.EndHorizontal();

        EditorUIUtils.Separator(5);

        //if (GUILayout.Button("UpdateMeshList"))
        //{
        //    sharedMeshListArg.tag = item.GetSharedMeshList();
        //}
        DrawSharedMeshListEx(sharedMeshListArg,()=> item.GetSharedMeshList());

        DrawObjectList
    }
}

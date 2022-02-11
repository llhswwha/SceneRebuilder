using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBuilder))]
public class PipeBuilderEditor : BaseFoldoutEditor<PipeBuilder>
{
    static PipeModelFoldoutEditorArg pipeModelListArg = new PipeModelFoldoutEditorArg(true,true);

    public override void OnInspectorGUI()
    {
        PipeBuilder targetT = target as PipeBuilder;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowOBB"))
        {
            targetT.ShowOBB();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("ClearOldGos"))
        {
            targetT.ClearGeneratedObjs();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateEachPipes"))
        {
            targetT.GetInfoAndCreateEachPipes();
        }
        targetT.UseOnlyEndPoint = GUILayout.Toggle(targetT.UseOnlyEndPoint, "OnlyEndPoint");
        targetT.IsGenerateElbowBeforeAfter = GUILayout.Toggle(targetT.IsGenerateElbowBeforeAfter, "ElbowBeforeAfter");
        if (GUILayout.Button("CreateOnePipe"))
        {
            targetT.CreateOnePipe();
        }

        //if (GUILayout.Button("CreateOnePipe1"))
        //{
        //    targetT.UseOnlyEndPoint = false;
        //    targetT.CreateOnePipe();
        //}
        //if (GUILayout.Button("CreateOnePipe2"))
        //{
        //    targetT.UseOnlyEndPoint = true;
        //    targetT.CreateOnePipe();
        //}

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("FindPipeModels"))
        {
            targetT.FindPipeModels(null);
        }
        if (GUILayout.Button("TestCreateOnePipe"))
        {
            targetT.TestCreateOnePipe();
        }
        GUILayout.EndHorizontal();

        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}

        DrawPipeModelsList(targetT.PipeModels, pipeModelListArg,"PipeModel List");
      
        base.OnInspectorGUI();
    }

    
}

public class PipeModelFoldoutEditorArg: FoldoutEditorArg
{
    public PipeModelFoldoutEditorArg():base()
    {

    }

    public PipeModelFoldoutEditorArg(bool isEnabled, bool isExpanded) : base(isEnabled, isExpanded)
    {
    }

    public PipeModelFoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle) : base(isEnabled, isExpanded, isToggle)
    {
    }

    public PipeModelFoldoutEditorArg(bool isEnabled, bool isExpanded, bool isToggle, bool separator, bool background) : base(isEnabled, isExpanded, isToggle,separator,background)
    {
    }

    public bool ShowCheckResult = false;

    public bool ShowPipeArg = true;

    public bool OnlySpecial = false;

    internal List<TM> FilterList<TM>(List<TM> list) where TM : PipeModelBase
    {
        List<TM> list2 = new List<TM>();
        if (OnlySpecial)
        {
            foreach(var item in list)
            {
                if (item.IsSpecial)
                {
                    list2.Add(item);
                }
            }
        }
        else
        {
            list2 = list;
        }
        return list2;
    }
}

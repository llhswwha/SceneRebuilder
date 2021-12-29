using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBuilder))]
public class PipeBuilderEditor : BaseFoldoutEditor<PipeBuilder>
{
    static FoldoutEditorArg pipeModelListArg = new FoldoutEditorArg(true,true);

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
        if (GUILayout.Button("TestCreateOnePipe"))
        {
            targetT.TestCreateOnePipe();
        }
        GUILayout.EndHorizontal();

        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}

        DrawPipeModelsList(targetT.PipeModels, pipeModelListArg);
      
        base.OnInspectorGUI();
    }

    
}

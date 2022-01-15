using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFactory))]
public class PipeFactoryEditor : BaseFoldoutEditor<PipeFactory>
{
    static FoldoutEditorArg pipeModelListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg pipeRunListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg testpipeRunListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg specialElbowListArg = new FoldoutEditorArg(true, false);
    public override void OnEnable()
    {
        base.OnEnable();

        pipeSegmentsValuesStr = new string[pipeSegmentsValues.Length];
        for (int i = 0; i < pipeSegmentsValues.Length; i++)
        {
            pipeSegmentsValuesStr[i] = pipeSegmentsValues[i].ToString();
        }

        elbowSegmentsValuesStr = new string[elbowSegmentsValues.Length];
        for (int i = 0; i < elbowSegmentsValues.Length; i++)
        {
            elbowSegmentsValuesStr[i] = elbowSegmentsValues[i].ToString();
        }
    }

    int defaultPipeSegments = 8;

    int[] pipeSegmentsValues = new int[] { 3,4,5,6,7, 8,10,12, 16, 20, 24, 32,36,48,64 };

    string[] pipeSegmentsValuesStr = null;

    int defaultElbowSegments = 6;

    int[] elbowSegmentsValues = new int[] { 3,4,5,6,7,8,9,10,11,12,13,14,15 };

    string[] elbowSegmentsValuesStr = null;

    public override void OnToolLayout(PipeFactory targetT)
    {
        base.OnToolLayout(targetT);

        GUILayout.BeginHorizontal();
        targetT.Target = ObjectField("Target", targetT.Target);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("generateWeld");
        targetT.generateArg.generateWeld = EditorGUILayout.Toggle(targetT.generateArg.generateWeld);
        GUILayout.Label("UniformRaidus");
        targetT.isUniformRaidus = EditorGUILayout.Toggle(targetT.isUniformRaidus);
        GUILayout.Label("pipeSegments");

        //int newPipeSegments = EditorGUILayout.IntField(targetT.generateArg.pipeSegments);
        //if (newPipeSegments != targetT.generateArg.pipeSegments)
        //{
        //    targetT.generateArg.pipeSegments = newPipeSegments;
        //}

        //defaultPipeSegments= EditorGUILayout.IntPopup(defaultPipeSegments, pipeSegmentsValuesStr, pipeSegmentsValues);
        //Debug.Log("defaultPipeSegments:"+ defaultPipeSegments);
        //targetT.generateArg.pipeSegments = defaultPipeSegments;

        //defaultPipeSegments = EditorGUILayout.IntPopup(defaultPipeSegments, pipeSegmentsValuesStr, pipeSegmentsValues);
        //Debug.Log("defaultPipeSegments:" + defaultPipeSegments);
        targetT.generateArg.pipeSegments = EditorGUILayout.IntPopup(targetT.generateArg.pipeSegments, pipeSegmentsValuesStr, pipeSegmentsValues);

        GUILayout.Label("elbowSegments");
        //targetT.generateArg.elbowSegments = EditorGUILayout.IntField(targetT.generateArg.elbowSegments);

        targetT.generateArg.elbowSegments = EditorGUILayout.IntPopup(targetT.generateArg.elbowSegments, elbowSegmentsValuesStr, elbowSegmentsValues);

        GUILayout.Label("Info:");
        GUILayout.Label(targetT.GetResultInfo(), GUILayout.Width(300));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Line({targetT.PipeLines.Count})");
        targetT.EnablePipeLine = EditorGUILayout.Toggle(targetT.EnablePipeLine);
        GUILayout.Label($"Elbow({targetT.PipeElbows.Count})");
        targetT.EnablePipeElbow = EditorGUILayout.Toggle(targetT.EnablePipeElbow);
        GUILayout.Label($"Tee({targetT.PipeTees.Count})");
        targetT.EnablePipeTee = EditorGUILayout.Toggle(targetT.EnablePipeTee);
        GUILayout.Label($"Reducer({targetT.PipeReducers.Count})");
        targetT.EnablePipeReducer = EditorGUILayout.Toggle(targetT.EnablePipeReducer);
        GUILayout.Label($"Flange({targetT.PipeFlanges.Count})");
        targetT.EnablePipeFlange = EditorGUILayout.Toggle(targetT.EnablePipeFlange);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1.GetParts"))
        {
            targetT.GetPipeParts();
        }
        if (GUILayout.Button("2.GetInfos"))
        {
            targetT.GetPipeInfos();
        }
        if (GUILayout.Button("3.Generate(Each)"))
        {

            targetT.ClearGeneratedObjs();
;
            targetT.RendererEachPipes();
            targetT.MovePipes();
        }
        if (GUILayout.Button("3.Generate(One)"))
        {

        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("MoveOthers"))
        {
            targetT.PrefabOthers();
        }
        if (GUILayout.Button("RecoverOthers"))
        {
            targetT.PrefabOthers();
        }
        if (GUILayout.Button("5.PrefabOthers"))
        {
            targetT.PrefabOthers();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("4.CreateRunList"))
        {
            targetT.CreateRunList();
        }
        if (GUILayout.Button("5.CheckResults"))
        {
            targetT.CheckResults();
        }
        if (GUILayout.Button("6.SetOtherPrefabs"))
        {
            targetT.SetOtherPrefabs();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetObbs"))
        {
            targetT.GetObbInfos();
        }
        if (GUILayout.Button("GetObbs(Job)"))
        {
            targetT.GetObbInfosJob();
        }
        if (GUILayout.Button("GetInfos(Job)"))
        {
            targetT.GetPipeInfosJob();
        }
        if (GUILayout.Button("CheckResults(Job)"))
        {
            targetT.CheckResultsJob();
           
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OneKey_GeneratePipe(Each)"))
        {
            targetT.GetPipeParts();
            targetT.GetInfoAndCreateEachPipes();
        }
        if (GUILayout.Button("OneKey_GeneratePipe(One)"))
        {

        }
        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearDebugObjs"))
        {
            targetT.ClearDebugObjs();
        }
        if (GUILayout.Button("MovePipes"))
        {
            targetT.MovePipes();
        }
        if (GUILayout.Button("TestIsConnected"))
        {
            targetT.TestModelIsConnected();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowAll"))
        {
            targetT.ShowAll();
        }
        if (GUILayout.Button("HidAll"))
        {
            targetT.HidAll();
        }
        if (GUILayout.Button("OnlyShowPipe"))
        {
            targetT.OnlyShowPipe();
        }
        GUILayout.EndHorizontal();

        DrawPipeModelsList(targetT.GetPipeModels(), pipeModelListArg, "PipeModel List");
        DrawPipeRunList(targetT.GetPipeRunList(), pipeRunListArg);
        DrawPipeModelsList(targetT.GetPipeRunList().SpecialElbows, specialElbowListArg, "SpecialElbow List");
        DrawPipeRunList(targetT.TestRunList, testpipeRunListArg);
    }
}

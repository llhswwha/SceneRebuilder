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

        pipeSegmentsValuesStr = GetIntArrayStrings(pipeSegmentsValues);
        elbowSegmentsValuesStr = GetIntArrayStrings(elbowSegmentsValues);

        pipeSegmentsValuesStr_weld = GetIntArrayStrings(pipeSegmentsValues_weld);
        elbowSegmentsValuesStr_weld = GetIntArrayStrings(elbowSegmentsValues_weld);

        minPipeSegmentsValuesStr = GetIntArrayStrings(minPipeSegmentsValues);
    }

    public static string[] GetIntArrayStrings(int[] list)
    {
        string[] sList = new string[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            sList[i] = list[i].ToString();
        }
        return sList;
    }

    int[] pipeSegmentsValues = new int[] { 3,4,5,6,7, 8,10,12, 16, 20, 24, 32,36,48,64 };

    string[] pipeSegmentsValuesStr = null;

    int[] elbowSegmentsValues = new int[] { 3,4,5,6,7,8,9,10,11,12,13,14,15, 16 };

    string[] elbowSegmentsValuesStr = null;

    int[] pipeSegmentsValues_weld = new int[] { 3, 4, 5, 6, 7, 8, 10, 12, 16, 20, 24, 32, 36, 48, 64 };

    string[] pipeSegmentsValuesStr_weld = null;

    int[] elbowSegmentsValues_weld = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    string[] elbowSegmentsValuesStr_weld = null;

    int[] minPipeSegmentsValues = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,16 };

    string[] minPipeSegmentsValuesStr = null;

    public override void OnToolLayout(PipeFactory targetT)
    {
        base.OnToolLayout(targetT);

        GUILayout.BeginHorizontal();
        targetT.Target = ObjectField("Target", targetT.Target);
        GUILayout.Label("Info:");
        GUILayout.Label(targetT.GetResultInfo(), GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("创建焊缝");
        targetT.generateArg.generateWeld = EditorGUILayout.Toggle(targetT.generateArg.generateWeld);
        GUILayout.Label("统一半径");
        targetT.isUniformRaidus = EditorGUILayout.Toggle(targetT.isUniformRaidus);
        GUILayout.Label("管道边数");
        targetT.generateArg.pipeSegments = EditorGUILayout.IntPopup(targetT.generateArg.pipeSegments, pipeSegmentsValuesStr, pipeSegmentsValues);
        GUILayout.Label("弯管段数");
        targetT.generateArg.elbowSegments = EditorGUILayout.IntPopup(targetT.generateArg.elbowSegments, elbowSegmentsValuesStr, elbowSegmentsValues);
        GUILayout.Label("焊缝边数");
        targetT.generateArg.weldPipeSegments = EditorGUILayout.IntPopup(targetT.generateArg.weldPipeSegments, pipeSegmentsValuesStr_weld, pipeSegmentsValues_weld);
        GUILayout.Label("焊缝段数");
        targetT.generateArg.weldElbowSegments = EditorGUILayout.IntPopup(targetT.generateArg.weldElbowSegments, elbowSegmentsValuesStr_weld, elbowSegmentsValues_weld);
        GUILayout.Label("最小边数");
        targetT.MinPipeSegments = EditorGUILayout.IntPopup(targetT.MinPipeSegments, minPipeSegmentsValuesStr, minPipeSegmentsValues);



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
        GUILayout.Label($"Welds({targetT.PipeWelds.Count})");
        GUILayout.Label($"Others({targetT.PipeOthers.Count})");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label($"CreateRuns");
        targetT.IsCreatePipeRuns = EditorGUILayout.Toggle(targetT.IsCreatePipeRuns);
        GUILayout.Label($"MoveToFac");
        targetT.IsMoveResultToFactory = EditorGUILayout.Toggle(targetT.IsMoveResultToFactory);
        GUILayout.Label($"SaveMats");
        targetT.IsSaveMaterials = EditorGUILayout.Toggle(targetT.IsSaveMaterials);
        GUILayout.Label($"CopyComps");
        targetT.IsCopyComponents = EditorGUILayout.Toggle(targetT.IsCopyComponents);
        GUILayout.Label($"CheckResult");
        targetT.IsCheckResult = EditorGUILayout.Toggle(targetT.IsCheckResult);
        GUILayout.Label($"ReplaceOld");
        targetT.IsReplaceOld = EditorGUILayout.Toggle(targetT.IsReplaceOld);
        GUILayout.Label($"PrefabGos");
        targetT.IsPrefabGos = EditorGUILayout.Toggle(targetT.IsPrefabGos);
        GUILayout.EndHorizontal();

        //public bool IsMoveResultToFactory = true;
        //public bool IsSaveMaterials = true;
        //public bool IsSaveComponents = true;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OneKey"))
        {
            targetT.OneKey(false);
        }
        if (GUILayout.Button("OneKey(Job)"))
        {
            targetT.OneKey(true);
        }
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
        if (GUILayout.Button("3.Generate"))
        {
            targetT.ClearGeneratedObjs();
            targetT.RendererEachPipes();
            targetT.MovePipes();
        }
        if (GUILayout.Button("4.CheckResults"))
        {
            targetT.CheckResults();
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("5.ReplacePipes"))
        {
            targetT.ReplacePipes();
        }
        if (GUILayout.Button("6.ReplaceWelds"))
        {
            targetT.ReplaceWelds();
        }
        if (GUILayout.Button("7.PrefabPipes"))
        {
            targetT.PrefabPipes();
        }
        if (GUILayout.Button("8.PrefabOthers"))
        {
            targetT.PrefabOthers();
        }
        if (GUILayout.Button("9.PrefabWelds"))
        {
            targetT.PrefabWelds();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("GetInfos(Job)"))
        {
            targetT.GetPipeInfosJob();
        }
        if (GUILayout.Button("CheckResults(Job)"))
        {
            targetT.CheckResultsJob();
        }

        if (GUILayout.Button("GetObbs"))
        {
            targetT.GetObbInfos();
        }
        if (GUILayout.Button("GetObbs(Job)"))
        {
            targetT.GetObbInfosJob();
        }
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("OneKey_GeneratePipe(Each)"))
        //{
        //    targetT.GetPipeParts();
        //    targetT.GetInfoAndCreateEachPipes();
        //}
        //if (GUILayout.Button("OneKey_GeneratePipe(One)"))
        //{

        //}
        //GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowPrefabs"))
        {
            targetT.ShowAllPrefabs();
        }
        if (GUILayout.Button("ClearDebugObjs"))
        {
            targetT.ClearDebugObjs();
        }
        if (GUILayout.Button("MovePipes"))
        {
            targetT.MovePipes();
        }
        if (GUILayout.Button("TestRun"))
        {
            targetT.TestModelIsConnected();
        }
        if (GUILayout.Button("CreateRuns"))
        {
            targetT.CreateRunList();
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
        if (GUILayout.Button("MoveOthers"))
        {
            targetT.MoveOthersParent();
        }
        if (GUILayout.Button("RecoverOthers"))
        {
            targetT.RecoverOthersParent();
        }
        GUILayout.EndHorizontal();

        DrawPipeModelsList(targetT.GetPipeModels(), pipeModelListArg, "PipeModel List");
        DrawPipeRunList(targetT.GetPipeRunList(), pipeRunListArg);
        DrawPipeModelsList(targetT.GetPipeRunList().SpecialElbows, specialElbowListArg, "SpecialElbow List");
        DrawPipeRunList(targetT.TestRunList, testpipeRunListArg);
    }
}

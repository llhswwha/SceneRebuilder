using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFactory))]
public class PipeFactoryEditor : BaseFoldoutEditor<PipeFactory>
{
    static PipeModelFoldoutEditorArg pipeModelListArg = new PipeModelFoldoutEditorArg(true, false);
    static FoldoutEditorArg pipeRunListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg testpipeRunListArg = new FoldoutEditorArg(true, false);

    internal static void DrawUI(PipeFactory targetT)
    {
        GUILayout.BeginHorizontal();
        targetT.Target = ObjectFieldS("Target", targetT.Target);
        targetT.generateArg.pipeMaterial = ObjectFieldS("PipeMat", targetT.generateArg.pipeMaterial);
        targetT.generateArg.weldMaterial = ObjectFieldS("WeldMat", targetT.generateArg.weldMaterial);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("RenderOnStart", GUILayout.Width(90));
        targetT.IsRendererOnStart = EditorGUILayout.Toggle(targetT.IsRendererOnStart, GUILayout.Width(15));
        GUILayout.Label("LoadXmlOnStart", GUILayout.Width(90));
        targetT.IsLoadXmlOnStart = EditorGUILayout.Toggle(targetT.IsLoadXmlOnStart, GUILayout.Width(15));
        GUILayout.Label($"UnitPrefab");
        targetT.IsCreatePipeByUnityPrefab = EditorGUILayout.Toggle(targetT.IsCreatePipeByUnityPrefab);

        GUILayout.Label("统一半径");
        targetT.isUniformRaidus = EditorGUILayout.Toggle(targetT.isUniformRaidus);
        GUILayout.Label("最小边数");
        if (minPipeSegmentsValuesStr == null)
            minPipeSegmentsValuesStr = GetIntArrayStrings(minPipeSegmentsValues);
        targetT.MinPipeSegments = EditorGUILayout.IntPopup(targetT.MinPipeSegments, minPipeSegmentsValuesStr, minPipeSegmentsValues, GUILayout.Width(35));

        GUILayout.Label("Info:");
        GUILayout.Label(targetT.GetResultInfo(), GUILayout.Width(300));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();


        if (generateArgEditorValues == null) generateArgEditorValues = new PipeGenerateArgEditorValues();
        DrawGenerateArg(targetT.generateArg, generateArgEditorValues);



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
        GUILayout.Label($"Weldolet({targetT.PipeWeldolets.Count})");
        targetT.EnablePipeWeldolet = EditorGUILayout.Toggle(targetT.EnablePipeWeldolet);
        GUILayout.Label($"Welds({targetT.PipeWelds.Count})");
        GUILayout.Label($"Others({targetT.PipeOthers.Count})");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label($"IsRun");
        targetT.IsCreatePipeRuns = EditorGUILayout.Toggle(targetT.IsCreatePipeRuns);
        GUILayout.Label($"IsMove");
        targetT.IsMoveResultToFactory = EditorGUILayout.Toggle(targetT.IsMoveResultToFactory);

        GUILayout.Label($"SaveMats");
        targetT.IsSaveMaterials = EditorGUILayout.Toggle(targetT.IsSaveMaterials);
        GUILayout.Label($"CopyComps");
        targetT.IsCopyComponents = EditorGUILayout.Toggle(targetT.IsCopyComponents);
        GUILayout.Label($"IsCheck");
        targetT.IsCheckResult = EditorGUILayout.Toggle(targetT.IsCheckResult);

        GUILayout.Label($"IsPrefab");
        targetT.IsPrefabGos = EditorGUILayout.Toggle(targetT.IsPrefabGos);
        GUILayout.Label($"(PrefabOthers");
        targetT.IsPrefabOthers = EditorGUILayout.Toggle(targetT.IsPrefabOthers);
        GUILayout.Label($"PrefabOldWeld");
        targetT.IsPrefabOldWeld = EditorGUILayout.Toggle(targetT.IsPrefabOldWeld);
        GUILayout.Label($")");

        GUILayout.Label($"IsReplace");
        targetT.IsReplaceOld = EditorGUILayout.Toggle(targetT.IsReplaceOld);

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
        if (GUILayout.Button("OneKeyEx(Job)"))
        {
            targetT.OneKeyEx(true);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("0.Clear"))
        {
            targetT.ClearGeneratedObjs();
        }
        if (GUILayout.Button("1.GetParts"))
        {
            targetT.GetPipeParts();
        }
        if (GUILayout.Button("2.GetInfos"))
        {
            targetT.GetPipeInfos();
        }
        if (GUILayout.Button("2.GetInfos(Job)"))
        {
            targetT.GetPipeInfosJob();
        }

        if (GUILayout.Button("3.Generate"))
        {
            targetT.RendererEachPipesEx();
        }

        if (GUILayout.Button("4.Check"))
        {
            targetT.CheckResults();
        }
        if (GUILayout.Button("4.Check(Job)"))
        {
            targetT.CheckResultsJob();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("RemComps"))
        {
            targetT.RemoveComponents();
        }
        if (GUILayout.Button("RemModels"))
        {
            targetT.RemovePipeModels();
        }
        if (GUILayout.Button("RemMeshes"))
        {
            targetT.RemoveMeshes();
        }
        if (GUILayout.Button("(Elbow "))
        {
            targetT.PrefabElbows();
        }
        if (GUILayout.Button(" Tee"))
        {
            targetT.PrefabTees();
        }
        if (GUILayout.Button(" Reducer"))
        {
            targetT.PrefabReducers();
        }
        if (GUILayout.Button(" Flange"))
        {
            targetT.PrefabFlanges();
        }
        if (GUILayout.Button(" Weldolet)"))
        {
            targetT.PrefabWeldolets();
        }
        //if (GUILayout.Button("ClearResult"))
        //{
        //    targetT.ClearGeneratedObjs();
        //}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("5.PrefabPipes"))
        {
            targetT.PrefabPipes();
        }

        if (GUILayout.Button("6.PrefabOthers"))
        {
            targetT.PrefabOthers();
        }
        if (GUILayout.Button("7.CombineWelds"))
        {
            targetT.CombineGeneratedWelds();
        }
        if (GUILayout.Button("8.PrefabWelds"))
        {
            targetT.PrefabWelds();
        }

        if (GUILayout.Button("9.ReplacePipes"))
        {
            targetT.ReplacePipes();
        }
        if (GUILayout.Button("10.ReplaceWelds"))
        {
            targetT.ReplaceWelds();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SaveXml"))
        {
            targetT.SaveSceneDataXml();
        }

        if (GUILayout.Button("FindModels"))
        {
            targetT.FindPipeModels();
        }
        if (GUILayout.Button("ShowModels"))
        {
            targetT.ShowPipeModels();
        }
        if (GUILayout.Button("LoadXml"))
        {
            targetT.LoadSceneDataXml();
        }
        if (GUILayout.Button("ClearXml"))
        {
            targetT.ClearGeneratedObjs();
            targetT.RemoveComponents();
            targetT.RemovePipeModels();
            targetT.RemoveMeshes();
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
        if (GUILayout.Button("GetObbs"))
        {
            targetT.GetObbInfos();
        }
        if (GUILayout.Button("GetObbs(Job)"))
        {
            targetT.GetObbInfosJob();
        }

        if (GUILayout.Button("ClearGeneratorsMesh"))
        {
            targetT.ClearGeneratorsMesh();
        }
        if (GUILayout.Button("ResetGeneratorsMesh"))
        {
            targetT.ResetGeneratorsMesh();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowPrefabs"))
        {
            targetT.ShowPrefabs();
        }
        if (GUILayout.Button("AlignPrefabs"))
        {
            targetT.AlignDirectionPrefabs();
        }

        if (GUILayout.Button("ClearWeldPrefabs"))
        {
            targetT.ClearWeldPrefabs();
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
        if (GUILayout.Button("DestroyOthers"))
        {
            targetT.DestroyOthers();
        }
        GUILayout.EndHorizontal();

        DrawPipeModelsList(targetT.GetPipeModels(), pipeModelListArg, "PipeModel List");
        DrawPipeRunList(targetT.GetPipeRunList(), pipeRunListArg);
        DrawPipeModelsList(targetT.GetPipeRunList().SpecialElbows, specialElbowListArg, "SpecialElbow List");
        DrawPipeRunList(targetT.TestRunList, testpipeRunListArg);
        DrawObjectList(pipeWeldPrefabDataListArg, "WeldPrefabDatas", targetT.PipeModelUnitPrefab_Welds_Datas, null, null, null);
        DrawObjectList(pipeWeldPrefabListArg, "WeldPrefabs", targetT.PipeModelUnitPrefab_Welds, null, null, null);
        DrawObjectList(pipeWeldPrefabMeshListArg, "WeldMeshPrefabs", targetT.PipeModelUnitPrefabMesh_Welds, null, null, null);
        EditorUIUtils.Separator(5);
        DrawObjectList(lineListArg, "Lines", targetT.PipeLines, null, null, null);
        DrawObjectList(elbowListArg, "Elbows", targetT.PipeElbows, null, null, null);
        DrawObjectList(teeListArg, "Tees", targetT.PipeTees, null, null, null);
        DrawObjectList(flangeListArg, "Flanges", targetT.PipeFlanges, null, null, null);
        DrawObjectList(reducerListArg, "Reducers", targetT.PipeReducers, null, null, null);
        DrawObjectList(weldoletListArg, "Weldolets", targetT.PipeWeldolets, null, null, null);
        DrawObjectList(weldListArg, "Welds", targetT.PipeWelds, null, null, null);
        DrawObjectList(othersListArg, "Others", targetT.PipeOthers, null, null, null);
    }
    static FoldoutEditorArg pipeWeldPrefabDataListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg pipeWeldPrefabMeshListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg pipeWeldPrefabListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg othersListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg weldListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg flangeListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg lineListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg elbowListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg teeListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg reducerListArg = new FoldoutEditorArg(true, false);
    static FoldoutEditorArg weldoletListArg = new FoldoutEditorArg(true, false);
    static PipeModelFoldoutEditorArg specialElbowListArg = new PipeModelFoldoutEditorArg(true, false);
    public override void OnEnable()
    {
        base.OnEnable();

        minPipeSegmentsValuesStr = GetIntArrayStrings(minPipeSegmentsValues);
    }

    public class PipeGenerateArgEditorValues
    {
        public int[] pipeSegmentsValues = new int[] { 3, 4, 5, 6, 7, 8, 10, 12, 16, 20, 24, 32, 36, 48, 64 };

        public string[] pipeSegmentsValuesStr = null;

        public int[] elbowSegmentsValues = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        public string[] elbowSegmentsValuesStr = null;

        public int[] pipeSegmentsValues_weld = new int[] { 3, 4, 5, 6, 7, 8, 10, 12, 16, 20, 24, 32, 36, 48, 64 };

        public string[] pipeSegmentsValuesStr_weld = null;

        public int[] elbowSegmentsValues_weld = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        public string[] elbowSegmentsValuesStr_weld = null;



        public int[] uniformRadiusPValues = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        public string[] uniformRadiusPValuesStr = null;

        public PipeGenerateArgEditorValues()
        {
            pipeSegmentsValuesStr = GetIntArrayStrings(pipeSegmentsValues);
            elbowSegmentsValuesStr = GetIntArrayStrings(elbowSegmentsValues);

            pipeSegmentsValuesStr_weld = GetIntArrayStrings(pipeSegmentsValues_weld);
            elbowSegmentsValuesStr_weld = GetIntArrayStrings(elbowSegmentsValues_weld);

            
            uniformRadiusPValuesStr = GetIntArrayStrings(uniformRadiusPValues);
        }


    }

    public static PipeGenerateArgEditorValues generateArgEditorValues;

    public static int[] minPipeSegmentsValues = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    public static string[] minPipeSegmentsValuesStr = null;

    public static string[] GetIntArrayStrings(int[] list)
    {
        string[] sList = new string[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            sList[i] = list[i].ToString();
        }
        return sList;
    }

    public static void DrawGenerateArg(PipeGenerateArg generateArg, PipeGenerateArgEditorValues vl)
    {
        if (generateArg == null) return;
        if (vl == null) return;
        GUILayout.Label("创建焊缝");
        generateArg.generateWeld = EditorGUILayout.Toggle(generateArg.generateWeld);

        int width = 35;
        GUILayout.Label("管道边数");
        generateArg.pipeSegments = EditorGUILayout.IntPopup(generateArg.pipeSegments, vl.pipeSegmentsValuesStr, vl.pipeSegmentsValues, GUILayout.Width(width));
        GUILayout.Label("弯管段数");
        generateArg.elbowSegments = EditorGUILayout.IntPopup(generateArg.elbowSegments, vl.elbowSegmentsValuesStr, vl.elbowSegmentsValues, GUILayout.Width(width));
        GUILayout.Label("焊缝边数");
        generateArg.weldPipeSegments = EditorGUILayout.IntPopup(generateArg.weldPipeSegments, vl.pipeSegmentsValuesStr_weld, vl.pipeSegmentsValues_weld, GUILayout.Width(width));
        GUILayout.Label("焊缝段数");
        generateArg.weldElbowSegments = EditorGUILayout.IntPopup(generateArg.weldElbowSegments, vl.elbowSegmentsValuesStr_weld, vl.elbowSegmentsValues_weld, GUILayout.Width(width));
        GUILayout.Label("半径精度");
        generateArg.uniformRadiusP = EditorGUILayout.IntPopup(generateArg.uniformRadiusP, vl.uniformRadiusPValuesStr, vl.uniformRadiusPValues, GUILayout.Width(width));
        
    }

    public override void OnToolLayout(PipeFactory targetT)
    {
        base.OnToolLayout(targetT);
        DrawUI(targetT);
    }
}

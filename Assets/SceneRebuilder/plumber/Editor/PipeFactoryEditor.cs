using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFactory))]
public class PipeFactoryEditor : BaseFoldoutEditor<PipeFactory>
{
    static FoldoutEditorArg pipeModelListArg = new FoldoutEditorArg(true, true);

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
        if (GUILayout.Button("1.GetPipeParts"))
        {
            targetT.GetPipeParts();
        }
        if (GUILayout.Button("2.GetPartInfos"))
        {
            targetT.GetPipeInfos();
        }
        if (GUILayout.Button("3.GeneratePipe(Each)"))
        {
            targetT.ClearGeneratedObjs();
            targetT.RendererEachPipes();
            targetT.MovePipes();
        }
        if (GUILayout.Button("3.GeneratePipe(One)"))
        {

        }
        if (GUILayout.Button("4.CheckResults"))
        {
            targetT.CheckResults();
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

        DrawPipeModelsList(targetT.GetPipeModels(), pipeModelListArg);
    }
}

using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBuilder))]
public class PipeBuilderEditor : BaseFoldoutEditor<PipeBuilder>
{
    FoldoutEditorArg pipeModelListArg = new FoldoutEditorArg(true,true);

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
            targetT.ClearOldGos();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateEachPipes"))
        {
            targetT.CreateEachPipes();
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

    public void DrawPipeModelsList(List<PipeModelBase> list, FoldoutEditorArg listArg)
    {
        listArg.caption = $"PipeModel List";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            //var doors = doorsRoot.Doors;
            arg.caption = $"PipeModel List ({list.Count})";
            InitEditorArg(list);
        },
        () =>
        {
        });

        if (listArg.isExpanded && listArg.isEnabled)
        {
            listArg.level = 1;
            //var doors = doorsRoot.Doors;
            InitEditorArg(list);
            listArg.DrawPageToolbar(list, (listItem, i) =>
            {
                var arg = FoldoutEditorArgBuffer.editorArgs[listItem];
                arg.caption = $"[{i + 1:00}] {listItem.name}";
                arg.info = listItem.ToString();
                arg.level = 2;
                EditorUIUtils.ObjectFoldout(arg, listItem.gameObject, () =>
                {

                });
            });
        }

    }
}

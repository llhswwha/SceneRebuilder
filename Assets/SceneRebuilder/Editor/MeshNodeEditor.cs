using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshNode))]
public class MeshNodeEditor : BaseFoldoutEditor<MeshNode>
{
    FoldoutEditorArg meshnodeListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        meshnodeListArg = new FoldoutEditorArg(true,true,true,true,true);
    }

    public override void OnToolLayout(MeshNode item)
    {
        base.OnToolLayout(item);

        GUILayout.Label($"vertexCount:{MeshHelper.GetVertexCountS(item.VertexCount)}");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Init"))
        {
            //DateTime start = DateTime.Now;
            //item.Init(0,true,p=>
            //{
            //    ProgressBarHelper.DisplayProgressBar("MeshNode.Init", $"{p:P2}", p);
            //});

            //var meshNodes = item.GetComponentsInChildren<MeshNode>(true);
            //ProgressBarHelper.ClearProgressBar();
            //Debug.Log($"MeshNode.Init count:{meshNodes.Length} time:{(DateTime.Now - start)}");
            MeshNode.InitNodes(item.gameObject);
        }
        if (GUILayout.Button("Refresh"))
        {
            item.RefreshInfo();
        }
        if (GUILayout.Button("Clear"))
        {
            //item.RefreshInfo();
            var meshNodes = item.GetComponentsInChildren<MeshNode>(true);
            foreach(var node in meshNodes)
            {
                GameObject.DestroyImmediate(node);
            }
        }

        if (GUILayout.Button("GetSum"))
        {
            item.GetSumVertexCount();
        }
        EditorGUILayout.EndHorizontal();

        meshnodeListArg.caption = $"MeshNode List";
        EditorUIUtils.ToggleFoldout(meshnodeListArg, arg =>
        {
            var subms = item.subMeshes;
            arg.caption = $"MeshNode List ({subms.Count})";
            //arg.info = $"({sv / 10000f:F0}){txt}";
            InitEditorArg(subms);
        },
        () =>
        {
        });
        //if (meshnodeListArg.isEnabled && meshnodeListArg.isExpanded)
        //{
        //    InitEditorArg(item.subMeshes);
        //    meshnodeListArg.DrawPageToolbar(item.subMeshes, (node, i) =>
        //    {
        //        var arg = editorArgs[node];
        //        arg.caption = $"[{i:00}] {node.name}";
        //        arg.info = $"{MeshHelper.GetVertexCountS(node.VertexCount)}[{node.VertexCount/(float)item.VertexCount:P1}]";
        //        EditorUIUtils.ObjectFoldout(arg, node.gameObject, () =>
        //        {

        //        });

        //        if (arg.isEnabled && arg.isExpanded)
        //        {
        //            InitEditorArg(node.subMeshes);

        //        }
        //    });
        //}
        DrawMeshNodeList(meshnodeListArg, item,0);
    }

    private void DrawMeshNodeList(FoldoutEditorArg meshnodeListArg, MeshNode item,int level)
    {
        meshnodeListArg.level = level;
        if (meshnodeListArg.isEnabled && meshnodeListArg.isExpanded)
        {
            InitEditorArg(item.subMeshes);
            meshnodeListArg.DrawPageToolbar(item.subMeshes, (node, i) =>
            {
                var arg = editorArgs[node];
                arg.caption = $"[{i:00}] {node.name} ({node.subMeshes.Count})";
                arg.isEnabled = true;
                arg.info = $"{MeshHelper.GetVertexCountS(node.VertexCount)}[{node.VertexCount / (float)item.VertexCount:P1}]";
                if (level == 0)
                {
                    arg.background = true;
                }
                EditorUIUtils.ObjectFoldout(arg, node.gameObject, () =>
                {

                });

                DrawMeshNodeList(arg, node,level+1);
            });
        }
    }
}

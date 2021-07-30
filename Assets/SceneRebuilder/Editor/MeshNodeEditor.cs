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

    private int rendererCount = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        meshnodeListArg = new FoldoutEditorArg(true,true,true,true,true);

        rendererCount = targetT.gameObject.GetComponentsInChildren<MeshRenderer>(true).Length;
    }

    public override void OnToolLayout(MeshNode item)
    {
        base.OnToolLayout(item);

        if (item.meshData.vertexCount > 0)
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)}({MeshHelper.GetVertexCountS(item.meshData.vertexCount)}|{item.meshData.vertexCount / (float)item.VertexCount:P1}),renderers:{rendererCount}");
        }
        else
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)},renderers:{rendererCount}");
        }
        

        

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

        var subms = item.GetMeshNodes();
        if (subms.Count > 0)
        {
            meshnodeListArg.caption = $"MeshNode List";
            EditorUIUtils.ToggleFoldout(meshnodeListArg, arg =>
            {
                arg.caption = $"MeshNode List ({subms.Count})";
                //arg.info = $"({sv / 10000f:F0}){txt}";
                InitEditorArg(subms);
            },
            () =>
            {
            });
            DrawMeshNodeList(meshnodeListArg, item, 0);
        }
    }

    private void DrawMeshNodeList(FoldoutEditorArg meshnodeListArg, MeshNode item,int level)
    {
        meshnodeListArg.level = level;
        var nodes = item.GetMeshNodes();
        if (meshnodeListArg.isEnabled && meshnodeListArg.isExpanded && nodes.Count>0)
        {
            
            InitEditorArg(nodes);
            meshnodeListArg.DrawPageToolbar(nodes, (node, i) =>
            {
                if (node == null)
                {
                    var arg = new FoldoutEditorArg();
                    arg.caption = "NULL";
                    EditorUIUtils.ObjectFoldout(new FoldoutEditorArg(), null, null);
                }
                else
                {
                    var arg = editorArgs[node];
                    arg.isFoldout = node.GetMeshNodes().Count > 0;
                    arg.caption = $"[{i:00}] {node.GetName()} ({node.GetMeshNodes().Count})";
                    arg.isEnabled = true;
                    arg.info = $"{MeshHelper.GetVertexCountS(node.VertexCount)}[{node.VertexCount / (float)item.VertexCount:P1}]";
                    if (level == 0)
                    {
                        arg.background = true;
                        arg.bold = node == item;
                    }

                    EditorUIUtils.ObjectFoldout(arg, node.gameObject, () =>
                    {

                    });

                    if (node != item)
                    {
                        DrawMeshNodeList(arg, node, level + 1);
                    }
                }
            });
        }
    }
}

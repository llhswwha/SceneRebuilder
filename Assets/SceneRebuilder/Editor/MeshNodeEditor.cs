using CodeStage.AdvancedFPSCounter.Editor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshNode))]
public class MeshNodeEditor : BaseFoldoutEditor<MeshNode>
{
    FoldoutEditorArg meshnodeListArg = new FoldoutEditorArg();

    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();

    FoldoutEditorArg assetPathListArg = new FoldoutEditorArg();

    private int rendererCount = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        meshnodeListArg = new FoldoutEditorArg(true,true,true,true,true);
        sharedMeshListArg = new FoldoutEditorArg(true, false, true, true, true);
        sharedMeshListArg.tag = targetT.sharedMeshInfos;
        assetPathListArg = new FoldoutEditorArg(true, true, true, true, true);
        rendererCount = targetT.gameObject.GetComponentsInChildren<MeshRenderer>(true).Length;
    }

    public override void OnToolLayout(MeshNode item)
    {
        base.OnToolLayout(item);

        if (item.meshData.vertexCount > 0)
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)}({MeshHelper.GetVertexCountS(item.meshData.vertexCount)}|{item.meshData.vertexCount / (float)item.VertexCount:P1}),renderers:{rendererCount}({item.sharedMeshInfos})");
        }
        else
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)},renderers:{rendererCount}({item.sharedMeshInfos})");
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("UpdateList"))
        {
            MeshNode.InitNodes(item.gameObject);
            sharedMeshListArg.tag = item.GetSharedMeshList();
        }
        if (GUILayout.Button("GetAssets"))
        {
            item.GetAssetPaths();
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowShared"))
        {
            item.GetChildrenSharedMeshInfo();
        }
        if (GUILayout.Button("ShowShared(All)"))
        {
            item.GetChildrenSharedMeshInfo_All();
        }
        //if (GUILayout.Button("Refresh"))
        //{
        //    item.RefreshInfo();
        //}
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

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Split"))
        {
            MeshCombineHelper.SplitByMaterials(item.gameObject,false);
        }
        if (GUILayout.Button("Combine"))
        {
            MeshCombineHelper.CombineEx(new MeshCombineArg(item.gameObject), MeshCombineMode.OneMesh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowVertexes"))
        {
            item.ShowVertexes();
        }
        if (GUILayout.Button("ClearVertexes"))
        {
            item.ClearVertexes();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        
        if (GUILayout.Button("TestGetVertexCenterInfo"))
        {
            item.TestGetVertexCenterInfo();
        }

        if (GUILayout.Button("ShowLongShortDebugDetail"))
        {
            item.ShowLongShortDebugDetail(true);
        }

        if (GUILayout.Button("ShowAllMaxPoints"))
        {
            item.ShowAllMaxPoints();
        }
        if (GUILayout.Button("ShowAllMinPoints"))
        {
            item.ShowAllMinPoints();
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
                var btnStyle = new GUIStyle(EditorStyles.miniButton);
                btnStyle.margin = new RectOffset(0, 0, 0, 0);
                btnStyle.padding = new RectOffset(0, 0, 0, 0);
                if (GUILayout.Button("UpdateSharedMesh", btnStyle, GUILayout.Width(120)))
                {
                    for (int i = 0; i < subms.Count; i++)
                    {
                        MeshNode node = subms[i];
                        ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("UpdateSharedMesh", i, subms.Count, node));
                        node.GetSharedMeshList();
                    }
                    ProgressBarHelper.ClearProgressBar();
                }
            });

            DrawMeshNodeList(meshnodeListArg, item, 0);

            //if (sharedMeshListArg.tag == null)
            //{
            //    sharedMeshListArg.tag = item.GetSharedMeshList();
            //}
            if (item == null) return;
            GameObject go = item.gameObject;
            bool isUpdate=DrawSharedMeshListEx(sharedMeshListArg,()=> item.GetSharedMeshList(),go);
            if (isUpdate)
            {
                MeshNode.InitNodes(go);
                //sharedMeshListArg.tag = item.GetSharedMeshList();
            }

            DrawMeshAssetPaths(assetPathListArg, item);
        }

    }

    private void DrawMeshAssetPaths(FoldoutEditorArg listArg, MeshNode item)
    {
        MeshRendererAssetInfoDict pathDict = item.assetPaths;
        BaseFoldoutEditorHelper.DrawMeshAssetPaths(pathDict, listArg);
    }


    private void DrawMeshNodeList(FoldoutEditorArg listArg, MeshNode item,int level)
    {
        listArg.level = level;
        var nodes = item.GetMeshNodes();
        if (listArg.isEnabled && listArg.isExpanded && nodes.Count>0)
        {
            
            InitEditorArg(nodes);
            listArg.DrawPageToolbar(nodes, (node, i) =>
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
                    arg.info = node.GetItemInfo(item.VertexCount);
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

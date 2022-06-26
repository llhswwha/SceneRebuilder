using CodeStage.AdvancedFPSCounter.Editor.UI;
using GPUInstancer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InnerMeshNode))]
public class InnerMeshNodeEditor : BaseFoldoutEditor<InnerMeshNode>
{
    FoldoutEditorArg meshnodeListArg = new FoldoutEditorArg();

    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();

    FoldoutEditorArg assetPathListArg = new FoldoutEditorArg();

    public static Dictionary<InnerMeshNode, int> rendererCountDict = new Dictionary<InnerMeshNode, int>();

    private int rendererCount = 0;

    private int gpuiCount = 0;

    private int gpuiVertexCount = 0;

    private SharedMeshInfoList gpuiSharedMeshes;

    public override void OnEnable()
    {
        base.OnEnable();
        meshnodeListArg = new FoldoutEditorArg(true, true, true, true, true);
        sharedMeshListArg = new FoldoutEditorArg(true, false, true, true, true);
        sharedMeshListArg.tag = targetT.sharedMeshInfos;
        assetPathListArg = new FoldoutEditorArg(true, true, true, true, true);

        if(rendererCountDict.ContainsKey(targetT))
        {
            rendererCount = rendererCountDict[targetT];
        }
        else
        {
            GetRendererCount(targetT, false);
            rendererCountDict.Add(targetT, rendererCount);
        }
    }

    private void GetRendererCount(InnerMeshNode item,bool isForce)
    {
        var meshFilters = targetT.gameObject.GetComponentsInChildren<MeshFilter>(item.isIncludeInactive);

        List<MeshFilter> gpuis = new List<MeshFilter>();
        if (gpuiSharedMeshes == null || isForce)
        {
            gpuis = meshFilters.ToList().FindAll(i => i.GetComponent<GPUInstancerPrefab>() != null);
            gpuiVertexCount = 0;
            for (int i = 0; i < gpuis.Count; i++)
            {
                MeshFilter gpui = gpuis[i];
                MeshFilter mf = gpui.GetComponent<MeshFilter>();
                if (mf.sharedMesh != null)
                {
                    gpuiVertexCount += mf.sharedMesh.vertexCount;
                }
                else
                {
                    Debug.LogError($"UpdateList[{i}/{gpuis.Count}] name:{gpui.name}");
                }
            }
            gpuiSharedMeshes = new SharedMeshInfoList(gpuis);
            gpuiCount = gpuis.Count;
        }
        
        rendererCount = meshFilters.Length;
        if (MeshNodeSetting.Instance.isIncludeGPUI == false)
        {
            rendererCount = meshFilters.Length - gpuis.Count;
        }
        else
        {

        }
    }

    public override void OnToolLayout(InnerMeshNode item)
    {
        base.OnToolLayout(item);

        string gupiInfo = "";
        if (gpuiCount > 0)
        {
            gupiInfo = $" [gpui renderers:{gpuiCount} vertex:{MeshHelper.GetVertexCountS(gpuiVertexCount)} ({gpuiSharedMeshes})]";
        }
        if (item.meshData.vertexCount > 0)
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)}({MeshHelper.GetVertexCountS(item.meshData.vertexCount)}|{item.meshData.vertexCount / (float)item.VertexCount:P1}),renderers:{rendererCount}({item.sharedMeshInfos}){gupiInfo}");
        }
        else
        {
            GUILayout.Label($"vertex:{MeshHelper.GetVertexCountS(item.VertexCount)},renderers:{rendererCount}({item.sharedMeshInfos}){gupiInfo}");
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("IncludeInactive", GUILayout.Width(100));
        item.isIncludeInactive = GUILayout.Toggle(item.isIncludeInactive, "", GUILayout.Width(30));
        GUILayout.Label("IncludeGPUI", GUILayout.Width(100));
        MeshNodeSetting.Instance.isIncludeGPUI = GUILayout.Toggle(MeshNodeSetting.Instance.isIncludeGPUI, "", GUILayout.Width(30));
        if (GUILayout.Button("UpdateList"))
        {
            GetRendererCount(item,true);
            InnerMeshNode.InitInnerNodes(item.gameObject);
            sharedMeshListArg.tag = item.GetSharedMeshList();
        }
        if (GUILayout.Button("Destroy"))
        {
            GameObject go = item.gameObject;
            TransformHelper.ClearComponents<InnerMeshNode>(go);
            TransformHelper.ClearComponents<MeshRendererInfo>(go);
        }
            //if (GUILayout.Button("GetAssets"))
            //{
            //    item.GetAssetPaths();
            //}
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
        if (GUILayout.Button("ClearNodes"))
        {
            //item.RefreshInfo();
            var meshNodes = item.GetComponentsInChildren<MeshNode>(true);
            foreach (var node in meshNodes)
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
            MeshCombineHelper.SplitByMaterials(item.gameObject, false);
        }
        if (GUILayout.Button("Combine"))
        {
            MeshCombineHelper.CombineEx(new MeshCombineArg(item.gameObject), MeshCombineMode.OneMesh);
        }
        if (GUILayout.Button("RecoverParent"))
        {
            item.RecoverParent();
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
        if (GUILayout.Button("ShowTriangles"))
        {
            MeshTriangles.DebugShowTriangles(item.gameObject, 0.01f);
        }
        if (GUILayout.Button("ClearChildren"))
        {
            MeshHelper.ClearChildren(item.transform);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();


        //if (GUILayout.Button("TestGetVertexCenterInfo"))
        //{
        //    item.TestGetVertexCenterInfo();
        //}

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
                        var node = subms[i];
                        ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("UpdateSharedMesh", i, subms.Count, node));
                        node.GetSharedMeshList();
                    }
                    ProgressBarHelper.ClearProgressBar();
                }
                //if (GUILayout.Button("ByCount", btnStyle, GUILayout.Width(120)))
                //{
                //    meshnodeListArg.sortType = 1;
                //}
                meshNodeSortType = EditorGUILayout.Popup(meshNodeSortType, meshNodeSortTypeNames, GUILayout.Width(100));
                meshnodeListArg.sortType = meshNodeSortType;
            });

            DrawMeshNodeList(meshnodeListArg, item, 0);

            //if (sharedMeshListArg.tag == null)
            //{
            //    sharedMeshListArg.tag = item.GetSharedMeshList();
            //}
            if (item == null) return;
            GameObject go = item.gameObject;
            bool isUpdate = DrawSharedMeshListEx(sharedMeshListArg, () => item.GetSharedMeshList(), go);
            if (isUpdate)
            {
                MeshNode.InitNodes(go);
                //sharedMeshListArg.tag = item.GetSharedMeshList();
            }

            //DrawMeshAssetPaths(assetPathListArg, item);
        }

    }

    int meshNodeSortType;

    string[] meshNodeSortTypeNames = new string[] { "Vertex", "Count", "Name" };

    //private void DrawMeshAssetPaths(FoldoutEditorArg listArg, InnerMeshNode item)
    //{
    //    MeshRendererAssetInfoDict pathDict = item.assetPaths;
    //    BaseFoldoutEditorHelper.DrawMeshAssetPaths(pathDict, listArg);
    //}


    private void DrawMeshNodeList(FoldoutEditorArg listArg, InnerMeshNode item, int level)
    {
        listArg.level = level;
        var nodes = item.GetMeshNodes();
        if (nodes == null) return;
        if (listArg.sortType == 0)//Vertex
        {
            nodes.Sort((a, b) => { return b.VertexCount.CompareTo(a.VertexCount); });
        }
        else if (listArg.sortType == 1)//Count
        {
            nodes.Sort((a, b) => { return b.rendererCount.CompareTo(a.rendererCount); });

        }
        else if (listArg.sortType == 2)//Name
        {
            nodes.Sort((a, b) => { return a.name.CompareTo(b.name); });
        }
        if (listArg.isEnabled && listArg.isExpanded && nodes.Count > 0)
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
                    arg.sortType = listArg.sortType;
                    arg.isFoldout = node.GetMeshNodes().Count > 0;
                    arg.caption = $"[{i:00}] {node.GetName()}{node.GetCountInfo()}";
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

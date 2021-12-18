using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavisModelRootList))]
public class NavisModelRootListEditor : BaseFoldoutEditor<NavisModelRootList>
{
    static FoldoutEditorArg<NavisModelRoot> rootListARg = new FoldoutEditorArg<NavisModelRoot>(true, true);

    private static FoldoutEditorArg<LODTwoRenderers> bimListArg_Old = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> bimListArg_New = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> transformListArg1 = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> transformListArg2 = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> NoConnectedBimsListArg = new FoldoutEditorArg<LODTwoRenderers>(true, false);
    private static FoldoutEditorArg<LODTwoRenderers> NoConnectedTransformListArg = new FoldoutEditorArg<LODTwoRenderers>(true, false);

    public static void DrawUI(NavisModelRootList item)
    {
        if (item == null) return;
        if (GUILayout.Button("LoadScenes"))
        {
            SceneRebuildManager.Instance.LoadScenes();
        }
        DrawModelRootList(item, rootListARg);

        GUILayout.BeginHorizontal();
        item.Root1 = BaseEditorHelper.ObjectField("Root1:", 100, item.Root1);
        if (GUILayout.Button("Bind1"))
        {
            item.Root1.BindBimInfo();
        }
        item.Root2 = BaseEditorHelper.ObjectField("Root2:", 100, item.Root2);
        if (GUILayout.Button("Bind2"))
        {
            item.Root2.BindBimInfo();
        }
        if (GUILayout.Button("Compare0"))
        {
            item.CompareModelByBIM_Position();
        }
        if (GUILayout.Button("Compare1"))
        {
            item.CompareModelByBIM(true);
        }
        if (GUILayout.Button("Compare2"))
        {
            item.CompareModelByBIM(false);
        }
        GUILayout.EndHorizontal();

        ModelUpdateManagerEditor.DrawListCompareResult("BIM List(OLD)", item.ModelRendersWaiting_Old_BIM, bimListArg_Old);
        ModelUpdateManagerEditor.DrawListCompareResult("ModelRenders1", item.ModelRenders1, transformListArg1);
        ModelUpdateManagerEditor.DrawListCompareResult("ModelRenders2", item.ModelRenders2, transformListArg2);

        ModelUpdateManagerEditor.DrawListCompareResult("NoConnectedBims", item.ModelRendersNoConnectedBims, NoConnectedBimsListArg);
        ModelUpdateManagerEditor.DrawListCompareResult("NoConnectedTransform", item.ModelRendersNoConnectedTransform, NoConnectedTransformListArg);
    }

    public override void OnToolLayout(NavisModelRootList item)
    {
        //DateTime start = DateTime.Now;
        base.OnToolLayout(item);
        //Debug.Log($"InitNavisFileInfoByModelEditor OnToolLayout t1:{DateTime.Now - start}");

        DrawUI(item);
        //Debug.Log($"InitNavisFileInfoByModelEditor OnToolLayout t2:{DateTime.Now - start}");

        if (GUILayout.Button("Window"))
        {
            InitNavisFileInfoByModelWindow.ShowWindow();
        }
    }

    public static void DrawModelRootList(NavisModelRootList item, FoldoutEditorArg<NavisModelRoot> listArg)
    {
        string name = "Model Root List";
        listArg.caption = $"{name}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list = item.ModelRoots;
            int count = list.Count;
            arg.caption = $"{name} ({list.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list);
        },
        () =>
        {
            if (GUILayout.Button("Update"))
            {
                item.GetModelRootObjects();
                //item.GetRootModels();
            }
            if (GUILayout.Button("BindBim"))
            {
                item.RootsBindBim();
            }
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = item.ModelRoots;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var listItemArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                listItemArg.level = 1;
                listItemArg.background = true;
                listItemArg.caption = $"[{i + 1:00}] {listItem.name}";
                //listItemArg.info = $"(ts:{listItem.transformList.Count})(ms:{listItem.ModelDict.Count})(bim:{listItem.bimInfos.Count})(noFound:{listItem.model2TransformResult.notFoundCount})";
                var countInfo = listItem.resultCount;
                listItemArg.info = $"(ts:{countInfo.TransformCount})(ms:{countInfo.ModelCount})(bim:{countInfo.BimCount})(noFound:{countInfo.NotFoundCount})";
                EditorUIUtils.ObjectFoldout(listItemArg, listItem, () =>
                {
                    var btnStyle = new GUIStyle(EditorStyles.miniButton);
                    btnStyle.margin = new RectOffset(0, 0, 0, 0);
                    btnStyle.padding = new RectOffset(0, 0, 0, 0);
                    if (GUILayout.Button("Bind", btnStyle,GUILayout.Width(50)))
                    {
                        listItem.BindBimInfo();
                    }
                });
            });
        }
    }
}

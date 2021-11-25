using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavisModelRootList))]
public class NavisModelRootListEditor : BaseFoldoutEditor<NavisModelRootList>
{
    static FoldoutEditorArg<NavisModelRoot> rootListARg = new FoldoutEditorArg<NavisModelRoot>(true, false);

    public static void DrawUI(NavisModelRootList item)
    {
        if (item == null) return;
        if (GUILayout.Button("LoadScenes"))
        {
            SceneRebuildManager.Instance.LoadScenes();
        }
        DrawModelRootList(item, rootListARg);
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
                //doorRootArg.info = listItem.GetItemText();
                EditorUIUtils.ObjectFoldout(listItemArg, listItem, () =>
                {
                });
            });
        }
    }
}

using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaTreeNode))]
public class AreaTreeNodeEditor : BaseFoldoutEditor<AreaTreeNode>
{
    FoldoutEditorArg<MeshRendererInfo> meshRendererInfoListArg = new FoldoutEditorArg<MeshRendererInfo>();

    public override void OnEnable()
    {
        base.OnEnable();
        meshRendererInfoListArg.Items = targetT.GetRendererInfos();
    }

    public override void OnToolLayout(AreaTreeNode areaTreeNode)
    {
        base.OnToolLayout(areaTreeNode);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("MoveRenderers"))
        {
            areaTreeNode.MoveRenderers();
        }

        if (GUILayout.Button("RecoverParentEx"))
        {
            areaTreeNode.RecoverParentEx();
        }
        if (GUILayout.Button("RecoverParent"))
        {
            areaTreeNode.RecoverParent();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("InitIds"))
        {
            IdDictionary.InitInfos();
        }
        if (GUILayout.Button("LoadRenderers"))
        {
            areaTreeNode.LoadRenderers();
        }
        if (GUILayout.Button("SwitchToRenderers"))
        {
            areaTreeNode.SwitchToRenderers();
        }

        if (GUILayout.Button("SwitchToCombined"))
        {
            areaTreeNode.SwitchToCombined();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateScenes"))
        {
            areaTreeNode.EditorCreateNodeScenes();
        }
        if (GUILayout.Button("LoadScenes"))
        {

            areaTreeNode.EditorLoadScenesEx();
        }
        if (GUILayout.Button("UnloadScenes"))
        {
            areaTreeNode.UnLoadScenes();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("GetRendererInfos"))
        {
            meshRendererInfoListArg.Items= areaTreeNode.GetRendererInfos();
        }

        meshRendererInfoListArg.caption = $"RendererInfo List";
        EditorUIUtils.ToggleFoldout(meshRendererInfoListArg, arg =>
        {
            var items = meshRendererInfoListArg.Items;
            arg.caption = $"RendererInfo List ({items.Count})";
            //arg.info = $"({sv / 10000f:F0}){txt}";
            InitEditorArg(items);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
        });
        if (meshRendererInfoListArg.isEnabled && meshRendererInfoListArg.isExpanded)
        {
            var items = meshRendererInfoListArg.Items;
            InitEditorArg(items);
            meshRendererInfoListArg.DrawPageToolbar(items, (item, i) =>
            {
                var arg = editorArgs[item];
                arg.level = 1;
                arg.isFoldout = false;
                arg.caption = $"[{i:00}] {item.name}";
                arg.info = $"LOD[{item.GetLODIds()}]";
                EditorUIUtils.ObjectFoldout(arg, item.gameObject, () =>
                {
                    
                });
            });
        }
    }
}

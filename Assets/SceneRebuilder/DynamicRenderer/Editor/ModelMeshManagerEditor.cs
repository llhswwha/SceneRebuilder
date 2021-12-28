using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelMeshManager))]
public class ModelMeshManagerEditor : BaseFoldoutEditor<ModelMeshManager>
{
    FoldoutEditorArg modelClassListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();

        targetT.InitPrefixNames();
    }

    public override void OnToolLayout(ModelMeshManager item)
    {
        if (GUILayout.Button("GetAllRenderers"))
        {
            item.GetAllRenderers();
        }
        if (GUILayout.Button("GetPrefixNames"))
        {
            item.GetPrefixNames();
        }

        DrawModelClassDict(item.ModelClassDict_Auto, modelClassListArg);
    }

    private void DrawModelClassDict(ModelClassDict<MeshRenderer> Dict, FoldoutEditorArg listArg)
    {
        var kesy = Dict.GetKeys();
        listArg.caption = $"ModelClass List";
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            arg.caption = $"ModelClass List ({kesy.Count})";
            //arg.info = $"({sv / 10000f:F0}){txt}";
            FoldoutEditorArgBuffer.InitEditorArg(kesy);
        },
            () =>
            {
            });

        if (listArg.isEnabled && listArg.isExpanded && kesy.Count > 0)
        {

            FoldoutEditorArgBuffer.InitEditorArg(kesy);
            //listArg.level = 2;
            listArg.DrawPageToolbar(kesy, (path, i) =>
            {
                var list = Dict.GetList(path);
                FoldoutEditorArgBuffer.InitEditorArg(list);
                var arg = FoldoutEditorArgBuffer.editorArgs[path];
                arg.isFoldout = list.Count > 0;
                arg.level = 2;
                arg.caption = $"[{i:00}] {path} ({list.Count})";
                //arg.info = $"{MeshHelper.GetVertexCountS((int)list.GetVertexCount())}";
                arg.isEnabled = true;

                EditorUIUtils.ObjectFoldout(arg, null, () =>
                {

                });

                if (arg.isEnabled && arg.isExpanded && list.Count > 0)
                {
                    //DrawMeshRendererInfoList(list, arg);

                    BaseFoldoutEditorHelper.DrawMeshRendererList(list, arg);
                }
            });
        }
    }
}

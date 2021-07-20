using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalMaterialManager))]
public class GlobalMaterialManagerEditor : BaseFoldoutEditor<GlobalMaterialManager>
{
    FoldoutEditorArg matListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        matListArg.isEnabled = true;
    }

    public override void OnToolLayout(GlobalMaterialManager item)
    {
        base.OnToolLayout(item);

        //if (GUILayout.Button("InitMaterials"))
        //{
        //    item.InitMaterials();
        //}

        matListArg.caption = $"Material List";
        EditorUIUtils.ToggleFoldout(matListArg, arg =>
        {
            arg.caption = $"Material List ({item.meshMaterialList.Count})";
            //arg.info = $"{item.VertexCount_Show / 10000f:F0}/{item.VertexCount / 10000f:F0}";
            InitEditorArg(item.meshMaterialList);
        },
        () =>
        {
            //MeshCombineHelper.GetMatFilters
            if (GUILayout.Button("Update"))
            {
                RemoveEditorArg(item.meshMaterialList);
                item.InitMaterials();
                InitEditorArg(item.meshMaterialList);
            }
        });
        if (matListArg.isEnabled && matListArg.isExpanded)
        {
            matListArg.DrawPageToolbar(item.meshMaterialList, (meshMat,i) =>
            {

                var matArg = editorArgs[meshMat];
                matArg.background = true;
                matArg.caption = $"[{i + 1:00}] {meshMat.GetName()} ({meshMat.subMeshs.Count})";
                //matArg.info = $"count:{meshMat.subMeshs.Count}";
                EditorUIUtils.ObjectFoldout(matArg, meshMat.GetMat(), () =>
                {
                    var newColor=EditorGUILayout.ColorField("Color", meshMat.GetColor(), GUILayout.Width(50));
                    meshMat.SetColor(newColor);
                });

                if (matArg.isExpanded)
                {
                    InitEditorArg(meshMat.subMeshs);
                    matArg.DrawPageToolbar(meshMat.subMeshs, (subMesh, j) =>
                    {

                        var arg = editorArgs[subMesh];
                        arg.caption = $"[{j + 1:00}] {subMesh.GetName()}";
                        //arg.info = $"count:{subMesh.subMeshs.Count}";
                        EditorUIUtils.ObjectFoldout(arg, subMesh.meshFilter.gameObject, () =>
                        {
                            //EditorGUILayout.ColorField("Color", subMesh.GetColor(), GUILayout.Width(50));
                        });
                    });
                }
            });
        }

        EditorGUILayout.BeginHorizontal();
        item.DefaultColor = EditorGUILayout.ColorField("DefaultColor", item.DefaultColor);
        if (GUILayout.Button("ResetColor"))
        {
            item.ResetColor();
        }
        EditorGUILayout.EndHorizontal();
    }
}

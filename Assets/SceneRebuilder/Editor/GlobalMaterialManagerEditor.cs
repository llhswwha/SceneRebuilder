using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalMaterialManager))]
public class GlobalMaterialManagerEditor : BaseFoldoutEditor<GlobalMaterialManager>
{
    static FoldoutEditorArg matListArg = new FoldoutEditorArg(true,true);

    public override void OnEnable()
    {
        base.OnEnable();
        matListArg.isEnabled = true;

        targetT.LocalTarget = null;
        targetT.GetSharedMaterials();
    }

    public override void OnToolLayout(GlobalMaterialManager item)
    {
        base.OnToolLayout(item);

        DrawUI(item);

        if (GUILayout.Button("Window"))
        {
            LODManagerEditorWindow.ShowWindow();
        }
    }

    public static void DrawUI(GlobalMaterialManager item)
    {
        EditorGUILayout.BeginHorizontal();
        var newTarget=BaseEditorHelper.ObjectField("LocalTarget",item.LocalTarget);
        EditorGUILayout.EndHorizontal();

        if(newTarget!=item.LocalTarget)
        {
            item.GetSharedMaterials();
        }
        item.LocalTarget=newTarget;
        DrawMatList(item, matListArg);


        EditorUIUtils.Separator(5);

        EditorGUILayout.BeginHorizontal();
        item.SetTargetRoot=BaseEditorHelper.ObjectField("SetTarget",item.SetTargetRoot);
        item.SetTargetMaterial=BaseEditorHelper.ObjectField("SetMaterial",item.SetTargetMaterial);
        if(GUILayout.Button("SetMaterial"))
        {
            item.DoChangeMaterial();
        }
        if(GUILayout.Button("SetChildrenMaterial"))
        {
            item.SetChildrenMaterial();
        }
        
        EditorGUILayout.EndHorizontal();
    }   
}

using AdvancedCullingSystem.DynamicCullingCore;
using AdvancedCullingSystem.StaticCullingCore;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeldingManager))]
public class WeldingManagerEditor : BaseFoldoutEditor<WeldingManager>
{
    private FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();
    private FoldoutEditorArg<MeshRendererInfo> meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>();

    public override void OnEnable()
    {
        base.OnEnable();

        sharedMeshListArg.tag = targetT.WeldingSharedMeshInfos;
    }

    private bool isCopyTarget = true;

    public override void OnToolLayout(WeldingManager item)
    {
        if (GUILayout.Button("GetWeldings"))
        {
            item.GetWeldings();
        }
        if (GUILayout.Button("AddCollider"))
        {
            item.AddCollider();
        }
        if (GUILayout.Button("StaticCulling"))
        {
            StaticCullingEditorWindow.CreateCullingWindow();
            item.SetStaticCulling();
        }
        if (GUILayout.Button("DymicCulling"))
        {
            DynamicCulling culling = DynamicCullingEditor.CreateCullingInstance(item.gameObject);
            item.SetDymicCulling();
        }

        DrawSharedMeshListEx(sharedMeshListArg, () => item.GetWeldings());


        DrawMeshRendererInfoListEx(meshinfoListArg, item.GetMeshInfoList);
    }
}

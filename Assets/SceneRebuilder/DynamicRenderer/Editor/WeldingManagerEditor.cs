using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeldingManager))]
public class WeldingManagerEditor : BaseFoldoutEditor<WeldingManager>
{
    FoldoutEditorArg sharedMeshListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
    }

    private bool isCopyTarget = true;

    public override void OnToolLayout(WeldingManager item)
    {
        if (GUILayout.Button("GetWeldings"))
        {
            item.GetWeldings();
        }

        DrawSharedMeshListEx(sharedMeshListArg, () => item.GetWeldings());
    }
}

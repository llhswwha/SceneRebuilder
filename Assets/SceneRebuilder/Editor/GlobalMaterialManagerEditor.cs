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

        targetT.LocalTarget = null;
        targetT.GetSharedMaterials();
    }

    public override void OnToolLayout(GlobalMaterialManager item)
    {
        base.OnToolLayout(item);

        DrawMatList(item, matListArg);
    }
}

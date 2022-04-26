using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HierarchyRoot))]
public class HierarchyRootEditor : BaseFoldoutEditor<HierarchyRoot>
{
    public FoldoutEditorArg NotFoundListArg = new FoldoutEditorArg();

    public override void OnToolLayout(HierarchyRoot item)
    {
        base.OnToolLayout(item);

        item.root = ObjectFieldH("Root", item.root);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            item.LoadXml();
        }
        if (GUILayout.Button("Save"))
        {
            item.SaveXml();
        }
        if (GUILayout.Button("Check"))
        {
            item.Check();
        }
        EditorGUILayout.EndHorizontal();

        //BaseEditorHelper.Dra
        DrawObjectList(NotFoundListArg, "NotFound", item.IdInfoList.notFoundList, null, null, null);
    }
}

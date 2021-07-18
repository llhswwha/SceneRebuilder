using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SubScene_List))]
public class SubScene_ListEditor : BaseFoldoutEditor<SubScene_List>
{
    private FoldoutEditorArg sceneListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        targetT.GetScenes();
        sceneListArg.isEnabled = true;
    }

    public override void OnToolLayout(SubScene_List item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("sceneCount:",GUILayout.Width(100));
        GUILayout.TextField(item.sceneCount.ToString());
        EditorGUILayout.Popup(0, new string[2] {"1","2" });

        EditorGUILayout.EndHorizontal();

        EditorUIUtils.SetupStyles();
        DrawSceneList(sceneListArg, item.scenes.ToList);


    }
}

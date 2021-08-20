using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabInstanceBuilderEditorWindow : EditorWindow
{
    [MenuItem("Window/Tools/PrefabBuilder")]
    public static void ShowWindow()
    {
        var window = (PrefabInstanceBuilderEditorWindow)EditorWindow.GetWindowWithRect(typeof(PrefabInstanceBuilderEditorWindow), new Rect(0, 0, 600, 400), true, "PrefabBuilder");
        window.Show();
        window.Init();
    }

    public PrefabInstanceBuilder target;
    private FoldoutEditorArg prefabInfoListArg = new FoldoutEditorArg();
    public void Init()
    {
        target = PrefabInstanceBuilder.Instance;
        prefabInfoListArg = new FoldoutEditorArg(true, false, true, true, false);
    }

    private void OnGUI()
    {
        //MeshComparerEditor.DrawSetting(target);

        PrefabInstanceBuilderEditor.DrawUI(target);

        BaseFoldoutEditorHelper.DrawPrefabList(prefabInfoListArg, () => target.PrefabInfoList);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectScripts"))
        {
            Selection.activeObject = target.gameObject;
        }
        EditorGUILayout.EndHorizontal();
    }
}

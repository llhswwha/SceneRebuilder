using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabInstanceBuilderEditorWindow : EditorWindow, IBaseEditorWindow
{
    [MenuItem("Window/Tools/PrefabBuilder")]
    public static void ShowWindow()
    {
        BaseEditorWindow.ShowWindow<PrefabInstanceBuilderEditorWindow>(700, 400, "PrefabBuilder");
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
        if (target == null)
        {
            Init();
        }

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

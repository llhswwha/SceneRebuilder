using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;

[CustomEditor(typeof(ModelAreaTree))]
public class ModelAreaTreeEditor : BaseFoldoutEditor<ModelAreaTree>
{
    private FoldoutEditorArg nodeListArg = new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        ModelAreaTree item = target as ModelAreaTree;
        item.UpdateSceneList();

        nodeListArg = new FoldoutEditorArg(true, false);
        sceneListArg = new FoldoutEditorArg(true, false);
    }

    public override void OnToolLayout(ModelAreaTree item)
    {
        base.OnToolLayout(item);

        int sceneCount = item.GetSceneCount();
        bool isAllLoaded = item.IsSceneLoaded();
        int unloadedSceneCount = item.SceneList.GetUnloadedScenes().Count;

        GUILayout.Label($"loaded:{isAllLoaded},scene:{sceneCount},unloaded:{unloadedSceneCount}");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowMeshes", contentStyle, GUILayout.Width(buttonWidth)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(item.gameObject);
        }
        NewButton("ShowLeafs", buttonWidth, true, null);
        NewButton("FindMaxNode", buttonWidth, true, null);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("SwToCombined", buttonWidth, true, item.SwitchToCombined);
        NewButton("SwToRenderers", buttonWidth, true, item.SwitchToRenderers);
        NewButton("MoveRenderers", buttonWidth, true, item.MoveRenderers);
        NewButton("RecoverParent", buttonWidth, true, item.RecoverParentEx);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool folderExists = item.IsScenesFolderExists();
        NewButton("CreateScenes", buttonWidth, isAllLoaded == true, item.EditorCreateNodeScenes);
        NewButton("RemoveScenes", buttonWidth, isAllLoaded == true && sceneCount > 0 && folderExists, item.DestroyScenes);
        NewButton("SelectFolder", buttonWidth, sceneCount > 0 && folderExists, item.SelectScenesFolder);
        NewButton("DeleteFolder", buttonWidth, isAllLoaded == true && sceneCount > 0 && folderExists, item.DeleteScenesFolder);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("LoadScenes", buttonWidth, isAllLoaded == false && sceneCount > 0 && folderExists, item.EditorLoadNodeScenesEx);
        NewButton("UnloadScenes", buttonWidth, isAllLoaded == true && sceneCount > 0 && folderExists, item.UnLoadScenes);
        EditorGUILayout.EndHorizontal();

        DrawNodeList(nodeListArg, () =>
        {
            return item.TreeLeafs;
        });
        DrawSceneList(sceneListArg, () =>
        {
            return item.GetSceneList();
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ModelAreaTree))]
public class ModelAreaTreeEditor : BaseEditor<ModelAreaTree>
{
    public override void OnEnable()
    {
        ModelAreaTree item = target as ModelAreaTree;
        item.UpdateSceneList();
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
    }
}

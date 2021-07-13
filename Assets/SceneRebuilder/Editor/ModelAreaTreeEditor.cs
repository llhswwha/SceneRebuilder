using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ModelAreaTree))]
public class ModelAreaTreeEditor : BaseEditor<ModelAreaTree>
{
    public override void OnToolLayout(ModelAreaTree item)
    {
        base.OnToolLayout(item);

        int sceneCount = item.GetSceneCount();
        bool isAllLoaded = item.IsSceneLoaded();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowMeshes", contentStyle, GUILayout.Width(buttonWidth)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(item.gameObject);
        }
        NewButton("ShowLeafs", buttonWidth, true, null);
        NewButton("FindMaxNode", buttonWidth, true, null);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("ShowCombined", buttonWidth, true, item.SwitchToCombined);
        NewButton("ShowRenderers", buttonWidth, true, item.SwitchToRenderers);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("CreateScenes", buttonWidth, isAllLoaded == true, item.EditorCreateNodeScenes);
        NewButton("RemoveScenes", buttonWidth, isAllLoaded == true && sceneCount > 0, item.DestroyScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("LoadScenes", buttonWidth, isAllLoaded == false && sceneCount > 0, item.EditorLoadNodeScenesEx);
        NewButton("UnloadScenes", buttonWidth, isAllLoaded == true && sceneCount > 0, item.UnLoadScenes);
        EditorGUILayout.EndHorizontal();
    }
}

using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// using System;

// [CanEditMultipleObjects]
[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : BaseFoldoutEditor<BuildingModelInfo>
{
    public override void OnEnable()
    {
        base.OnEnable();

        BuildingModelInfo item = target as BuildingModelInfo;
        item.UpdateSceneList();
    }
    public static void DrawToolbar(BuildingModelInfo info, GUIStyle btnStyle,int buttonWidth)
    {
        // if (GUILayout.Button("Multi-Objects", contentStyle, GUILayout.Width(100)))
        // {

        // }

        //BuildingModelInfo info = target as BuildingModelInfo;

        // int sceneCount = info.GetSceneCount();
        // int unloadedSceneCount = info.SceneList.GetUnloadedScenes().Count;
        // bool isAllLoaded = info.IsSceneLoaded();
        // int treeCount = info.GetTreeCount();
        // int partCount = info.GetPartCount();

        BuildingModelState state=info.GetState();


        //if (state.HaveUnloadedScenes)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("SelectUnload", btnStyle, GUILayout.Width(100)))
            {
                //info.SceneList.IsAllLoaded_Debug();

                var scenes = info.SceneList.GetUnloadedScenes();
                Debug.Log($"scenes:{scenes.Count}");
                //EditorHelper.SelectObject(element.rootObj);
                EditorHelper.SelectObjects(scenes);
            }
            if (GUILayout.Button("LoadUnloaded", btnStyle, GUILayout.Width(100)))
            {
                IdDictionary.InitInfos();
                info.LoadUnloadedScenes();
            }
            GUILayout.Label(state.ToString());
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.BeginHorizontal();
        NewButton("1.GetInfo", buttonWidth, state.CanGetInfo(), btnStyle,() => {
            info.InitInOut();
            // foreach (Object obj in targets)
            // {
            //     BuildingModelInfo item = obj as BuildingModelInfo;
            //     // item.InitInOut();
            //     Debug.Log("GetInfo_"+item);
            // }
        });
        NewButton("FindDoors", buttonWidth, state.CanFindDoors(), btnStyle, info.FindInDoors);
        NewButton("ClearParts", buttonWidth, state.partCount>0, btnStyle, info.ClearInOut);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTree", buttonWidth,state.CanCreateTrees, btnStyle, info.CreateTreesBSEx);
        NewButton("RemoveTrees", buttonWidth,state.CanRemoveTrees, btnStyle, info.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //bool folderExists = info.IsScenesFolderExists();
        NewButton("3.CreateScenes", buttonWidth, state.CanCreateScenes, btnStyle, info.EditorCreateNodeScenes);
        NewButton("RemoveScenes", buttonWidth, state.CanRemoveScenes, btnStyle, info.DestroyScenes);
        NewButton("SelectFolder", buttonWidth, state.sceneCount > 0, btnStyle, info.SelectScenesFolder);
        NewButton("DeleteFolder", buttonWidth, state.isAllLoaded == true && state.sceneCount > 0, btnStyle, info.DeleteScenesFolder);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //int unloadedSceneCount = info.SceneList.GetUnloadedScenes().Count;
        NewButton("4.LoadScenes", buttonWidth, state.CanLoadScenes, btnStyle, () =>
        {
            info.EditorLoadNodeScenesEx();
        });
        NewButton("UnloadScenes", buttonWidth, state.CanUnloadScenes, btnStyle, info.UnLoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("5.OneKey", buttonWidth, state.CanOneKey(), btnStyle, () => {
            // info.InitInOut();
            // info.CreateTreesBSEx();
            // info.EditorCreateNodeScenes();
            info.OneKey_TreeNodeScene();

        });
        NewButton("Reset", buttonWidth, state.CanReset(), btnStyle, () => {
            info.EditorLoadNodeScenesEx();
            info.DestroyScenes();
            info.ClearTrees();
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("All", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.gameObject);
        }
        GUILayout.Button(info.AllVertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.AllRendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out0", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart0.gameObject);
        }
        GUILayout.Button(info.Out0VertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.Out0RendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("In", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.InPart.gameObject);
        }
        GUILayout.Button(info.InVertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.InRendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out1", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out1VertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.Out1RendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out0B", GUILayout.Width(50)))
        {
            //MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out0BigVertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.Out0BigRendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out0S", GUILayout.Width(50)))
        {
            //MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out0SmallVertextCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.Out0SmallRendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        
    }

    //private FoldoutEditorArg buildingListArg = new FoldoutEditorArg();

    private FoldoutEditorArg treeListArg = new FoldoutEditorArg();

    private FoldoutEditorArg nodeListArg = new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg = new FoldoutEditorArg();

    public override void OnInspectorGUI()
    {
        // serializedObject.Update ();

        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        BuildingModelInfo info = target as BuildingModelInfo;
        DrawToolbar(info,contentStyle,buttonWidth);

        //DrawModelList(buildingListArg, 
        //    () => { return null; }, 
        //    () =>
        //   {
        //   });

        DrawTreeList(treeListArg, () =>
         {
             return info.GetTreeList();
         });
        DrawNodeList(nodeListArg, () =>
        {
            return info.GetNodeList();
        });
        DrawSceneList(sceneListArg, () =>
        {
            return info.GetSceneList();
        });

        base.OnInspectorGUI();
    }
}

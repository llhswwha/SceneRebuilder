using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
// using System;

// [CanEditMultipleObjects]
[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : BaseFoldoutEditor<BuildingModelInfo>
{
    //private FoldoutEditorArg buildingListArg = new FoldoutEditorArg();

    private FoldoutEditorArg treeListArg = new FoldoutEditorArg();

    private FoldoutEditorArg nodeListArg = new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg = new FoldoutEditorArg();

    private FoldoutEditorArg<MeshFilter> meshListArg = new FoldoutEditorArg<MeshFilter>();

    private FoldoutEditorArg<MeshRendererInfo> meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>();

    private FoldoutEditorArg matListArg = new FoldoutEditorArg();

    private FoldoutEditorArg doorListArg = new FoldoutEditorArg();

    private FoldoutEditorArg<LODGroupDetails> lodGroupListArg = new FoldoutEditorArg<LODGroupDetails>();

    public override void OnEnable()
    {
        base.OnEnable();

        treeListArg = new FoldoutEditorArg(true, false,true,true,false);
        nodeListArg = new FoldoutEditorArg(true, false, true, true, false);
        sceneListArg = new FoldoutEditorArg(true, false, true, true, false);

        meshListArg = new FoldoutEditorArg<MeshFilter>(true, false, true, true, false);
        meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>(true, false, true, true, false);

        matListArg = new FoldoutEditorArg(true, false, true, true, false);
        doorListArg = new FoldoutEditorArg(true, false, true, true, false);
        lodGroupListArg = new FoldoutEditorArg<LODGroupDetails>(true, false, true, true, false);

        GlobalMaterialManager.Instance.LocalTarget = targetT.gameObject;
        GlobalMaterialManager.Instance.GetSharedMaterials();

        DoorManager.Instance.LocalTarget = targetT.gameObject;
        DoorManager.Instance.UpdateDoors();

        LODManager.Instance.LocalTarget = targetT.gameObject;
        LODManager.Instance.GetRuntimeLODDetail(true);

        targetT.UpdateSceneList();
    }
    public static void DrawToolbar(BuildingModelInfo info, GUIStyle btnStyle, int buttonWidth)
    {
        BuildingModelState state = info.GetState();
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
        NewButton("1.GetInfo", buttonWidth, state.CanGetInfo(), btnStyle, () => {
            info.InitInOut();
        });
        int btnW1 = 90;
        NewButton("FindDoors", buttonWidth, state.CanFindDoors(), btnStyle, info.FindInDoors);
        NewButton("SplitDoors", buttonWidth, true, btnStyle, () =>
        {
            DoorManager.Instance.SplitDoors(info.gameObject);
        });
        NewButton("ClearParts", buttonWidth, state.partCount > 0, btnStyle, info.ClearInOut);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTree", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesBSEx);
        NewButton("RemoveTrees", buttonWidth, state.CanRemoveTrees, btnStyle, info.ClearTrees);
        NewButton("CreateByLOD", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);
        NewButton("CreateNoLOD", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);
        AreaTreeManager.Instance.isCombine = GUILayout.Toggle(AreaTreeManager.Instance.isCombine, "Combine");
        //NewButton("GetTreeAreas", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //bool folderExists = info.IsScenesFolderExists();
        NewButton("3.CreateScenes", buttonWidth, state.CanCreateScenes, btnStyle, () =>
        {
            //info.DeleteScenesFolder();
            info.EditorCreateNodeScenes();
        });
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
        NewButton("SetBuildings", buttonWidth, true, btnStyle, () => {
            SubSceneManager.Instance.SetBuildings_All();
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("ShowRenderers", buttonWidth, true, btnStyle, info.ShowRenderers);
        NewButton("InitMeshNodes", buttonWidth, true, btnStyle, () =>
        {
            MeshNode.InitNodes(info.gameObject);
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

    public override void OnToolLayout(BuildingModelInfo item)
    {
        base.OnToolLayout(item);

        DrawToolbar(item, contentStyle, buttonWidth);

        //DrawModelList(buildingListArg, 
        //    () => { return null; }, 
        //    () =>
        //   {
        //   });

        DrawTreeList(treeListArg, () =>
        {
            return item.GetTreeList();
        });
        List<AreaTreeNode> nodes = DrawNodeList(nodeListArg,true, () =>
        {
            return item.GetNodeList();
        });
        DrawSceneList(sceneListArg, () =>
        {
            return item.GetSceneList();
        });

        DrawMeshList(meshListArg, () =>
        {
           List<MeshFilter> meshes = new List<MeshFilter>();
           if(item.InPart)
               meshes.AddRange(item.InPart.GetComponentsInChildren<MeshFilter>(true));
           if (item.OutPart0)
               meshes.AddRange(item.OutPart0.GetComponentsInChildren<MeshFilter>(true));
           if (item.OutPart1)
               meshes.AddRange(item.OutPart1.GetComponentsInChildren<MeshFilter>(true));
           meshes=meshes.Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
           return meshes;
        });

        DrawMeshListEx(meshinfoListArg, () =>
        {
            MeshRendererInfo.InitRenderers(item.gameObject);

            List<MeshRendererInfo> meshes = new List<MeshRendererInfo>();
            if (item.InPart)
                meshes.AddRange(item.InPart.GetComponentsInChildren<MeshRendererInfo>(true));
            if (item.OutPart0)
                meshes.AddRange(item.OutPart0.GetComponentsInChildren<MeshRendererInfo>(true));
            if (item.OutPart1)
                meshes.AddRange(item.OutPart1.GetComponentsInChildren<MeshRendererInfo>(true));
            meshes = meshes.Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
            return meshes;
        });

        GlobalMaterialManager.Instance.LocalTarget = item.gameObject;
        DrawMatList(GlobalMaterialManager.Instance, matListArg);

        DoorManager.Instance.LocalTarget = item.gameObject;
        DrawDoorList(doorListArg, DoorManager.Instance);

        LODManager.Instance.LocalTarget = item.gameObject;
        DrawLODGroupList(lodGroupListArg, LODManager.Instance);
    }
}

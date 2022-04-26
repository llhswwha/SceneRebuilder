using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        //EnableFunction();
    }

    private void OnEnableFunction()
    {
        treeListArg = new FoldoutEditorArg(true, false, true, true, false);
        nodeListArg = new FoldoutEditorArg(true, false, true, true, false);
        sceneListArg = new FoldoutEditorArg(true, false, true, true, false);

        meshListArg = new FoldoutEditorArg<MeshFilter>(true, false, true, true, false);
        meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>(true, false, true, true, false);

        matListArg = new FoldoutEditorArg(true, false, true, true, false);
        doorListArg = new FoldoutEditorArg(true, false, true, true, false);
        lodGroupListArg = new FoldoutEditorArg<LODGroupDetails>(true, false, true, true, false);

        GlobalMaterialManager.Instance.LocalTarget = targetT.gameObject;
        GlobalMaterialManager.Instance.GetSharedMaterials();

        targetT.UpdateDoors();

        LODManager.Instance.LocalTarget = targetT.gameObject;
        LODManager.Instance.GetRuntimeLODDetail(true);

        targetT.UpdateSceneList();
    }

    public static void DrawToolbar(BuildingModelInfo info, GUIStyle btnStyle, int buttonWidth)
    {
        EditorUIUtils.ToggleFoldout(drawToolButtonsArg, () =>
        {
            DrawToolButtons(info, btnStyle, buttonWidth);
        });      

        EditorUIUtils.ToggleFoldout(drawModelInfoTableArg, () =>
        {
            DrawModelInfoTable(info, btnStyle);
        });
    }

    private static void DrawToolButtons(BuildingModelInfo info, GUIStyle btnStyle, int buttonWidth)
    {
        BuildingModelState state = info.GetState();

        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("1.GetInfo", buttonWidth, state.CanGetInfo(), btnStyle, () =>
        {
            info.InitInOut(true);
        });
        int btnW1 = 90;
        NewEnabledButton("FindDoors", buttonWidth, state.CanFindDoors(), btnStyle, info.FindInDoors);
        NewEnabledButton("SplitDoors", buttonWidth, true, btnStyle, () =>
        {
            DoorManager.Instance.SplitDoors(info.gameObject);
        });
        NewEnabledButton("ClearParts", buttonWidth, state.partCount > 0, btnStyle, info.ClearInOut);
        NewEnabledButton("CombineDoors", buttonWidth, state.partCount > 0, btnStyle, () =>
        {
            info.CombineDoors();
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("2.CreateTree", buttonWidth, state.CanCreateTrees, btnStyle, () => { AreaTreeManager.Instance.isCombine = false; info.CreateTreesBSEx(); });
        NewEnabledButton("2.CreateTree(C)", buttonWidth, state.CanCreateTrees, btnStyle, () => { AreaTreeManager.Instance.isCombine = true; info.CreateTreesBSEx(); });
        NewEnabledButton("RemoveTrees", buttonWidth, state.CanRemoveTrees, btnStyle, info.ClearTrees);
        NewEnabledButton("DestroyBox", 80, state.CanRemoveTrees, btnStyle, info.DestroyBoundBox);
        if (GUILayout.Button("Setting...", btnStyle, GUILayout.Width(70)))
        {
            EditorHelper.SelectObject(AreaTreeManager.Instance.gameObject);
        }

        var setting = AreaTreeManager.Instance.nodeSetting;
        setting.MinLevel = EditorGUILayout.IntField("", setting.MinLevel, GUILayout.Width(35));
        //setting.MaxLevel = EditorGUILayout.IntField("MaxL", setting.MaxLevel);
        setting.MaxRenderCount = EditorGUILayout.IntField("", setting.MaxRenderCount, GUILayout.Width(40));
        setting.MinRenderCount = EditorGUILayout.IntField("", setting.MinRenderCount, GUILayout.Width(40));
        setting.MaxVertexCount = EditorGUILayout.IntField("", setting.MaxVertexCount, GUILayout.Width(40));

        //NewEnabledButton("CreateByLOD", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);
        //NewEnabledButton("CreateNoLOD", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);

        //AreaTreeManager.Instance.isCombine = GUILayout.Toggle(AreaTreeManager.Instance.isCombine, "Combine");
        //PrefabInstanceBuilder.Instance.IsCopyTargetRoot = GUILayout.Toggle(PrefabInstanceBuilder.Instance.IsCopyTargetRoot, "Copy");

        //NewButton("GetTreeAreas", buttonWidth, state.CanCreateTrees, btnStyle, info.CreateTreesByLOD);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //bool folderExists = info.IsScenesFolderExists();
        NewEnabledButton("3.CreateScenes", buttonWidth, state.CanCreateScenes, btnStyle, () =>
        {
            //info.DeleteScenesFolder();
            info.EditorCreateNodeScenes();
        });
        NewEnabledButton("RemoveScenes", buttonWidth, state.CanRemoveScenes, btnStyle, info.DestroyScenesEx);
        NewEnabledButton("SelectFolder", buttonWidth, state.sceneCount > 0, btnStyle, info.SelectScenesFolder);
        NewEnabledButton("DeleteFolder", buttonWidth, state.isAllLoaded == true && state.sceneCount > 0, btnStyle, info.DeleteScenesFolder);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        //int unloadedSceneCount = info.SceneList.GetUnloadedScenes().Count;
        NewEnabledButton("4.LoadScenes", buttonWidth, state.CanLoadScenes, btnStyle, () =>
        {
            info.EditorLoadNodeScenesEx();
        });
        NewEnabledButton("UnloadScenes", buttonWidth, state.CanUnloadScenes, btnStyle, info.UnLoadScenes);

        NewEnabledButton("SavePrefab", buttonWidth, state.CanLoadScenes, btnStyle, () =>
        {
            info.EditorSavePrefab();
        });

        //if (NewEnabledButton("LoadPrefab", buttonWidth, info.ModelPrefab != null, btnStyle, info.EditorLoadPrefab))
        //{
        //    return;
        //}
        //GUILayout.Label("ID:" + info.gameObject.GetInstanceID());
        //info.ModelPrefab = BaseEditorHelper.ObjectField(info.ModelPrefab);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("5.OneKey", buttonWidth, state.CanOneKey(), btnStyle, () =>
        {
            // info.InitInOut();
            // info.CreateTreesBSEx();
            // info.EditorCreateNodeScenes();
            info.OneKey_TreeNodeScene();

        });
        NewEnabledButton("Reset", buttonWidth, state.CanReset(), btnStyle, () =>
        {
            info.ResetModel(null);
        });
        NewEnabledButton("SetBuildings", buttonWidth, true, btnStyle, () =>
        {
            SubSceneManager.Instance.SetBuildings_All();
        });
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("ShowRenderers", buttonWidth, true, btnStyle, info.ShowRenderers);
        NewEnabledButton("InitMeshNodes", buttonWidth, true, btnStyle, () =>
        {
            MeshNode.InitNodes(info.gameObject);
        });
        NewEnabledButton("ResaveScenes", buttonWidth, state.CanLoadScenes, btnStyle, () =>
        {
            info.ResaveScenes(null);
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("CombineDoors", buttonWidth, true, btnStyle, () =>
        {
            info.CombineDoors();
        });
        NewEnabledButton("DeleteOthers", buttonWidth, true, btnStyle, () =>
        {
            info.DeleteOthersOfDoor();
        });
        if (GUILayout.Button("|", GUILayout.Width(10)))
        {

        }
        NewEnabledButton("MoveRenderers", buttonWidth, true, btnStyle, () =>
        {
            info.MoveRenderers();
        });
        NewEnabledButton("RecoverParent", buttonWidth, true, btnStyle, () =>
        {
            info.RecoverParent();
        });
        EditorGUILayout.EndHorizontal();

        //BuildingModelState state = info.GetState();
        //if (state.HaveUnloadedScenes)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("SelectUnload", btnStyle, GUILayout.Width(100)))
            {
                //info.SceneList.IsAllLoaded_Debug();

                var scenes = info.SceneList.GetUnloadedScenes();
                Debug.Log($"scenes:{scenes.Count}");
                //EditorHelper.SelectObject(element.rootObj);
                EditorHelper.SelectScriptObjects(scenes);
            }
            if (GUILayout.Button("LoadUnloaded", btnStyle, GUILayout.Width(100)))
            {
                IdDictionary.InitInfos();
                info.LoadUnloadedScenes();
            }
            GUILayout.Label(state.ToString());

            if (GUILayout.Button("DeleteRepeatScenes"))
            {
                info.EditorDeleteOtherRepleatScenes();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private static FoldoutEditorArg drawToolButtonsArg = new FoldoutEditorArg("Tool Buttons", true, false, false, false, true);

    private static FoldoutEditorArg drawModelInfoTableArg = new FoldoutEditorArg("MeshInfo Table", true, false, false, false, true);

    private static FoldoutEditorArg drawListObjectsArg = new FoldoutEditorArg("Objects List",true, false, false, false, true);

    private static void DrawModelInfoTable(BuildingModelInfo info, GUIStyle btnStyle)
    {
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

        NewEnabledButton("LOD", 50, info.LODPart != null, btnStyle, () =>
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.LODPart.gameObject);
        });

        //if (GUILayout.Button("LOD", GUILayout.Width(50)))
        //{
        //    MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.LODPart.gameObject);
        //}
        GUILayout.Button(info.LODVertexCount.ToString("F1") + "w", GUILayout.Width(80));
        GUILayout.Button(info.LODRendererCount.ToString(), GUILayout.Width(50));

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
        if (item == null) return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InitInfoOnEnable"))
        {
            OnEnableFunction();
        }
        GUILayout.Label($"ID:{item.gameObject.GetInstanceID()}");

        GUILayout.Label($"RId:{RendererId.GetRId(item.gameObject).GetText()}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        BuildingModelState state = item.GetState();
        NewEnabledButton("LoadScene", state.CanLoadScenes, () =>
        {
            item.OneKeyLoadScene();
        });
        NewEnabledButton("SaveScene", state.CanCreateTrees, () =>
        {
            item.OneKeySaveScene();
        });
        if (GUILayout.Button("SelectSceneFiles"))
        {
            var scenes = item.GetComponentsInChildren<SubScene_Out0>(true);
            if (scenes.Length > 0)
            {
                var arg = scenes[0].GetSceneArg();
                string scenePath1 = arg.path;
                string sceneAssetPath = arg.GetSceneAssetPath();
                string sceneFilePath = arg.GetSceneFilePath();

                Debug.Log($"ScenePath:{scenePath1}");
                Debug.Log($"AssetPath:{sceneAssetPath}");
                Debug.Log($"DataPath:{Application.dataPath}");
                Debug.Log(sceneFilePath + "|" + System.IO.File.Exists(sceneFilePath));

                FileInfo fileInfo = new FileInfo(sceneFilePath);
                DirectoryInfo dirInfo = fileInfo.Directory;
                Debug.Log($"DirInfo:{dirInfo.FullName}");
                if (dirInfo.Name.Contains("Tree"))
                {
                    DirectoryInfo dirInfo2 = dirInfo.Parent;
                    Debug.Log($"DirInfo2:{dirInfo2.FullName}");
                    string dir1 = dirInfo2.FullName.Replace("\\","/");
                    Debug.Log($"dir1:{dir1}");
                    string assetDir2Path = "Assets"+dir1.Replace(Application.dataPath,"")+"/";
                    Debug.Log($"AssetDir2:{assetDir2Path}");
                    SceneAsset dirAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetDir2Path);
                    if (dirAsset != null)
                    {
                        EditorHelper.SelectObject(dirAsset);
                    }
                    else
                    {
                        Debug.Log($"dirAsset==null");

                        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
                        Debug.Log(sceneAsset);
                        if (sceneAsset != null)
                            EditorHelper.SelectObject(sceneAsset);
                    }
                    //AssetDatabase.Folder
                }
                else
                {
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
                    Debug.Log(sceneAsset);
                    if (sceneAsset != null)
                        EditorHelper.SelectObject(sceneAsset);
                }
            }
            else
            {

            }
        }

        EditorGUILayout.BeginHorizontal();
        NewEnabledButton("LoadLODs", true, () =>
        {
            item.EditorLoadLODsScene();
        });
        NewEnabledButton("SaveLODs", true, () =>
        {
            item.EditorCreateLODsScene();
            EditorHelper.ClearOtherScenes();
        });
        EditorGUILayout.EndHorizontal();

        //NewEnabledButton("UnloadScene", state.CanCreateTrees, () =>
        //    {
        //        AreaTreeManager.Instance.isCombine = false;
        //        item.CreateTreesBSEx();
        //    }); 

        EditorGUILayout.EndHorizontal();

        base.OnToolLayout(item);
        if (item == null) return;

        DrawToolbar(item, contentStyle, buttonWidth);
        if (item == null) return;


        //DrawListObjects(item);

        EditorUIUtils.ToggleFoldout(drawListObjectsArg, () =>
        {
            DrawListObjects(item);
        });
    }

    private void DrawListObjects(BuildingModelInfo item)
    {
        DrawTreeList(treeListArg, () =>
        {
            return item.GetTreeList();
        });
        List<AreaTreeNode> nodes = DrawNodeList(nodeListArg, true, () =>
        {
            return item.GetNodeList();
        });
        DrawSceneList(sceneListArg, () =>
        {
            return item.GetSceneList();
        });

        DrawMeshFilterList(meshListArg, () =>
        {
            List<MeshFilter> meshes = new List<MeshFilter>();
            if (item.InPart)
                meshes.AddRange(item.InPart.GetComponentsInChildren<MeshFilter>(true));
            if (item.OutPart0)
                meshes.AddRange(item.OutPart0.GetComponentsInChildren<MeshFilter>(true));
            if (item.OutPart1)
                meshes.AddRange(item.OutPart1.GetComponentsInChildren<MeshFilter>(true));
            meshes = meshes.Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
            return meshes;
        });

        DrawMeshRendererInfoListEx(meshinfoListArg, item.GetMeshRenderers(false),
            () =>
            {
                return item.GetMeshRenderers(true);
            });

        GlobalMaterialManager.Instance.LocalTarget = item.gameObject;
        DrawMatList(GlobalMaterialManager.Instance, matListArg);

        DoorManager.Instance.LocalTarget = item.gameObject;
        DrawDoorPartList(doorListArg, DoorManager.Instance);

        LODManager.Instance.LocalTarget = item.gameObject;
        DrawLODGroupList(lodGroupListArg, LODManager.Instance);
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using Base.Common;
using System.IO;
using System.Text;

public class MyEditorTools2
{
    #region Hierarchy
    [MenuItem("SceneTools/Hierarchy/InitIds")]
    public static void InitIds()
    {
        IdDictionary.InitInfos();
    }

    [MenuItem("SceneTools/Hierarchy/Init")]
    public static void InitHierarchy()
    {
        HierarchyHelper.InitHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Clear")]
    public static void ClearHierarchy()
    {
        HierarchyHelper.ClearHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Save")]
    public static void SaveHierarchy()
    {
        HierarchyHelper.SaveHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Load")]
    public static void LoadHierarchy()
    {
        HierarchyHelper.LoadHierarchy_All(true);
    }

    [MenuItem("SceneTools/Hierarchy/Check")]
    public static void CheckHierarchy()
    {
        HierarchyHelper.CheckHierarchy();
    }


    #endregion

    #region Debug
    [MenuItem("SceneTools/Debug/Selections")]
    public static void ShowSelections()
    {
        var objs=Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            UnityEngine.Object obj = objs[i];
            Debug.Log($"ShowSelections[{i}] obj:{obj}");
        }
    }
    #endregion

    #region TreeNode
    [MenuItem("SceneTools/TreeNode/ClearRenderers")]
    public static void ClearTreeNodeRenderers()
    {
        AreaTreeNode[] nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true);
        foreach(var node in nodes)
        {
            node.Renderers.RemoveAll(i => i == null);
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
        //MeshRendererInfoEx
    }
    #endregion

    #region MeshRenderer
    [MenuItem("SceneTools/MeshRenderer/ClearRenderers")]
    public static void ClearMeshRendererInfoEx()
    {
        MeshRendererInfoEx[] nodes = GameObject.FindObjectsOfType<MeshRendererInfoEx>(true);
        foreach (var node in nodes)
        {
            node.RemoveNull();
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
        //MeshRendererInfoEx
    }

    //
    #endregion

    #region NavisModelRoot
    [MenuItem("SceneTools/NavisModelRoot/ClearModelDict")]
    public static void ClearNavisModelRoots()
    {
        NavisModelRoot[] nodes = GameObject.FindObjectsOfType<NavisModelRoot>(true);
        foreach (var node in nodes)
        {
            node.BimDict = new BIMModelInfoDictionary();
            node.ModelDict = new ModelItemInfoDictionary();
            node.model2TransformResult = new Model2TransformResult();
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
    }

    //NavisModelRoot
    #endregion

    #region GameObject
    [MenuItem("SceneTools/GameObject/Copy")]
    public static void CopyGameObject()
    {
        GameObject go=MeshHelper.CopyGO(Selection.activeGameObject);
        go.name += "_Copy";
    }
    #endregion

    #region Pipe
    [MenuItem("SceneTools/PipeSystem/OneKeyEx(Job)")]
    public static void PipeOneKeyExJob()
    {
        PipeFactory.Instance.Target = Selection.activeGameObject;
        PipeFactory.Instance.OneKeyEx(true);
    }
    [MenuItem("SceneTools/PipeSystem/Generate")]
    public static void PipeGenerate()
    {
        PipeFactory.Instance.Target = Selection.activeGameObject;
        PipeFactory.Instance.OneKey_Generate(true);
    }
    [MenuItem("SceneTools/PipeSystem/Setting")]
    public static void PipeSetting()
    {
        EditorHelper.SelectObject(PipeFactory.Instance.gameObject);
    }

    [MenuItem("SceneTools/PipeSystem/Window")]
    public static void ShowWindow()
    {
        PipeFactoryEditorWindow.ShowWindow();
    }

    #endregion

    #region LOD 

    [MenuItem("SceneTools/LOD/Clear")]
    public static void ClearLODGroup2()
    {
        ClearComponents<LODGroupInfo>();
        ClearComponents<LODGroup>();
    }

    [MenuItem("SceneTools/LOD/SetSetting")]
    public static void LODSetSetting()
    {
        EditorHelper.SelectObject(LODManager.Instance.gameObject);
    }
    [MenuItem("SceneTools/LOD/SetLODDev")]
    public static void SetLODDev()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/LOD/SetLODBoxMat")]
    public static void SetLODBoxMat()
    {
        DateTime start = DateTime.Now;
        int count = 0;
        var lodMat = LODManager.Instance.LODBoxMat;
        if (lodMat == null)
        {
            Debug.LogError("SetLODBoxMat lodMat == null");
            return;
        }
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            ProgressArg pA = new ProgressArg("SetLODBoxMat", i, renderers.Length, r);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            if (r.name.Contains("_LODBox"))
            {
                r.sharedMaterial = lodMat;
                count++;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetLODBoxMat count:{count} renderers:{renderers.Length}");
    }

    [MenuItem("SceneTools/LOD/LODManager")]
    public static void SelectLODManager()
    {
        EditorHelper.SelectObject(LODManager.Instance);
    }

    [MenuItem("SceneTools/LOD/RemoveOthers")]
    public static void RemoveLODGroupOthers()
    {
        LODHelper.RemoveLODGroupOthers();
    }


    [MenuItem("SceneTools/LOD/GetLODGroups")]
    public static void GetLODGroups()
    {
        LODGroup[] groups = GameObject.FindObjectsOfType<LODGroup>(true);
        GameObject groupRoot = new GameObject("LODGroupList");
        for (int i = 0; i < groups.Length; i++)
        {
            LODGroup group = groups[i];
            EditorHelper.UnpackPrefab(groupRoot.gameObject);
            group.transform.SetParent(groupRoot.transform);
            ProgressArg pA = new ProgressArg("GetLODGroups", i, groups.Length, group);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetLODGroups groups:{groups.Length}");
    }

    [MenuItem("SceneTools/LOD/ClearLODGroups")]
    public static void ClearLODGroups()
    {
        LODHelper.ClearLODGroups(Selection.gameObjects);
    }

    [MenuItem("SceneTools/LOD/ClearLODGroupsByKey_MemberPartPrismatic(MemberPartPrismatic)")]
    public static void ClearLODGroupsByKey_MemberPartPrismatic()
    {
        LODHelper.ClearLODGroupsByKey(Selection.gameObjects, "MemberPartPrismatic");
    }

    [MenuItem("SceneTools/LOD/AddLOD1(U)")]
    public static void AddLOD1_U()
    {
        GameObject go = Selection.activeGameObject;
        Transform t = go.transform;
        if (t.childCount == 1)
        {
            Transform child = t.GetChild(0);
            child.transform.SetParent(go.transform.parent);
            var group = LODManager.Instance.AddLOD1(go, child.gameObject,true);
            //t.transform.SetParent(go.transform);
            //var groupNew = LODHelper.UniformLOD0(group);
        }
        else
        {
            Debug.LogError($"AddLOD1 t.childCount != 1");
        }
    }

    [MenuItem("SceneTools/LOD/AddLOD1")]
    public static void AddLOD1()
    {
        GameObject go = Selection.activeGameObject;
        Transform t = go.transform;
        if (t.childCount == 1)
        {
            Transform child = t.GetChild(0);
            child.transform.SetParent(go.transform.parent);
            var group=LODManager.Instance.AddLOD1(go, child.gameObject,false);
            child.transform.SetParent(go.transform);
            //var groupNew = LODHelper.UniformLOD0(group);
        }
        else
        {
            Debug.LogError($"AddLOD1 t.childCount != 1");
        }
    }

    [MenuItem("SceneTools/LOD/AddLOD2")]
    public static void AddLOD2()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();

    }

    [MenuItem("SceneTools/LOD/AddLOD3")]
    public static void AddLOD3()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();

    }
    #endregion


    #region Mesh 

    [MenuItem("SceneTools/Mesh/Select")]
    public static void SelectMesh()
    {
        GameObject go = Selection.activeGameObject;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        EditorHelper.SelectObject(mesh);
    }

    [MenuItem("SceneTools/Mesh/CombineMeshLeafs")]
    public static void CombineMeshLeafs()
    {
        Dictionary<Transform, List<Transform>> dict = new Dictionary<Transform, List<Transform>>();
        Dictionary<Transform, List<Transform>> dictError = new Dictionary<Transform, List<Transform>>();

        List<MeshFilter> mfList = new List<MeshFilter>();
        foreach (var obj in Selection.gameObjects)
        {
            EditorHelper.UnpackPrefab(obj);
            //ResetRotation(obj);
            var mfs = obj.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < mfs.Length; i++)
            {
                MeshFilter mf = mfs[i];
                if (mf.transform.childCount == 0)
                {
                    mfList.Add(mf);
                    var p = mf.transform.parent;
                    if (p == null)
                    {
                        Debug.LogError($"[{i}/{mfs.Length}] p==null mf:{mf}");
                        continue;
                    }

                    if(dictError.ContainsKey(p))
                    {
                        continue;
                    }

                    
                    if (!dict.ContainsKey(p))
                    {
                        if (IsChildrenAllMesh(p))
                        {
                            dict.Add(p, new List<Transform>());
                        }
                        else
                        {
                            dictError.Add(p, new List<Transform>());
                        }
                       
                        Debug.Log($"[{i}/{mfs.Length}] p[{dict.Count}]:{p}");
                    }

                    if (dict.ContainsKey(p))
                    {
                        dict[p].Add(mf.transform);
                    }

                    
                }
            }
            //mfList.AddRange(mfs);

            Debug.Log($"selection:{obj}");
        }

        int count = 0;
        List<Transform> list = dict.Keys.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            Transform p = list[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CombineMeshLeafs", i, list.Count, p)))
            {
                break;
            }
            if (p == null)
            {
                Debug.LogError($"CombineMeshLeafs p == null {i}/{list.Count}");
                continue;
            }
            count += dict[p].Count;
            if (p.gameObject == null)
            {
                Debug.LogError($"CombineMeshLeafs p.gameObject == null {i}/{list.Count}");
                continue;
            }


            MeshCombineHelper.Combine(p.gameObject);
        }

        foreach (var obj in Selection.gameObjects)
        {
            MeshHelper.DecreaseEmptyGroup(obj);
            MeshHelper.DecreaseEmptyGroup(obj);
            MeshHelper.DecreaseEmptyGroup(obj);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"CombineMeshLeafs list:{list.Count} children:{count}");


    }

    private static bool IsChildrenAllMesh(Transform p)
    {
        for(int i = 0; i < p.childCount; i++)
        {
            var t = p.GetChild(i);

            MeshFilter mf = t.GetComponent<MeshFilter>();
            MeshRenderer mr = t.GetComponent<MeshRenderer>();
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mf == null && mr == null && mc == null)
            {
                return false;
            }
        }
        return true;
    }
    
    [MenuItem("SceneTools/Mesh/Save")]
    public static void SaveMesh()
    {
        EditorHelper.SaveMeshAsset(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Mesh/SaveAll")]
    public static void SaveMeshAll()
    {
        //EditorHelper.SaveMeshAsset(Selection.activeGameObject);

        MeshFilter[] mfs = GameObject.FindObjectsOfType<MeshFilter>(true);
        //foreach(var mf  in mfs)
        //{
        //    if (UnityEditor.AssetDatabase.Contains(mf.sharedMesh)) continue;
        //}


        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(mfs);
        int count = 0;
        foreach (var sharedMesh in sharedMeshInfos)
        {
            if (UnityEditor.AssetDatabase.Contains(sharedMesh.mainMeshFilter.sharedMesh)) continue;
            EditorHelper.SaveMeshAsset(sharedMesh.gameObject);
            count++;
        }

        Debug.Log($"SaveMeshAll sharedMeshInfos:{sharedMeshInfos.Count} count:{count}");
    }

    [MenuItem("SceneTools/Mesh/New")]
    public static void NewMesh()
    {
        CenterPivotAll();

        DateTime startT = DateTime.Now;
        GameObject go = Selection.activeGameObject;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);

        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            //mf.transform.SetParent(null);
            string meshName = mf.name;
            if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("NewMesh", i,mfs.Length,meshName)))
            {
                break;
            }
            //string meshName = mf.sharedMesh.name;
            MeshFilter mfOld = mf.gameObject.GetComponent<MeshFilter>();
            GameObject newGo=MeshCombiner.Instance.CombineToOne(mf.gameObject, false, false);
            MeshFilter mfNew = newGo.GetComponent<MeshFilter>();

            mfNew.name = meshName + "_NewMesh";
            mfNew.sharedMesh.name = meshName + "_NewMesh";

            //mfNew.name = meshName;
            //mfNew.sharedMesh.name = meshName;

            MeshHelper.CopyTransformMesh(newGo, mf.gameObject);

            GameObject.DestroyImmediate(mfNew.gameObject);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"NewMesh go:{go} mfs:{mfs.Length} time:{DateTime.Now-startT}");
    }

    [MenuItem("SceneTools/Mesh/Combine")]
    public static void CombineMesh()
    {
        //MeshCombiner.Instance.CombineToOne(Selection.activeGameObject, true, true);
        MeshCombineHelper.Combine(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Mesh/Combine(Not Save & Destroy)")]
    public static void CombineMesh2()
    {
        MeshCombiner.Instance.CombineToOne(Selection.activeGameObject, false, false);
    }
    [MenuItem("SceneTools/Mesh/Split")]
    public static void SplitMesh()
    {
        MeshCombineHelper.SplitByMaterials(Selection.activeGameObject, false);
    }
    [MenuItem("SceneTools/Mesh/ShowAll")]
    public static void ShowAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetEnbled(meshRenderers, true);
    }
    [MenuItem("SceneTools/Mesh/HideAll")]
    public static void HideAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetEnbled(meshRenderers, false);
    }
    [MenuItem("SceneTools/Mesh/ShowSelection")]
    public static void ShowSelectionMesh()
    {
        var meshRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        SetEnbled(meshRenderers, true);
    }
    [MenuItem("SceneTools/Mesh/HideSelection")]
    public static void HideSelectionMesh()
    {
        var meshRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        SetEnbled(meshRenderers, false);
    }

    [MenuItem("SceneTools/Mesh/HideOthers")]
    public static void HideOthersMesh()
    {
        HideAllMesh();
        ShowSelectionMesh();
    }

    private static void SetEnbled(MeshRenderer[] meshRenderers,bool isEnabled)
    {
        foreach (var mr in meshRenderers)
        {
            mr.enabled = isEnabled;
        }
    }

    #endregion

    #region Prefab
    [MenuItem("SceneTools/Prefab/RemoveNew")]
    public static void PrefabRemoveNew()
    {
        MeshHelper.RemoveNew(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Prefab/CleanErrors")]
    public static void PrefabCleanErrors()
    {
        MeshHelper.DestroyError(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Prefab/SetSetting")]
    public static void PrefabSetSetting()
    {
        EditorHelper.SelectObject(PrefabInstanceBuilder.Instance.gameObject);
    }
    [MenuItem("SceneTools/Prefab/InitMeshNodes")]
    public static void InitMeshNodes()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("InitMeshNodes", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            MeshNode.InitNodes(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos")]
    public static void GetTargetGos()
    {
        GetTargetGos(PrefabInstanceBuilder.Instance.vertexCountOffset);
    }

    [MenuItem("SceneTools/Prefab/InitInstancesDict")]
    public static void InitInstancesDict()
    {
        MeshPrefabInstance.InitInstancesDict();
    }

[MenuItem("SceneTools/Prefab/GetTargetGos(0)")]
    public static void GetTargetGos0()
    {
        GetTargetGos(0);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(5)")]
    public static void GetTargetGos5()
    {
        GetTargetGos(5);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(10)")]
    public static void GetTargetGos10()
    {
        GetTargetGos(10);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(50)")]
    public static void GetTargetGos50()
    {
        GetTargetGos(50);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(100)")]
    public static void GetTargetGos100()
    {
        GetTargetGos(100);
    }

    public static void GetTargetGos(int vertexCountOffset)
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPrefabInfos", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            var mps = PrefabInstanceBuilder.Instance.FilterMeshPoints(obj);
            var dict = AcRTAlignJobContainer.CreateMeshFilterListDict(mps, vertexCountOffset);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/Prefab/GetPrefabInfos")]
    public static void GetPrefabInfos()
    {
        AcRTAlignJobSetting.Instance.SetDefault();

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPrefabInfos", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            PrefabInstanceBuilder.Instance.GetPrefabsOfList(obj);
            MeshNode.InitNodes(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }
    [MenuItem("SceneTools/Prefab/RemoveNew")]
    public static void RemoveNew()
    {
        MeshHelper.RemoveNew(Selection.activeGameObject);
    }

    #endregion

    #region SubScene

    [MenuItem("SceneTools/SubScene/SetBuildings")]
    public static void SetBuildings()
    {
        SubSceneHelper.SetBuildings();
    }
    [MenuItem("SceneTools/SubScene/SetBuildingsWithNavmesh")]
    public static void SetBuildingsWithNavmesh()
    {
        SubSceneHelper.SetBuildingsWithNavmesh(true);
    }
    [MenuItem("SceneTools/SubScene/ClearBuildings")]
    public static void ClearBuildings()
    {
        SubSceneHelper.ClearBuildings();
    }

    [MenuItem("SceneTools/SubScene/GetLODsIds")]
    public static void GetLODsIds()
    {
        IdDictionary.InitInfos();
        var lodsScenes = GameObject.FindObjectsOfType<SubScene_LODs>(true);
        for (int i = 0; i < lodsScenes.Length; i++)
        {
            SubScene_LODs scene = lodsScenes[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetLODsIds", i, lodsScenes.Length, scene)))
            {
                break; 
            }
            RendererId rid = RendererId.GetRId(scene);
            rid.UpdateParent();
            scene.EditorLoadScene();
            
            ////scene.UnLoadSceneAsync();
            scene.UnLoadGosM();
            scene.ShowBounds();
            //scene.EditorCreateScene(true);
        }

        ProgressBarHelper.ClearProgressBar();
        EditorHelper.ClearOtherScenes();
    }

    [MenuItem("SceneTools/SubScene/CheckScenes")]
    public static void CheckScenes()
    {
        SubSceneHelper.CheckSceneIndex(true);
    }

    [MenuItem("SceneTools/SubScene/DeleteOtherRepleatScenes")]
    public static void EditorDeleteOtherRepleatScenes()
    {
        SubSceneManager.Instance.EditorDeleteOtherRepleatScenes(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/LoadBuildingScenes")]
    public static void OneKeyLoadScene()
    {
        BuildingModelManager.Instance.OneKeyLoadScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/SaveBuildingScenes")]
    public static void OneKeySaveScene()
    {
        BuildingModelManager.Instance.OneKeySaveScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/LoadLODsScene")]
    public static void LoadLODs()
    {
        SubSceneManager.Instance.EditorLoadLODs(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/SaveLODsScene")]
    public static void CreateLODs()
    {
        SubSceneManager.Instance.EditorCreateLODs(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes(LOD)")]
    public static void CreateSubScenes_LOD()
    {
        SubSceneHelper.CreateSubScenes<SubScene_LODs>(Selection.gameObjects);
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes")]
    public static void CreateSubScenes()
    {
        SubSceneHelper.CreateSubScenes<SubScene_Single>(Selection.gameObjects);
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes(Children)")]
    public static void CreateSubScenes_Children()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject go = Selection.gameObjects[i];
            for(int j = 0; j < go.transform.childCount; j++)
            {
                GameObject subGo = go.transform.GetChild(j).gameObject;
                if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CreateSubScenes", i, Selection.gameObjects.Length, subGo)))
                {
                    break;
                }
                SubScene_Single subScene = subGo.GetComponent<SubScene_Single>();
                if (subScene == null)
                {
                    subScene = subGo.AddComponent<SubScene_Single>();
                    subScene.IsLoaded = true;
                }
                subScene.EditorCreateScene(true);
            }
        }
        EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/SubScene/Clear")]
    public static void ClearSubScenes()
    {
        ClearComponents<SubScene_Single>();
    }

    [MenuItem("SceneTools/SubScene/ClearSceneArg")]
    public static void ClearSceneArg()
    {
        var scenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
        foreach(var scene in scenes)
        {
            scene.sceneArg.objs.Clear();
        }
        Debug.Log($"ClearSceneArg scenes:{scenes.Length}");
    }

    [MenuItem("SceneTools/SubScene/ClearOtherScenes")]
    public static void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    [MenuItem("SceneTools/SubScene/LoadSubScenes(All)")]
    public static void LoadSubScenes_All()
    {
        SubScene_Single[] scenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Single scene = scenes[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("LoadSubScenes_All", i, scenes.Length, scene)))
            {
                break;
            }
            scene.EditorLoadSceneEx();
        }
        ProgressBarHelper.ClearProgressBar();
    }
    #endregion

    #region ClearComponents

    public static void ClearComponents<T>() where T : Component
    {
        TransformHelper.ClearComponents<T>(Selection.gameObjects);
    }


    [MenuItem("SceneTools/Clear/ClearSceneNullObjs")]
    public static void ClearSceneNullObjs()
    {
        ClearSceneArg();
        ClearTreeNodeRenderers();
        ClearNavisRootComponents();
        ClearNavisModelRoots();//
        ClearMeshRendererInfoEx();
        InitNavisFileInfoByModel.Instance.vueRootModels = new List<NavisPlugins.Infos.ModelItemInfo>();
        SubSceneShowManager.Instance.scenes_Out0 = new List<SubScene_Out0>();
        SubSceneShowManager.Instance.scenes_Out1 = new List<SubScene_Out1>();
    }

    [MenuItem("SceneTools/Clear/BIMObjects")]
    public static void ClearBIMObjects()
    {
        TransformHelper.ClearComponentGos<BIMModelInfo>(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Clear/NotBIMObjects")]
    public static void ClearNotBIMObjects()
    {
        TransformHelper.ClearNotComponentGos<BIMModelInfo>(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Clear/LODGroup")]
    public static void ClearLODGroup()
    {
        ClearComponents<LODGroupInfo>();
        ClearComponents<LODGroup>();
    }

    [MenuItem("SceneTools/Clear/ClearRId")]
    public static void ClearRendererId()
    {
        ClearComponents<RendererId>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshNode")]
    public static void ClearMeshNode()
    {
        ClearComponents<MeshNode>();
    }

    [MenuItem("SceneTools/Clear/DestroyDoors")]
    public static void DestroyDoors()
    {
        DoorsRootList doors =DoorManager.Instance.UpdateAllDoors();
        foreach(DoorsRoot door in doors)
        {
            GameObject.DestroyImmediate(door.gameObject);
        }
    }

    [MenuItem("SceneTools/Clear/DestroyTrees")]
    public static void DestroyTrees()
    {
        //ClearComponents<MeshNode>();

        //TransformHelper.ClearComponentGos<ModelAreaTree>(Selection.activeGameObject);

        TransformHelper.ClearComponentsAllGo<ModelAreaTree>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshNodeAll")]
    public static void ClearMeshNodeAll()
    {
        TransformHelper.ClearComponentsAll<MeshNode>();
    }

    [MenuItem("SceneTools/Clear/ClearRendererInfo")]
    public static void ClearRendererInfo()
    {
        ClearComponents<MeshRendererInfo>();
    }

    [MenuItem("SceneTools/Clear/ClearGenerators")]
    public static void ClearGenerators()
    {
        ClearComponents<PipeMeshGeneratorBase>();
    }

    [MenuItem("SceneTools/Clear/ClearGeneratorArgs")]
    public static void ClearGeneratorArgs()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var ids = obj.GetComponentsInChildren<PipeModelComponent>(true);
            foreach (var id in ids)
            {
                //GameObject.DestroyImmediate(id);
                id.generateArg = null;
            }
            Debug.Log($"ClearGeneratorArgs ids:{ids.Length}");
        }
    }

    [MenuItem("SceneTools/Clear/ClearPipeModels")]
    public static void ClearPipeModels()
    {
        ClearComponents<PipeModelComponent>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshComponents")]
    public static void ClearMeshComponents()
    {
        ClearComponents<MeshFilter>();
        ClearComponents<MeshRenderer>();
        ClearComponents<MeshCollider>();
    }


    [MenuItem("SceneTools/Clear/ClearMesh")]
    public static void ClearMesh()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var mfs = obj.GetComponentsInChildren<MeshFilter>(true);
            foreach (var id in mfs)
            {
                id.sharedMesh = null;
            }
            var mcs = obj.GetComponentsInChildren<MeshCollider>(true);
            foreach (var id in mcs)
            {
                id.sharedMesh = null;
            }
            Debug.Log($"ClearMesh       mfs:{mfs.Length} mcs:{mcs.Length}");
        }
    }


    [MenuItem("SceneTools/Clear/ClearScripts")]
    public static void ClearScripts()
    {
        ClearComponents<MonoBehaviour>();
        //foreach (var obj in Selection.gameObjects)
        //{
        //    var ids = obj.GetComponentsInChildren<MonoBehaviour>(true);
        //    foreach (var id in ids)
        //    {
        //        GameObject.DestroyImmediate(id);
        //    }
        //    Debug.Log($"ClearScripts       ids:{ids.Length}");
        //}
    }

    [MenuItem("SceneTools/Clear/ClearNavisModelRoot")]
    public static void ClearNavisRootComponents()
    {
        ClearComponents<NavisModelRoot>();
        ClearComponents<InitNavisFileInfoByModel>();
    }

    [MenuItem("SceneTools/Clear/ClearBuildingModelInfo")]
    public static void ClearBuildingModelInfo()
    {
        ClearComponents<BuildingModelInfo>();
        ClearComponents<BuildingModelInfoList>();
    }

    [MenuItem("SceneTools/Clear/ClearSubScenesAll")]
    public static void ClearSubScenesAll()
    {
        ClearComponents<SubScene_Base>();
        ClearComponents<SubScene_List>();
    }

    #endregion

    #region Transform

    [MenuItem("SceneTools/Transform/CenterPivot")]
    public static void CenterPivot()
    {
        MeshHelper.CenterPivot(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Transform/CenterPivotAll")]
    public static void CenterPivotAll()
    {
        GameObject root = Selection.activeGameObject;
        MeshHelper.CenterPivotAll(root);
    }

    [MenuItem("SceneTools/Transform/MoveToZero")]
    public static void MoveToZero()
    {
        TransformHelper.MoveToNewRoot(Selection.activeGameObject, "Zero");
    }

    [MenuItem("SceneTools/Transform/SplitParts")]
    public static void SplitParts()
    {
        GameObject newRoot = GameObject.Find("SplitParts");
        if (newRoot == null)
            newRoot = new GameObject("SplitParts");
        GameObject[] gos = Selection.gameObjects;
        foreach (GameObject go in gos)
        {
            //go.transform.SetParent(newRoot.transform);
            //var path = TransformHelper.GetAncestors(go.transform);

            //List<Transform> path = TransformHelper.GetAncestors(go.transform, null);
            //Transform newP = TransformHelper.FindOrCreatePath(newRoot.transform, path, true);
            //go.transform.SetParent(newP.transform);

            TransformHelper.MoveGameObject(go.transform, newRoot.transform);
            //break;
        }
        Debug.LogError($"SplitParts gos:{gos.Length}");
    }

    [MenuItem("SceneTools/Transform/CoypChildren100")]
    public static void CoypChildren100()
    {
        var p = Selection.activeGameObject;
        GameObject go = new GameObject(p.name+"(Copy)");
        go.transform.position = p.transform.position;
        go.transform.SetParent(p.transform.parent);


        for (int i = 0; i < 100 && i < p.transform.childCount; i++)
        {
            var chid = p.transform.GetChild(i);
            var cloned = GameObject.Instantiate(chid.gameObject);
            cloned.transform.SetParent(go.transform);
        }
    }

    [MenuItem("SceneTools/Transform/RemoveOtherBrothers")]
    public static void RemoveOtherBrothers()
    {
        List<GameObject> gos = Selection.gameObjects.ToList();
        var go = Selection.activeGameObject;
        var parent = go.transform.parent;
        List<Transform> childrens = new List<Transform>();
        for(int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            //if (child.gameObject == go) continue;
            if (gos.Contains(child.gameObject)) continue;
            childrens.Add(child);
        }
        foreach(var item in childrens)
        {
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    [MenuItem("SceneTools/Transform/DcsEmpty")]
    public static void DcsEmpty()
    {
        MeshHelper.DecreaseEmptyGroup(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Transform/RmEmpty")]
    public static void RmEmpty()
    {
        MeshHelper.RemoveEmptyObjects(Selection.activeGameObject);
    }

    public static void ResetRotation(GameObject root)
    {
        EditorHelper.UnpackPrefab(root);
        DateTime start = DateTime.Now;
        GameObject tmp = new GameObject("TempParent");
        Dictionary<Transform,Transform> dict = new System.Collections.Generic.Dictionary<Transform, Transform>();
        var ts = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in ts)
        {
            dict.Add(t, t.parent);
            t.transform.SetParent(tmp.transform);
        }

        foreach(var t in ts)
        {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            MeshRenderer mr = t.GetComponent<MeshRenderer>();
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mf == null && mr == null && mc == null)
            {
                t.rotation = Quaternion.identity;
            }
        }

        foreach (var t in dict.Keys)
        {
            t.transform.SetParent(dict[t]);
        }

        Debug.Log($"ResetRotation root:{root} ts:{ts.Length} time:{DateTime.Now-start}");
    }

    [MenuItem("SceneTools/Transform/ClearChildren")]
    public static void ClearChildren()
    {
        foreach (var obj in Selection.gameObjects)
        {
            MeshHelper.ClearChildren(obj.transform);
        }
    }

    [MenuItem("SceneTools/Transform/SelectParent")]
    public static void SelectParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.SelectObject(go.transform.parent);
    }

    [MenuItem("SceneTools/Transform/RootParent")]
    public static void RootParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(null);
        EditorHelper.SelectObject(go);
    }

    [MenuItem("SceneTools/Transform/UpParent")]
    public static void UpParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(null);
        EditorHelper.SelectObject(go);
    }

    [MenuItem("SceneTools/Transform/X10")]
    public static void TransformX10()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position *= 10;
            obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/D100Ex_P")]
    public static void TransformD100Ex_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                children.Add(child);
            }
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(null);
            }
            obj.transform.localScale *= 100;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(obj.transform);
            }
            obj.transform.localScale /= 100;
        }
    }

    [MenuItem("SceneTools/Transform/X10C")]
    public static void TransformX10C()
    {
        foreach (var obj in Selection.gameObjects)
        {
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale *= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_S")]
    public static void TransformD10C_S()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_PS")]
    public static void TransformD10C_PS()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.position /= 10;
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_P")]
    public static void TransformD10C_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10_P")]
    public static void TransformD10_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 10;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/D100_P")]
    public static void TransformD100_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/APP")]
    public static void TransformAPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var parent = obj.transform.parent;
            obj.transform.position += parent.position;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/CSPP")]
    public static void TransformCSPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var p1 = obj.transform.position;

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position -= p1;
            }
        }
    }

    [MenuItem("SceneTools/Transform/GetPositionOffset")]
    public static void GetPositionOffset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            string name = obj.name;
            foreach(var i in allT)
            {
                if (i.name == name && i.gameObject != obj.gameObject)
                {
                    var posOff = i.position - obj.transform.position;
                    Debug.Log("posOffset:" + posOff + "|" + name);
                }
            }
        }
    }

    [MenuItem("SceneTools/Transform/SetParentNull")]
    public static void SetParentNull()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.SetParent(null);
        }
    }

    [MenuItem("SceneTools/Transform/Reset")]
    public static void Reset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero;
        }
    }

    [MenuItem("SceneTools/Transform/LayoutX10")]
    public static void LayoutX10()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero+i*Vector3.forward*1f;
        }
    }
    [MenuItem("SceneTools/Transform/LayoutX05")]
    public static void LayoutX05()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.5f;
        }
    }
    [MenuItem("SceneTools/Transform/LayoutX01")]
    public static void LayoutX01()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.1f;
        }
    }
    #endregion

    #region Renderers

    [MenuItem("SceneTools/Renderers/ShowSelection")]
    public static void ShowSelectionRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/HideSelection")]
    public static void HideSelectionRenders()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/ShowAll")]
    public static void ShowAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/HideAll")]
    public static void HideAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/RemoveOtherIds")]
    public static void RemoveOtherIds()
    {
        var currentIds= Selection.activeGameObject.GetComponentsInChildren<RendererId>(true).ToList();
        var allIds=GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allIds){
            if(currentIds.Contains(id)){
                continue;
            }
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"RemoveOtherIds currentIds:{currentIds.Count} allIds:{allIds.Count}");
    }

    [MenuItem("SceneTools/Renderers/ShowIdDictionary")]
    public static void ShowIdDictionary()
    {
        var dict = IdDictionary.IdDict;
        int i = 0;
        foreach(var key in dict.Keys)
        {
            RendererId id = dict[key];
            if (id == null)
            {
                continue;
            }
            i++;
            Debug.Log($"ShowIdDictionary[{i}] id:{id.Id} pid:{id.parentId} name:{id.name} path:{TransformHelper.GetPath(id.transform)}");
        }
        Debug.Log($"ShowIdDictionary allIds:{dict.Count}");
    }

    [MenuItem("SceneTools/Renderers/ShowTreeNodeDict")]
    public static void ShowTreeNodeDict()
    {
        var dict = AreaTreeHelper.renderId2NodeDict;
        int i = 0;
        foreach (var key in dict.Keys)
        {
            AreaTreeNode id = dict[key];
            if (id == null)
            {
                continue;
            }
            i++;
            Debug.Log($"ShowTreeNodeDict[{i}] tree:{id.tree} name:{id.name} path:{TransformHelper.GetPath(id.transform)}");
        }
        Debug.Log($"ShowTreeNodeDict allIds:{dict.Count}");
    }

    [MenuItem("SceneTools/Renderers/InitTreeNodeDict")]
    public static void InitTreeNodeDict()
    {
        var nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true).ToList();
        int i = 0;
        foreach(var node in nodes)
        {
            node.CreateDictionary();
        }
        Debug.Log($"InitTreeNodeDict nodes:{nodes.Count}");

        ShowTreeNodeDict();
    }

    [MenuItem("SceneTools/Renderers/ShowIds")]
    public static void ShowIds()
    {
        var allIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        for (int i = 0; i < allIds.Count; i++)
        {
            RendererId id = allIds[i];
            Debug.Log($"ShowIds[{i}] id:{id.Id} pid:{id.parentId} name:{id.name} path:{TransformHelper.GetPath(id.transform)}");
        }
        Debug.Log($"ShowIds allIds:{allIds.Count}");
    }

    [MenuItem("SceneTools/Renderers/ClearEmptyIds")]
    public static void ClearEmptyIds()
    {
        int count = 0;
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        StringBuilder sb = new StringBuilder();
       foreach(var rid in rids)
        {
            if (rid == null) continue;
            if (rid.gameObject == null) continue;
            if (string.IsNullOrEmpty(rid.Id))
            {
                count++;
                sb.AppendLine(rid.name);

                GameObject.DestroyImmediate(rid);
            }
        }
        Debug.LogError($"ClearEmptyIds rids:{rids.Length} emptyCount:{count} list:{sb.ToString()}");
    }

    [MenuItem("SceneTools/Renderers/UpdateIds(A)")]
    public static void UpdateRendererIdsA()
    {
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        UpdateRendererIds(rids, true);
    }

    [MenuItem("SceneTools/Renderers/CheckIds(A)")]
    public static void CheckRendererIdsA()
    {
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        UpdateRendererIds(rids, false);
    }

    [MenuItem("SceneTools/Renderers/UpdateIds(S)")]
    public static void UpdateRendererIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, true);
    }

    [MenuItem("SceneTools/Renderers/CheckIds(S)")]
    public static void CheckRendererIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, false);
    }


    [MenuItem("SceneTools/Renderers/UpdateChildrenIds(S)")]
    public static void UpdateRendererChildrenIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, true, true);
    }

    public static void UpdateRendererIds(RendererId[] rids,bool isUpdateId,bool isUpdateChildrenId=false)
    {
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        int count = 0;
        foreach (var rid in rids)
        {
            if (isUpdateChildrenId)
            {
                rid.UpdateChildrenId(false);
            }

            if (ridDict.ContainsKey(rid.Id))
            {
                Debug.LogError($"UpdateRendererIds[{count++}] rid:{rid.Id} name:{rid.name} parent:{rid.transform.parent}");
                if (isUpdateId)
                {
                    rid.NewId();
                    ridDict.Add(rid.Id, rid);
                }
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        Debug.LogError($"UpdateRendererIds rids:{rids.Length} UpdateId:{isUpdateId} UpdateChildren:{isUpdateChildrenId}");
    }

    #endregion

    #region Collider

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/ClearCollders")]
    public static void ClearCollders()
    {
        ClearComponents<Collider>();
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/EnabledColliders")]
    public static void EnabledColliders()
    {
        //ClearComponents<Collider>();
        //TransformHelper.SetCollidersEnabled(Selection.gameObjects)
        var cs = Selection.activeGameObject.GetComponentsInChildren<Collider>(true);
        foreach (var item in cs)
        {
            item.enabled = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/DisenabledColliders")]
    public static void DisenabledColliders()
    {
        //ClearComponents<Collider>();
        //TransformHelper.SetCollidersEnabled(Selection.gameObjects)
        var cs = Selection.activeGameObject.GetComponentsInChildren<Collider>(true);
        foreach (var item in cs)
        {
            item.enabled = false;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/RefreshCollderMesh")]
    public static void RefreshCollderMesh()
    {
        MeshHelper.RefreshCollderMesh();
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider_IsTrigger")]
    public static void AddBoxCollider_IsTrigger()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider_IsTrigger_NotRemoveChild")]
    public static void AddBoxCollider_IsTrigger_NotRemoveChild()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform, false);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 添加所有的MeshCollider
    /// </summary>
    [MenuItem("SceneTools/Collider/AddAllMeshCollider")]
    public static void AddAllMeshCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        AddAllMeshCollider(parent);
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        var meshFilters = parent.GetComponentsInChildren<MeshFilter>(true);
        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }
    #endregion
}

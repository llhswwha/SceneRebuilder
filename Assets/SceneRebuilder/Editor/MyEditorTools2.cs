using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MyEditorTools2
{
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

    #region Mesh 
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
    #endregion


    #region Mesh 
    [MenuItem("SceneTools/Mesh/Combine")]
    public static void CombineMesh()
    {
        MeshCombiner.Instance.CombineToOne(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Mesh/Split")]
    public static void SplitMesh()
    {
        MeshCombineHelper.SplitByMaterials(Selection.activeGameObject, false);
    }
    [MenuItem("SceneTools/Mesh/ShowAll")]
    public static void ShowAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
        SetEnbled(meshRenderers, true);
    }
    [MenuItem("SceneTools/Mesh/HideAll")]
    public static void HideAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>();
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

    [MenuItem("SceneTools/SubScene/CreateSubScenes")]
    public static void CreateSubScenes()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CreateSubScenes", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            SubScene_Single subScene=obj.GetComponent<SubScene_Single>();
            if (subScene == null)
            {
                subScene = obj.AddComponent<SubScene_Single>();
                subScene.IsLoaded = true;
            }
            subScene.EditorCreateScene(true);
        }
        EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();
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

    [MenuItem("SceneTools/SubScene/ClearOtherScenes")]
    public static void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    [MenuItem("SceneTools/SubScene/LoadSubScenes(All)")]
    public static void LoadSubScenes_All()
    {
        SubScene_Single[] scenes = GameObject.FindObjectsOfType<SubScene_Single>();
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

    public static void ClearComponents<T>(GameObject obj) where T : Component
    {
        var ids = obj.GetComponentsInChildren<T>(true);
        foreach (var id in ids)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] ids:{ids.Length}");
    }

    public static void ClearComponents<T>() where T : Component
    {
        foreach (var obj in Selection.gameObjects)
        {
            ClearComponents<T>(obj);
        }
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
    }


    [MenuItem("SceneTools/Clear/ClearMesh")]
    public static void ClearMesh()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var ids = obj.GetComponentsInChildren<MeshFilter>(true);
            foreach (var id in ids)
            {
                id.sharedMesh = null;
            }
            Debug.Log($"ClearMesh       ids:{ids.Length}");
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
    #endregion

    #region Transform

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
        var allT = GameObject.FindObjectsOfType<Transform>();
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
        var allT = GameObject.FindObjectsOfType<Transform>();
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.SetParent(null);
        }
    }

    [MenuItem("SceneTools/Transform/Reset")]
    public static void Reset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        foreach (var obj in Selection.gameObjects)
        {
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero;
        }
    }

    [MenuItem("SceneTools/Transform/LayoutX10")]
    public static void LayoutX10()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
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
        var allT = GameObject.FindObjectsOfType<Transform>();
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
        var allT = GameObject.FindObjectsOfType<Transform>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.1f;
        }
    }
    #endregion

    [MenuItem("SceneTools/Renderers/ShowSelection")]
    public static void ShowSelectionRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
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
            var renderers = obj.GetComponentsInChildren<Renderer>();
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
            var renderers = obj.GetComponentsInChildren<Renderer>();
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
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
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
        var meshFilters = parent.GetComponentsInChildren<MeshFilter>();
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
}

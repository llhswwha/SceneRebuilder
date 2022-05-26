using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneRebuildManager : SingletonBehaviour<SceneRebuildManager>
{
    public void Start()
    {
//#if UNITY_EDITOR
//        SetBuildings();
//#endif
    }

    public SubSceneManager subSceneManager;

    public BuildingModelManager buildingModelManager;

    //public AreaTreeManager areaTreeManager;

    [ContextMenu("InitBuildings")]
    public void InitBuildings()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.InitBuildings();
    }

    [ContextMenu("CombineBuildings")]
    public void CombineBuildings()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.CombineAll();//Model -> CombinedToTree
    }

    [ContextMenu("MoveRenderers")]
    public void MoveRenderers()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.MoveRenderers();//Model -> CombinedToTree
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.ClearTrees();//Model -> CombinedToTree
    }

    [ContextMenu("ClearTrees")]
    public void DestroyBox()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.DestroyBox();//Model -> CombinedToTree
    }

    public List<BuildingModelInfo> GetBuildings()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        return buildingModelManager.Buildings;
    }

    public List<ModelAreaTree> GetTrees()
    {
        //if (areaTreeManager == null) areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //return areaTreeManager.Trees;
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        return buildingModelManager.GetTrees();
    }

    public List<AreaTreeNode> GetLeafNodes()
    {

        List<AreaTreeNode> nodes = new List<AreaTreeNode>();
        var trees = GetTrees();
        trees.ForEach(t =>
        {
            if (t != null)
                nodes.AddRange(t.TreeLeafs);
        }
        );
        return nodes;
        //return GameObject.FindObjectsOfType<AreaTreeNode>(true).Where(n => n.IsLeaf).ToList();
    }

    public SubSceneBag GetScenes()
    {
        if (subSceneManager == null)
        {
            subSceneManager = SubSceneManager.Instance;
        }
        return subSceneManager.GetScenes();
    }

    public void UpdateList()
    {
        //if (areaTreeManager == null) areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //areaTreeManager.UpdateTrees();

        //buildingModelManager.UpdateBuildings();

        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        if (subSceneManager == null)
        {
            subSceneManager = SubSceneManager.Instance;
        }

        //if (buildingModelManager)
            buildingModelManager.UpdateTrees();//.UpdateBuildings();
        //if(subSceneManager)
            subSceneManager.UpdateScenes();
    }

    [ContextMenu("SaveScenes")]
    public void SaveScenes()
    {
        if (subSceneManager == null)
        {
            subSceneManager = SubSceneManager.Instance;
        }
        subSceneManager.contentType = SceneContentType.TreeNode;
        subSceneManager.EditorCreateBuildingScenes(); // CombinedTree To Scene
    }

#if UNITY_EDITOR
    public void SavePrefabs()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        BuildingModelInfoList.SavePrefabs(buildings);
    }


    public void LoadPrefabs()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        BuildingModelInfoList.LoadPrefabs(buildings);
    }

#endif

    [ContextMenu("SetBuildings")]
    public void SetBuildings()
    {
        subSceneManager.SetBuildings_All();
    }

    //[ContextMenu("SetBuildingsActive")]
    //public void SetBuildingsActive()
    //{
    //    subSceneManager.SetBuildings_Parts();
    //}

    [ContextMenu("OneKey_Save")]
    public void OneKey_Save()
    {
        //DateTime start = DateTime.Now;
        //InitBuildings();
        //CombineBuildings();
        //SaveScenes();
        //SetBuildings();
        //Debug.LogError($"OneKey_Save Time:{(DateTime.Now - start).ToString()}");
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.OneKey_Save();
       
    }

    [ContextMenu("OneKey_Reset")]
    public void OneKey_Reset()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.OneKey_Reset();
    }

    [ContextMenu("OneKey_Save")]
    public void OneKey_Resave()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.OneKey_Resave();
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        if (subSceneManager == null)
        {
            subSceneManager = SubSceneManager.Instance;
        }
        subSceneManager.contentType = SceneContentType.TreeNode;
        subSceneManager.EditorLoadScenes();
    }

    [ContextMenu("UnLoadScenes")]
    public void UnLoadScenes()
    {
        if (subSceneManager == null)
        {
            subSceneManager = SubSceneManager.Instance;
        }
        subSceneManager.contentType = SceneContentType.TreeNode;
        //subSceneManager.UnLoadScenesAsync();
        subSceneManager.UnLoadScenes();
    }

    [ContextMenu("RemoveScenes")]
    public void RemoveScenes()
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.DestroyScenes();
    }

    [ContextMenu("ClearBuildings")]
    public void ClearBuildings()
    {
        SubSceneHelper.ClearBuildings();
    }

    public void ShowBuildings()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        int count = 0;
        foreach(var b in buildings)
        {
            bool isActive = b.gameObject.activeSelf;
            if (isActive == false)
            {
                b.gameObject.SetActive(true);
                count++;
            }
        }
        Debug.Log($"ShowBuildings buildings:{buildings.Length} hiddenCount:{count}");
    }

    public void SetModelsActive(bool v)
    {
        if (buildingModelManager == null)
        {
            buildingModelManager = BuildingModelManager.Instance;
        }
        buildingModelManager.SetModelsActive(v);
    }

    //[ContextMenu("LoadScenes")]
    //public void LoadScenes()
    //{
    //    SubSceneManager.contentType = SceneContentType.TreeWithPart;
    //    SubSceneManager.EditorLoadScenes();
    //}

    //[ContextMenu("LoadTrees")]
    //public void LoadTrees()
    //{
    //    SubSceneManager.contentType = SceneContentType.Tree;
    //    SubSceneManager.EditorLoadScenes();
    //}

    //[ContextMenu("LoadOut0")]
    //public void LoadOut0()
    //{
    //    SubSceneManager.contentType = SceneContentType.Tree;
    //    SubSceneManager.EditorLoadScenes();
    //}

    //[ContextMenu("LoadOutTree")]
    //public void LoadOutTree()
    //{
    //    SubSceneManager.contentType = SceneContentType.Tree;
    //    SubSceneManager.EditorLoadScenes();
    //}

    //[ContextMenu("LoadOutPart")]
    //public void LoadOutPart()
    //{
    //    SubSceneManager.contentType = SceneContentType.Tree;
    //    SubSceneManager.EditorLoadScenes();
    //}


    //public void LoadScenesOfChildren<T>(GameObject buildingOrFloor, Action<SceneLoadProgress> finishedCallback) where T : SubScene_Base
    //{
    //    T[] subScenes = buildingOrFloor.GetComponentsInChildren<T>(true);
    //    SubSceneManager.Instance.LoadScenesEx(subScenes, finishedCallback);
    //}

    public void TestLoadBySetting()
    {

    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneRebuildManager : MonoBehaviour
{

    public SubSceneManager subSceneManager;

    public BuildingModelManager buildingModelManager;

    //public AreaTreeManager areaTreeManager;

    [ContextMenu("InitBuildings")]
    public void InitBuildings()
    {
        buildingModelManager.InitBuildings();
    }

    [ContextMenu("CombineBuildings")]
    public void CombineBuildings()
    {
        buildingModelManager.CombineAll();//Model -> CombinedToTree
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        buildingModelManager.ClearTrees();//Model -> CombinedToTree
    }

    public List<BuildingModelInfo> GetBuildings()
    {
        return buildingModelManager.Buildings;
    }

    public List<ModelAreaTree> GetTrees()
    {
        //if (areaTreeManager == null) areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //return areaTreeManager.Trees;
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

    public List<SubScene_Base> GetScenes()
    {
        return subSceneManager.GetScenes();
    }

    public void UpdateList()
    {
        //if (areaTreeManager == null) areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        //areaTreeManager.UpdateTrees();

        //buildingModelManager.UpdateBuildings();
        
        buildingModelManager.UpdateTrees();//.UpdateBuildings();
        subSceneManager.UpdateScenes();
    }

    [ContextMenu("SaveScenes")]
    public void SaveScenes()
    {
        subSceneManager.contentType = SceneContentType.TreeNode;
        subSceneManager.EditorCreateBuildingScenes(); // CombinedTree To Scene
    }

    [ContextMenu("SetBuildings")]
    public void SetBuildings()
    {
        subSceneManager.SetBuildings_Parts();
    }


    [ContextMenu("OneKey")]
    public void OneKey()
    {
        DateTime start = DateTime.Now;
        InitBuildings();
        CombineBuildings();
        SaveScenes();
        SetBuildings();
        Debug.LogError($"OneKey Time:{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        subSceneManager.contentType = SceneContentType.TreeNode;
        subSceneManager.EditorLoadScenes();
    }

    [ContextMenu("ClearBuildings")]
    public void ClearBuildings()
    {
        subSceneManager.ClearBuildings();
    }

    public void SetModelsActive(bool v)
    {
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

}
#endif
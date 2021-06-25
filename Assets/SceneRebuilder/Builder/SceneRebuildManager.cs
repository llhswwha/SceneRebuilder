using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneRebuildManager : MonoBehaviour
{
    public SubSceneManager subSceneManager;

    public BuildingModelManager buildingModelManager;

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

    [ContextMenu("SaveScenes")]
    public void SaveScenes()
    {
        subSceneManager.contentType = SceneContentType.TreeWithPart;
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

    [ContextMenu("ClearBuildings")]
    public void ClearBuildings()
    {
        subSceneManager.ClearBuildings();
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
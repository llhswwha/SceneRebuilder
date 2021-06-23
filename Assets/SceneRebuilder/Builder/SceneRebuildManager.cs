using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneRebuildManager : MonoBehaviour
{
    public SubSceneManager SubSceneManager;

    public BuildingModelManager BuildingModelManager;

    [ContextMenu("InitBuildings")]
    public void InitBuildings()
    {
        BuildingModelManager.InitBuildings();
    }

    [ContextMenu("CombineBuildings")]
    public void CombineBuildings()
    {
        BuildingModelManager.CombineAll();//Model -> CombinedToTree
    }

    [ContextMenu("SaveScenes")]
    public void SaveScenes()
    {
        SubSceneManager.contentType = SceneContentType.TreeWithPart;
        SubSceneManager.EditorCreateBuildingScenes(); // CombinedTree To Scene
    }

    [ContextMenu("SetBuildings")]
    public void SetBuildings()
    {
        SubSceneManager.SetBuildings_Parts();
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
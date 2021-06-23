using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    [ContextMenu("OneKey")]
    public void OneKey()
    {
        InitBuildings();
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        SubSceneManager.contentType = SceneContentType.TreeWithPart;
        SubSceneManager.EditorLoadScenes();
    }

    [ContextMenu("LoadTrees")]
    public void LoadTrees()
    {
        SubSceneManager.contentType = SceneContentType.Tree;
        SubSceneManager.EditorLoadScenes();
    }

    [ContextMenu("LoadOut0")]
    public void LoadOut0()
    {
        SubSceneManager.contentType = SceneContentType.Tree;
        SubSceneManager.EditorLoadScenes();
    }

    [ContextMenu("LoadOutTree")]
    public void LoadOutTree()
    {
        SubSceneManager.contentType = SceneContentType.Tree;
        SubSceneManager.EditorLoadScenes();
    }

    [ContextMenu("LoadOutPart")]
    public void LoadOutPart()
    {
        SubSceneManager.contentType = SceneContentType.Tree;
        SubSceneManager.EditorLoadScenes();
    }

}
#endif
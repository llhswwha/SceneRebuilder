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
        SubSceneManager.EditorCreateBuildingScenes(); // CombinedTree To Scene
    }

    [ContextMenu("LoadOut0")]
    public void LoadOut0()
    {
        //SubSceneManager //LoadScenes
    }

    [ContextMenu("OneKey")]
    public void OneKey()
    {
        InitBuildings();
    }
}
#endif
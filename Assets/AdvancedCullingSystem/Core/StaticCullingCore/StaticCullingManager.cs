using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticCullingManager : Singleton<StaticCullingManager>
{
    //public Scene StaticCullingScene;
    public string StaticCullingPath = @"Assets\Scenes\MinHang\StaticCullingTree.unity";//Assets\Scenes\MinHang\StaticCulling.unity
    public int StaticCullingIndex = 0;

    private void Start()
    {
        LoadSceneAsync();
    }

    [ContextMenu("LoadSceneAsync")]
    private void LoadSceneAsync()
    {
        AsyncOperation op=SceneManager.LoadSceneAsync(StaticCullingIndex, LoadSceneMode.Additive);
    }

    [ContextMenu("AddToBuildingsScene")]
    private void AddToBuildingsScene()
    {
        Debug.Log($"LoadScene Path:{StaticCullingPath}");
        EditorBuildSettingsScene[] buildingScenes = EditorBuildSettings.scenes;
        if (!File.Exists(StaticCullingPath))
        {
            Debug.LogErrorFormat("Path:{0} not exist!", StaticCullingPath);
            EditorBuildSettings.scenes = buildingScenes;
        }
        else
        {
            List<EditorBuildSettingsScene> buildingScenes2 = buildingScenes.ToList();
            buildingScenes2.Add(new EditorBuildSettingsScene(StaticCullingPath, true));
            EditorBuildSettings.scenes = buildingScenes2.ToArray();
            StaticCullingIndex = buildingScenes2.Count-1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManager : MonoBehaviour
{
    public string RootDir = "SubScenes";

    public string SceneDir = "MinHangBuildings";

    public string SceneName = "abc";

    public bool IsOverride = true;

    private string GetScenePath(string sceneName)
    {
        //return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
        //return Application.dataPath + SaveDir + sceneName + ".unity";
        return $"{Application.dataPath}/{RootDir}/{SceneDir}/{sceneName}.unity";
    }

#if UNITY_EDITOR
    [ContextMenu("TestCreateDir")]
    public void TestCreateDir()
    {

        EditorHelper.CreateDir(RootDir, SceneDir);

    }

    public int Count = 1;

    [ContextMenu("TestCreateScene")]
    public void TestCreateScene()
    {
        string scenePath = GetScenePath(SceneName);
        List<GameObject> gos = new List<GameObject>();
        for (int i = 0; i<Count;i++)
        {
            gos.Add(new GameObject($"go_{Count}_{i}"));
        }
        Count++;
        EditorHelper.CreateScene( scenePath, IsOverride, gos.ToArray());
    }

    [ContextMenu("TestCloseScene")]
    public void TestCloseScene()
    {
        string scenePath = GetScenePath(SceneName);

        Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(scenePath);
        Debug.Log("scene IsValid:" + scene.IsValid());
        if (scene.IsValid() == true)//打开
        {
            bool r=UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);//关闭场景，不关闭无法覆盖
            Debug.Log("r:" + r);

        }
    }
#endif
}

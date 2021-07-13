using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SubSceneCreater : MonoBehaviour
{
    public SubScene_List SceneList = null;

    //[ContextMenu("GetTreeNodeScenes")]
    public void GetTreeNodeScenes()
    {
        if (SceneList == null)
        {
            SceneList = this.gameObject.GetComponent<SubScene_List>();
        }
        if (SceneList == null)
        {
            SceneList = this.gameObject.AddComponent<SubScene_List>();
            SceneList.Init();
        }
    }

    public void UpdateSceneList()
    {
        if (SceneList == null)
        {
            SceneList = this.gameObject.GetComponent<SubScene_List>();
        }
        if (SceneList == null)
        {
            SceneList = this.gameObject.AddComponent<SubScene_List>();
        }
        SceneList.Init();
    }

    public int GetSceneCount()
    {
        GetTreeNodeScenes();
        return SceneList.sceneCount;
    }

    public bool IsSceneLoaded()
    {
        GetTreeNodeScenes();
        return SceneList.IsAllLoaded();
    }

    private void InitSceneListGO()
    {
        if (SceneList == null)
        {
            GameObject go = new GameObject("SubScenes");
            go.transform.position = this.transform.position;
            go.transform.SetParent(this.transform);
            SceneList = go.AddComponent<SubScene_List>();
            SceneList.Init();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("EditorMoveScenes")]
    public void EditorMoveScenes()
    {
        InitSceneListGO();
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        foreach (var scene in scenes)
        {
            SceneList.AddScene(scene);
            GameObject.DestroyImmediate(scene);
        }
        var scenes2 = SceneList.gameObject.GetComponentsInChildren<SubScene_Base>(true);
        SubSceneHelper.LinkScenes(scenes2);
    }

    [ContextMenu("DestroyScenes")]
    public void DestroyScenes()
    {
        //Debug.Log("DestroyOldPartScenes");
        if (SceneList == null)
        {
            //SceneList = this.GetComponentInChildren<SubScene_List>();
            GetTreeNodeScenes();
        }
        if (SceneList != null)
        {
            SceneList.Clear();
        }
        else
        {
            var components = this.GetComponentsInChildren<SubScene_Base>(true);//In Out0 Out1
            foreach (var c in components)
            {
                //if (c.contentType == contentType)
                GameObject.DestroyImmediate(c);
            }
        }

    }

    public void EditorCreateScenes(List<SubScene_Base> scenes, Action<float, int, int> progressChanged)
    {
        int count = scenes.Count;
        //Debug.Log("EditorCreateScenes:" + count);
        for (int i = 0; i < count; i++)
        {
            SubScene_Base scene = scenes[i];

            if (scene.gos.Count == 0)
            {
                Debug.LogError($"EditorCreateScenes scene.gos.Count == 0 Scene:{scene.name}");
                GameObject.DestroyImmediate(scene);
                continue;
            }
            //scene.IsLoaded = true;
            scene.SaveScene();
            scene.ShowBounds();

            float progress = (float)i / count;
            float percents = progress * 100;
            if (progressChanged != null)
            {
                progressChanged(progress, i, count);
            }
            else
            {
                Debug.Log($"EditorCreateScenes progress:{progress:F2},percents:{percents:F2}");
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateScenes", $"{i}/{count} {percents:F2}% of 100%", progress))
                {
                    break;
                }
            }
            //System.Threading.Thread.Sleep(1000);
        }
        if (progressChanged == null)
            ProgressBarHelper.ClearProgressBar();

        UpdateSceneList();
    }

    public void LoadUnloadedScenes()
    {
        SceneList.LoadUnloadedScenes();
    }

    public void EditorLoadScenes(Action<float> progressChanged)
    {
        SubScene_Base[] scenes=gameObject.GetComponentsInChildren<SubScene_Base>(true);
        EditorLoadScenes(scenes,progressChanged);
    }

    public static void EditorLoadScenes(List<SubScene_Base> scenes, Action<float> progressChanged)
    {
        EditorLoadScenes(scenes.ToArray(), progressChanged);
    }

        public static void EditorLoadScenes(SubScene_Base[] scenes, Action<float> progressChanged)
    {
        Debug.Log("EditorLoadScenes:" + scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.IsLoaded = false;
            scene.EditorLoadSceneEx();

            float progress = (float)i / scenes.Length;
            float percents = progress * 100;

            if (progressChanged != null)
            {
                progressChanged(progress);
            }
            else
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadScenes", $"{i}/{scenes.Length} {percents:F2}% of 100%", progress))
                {
                    break;
                }
            }
        }
        if (progressChanged != null)
        {
            progressChanged(1);
        }
        else
        {
            ProgressBarHelper.ClearProgressBar();
        }

        //this.InitInOut(false);
        //SceneState = "EditLoadScenes_Part";
    }

#endif

    protected List<SubScene_Base> GetSubScenesOfTypes(List<SceneContentType> types)
    {
        List<SubScene_Base> list = new List<SubScene_Base>();
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            if (types.Contains(scene.contentType))
            {
                list.Add(scene);
            }
        }
        return list;
    }


    //[ContextMenu("Base> SaveTreeRendersId")]
    public virtual void SaveTreeRendersId()
    {
        Debug.Log("SubSceneCreater.SaveTreeRendersId");
    }

    // [ContextMenu("* UnLoadScenes")]
    // public void UnLoadScenes()
    // {
    //     var scenes = this.GetComponentsInChildren<SubScene_Base>(true);
    //    foreach (var scene in scenes)
    //    {
    //        scene.UnLoadGosM();
    //    }
    // }

    [ContextMenu("ShowSceneBounds")]
    public void ShowSceneBounds()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        foreach (var scene in scenes)
        {
            scene.ShowBounds();
        }
    }

    [ContextMenu("UnLoadScenes")]
    public virtual void UnLoadScenes()
    {
        // var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        // foreach(var scene in scenes)
        // {
        //     scene.UnLoadGosM();
        //     scene.ShowBounds();
        // }

        UnLoadScenes(null);
    }

    public void UnLoadScenes(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;

        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            var scene = scenes[i];
            if (scene == null) continue;
            float progress = (float)i / scenes.Length;
            float percents = progress * 100;

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar("UnLoadScenes", $"Progress1 {i}/{ scenes.Length} {percents:F2}%", progress))
                {
                    break;
                }
            }
            else
            {
                progressChanged(progress);
            }
             scene.UnLoadGosM();
            scene.ShowBounds();
        }

        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(1);
        }

        Debug.Log($"UnLoadScenes time:{(DateTime.Now - start)}");
    }

}

public enum ScenesState
{

}

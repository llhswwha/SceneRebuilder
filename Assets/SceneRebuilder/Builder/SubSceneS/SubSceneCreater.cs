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
    public SubScene_Base[] GetScenes()
    {
        if (SceneList == null)
        {
            SceneList = this.gameObject.GetComponent<SubScene_List>();
        }
        if (SceneList == null)
        {
            SceneList = this.gameObject.AddComponent<SubScene_List>();
            SceneList.GetScenes();
        }
        return SceneList.scenes;
    }

    public List<SubScene_Base> GetSceneList()
    {
        return GetScenes().ToList();
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
        SceneList.GetScenes();
    }

    public int GetSceneCount()
    {
        GetScenes();
        return SceneList.sceneCount;
    }

    public bool IsSceneLoaded()
    {
        GetScenes();
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
            SceneList.GetScenes();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("EditorMoveScenes")]
    public void EditorMoveScenes()
    {
        InitSceneListGO();
        var scenes = SubScene_List.GetBaseScenes(gameObject);
        foreach (var scene in scenes)
        {
            SceneList.AddScene(scene);
            GameObject.DestroyImmediate(scene);
        }
        var scenes2 = SubScene_List.GetBaseScenes(SceneList.gameObject);
        SubSceneHelper.LinkScenes(scenes2);
    }

    [ContextMenu("DestroyScenes")]
    public void DestroyScenes()
    {
        //Debug.Log("DestroyOldPartScenes");
        if (SceneList == null)
        {
            //SceneList = this.GetComponentInChildren<SubScene_List>();
            GetScenes();
        }
        if (SceneList != null)
        {
            SceneList.Clear();
        }
        else
        {
            var components = SubScene_List.GetBaseScenes(this.gameObject);//In Out0 Out1
            foreach (var c in components)
            {
                //if (c.contentType == contentType)
                GameObject.DestroyImmediate(c);
            }
        }

    }

    public void EditorCreateScenes(List<SubScene_Base> scenes, Action<ProgressArg> progressChanged)
    {
        //int count = scenes.Count;
        ////Debug.Log("EditorCreateScenes:" + count);
        //for (int i = 0; i < count; i++)
        //{
        //    SubScene_Base scene = scenes[i];

        //    if (scene.gos.Count == 0)
        //    {
        //        Debug.LogError($"EditorCreateScenes scene.gos.Count == 0 Scene:{scene.name}");
        //        GameObject.DestroyImmediate(scene);
        //        continue;
        //    }
        //    //scene.IsLoaded = true;
        //    scene.SaveScene();
        //    scene.ShowBounds();

        //    float progress = (float)i / count;
        //    float percents = progress * 100;
        //    if (progressChanged != null)
        //    {
        //        progressChanged(progress, i, count);
        //    }
        //    else
        //    {
        //        Debug.Log($"EditorCreateScenes progress:{progress:F2},percents:{percents:F2}");
        //        if (ProgressBarHelper.DisplayCancelableProgressBar("EditorCreateScenes", $"{i}/{count} {percents:F2}% of 100%", progress))
        //        {
        //            break;
        //        }
        //    }
        //    //System.Threading.Thread.Sleep(1000);
        //}
        //if (progressChanged == null)
        //    ProgressBarHelper.ClearProgressBar();

        SubSceneHelper.EditorCreateScenes(scenes, progressChanged);

        UpdateSceneList();
    }

    public void LoadUnloadedScenes()
    {
        SceneList.LoadUnloadedScenes();
    }

    public virtual void EditorLoadScenes(Action<ProgressArg> progressChanged)
    {
        SubScene_Base[] scenes= SubScene_List.GetBaseScenes(gameObject);
        EditorLoadScenes(scenes,progressChanged);
    }

    public static void EditorLoadScenes(List<SubScene_Base> scenes, Action<ProgressArg> progressChanged)
    {
        EditorLoadScenes(scenes.ToArray(), progressChanged);
    }

        public static void EditorLoadScenes(SubScene_Base[] scenes, Action<ProgressArg> progressChanged)
    {
        Debug.Log("EditorLoadScenes:" + scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Base scene = scenes[i];
            scene.IsLoaded = false;
            scene.EditorLoadSceneEx();
            ProgressArg p = new ProgressArg("EditorLoadScenes", i, scenes.Length, scene);
            if (progressChanged != null)
            {
                progressChanged(p);
            }
            else
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p))
                {
                    break;
                }
            }
        }
        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("EditorLoadScenes", scenes.Length, scenes.Length));
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
        var scenes = SubScene_List.GetBaseScenes(gameObject);
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
        var scenes = SubScene_List.GetBaseScenes(gameObject);
        foreach (var scene in scenes)
        {
            scene.ShowBounds();
        }
    }

    [ContextMenu("UnLoadScenes")]
    public virtual void UnLoadScenes()
    {
        UnLoadScenes(null);
    }

    public void UnLoadScenes(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        var scenes = SubScene_List.GetBaseScenes(gameObject);
        for (int i = 0; i < scenes.Length; i++)
        {
            var scene = scenes[i];
            if (scene == null) continue;
            ProgressArg progressArg=new ProgressArg("UnLoadScenes", i, scenes.Length,scene);
            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(progressArg))
                {
                    break;
                }
            }
            else
            {
                progressChanged(progressArg);
            }
             scene.UnLoadGosM();
            scene.ShowBounds();
        }

        if (progressChanged == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("UnLoadScenes", scenes.Length, scenes.Length));
        }

        Debug.Log($"UnLoadScenes time:{(DateTime.Now - start)}");
    }

}

public class ProgressArg
{
    public string title = "";
    public float progress;
    public int i;
    public int count;
    public object tag;

    //public ProgressArg(float p,object t=null)
    //{
    //    this.progress = p;
    //    this.tag = t;
    //}

    //public ProgressArg(string title, float p, object t = null)
    //{
    //    this.title = title;
    //    this.progress = p;
    //    this.tag = t;
    //}

    //public ProgressArg(int i,int count, object t = null)
    //{
    //    this.i = i;
    //    this.count = count;
    //    this.progress = (float)i / count;
    //    this.tag = t;
    //}

    public ProgressArg(string title,int i, int count, object t = null)
    {
        this.title = title;
        this.i = i;
        this.count = count;
        this.progress = (float)i / count;
        this.tag = t;

        if (count == 0)
        {
            Debug.LogError($"ProgressArg count==0 title:{title} i:{i} t:{t}");
        }

        if (float.IsNaN(progress))
        {
            Debug.LogError($"ProgressArg IsNaN title:{title} i:{i} t:{t}");
        }
    }

    private string GetProgress()
    {
        return $"{i}/{count} {progress:P1}";
    }

    public override string ToString()
    {
        if (subP == null)
        {
            return $"P1{GetProgress()} ({tag})";
        }
        else
        {
            //"{i1}/{count1} {i2}/{count2} {progress1:P1}"
            //return $"Progress2 [{i}/{count} > {subP.i}/{subP.count}] [{progress:P1} > {subP.progress:P1}]";

            //return $"P2[{GetProgress()}]>[{subP.GetProgress()}] ({tag}>{subP.tag})";
            var subP2 = subP.subP;
            if (subP2 == null)
            {
                return $"P2[{GetProgress()}]>[{subP.GetProgress()}] ({tag}>{subP.tag})";
            }
            else
            {
                //return $"P3[{GetProgress()}]>[{subP.GetProgress()}]>[{subP.subP.GetProgress()}] ({tag}>{subP.tag}>{subP.subP.tag})";
                var subP3 = subP2.subP;
                if (subP3 == null)
                {
                    return $"P3[{GetProgress()}]>[{subP.GetProgress()}]>[{subP2.GetProgress()}] ({tag}>{subP.tag}>{subP2.tag})";
                }
                else
                {
                    return $"P4[{GetProgress()}]>[{subP.GetProgress()}]>[{subP2.GetProgress()}]>[{subP3.GetProgress()}] ({tag}>{subP.tag}>{subP2.tag}>{subP3.tag})";
                }
            }
        }
        
    }

    public string GetTitle()
    {
        if (subP == null)
        {
            return $"T1{title}";
        }
        else
        {
            //"{i1}/{count1} {i2}/{count2} {progress1:P1}"
            //return $"Progress2 [{i}/{count} > {subP.i}/{subP.count}] [{progress:P1} > {subP.progress:P1}]";

            //return $"P2[{GetProgress()}]>[{subP.GetProgress()}] ({tag}>{subP.tag})";

            if (subP.subP == null)
            {
                return $"T2[{title}]>[{subP.title}]";
            }
            else
            {
                
                if (subP.subP.subP == null)
                {
                    return $"T3[{title}]>[{subP.title}]>[{subP.subP.title}]";
                }
                else
                {
                    
                    return $"T3[{title}]>[{subP.title}]>[{subP.subP.title}]>[{subP.subP.subP.title}]";
                }
            }
        }
    }

    public ProgressArg subP;

    internal void AddSubProgress(ProgressArg p)
    {
        subP = p;
        this.progress = (i + p.progress) / count;
    }
}

public enum ScenesState
{

}

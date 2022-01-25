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

    public void EditorCreateScenes(SubSceneBag scenes, Action<ProgressArg> progressChanged)
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

    public static void EditorLoadScenes(SubSceneBag scenes, Action<ProgressArg> progressChanged)
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

    protected SubSceneBag GetSubScenesOfTypes(List<SceneContentType> types)
    {
        SubSceneBag list = new SubSceneBag();
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

public class ProgressArgEx : List<ProgressArg>, IProgressArg
{
    public ProgressArgEx()
    {

    }

    public ProgressArgEx(ProgressArg arg)
    {
        this.Add(arg);
    }

    public float GetProgress()
    {
        return this[0].GetProgress();
    }

    public string GetTitle()
    {
        return this[0].GetTitle();
    }

    public override string ToString()
    {
        if (this.Count > 0)
        {
            return this[0].ToString();
        }
        else
        {
            return base.ToString();
        }
    }

    internal void AddSubProgress(ProgressArg subProgress)
    {
        foreach (var item in this)
        {
            if (item == subProgress)
            {
                Debug.LogError("ExistProgress:" + subProgress);
                return;
            }
        }
        this.Last().AddSubProgress(subProgress);
        this.Add(subProgress);
    }

    public IProgressArg Clone()
    {
        ProgressArgEx list = new ProgressArgEx();
        list.AddRange(this);
        return list;
    }
}

public interface IProgressArg
{
    string GetTitle();

    float GetProgress();

    IProgressArg Clone();
}

public class ProgressArg: IProgressArg
{
    public IProgressArg Clone()
    {
        ProgressArg arg = new ProgressArg();
        arg.title = this.title;
        //arg.progress = this.progress;
        arg.i = this.i;
        arg.count = this.count;
        arg.tag = this.tag;
        if (this.subP != null)
        {
            //arg.subP = this.subP.Clone();
            arg.subP = this.subP;
        }
        return arg;
    }

    public string title = "";
    public float progress;

    public float GetProgress()
    {
        //float progress = i / count;
        if (subP == null)
        {
            return progress;
        }
        else
        {
            return (i + subP.GetProgress()) / count;
        }
        //return progress;
    }
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

    public static ProgressArgEx New(string title, int i, int count, object t = null, IProgressArg pp = null)
    {
        ProgressArg subProgress = new ProgressArg(title, i, count, t);

        if (pp == null)
        {
            ProgressArgEx list = new ProgressArgEx(subProgress);
            return list;
        }
        else
        {
            IProgressArg cloneP = pp.Clone();
            if (cloneP is ProgressArgEx)
            {
                ProgressArgEx list = cloneP as ProgressArgEx;
                //list.Last().AddSubProgress(subProgress);
                //list.Add(subProgress);
                list.AddSubProgress(subProgress);
                return list;
            }
            else
            {
                //ProgressArg p0 = (pp as ProgressArg).Clone();
                ProgressArg p0 = (cloneP as ProgressArg);
                ProgressArgEx list = new ProgressArgEx();
                p0.AddSubProgress(subProgress);
                //return p0;
                list.Add(p0);
                list.Add(subProgress);
                return list;
            }
        }
        
    }

    public ProgressArg()
    {

    }

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

    public bool IsFinished()
    {
        return this.progress >= 1;
    }

    private string GetProgressText()
    {
        return $"{i}/{count} {GetProgress():P2}";
    }

    public override string ToString()
    {
        //if (subP == null)
        //{
        //    return $"P1:{GetProgressText()} ({tag})";
        //}
        //else
        //{
        //    var subP2 = subP.subP;
        //    if (subP2 == null)
        //    {
        //        return $"P2:[{GetProgressText()}]>[{subP.GetProgressText()}] ({tag}>{subP.tag})";
        //    }
        //    else
        //    {
        //        //return $"P3[{GetProgress()}]>[{subP.GetProgress()}]>[{subP.subP.GetProgress()}] ({tag}>{subP.tag}>{subP.subP.tag})";
        //        var subP3 = subP2.subP;
        //        if (subP3 == null)
        //        {
        //            return $"P3:[{GetProgressText()}]>[{subP.GetProgressText()}]>[{subP2.GetProgressText()}] ({tag}>{subP.tag}>{subP2.tag})";
        //        }
        //        else
        //        {
        //            return $"P4:[{GetProgressText()}]>[{subP.GetProgressText()}]>[{subP2.GetProgressText()}]>[{subP3.GetProgressText()}] ({tag}>{subP.tag}>{subP2.tag}>{subP3.tag})";
        //        }
        //    }
        //}

        int count = 1;
        ProgressArg sub = this.subP;
        string totalProgress = $"[{this.GetProgressText()}]";
        string totalTag = this.tag + "";
        while (sub != null)
        {
            count++;
            totalProgress += $">[{sub.GetProgressText()}]";
            totalTag+= ">"+sub.tag;
            sub = sub.subP;
            if (count > 5)
            {
                break;
            }
        }
        return $"[T{count}]{totalProgress}({totalTag})";

    }

    public string GetTitle()
    {
        int count = 1;
        ProgressArg sub = this.subP;
        string totalTitle= $"[{title}]";
        while (sub != null)
        {
            count++;
            totalTitle += $">[{sub.title}]";
            sub = sub.subP;
            if (count > 5)
            {
                break;
            }
        }
        return $"[T{count}]{totalTitle}";

        //if (subP == null)
        //{
        //    return $"[T1][{title}]";
        //}
        //else
        //{
        //    if (subP.subP == null)
        //    {
        //        return $"[T2][{title}]>[{subP.title}]";
        //    }
        //    else
        //    {
                
        //        if (subP.subP.subP == null)
        //        {
        //            return $"[T3][{title}]>[{subP.title}]>[{subP.subP.title}]";
        //        }
        //        else
        //        {
                    
        //            return $"[T4][{title}]>[{subP.title}]>[{subP.subP.title}]>[{subP.subP.subP.title}]";
        //        }
        //    }
        //}
    }

    public ProgressArg subP;

    public ProgressArg parent;

    internal void AddSubProgress(ProgressArg p)
    {
        if (p == this)
        {
            Debug.LogError("AddSubProgress subProgress==this:"+this);
            return;
        }
        p.parent = this;
        subP = p;

        //this.progress = (i + p.progress) / count;
    }
}

public enum ScenesState
{

}

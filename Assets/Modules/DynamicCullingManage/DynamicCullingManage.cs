using AdvancedCullingSystem.DynamicCullingCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCullingManage : SingletonBehaviour<DynamicCullingManage>
{
    //public static DynamicCullingManage Instance;
    public DynamicCulling dynamicCulling;

    public bool EnableOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        dynamicCulling = DynamicCulling.Instance;

        //Instance = this;
        Debug.LogError("Application.backgroundLoadingPriority:" + Application.backgroundLoadingPriority);
        if (IgnoreObjs == null) IgnoreObjs = new List<GameObject>();
        //SystemSettingHelper.GetSystemSetting(() =>
        //{
        //    if (SystemSettingHelper.systemSetting.IsDynamicCulling)
        //    {
        //        SetCullingRenders();
        //    }
        //});
        if (EnableOnStart)
        {
            SetCullingRenders();
        }
        
    }

    public bool IsInEditor;

    /// <summary>
    /// 是否开启遮挡剔除
    /// </summary>
    public bool IsDynamicCulling = true;
    /// <summary>
    /// 遮挡剔除精度（可以理解为对象计算个数）
    /// </summary>
    public int DynamicCullingJobsFrame = 500;
    /// <summary>
    /// 遮挡剔除计算时间间隔
    /// </summary>
    public float DynamicCullingTimeInterval = 0.3f;

    public float DynamicCullingObjectsLifeTime = 20f;

    public bool includeInactive = false;

    public int count = 0;

    public bool isIncludeStatic = false;

    public List<GameObject> IgnoreObjs;

    [ContextMenu("SetDynamicArg")]
    public void SetDynamicArg()
    {
        dynamicCulling.gameObject.SetActive(IsDynamicCulling);
        dynamicCulling.SetJobsPerFrame(DynamicCullingJobsFrame);
        dynamicCulling.DynamicCullingTimeInterval = DynamicCullingTimeInterval;
        dynamicCulling.SetObjectsLifetime(DynamicCullingObjectsLifeTime);
    }

    private GameObject Target ;

    public MeshRenderer[] GetTargetRenderers()
    {
        DateTime start = DateTime.Now;
        //if (Target==null)
        //{
        //    Target = FactoryDepManager.Instance.Facotory;
        //}

        List<MeshRenderer> IgnoreRenderers = new List<MeshRenderer>();
        if (IgnoreObjs != null)
        {
            foreach (GameObject O in IgnoreObjs)
            {
                IgnoreRenderers.AddRange(O.GetComponentsInChildren<MeshRenderer>(includeInactive));
            }
        }

        List<MeshRenderer>  needRenderers = new List<MeshRenderer>();
        MeshRenderer[] renderersT = GameObject.FindObjectsOfType<MeshRenderer>(includeInactive);
        for (int i = 0; i < renderersT.Length; i++)
        {
            MeshRenderer rt = renderersT[i];
            if (IgnoreRenderers.Contains(rt)) continue;
            needRenderers.Add(rt);
        }
        MeshRendererInfoList list = new MeshRendererInfoList(needRenderers);
        MeshRendererInfoList list2 = list.GetLODs(0, -1);
        var rs=list2.GetAllRenderers().ToArray();
        count = needRenderers.Count;

        Debug.LogError($"DynamicCullingManage.GetTargetRenderers allRenderers:{needRenderers.Count} renderers:{rs.Length} time:{DateTime.Now- start}");
        Debug.LogError($"DynamicCullingManage.GetTargetRenderers renderersT:{renderersT.Length}");
        return rs;
    }

    //private void StartDynamicCullingRun()
    //{
    //    var renderersT = GetTargetRenderers();
    //    //dynamicCulling.OnEditorRemoveAllOccluders
    //    //dynamicCulling.AddStartRenderers(renderersT);
    //    dynamicCulling.AddObjectsForCulling(renderersT);
    //    dynamicCulling.SetJobsPerFrame(SystemSettingHelper.systemSetting.DynamicCullingJobsFrame);
    //    dynamicCulling.SetObjectsLifetime(SystemSettingHelper.systemSetting.DynamicCullingObjectsLifeTime);
    //    dynamicCulling.DynamicCullingTimeInterval = SystemSettingHelper.systemSetting.DynamicCullingTimeInterval;
    //    dynamicCulling.Enable();
    //    dynamicCulling.gameObject.SetActive(true);//添加完AddStartRenderers(renderersT)，重新打开一下dynamicCulling才有效（必须在项目运行前dynamicCulling处于隐藏状态，才有效）
    //}

    private void StartDynamicCullingEditor()
    {
        var renderersT = GetTargetRenderers();
        //dynamicCulling.OnEditorRemoveAllOccluders
        //dynamicCulling.AddStartRenderers(renderersT);
        dynamicCulling.AddObjectsForCulling(renderersT);
        dynamicCulling.SetJobsPerFrame(DynamicCullingJobsFrame);
        dynamicCulling.SetObjectsLifetime(DynamicCullingObjectsLifeTime);
        dynamicCulling.DynamicCullingTimeInterval = DynamicCullingTimeInterval;
        dynamicCulling.Enable();
        dynamicCulling.gameObject.SetActive(true);//添加完AddStartRenderers(renderersT)，重新打开一下dynamicCulling才有效（必须在项目运行前dynamicCulling处于隐藏状态，才有效）
        Debug.LogError("renderersTPPP:" + renderersT.Length);
    }

    /// <summary>
    /// 设置遮挡剔除对象
    /// </summary>
    [ContextMenu("SetCullingRenders")]
    public void SetCullingRenders()
    {
#if UNITY_EDITOR
        IsInEditor = true;
#endif
        //if (IsInEditor)
        {
            if (!IsDynamicCulling)
            {
                if (dynamicCulling)
                {
                    GameObject.DestroyImmediate(dynamicCulling);
                }
                return;
            }
            dynamicCulling.isIncludeStatic = this.isIncludeStatic;
            StartDynamicCullingEditor();
        }
        //else
        //{
        //    if (!SystemSettingHelper.systemSetting.IsDynamicCulling)
        //    {
        //        if (dynamicCulling)
        //        {
        //            GameObject.DestroyImmediate(dynamicCulling);
        //        }
        //        return;
        //    }
        //    dynamicCulling.isIncludeStatic = this.isIncludeStatic;
        //    StartDynamicCullingRun();
        //}
    }

    //public GameObject ooo;
    //[ContextMenu("SetCullingRenders2")]
    //public void SetCullingRenders2()
    //{
    //    //dynamicCulling.OnEditorRemoveAllOccluders
    //    //MeshRenderer[] renderersT = FactoryDepManager.Instance.Facotory.GetComponentsInChildren<MeshRenderer>();
    //    MeshRenderer[] renderersT = ooo.GetComponentsInChildren<MeshRenderer>();
    //    dynamicCulling.AddObjectsForCulling(renderersT);
    //}

    /// <summary>
    /// 添加遮挡剔除对象列表
    /// </summary>
    public void AddObjectsForCulling(MeshRenderer[] renderers)
    {
        Debug.LogError($"AddObjectsForCulling renderers:{renderers.Length}");
        dynamicCulling.AddObjectsForCulling(renderers);
    }

    public void AddObjectsForCullingCoroutine(MeshRenderer[] renderers)
    {
        Debug.LogError($"AddObjectsForCulling renderers:{renderers.Length}");
        StartCoroutine(dynamicCulling.AddObjectsForCullingCoroutine(renderers));
    }

    public void RemoveObjects(MeshRenderer[] renderers)
    {
        Debug.LogError($"RemoveObjects renderersT:{renderers.Length}");
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null)
                dynamicCulling.RemoveObject(renderers[i]);
    }

    /// <summary>
    /// 移除遮挡剔除列表中的一个对象
    /// </summary>
    public void RemoveObject(MeshRenderer rendererT)
    {
        dynamicCulling.RemoveObject(rendererT);
    }
}

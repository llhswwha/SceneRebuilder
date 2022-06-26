using AdvancedCullingSystem.DynamicCullingCore;
using GPUInstancer;
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
        Debug.Log("DynamicCullingManage.Start Application.backgroundLoadingPriority:" + Application.backgroundLoadingPriority);
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
        CheckModels();
    }

    private void CheckModels()
    {
        if(RoomFactory.Instance&&RoomFactory.Instance.FactoryType==FactoryTypeEnum.MinHang)
        {
            bool hasBuilding = true;
            if(IgnoreObjs==null)
            {
                hasBuilding = false;
            }
            else
            {
                var reslut = IgnoreObjs.Find(i=>i!=null&&i.transform.name.Contains("混合反应沉淀池"));
                hasBuilding = reslut == null ? false : true;
            }
            if(!hasBuilding)
            {
                //GIS及出线架构区域及厂家设备、污泥浓缩池、34_混合反应沉淀池、通风冷却塔，这几个需要添加至忽略列表。
                //混合反应沉淀池不忽略，会导致程序崩溃。  add by wk 220618
                Debug.LogError("Exception:建筑-混合反应沉淀池,需添加至IgnoreList.参与遮挡剔除，会导致崩溃");
                UGUIMessageBox.Show("警告：建筑-混合反应沉淀池，未添加至忽略列表！如参与遮挡剔除计算，会导致程序异常！");
            }
        }
    }


    public bool IsInEditor;

    public bool IsAutoUpdateCullingInfo = false;

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

    public GameObject Target ;

    public List<MeshRenderer> IgnoreRenderers = new List<MeshRenderer>();

    public void AddToIgnoreRenderers(GameObject root)
    {
        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        IgnoreRenderers.AddRange(renderers);

        Debug.Log($"AddToIgnoreRenderers root:{root} renderers:{renderers.Length} IgnoreRenderers:{IgnoreRenderers.Count}");
    }

    public DictList<MeshRenderer> GetIgnoreRenderers()
    {
        DictList<MeshRenderer> ignoreRenderers = new DictList<MeshRenderer>();
        if (IgnoreObjs != null)
        {
            foreach (GameObject O in IgnoreObjs)
            {
                if (O == null) continue;
                ignoreRenderers.AddRange(O.GetComponentsInChildren<MeshRenderer>(includeInactive));
            }
        }
        foreach(var renderer in IgnoreRenderers)
        {
            ignoreRenderers.Add(renderer);
        }
        return ignoreRenderers;
    }

    public MeshRenderer[] GetTargetRenderers()
    {
        DateTime start = DateTime.Now;

        DictList<MeshRenderer> ignoreRenderers = GetIgnoreRenderers();

        List<MeshRenderer> needRenderers = new List<MeshRenderer>();

        if (Target == null && FactoryDepManager.Instance != null)
        {
            Target = FactoryDepManager.Instance.Facotory;
        }
        MeshRenderer[] renderersT = null;
        if (Target==null)
        {
            renderersT = GameObject.FindObjectsOfType<MeshRenderer>(includeInactive);
            //for (int i = 0; i < renderersT.Length; i++)
            //{
            //    MeshRenderer rt = renderersT[i];
            //    if (IgnoreRenderers.Contains(rt)) continue;
            //    AreaTreeNode node = rt.GetComponent<AreaTreeNode>();
            //    if (node != null) continue;
            //    GPUInstancerPrefab gpui= rt.GetComponent<GPUInstancerPrefab>();
            //    if (gpui != null) continue;
            //    needRenderers.Add(rt);
            //}
        }
        else
        {
            renderersT = Target.GetComponentsInChildren<MeshRenderer>(includeInactive);
            //for (int i = 0; i < renderersT.Length; i++)
            //{
            //    MeshRenderer rt = renderersT[i];
            //    if (IgnoreRenderers.Contains(rt)) continue;
            //    AreaTreeNode node = rt.GetComponent<AreaTreeNode>();
            //    if (node != null) continue;
            //    GPUInstancerPrefab gpui = rt.GetComponent<GPUInstancerPrefab>();
            //    if (gpui != null) continue;
            //    needRenderers.Add(rt);
            //}
        }

        for (int i = 0; i < renderersT.Length; i++)
        {
            MeshRenderer rt = renderersT[i];
            if (ignoreRenderers.Contains(rt)) continue;
            AreaTreeNode node = rt.GetComponent<AreaTreeNode>();
            if (node != null) continue;
            GPUInstancerPrefab gpui = rt.GetComponent<GPUInstancerPrefab>();
            if (gpui != null) continue;
            needRenderers.Add(rt);
        }


        MeshRendererInfoList list = new MeshRendererInfoList(needRenderers);
        MeshRendererInfoList list2 = list.GetLODs(0, -1);
        var rs=list2.GetAllRenderers().ToArray();
        count = needRenderers.Count;

        Debug.Log($"DynamicCullingManage.GetTargetRenderers allRenderers:{needRenderers.Count} renderers:{rs.Length} time:{DateTime.Now- start}");
        //Debug.LogError($"DynamicCullingManage.GetTargetRenderers renderersT:{renderersT.Length}");
        return rs;
    }

    private void StartDynamicCullingRun()
    {
        DateTime start = DateTime.Now;
        var renderersT = GetTargetRenderers();
        //dynamicCulling.OnEditorRemoveAllOccluders
        //dynamicCulling.AddStartRenderers(renderersT);
        dynamicCulling.AddObjectsForCulling(renderersT);
        dynamicCulling.SetJobsPerFrame(SystemSettingHelper.systemSetting.DynamicCullingJobsFrame);
        dynamicCulling.SetObjectsLifetime(SystemSettingHelper.systemSetting.DynamicCullingObjectsLifeTime);
        dynamicCulling.DynamicCullingTimeInterval = SystemSettingHelper.systemSetting.DynamicCullingTimeInterval;
        dynamicCulling.Enable();
        dynamicCulling.gameObject.SetActive(true);//添加完AddStartRenderers(renderersT)，重新打开一下dynamicCulling才有效（必须在项目运行前dynamicCulling处于隐藏状态，才有效）
        Debug.Log($"StartDynamicCullingRun renderersT:{renderersT.Length} time:{DateTime.Now - start}  JobsFrame:{SystemSettingHelper.systemSetting.DynamicCullingJobsFrame} ObjectsLifeTime:{SystemSettingHelper.systemSetting.DynamicCullingObjectsLifeTime} TimeInterval:{SystemSettingHelper.systemSetting.DynamicCullingTimeInterval}");
    }

    [ContextMenu("SetCullingStartRenderers")]
    public void SetCullingStartRenderers()
    {
        var renderersT = GetTargetRenderers();
        if(dynamicCulling==null)
            dynamicCulling=DynamicCulling.Instance;
        dynamicCulling.Clear();
        int count=dynamicCulling.AddStartRenderers(renderersT);
        Debug.LogError($"SetCullingStartRenderers renderersT:renderersT.Length count:{count}");
    }

    private void StartDynamicCullingEditor()
    {
        DateTime start = DateTime.Now;
        var renderersT = GetTargetRenderers();
        //dynamicCulling.OnEditorRemoveAllOccluders
        //dynamicCulling.AddStartRenderers(renderersT);
        dynamicCulling.AddObjectsForCulling(renderersT);
        dynamicCulling.SetJobsPerFrame(DynamicCullingJobsFrame);
        dynamicCulling.SetObjectsLifetime(DynamicCullingObjectsLifeTime);
        dynamicCulling.DynamicCullingTimeInterval = DynamicCullingTimeInterval;
        dynamicCulling.Enable();
        dynamicCulling.gameObject.SetActive(true);//添加完AddStartRenderers(renderersT)，重新打开一下dynamicCulling才有效（必须在项目运行前dynamicCulling处于隐藏状态，才有效）
        Debug.Log($"StartDynamicCullingEditor renderersT:{renderersT.Length} time:{DateTime.Now-start} JobsFrame:{DynamicCullingJobsFrame} LifeTime:{DynamicCullingObjectsLifeTime} TimeInterval:{DynamicCullingTimeInterval}");
    }

    private int _newJobsCount = 500;

    public int JobsPerFrame
    {
        get
        {
            return dynamicCulling.GetJobsPerFrame();
        }
        set
        {
            if (_newJobsCount != value)
            {
                _newJobsCount = value;
                dynamicCulling.SetJobsPerFrame(value);
            }
        }
    }

    private float _objectsLifetime = 500;

    public float ObjectsLifetime
    {
        get
        {
            return dynamicCulling.GetObjectsLifetime();
        }
        set
        {
            if (_objectsLifetime != value)
            {
                _objectsLifetime = value;
                dynamicCulling.SetObjectsLifetime(value);
            }
        }
    }

    public float CullingTimeInterval
    {
        get
        {
            return dynamicCulling.DynamicCullingTimeInterval;
        }
        set
        {
            dynamicCulling.DynamicCullingTimeInterval = value;
        }
    }

    public void StopCulling()
    {
        dynamicCulling.Disable();
    }

    public void PauseCulling()
    {
        dynamicCulling.IsEnableCulling = false;
    }

    public void ContinueCulling()
    {
        dynamicCulling.IsEnableCulling = true;
    }

    public void ShowAllRenderers()
    {
        dynamicCulling.ShowAll();
    }

    public void HideAllRenderers()
    {
        dynamicCulling.HideAll();
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
        if (IsInEditor)
        {
            if (!IsDynamicCulling)
            {
                if (dynamicCulling)
                {
                    //GameObject.DestroyImmediate(dynamicCulling);
                    dynamicCulling.gameObject.SetActive(false);
                }
                Debug.LogError("StartDynamicCullingEditor NoStart! return");
                return;
            }
            dynamicCulling.isIncludeStatic = this.isIncludeStatic;
            StartDynamicCullingEditor();
        }
        else
        {
            if (!SystemSettingHelper.systemSetting.IsDynamicCulling)
            {
                if (dynamicCulling)
                {
                    //GameObject.DestroyImmediate(dynamicCulling);
                    dynamicCulling.gameObject.SetActive(false);
                }
                Debug.LogError("StartDynamicCullingRun NoStart! return");
                return;
            }
            dynamicCulling.isIncludeStatic = this.isIncludeStatic;
            StartDynamicCullingRun();
        }
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

    public string CullingInfo = "";

    public void UpdateCullingInfo()
    {
        CullingInfo = $"Renderers:{dynamicCulling.GetRendererCount()} Visible:{dynamicCulling.GetVisibleCount()} Hitted:{dynamicCulling.GetHittedCount()} AddList:{addIdList.Count} RemoveList:{removeIdList.Count}";
    }

    [ContextMenu("AddObjectsForCulling")]
    public void AddObjectsForCulling()
    {
        var renderersT = GetTargetRenderers();
        AddObjectsForCulling(renderersT);
        Debug.LogError($"AddObjectsForCulling renderersT:{renderersT.Length}");
    }

    /// <summary>
    /// 添加遮挡剔除对象列表
    /// </summary>
    public void AddObjectsForCulling(MeshRenderer[] renderers)
    {
        //Debug.LogError($"AddObjectsForCulling renderers:{renderers.Length}");
        //dynamicCulling.AddObjectsForCulling(renderers);

        //WaitingForAdd.AddRange(renderers);

        //GetTargetRenderers();

        //DictList<MeshRenderer> ignoreRenderers = GetIgnoreRenderers();

        if (renderers == null) return;
        foreach (var item in renderers)
        {
            if (item == null||!item.gameObject.activeInHierarchy) continue;

            //if (ignoreRenderers.Contains(item)) continue;
            //AreaTreeNode node = item.GetComponent<AreaTreeNode>();
            //if (node != null) continue;
            GPUInstancerPrefab gpui = item.GetComponent<GPUInstancerPrefab>();
            if (gpui != null) continue;
            //needRenderers.Add(rt);

            int instanceId = item.GetInstanceID();
            if (!waitingForAddDic.ContainsKey(instanceId))
            {
                waitingForAddDic.Add(instanceId, item);
                addIdList.Add(instanceId);
            }
        }
    }

    [ContextMenu("RemoveEmtpyRender")]
    public void RemoveEmtpyRender()
    {
        int nullNum = 0;
        int inactiveNum = 0;
        foreach(var item in waitingForAddDic.Values)
        {
            if(item==null)
            {
                nullNum++;
                continue;
            }
            if(!item.gameObject.activeInHierarchy)
            {
                inactiveNum++;
            }
        }
        Debug.LogError("NullNum:"+nullNum+" inActiveNum:"+inactiveNum);
    }

    public void AddObjectsForCullingCoroutine(MeshRenderer[] renderers)
    {
        Debug.LogError($"AddObjectsForCulling renderers:{renderers.Length}");
        StartCoroutine(dynamicCulling.AddObjectsForCullingCoroutine(renderers));
    }
    Dictionary<int, MeshRenderer> waitingForAddDic = new Dictionary<int, MeshRenderer>();
    public List<int> addIdList = new List<int>();

    //public List<MeshRenderer> WaitingForAdd = new List<MeshRenderer>();
    //public List<MeshRenderer> WaitingForRemove = new List<MeshRenderer>();

    Dictionary<int, MeshRenderer> waitingForRemoveDic = new Dictionary<int, MeshRenderer>();
    public List<int> removeIdList = new List<int>();

    public int AddModelSize = 100;

    private void Update()
    {
        if (addIdList.Count > 0)
        {
            for(int i = 0; i < AddModelSize; i++)
            {
                int addR = addIdList[0];
                addIdList.RemoveAt(0);
                if (waitingForAddDic.ContainsKey(addR))
                {
                    dynamicCulling.AddObjectForCulling(waitingForAddDic[addR]);
                    waitingForAddDic.Remove(addR);
                }
                if (addIdList.Count == 0)
                {
                    break;
                }
            }
            

            UpdateCullingInfo();
            //Debug.Log("");
        }

        if (removeIdList.Count > 0)
        {
            for (int i = 0; i < AddModelSize; i++)
            {
                int addR = removeIdList[0];
                removeIdList.RemoveAt(0);
                if (waitingForRemoveDic.ContainsKey(addR))
                {
                    dynamicCulling.RemoveObject(waitingForRemoveDic[addR]);
                    waitingForRemoveDic.Remove(addR);
                }
                if (removeIdList.Count == 0)
                {
                    break;
                }
            }
            UpdateCullingInfo();
        }
    }

    public void RemoveObjects(MeshRenderer[] renderers)
    {
        //Debug.LogError($"RemoveObjects renderersT:{renderers.Length}");
        //if (renderers == null)
        //    return;

        //for (int i = 0; i < renderers.Length; i++)
        //    if (renderers[i] != null)
        //        dynamicCulling.RemoveObject(renderers[i]);

        //WaitingForRemove.AddRange(renderers);
        if (renderers == null) return;
        for (int i=0;i<renderers.Length;i++)
        {
            MeshRenderer renderT = renderers[i];
            if (renderT == null) continue;
            int instanceId = renderT.GetInstanceID();
            if (!waitingForRemoveDic.ContainsKey(instanceId))
            {
                waitingForRemoveDic.Add(instanceId, renderT);
                removeIdList.Add(instanceId);
            }
            //try
            //{
            //    //Todo:是否把缓存中等待添加的，也先移除？
            //    if (waitingForAddDic.ContainsKey(instanceId))
            //    {
            //        removeIdList.Remove(instanceId);
            //        waitingForAddDic.Remove(instanceId);
            //    }
            //}catch(Exception e)
            //{

            //}            
        }
    }

    public void RemoveObjectsCoroutine(MeshRenderer[] renderers)
    {
        StartCoroutine(RemoveObjectsCoroutineInner(renderers));
    }

    public IEnumerator RemoveObjectsCoroutineInner(MeshRenderer[] renderers)
    {
        Debug.LogError($"RemoveObjectsCoroutineInner renderersT:{renderers.Length}");
        if (renderers != null)
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    dynamicCulling.RemoveObject(renderers[i]);
                yield return null;
            }
        yield return null;
    }

    /// <summary>
    /// 移除遮挡剔除列表中的一个对象
    /// </summary>
    public void RemoveObject(MeshRenderer rendererT)
    {
        dynamicCulling.RemoveObject(rendererT);
    }
}

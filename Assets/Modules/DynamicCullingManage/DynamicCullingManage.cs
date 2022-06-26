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
                var reslut = IgnoreObjs.Find(i=>i!=null&&i.transform.name.Contains("��Ϸ�Ӧ�����"));
                hasBuilding = reslut == null ? false : true;
            }
            if(!hasBuilding)
            {
                //GIS�����߼ܹ����򼰳����豸������Ũ���ء�34_��Ϸ�Ӧ����ء�ͨ����ȴ�����⼸����Ҫ����������б�
                //��Ϸ�Ӧ����ز����ԣ��ᵼ�³��������  add by wk 220618
                Debug.LogError("Exception:����-��Ϸ�Ӧ�����,�������IgnoreList.�����ڵ��޳����ᵼ�±���");
                UGUIMessageBox.Show("���棺����-��Ϸ�Ӧ����أ�δ����������б�������ڵ��޳����㣬�ᵼ�³����쳣��");
            }
        }
    }


    public bool IsInEditor;

    public bool IsAutoUpdateCullingInfo = false;

    /// <summary>
    /// �Ƿ����ڵ��޳�
    /// </summary>
    public bool IsDynamicCulling = true;
    /// <summary>
    /// �ڵ��޳����ȣ��������Ϊ������������
    /// </summary>
    public int DynamicCullingJobsFrame = 500;
    /// <summary>
    /// �ڵ��޳�����ʱ����
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
        dynamicCulling.gameObject.SetActive(true);//�����AddStartRenderers(renderersT)�����´�һ��dynamicCulling����Ч����������Ŀ����ǰdynamicCulling��������״̬������Ч��
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
        dynamicCulling.gameObject.SetActive(true);//�����AddStartRenderers(renderersT)�����´�һ��dynamicCulling����Ч����������Ŀ����ǰdynamicCulling��������״̬������Ч��
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
    /// �����ڵ��޳�����
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
    /// ����ڵ��޳������б�
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
            //    //Todo:�Ƿ�ѻ����еȴ���ӵģ�Ҳ���Ƴ���
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
    /// �Ƴ��ڵ��޳��б��е�һ������
    /// </summary>
    public void RemoveObject(MeshRenderer rendererT)
    {
        dynamicCulling.RemoveObject(rendererT);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallModelController : MonoBehaviour
{
    public Transform cameraT;

    public Rect screenRect;

    // Start is called before the first frame update
    public DistanceMode DisMode;

    public bool IsAddScript = true;
    public int MaxNodeCount = 5000;
    public float DisPower = 50;
    public float MinDis = 100f;
    public float CameraMaxAngle = 65;

    public int GroupSize = 250;
    public int CurrentId = 0;
    public int GroupCount = 0;

    public bool UpdateEnbale = true;
    public float UpdateInterval = 0.1f;
    public double UpdateTime = 0;

    public int AllCount = 0;
    public int VisibleCount = 0;
    public int HiddenCount = 0;


    public bool CheckAngle = true;
    public List<RendererInfo> VisibleRenderers = new List<RendererInfo>();
    public List<MeshRenderer[]> rendererGroups = new List<MeshRenderer[]>();
    public List<RendererInfo> rendererInfos = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels0 = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels1 = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels2 = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels3 = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels4 = new List<RendererInfo>();
    public List<RendererInfo> rendererLevels5 = new List<RendererInfo>();

    public List<float> LevelsDis = new List<float>() { 1, 2, 3, 4, 5, 10, 20, 30, 40,80 };
    public List<float> LevelsCount = new List<float>();
    public List<List<RendererInfo>> LevelsItems = new List<List<RendererInfo>>() { };

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        UpdateEnbale = false;
        SetLevelItemsVisible(rendererInfos, true);
    }
    [ContextMenu("HideAll")]
    public void HideAll()
    {
        UpdateEnbale = false;
        SetLevelItemsVisible(rendererInfos, false);
    }
    [ContextMenu("ShowLevel5")]
    public void ShowLevel5()
    {
        isUpdateLv5 = false;
        SetLevelItemsVisible(rendererLevels5, true);
    }
    [ContextMenu("ShowLevel4")]
    public void ShowLevel4()
    {
        isUpdateLv4 = false;
        SetLevelItemsVisible(rendererLevels4, true);
    }
    [ContextMenu("ShowLevel3")]
    public void ShowLevel3()
    {
        isUpdateLv3 = false;
        SetLevelItemsVisible(rendererLevels3,true);
    }
    public void SetLevelItemsVisible(List<RendererInfo> items,bool v)
    {
        foreach (var item in items)
        {
            item.SetVisible(v);
        }
    }
    public Text LogText;

    [ContextMenu("InitRenderers")]
    public void InitRenderers()
    {
        DateTime start = DateTime.Now;

        if (cameraT == null)
        {
            cameraT = Camera.main.transform;
        }
        this.screenRect = new Rect(0, 0, Screen.width, Screen.height);

        var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        AllCount = renderers.Length;

        int GroupCount = renderers.Length / GroupSize + 1;

        rendererInfos.Clear();
        foreach (var renderer in renderers)
        {
            RendererInfo info = new RendererInfo(renderer);
            if (IsAddScript)
            {
                ModelDisInfo modelInfo = renderer.gameObject.AddComponent<ModelDisInfo>();
                modelInfo.info = info;
            }
            rendererInfos.Add(info);

            if (info.diameter < 1)
            {
                rendererLevels0.Add(info);

            }
            else if (info.diameter < 5)
            {
                rendererLevels1.Add(info);
            }
            else if (info.diameter < 10)
            {
                rendererLevels2.Add(info);
            }
            else if (info.diameter < 20)
            {
                rendererLevels3.Add(info);
            }
            else if (info.diameter < 40)
            {
                rendererLevels4.Add(info);
            }
            else
            {
                rendererLevels5.Add(info);
            }

        }
        for (int i = 0; i < GroupCount; i++)
        {
            MeshRenderer[] group = new MeshRenderer[GroupSize];
            for (int j = 0; j < GroupSize; j++)
            {
                int k = j * GroupSize + j;
                if (k >= renderers.Length) break;
                group[j] = renderers[k];
            }
            rendererGroups.Add(group);
        }

        SetLevelItemsVisible(rendererInfos, false);
        DateTime end = DateTime.Now;
        TimeSpan t = end - start;
        Debug.Log(string.Format("InitRenderers 用时{0}ms", t.TotalMilliseconds));
    }

    void Start()
    {

        InitRenderers();
        StartCoroutine(UpdateRenderer());
    }

    public bool StopUpdate = false;

    public bool isUpdateLv5 = true;
    public bool isUpdateLv4 = true;
    public bool isUpdateLv3 = true;
    public bool isUpdateLv2 = true;
    public bool isUpdateLv1 = true;
    public bool isUpdateLv0 = true;

    private IEnumerator UpdateRenderersEx()
    {
        DateTime start = DateTime.Now;

        HiddenCount = 0;
        VisibleCount = 0;
        CurrentId = 0;
        GroupCount = 0;
        VisibleRenderers.Clear();
        //UpdateRenderers(rendererInfos);

        bool isSetEnabled = true;
        if (isUpdateLv5)
        {
            yield return UpdateRenderers(rendererLevels5, isSetEnabled);
        }
        if (isUpdateLv4)
        {
            yield return UpdateRenderers(rendererLevels4, isSetEnabled);
        }
        if (isUpdateLv3)
        {
            yield return UpdateRenderers(rendererLevels3, isSetEnabled);
        }
        if (isUpdateLv2)
        {
            yield return UpdateRenderers(rendererLevels2, isSetEnabled);
        }
        if (isUpdateLv1)
        {
            yield return UpdateRenderers(rendererLevels1, isSetEnabled);
        }
        if (isUpdateLv0)
        {
            yield return UpdateRenderers(rendererLevels0, isSetEnabled);
        }
        //SetLevelItemsVisible(rendererInfos, false);
        //if (MaxNodeCount > 0 && VisibleRenderers.Count > MaxNodeCount)
        //{
        //    VisibleRenderers.Sort();
        //    for (int i = 0; i < MaxNodeCount; i++)
        //    {
        //        VisibleRenderers[i].enabled = true;
        //    }
        //}
        //else
        //{
        //    SetLevelItemsVisible(VisibleRenderers, true);
        //}

        DateTime end = DateTime.Now;
        TimeSpan t = end - start;
        //VisibleCount = c1;
        //HiddenCount = c2;
        UpdateTime = t.TotalMilliseconds;
        //Debug.Log(string.Format("{0} {1} {2}ms {3} {4}", this.name, rendererInfos.Count, t.TotalMilliseconds, sumDis, sumDis / rendererInfos.Count));
        if (LogText != null)
        {
            LogText.text = string.Format("Count={0}\nVisible={1}\nHidden={2}\nTime={3}", AllCount, VisibleCount, HiddenCount, t.TotalMilliseconds);
        }
        yield return new WaitForSeconds(UpdateInterval);
    }

    IEnumerator UpdateRenderer()
    {
        while (StopUpdate == false)
        {
            

            //foreach (var group in rendererGroups)
            //{

            //}

            if (UpdateEnbale == false)
            {
                yield return new WaitForSeconds(2);
            }
            else
            {
                yield return UpdateRenderersEx();
            }
        }
    }

    IEnumerator UpdateRenderers(List<RendererInfo> rs,bool setEnabled)
    {
        //int c1 = 0;
        //int c2 = 0;
        float sumDis = 0;
        
        foreach (var item in rs)
        {
            CurrentId++;
            if(CurrentId> GroupSize)
            {
                GroupCount++;
                CurrentId = 0;
                yield return null;
            }
            float dis = Vector3.Distance(item.transform.position, cameraT.position);
            item.distance = dis;
            item.fullDistance = dis + item.diameter;
            //Debug.Log(string.Format("dis {0} {1}", item.name, dis));
            bool isEnable = false;
            if(DisMode==DistanceMode.Absolute)
            {
                isEnable = item.fullDistance < MinDis;
            }
            if (DisMode == DistanceMode.Relative)
            {
                item.fullDiameter = (item.diameter * DisPower);
                isEnable = dis < item.fullDiameter;
            }
            //if (item.fullDistance > MinDis)
            if (isEnable==false)
            {
                if (setEnabled) item.SetVisible(false);
                HiddenCount++;
            }
            else
            {
                if (CheckAngle)
                {
                    var angle=Vector3.Angle(cameraT.forward, (item.transform.position - cameraT.position));
                    //if (screenRect.Contains(camera.WorldToScreenPoint(item.transform.position)))
                    if(angle< CameraMaxAngle)
                    {
                        //Debug.Log("第一个cube在视野内");
                        if (setEnabled) item.SetVisible(true);
                        VisibleCount++;
                        VisibleRenderers.Add(item);
                    }
                    else
                    {
                        if (setEnabled) item.SetVisible(false);
                        //print(string.Format("{0},{1}", item.transform.name, angle));
                    }
                    item.angle = angle;
                }
                else
                {
                    if (setEnabled) item.SetVisible(true);
                    VisibleCount++;
                    VisibleRenderers.Add(item);
                }
            }
            sumDis += dis;
        }

        yield return null;
    }

    //float sumDis = 0;

    // Update is called once per frame
    void Update()
    {
        //UpdateRenderers(renderers);
    }
}

public enum DistanceMode
{
    Absolute, Relative
}

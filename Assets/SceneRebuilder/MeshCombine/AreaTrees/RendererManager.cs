using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RendererManager : MonoBehaviour
{
    public GameObject[] GetAllGos;

    public MeshRenderer[] allRenderers;

    public List<RendererId> allRIds = new List<RendererId>();

    public List<string> allIds = new List<string>();

    public int Count = 0;

    public string TestId = "";

    public GameObject TestGo;

    public Renderer TestRenderer;

    [ContextMenu("TestGetRenderer")]
    public void TestGetRenderer()
    {
        TestRenderer = IdDictionay.GetRenderer(TestId);
        Debug.Log($"TestGetRenderer id:{TestId},renderer:{TestRenderer}");
    }

    [ContextMenu("TestGetGo")]
    public void TestGetGo()
    {
        TestGo = IdDictionay.GetGo(TestId);
        Debug.Log($"TestGetRenderer id:{TestId},go:{TestGo}");
    }

    [ContextMenu("TestBounds")]
    public void TestBounds()
    {
        var renderers=TestGo.GetComponentsInChildren<MeshRenderer>(true);
        var bounds = ColliderHelper.CaculateBounds(renderers);
        AreaTreeHelper.CreateBoundsCube(bounds, "TestBounds", transform,0);
    }

    [ContextMenu("InitIds")]
    public void InitIds()
    {
         DateTime start = DateTime.Now;
        IdDictionay.InitInfos();
        allIds = IdDictionay.GetIds();
        allRIds = IdDictionay.GetRIds();
        allRenderers = IdDictionay.GetRenderers().ToArray();
        Count = allRIds.Count;
        Debug.LogError($"InitIds count:{Count} time:{(DateTime.Now - start)}");
    }

    public void Start()
    {
        InitIds();
    }

    [ContextMenu("InitRenderers_All")]
    public void InitRenderers_All()
    {
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayProgressBar("InitIds", "Start", 0);
        allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        allRIds.Clear();
        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            RendererId id = r.GetComponent<RendererId>();
            if (id == null)
            {
                id = r.gameObject.AddComponent<RendererId>();
                id.Init(r);
            }
            allRIds.Add(id);

            MeshRendererInfo info = r.GetComponent<MeshRendererInfo>();
            if (info == null)
            {
                info = r.gameObject.AddComponent<MeshRendererInfo>();
                info.Init();
            }


            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"InitRenderers_All count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("InitRenderers_Target")]
    public void InitRenderers_Target()
    {
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayProgressBar("InitIds", "Start", 0);
        allRenderers = TestGo.GetComponentsInChildren<MeshRenderer>(true);
        allRIds.Clear();
        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            RendererId id = r.GetComponent<RendererId>();
            if (id == null)
            {
                id = r.gameObject.AddComponent<RendererId>();
                id.Init(r);
            }
            allRIds.Add(id);

            MeshRendererInfo info = r.GetComponent<MeshRendererInfo>();
            if (info == null)
            {
                info = r.gameObject.AddComponent<MeshRendererInfo>();
                info.Init();
            }


            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        allRIds = TestGo.GetComponentsInChildren<RendererId>(true).ToList();

        Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"InitRenderers_Target count:{allRenderers.Length} allRIds:{allRIds.Count} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds_All")]
    public void ClearIds_All()
    {
        DateTime start = DateTime.Now;
        allRIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allRIds)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.LogError($"ClearIds_All count:{allRIds.Count} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds_Target")]
    public void ClearIds_Target()
    {
        DateTime start = DateTime.Now;
        allRIds = TestGo.GetComponentsInChildren<RendererId>(true).ToList();
        foreach(var id in allRIds)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.LogError($"ClearIds_Target count:{allRIds.Count} time:{(DateTime.Now - start)}");
    }

    public float centerPivotDis=0.0001f;

    [ContextMenu("CenterPivotAll")]
    public void CenterPivotAll()
    {
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayProgressBar("ClearIds", "Start", 0);
        allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        int count = allRenderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            MeshRendererInfo id = r.GetComponent<MeshRendererInfo>();
            if (id == null)
            {
                id = r.gameObject.AddComponent<MeshRendererInfo>();
            }
            id.Init();
            id.CenterPivot(centerPivotDis);

            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("ClearIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"ClearIds count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("DisableShadow_All")]
    public void DisableShadow_All()
    {
        allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetShadowCastingMode(allRenderers,UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    [ContextMenu("EnableShadow_All")]
    public void EnableShadow_All()
    {
        allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetShadowCastingMode(allRenderers,UnityEngine.Rendering.ShadowCastingMode.On);
    }

    public static void SetShadowCastingMode(MeshRenderer[] renderers,UnityEngine.Rendering.ShadowCastingMode shadowCastingMode)
    {
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayProgressBar("SetShadowCastingMode", "Start", 0);
        int count = renderers.Length;
        int count2=0;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = renderers[i];
            BoundsBox box=r.GetComponent<BoundsBox>();
            if(box!=null)continue;
            r.shadowCastingMode=shadowCastingMode;
            count2++;
            float progress = (float)i / count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("SetShadowCastingMode", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"SetShadowCastingMode count:{renderers.Length} count2:{count2} time:{(DateTime.Now - start)}");
    }

    //public  
}

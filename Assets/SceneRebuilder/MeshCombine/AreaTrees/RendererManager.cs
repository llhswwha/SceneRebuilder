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
        AreaTreeHelper.CreateBoundsCube(bounds, "TestBounds", transform);
    }

    [ContextMenu("InitIds")]
    public void InitIds()
    {
        IdDictionay.InitInfos();
        allIds = IdDictionay.GetIds();
        allRIds = IdDictionay.GetRIds();
        allRenderers = IdDictionay.GetRenderers().ToArray();
        Count = allRIds.Count;
    }

    [ContextMenu("InitRenderers")]
    public void InitRenderers()
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

            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"InitRenderers count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds")]
    public void ClearIds()
    {
        DateTime start = DateTime.Now;
        allRIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allRIds)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.LogError($"ClearIds count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }

    //public  
}

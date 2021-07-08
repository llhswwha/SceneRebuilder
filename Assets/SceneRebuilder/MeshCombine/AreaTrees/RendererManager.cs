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
        TestRenderer = IdDictionary.GetRenderer(TestId);
        Debug.Log($"TestGetRenderer id:{TestId},renderer:{TestRenderer}");
    }

    [ContextMenu("TestGetGo")]
    public void TestGetGo()
    {
        TestGo = IdDictionary.GetGo(TestId);
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
        IdDictionary.InitInfos();
        allIds = IdDictionary.GetIds();
        allRIds = IdDictionary.GetRIds();
        allRenderers = IdDictionary.GetRenderers().ToArray();
        Count = allRIds.Count;
        Debug.Log($"InitIds count:{Count} time:{(DateTime.Now - start)}");
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
            }
            info.Init();


            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }
        Count = allRenderers.Length;
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"InitRenderers_All count:{allRenderers.Length} time:{(DateTime.Now - start)}");
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
        Debug.Log($"InitRenderers_Target count:{allRenderers.Length} allRIds:{allRIds.Count} time:{(DateTime.Now - start)}");
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
        Debug.Log($"ClearIds_All count:{allRIds.Count} time:{(DateTime.Now - start)}");
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
        Debug.Log($"ClearIds_Target count:{allRIds.Count} time:{(DateTime.Now - start)}");
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
        Debug.Log($"ClearIds count:{allRenderers.Length} time:{(DateTime.Now - start)}");
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
        Debug.Log($"SetShadowCastingMode count:{renderers.Length} count2:{count2} time:{(DateTime.Now - start)}");
    }

    public List<string> DetailNames=new List<string>();

    public bool IsDetail(GameObject go)
    {
        // List<string> detailNames=new List<string>(){
        //     //"90 Degree Direction Change"
        //     };
        foreach(var dn in DetailNames){
            var parent=go.transform.parent;
            if(go.name.Contains(dn) || 
                (parent!=null && parent.childCount==1 && parent.GetComponent<MeshRenderer>()==null && parent.name.Contains(dn))
            )
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu("SetDetailRenderers")]
    public void SetDetailRenderers()
    {
        DateTime start = DateTime.Now;
        int count=0;
        float vertexCount=0;
        var rendererInfos=GameObject.FindObjectsOfType<MeshRendererInfo>(true);
        foreach(var info in rendererInfos){
            if(IsDetail(info.gameObject))
            {
                info.rendererType=MeshRendererType.Detail;
                count++;
                vertexCount+=info.vertexCount;
            }
        }
        Debug.Log($"SetDetailRenderers count:{rendererInfos.Length} detailCount:{count} vertexCount:{vertexCount/10000:F1} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearAllType")]
    public void ClearAllType()
    {
        var rendererInfos=GameObject.FindObjectsOfType<MeshRendererInfo>(true);
        foreach(var info in rendererInfos){
            info.rendererType=MeshRendererType.None;
        }
    }

    //public  
}

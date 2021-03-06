using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// MeshRenderer&MeshFilterManager
/// </summary>
public class RendererManager : SingletonBehaviour<RendererManager>
{
    // private static RendererManager _instance;

    // public static RendererManager Instance
    // {
    //     get{
    //         if(_instance==null){
    //             _instance=GameObject.FindObjectOfType<RendererManager>();
    //         }
    //         return _instance;
    //     }
    // }
    public GameObject[] GetAllGos;

    // private MeshRenderer[] allRenderers;

    // private List<RendererId> allRIds = new List<RendererId>();

    private List<string> allIds = new List<string>();

    public int Count = 0;

    public string TestId = "";

    public GameObject TestGo;

    public Renderer TestRenderer;

    [ContextMenu("TestGetRenderer")]
    public void TestGetRenderer()
    {
        TestRenderer = IdDictionary.GetRenderer(TestId,true);
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

    public bool includeInactive = true;

    [ContextMenu("InitIds")]
    public void InitIds()
    {
         DateTime start = DateTime.Now;
        IdDictionary.InitInfos(null, includeInactive,false);
        allIds = IdDictionary.GetIds();
        var allRIds = IdDictionary.GetRIds();
        var allRenderers = IdDictionary.GetRenderers().ToArray();
        Count = allRIds.Count;
        Debug.Log($"InitIds count:{Count} time:{(DateTime.Now - start)}");
    }

    public void Start()
    {
        InitIds();
    }

    public MeshRenderer[] GetRenderers()
    {
        MeshRenderer[] allRenderers = null;
        if (TestGo)
        {
            allRenderers = TestGo.GetComponentsInChildren<MeshRenderer>(true);
        }
        else
        {
            allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        }
        return allRenderers;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        var allRenderers = GetRenderers();
        MeshHelper.ShowAllRenderers(allRenderers,5,"RendererManager");
    }

    private List<MeshRendererInfo> InitRenderers_Inner(MeshRenderer[] allRenderers)
    {
        DateTime start = DateTime.Now;
        int count = allRenderers.Length;
        List<RendererId> allRIds = new List<RendererId>();
        List<MeshRendererInfo> allInfos = new List<MeshRendererInfo>();
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            RendererId id = RendererId.InitId(r);

            allRIds.Add(id);

            MeshRendererInfo info = r.GetComponent<MeshRendererInfo>();
            if (info == null)
            {
                info = r.gameObject.AddComponent<MeshRendererInfo>();
            }
            info.Init();
            allInfos.Add(info);

            float progress = (float)i / count;
            float percents = progress * 100;

            //if (ProgressBarHelper.DisplayCancelableProgressBar("InitRenderers", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            //{
            //    break;
            //}
        }
        Count = allRenderers.Length;
        //ProgressBarHelper.ClearProgressBar();
        //Debug.Log($"InitRenderers count:{allRenderers.Length} time:{(DateTime.Now - start)}");
        return allInfos;
    }

    [ContextMenu("InitRenderers(All)")]
    public void InitRenderers_All()
    {
        ProgressBarHelper.DisplayProgressBar("InitRenderers_All", "Start", 0);
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        //allRIds.Clear();

        InitRenderers_Inner(allRenderers);
    }

    [ContextMenu("InitRenderers(Target)")]
    public void InitRenderers_Target()
    {
        ProgressBarHelper.DisplayProgressBar("InitRenderers_Target", "Start", 0);
        var allRenderers = TestGo.GetComponentsInChildren<MeshRenderer>(true);
        //allRIds.Clear();

        InitRenderers_Inner(allRenderers);
    }

    [ContextMenu("CheckRendererParent")]
    public void CheckRendererParent()
    {
        DateTime start = DateTime.Now;
        
        IdDictionary.InitInfos();

        var allRIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        int changedCount=0;
        foreach(var id in allRIds)
        {
            if(id.IsParentChanged())
            {
                // Debug.Log($"ParentChanged \t{id.name}");
                changedCount++;
            }
        }
        Debug.Log($"CheckRendererParent count:{allRIds.Count} changedCount:{changedCount} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds(All)")]
    public void ClearIds_All()
    {
        DateTime start = DateTime.Now;
        var allRIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allRIds)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"ClearIds_All count:{allRIds.Count} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds(Target)")]
    public void ClearIds_Target()
    {
        DateTime start = DateTime.Now;
        var allRIds = TestGo.GetComponentsInChildren<RendererId>(true).ToList();
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
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
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
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetShadowCastingMode(allRenderers,UnityEngine.Rendering.ShadowCastingMode.Off);
    }

    [ContextMenu("EnableShadow_All")]
    public void EnableShadow_All()
    {
        var allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
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

    public bool IsDetail2(GameObject go)
    {
        Debug.Log($"IsDetail2 go:{go.name}");
        foreach (var dn in DetailNames)
        {
            Debug.Log($"dn:{dn} contains:{go.name.Contains(dn)}");
            if (go.name.Contains(dn))
            {
                return true;
            }
        }
        return false;
    }


    public void SetDetailRenderers(MeshRenderer[] renderers)
    {
        var rendererInfos=InitRenderers_Inner(renderers);
        SetIsDetail(rendererInfos, renderers);
    }

    [ContextMenu("SetDetailRenderers")]
    public void SetDetailRenderers()
    {
        var rendererInfos = GameObject.FindObjectsOfType<MeshRendererInfo>(true);
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.GetComponent<MeshFilter>()!=null && i.GetComponent<MeshFilter>().sharedMesh != null).ToArray();
        if (rendererInfos.Length != renderers.Length)
        {
            InitRenderers_All();
            rendererInfos = GameObject.FindObjectsOfType<MeshRendererInfo>(true);
        }
        SetIsDetail(rendererInfos, renderers);
    }

    private void SetIsDetail(IEnumerable<MeshRendererInfo> rendererInfos, MeshRenderer[] renderers)
    {
        DateTime start = DateTime.Now;
        int count = 0;
        float vertexCount = 0;
        foreach (var info in rendererInfos)
        {
            if (IsDetail(info.gameObject))
            {
                info.rendererType = MeshRendererType.Detail;
                count++;
                vertexCount += info.vertexCount;
            }
        }
        Debug.Log($"SetDetailRenderers renderers:{renderers.Length} infos:{rendererInfos.Count()} detailCount:{count} vertexCount:{vertexCount / 10000:F1} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearAllType")]
    public void ClearAllType()
    {
        var rendererInfos=GameObject.FindObjectsOfType<MeshRendererInfo>(true);
        foreach(var info in rendererInfos){
            info.rendererType=MeshRendererType.None;
        }
    }

    [ContextMenu("RemoveEmptyParent_Target")]
    public void RemoveEmptyParent_Target()
    {
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayProgressBar("RemoveEmptyParent_Target", "Start", 0);
        var allRenderers = TestGo.GetComponentsInChildren<MeshRenderer>(true);
        int count = allRenderers.Length;
        int removeCount = 0;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = allRenderers[i];
            Transform p = r.transform.parent;
            Transform pp = p.parent;
            if (p.transform.childCount == 1)
            {
                r.transform.SetParent(pp);
                
                removeCount++;
                r.name = p.name;

                GameObject.DestroyImmediate(p);
            }

            float progress = (float)i / count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("RemoveEmptyParent_Target", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            {
                break;
            }
        }

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"RemoveEmptyParent_Target TestGo:{TestGo} count:{allRenderers.Length} removeCount:{removeCount} time:{(DateTime.Now - start)}");
    }

    public SharedMeshInfoList GetSharedMeshList()
    {
        SharedMeshInfoList sharedMeshes = new SharedMeshInfoList();
        sharedMeshes.InitAll(true);
        return sharedMeshes;
    }

    public static List<MeshRenderer> FindRenderers(GameObject go, string key)
    {
        //List<Transform> findList = new List<Transform>();
        //Transform[] ts = null;
        //if (go == null)
        //{
        //    ts = GameObject.FindObjectsOfType<Transform>();
        //}
        //else
        //{
        //    ts = go.GetComponentsInChildren<Transform>(true);
        //}

        //foreach (Transform t in ts)
        //{
        //    if (t.name.ToLower().Contains(key))
        //    {
        //        findList.Add(t);
        //    }
        //}
        //List<MeshRenderer> renderers = new List<MeshRenderer>();
        //foreach (var t in findList)
        //{
        //    MeshRenderer[] rs = t.GetComponentsInChildren<MeshRenderer>(true);
        //    foreach (var r in rs)
        //    {
        //        if (r.name.Contains("Bounds")) continue;
        //        if (!renderers.Contains(r))
        //            renderers.Add(r);
        //    }
        //    //renderers.AddRange(rs);
        //}
        //return renderers;
        return FindComponents<MeshRenderer>(go, key);
    }

    public static List<T> FindComponents<T>(GameObject go, string key) where T : Component
    {
        List<Transform> findList = new List<Transform>();
        Transform[] ts = null;
        if (go == null)
        {
            ts = GameObject.FindObjectsOfType<Transform>();
        }
        else
        {
            ts = go.GetComponentsInChildren<Transform>(true);
        }

        foreach (Transform t in ts)
        {
            if (t.name.ToLower().Contains(key))
            {
                findList.Add(t);
            }
        }
        List<T> renderers = new List<T>();
        foreach (var t in findList)
        {
            T[] rs = t.GetComponentsInChildren<T>(true);
            foreach (var r in rs)
            {
                if (r.name.Contains("Bounds")) continue;
                if (!renderers.Contains(r))
                    renderers.Add(r);
            }
            //renderers.AddRange(rs);
        }
        return renderers;
    }

    //public static List<MeshRenderer> FindRenderers(MeshRenderer[] rs, string key)
    //{
    //    //List<MeshRenderer> renderers = new List<MeshRenderer>();
    //    //foreach (var r in rs)
    //    //{
    //    //    //if (r.name.Contains("Bounds")) continue;
    //    //    if (r.name.ToLower().Contains(key))
    //    //    {
    //    //        if (!renderers.Contains(r))
    //    //            renderers.Add(r);
    //    //    }
    //    //}
    //    //return renderers;

    //    return FindComponents<MeshRenderer>(rs,key);
    //}

    //public static List<MeshRenderer> FindRenderers(string key)
    //{
    //    //MeshRenderer[] rs = GameObject.FindObjectsOfType<MeshRenderer>(true);
    //    //return FindRenderers(rs, key);
    //    return FindComponents<MeshRenderer>(key);
    //}

    //public static List<T> FindComponents<T>(T[] rs, string key) where T :Component
    //{
    //    List<T> renderers = new List<T>();
    //    foreach (var r in rs)
    //    {
    //        //if (r.name.Contains("Bounds")) continue;
    //        if (r.name.ToLower().Contains(key))
    //        {
    //            //if (!renderers.Contains(r))
    //                renderers.Add(r);
    //        }
    //    }
    //    return renderers;
    //}

    //public static List<T> FindComponents<T>(string key) where T : Component
    //{
    //    T[] rs = GameObject.FindObjectsOfType<T>(true);
    //    Debug.Log($"FindComponents key:{key} rs:{rs.Length}");
    //    return FindComponents(rs, key);
    //}
}

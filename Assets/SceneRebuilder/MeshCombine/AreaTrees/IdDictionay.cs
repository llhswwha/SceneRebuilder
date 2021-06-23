using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IdDictionay
{
    public static Dictionary<string, MeshRenderer> RendererDict = new Dictionary<string, MeshRenderer>();

    public static Dictionary<string, RendererId> IdDict = new Dictionary<string, RendererId>();

    private static void SetId(RendererId id)
    {
        try
        {

            if (!IdDict.ContainsKey(id.Id))
            {
                IdDict.Add(id.Id, id);
            }
            else
            {
                IdDict[id.Id] = id;//旧的可能被卸载、删除。
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"RendererDictionay.SetId Renderer:{id},Exception:{ex}");
        }
    }

    private static void SetRendererId(RendererId id,MeshRenderer renderer)
    {
        try
        {
            if (!RendererDict.ContainsKey(id.Id))
            {
                RendererDict.Add(id.Id, renderer);
            }
            else
            {
                RendererDict[id.Id] = renderer;
            }

            if (!IdDict.ContainsKey(id.Id))
            {
                IdDict.Add(id.Id, id);
            }
            else
            {
                IdDict[id.Id] = id;//旧的可能被卸载、删除。
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"RendererDictionay.SetRendererId Renderer:{id},Exception:{ex}");
        }
    }

    public static void InitInfos()
    {
        RendererDict.Clear();
        IdDict.Clear();
        InitRenderers();
        InitIds();
    }

    public static void InitRenderers()
    {
        DateTime start = DateTime.Now;
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        InitRenderers(renderers);
        Debug.LogError($"RendererDictionay.InitRenderers count:{renderers.Length},Dict:{RendererDict.Count} time:{(DateTime.Now - start)}");
    }

    public static void InitIds()
    {
        DateTime start = DateTime.Now;
        var ids = GameObject.FindObjectsOfType<RendererId>(true);
        foreach (var id in ids)
        {
            SetId(id);
        }
        Debug.LogError($"RendererDictionay.InitIds count:{ids.Length},Dict:{IdDict.Count} time:{(DateTime.Now - start)}");
    }

    public static void InitRenderers(MeshRenderer[] renderers)
    {
        foreach (var renderer in renderers)
        {
            if(renderer==null)
            {
                Debug.LogError($"renderer==null");
                continue;
            }
            RendererId id = RendererId.GetId(renderer);
            SetRendererId(id, renderer);
        }
    }


    internal static void InitRenderers(GameObject[] objs)
    {
        DateTime start = DateTime.Now;
        int count1 = RendererDict.Count;
        foreach(var obj in objs)
        {
            //var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            //InitRenderers(renderers);

            var ids = obj.GetComponentsInChildren<RendererId>();
            foreach(var id in ids)
            {
                SetId(id);
            }
        }
        int count2 = RendererDict.Count;
        Debug.LogError($"RendererDictionay.InitRenderers count1:{count1} count2:{count2} add:{count2-count1} time:{(DateTime.Now - start)}");
    }

    public static GameObject GetGo(string id)
    {
        if (!IdDict.ContainsKey(id))
        {
            InitIds();
        }

        if (IdDict.ContainsKey(id))
        {
            if (IdDict[id] == null)
            {
                InitIds();
            }
            return IdDict[id].gameObject;
        }
        return null;
    }

    public static MeshRenderer GetRenderer(string id)
    {
        if (!RendererDict.ContainsKey(id))
        {
            InitInfos();
        }

        if (RendererDict.ContainsKey(id))
        {
            if (RendererDict[id] == null)
            {
                InitInfos();//可能被删除掉
            }
            return RendererDict[id];
        }
        Debug.LogError($"RendererDictionay.GerRenderer not found id:{id},Dict:{RendererDict.Count}");
        return null;
    }

    internal static List<MeshRenderer> GetRenderers(List<string> renderersId)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach(var id in renderersId)
        {
            renderers.Add(GetRenderer(id));
        }
        return renderers;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RendererDictionay
{
    public static Dictionary<int, MeshRenderer> Dict = new Dictionary<int, MeshRenderer>();

    public static void InitRenderers()
    {
        DateTime start = DateTime.Now;
        Dict.Clear();
        var renderers=GameObject.FindObjectsOfType<MeshRenderer>(true);
        InitRenderers(renderers);
        Debug.LogError($"RendererDictionay.InitRenderers count:{renderers.Length},Dict:{Dict.Count} time:{(DateTime.Now-start)}");
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
            try
            {
                Dict.Add(renderer.GetInstanceID(), renderer);
                //Dict.Add(renderer., renderer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"RendererDictionay.InitRenderers Renderer:{renderer},Exception:{ex}");
            }
           
        }
    }


    internal static void InitRenderers(GameObject[] objs)
    {
        DateTime start = DateTime.Now;
        int count1 = Dict.Count;
        foreach(var obj in objs)
        {
            var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            InitRenderers(renderers);
        }
        int count2 = Dict.Count;
        Debug.LogError($"RendererDictionay.InitRenderers count1:{count1} count2:{count2} add:{count2-count1} time:{(DateTime.Now - start)}");
    }

    public static MeshRenderer GerRenderer(int id)
    {
        if (!Dict.ContainsKey(id))
        {
            InitRenderers();
        }

        if (Dict.ContainsKey(id))
        {
            return Dict[id];
        }
        Debug.LogError($"RendererDictionay.GerRenderer not found id:{id},Dict:{Dict.Count}");
        return null;
    }

    internal static List<MeshRenderer> GetRenderers(List<int> renderersId)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach(var id in renderersId)
        {
            renderers.Add(GerRenderer(id));
        }
        return renderers;
    }

}

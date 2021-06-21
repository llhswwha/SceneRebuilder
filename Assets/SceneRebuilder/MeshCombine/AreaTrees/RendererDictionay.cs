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
        foreach(var renderer in renderers)
        {
            Dict.Add(renderer.GetInstanceID(), renderer);
        }
        Debug.LogError($"RendererDictionay.InitRenderers count:{renderers.Length} time:{(DateTime.Now-start)}");
    }

    public static MeshRenderer GerRenderer(int id)
    {
        if (Dict.ContainsKey(id))
        {
            return Dict[id];
        }
        Debug.LogError("RendererDictionay.GerRenderer not found id:"+id);
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

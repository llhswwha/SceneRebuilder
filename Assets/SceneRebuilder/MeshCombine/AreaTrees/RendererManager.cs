using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RendererManager : MonoBehaviour
{

    public MeshRenderer[] allRenderers;

    public List<RendererId> allIds = new List<RendererId>();

    public int Count = 0;

    [ContextMenu("InitIds")]
    public void InitIds()
    {
        DateTime start = DateTime.Now;
        allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        allIds.Clear();
        foreach(var r in allRenderers)
        {
            RendererId id = r.GetComponent<RendererId>();
            if (id == null)
            {
                id = r.gameObject.AddComponent<RendererId>();
                id.Init(r);
            }
            allIds.Add(id);
        }
        Count = allRenderers.Length;
        Debug.LogError($"InitRenderers count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("ClearIds")]
    public void ClearIds()
    {
        DateTime start = DateTime.Now;
        allIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allIds)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.LogError($"ClearIds count:{allRenderers.Length} time:{(DateTime.Now - start)}");
    }
}

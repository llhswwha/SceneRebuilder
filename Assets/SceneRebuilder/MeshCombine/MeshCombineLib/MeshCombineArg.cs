using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class MeshCombineArg
{
    public string prefix = "";
    public string sourceName;
    public GameObject source;
    public bool isCenterPivot = false;

    public MeshCombineArg(GameObject sour)
    {
        this.sourceName = sour.name;
        //prefix = source.name;
        this.source = sour;
        this.renderers = sour.GetComponentsInChildren<MeshRenderer>(true);
    }

    public MeshCombineArg(GameObject sour, MeshRenderer[] rs)
    {
        this.sourceName = sour.name;
        this.source = sour;
        this.renderers = rs;
    }

    public MeshCombineArg(List<GameObject> gameObjects)
    {
        //this.sourceName = sour.name;
        //this.source = sour;
        //this.renderers = rs;
        //List<MeshRenderer>
    }

    public MeshRenderer[] renderers;

    public MeshRenderer[] GetRenderers()
    {
        return renderers;
    }

    public void ShowRendererers()
    {
        if (renderers != null)
        {
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                renderer.enabled = true;
            }
        }
    }

    public string name
    {
        get
        {
            if (source == null) return "";
            return source.name;
        }
    }

    public Transform transform
    {
        get
        {
            if (source == null) return null;
            return source.transform;
        }
    }

    public void DestroySource()
    {
        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;
                GameObject.Destroy(r.gameObject);
            }
        }
        else
        {
            GameObject.Destroy(source);
        }

    }
}
public enum MeshCombineMode
{
    OneMesh, MultiByMat
}
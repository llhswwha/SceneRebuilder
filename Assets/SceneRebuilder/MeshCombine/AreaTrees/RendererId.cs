using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererId
    : MonoBehaviour
    //: Behaviour
{
    public string Id;

    public MeshRenderer mr;

    internal void Init(MeshRenderer r)
    {
        this.mr = r;
        Id = Guid.NewGuid().ToString();
    }

    internal void Init(GameObject go)
    {
        if (go == null) return;
        this.mr = go.GetComponent<MeshRenderer>();
        Id = Guid.NewGuid().ToString();
    }

    public static string GetId(GameObject r)
    {
        if (r == null) return "";
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        return id.Id;
    }

    public static RendererId GetId(MeshRenderer r)
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        return id;
    }
}

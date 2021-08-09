using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererInfoEx : MeshRendererInfo
{
    public MeshRenderer[] renderers;

    public MeshFilter[] meshFilters;

    public override MeshRenderer[] GetRenderers()
    {
        if (renderers==null|| renderers.Length==0)
        {
            renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>(true);
        }
        return renderers;
    }

    public override MeshFilter[] GetMeshFilters()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = this.gameObject.GetComponentsInChildren<MeshFilter>(true);
        }
        return meshFilters;
    }

    [ContextMenu("Init")]
    public override void Init()
    {
        //Debug.Log("Init");
        position = this.transform.position;
        meshRenderer = GetRenderers()[0];
        meshFilter = GetMeshFilters()[0];

        InitPos();
    }
}

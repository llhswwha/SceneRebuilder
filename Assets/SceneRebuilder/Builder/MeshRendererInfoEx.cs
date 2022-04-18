using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshRendererInfoEx : MeshRendererInfo
{
    public MeshRenderer[] renderers;

    public MeshFilter[] meshFilters;

    public void RemoveNull()
    {
        List<MeshRenderer> mrs = renderers.ToList();
        mrs.RemoveAll(i => i == null);
        renderers = mrs.ToArray();

        List<MeshFilter> mfs = meshFilters.ToList();
        mfs.RemoveAll(i => i == null);
        meshFilters = mfs.ToArray();
    }

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
        //position = this.transform.position;
        var rs = GetRenderers();
        if(rs.Length>0)
            meshRenderer = GetRenderers()[0];
        else
        {
            Debug.LogError($"[MeshRendererInfoEx.Init] No Renderers:{this.name}");
        }
        var mfs = GetMeshFilters();
        if(mfs.Length>0)
            meshFilter = mfs[0];
        else
        {

        }

        InitPos();
    }
}

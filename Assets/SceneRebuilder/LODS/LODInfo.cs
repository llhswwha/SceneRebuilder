using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LODInfo
{
    //
    // 摘要:
    //     The screen relative height to use for the transition [0-1].
    public float screenRelativeTransitionHeight;
    //
    // 摘要:
    //     Width of the cross-fade transition zone (proportion to the current LOD's whole
    //     length) [0-1]. Only used if it's not animated.
    public float fadeTransitionWidth;
    //
    // 摘要:
    //     List of renderers for this LOD level.
    public Renderer[] renderers;

    public int id;

    public LODInfo(LOD lod,int id)
    {
        screenRelativeTransitionHeight = lod.screenRelativeTransitionHeight;
        fadeTransitionWidth = lod.fadeTransitionWidth;
        renderers = lod.renderers;

        this.id = id;
    }

    internal LOD GetLOD()
    {
        LOD lod = new LOD();
        lod.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
        lod.fadeTransitionWidth = fadeTransitionWidth;
        lod.renderers = renderers;
        return lod;
    }

    public int vertextCount = 0;

    public string GetName()
    {
        if (renderers == null)
        {
            return $"LOD{id} (NULL)";
        }
        if(renderers.Length ==1 && renderers[0]!=null)
        {
            return $"LOD{id} ({renderers[0].name})";
        }
        else
        {
            return $"LOD{id} ([{renderers.Length}])";
        }
    }

    public Renderer GetRenderer()
    {
        if (renderers == null || renderers.Length==0)
        {
            return null;
        }
        return renderers[0];
    }

    public Material[] GetMats()
    {
        return renderers[0].sharedMaterials;
    }

    public void SetMats(Material[] matsNew)
    {
        foreach (var renderer in renderers)
        {
            //renderer.sharedMaterials = matsNew;
            var mats = renderer.sharedMaterials;
            for(int i=0;i<mats.Length;i++)
            {
                if(i< matsNew.Length)
                {
                    mats[i] = matsNew[i];
                }
                else
                {
                    mats[i] = matsNew[0];
                }
            }
            renderer.sharedMaterials = mats;
        }
    }
}

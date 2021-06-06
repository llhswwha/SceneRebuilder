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

    public LODInfo(LOD lod)
    {
        screenRelativeTransitionHeight = lod.screenRelativeTransitionHeight;
        fadeTransitionWidth = lod.fadeTransitionWidth;
        renderers = lod.renderers;
    }

    internal LOD GetLOD()
    {
        LOD lod = new LOD();
        lod.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
        lod.fadeTransitionWidth = fadeTransitionWidth;
        lod.renderers = renderers;
        return lod;
    }
}

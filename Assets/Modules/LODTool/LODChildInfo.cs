
using System;
using UnityEngine;

[Serializable]
public class LODChildInfo
{
    public int vertexCount;

    public float GetVertexCountF()
    {
        return vertexCount / 10000f;
    }

    public float vertexPercent;

    public int meshCount;
    public float meshPercent;

    public Renderer[] renderers;
    public float screenRelativeTransitionHeight;
}
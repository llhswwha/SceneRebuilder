using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CylinderMeshData 
{
    public List<CircleMeshData> Circles = new List<CircleMeshData>();

    public CylinderMeshData()
    {

    }

    public CylinderMeshData(params CircleMeshData[] circles)
    {
        Circles.AddRange(circles);
    }
}

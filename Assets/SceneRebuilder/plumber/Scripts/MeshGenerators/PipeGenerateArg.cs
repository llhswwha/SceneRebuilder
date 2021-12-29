using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeGenerateArg 
{
    public Material pipeMaterial;

    public Material weldMaterial;

    public int pipeSegments = 24;

    public int elbowSegments = 6;

    public float weldRadius = 0.005f;

    public bool generateWeld = false;

    public bool makeDoubleSided = false;

    public Vector3 Offset = Vector3.zero;

    public bool generateEndCaps = false;

    public void SetArg(PipeMeshGenerator pipe)
    {
        pipe.pipeSegments = this.pipeSegments;
        pipe.pipeMaterial = this.pipeMaterial;
        pipe.weldMaterial = this.weldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.generateWeld = this.generateWeld;
        pipe.elbowSegments = this.elbowSegments;
        pipe.makeDoubleSided = this.makeDoubleSided;
        pipe.generateEndCaps = this.generateEndCaps;
    }
}

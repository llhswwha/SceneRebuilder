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

    public Vector3 Offset = Vector3.zero;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PipeWeldSaveData : MeshModelSaveData
{
    public PipeWeldData Data;
}

[Serializable]
public struct PipeWeldData
{
    public Vector3 start; 
    public Vector3 direction; 
    [XmlAttribute]
    public float elbowRadius;
    [XmlAttribute]
    public float pipeRadius;


    public PipeWeldData(Vector3 start, Vector3 direction, float elbowRadius, float pipeRadius)
    {
        this.start = start;
        this.direction = direction;
        this.elbowRadius = elbowRadius;
        this.pipeRadius = pipeRadius;
    }
}

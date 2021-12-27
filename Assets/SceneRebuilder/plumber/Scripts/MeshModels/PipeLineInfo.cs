using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeLineInfo
{
    public Vector3 StartPoint = Vector3.zero;
    public Vector3 EndPoint = Vector3.zero;

    public Transform transform;

    public PipeLineInfo()
    {

    }

    public PipeLineInfo(Vector3 p1,Vector3 p2,Transform t)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
    }

    public Vector3 GetStartPoint()
    {
        //return StartPoint + this.transform.position;
        //return StartPoint;
        if (this.transform == null) return StartPoint;
        return this.transform.TransformPoint(StartPoint);
    }

    public Vector3 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        if (this.transform == null) return EndPoint;
        return this.transform.TransformPoint(EndPoint);
    }
}

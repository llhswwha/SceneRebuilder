using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeLineInfo
{
    //public Vector4 StartPoint = Vector3.zero;
    //public Vector4 EndPoint = Vector3.zero;

    public Vector4 StartPoint { get; private set; }
    public Vector4 EndPoint { get; private set; }

    public Vector3 Direction { get; private set; }

    //public PipeLineData=

    public Transform transform;

    public PipeLineInfo()
    {

    }

    public PipeLineInfo(PipeLineData data, Transform t)
    {
        this.StartPoint = data.StartPoint;
        this.EndPoint = data.EndPoint;
        this.transform = t;
        Direction = StartPoint - EndPoint;
    }

    public PipeLineInfo(Vector4 p1,Vector4 p2,Transform t=null)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
        Direction = StartPoint - EndPoint;
    }
    public PipeLineInfo(Vector4 p1, Vector4 p2, Transform t, Vector3 direction)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
        this.Direction = direction;
    }

    public Vector4 GetStartPoint()
    {
        //return StartPoint + this.transform.position;
        //return StartPoint;
        if (this.transform == null) return StartPoint;
        Vector4 p = this.transform.TransformPoint(StartPoint);
        p.w = StartPoint.w;
        return p;
    }

    public Vector4 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        if (this.transform == null) return EndPoint;
        Vector4 p= this.transform.TransformPoint(EndPoint);
        p.w = EndPoint.w;
        return p;
    }
}


public struct PipeLineData
{
    public Vector4 StartPoint;
    public Vector4 EndPoint;

    public Vector3 Direction;

    public bool IsGetInfoSuccess;

    public bool IsObbError;

    public override string ToString()
    {
        //return $"[{StartPoint}_{EndPoint}]";
        return $"[IsGetInfoSuccess:{IsGetInfoSuccess}_({StartPoint.x},{StartPoint.y},{StartPoint.z},{StartPoint.w})_({EndPoint.x},{EndPoint.y},{EndPoint.z},{EndPoint.w})]";
    }

    public PipeLineData(Vector4 p1, Vector4 p2, Vector3 direction)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.Direction = direction;
        this.IsGetInfoSuccess = true;
        this.IsObbError = false;
    }
}



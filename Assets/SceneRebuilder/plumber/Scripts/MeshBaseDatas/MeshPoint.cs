using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshPoint
{
    public Vector3 Point;

    public int Id;

    public Vector3 Normal;

    public MeshPoint()
    {

    }

    public MeshPoint(Vector3 p, int i,Vector3 n)
    {
        this.Point = p;
        this.Id = i;
        this.Normal = n;
    }

    //public override string ToString()
    //{
    //    return Point.ToString();
    //}

    public override string ToString()
    {
        return Id.ToString();
    }
}

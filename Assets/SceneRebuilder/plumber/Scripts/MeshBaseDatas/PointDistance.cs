using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointDistance :IComparable<PointDistance>
{
    public Vector3 P1;
    public Vector3 P2;
    public float Distance = 0;
    public PointDistance(Vector3 p1,Vector3 p2)
    {
        this.P1 = p1;
        this.P2 = p2;
        this.Distance = Vector3.Distance(p1, p2);
    }

    public int CompareTo(PointDistance other)
    {
        return this.Distance.CompareTo(other.Distance);
    }
}

[Serializable]
public class PointDistanceEx : IComparable<PointDistanceEx>
{
    public MeshPoint P1;
    public Vector3 P2;
    public float Distance = 0;
    public PointDistanceEx(MeshPoint p1, Vector3 p2)
    {
        this.P1 = p1;
        this.P2 = p2;
        this.Distance = Vector3.Distance(p1.Point, p2);
    }

    public int CompareTo(PointDistanceEx other)
    {
        return this.Distance.CompareTo(other.Distance);
    }
}

[Serializable]
public class PlanePointDistance : IComparable<PlanePointDistance>
{
    public SharedMeshTriangles Plane;
    public Vector3 P2;
    public float Distance = 0;
    public PlanePointDistance(SharedMeshTriangles p1, Vector3 p2)
    {
        this.Plane = p1;
        this.P2 = p2;
        this.Distance = Vector3.Distance(p1.GetCenter(), p2);
    }

    public int CompareTo(PlanePointDistance other)
    {
        return this.Distance.CompareTo(other.Distance);
    }
}


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

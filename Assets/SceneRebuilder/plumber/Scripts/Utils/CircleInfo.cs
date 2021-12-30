using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleInfo
{
    public Vector3 Center;

    public float Radius;

    public CircleInfo()
    {

    }

    public CircleInfo(Vector3 c, float r)
    {
        this.Center = c;
        this.Radius = r;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleInfo
{
    public static float IsCircleMaxP = 1.2f;

    public List<Vector3> Points;

    public Vector3 Center;

    public float Radius;

    public bool IsCircle = true;

    public float CheckCircleP = 0;

    public Vector4 GetCenter4()
    {
        Vector4 c = Center;
        c.w = Radius;
        return c;
    }

    public CircleInfo()
    {

    }

    public CircleInfo(Vector3 c, float r)
    {
        this.Center = c;
        this.Radius = r;
    }

    public CircleInfo(List<Vector3> vs)
    {
        this.Points = vs;

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < vs.Count; i++)
        {
            Vector3 v = vs[i];
            sum += v;
        }
        Vector3 center = sum / vs.Count;
        this.Center = center;

        float radiusSum = 0;
        List<float> radiusList = new List<float>();
        for (int i = 0; i < vs.Count; i++)
        {
            Vector3 v = vs[i];
            var r= Vector3.Distance(v, center);
            radiusSum += r;
            radiusList.Add(r);
        }

        float radius = radiusSum / vs.Count;
        this.Radius = radius;

        float min = radiusList[0];
        float max = radiusList[radiusList.Count - 1];
        float p = max / min;
        IsCircle = p <= IsCircleMaxP;
        CheckCircleP = p;
    }
}

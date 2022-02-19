using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointHelper
{

    public static List<GameObject> ShowLocalPoints(IEnumerable<Vector3> ps, Vector3 scale, Transform t)
    {
        List<GameObject> gos = new List<GameObject>();
        int i = 0;
        foreach (var p in ps)
        {
            i++;
            GameObject go = ShowLocalPoint(p, scale, t);
            //go.name = $"Point_{i}_{p}";
            go.name = $"Point_{i}_({p.x},{p.y},{p.z})";
            gos.Add(go);
        }
        return gos;
    }

    public static List<GameObject> ShowPoints(IEnumerable<Vector4> ps, Vector3 scale, Transform t)
    {
        List<GameObject> gos = new List<GameObject>();
        int i = 0;
        foreach (var p in ps)
        {
            i++;
            GameObject go = ShowLocalPoint(p, scale, t);
            //go.name = $"Point_{i}_{p}";
            go.name = $"Point_{i}_({p.x},{p.y},{p.z})";
            gos.Add(go);
        }
        return gos;
    }

    public static GameObject ShowLocalPoint(Vector3 p, Vector3 scale, Transform t)
    {
        //Vector3 p2 = t.TransformPoint(p);

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.transform.SetParent(t);
        //go.transform.position = points[i];
        go.transform.localScale = scale;

        go.transform.localPosition = p;

        //go.transform.position = Vector3.zero;
        //Vector3 p2 = go.transform.TransformPoint(p);
        //go.transform.localPosition = p2;
        return go;
    }

    public static GameObject ShowWorldPoint2(Vector3 p, Vector3 scale, Transform t)
    {
        Vector3 p2 = t.TransformPoint(p);

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.transform.SetParent(t);
        //go.transform.position = points[i];
        go.transform.localScale = scale;

        go.transform.position = p2;

        //go.transform.position = Vector3.zero;
        //Vector3 p2 = go.transform.TransformPoint(p);
        //go.transform.localPosition = p2;
        return go;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointHelper
{
    public static List<GameObject> ShowPoints(IEnumerable<Vector3> ps, Vector3 scale, Transform t)
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (var p in ps)
        {
            gos.Add(ShowPoint(p, scale, t));
        }
        return gos;
    }

    public static GameObject ShowPoint(Vector3 p, Vector3 scale, Transform t)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.transform.SetParent(t);
        //go.transform.position = points[i];
        go.transform.localScale = scale;
        go.transform.localPosition = p;
        return go;
    }
}

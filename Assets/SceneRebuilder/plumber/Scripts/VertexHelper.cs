using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VertexHelper
{
    public static void ShowPoints(IEnumerable<Vector3> ps, Vector3 scale, Transform t)
    {
        foreach (var p in ps)
        {
            ShowPoint(p, scale, t);
        }
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

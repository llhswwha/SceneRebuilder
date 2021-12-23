using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointHelper
{
    public static List<GameObject> ShowPoints(IEnumerable<Vector3> ps, Vector3 scale, Transform t)
    {
        List<GameObject> gos = new List<GameObject>();
        int i = 0;
        foreach (var p in ps)
        {
            i++;
            GameObject go = ShowPoint(p, scale, t);
            //go.name = $"Point_{i}_{p}";
            go.name = $"Point_{i}_({p.x},{p.y},{p.z})";
            gos.Add(go);
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

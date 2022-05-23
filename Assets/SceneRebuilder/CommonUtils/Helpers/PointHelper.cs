using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointHelper 
{
    public static GameObject ShowLocalPoint(Vector3 point, float pointScale, Transform transform1, Transform transform2)
    {
        GameObject objPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        objPoint.transform.SetParent(transform1);
        if (point.x == float.NaN)
        {
            Debug.LogError($"ShowLocalPoint NaN transform1:{transform1} transform2:{transform2} point:{point}");
            return objPoint;
        }
        try
        {
            //objPoint.name = $"Point[{i + 1}][{j + 1}]({p.Point})";
            //objPoint.name = $"Point[{j + 1}]({p.Point})";
            objPoint.name = $"Point({point.x},{point.y},{point.z})";
            objPoint.transform.localPosition = point;
            objPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            if (transform2 != null)
                objPoint.transform.SetParent(transform2);
            return objPoint;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ShowLocalPoint transform1:{transform1} transform2:{transform2} point:{point} Exception:{ex}");
            return objPoint;
        }

    }


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

    public static GameObject CreatePoint(Vector3 p, string n, Transform pT, float size)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=p;
        g1.transform.position = p;
        g1.transform.localScale = new Vector3(size, size, size);
        g1.name = n;

        g1.transform.SetParent(pT);
        return g1;
    }

    public static GameObject CreateLocalPoint(Vector3 p, string n, Transform pT, float scale)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(pT);
        g1.transform.localPosition = p;
        //g1.transform.position = p;
        g1.transform.localScale = new Vector3(scale, scale, scale);
        g1.name = n;

        //g1.transform.SetParent(this.transform);

        g1.AddComponent<DebugInfoRoot>();
        return g1;
    }
}

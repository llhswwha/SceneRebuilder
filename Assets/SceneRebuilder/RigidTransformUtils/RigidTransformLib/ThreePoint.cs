using MeshJobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ThreePoint
{
    public Vector3 center;

    public Vector3 minP;

    public int minId;

    public Vector3 maxP;

    public int maxId;

    public Matrix4x4 localToWorldMatrix;

    public ThreePoint(Vector3 c, Vector3 min, Vector3 max, int minId, int maxId, Matrix4x4 matrix)
    {
        this.center = c;
        this.minP = min;
        this.maxP = max;
        this.minId = minId;
        this.maxId = maxId;
        this.localToWorldMatrix = matrix;
    }

    public override string ToString()
    {
        return string.Format("(ThreePoint center:{0},maxP:{1},minP:{2})",
             center.Vector3ToString(), maxP.Vector3ToString(), minP.Vector3ToString());
    }

    public Vector3 GetCenterP(Transform t)
    {
        // return t.TransformPoint(center);
        //return TransformPoint(t.localToWorldMatrix,center);

        var c1 = t.TransformPoint(center);
        // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
        // c1.PrintVector3("c1");
        // c2.PrintVector3("c2");
        return c1;
    }

    public Vector3 GetCenterP()
    {
        // return t.TransformPoint(center);
        //return TransformPoint(t.localToWorldMatrix,center);

        var c1 = localToWorldMatrix.MultiplyPoint3x4(center);
        // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
        // c1.PrintVector3("c1");
        // c2.PrintVector3("c2");
        return c1;
    }

    public Vector3 GetNormalP()
    {
        return GetCenterP() + GetLongShortNormal();
    }

    public Vector3 GetMaxP()
    {
        var c1 = localToWorldMatrix.MultiplyPoint3x4(maxP);
        return c1;
    }

    public Vector3 GetMinP()
    {
        var c1 = localToWorldMatrix.MultiplyPoint3x4(minP);
        return c1;
    }

    public Vector3 GetMaxPN()
    {
        return GetCenterP() + GetLongLineN();
    }

    public Vector3 GetMinPN()
    {
        return GetCenterP() + GetShortLineN();
    }


    public Vector3 GetCenterP(Matrix4x4 matrix)
    {
        var c1 = matrix.MultiplyPoint3x4(center);
        return c1;
    }

    public Vector3 GetLongShortNormal()
    {
        return GetLongShortNormal(this.localToWorldMatrix);
    }

    public Vector3 GetLongShortNormal(Matrix4x4 matrix)
    {
        var maxP = matrix.MultiplyPoint3x4(this.maxP);
        var minP = matrix.MultiplyPoint3x4(this.minP);
        var centerP = matrix.MultiplyPoint3x4(this.center);

        var longLine = maxP - centerP;
        longLine = Vector3.Normalize(longLine);

        var shortLine = minP - centerP;
        shortLine = Vector3.Normalize(shortLine);

        //Vector3 nor= Vector3.Cross(longLine,shortLine);

        Vector3 nor = Vector3.Cross(shortLine, longLine);

        // float angle1=Vector3.Angle(longLine,nor);//90度
        // float angle2=Vector3.Angle(shortLine,nor);//90度
        // Debug.Log($"GetLongShortNormal2 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");
        return nor;
    }

    public void ShowDebugDetail(Transform t)
    {
        var centerP = GetCenterP(t);
        CreateWorldPoint(centerP, "CenterP", t);

        //   var minP=
    }

    private GameObject CreateWorldPoint(Vector3 p, string n, Transform t)
    {
        float pScale = 0.1f;
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(pScale, pScale, pScale);
        go.transform.position = p;
        go.name = n;
        go.transform.SetParent(t);
        // vertextObjects.Add(go);
        return go;
    }

    //   private Vector3 GetShortLine(Matrix4x4 matrix)
    //   {
    //     throw new NotImplementedException();
    //   }

    //   private Vector3 GetLongLine(Matrix4x4 matrix)
    //   {
    //     throw new NotImplementedException();
    //   }

    public Vector3 GetLongLine()
    {
        var maxP = localToWorldMatrix.MultiplyPoint3x4(this.maxP);
        var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
        var longLine = maxP - centerP;
        return longLine;
    }

    public Vector3 GetShortLine()
    {
        var minP = localToWorldMatrix.MultiplyPoint3x4(this.minP);
        var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
        var shortLine = minP - centerP;
        return shortLine;
    }


    public Vector3 GetLongLineN()
    {
        var maxP = localToWorldMatrix.MultiplyPoint3x4(this.maxP);
        var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
        var longLine = maxP - centerP;
        return longLine.normalized;
    }

    public Vector3 GetShortLineN()
    {
        var minP = localToWorldMatrix.MultiplyPoint3x4(this.minP);
        var centerP = localToWorldMatrix.MultiplyPoint3x4(this.center);
        var shortLine = minP - centerP;
        return shortLine.normalized;
    }

    public float GetLongShortAngle()
    {
        return Vector3.Angle(this.GetLongLine(), this.GetShortLine());
    }

    public void PrintLog(string tag)
    {
        Debug.Log($"[{tag}]{this.ToString()}");
    }
}

public static class ThreePointHelper
{
    public static float planeScale = 0.1f;

    public static float pScale = 0.01f;

    public static float lineScale = 0.005f;

    public static GameObject RTPrefab = null;

    public static int TotalRTCount;

    public static RTTransform GetRT(ref ThreePoint tp, Transform parent)
    {
        //GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if (RTPrefab == null)
        {
            RTPrefab = new GameObject("RT");
            RTPrefab.SetActive(false);
        }
        GameObject tmp = RTPrefab;//重复利用同一个物体，获取不同的变换数据

        Transform t = tmp.transform;
        // tmp.name="RT";
        // t.localScale = new Vector3(planeScale, planeScale, planeScale);

        t.position = tp.GetCenterP();
        t.up = tp.GetLongShortNormal();
        var angle = Vector3.Angle(t.right, tp.GetShortLine());
        t.Rotate(Vector3.up, angle);

        RTTransform rT = new RTTransform(t.rotation, t.position);//获取不同的变换数据
                                                                 // t.SetParent(parent);
        TotalRTCount++;
        return rT;
    }

    public static Transform CreateNormalPlane(ref ThreePoint tp, Transform parent, bool showDetail)
    {
        var normalPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Transform t = normalPlane.transform;

        Vector3 center = tp.GetCenterP();
        Vector3 minMaxNormal = tp.GetLongShortNormal();

        t.localScale = new Vector3(planeScale, planeScale, planeScale);
        t.position = center;
        t.up = minMaxNormal;
        //t.right=this.GetShortLine();

        var angle = Vector3.Angle(t.right, tp.GetShortLine());
        t.Rotate(Vector3.up, angle);

        // normalPlane.name = "NormalPlane"+ log;
        // t.SetParent(this.transform);
        // vertextObjects.Add(normalPlane);

        if (showDetail)
        {
            Vector3 maxP = tp.GetMaxP();
            Vector3 minP = tp.GetMinP();
            CreateWorldPoint(center, "CenterP", t);
            CreateWorldPoint(maxP, "MaxP", t);
            CreateWorldPoint(minP, "MinP", t);
            CreateWorldLine(center, maxP, "LoneLine", t);
            CreateWorldLine(center, minP, "ShortLine", t);

            // Vector3 normalPoint = minMaxNormal + center;
            //  CreateWorldPoint(normalPoint, string.Format("[{0}]({1},{2},{3})", "minMaxNormal1", minMaxNormal.x, minMaxNormal.y, minMaxNormal.z),t);
            //  //NormalLineGo = CreateWorldLine(center, normalPoint, "minMaxNormal1:" + Vector3.Distance(center, normalPoint));
            // CreateWorldLine(center, normalPoint, "minMaxNormal1_" + Vector3.Distance(center, normalPoint),t);
        }
        t.SetParent(parent);
        return t;
    }

    private static GameObject CreateWorldLine(Vector3 p1, Vector3 p2, string n, Transform t)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward = p2 - p1;
        var dis = Vector3.Distance(p2, p1);
        if (dis < 0.1f)
        {
            dis = 0.1f;
        }
        Vector3 scale = new Vector3(1f * lineScale, 1f * lineScale, dis);
        g1.transform.localScale = scale;
        g1.name = n;
        g1.transform.SetParent(t);
        return g1;
    }

    private static GameObject CreateWorldPoint(Vector3 p, string n, Transform t)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(pScale, pScale, pScale);
        go.transform.position = p;
        go.name = n;
        go.transform.SetParent(t);
        return go;
    }

}


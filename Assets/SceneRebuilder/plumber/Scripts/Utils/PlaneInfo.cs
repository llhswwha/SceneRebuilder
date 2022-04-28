using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlaneInfo
{
    public Vector3 planeNormal;
    public Vector3 planePoint;
    public Vector3 planeCenter;

    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 pointC;
    public Vector3 pointD;

    public float Size = 0;
    public float SizeX = 0;
    public float SizeY = 0;

    public Vector3 Edge1;

    public Vector3 Edge2;

    //public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3)
    //{
    //    pointA = p1.GetVector3();
    //    pointB = p2.GetVector3();
    //    pointC = p3.GetVector3();
    //    Size = Vector3.Distance(pointA, pointB);
    //    SizeX = Vector3.Distance(pointA, pointB);
    //    SizeY = Vector3.Distance(pointB, pointC);
    //    if (SizeX > SizeY)
    //    {
    //        Size = SizeX;
    //    }
    //    else
    //    {
    //        Size = SizeY;
    //    }

    //    Edge1 = pointB - pointA;
    //    Edge2 = pointC - pointB;
    //    Math3D.PlaneFrom3Points(out planeNormal, out planePoint, pointA, pointB, pointC);
    //    planeCenter = planePoint;
    //}

    public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3,Vector3S p4)
    {
        pointA = p1.GetVector3();
        pointB = p2.GetVector3();
        pointC = p3.GetVector3(); 
        pointD = p4.GetVector3();
        Init();
    }

    private void Init()
    {
        planeCenter = (pointA + pointB + pointC + pointD) / 4;
        Size = Vector3.Distance(pointA, pointB);
        SizeX = Vector3.Distance(pointA, pointB);
        SizeY = Vector3.Distance(pointB, pointC);
        if (SizeX > SizeY)
        {
            Size = SizeX;
        }
        else
        {
            Size = SizeY;
        }
        Edge1 = pointB - pointA;
        Edge2 = pointC - pointB;
        Math3D.PlaneFrom3Points(out planeNormal, out planePoint, pointA, pointB, pointC);
    }

    public PlaneInfo(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        pointA = p1;
        pointB = p2;
        pointC = p3;
        pointD = p4;
        //Math3D.PlaneFrom3Points(out planeNormal, out planePoint, pointA, pointB, pointC);
        Init();
    }

    //public PlaneInfo(Vector3 p1,Vector3 p2,Vector3 p3)
    //{
    //    pointA = p1;
    //    pointB = p2;
    //    pointC = p3;
    //    Math3D.PlaneFrom3Points(out planeNormal,out planePoint,pointA,pointB,pointC);
    //}

    public List<Vector3> GetPoints()
    {
        return new List<Vector3>() { pointA, pointB, pointC, pointD };
    }

    public Vector3 GetClosedPlanePoint1(Vector3 p)
    {
        var list = GetPoints();
        float minD = float.MaxValue;
        int minI = 0;
        for (int i = 0; i < list.Count; i++)
        {
            float dis = Vector3.Distance(list[i], p);
            if (dis < minD)
            {
                minD = dis;
                minI = i;
            }
        }
        return list[minI];
    }

    public static PlaneInfo GetMiddlePlane(PlaneInfo plane1, PlaneInfo plane2)
    {
        var ps1 = plane1.GetPoints();
        var ps2 = plane2.GetPoints();
        if (ps1.Count != ps2.Count)
        {
            Debug.LogError($"PlaneInfo.GetMiddlePlane ps1.Count != ps2.Count ps1:{ps1.Count},ps2:{ps2.Count}");
            return null;
        }
        List<Vector3> ps3 = new List<Vector3>();
        foreach (var p1 in ps1)
        {
            Vector3 p2 = plane2.GetClosedPlanePoint1(p1);
            float dis = Vector3.Distance(p1, p2);
            Vector3 p3 = (p1 + p2) / 2;
            ps3.Add(p3);
        }
        PlaneInfo plane30 = new PlaneInfo(ps3[0], ps3[1], ps3[2], ps3[3]);
        return plane30;
    }

    public void ShowPlaneInfo( int i, GameObject go, VerticesToPlaneInfo v2p,float lineSize,Transform transform,bool isShowPlane2Ponit=false,bool isShowPlane1Points12=false)
    {
        PlaneInfo plane = this;
        GameObject planeObjRoot = new GameObject($"Plane[{i}]");
        planeObjRoot.transform.SetParent(go.transform);
        planeObjRoot.transform.localPosition = Vector3.zero;

        var point = plane.planePoint;
        var normal = plane.planeNormal * 0.1f;
        var normalPoint = (point + normal);
        TransformHelper.ShowLocalPoint(point, lineSize, transform, planeObjRoot.transform).name = $"Point:{point}";
        TransformHelper.ShowLocalPoint(normalPoint, lineSize, transform, planeObjRoot.transform).name = $"Normal:{normal}";
        TransformHelper.ShowLocalPoint(plane.planeCenter, lineSize, transform, planeObjRoot.transform).name = $"Center:{plane.planeCenter}";

        TransformHelper.ShowLocalPoint(plane.pointA, lineSize, transform, planeObjRoot.transform).name = $"pointA:{plane.pointA}";
        TransformHelper.ShowLocalPoint(plane.pointB, lineSize, transform, planeObjRoot.transform).name = $"pointB:{plane.pointB}";
        TransformHelper.ShowLocalPoint(plane.pointC, lineSize, transform, planeObjRoot.transform).name = $"pointC:{plane.pointC}";
        TransformHelper.ShowLocalPoint(plane.pointD, lineSize, transform, planeObjRoot.transform).name = $"pointD:{plane.pointD}";

        CreateLine(transform.TransformPoint(point), transform.TransformPoint(normalPoint), $"NormalLine:{normal}", planeObjRoot.transform,lineSize,transform);

        //GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        string nameInfo = $"Plane[{i}] size:({plane.SizeX},{plane.SizeY})_";
        if (v2p != null)
        {
            nameInfo += v2p.ToString();
        }
        planeObjRoot.name = nameInfo;

        planeObj.name = nameInfo;
        planeObj.transform.SetParent(transform);
        //planeObj.transform.localPosition = point;
        planeObj.transform.localPosition = plane.planeCenter;
        planeObj.transform.forward = normal;
        //planeObj.transform.right = plane.Edge1;
        //planeObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.001f);

        //planeObj.transform.localScale = new Vector3(plane.SizeX, plane.SizeY, 0.001f);
        planeObj.transform.localScale = new Vector3(plane.Size, plane.Size, 0.001f);

        planeObj.transform.SetParent(planeObjRoot.transform);

        if (v2p != null)
        {
            for (int i1 = 0; i1 < v2p.Plane1Points.Count; i1++)
            {
                Vector3 p = v2p.Plane1Points[i1];
                TransformHelper.ShowLocalPoint(p, lineSize, transform, planeObjRoot.transform).name = $"Plane1Points[{i1}]";
            }
            if (isShowPlane1Points12)
            {
                for (int i1 = 0; i1 < v2p.Plane1Points1.Count; i1++)
                {
                    Vector3 p = v2p.Plane1Points1[i1];
                    TransformHelper.ShowLocalPoint(p, lineSize, transform, planeObjRoot.transform).name = $"Plane1Points+[{i1}]";
                }
                for (int i1 = 0; i1 < v2p.Plane1Points2.Count; i1++)
                {
                    Vector3 p = v2p.Plane1Points2[i1];
                    TransformHelper.ShowLocalPoint(p, lineSize, transform, planeObjRoot.transform).name = $"Plane1Points-[{i1}]";
                }
            }
            if (isShowPlane2Ponit)
            {
                for (int i1 = 0; i1 < v2p.Plane2Points.Count; i1++)
                {
                    Vector3 p = v2p.Plane2Points[i1];
                    TransformHelper.ShowLocalPoint(p, lineSize, transform, planeObjRoot.transform).name = $"Plane2Points[{i1}]";
                }
            }
        }
    }

    public Vector3 CreateLine(Vector3 p1, Vector3 p2, string n, Transform pt, float lineSize,Transform transform)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=(p1+p2)/2;
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward = p2 - p1;
        Vector3 scale = new Vector3(lineSize, lineSize, Vector3.Distance(p2, p1));
        g1.transform.localScale = scale;
        g1.name = n;
        g1.transform.SetParent(transform);
        if (pt != null)
            g1.transform.SetParent(pt);
        return p2 - p1;
    }
}

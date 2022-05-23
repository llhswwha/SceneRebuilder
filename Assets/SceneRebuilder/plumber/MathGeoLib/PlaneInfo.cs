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

    public bool IsNaN()
    {
        return float.IsNaN(pointA.x) || float.IsNaN(pointB.x)|| float.IsNaN(pointC.x);
    }

    public override string ToString()
    {
        return $"{pointA},{pointB},{pointC},{pointD}";
    }

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

        //if (IsNaN())
        //{
        //    Debug.LogError($"PlaneInfo plane:{this}");
        //}
    }

    public PlaneInfo()
    {
        pointA = Vector3.zero;
        pointB = Vector3.zero;
        pointC = Vector3.zero;
        pointD = Vector3.zero;
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

    public PlaneInfo(List<Vector3> list)
    {
        if (list.Count < 3)
        {
            Debug.LogError($"PlaneInfo list.Count < 3");
            return;
        }
        pointA = list[0];
        pointB = list[1];
        pointC = list[2];

        if (list.Count > 3)
        {
            pointD = list[3];
        }
        else
        {
            pointD = Vector3.zero;
        }
        Init();
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


}

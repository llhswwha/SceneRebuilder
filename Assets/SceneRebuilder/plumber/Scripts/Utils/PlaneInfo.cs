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

    public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3)
    {
        pointA = p1.GetVector3();
        pointB = p2.GetVector3();
        pointC = p3.GetVector3();
        Math3D.PlaneFrom3Points(out planeNormal, out planePoint, pointA, pointB, pointC);
    }

    public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3,Vector3S p4)
    {
        pointA = p1.GetVector3();
        pointB = p2.GetVector3();
        pointC = p3.GetVector3(); 
        pointD = p4.GetVector3();
        planeCenter = (pointA+pointB+pointC+pointD) / 4;
        Math3D.PlaneFrom3Points(out planeNormal, out planePoint, pointA, pointB, pointC);
    }

    public PlaneInfo(Vector3 p1,Vector3 p2,Vector3 p3)
    {
        pointA = p1;
        pointB = p2;
        pointC = p3;
        Math3D.PlaneFrom3Points(out planeNormal,out planePoint,pointA,pointB,pointC);
    }
}

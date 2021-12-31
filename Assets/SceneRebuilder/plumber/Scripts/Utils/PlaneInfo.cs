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

    public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3)
    {
        pointA = p1.GetVector3();
        pointB = p2.GetVector3();
        pointC = p3.GetVector3();
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
        planeCenter = planePoint;
    }

    public PlaneInfo(Vector3S p1, Vector3S p2, Vector3S p3,Vector3S p4)
    {
        pointA = p1.GetVector3();
        pointB = p2.GetVector3();
        pointC = p3.GetVector3(); 
        pointD = p4.GetVector3();
        planeCenter = (pointA+pointB+pointC+pointD) / 4;
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

    public PlaneInfo(Vector3 p1,Vector3 p2,Vector3 p3)
    {
        pointA = p1;
        pointB = p2;
        pointC = p3;
        Math3D.PlaneFrom3Points(out planeNormal,out planePoint,pointA,pointB,pointC);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlaneHelper 
{
    public static Vector3 CreateLine(Vector3 p1, Vector3 p2, string n, Transform pt, float lineSize, Transform transform)
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

    public static void ShowPlaneInfo(this PlaneInfo plane,object tag, GameObject parentGo, VerticesToPlaneInfo v2p, float lineSize, Transform rootT, bool isShowPlane2Ponit = false, bool isShowPlane1Points12 = false)
    {
        //PlaneInfo plane = this;
        GameObject planeObjRoot = new GameObject($"Plane[{tag}]");
        planeObjRoot.transform.SetParent(parentGo.transform);
        planeObjRoot.transform.localPosition = Vector3.zero;

        var point = plane.planePoint;
        var normal = plane.planeNormal * 0.1f;
        var normalPoint = (point + normal);
        PointHelper.ShowLocalPoint(point, lineSize, rootT, planeObjRoot.transform).name = $"Point:{point}";
        PointHelper.ShowLocalPoint(normalPoint, lineSize, rootT, planeObjRoot.transform).name = $"Normal:{normal}";
        PointHelper.ShowLocalPoint(plane.planeCenter, lineSize, rootT, planeObjRoot.transform).name = $"Center:{plane.planeCenter}";

        PointHelper.ShowLocalPoint(plane.pointA, lineSize, rootT, planeObjRoot.transform).name = $"pointA:{plane.pointA}";
        PointHelper.ShowLocalPoint(plane.pointB, lineSize, rootT, planeObjRoot.transform).name = $"pointB:{plane.pointB}";
        PointHelper.ShowLocalPoint(plane.pointC, lineSize, rootT, planeObjRoot.transform).name = $"pointC:{plane.pointC}";
        PointHelper.ShowLocalPoint(plane.pointD, lineSize, rootT, planeObjRoot.transform).name = $"pointD:{plane.pointD}";

        CreateLine(rootT.TransformPoint(point), rootT.TransformPoint(normalPoint), $"NormalLine:{normal}", planeObjRoot.transform, lineSize, rootT);

        //GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        string nameInfo = $"Plane[{tag}] size:({plane.SizeX:F5},{plane.SizeY:F5})_";
        if (v2p != null)
        {
            nameInfo += v2p.ToString();
        }
        planeObjRoot.name = nameInfo;

        planeObj.name = nameInfo;
        planeObj.transform.SetParent(rootT);
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
                float dis1 = Math3D.SignedDistancePlanePoint(plane.planeNormal, plane.planePoint, p);
                PointHelper.ShowLocalPoint(p, lineSize, rootT, planeObjRoot.transform).name = $"Plane1Points[{i1}]_{dis1}";
            }
            if (isShowPlane1Points12)
            {
                for (int i1 = 0; i1 < v2p.Plane1Points1.Count; i1++)
                {
                    Vector3 p = v2p.Plane1Points1[i1];
                    PointHelper.ShowLocalPoint(p, lineSize, rootT, planeObjRoot.transform).name = $"Plane1Points+[{i1}]";
                }
                for (int i1 = 0; i1 < v2p.Plane1Points2.Count; i1++)
                {
                    Vector3 p = v2p.Plane1Points2[i1];
                    PointHelper.ShowLocalPoint(p, lineSize, rootT, planeObjRoot.transform).name = $"Plane1Points-[{i1}]";
                }
            }
            //if (isShowPlane2Ponit)
            {
                for (int i1 = 0; i1 < v2p.Plane2Points.Count; i1++)
                {
                    Vector3 p = v2p.Plane2Points[i1];
                    float dis1 = Math3D.SignedDistancePlanePoint(plane.planeNormal, plane.planePoint, p);
                    PointHelper.ShowLocalPoint(p, lineSize, rootT, planeObjRoot.transform).name = $"Plane2Points[{i1}]_{dis1}";
                }
            }
        }
    }

    public static void ShowPlaneInfo(this OBBCollider obbc,PlaneInfo plane, int i, GameObject go, VerticesToPlaneInfo v2p)
    {
        PlaneHelper.ShowPlaneInfo(plane, i, go, v2p, obbc.lineSize, obbc.transform);
    }

    public static void ShowObbPlanes(this OBBCollider obbc)
    {
        PlaneInfo[] planes = obbc.OBB.GetPlaneInfos();
        GameObject go = new GameObject("Planes");
        go.transform.SetParent(obbc.transform);
        go.transform.localPosition = Vector3.zero;

        for (int i = 0; i < planes.Length; i++)
        {
            var plane = planes[i];
            //ShowPlaneInfo(plane, i, go,null);
            PlaneHelper.ShowPlaneInfo(obbc, plane, i, go, null);
        }
    }
}

using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PipeLineInfoJob : IJob
{
    public int Id;

    public OrientedBoundingBox OBB;

    public NativeArray<Vector3>  points;

    public static NativeArray<PipeLineData> Result;

    public void Execute()
    {
        DateTime start = DateTime.Now;
        Vector3 ObbExtent = OBB.Extent;

        Vector3 startPoint = OBB.Up * ObbExtent.y;
        Vector3 endPoint = -OBB.Up * ObbExtent.y;

        //GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        //planInfoRoot.transform.SetParent(go.transform);
        //planInfoRoot.transform.localPosition = Vector3.zero;

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
        //var rendererInfo = MeshRendererInfo.GetInfo(go.gameObject);
        //Vector3[] vs = rendererInfo.GetVertices();

        Vector3[] vs = points.ToArray();
        var VertexCount = vs.Length;

        //2.Planes
        PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        var verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false);
            verticesToPlaneInfos_All.Add(v2p);
            //oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);
            if (v2p.IsCircle() == false)
            {
                continue;
            }
            verticesToPlaneInfos.Add(v2p);
            var isC = v2p.IsCircle();
        }
        verticesToPlaneInfos.Sort();

        if (verticesToPlaneInfos.Count < 1)
        {
            //IsGetInfoSuccess = false;
            Debug.LogError($"GetModelInfo verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count}");
            return;
        }

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >= 2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"GetModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count}");
            endPlane = GetEndPlane(startPlane, verticesToPlaneInfos_All);
        }


        var P1 = OBB.Right * ObbExtent.x;
        var P2 = -OBB.Forward * ObbExtent.z;
        var P3 = -OBB.Right * ObbExtent.x;
        var P4 = OBB.Forward * ObbExtent.z;
        var P5 = OBB.Up * ObbExtent.y;
        var P6 = -OBB.Up * ObbExtent.y;
        var Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        CircleInfo startCircle = startPlane.GetCircleInfo();
        if (startCircle == null)
        {
            Debug.LogError($"GetModelInfo startCircle == null ");
            //IsGetInfoSuccess = false;

            //CreateLocalPoint(startPlane.Point.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Point.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return;
        }
        startPoint = startCircle.Center;
        CircleInfo endCircle = endPlane.GetCircleInfo();
        if (endCircle == null)
        {
            Debug.LogError($"GetModelInfo endCircle == null ");
            //IsGetInfoSuccess = false;
            //CreateLocalPoint(startPlane.Point.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Point.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
            return;
        }
        endPoint = endCircle.Center;

        var PipeRadius1 = startCircle.Radius;
        var PipeRadius2 = endCircle.Radius;

        var EndPoints = new List<Vector3>() { startPoint, endPoint };

        //CreateLocalPoint(startPoint, $"StartPoint1_{startCircle.Radius}_{startCircle.Points.Count}", planInfoRoot.transform);
        //CreateLocalPoint(endPoint, $"EndPoint1_{endCircle.Radius}_{endCircle.Points.Count}", planInfoRoot.transform);

        var PipeRadius = 0f;
        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }

        var PipeLength = Vector3.Distance(startPoint, endPoint);
        PipeLineData LineInfo = new PipeLineData();
        LineInfo.StartPoint = startPoint;
        LineInfo.EndPoint = endPoint;

        //Vector4 ModelStartPoint = startPoint;
        //ModelStartPoint.w = PipeRadius;
        //Vector4 ModelEndPoint = endPoint;
        //ModelEndPoint.w = PipeRadius;

        Debug.Log($"PipeLineInfoJob[{Id}] time:{(DateTime.Now-start).TotalMilliseconds}");
    }

    private static VerticesToPlaneInfo GetEndPlane(VerticesToPlaneInfo startPlane, List<VerticesToPlaneInfo> verticesToPlaneInfos_All)
    {
        VerticesToPlaneInfo endPlane = null;
        if (startPlane == verticesToPlaneInfos_All[0])
        {
            endPlane = verticesToPlaneInfos_All[1];
        }
        if (startPlane == verticesToPlaneInfos_All[1])
        {
            endPlane = verticesToPlaneInfos_All[0];
        }
        if (startPlane == verticesToPlaneInfos_All[2])
        {
            endPlane = verticesToPlaneInfos_All[3];
        }
        if (startPlane == verticesToPlaneInfos_All[3])
        {
            endPlane = verticesToPlaneInfos_All[2];
        }
        if (startPlane == verticesToPlaneInfos_All[4])
        {
            endPlane = verticesToPlaneInfos_All[5];
        }
        if (startPlane == verticesToPlaneInfos_All[5])
        {
            endPlane = verticesToPlaneInfos_All[4];
        }
        return endPlane;
    }
}

public struct PipeLineData
{
    public Vector4 StartPoint;
    public Vector4 EndPoint;

    public Vector3 Direction;
}

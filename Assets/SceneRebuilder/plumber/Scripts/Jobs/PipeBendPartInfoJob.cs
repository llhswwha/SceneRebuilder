using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeBendPartInfoJob : IMeshInfoJob
{
    public int id;

    public NativeArray<Vector3> points;

    public PipeLineData lineData;

    public static NativeArray<PipeLineData> Result;

    public static NativeList<int> ErrorIds;

    public static float planeClosedMinDis = 0.001f;
    public static int planeClosedMaxCount1 = 20;
    public static int planeClosedMaxCount2 = 100;

    public void Dispose()
    {

    }

    public void Execute()
    {
        lineData = new PipeLineData();

        Vector3[] vs = points.ToArray();
        OrientedBoundingBox obb = OrientedBoundingBox.Create(vs, false, this.id.ToString());

        //GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        //planInfoRoot.AddComponent<DebugInfoRoot>();
        //planInfoRoot.transform.SetParent(tParent);
        //planInfoRoot.transform.localPosition = Vector3.zero;

        Vector3 ObbExtent = obb.Extent;

        Vector4 startPoint = obb.Up * ObbExtent.y;
        Vector4 endPoint = -obb.Up * ObbExtent.y;

        //2.Planes
        PlaneInfo[] planeInfos = obb.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        var verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false, planeClosedMinDis, planeClosedMaxCount1, planeClosedMaxCount2);
            v2p.SplitToTwoPlane();
            verticesToPlaneInfos_All.Add(v2p);
            //oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);

            if (v2p.Plane1Points.Count != vs.Length / 2)
            {
                continue;
            }

            //if (isShowDebugObj)
            //{
            //    plane.ShowPlaneInfo(i, planInfoRoot, v2p, lineSize, tParent);
            //}
            verticesToPlaneInfos.Add(v2p);
            //var isC = v2p.IsCircle();
        }
        verticesToPlaneInfos.Sort();

        if (verticesToPlaneInfos.Count < 1)
        {
            lineData.IsGetInfoSuccess = false;
            Debug.LogError($"PipeLine.ShowLinePartModelInfo IsCircle Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.id}");
            return ;
        }

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >= 2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"PipeLine.ShowLinePartModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{this.id}");
            endPlane = VerticesToPlaneInfo.GetEndPlane(startPlane, verticesToPlaneInfos_All);
        }


        var P1 = obb.Right * ObbExtent.x;
        var P2 = -obb.Forward * ObbExtent.z;
        var P3 = -obb.Right * ObbExtent.x;
        var P4 = obb.Forward * ObbExtent.z;
        var P5 = obb.Up * ObbExtent.y;
        var P6 = -obb.Up * ObbExtent.y;
        var Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        CircleInfo startCircle = startPlane.GetPlane1Circle();
        if (startCircle == null)
        {
            Debug.LogError($"PipeLine.ShowLinePartModelInfo startCircle == null gameObject:{this.id} count:{verticesToPlaneInfos.Count},gameObject:{this.id}");
            lineData.IsGetInfoSuccess = false;

            //CreateLocalPoint(startPlane.Plane.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Plane.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return;
        }
        startPoint = startCircle.GetCenter4();
        CircleInfo endCircle = endPlane.GetPlane1Circle();
        if (endCircle == null)
        {
            Debug.LogError($"PipeLine.ShowLinePartModelInfo endCircle == null gameObject:{this.id} count:{verticesToPlaneInfos.Count},gameObject:{this.id}");
            lineData.IsGetInfoSuccess = false;
            //CreateLocalPoint(startPlane.Plane.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Plane.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
            return;
        }
        endPoint = endCircle.GetCenter4();

        var PipeRadius1 = startCircle.Radius;
        var PipeRadius2 = endCircle.Radius;

        var EndPoints = new List<Vector3>() { startPoint, endPoint };

        //CreateLocalPoint(startPoint, $"StartPoint1_{startCircle.Radius}_{startCircle.Points.Count}", planInfoRoot.transform);
        //CreateLocalPoint(endPoint, $"EndPoint1_{endCircle.Radius}_{endCircle.Points.Count}", planInfoRoot.transform);

        float PipeRadius = 0;
        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }

        //var PipeLength = Vector3.Distance(startPoint, endPoint);
        ////LineInfo.StartPoint = startPoint;
        ////LineInfo.EndPoint = endPoint;
        //var LineInfo = new PipeLineInfo(startPoint, endPoint, startCircle.GetNormal(), endCircle.GetNormal(), null);

        //var ModelStartPoint = startPoint;
        //ModelStartPoint.w = PipeRadius;
        //var ModelEndPoint = endPoint;
        //ModelEndPoint.w = PipeRadius;

        //PipeLength = Vector3.Distance(startPoint, endPoint);
        //var KeyPointCount = 2;

        lineData.StartPoint = startPoint;
        lineData.StartPoint.w = PipeRadius;
        lineData.EndPoint = endPoint;
        lineData.EndPoint.w = PipeRadius;
        lineData.IsGetInfoSuccess = true;

        if (Result.Length > id)
        {
            Result[id] = lineData;
        }
        else
        {
            Debug.LogWarning($"PipeBendPartInfoJob[{id}] Result.Length :{Result.Length }");
        }
    }
}

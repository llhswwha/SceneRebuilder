using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeBendPartInfoJob : IMeshInfoJob
{
    public int id;

    public NativeArray<MeshTriangle> meshTriangles;

    public PipeLineData lineData;

    public static NativeArray<PipeLineData> Result;

    public static NativeList<int> ErrorIds;

    public static float planeClosedMinDis = 0.001f;
    public static int planeClosedMaxCount1 = 20;
    public static int planeClosedMaxCount2 = 100;

    //public static string logTag = "";

    public void Dispose()
    {
        meshTriangles.Dispose();
    }

    public void Execute()
    {
        lineData = StaticExecuteByTriangles(meshTriangles.ToArray(), id, false);

        if (Result.Length > id)
        {
            Result[id] = lineData;
        }
        else
        {
            Debug.LogWarning($"PipeBendPartInfoJob[{id}] Result.Length :{Result.Length }");
        }
    }

    public static PipeLineData StaticExecuteByTriangles(MeshTriangle[] triangles, int id, bool isShowError)
    {
        Vector3[] vs = MeshTriangle.GetTrianglePoints(triangles);
        PipeLineData data=StaticExecuteByPoints(vs, id, isShowError);
        if (data.IsGetInfoSuccess == false)
        {
            //Debug.LogWarning($"StaticExecuteByTriangles[{id}] StaticExecuteByPoints.IsGetInfoSuccess == false");
            List<List<Vector3>> pointLines = GetLinePoints(triangles, 0);
            if (pointLines.Count == 2)
            {
                data = new PipeLineData(pointLines);
            }
            else
            {
                Debug.LogError($"StaticExecuteByTriangles[{id}] pointLines.Count != 2 pointLines:{pointLines.Count}");
            }
        }
        return data;
    }

    public static List<List<Vector3>> GetLinePoints(MeshTriangle[] triangles0, int minCount)
    {
        MeshPointCountDictList lineCountList = new MeshPointCountDictList();
        List<Key2List<Vector3, MeshTriangle>> sharedPoints2 = MeshTriangle.FindSharedPointsByPoint(triangles0);
        int id = 0;
        for (int i = 0; i < sharedPoints2.Count; i++)
        {
            Vector3 point = sharedPoints2[i].Key;
            List<MeshTriangle> triangles = sharedPoints2[i].List;
            if (triangles.Count < minCount) continue;
            id++;

            MeshPointCountDict pointCount = new MeshPointCountDict(point);
            lineCountList.Add(pointCount);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                List<MeshPoint> mps = t.GetPoints();
                foreach (MeshPoint mp in mps)
                {
                    pointCount.AddPoint(mp);
                }
            }
        }
        List<List<Vector3>> pointLines = lineCountList.GetLines();
        //GameObject linesObj = CreateSubTestObj($"PointLines:{pointLines.Count}", root);
        //for (int i = 0; i < pointLines.Count; i++)
        //{
        //    List<Vector3> ps = pointLines[i];
        //    GameObject lineObj = CreateSubTestObj($"PointLine[{i}]:{ps.Count}", linesObj.transform);
        //    for (int j = 0; j < ps.Count; j++)
        //    {
        //        Vector3 p = ps[j];
        //        TransformHelper.ShowLocalPoint(p, pointScale * 2, root, lineObj.transform);
        //    }
        //}
        return pointLines;
    }

    public static PipeLineData StaticExecuteByPoints(Vector3[] vs,int id,bool isShowError)
    {
        PipeLineData lineData = new PipeLineData();

        //Vector3[] vs = points.ToArray();
        OrientedBoundingBox? obb1= OrientedBoundingBox.Create(vs, true, id.ToString());
        if (obb1 == null)
        {
            lineData.IsGetInfoSuccess = false;
            //Debug.LogWarning($"PipeBendPartInfoJob.StaticExecute[{logTag}] OrientedBoundingBox==null id:{id} vs:{vs.Length}");
            return lineData;
        }
        OrientedBoundingBox obb = (OrientedBoundingBox)obb1;

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
        List<VerticesToPlaneInfo> verticesToPlaneInfos = new List<VerticesToPlaneInfo>();

        float percent = 0;
        float j = 5;
        for (; j < 10; j++)
        {
            percent = 0.1f * j;

            verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
            for (int i = 0; i < planeInfos.Length; i++)
            {
                PlaneInfo plane = (PlaneInfo)planeInfos[i];
                //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
                VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false, planeClosedMinDis, planeClosedMaxCount1, planeClosedMaxCount2);
                v2p.SplitToTwoPlane(percent);
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
            }
            verticesToPlaneInfos.Sort();

            if (verticesToPlaneInfos.Count == 2)
            {
                break;
            }
            if (verticesToPlaneInfos.Count == 1)
            {
                break;
            }
        }

        //Debug.Log($"PipeBendPartInfoJob.StaticExecute IsCircle count:{verticesToPlaneInfos.Count},gameObject:{id} percent:{percent}");

        if (verticesToPlaneInfos.Count < 1)
        {
            lineData.IsGetInfoSuccess = false;
            Debug.LogError($"PipeBendPartInfoJob.StaticExecute[{LogTag.logTag}] IsCircle Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{id} percent:{percent}");
            return lineData;
        }

        if (verticesToPlaneInfos.Count == 1)
        {
            VerticesToPlaneInfo startPlane1 = verticesToPlaneInfos[0];
            VerticesToPlaneInfo endPlane1 = VerticesToPlaneInfo.GetEndPlane(startPlane1, verticesToPlaneInfos_All);
            verticesToPlaneInfos.Add(endPlane1);

            if (endPlane1 == null)
            {
                if (isShowError)
                {
                    Debug.LogError($"PipeBendPartInfoJob.StaticExecute[{LogTag.logTag}] endPlane1 == null count:{verticesToPlaneInfos.Count},gameObject:{id} percent:{percent} startPlane1:{startPlane1}");
                }
                lineData.IsGetInfoSuccess = false;
                return lineData;
            }
            if(endPlane1.Plane1Points==null)
            {
                Debug.LogError($"PipeBendPartInfoJob.StaticExecute[{LogTag.logTag}] endPlane1.Plane1Points==null count:{verticesToPlaneInfos.Count},gameObject:{id} percent:{percent} startPlane1:{startPlane1}");
                lineData.IsGetInfoSuccess = false;
                return lineData;
            } 

            for (; j < 10; j++)
            {
                if (endPlane1.Plane1Points.Count != vs.Length / 2)
                {
                    percent = 0.1f * j;
                    endPlane1.SplitToTwoPlane(percent);
                }
                else
                {
                    break;
                }
            }
        }

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >= 2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"PipeLine.ShowLinePartModelInfo[{LogTag.logTag}] verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{id}");
            endPlane = VerticesToPlaneInfo.GetEndPlane(startPlane, verticesToPlaneInfos_All);
            ErrorIds.Add(id);
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
            Debug.LogError($"PipeLine.ShowLinePartModelInfo[{LogTag.logTag}] startCircle == null gameObject:{id} count:{verticesToPlaneInfos.Count},gameObject:{id}");
            lineData.IsGetInfoSuccess = false;

            //CreateLocalPoint(startPlane.Plane.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Plane.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return lineData;
        }
        startPoint = startCircle.GetCenter4();
        CircleInfo endCircle = endPlane.GetPlane1Circle();
        if (endCircle == null)
        {
            Debug.LogError($"PipeLine.ShowLinePartModelInfo[{LogTag.logTag}] endCircle == null gameObject:{id} count:{verticesToPlaneInfos.Count},gameObject:{id}");
            lineData.IsGetInfoSuccess = false;
            //CreateLocalPoint(startPlane.Plane.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            //CreateLocalPoint(endPlane.Plane.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
            return lineData;
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

        var PipeLength = Vector3.Distance(startPoint, endPoint);
        //LineInfo.StartPoint = startPoint;
        //LineInfo.EndPoint = endPoint;

        startPoint.w = PipeRadius;
        endPoint.w = PipeRadius;
        lineData = new PipeLineData(startPoint, endPoint, startCircle.GetNormal(), endCircle.GetNormal());

        var ModelStartPoint = startPoint;
        ModelStartPoint.w = PipeRadius;
        var ModelEndPoint = endPoint;
        ModelEndPoint.w = PipeRadius;

        PipeLength = Vector3.Distance(startPoint, endPoint);
        //var KeyPointCount = 2;

        //lineData.StartPoint = startPoint;
        //lineData.StartPoint.w = PipeRadius;
        //lineData.EndPoint = endPoint;
        //lineData.EndPoint.w = PipeRadius;
        lineData.IsGetInfoSuccess = true;
        return lineData;
    }
}

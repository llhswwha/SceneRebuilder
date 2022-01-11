using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static PipeElbowModel;

public struct PipeElbowInfoJob : IPipeJob
{
    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.00005f;
    public static float PipeLineOffset = 0.05f;

    public int id;
    public MeshStructure mesh;

    public static NativeArray<PipeElbowData> Result;

    public void Execute()
    {
        DateTime start = DateTime.Now;

        var VertexCount = mesh.vertices.Length;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"PipeElbowInfoJob[{id}] time1:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms meshTriangles:{meshTriangles.Count}");
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByPointEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        //Debug.Log($"PipeElbowInfoJob[{id}] time2:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms trianglesList:{trianglesList.Count}");

        if (trianglesList.Count == 3)
        {
            trianglesList.CombineSameCenter(0.002f);
        }

        PipeElbowData data = new PipeElbowData();

        if (trianglesList.Count == 6)
        {
            data = GetElbow6(trianglesList, mesh);
        }
        else if (trianglesList.Count == 4)
        {
            data = new PipeElbowData();
            data.KeyPointInfo = GetElbow4(trianglesList);
            //ModelStartPoint = KeyPointInfo.EndPointOut1;
            //ModelEndPoint = KeyPointInfo.EndPointOut2;
            data.IsGetInfoSuccess = true;
        }
        else if (trianglesList.Count == 2)
        {
            data = new PipeElbowData();
            data.KeyPointInfo = GetElbow2(trianglesList, mesh);
            //ModelStartPoint = KeyPointInfo.EndPointOut1;
            //ModelEndPoint = KeyPointInfo.EndPointOut2;
            data.IsGetInfoSuccess = true;
        }
        else
        {
            data = new PipeElbowData();
            data.IsGetInfoSuccess = false;
            Debug.LogError($">>>PipeElbowInfoJob[{id}] GetModelInfo points.Count Error count:{trianglesList.Count} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
        }

        if (Result.Length > id)
        {
            Result[id] = data;
        }
        else
        {
            Debug.LogWarning($"PipeElbowInfoJob[{id}] Result.Length :{Result.Length }");
        }

        Debug.Log($"PipeElbowInfoJob[{id}] time:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms VertexCount:{VertexCount} meshTriangles:{meshTriangles.Count} trianglesList:{trianglesList.Count}");

    }

    private PipeElbowData GetElbow6(SharedMeshTrianglesList list, MeshStructure mesh)
    {
        PipeElbowData data = new PipeElbowData();

        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);
        SharedMeshTriangles plane1 = trianglesList[0];
        SharedMeshTriangles plane2 = trianglesList[1];
        SharedMeshTriangles plane3 = trianglesList[2];
        SharedMeshTriangles plane4 = trianglesList[3];
        SharedMeshTriangles plane5 = trianglesList[4];
        SharedMeshTriangles plane6 = trianglesList[5];
        data.IsSpecial = true;

        SharedMeshTrianglesList planes4 = new SharedMeshTrianglesList();
        planes4.Add(plane1);
        planes4.Add(plane2);
        planes4.Add(plane3);
        planes4.Add(plane4);
        data.KeyPointInfo = GetElbow4(planes4);
        var ModelStartPoint = data.KeyPointInfo.EndPointOut1;
        var ModelEndPoint = data.KeyPointInfo.EndPointOut2;

        SharedMeshTrianglesList planes2 = new SharedMeshTrianglesList();
        planes2.Add(plane5);
        planes2.Add(plane6);

        data.InnerKeyPointInfo = GetElbow2(planes2, mesh);

        return data;
    }

    private PipeElbowKeyPointData GetElbow2(SharedMeshTrianglesList list, MeshStructure mesh)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);
        var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
        var distanceList = new List<PlanePointDistance>();
        foreach (var p in trianglesList)
        {
            distanceList.Add(new PlanePointDistance(p, centerOfPoints));
        }
        distanceList.Sort();

        SharedMeshTriangles startPlane = distanceList[0].Plane;
        SharedMeshTriangles endPlane = distanceList[1].Plane;

        var endPoint1 = startPlane.GetCenter();
        var endPoint2 = endPlane.GetCenter();

        var normal1 = mesh.normals[startPlane.PointId];
        var normal2 = mesh.normals[endPlane.PointId];
        trianglesList.Remove(endPoint1);
        trianglesList.Remove(endPoint2);

        //GetPipeRadius();

        var PipeRadius1 = startPlane.GetRadius();
        var PipeRadius2 = endPlane.GetRadius();
        var PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        Vector3 crossPoint1;
        Vector3 crossPoint2;
        Math3D.ClosestPointsOnTwoLines(out crossPoint1, out crossPoint2, endPoint1, normal1, endPoint2, normal2);
        Vector3 crossPoint12 = 0.5f * (crossPoint1 + crossPoint2);

        PipeElbowKeyPointData info = new PipeElbowKeyPointData();
        //EndPointOut1 = EndPointIn1- normal1 * PipeRadius* PipeLineOffset;
        //EndPointOut2 = EndPointIn2+ normal2 * PipeRadius * PipeLineOffset;
        info.EndPointOut1 = endPoint1;
        info.EndPointOut2 = endPoint2;
        info.EndPointIn1 = endPoint1 + (crossPoint1 - endPoint1).normalized * PipeRadius * PipeLineOffset;
        info.EndPointIn2 = endPoint2 + (crossPoint2 - endPoint2).normalized * PipeRadius * PipeLineOffset;
        info.EndPointIn1.w = PipeRadius;
        info.EndPointIn2.w = PipeRadius;
        info.EndPointOut1.w = PipeRadius;
        info.EndPointOut2.w = PipeRadius;

        info.Line1 = new PipeLineData(info.EndPointOut1, info.EndPointIn1, normal1);
        info.Line2 = new PipeLineData(info.EndPointIn2, info.EndPointOut2, normal2);

        //TransformHelper.ShowLocalPoint(info.EndPointOut1, PointScale, this.transform, null).name = "Elbow2_OutPoint1";
        //TransformHelper.ShowLocalPoint(info.EndPointOut2, PointScale, this.transform, null).name = "Elbow2_OutPoint2";
        //TransformHelper.ShowLocalPoint(info.EndPointIn1, PointScale, this.transform, null).name = "Elbow2_InPoint1";
        //TransformHelper.ShowLocalPoint(info.EndPointIn2, PointScale, this.transform, null).name = "Elbow2_InPoint2";
        //TransformHelper.ShowLocalPoint(crossPoint1, PointScale, this.transform, null).name = "Elbow2_crossPoint1";
        //TransformHelper.ShowLocalPoint(crossPoint2, PointScale, this.transform, null).name = "Elbow2_crossPoint2";
        //TransformHelper.ShowLocalPoint(crossPoint12, PointScale, this.transform, null).name = "Elbow2_crossPoint12";

        //ModelStartPoint = info.EndPointOut1;
        //ModelEndPoint = info.EndPointOut2;

        //IsGetInfoSuccess = true;

        return info;
    }

    //private PipeElbowKeyPlaneInfo GetElbow4Planes(SharedMeshTrianglesList list)
    //{
    //    SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);

    //    PipeElbowKeyPlaneInfo info = new PipeElbowKeyPlaneInfo();

    //    var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
    //    var distanceList = new List<PlanePointDistance>();
    //    foreach (var p in trianglesList)
    //    {
    //        distanceList.Add(new PlanePointDistance(p, centerOfPoints));
    //    }
    //    distanceList.Sort();
    //    SharedMeshTriangles endPointIn1Plane = distanceList[0].Plane;
    //    SharedMeshTriangles endPointIn2Plane = distanceList[1].Plane;

    //    info.EndPointIn1 = endPointIn1Plane;
    //    info.EndPointIn2 = endPointIn2Plane;
    //    trianglesList.Remove(info.EndPointIn1);
    //    trianglesList.Remove(info.EndPointIn2);

    //    SharedMeshTriangles endPointOut1Plane = MeshHelper.FindClosedPlane(info.EndPointIn1.GetCenter4(), trianglesList);
    //    info.EndPointOut1 = endPointOut1Plane;
    //    trianglesList.Remove(info.EndPointOut1);
    //    SharedMeshTriangles endPointOut2Plane = MeshHelper.FindClosedPlane(info.EndPointIn2.GetCenter4(), trianglesList);
    //    info.EndPointOut2 = endPointOut2Plane;
    //    trianglesList.Remove(info.EndPointOut2);
    //    return info;
    //}

    private PipeElbowKeyPointData GetElbow4(SharedMeshTrianglesList list)
    {
        PipeElbowKeyPlaneInfo keyPlanes = PipeElbowKeyPlaneInfo.GetElbow4Planes(list);
        var info2 = keyPlanes.GetKeyPointsData();
        //ShowKeyPoints(info2, "Elbow4_");
        return info2;
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}

public struct PipeElbowData
{

    public PipeElbowKeyPointData KeyPointInfo;

    public PipeElbowKeyPointData InnerKeyPointInfo;

    public bool IsSpecial;

    public bool IsGetInfoSuccess;
}

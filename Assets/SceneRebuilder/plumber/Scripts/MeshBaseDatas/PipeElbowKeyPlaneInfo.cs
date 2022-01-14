using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PipeElbowKeyPlaneInfo
{
    public SharedMeshTriangles EndPointIn1;
    public SharedMeshTriangles EndPointOut1;
    public SharedMeshTriangles EndPointIn2;
    public SharedMeshTriangles EndPointOut2;

    public override string ToString()
    {
        return $"[Int1:{EndPointIn1} Out1:{EndPointOut1} In2:{EndPointIn2} Out2:{EndPointOut2}]";
    }

    internal PipeElbowKeyPointInfo GetKeyPoints()
    {
        var out1 = EndPointOut1.GetCenter4();
        var in1 = EndPointIn1.GetCenter4();
        var out2 = EndPointOut2.GetCenter4();
        var in2 = EndPointIn2.GetCenter4();
        PipeElbowKeyPointInfo points = new PipeElbowKeyPointInfo(out1, in1,
            out2, in2);
        return points;
    }

    internal PipeElbowKeyPointData GetKeyPointsData()
    {
        var out1 = EndPointOut1.GetCenter4();
        var in1 = EndPointIn1.GetCenter4();
        var out2 = EndPointOut2.GetCenter4();
        var in2 = EndPointIn2.GetCenter4();
        PipeElbowKeyPointData points = new PipeElbowKeyPointData(out1, in1,
            out2, in2);
        return points;
    }

    //public PipeElbowKeyPlaneInfo()
    //{
    //    EndPointIn1 = new SharedMeshTriangles();
    //    EndPointOut1 = new SharedMeshTriangles();
    //    EndPointIn2 = new SharedMeshTriangles();
    //    EndPointOut2 = new SharedMeshTriangles();
    //}

    public PipeElbowKeyPlaneInfo(SharedMeshTriangles out1, SharedMeshTriangles in1, SharedMeshTriangles out2, SharedMeshTriangles in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;
    }

    public PipeElbowKeyPlaneInfo(PipeElbowKeyPlaneData data)
    {
        EndPointOut1 = new SharedMeshTriangles(data.EndPointOut1);
        EndPointIn1 = new SharedMeshTriangles(data.EndPointIn1);
        EndPointOut2 = new SharedMeshTriangles(data.EndPointOut2);
        EndPointIn2 = new SharedMeshTriangles(data.EndPointIn2);
    }

    public static PipeElbowKeyPlaneInfo GetElbow4Planes(SharedMeshTrianglesList list)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);

        PipeElbowKeyPlaneInfo info = new PipeElbowKeyPlaneInfo();

        var distanceList = trianglesList.GetPlanePointDistanceList();

        SharedMeshTriangles endPointIn1Plane = distanceList[0].Plane;
        SharedMeshTriangles endPointIn2Plane = distanceList[1].Plane;

        info.EndPointIn1 = endPointIn1Plane;
        info.EndPointIn2 = endPointIn2Plane;
        trianglesList.Remove(info.EndPointIn1);
        trianglesList.Remove(info.EndPointIn2);

        SharedMeshTriangles endPointOut1Plane = MeshHelper.FindClosedPlane(info.EndPointIn1.GetCenter4(), trianglesList);
        info.EndPointOut1 = endPointOut1Plane;
        trianglesList.Remove(info.EndPointOut1);
        SharedMeshTriangles endPointOut2Plane = MeshHelper.FindClosedPlane(info.EndPointIn2.GetCenter4(), trianglesList);
        info.EndPointOut2 = endPointOut2Plane;
        trianglesList.Remove(info.EndPointOut2);
        return info;
    }
}

public struct PipeElbowKeyPlaneData
{
    public SharedMeshTrianglesData EndPointIn1;
    public SharedMeshTrianglesData EndPointOut1;
    public SharedMeshTrianglesData EndPointIn2;
    public SharedMeshTrianglesData EndPointOut2;

    public override string ToString()
    {
        return $"[Int1:{EndPointIn1} Out1:{EndPointOut1} In2:{EndPointIn2} Out2:{EndPointOut2}]";
    }

    public PipeElbowKeyPlaneData(PipeElbowKeyPlaneInfo info)
    {
        EndPointOut1 = new SharedMeshTrianglesData(info.EndPointOut1);
        EndPointIn1 = new SharedMeshTrianglesData(info.EndPointIn1);
        EndPointOut2 = new SharedMeshTrianglesData(info.EndPointOut2);
        EndPointIn2 = new SharedMeshTrianglesData(info.EndPointIn2);
    }

    public PipeElbowKeyPlaneData(SharedMeshTriangles out1, SharedMeshTriangles in1, SharedMeshTriangles out2, SharedMeshTriangles in2)
    {
        EndPointOut1 = new SharedMeshTrianglesData(out1);
        EndPointIn1 = new SharedMeshTrianglesData(in1);
        EndPointOut2 = new SharedMeshTrianglesData(out2);
        EndPointIn2 = new SharedMeshTrianglesData(in2);
    }

    internal PipeElbowKeyPointData GetKeyPointsData()
    {
        var out1 = EndPointOut1.GetCenter4();
        var in1 = EndPointIn1.GetCenter4();
        var out2 = EndPointOut2.GetCenter4();
        var in2 = EndPointIn2.GetCenter4();
        PipeElbowKeyPointData points = new PipeElbowKeyPointData(out1, in1,
            out2, in2);
        return points;
    }
}

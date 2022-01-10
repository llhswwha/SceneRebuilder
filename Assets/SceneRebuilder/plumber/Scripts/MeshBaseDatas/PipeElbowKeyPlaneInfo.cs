using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeElbowKeyPlaneInfo
{
    public SharedMeshTriangles EndPointIn1;
    public SharedMeshTriangles EndPointOut1;
    public SharedMeshTriangles EndPointIn2;
    public SharedMeshTriangles EndPointOut2;

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

    public PipeElbowKeyPlaneInfo()
    {

    }

    public PipeElbowKeyPlaneInfo(SharedMeshTriangles out1, SharedMeshTriangles in1, SharedMeshTriangles out2, SharedMeshTriangles in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;
    }
}

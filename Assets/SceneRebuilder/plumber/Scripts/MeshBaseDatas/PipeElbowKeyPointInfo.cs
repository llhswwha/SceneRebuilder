using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeElbowKeyPointInfo
{
    public Vector4 EndPointIn1 = Vector3.zero;
    public Vector4 EndPointOut1 = Vector3.zero;
    public Vector4 EndPointIn2 = Vector3.zero;
    public Vector4 EndPointOut2 = Vector3.zero;

    public PipeLineInfo Line1 = new PipeLineInfo();

    public PipeLineInfo Line2 = new PipeLineInfo();

    public PipeElbowKeyPointInfo()
    {

    }

    public PipeElbowKeyPointInfo(Vector4 out1, Vector4 in1, Vector4 out2, Vector4 in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;

        Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
        Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);
    }

    public PipeElbowKeyPointInfo(PipeElbowKeyPointData data)
    {
        EndPointOut1 = data.EndPointOut1;
        EndPointIn1 = data.EndPointIn1;
        EndPointOut2 = data.EndPointOut2;
        EndPointIn2 = data.EndPointIn2; ;

        Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
        Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);
    }
}

public struct PipeElbowKeyPointData
{
    public Vector4 EndPointIn1;
    public Vector4 EndPointOut1;
    public Vector4 EndPointIn2;
    public Vector4 EndPointOut2;

    public PipeLineData Line1;

    public PipeLineData Line2;

    public PipeElbowKeyPointData(Vector4 out1, Vector4 in1, Vector4 out2, Vector4 in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;

        Line1 = new PipeLineData();

        Line2 = new PipeLineData();
    }
}

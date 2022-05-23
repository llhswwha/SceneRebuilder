using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeModelKeyPointInfo4
{
    public Vector4 EndPointIn1 = Vector3.zero;
    public Vector4 EndPointOut1 = Vector3.zero;
    public Vector4 EndPointIn2 = Vector3.zero;
    public Vector4 EndPointOut2 = Vector3.zero;


    //public float GetPipeRadius1()
    //{
    //    return (EndPointIn1.w + EndPointOut1.w) / 2;
    //}

    //public float GetPipeRadius2()
    //{
    //    return (EndPointIn2.w + EndPointOut2.w) / 2;
    //}

    public float GetLengthOut()
    {
        return Vector3.Distance(EndPointOut1, EndPointOut2);
    }

    public float GetLengthIn()
    {
        return Vector3.Distance(EndPointIn1, EndPointIn2);
    }

    public float GetLength1()
    {
        return Vector3.Distance(EndPointOut1, EndPointIn1);
    }

    public float GetLength2()
    {
        return Vector3.Distance(EndPointOut2, EndPointIn2);
    }

    public float GetRadiusOut()
    {
        return (EndPointOut1.w + EndPointOut2.w) / 2;
    }

    public float GetRadiusIn1Out1()
    {
        return (EndPointIn1.w + EndPointOut1.w) / 2;
    }
    public float GetRadiusIn2Out2()
    {
        return (EndPointIn2.w + EndPointOut2.w) / 2;
    }

    //public PipeLineInfo Line1 = new PipeLineInfo();

    //public PipeLineInfo Line2 = new PipeLineInfo();

    public PipeLineInfo GetLine1()
    {
        return new PipeLineInfo(EndPointOut1, EndPointIn1, null);
    }

    public PipeLineInfo GetLine2()
    {
        return new PipeLineInfo(EndPointIn2, EndPointOut2, null); 
    }

    public PipeModelKeyPointInfo4()
    {

    }

    public PipeModelKeyPointInfo4(Vector4 out1, Vector4 in1, Vector4 out2, Vector4 in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;

        //Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
        //Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);
    }

    public PipeModelKeyPointInfo4(PipeModelKeyPointData4 data)
    {
        EndPointOut1 = data.EndPointOut1;
        EndPointIn1 = data.EndPointIn1;
        EndPointOut2 = data.EndPointOut2;
        EndPointIn2 = data.EndPointIn2; ;

        //Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
        //Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);
    }
}

public struct PipeModelKeyPointData4
{
    public Vector4 EndPointIn1;
    public Vector4 EndPointOut1;
    public Vector4 EndPointIn2;
    public Vector4 EndPointOut2;

    //public PipeLineData Line1;

    //public PipeLineData Line2;

    public PipeModelKeyPointData4(Vector4 out1, Vector4 in1, Vector4 out2, Vector4 in2)
    {
        EndPointOut1 = out1;
        EndPointIn1 = in1;
        EndPointOut2 = out2;
        EndPointIn2 = in2;

        //Line1 = new PipeLineData();

        //Line2 = new PipeLineData();
    }
    public PipeModelKeyPointData4(PipeModelKeyPointInfo4 data)
    {
        if (data != null)
        {
            EndPointOut1 = data.EndPointOut1;
            EndPointIn1 = data.EndPointIn1;
            EndPointOut2 = data.EndPointOut2;
            EndPointIn2 = data.EndPointIn2;
        }
        else
        {
            EndPointOut1 = Vector4.zero;
            EndPointIn1 = Vector4.zero;
            EndPointOut2 = Vector4.zero;
            EndPointIn2 = Vector4.zero;
        }


        //Line1 = new PipeLineData();
        //Line2 = new PipeLineData();

        //Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
        //Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);
    }

    public override string ToString()
    {
        return $"[({EndPointIn1.x.ToString("F3")},{EndPointIn1.y.ToString("F3")},{EndPointIn1.z.ToString("F3")},{EndPointIn1.w.ToString("F3")})_{EndPointOut1}_{EndPointIn2}_{EndPointOut2}]";
    }
}

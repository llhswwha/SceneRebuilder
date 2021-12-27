using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeCreateArgs
{
    public List<PipeCreateArg> PipeArgs = new List<PipeCreateArg>();
    public float elbowRadius_Sum = 0;
    public float elbowRadius = 0;

    private List<PipeLineModel> PipeModels;

    public PipeCreateArgs()
    {

    }

    public PipeCreateArgs(List<PipeLineModel> PipeModels)
    {
        this.PipeModels = PipeModels;
    }

    public void AddArg(PipeCreateArg arg)
    {
        this.PipeArgs.Add(arg);
        elbowRadius_Sum += arg.elbowRadius;

    }

    internal List<Vector3> GetPoints(bool useOnlyEndPoint)
    {
        List<Vector3> points = new List<Vector3>();
        PipeArgs = new List<PipeCreateArg>();

        if (PipeModels.Count == 1)
        {
            PipeLineModel pm1 = PipeModels[0];
            points.Add(pm1.GetStartPoint());
            points.Add(pm1.GetEndPoint());
        }
        else
        {
            for (int i = 0; i < PipeModels.Count - 1; i++)
            {
                PipeLineModel pm1 = PipeModels[i];
                PipeLineModel pm2 = PipeModels[i + 1];
                PipeCreateArg pipeArg = new PipeCreateArg(pm1.LineInfo, pm2.LineInfo);
                //var ps = pipeArg.GetPoints();
                //if (useOnlyEndPoint)
                //{
                //    if (i == 0)
                //    {
                //        points.Add(ps[0]);
                //        points.Add(ps[2]);
                //    }
                //    else if (i == PipeModels.Count - 2)
                //    {
                //        points.Add(ps[2]);
                //        points.Add(ps[4]);
                //    }
                //    else
                //    {
                //        points.Add(ps[2]);
                //    }
                //}
                //else
                //{
                //    if (i == 0)
                //    {
                //        points.AddRange(ps);
                //    }
                //    //else if (i == PipeModels.Count - 2)
                //    //{
                //    //    points.Add(ps[2]);
                //    //    points.Add(ps[3]);
                //    //    points.Add(ps[4]);
                //    //}
                //    else
                //    {
                //        points.Add(ps[2]);
                //        points.Add(ps[3]);
                //        points.Add(ps[4]);
                //    }
                //}
                var ps = pipeArg.GetGeneratePoints(i, PipeModels.Count, useOnlyEndPoint);
                points.AddRange(ps);
                AddArg(pipeArg);
            }
            elbowRadius = elbowRadius_Sum / PipeArgs.Count;
        }
        return points;
    }

    public Vector3 sumP = Vector3.zero;
    public Vector3 centerP = Vector3.zero;

    public List<Vector3> GetLocalPoints(bool useOnlyEndPoint)
    {
        List<Vector3> points = GetPoints(useOnlyEndPoint);
        sumP = Vector3.zero;
        for (int i = 0; i < points.Count; i++)
        {
            sumP += points[i];
        }

        centerP = sumP / (points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            points[i] = p - centerP;
        }

        return points;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFlangeModel : PipeReducerModel
{
    private float defaultMinRepeatPointDistance=0.0002f;
    public override void GetModelInfo()
    {
        if (minRepeatPointDistance < defaultMinRepeatPointDistance)
        {
            minRepeatPointDistance = defaultMinRepeatPointDistance;
        }
        base.GetModelInfo();
        if(PipeRadius1> PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }
        StartPoint.w = PipeRadius;
        EndPoint.w = PipeRadius;

        ModelStartPoint = StartPoint;
        ModelEndPoint = EndPoint;
    }
    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return base.RendererModel(arg, afterName);
    }
}

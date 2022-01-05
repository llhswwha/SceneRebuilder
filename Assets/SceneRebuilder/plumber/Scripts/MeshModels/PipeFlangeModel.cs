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

    public override int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    {
        int cCount = base.ConnectedModel(model2, minPointDis, isShowLog, isUniformRaidus, minRadiusDis);

        PipeModelBase model1 = this;
        var keyPoints = model2.GetModelKeyPoints();
        foreach(var p in keyPoints)
        {
            Vector3 model1ToModel2S = this.GetModelStartPoint() - p;
            Vector3 model1ToModel2E = this.GetModelEndPoint() - p;
            float angle = Vector3.Dot(model1ToModel2S, model1ToModel2E);
            if (isShowLog)
                Debug.Log($"PipeFlange ConnectedModel count:{cCount} model1:{model1.name} model2:{model2.name} angleS:{angle} p:{p}");
            if (angle < 0)
            {
                model1.AddConnectedModel(model2);
                model2.AddConnectedModel(model1);
                cCount++;
            }
        }

       
        //Vector3 model1ToModel2SS = model1.GetModelStartPoint() - model2.GetModelStartPoint();
        //Vector3 model1ToModel2SE = model1.GetModelStartPoint() - model2.GetModelEndPoint();
        //Vector3 model1ToModel2ES = model1.GetModelEndPoint() - model2.GetModelStartPoint();
        //Vector3 model1ToModel2EE = model1.GetModelEndPoint() - model2.GetModelEndPoint();
        //float angleS = Vector3.Dot(model1ToModel2SS, model1ToModel2SE);
        //float angleE = Vector3.Dot(model1ToModel2ES, model1ToModel2EE);
        //if (angleS < 0)
        //{
        //    model1.AddConnectedModel(model2);
        //    model2.AddConnectedModel(model1);
        //    cCount++;
        //}
        //if (angleE < 0)
        //{
        //    model1.AddConnectedModel(model2);
        //    model2.AddConnectedModel(model1);
        //    cCount++;
        //}
        //if(isShowLog)
        //    Debug.Log($"PipeFlange ConnectedModel count:{cCount} model1:{model1.name} model2:{model2.name} angleS:{angleS} angleE:{angleE}");
        return cCount;
    }
}

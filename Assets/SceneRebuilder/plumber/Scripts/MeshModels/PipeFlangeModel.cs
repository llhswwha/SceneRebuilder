using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeFlangeModel : PipeReducerModel
{
    private float defaultMinRepeatPointDistance=0.0002f;

    public override SharedMeshTrianglesList GetSharedMeshTrianglesList(MeshTriangles meshTriangles)
    {
        SharedMeshTrianglesList list= base.GetSharedMeshTrianglesList(meshTriangles);
        list.CombineSameCenter(defaultMinRepeatPointDistance);
        return list;
    }

    public override void GetModelInfo()
    {
        if (minRepeatPointDistance < defaultMinRepeatPointDistance)
        {
            minRepeatPointDistance = defaultMinRepeatPointDistance;
        }
        base.GetModelInfo();//->GetSharedMeshTrianglesList
        //if (PipeRadius1> PipeRadius2)
        //{
        //    PipeRadius = PipeRadius1;
        //}
        //else
        //{
        //    PipeRadius = PipeRadius2;
        //}
        //StartPoint.w = PipeRadius;
        //EndPoint.w = PipeRadius;

        ModelStartPoint = StartPoint;
        ModelEndPoint = EndPoint;
    }

    protected override bool GetModelInfo3(SharedMeshTrianglesList trianglesList)
    {
            IsSpecial = true;
            trianglesList.Sort((a, b) => { return (b.Radius+b.MinRadius).CompareTo((a.Radius+a.MinRadius)); });

            //meshTriangles.ShowSharedMeshTrianglesList(this.transform, PointScale, 15, trianglesList);

            SharedMeshTrianglesList list2 = new SharedMeshTrianglesList();
            list2.Add(trianglesList[0]);
            list2.Add(trianglesList[1]);
            //list2.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });

            KeyPointInfo = new PipeElbowKeyPointInfo(list2[0].GetCenter4(), list2[1].GetCenter4(), list2[1].GetMinCenter4(), trianglesList[2].GetCenter4());

            StartPoint= list2[0].GetCenter4();
            EndPoint= trianglesList[2].GetCenter4();
            ModelStartPoint = StartPoint;
            ModelEndPoint = EndPoint;

            ShowKeyPoints(KeyPointInfo, "Flange3");
            return true;
    }

    protected override void SetRadius()
    {
        PipeRadius1 = StartPoint.w;
        PipeRadius2 = EndPoint.w;
        //PipeRadius = (PipeRadius1 + PipeRadius2) / 2;


        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }
    }

    public override GameObject RendererModel(PipeGenerateArg arg0, string afterName)
    {
        PipeGenerateArg arg = arg0.Clone();
        arg.generateWeld = false;
        if (IsSpecial)
        {
            GameObject pipeNew = GetPipeNewGo(arg, afterName);
            GameObject pipe11 = RenderPipeLine(arg, afterName + "_1", KeyPointInfo.EndPointIn1, KeyPointInfo.EndPointOut1);
            GameObject pipe12 = RenderPipeLine(arg, afterName + "_2", KeyPointInfo.EndPointOut2, KeyPointInfo.EndPointIn2);
            pipe11.transform.SetParent(pipeNew.transform);
            pipe12.transform.SetParent(pipeNew.transform);

            GameObject target = pipeNew;
            target = MeshCombineHelper.Combine(pipeNew);
            this.ResultGo = target;

            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
        else
        {

            StartPoint.w = PipeRadius;
            EndPoint.w = PipeRadius;

            return base.RendererModel(arg, afterName);
        }

    }

    public override int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    {
        int cCount = base.ConnectedModel(model2, minPointDis, isShowLog, isUniformRaidus, minRadiusDis);

        PipeModelBase model1 = this;
        var keyPoints = model2.GetModelKeyPoints();
        var msp = this.GetModelStartPoint();
        var mep = this.GetModelEndPoint();
        //TransformHelper.ShowPoint(msp, PointScale, this.transform).name=$"{this.name}_StartPoint_{Vector3String(msp)}_{Vector3String(ModelStartPoint)}";
        //TransformHelper.ShowPoint(mep, PointScale, this.transform).name = $"{this.name}_EndPoint_{Vector3String(mep)}_{Vector3String(ModelEndPoint)}";
        for (int i = 0; i < keyPoints.Count; i++)
        {
            Vector4 p = keyPoints[i];
            //TransformHelper.ShowPoint(p, PointScale, model2.transform).name = $"{model2.name}_Point[{i}]_{Vector3String(p)}";
            Vector3 model1ToModel2S = msp - p;
            Vector3 model1ToModel2E = mep - p;
            float angle = Vector3.Dot(model1ToModel2S, model1ToModel2E);
            if (isShowLog)
                Debug.Log($"PipeFlange ConnectedModel count:{cCount} model1:{model1.name} model2:{model2.name} angleS:{angle} p:{p} model1ToModel2S:{Vector3String(model1ToModel2S)} model1ToModel2E:{Vector3String(model1ToModel2E)}");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeWeldoletModel : PipeTeeModel
{


    public float CircleWidth = 0.001f;

    public override void GetModelInfo()
    {
        if (minRepeatPointDistance < 0.001f)
        {
            minRepeatPointDistance = 0.001f;
        }
        //base.GetModelInfo();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);
        //SharedMeshTrianglesList keyPoints = meshTriangles.GetWeldoletKeyPoints(sharedMinCount, minRepeatPointDistance);
        //KeyPointCount = keyPoints.Count;
        //if (keyPoints.Count == 3)
        //{
        //    var circle1 = keyPoints[0];//Center
        //    var circle2 = keyPoints[1];//Circle
        //    var circle3 = keyPoints[2];//Right

        //    circle1.SetCenterWithOutPoint(minRepeatPointDistance);

        //    var p1 = circle1.GetCenter4WithPower(0.99f);
        //    var p3 = circle3.GetCenter4WithPower(0.99f);

        //    TransformHelper.ShowLocalPoint(p1, PointScale, this.transform, null);
        //    TransformHelper.ShowLocalPoint(p3, PointScale, this.transform, null);

        //    KeyPointInfo = new PipeElbowKeyPointInfo(p3, p1, circle2.GetCenter4WithOff(CircleWidth), circle2.GetCenter4WithOff(-CircleWidth));

        //    IsGetInfoSuccess = true;
        //}
        //else if(keyPoints.Count == 2)
        //{
        //    var circle2 = keyPoints[0];//Circle
        //    var circle3 = keyPoints[1];//Right

        //    var p1 = circle3.GetCenter4WithOff(CircleWidth);
        //    var p3 = circle3.GetCenter4WithPower(0.99f);

        //    TransformHelper.ShowLocalPoint(p1, PointScale, this.transform, null);
        //    TransformHelper.ShowLocalPoint(p3, PointScale, this.transform, null);

        //    KeyPointInfo = new PipeElbowKeyPointInfo(p3, p1, circle2.GetCenter4WithOff(CircleWidth), circle2.GetCenter4WithOff(-CircleWidth));

        //    IsGetInfoSuccess = true;
        //}
        //else
        //{
        //    IsGetInfoSuccess = false;
        //}

        PipeWeldoletData data = PipeWeldoletInfoJob.GetModelData(mesh, sharedMinCount, minRepeatPointDistance, CircleWidth, this.name);
        SetModelData(data);

        meshTriangles.Dispose();
    }

    internal void SetModelData(PipeWeldoletData lineData)
    {
        ModelData = lineData;
        //this.IsSpecial = lineData.IsSpecial;
        this.IsGetInfoSuccess = lineData.IsGetInfoSuccess;
        this.KeyPointCount = lineData.KeyPointCount;
        this.KeyPointInfo = new PipeModelKeyPointInfo4(lineData.KeyPointInfo);
        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointIn1;

    }

    public new PipeWeldoletData ModelData;

    public new PipeWeldoletData GetModelData()
    {
        ModelData.KeyPointInfo = new PipeModelKeyPointData4(KeyPointInfo);
        //ModelData.InnerKeyPointInfo = new PipeModelKeyPointData4(InnerKeyPointInfo);
        //ModelData.KeyPlaneInfo = new PipeModelKeyPlaneData4(KeyPlaneInfo);
        //ModelData.InnerKeyPlaneInfo = new PipeModelKeyPlaneData4(InnerKeyPlaneInfo);
        ModelData.IsGetInfoSuccess = IsGetInfoSuccess;
        //ModelData.IsSpecial = IsSpecial;
        ModelData.KeyPointCount = KeyPointCount;
        return ModelData;
    }

    public new PipeWeldoletSaveData GetSaveData()
    {
        PipeWeldoletSaveData data = new PipeWeldoletSaveData();
        InitSaveData(data);
        data.Data = GetModelData();
        //KeyPointInfo = null;
        //InnerKeyPointInfo = null;
        ////KeyPlaneInfo = null;
        return data;
    }

    public override void SetSaveData(MeshModelSaveData data)
    {
        //this.LineInfo = data.Info;
        SetModelData((data as PipeWeldoletSaveData).Data);
        //PipeFactory.Instance.RendererModelFromXml(this, data);
    }

    public override GameObject RendererModel(PipeGenerateArg arg0, string afterName)
    {
        if (RendererErrorModel())
        {
            return null;
        }

        if (arg0.pipeMaterial == null)
        {
            arg0.pipeMaterial = PipeFactory.Instance.generateArg.pipeMaterial;
        }
        if (arg0.weldMaterial == null)
        {
            arg0.weldMaterial = PipeFactory.Instance.generateArg.weldMaterial;
        }

        if (KeyPointInfo!=null)
        {
            var arg = arg0.Clone();
            arg.generateEndCaps=true;

            GameObject pipeNew = GetPipeNewGo(arg, afterName);
            float capOffset = Vector3.Distance(KeyPointInfo.EndPointIn1, (KeyPointInfo.EndPointIn2 + KeyPointInfo.EndPointOut2) / 2);

            //capOffset = 0;
            arg.EndCapOffset = 0;
            arg.StartCapOffset = capOffset;
            //if (KeyPointCount == 3)
            //{
                arg.generateWeld = arg0.generateWeld;
                arg.weldCircleRadius = KeyPointInfo.EndPointOut1.w;
            //}
            //else
            //{
            //    arg.generateWeld = false;
            //}
            
            GameObject pipe11 = RenderPipeLine(arg, afterName + "_1", KeyPointInfo.EndPointIn1, KeyPointInfo.EndPointOut1);

            arg.generateWeld = false;
            arg.EndCapOffset = 0;
            arg.StartCapOffset = 0;
            //if(arg.pipeSegments<32)
            //{
            //    arg.pipeSegments = 32;
            //}

            var p1 = KeyPointInfo.EndPointIn2;
            p1.w *= 0.95f;
            var p2 = KeyPointInfo.EndPointOut2;
            p2.w *= 0.95f;
            GameObject pipe12 = RenderPipeLine(arg, afterName + "_2", p1, p2);
            
            pipe11.transform.SetParent(pipeNew.transform);
            pipe12.transform.SetParent(pipeNew.transform);
            //GameObject target = pipeNew;

            //if (IsCombineResult)
            //{
            //    target = MeshCombineHelper.Combine(pipeNew);
            //}

            GameObject target = CombineTarget(arg,pipeNew);

            target = CopyMeshComponentsEx(target);

            this.ResultGo = target;

            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
        else
        {
            return null;
        }
        
    }

    public override string GetDictKey()
    {
        float capOffset = Vector3.Distance(KeyPointInfo.EndPointIn1, (KeyPointInfo.EndPointIn2 + KeyPointInfo.EndPointOut2) / 2);
        return $"Weldolet_{KeyPointInfo.GetRadiusIn1Out1():F3},{capOffset:F3},{KeyPointInfo.GetRadiusIn2Out2():F3}";
    }
}

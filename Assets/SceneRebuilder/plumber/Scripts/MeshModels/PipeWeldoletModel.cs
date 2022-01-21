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
            if (this.ResultGo)
            {
                GameObject.DestroyImmediate(ResultGo);
            }

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
            GameObject pipe12 = RenderPipeLine(arg, afterName + "_2", KeyPointInfo.EndPointIn2, KeyPointInfo.EndPointOut2);
            
            pipe11.transform.SetParent(pipeNew.transform);
            pipe12.transform.SetParent(pipeNew.transform);
            //GameObject target = pipeNew;

            //if (IsCombineResult)
            //{
            //    target = MeshCombineHelper.Combine(pipeNew);
            //}

            GameObject target = CombineTarget(arg,pipeNew);

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
}

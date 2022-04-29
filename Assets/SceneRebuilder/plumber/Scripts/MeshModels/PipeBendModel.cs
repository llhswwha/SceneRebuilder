using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBendModel : PipeModelBase
{

    public PipeLineInfoList info = new PipeLineInfoList();

    public void ShowPartLines()
    {
        ClearDebugInfoGos();
        info = MeshTriangles.ShowPartLines(this.gameObject, PointScale, 40, true);
    }

    public override void GetModelInfo()
    {
        //base.GetModelInfo();
        ClearDebugInfoGos();
        info = MeshTriangles.ShowPartLines(this.gameObject, PointScale, 40, false);
    }

    [ContextMenu("RendererElbowModel")]
    public GameObject RendererElbowModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"RendererElbow IsGetInfoSuccess == false :{this.name}");
            return null;
        }
        if (info == null || info.Count == 0)
        {
            Debug.LogError($"RendererElbow info==null :{this.name}");
            return null;
        }
        bool isNewGo = true;
        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, isNewGo);
        //PipeLineInfo startLine= PipeLineInfo
        PipeCreateArg pipeArg = new PipeCreateArg(info.GetLine1(), info.GetLine2());
        var ps = pipeArg.GetGeneratePoints(0, 2, false);
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        arg.SetArg(pipe);

        pipe.IsGenerateEndWeld = true;
        //pipe.IsElbow = true;
        pipe.IsBend = true;
        //pipe.pipeRadius = (info.EndPointOut1.w + info.EndPointOut2.w) / 2;
        pipe.pipeRadius = info.GetRadius();
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipe.gameObject;
    }

    public float startEndDis = 0.3f;

    public int elbowSegments = 3;

    [ContextMenu("RendererLinesModel")]
    public GameObject RendererLinesModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"RendererElbow IsGetInfoSuccess == false :{this.name}");
            return null;
        }
        if (info == null || info.Count == 0)
        {
            Debug.LogError($"RendererElbow info==null :{this.name}");
            return null;
        }
        bool isNewGo = true;
        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, isNewGo);

        //var ps = pipeArg.GetGeneratePoints(0, 2, false);
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = info.GetLinePoints(startEndDis);

        arg.SetArg(pipe);

        pipe.IsGenerateEndWeld = true;
        pipe.IsElbow = true; 
        pipe.IsBend = true;
        //pipe.pipeRadius = (info.EndPointOut1.w + info.EndPointOut2.w) / 2;
        pipe.pipeRadius = info.GetRadius();
        //pipe.elbowRadius = info.GetElbowRadius();
        pipe.elbowRadius = 0;

        //pipe.elbowSegments = elbowSegments;
        //pipe.elbowRadius = 0.015f;
        //pipe.elbowRadius = pipe.pipeRadius / 3;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipe.gameObject; 
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return RendererLinesModel(arg, afterName);
    }
}

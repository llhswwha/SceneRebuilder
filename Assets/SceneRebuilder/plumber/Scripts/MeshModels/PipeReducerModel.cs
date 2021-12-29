using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeReducerModel : PipeLineModel
{
    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        GameObject pipeNew = new GameObject(this.name + afterName);
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = new List<Vector3>() { LineInfo.StartPoint, LineInfo.EndPoint };
        arg.SetArg(pipe);
        pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius1 = PipeRadius1;
        pipe.pipeRadius2 = PipeRadius2;
        pipe.IsGenerateEndWeld = true;
        pipe.RenderPipe();
        return pipeNew;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeeModel : PipeElbowModel
{
    public Vector4 StartPoint = Vector3.zero;
    public Vector4 EndPoint = Vector3.zero;

    //public List<PlanePointDistance> distanceListEx;

    public override void GetModelInfo()
    {
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return base.RendererModel(arg, afterName);
    }
}

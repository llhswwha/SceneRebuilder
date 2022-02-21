using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMeshModel : BaseMeshModel
{
    public MeshBoxData ModelData;

    public override void GetModelInfo()
    {
        base.GetModelInfo();
    }

    public override void GetModelInfo_Job()
    {
        BoxMeshInfoJob job = new BoxMeshInfoJob();
    }
}

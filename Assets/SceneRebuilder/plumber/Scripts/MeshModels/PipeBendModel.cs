using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBendModel : PipeModelBase
{

    public void ShowPartLines()
    {
        ClearDebugInfoGos();
        MeshTriangles.ShowPartLines(this.gameObject, PointScale,40);
    }
}

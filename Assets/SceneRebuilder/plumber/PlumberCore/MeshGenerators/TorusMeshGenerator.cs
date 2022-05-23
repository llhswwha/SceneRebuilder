using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusMeshGenerator : MonoBehaviour
{
    public Vector3 center;
    public Vector3 direction;
    public int pipeSegments;
    public float pipeRadius;

    // Start is called before the first frame update
    void Start()
    {
        RenderMesh();
    }

    [ContextMenu("RenderMesh")]
    public void RenderMesh()
    {
        MeshGeneratorHelper.GenerateCircleAtPoint(center, direction, pipeSegments, pipeRadius);
    }
}

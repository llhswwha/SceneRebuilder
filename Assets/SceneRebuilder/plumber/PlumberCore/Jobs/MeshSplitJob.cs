using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public struct MeshSplitJob : IJob
{
    public int id;

    public MeshStructure mesh;

    public void Execute()
    {
        System.DateTime start = System.DateTime.Now;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        List<MeshTriangleList> splitedParts = meshTriangles.GetSplitedPartsByPoint(0, null,false);
        Debug.Log($"MeshSplitJob[{id}] time:{ System.DateTime.Now - start} meshTriangles:{meshTriangles.Count} triangleList:{splitedParts.Count}");
    }
}

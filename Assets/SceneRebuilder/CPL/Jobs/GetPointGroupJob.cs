using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct GetPointGroupJob : IMeshInfoJob
{
    public int id;

    //public OrientedBoundingBox OBB;

    public NativeArray<float3> points;

    public GetPointGroupJobResult lineData;

    public static NativeArray<GetPointGroupJobResult> Result;

    public static NativeList<int> ErrorIds;

    public void Execute()
    {
        float3 v1 = points[0];
        float3 closedP = MeshHelper.FindClosedPoint(v1, points);
        float dis = math.distance(v1, closedP);
    }

    public void Dispose()
    {
        points.Dispose();
    }
}

public struct GetPointGroupJobResult
{

}

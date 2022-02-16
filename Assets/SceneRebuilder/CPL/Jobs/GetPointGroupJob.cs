using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct GetPointGroupJob : IPipeJob
{
    public int id;

    //public OrientedBoundingBox OBB;

    public NativeArray<Vector3> points;

    public GetPointGroupJobResult lineData;

    public static NativeArray<GetPointGroupJobResult> Result;

    public static NativeList<int> ErrorIds;

    public void Execute()
    {
        
    }

    public void Dispose()
    {
        points.Dispose();
    }
}

public struct GetPointGroupJobResult
{

}

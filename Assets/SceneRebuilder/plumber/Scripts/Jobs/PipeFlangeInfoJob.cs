using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeFlangeInfoJob : IPipeJob
{
    public void Dispose()
    {
        mesh.Dispose();
    }

    public int id;
    public MeshStructure mesh;

    public static NativeArray<PipeReducerData> Result;

    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.0002f;

    public void Execute()
    {
        DateTime start = DateTime.Now;
        PipeReducerData data = PipeReducerInfoJob.GetReducerData(ref mesh, id, sharedMinCount, minRepeatPointDistance);

        //PipeReducerData data = new PipeReducerData();
        //var meshTriangles = new MeshTriangles(mesh);
        //SharedMeshTrianglesList points = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        //var distanceList = points.GetPlanePointDistanceList();
        //if (points.Count != 2)
        //{
        //    data.IsGetInfoSuccess = false;
        //    Debug.LogError($"GetKeyPointsById points.Count != 2 count:{points.Count} gameObject:{id}");
        //    return;
        //}
        //SharedMeshTriangles startP = distanceList[0].Plane;
        //data.StartPoint = startP.GetCenter4();

        //var PipeRadius1 = data.StartPoint.w;

        //SharedMeshTriangles endP = distanceList[1].Plane;
        //data.EndPoint = endP.GetCenter4();
        //var PipeRadius2 = data.EndPoint.w;

        //var PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        //points.Remove(data.StartPoint);
        //points.Remove(data.EndPoint);

        //data.IsGetInfoSuccess = true;

        if (Result.Length > id)
        {
            Result[id] = data;
        }
        else
        {
            Debug.LogWarning($"GetReducerInfo[{id}] Result.Length :{Result.Length }");
        }
        Debug.Log($">>>PipeFlangeInfoJob time:{DateTime.Now - start} data:{data}");
    }
}

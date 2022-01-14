using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeReducerInfoJob : IPipeJob
{
    public void Dispose()
    {
        mesh.Dispose();
    }

    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.00005f;

    public int id;
    public MeshStructure mesh;

    public static NativeArray<PipeReducerData> Result;

    public static NativeList<int> ErrorIds;

    public void Execute()
    {
        
        DateTime start = DateTime.Now;
        PipeReducerData data = GetReducerData(ref mesh,id, sharedMinCount, minRepeatPointDistance,false, ErrorIds);

        //PipeReducerData data = new PipeReducerData();
        //var meshTriangles = new MeshTriangles(mesh);
        //SharedMeshTrianglesList points = meshTriangles.GetKeyPointsByIdEx();
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
        Debug.Log($">>>GetReducerInfo time:{DateTime.Now - start} data:{data}");
    }

    public static PipeReducerData GetReducerData(ref MeshStructure mesh,int id, int minCount, float minDis,bool isCombineCenter, NativeList<int> errorIds)
    {
        PipeReducerData data = new PipeReducerData();
        var meshTriangles = new MeshTriangles(mesh);
        SharedMeshTrianglesList points = meshTriangles.GetKeyPointsByIdEx(minCount, minDis);
        if (isCombineCenter)
        {
            points.CombineSameCenter(minDis);
        }
        var distanceList = points.GetPlanePointDistanceList();
        if (points.Count != 2)
        {
            data.IsGetInfoSuccess = false;
            Debug.LogError($"PipeReducerInfoJob GetKeyPointsById points.Count != 2 count:{points.Count} gameObject:{id}");
            errorIds.Add(id);
            return data;
        }
        SharedMeshTriangles startP = distanceList[0].Plane;
        data.StartPoint = startP.GetCenter4();

        var PipeRadius1 = data.StartPoint.w;

        SharedMeshTriangles endP = distanceList[1].Plane;
        data.EndPoint = endP.GetCenter4();
        var PipeRadius2 = data.EndPoint.w;

        var PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        points.Remove(data.StartPoint);
        points.Remove(data.EndPoint);

        data.IsGetInfoSuccess = true;
        return data;
    }
}

public struct PipeReducerData
{
    public Vector4 StartPoint;
    public Vector4 EndPoint ;

    public bool IsSpecial;

    public bool IsGetInfoSuccess;

    public override string ToString()
    {
        return $"ReducerData IsSpecial:{IsSpecial} IsGetInfoSuccess:{IsGetInfoSuccess} StartPoint:{StartPoint} EndPoint:{EndPoint}";
    }
}

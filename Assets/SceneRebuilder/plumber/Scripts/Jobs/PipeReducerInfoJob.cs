using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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

    public static PipeReducerData GetReducerData(ref MeshStructure mesh,int id, int minCount, float minDis,bool isFlange, NativeList<int> errorIds)
    {
        PipeReducerData data = new PipeReducerData();
        var meshTriangles = new MeshTriangles(mesh);
        SharedMeshTrianglesList trianglesList = meshTriangles.GetSharedMeshTrianglesListById(minCount, minDis);
        if (isFlange)
        {
            trianglesList.CombineSameCenter(minDis);
        }
        var distanceList = trianglesList.GetPlanePointDistanceList();
        //if (points.Count != 2)
        //{
        //    data.IsGetInfoSuccess = false;
        //    Debug.LogError($"PipeReducerInfoJob GetKeyPointsById points.Count != 2 count:{points.Count} gameObject:{id}");
        //    errorIds.Add(id);
        //    return data;
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
        //return data;

        ////Debug.LogError($"GetReducerData count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance} isFlange:{isFlange}");

        data.KeyPointCount = trianglesList.Count;
        if (trianglesList.Count == 3)
        {

            if (isFlange)
            {
                data.IsGetInfoSuccess = true;
                data.IsSpecial = true;
                trianglesList.Sort((a, b) => { return (b.Radius + b.MinRadius).CompareTo((a.Radius + a.MinRadius)); });

                //meshTriangles.ShowSharedMeshTrianglesList(this.transform, PointScale, 15, trianglesList);

                SharedMeshTrianglesList list2 = new SharedMeshTrianglesList();
                list2.Add(trianglesList[0]);
                list2.Add(trianglesList[1]);
                //list2.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });

                data.KeyPointInfo = new PipeModelKeyPointData4(list2[0].GetCenter4(), list2[1].GetCenter4(), list2[1].GetMinCenter4(), trianglesList[2].GetCenter4());

                data.StartPoint = list2[0].GetCenter4();
                data.EndPoint = trianglesList[2].GetCenter4();
                //ModelStartPoint = StartPoint;
                //ModelEndPoint = EndPoint;

                //ShowKeyPoints(KeyPointInfo, "Flange3");
                //return true;
                return data;
            }
            else
            {
                data.IsGetInfoSuccess = false;
                Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
                return data;
            }
        }
        else if (trianglesList.Count == 2)
        {
            SharedMeshTriangles startP = distanceList[0].Plane;
            data.StartPoint = startP.GetCenter4();

            var PipeRadius1 = data.StartPoint.w;

            SharedMeshTriangles endP = distanceList[1].Plane;
            data.EndPoint = endP.GetCenter4();
            var PipeRadius2 = data.EndPoint.w;

            var PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

            trianglesList.Remove(data.StartPoint);
            trianglesList.Remove(data.EndPoint);

            //TransformHelper.ShowLocalPoint(data.StartPoint, data.PointScale, this.transform, null).name = "StartPoint";
            //TransformHelper.ShowLocalPoint(data.EndPoint, data.PointScale, this.transform, null).name = "EndPoint";

            //GetPipeRadius();

            data.IsGetInfoSuccess = true;

            //ModelStartPoint = StartPoint;
            //ModelEndPoint = EndPoint;
            return data;
        }
        else
        {
            data.IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            errorIds.Add(id);
            return data;
        }
    }
}

public struct PipeReducerData
{
    public Vector4 StartPoint;
    public Vector4 EndPoint ;

    [XmlAttribute]
    public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;

    public PipeModelKeyPointData4 KeyPointInfo;

    public override string ToString()
    {
        return $"ReducerData IsSpecial:{IsSpecial} IsGetInfoSuccess:{IsGetInfoSuccess} StartPoint:{StartPoint} EndPoint:{EndPoint}";
    }
}

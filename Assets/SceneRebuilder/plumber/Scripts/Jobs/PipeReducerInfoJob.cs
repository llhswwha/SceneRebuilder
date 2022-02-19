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
        //Debug.Log($">>>GetReducerInfo time:{DateTime.Now - start} data:{data}");
    }

    public static PipeReducerData GetReducerData(ref MeshStructure mesh,int id, int minCount, float minDis,bool isFlange, NativeList<int> errorIds)
    {
        PipeReducerData data = new PipeReducerData();
        var meshTriangles = new MeshTriangles(mesh);
        SharedMeshTrianglesList trianglesList = meshTriangles.GetSharedMeshTrianglesListById(minCount, minDis);
        //if (isFlange)
        //{
        //    trianglesList.CombineSameCenter(minDis);
        //}
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
        if (trianglesList.Count >= 3)
        {

            if (isFlange)
            {
                //data.IsGetInfoSuccess = true;
                //data.IsSpecial = true;
                //trianglesList.Sort((a, b) => { return (b.Radius + b.MinRadius).CompareTo((a.Radius + a.MinRadius)); });

                ////meshTriangles.ShowSharedMeshTrianglesList(this.transform, PointScale, 15, trianglesList);

                //SharedMeshTrianglesList list2 = new SharedMeshTrianglesList();
                //list2.Add(trianglesList[0]);
                //list2.Add(trianglesList[1]);
                ////list2.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });

                //data.KeyPointInfo = new PipeModelKeyPointData4(list2[0].GetCenter4(), list2[1].GetCenter4(), list2[1].GetMinCenter4(), trianglesList[2].GetCenter4());

                //data.StartPoint = list2[0].GetCenter4();
                //data.EndPoint = trianglesList[2].GetCenter4();
                ////ModelStartPoint = StartPoint;
                ////ModelEndPoint = EndPoint;

                ////ShowKeyPoints(KeyPointInfo, "Flange3");
                ////return true;
                //return data;

                if (trianglesList.Count == 3)
                {
                    trianglesList.CombineSameCenter(minDis);
                    trianglesList.RemoveNotCircle();

                    if (trianglesList.Count == 2)
                    {
                        return GetLineData2(data, trianglesList[0], trianglesList[1]);
                    }
                    else
                    {
                        Debug.LogError($"PipeFlangeModel.GetModelInfo3 Error trianglesList.Count == 3 Count:{trianglesList.Count} id:{id}");
                        data.IsSpecial = true;
                        data.IsGetInfoSuccess = false;
                        errorIds.Add(id);
                        return data;
                    }
                }
                else if (trianglesList.Count == 4 || trianglesList.Count == 5)
                {
                    SharedMeshTrianglesList trianglesList2 = new SharedMeshTrianglesList(trianglesList);
                    trianglesList2.CombineSameCenter(minDis);
                    if (trianglesList2.Count == 2)
                    {
                        return GetLineData2(data, trianglesList2[0], trianglesList2[1]);
                    }
                    //else if (trianglesList2.Count == 3)
                    //{
                    //    //Debug.LogError($"PipeFlangeModel.GetModelInfo4 5 Error trianglesList.Count == 3 Count:{trianglesList.Count} gameObject:{this.name}");
                    //    //IsSpecial = true;
                    //    //IsGetInfoSuccess = false;
                    //    return true;
                    //}
                    else
                    {
                        //trianglesList2.RemoveNotCircle();
                        trianglesList.Sort((a, b) => { return (b.Radius + b.MinRadius).CompareTo((a.Radius + a.MinRadius)); });

                        //meshTriangles.ShowSharedMeshTrianglesList(this.transform, PointScale, 15, trianglesList);

                        SharedMeshTrianglesList list2 = new SharedMeshTrianglesList();
                        list2.Add(trianglesList[0]);
                        list2.Add(trianglesList[1]);
                        //list2.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });


                        var line2StartPlane = trianglesList[2];
                        var line2EndPlane = trianglesList[3];


                        var line2Start = line2StartPlane.GetCenter4();
                        var line2End = line2EndPlane.GetCenter4();

                        float dis1 = Math.Abs(line2StartPlane.Radius - line2EndPlane.Radius);
                        float dis2 = Math.Abs(line2StartPlane.MinRadius - line2EndPlane.MinRadius);
                        float dis3 = Math.Abs(line2StartPlane.Radius - line2StartPlane.MinRadius);
                        if (dis1 > 0.001f)
                        {
                            Debug.LogWarning($"PipeFlangeModel.GetModelInfo3 RadiusError dis1:{dis1} dis2:{dis2} dis3:{dis3} id:{id}");
                            if (dis3 > 0.001f && dis2 < 0.0001f && line2StartPlane.MinRadius > 0.001f)
                            {
                                line2Start = line2StartPlane.GetMinCenter4();
                                line2End = line2EndPlane.GetMinCenter4();
                            }
                            else
                            {
                                Debug.LogError($"PipeFlangeModel.GetModelInfo3 RadiusError dis1:{dis1} dis2:{dis2} dis3:{dis3} id:{id}");
                                errorIds.Add(id);
                            }
                        }

                        data.KeyPointInfo = new PipeModelKeyPointData4(trianglesList[0].GetCenter4(), trianglesList[1].GetCenter4(), line2Start, line2End);

                        data.StartPoint = list2[0].GetCenter4();
                        data.EndPoint = trianglesList[2].GetCenter4();
                        //data.ModelStartPoint = StartPoint;
                        //data.ModelEndPoint = EndPoint;

                        //ShowKeyPoints(KeyPointInfo, "Flange3");

                        data.IsSpecial = true;
                        data.IsGetInfoSuccess = true;
                        return data;
                    }

                }
                else
                {
                    Debug.LogError($"PipeFlangeModel.GetModelInfo3 Error Count:{trianglesList.Count} id:{id}");
                    errorIds.Add(id);
                    data.IsSpecial = true;
                    data.IsGetInfoSuccess = false;
                    return data;
                }
            }
            else
            {
                data.IsGetInfoSuccess = false;
                Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minDis}");
                errorIds.Add(id);
                return data;
            }
        }
        else if (trianglesList.Count == 2)
        {
            return GetLineData2(data, distanceList[0].Plane, distanceList[1].Plane);
        }
        else
        {
            data.IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minDis}");
            errorIds.Add(id);
            return data;
        }
    }

    private static PipeReducerData GetLineData2(PipeReducerData data, SharedMeshTriangles  startP, SharedMeshTriangles endP)
    {
        //SharedMeshTriangles startP = distanceList[0].Plane;
        data.StartPoint = startP.GetCenter4();

        var PipeRadius1 = data.StartPoint.w;

        //SharedMeshTriangles endP = distanceList[1].Plane;
        data.EndPoint = endP.GetCenter4();
        var PipeRadius2 = data.EndPoint.w;

        var PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        //trianglesList.Remove(data.StartPoint);
        //trianglesList.Remove(data.EndPoint);

        //TransformHelper.ShowLocalPoint(data.StartPoint, data.PointScale, this.transform, null).name = "StartPoint";
        //TransformHelper.ShowLocalPoint(data.EndPoint, data.PointScale, this.transform, null).name = "EndPoint";

        //GetPipeRadius();

        data.IsGetInfoSuccess = true;

        //ModelStartPoint = StartPoint;
        //ModelEndPoint = EndPoint;
        return data;
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

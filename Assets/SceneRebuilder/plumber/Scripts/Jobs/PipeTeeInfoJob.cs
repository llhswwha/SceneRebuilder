using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Collections;
using UnityEngine;

public struct PipeTeeInfoJob : IPipeJob
{
    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.00005f;
    public static float PipeLineOffset = 0.05f;

    public int id;
    public MeshStructure mesh;

    public static NativeArray<PipeTeeData> Result;


    public void Execute()
    {
        PipeTeeData data = new PipeTeeData();

        DateTime start = DateTime.Now;
        var VertexCount = mesh.vertexCount;
        var meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByPointEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        //trianglesList.RemoveNotCircle();
        if (trianglesList.Count == 7)
        {
            data=GetTeeInfo7(trianglesList);
            data.IsGetInfoSuccess = true;
            data.KeyPointCount = 7;

        }
        else if (trianglesList.Count == 4)
        {
            data=GetTeeInfo4(trianglesList);
            data.IsGetInfoSuccess = true;
            data.KeyPointCount = 4;
        }
        else
        {
            data.KeyPointCount = trianglesList.Count;
            data.IsGetInfoSuccess = false;
            Debug.LogError($">>>GetTeeInfo points.Count Error count:{trianglesList.Count} gameObject:{id} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }

        if (Result.Length > id)
        {
            Result[id] = data;
        }
        else
        {
            Debug.LogWarning($"PipeElbowInfoJob[{id}] Result.Length :{Result.Length }");
        }

        //Debug.Log($">>>GetTeeInfo time:{DateTime.Now - start} data:{data}");
    }

    private PipeTeeData GetTeeInfo7(SharedMeshTrianglesList trianglesList)
    {
        PipeTeeData data = new PipeTeeData();
        SharedMeshTrianglesList list11 = new SharedMeshTrianglesList();
        SharedMeshTrianglesList list12 = new SharedMeshTrianglesList();
        var plane1 = trianglesList[0];
        list11.Add(plane1);
        for (int i = 1; i < trianglesList.Count; i++)
        {
            var plane = trianglesList[i];
            float angle = Vector3.Dot(plane1.Normal, plane.Normal);
            //Debug.Log($"Tee GetModelInfo7 angle[{angle}] normal1:{plane1.Normal} normal2:{plane.Normal}");
            if (Mathf.Abs(angle) < 0.001f)
            {
                list12.Add(plane);
            }
            else
            {
                list11.Add(plane);
            }
        }
        

        SharedMeshTrianglesList list4;
        SharedMeshTrianglesList list3;
        if (list11.Count == 4)
        {
            list4 = list11;
            list3 = list12;
        }
        else
        {
            list4 = list12;
            list3 = list11;
        }

        SharedMeshTrianglesList list;

        var KeyPlaneInfo = PipeModelKeyPlaneInfo4.GetElbow4Planes(list4);
        data.KeyPointInfo = KeyPlaneInfo.GetKeyPointsData();
        data.KeyPlaneInfo = new PipeModelKeyPlaneData4(KeyPlaneInfo);
        //KeyPointInfo = GetElbow4(list4);

        //ModelStartPoint = KeyPointInfo.EndPointOut1;
        //ModelEndPoint = KeyPointInfo.EndPointOut2;

        list3.Sort((a, b) => { return a.Radius.CompareTo(b.Radius); });

        data.InnerKeyPlaneInfo = new PipeModelKeyPlaneData4(list3[1], list3[2], list3[0], list3[0]);
        data.InnerKeyPointInfo = data.InnerKeyPlaneInfo.GetKeyPointsData();


        data.IsSpecial = true;

        Debug.Log($"Tee GetModelInfo7 list1:{list11.Count} list2:{list12.Count} KeyPlaneInfo:{KeyPlaneInfo} InnerKeyPlaneInfo:{data.InnerKeyPlaneInfo}");
        return data;
    }

    private PipeTeeData GetTeeInfo4(SharedMeshTrianglesList trianglesList)
    {
        PipeTeeData data = new PipeTeeData();

        var distanceList = trianglesList.GetPlanePointDistanceList();

        SharedMeshTriangles teePlane1 = distanceList[0].Plane;

        var TeeStartPoint = teePlane1.GetCenter4();
        trianglesList.Remove(teePlane1);

        //SharedMeshTriangles? teePlane2 = null;

        ////float minNormalAngle = 0;
        //for (int i = 0; i < trianglesList.Count; i++)
        //{
        //    SharedMeshTriangles plane = trianglesList[i];
        //    var normalAngle = Vector3.Dot(teePlane1.Normal, plane.Normal);
        //    Debug.Log($"go:{id} angle[{i}] normal1:{teePlane1.Normal} normal2:{plane.Normal} angle:{normalAngle}");
        //    if (Mathf.Abs(normalAngle + 1) <= 0.00001)//相反或者平行
        //    {
        //        teePlane2 = plane;
        //        //break;
        //    }
        //    if (Mathf.Abs(normalAngle - 1) <= 0.00001)
        //    {
        //        teePlane2 = plane;
        //        //break;
        //    }
        //}

        SharedMeshTriangles? teePlane2 = trianglesList.FindSameDirectionPlane(teePlane1,id);

        if (teePlane2 == null)
        {
            data.IsGetInfoSuccess = false;
            Debug.LogError($"GetTeeModelInfo go:{id} teePlane2 == null");
            return data;
        }

        var TeeEndPoint = ((SharedMeshTriangles)teePlane2).GetCenter4();
        trianglesList.Remove(((SharedMeshTriangles)teePlane2));

        var LineStartPoint = trianglesList[0].GetCenter4();
        var LineEndPoint = trianglesList[1].GetCenter4();

        //EndPointIn2 = distanceList[1].Plane.GetCenter();
        //trianglesList.Remove(EndPointIn1);
        //trianglesList.Remove(EndPointIn2);

        //EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, trianglesList);
        //trianglesList.Remove(EndPointOut1);
        //EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, trianglesList);
        //trianglesList.Remove(EndPointOut2);

        //KeyPointInfo.Line1 = new PipeLineInfo(TeeStartPoint, TeeEndPoint, null);
        //KeyPointInfo.Line2 = new PipeLineInfo(LineStartPoint, LineEndPoint, null);

        data.KeyPointInfo = new PipeModelKeyPointData4(TeeStartPoint, TeeEndPoint, LineStartPoint, LineEndPoint);

        //TransformHelper.ShowLocalPoint(TeeStartPoint, PointScale, this.transform, null).name = $"TeeStartPoint_{TeeStartPoint.w}";
        //TransformHelper.ShowLocalPoint(TeeEndPoint, PointScale, this.transform, null).name = $"TeeEndPoint_{TeeEndPoint.w}";
        //TransformHelper.ShowLocalPoint(LineStartPoint, PointScale, this.transform, null).name = $"LineStartPoint_{LineStartPoint.w}";
        //TransformHelper.ShowLocalPoint(LineEndPoint, PointScale, this.transform, null).name = $"LineEndPoint_{LineEndPoint.w}";

        //GetPipeRadius();

        //ModelStartPoint = LineStartPoint;
        //ModelEndPoint = LineEndPoint;

        return data;
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}

public struct PipeTeeData
{


    public PipeModelKeyPointData4 KeyPointInfo;

    public PipeModelKeyPointData4 InnerKeyPointInfo;

    public PipeModelKeyPlaneData4 KeyPlaneInfo;
    public PipeModelKeyPlaneData4 InnerKeyPlaneInfo;

    [XmlAttribute]
    public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;

    public override string ToString()
    {
        return $"TeeData IsSpecial:{IsSpecial} IsGetInfoSuccess:{IsGetInfoSuccess} KeyPointInfo:{KeyPointInfo} InnerKeyPointInfo:{InnerKeyPointInfo} KeyPlaneInfo:{KeyPlaneInfo} InnerKeyPlaneInfo:{InnerKeyPlaneInfo}";
    }
}

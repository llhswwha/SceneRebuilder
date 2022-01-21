using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeWeldoletInfoJob : IPipeJob
{
    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.001f;
    public static float PipeLineOffset = 0.05f;
    public static float CircleWidth = 0.001f;

    public int id;
    public MeshStructure mesh;

    public static NativeArray<PipeWeldoletData> Result;

    public static PipeWeldoletData GetModelData(Mesh mesh, int minCount, float minDis, float circleWidth, object name)
    {
        MeshStructure ms = new MeshStructure(mesh);
        var data=GetModelData(ref ms, minCount, minDis, circleWidth, name);
        ms.Dispose();
        return data;
    }

    public static PipeWeldoletData GetModelData(ref MeshStructure mesh ,int minCount,float minDis,float circleWidth,object name)
    {
        DateTime start = DateTime.Now;

        PipeWeldoletData data = new PipeWeldoletData();

        

        var VertexCount = mesh.vertices.Length;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        SharedMeshTrianglesList keyPoints = meshTriangles.GetWeldoletKeyPoints(minCount, minDis);
        data.KeyPointCount = keyPoints.Count;
        if (keyPoints.Count == 3 || keyPoints.Count == 4)
        {
            var centerC = keyPoints[0];//Center
            var circleC = keyPoints[keyPoints.Count - 2];//Circle
            var rightC = keyPoints[keyPoints.Count-1];//Right

            centerC.SetCenterWithOutPoint(minRepeatPointDistance);

            Vector3 v1 = circleC.Center - rightC.Center;

            float centerDistance1 = Vector3.Distance(centerC.Center, circleC.Center);
            float centerDistance2 = Vector3.Distance(rightC.Center, circleC.Center);
            float centerOffset1 = circleC.Radius - centerDistance1;
            float centerOffset2 = centerDistance2 - circleC.Radius;
            //var p1 = circleCenter.GetCenter4();
            var p3 = rightC.GetCenter4();
            var p1 = rightC.GetCenter4WithOff(centerOffset2* v1.normalized);

            Debug.Log($"Weldolet radisu:{circleC.Radius} centerDistance1:{centerDistance1} centerDistance2:{centerDistance2} centerOffset1:{centerOffset1} :{centerOffset2}");

            //TransformHelper.ShowLocalPoint(p1, PointScale, this.transform, null);
            //TransformHelper.ShowLocalPoint(p3, PointScale, this.transform, null);

            data.KeyPointInfo = new PipeElbowKeyPointData(p3, p1, circleC.GetCenter4WithOff(circleWidth), circleC.GetCenter4WithOff(-circleWidth));

            data.IsGetInfoSuccess = true;
        }
        else if (keyPoints.Count == 2)
        {
            var circle2 = keyPoints[0];//Circle
            var circle3 = keyPoints[1];//Right

            var p1 = circle3.GetCenter4WithOff(CircleWidth);
            var p3 = circle3.GetCenter4WithPower(0.99f);

            //TransformHelper.ShowLocalPoint(p1, PointScale, this.transform, null);
            //TransformHelper.ShowLocalPoint(p3, PointScale, this.transform, null);

            data.KeyPointInfo = new PipeElbowKeyPointData(p3, p1, circle2.GetCenter4WithOff(circleWidth), circle2.GetCenter4WithOff(-circleWidth));

            data.IsGetInfoSuccess = true;
        }
        else
        {
            Debug.LogError($"PipeWeldoletInfoJob.GetModelData[{name}] keyPoints.Count Error time:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms VertexCount:{VertexCount} meshTriangles:{meshTriangles.Count} data:{data}");
            data.IsGetInfoSuccess = false;
        }

        Debug.Log($"PipeWeldoletInfoJob.GetModelData[{name}] time:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms VertexCount:{VertexCount} meshTriangles:{meshTriangles.Count} data:{data}");

        return data;
    }

    public void Execute()
    {
        PipeWeldoletData data = GetModelData(ref mesh,sharedMinCount,minRepeatPointDistance,CircleWidth,this.id);

        if (Result.Length > id)
        {
            Result[id] = data;
        }
        else
        {
            Debug.LogWarning($"PipeWeldoletInfoJob[{id}] Result.Length :{Result.Length }");
        }

        
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}


public struct PipeWeldoletData
{

    public PipeElbowKeyPointData KeyPointInfo;

    //public PipeElbowKeyPointData InnerKeyPointInfo;

    //public bool IsSpecial;

    public bool IsGetInfoSuccess;

    public int KeyPointCount;

    public override string ToString()
    {
        return $"PipeWeldoletData KeyPointCount:{KeyPointCount} IsGetInfoSuccess:{IsGetInfoSuccess} ";
    }
}
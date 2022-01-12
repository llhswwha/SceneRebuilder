using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeReducerModel
    //: PipeLineModel
    : PipeElbowModel
    //: PipeModelBase
{
    //public MeshTriangles meshTriangles;

    public Vector4 StartPoint = Vector3.zero;
    public Vector4 EndPoint = Vector3.zero;

    //public List<PlanePointDistance> distanceListEx;

    public override void GetModelInfo()
    {
        MinKeyPointCount = 2;
        //base.GetModelInfo();

        DateTime start = DateTime.Now;
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);

        distanceList = trianglesList.GetPlanePointDistanceList();

        for (int i = 0; i < distanceList.Count; i++)
        {
            var p = distanceList[i];
            TransformHelper.ShowLocalPoint(p.Plane.GetCenter(), PointScale, this.transform, null).name = $"KeyPoint[{i + 1}]";
        }

        if (trianglesList.Count != 2)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }



        SharedMeshTriangles startP = distanceList[0].Plane;
        StartPoint = startP.GetCenter4();

        PipeRadius1 = StartPoint.w;

        SharedMeshTriangles endP = distanceList[1].Plane;
        EndPoint = endP.GetCenter4();
        PipeRadius2 = EndPoint.w;

        PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        trianglesList.Remove(StartPoint);
        trianglesList.Remove(EndPoint);

        //EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, points);
        //points.Remove(EndPointOut1);
        //EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, points);
        //points.Remove(EndPointOut2);
        //KeyPointInfo = new PipeElbowKeyPointInfo();
        //KeyPointInfo.Line1 = new PipeLineInfo(StartPoint, EndPoint, null);
        //Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

        //TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
        //TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
        TransformHelper.ShowLocalPoint(StartPoint, PointScale, this.transform, null).name = "StartPoint";
        TransformHelper.ShowLocalPoint(EndPoint, PointScale, this.transform, null).name = "EndPoint";

        GetPipeRadius();

        IsGetInfoSuccess = true;

        ModelStartPoint = StartPoint;
        ModelEndPoint = EndPoint;

        meshTriangles.Dispose();

        Debug.Log($">>>GetReducerInfo time:{DateTime.Now - start} points:{trianglesList.Count} meshTriangles:{meshTriangles.Count} trianglesList:{trianglesList.Count} distanceList:{distanceList.Count}");
    }


    public override List<Vector4> GetModelKeyPoints()
    {
        //return base.GetModelKeyPoints();
        List<Vector4> list = new List<Vector4>();
        list.Add(GetModelStartPoint());
        list.Add(GetModelEndPoint());
        return list;
    }


    internal void SetModelData(PipeReducerData lineData)
    {
        this.IsSpecial = lineData.IsSpecial;
        this.IsGetInfoSuccess = lineData.IsGetInfoSuccess;
        this.StartPoint = lineData.StartPoint;
        this.EndPoint = lineData.EndPoint;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        PipeMeshGeneratorEx pipe = GetGenerator<PipeMeshGeneratorEx>(arg, afterName);
        pipe.points = new List<Vector4>() { StartPoint, EndPoint };

        //PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName);
        //pipe.points = new List<Vector3>() { StartPoint, EndPoint };
        arg.SetArg(pipe);
        pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius1 = PipeRadius1;
        pipe.pipeRadius2 = PipeRadius2;
        pipe.IsGenerateEndWeld = true;
        pipe.generateEndCaps = true;
        pipe.RenderPipe();

        return pipe.gameObject;
    }

    public override void GetPipeRadius()
    {
        PipeRadius = meshTriangles.GetPipeRadius(sharedMinCount);
    }
}

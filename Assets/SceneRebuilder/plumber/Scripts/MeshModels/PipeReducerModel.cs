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
        SharedMeshTrianglesList points = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);

        var centerOfPoints = MeshHelper.GetCenterOfList(points);
        distanceList = new List<PlanePointDistance>();
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            distanceList.Add(new PlanePointDistance(p, centerOfPoints));

            //TransformHelper.ShowLocalPoint(p.Point, PointScale, this.transform, null).name = $"KeyPoint[{i + 1}]";
        }
        distanceList.Sort();

        for (int i = 0; i < distanceList.Count; i++)
        {
            var p = distanceList[i];
            TransformHelper.ShowLocalPoint(p.Plane.GetCenter(), PointScale, this.transform, null).name = $"KeyPoint[{i + 1}]";
        }

        if (points.Count != 2)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count != 2 count:{points.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }



        SharedMeshTriangles startP = distanceList[0].Plane;
        StartPoint = startP.GetCenter();
        StartPoint.w = startP.GetRadius();

        PipeRadius1 = StartPoint.w;

        SharedMeshTriangles endP = distanceList[1].Plane;
        EndPoint = endP.GetCenter();
        EndPoint.w = endP.GetRadius();
        PipeRadius2 = EndPoint.w;

        PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

        points.Remove(StartPoint);
        points.Remove(EndPoint);

        //EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, points);
        //points.Remove(EndPointOut1);
        //EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, points);
        //points.Remove(EndPointOut2);

        Line1 = new PipeLineInfo(StartPoint, EndPoint, null);
        //Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

        //TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
        //TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
        TransformHelper.ShowLocalPoint(StartPoint, PointScale, this.transform, null).name = "StartPoint";
        TransformHelper.ShowLocalPoint(EndPoint, PointScale, this.transform, null).name = "EndPoint";

        GetPipeRadius();

        IsGetInfoSuccess = true;
        Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start} points:{points.Count}");
    }
    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        //GameObject pipeNew = new GameObject(this.name + afterName);
        //pipeNew.transform.position = this.transform.position + arg.Offset;
        //pipeNew.transform.SetParent(this.transform.parent);

        //PipeMeshGeneratorEx pipe = pipeNew.GetComponent<PipeMeshGeneratorEx>();
        //if (pipe == null)
        //{
        //    pipe = pipeNew.AddComponent<PipeMeshGeneratorEx>();
        //}
        //pipe.Target = this.gameObject;


        PipeMeshGeneratorEx pipe = GetGenerator<PipeMeshGeneratorEx>(arg, afterName);
        pipe.points = new List<Vector4>() { StartPoint, EndPoint };
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

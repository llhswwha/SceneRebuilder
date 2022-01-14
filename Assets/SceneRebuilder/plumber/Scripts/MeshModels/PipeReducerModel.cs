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
        SharedMeshTrianglesList trianglesList = GetSharedMeshTrianglesList(meshTriangles);

        distanceList = trianglesList.GetPlanePointDistanceList();

        for (int i = 0; i < distanceList.Count; i++)
        {
            var p = distanceList[i];
            TransformHelper.ShowLocalPoint(p.Plane.GetCenter(), PointScale, this.transform, null).name = $"KeyPoint[{i + 1}]";
        }

        //if (trianglesList.Count != 2)
        //{
        //    IsGetInfoSuccess = false;
        //    Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
        //    return;
        //}

        if(trianglesList.Count == 3)
        {
            if(GetModelInfo3(trianglesList) ==false)
            {
                IsGetInfoSuccess = false;
                Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
                return;
            }
        }
        else if(trianglesList.Count == 2)
        {
            SharedMeshTriangles startP = distanceList[0].Plane;
            StartPoint = startP.GetCenter4();

            PipeRadius1 = StartPoint.w;

            SharedMeshTriangles endP = distanceList[1].Plane;
            EndPoint = endP.GetCenter4();
            PipeRadius2 = EndPoint.w;

            PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

            trianglesList.Remove(StartPoint);
            trianglesList.Remove(EndPoint);

            TransformHelper.ShowLocalPoint(StartPoint, PointScale, this.transform, null).name = "StartPoint";
            TransformHelper.ShowLocalPoint(EndPoint, PointScale, this.transform, null).name = "EndPoint";

            GetPipeRadius();

            IsGetInfoSuccess = true;

            ModelStartPoint = StartPoint;
            ModelEndPoint = EndPoint;
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }

        meshTriangles.Dispose();

        Debug.Log($">>>{this.GetType().Name} time:{DateTime.Now - start} points:{trianglesList.Count} meshTriangles:{meshTriangles.Count} trianglesList:{trianglesList.Count} distanceList:{distanceList.Count}");
    }

    protected virtual bool GetModelInfo3(SharedMeshTrianglesList trianglesList)
    {
        return false;
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

        ModelStartPoint = StartPoint;
        ModelEndPoint = EndPoint;

        SetRadius();
    }

    protected virtual void SetRadius()
    {
        PipeRadius1 = StartPoint.w;
        PipeRadius2 = EndPoint.w;
        PipeRadius = (PipeRadius1 + PipeRadius2) / 2;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            this.gameObject.SetActive(true);
            return null;
        }

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

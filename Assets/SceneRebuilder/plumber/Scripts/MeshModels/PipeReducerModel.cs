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

    //public PipeLineInfo LineInfo = new PipeLineInfo();

    //public List<PlanePointDistance> distanceListEx;



    public override void GetModelInfo()
    {
        MinKeyPointCount = 2;
        //base.GetModelInfo();

        DateTime start = DateTime.Now;
        //ClearChildren();
        ClearDebugInfoGos();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = GetSharedMeshTrianglesList(meshTriangles);

        distanceList = trianglesList.GetPlanePointDistanceList();
        GameObject keyPoints = CreateKeyPointsGo();
        for (int i = 0; i < distanceList.Count; i++)
        {
            var p = distanceList[i];
            TransformHelper.ShowLocalPoint(p.Plane.GetCenter(), PointScale, keyPoints.transform, null).name = $"KeyPoint[{i + 1}]";
        }

        //if (trianglesList.Count != 2)
        //{
        //    IsGetInfoSuccess = false;
        //    Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
        //    return;
        //}

        if(trianglesList.Count >= 3)
        {
            KeyPointCount = trianglesList.Count;
            if (GetModelInfo3(trianglesList) ==false)
            {
                IsGetInfoSuccess = false;
                Debug.LogError($"GetKeyPointsById points.Count != 2 count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
                return;
            }
        }
        else if(trianglesList.Count == 2)
        {
            SharedMeshTriangles startP = distanceList[0].Plane;
            var StartPoint = startP.GetCenter4();

            PipeRadius1 = StartPoint.w;

            SharedMeshTriangles endP = distanceList[1].Plane;
            var EndPoint = endP.GetCenter4();
            PipeRadius2 = EndPoint.w;

            PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

            trianglesList.Remove(StartPoint);
            trianglesList.Remove(EndPoint);

            TransformHelper.ShowLocalPoint(StartPoint, PointScale, keyPoints.transform, null).name = "StartPoint";
            TransformHelper.ShowLocalPoint(EndPoint, PointScale, keyPoints.transform, null).name = "EndPoint";

            //GetPipeRadius();

            IsGetInfoSuccess = true;

            ModelStartPoint = StartPoint;
            ModelEndPoint = EndPoint;
            KeyPointCount = 2;
        }
        else
        {
            KeyPointCount = trianglesList.Count;
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

    public new PipeReducerData ModelData;


    internal void SetModelData(PipeReducerData lineData)
    {
        ModelData = lineData;

        this.IsSpecial = lineData.IsSpecial;
        this.IsGetInfoSuccess = lineData.IsGetInfoSuccess;
        this.KeyPointCount = lineData.KeyPointCount;
        this.StartPoint = lineData.StartPoint;
        this.EndPoint = lineData.EndPoint;
        this.KeyPointInfo = new PipeModelKeyPointInfo4(lineData.KeyPointInfo);

        ModelStartPoint = StartPoint;
        ModelEndPoint = EndPoint;

        SetRadius();
    }

    public new PipeReducerData GetModelData()
    {
        ModelData.KeyPointInfo = new PipeModelKeyPointData4(KeyPointInfo);
        ModelData.StartPoint = this.StartPoint;
        ModelData.EndPoint = this.EndPoint;
        ModelData.IsGetInfoSuccess = IsGetInfoSuccess;
        ModelData.IsSpecial = IsSpecial;
        ModelData.KeyPointCount = KeyPointCount;
        return ModelData;
    }

    public new PipeReducerSaveData GetSaveData()
    {
        PipeReducerSaveData data = new PipeReducerSaveData();
        InitSaveData(data);
        data.Data = GetModelData();
        //KeyPointInfo = null;
        //InnerKeyPointInfo = null;
        ////KeyPlaneInfo = null;
        return data;
    }

    public override void SetSaveData(MeshModelSaveData data)
    {
        //this.LineInfo = data.Info;
        SetModelData((data as PipeReducerSaveData).Data);
        //PipeFactory.Instance.RendererModelFromXml(this, data);
    }

    protected virtual void SetRadius()
    {
        PipeRadius1 = StartPoint.w;
        PipeRadius2 = EndPoint.w;
        PipeRadius = (PipeRadius1 + PipeRadius2) / 2;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (RendererErrorModel())
        {
            return null;
        }

        PipeMeshGeneratorEx pipe = GetGenerator<PipeMeshGeneratorEx>(arg, afterName,false);
        pipe.points = new List<Vector4>() { StartPoint, EndPoint };

        //PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName);
        //pipe.points = new List<Vector3>() { StartPoint, EndPoint };
        arg.SetArg(pipe);
        pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius1 = PipeRadius1;
        pipe.pipeRadius2 = PipeRadius2;
        pipe.IsGenerateEndWeld = true;
        pipe.generateEndCaps = true;
        //pipe.generateWeld = false;

        if (PipeRadius1 < 0.025 || PipeRadius2 < 0.025)
        {
            //pipe.weldRadius = 0.003f;
            pipe.weldPipeRadius = arg.weldRadius * 0.6f;
        }

        pipe.RenderPipe();

        pipe.transform.rotation = this.transform.rotation;

        return pipe.gameObject;
    }

    public override string GetDictKey()
    {
        return "";
    }

    public override Vector3 GetStartPoint()
    {
        return this.transform.TransformPoint(StartPoint);
    }

    public override  Vector3 GetEndPoint()
    {
        return this.transform.TransformPoint(EndPoint);
    }

    //public override void CreateBoxLine()
    //{
    //    TransformHelper.CreateBoxLine(GetStartPoint(), GetEndPoint(), PipeRadius * 2, this.name + "_BoxLine", this.transform.parent);
    //}


    //public override void GetPipeRadius()
    //{
    //    PipeRadius = meshTriangles.GetPipeRadius(sharedMinCount);
    //}
}

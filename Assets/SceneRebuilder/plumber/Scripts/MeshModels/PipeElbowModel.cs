using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PipeBuilder;

public class PipeElbowModel : PipeModelBase
{
    public Vector3 GetStartPoint()
    {
        return Vector3.zero;
    }

    public Vector3 GetEndPoint()
    {
        return Vector3.zero;
    }

    public MeshTriangles meshTriangles = new MeshTriangles();



    public List<PointDistance> distanceList;

    public int MinKeyPointCount = 4;

    // Start is called before the first frame update
    [ContextMenu("GetElbowInfo")]
    public override void GetModelInfo()
    {
        DateTime start = DateTime.Now;
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        List<Vector3> points = meshTriangles.GetKeyPointsById(sharedMinCount, minRepeatPointDistance);
        if (points.Count < MinKeyPointCount)
        {
            Debug.LogError($"GetKeyPointsById points.Count < {MinKeyPointCount} count:{points.Count}");
            return;
        }

        var centerOfPoints = MeshHelper.GetCenterOfList(points);
        distanceList = new List<PointDistance>();
        foreach (var p in points)
        {
            distanceList.Add(new PointDistance(p, centerOfPoints));
        }
        distanceList.Sort();

        EndPointIn1 = distanceList[0].P1;
        EndPointIn2 = distanceList[1].P1;
        points.Remove(EndPointIn1);
        points.Remove(EndPointIn2);

        EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, points);
        points.Remove(EndPointOut1);
        EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, points);
        points.Remove(EndPointOut2);

        Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1,null);
        Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

        TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
        TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
        TransformHelper.ShowLocalPoint(EndPointIn1, PointScale, this.transform, null).name = "InPoint1";
        TransformHelper.ShowLocalPoint(EndPointIn2, PointScale, this.transform, null).name = "InPoint2";

        GetPipeRadius();
        Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start}");
    }

    public virtual void GetPipeRadius()
    {
        PipeRadius = meshTriangles.GetPipeRadius(sharedMinCount);
    }

    public void ShowKeyPoints()
    {
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);
        meshTriangles.ShowKeyPointsById(this.transform, PointScale);
    }

    public void ShowSharedPoints()
    {
        ClearChildren();

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        meshTriangles.ShowSharedPointsById(this.transform, PointScale);
        meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale);
    }


    public void ShowTriangles()
    {
        ClearChildren();

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");

        Debug.Log($"GetElbowInfo trialges:{meshTriangles.Count}");
        for (int i = 0; i < meshTriangles.Count; i++)
        {
            var t = meshTriangles.GetTriangle(i);
            
            Debug.Log($"GetElbowInfo[{i + 1}/{meshTriangles.Count}] trialge:{t}");
            GameObject sharedPoints1Obj = CreateSubTestObj($"trialge:{t}", this.transform);
            t.ShowTriangle(this.transform, sharedPoints1Obj.transform, PointScale);
        }
    }

    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public void RendererModel()
    {
        RendererModel(generateArg,"_New");
    }

    public override GameObject RendererModel(PipeGenerateArg arg,string newAfterName)
    {
        PipeCreateArg pipeArg = new PipeCreateArg(Line1, Line2);
        var ps = pipeArg.GetGeneratePoints(0, 2, false);

        GameObject pipeNew = new GameObject(this.name + newAfterName);
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        arg.SetArg(pipe);

        //pipe.generateWeld = arg.generateWeld;
        pipe.generateWeld = false;
        pipe.pipeRadius = PipeRadius;
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipeNew;
    }

    public Vector3 EndPointIn1 = Vector3.zero;
    public Vector3 EndPointOut1 = Vector3.zero;
    public Vector3 EndPointIn2 = Vector3.zero;
    public Vector3 EndPointOut2= Vector3.zero;

    public PipeLineInfo Line1 = new PipeLineInfo();

    public PipeLineInfo Line2 = new PipeLineInfo();

    public int sharedMinCount = 36;

    public float minRepeatPointDistance = 0.00002f;

    public Vector3 TestMeshOffset = new Vector3(0.1f, 0.1f,0.1f);

    private GameObject CreateSubTestObj(string objName,Transform parent)
    {
        GameObject objTriangles = new GameObject(objName);
        objTriangles.transform.SetParent(parent);
        objTriangles.transform.localPosition = Vector3.zero;
        return objTriangles;
    }

    public float PointScale = 0.01f;

    public void OnDestroy()
    {
        Debug.Log($"OnDestroy {this.name}");
    }
}

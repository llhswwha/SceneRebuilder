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



    public List<PlanePointDistance> distanceList;

    public int MinKeyPointCount = 4;

    public float PipeLineOffset = 0.05f;

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
        SharedMeshTrianglesList points = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        points.RemoveNotCircle();
        if (points.Count == 4)
        {
            var centerOfPoints = MeshHelper.GetCenterOfList(points);
            distanceList = new List<PlanePointDistance>();
            foreach (var p in points)
            {
                distanceList.Add(new PlanePointDistance(p, centerOfPoints));
            }
            distanceList.Sort();

            EndPointIn1 = distanceList[0].Plane.GetCenter();
            EndPointIn2 = distanceList[1].Plane.GetCenter();
            points.Remove(EndPointIn1);
            points.Remove(EndPointIn2);

            EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, points);
            points.Remove(EndPointOut1);
            EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, points);
            points.Remove(EndPointOut2);

            Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
            Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

            TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
            TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
            TransformHelper.ShowLocalPoint(EndPointIn1, PointScale, this.transform, null).name = "InPoint1";
            TransformHelper.ShowLocalPoint(EndPointIn2, PointScale, this.transform, null).name = "InPoint2";

            GetPipeRadius();

            IsGetInfoSuccess = true;
            Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start}");
        }
        else if (points.Count == 2)
        {
            var centerOfPoints = MeshHelper.GetCenterOfList(points);
            distanceList = new List<PlanePointDistance>();
            foreach (var p in points)
            {
                distanceList.Add(new PlanePointDistance(p, centerOfPoints));
            }
            distanceList.Sort();

            SharedMeshTriangles startPlane = distanceList[0].Plane;
            SharedMeshTriangles endPlane = distanceList[1].Plane;

            var endPoint1= startPlane.GetCenter();
            var endPoint2= endPlane.GetCenter();

            var normal1 = mesh.normals[startPlane.PointId];
            var normal2 = mesh.normals[endPlane.PointId];
            points.Remove(endPoint1);
            points.Remove(endPoint2);

            //GetPipeRadius();

            PipeRadius1 = startPlane.GetRadius();
            PipeRadius2 = endPlane.GetRadius();
            PipeRadius = (PipeRadius1 + PipeRadius2) / 2;

            Vector3 crossPoint1;
            Vector3 crossPoint2;
            Math3D.ClosestPointsOnTwoLines(out crossPoint1, out crossPoint2, endPoint1, normal1, endPoint2, normal2);
            Vector3 crossPoint12 = 0.5f * (crossPoint1 + crossPoint2);

            //EndPointOut1 = EndPointIn1- normal1 * PipeRadius* PipeLineOffset;
            //EndPointOut2 = EndPointIn2+ normal2 * PipeRadius * PipeLineOffset;
            EndPointOut1 = endPoint1;
            EndPointOut2 = endPoint2;

            EndPointIn1 = endPoint1 + (crossPoint1-endPoint1).normalized * PipeRadius * PipeLineOffset;
            EndPointIn2 = endPoint2 + (crossPoint2-endPoint2).normalized * PipeRadius * PipeLineOffset;

            Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null, normal1);
            Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null, normal2);

            TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
            TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
            TransformHelper.ShowLocalPoint(EndPointIn1, PointScale, this.transform, null).name = "InPoint1";
            TransformHelper.ShowLocalPoint(EndPointIn2, PointScale, this.transform, null).name = "InPoint2";
            TransformHelper.ShowLocalPoint(crossPoint1, PointScale, this.transform, null).name = "crossPoint1";
            TransformHelper.ShowLocalPoint(crossPoint2, PointScale, this.transform, null).name = "crossPoint2";
            TransformHelper.ShowLocalPoint(crossPoint12, PointScale, this.transform, null).name = "crossPoint12";


            IsGetInfoSuccess = true;
            Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start}");
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetKeyPointsById points.Count Error count:{points.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }

        
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
        meshTriangles.ShowSharedPointsById(this.transform, PointScale,10);
        meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale,10);
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

    public float minRepeatPointDistance = 0.00005f;

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

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
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        foreach(SharedMeshTriangles triangles in trianglesList)
        {

        }
        trianglesList.RemoveNotCircle();
        if (trianglesList.Count == 4)
        {
            var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
            distanceList = new List<PlanePointDistance>();
            foreach (var p in trianglesList)
            {
                distanceList.Add(new PlanePointDistance(p, centerOfPoints));
            }
            distanceList.Sort();

            EndPointIn1 = distanceList[0].Plane.GetCenter();
            EndPointIn2 = distanceList[1].Plane.GetCenter();
            trianglesList.Remove(EndPointIn1);
            trianglesList.Remove(EndPointIn2);

            EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, trianglesList);
            trianglesList.Remove(EndPointOut1);
            EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, trianglesList);
            trianglesList.Remove(EndPointOut2);

            Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1, null);
            Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

            TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
            TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
            TransformHelper.ShowLocalPoint(EndPointIn1, PointScale, this.transform, null).name = "InPoint1";
            TransformHelper.ShowLocalPoint(EndPointIn2, PointScale, this.transform, null).name = "InPoint2";

            GetPipeRadius();

            EndPointIn1.w = PipeRadius;
            EndPointIn2.w = PipeRadius;
            EndPointOut1.w = PipeRadius;
            EndPointOut2.w = PipeRadius;
            ModelStartPoint = EndPointOut1;
            ModelEndPoint = EndPointOut2;

            IsGetInfoSuccess = true;
            Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start}");
        }
        else if (trianglesList.Count == 2)
        {
            var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
            distanceList = new List<PlanePointDistance>();
            foreach (var p in trianglesList)
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
            trianglesList.Remove(endPoint1);
            trianglesList.Remove(endPoint2);

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

            EndPointIn1.w = PipeRadius;
            EndPointIn2.w = PipeRadius;
            EndPointOut1.w = PipeRadius;
            EndPointOut2.w = PipeRadius;
            ModelStartPoint = EndPointOut1;
            ModelEndPoint = EndPointOut2;

            IsGetInfoSuccess = true;
            Debug.Log($">>>GetElbowInfo time:{DateTime.Now - start}");
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($">>>GetElbowInfo GetModelInfo points.Count Error count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
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

        GameObject trianglesObj = CreateSubTestObj($"KeyPoints", this.transform);

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);
        meshTriangles.ShowKeyPointsById(trianglesObj.transform, PointScale, sharedMinCount, minRepeatPointDistance);
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
        //GameObject pipeNew = new GameObject(this.name + newAfterName);
        //pipeNew.transform.position = this.transform.position + arg.Offset;
        //pipeNew.transform.SetParent(this.transform.parent);

        //PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        //if (pipe == null)
        //{
        //    pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        //}
        //pipe.Target = this.gameObject;

        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, newAfterName);
        PipeCreateArg pipeArg = new PipeCreateArg(Line1, Line2);
        var ps = pipeArg.GetGeneratePoints(0, 2, false);

        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        arg.SetArg(pipe);

        //pipe.generateWeld = arg.generateWeld;
        pipe.generateWeld = false;
        //pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius = (EndPointOut1.w+ EndPointOut2.w)/2;
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();

        return pipe.gameObject;
    }

    public Vector4 EndPointIn1 = Vector3.zero;
    public Vector4 EndPointOut1 = Vector3.zero;
    public Vector4 EndPointIn2 = Vector3.zero;
    public Vector4 EndPointOut2= Vector3.zero;

    public Vector4 GetEndPointIn1()
    {
        return this.TransformPoint(EndPointIn1);
    }

    public Vector4 GetEndPointIn2()
    {
        return this.TransformPoint(EndPointIn2);
    }

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

    public override List<Vector4> GetModelKeyPoints()
    {
        var list= base.GetModelKeyPoints();
        list.Add(GetEndPointIn1());
        list.Add(GetEndPointIn2());
        return list;
    }

    private void SetRadius(int id,float r)
    {
        if (id == 0)
        {
            EndPointOut1.w = r;
        }
        if (id == 1)
        {
            EndPointOut2.w = r;
        }
        if (id == 2)
        {
            EndPointIn1.w = r;
            EndPointOut1.w = r;
        }
        if (id == 3)
        {
            EndPointIn2.w = r;
            EndPointOut2.w = r;
        }
    }

    public override void SetUniformRadius(bool isUniformRaidus, Vector4 p2,int p1Id)
    {
        if (isUniformRaidus)
        {
            //p1.w = p2.w;//这个怎么保存回去呢？
            this.SetRadius(p1Id, p2.w);
            //this.PipeRadius = model2.PipeRadius;
            this.generateArg.generateEndCaps = false;
        }
        else
        {
            this.generateArg.generateEndCaps = true;
        }
    }

    //public override int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    //{
    //    base.ConnectedModel()
    //    var model1 = this;
    //    int ConnectedCount = 0;
    //    var points1 = model1.GetModelKeyPoints();
    //    var points2 = model2.GetModelKeyPoints();
    //    for (int i = 0; i < points1.Count; i++)
    //    {
    //        Vector4 p1 = points1[i];
    //        int cCount = 0;
    //        for (int i1 = 0; i1 < points2.Count; i1++)
    //        {
    //            Vector4 p2 = points2[i1];
    //            float dis12 = Vector3.Distance(p1, p2);

    //            if (isShowLog)
    //            {
    //                Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1:{p1} p2:{p2}");
    //            }

    //            if (dis12 < minPointDis)
    //            {
    //                if (Mathf.Abs(p1.w - p2.w) > minRadiusDis)
    //                {
    //                    Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1:{p1} p2:{p2}");
    //                }
    //                model1.AddConnectedModel(model2);
    //                model2.AddConnectedModel(model1);
    //                cCount++;

                    
    //            }
    //        }
    //        if (cCount == points2.Count)
    //        {
    //            Debug.LogError($"IsConnected Error1 cCount== points2.Count model1:{model1.name} model2:{model2.name} p1:{p1} points2:{points2.Count} cCount:{cCount}");
    //            //return -1;
    //        }
    //        ConnectedCount += cCount;
    //    }

    //    return ConnectedCount;
    //}
}

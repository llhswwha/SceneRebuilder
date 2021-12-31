using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PipeLineModel
/// </summary>
public class PipeLineModel : PipeModelBase
{

    public void ShowOBB()
    {
        OBBCollider oBB = this.gameObject.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = this.gameObject.AddComponent<OBBCollider>();
        }
        if (oBB != null)
        {
            oBB.ShowObbInfo();
        }
    }



    public PipeLineInfo LineInfo = new PipeLineInfo();

    public Vector3 GetStartPoint()
    {
        //return StartPoint + this.transform.position;
        //return StartPoint;
        LineInfo.transform = this.transform;
        return LineInfo.GetStartPoint();
    }

    public Vector3 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        LineInfo.transform = this.transform;
        return LineInfo.GetEndPoint();
    }

    public float PipeLength = 0;

    //public float SizeX = 0;
    //public float SizeY = 0;
    //public float SizeZ = 0;

    public Vector3 Size = Vector3.zero;

    public string sizeX;
    public string sizeY;
    public string sizeZ;

    public int SizeFloat = 4;

    public float PipeWidth = 0;

    public Vector3 P1 = Vector3.zero;
    public Vector3 P2 = Vector3.zero;
    public Vector3 P3 = Vector3.zero;
    public Vector3 P4 = Vector3.zero;
    public Vector3 P5 = Vector3.zero;
    public Vector3 P6 = Vector3.zero;

    public OrientedBoundingBox OBB;

    private GameObject CreateLocalPoint(Vector3 p, string n)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(this.transform);
        g1.transform.localPosition = p;
        g1.transform.localScale = new Vector3(lineSize, lineSize, lineSize);
        g1.name = n;
        return g1;
    }

    private GameObject CreateLocalPoint(Vector3 p, string n,Transform pT)
    {
        GameObject g1 = CreateLocalPoint(p, n);

        g1.transform.SetParent(pT);
        return g1;
    }

    //private Vector3 CreateLine(Vector3S p1, Vector3S p2, string n)
    //{
    //    return CreateLine(p1.GetVector3(), p2.GetVector3(), n);
    //}

    public float lineSize = 0.01f;

    //public void ShowVerticesToPlaneInfos()
    //{

    //}

    public override void GetModelInfo()
    {
        ClearChildren();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
        }
        oBBCollider.ShowObbInfo();
        IsGetInfoSuccess = oBBCollider.IsObbError == false;
        OBB = oBBCollider.OBB;

        Vector3 ObbExtent = OBB.Extent;

        Vector3 startPoint = OBB.Up * ObbExtent.y;
        Vector3 endPoint = -OBB.Up * ObbExtent.y;

        GameObject go = new GameObject("PipeModel_PlaneInfo");
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
 var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
        verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            oBBCollider.ShowPlaneInfo(plane, i, go, v2p);
            if (v2p.IsCircle() == false)
            {
                continue;
            }
            verticesToPlaneInfos.Add(v2p);
            
        }
        if (verticesToPlaneInfos.Count < 2)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetModelInfo verticesToPlaneInfos.Count < 2 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            return;
        }
        verticesToPlaneInfos.Sort();

        var startPlane = verticesToPlaneInfos[0];
        var endPlane= verticesToPlaneInfos[1];


        P1 = OBB.Right * ObbExtent.x;
        P2 = -OBB.Forward * ObbExtent.z;
        P3 = -OBB.Right * ObbExtent.x;
        P4 = OBB.Forward * ObbExtent.z;
        P5 = OBB.Up * ObbExtent.y;
        P6 = -OBB.Up * ObbExtent.y;
        Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);



        //List<Vector3> extentPoints = new List<Vector3>() { P1, P2, P3, P4, P5, P6 };
        //verticesToPointInfos = new List<VerticesToPointInfo>();
        //CreateLocalPoint(P1, $"P1_{GetVerticesToPointInfo(vs, P1, false)}", go.transform);
        //CreateLocalPoint(P2, $"P2_{GetVerticesToPointInfo(vs, P2, false)}", go.transform);
        //CreateLocalPoint(P3, $"P3_{GetVerticesToPointInfo(vs, P3, false)}", go.transform);
        //CreateLocalPoint(P4, $"P4_{GetVerticesToPointInfo(vs, P4, false)}", go.transform);
        //CreateLocalPoint(P5, $"P5_{GetVerticesToPointInfo(vs, P5, false)}", go.transform);
        //CreateLocalPoint(P6, $"P6_{GetVerticesToPointInfo(vs, P6, false)}", go.transform);
        //verticesToPointInfos.Sort();


        //VerticesToPointInfo planeCenterPointInfo1 ;
        //VerticesToPointInfo planeCenterPointInfo2;

        //float minDisOfSize = 0.0001f;
        //if (Mathf.Abs(Size.x - Size.y) <= minDisOfSize && Mathf.Abs(Size.x - Size.z) > minDisOfSize)
        //{
        //    startPoint = P2;
        //    endPoint = P4;

        //    planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
        //    planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
        //    Debug.Log("Route1 P2 P4");
        //}
        //else if (Mathf.Abs(Size.x - Size.z) <= minDisOfSize && Mathf.Abs(Size.x - Size.y) > minDisOfSize)
        //{
        //    startPoint = P5;
        //    endPoint = P6;

        //    planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
        //    planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
        //    Debug.Log("Route2 P5 P6");
        //}
        //else if (Mathf.Abs(Size.y - Size.z) <= minDisOfSize && Mathf.Abs(Size.y - Size.x) > minDisOfSize)
        //{
        //    startPoint = P1;
        //    endPoint = P3;

        //    planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
        //    planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
        //    Debug.Log("Route3 P1 P3");
        //}
        //else
        //{
        //    Debug.Log("Route4 NotEqual");
        //    planeCenterPointInfo1 = verticesToPointInfos[0];
        //    planeCenterPointInfo2 = verticesToPointInfos[1];

        //    //var startCircle = planeCenterPointInfos[0].GetCircleInfo();
        //    //startPoint = startCircle.Center;
        //    //var endCircle = planeCenterPointInfos[1].GetCircleInfo();
        //    //endPoint = endCircle.Center;

        //    //EndPoints = new List<Vector3>() { startPoint, endPoint };

        //    //CreateLocalPoint(startPoint, "StartPoint1", go.transform);
        //    //CreateLocalPoint(endPoint, "EndPoint1", go.transform);

        //    //PipeRadius = (startCircle.Radius + endCircle.Radius) / 2;
        //    //PipeLength = Vector3.Distance(startPoint, endPoint);
        //    //LineInfo.StartPoint = startPoint;
        //    //LineInfo.EndPoint = endPoint;
        //}

        var startCircle = startPlane.GetCircleInfo();
        if (startCircle == null)
        {
            Debug.LogError($"GetModelInfo startCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;

            CreateLocalPoint(startPlane.Point.planeCenter, "StartPoint1", go.transform);
            CreateLocalPoint(endPlane.Point.planeCenter, "EndPoint1", go.transform);
            return;
        }
        startPoint = startCircle.Center;
        var endCircle = endPlane.GetCircleInfo();
        if (endCircle == null)
        {
            Debug.LogError($"GetModelInfo endCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;
            CreateLocalPoint(startPlane.Point.planeCenter, "StartPoint2", go.transform);
            CreateLocalPoint(endPlane.Point.planeCenter, "EndPoint2", go.transform);
            return;
        }
        endPoint = endCircle.Center;

        PipeRadius1 = startCircle.Radius;
        PipeRadius2 = endCircle.Radius;

        //优化
        //PlaneCenterPointInfo planeCenterPointInfo12 = new PlaneCenterPointInfo(vs, startCircle.Center, false);
        //PlaneCenterPointInfo planeCenterPointInfo22 = new PlaneCenterPointInfo(vs, endCircle.Center, false);
        //var startCircle2 = planeCenterPointInfo12.GetCircleInfo();
        //startPoint = startCircle.Center;
        //var endCircle2 = planeCenterPointInfo22.GetCircleInfo();
        //endPoint = endCircle.Center;
        //PipeRadius1 = startCircle2.Radius;
        //PipeRadius2 = endCircle2.Radius;

        EndPoints = new List<Vector3>() { startPoint, endPoint };

        CreateLocalPoint(startPoint, "StartPoint1", go.transform);
        CreateLocalPoint(endPoint, "EndPoint1", go.transform);

        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }

        //PipeRadius = (startCircle.Radius + endCircle.Radius) / 2;


        PipeLength = Vector3.Distance(startPoint, endPoint);
        LineInfo.StartPoint = startPoint;
        LineInfo.EndPoint = endPoint;

    }
    public void GetModelInfo_OLD()
    {
        ClearChildren();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();

            //var mf = this.gameObject.GetComponent<MeshFilter>();
            //for(int i=0;i<mf.sharedMesh.vertices.Length;i++)
            //{
            //    var v = mf.sharedMesh.vertices[i];
            //    PointHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
            //    var m = mf.sharedMesh.normals[i];
            //    //if (!sharedNormals.Contains(m))
            //    //{
            //    //    sharedNormals.Add(m);
            //    //}
            //}

        }
        oBBCollider.ShowObbInfo();
        IsGetInfoSuccess = oBBCollider.IsObbError == false;
        OBB = oBBCollider.OBB;

        Vector3 ObbExtent = OBB.Extent;

        Vector3 startPoint = OBB.Up * ObbExtent.y;
        Vector3 endPoint = -OBB.Up * ObbExtent.y;

        GameObject go = new GameObject("PipeModel_PipeInfo");
        go.transform.SetParent(this.transform);
        go.transform.position = Vector3.zero;

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);

        //Plane plane1=OBB.FacePlane(1);

        P1 = OBB.Right * ObbExtent.x;
        P2 = -OBB.Forward * ObbExtent.z;
        P3 = -OBB.Right * ObbExtent.x;
        P4 = OBB.Forward * ObbExtent.z;
        P5 = OBB.Up * ObbExtent.y;
        P6 = -OBB.Up * ObbExtent.y;
        Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        List<Vector3> extentPoints = new List<Vector3>() { P1, P2, P3, P4, P5, P6 };
        verticesToPointInfos = new List<VerticesToPointInfo>();
        CreateLocalPoint(P1, $"P1_{GetVerticesToPointInfo(vs, P1, false)}", go.transform);
        CreateLocalPoint(P2, $"P2_{GetVerticesToPointInfo(vs, P2, false)}", go.transform);
        CreateLocalPoint(P3, $"P3_{GetVerticesToPointInfo(vs, P3, false)}", go.transform);
        CreateLocalPoint(P4, $"P4_{GetVerticesToPointInfo(vs, P4, false)}", go.transform);
        CreateLocalPoint(P5, $"P5_{GetVerticesToPointInfo(vs, P5, false)}", go.transform);
        CreateLocalPoint(P6, $"P6_{GetVerticesToPointInfo(vs, P6, false)}", go.transform);
        verticesToPointInfos.Sort();


        VerticesToPointInfo planeCenterPointInfo1 ;
        VerticesToPointInfo planeCenterPointInfo2;

        float minDisOfSize = 0.0001f;
        if (Mathf.Abs(Size.x-Size.y)<= minDisOfSize && Mathf.Abs(Size.x-Size.z) > minDisOfSize)
        {
            startPoint = P2;
            endPoint = P4;

            planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
            planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
            Debug.Log("Route1 P2 P4");
        }
        else if (Mathf.Abs(Size.x - Size.z) <= minDisOfSize && Mathf.Abs(Size.x - Size.y) > minDisOfSize)
        {
            startPoint = P5;
            endPoint = P6;

            planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
            planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
            Debug.Log("Route2 P5 P6");
        }
        else  if (Mathf.Abs(Size.y - Size.z) <= minDisOfSize && Mathf.Abs(Size.y - Size.x) > minDisOfSize)
        {
            startPoint = P1;
            endPoint = P3;

            planeCenterPointInfo1 = new VerticesToPointInfo(vs, startPoint, false);
            planeCenterPointInfo2 = new VerticesToPointInfo(vs, endPoint, false);
            Debug.Log("Route3 P1 P3");
        }
        else
        {
            Debug.Log("Route4 NotEqual");
            planeCenterPointInfo1 = verticesToPointInfos[0];
            planeCenterPointInfo2 = verticesToPointInfos[1];

            //var startCircle = planeCenterPointInfos[0].GetCircleInfo();
            //startPoint = startCircle.Center;
            //var endCircle = planeCenterPointInfos[1].GetCircleInfo();
            //endPoint = endCircle.Center;

            //EndPoints = new List<Vector3>() { startPoint, endPoint };

            //CreateLocalPoint(startPoint, "StartPoint1", go.transform);
            //CreateLocalPoint(endPoint, "EndPoint1", go.transform);

            //PipeRadius = (startCircle.Radius + endCircle.Radius) / 2;
            //PipeLength = Vector3.Distance(startPoint, endPoint);
            //LineInfo.StartPoint = startPoint;
            //LineInfo.EndPoint = endPoint;
        }

        var startCircle = planeCenterPointInfo1.GetCircleInfo();
        if (startCircle == null)
        {
            Debug.LogError($"GetModelInfo startCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;

            CreateLocalPoint(planeCenterPointInfo1.Point, "StartPoint1", go.transform);
            CreateLocalPoint(planeCenterPointInfo2.Point, "EndPoint1", go.transform);
            return;
        }
        startPoint = startCircle.Center;
        var endCircle = planeCenterPointInfo2.GetCircleInfo();
        if (endCircle == null)
        {
            Debug.LogError($"GetModelInfo endCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;
            CreateLocalPoint(planeCenterPointInfo1.Point, "StartPoint2", go.transform);
            CreateLocalPoint(planeCenterPointInfo2.Point, "EndPoint2", go.transform);
            return;
        }
        endPoint = endCircle.Center;

        PipeRadius1 = startCircle.Radius;
        PipeRadius2 = endCircle.Radius;

        //优化
        //PlaneCenterPointInfo planeCenterPointInfo12 = new PlaneCenterPointInfo(vs, startCircle.Center, false);
        //PlaneCenterPointInfo planeCenterPointInfo22 = new PlaneCenterPointInfo(vs, endCircle.Center, false);
        //var startCircle2 = planeCenterPointInfo12.GetCircleInfo();
        //startPoint = startCircle.Center;
        //var endCircle2 = planeCenterPointInfo22.GetCircleInfo();
        //endPoint = endCircle.Center;
        //PipeRadius1 = startCircle2.Radius;
        //PipeRadius2 = endCircle2.Radius;

        EndPoints = new List<Vector3>() { startPoint, endPoint };

        CreateLocalPoint(startPoint, "StartPoint1", go.transform);
        CreateLocalPoint(endPoint, "EndPoint1", go.transform);

        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }

        //PipeRadius = (startCircle.Radius + endCircle.Radius) / 2;


        PipeLength = Vector3.Distance(startPoint, endPoint);
        LineInfo.StartPoint = startPoint;
        LineInfo.EndPoint = endPoint;

    }

    public List<Vector3> EndPoints = new List<Vector3>();



    public List<VerticesToPointInfo> verticesToPointInfos = new List<VerticesToPointInfo>();

    private VerticesToPointInfo GetVerticesToPointInfo(Vector3[] vs,Vector3 p,bool isShowLog)
    {
        VerticesToPointInfo verticesToPointInfo = new VerticesToPointInfo(vs, p, isShowLog);
        verticesToPointInfos.Add(verticesToPointInfo);
        return verticesToPointInfo;
    }

    public List<VerticesToPlaneInfo> verticesToPlaneInfos = new List<VerticesToPlaneInfo>();

    private VerticesToPlaneInfo GetVerticesToPlaneInfo(Vector3[] vs, PlaneInfo p, bool isShowLog)
    {
        VerticesToPlaneInfo verticesToPlaneInfo = new VerticesToPlaneInfo(vs, p, isShowLog);
        //verticesToPlaneInfos.Add(verticesToPlaneInfo);
        return verticesToPlaneInfo;
    }

    public void RendererModel()
    {
        GetModelInfo();

        RendererModel(this.generateArg,"_New");
    }

    public override GameObject RendererModel(PipeGenerateArg arg,string afterName)
    {
        GameObject pipeNew = new GameObject(this.name + afterName);
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = new List<Vector3>() { LineInfo.StartPoint, LineInfo.EndPoint };
        arg.SetArg(pipe);
        pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius1 = PipeRadius;
        pipe.pipeRadius2 = PipeRadius;
        pipe.IsGenerateEndWeld = true;
        pipe.RenderPipe();
        return pipeNew;
    }

    public void CreateWeld()
    {
        GameObject go = new GameObject("WeldPoints");
        go.transform.SetParent(this.transform);
        go.transform.position = Vector3.zero;

        ClearChildren();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
            oBBCollider.ShowObbInfo();
        }
        OBB = oBBCollider.OBB;

        Vector3 ObbExtent = OBB.Extent;

        Vector3 startPoint = OBB.Up * ObbExtent.y;
        Vector3 endPoint = -OBB.Up * ObbExtent.y;

        CreateLocalPoint(startPoint, "StartPoint2", go.transform);
        CreateLocalPoint(endPoint, "EndPoint2", go.transform);

        P1 = OBB.Right * ObbExtent.x ;
        P2 = -OBB.Forward * ObbExtent.z;
        P3 = -OBB.Right * ObbExtent.x;
        P4 = OBB.Forward * ObbExtent.z;

        CreateLocalPoint(P1, "P1", go.transform);
        CreateLocalPoint(P2, "P2", go.transform);
        CreateLocalPoint(P3, "P3", go.transform);
        CreateLocalPoint(P4, "P4", go.transform);

        float p = 1.414213562373f;
        CreateLocalPoint(P1 * p, "P11", go.transform);
        CreateLocalPoint(P2 * p, "P22", go.transform);
        CreateLocalPoint(P3 * p, "P33", go.transform);
        CreateLocalPoint(P4 * p, "P44", go.transform);

        GameObject pipeNew = new GameObject(this.name + "_NewWeld");
        pipeNew.transform.position = this.transform.position + generateArg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = new List<Vector3>() { P1, P2, P3, P4 };
        pipe.pipeSegments = generateArg.pipeSegments;
        //pipe.pipeMaterial = this.PipeMaterial;
        //pipe.weldMaterial = this.WeldMaterial;
        pipe.weldRadius = generateArg.weldRadius;
        pipe.elbowRadius = Vector3.Distance(P1, P2)/2;
        pipe.IsLinkEndStart = true;
        pipe.generateWeld = false;
        pipe.pipeRadius = generateArg.weldRadius;
        pipe.RenderPipe();
    }

    //public Vector3[] vertices;
    //public Vector3[] normals;
    //public Vector4[] tangents;
    //public int[] triangles;
    //public List<Vector3> sharedNormals;
    //[ContextMenu("ShowVertex")]
    //public void ShowVertex()
    //{
    //    //Dictionary<Vector3,List<Vector3>> 
    //    MeshFilter mf = this.GetComponent<MeshFilter>();
    //    vertices = mf.sharedMesh.vertices;
    //    normals = mf.sharedMesh.normals;
    //    tangents = mf.sharedMesh.tangents;
    //    triangles= mf.sharedMesh.triangles;
    //    sharedNormals = new List<Vector3>();
    //    ClearChildren();
    //    for (int i=0;i<mf.sharedMesh.vertices.Length;i++)
    //    {
    //        var v = mf.sharedMesh.vertices[i];
    //        VertexHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
    //        var m = mf.sharedMesh.normals[i];
    //        if(!sharedNormals.Contains(m))
    //        {
    //            sharedNormals.Add(m);
    //        }
    //    }
    //}
}

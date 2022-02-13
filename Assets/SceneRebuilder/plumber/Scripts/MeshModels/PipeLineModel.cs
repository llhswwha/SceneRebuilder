using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// PipeLineModel
/// </summary>
public class PipeLineModel : PipeModelBase
{
    public void ShowOBB()
    {
        OBBCollider.ShowOBB(this.gameObject,true);
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


    public override string GetPipeArgString()
    {
        if (PipeRadius == 0)
        {
            SetRadius();
        }
        PipeRadius = GetRadiusValue(PipeRadius);
        PipeRadius1 = GetRadiusValue(PipeRadius1);
        PipeRadius2 = GetRadiusValue(PipeRadius2);

        if (PipeLength == 0)
        {
            PipeLength = Vector3.Distance(ModelStartPoint, ModelEndPoint);
        }
        if (KeyPointCount == 0)
        {
            KeyPointCount = 2;
        }
        return $"Radius:{PipeRadius}({PipeRadius1},{PipeRadius1}) Length:{PipeLength}  Keys:{KeyPointCount} V:{VertexCount}";
    }

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

    public void GetModelInfoJob()
    {
        ObbInfoJob obbJob = ObbInfoJob.InitJob(this.gameObject, 1);
        obbJob.Execute();

        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();

        PipeLineInfoJob job = new PipeLineInfoJob()
        {
            id = 0,points=new NativeArray<Vector3>(vs, Allocator.TempJob),
        };
        job.Execute();

        LineInfo = new PipeLineInfo(job.lineData,this.transform);

        //ClearChildren();
        ClearDebugInfoGos();
        var keyPoints = CreateKeyPointsGo();
        CreateLocalPoint(LineInfo.StartPoint, $"StartPoint1_{LineInfo.StartPoint.w}", keyPoints.transform);
        CreateLocalPoint(LineInfo.EndPoint, $"EndPoint1_{LineInfo.EndPoint.w}", keyPoints.transform);
    }

    //private static void GetKeyPoints(OrientedBoundingBox OBB,GameObject go)
    //{
    //    Vector3 ObbExtent = OBB.Extent;

    //    Vector3 startPoint = OBB.Up * ObbExtent.y;
    //    Vector3 endPoint = -OBB.Up * ObbExtent.y;

    //    GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
    //    planInfoRoot.transform.SetParent(go.transform);
    //    planInfoRoot.transform.localPosition = Vector3.zero;

    //    //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
    //    //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
    //    var rendererInfo = MeshRendererInfo.GetInfo(go.gameObject);
    //    Vector3[] vs = rendererInfo.GetVertices();
    //    var VertexCount = vs.Length;

    //    //2.Planes
    //    PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
    //    List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
    //    var verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
    //    for (int i = 0; i < planeInfos.Length; i++)
    //    {
    //        PlaneInfo plane = (PlaneInfo)planeInfos[i];
    //        //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
    //        VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false);
    //        verticesToPlaneInfos_All.Add(v2p);
    //        //oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);
    //        if (v2p.IsCircle() == false)
    //        {
    //            continue;
    //        }
    //        verticesToPlaneInfos.Add(v2p);
    //        var isC = v2p.IsCircle();
    //    }
    //    verticesToPlaneInfos.Sort();

    //    if (verticesToPlaneInfos.Count < 1)
    //    {
    //        //IsGetInfoSuccess = false;
    //        Debug.LogError($"GetModelInfo verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{go.name}");
    //        return;
    //    }

    //    VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
    //    VerticesToPlaneInfo endPlane = null;
    //    if (verticesToPlaneInfos.Count >= 2)
    //    {
    //        endPlane = verticesToPlaneInfos[1];
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"GetModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{go.name}");
    //        endPlane = GetEndPlane(startPlane, verticesToPlaneInfos_All);
    //    }


    //    var P1 = OBB.Right * ObbExtent.x;
    //    var P2 = -OBB.Forward * ObbExtent.z;
    //    var P3 = -OBB.Right * ObbExtent.x;
    //    var P4 = OBB.Forward * ObbExtent.z;
    //    var P5 = OBB.Up * ObbExtent.y;
    //    var P6 = -OBB.Up * ObbExtent.y;
    //    var Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

    //    CircleInfo startCircle = startPlane.GetCircleInfo();
    //    if (startCircle == null)
    //    {
    //        Debug.LogError($"GetModelInfo startCircle == null gameObject:{go.gameObject.name}");
    //        //IsGetInfoSuccess = false;

    //        //CreateLocalPoint(startPlane.Point.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
    //        //CreateLocalPoint(endPlane.Point.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
    //        return;
    //    }
    //    startPoint = startCircle.Center;
    //    CircleInfo endCircle = endPlane.GetCircleInfo();
    //    if (endCircle == null)
    //    {
    //        Debug.LogError($"GetModelInfo endCircle == null gameObject:{go.gameObject.name}");
    //        //IsGetInfoSuccess = false;
    //        //CreateLocalPoint(startPlane.Point.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
    //        //CreateLocalPoint(endPlane.Point.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
    //        return;
    //    }
    //    endPoint = endCircle.Center;

    //    var PipeRadius1 = startCircle.Radius;
    //    var PipeRadius2 = endCircle.Radius;

    //    var EndPoints = new List<Vector3>() { startPoint, endPoint };

    //    //CreateLocalPoint(startPoint, $"StartPoint1_{startCircle.Radius}_{startCircle.Points.Count}", planInfoRoot.transform);
    //    //CreateLocalPoint(endPoint, $"EndPoint1_{endCircle.Radius}_{endCircle.Points.Count}", planInfoRoot.transform);

    //    var PipeRadius = 0f;
    //    if (PipeRadius1 > PipeRadius2)
    //    {
    //        PipeRadius = PipeRadius1;
    //    }
    //    else
    //    {
    //        PipeRadius = PipeRadius2;
    //    }

    //    var PipeLength = Vector3.Distance(startPoint, endPoint);
    //    PipeLineInfo LineInfo = new PipeLineInfo(startPoint, endPoint);
    //    //LineInfo.StartPoint = startPoint;
    //    //LineInfo.EndPoint = endPoint;

    //    Vector4 ModelStartPoint = startPoint;
    //    ModelStartPoint.w = PipeRadius;
    //    Vector4 ModelEndPoint = endPoint;
    //    ModelEndPoint.w = PipeRadius;
    //}



    public override void GetModelInfo()
    {
        //ClearChildren();

        ClearDebugInfoGos();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
        }
        oBBCollider.ShowObbInfo(true);
        IsObbError = oBBCollider.IsObbError;
        OBB = oBBCollider.OBB;

        //1.Obb

        Vector3 ObbExtent = OBB.Extent;

        Vector4 startPoint = OBB.Up * ObbExtent.y;
        Vector4 endPoint = -OBB.Up * ObbExtent.y;

        GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        planInfoRoot.AddComponent<DebugInfoRoot>();
        planInfoRoot.transform.SetParent(this.transform);
        planInfoRoot.transform.localPosition = Vector3.zero;

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
 var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        //2.Planes
        PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false);
            verticesToPlaneInfos_All.Add(v2p);
            oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);
            if (v2p.IsCircle() == false)
            {
                continue;
            }
            verticesToPlaneInfos.Add(v2p);
            var isC = v2p.IsCircle();
        }
        verticesToPlaneInfos.Sort();

        if (verticesToPlaneInfos.Count < 1)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"PipeLine.GetModelInfo verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            return;
        }

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >=2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"PipeLine.GetModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            endPlane = GetEndPlane(startPlane, verticesToPlaneInfos_All);
        }


        P1 = OBB.Right * ObbExtent.x;
        P2 = -OBB.Forward * ObbExtent.z;
        P3 = -OBB.Right * ObbExtent.x;
        P4 = OBB.Forward * ObbExtent.z;
        P5 = OBB.Up * ObbExtent.y;
        P6 = -OBB.Up * ObbExtent.y;
        Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        CircleInfo startCircle = startPlane.GetCircleInfo();
        if (startCircle == null)
        {
            Debug.LogError($"PipeLine.GetModelInfo startCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;

            CreateLocalPoint(startPlane.Point.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Point.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return;
        }
        startPoint = startCircle.GetCenter4();
        CircleInfo endCircle = endPlane.GetCircleInfo();
        if (endCircle == null)
        {
            Debug.LogError($"PipeLine.GetModelInfo endCircle == null gameObject:{this.gameObject.name}");
            IsGetInfoSuccess = false;
            CreateLocalPoint(startPlane.Point.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Point.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
            return;
        }
        endPoint = endCircle.GetCenter4();

        PipeRadius1 = startCircle.Radius;
        PipeRadius2 = endCircle.Radius;

        EndPoints = new List<Vector3>() { startPoint, endPoint };

        CreateLocalPoint(startPoint, $"StartPoint1_{startCircle.Radius}_{startCircle.Points.Count}", planInfoRoot.transform);
        CreateLocalPoint(endPoint, $"EndPoint1_{endCircle.Radius}_{endCircle.Points.Count}", planInfoRoot.transform);

        if (PipeRadius1 > PipeRadius2)
        {
            PipeRadius = PipeRadius1;
        }
        else
        {
            PipeRadius = PipeRadius2;
        }

        PipeLength = Vector3.Distance(startPoint, endPoint);
        //LineInfo.StartPoint = startPoint;
        //LineInfo.EndPoint = endPoint;
        LineInfo = new PipeLineInfo(startPoint, endPoint, this.transform);

        ModelStartPoint = startPoint;
        ModelStartPoint.w = PipeRadius;
        ModelEndPoint = endPoint;
        ModelEndPoint.w = PipeRadius;

        PipeLength = Vector3.Distance(startPoint, endPoint);
        KeyPointCount = 2;
    }

    public PipeLineData ModelData ;

    public void SetModelData(PipeLineData data)
    {
        ModelData = data;

        LineInfo = new PipeLineInfo(data, this.transform);
        ModelStartPoint = LineInfo.StartPoint;
        ModelEndPoint = LineInfo.EndPoint;
        SetRadius();
        PipeLength = Vector3.Distance(ModelStartPoint, ModelEndPoint);
        KeyPointCount = 2;

        //this.IsObbError = data.IsObbError;
        this.IsGetInfoSuccess = data.IsGetInfoSuccess;
        this.IsObbError = data.IsObbError;
        //Debug.Log($"SetLineData data:{data}");
    }

    public PipeLineData GetModelData()
    {
        ModelData.StartPoint = LineInfo.StartPoint;
        ModelData.EndPoint = LineInfo.EndPoint;
        ModelData.Direction = LineInfo.Direction;
        ModelData.IsGetInfoSuccess = IsGetInfoSuccess;
        ModelData.IsObbError = IsObbError;
        return ModelData;
    }

    public PipeLineSaveData GetSaveData()
    {
        PipeLineSaveData data = new PipeLineSaveData();
        InitSaveData(data);
        data.Data = GetModelData();
        //LineInfo = null;
        return data;
    }

    public override void SetSaveData(MeshModelSaveData data)
    {
        //this.LineInfo = data.Info;
        SetModelData((data as PipeLineSaveData).Data);
        //PipeFactory.Instance.RendererModelFromXml(this,data);
    }



    private static VerticesToPlaneInfo GetEndPlane(VerticesToPlaneInfo startPlane,List<VerticesToPlaneInfo> verticesToPlaneInfos_All)
    {
        VerticesToPlaneInfo endPlane = null;
        if (startPlane == verticesToPlaneInfos_All[0])
        {
            endPlane = verticesToPlaneInfos_All[1];
        }
        if (startPlane == verticesToPlaneInfos_All[1])
        {
            endPlane = verticesToPlaneInfos_All[0];
        }
        if (startPlane == verticesToPlaneInfos_All[2])
        {
            endPlane = verticesToPlaneInfos_All[3];
        }
        if (startPlane == verticesToPlaneInfos_All[3])
        {
            endPlane = verticesToPlaneInfos_All[2];
        }
        if (startPlane == verticesToPlaneInfos_All[4])
        {
            endPlane = verticesToPlaneInfos_All[5];
        }
        if (startPlane == verticesToPlaneInfos_All[5])
        {
            endPlane = verticesToPlaneInfos_All[4];
        }
        return endPlane;
    }

    public void GetModelInfo_OLD()
    {
        //ClearChildren();
        ClearDebugInfoGos();
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
        oBBCollider.ShowObbInfo(true);
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

        //�Ż�
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
        //LineInfo.StartPoint = startPoint;
        //LineInfo.EndPoint = endPoint;

        LineInfo = new PipeLineInfo(startPoint, endPoint, this.transform);
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

    //private VerticesToPlaneInfo GetVerticesToPlaneInfo(Vector3[] vs, PlaneInfo p, bool isShowLog)
    //{
    //    VerticesToPlaneInfo verticesToPlaneInfo = new VerticesToPlaneInfo(vs, p, isShowLog);
    //    //verticesToPlaneInfos.Add(verticesToPlaneInfo);
    //    return verticesToPlaneInfo;
    //}

    //public void RendererModel()
    //{
    //    //GetModelInfo();

    //    RendererModel(this.generateArg,"_New");
    //}

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            Debug.LogWarning($"PipeLineModel.RendererModel IsGetInfoSuccess == false model:{this.name}");
            return null;
        }

        if (RendererErrorModel())
        {
            return null;
        }

        //GameObject pipeNew = new GameObject(this.name + afterName);
        //pipeNew.transform.position = this.transform.position + arg.Offset;
        //pipeNew.transform.SetParent(this.transform.parent);

        //PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        //if (pipe == null)
        //{
        //    pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        //}
        //pipe.Target = this.gameObject;

        if (LineInfo == null)
        {
            Debug.LogError($"PipeLineModel.RendererModel LineInfo == null model:{this.name}");
            return null;
        }



        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName,false);

        if (pipe == null)
        {
            Debug.LogError($"PipeLineModel.RendererModel pipe == null model:{this.name}");
            return null;
        }

        pipe.points = new List<Vector3>() { LineInfo.StartPoint, LineInfo.EndPoint };
        arg.SetArg(pipe);
        var radius = (LineInfo.StartPoint.w + LineInfo.EndPoint.w) / 2;
        pipe.pipeRadius = radius;
        pipe.pipeRadius1 = radius;
        pipe.pipeRadius2 = radius;
        pipe.IsGenerateEndWeld = true;
        pipe.generateEndCaps = false;

        if (radius < 0.01)
        {
            //pipe.weldRadius = 0.003f;
            pipe.weldPipeRadius = arg.weldRadius * 0.6f;
        }

        pipe.RenderPipe();



        return pipe.gameObject;
    }

    public override void AddConnectedModel(PipeModelBase other)
    {
        //if (this.ConnectedModels.Contains(other))
        //{
        //    return;
        //}
        if(other is PipeLineModel)
        {
            PipeLineModel pipeLine2 = other as PipeLineModel;
            var dir1 = this.LineInfo.Direction;
            var dir2 = pipeLine2.LineInfo.Direction;
            float dot = Vector3.Dot(dir1, dir2);
            float angle2= Vector3.Angle(dir1, dir2);
            
            if(angle2<0.00001f|| Mathf.Abs(angle2 - 180) < 0.00001f)
            {
                base.AddConnectedModel(other);
            }
            else if(angle2 < 1f || Mathf.Abs(angle2 - 180) < 1f)
            {
                Debug.LogWarning($"AddConnectedModel Warning1 pipeLine1:{this.name} pipeLine2:{pipeLine2.name} dot:{dot} angle2:{angle2} dir1:{dir1} dir2:{dir2}");
                base.AddConnectedModel(other);
            }
            else
            {
                Debug.LogWarning($"AddConnectedModel Warning2 pipeLine1:{this.name} pipeLine2:{pipeLine2.name} dot:{dot} angle2:{angle2} dir1:{dir1} dir2:{dir2}");
            }
        }
        else
        {
            base.AddConnectedModel(other);
        }
    }

    public void CreateWeld()
    {
        GameObject go = CreateDebugInfoRoot("WeldPoints");
        //go.transform.SetParent(this.transform);
        //go.transform.position = Vector3.zero;

        //ClearChildren();
        ClearDebugInfoGos();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
            oBBCollider.ShowObbInfo(true);
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
        pipe.weldPipeRadius = generateArg.weldRadius;
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

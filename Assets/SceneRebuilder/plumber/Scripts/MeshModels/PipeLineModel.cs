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


    public PipeLineInfo LineInfo = new PipeLineInfo();

    public override Vector3 GetStartPoint()
    {
        //return StartPoint + this.transform.position;
        //return StartPoint;
        LineInfo.transform = this.transform;
        return LineInfo.GetStartPoint();
    }

    public override Vector3 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        LineInfo.transform = this.transform;
        return LineInfo.GetEndPoint();
    }

    //public void CreateBoxLine()
    //{
    //    TransformHelper.CreateBoxLine(GetStartPoint(), GetEndPoint(), PipeRadius*2, this.name + "_BoxLine", this.transform.parent);
    //}

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

    public OBBCollider ShowObbInfo(Vector3[] vs)
    {
        OBBCollider oBBCollider = this.gameObject.AddMissingComponent<OBBCollider>();
        oBBCollider.lineSize = this.lineSize;
        oBBCollider.ShowObbInfo(vs, true);

        IsObbError = oBBCollider.IsObbError;
        OBB = oBBCollider.OBB;
        return oBBCollider;
    }

    public PipeLineInfo ShowLinePartModelInfo(Vector3[] vs, Transform tParent, bool isShowDebugObj, float planeClosedMinDis = 0.00025f, int planeClosedMaxCount1 = 20, int planeClosedMaxCount2 = 100)
    {
        //1.Obb
        OBBCollider oBBCollider = ShowObbInfo(vs);

        //OrientedBoundingBox obb = OrientedBoundingBox.Create(vs, true, this.name);

        OrientedBoundingBox? obb1 = OrientedBoundingBox.Create(vs, true, this.name);
        if (obb1 == null)
        {
            Debug.LogError($"ShowLinePartModelInfo[{tParent.name}] endPlane1.Plane1Points==null count:{verticesToPlaneInfos.Count}");
            return null;
        }
        OrientedBoundingBox obb = (OrientedBoundingBox)obb1;

        GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");  
        planInfoRoot.AddComponent<DebugInfoRoot>();
        planInfoRoot.transform.SetParent(tParent);
        planInfoRoot.transform.localPosition = Vector3.zero;

        Vector3 ObbExtent = obb.Extent;

        Vector4 startPoint = obb.Up * ObbExtent.y;
        Vector4 endPoint = -obb.Up * ObbExtent.y;

        //2.Planes
        PlaneInfo[] planeInfos = obb.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();

        float percent = 0;
        float j = 5;
        for (; j < 10; j++)
        {
            percent = 0.1f * j;

            verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
            for (int i = 0; i < planeInfos.Length; i++)
            {
                //PipeBendPartInfoJob.logTag += $"_{i}";

                PlaneInfo plane = (PlaneInfo)planeInfos[i];

                if (plane.IsNaN())
                {
                    Debug.LogError($"ShowLinePartModelInfo[{tParent.name}_{i}_{percent}] plane.IsNaN plane:{plane} obb:{obb}");
                    //break;
                } 

                //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
                VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false, planeClosedMinDis, planeClosedMaxCount1, planeClosedMaxCount2);
                v2p.SplitToTwoPlane(percent);
                verticesToPlaneInfos_All.Add(v2p);

                //plane.ShowPlaneInfo(i, planInfoRoot, v2p, lineSize, tParent);

                if (v2p.Plane1Points.Count != vs.Length / 2)
                {
                    continue;
                }

                //if (isShowDebugObj)
                //{
                //    plane.ShowPlaneInfo(i, planInfoRoot, v2p, lineSize, tParent);
                //}

                verticesToPlaneInfos.Add(v2p);
                //var isC = v2p.IsCircle();
            }
            verticesToPlaneInfos.Sort();
            //break;

            if (verticesToPlaneInfos.Count == 2)
            {
                break;
            }
            else if (verticesToPlaneInfos.Count == 1)
            {
                break;
            }
        }

        if (verticesToPlaneInfos.Count == 1)
        {
            VerticesToPlaneInfo startPlane1 = verticesToPlaneInfos[0];
            VerticesToPlaneInfo endPlane1 = VerticesToPlaneInfo.GetEndPlane(startPlane1, verticesToPlaneInfos_All);
            verticesToPlaneInfos.Add(endPlane1);

            if (endPlane1 == null)
            {
                Debug.LogError($"ShowLinePartModelInfo[{tParent.name}] endPlane1 == null count:{verticesToPlaneInfos.Count},all:{verticesToPlaneInfos_All.Count},percent:{percent} startPlane1:{startPlane1}");
                return null;
            }
            if (endPlane1.Plane1Points == null)
            {
                Debug.LogError($"ShowLinePartModelInfo[{tParent.name}] endPlane1.Plane1Points==null count:{verticesToPlaneInfos.Count},all:{verticesToPlaneInfos_All.Count},percent:{percent} startPlane1:{startPlane1}");
                return null;
            }

            for (; j < 10; j++)
            {
                if (endPlane1.Plane1Points.Count != vs.Length / 2)
                {
                    percent = 0.1f * j;
                    endPlane1.SplitToTwoPlane(percent);
                }
                else
                {
                    break;
                }
            }
        }

        for (int i = 0; i < verticesToPlaneInfos.Count; i++)
        {
            VerticesToPlaneInfo v2p = verticesToPlaneInfos[i];
            if (isShowDebugObj)
            {
                PlaneInfo plane = v2p.Plane;
                PlaneHelper.ShowPlaneInfo(plane,i, planInfoRoot, v2p, lineSize, tParent);
            }
        }

        if (verticesToPlaneInfos.Count < 1)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"PipeLine.ShowLinePartModelInfo IsCircle Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name},percent:{percent}");
            return null;
        }

        planInfoRoot.name += $"_{verticesToPlaneInfos.Count}_{percent}" ;

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >= 2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"PipeLine.ShowLinePartModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            endPlane = VerticesToPlaneInfo.GetEndPlane(startPlane, verticesToPlaneInfos_All);
        }


        P1 = obb.Right * ObbExtent.x;
        P2 = -obb.Forward * ObbExtent.z;
        P3 = -obb.Right * ObbExtent.x;
        P4 = obb.Forward * ObbExtent.z;
        P5 = obb.Up * ObbExtent.y;
        P6 = -obb.Up * ObbExtent.y;
        Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        CircleInfo startCircle = startPlane.GetPlane1Circle();
        if (startCircle == null)
        {
            Debug.LogError($"PipeLine.ShowLinePartModelInfo startCircle == null gameObject:{this.gameObject.name} count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            IsGetInfoSuccess = false;

            CreateLocalPoint(startPlane.Plane.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Plane.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return null;
        }
        startPoint = startCircle.GetCenter4();
        CircleInfo endCircle = endPlane.GetPlane1Circle();
        if (endCircle == null)
        {
            Debug.LogError($"PipeLine.ShowLinePartModelInfo endCircle == null gameObject:{this.gameObject.name} count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            IsGetInfoSuccess = false;
            CreateLocalPoint(startPlane.Plane.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Plane.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
            return null;
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
        LineInfo = new PipeLineInfo(startPoint, endPoint, startCircle.GetNormal(), endCircle.GetNormal(), null);

        ModelStartPoint = startPoint;
        ModelStartPoint.w = PipeRadius;
        ModelEndPoint = endPoint;
        ModelEndPoint.w = PipeRadius;

        PipeLength = Vector3.Distance(startPoint, endPoint);
        KeyPointCount = 2;

        return LineInfo;
    }

    public void ShowLineModelInfo(Vector3[] vs, Transform tParent, float planeClosedMinDis = 0.00025f, int planeClosedMaxCount1 = 20, int planeClosedMaxCount2 = 100)
    {
        //OBBCollider oBBCollider = ShowObbInfo(vs); //1.Obb

        //OrientedBoundingBox obb = OrientedBoundingBox.Create(vs, false, this.name);

        OrientedBoundingBox? obb1 = OrientedBoundingBox.Create(vs, false, this.name);
        if (obb1 == null)
        {
            Debug.LogError($"PipeLine.GetModelInfo OrientedBoundingBox==null tParent:{tParent.name} this:{this.name} vs:{vs.Length}");
            return;
        }
        OrientedBoundingBox obb =(OrientedBoundingBox)obb1;

        GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        planInfoRoot.AddComponent<DebugInfoRoot>();
        planInfoRoot.transform.SetParent(tParent);
        planInfoRoot.transform.localPosition = Vector3.zero;

        Vector3 ObbExtent = obb.Extent;

        Vector4 startPoint = obb.Up * ObbExtent.y;
        Vector4 endPoint = -obb.Up * ObbExtent.y;

        //2.Planes
        PlaneInfo[] planeInfos = obb.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false, planeClosedMinDis, planeClosedMaxCount1, planeClosedMaxCount2);
            //v2p.SplitToTwoPlane();
            verticesToPlaneInfos_All.Add(v2p);
            //oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);
            PlaneHelper.ShowPlaneInfo(plane, i, planInfoRoot, v2p, lineSize, tParent);
            //var plane1Circle = v2p.GetPlane1Circle();

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
            Debug.LogError($"PipeLine.GetModelInfo IsCircle Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            return;
        }

        VerticesToPlaneInfo startPlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo endPlane = null;
        if (verticesToPlaneInfos.Count >= 2)
        {
            endPlane = verticesToPlaneInfos[1];
        }
        else
        {
            Debug.LogWarning($"PipeLine.GetModelInfo verticesToPlaneInfos.Count == 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            endPlane = VerticesToPlaneInfo.GetEndPlane(startPlane, verticesToPlaneInfos_All);
        }


        P1 = obb.Right * ObbExtent.x;
        P2 = -obb.Forward * ObbExtent.z;
        P3 = -obb.Right * ObbExtent.x;
        P4 = obb.Forward * ObbExtent.z;
        P5 = obb.Up * ObbExtent.y;
        P6 = -obb.Up * ObbExtent.y;
        Size = new Vector3(ObbExtent.x, ObbExtent.y, ObbExtent.z);

        CircleInfo startCircle = startPlane.GetCircleInfo();
        if (startCircle == null)
        {
            Debug.LogError($"PipeLine.GetModelInfo startCircle == null gameObject:{this.gameObject.name} count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            IsGetInfoSuccess = false;

            CreateLocalPoint(startPlane.Plane.planeCenter, $"Error1_StartPoint1", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Plane.planeCenter, "Error1_EndPoint1", planInfoRoot.transform);
            return;
        }
        startPoint = startCircle.GetCenter4();
        CircleInfo endCircle = endPlane.GetCircleInfo();
        if (endCircle == null)
        {
            Debug.LogError($"PipeLine.GetModelInfo endCircle == null gameObject:{this.gameObject.name} count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            IsGetInfoSuccess = false;
            CreateLocalPoint(startPlane.Plane.planeCenter, "Error3_StartPoint2", planInfoRoot.transform);
            CreateLocalPoint(endPlane.Plane.planeCenter, "Error3_EndPoint2", planInfoRoot.transform);
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
        LineInfo = new PipeLineInfo(startPoint, endPoint, tParent);

        ModelStartPoint = startPoint;
        ModelStartPoint.w = PipeRadius;
        ModelEndPoint = endPoint;
        ModelEndPoint.w = PipeRadius;

        PipeLength = Vector3.Distance(startPoint, endPoint);
        KeyPointCount = 2;
    }

    public override void GetModelInfo()
    {
        //ClearChildren();

        ClearDebugInfoGos();

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        OBBCollider oBBCollider = ShowObbInfo(null);

        ShowLineModelInfo(vs, this.transform);
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
        oBBCollider.ShowObbInfo(null,true);
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

        //????????
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
        this.generateArg = arg;
        Debug.Log($"PipeLineModel.RendererModel 1 name:{this.name} arg:{arg}");

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

        if(PipeFactory.Instance.IsCreatePipeByUnityPrefab)
        {
            var go = CreateModelByPrefabMesh();

            PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, true);
            //PipeMeshGenerator pipe = go.AddMissingComponent<PipeMeshGenerator>();
            SetPipeLineGeneratorArg(pipe, arg, LineInfo.StartPoint, LineInfo.EndPoint);
            pipe.IsOnlyWeld = true;//???????????????
            pipe.RenderPipe();

            PipeMeshGenerator pipe2 = go.AddMissingComponent<PipeMeshGenerator>();
            foreach(GameObject w in pipe.Welds)
            {
                w.transform.SetParent(go.transform);
                pipe2.AddWeld(w);
            }
            GameObject.DestroyImmediate(pipe.gameObject);
            ResultGo = go;

            Debug.Log($"PipeLineModel.RendererModel 2 name:{go.name} arg:{arg}");

            return go.gameObject;
        }
        else
        {
            PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, false);
            if (pipe == null)
            {
                Debug.LogError($"PipeLineModel.RendererModel pipe == null model:{this.name}");
                return null;
            }
            SetPipeLineGeneratorArg(pipe,arg, LineInfo.StartPoint, LineInfo.EndPoint);
            pipe.RenderPipe();
            //var pipe = CreateModelByPrefabMesh();

            Debug.Log($"PipeLineModel.RendererModel 3 name:{pipe.gameObject.name} arg:{arg}");
            return pipe.gameObject;
        }
    }



    

    public Vector3 GetCenter()
    {
        return (GetStartPoint() + GetEndPoint()) / 2;
    }

    public Vector3 GetDirection()
    {
        return GetStartPoint() - GetEndPoint() / 2;
    }

    public override GameObject CreateModelByPrefab()
    {
        if (ResultGo!=null && ResultGo!=this.gameObject)
        {
            GameObject.DestroyImmediate(ResultGo);
        }
        
        GameObject prefab = GameObject.Instantiate(PipeFactory.Instance.GetPipeModelUnitPrefab_Line(this));
        prefab.SetActive(true);
        prefab.name = this.name + "_New2";
        SetPrefabTransfrom(prefab);
        prefab.transform.SetParent(this.transform.parent);
        ResultGo = prefab;
        return prefab;
    }

    public override string GetDictKey()
    {
        return "";
    }

    public override GameObject CreateModelByPrefabMesh()
    {
        if (ResultGo != null && ResultGo != this.gameObject)
        {
            GameObject.DestroyImmediate(ResultGo);
        }
        ClearDebugInfoGos();

        GameObject prefab = this.gameObject;
        prefab.SetActive(true);
        prefab.name = this.name + "_New3";
        SetPrefabTransfrom(prefab);
        MeshHelper.SetNewMesh(prefab, PipeFactory.Instance.GetPipeModelUnitPrefabMesh_Line(this));
        ResultGo = prefab;
        return prefab;
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

        //OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        //if (oBBCollider == null)
        //{
        //    oBBCollider = this.gameObject.AddComponent<OBBCollider>();
        //    oBBCollider.ShowObbInfo(null,true);
        //}
        //OBB = oBBCollider.OBB;

        OBBCollider oBBCollider = ShowObbInfo(null);

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

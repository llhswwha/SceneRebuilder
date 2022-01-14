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

    private PipeElbowKeyPointInfo GetElbow2(SharedMeshTrianglesList list, Mesh mesh)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);

        distanceList = trianglesList.GetPlanePointDistanceList();

        SharedMeshTriangles startPlane = distanceList[0].Plane;
        SharedMeshTriangles endPlane = distanceList[1].Plane;

        var endPoint1 = startPlane.GetCenter();
        var endPoint2 = endPlane.GetCenter();

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

        PipeElbowKeyPointInfo info = new PipeElbowKeyPointInfo();
        //EndPointOut1 = EndPointIn1- normal1 * PipeRadius* PipeLineOffset;
        //EndPointOut2 = EndPointIn2+ normal2 * PipeRadius * PipeLineOffset;
        info.EndPointOut1 = endPoint1;
        info.EndPointOut2 = endPoint2;
        info.EndPointIn1 = endPoint1 + (crossPoint1 - endPoint1).normalized * PipeRadius * PipeLineOffset;
        info.EndPointIn2 = endPoint2 + (crossPoint2 - endPoint2).normalized * PipeRadius * PipeLineOffset;
        info.EndPointIn1.w = PipeRadius;
        info.EndPointIn2.w = PipeRadius;
        info.EndPointOut1.w = PipeRadius;
        info.EndPointOut2.w = PipeRadius;

        info.Line1 = new PipeLineInfo(info.EndPointOut1, info.EndPointIn1, null, normal1);
        info.Line2 = new PipeLineInfo(info.EndPointIn2, info.EndPointOut2, null, normal2);

        TransformHelper.ShowLocalPoint(info.EndPointOut1, PointScale, this.transform, null).name = "Elbow2_OutPoint1";
        TransformHelper.ShowLocalPoint(info.EndPointOut2, PointScale, this.transform, null).name = "Elbow2_OutPoint2";
        TransformHelper.ShowLocalPoint(info.EndPointIn1, PointScale, this.transform, null).name = "Elbow2_InPoint1";
        TransformHelper.ShowLocalPoint(info.EndPointIn2, PointScale, this.transform, null).name = "Elbow2_InPoint2";
        TransformHelper.ShowLocalPoint(crossPoint1, PointScale, this.transform, null).name = "Elbow2_crossPoint1";
        TransformHelper.ShowLocalPoint(crossPoint2, PointScale, this.transform, null).name = "Elbow2_crossPoint2";
        TransformHelper.ShowLocalPoint(crossPoint12, PointScale, this.transform, null).name = "Elbow2_crossPoint12";

        //ModelStartPoint = info.EndPointOut1;
        //ModelEndPoint = info.EndPointOut2;

        //IsGetInfoSuccess = true;

        return info;
    }

    //protected PipeElbowKeyPlaneInfo GetElbow4Planes(SharedMeshTrianglesList list)
    //{
    //    SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);

    //    PipeElbowKeyPlaneInfo info = new PipeElbowKeyPlaneInfo();

    //    var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
    //    distanceList = new List<PlanePointDistance>();
    //    foreach (var p in trianglesList)
    //    {
    //        distanceList.Add(new PlanePointDistance(p, centerOfPoints));
    //    }
    //    distanceList.Sort();
    //    SharedMeshTriangles endPointIn1Plane = distanceList[0].Plane;
    //    SharedMeshTriangles endPointIn2Plane = distanceList[1].Plane;

    //    info.EndPointIn1 = endPointIn1Plane;
    //    info.EndPointIn2 = endPointIn2Plane;
    //    trianglesList.Remove(info.EndPointIn1);
    //    trianglesList.Remove(info.EndPointIn2);

    //    SharedMeshTriangles endPointOut1Plane = MeshHelper.FindClosedPlane(info.EndPointIn1.GetCenter4(), trianglesList);
    //    info.EndPointOut1 = endPointOut1Plane;
    //    trianglesList.Remove(info.EndPointOut1);
    //    SharedMeshTriangles endPointOut2Plane = MeshHelper.FindClosedPlane(info.EndPointIn2.GetCenter4(), trianglesList);
    //    info.EndPointOut2 = endPointOut2Plane;
    //    trianglesList.Remove(info.EndPointOut2);
    //    return info;
    //}

    protected PipeElbowKeyPointInfo GetElbow4(SharedMeshTrianglesList list)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);

        //Debug.Log($"GetElbow4 trianglesList:{trianglesList.Count}");

        //PipeElbowKeyPointInfo info = new PipeElbowKeyPointInfo();

        //var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
        //distanceList = new List<PlanePointDistance>();
        //foreach (var p in trianglesList)
        //{
        //    distanceList.Add(new PlanePointDistance(p, centerOfPoints));
        //}
        //distanceList.Sort();
        //SharedMeshTriangles endPointIn1Plane = distanceList[0].Plane;
        //SharedMeshTriangles endPointIn2Plane = distanceList[1].Plane;

        //info.EndPointIn1 = endPointIn1Plane.GetCenter4();
        //info.EndPointIn2 = endPointIn2Plane.GetCenter4();
        //trianglesList.Remove(info.EndPointIn1);
        //trianglesList.Remove(info.EndPointIn2);

        //SharedMeshTriangles endPointOut1Plane = MeshHelper.FindClosedPlane(info.EndPointIn1, trianglesList);
        //info.EndPointOut1 = endPointOut1Plane.GetCenter4();
        //trianglesList.Remove(info.EndPointOut1);
        //SharedMeshTriangles endPointOut2Plane = MeshHelper.FindClosedPlane(info.EndPointIn2, trianglesList);
        //info.EndPointOut2 = endPointOut2Plane.GetCenter4();
        //trianglesList.Remove(info.EndPointOut2);

        //info.Line1 = new PipeLineInfo(info.EndPointOut1, info.EndPointIn1, null);
        //info.Line2 = new PipeLineInfo(info.EndPointIn2, info.EndPointOut2, null);

        ////TransformHelper.ShowLocalPoint(info.EndPointOut1, PointScale, this.transform, null).name = "Elbow4_OutPoint1";
        ////TransformHelper.ShowLocalPoint(info.EndPointOut2, PointScale, this.transform, null).name = "Elbow4_OutPoint2";
        ////TransformHelper.ShowLocalPoint(info.EndPointIn1, PointScale, this.transform, null).name = "Elbow4_InPoint1";
        ////TransformHelper.ShowLocalPoint(info.EndPointIn2, PointScale, this.transform, null).name = "Elbow4_InPoint2";
        //ShowKeyPoints(info, "Elbow4_");
        //return info;

        PipeElbowKeyPlaneInfo keyPlanes = PipeElbowKeyPlaneInfo.GetElbow4Planes(list);
        PipeElbowKeyPointInfo info2 = keyPlanes.GetKeyPoints();
        ShowKeyPoints(info2, "Elbow4_");
        return info2;
    }

    protected void ShowKeyPoints(PipeElbowKeyPointInfo info,string tag)
    {
        TransformHelper.ShowLocalPoint(info.EndPointOut1, PointScale, this.transform, null).name = tag+"OutPoint1";
        TransformHelper.ShowLocalPoint(info.EndPointOut2, PointScale, this.transform, null).name = tag + "OutPoint2";
        TransformHelper.ShowLocalPoint(info.EndPointIn1, PointScale, this.transform, null).name = tag + "InPoint1";
        TransformHelper.ShowLocalPoint(info.EndPointIn2, PointScale, this.transform, null).name = tag + "InPoint2";
    }

    protected void GetElbow6(SharedMeshTrianglesList list, Mesh mesh)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList(list);
        SharedMeshTriangles plane1 = trianglesList[0];
        SharedMeshTriangles plane2 = trianglesList[1];
        SharedMeshTriangles plane3 = trianglesList[2];
        SharedMeshTriangles plane4 = trianglesList[3];
        SharedMeshTriangles plane5 = trianglesList[4];
        SharedMeshTriangles plane6 = trianglesList[5];
        IsSpecial = true;

        SharedMeshTrianglesList planes4 = new SharedMeshTrianglesList();
        planes4.Add(plane1);
        planes4.Add(plane2);
        planes4.Add(plane3);
        planes4.Add(plane4);
        KeyPointInfo=GetElbow4(planes4);
        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointOut2;

        SharedMeshTrianglesList planes2 = new SharedMeshTrianglesList();
        planes2.Add(plane5);
        planes2.Add(plane6);

        InnerKeyPointInfo = GetElbow2(planes2, mesh);
    }

    public virtual SharedMeshTrianglesList GetSharedMeshTrianglesList(MeshTriangles meshTriangles)
    {
        //SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByPointEx(sharedMinCount, minRepeatPointDistance);
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        return trianglesList;
    }

    // Start is called before the first frame update
    [ContextMenu("GetElbowInfo")]
    public override void GetModelInfo()
    {
        DateTime start = DateTime.Now;
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        //MeshStructure meshS = new MeshStructure(mesh);
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($">>>GetElbowInfo_{this.name} time1:{(DateTime.Now - start).TotalMilliseconds} meshTriangles:{meshTriangles.Count}");

        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = GetSharedMeshTrianglesList(meshTriangles);
        //Debug.Log($">>>GetElbowInfo_{this.name} time2:{(DateTime.Now - start).TotalMilliseconds} trianglesList:{trianglesList.Count}");

        //if (trianglesList.Count > 6)
        //{
        //    trianglesList.RemoveNotCircle();
        //}

        //
        //trianglesList.CombineSameCenter(minRepeatPointDistance);

        if (trianglesList.Count == 3)
        {
            trianglesList.CombineSameCenter(0.002f);
        }
        if (trianglesList.Count == 5)
        {
            trianglesList.CombineSameCenter(minRepeatPointDistance);
        }

        if (trianglesList.Count == 6)
        {
            GetElbow6(trianglesList, mesh);
        }
        else if(trianglesList.Count == 4)
        {
            KeyPointInfo = GetElbow4(trianglesList);
            ModelStartPoint = KeyPointInfo.EndPointOut1;
            ModelEndPoint = KeyPointInfo.EndPointOut2;
            IsGetInfoSuccess = true;
        }
        else if (trianglesList.Count == 2)
        {
            KeyPointInfo = GetElbow2(trianglesList, mesh);
            ModelStartPoint = KeyPointInfo.EndPointOut1;
            ModelEndPoint = KeyPointInfo.EndPointOut2;
            IsGetInfoSuccess = true;
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($">>>GetElbowInfo GetModelInfo points.Count Error count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }

        //meshS.Dispose();
        meshTriangles.Dispose();

        Debug.Log($">>>GetElbowInfo_{this.name} time:{(DateTime.Now - start).TotalMilliseconds} trianglesList:{trianglesList.Count}");
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
        meshTriangles.Dispose();
    }

    public void ShowSharedPoints()
    {
        ClearChildren();

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowSharedPointsById(this.transform, PointScale,10);
        meshTriangles.ShowSharedPointsByIdEx(this.transform, PointScale, 15, minRepeatPointDistance);
        //meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale,10);
        //meshTriangles.ShowSharedPointsByPointEx(this.transform, PointScale, sharedMinCount, minRepeatPointDistance);
        meshTriangles.Dispose();
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
        meshTriangles.Dispose();
    }

    internal void SetModelData(PipeElbowData lineData)
    {
        this.IsSpecial = lineData.IsSpecial;
        this.IsGetInfoSuccess = lineData.IsGetInfoSuccess;
        this.KeyPointInfo = new PipeElbowKeyPointInfo(lineData.KeyPointInfo);
        this.InnerKeyPointInfo = new PipeElbowKeyPointInfo(lineData.InnerKeyPointInfo);
        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointOut2;
    }


    public void RendererModel()
    {
        RendererModel(generateArg,"_New");
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            this.gameObject.SetActive(true);
            return null;
        }

        if (IsSpecial)
        {
            GameObject pipeNew = GetPipeNewGo(arg, afterName);

            GameObject pipe1 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointIn1, KeyPointInfo.EndPointOut1);
            GameObject pipe2 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointIn2, KeyPointInfo.EndPointOut2);
            pipe1.transform.SetParent(pipeNew.transform);
            pipe2.transform.SetParent(pipeNew.transform);

            GameObject pipe3 = RenderElbow(arg, afterName, InnerKeyPointInfo);



            pipe3.transform.SetParent(pipeNew.transform);
            //GameObject target = pipeNew;
            GameObject target = MeshCombineHelper.Combine(pipeNew);
            this.ResultGo = target;

            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
        else
        {
            return RenderElbow(arg, afterName, KeyPointInfo);
        }
    }

    private GameObject RenderElbow(PipeGenerateArg arg, string afterName, PipeElbowKeyPointInfo info)
    {
        if (info == null)
        {
            Debug.LogError($"RendererElbow info==null :{this.name}");
            return null;
        }
        //GameObject pipeNew = new GameObject(this.name + newAfterName);
        //pipeNew.transform.position = this.transform.position + arg.Offset;
        //pipeNew.transform.SetParent(this.transform.parent);

        //PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        //if (pipe == null)
        //{
        //    pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        //}
        //pipe.Target = this.gameObject;

        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName);
        PipeCreateArg pipeArg = new PipeCreateArg(info.Line1, info.Line2);
        var ps = pipeArg.GetGeneratePoints(0, 2, false);

        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        arg.SetArg(pipe);

        //pipe.generateWeld = arg.generateWeld;
        pipe.generateWeld = false;
        //pipe.pipeRadius = PipeRadius;
        pipe.pipeRadius = (info.EndPointOut1.w + info.EndPointOut2.w) / 2;
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();

        return pipe.gameObject;
    }

    //public Vector4 EndPointIn1 = Vector3.zero;
    //public Vector4 EndPointOut1 = Vector3.zero;
    //public Vector4 EndPointIn2 = Vector3.zero;
    //public Vector4 EndPointOut2 = Vector3.zero;
    public PipeElbowKeyPlaneInfo KeyPlaneInfo;

    public PipeElbowKeyPointInfo KeyPointInfo;

    public PipeElbowKeyPointInfo InnerKeyPointInfo;

    public PipeElbowKeyPlaneInfo InnerKeyPlaneInfo;

    public Vector4 GetEndPointIn1()
    {
        if (KeyPointInfo == null)
        {
            Debug.LogError($"GetEndPointIn1 KeyPointInfo == null :{this.name}");
            return Vector4.zero;
        }
        return this.TransformPoint(KeyPointInfo.EndPointIn1);
    }

    public Vector4 GetEndPointIn2()
    {
        if (KeyPointInfo == null)
        {
            Debug.LogError($"GetEndPointIn2 KeyPointInfo == null :{this.name}");
            return Vector4.zero;
        }
        return this.TransformPoint(KeyPointInfo.EndPointIn2);
    }

    //public PipeLineInfo Line1 = new PipeLineInfo();

    //public PipeLineInfo Line2 = new PipeLineInfo();

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

    public float PointScale = 0.001f;

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
            KeyPointInfo.EndPointOut1.w = r;
        }
        if (id == 1)
        {
            KeyPointInfo.EndPointOut2.w = r;
        }
        if (id == 2)
        {
            KeyPointInfo.EndPointIn1.w = r;
            KeyPointInfo.EndPointOut1.w = r;
        }
        if (id == 3)
        {
            KeyPointInfo.EndPointIn2.w = r;
            KeyPointInfo.EndPointOut2.w = r;
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

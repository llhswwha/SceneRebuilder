using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeeModel : PipeElbowModel
{
    public Vector4 LineStartPoint = Vector3.zero;
    public Vector4 LineEndPoint = Vector3.zero;
    public Vector4 TeeStartPoint = Vector3.zero;
    public Vector4 TeeEndPoint = Vector3.zero;

    //public List<PlanePointDistance> distanceListEx;

    [ContextMenu("GetModelInfo")]
    public override void GetModelInfo()
    {
        //base.GetModelInfo();

        DateTime start = DateTime.Now;
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByPointEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        //trianglesList.RemoveNotCircle();
        if (trianglesList.Count == 7)
        {
            GetTeeInfo7(trianglesList);
            IsGetInfoSuccess = true;

        }
        else if (trianglesList.Count == 4)
        {
            GetTeeInfo4(trianglesList);
            IsGetInfoSuccess = true;
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($">>>GetTeeInfo points.Count Error count:{trianglesList.Count} gameObject:{this.gameObject.name} sharedMinCount:{sharedMinCount} minRepeatPointDistance:{minRepeatPointDistance}");
            return;
        }
        Debug.Log($">>>GetTeeInfo time:{DateTime.Now - start}");
    }
    private void GetTeeInfo7(SharedMeshTrianglesList trianglesList)
    {
        SharedMeshTrianglesList list11 = new SharedMeshTrianglesList();
        SharedMeshTrianglesList list12 = new SharedMeshTrianglesList();
        var plane1 = trianglesList[0];
        list11.Add(plane1);
        for (int i = 1; i < trianglesList.Count; i++)
        {
            var plane = trianglesList[i];
            float angle = Vector3.Dot(plane1.Normal, plane.Normal);
            Debug.Log($"Tee GetModelInfo7 angle[{angle}] normal1:{plane1.Normal} normal2:{plane.Normal}");
            if (Mathf.Abs(angle) < 0.001f)
            {
                list12.Add(plane);
            }
            else
            {
                list11.Add(plane);
            }
        }
        Debug.Log($"Tee GetModelInfo7 list1:{list11.Count} list2:{list12.Count}");

        SharedMeshTrianglesList list4;
        SharedMeshTrianglesList list3;
        if (list11.Count == 4)
        {
            list4 = list11;
            list3 = list12;
        }
        else
        {
            list4 = list12;
            list3 = list11;
        }

        SharedMeshTrianglesList list;

        KeyPlaneInfo = GetElbow4Planes(list4);
        KeyPointInfo = KeyPlaneInfo.GetKeyPoints();

        //KeyPointInfo = GetElbow4(list4);

        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointOut2;

        list3.Sort((a, b) => { return a.Radius.CompareTo(b.Radius); });
        InnerKeyPlaneInfo = new PipeElbowKeyPlaneInfo(list3[1], list3[2], list3[0], list3[0]);
        InnerKeyPointInfo = InnerKeyPlaneInfo.GetKeyPoints();


        IsSpecial = true;
    }

    private void GetTeeInfo4(SharedMeshTrianglesList trianglesList)
    {
        var centerOfPoints = MeshHelper.GetCenterOfList(trianglesList);
        distanceList = new List<PlanePointDistance>();
        foreach (var p in trianglesList)
        {
            distanceList.Add(new PlanePointDistance(p, centerOfPoints));
        }
        distanceList.Sort();

        SharedMeshTriangles teePlane1 = distanceList[0].Plane;

        TeeStartPoint = teePlane1.GetCenter4();
        trianglesList.Remove(teePlane1);

        SharedMeshTriangles teePlane2 = null;

        //float minNormalAngle = 0;
        for (int i = 0; i < trianglesList.Count; i++)
        {
            SharedMeshTriangles plane = trianglesList[i];
            var normalAngle = Vector3.Dot(teePlane1.Normal, plane.Normal);
            Debug.Log($"go:{this.name} angle[{i}] normal1:{teePlane1.Normal} normal2:{plane.Normal} angle:{normalAngle}");
            if (normalAngle + 1 <= 0.00001)//相反或者平行
            {
                teePlane2 = plane;
                //break;
            }
            if (normalAngle - 1 <= 0.00001)
            {
                teePlane2 = plane;
                //break;
            }
        }

        if (teePlane2 == null)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetTeeModelInfo go:{this.name} teePlane2 == null");
            return;
        }

        TeeEndPoint = teePlane2.GetCenter4();
        trianglesList.Remove(teePlane2);

        LineStartPoint = trianglesList[0].GetCenter4();
        LineEndPoint = trianglesList[1].GetCenter4();

        //EndPointIn2 = distanceList[1].Plane.GetCenter();
        //trianglesList.Remove(EndPointIn1);
        //trianglesList.Remove(EndPointIn2);

        //EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, trianglesList);
        //trianglesList.Remove(EndPointOut1);
        //EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, trianglesList);
        //trianglesList.Remove(EndPointOut2);

        KeyPointInfo.Line1 = new PipeLineInfo(TeeStartPoint, TeeEndPoint, null);
        KeyPointInfo.Line2 = new PipeLineInfo(LineStartPoint, LineEndPoint, null);

        TransformHelper.ShowLocalPoint(TeeStartPoint, PointScale, this.transform, null).name = $"TeeStartPoint_{TeeStartPoint.w}";
        TransformHelper.ShowLocalPoint(TeeEndPoint, PointScale, this.transform, null).name = $"TeeEndPoint_{TeeEndPoint.w}";
        TransformHelper.ShowLocalPoint(LineStartPoint, PointScale, this.transform, null).name = $"LineStartPoint_{LineStartPoint.w}";
        TransformHelper.ShowLocalPoint(LineEndPoint, PointScale, this.transform, null).name = $"LineEndPoint_{LineEndPoint.w}";

        //GetPipeRadius();

        ModelStartPoint = LineStartPoint;
        ModelEndPoint = LineEndPoint;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (IsSpecial) 
        {
            GameObject pipeNew = GetPipeNewGo(arg, afterName);

            GameObject pipe11 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointIn1, KeyPointInfo.EndPointOut1);
            GameObject pipe12 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointIn2, KeyPointInfo.EndPointOut2);
            pipe11.transform.SetParent(pipeNew.transform);
            pipe12.transform.SetParent(pipeNew.transform);
            GameObject pipe13 = RenderPipeLine(arg, afterName, KeyPlaneInfo.EndPointIn1.GetMinCenter4(), KeyPlaneInfo.EndPointIn2.GetMinCenter4());
            pipe13.transform.SetParent(pipeNew.transform);

            GameObject pipe21 = RenderPipeLine(arg, afterName, InnerKeyPointInfo.EndPointIn1, InnerKeyPointInfo.EndPointOut1);
            pipe21.transform.SetParent(pipeNew.transform);
            GameObject pipe22 = RenderPipeLine(arg, afterName, InnerKeyPlaneInfo.EndPointIn1.GetMinCenter4(), InnerKeyPlaneInfo.EndPointIn2.GetMinCenter4());
            pipe22.transform.SetParent(pipeNew.transform);

            //GameObject pipe3 = RenderElbow(arg, afterName, InnerElbowInfo);
            //pipe3.transform.SetParent(pipeNew.transform);

            //GameObject target = pipeNew;
            GameObject target = MeshCombineHelper.Combine(pipeNew);
            this.ResultGo = target;

            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
        else
        {
            GameObject pipe1 = RenderPipeLine(arg, afterName, LineStartPoint, LineEndPoint);
            GameObject pipe2 = RenderPipeLine(arg, afterName, TeeStartPoint, TeeEndPoint);
            GameObject pipeNew = GetPipeNewGo(arg, afterName);

            pipe1.transform.SetParent(pipeNew.transform);
            pipe2.transform.SetParent(pipeNew.transform);
            GameObject target = MeshCombineHelper.Combine(pipeNew);
            this.ResultGo = target;

            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
    }

    public override List<Vector4> GetModelKeyPoints()
    {
        var list = base.GetModelKeyPoints();
        list.Add(this.TransformPoint(TeeEndPoint));
        return list;
    }

    //public override int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    //{
    //    int cCount = base.ConnectedModel(model2, minPointDis, isShowLog, isUniformRaidus, minRadiusDis);
    //    return cCount;
    //}
}

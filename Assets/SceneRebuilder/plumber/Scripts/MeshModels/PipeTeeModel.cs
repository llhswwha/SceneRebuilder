using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTeeModel : PipeElbowModel
{
    //public Vector4 LineStartPoint = Vector3.zero;
    //public Vector4 LineEndPoint = Vector3.zero;
    //public Vector4 TeeStartPoint = Vector3.zero;
    //public Vector4 TeeEndPoint = Vector3.zero;

    //public List<PlanePointDistance> distanceListEx;

    [ContextMenu("GetModelInfo")]
    public override void GetModelInfo()
    {
        //base.GetModelInfo();

        DateTime start = DateTime.Now;
        //ClearChildren();
        ClearDebugInfoGos();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        this.VertexCount = mesh.vertexCount;
        meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByPointEx(sharedMinCount, minRepeatPointDistance, $"PipeTeeModel_{this.name}");
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }

        KeyPointCount = trianglesList.Count;
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
        meshTriangles.Dispose();
        //Debug.Log($">>>GetTeeInfo  time:{DateTime.Now - start} IsGetInfoSuccess:{IsGetInfoSuccess} KeyPointCount:{KeyPointCount}");
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
            //Debug.Log($"Tee GetModelInfo7 angle[{angle}] normal1:{plane1.Normal} normal2:{plane.Normal}");
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
        KeyPlaneInfo = PipeModelKeyPlaneInfo4.GetElbow4Planes(list4);
        KeyPointInfo = KeyPlaneInfo.GetKeyPoints();

        //KeyPointInfo = GetElbow4(list4);

        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointOut2;

        list3.Sort((a, b) => { return a.Radius.CompareTo(b.Radius); });
        InnerKeyPlaneInfo = new PipeModelKeyPlaneInfo4(list3[1], list3[2], list3[0], list3[0]);
        InnerKeyPointInfo = InnerKeyPlaneInfo.GetKeyPoints();


        IsSpecial = true;
    }

    private void GetTeeInfo4(SharedMeshTrianglesList trianglesList)
    {

        var distanceList = trianglesList.GetPlanePointDistanceList();

        SharedMeshTriangles teePlane1 = distanceList[0].Plane;

        var TeeStartPoint = teePlane1.GetCenter4();
        trianglesList.Remove(teePlane1);

        //SharedMeshTriangles? teePlane2 = null;

        ////float minNormalAngle = 0;
        //for (int i = 0; i < trianglesList.Count; i++)
        //{
        //    SharedMeshTriangles plane = trianglesList[i];
        //    var normalAngle = Vector3.Dot(teePlane1.Normal, plane.Normal);
        //    Debug.Log($"GetTeeInfo4 go:{this.name} angle[{i}] normal1:{teePlane1.Normal} normal2:{plane.Normal} angle:{normalAngle} ({normalAngle + 1},{normalAngle - 1})");
        //    if (Mathf.Abs(normalAngle + 1) <= 0.00001)//????????????
        //    {
        //        teePlane2 = plane;
        //        //break;
        //    }
        //    if (Mathf.Abs(normalAngle - 1) <= 0.00001)
        //    {
        //        teePlane2 = plane;
        //        //break;
        //    }
        //}

        SharedMeshTriangles? teePlane2 = trianglesList.FindSameDirectionPlane(teePlane1, this.name);

        if (teePlane2 == null)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetTeeModelInfo go:{this.name} teePlane2 == null");
            return;
        }

        var TeeEndPoint = ((SharedMeshTriangles)teePlane2).GetCenter4();
        trianglesList.Remove(((SharedMeshTriangles)teePlane2));

        var LineStartPoint = trianglesList[0].GetCenter4();
        var LineEndPoint = trianglesList[1].GetCenter4();

        //EndPointIn2 = distanceList[1].Plane.GetCenter();
        //trianglesList.Remove(EndPointIn1);
        //trianglesList.Remove(EndPointIn2);

        //EndPointOut1 = MeshHelper.FindClosedPoint(EndPointIn1, trianglesList);
        //trianglesList.Remove(EndPointOut1);
        //EndPointOut2 = MeshHelper.FindClosedPoint(EndPointIn2, trianglesList);
        //trianglesList.Remove(EndPointOut2);
        
        //KeyPointInfo.Line1 = new PipeLineInfo(TeeStartPoint, TeeEndPoint, null);
        //KeyPointInfo.Line2 = new PipeLineInfo(LineStartPoint, LineEndPoint, null);

        KeyPointInfo = new PipeModelKeyPointInfo4(TeeStartPoint, TeeEndPoint, LineStartPoint, LineEndPoint);

        var keyPoints = CreateKeyPointsGo();

        TransformHelper.ShowLocalPoint(TeeStartPoint, PointScale, keyPoints.transform, null).name = $"TeeStartPoint_{TeeStartPoint.w}_{teePlane1.Normal}";
        TransformHelper.ShowLocalPoint(TeeEndPoint, PointScale, keyPoints.transform, null).name = $"TeeEndPoint_{TeeEndPoint.w}_{((SharedMeshTriangles)teePlane2).Normal}";
        TransformHelper.ShowLocalPoint(LineStartPoint, PointScale, keyPoints.transform, null).name = $"LineStartPoint_{LineStartPoint.w}";
        TransformHelper.ShowLocalPoint(LineEndPoint, PointScale, keyPoints.transform, null).name = $"LineEndPoint_{LineEndPoint.w}";

        //GetPipeRadius();

        ModelStartPoint = LineStartPoint;
        ModelEndPoint = LineEndPoint;
    }

    public new PipeTeeData ModelData;

    internal void SetModelData(PipeTeeData lineData)
    {
        this.IsSpecial = lineData.IsSpecial;
        this.IsGetInfoSuccess = lineData.IsGetInfoSuccess;
        this.KeyPointCount = lineData.KeyPointCount;
        this.KeyPointInfo = new PipeModelKeyPointInfo4(lineData.KeyPointInfo);
        this.InnerKeyPointInfo = new PipeModelKeyPointInfo4(lineData.InnerKeyPointInfo);

        this.KeyPlaneInfo = new PipeModelKeyPlaneInfo4(lineData.KeyPlaneInfo);
        this.InnerKeyPlaneInfo = new PipeModelKeyPlaneInfo4(lineData.InnerKeyPlaneInfo);

        var p11 = KeyPlaneInfo.EndPointIn1.GetMinCenter4();
        var p12 = KeyPlaneInfo.EndPointIn2.GetMinCenter4();

        var p21 = lineData.KeyPlaneInfo.EndPointIn1.GetCenter4();
        var p22 = lineData.KeyPlaneInfo.EndPointIn2.GetCenter4();

        ModelStartPoint = KeyPointInfo.EndPointOut1;
        ModelEndPoint = KeyPointInfo.EndPointOut2;

        //Debug.Log($"Tee.SetModelData p11:{Vector4String(p11)} p12:{Vector4String(p12)} p21:{Vector4String(p21)} p22:{Vector4String(p22)}");
    }

    public new PipeTeeData GetModelData()
    {
        ModelData.KeyPointInfo = new PipeModelKeyPointData4(KeyPointInfo);
        ModelData.InnerKeyPointInfo = new PipeModelKeyPointData4(InnerKeyPointInfo);
        ModelData.KeyPlaneInfo = new PipeModelKeyPlaneData4(KeyPlaneInfo);
        ModelData.InnerKeyPlaneInfo = new PipeModelKeyPlaneData4(InnerKeyPlaneInfo);
        ModelData.IsGetInfoSuccess = IsGetInfoSuccess;
        ModelData.IsSpecial = IsSpecial;
        ModelData.KeyPointCount = KeyPointCount;
        return ModelData;
    }

    public new PipeTeeSaveData GetSaveData()
    {
        PipeTeeSaveData data = new PipeTeeSaveData();
        InitSaveData(data);
        data.Data = GetModelData();
        //KeyPointInfo = null;
        //InnerKeyPointInfo = null;
        ////KeyPlaneInfo = null;
        return data;
    }

    //public void SetSaveData(PipeTeeSaveData data)
    //{
    //    //this.LineInfo = data.Info;
    //    SetModelData(data.Data);
    //}

    public override void SetSaveData(MeshModelSaveData data)
    {
        //this.LineInfo = data.Info;
        SetModelData((data as PipeTeeSaveData).Data);
        //PipeFactory.Instance.RendererModelFromXml(this, data);
    }





    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        if (RendererErrorModel())
        {
            return null;
        }

        arg = arg.Clone();

        if (IsSpecial) 
        {
            arg.generateWeld = false;

            GameObject pipeNew = GetPipeNewGo(arg, afterName);
            pipeNew.transform.up = InnerKeyPlaneInfo.EndPointIn1.GetMinCenter4()-InnerKeyPlaneInfo.EndPointIn2.GetMinCenter4();
            //pipeNew.transform.right = KeyPlaneInfo.EndPointIn1.GetMinCenter4() - KeyPlaneInfo.EndPointIn2.GetMinCenter4();
            pipeNew.transform.Rotate(pipeNew.transform.right, 90);

            if (KeyPointInfo == null)
            {
                Debug.LogError($"Tee.RendererModel KeyPointInfo == null gameObject:{this.name}");
                return pipeNew;
            }
            if (KeyPlaneInfo.EndPointIn1 == null)
            {
                Debug.LogError($"Tee.RendererModel KeyPlaneInfo.EndPointIn1 == null gameObject:{this.name}");
                return pipeNew;
            }
            if (KeyPlaneInfo.EndPointIn2 == null)
            {
                Debug.LogError($"Tee.RendererModel KeyPlaneInfo.EndPointIn2 == null gameObject:{this.name}");
                return pipeNew;
            }
            var lineArg = arg.Clone();
            if (lineArg.pipeSegments < 32)
                lineArg.pipeSegments = 32;
            if (arg.pipeSegments < 24)
                arg.pipeSegments = 24;
            lineArg.generateEndCaps = true;
            GameObject pipe11 = RenderPipeLine(lineArg, afterName+"_1", KeyPointInfo.EndPointIn1, KeyPointInfo.EndPointOut1);
            GameObject pipe12 = RenderPipeLine(lineArg, afterName + "_2", KeyPointInfo.EndPointIn2, KeyPointInfo.EndPointOut2);
            pipe11.transform.SetParent(pipeNew.transform);
            pipe12.transform.SetParent(pipeNew.transform);
            GameObject pipe13 = RenderPipeLine(arg, afterName + "_3", KeyPlaneInfo.EndPointIn1.GetMinCenter4(), KeyPlaneInfo.EndPointIn2.GetMinCenter4());
            pipe13.transform.SetParent(pipeNew.transform);

            GameObject pipe21 = RenderPipeLine(lineArg, afterName + "_4", InnerKeyPointInfo.EndPointIn1, InnerKeyPointInfo.EndPointOut1);
            pipe21.transform.SetParent(pipeNew.transform);
            GameObject pipe22 = RenderPipeLine(arg, afterName + "_5", InnerKeyPlaneInfo.EndPointIn1.GetMinCenter4(), InnerKeyPlaneInfo.EndPointIn2.GetMinCenter4());
            pipe22.transform.SetParent(pipeNew.transform);
            GameObject target = pipeNew;
            target=CombineTarget(arg, pipeNew);
            target = CopyMeshComponentsEx(target);
            this.ResultGo = target;
            PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
            pipeG.Target = this.gameObject;
            return target;
        }
        else
        {
            if (KeyPointInfo.EndPointOut2.w > PipeGenerateArg.MinPipeRadius)
            {
                if (arg.pipeSegments < 24)
                {
                    arg.pipeSegments = 24;
                }
            }

            arg.IsGenerateEndWeld = false;
            arg.generateEndCaps = true;
            GameObject pipe1 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointOut1, KeyPointInfo.EndPointIn1);
            arg.IsGenerateEndWeld = true;
            GameObject pipe2 = RenderPipeLine(arg, afterName, KeyPointInfo.EndPointIn2, KeyPointInfo.EndPointOut2);
            GameObject pipeNew = GetPipeNewGo(arg, afterName);
            pipeNew.transform.up =  KeyPointInfo.EndPointIn1- KeyPointInfo.EndPointOut1;
            //pipeNew.transform.right = KeyPointInfo.EndPointIn2 - KeyPointInfo.EndPointOut2;
            Vector3 dir = KeyPointInfo.EndPointIn2 - KeyPointInfo.EndPointOut2;
            float angle = Vector3.Angle(dir, Vector3.right);
            //pipeNew.transform.Rotate(pipeNew.transform.up, angle, Space.World);
            //Debug.Log($"Tee angle:{angle}");

            pipeNew.transform.Rotate(Vector3.up, angle, Space.Self);

            pipe1.transform.SetParent(pipeNew.transform);
            pipe2.transform.SetParent(pipeNew.transform);
            GameObject target = pipeNew;

            target = CombineTarget(arg, pipeNew);

            target = CopyMeshComponentsEx(target);

            this.ResultGo = target;
            //else
            {
                PipeMeshGenerator pipeG = target.AddComponent<PipeMeshGenerator>();
                if (this == null)
                {
                    Debug.LogError("PipeTeeModel this == null");
                }
                if (this.gameObject == null)
                {
                    Debug.LogError("PipeTeeModel gameObject == null");
                }
                pipeG.Target = this.gameObject;
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    pipeG.Childrens.Add(target.transform.GetChild(i));
                }
            }
            
            return target;
        }
    }

    public override string GetDictKey()
    {
        if (IsSpecial)
        {
            return $"Tee_{IsSpecial},{KeyPlaneInfo.GetMinRadiusIn():F3},{InnerKeyPlaneInfo.GetMinRadiusIn():F3}";
        }
        else
        {
            if (KeyPointInfo == null)
            {
                Debug.LogError($"GetDictKey3 KeyPointInfo == null gameObject:{this.name}");
                return this.VertexCount + "";
            }
            return $"Tee_{IsSpecial},{KeyPointInfo.GetRadiusIn2Out2():F3},{KeyPointInfo.GetRadiusIn1Out1():F3},{KeyPointInfo.GetLength2():F3},{KeyPointInfo.GetLength1():F3}";
        }
    }

    public override string GetSortKey()
    {
        return $"[{GetDictKey()}]";
    }


    public override List<Vector4> GetModelKeyPoints()
    {
        var list = base.GetModelKeyPoints();
        if (IsSpecial)
        {
            //list.Add(this.TransformPoint(KeyPointInfo.EndPointIn1));
            //list.Add(this.TransformPoint(KeyPointInfo.EndPointOut1));
            //list.Add(this.TransformPoint(KeyPointInfo.EndPointIn2));
            //list.Add(this.TransformPoint(KeyPointInfo.EndPointOut2));
            list.Add(this.TransformPoint(InnerKeyPointInfo.EndPointIn1));
            list.Add(this.TransformPoint(InnerKeyPointInfo.EndPointOut1));
        }
        else
        {
            list.Add(this.TransformPoint(KeyPointInfo.EndPointIn1));
            list.Add(this.TransformPoint(KeyPointInfo.EndPointOut1));
        }       
        return list;
    }

    //public override int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    //{
    //    int cCount = base.ConnectedModel(model2, minPointDis, isShowLog, isUniformRaidus, minRadiusDis);
    //    return cCount;
    //}

    //public new void OnDestroy()
    //{
    //    //Debug.LogError("PipeTeeModel.OnDestroy:"+this);
    //}
}

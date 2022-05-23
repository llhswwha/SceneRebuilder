using CommonUtils;
using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelStructureBaseModel : BaseMeshModel
{
    public float minSamePlaneNormalDis = 0.0001f;

    protected GameObject planInfoRoot;

    protected List<VerticesToPlaneInfo> verticesToPlaneInfos;

    protected OBBCollider oBBCollider;

     public virtual void ClearData()
    {
        
    }

    [ContextMenu("GetHModelInfo")]
    public void GetHModelInfo()
    {
         ClearDebugInfoGos();

        oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
        }
        oBBCollider.ShowObbInfo(null,true);

        var IsObbError = oBBCollider.IsObbError;
        var OBB = oBBCollider.OBB;

        //1.Obb

        Vector3 ObbExtent = OBB.Extent;

        Vector4 startPoint = OBB.Up * ObbExtent.y;
        Vector4 endPoint = -OBB.Up * ObbExtent.y;

        //GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        //planInfoRoot.AddComponent<DebugInfoRoot>();
        //planInfoRoot.transform.SetParent(this.transform);
        //planInfoRoot.transform.localPosition = Vector3.zero;

        planInfoRoot = DebugInfoRoot.NewGo("PipeModel_PlaneInfo", this.transform);

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
            //if (v2p.IsCircle() == false)
            //{
            //    continue;
            //}
            verticesToPlaneInfos.Add(v2p);
            var isC = v2p.IsCircle();
        }
        verticesToPlaneInfos.Sort();

        Debug.Log($"verticesToPlaneInfos:{verticesToPlaneInfos.Count}");

        if (verticesToPlaneInfos.Count < 1)
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"PipeLine.GetModelInfo verticesToPlaneInfos.Count < 1 count:{verticesToPlaneInfos.Count},gameObject:{this.name}");
            return;
        }

        GenerateHModel(verticesToPlaneInfos, oBBCollider, planInfoRoot);
    }

    public void GenerateHModel(List<VerticesToPlaneInfo> verticesToPlaneInfos, OBBCollider oBBCollider, GameObject planInfoRoot)
    {
        GameObject goNew = new GameObject(gameObject.name);
        goNew.transform.position = this.transform.position;

        HMeshGenerator hMesh = goNew.AddComponent<HMeshGenerator>();
        VerticesToPlaneInfo beforePlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo leftPlane = verticesToPlaneInfos[4];
        VerticesToPlaneInfo rightPlane = verticesToPlaneInfos[5];
        VerticesToPlaneInfo topPlane = verticesToPlaneInfos[2];

        hMesh.forward = verticesToPlaneInfos[0].DirectionToPlane(verticesToPlaneInfos[1], transform, "Length");
        hMesh.length = verticesToPlaneInfos[0].DistanceToPlane(verticesToPlaneInfos[1], transform, "Length");// Vector3.Distance(verticesToPlaneInfos[0].Point.planeCenter, verticesToPlaneInfos[1].Point.planeCenter);
        hMesh.height = topPlane.DistanceToPlane(verticesToPlaneInfos[3], transform, "Height");//Vector3.Distance(verticesToPlaneInfos[2].Point.planeCenter, verticesToPlaneInfos[3].Point.planeCenter);
        hMesh.width = leftPlane.DistanceToPlane(rightPlane, transform, "Width");//Vector3.Distance(verticesToPlaneInfos[4].Point.planeCenter, verticesToPlaneInfos[5].Point.planeCenter);

        VerticesToPlaneInfo left2Top = new VerticesToPlaneInfo(leftPlane.Plane1Points.ToArray(), topPlane.Plane, true);

        //VerticesToPlaneInfo top2Left = new VerticesToPlaneInfo(topPlane.Plane1Points.ToArray(), leftPlane.Point, false);

        //oBBCollider.ShowPlaneInfo(leftPlane.Point, 0, planInfoRoot, leftPlane);

        oBBCollider.ShowPlaneInfo(topPlane.Plane, 7, planInfoRoot, left2Top);

        //oBBCollider.ShowPlaneInfo(leftPlane.Point, 1, planInfoRoot, top2Left);

        //oBBCollider.ShowPlaneInfo(topPlane.Point, 0, planInfoRoot, left2Top);


        PlaneInfo middlePlaneOfleftRight = PlaneInfo.GetMiddlePlane(leftPlane.Plane, rightPlane.Plane);
        VerticesToPlaneInfo middlePlaneVP = new VerticesToPlaneInfo(beforePlane.Plane1Points.ToArray(), middlePlaneOfleftRight, true);
        GameObject middlePlaneOfleftRightGo = DebugInfoRoot.NewGo("middlePlaneOfleftRight", this.transform);
        PlaneHelper.ShowPlaneInfo(middlePlaneOfleftRight,0, middlePlaneOfleftRightGo, middlePlaneVP, PointScale, this.transform, true, true);

        hMesh.sizeX = hMesh.width * 0.07f;
        hMesh.sizeY = hMesh.height * 0.03f;

        //hMesh.sizeX = middlePlaneVP.GetPlaneHeight(transform, "sizeX");
        //hMesh.sizeY = left2Top.DebugDistanceOfPlane12(transform, "sizeY");

        hMesh.sizeDetail = hMesh.sizeX * 1.8f;

        //VerticesToPlaneInfo middleOfleftRight=VerticesToPlaneInfo.GetMiddlePlane(leftPlane, rightPlane);

        hMesh.CreateLine();

        if (ResultGo != null)
        {
            GameObject.DestroyImmediate(ResultGo);
        }
        ResultGo = goNew;
    }

    [ContextMenu("GetCModelInfo")]
    public void GetCModelInfo()
    {
        GetHModelInfo();
    }

    protected List<VerticesToPlaneInfo> GetVerticesToPlaneInfos(float planeClosedMinDis = 0.00025f, int planeClosedMaxCount1 = 20, int planeClosedMaxCount2 = 100)
    {
        oBBCollider = this.gameObject.AddMissingComponent<OBBCollider>();
        oBBCollider.lineSize = this.PointScale;

        if (isShowDebug)
        {
            oBBCollider.ShowObbInfo(null, true);
        }
        else
        {
            oBBCollider.GetObb(null, true); 
        }

        var IsObbError = oBBCollider.IsObbError;
        var OBB = oBBCollider.OBB;

        //1.Obb

        Vector3 ObbExtent = OBB.Extent;

        Vector4 startPoint = OBB.Up * ObbExtent.y;
        Vector4 endPoint = -OBB.Up * ObbExtent.y;

        //GameObject planInfoRoot = new GameObject("PipeModel_PlaneInfo");
        //planInfoRoot.AddComponent<DebugInfoRoot>();
        //planInfoRoot.transform.SetParent(this.transform);
        //planInfoRoot.transform.localPosition = Vector3.zero;

        if (isShowDebug)
        {
            planInfoRoot = DebugInfoRoot.NewGo("PipeModel_PlaneInfo", this.transform);
        }

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        //2.Planes
        PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        var verToPlaneInfos = new List<VerticesToPlaneInfo>();
        for (int i = 0; i < planeInfos.Length; i++)
        {
            PlaneInfo plane = (PlaneInfo)planeInfos[i];
            //VerticesToPlaneInfo v2p =GetVerticesToPlaneInfo(vs, plane, false);
            VerticesToPlaneInfo v2p = new VerticesToPlaneInfo(vs, plane, false, planeClosedMinDis, planeClosedMaxCount1, planeClosedMaxCount2);
            verticesToPlaneInfos_All.Add(v2p);
            //v2p.SplitToTwoPlane();
            if (isShowDebug)
            {
                oBBCollider.ShowPlaneInfo(plane, i, planInfoRoot, v2p);
            }
            verToPlaneInfos.Add(v2p);
            //var isC = v2p.IsCircle();
        }

        verToPlaneInfos = VerticesToPlaneInfo.SortList(verToPlaneInfos, verticesToPlaneInfos_All);
        if (isShowDebug)
        {
            planInfoRootNew = DebugInfoRoot.NewGo("PipeModel_PlaneInfo_New", this.transform);
            for (int i = 0; i < verToPlaneInfos.Count; i++)
            {
                VerticesToPlaneInfo v2p = verToPlaneInfos[i];
                PlaneInfo plane = v2p.Plane;
                oBBCollider.ShowPlaneInfo(plane, i, planInfoRootNew, v2p);
            }
        }
        return verToPlaneInfos;
    }

    GameObject planInfoRootNew;


    public float meshDis;

    public class SteelModelGenerateArg:IComparable<SteelModelGenerateArg>
    {
        public GameObject go;

        public float meshDis;

        public float angle;

        public SteelModelGenerateArg()
        {

        }

        public SteelModelGenerateArg(GameObject go,float dis,float angle)
        {
            this.go = go;
            this.meshDis = dis;
            this.angle = angle;
        }

        public int CompareTo(SteelModelGenerateArg other)
        {
            return this.meshDis.CompareTo(other.meshDis);
        }

        public override string ToString()
        {
            return $"[{angle},{meshDis}]";
        }
    }

    public SteelModelLData GetModelData_LMesh(List<VerticesToPlaneInfo> verticesToPlaneInfos)
    {
        SteelModelLData lData = new SteelModelLData();
        VerticesToPlaneInfo beforePlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo afterPlane = verticesToPlaneInfos[1];

        VerticesToPlaneInfo topPlane = verticesToPlaneInfos[2];
        VerticesToPlaneInfo bottomPlane = verticesToPlaneInfos[3];
        if(topPlane.Plane1PointCount > bottomPlane.Plane1PointCount)
        {
            VerticesToPlaneInfo tmp = topPlane;
            topPlane = bottomPlane;
            bottomPlane = tmp;
        }

        VerticesToPlaneInfo leftPlane = verticesToPlaneInfos[4];
        VerticesToPlaneInfo rightPlane = verticesToPlaneInfos[5];
        if (leftPlane.Plane1PointCount > rightPlane.Plane1PointCount)
        {
            VerticesToPlaneInfo tmp = leftPlane;
            leftPlane = rightPlane;
            rightPlane = tmp;
        }

        //OrientedBoundingBox OBB = oBBCollider.OBB;
        var size = beforePlane.GetSize();
        lData.forward = beforePlane.DirectionToPlane(afterPlane, transform, "Length");
        lData.length = beforePlane.DistanceToPlaneEx(afterPlane, transform, "Length", isShowDebug);
        float height = topPlane.DistanceToPlaneEx(bottomPlane, transform, "Height", isShowDebug);
        lData.up= topPlane.DirectionToPlane(bottomPlane, transform, "Height");
        float width = leftPlane.DistanceToPlaneEx(rightPlane, transform, "Width", isShowDebug); 
        lData.SetSize(height, width);
        //Debug.Log($"GetLModelData sizeX:{size.x} sizeY:{size.y} height:{height} width:{width} planeSizeX:{beforePlane.Plane.SizeX} planeSizeY:{beforePlane.Plane.SizeY}");
        lData.sizePlane = lData.height / 10f;
        lData.sizeDetail = lData.sizePlane;
        return lData;
    }

    public CornerBoxMeshData GetModelData_CornerBox(List<VerticesToPlaneInfo> verticesToPlaneInfos)
    {
        VerticesToPlaneInfo topPlane0 = verticesToPlaneInfos.Find(i => i.IsCount(4, 4) || i.IsCount(2, 2));
        VerticesToPlaneInfo leftPlane0 = verticesToPlaneInfos.Find(i => i.IsCount(4, 2));
        if (topPlane0 == null || leftPlane0 == null)
        {
            List<VerticesToPlaneInfo> planes = verticesToPlaneInfos.FindAll(i => i.IsCount(4, 0) || i.IsCount(4, 1) || i.IsCount(4, 2));
            for (int i = 0; i < planes.Count; i++)
            {
                VerticesToPlaneInfo plane = planes[i];
                string r1 = plane.ResultInfo;
                plane.AddPlane2PointsEx();
                if (isShowDebug)
                {
                    Debug.LogError($"GetModelData_CornerBox1({this.name})[{i}] plane:{plane.ResultInfo} \nr1:{r1}");
                }
            }

            VerticesToPlaneInfo topPlane1 = verticesToPlaneInfos.Find(i => i.IsCount(4, 4) || i.IsCount(2, 2));
            VerticesToPlaneInfo leftPlane1 = verticesToPlaneInfos.Find(i => i.IsCount(4, 2));
            if (topPlane1 == null || leftPlane1 == null)
            {
                List<VerticesToPlaneInfo> planes2 = verticesToPlaneInfos.FindAll(i => i.IsCount(4, 0) || i.IsCount(4, 1) || i.IsCount(4, 2));
                for (int i = 0; i < planes2.Count; i++)
                {
                    VerticesToPlaneInfo plane = planes2[i];
                    string r1 = plane.ResultInfo;
                    plane.AddPlane2PointsEx(0.1f,0.7f); 
                    if (isShowDebug)
                    {
                        Debug.LogError($"GetModelData_CornerBox2({this.name})[{i}] plane:{plane.ResultInfo} \nr1:{r1}");
                    }
                }
            }
        }


        CornerBoxMeshData lData = new CornerBoxMeshData();
        VerticesToPlaneInfo topPlane = verticesToPlaneInfos.Find(i => i.IsCount(4, 4));
        VerticesToPlaneInfo bottomPlane = null;
        if (topPlane == null)
        {
            VerticesToPlaneInfo topPlane2 = verticesToPlaneInfos.Find(i => i.IsCount(2, 2));
            if (topPlane2 == null)
            {
                if (isShowDebug)
                {
                    Debug.LogError($"GetModelData_CornerBox topPlane == null No44 And No22 name:{this.name}");
                }
                return lData;
            }
            else
            {
                if (topPlane2.Plane2PointCount == 0)
                {
                    Debug.LogError($"GetModelData_CornerBox_Error1 topPlane2.Plane2PointCount == 0 name:{this.name}");
                    return lData;
                }
                topPlane2.AddPlane1Points(isShowDebug);
                string r1 = topPlane2.ResultInfo;
                topPlane2.AddPlane2PointsEx();
                if (isShowDebug)
                {
                    Debug.LogError($"GetModelData_CornerBox({this.name})[topPlane2] plane:{topPlane2.ResultInfo} \nr1:{r1}");
                }

                VerticesToPlaneInfo bottomPlane2 = VerticesToPlaneInfo.GetEndPlane(topPlane2, verticesToPlaneInfos);
                if (bottomPlane2.Plane2PointCount == 0)
                {
                    Debug.LogError($"GetModelData_CornerBox_Error2 bottomPlane2.Plane2PointCount == 0 name:{this.name}");
                    return lData;
                }
                bottomPlane2.AddPlane1Points(isShowDebug);
                string r2 = bottomPlane2.ResultInfo;
                bottomPlane2.AddPlane2PointsEx();
                if (isShowDebug)
                {
                    Debug.LogError($"GetModelData_CornerBox({this.name})[bottomPlane2] plane:{bottomPlane2.ResultInfo} \nr1:{r2}");
                }
                //if (isShowDebug)
                //{
                //    topPlane2.ShowPlaneInfo("NewTop", planInfoRootNew, PointScale, this.transform);
                //    bottomPlane2.ShowPlaneInfo("NewBottom", planInfoRootNew, PointScale, this.transform);
                //}

                //int id1 = verticesToPlaneInfos.IndexOf(topPlane);
                //int id2 = verticesToPlaneInfos.IndexOf(bottomPlane);
                if (topPlane2.IsCount(4, 4))
                {
                    topPlane = topPlane2;
                    bottomPlane = bottomPlane2;
                }
                else
                {
                    topPlane = bottomPlane2;
                    bottomPlane = topPlane2;
                }

                if (isShowDebug)
                {
                    topPlane.ShowPlaneInfo("NewTop", planInfoRootNew, PointScale, this.transform);
                    bottomPlane.ShowPlaneInfo("NewBottom", planInfoRootNew, PointScale, this.transform);
                }

                //verticesToPlaneInfos[id1] = topPlane;
                //verticesToPlaneInfos[id2] = bottomPlane;
            }
            //if (isShowDebug)
            //{
            //    Debug.LogError($"GetModelData_CornerBox topPlane == null 1 name:{this.name}");
            //}
            //return lData;
        }
        else
        {
            bottomPlane = VerticesToPlaneInfo.GetEndPlane(topPlane, verticesToPlaneInfos);
        }

        //VerticesToPlaneInfo beforePlane = verticesToPlaneInfos[0];
        VerticesToPlaneInfo beforePlane = verticesToPlaneInfos.Find(i => i.IsCount(6, 0));
        if (beforePlane == null)
        {
            if (isShowDebug)
            {
                Debug.LogError($"GetModelData_CornerBox beforePlane == null");
            }
            return lData;
        }
        VerticesToPlaneInfo afterPlane = VerticesToPlaneInfo.GetEndPlane(beforePlane, verticesToPlaneInfos);
        VerticesToPlaneInfo leftPlane = verticesToPlaneInfos.Find(i => i.IsCount(4, 2));
        if (leftPlane == null)
        {
            if (isShowDebug)
            {
                Debug.LogError($"GetModelData_CornerBox leftPlane == null");
            }
            return lData;
        }
        VerticesToPlaneInfo rightPlane = VerticesToPlaneInfo.GetEndPlane(leftPlane, verticesToPlaneInfos);
        //if (rightPlane.IsCount(4, 2)==false)
        //{
        //    if (isShowDebug)
        //    {
        //        Debug.LogError($"GetModelData_CornerBox IsCount(4, 2) == false rightPlane:{rightPlane}");
        //    }
        //    return lData;
        //}
        if (rightPlane.Plane2PointCount == 0)
        {
            rightPlane.AddPlane2PointsEx();
        }

        if (rightPlane.Plane2PointCount == 0)
        {
            if (isShowDebug)
            {
                Debug.LogError($"GetModelData_CornerBox_Error3 rightPlane.Plane2PointCount == 0 name:{this.name}");
            }
            return lData;
        }

        var size = beforePlane.GetSize();
        lData.forward = beforePlane.DirectionToPlane(afterPlane, transform, "Length");
        lData.length = beforePlane.DistanceToPlaneEx(afterPlane, transform, "Length", isShowDebug, PointScale);
        float height = topPlane.DistanceToPlaneEx(bottomPlane, transform, "Height", isShowDebug, PointScale);
        lData.up = bottomPlane.DirectionToPlane(topPlane, transform, "Height");
        float width = leftPlane.DistanceToPlaneEx(rightPlane, transform, "Width", isShowDebug, PointScale);
        lData.width = width;
        lData.height = height;
        lData.cornerX = rightPlane.DistanceOfPlane12Ex(transform, "CornerX", isShowDebug, PointScale);
        lData.cornerY = topPlane.DistanceOfPlane12Ex(transform, "CornerY", isShowDebug, PointScale);
        //lData.SetSize(height, width);
        //Debug.Log($"GetLModelData sizeX:{size.x} sizeY:{size.y} height:{height} width:{width} planeSizeX:{beforePlane.Plane.SizeX} planeSizeY:{beforePlane.Plane.SizeY}");
        //lData.sizePlane = lData.height / 10f;
        //lData.sizeDetail = lData.sizePlane;
        lData.RoundTo(3);
        return lData;
    }

    

    //public List<LModelGenerateArg> GenerateLModel(List<VerticesToPlaneInfo> verticesToPlaneInfos, OrientedBoundingBox OBB, float angle)
    //{
    //    SteelModelLData lData = GetLModelData(verticesToPlaneInfos);
    //    lData.angle = angle;
    //    return GenerateModel_LMesh(lData);
    //}



    public override void GetModelInfo()
    {
        GetHModelInfo();
    }



    //public override void RendererModel()
    //{
    //    RendererModel_LMesh();
    //}



    
}

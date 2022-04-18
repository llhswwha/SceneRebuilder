using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelStructureBaseModel : BaseMeshModel
{
    public float minSamePlaneNormalDis = 0.0001f;

    private static VerticesToPlaneInfo GetEndPlane(VerticesToPlaneInfo startPlane, List<VerticesToPlaneInfo> verticesToPlaneInfos_All)
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

    public override void GetModelInfo()
    {
        //Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        //MeshTriangles meshTriangles = new MeshTriangles(mesh);
        ////Debug.Log($"GetModelInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        ////meshTriangles.ShowCirclesById(this.transform, PointScale, 0, 3, minRepeatPointDistance);
        //var planes = meshTriangles.GetSharedMeshTrianglesListByNormal(2, minSamePlaneNormalDis, this.name);
        //Debug.Log($"GetModelInfo planes:{planes.Count}");
        //var plane1 = planes[0];
        //var plane2 = planes[1];
        //if(plane1.TriangleCount==14 || plane2.TriangleCount == 14)
        //{
        //    //var min1=plane1.MinRadius
        //}
        //var plane3 = planes[2];
        //var plane4 = planes[3];
        //if (plane3.TriangleCount == 4 || plane4.TriangleCount == 4)
        //{

        //}


        //ClearChildren();

        ClearDebugInfoGos();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
        }
        oBBCollider.ShowObbInfo(true);

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

        GameObject planInfoRoot = DebugInfoRoot.NewGo("PipeModel_PlaneInfo", this.transform);

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);
        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        this.VertexCount = vs.Length;

        //2.Planes
        PlaneInfo[] planeInfos = OBB.GetPlaneInfos();
        List<VerticesToPlaneInfo> verticesToPlaneInfos_All = new List<VerticesToPlaneInfo>();
        var verticesToPlaneInfos = new List<VerticesToPlaneInfo>();
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

        GameObject goNew = new GameObject(gameObject.name);
        goNew.transform.position = this.transform.position;

        HMeshGenerator hMesh = goNew.AddComponent<HMeshGenerator>();
        VerticesToPlaneInfo beforePlane= verticesToPlaneInfos[0];
        VerticesToPlaneInfo leftPlane = verticesToPlaneInfos[4];
        VerticesToPlaneInfo rightPlane = verticesToPlaneInfos[5];
        VerticesToPlaneInfo topPlane = verticesToPlaneInfos[2];
        hMesh.length = verticesToPlaneInfos[0].DebugDistanceToPlane(verticesToPlaneInfos[1], transform, "Length");// Vector3.Distance(verticesToPlaneInfos[0].Point.planeCenter, verticesToPlaneInfos[1].Point.planeCenter);
        //hMesh.height = topPlane.DebugDistanceToPlane(verticesToPlaneInfos[3], transform, "Height");//Vector3.Distance(verticesToPlaneInfos[2].Point.planeCenter, verticesToPlaneInfos[3].Point.planeCenter);
        //hMesh.width = leftPlane.DebugDistanceToPlane(rightPlane, transform, "Width");//Vector3.Distance(verticesToPlaneInfos[4].Point.planeCenter, verticesToPlaneInfos[5].Point.planeCenter);

        VerticesToPlaneInfo left2Top = new VerticesToPlaneInfo(leftPlane.Plane1Points.ToArray(), topPlane.Plane, true);

        //VerticesToPlaneInfo top2Left = new VerticesToPlaneInfo(topPlane.Plane1Points.ToArray(), leftPlane.Point, false);

        //oBBCollider.ShowPlaneInfo(leftPlane.Point, 0, planInfoRoot, leftPlane);

        oBBCollider.ShowPlaneInfo(topPlane.Plane, 7, planInfoRoot, left2Top);

        //oBBCollider.ShowPlaneInfo(leftPlane.Point, 1, planInfoRoot, top2Left);

        //oBBCollider.ShowPlaneInfo(topPlane.Point, 0, planInfoRoot, left2Top);


        PlaneInfo middlePlaneOfleftRight = PlaneInfo.GetMiddlePlane(leftPlane.Plane, rightPlane.Plane);
        VerticesToPlaneInfo middlePlaneVP = new VerticesToPlaneInfo(beforePlane.Plane1Points.ToArray(), middlePlaneOfleftRight, true);
        GameObject middlePlaneOfleftRightGo = DebugInfoRoot.NewGo("middlePlaneOfleftRight", this.transform);
        middlePlaneOfleftRight.ShowPlaneInfo(0, middlePlaneOfleftRightGo, middlePlaneVP, PointScale, this.transform);

        hMesh.sizeX = hMesh.width * 0.07f;
        //hMesh.sizeX = middlePlaneVP.GetPlaneHeight(transform, "sizeX");
        //hMesh.sizeY = hMesh.height * 0.03f;
        hMesh.sizeY = left2Top.DebugDistanceOfPlane12(transform, "sizeY");
        hMesh.sizeDetail = hMesh.sizeX * 2f;

        //VerticesToPlaneInfo middleOfleftRight=VerticesToPlaneInfo.GetMiddlePlane(leftPlane, rightPlane);


        hMesh.CreateLine();

        if (ResultGo != null)
        {
            GameObject.DestroyImmediate(ResultGo);
        }
        ResultGo = goNew;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PipeMeshGenerator : PipeMeshGeneratorBase
{
    public List<Vector3> points = new List<Vector3>();

    public List<Vector3> points2 = new List<Vector3>();

    //public class PipeLineInfo
    //{
    //    public Vector3 P1;
    //    public Vector3 P2;

    //    public bool generateElbows;

    //    public PipeLineInfo(Vector3 s,Vector3 e,bool g)
    //    {
    //        this.P1 = s;
    //        this.P2 = e;
    //        this.generateElbows = g;
    //    }
    //}

    public override void AlignDirection()
    {
        if (points.Count < 2)
        {
            Debug.LogError($"PipeMeshGenerator.AlignDirection points.Count < 2 count:{points.Count}");
            return;
        }
        Vector3 dir1 = points[0] - points[1];
        this.transform.up = dir1;
        
        Debug.Log($"AlignDirection p1:{points[0].Vector3ToString()} p2:{points[1].Vector3ToString()} dir:{dir1.Vector3ToString()}");
    }


    // see README.md file for more information about the following parameters



    public void GetPointsFromTransforms()
    {
        points.Clear();
        foreach(var go in pointsT)
        {
            points.Add(go.transform.localPosition);
        }
    }


    
    [ContextMenu("ShowPoints")]
    public void ShowPoints()
    {
        var ps = points;
        pointsT = ShowLocalPoints(ps);
    }

    [ContextMenu("ShowTransformPoints")]
    public void ShowTransformPoints()
    {
        var ps = GetTransformPoints(points);
        pointsT = ShowLocalPoints(ps);
    }

    public List<Vector3> GetTransformPoints(List<Vector3> ps0)
    {
        List<Vector3> ps = new List<Vector3>();
        var t = this.transform;
        var pos = t.position;
        foreach (var p in ps0)
        {
            
            var pNew = t.TransformPoint(p)- pos;
            ps.Add(pNew);
        }
        return ps;
    }

    [ContextMenu("ShowPoints2")]
    public void ShowPoints2()
    {
        pointsT = ShowLocalPoints(points2);
    }




    //public float pipeRadius1 = 0;
    //public float pipeRadius2 = 0;

    //public float pipeRadius = 0.2f;

    public override void SetRadiusUniform(int pt)
    {
        elbowRadius = GetRadiusValue(elbowRadius, pt);
        pipeRadius1 = GetRadiusValue(pipeRadius1, pt);
        pipeRadius2 = GetRadiusValue(pipeRadius2, pt);
        pipeRadius = GetRadiusValue(pipeRadius, pt);
    }



    //public bool IsLinkEndStart = false;

    //public List<Vector3> points_raw;

    //public bool generateWeld = false;

    //public float weldRadius = 0.1f;

    //public Material weldMaterial;

    void Start() {

        //points_raw = new List<Vector3>();
        //points_raw.AddRange(points);

        if (generateOnStart) {
            RenderPipe();
        }
    }

    public void RenderTorusXZ()
    {
        //PipeMeshGenerator torus = this.gameObject.AddComponent<PipeMeshGenerator>();
        //torus.

        List<Vector3> ps = new List<Vector3>();
        float r = elbowRadius;
        ps.Add(new Vector3(-r, 0, -r));
        ps.Add(new Vector3(r, 0, -r));
        ps.Add(new Vector3(r, 0, r));
        ps.Add(new Vector3(-r, 0, r));

        points = ps;
        this.IsLinkEndStart = true;
        this.generateWeld = false;
        RenderPipe();
    }

    public void RenderTorusYZ()
    {
        //PipeMeshGenerator torus = this.gameObject.AddComponent<PipeMeshGenerator>();
        //torus.

        List<Vector3> ps = new List<Vector3>();
        float r = elbowRadius;
        ps.Add(new Vector3(0,-r, -r));
        ps.Add(new Vector3(0, r,  -r));
        ps.Add(new Vector3(0, r, r));
        ps.Add(new Vector3(0, -r, r));

        points = ps;
        this.IsLinkEndStart = true;
        this.generateWeld = false;
        RenderPipe();
    }

    public void RenderTorusXY()
    {
        //PipeMeshGenerator torus = this.gameObject.AddComponent<PipeMeshGenerator>();
        //torus.

        List<Vector3> ps = new List<Vector3>();
        float r = elbowRadius;
        ps.Add(new Vector3(-r, -r, 0));
        ps.Add(new Vector3(r,  -r, 0));
        ps.Add(new Vector3(r, r, 0));
        ps.Add(new Vector3(-r, r, 0));
        points = ps;

        this.IsLinkEndStart = true;
        this.generateWeld = false;
        RenderPipe();
        
    }

    //public void ShowPoints()
    //{
    //    PointHelper.ShowPoints(points, new Vector3(0.05f, 0.05f, 0.05f),this.transform);
    //}
    public void RenderPipeFromTransforms()
    {
        points = new List<Vector3>();
        foreach(var t in pointsT)
        {
            if (t == null) continue;
            points.Add(t.transform.position-this.transform.position);
        }
        RenderPipe();
    }



    public override void RenderPipe() {

        if (uniformRadiusP > 0)
        {
            SetRadiusUniform(uniformRadiusP);
        }

        //List<Vector3> ps = GetPoints();
        List<Vector3> ps = new List<Vector3>(points);
        if (IsLinkEndStart)
        {
            ps = new List<Vector3>();
            ps.AddRange(points);
            ps.Add(points[0]);
            if (generateElbows)
            {
                ps.Add(points[1]);
            }
        }
        if (ps.Count < 2) {
            throw new System.Exception("Cannot render a pipe with fewer than 2 points");
        }
        RenderPipe(ps);
    }

    private void RenderPipe(List<Vector3> ps)
    {
        Childrens.Clear();
        CircleDatas.Clear();
        Mesh mesh = GeneratePipeMesh(ps, generateWeld);
        mesh.name = this.name;
        if(IsOnlyWeld==false)
        {
            SetMeshRenderers(mesh);
        }
    }



    public List<Vector3> GetPoints()
    {
        //List<Vector3> ps = new List<Vector3>();
        //ps.AddRange(points);
        //return ps;
        return GetTransformPoints(points);
    }

    [ContextMenu("GetVertexCount")]
    public void GetVertexCount()
    {
        MeshFilter mf = this.GetComponent<MeshFilter>();
        Debug.LogError($"vertexCount:{mf.mesh.vertexCount}");
    }





    GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction)
    {
        return GenerateWeld(vertices, normals, start, direction, pipeRadius);
    }

    protected GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction, float radius)
    {
        if (IsWeldSeperated)
        {
            float size = radius;
            if (weldCircleRadius > 0)
            {
                size = weldCircleRadius;
            }
            GameObject go = CreateLocalWeldGo(start, direction);
            go.transform.localScale = new Vector3(1, 2, 1);
            PipeWeldModel weldModel = go.AddComponent<PipeWeldModel>();
            weldModel.WeldData = new PipeWeldData(go.transform.position, direction, size, weldPipeRadius);
            //weldModel.ResultGo = go;
            weldModel.RendererModel();
            return go;
        }
        else
        {
            CircleMeshArg arg1 = MeshGeneratorHelper.GenerateCircleAtPoint(Vector3.zero, direction, 4, radius * 1.414213562373f);
            ////Debug.LogError($"Generate[{i}] start:{initialPoint} end:{endPoint} direction:{direction} gWeld:{gWeld} ps1:{ps1.Count}");

            GameObject p1 = PointHelper.ShowLocalPoint(start, new Vector3(0.05f, 0.05f, 0.05f), this.transform);
            Welds.Add(p1);

            GameObject go = CreateLocalWeldGo(start, direction);
            //GameObject go = CreateWorldWeldGo(start, direction);

            PointHelper.ShowLocalPoints(arg1.vertices, new Vector3(0.05f, 0.05f, 0.05f), go.transform);

            //float size = pipeRadius;
            float size = Vector3.Distance(arg1.vertices[0], arg1.vertices[1]) / 2;

            PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
            SetPipeMeshGenerator(weldGenerator, size);
            weldGenerator.points = arg1.vertices;
            weldGenerator.RenderPipe();
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return go;
        }
    }




    [ContextMenu("TestRemoveColinearPoints")]
    public void TestRemoveColinearPoints()
    {
        List<Vector3> ps = new List<Vector3>(points);
        RemoveColinearPoints(ps,true);
        points2 = new List<Vector3>(ps);
        ShowPoints2();
    }

    public bool IsRemoveColinearPoints = true;

    public class ElbowInfo
    {
        Vector3 initialPoint;
        Vector3 endPoint;
    }

    //public List<CylinderMeshData> PipeDatas = new List<CylinderMeshData>();

    //public List<ElbowMeshData> ElbowDatas = new List<ElbowMeshData>();

    //[ContextMenu("ShowCirclePoints")]
    //public void ShowCirclePoints()
    //{
    //    foreach(var pipe in PipeDatas)
    //    {
    //        foreach(var circle in pipe.Circles)
    //        {
    //            ShowPoints(circle.Vertices,circle.name);
    //        }
    //    }
    //    foreach (var pipe in ElbowDatas)
    //    {
    //        foreach (var circle in pipe.Circles)
    //        {
    //            ShowPoints(circle.Vertices, circle.name);
    //        }
    //    }
    //}

    //public void ShowCirclePointsAll()
    //{
    //    foreach (var pipe in PipeDatas)
    //    {
    //        foreach (var circle in pipe.Circles)
    //        {
    //            ShowPoints(circle.GetPoints(), circle.name);
    //        }
    //    }
    //    foreach (var pipe in ElbowDatas)
    //    {
    //        foreach (var circle in pipe.Circles)
    //        {
    //            ShowPoints(circle.GetPoints(), circle.name);
    //        }
    //    }
    //}

    public bool IsOnlyWeld = false;

    Mesh GeneratePipeMesh(List<Vector3> ps,bool gWeld) {
        Mesh m = new Mesh();
        m.name = "UnityPlumber Pipe";

        if (pipeRadius == 0)
        {
            Debug.LogError($"GeneratePipeMesh ps:{ps.Count} pipeRadius:{pipeRadius} gameObject:{this.name}");
            return m;
        }
        
        if(IsRemoveColinearPoints)
            RemoveColinearPoints(ps,false);
        points2 = new List<Vector3>(ps);
        //CircleDatas = new List<CircleMeshData>();
        //PipeDatas = new List<CylinderMeshData>();
        //ElbowDatas = new List<ElbowMeshData>();


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        //List<Vector3> ps = GetPoints();

        ClearWelds();

        if (IsLinkEndStart)
        {
            // for each segment, generate a cylinder
            for (int i = 0; i < ps.Count - 1; i++)
            {
                Vector3 initialPoint = ps[i];
                Vector3 endPoint = ps[i + 1];
                Vector3 dir = ps[i + 1] - ps[i];

                Vector3 directionN = (dir).normalized;
                float dirD = dir.magnitude;
                if (dirD < 0.000001)
                {
                    Debug.LogError($"【GeneratePipeMesh dirD < 0.000001】 gameObject:{this.name} parent:{this.transform.parent} initialPoint:{initialPoint} endPoint:{endPoint} dir:{dir} dirD:{dirD}");
                }

                if (generateElbows)
                {
                    // leave space for the elbow that will connect to the previous
                    // segment, except on the very first segment
                    initialPoint = initialPoint + directionN * elbowRadius;
                }

                if (i < ps.Count - 1 && generateElbows)
                {
                    // leave space for the elbow that will connect to the next
                    // segment, except on the last segment
                    endPoint = endPoint - directionN * elbowRadius;
                }
                // generate two circles with "pipeSegments" sides each and then
                // connect them to make the cylinder

                if(IsOnlyWeld==false)
                {
                    CircleMeshData circle1 = GenerateCircleAtPoint(vertices, normals, initialPoint, directionN, $"Pipe[{i}]_start", i,ps.Count,0,true);
                    CircleMeshData circle2 = GenerateCircleAtPoint(vertices, normals, endPoint, directionN, $"Pipe[{i}]_end", i, ps.Count,0,false);
                    MakeCylinderTriangles(triangles, i);
                }
                

                //CylinderMeshData cylinderMeshData = new CylinderMeshData(circle1, circle2);
                //PipeDatas.Add(cylinderMeshData);

                if (gWeld && i < ps.Count - 2)
                {
                    GenerateWeld(vertices, normals, initialPoint, directionN);
                    GenerateWeld(vertices, normals, endPoint, directionN);
                }
            }
        }
        else
        {
            // for each segment, generate a cylinder
            for (int i = 0; i < ps.Count - 1; i++)
            {
                Vector3 initialPoint = ps[i];
                Vector3 endPoint = ps[i + 1];
                Vector3 dir = ps[i + 1] - ps[i];
                float dirD = dir.magnitude;
                Vector3 directionN = (dir).normalized;
                if (dirD < 0.000001)
                {
                    Debug.LogError($"gameObject:{this.name} initialPoint:{initialPoint.Vector3ToString()} endPoint:{endPoint.Vector3ToString()} dir:{dir.Vector3ToString()} dirD:{dirD}"); 
                }
                if (i > 0 && generateElbows)
                {
                    // leave space for the elbow that will connect to the previous  // segment, except on the very first segment
                    initialPoint = initialPoint + directionN * elbowRadius;
                }
                if (i < ps.Count - 2 && generateElbows)
                {
                    // leave space for the elbow that will connect to the next // segment, except on the last segment
                    endPoint = endPoint - directionN * elbowRadius;
                }
                // generate two circles with "pipeSegments" sides each and then // connect them to make the cylinder

                if (IsOnlyWeld == false)
                {
                    CircleMeshData circle1 = GenerateCircleAtPoint(vertices, normals, initialPoint, directionN, $"Pipe({this.name})[{i}]_start", i, ps.Count, 0,true);
                    CircleMeshData circle2 = GenerateCircleAtPoint(vertices, normals, endPoint, directionN, $"Pipe({this.name})[{i}]_end", i, ps.Count, 0,false);
                    MakeCylinderTriangles(triangles, i);
                    //CylinderMeshData cylinderMeshData = new CylinderMeshData(circle1, circle2);
                    //PipeDatas.Add(cylinderMeshData);
                }

                GenerateWeldEx(ps, gWeld, vertices, normals, i, initialPoint, endPoint, directionN);
            }
        }


        // for each segment generate the elbow that connects it to the next one
        if (generateElbows && IsOnlyWeld==false)
        {
            if (IsGenerateElbowBeforeAfter)
            {
                for (int i = 0; i < ps.Count - 2; i++)
                {
                    Vector3 point1 = ps[i]; // starting point
                    Vector3 point2 = ps[i + 1]; // the point around which the elbow will be built
                    Vector3 point3 = ps[i + 2]; // next point
                    GenerateElbowBeforeAfter(ps, i, vertices, normals, triangles, point1, point2, point3,i);
                }
            }


            if (IsGenerateElbowsMain && elbowRadius>0 )
            {
                for (int i = 0; i < ps.Count - 2; i++)
                {
                    Vector3 point1 = ps[i]; // starting point
                    Vector3 point2 = ps[i + 1]; // the point around which the elbow will be built
                    Vector3 point3 = ps[i + 2]; // next point

                    ElbowMeshData elbowMeshData = GenerateElbow(ps, i, vertices, normals, triangles, point1, point2, point3);
                    //ElbowDatas.Add(elbowMeshData);

                    if (IsDebugElbow)
                    {
                        GameObject debugGo = new GameObject($"Elbow_{i}");
                        debugGo.transform.SetParent(this.transform);
                        debugGo.transform.localPosition = Vector3.zero;
                        PointHelper.ShowLocalPoints(new Vector3[] { point1, point2, point3 }, new Vector3(PointScale,PointScale,PointScale), debugGo.transform);
                    }
                    //break;
                }
            }
        }

        if (IsOnlyWeld == false)
        {
            if (generateEndCaps)
            {
                GenerateEndCaps(ps, vertices, triangles, normals);
            }
        }

        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0);
        m.SetNormals(normals);
        return m;
    }

    private void GenerateWeldEx(List<Vector3> ps, bool gWeld, List<Vector3> vertices, List<Vector3> normals, int i, Vector3 initialPoint, Vector3 endPoint, Vector3 directionN)
    {
        if (gWeld && i < ps.Count - 1)
        {
            if (IsElbow)
            {
                if (i == 0)
                {
                    GenerateWeld(vertices, normals, initialPoint, directionN);
                }
                else if (i == ps.Count - 2)
                {
                    GenerateWeld(vertices, normals, endPoint, directionN);
                }
            }
            else
            {
                if (IsGenerateEndWeld == false)
                {
                    if (i == 0)
                    {
                        GenerateWeld(vertices, normals, endPoint, directionN);
                    }
                    else if (i == ps.Count - 2)
                    {
                        GenerateWeld(vertices, normals, initialPoint, directionN);
                    }
                    else
                    {
                        GenerateWeld(vertices, normals, initialPoint, directionN);
                        GenerateWeld(vertices, normals, endPoint, directionN);
                    }
                }
                else
                {
                    GenerateWeld(vertices, normals, initialPoint, directionN);
                    GenerateWeld(vertices, normals, endPoint, directionN);
                }
            }

        }
    }

    //public bool IsGenerateEndWeld = false;

    //public bool IsElbow = false;

    //public bool IsGenerateElbowBeforeAfter = false;

    //public bool IsGenerateElbowsMain = true;

    //public bool IsDebugElbow = false;

    void RemoveColinearPoints(List<Vector3> points,bool isShowLog) {
        int count1 = points.Count;
        List<int> pointsToRemove = new List<int>();
        for (int i = 0; i < points.Count - 2; i++) {
            Vector3 point1 = points[i];
            Vector3 point2 = points[i + 1];
            Vector3 point3 = points[i + 2];

            Vector3 dir1 = point2 - point1;
            Vector3 dir2 = point3 - point2;

            // check if their directions are roughly the same by
            // comparing the distance between the direction vectors
            // with the threshold
            float dis = Vector3.Distance(dir1.normalized, dir2.normalized);
            if (dis < colinearThreshold) {
                pointsToRemove.Add(i + 1);
            }

            if(isShowLog)
                Debug.Log($"RemoveColinearPoints[{i}] dis:{dis} 【{dis < colinearThreshold}】 point1:{point1} point2:{point2} point3:{point3} dir1:{dir1} dir2:{dir2}");
        }

        pointsToRemove.Reverse();
        foreach (int idx in pointsToRemove) {
            points.RemoveAt(idx);
        }
        int count2 = points.Count;
        if (count1 != count2)
        {
            if (count1 == 5 && count2 != 3)
            {
                Debug.LogError($"RemoveColinearPoints[{this.name}] {count1}->{count2}");
            }
        }  

        if (isShowLog)
            Debug.LogError($"RemoveColinearPoints[{this.name}] {count1}->{count2}");
    }

    CircleMeshData GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction,string circleName,int index,int psCount,int circleType,bool isStart) {
        return GenerateCircleAtPointInner(vertices, normals, center, direction, pipeRadius, circleName,index,psCount, circleType,isStart);
    }

   

    //public List<CircleMeshData> CircleDatas = new List<CircleMeshData>();

    void MakeCylinderTriangles(List<int> triangles, int segmentIdx) {
        // connect the two circles corresponding to segment segmentIdx of the pipe
        int offset = segmentIdx * pipeSegments * 2;
        for (int i = 0; i < pipeSegments; i++) {
            triangles.Add(offset + (i + 1) % pipeSegments);
            triangles.Add(offset + i + pipeSegments);
            triangles.Add(offset + i);

            triangles.Add(offset + (i + 1) % pipeSegments);
            triangles.Add(offset + (i + 1) % pipeSegments + pipeSegments);
            triangles.Add(offset + i + pipeSegments);
        }
    }


    public float ElbowOffsetPower = 0.85f;

    void GenerateElbowBeforeAfter(List<Vector3> points, int elbowIdx, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3,int index)
    {
        //int offset = (points.Count - 1) * pipeSegments * 2; // all vertices of cylinders
        //offset += elbowIdx * (elbowSegments + 1) * pipeSegments; // all vertices of previous elbows
        //offset += segmentIdx * pipeSegments; // the current segment of the current elbow

        // generates the elbow around the area of point2, connecting the cylinders
        // corresponding to the segments point1-point2 and point2-point3
        Vector3 offset1 = (point2 - point1).normalized * elbowRadius;
        Vector3 offset2 = (point3 - point2).normalized * elbowRadius;

        Vector3 startPoint0 = point2 - offset1;
        Vector3 endPoint0 = point2 + offset2;


        Vector3 startPoint1 = point2 - offset1 * ElbowOffsetPower;
        Vector3 endPoint1 = point2 + offset2 * ElbowOffsetPower;


        //CircleMeshData cirlce11 = GenerateCircleAtPoint(vertices, normals, startPoint0, (startPoint1 - startPoint0).normalized, "Elbow_Start1");
        //CircleMeshData cirlce12 = GenerateCircleAtPoint(vertices, normals, startPoint1, (startPoint1 - startPoint0).normalized, "Elbow_Start2");
        //上面的startPoint1 - startPoint0似乎一样，但是计算后还是不一样，有误差。
        CircleMeshData cirlce11 = GenerateCircleAtPoint(vertices, normals, startPoint0, (point2 - point1).normalized, "Elbow_Start1",index, points.Count,1,true);
        CircleMeshData cirlce12 = GenerateCircleAtPoint(vertices, normals, startPoint1, (point2 - point1).normalized, "Elbow_Start2", index, points.Count,1,false);
        CylinderMeshData pipe1 = new CylinderMeshData(cirlce11, cirlce12);
        //PipeDatas.Add(pipe1);

        //int index1 = (points.Count - 1) + (elbowIdx + 1) * 2-1;
        int index1 = (points.Count - 1) + (elbowIdx) * 2;
        MakeCylinderTriangles(triangles, index1);

        //CircleMeshData cirlce21 = GenerateCircleAtPoint(vertices, normals, endPoint1, (endPoint0 - endPoint1).normalized, "Elbow_End1");
        //CircleMeshData cirlce22 = GenerateCircleAtPoint(vertices, normals, endPoint0, (endPoint0 - endPoint1).normalized, "Elbow_End2");
        CircleMeshData cirlce21 = GenerateCircleAtPoint(vertices, normals, endPoint1, (point3 - point2).normalized, "Elbow_End1", index, points.Count,1,true);
        CircleMeshData cirlce22 = GenerateCircleAtPoint(vertices, normals, endPoint0, (point3 - point2).normalized, "Elbow_End2", index, points.Count,1,false);
        MakeCylinderTriangles(triangles, index1 + 1);
        CylinderMeshData pipe2 = new CylinderMeshData(cirlce21, cirlce22);
        //PipeDatas.Add(pipe2);
    }

    public static float ElbowOffsetCrossPower = 100;

    ElbowMeshData GenerateElbow(List<Vector3> points,int index, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3) {

        //ShowPoint(point1, "point1", this.transform);
        //ShowPoint(point2, "point2", this.transform);
        //ShowPoint(point3, "point3", this.transform);

        // generates the elbow around the area of point2, connecting the cylinders
        // corresponding to the segments point1-point2 and point2-point3

        Vector3 v21 = point2 - point1;
        Vector3 v32 = point3 - point2;
        Vector3 offset1 = v21.normalized * elbowRadius;
        Vector3 offset2 = v32.normalized * elbowRadius;

        Vector3 startPoint = point2 - offset1;
        Vector3 endPoint = point2 + offset2;

        if (IsGenerateElbowBeforeAfter)
        {
            startPoint = point2 - offset1 * ElbowOffsetPower;
            endPoint = point2 + offset2 * ElbowOffsetPower;
        }

        //GenerateCircleAtPoint(vertices, normals, startPoint0, (startPoint - startPoint0).normalized);
        //GenerateCircleAtPoint(vertices, normals, startPoint, (startPoint - startPoint0).normalized);

        // auxiliary vectors to calculate lines parallel to the edge of each
        // cylinder, so the point where they meet can be the center of the elbow
        Vector3 perpendicularToBoth = Vector3.Cross(offset1 * ElbowOffsetCrossPower, offset2 * ElbowOffsetCrossPower);//offset1太小时会导致错误。
        Vector3 startDir = Vector3.Cross(perpendicularToBoth, offset1 * ElbowOffsetCrossPower).normalized;
        Vector3 endDir = Vector3.Cross(perpendicularToBoth, offset2 * ElbowOffsetCrossPower).normalized;

        // calculate torus arc center as the place where two lines projecting
        // from the edges of each cylinder intersect
        Vector3 torusCenter1;
        Vector3 torusCenter2;

        Math3D.ClosestPointsOnTwoLines(out torusCenter1, out torusCenter2, startPoint, startDir, endPoint, endDir);
        Vector3 torusCenter = 0.5f * (torusCenter1 + torusCenter2);

        //ShowPoint(startPoint, "startPoint1", this.transform);
        //ShowPoint(startPoint+ startDir, "endPoint1", this.transform);
        //ShowPoint(endPoint, "startPoint2", this.transform);
        //ShowPoint(endPoint+ endDir, "endPoint2", this.transform);
        //ShowPoint(torusCenter, "torusCenter", this.transform);
        //ShowPoint(torusCenter1, "torusCenter1", this.transform);
        //ShowPoint(torusCenter2, "torusCenter2", this.transform);

        // calculate actual torus radius based on the calculated center of the 
        // torus and the point where the arc starts
        float actualTorusRadius = (torusCenter - startPoint).magnitude;

        float angle = Vector3.Angle(startPoint - torusCenter, endPoint - torusCenter);
        float radiansPerSegment = (angle * Mathf.Deg2Rad) / elbowSegments;
        Vector3 lastPoint = point2 - startPoint;

        if (angle > 100)
        {
            Debug.LogWarning($"GenerateElbow go:{this.name} actualTorusRadius:{actualTorusRadius} angle:{angle} radiansPerSegment:{radiansPerSegment} v21:{v21.magnitude} elbowRadius:{elbowRadius} startDir:{startDir} endDir:{endDir}");
        }
        else
        {
            //Debug.Log($"GenerateElbow go:{this.name} actualTorusRadius:{actualTorusRadius} angle:{angle} radiansPerSegment:{radiansPerSegment} v21:{v21.magnitude} elbowRadius:{elbowRadius} startDir:{startDir} endDir:{endDir}");
        }

        

        ElbowMeshData elbowMeshData = new ElbowMeshData();
        for (int i = 0; i <= elbowSegments; i++) {
            // create a coordinate system to build the circular arc
            // for the torus segments center positions
            Vector3 xAxis = (startPoint - torusCenter).normalized;
            Vector3 yAxis = (endPoint - torusCenter).normalized;
            Vector3.OrthoNormalize(ref xAxis, ref yAxis);

            Vector3 circleCenter = torusCenter +
                (actualTorusRadius * Mathf.Cos(radiansPerSegment * i) * xAxis) +
                (actualTorusRadius * Mathf.Sin(radiansPerSegment * i) * yAxis);

            Vector3 direction = circleCenter - lastPoint;
            lastPoint = circleCenter;

            if (i == elbowSegments) {
                // last segment should always have the same orientation
                // as the next segment of the pipe
                direction = endPoint - point2;
            } else if (i == 0) {
                // first segment should always have the same orientation
                // as the how the previous segmented ended
                direction = point2 - startPoint;
            }
            //Debug.LogError($"GenerateElbow[{i}] circleCenter:{circleCenter} direction:{direction} lastPoint:{lastPoint}");
            CircleMeshData circle=GenerateCircleAtPoint(vertices, normals, circleCenter, direction,$"Elbow[{index},{i}]",i, points.Count,1,false);
            elbowMeshData.Circles.Add(circle);
            if (i > 0)
            {
                MakeElbowTriangles(points, vertices, triangles, i, index);
            }

            //if (i == elbowSegments)
            //{
            //    MakeElbowTriangles(points, vertices, triangles, i, index);
            //}
        }
        return elbowMeshData;
    }

    
}
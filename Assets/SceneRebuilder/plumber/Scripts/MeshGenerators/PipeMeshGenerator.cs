using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMeshGenerator : PipeMeshGeneratorBase
{

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

  

    // see README.md file for more information about the following parameters
    public List<Vector3> points = new List<Vector3>();

    public List<Vector3> points2 = new List<Vector3>();

    public List<GameObject> pointsT = new List<GameObject>();

    public List<PipeLineInfo> lines = new List<PipeLineInfo>();


    public void GetPointsFromTransforms()
    {
        points.Clear();
        foreach(var go in pointsT)
        {
            points.Add(go.transform.localPosition);
        }
    }

    public void CleanChildren()
    {
        List<GameObject> go = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            go.Add(transform.GetChild(i).gameObject);
        }
        foreach(var item in go)
        {
            GameObject.DestroyImmediate(item);
        }
    }
    
    [ContextMenu("ShowPoints")]
    public void ShowPoints()
    {
        pointsT = ShowPoints(points);
    }

    [ContextMenu("ShowPoints2")]
    public void ShowPoints2()
    {
        pointsT = ShowPoints(points2);
    }


    

    public float pipeRadius1 = 0;
    public float pipeRadius2 = 0;

    public float pipeRadius = 0.2f;



    public bool IsLinkEndStart = false;

    //public List<Vector3> points_raw;

    public bool generateWeld = false;

    public float weldRadius = 0.1f;

    public Material weldMaterial;

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



    public void RenderPipe() {

        List<Vector3> ps = GetPoints();
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
        Mesh mesh = GeneratePipeMesh(ps, generateWeld);
        SetMeshRenderers(mesh);
    }

    private void SetMeshRenderers(Mesh mesh)
    {
        // add mesh filter if not present
        MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
        MeshFilter mf = currentMeshFilter != null ? currentMeshFilter : gameObject.AddComponent<MeshFilter>();
        //Mesh mesh = GeneratePipeMesh(ps);

        if (flatShading)
            mesh = MakeFlatShading(mesh);
        if (makeDoubleSided)
            mesh = MakeDoubleSided(mesh);
        mf.mesh = mesh;

        // add mesh renderer if not present
        MeshRenderer currentMeshRenderer = GetComponent<MeshRenderer>();
        MeshRenderer mr = currentMeshRenderer != null ? currentMeshRenderer : gameObject.AddComponent<MeshRenderer>();
        mr.materials = new Material[1] { pipeMaterial };

        //Debug.LogError($"vertexCount:{mesh.vertexCount} gameObject:{gameObject.name}");
    }

    public List<Vector3> GetPoints()
    {
        List<Vector3> ps = new List<Vector3>();
        ps.AddRange(points);
        return ps;
    }

    [ContextMenu("GetVertexCount")]
    public void GetVertexCount()
    {
        MeshFilter mf = this.GetComponent<MeshFilter>();
        Debug.LogError($"vertexCount:{mf.mesh.vertexCount}");
    }

    public List<GameObject> Welds = new List<GameObject>();

    public bool IsWeldSeperated = true;

    private GameObject CreateWeldGo(Vector3 start, Vector3 direction)
    {
        GameObject go = new GameObject();
        go.name = $"Weld start:{start} direction:{direction}";
        go.transform.SetParent(this.transform);
        //go.transform.localPosition = Vector3.zero;
        go.transform.localPosition = start;
        Welds.Add(go);
        return go;
    }

    GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction)
    {
        return GenerateWeld(vertices, normals, start, direction, pipeRadius);
    }

    GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction,float radius)
    {
        if (IsWeldSeperated)
        {
            //GameObject go = new GameObject();
            //go.name = $"Weld start:{start} direction:{direction}";
            //go.transform.SetParent(this.transform);
            ////go.transform.localPosition = Vector3.zero;
            //go.transform.localPosition = start;
            //Welds.Add(go);

            GameObject go = CreateWeldGo(start, direction);

            float size = radius;
            PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
            SetPipeMeshGenerator(weldGenerator, size);
            weldGenerator.RenderTorusXZ();
            //weldGenerator.ShowPoints();
            go.transform.up = direction;
            return go;
        }
        else
        {
            CircleMeshArg arg1 = MeshGeneratorHelper.GenerateCircleAtPoint(Vector3.zero, direction, 4, radius * 1.414213562373f);
            ////Debug.LogError($"Generate[{i}] start:{initialPoint} end:{endPoint} direction:{direction} gWeld:{gWeld} ps1:{ps1.Count}");

            //var ms1 = GeneratePipeMesh(arg1.vertices, false);

            GameObject p1= PointHelper.ShowPoint(start, new Vector3(0.05f, 0.05f, 0.05f), this.transform);
            Welds.Add(p1);

            //GameObject go = new GameObject();
            //go.name = $"Weld start:{start} direction:{direction}";
            //go.transform.SetParent(this.transform);
            //go.transform.localPosition = start;
            //Welds.Add(go);

            GameObject go = CreateWeldGo(start, direction);

            PointHelper.ShowPoints(arg1.vertices, new Vector3(0.05f, 0.05f, 0.05f), go.transform);

            //float size = pipeRadius;
            float size = Vector3.Distance(arg1.vertices[0], arg1.vertices[1])/2;

            PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
            SetPipeMeshGenerator(weldGenerator, size);
            weldGenerator.points = arg1.vertices;
            weldGenerator.RenderPipe();

            //Debug.LogError($"arg1.vertices:{arg1.vertices.Count}| ms1.vertexCount:{ms1.vertexCount}");
            //for (int j = 0; j < ms1.vertexCount; j++)
            //{
            //    //vertices.Add(ms1.vertices[j]);
            //    //normals.Add(ms1.normals[j]);
            //    //ShowPoints(ms1.vertices, new Vector3(0.05f, 0.05f, 0.05f));
            //}
            return go;

        }
    }

    private void SetPipeMeshGenerator(PipeMeshGenerator weldGenerator,float size)
    {
        weldGenerator.generateOnStart = false;
        //weldGenerator.points = arg1.vertices;
        weldGenerator.pipeMaterial = this.weldMaterial;
        weldGenerator.flatShading = this.flatShading;
        weldGenerator.avoidStrangling = this.avoidStrangling;
        if (weldGenerator.pipeMaterial == null)
        {
            weldGenerator.pipeMaterial = this.pipeMaterial;
        }
        weldGenerator.elbowRadius = size;
        //weldGenerator.pipeRadius = size / 5;
        //weldGenerator.pipeRadius = size / 10;
        weldGenerator.pipeRadius = weldRadius;
        weldGenerator.pipeSegments = 12;
        weldGenerator.IsLinkEndStart = true;
    }

    [ContextMenu("TestRemoveColinearPoints")]
    public void TestRemoveColinearPoints()
    {
        List<Vector3> ps = new List<Vector3>(points);
        RemoveColinearPoints(ps);
        points2 = new List<Vector3>(ps);
    }

    public bool IsRemoveColinearPoints = true;

    public class ElbowInfo
    {
        Vector3 initialPoint;
        Vector3 endPoint;
    }

    public List<CylinderMeshData> PipeDatas = new List<CylinderMeshData>();

    public List<ElbowMeshData> ElbowDatas = new List<ElbowMeshData>();

    [ContextMenu("ShowCirclePoints")]
    public void ShowCirclePoints()
    {
        foreach(var pipe in PipeDatas)
        {
            foreach(var circle in pipe.Circles)
            {
                ShowPoints(circle.Vertices,circle.name);
            }
        }
        foreach (var pipe in ElbowDatas)
        {
            foreach (var circle in pipe.Circles)
            {
                ShowPoints(circle.Vertices, circle.name);
            }
        }
    }

    public void ShowCirclePointsAll()
    {
        foreach (var pipe in PipeDatas)
        {
            foreach (var circle in pipe.Circles)
            {
                ShowPoints(circle.GetPoints(), circle.name);
            }
        }
        foreach (var pipe in ElbowDatas)
        {
            foreach (var circle in pipe.Circles)
            {
                ShowPoints(circle.GetPoints(), circle.name);
            }
        }
    }

    Mesh GeneratePipeMesh(List<Vector3> ps,bool gWeld) {
        if (pipeRadius == 0)
        {
            Debug.LogError($"GeneratePipeMesh ps:{ps.Count} pipeRadius:{pipeRadius} gameObject:{this.name}");
        }
        
        if(IsRemoveColinearPoints)
            RemoveColinearPoints(ps);
        points2 = new List<Vector3>(ps);
        CircleDatas = new List<CircleMeshData>();
        PipeDatas = new List<CylinderMeshData>();
        ElbowDatas = new List<ElbowMeshData>();

        Mesh m = new Mesh();
        m.name = "UnityPlumber Pipe";
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        //List<Vector3> ps = GetPoints();

        foreach(var weld in Welds)
        {
            if (weld == null) continue;
            GameObject.DestroyImmediate(weld.gameObject);
        }
        Welds.Clear();

        if (IsLinkEndStart)
        {
            // for each segment, generate a cylinder
            for (int i = 0; i < ps.Count - 1; i++)
            {
                Vector3 initialPoint = ps[i];
                Vector3 endPoint = ps[i + 1];
                Vector3 direction = (ps[i + 1] - ps[i]).normalized;
                //Debug.LogError($"Generate[{i}] start:{initialPoint} end:{endPoint} direction:{direction} ");
                //if (i == ps.Count - 2)
                //{
                //    break;
                //}

                if (generateElbows)
                {
                    // leave space for the elbow that will connect to the previous
                    // segment, except on the very first segment
                    initialPoint = initialPoint + direction * elbowRadius;
                }

                if (i < ps.Count - 1 && generateElbows)
                {
                    // leave space for the elbow that will connect to the next
                    // segment, except on the last segment
                    endPoint = endPoint - direction * elbowRadius;
                }
                // generate two circles with "pipeSegments" sides each and then
                // connect them to make the cylinder
                CircleMeshData circle1 = GenerateCircleAtPoint(vertices, normals, initialPoint, direction,$"Pipe[{i}]_start");
                CircleMeshData circle2 = GenerateCircleAtPoint(vertices, normals, endPoint, direction, $"Pipe[{i}]_end");
                MakeCylinderTriangles(triangles, i);

                CylinderMeshData cylinderMeshData = new CylinderMeshData(circle1, circle2);
                PipeDatas.Add(cylinderMeshData);

                if (gWeld && i < ps.Count - 2)
                {
                    GenerateWeld(vertices, normals, initialPoint, direction);
                    GenerateWeld(vertices, normals, endPoint, direction);
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
                Vector3 direction = (ps[i + 1] - ps[i]).normalized;

                if (i > 0 && generateElbows)
                {
                    // leave space for the elbow that will connect to the previous
                    // segment, except on the very first segment
                    initialPoint = initialPoint + direction * elbowRadius;
                }

                if (i < ps.Count - 2 && generateElbows)
                {
                    // leave space for the elbow that will connect to the next
                    // segment, except on the last segment
                    endPoint = endPoint - direction * elbowRadius;
                }
                // generate two circles with "pipeSegments" sides each and then
                // connect them to make the cylinder

                CircleMeshData circle1 = GenerateCircleAtPoint(vertices, normals, initialPoint, direction, $"Pipe[{i}]_start");
                CircleMeshData circle2 = GenerateCircleAtPoint(vertices, normals, endPoint, direction, $"Pipe[{i}]_end");
                MakeCylinderTriangles(triangles, i);
                CylinderMeshData cylinderMeshData = new CylinderMeshData(circle1, circle2);
                PipeDatas.Add(cylinderMeshData);

                if (gWeld && i < ps.Count - 1)
                {
                    if (IsGenerateEndWeld == false)
                    {
                        if (i == 0)
                        {
                            GenerateWeld(vertices, normals, endPoint, direction);
                        }
                        else if (i == ps.Count - 2)
                        {
                            GenerateWeld(vertices, normals, initialPoint, direction);
                        }
                        else
                        {
                            GenerateWeld(vertices, normals, initialPoint, direction);
                            GenerateWeld(vertices, normals, endPoint, direction);
                        }
                    }
                    else
                    {
                        GenerateWeld(vertices, normals, initialPoint, direction);
                        GenerateWeld(vertices, normals, endPoint, direction);
                    }
                }
            }
        }


        // for each segment generate the elbow that connects it to the next one
        if (generateElbows)
        {
            if (IsGenerateElbowBeforeAfter)
            {
                for (int i = 0; i < ps.Count - 2; i++)
                {
                    Vector3 point1 = ps[i]; // starting point
                    Vector3 point2 = ps[i + 1]; // the point around which the elbow will be built
                    Vector3 point3 = ps[i + 2]; // next point
                    GenerateElbowBeforeAfter(ps, i, vertices, normals, triangles, point1, point2, point3);
                }
            }


            if (IsGenerateElbowsMain)
            {
                for (int i = 0; i < ps.Count - 2; i++)
                {
                    Vector3 point1 = ps[i]; // starting point
                    Vector3 point2 = ps[i + 1]; // the point around which the elbow will be built
                    Vector3 point3 = ps[i + 2]; // next point

                    ElbowMeshData elbowMeshData=GenerateElbow(ps, i, vertices, normals, triangles, point1, point2, point3);
                    ElbowDatas.Add(elbowMeshData);

                    if (IsDebugElbow)
                    {
                        GameObject debugGo = new GameObject($"Elbow_{i}");
                        debugGo.transform.SetParent(this.transform);
                        debugGo.transform.localPosition = Vector3.zero;
                        PointHelper.ShowPoints(new Vector3[] { point1, point2, point3 }, new Vector3(0.05f, 0.05f, 0.05f), debugGo.transform);

                        //
                    }
                }
            }
        }

        if (generateEndCaps) {
            GenerateEndCaps(ps, vertices, triangles, normals);
        }

        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0);
        m.SetNormals(normals);
        return m;
    }

    public bool IsGenerateEndWeld = false;

    public bool IsGenerateElbowBeforeAfter = false;

    public bool IsGenerateElbowsMain = true;

    public bool IsDebugElbow = false;

    void RemoveColinearPoints(List<Vector3> points) {
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

            //Debug.Log($"RemoveColinearPoints[{i}] dis:{dis} 【{dis < colinearThreshold}】 point1:{point1} point2:{point2} point3:{point3} dir1:{dir1} dir2:{dir2}");
        }

        pointsToRemove.Reverse();
        foreach (int idx in pointsToRemove) {
            points.RemoveAt(idx);
        }
        int count2 = points.Count;
        //Debug.LogError($"RemoveColinearPoints {count1}->{count2}");
    }

    CircleMeshData GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction,string circleName) {

        //List<Vector3> newVertics = new List<Vector3>();
        //// 'direction' is the normal to the plane that contains the circle

        //// define a couple of utility variables to build circles
        //float twoPi = Mathf.PI * 2;
        //float radiansPerSegment = twoPi / pipeSegments;

        //// generate two axes that define the plane with normal 'direction'
        //// we use a plane to determine which direction we are moving in order
        //// to ensure we are always using a left-hand coordinate system
        //// otherwise, the triangles will be built in the wrong order and
        //// all normals will end up inverted!
        //Plane p = new Plane(Vector3.forward, Vector3.zero);
        //Vector3 xAxis = Vector3.up;
        //Vector3 yAxis = Vector3.right;
        //if (p.GetSide(direction)) {
        //    yAxis = Vector3.left;
        //}

        //// build left-hand coordinate system, with orthogonal and normalized axes
        //Vector3.OrthoNormalize(ref direction, ref xAxis, ref yAxis);

        //for (int i = 0; i < pipeSegments; i++) {
        //    Vector3 currentVertex =
        //        center +
        //        (pipeRadius * Mathf.Cos(radiansPerSegment * i) * xAxis) +
        //        (pipeRadius * Mathf.Sin(radiansPerSegment * i) * yAxis);
        //    vertices.Add(currentVertex);
        //    newVertics.Add(currentVertex);
        //    normals.Add((currentVertex - center).normalized);
        //}

        //CircleMeshData circleData = new CircleMeshData(center, direction, newVertics, circleName);
        //circleData.SetAxis(xAxis, yAxis);
        //CircleDatas.Add(circleData);

        //return circleData;

        return GenerateCircleAtPoint(vertices, normals, center, direction, pipeRadius, circleName);
    }

    CircleMeshData GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction, float radius,string circleName)
    {
        
        List<Vector3> newVertics = new List<Vector3>();
        // 'direction' is the normal to the plane that contains the circle

        // define a couple of utility variables to build circles
        float twoPi = Mathf.PI * 2;
        float radiansPerSegment = twoPi / pipeSegments;

        // generate two axes that define the plane with normal 'direction'
        // we use a plane to determine which direction we are moving in order
        // to ensure we are always using a left-hand coordinate system
        // otherwise, the triangles will be built in the wrong order and
        // all normals will end up inverted!
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis = Vector3.up;
        Vector3 yAxis = Vector3.right;
        if (p.GetSide(direction))
        {
            yAxis = Vector3.left;
        }

        // build left-hand coordinate system, with orthogonal and normalized axes
        Vector3.OrthoNormalize(ref direction, ref xAxis, ref yAxis);

        for (int i = 0; i < pipeSegments; i++)
        {
            Vector3 currentVertex =
                center +
                (radius * Mathf.Cos(radiansPerSegment * i) * xAxis) +
                (radius * Mathf.Sin(radiansPerSegment * i) * yAxis);
            vertices.Add(currentVertex);
            newVertics.Add(currentVertex);
            normals.Add((currentVertex - center).normalized);

            //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
        }

        CircleMeshData circleData = new CircleMeshData(center, direction, newVertics, circleName);
        circleData.SetAxis(xAxis, yAxis);
        CircleDatas.Add(circleData);

        Debug.Log($"GenerateCircleAtPoint center:{center} direction:{direction} radius:{radius} vertices:{vertices.Count} newVertics:{newVertics.Count}");
        return circleData;
    }

    public List<CircleMeshData> CircleDatas = new List<CircleMeshData>();

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

   

    void MakeElbowTriangles(List<Vector3> points,List<Vector3> vertices, List<int> triangles, int segmentIdx, int elbowIdx) {
        // connect the two circles corresponding to segment segmentIdx of an
        // elbow with index elbowIdx
        int offset = (points.Count - 1) * pipeSegments * 2; // all vertices of cylinders
        if (IsGenerateElbowBeforeAfter)
        {
            offset = (points.Count - 1 + (points.Count - 2) * 2) * pipeSegments * 2; // all vertices of cylinders
        }

        offset += elbowIdx * (elbowSegments + 1) * pipeSegments; // all vertices of previous elbows
        offset += segmentIdx * pipeSegments; // the current segment of the current elbow

        // algorithm to avoid elbows strangling under dramatic
        // direction changes... we basically map vertices to the
        // one closest in the previous segment
        Dictionary<int, int> mapping = new Dictionary<int, int>();
        if (avoidStrangling) {
            List<Vector3> thisRingVertices = new List<Vector3>();
            List<Vector3> lastRingVertices = new List<Vector3>();

            for (int i = 0; i < pipeSegments; i++) {
                lastRingVertices.Add(vertices[offset + i - pipeSegments]);
            }

            for (int i = 0; i < pipeSegments; i++) {
                // find the closest one for each vertex of the previous segment
                Vector3 minDistVertex = Vector3.zero;
                float minDist = Mathf.Infinity;
                for (int j = 0; j < pipeSegments; j++) {
                    Vector3 currentVertex = vertices[offset + j];
                    float distance = Vector3.Distance(lastRingVertices[i], currentVertex);
                    if (distance < minDist) {
                        minDist = distance;
                        minDistVertex = currentVertex;
                    }
                }
                thisRingVertices.Add(minDistVertex);
                mapping.Add(i, vertices.IndexOf(minDistVertex));
            }
        } else {
            // keep current vertex order (do nothing)
            for (int i = 0; i < pipeSegments; i++) {
                mapping.Add(i, offset + i);
            }
        }

        // build triangles for the elbow segment
        for (int i = 0; i < pipeSegments; i++) {
            triangles.Add(mapping[i]);
            triangles.Add(offset + i - pipeSegments);
            triangles.Add(mapping[(i + 1) % pipeSegments]);

            triangles.Add(offset + i - pipeSegments);
            triangles.Add(offset + (i + 1) % pipeSegments - pipeSegments);
            triangles.Add(mapping[(i + 1) % pipeSegments]);
        }
    }

    Mesh MakeFlatShading(Mesh mesh) {
        // in order to achieve flat shading all vertices need to be
        // duplicated, because in Unity normals are assigned to vertices
        // and not to triangles.
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            // for each face we need to clone vertices and assign normals
            int vertIdx1 = mesh.triangles[i];
            int vertIdx2 = mesh.triangles[i + 1];
            int vertIdx3 = mesh.triangles[i + 2];

            newVertices.Add(mesh.vertices[vertIdx1]);
            newVertices.Add(mesh.vertices[vertIdx2]);
            newVertices.Add(mesh.vertices[vertIdx3]);

            newTriangles.Add(newVertices.Count - 3);
            newTriangles.Add(newVertices.Count - 2);
            newTriangles.Add(newVertices.Count - 1);

            Vector3 normal = Vector3.Cross(
                mesh.vertices[vertIdx2] - mesh.vertices[vertIdx1],
                mesh.vertices[vertIdx3] - mesh.vertices[vertIdx1]
            ).normalized;
            newNormals.Add(normal);
            newNormals.Add(normal);
            newNormals.Add(normal);
        }

        mesh.SetVertices(newVertices);
        mesh.SetTriangles(newTriangles, 0);
        mesh.SetNormals(newNormals);

        return mesh;
    }

    Mesh MakeDoubleSided(Mesh mesh) {
        // duplicate all triangles with inverted normals so the mesh
        // can be seen both from the outside and the inside
        List<int> newTriangles = new List<int>(mesh.triangles);

        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            int vertIdx1 = mesh.triangles[i];
            int vertIdx2 = mesh.triangles[i + 1];
            int vertIdx3 = mesh.triangles[i + 2];

            newTriangles.Add(vertIdx3);
            newTriangles.Add(vertIdx2);
            newTriangles.Add(vertIdx1);
        }

        mesh.SetTriangles(newTriangles, 0);

        return mesh;
    }

    public float ElbowOffsetPower = 0.85f;

    void GenerateElbowBeforeAfter(List<Vector3> points, int elbowIdx, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3)
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
        CircleMeshData cirlce11 = GenerateCircleAtPoint(vertices, normals, startPoint0, (point2 - point1).normalized, "Elbow_Start1");
        CircleMeshData cirlce12 = GenerateCircleAtPoint(vertices, normals, startPoint1, (point2 - point1).normalized, "Elbow_Start2");
        CylinderMeshData pipe1 = new CylinderMeshData(cirlce11, cirlce12);
        PipeDatas.Add(pipe1);

        //int index1 = (points.Count - 1) + (elbowIdx + 1) * 2-1;
        int index1 = (points.Count - 1) + (elbowIdx) * 2;
        MakeCylinderTriangles(triangles, index1);

        //CircleMeshData cirlce21 = GenerateCircleAtPoint(vertices, normals, endPoint1, (endPoint0 - endPoint1).normalized, "Elbow_End1");
        //CircleMeshData cirlce22 = GenerateCircleAtPoint(vertices, normals, endPoint0, (endPoint0 - endPoint1).normalized, "Elbow_End2");
        CircleMeshData cirlce21 = GenerateCircleAtPoint(vertices, normals, endPoint1, (point3 - point2).normalized, "Elbow_End1");
        CircleMeshData cirlce22 = GenerateCircleAtPoint(vertices, normals, endPoint0, (point3 - point2).normalized, "Elbow_End2");
        MakeCylinderTriangles(triangles, index1 + 1);
        CylinderMeshData pipe2 = new CylinderMeshData(cirlce21, cirlce22);
        PipeDatas.Add(pipe2);
    }

    ElbowMeshData GenerateElbow(List<Vector3> points,int index, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3) {
        // generates the elbow around the area of point2, connecting the cylinders
        // corresponding to the segments point1-point2 and point2-point3
        Vector3 offset1 = (point2 - point1).normalized * elbowRadius;
        Vector3 offset2 = (point3 - point2).normalized * elbowRadius;

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
        Vector3 perpendicularToBoth = Vector3.Cross(offset1, offset2);
        Vector3 startDir = Vector3.Cross(perpendicularToBoth, offset1).normalized;
        Vector3 endDir = Vector3.Cross(perpendicularToBoth, offset2).normalized;

        // calculate torus arc center as the place where two lines projecting
        // from the edges of each cylinder intersect
        Vector3 torusCenter1;
        Vector3 torusCenter2;
        Math3D.ClosestPointsOnTwoLines(out torusCenter1, out torusCenter2, startPoint, startDir, endPoint, endDir);
        Vector3 torusCenter = 0.5f * (torusCenter1 + torusCenter2);

        // calculate actual torus radius based on the calculated center of the 
        // torus and the point where the arc starts
        float actualTorusRadius = (torusCenter - startPoint).magnitude;

        float angle = Vector3.Angle(startPoint - torusCenter, endPoint - torusCenter);
        float radiansPerSegment = (angle * Mathf.Deg2Rad) / elbowSegments;
        Vector3 lastPoint = point2 - startPoint;


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
            CircleMeshData circle=GenerateCircleAtPoint(vertices, normals, circleCenter, direction,$"Elbow[{index},{i}]");
            elbowMeshData.Circles.Add(circle);
            if (i > 0)
            {
                MakeElbowTriangles(points,vertices, triangles, i, index);
            }
        }
        return elbowMeshData;
    }

    
}
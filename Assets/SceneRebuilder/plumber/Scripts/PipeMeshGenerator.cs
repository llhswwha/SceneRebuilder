﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMeshGenerator : MonoBehaviour {

    // see README.md file for more information about the following parameters
    public List<Vector3> points;
    public float pipeRadius = 0.2f;
    public float elbowRadius = 0.5f;
    [Range(3, 32)]
    public int pipeSegments = 8;
    [Range(3, 32)]
    public int elbowSegments = 6;
    public Material pipeMaterial;
    public bool flatShading;
    public bool avoidStrangling;
    public bool generateEndCaps;
    public bool generateElbows = true;
    public bool generateOnStart;
    public bool makeDoubleSided;
    public float colinearThreshold = 0.001f;

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
        ps.Add(new Vector3(-elbowRadius, 0, -elbowRadius));
        ps.Add(new Vector3(elbowRadius, 0, -elbowRadius));
        ps.Add(new Vector3(elbowRadius, 0, elbowRadius));
        ps.Add(new Vector3(-elbowRadius, 0, elbowRadius));

        points = ps;

        RenderPipe();
    }

    public void RenderTorusYZ()
    {
        //PipeMeshGenerator torus = this.gameObject.AddComponent<PipeMeshGenerator>();
        //torus.

        List<Vector3> ps = new List<Vector3>();
        ps.Add(new Vector3(0,-elbowRadius, -elbowRadius));
        ps.Add(new Vector3(0, elbowRadius,  -elbowRadius));
        ps.Add(new Vector3(0, elbowRadius,  elbowRadius));
        ps.Add(new Vector3(0, -elbowRadius,  elbowRadius));

        points = ps;

        RenderPipe();
    }

    public void RenderTorusXY()
    {
        //PipeMeshGenerator torus = this.gameObject.AddComponent<PipeMeshGenerator>();
        //torus.

        List<Vector3> ps = new List<Vector3>();
        ps.Add(new Vector3(-elbowRadius, -elbowRadius, 0));
        ps.Add(new Vector3(elbowRadius,  -elbowRadius, 0));
        ps.Add(new Vector3(elbowRadius,  elbowRadius, 0));
        ps.Add(new Vector3(-elbowRadius,  elbowRadius, 0));

        points = ps;

        RenderPipe();
    }

    public void ShowPoints()
    {
        PointHelper.ShowPoints(points, new Vector3(0.05f, 0.05f, 0.05f),this.transform);
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

        // remove any colinear points, as creating elbows between them
        // would result in a torus of infinite radius, which is generally
        // frowned upon. also, it helps in keeping the triangle count low. :)
        //RemoveColinearPoints(ps);
        Mesh mesh = GeneratePipeMesh(ps, generateWeld);

        //// add mesh filter if not present
        //MeshFilter currentMeshFilter = GetComponent<MeshFilter>();
        //MeshFilter mf = currentMeshFilter != null ? currentMeshFilter : gameObject.AddComponent<MeshFilter>();
        //Mesh mesh = GeneratePipeMesh(ps);

        //if (flatShading)
        //    mesh = MakeFlatShading(mesh);
        //if (makeDoubleSided)
        //    mesh = MakeDoubleSided(mesh);
        //mf.mesh = mesh;

        //// add mesh renderer if not present
        //MeshRenderer currentMeshRenderer = GetComponent<MeshRenderer>();
        //MeshRenderer mr = currentMeshRenderer != null ? currentMeshRenderer : gameObject.AddComponent<MeshRenderer>();
        //mr.materials = new Material[1] { pipeMaterial };
        SetMeshRenderers(mesh);

        //Debug.LogError($"vertexCount:{mesh.vertexCount}");
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

        Debug.LogError($"vertexCount:{mesh.vertexCount} gameObject:{gameObject.name}");
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

    void GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction)
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

            float size = pipeRadius;
            PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
            SetPipeMeshGenerator(weldGenerator, size);
            weldGenerator.RenderTorusXZ();
            //weldGenerator.ShowPoints();
            go.transform.up = direction;
            
        }
        else
        {
            CircleMeshArg arg1 = MeshGeneratorHelper.GenerateCircleAtPoint(Vector3.zero, direction, 4, pipeRadius* 1.414213562373f);
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
        weldGenerator.pipeRadius = size / 6;
        weldGenerator.pipeSegments = 12;
        weldGenerator.IsLinkEndStart = true;
    }

    Mesh GeneratePipeMesh(List<Vector3> ps,bool gWeld) {

        RemoveColinearPoints(ps);

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
                GenerateCircleAtPoint(vertices, normals, initialPoint, direction);
                GenerateCircleAtPoint(vertices, normals, endPoint, direction);
                if (gWeld && i < ps.Count - 2)
                {
                    GenerateWeld(vertices, normals, initialPoint, direction);
                    GenerateWeld(vertices, normals, endPoint, direction);
                }

                MakeCylinderTriangles(triangles, i);
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
                GenerateCircleAtPoint(vertices, normals, initialPoint, direction);
                GenerateCircleAtPoint(vertices, normals, endPoint, direction);

                if (gWeld && i < ps.Count - 1)
                {
                    GenerateWeld(vertices, normals, initialPoint, direction);
                    GenerateWeld(vertices, normals, endPoint, direction);
                }
                MakeCylinderTriangles(triangles, i);
            }
        }



        // for each segment generate the elbow that connects it to the next one
        if (generateElbows)
        {
            for (int i = 0; i < ps.Count - 2; i++)
            {
                Vector3 point1 = ps[i]; // starting point
                Vector3 point2 = ps[i + 1]; // the point around which the elbow will be built
                Vector3 point3 = ps[i + 2]; // next point
                GenerateElbow(ps,i, vertices, normals, triangles, point1, point2, point3);
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

    void RemoveColinearPoints(List<Vector3> points) {
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
            if (Vector3.Distance(dir1.normalized, dir2.normalized) < colinearThreshold) {
                pointsToRemove.Add(i + 1);
            }
        }

        pointsToRemove.Reverse();
        foreach (int idx in pointsToRemove) {
            points.RemoveAt(idx);
        }
    }

    List<Vector3> GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction) {

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
        if (p.GetSide(direction)) {
            yAxis = Vector3.left;
        }

        // build left-hand coordinate system, with orthogonal and normalized axes
        Vector3.OrthoNormalize(ref direction, ref xAxis, ref yAxis);

        for (int i = 0; i < pipeSegments; i++) {
            Vector3 currentVertex =
                center +
                (pipeRadius * Mathf.Cos(radiansPerSegment * i) * xAxis) +
                (pipeRadius * Mathf.Sin(radiansPerSegment * i) * yAxis);
            vertices.Add(currentVertex);
            newVertics.Add(currentVertex);
            normals.Add((currentVertex - center).normalized);
        }
        return newVertics;
    }

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

    void GenerateElbow(List<Vector3> points,int index, List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 point1, Vector3 point2, Vector3 point3) {
        // generates the elbow around the area of point2, connecting the cylinders
        // corresponding to the segments point1-point2 and point2-point3
        Vector3 offset1 = (point2 - point1).normalized * elbowRadius;
        Vector3 offset2 = (point3 - point2).normalized * elbowRadius;
        Vector3 startPoint = point2 - offset1;
        Vector3 endPoint = point2 + offset2;

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

            GenerateCircleAtPoint(vertices, normals, circleCenter, direction);

            if (i > 0) {
                MakeElbowTriangles(points,vertices, triangles, i, index);
            }
        }
    }

    void GenerateEndCaps(List<Vector3> points,List<Vector3> vertices, List<int> triangles, List<Vector3> normals) {
        // create the circular cap on each end of the pipe
        int firstCircleOffset = 0;
        int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        vertices.Add(points[0]); // center of first segment cap
        int firstCircleCenter = vertices.Count - 1;
        normals.Add(points[0] - points[1]);

        vertices.Add(points[points.Count - 1]); // center of end segment cap
        int secondCircleCenter = vertices.Count - 1;
        normals.Add(points[points.Count - 1] - points[points.Count - 2]);

        for (int i = 0; i < pipeSegments; i++) {
            triangles.Add(firstCircleCenter);
            triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
            triangles.Add(firstCircleOffset + i);

            triangles.Add(secondCircleOffset + i);
            triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
            triangles.Add(secondCircleCenter);
        }
    }
}
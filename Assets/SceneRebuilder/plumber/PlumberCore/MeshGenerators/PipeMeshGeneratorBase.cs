using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

public class PipeMeshGeneratorBase : BaseMeshGenerator
{

    public void SetMeshRenderers(Mesh mesh)
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
        //MeshRenderer mr = currentMeshRenderer != null ? currentMeshRenderer : gameObject.AddComponent<MeshRenderer>();
        //mr.materials = new Material[1] { pipeMaterial };
        if (currentMeshRenderer == null)
        {
            currentMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            currentMeshRenderer.materials = new Material[1] { pipeMaterial };
        }

        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            mc.sharedMesh = mesh;
        }

        //Debug.LogError($"vertexCount:{mesh.vertexCount} gameObject:{gameObject.name}");
    }

    Mesh MakeFlatShading(Mesh mesh)
    {
        // in order to achieve flat shading all vertices need to be
        // duplicated, because in Unity normals are assigned to vertices
        // and not to triangles.
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
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

    Mesh MakeDoubleSided(Mesh mesh)
    {
        // duplicate all triangles with inverted normals so the mesh
        // can be seen both from the outside and the inside
        List<int> newTriangles = new List<int>(mesh.triangles);

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
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

    public List<GameObject> pointsT = new List<GameObject>();

    public List<PipeLineInfo> lines = new List<PipeLineInfo>();

    public virtual void RenderPipe()
    {
    }

    public virtual void AlignDirection()
    {
        
    }

    public float pipeRadius1 = 0;
    public float pipeRadius2 = 0;

    public float pipeRadius = 0.2f;

    public int uniformRadiusP = 6;//焊缝的弯管段数 n*4

    public List<Transform> Childrens
    {
        get
        {
            List<Transform> ts = new List<Transform>();
            for(int i = 0; i < this.transform.childCount; i++)
            {
                ts.Add(this.transform.GetChild(i));
            }
            return ts;
        }
    }

    public void ClearResult()
    {
        DestroyMeshComponent();
        foreach(var child in Childrens)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public void DestroyMeshComponent()
    {
        //MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        //if (renderer)
        //{
        //    GameObject.DestroyImmediate(renderer);
        //}

        //MeshFilter filter = this.GetComponent<MeshFilter>();
        //if (filter)
        //{
        //    GameObject.DestroyImmediate(filter);
        //}
    }


    public float elbowRadius = 0.5f;
    [Range(3, 32)]
    public int pipeSegments = 12;
    [Range(0, 32)]
    public int elbowSegments = 6;

    [Range(3, 32)]
    public int weldPipeSegments = 6;//焊缝的管道面数

    [Range(1, 32)]
    public int weldElbowSegments = 6;//焊缝的弯管段数 n*4

    public Material pipeMaterial;
    public bool flatShading;
    public bool avoidStrangling;
    public bool avoidStrangling2 = true;
    public bool generateEndCaps;
    public float StartCapOffset = 0;
    public float EndCapOffset = 0;
    public bool generateElbows = true;
    public bool generateOnStart;
    public bool makeDoubleSided;
    //public float colinearThreshold = 0.001f;
    public float colinearThreshold = 0.002f;

    public bool generateWeld = false;

    public float weldPipeRadius = 0.1f;

    public bool IsLinkEndStart = false;

    public void SetWeldRadius(float r, PipeGenerateArg arg)
    {
        if (r < 0.01)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 0.6f;
        }
        else if (r > 0.8)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 2f;
        }
        else if (r > 0.5)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 1.5f;
        }
    }

    public void SetWeldRadius(float r1,float r2, PipeGenerateArg arg)
    {
        if (r1 < 0.025 || r2 < 0.025)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 0.6f;
        }
        else if (r1 > 0.8 || r2 > 0.8)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 2f;
        }
        else if (r1 > 0.5|| r2 > 0.5)
        {
            //pipe.weldRadius = 0.003f;
            weldPipeRadius = arg.weldRadius * 1.5f;
        }

    }

    public float weldCircleRadius = 0;

    public bool IsElbow = false;

    public bool IsGenerateEndWeld = false;

    public bool IsGenerateElbowBeforeAfter = false;

    public bool IsGenerateElbowsMain = true;

    public bool IsDebugElbow = false;

    public Material weldMaterial;

    public List<GameObject> Welds = new List<GameObject>();

    public void AddWeld(GameObject w)
    {
        Welds.Add(w);
        Childrens.Add(w.transform);
    }

    public bool IsWeldSeperated = true;

    public virtual void SetRadiusUniform(int pt)
    {
        elbowRadius = GetRadiusValue(elbowRadius, pt);
    }

    protected GameObject CreateLocalWeldGo(Vector3 start, Vector3 direction)
    {
        GameObject go = new GameObject();
        go.name = $"Weld start:{start} direction:{direction}";

        go.transform.SetParent(this.transform);
        //go.transform.localPosition = Vector3.zero;
        go.transform.localPosition = start;

        //go.transform.position = start;
        //go.transform.SetParent(this.transform);

        Welds.Add(go);
        return go;
    }

    protected GameObject CreateWorldWeldGo(Vector3 start, Vector3 direction)
    {
        GameObject go = new GameObject();
        go.name = $"Weld start:{start} direction:{direction}";

        //go.transform.SetParent(this.transform);
        ////go.transform.localPosition = Vector3.zero;
        //go.transform.localPosition = start;

        go.transform.position = this.transform.TransformPoint(start);
        go.transform.SetParent(this.transform);

        Welds.Add(go);
        return go;
    }

    protected void SetPipeMeshGenerator(PipeMeshGeneratorBase weldGenerator, float size)
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
        weldGenerator.pipeRadius = weldPipeRadius;
        weldGenerator.pipeSegments = weldPipeSegments;
        weldGenerator.elbowSegments = weldElbowSegments;
        weldGenerator.IsLinkEndStart = true;
    }

    public List<PipeWeldArg> WeldArgs = new List<PipeWeldArg>();

    public void ClearWelds()
    {
        foreach (var weld in Welds)
        {
            if (weld == null) continue;
            GameObject.DestroyImmediate(weld.gameObject);
        }
        Welds.Clear();

        WeldArgs.Clear();
    }

    public static float GetRadiusValue(float v,int pt)
    {
        //float p = Mathf.Pow(10,pt);
        //int a = Mathf.RoundToInt(v * p);
        //float b = a / p;
        //return b;
        return v.RoundToEx(pt);
    }

    //protected GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction, float radius)
    //{
    //    if (IsWeldSeperated)
    //    {
    //        //GameObject go = new GameObject();
    //        //go.name = $"Weld start:{start} direction:{direction}";
    //        //go.transform.SetParent(this.transform);
    //        ////go.transform.localPosition = Vector3.zero;
    //        //go.transform.localPosition = start;
    //        //Welds.Add(go);

    //        //GameObject go = CreateLocalWeldGo(start, direction);
    //        ////GameObject go = CreateWorldWeldGo(start, direction);
    //        //float size = radius;
    //        //if (weldCircleRadius > 0)
    //        //{
    //        //    size = weldCircleRadius;
    //        //}
    //        //PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
    //        //SetPipeMeshGenerator(weldGenerator, size);
    //        //weldGenerator.RenderTorusXZ();
    //        ////weldGenerator.ShowPoints();
    //        //go.transform.up = direction;
    //        //go.transform.localScale = new Vector3(1, 2, 1);
    //        ////Childrens.Add(go.transform);

    //        //MeshRenderer renderer = go.GetComponent<MeshRenderer>();
    //        //renderer.shadowCastingMode = ShadowCastingMode.Off;

    //        float size = radius;
    //        if (weldCircleRadius > 0)
    //        {
    //            size = weldCircleRadius;
    //        }
    //        GameObject go = CreateLocalWeldGo(start, direction);
    //        go.transform.localScale = new Vector3(1, 2, 1);
    //        PipeWeldModel weldModel = go.AddComponent<PipeWeldModel>();
    //        weldModel.WeldData = new PipeWeldData(go.transform.position, direction, size, weldPipeRadius);
    //        //weldModel.ResultGo = go;
    //        weldModel.RendererModel();
    //        return go;
    //    }
    //    else
    //    {
    //        CircleMeshArg arg1 = MeshGeneratorHelper.GenerateCircleAtPoint(Vector3.zero, direction, 4, radius * 1.414213562373f);
    //        ////Debug.LogError($"Generate[{i}] start:{initialPoint} end:{endPoint} direction:{direction} gWeld:{gWeld} ps1:{ps1.Count}");

    //        //var ms1 = GeneratePipeMesh(arg1.vertices, false);

    //        GameObject p1 = PointHelper.ShowLocalPoint(start, new Vector3(0.05f, 0.05f, 0.05f), this.transform);
    //        Welds.Add(p1);

    //        //GameObject go = new GameObject();
    //        //go.name = $"Weld start:{start} direction:{direction}";
    //        //go.transform.SetParent(this.transform);
    //        //go.transform.localPosition = start;
    //        //Welds.Add(go);

    //        GameObject go = CreateLocalWeldGo(start, direction);
    //        //GameObject go = CreateWorldWeldGo(start, direction);

    //        PointHelper.ShowLocalPoints(arg1.vertices, new Vector3(0.05f, 0.05f, 0.05f), go.transform);

    //        //float size = pipeRadius;
    //        float size = Vector3.Distance(arg1.vertices[0], arg1.vertices[1]) / 2;

    //        PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
    //        SetPipeMeshGenerator(weldGenerator, size);
    //        weldGenerator.points = arg1.vertices;
    //        weldGenerator.RenderPipe();

    //        //Debug.LogError($"arg1.vertices:{arg1.vertices.Count}| ms1.vertexCount:{ms1.vertexCount}");
    //        //for (int j = 0; j < ms1.vertexCount; j++)
    //        //{
    //        //    //vertices.Add(ms1.vertices[j]);
    //        //    //normals.Add(ms1.normals[j]);
    //        //    //ShowPoints(ms1.vertices, new Vector3(0.05f, 0.05f, 0.05f));
    //        //}

    //        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
    //        renderer.shadowCastingMode = ShadowCastingMode.Off;
    //        return go;

    //    }
    //}

    protected void GenerateEndCaps(List<Vector3> points, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    {
        //Debug.Log($"GenerateEndCaps EndCapOffset:{EndCapOffset} StartCapOffset:{StartCapOffset}");
        // create the circular cap on each end of the pipe
        int firstCircleOffset = 0;
        int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        var firstCircleNormal = points[0] - points[1];
        vertices.Add(points[0] + firstCircleNormal.normalized * StartCapOffset); // center of first segment cap
        int firstCircleCenter = vertices.Count - 1;
        
        //vertices.Add(points[0]); // center of first segment cap
        normals.Add(firstCircleNormal);
        //vertices.Add(points[0]); // center of first segment cap

        //vertices.Add(points[0]- firstCircleNormal.normalized*StartCapOffset); // center of first segment cap

        //vertices.Add(points[points.Count - 1]); // center of end segment cap
        //int secondCircleCenter = vertices.Count - 1;
        //normals.Add(points[points.Count - 1] - points[points.Count - 2]);

        //for (int i = 0; i < pipeSegments; i++) {
        //    triangles.Add(firstCircleCenter);
        //    triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(firstCircleOffset + i);

        //    triangles.Add(secondCircleOffset + i);
        //    triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(secondCircleCenter);
        //}

        //GameObject root = new GameObject("CapInfos");
        //root.transform.SetParent(this.transform);
        //root.transform.localPosition = Vector3.zero;

        for (int i = 0; i < pipeSegments ; i++)
        {
            //triangles.Add(firstCircleCenter);
            //triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
            //triangles.Add(firstCircleOffset + i);

            Vector3 p1 = vertices[firstCircleOffset + (i + 1) % pipeSegments];
            Vector3 p2 = vertices[firstCircleOffset + i];

            vertices.Add(p1);
            normals.Add(firstCircleNormal);

            vertices.Add(p2);
            normals.Add(firstCircleNormal);

            triangles.Add(firstCircleCenter);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            //ShowPoint(p1, $"point[{i + 1}][{firstCircleOffset + i}]", root.transform);
            //ShowPoint(p1, $"point[{i + 2}][{firstCircleOffset + (i + 1) % pipeSegments}]", root.transform);


        }
        Vector3 sencondCircleNormal = points[points.Count - 1] - points[points.Count - 2];
        vertices.Add(points[points.Count - 1] + sencondCircleNormal.normalized * EndCapOffset); // center of end segment cap
        int secondCircleCenter = vertices.Count - 1;
        
        normals.Add(sencondCircleNormal);
        //vertices.Add(points[points.Count - 1] + sencondCircleNormal.normalized * EndCapOffset); // center of end segment cap

        for (int i = 0; i < pipeSegments; i++)
        {
            //triangles.Add(secondCircleOffset + i);
            //triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
            //triangles.Add(secondCircleCenter);

            Vector3 p1 = vertices[secondCircleOffset + i];
            Vector3 p2 = vertices[secondCircleOffset + (i + 1) % pipeSegments];

            vertices.Add(p1);
            normals.Add(sencondCircleNormal);

            vertices.Add(p2);
            normals.Add(sencondCircleNormal);

            triangles.Add(secondCircleCenter);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            //ShowPoint(p1, $"point[{i + 1}][{secondCircleOffset + i}]", root.transform);
            //ShowPoint(p1, $"point[{i + 2}][{secondCircleOffset + (i + 1) % pipeSegments}]", root.transform);
        }
    }

    protected void GenerateEndCaps(List<Vector4> points4, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    {
        //Debug.Log($"GenerateEndCaps 4 points:{points4.Count} vertices:{vertices.Count} triangles:{triangles.Count}" );
        //// create the circular cap on each end of the pipe
        //int firstCircleOffset = 0;
        //int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        //vertices.Add(points[0]); // center of first segment cap
        //int firstCircleCenter = vertices.Count - 1;
        //normals.Add(points[0] - points[1]);

        //vertices.Add(points[points.Count - 1]); // center of end segment cap
        //int secondCircleCenter = vertices.Count - 1;
        //normals.Add(points[points.Count - 1] - points[points.Count - 2]);

        //for (int i = 0; i < pipeSegments; i++)
        //{
        //    triangles.Add(firstCircleCenter);
        //    triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(firstCircleOffset + i);

        //    triangles.Add(secondCircleOffset + i);
        //    triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(secondCircleCenter);
        //}

        List<Vector3> points3 = new List<Vector3>();
        foreach (var p in points4)
        {
            points3.Add(p);
        }
        GenerateEndCaps(points3, vertices, triangles, normals);
    }

    public float PointScale = 0.01f;

    public List<GameObject> ShowLocalPoints(List<Vector3> ps)
    {
        var psObjs = PointHelper.ShowLocalPoints(ps, new Vector3(PointScale, PointScale, PointScale), this.transform);
        return psObjs;
    }

    public GameObject ShowPoint(Vector3 p, string pName, Transform parent)
    {
        GameObject pObj = PointHelper.ShowLocalPoint(p, new Vector3(PointScale, PointScale, PointScale), this.transform);
        pObj.name = pName;
        pObj.transform.SetParent(parent);
        return pObj;
    }

    public List<GameObject> ShowPoints(List<Vector3> ps, string n)
    {
        GameObject go = new GameObject(n);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        var psObjs = PointHelper.ShowLocalPoints(ps, new Vector3(PointScale, PointScale, PointScale), go.transform);
        return psObjs;
    }

    public List<GameObject> ShowPoints(List<Vector4> ps, string n)
    {
        GameObject go = new GameObject(n);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), go.transform);
        return psObjs;
    }

    //public List<GameObject> ShowPoints(List<Vector3> ps)
    //{
    //    var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), this.transform);
    //    return psObjs;
    //}

    //public List<GameObject> ShowPoints(List<Vector3> ps, string n)
    //{
    //    GameObject go = new GameObject(n);
    //    go.transform.SetParent(this.transform);
    //    go.transform.localPosition = Vector3.zero;
    //    var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), go.transform);
    //    return psObjs;
    //}

    //public static void OrthoNormalize(ref Vector3 direction, ref Vector3 tangent, ref Vector3 binormal, string name, int index, int psCount, int circleType)
    //{
    //    Vector3 dir0 = direction;
    //    float dis = direction.magnitude;
    //    direction = direction * 100;
    //    Plane p = new Plane(Vector3.forward, Vector3.zero);
    //    Vector3 xAxis1 = Vector3.up;
    //    Vector3 yAxis1 = Vector3.right;
    //    if (p.GetSide(direction))
    //    {
    //        yAxis1 = Vector3.left;
    //    }



    //    // build left-hand coordinate system, with orthogonal and normalized axes
    //    Vector3.OrthoNormalize(ref direction, ref xAxis1, ref yAxis1);

    //    if (
    //        //circleType == 0 && 
    //        psCount > 1 && psCount < 5)// 2 3 4
    //    {
    //        Vector3 xAxis2 = Vector3.up;
    //        Vector3 yAxis2 = Vector3.right;
    //        if (p.GetSide(direction))
    //        {
    //            yAxis2 = Vector3.left;
    //        }
    //        Vector3.OrthoNormalize(ref direction, ref yAxis2, ref xAxis2);
    //        //[{Vector3.Dot(direction, xAxis)},{Vector3.Dot(direction, yAxis)},{Vector3.Dot(xAxis, yAxis)}]
    //        //float minDot = 0.0002f;
    //        float minDot = 0.001f;
    //        float minDot2 = 0.00001f;
    //        float dot1 = Mathf.Abs(Vector3.Dot(direction, xAxis1));
    //        float dot2 = Mathf.Abs(Vector3.Dot(direction, yAxis1));
    //        float dot3 = Mathf.Abs(Vector3.Dot(xAxis1, yAxis1));
    //        List<float> dotList1 = new List<float>() { dot1, dot2, dot3 };
    //        dotList1.Sort();
    //        if (dot1 > minDot || dot2 > minDot || dot3 > minDot)
    //        {
    //            float dot11 = Mathf.Abs(Vector3.Dot(direction, xAxis2));
    //            float dot22 = Mathf.Abs(Vector3.Dot(direction, yAxis2));
    //            float dot33 = Mathf.Abs(Vector3.Dot(xAxis2, yAxis2));

    //            if (dot11 > minDot || dot22 > minDot || dot33 > minDot)
    //            {
    //                string errorLog = $"OrthoNormalize Error!(1:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount})direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
    //                errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
    //                errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
    //                Debug.Log(errorLog);
    //            }
    //            else
    //            {
    //                List<float> dotList2 = new List<float>() { dot11, dot22, dot33 };
    //                dotList2.Sort();

    //                if (dotList1[2] > dotList2[2])
    //                {
    //                    if (dotList2[2] < minDot2)
    //                    {

    //                        xAxis1 = yAxis2;
    //                        yAxis1 = xAxis2;

    //                        if (dotList2[2] < 0.000001f)
    //                        {
    //                            //不需要打印
    //                        }
    //                        else
    //                        {
    //                            string errorLog = $"OrthoNormalize Error! (2:Change)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
    //                            errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
    //                            errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
    //                            Debug.LogWarning(errorLog);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        string errorLog = $"OrthoNormalize Error!(3:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
    //                        errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
    //                        errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
    //                        Debug.Log(errorLog);
    //                    }
    //                }
    //                else
    //                {
    //                    string errorLog = $"OrthoNormalize Error!(4:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
    //                    errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
    //                    errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
    //                    Debug.Log(errorLog);

    //                    //xAxis1 = yAxis2;
    //                    //yAxis1 = xAxis2;
    //                }
    //            }



    //        }
    //    }


    //    tangent = xAxis1;
    //    binormal = yAxis1;
    //}

    protected CircleMeshData GenerateCircleAtPointInner(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction, float radius, string circleName,int index,int psCount,int circleType,bool isStart)
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
        Vector3 xAxis1 = Vector3.up;
        Vector3 yAxis1 = Vector3.right;
        MeshGeneratorHelper.OrthoNormalize(ref direction, ref xAxis1, ref yAxis1,this.name, index,psCount, circleType);

        CircleMeshData lastCircleData = null;
        if (CircleDatas.Count > 0)
        {
            lastCircleData = CircleDatas.Last();
        }

        //for (int i = 0; i < pipeSegments; i++)
        //{
        //    Vector3 currentVertex =
        //        center +
        //        (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
        //        (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);

        //    if (IsBend)
        //    {
        //        if (isStart && lastCircleData != null)
        //        {
        //            Vector3 vertexClosed0 = lastCircleData.GetClosedPointEx(currentVertex);
        //            int closedId = lastCircleData.GetIndex(vertexClosed0);

        //            Vector3 vertexClosed = lastCircleData.GetPoint(i);

        //            float dis = Vector3.Distance(currentVertex, vertexClosed);
        //            Vector3 vertexNew = (currentVertex + vertexClosed) / 2;
        //            currentVertex = vertexNew;

        //            int id = vertices.IndexOf(vertexClosed);

        //            //Debug.Log($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");

        //            if (vertexClosed0 != vertexClosed)
        //            {
        //                Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
        //            }

        //            if (id == -1 || id > vertices.Count - 1)
        //            {
        //                Vector3 closedV = MeshHelper.GetClosedPoint(vertexClosed, vertices);
        //                int id2 = vertices.IndexOf(closedV);

        //                if (id2 == -1 || id2 > vertices.Count - 1)
        //                {
        //                    Debug.LogError($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
        //                }
        //                else
        //                {
        //                    Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id} | closedV:{closedV.Vector3ToString()} id2:{id2}");

        //                    vertices[id2] = vertexNew;
        //                    Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
        //                    normals[id2] = normalNew;
        //                }
        //            }
        //            else
        //            {
        //                vertices[id] = vertexNew;
        //                Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
        //                normals[id] = normalNew;
        //            }

        //        }
        //    }

        //    vertices.Add(currentVertex);
        //    newVertics.Add(currentVertex);
        //    normals.Add((currentVertex - center).normalized);

        //    //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
        //}

        #region 方案1
        /* 方案1
        if (IsBend)
        {
            List<Vector3> verticsOfIndex = new List<Vector3>();
            List<Vector3> verticsOfClosed = new List<Vector3>();
            List<int> indexOffset = new List<int>();
            if (isStart && lastCircleData != null)
            {
                for (int i = 0; i < pipeSegments; i++)
                {
                    Vector3 currentVertex =
                        center +
                        (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                        (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);

                    Vector3 vertexClosed = lastCircleData.GetClosedPointEx(currentVertex);
                    int closedId = lastCircleData.GetIndex(vertexClosed);

                    Vector3 vertexIndex = lastCircleData.GetPoint(i);

                    verticsOfIndex.Add(vertexIndex);
                    verticsOfClosed.Add(vertexClosed);
                    indexOffset.Add(closedId - i);

                    //float dis = Vector3.Distance(currentVertex, vertexIndex);
                    //Vector3 vertexNew = (currentVertex + vertexIndex) / 2;

                    //currentVertex = vertexNew;

                    //int id = vertices.IndexOf(vertexIndex);

                    ////Debug.Log($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");

                    //if (vertexClosed != vertexIndex)
                    //{
                    //    Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    //}

                    //if (id == -1 || id > vertices.Count - 1)
                    //{
                    //    Vector3 closedV = MeshHelper.GetClosedPoint(vertexIndex, vertices);
                    //    int id2 = vertices.IndexOf(closedV);

                    //    if (id2 == -1 || id2 > vertices.Count - 1)
                    //    {
                    //        Debug.LogError($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    //    }
                    //    else
                    //    {
                    //        Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id} | closedV:{closedV.Vector3ToString()} id2:{id2}");

                    //        vertices[id2] = vertexNew;
                    //        Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                    //        normals[id2] = normalNew;
                    //    }
                    //}
                    //else
                    //{
                    //    vertices[id] = vertexNew;
                    //    Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                    //    normals[id] = normalNew;
                    //}
                }
                //vertices.Add(currentVertex);
                //newVertics.Add(currentVertex);
                //normals.Add((currentVertex - center).normalized);
                //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);

                //verticsOfIndex.Add(currentVertex);

                bool isSameOffset = true;
                int off = indexOffset[0];
                for (int j = 1; j < indexOffset.Count; j++)
                {
                    if (indexOffset[j] != off)
                    {
                        isSameOffset = false;
                        break;
                    }
                }

                for (int i = 0; i < pipeSegments; i++)
                {
                    Vector3 currentVertex =
                        center +
                        (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                        (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);

                    Vector3 vertexClosed = verticsOfIndex[i];
                    int closedId = lastCircleData.GetIndex(vertexClosed);

                    Vector3 vertexIndex = verticsOfClosed[i];

                    Vector3 vertexOld = vertexIndex;
                    //if (isSameOffset)
                    //{
                    //    vertexOld = vertexClosed;
                    //}

                    float dis = Vector3.Distance(currentVertex, vertexOld);
                    Vector3 vertexNew = (currentVertex + vertexOld) / 2;

                    currentVertex = vertexNew;

                    int id = vertices.IndexOf(vertexOld);

                    //if (isSameOffset == false)
                    //{
                    //    Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    //}

                    Debug.Log($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");

                    if (vertexClosed != vertexIndex)
                    {
                        Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    }

                    if (id == -1 || id > vertices.Count - 1)
                    {
                        Vector3 closedV = MeshHelper.GetClosedPoint(vertexOld, vertices);
                        int id2 = vertices.IndexOf(closedV);

                        if (id2 == -1 || id2 > vertices.Count - 1)
                        {
                            Debug.LogError($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                        } 
                        else
                        {
                            Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexIndex.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id} | closedV:{closedV.Vector3ToString()} id2:{id2}");

                            vertices[id2] = vertexNew;
                            Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                            normals[id2] = normalNew;
                        }
                    }
                    else
                    {
                        vertices[id] = vertexNew;
                        Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                        normals[id] = normalNew;
                    }

                    vertices.Add(currentVertex);
                    newVertics.Add(currentVertex);
                    normals.Add((currentVertex - center).normalized);

                    //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
                }
            }
            else
            {
                for (int i = 0; i < pipeSegments; i++)
                {
                    Vector3 currentVertex =
                        center +
                        (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                        (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);
                    vertices.Add(currentVertex);
                    newVertics.Add(currentVertex);
                    normals.Add((currentVertex - center).normalized);

                    //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < pipeSegments; i++)
            {
                Vector3 currentVertex =
                    center +
                    (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                    (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);
                vertices.Add(currentVertex);
                newVertics.Add(currentVertex);
                normals.Add((currentVertex - center).normalized);

                //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
            }
        }
        */
        #endregion

        List<Vector3> verticsOfIndex = new List<Vector3>();
        List<Vector3> verticsOfClosed = new List<Vector3>();
        List<int> indexOffset = new List<int>();

        for (int i = 0; i < pipeSegments; i++)
        {
            Vector3 currentVertex =
                center +
                (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);

            if (IsBend)
            {
                if (isStart && lastCircleData != null)
                {
                    Vector3 vertexClosed0 = lastCircleData.GetClosedPointEx(currentVertex);
                    int closedId = lastCircleData.GetIndex(vertexClosed0);

                    Vector3 vertexClosed = lastCircleData.GetPoint(i);

                    float dis = Vector3.Distance(currentVertex, vertexClosed);

                    Vector3 vertexNew = (currentVertex + vertexClosed) / 2;
                    currentVertex = vertexNew;

                    int id = vertices.IndexOf(vertexClosed);

                    verticsOfIndex.Add(vertexClosed);
                    verticsOfClosed.Add(vertexClosed0);
                    indexOffset.Add(closedId - i);

                    //Debug.Log($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");

                    //if (vertexClosed0 != vertexClosed)
                    //{
                    //    Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId}_{closedId-i})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    //} 

                    //if (id == -1 || id > vertices.Count - 1)
                    //{
                    //    Vector3 closedV = MeshHelper.GetClosedPoint(vertexClosed, vertices);
                    //    int id2 = vertices.IndexOf(closedV);

                    //    if (id2 == -1 || id2 > vertices.Count - 1)
                    //    {
                    //        Debug.LogError($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    //    }
                    //    else
                    //    {
                    //        Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id} | closedV:{closedV.Vector3ToString()} id2:{id2}");

                    //        vertices[id2] = vertexNew;
                    //        Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                    //        normals[id2] = normalNew;
                    //    }
                    //}
                    //else
                    //{
                    //    vertices[id] = vertexNew;
                    //    Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                    //    normals[id] = normalNew;
                    //}

                }
            }

            //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
        }

        string isSameOffset = "Null";//为 
        bool isSame = true;
        if (indexOffset.Count == pipeSegments)
        {
            isSameOffset = "Same";
            isSame = true;
            int off = indexOffset[0];
            for (int j = 1; j < indexOffset.Count; j++)
            {
                if (indexOffset[j] != off && indexOffset[j] != off-indexOffset.Count)
                {
                    isSameOffset = "NotSame";
                    isSame = false;
                    break;
                }
            }
        }

        StringBuilder sbWarning = new StringBuilder();

        for (int i = 0; i < pipeSegments; i++)
        {
            Vector3 currentVertex =
                center +
                (radius * Mathf.Cos(radiansPerSegment * i) * xAxis1) +
                (radius * Mathf.Sin(radiansPerSegment * i) * yAxis1);

            if (IsBend)
            {
                if (isStart && lastCircleData != null)
                {
                    Vector3 vertexClosed0 = lastCircleData.GetClosedPointEx(currentVertex);
                    int closedId = lastCircleData.GetIndex(vertexClosed0);

                    Vector3 vertexClosed = lastCircleData.GetPoint(i);

                    if (isSame)
                    {
                        //Debug.Log($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId}_{closedId - i}_{isSameOffset})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} "); 

                        vertexClosed = vertexClosed0;
                    }

                    float dis = Vector3.Distance(currentVertex, vertexClosed);
                    Vector3 vertexNew = (currentVertex + vertexClosed) / 2;
                    currentVertex = vertexNew;

                    int id = vertices.IndexOf(vertexClosed);



                    if (isSame ==false &&  vertexClosed0 != vertexClosed)
                    {
                        //Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId}_{closedId - i}_{isSameOffset})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                        sbWarning.AppendLine($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId}_{closedId - i}_{isSameOffset})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                    } 

                    if (id == -1 || id > vertices.Count - 1)
                    {
                        Vector3 closedV = VertexHelper.GetClosedPoint(vertexClosed, vertices);
                        int id2 = vertices.IndexOf(closedV);

                        if (id2 == -1 || id2 > vertices.Count - 1)
                        {
                            Debug.LogError($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id}");
                        }
                        else
                        {
                            Debug.LogWarning($"GenerateCircleAtPoint[{this.name}][{index}_{i}/{pipeSegments}][{CircleDatas.Count}] ({closedId})vertexClosed:{vertexClosed.Vector3ToString()} currentVertex:{currentVertex.Vector3ToString()} dis:{dis} vertexNew:{vertexNew.Vector3ToString()} vertexId:{id} | closedV:{closedV.Vector3ToString()} id2:{id2}");

                            vertices[id2] = vertexNew;
                            Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                            normals[id2] = normalNew;
                        }
                    }
                    else
                    {
                        vertices[id] = vertexNew;
                        Vector3 normalNew = (vertexNew - lastCircleData.Center).normalized;
                        normals[id] = normalNew;
                    }

                }
            }

            vertices.Add(currentVertex);
            newVertics.Add(currentVertex);
            normals.Add((currentVertex - center).normalized);

            //ShowPoint(currentVertex, $"p:{vertices.Count}", this.transform);
        }

        string log = sbWarning.ToString();
        if (!string.IsNullOrEmpty(log))
        {
            Debug.LogWarning(log);
        }

        //CircleMeshData circleData = null;
        CircleMeshData circleData = new CircleMeshData(center, direction, newVertics, circleName);
        circleData.SetAxis(xAxis1, yAxis1);
        CircleDatas.Add(circleData);

        //Debug.Log($"GenerateCircleAtPoint[{circleName}][{circleType}][{CircleDatas.Count}] center:{center} radius:{radius} vertices:{vertices.Count} newVertics:{newVertics.Count}");
        var dr = direction.normalized;
        return circleData;
    }

    public bool IsBend = false;

    public List<CircleMeshData> CircleDatas = new List<CircleMeshData>();

    public void MakeElbowTriangles(List<Vector3> points, List<Vector3> vertices, List<int> triangles, int segmentIdx, int elbowIdx)
    {
        // connect the two circles corresponding to segment segmentIdx of an
        // elbow with index elbowIdx
        int offset = (points.Count - 1) * pipeSegments * 2; // all vertices of cylinders
        if (IsGenerateElbowBeforeAfter)
        {
            offset = (points.Count - 1 + (points.Count - 2) * 2) * pipeSegments * 2; // all vertices of cylinders
        }

        MakeElbowTriangles(vertices, triangles, segmentIdx, elbowIdx, offset);
    }

    private int MakeElbowTriangles(List<Vector3> vertices, List<int> triangles, int segmentIdx, int elbowIdx, int offset)
    {
        offset += elbowIdx * (elbowSegments + 1) * pipeSegments; // all vertices of previous elbows
        offset += segmentIdx * pipeSegments; // the current segment of the current elbow

        // algorithm to avoid elbows strangling under dramatic
        // direction changes... we basically map vertices to the
        // one closest in the previous segment
        Dictionary<int, int> mapping = new Dictionary<int, int>();
        if (avoidStrangling)
        {
            List<Vector3> thisRingVertices = new List<Vector3>();
            List<Vector3> lastRingVertices = new List<Vector3>();



            for (int i = 0; i < pipeSegments; i++)
            {
                lastRingVertices.Add(vertices[offset + i - pipeSegments]);
            }

            for (int i = 0; i < pipeSegments; i++)
            {
                int i1 = offset + i - pipeSegments;

                // find the closest one for each vertex of the previous segment
                Vector3 minDistVertex = Vector3.zero;
                float minDist = Mathf.Infinity;
                for (int j = 0; j < pipeSegments; j++)
                {
                    Vector3 currentVertex = vertices[offset + j];
                    if (avoidStrangling2)
                    {
                        if (thisRingVertices.Contains(currentVertex))
                        {
                            continue;
                        }
                    }
                    float distance = Vector3.Distance(lastRingVertices[i], currentVertex);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minDistVertex = currentVertex;
                    }
                }

                int i2 = vertices.IndexOf(minDistVertex);

                if (thisRingVertices.Contains(minDistVertex))
                {
                    Debug.LogWarning($"MakeElbowTriangles thisRingVertices.Contains(currentVertex) gameObject:[{this.name}] id:{i2}");
                    //ShowPoints(new List<Vector3>() { vertices[i1], minDistVertex, vertices[offset + i] }, $"error[{segmentIdx}][{i}]_{i1}_{ i2}_{offset + i}");
                }

                int i3 = offset + i;

                if (i3 != i2)
                {
                    Vector3 currentVertex = vertices[i3];
                    float distance = Vector3.Distance(lastRingVertices[i], currentVertex);


                    

                    if(distance - minDist < 0.001f)
                    {
                        //ShowPoints(new List<Vector3>() { vertices[i1], minDistVertex, vertices[i3] }, $"mapError1[{segmentIdx}][{i}]_{i1}_{i2}_{i3}");
                        //Debug.LogWarning($"MakeElbowTriangles gameObject:[{this.name}] MapError(1) minDist:{minDist} distance:{distance} disdis:{distance - minDist}");
                        thisRingVertices.Add(vertices[i3]);
                        mapping.Add(i, i3);
                    }
                    else
                    {
                        //ShowPoints(new List<Vector3>() { vertices[i1], minDistVertex, vertices[i3] }, $"mapError2[{segmentIdx}][{i}]_{i1}_{i2}_{i3}");
                        //Debug.LogWarning($"MakeElbowTriangles gameObject:[{this.name}] MapError(2) minDist:{minDist} distance:{distance} disdis:{distance - minDist}");
                        thisRingVertices.Add(minDistVertex);
                        mapping.Add(i, i2);
                    }
                }
                else
                {
                    //ShowPoints(new List<Vector3>() { vertices[i1], minDistVertex, vertices[i3] }, $"map[{segmentIdx}][{i}]_{i1}_{i2}_{i3}");
                    thisRingVertices.Add(minDistVertex);
                    mapping.Add(i, i2);
                }
            }
        }
        else
        {
            // keep current vertex order (do nothing)
            for (int i = 0; i < pipeSegments; i++)
            {
                mapping.Add(i, offset + i);
                int i1 = offset + i - pipeSegments;
                //ShowPoints(new List<Vector3>() { vertices[i1], vertices[offset + i] }, $"map[{segmentIdx}][{i}]_{i1}_{ offset + i}");
            }
        }

        // build triangles for the elbow segment
        for (int i = 0; i < pipeSegments; i++)
        {
            triangles.Add(mapping[i]);
            triangles.Add(offset + i - pipeSegments);
            triangles.Add(mapping[(i + 1) % pipeSegments]);

            triangles.Add(offset + i - pipeSegments);
            triangles.Add(offset + (i + 1) % pipeSegments - pipeSegments);
            triangles.Add(mapping[(i + 1) % pipeSegments]);
        }

        return offset;
    }

    public void MakeElbowTriangles(List<Vector4> points, List<Vector3> vertices, List<int> triangles, int segmentIdx, int elbowIdx)
    {
        // connect the two circles corresponding to segment segmentIdx of an
        // elbow with index elbowIdx
        int offset = (points.Count - 1) * pipeSegments * 2; // all vertices of cylinders
        if (IsGenerateElbowBeforeAfter)
        {
            offset = (points.Count - 1 + (points.Count - 2) * 2) * pipeSegments * 2; // all vertices of cylinders
        }

        MakeElbowTriangles(vertices, triangles, segmentIdx, elbowIdx, offset);
    }

    public void CleanChildren()
    {
        List<GameObject> go = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            go.Add(transform.GetChild(i).gameObject);
        }
        foreach (var item in go)
        {
            GameObject.DestroyImmediate(item);
        }
    }

    public void DebugShowTriangles()
    {
        CleanChildren();
        //ClearDebugInfoGos();
        MeshTriangles.DebugShowTriangles(this.gameObject, PointScale);
    }
}

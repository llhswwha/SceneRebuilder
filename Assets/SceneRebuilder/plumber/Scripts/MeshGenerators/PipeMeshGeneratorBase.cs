using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMeshGeneratorBase : MonoBehaviour
{


    public List<Transform> Childrens = new List<Transform>();

    public GameObject Target;

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
    //public float colinearThreshold = 0.001f;
    public float colinearThreshold = 0.002f;

    public bool generateWeld = false;

    public float weldRadius = 0.1f;

    public Material weldMaterial;

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

    private void SetPipeMeshGenerator(PipeMeshGenerator weldGenerator, float size)
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

    protected GameObject GenerateWeld(List<Vector3> vertices, List<Vector3> normals, Vector3 start, Vector3 direction, float radius)
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
            go.transform.localScale = new Vector3(1, 2, 1);
            Childrens.Add(go.transform);
            return go;
        }
        else
        {
            CircleMeshArg arg1 = MeshGeneratorHelper.GenerateCircleAtPoint(Vector3.zero, direction, 4, radius * 1.414213562373f);
            ////Debug.LogError($"Generate[{i}] start:{initialPoint} end:{endPoint} direction:{direction} gWeld:{gWeld} ps1:{ps1.Count}");

            //var ms1 = GeneratePipeMesh(arg1.vertices, false);

            GameObject p1 = PointHelper.ShowPoint(start, new Vector3(0.05f, 0.05f, 0.05f), this.transform);
            Welds.Add(p1);

            //GameObject go = new GameObject();
            //go.name = $"Weld start:{start} direction:{direction}";
            //go.transform.SetParent(this.transform);
            //go.transform.localPosition = start;
            //Welds.Add(go);

            GameObject go = CreateWeldGo(start, direction);

            PointHelper.ShowPoints(arg1.vertices, new Vector3(0.05f, 0.05f, 0.05f), go.transform);

            //float size = pipeRadius;
            float size = Vector3.Distance(arg1.vertices[0], arg1.vertices[1]) / 2;

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

    protected void GenerateEndCaps(List<Vector3> points, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    {
        // create the circular cap on each end of the pipe
        int firstCircleOffset = 0;
        int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        vertices.Add(points[0]); // center of first segment cap
        int firstCircleCenter = vertices.Count - 1;
        var firstCircleNormal = points[0] - points[1];
        normals.Add(firstCircleNormal);

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

        vertices.Add(points[points.Count - 1]); // center of end segment cap
        int secondCircleCenter = vertices.Count - 1;
        Vector3 sencondCircleNormal = points[points.Count - 1] - points[points.Count - 2];
        normals.Add(sencondCircleNormal);

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

    public List<GameObject> ShowPoints(List<Vector3> ps)
    {
        var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), this.transform);
        return psObjs;
    }

    public GameObject ShowPoint(Vector3 p, string pName, Transform parent)
    {
        GameObject pObj = PointHelper.ShowPoint(p, new Vector3(PointScale, PointScale, PointScale), this.transform);
        pObj.name = pName;
        pObj.transform.SetParent(parent);
        return pObj;
    }

    public List<GameObject> ShowPoints(List<Vector3> ps, string n)
    {
        GameObject go = new GameObject(n);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), go.transform);
        return psObjs;
    }

    public static void OrthoNormalize(ref Vector3 direction, ref Vector3 tangent, ref Vector3 binormal,string name,int psCount)
    {
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis1 = Vector3.up;
        Vector3 yAxis1 = Vector3.right;
        if (p.GetSide(direction))
        {
            yAxis1 = Vector3.left;
        }
        // build left-hand coordinate system, with orthogonal and normalized axes
        Vector3.OrthoNormalize(ref direction, ref xAxis1, ref yAxis1);

        if (psCount == 2)
        {
            Vector3 xAxis2 = Vector3.up;
            Vector3 yAxis2 = Vector3.right;
            if (p.GetSide(direction))
            {
                yAxis2 = Vector3.left;
            }
            Vector3.OrthoNormalize(ref direction, ref yAxis2, ref xAxis2);
            //[{Vector3.Dot(direction, xAxis)},{Vector3.Dot(direction, yAxis)},{Vector3.Dot(xAxis, yAxis)}]
            float minDot = 0.0002f;
            float dot1 = Mathf.Abs(Vector3.Dot(direction, xAxis1));
            float dot2 = Mathf.Abs(Vector3.Dot(direction, yAxis1));
            float dot3 = Mathf.Abs(Vector3.Dot(xAxis1, yAxis1));
            List<float> dotList1 = new List<float>() { dot1, dot2, dot3 };
            dotList1.Sort();
            if (dot1 > minDot || dot2 > minDot || dot3 > minDot)
            {
                float dot11 = Mathf.Abs(Vector3.Dot(direction, xAxis2));
                float dot22 = Mathf.Abs(Vector3.Dot(direction, yAxis2));
                float dot33 = Mathf.Abs(Vector3.Dot(xAxis2, yAxis2));

                List<float> dotList2 = new List<float>() { dot11, dot22, dot33 };
                dotList2.Sort();

                if (dotList1[2] > dotList2[2])
                {
                    string errorLog = $"OrthoNormalize Error! gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z}) ¡¾{dot1},{dot2},{dot3}¡¿";
                    errorLog += $"\ngameObject:{name} direction1:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})¡¾{dot1},{dot2},{dot3}¡¿";
                    errorLog += $"\ngameObject:{name} direction2:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})¡¾{dot11},{dot22},{dot33}¡¿";
                    Debug.LogWarning(errorLog);

                    xAxis1 = yAxis2;
                    yAxis1 = xAxis2;
                }
                else
                {
                    string errorLog = $"OrthoNormalize Error!(2) gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z}) ¡¾{dot1},{dot2},{dot3}¡¿";
                    errorLog += $"\ngameObject:{name} direction1:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})¡¾{dot1},{dot2},{dot3}¡¿";
                    errorLog += $"\ngameObject:{name} direction2:({direction.x},{direction.y},{direction.z}) xAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})¡¾{dot11},{dot22},{dot33}¡¿";
                    Debug.LogWarning(errorLog);

                    //xAxis1 = yAxis2;
                    //yAxis1 = xAxis2;
                }

            }
        }
        

        tangent = xAxis1;
        binormal = yAxis1;
    }

    protected CircleMeshData GenerateCircleAtPoint(List<Vector3> vertices, List<Vector3> normals, Vector3 center, Vector3 direction, float radius, string circleName,int psCount)
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
        OrthoNormalize(ref direction, ref xAxis1, ref yAxis1,this.name, psCount);

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

        CircleMeshData circleData = new CircleMeshData(center, direction, newVertics, circleName);
        circleData.SetAxis(xAxis1, yAxis1);
        CircleDatas.Add(circleData);

        //Debug.Log($"GenerateCircleAtPoint[{circleName}] center:{center} radius:{radius} vertices:{vertices.Count} newVertics:{newVertics.Count}");
        var dr = direction.normalized;
        return circleData;
    }


    public List<CircleMeshData> CircleDatas = new List<CircleMeshData>();
}

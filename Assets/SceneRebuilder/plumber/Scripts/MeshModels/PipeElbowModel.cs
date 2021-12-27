using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PipeBuilder;

public class PipeElbowModel : MonoBehaviour
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

    public Vector3 FindClosedPoint(Vector3 p,List<Vector3> list)
    {
        float minDis = float.MaxValue;
        Vector3 minP = Vector3.zero;
        foreach(var item in list)
        {
            float dis = Vector3.Distance(item, p);
            if (dis < minDis)
            {
                minDis = dis;
                minP = item;
            }
        }
        return minP;
    }

    // Start is called before the first frame update
    [ContextMenu("GetElbowInfo")]
    public void GetElbowInfo()
    {
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);
        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        List<Vector3> points = meshTriangles.GetKeyPointsById(sharedMinCount, minRepeatPointDistance);
        if (points.Count < 4)
        {
            Debug.LogError("GetKeyPointsById points.Count < 4 :" + points.Count);
            return;
        }
        List<PointDistance> distanceList = new List<PointDistance>();
        foreach (var p in points)
        {
            distanceList.Add(new PointDistance(p, meshTriangles.center));
        }
        distanceList.Sort();

        EndPointIn1 = distanceList[0].P1;
        EndPointIn2 = distanceList[1].P1;
        points.Remove(EndPointIn1);
        points.Remove(EndPointIn2);

        EndPointOut1 = FindClosedPoint(EndPointIn1, points);
        points.Remove(EndPointOut1);
        EndPointOut2 = FindClosedPoint(EndPointIn2, points);
        points.Remove(EndPointOut2);

        Line1 = new PipeLineInfo(EndPointOut1, EndPointIn1,null);
        Line2 = new PipeLineInfo(EndPointIn2, EndPointOut2, null);

        TransformHelper.ShowLocalPoint(EndPointOut1, PointScale, this.transform, null).name = "OutPoint1";
        TransformHelper.ShowLocalPoint(EndPointOut2, PointScale, this.transform, null).name = "OutPoint2";
        TransformHelper.ShowLocalPoint(EndPointIn1, PointScale, this.transform, null).name = "InPoint1";
        TransformHelper.ShowLocalPoint(EndPointIn2, PointScale, this.transform, null).name = "InPoint2";

        GetPipeRadius();
    }

    public void GetPipeRadius()
    {
        PipeRadius = meshTriangles.GetPipeRadius(sharedMinCount);
    }

    public float PipeRadius = 0;


    public void ShowKeyPoints()
    {
        ClearChildren();
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);
        meshTriangles.ShowKeyPointsById(this.transform, PointScale);
    }

    public void ShowSharedPoints()
    {
        ClearChildren();

        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        meshTriangles.ShowSharedPointsById(this.transform, PointScale);
        meshTriangles.ShowSharedPointsByPoint(this.transform, PointScale);
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
    }

    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public void CreateElbow()
    {
        RendererPipe(generateArg,true);
    }

    public GameObject RendererPipe(PipeGenerateArg arg,bool generateEndCaps)
    {
        PipeCreateArg pipeArg = new PipeCreateArg(Line1, Line2);
        var ps = pipeArg.GetGeneratePoints(0, 2, false);

        GameObject pipeNew = new GameObject(this.name + "_NewPipe");
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        pipe.pipeSegments = arg.pipeSegments;
        pipe.pipeMaterial = arg.PipeMaterial;
        pipe.weldMaterial = arg.WeldMaterial;
        pipe.weldRadius = arg.weldRadius;
        //pipe.generateWeld = arg.generateWeld;
        pipe.generateWeld = false;
        pipe.pipeRadius = PipeRadius;
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.generateEndCaps = generateEndCaps;
        pipe.RenderPipe();
        return pipeNew;
    }

    public Vector3 EndPointIn1 = Vector3.zero;
    public Vector3 EndPointOut1 = Vector3.zero;
    public Vector3 EndPointIn2 = Vector3.zero;
    public Vector3 EndPointOut2= Vector3.zero;

    public PipeLineInfo Line1 = new PipeLineInfo();

    public PipeLineInfo Line2 = new PipeLineInfo();

    public int sharedMinCount = 36;

    public float minRepeatPointDistance = 0.00002f;

    public Vector3 TestMeshOffset = new Vector3(0.1f, 0.1f,0.1f);

    private GameObject CreateSubTestObj(string objName,Transform parent)
    {
        GameObject objTriangles = new GameObject(objName);
        objTriangles.transform.SetParent(parent);
        objTriangles.transform.localPosition = Vector3.zero;
        return objTriangles;
    }

    public float PointScale = 0.01f;

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
    }
}

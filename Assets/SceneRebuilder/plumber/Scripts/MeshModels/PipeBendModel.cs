using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PipeBendModel : PipeModelBase
{
    public int linePartTriangleCount = 40;

    public PipeLineInfoList info = new PipeLineInfoList();

    public void ShowPartLines()
    {
        ClearDebugInfoGos();
        info = ShowPartLines(this.gameObject, PointScale, linePartTriangleCount, true);
        Debug.Log($"PipeBendModel ShowPartLines:{this.name} PipeLineInfoList:{info}");
    }

    public static PipeLineInfoList ShowPartLines(GameObject target, float PointScale, int count, bool isShowDebugObj)
    {
        Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);

        Debug.Log($"ShowPartLines mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowTriangles(target.transform, PointScale);
        List<MeshTriangles> subTriangles = meshTriangles.Split(count);
        PipeLineInfoList lineInfoList = new PipeLineInfoList();
        for (int i = 0; i < subTriangles.Count; i++)
        {
            GameObject tsRoot = CreateSubTestObj($"Parts {i + 1}({mesh.vertexCount})", target.transform);

            MeshTriangles subT = subTriangles[i];
            LogTag.logTag = tsRoot.name;
            PipeLineInfo lineInfo = ShowTrianglesWithObb(subT, tsRoot.transform, PointScale, $"[{i + 1}]", isShowDebugObj);
            lineInfoList.Add(lineInfo);
        }
        foreach (var subT in subTriangles)
        {
            subT.Dispose();
        }
        meshTriangles.Dispose();
        return lineInfoList;
    }

    public static PipeLineInfo ShowTrianglesWithObb(MeshTriangles meshTriangles, Transform t, float pointScale, string tag, bool isShowDebugObj)
    {
        PipeLineModel pipeLine = t.gameObject.AddMissingComponent<PipeLineModel>();
        pipeLine.lineSize = pointScale;
        PipeLineInfo pipeLineInfo = pipeLine.ShowLinePartModelInfo(meshTriangles.GetPoints().ToArray(), t, isShowDebugObj, 0.001f, 20, 100);
        if (pipeLineInfo == null)
        {
            List<List<Vector3>> pointLines = meshTriangles.GetLinePoints(t, pointScale, 0);
            if (pointLines.Count == 2)
            {
                pipeLineInfo = new PipeLineInfo(pointLines);
            }
            else
            {
                Debug.LogError($"ShowTrianglesWithObb[{tag}] pointLines.Count != 2 transform:{t.name}");
            }
        }
        return pipeLineInfo;
    }

    public override void GetModelInfo()
    {
        ////base.GetModelInfo();
        //ClearDebugInfoGos();
        //info = MeshTriangles.ShowPartLines(this.gameObject, PointScale, lineVertexCount, false);
        //Debug.Log($"PipeBendModel GetModelInfo1:{this.name} PipeLineInfoList:{info}");

        GetModelInfo_Job2();
    }

    public override void GetModelInfo_Job()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        PipeBendInfoJob.InitResult(1);
        PipeBendInfoJob job = new PipeBendInfoJob()
        {
            id = 0,
            partTriangleCount = linePartTriangleCount,
            mesh = new MeshStructure(mesh)
        };
        job.Execute();

        PipeBendData bendData = PipeBendInfoJob.Result[0];
        Debug.Log($"GetModelInfo_Job bendData:{bendData}");
        //PipeBendInfoJob.Result.Dispose();


        job.Dispose();

        SetModelData(bendData);
    }

    public void SetModelData(PipeBendData bendData)
    {
        info = new PipeLineInfoList(bendData);
    }

    public void GetModelInfo_Job2()
    {
        //ClearDebugInfoGos();
        //var info2 = MeshTriangles.ShowPartLines(this.gameObject, PointScale, 40, false);
        //Debug.Log($"PipeBendModel GetModelInfo1:{this.name} PipeLineInfoList:{info2}");

        //string s1 = info + "";

        //string s1 = info + "";
        //base.GetModelInfo_Job();
        ClearDebugInfoGos();
        info = GetPartLinesInfo_Job(this.gameObject, PointScale, 40, false);
        //string s2 = info + "";
        //Debug.Log($"PipeBendModel GetModelInfo2:{this.name} PipeLineInfoList:{info} \n\t{s1}\n\t{s2}");
    }

    public static PipeLineInfoList GetPartLinesInfo_Job(GameObject target, float PointScale, int count, bool isShowDebugObj)
    {
        Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);


        //meshTriangles.ShowTriangles(target.transform, PointScale);
        List<MeshTriangles> subTriangles = meshTriangles.Split(count);

        //Debug.Log($"GetPartLinesInfo_Job mesh vertexCount:{mesh.vertexCount} MeshTriangles:{mesh.triangles.Length} meshTriangles:{meshTriangles.Count} count:{count} subTriangles:{subTriangles.Count}");

        LogTag.logTag = target.name;
        PipeBendPartInfoJob.Result = new NativeArray<PipeLineData>(subTriangles.Count, Allocator.Persistent);
        PipeBendPartInfoJob.ErrorIds = new NativeList<int>(Allocator.Persistent);
        JobList<PipeBendPartInfoJob> jobs = new JobList<PipeBendPartInfoJob>(100);
        for (int i = 0; i < subTriangles.Count; i++)
        {
            MeshTriangles subT = subTriangles[i];
            Vector3[] vs = subT.GetPoints().ToArray();

            PipeBendPartInfoJob job = new PipeBendPartInfoJob()
            {
                id = i,
                meshTriangles = new Unity.Collections.NativeArray<MeshTriangle>(subT.Triangles.ToArray(), Unity.Collections.Allocator.Persistent)
            };
            jobs.Add(job);
        }
        jobs.CompleteAll();

        PipeLineInfoList lineInfoList = new PipeLineInfoList();
        for (int i = 0; i < subTriangles.Count; i++)
        {
            PipeLineData lineData = PipeBendPartInfoJob.Result[i];
            PipeLineInfo lineInfo = new PipeLineInfo(lineData, null);
            lineInfoList.Add(lineInfo);
        }

        PipeBendPartInfoJob.Result.Dispose();
        PipeBendPartInfoJob.ErrorIds.Dispose();

        foreach (var subT in subTriangles)
        {
            //subT.Dispose(); 
        }
        meshTriangles.Dispose();
        jobs.Dispose();

        return lineInfoList;
    }

    public override void ClearGo()
    {
        //Debug.Log($"PipeBendModel ClearGo1:{this.name}");
        base.ClearGo();
        info = new PipeLineInfoList();
        //Debug.Log($"PipeBendModel ClearGo:{this.name} PipeLineInfoList:{info}"); 
    }

    [ContextMenu("RendererElbowModel")]
    public GameObject RendererElbowModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"RendererElbow IsGetInfoSuccess == false :{this.name}");
            return null;
        }
        if (info == null || info.Count == 0)
        {
            Debug.LogError($"RendererElbow info==null :{this.name}");
            return null;
        }
        bool isNewGo = false;
        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, isNewGo);
        //PipeLineInfo startLine= PipeLineInfo
        PipeCreateArg pipeArg = new PipeCreateArg(info.GetLine1(), info.GetLine2());
        var ps = pipeArg.GetGeneratePoints(0, 2, false);
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = ps;
        //arg.SetArg(pipe);
        PipeFactory.Instance.SetArg(arg, pipe);

        pipe.IsGenerateEndWeld = true;
        //pipe.IsElbow = true;
        pipe.IsBend = true;
        //pipe.pipeRadius = (info.EndPointOut1.w + info.EndPointOut2.w) / 2;
        pipe.pipeRadius = info.GetRadius();
        pipe.elbowRadius = pipeArg.elbowRadius;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipe.gameObject;
    }

    public float startEndDis = 0.3f;

    public int elbowSegments = 3;

    [ContextMenu("RendererLinesModel")]
    public GameObject RendererLinesModel(PipeGenerateArg arg, string afterName)
    {
        if (IsGetInfoSuccess == false)
        {
            Debug.LogError($"RendererElbow IsGetInfoSuccess == false :{this.name}");
            return null;
        }
        if (info == null || info.Count == 0)
        {
            Debug.LogError($"RendererElbow info==null :{this.name}");
            return null;
        }
        bool isNewGo = false;
        PipeMeshGenerator pipe = GetGenerator<PipeMeshGenerator>(arg, afterName, isNewGo);

        //var ps = pipeArg.GetGeneratePoints(0, 2, false);
        //pipe.points = new List<Vector3>() { EndPointOut1, EndPointIn1, EndPointIn2, EndPointOut2 };
        pipe.points = info.GetLinePoints(startEndDis);

        //arg.SetArg(pipe);
        PipeFactory.Instance.SetArg(arg, pipe);

        pipe.IsGenerateEndWeld = true;
        pipe.IsElbow = true;
        pipe.IsBend = true;
        //pipe.pipeRadius = (info.EndPointOut1.w + info.EndPointOut2.w) / 2;
        pipe.pipeRadius = info.GetRadius();
        //pipe.elbowRadius = info.GetElbowRadius();
        pipe.elbowRadius = 0;

        //pipe.elbowSegments = elbowSegments;
        //pipe.elbowRadius = 0.015f;
        //pipe.elbowRadius = pipe.pipeRadius / 3;
        pipe.avoidStrangling = true;
        pipe.RenderPipe();
        return pipe.gameObject;
    }

    public override GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return RendererLinesModel(arg, afterName);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PipeBendInfoJob : IMeshInfoJob
{
    public int partTriangleCount;
    public int id;
    public MeshStructure mesh;

    public PipeBendData bendData;

    public static NativeArray<PipeBendData> Result;

    public static void InitResult(int count)
    {
        Result = new NativeArray<PipeBendData>(count, Allocator.Persistent);
    }
    public static NativeList<int> ErrorIds;

    public void Dispose()
    {
        mesh.Dispose();
    }

    public void Execute() 
    {
        //bendData.StartPoints = new NativeArray<Vector3>(4, Allocator.Persistent);
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        //meshTriangles.ShowTriangles(target.transform, PointScale);
        List<MeshTriangles> subTriangles = meshTriangles.Split(partTriangleCount);

        //Debug.Log($"PipeBendInfoJob mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length} TotalVertexCount:{partTriangleCount} subTriangles:{subTriangles.Count} Lines:{bendData.Count} id:{id}");

        //JobList<PipeBendPartInfoJob> jobs = new JobList<PipeBendPartInfoJob>(100);

        //bendData.Lines = new PipeLineData[subTriangles.Count];
        //bendData.Lines = new NativeArray<PipeLineData>(subTriangles.Count, Allocator.Persistent);

        //if (bendData.Count != subTriangles.Count)
        //{
        //    Debug.LogError($"PipeBendInfoJob bendData.Lines.Length != subTriangles.Count {bendData.Count}!={subTriangles.Count} id:{id}");
        //    //bendData.Lines = new NativeArray<PipeLineData>(subTriangles.Count, Allocator.Persistent);
        //    ErrorIds.Add(id);
        //}

        for (int i = 0; i < subTriangles.Count; i++)
        {
            MeshTriangles subT = subTriangles[i];

            //Vector3[] vs = subT.GetPoints().ToArray();

            //PipeBendPartInfoJob job = new PipeBendPartInfoJob()
            //{
            //    id = i,
            //    points = new Unity.Collections.NativeArray<Vector3>(vs, Unity.Collections.Allocator.Persistent)
            //};
            //jobs.Add(job);
            //job.Schedule().Complete();

            //bendData.Lines[i] = PipeBendPartInfoJob.StaticExecute(vs, i);

            bendData.Set(i, PipeBendPartInfoJob.StaticExecuteByTriangles(subT.Triangles.ToArray(), i,false));

            //Debug.LogWarning($"PipeBendInfoJob[{id}][{i}/{subTriangles.Count}] Result.Length:{Result.Length} ");
        }

        if (bendData.Count != subTriangles.Count)
        {
            Debug.LogError($"PipeBendInfoJob bendData.Lines.Length != subTriangles.Count {bendData.Count}!={subTriangles.Count} id:{id}");
            //bendData.Lines = new NativeArray<PipeLineData>(subTriangles.Count, Allocator.Persistent);
            ErrorIds.Add(id);
        }

        //foreach (var subT in subTriangles)
        //{
        //    subT.Dispose();
        //}
        //meshTriangles.Dispose();

        //return lineInfoList;

        //Debug.LogWarning($"PipeBendInfoJob[{id}] Result.Length :{Result.Length } id:{id} bendData:{Result[id]}");
        if (Result.Length > id)
        {
            //Debug.LogWarning($"PipeBendInfoJob[{id}] SetResult1 Result.Length :{Result.Length } id:{id} bendData:{Result[id]}");
            Result[id] = bendData;
            //Debug.LogWarning($"PipeBendInfoJob[{id}] SetResult2 Result.Length :{Result.Length } id:{id} bendData:{Result[id]}");
        }
        else
        {
            Debug.LogWarning($"PipeBendInfoJob[{id}] Result.Length :{Result.Length }");
        }

        
    }
}


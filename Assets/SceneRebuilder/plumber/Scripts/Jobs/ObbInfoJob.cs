using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static MathGeoLib.OrientedBoundingBox;

//[BurstCompile]
public struct ObbInfoJob : IJob
{
    public int id;
    public NativeArray<Vector3> points;

    //public ObbData obbData;

    public OrientedBoundingBox box;

    public static NativeArray<OrientedBoundingBox> Result;

    public void Execute()
    {
        DateTime startT = DateTime.Now;
        var ps = points.ToArray();
        box = OrientedBoundingBox.GetObb(ps, id,true);

        if (Result.Length > id)
        {
            Result[id] = box;
        }
        else
        {
            Debug.LogWarning($"JobInfo[{id}] Result.Length :{Result.Length }");
        }
    }

    public override string ToString()
    {
        return $"[{id}] points:{points.Length} obb:[{box}]";
    }

    public static ObbInfoJob InitJob(GameObject go,int i)
    {
        Vector3[] vs = OrientedBoundingBox.GetVertices(go);
        //NativeArray<Vector3> vsS = new NativeArray<Vector3>(vs, Allocator.Persistent);
        //ObbInfoJob job = new ObbInfoJob()
        //{
        //    id = i,
        //    points = vsS
        //};
        //return job;
        return InitJob(vs, i);
    }

    public static ObbInfoJob InitJob(Vector3[] vs, int i)
    {
        //var vs = OrientedBoundingBox.GetVertices(go);
        NativeArray<Vector3> vsS = new NativeArray<Vector3>(vs, Allocator.Persistent);
        ObbInfoJob job = new ObbInfoJob()
        {
            id = i,
            points = vsS
        };
        return job;
    }

    public static void GetObbInfoJobs(List<Transform> gos)
    {
        DateTime startT = DateTime.Now;
        JobList<ObbInfoJob> jobs = new JobList<ObbInfoJob>(10);
        NativeArray<OrientedBoundingBox> result = new NativeArray<OrientedBoundingBox>(gos.Count, Allocator.Persistent);
        ObbInfoJob.Result = result;
        for (int i = 0; i < gos.Count; i++)
        {
            Transform go = gos[i];
            ObbInfoJob job = ObbInfoJob.InitJob(go.gameObject, i);
            jobs.Add(job);
        }
        Debug.Log($"GetObbInfoJobs count:{gos.Count} result:{result.Length} time1:{(DateTime.Now - startT).TotalMilliseconds.ToString("F2")}");
        jobs.CompleteAll();

        Debug.Log($"GetObbInfoJobs count:{gos.Count} result:{result.Length} time2:{(DateTime.Now - startT).TotalMilliseconds.ToString("F2")}");
        //for (int i = 0; i < result.Length; i++)
        //{
        //    var r = result[i];
        //    Debug.Log($"Job[{i}] result:{r}");
        //}
        jobs.Dispose();
    }
}

public struct ObbData
{
    public Vector3S center;
    public Vector3S extent;
    public Vector3S right;
    public Vector3S up;
    public Vector3S forward;

    public override string ToString()
    {
        return $"center:{center} extent:{extent} right:{right}";
    }
}

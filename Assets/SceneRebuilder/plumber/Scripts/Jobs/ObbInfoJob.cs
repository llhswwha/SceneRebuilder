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
    public NativeArray<Vector3S> points;

    public ObbData obbData;

    public static NativeArray<ObbData> Result;

    public void Execute()
    {
        DateTime startT = DateTime.Now;
        var axis = new Vector3S[3];
        var ps = points.ToArray();
        MathGeoLibNativeMethods.obb_optimal_enclosing(points.ToArray(), points.Length, out var center, out var extent, axis);

        obbData.center = center;
        obbData.extent = extent;
        obbData.right = axis[0];
        obbData.up = axis[1];
        obbData.forward = axis[2];
        //box = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);

        Debug.Log($"JobInfo {this.ToString()} time:{(DateTime.Now-startT).TotalMilliseconds.ToString("F1")}ms ");

        Result[id]=obbData;
    }

    public override string ToString()
    {
        return $"[{id}] points:{points.Length} obb:[{obbData}]";
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

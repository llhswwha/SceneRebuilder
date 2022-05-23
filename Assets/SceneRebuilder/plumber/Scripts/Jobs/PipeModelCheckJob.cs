using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PipeModelCheckJob : IMeshInfoJob
{
    public int id;

    public NativeArray<Vector3> points1;

    public NativeArray<Vector3> points2;

    public NativeArray<Vector3> connerPoints1;

    public NativeArray<Vector3> connerPoints2;

    public static NativeArray<PipeModelCheckResult> Result;

    public void Dispose()
    {
        points1.Dispose();
        points2.Dispose();
        connerPoints1.Dispose();
        connerPoints2.Dispose();
    }

    public void Execute()
    {
        DateTime start = DateTime.Now;

        PipeModelCheckResult mcResult = new PipeModelCheckResult();
        var vs1 = points1.ToArray();
        var vs2 = points2.ToArray();
        OrientedBoundingBox? obb1 = OrientedBoundingBox.GetObb(vs1, id, false);
        if (obb1 == null)
        {
            return;
        }
        OrientedBoundingBox OBB1 = (OrientedBoundingBox)obb1;

        OrientedBoundingBox? obb2 = OrientedBoundingBox.GetObb(vs2, id, false);
        if (obb2 == null)
        {
            return;
        } 
        OrientedBoundingBox OBB2 = (OrientedBoundingBox)obb2;



        mcResult.MeshDistance = DistanceUtil.GetDistance(vs1, vs2, false);
        mcResult.SizeDistance = DistanceUtil.GetDistance(connerPoints1.ToArray(), connerPoints2.ToArray(), false);
        //mcResult.RTDistance = OBBCollider.GetObbRTDistance(ResultGo, this.gameObject);

        if (OBB1.IsInfinity() == false && OBB2.IsInfinity() == false)
        {
            mcResult.ObbDistance = OBBCollider.GetObbDistance(OBB1, OBB2);
            var obbCornerPoints1 = OBB1.CornerPointsVector3();
            var obbCornerPoints2 = OBB2.CornerPointsVector3();
            //MeshHelper.GetVertexDistanceEx(vs1, vs2,)
            mcResult.RTDistance = MeshHelper.GetRTDistance(obbCornerPoints1, obbCornerPoints2);
        }
        else
        {
            mcResult.ObbDistance = 22;
            mcResult.RTDistance = 33;
        }

        if (Result.Length > id)
        {
            Result[id] = mcResult;
        }
        else
        {
            Debug.LogWarning($"PipeModelCheckJob[{id}] Result.Length :{Result.Length }");
        }

        Debug.Log($"PipeModelCheckJob[{id}] time:{(DateTime.Now - start).TotalMilliseconds.ToString("F1")}ms points1:{points1.Length} points2:{points2.Length} mcResult:{mcResult}");

    }

}

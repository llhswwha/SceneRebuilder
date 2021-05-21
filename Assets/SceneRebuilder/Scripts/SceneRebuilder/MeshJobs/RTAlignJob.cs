using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;

namespace MeshJobs
{

  //没用,AcRTAlignJob代替，可以把核心换一下，暂时不用，
  public struct RTAlignJob : IJob
  {
    public int Id;

    public int RId;

    [ReadOnly] public RTTransform rt;

    [ReadOnly] public NativeArray<Vector3> vertices1;//世界坐标，不变的

    [ReadOnly] public NativeArray<Vector3> vertices2;//也是世界坐标，因为RTTransform是世界坐标之间的变换

    public float Distance;

    public void Execute()
  {
      
      //DateTime start = DateTime.Now;

      Vector3[] vertices2World=GetWorldVertexes(vertices2);

    Distance=DistanceUtil.GetDistance(vertices1.ToArray(),vertices2World);

    MeshDistanceJobHelper.SetResult(RId,Id,Distance);
    Debug.Log(string.Format("RTAlignJob[{0}][{1}] Distance:{2}",Id,RId,Distance));
  }

 public Vector3[] GetWorldVertexes(NativeArray<Vector3> vs){
        var vCount=vs.Length;
        Vector3[] points1 = new Vector3[vCount];
        // var vs=mesh1.vertices;
        for (int i = 0; i < vCount; i++)
        {
            Vector3 p1 = vs[i];
            Vector3 p11 = rt.Apply(p1);
            //points1.Add(p11);
            points1[i]=p11;
        }
        return points1;
    }
  }

  public static class RTAlignJobsHelper
  {
    
  }
}
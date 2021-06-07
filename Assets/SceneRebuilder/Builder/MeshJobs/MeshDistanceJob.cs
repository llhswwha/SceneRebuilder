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
public class MeshDistanceJobResult
{
    public float min=float.MaxValue;

    public int minId=-1;

    public bool IsZero=false;

    public GameObject Instance;
}

public static class MeshDistanceJobHelper
{

    public const float zero=1E-08f;

    public static int ID=0;

    public static Dictionary<int,MeshDistanceJobResult> results=new Dictionary<int, MeshDistanceJobResult>();

    public static int InitResult(){
        MeshDistanceJobResult result=new MeshDistanceJobResult();
        int id=ID++;
        results.Add(id,result);
        return id;
    }

    public static void SetResult(int rId,int id,float dis)
    {
        MeshDistanceJobResult result=results[rId];
        if(dis<result.min){
            result.min=dis;
            result.minId=id;

            if(dis<=zero){
                result.IsZero=true;
            }
        }
        
    }
}

[BurstCompile]
public struct MeshDistanceJob : IJob
{
    // [ReadOnly] public int vertexCount1;

    public int Id;

    public int RId;

    // [ReadOnly] public Matrix4x4 Matrix1;
    [ReadOnly] public NativeArray<Vector3> vertices1;//世界坐标

    // [ReadOnly] public int vertexCount2;
    // public NativeArray<float3> vertices;

    [ReadOnly] public Matrix4x4 Matrix2;//坐标变换（到世界坐标）

    [ReadOnly] public NativeArray<Vector3> vertices2;//本地坐标


    public float Distance;


  public void Execute()
  {
    //DateTime start = DateTime.Now;

    // Vector3[] vertices2World=GetWorldVertexes(vertices2.ToArray(),Matrix2);

    // Distance=DistanceUtil.GetDistance(vertices1.ToArray(),vertices2World);

    // MeshDistanceJobHelper.SetResult(RId,Id,Distance);

    // Debug.Log(string.Format("MeshDistanceJob[{0}][{1}] Distance:{2}",Id,RId,Distance));
  }


//  public static Vector3[] GetWorldVertexes(Vector3[] vs, Matrix4x4 matrix){
//         var vCount=vs.Length;
//         Vector3[] points1 = new Vector3[vCount];
//         // var vs=mesh1.vertices;
//         for (int i = 0; i < vCount; i++)
//         {
//             Vector3 p1 = vs[i];
//             Vector3 p11 = matrix.MultiplyPoint3x4(p1);
//             //points1.Add(p11);
//             points1[i]=p11;
//         }
//         return points1;
//     }

//   public void Init

//   public void InitVertices1(Mesh mesh1,Matrix4x4 max1)
//   {
//     // vertexCount1=mesh1.vertexCount;
//     vertices1=new NativeArray<Vector3>(mesh1.vertices,Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
//     Matrix1=max1;
//   }

//   public void InitVertices2(Mesh mesh1,Matrix4x4 max1)
//   {
//     // vertexCount2=mesh1.vertexCount;
//     vertices2=new NativeArray<Vector3>(mesh1.vertices,Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
//     Matrix2=max1;
//   }
}
}
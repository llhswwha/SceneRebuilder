using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct BoxMeshInfoJob : IMeshInfoJob
{
    public int id;
    public MeshStructure mesh;

    public static NativeArray<MeshBoxData> Result;

    public static void InitResult(int count)
    {
        Result = new NativeArray<MeshBoxData>(count, Allocator.Persistent);
    }

    public static void DisposeResult()
    {
        Result.Dispose();
    }

    public static BoxMeshInfoJob GetOneJob(int i, MeshStructure m)
    {
        BoxMeshInfoJob job = new BoxMeshInfoJob(i, m);
        return job;
    }

    public static BoxMeshInfoJob GetOneJob(int i, GameObject go)
    {
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        MeshStructure meshS = new MeshStructure(mesh);
        BoxMeshInfoJob job = new BoxMeshInfoJob(i, meshS);
        return job;
    }

   

    public BoxMeshInfoJob(int i ,MeshStructure m)
    {
        this.id = i;
        this.mesh = m;
        this.data = new MeshBoxData();
    }

    public MeshBoxData data;
    public void Execute()
    {

    }

    public void Dispose()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct MeshStructure
{
    public int vertexCount;
    //
    // ժҪ:
    //     Returns a copy of the vertex positions or assigns a new vertex positions array.
    public NativeArray<Vector3> vertices { get; set; }
    //
    // ժҪ:
    //     The normals of the Mesh.
    public NativeArray<Vector3> normals { get; set; }

    //
    // ժҪ:
    //     An array containing all triangles in the Mesh.
    public NativeArray<int> triangles { get; set; }

    public Vector3 boundCenter;

    public bool isDisposed;

    public int VertexCount
    {
        get
        {
            return vertices.Length;
        }
    }

    public int TriangleCount
    {
        get
        {
            return triangles.Length;
        }
    }

    public MeshStructure(Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError(" MeshStructure mesh == null");
            this.vertices = new NativeArray<Vector3>(0, Allocator.Persistent);
            this.normals = new NativeArray<Vector3>(0, Allocator.Persistent);
            this.triangles = new NativeArray<int>(0, Allocator.Persistent);
            this.vertexCount = 0;
            this.boundCenter = Vector3.zero;
            isDisposed = false;
        }
        else
        {
            this.vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
            this.normals = new NativeArray<Vector3>(mesh.normals, Allocator.Persistent);
            this.triangles = new NativeArray<int>(mesh.triangles, Allocator.Persistent);
            this.vertexCount = mesh.vertexCount;
            this.boundCenter = mesh.bounds.center;
            isDisposed = false;
        }
        
    }

    public MeshStructure(MeshTriangleList list)
    {
        int count = list.Count;
        this.vertices = new NativeArray<Vector3>(count*3, Allocator.Persistent);
        this.normals = new NativeArray<Vector3>(count * 3, Allocator.Persistent);
        this.triangles = new NativeArray<int>(count, Allocator.Persistent);
        this.vertexCount = count * 3;
        this.boundCenter = list.GetCenter();
        isDisposed = false;
    }

    public void Dispose()
    {
        if (isDisposed) return;
        isDisposed = true;
        try
        {
            vertices.Dispose();
            normals.Dispose();
            triangles.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"MeshStructure.Dispose Exception:{ex}");
        }
        
    }

    public void SetVertices(NativeArray<Vector3> inVertices)
    {
        this.vertices=inVertices;
    }

    public void SetVertices(List<Vector3> inVertices)
    {
        this.vertices=new NativeArray<Vector3>(inVertices.ToArray(),Allocator.Persistent);
    }

    public void SetTriangles(List<int> triangles, int submesh)
    {
        this.triangles=new NativeArray<int>(triangles.ToArray(),Allocator.Persistent);
    }

    public void SetNormals(List<Vector3> inNormals)
    {
        this.normals=new NativeArray<Vector3>(inNormals.ToArray(),Allocator.Persistent);
    }
}

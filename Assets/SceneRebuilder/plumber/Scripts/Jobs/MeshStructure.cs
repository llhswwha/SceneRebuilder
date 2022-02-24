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

    public void Dispose()
    {
        if (isDisposed) return;
        isDisposed = true;
        vertices.Dispose();
        normals.Dispose();
        triangles.Dispose();
    }
}

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

    public MeshStructure(Mesh mesh)
    {
        this.vertices = new NativeArray<Vector3>( mesh.vertices,Allocator.Persistent);
        this.normals = new NativeArray<Vector3>(mesh.normals, Allocator.Persistent);
        this.triangles = new NativeArray<int>(mesh.triangles, Allocator.Persistent);
        this.vertexCount = mesh.vertexCount;
        this.boundCenter = mesh.bounds.center;
    }

    public void Dispose()
    {
        vertices.Dispose();
        normals.Dispose();
        triangles.Dispose();
    }
}

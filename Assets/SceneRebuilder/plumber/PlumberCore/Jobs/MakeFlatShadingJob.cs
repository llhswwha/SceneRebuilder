using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

public struct MakeFlatShadingJob : IJob
{
    public MeshStructure mesh;

    public void Execute()
    {
        //Unity.Collections
        DateTime startT=DateTime.Now;
        // in order to achieve flat shading all vertices need to be
        // duplicated, because in Unity normals are assigned to vertices
        // and not to triangles.
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        //NativeList

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            // for each face we need to clone vertices and assign normals
            int vertIdx1 = mesh.triangles[i];
            int vertIdx2 = mesh.triangles[i + 1];
            int vertIdx3 = mesh.triangles[i + 2];

            newVertices.Add(mesh.vertices[vertIdx1]);
            newVertices.Add(mesh.vertices[vertIdx2]);
            newVertices.Add(mesh.vertices[vertIdx3]);

            newTriangles.Add(newVertices.Count - 3);
            newTriangles.Add(newVertices.Count - 2);
            newTriangles.Add(newVertices.Count - 1);

            Vector3 normal = Vector3.Cross(
                mesh.vertices[vertIdx2] - mesh.vertices[vertIdx1],
                mesh.vertices[vertIdx3] - mesh.vertices[vertIdx1]
            ).normalized;
            newNormals.Add(normal);
            newNormals.Add(normal);
            newNormals.Add(normal);
        }

        Debug.Log($"MakeFlatShadingJob count:{mesh.triangles.Length} t1:{(DateTime.Now-startT).TotalMilliseconds}:ms ");

        mesh.SetVertices(newVertices);
        mesh.SetTriangles(newTriangles, 0);
        mesh.SetNormals(newNormals);
        Debug.Log($"MakeFlatShadingJob t2:{(DateTime.Now-startT).TotalMilliseconds}:ms ");
    }
}

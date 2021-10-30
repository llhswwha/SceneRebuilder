using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModel : MonoBehaviour
{
    public Vector3 startP;

    public Vector3 endP;

    public float radius;

    [ContextMenu("PipeModel")]
    public void GetInfo()
    {

    }

    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector4[] tangents;
    public int[] triangles;

    public List<Vector3> sharedNormals;

    [ContextMenu("ShowVertex")]
    public void ShowVertex()
    {
        //Dictionary<Vector3,List<Vector3>> 
        MeshFilter mf = this.GetComponent<MeshFilter>();
        vertices = mf.sharedMesh.vertices;
        normals = mf.sharedMesh.normals;
        tangents = mf.sharedMesh.tangents;
        triangles= mf.sharedMesh.triangles;

        sharedNormals = new List<Vector3>();
        ClearChildren();
        for (int i=0;i<mf.sharedMesh.vertices.Length;i++)
        {
            var v = mf.sharedMesh.vertices[i];
            VertexHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
            var m = mf.sharedMesh.normals[i];
            if(!sharedNormals.Contains(m))
            {
                sharedNormals.Add(m);
            }
        }
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach(var child in children)
        {
            if (child.gameObject == this.gameObject) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}

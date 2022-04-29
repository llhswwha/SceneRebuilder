using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CircleMeshData 
{
    public CircleMeshData()
    {

    }

    public CircleMeshData(Vector3 center, Vector3 direction, List<Vector3> vertices, string name)
    {
        this.Center = center;
        this.Direction = direction;
        this.Vertices = vertices;
        this.name = name;
    }

    public string name;

    public Vector3 Center;

    public Vector3 Direction;

    public Vector3 xAxis = Vector3.up;
    public Vector3 yAxis = Vector3.right;

    public void SetAxis(Vector3 xa,Vector3 ya)
    {
        xAxis = xa;
        yAxis = ya;
    }

    public List<Vector3> Vertices = new List<Vector3>();

    public List<Vector3> GetPoints()
    {
        List < Vector3 > ps= new List<Vector3>();
        ps.AddRange(Vertices);
        ps.Add(Center);
        ps.Add(Direction);
        ps.Add(xAxis);
        ps.Add(yAxis);
        return ps;
    }

    internal Vector3 GetClosedPoint(Vector3 currentVertex)
    {
        return MeshHelper.GetClosedPoint(currentVertex, Vertices);
    }
}

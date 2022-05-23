using CommonUtils;
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
        return VertexHelper.GetClosedPoint(currentVertex, Vertices);
    }

    public List<Vector3> Vertices2 = new List<Vector3>();

    public Vector3 GetClosedPointEx(Vector3 currentVertex)
    {
        if (Vertices2.Count == 0)
        {
            Vertices2 = new List<Vector3>(Vertices);
        }
        Vector3 p= VertexHelper.GetClosedPoint(currentVertex, Vertices2);
        Vertices2.Remove(p);
        return p;
    }

    public int GetIndex(Vector3 p)
    {
        return Vertices.IndexOf(p);
    }

    public Vector3 GetPoint(int id)
    {
        return Vertices[id];
    }

    public void RemoveByIndex(int index)
    {
        Vertices.RemoveAt(index);
    }

    public void RemovePoint(Vector3 p)
    {
        Vertices.Remove(p);
    }
}

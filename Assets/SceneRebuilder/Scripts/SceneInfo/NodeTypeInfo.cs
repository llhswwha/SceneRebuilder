using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTypeInfo :IComparable<NodeTypeInfo>
{
    public List<NodeInfo> nodes = new List<NodeInfo>();
    public string typeName;

    public string GetTypeName(bool isTest)
    {
        if (isTest)
        {
            return typeName + "(test)";
        }
        else
        {
            return typeName;
        }
    }

    public string prefabName;

    public string lodName;

    public int Count = 0;

    private int vertexCount;
    private long totalVertexCount;
    private float percent;


    public int tris;
    private long totaltris;
    public long GetTotalTriCount()
    {
        return totaltris;
    }
    internal bool isLod;

    public void SetVertexCount(GameObject target)
    {
        tris = 0;
        vertexCount = 0;
        if (target)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter f in meshFilters)
            {
                tris += f.sharedMesh.triangles.Length / 3;
                vertexCount += f.sharedMesh.vertexCount;
            }
        }
        totalVertexCount = vertexCount * Count;
        totaltris = tris * Count;

        //string text = string.Format("tris:{0}w,verts:{1}w", tris / 10000, verts / 10000);
        //Debug.Log("GetMeshVertsCount End:" + verts);
    }

    public long GetTotalVertCount()
    {
        return totalVertexCount;
    }

    public override string ToString()
    {
        //return typeName + " : " + Count;
        return string.Format("\t[{0} * {1}= {2:F1}w]\t[{3:F1}]\t[{4}]\t[{5}]", Count, vertexCount, totalVertexCount/10000f, percent,isLod,typeName);
    }

    public string GetInfo()
    {
        //return typeName + " : " + Count;
        return string.Format("[{0} * {1}= {2:F1}w][{3:F1}][{4}][{5}]", Count, vertexCount, totalVertexCount / 10000f, percent, isLod, typeName);
    }

    public void Add(NodeInfo node)
    {
        nodes.Add(node);
        Count++;
    }

    public int CompareTo(NodeTypeInfo other)
    {
        int r = 0;
        if (percent != 0 || other.percent!=0)
        {
            r = other.percent.CompareTo(this.percent);
        }
        else if(totalVertexCount != 0 || other.percent != 0)
        {
            r = other.totalVertexCount.CompareTo(this.totalVertexCount);
        }
        else
        {
            r = other.Count.CompareTo(this.Count);
        }
        if (r ==0)
        {
            r = this.typeName.CompareTo(other.typeName);
        }
        return r;
    }

    public Vector3 GetCenter()
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        foreach (var node in nodes)
        {
            var pos = node.GetPos();
            if (pos.x < minX)
            {
                minX = pos.x;
            }
            if (pos.y < minY)
            {
                minY = pos.y;
            }
            if (pos.z < minZ)
            {
                minZ = pos.z;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
            }
            if (pos.y > maxY)
            {
                maxY = pos.y;
            }
            if (pos.z > maxZ)
            {
                maxZ = pos.z;
            }
        }

        float centerX = (minX + maxX) / 2.0f;
        float centerY = (minY + maxY) / 2.0f;
        float centerZ = (minZ + maxZ) / 2.0f;
        return new Vector3(centerX, centerY, centerZ);
    }

        public NodeTypeInfo(string name)
    {
        typeName = name;
    }



    //internal void SetVertexCount(int vertexCount)
    //{
    //    this.vertexCount = vertexCount;
    //    totalVertexCount = vertexCount * Count;
    //}

    internal void SetPercent(long total)
    {
        if (total == 0) return;
        this.percent = (this.totalVertexCount+0.0f) / total *100;
    }
}

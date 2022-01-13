using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshData 
{
    public Mesh mesh;

    public int triangleCount = 0;

    public int vertexCount = 0;

    public int vertexBufferCount = 0;

    public int vertexAttributeCount = 0;

    public int subMeshCount = 0;

    //public Vector3[] vertices;

    //public Vector3[] normals;

    //public Vector4[] tangents;

    //public Vector2[] uv;

    //public Color[] colors;

    //public Color32[] colors32;

    //public Vector2[] uv2;

    //public Vector2[] uv3;

    //private MeshInfo Info = new MeshInfo();

    //public MeshInfo GetInfo()
    //{
    //    return Info;
    //}

    public GameObject _obj;

    public override string ToString()
    {
        if (_obj == null) return base.ToString();
        return "MeshData:" + _obj.name;
    }

    public string name;

    public Vector3 scale;

    public Vector3 size;

    public float scaleX=1;

    public Transform t;

    public MeshData(GameObject obj)
    {
        //Debug.Log($"MeshData.ctor obj:{obj}");
        _obj = obj;
        this.name=obj.name;
        t=obj.transform;
        scale=t.lossyScale;
        scaleX=scale.x;
        MeshFilter msFilter = obj.GetComponent<MeshFilter>();
        try
        {
           
            if (msFilter != null)
            {
                Mesh ms = msFilter.sharedMesh;
                if (ms == null)
                {
                    Debug.LogError("MeshData.cotr, ms == null," + msFilter);
                    return;
                }
                this.mesh = ms;
                this.size=ms.bounds.size;
                triangleCount = ms.triangles.Length;
                vertexCount = ms.vertexCount;
                vertexBufferCount = ms.vertexBufferCount;
                vertexAttributeCount = ms.vertexAttributeCount;
                subMeshCount = ms.subMeshCount;

                //uv = ms.uv;
                //uv2 = ms.uv2;
                //uv3 = ms.uv3;

                //vertices = ms.vertices;
                //normals = ms.normals;
                //tangents = ms.tangents;
                //colors = ms.colors;
                //colors32 = ms.colors32;

                //Info.m_Mesh = ms;
                //Info.Update();
            }
            else{
                //Debug.Log($"MeshData.ctor msFilter==null obj:{obj}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("MeshData.ctor,"+ msFilter + ","+ex);
        }
    }

    public MeshData()
    {

    }

    public void Add(MeshData info)
    {
        if (info != null)
        {
            Mesh ms = info.mesh;
            if (ms != null)
            {
                triangleCount += ms.triangles.Length;
                vertexCount += ms.vertexCount;
                vertexBufferCount += ms.vertexBufferCount;
                vertexAttributeCount += ms.vertexAttributeCount;
            }
            else
            {
                //Debug.LogWarning("MeshData.Add ms==null:"+info);

                triangleCount += info.triangleCount;
                vertexCount += info.vertexCount;
                vertexBufferCount += info.vertexBufferCount;
                vertexAttributeCount += info.vertexAttributeCount;
            }
        }
        else
        {
            Debug.LogWarning("MeshData.Add info==null");
        }
        

    }

    internal bool IsSameMesh(MeshData info)
    {
        //if (this.Info != null && info.Info != null)
        //{
        //    return this.Info.m_meshFeature == info.Info.m_meshFeature;
        //}
        //else
        {
            bool r1 = triangleCount == info.triangleCount && vertexCount == info.vertexCount && vertexBufferCount == info.vertexBufferCount && vertexAttributeCount == info.vertexAttributeCount;
            return r1;
        }

    }

    public Vector3 GetCenterP()
    {
        if(IsWorld)return center;
        return TransformPoint(center);
    }

    public Vector3 TransformPoint(Vector3 p)
    {
        if (t == null) t = _obj.transform;
        return t.TransformPoint(p);
    }

    public Vector3 GetLongLine()
    {
        if(IsWorld)return maxPList[maxPId]-center;
        if(t==null) t=_obj.transform;
        return t.TransformPoint(maxPList[maxPId]) - t.TransformPoint(center);
    }

    public Vector3 GetShortLine()
    {
        if(IsWorld)return minPList[minPId]-center;
        if(t==null) t=_obj.transform;
        return t.TransformPoint(minPList[minPId]) - t.TransformPoint(center);
    }

    public Vector3 GetLongShortNormal()
    {
        var longLine=this.GetLongLine();

        longLine=Vector3.Normalize(longLine);

        var shortLine=this.GetShortLine();
        shortLine=Vector3.Normalize(shortLine);

        Vector3 nor= Vector3.Cross(longLine,shortLine);
        //float angle1=Vector3.Angle(longLine,nor);
        //float angle2=Vector3.Angle(shortLine,nor);
        //Debug.Log($"]{this.name}]GetLongShortNormal longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");
        return nor;
    }

    public float GetLongShortAngle()
    {
        return Vector3.Angle(this.GetLongLine(), this.GetShortLine());
    }

    public List<Vector3> maxPList=new List<Vector3>();

    public int maxPId=0;

    public int minPId=0;

    public Vector3 GetMaxP(int i)
    {
        if (maxPList.Count == 0)
        {
            Debug.LogError($"MeshData.GetMaxP maxPList.Count == 0 i:{i} Count:{maxPList.Count}");
            return Vector3.zero;
        }
        if(i<0||i>maxPList.Count)
        {
            Debug.LogError($"MeshData.GetMaxP i<0||i>maxPList.Count i:{i} Count:{maxPList.Count}");
            return Vector3.zero;
        }
        var p=maxPList[i];
        if(IsWorld)return p;
        return TransformPoint(p);
    }

    public Vector3 GetMinP(int i)
    {
        if (maxPList.Count == 0)
        {
            Debug.LogError($"MeshData.GetMinP maxPList.Count == 0 i:{i} Count:{maxPList.Count}");
            return Vector3.zero;
        }
        if (i < 0 || i > maxPList.Count)
        {
            Debug.LogError($"MeshData.GetMinP i<0||i>maxPList.Count i:{i} Count:{maxPList.Count}");
            return Vector3.zero;
        }
        var p=minPList[i];
        if(IsWorld)return p;
        return TransformPoint(p);
    }

    public Vector3 GetMaxP()
    {
        return GetMaxP(maxPId);
    }

    public Vector3 GetMinP()
    {
        return GetMinP(minPId);
    }

    public List<Vector3> minPList=new List<Vector3>();

    public List<Vector3> ps=new List<Vector3>();//正式要注释掉，或者private

    public bool IsWorld=true;

    public static int InvokeGetVertexCenterInfoCount=0;

    public void GetVertexCenterInfo(bool isForce,Vector3 centerOffset,bool isWorld)
    {
        this.IsWorld=isWorld;
        if(maxDis!=0 && isForce==false){
            Debug.Log("maxDis!=0 && isForce==false");
            return;
        }
        
        DateTime start=DateTime.Now;

        Vector3 sumP = Vector3.zero;

        this.vertexCount = this.mesh.vertices.Length;

        ps=new List<Vector3>();

        if(t==null) t=_obj.transform;
        var vCount=this.vertexCount;
        var vs=this.mesh.vertices;
        for (int i = 0; i < vCount; i++)
        {
            Vector3 p = vs[i];
            // if(scaleX!=1)
            // {
            //     p=new Vector3(p.x*scale.x,p.y*scale.y,p.z*scale.z);
            // }
            if(IsWorld)
            {
                 p=t.TransformPoint(p);
            }
            ps.Add(p);
            sumP += p;
        }

        //center = sumP / this.vertexCount;//偏移一下避免对称模型无法得到唯一的特征点
        center = sumP / this.vertexCount+centerOffset;//偏移一下避免对称模型无法得到唯一的特征点

        Vector3 maxP = Vector3.zero;
        Vector3 minP = Vector3.zero;
        maxDis = 0;
        minDis = float.MaxValue;
        for (int i = 0; i < ps.Count; i++)
        {
            Vector3 p = ps[i];
            // if(scaleX!=1)
            // {
            //     p=new Vector3(p.x*scale.x,p.y*scale.y,p.z*scale.z);
            // }
            var dis = Vector3.Distance(p, center);
            if (dis == maxDis)
            {
                if(p==maxP)continue;//重复点
                if (!maxPList.Contains(p))
                {
                    maxPList.Add(p);
                }
            }
            if (dis > maxDis)
            {
                maxPList.Clear();
                maxPList.Add(p);
                maxDis = dis;
                maxP = p;
            }

            if (dis == minDis)
            {
                if(p==minP)continue;//重复点
                //minPList.Add(p);
                if (!minPList.Contains(p))
                {
                    minPList.Add(p);
                }
            }
            if (dis < minDis)
            {
                minPList.Clear();
                minPList.Add(p);
                minDis = dis;
                minP = p;
            }
        }

        InvokeGetVertexCenterInfoCount++;

        //Debug.LogWarning($"GetVertexCenterInfo vertexCount:{this.vertexCount},time:{(DateTime.Now-start).TotalMilliseconds}ms,center:{center},maxP:{maxP},minP:{minP},maxDis:{maxDis},minDis:{minDis},{maxPList.Count},{minPList.Count};centerOffset:{centerOffset}||[{name}]");
        if(maxPList.Count>1||minPList.Count>1){
            Debug.LogError($"模型中心可能是对称的，存在多个最远点和最近点 {maxPList.Count} * {minPList.Count} = {maxPList.Count * minPList.Count} maxDis:{maxDis} minDis:{minDis}");
        }
    }

    public List<MinMaxId> allMinMax = new List<MinMaxId>();

    public List<MinMaxId> GetAllMinMaxIds()
    {
        var meshData = this;

        List<MinMaxId> ids = new List<MinMaxId>();
        for (int i = 0; i < meshData.maxPList.Count; i++)
        {
            for (int j = 0; j < meshData.minPList.Count; j++)
            {
                ids.Add(new MinMaxId(i, j));
            }
        }
        allMinMax = ids;
        return ids;
    }

    public void SetMinMaxId(MinMaxId id)
    {
        // if(id==null){
        //     Debug.LogError("SetMinMaxId id==null");
        //     minPId=0;
        //     maxPId=0;
        //     return;
        // }
        minPId=id.min;
        maxPId=id.max;
    }
    
    public Vector3 center;

    // private Vector3 maxP = Vector3.zero;
    // private Vector3 minP = Vector3.zero;
    public float maxDis = 0;
    public float minDis = float.MaxValue;
}

[Serializable]
public struct MinMaxId
{
    public int max;
    public int min;

    public MinMaxId(int max, int min)
    {
        this.max = max;
        this.min = min;
    }
}

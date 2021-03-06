using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PipeLineInfo
{
    //public Vector4 StartPoint = Vector3.zero;
    //public Vector4 EndPoint = Vector3.zero;

    public Vector4 StartPoint;
    public Vector3 StartNormal;

    public Vector4 EndPoint;
    public Vector3 EndNormal;

    public Vector3 Direction;

    public override string ToString()
    {
        return $"[{((Vector3)StartPoint).Vector3ToString3()},{StartNormal.Vector3ToString3()} - {((Vector3)EndPoint).Vector3ToString3()}],{EndNormal.Vector3ToString3()}"; 
    }

    public Vector3 GetNotClosedPoint(Vector3 p)
    {
        float p1 = Vector3.Distance(p, StartPoint);
        float p2 = Vector3.Distance(p, EndPoint);
        if (p1 > p2)
        {
            return StartPoint;
        }
        else
        {
            return EndPoint;
        }
    }

    public MeshPoint GetNotClosedMeshPoint(Vector3 p)
    {
        float p1 = Vector3.Distance(p, StartPoint);
        float p2 = Vector3.Distance(p, EndPoint);
        if (p1 > p2)
        {
            return new MeshPoint(StartPoint,StartNormal);
        }
        else
        {
            return new MeshPoint(EndPoint,EndNormal);
        }
    }

    public Vector3[] GetNotClosedPoints(Vector3 p)
    {
        float p1 = Vector3.Distance(p, StartPoint);
        float p2 = Vector3.Distance(p, EndPoint);
        if (p1 > p2)
        {
            return new Vector3[2] { StartPoint,EndPoint };
        }
        else
        {
            return new Vector3[2] { EndPoint, StartPoint };
        }
    }

    //public PipeLineData=
    [XmlIgnore]
    public Transform transform;

    public PipeLineInfo()
    {

    }

    public PipeLineInfo(PipeLineData data, Transform t)
    {
        this.StartPoint = data.StartPoint;
        this.EndPoint = data.EndPoint;
        this.StartNormal = data.StartNormal;
        this.EndNormal = data.EndNormal;
        this.transform = t;
        //Direction = StartPoint - EndPoint;
        Direction = data.Direction; 
    }

    public PipeLineInfo(Vector3 data, Transform t)
    {
        this.StartPoint = data;
        this.EndPoint = data;
        this.StartNormal = data;
        this.EndNormal = data;
        this.transform = t;
        //Direction = StartPoint - EndPoint;
        Direction = data;
    }

    public PipeLineInfo(Vector4 p1,Vector4 p2,Transform t=null)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
        Direction = StartPoint - EndPoint;
    }

    public PipeLineInfo(Vector4 p1, Vector4 p2,Vector3 n1,Vector3 n2, Transform t = null)
    {
        Init(p1, p2, n1, n2, t);
    }

    public void Init(Vector4 p1, Vector4 p2, Vector3 n1, Vector3 n2, Transform t = null)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
        this.StartNormal = n1;
        this.EndNormal = n2;

        Direction = StartPoint - EndPoint;
    }

    public PipeLineInfo(List<Vector3> line1,List<Vector3> line2)
    {
        CircleInfo circle1 = new CircleInfo(line1);
        CircleInfo circle2 = new CircleInfo(line2);
        InitByCircle(circle1, circle2);
    }

    public PipeLineInfo(List<List<Vector3>> lines)
    {
        if (lines.Count != 2)
        {
            Debug.LogError($"PipeLineInfo lines.Count != 2 lines:{lines.Count}");
        }
        CircleInfo circle1 = new CircleInfo(lines[0]);
        CircleInfo circle2 = new CircleInfo(lines[1]);
        InitByCircle(circle1, circle2);
    }

    public PipeLineInfo(CircleInfo startCircle, CircleInfo endCircle)
    {
        InitByCircle(startCircle, endCircle);
    }

    public void InitByCircle(CircleInfo startCircle, CircleInfo endCircle)
    {
        Init(startCircle.GetCenter4(), endCircle.GetCenter4(), startCircle.GetNormal(), endCircle.GetNormal(), null);
    }

    public PipeLineInfo(Vector4 p1, Vector4 p2, Transform t, Vector3 direction)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.transform = t;
        this.Direction = direction;
    }

    public Vector4 GetStartPoint()
    {
        //return StartPoint + this.transform.position;
        //return StartPoint;
        if (this.transform == null) return StartPoint;
        Vector4 p = this.transform.TransformPoint(StartPoint);
        p.w = StartPoint.w;
        return p;
    }

    public Vector4 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        if (this.transform == null) return EndPoint;
        Vector4 p= this.transform.TransformPoint(EndPoint);
        p.w = EndPoint.w;
        return p;
    }
}

[Serializable]
public class PipeLineInfoList : List<PipeLineInfo>
{
    public PipeLineInfoList()
    {

    }

    public PipeLineInfoList(PipeBendData data)
    {
        //foreach (var line in data.Lines)
        //{
        //    PipeLineInfo info = new PipeLineInfo(line, null);
        //    this.Add(info);
        //}

        for (int i = 0; i < data.Count; i++)
        {
            PipeLineData lineData = data.Get(i);
            PipeLineInfo info = new PipeLineInfo(lineData, null);
            this.Add(info);
        }
    }

    public PipeLineInfo GetLine1()
    {
        if (Count == 0) return null;
        return this[0];
    }

    public Vector3 GetCenter()
    {
        List<Vector3> ps = new List<Vector3>();
        for (int i = 0; i < this.Count; i++)
        {
            var item = this[i];
            if (!ps.Contains(item.StartPoint))
            {
                ps.Add(item.StartPoint);
            }
            if (!ps.Contains(item.EndPoint))
            {
                ps.Add(item.EndPoint);
            }
        }

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < ps.Count; i++)
        {
            Vector3 p = ps[i];
            sum += p;
            //Debug.Log($"GetCenter[{i}] p:{p.Vector3ToString()} sum:{sum.Vector3ToString()}");
        }
        Vector3 center = sum / ps.Count;
        //Debug.Log($"GetCenter center:{center.Vector3ToString()} Count:{this.Count} ps:{ps.Count}");
        return center;
    }

    public List<Vector3> GetLinePoints(float startEndDis)
    {
        List<Vector3> ps = new List<Vector3>();
        Vector3 center = GetCenter();
        Vector3 lastP = center;
        ////MeshHelper.CreatePoint(lastP, "LineCenter", null, 0.1f);
        for (int i = 0; i < this.Count; i++)
        {
            var item = this[i];
            if (i == 0)
            {
                Vector3[] ps0 = item.GetNotClosedPoints(lastP);
                ps.AddRange(ps0);
                lastP = ps.Last();
            }
            else
            {
                //ps.Add(item.EndPoint);

                Vector3 p0 = item.GetNotClosedPoint(lastP);
                ps.Add(p0);
                lastP = p0;
            }
        }

        if(startEndDis>0)
        {
            MeshPoint mp = this[0].GetNotClosedMeshPoint(center);
            Vector3 p0 = ps[0];
            Vector3 p1 = ps[1];
            Vector3 dir01 = p1 - p0;

            Vector3 normal00 = Vector3.zero;
            Vector3 normal01 = mp.Normal.normalized;
            Vector3 normal02 = -normal01;
            float dot1 = Vector3.Dot(normal01, dir01);
            float dot2 = Vector3.Dot(normal02, dir01);
            if (dot1 > 0)
            {
                normal00 = normal01;
            }
            else
            {
                normal00 = normal02;
            }

            float dis = dir01.magnitude;

                  
            Vector3 p01 = mp.Point + normal00 * dis * startEndDis;
            ps.Insert(1, p01);

            //Vector3 p00 = mp.Point - normal00 * dis * startEndDis * 1f;
            //ps[0] = p00; 
        }

        if (startEndDis > 0)
        {
            MeshPoint mp = this[Count - 1].GetNotClosedMeshPoint(center);
            Vector3 p0 = ps[ps.Count - 1];
            Vector3 p1 = ps[ps.Count - 2];
            Vector3 dir01 = p1 - p0;

            Vector3 normal00 = Vector3.zero;
            Vector3 normal01 = mp.Normal.normalized;
            Vector3 normal02 = -normal01;
            float dot1 = Vector3.Dot(normal01, dir01);
            float dot2 = Vector3.Dot(normal02, dir01);
            if (dot1 > 0)
            {
                normal00 = normal01;
            }
            else
            {
                normal00 = normal02;
            }

            float dis = dir01.magnitude;

            Vector3 p00 = mp.Point + normal00 * dis * startEndDis;
            ps.Insert(ps.Count - 1, p00);
        }

        return ps;
    }

    public PipeLineInfo GetLine2()
    {
        if (Count < 2) return null;
        return this[Count - 1];
    }

    public float GetRadius()
    {
        float radiusSum = 0;
        foreach(PipeLineInfo item in this)
        {
            radiusSum += item.StartPoint.w;
            radiusSum += item.EndPoint.w;
        }
        float radius = radiusSum / (Count*2);
        return radius;
    }

    public float GetElbowRadius()
    {
        PipeCreateArg pipeArg = new PipeCreateArg(this[0], this[1]);
        return pipeArg.GetElbowRadius();
    }

    public override string ToString()
    {
        string s = $"[{Count}]";
        for (int i = 0; i < Count; i++)
        {
            //s += this[i].ToString() + ";";
            s += $"{this[i]};";
        }
        return s;
    }
}


public struct PipeLineData
{
    public int Length
    {
        get
        {
            return 0;
        }
    }

    public Vector4 StartPoint;
    public Vector3 StartNormal;

    public Vector4 EndPoint;
    public Vector3 EndNormal;
      
    public Vector3 Direction;


    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public bool IsObbError;

    //public override string ToString()
    //{
    //    //return $"[{StartPoint}_{EndPoint}]";
    //    return $"[IsGetInfoSuccess:{IsGetInfoSuccess}_({StartPoint.x},{StartPoint.y},{StartPoint.z},{StartPoint.w})_({EndPoint.x},{EndPoint.y},{EndPoint.z},{EndPoint.w})]";
    //}

    public override string ToString()
    {
        return $"[{((Vector3)StartPoint).Vector3ToString3()},{StartNormal.Vector3ToString3()} - {((Vector3)EndPoint).Vector3ToString3()}],{EndNormal.Vector3ToString3()}"; 
    }


    public PipeLineData(Vector4 p1, Vector4 p2, Vector3 direction)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.Direction = direction;
        this.IsGetInfoSuccess = true;
        this.IsObbError = false;
        this.StartNormal = Vector3.zero;
        this.EndNormal = Vector3.zero;
    }

    public PipeLineData(List<List<Vector3>> lines)
    {
        if (lines.Count != 2)
        {
            Debug.LogError($"PipeLineInfo lines.Count != 2 lines:{lines.Count}");
        }
        CircleInfo circle1 = new CircleInfo(lines[0]);
        CircleInfo circle2 = new CircleInfo(lines[1]);
        //InitByCircle(circle1, circle2);

        this.StartPoint = circle1.GetCenter4();
        this.EndPoint = circle2.GetCenter4();
        this.StartNormal = circle1.GetNormal();
        this.EndNormal = circle2.GetNormal();

        this.Direction = this.EndPoint- this.StartPoint;

        this.IsGetInfoSuccess = true;
        this.IsObbError = false;
       
    }

    public PipeLineData(Vector4 p1, Vector4 p2, Vector3 p1N, Vector3 p2N, Vector3 direction)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.Direction = direction;
        this.IsGetInfoSuccess = true;
        this.IsObbError = false;
        this.StartNormal = p1N;
        this.EndNormal = p2N;
    }

    public PipeLineData(Vector4 p1, Vector4 p2, Vector3 p1N, Vector3 p2N)
    {
        this.StartPoint = p1;
        this.EndPoint = p2;
        this.Direction = p1-p2;
        this.IsGetInfoSuccess = true;
        this.IsObbError = false;
        this.StartNormal = p1N;
        this.EndNormal = p2N;
    }
}




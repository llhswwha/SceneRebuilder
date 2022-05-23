using MeshJobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ThreePointJobResult
{
    public bool IsNull;
    public int id;

    public int gId;
    public int count;

    public int vertexCount;

    public double time;

    public Vector3 center;

    public float maxDis;
    public float minDis;

    public Vector3[] maxPList;

    public int minPCount;

    public Vector3[] minPList;

    public int maxPCount;

    public void Init()
    {
        // maxPList=new List<Vector3>();
        // minPList=new List<Vector3>();
    }

    public void Print()
    {
        // Debug.LogWarning(string.Format("ThreePointJobResult[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
        //  this.vertexCount,time,center,0,0,maxDis,minDis,0,0,count));

        Debug.LogWarning(string.Format("ThreePointJobResult[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
         this.vertexCount, time, center, maxPList[0], minPList[0], maxDis, minDis, maxPList.Length, minPList.Length, count));
    }

    public override string ToString()
    {
        return string.Format("(Result vertex:{0},center:{1},maxP:{2},minP:{3},maxDis:{4},minDis:{5},{6},{7};)",
             this.vertexCount, center, maxPList[0], minPList[0], maxDis, minDis, maxPList.Length, minPList.Length);
    }

    public List<MinMaxId> GetAllMinMaxIds()
    {
        var meshData = this;

        List<MinMaxId> ids = new List<MinMaxId>();
        for (int i = 0; i < meshData.maxPList.Length; i++)
        {
            for (int j = 0; j < meshData.minPList.Length; j++)
            {
                ids.Add(new MinMaxId(i, j));
            }
        }
        //allMinMax = ids;
        return ids;
    }

    public ThreePoint[] tps;

    public static int MaxPointCount = 3;

    public ThreePoint[] GetThreePoints(Matrix4x4 matrix)
    {
        var meshData = this;
        int count = meshData.maxPList.Length * meshData.minPList.Length;
        int id = 0;
        ThreePoint[] ids = new ThreePoint[count];
        for (int i = 0; i < meshData.maxPList.Length && i < MaxPointCount; i++)
        {
            var max = meshData.maxPList[i];
            for (int j = 0; j < meshData.minPList.Length && j < MaxPointCount; j++)
            {
                var min = meshData.minPList[j];
                ids[id] = new ThreePoint(center, min, max, j, i, matrix);
                // ids.Add(new ThreePoint(center,min,max,j,i,matrix));
                id++;//这个不能忘记了
            }
        }
        //allMinMax = ids;

        this.tps = ids;
        return ids;
    }

    public List<RTTransform> GetRTTransforms(Matrix4x4 matrix)
    {
        if (rts != null)
        {
            return rts;
        }
        GetThreePoints(matrix);
        return GetRTTransforms();
    }

    public List<RTTransform> rts;

    public List<RTTransform> GetRTTransforms()
    {
        List<RTTransform> tmp = new List<RTTransform>();
        for (int l = 0; l < tps.Length; l++)
        {
            var id1 = tps[l];

            var rt = ThreePointHelper.GetRT(ref id1, null);
            tmp.Add(rt);
        }
        this.rts = tmp;
        return tmp;
    }

    //     public Vector3 GetLongLine(Transform t)
    // {
    //     if(IsWorld)return maxPList[maxPId]-center;
    //     if(t==null) t=_obj.transform;
    //     return t.TransformPoint(maxPList[maxPId]) - t.TransformPoint(center);
    // }

    // public Vector3 GetShortLine(Transform t)
    // {
    //     if(IsWorld)return minPList[minPId]-center;
    //     if(t==null) t=_obj.transform;
    //     return t.TransformPoint(minPList[minPId]) - t.TransformPoint(center);
    // }


    public Vector3 GetLongShortNormal(Transform t, ThreePoint id)
    {
        var longLine = t.TransformPoint(id.maxP) - t.TransformPoint(center);
        longLine = Vector3.Normalize(longLine);

        var shortLine = t.TransformPoint(id.minP) - t.TransformPoint(center);
        shortLine = Vector3.Normalize(shortLine);

        Vector3 nor = Vector3.Cross(longLine, shortLine);

        Vector3 r = nor;

        // float angle1=Vector3.Angle(longLine,nor);//90度
        // float angle2=Vector3.Angle(shortLine,nor);//90度
        // Debug.Log($"GetLongShortNormal1 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");

        // GetLongShortNormal(t.localToWorldMatrix,id);

        return r;
    }

    public Matrix4x4 localToWorldMatrix;

    public Vector3 GetLongShortNormal(ThreePoint id)
    {
        return GetLongShortNormal(this.localToWorldMatrix, id);
    }

    public Vector3 GetLongShortNormal(Matrix4x4 localToWorldMatrix, ThreePoint id)
    {
        var maxP = localToWorldMatrix.MultiplyPoint3x4(id.maxP);
        var minP = localToWorldMatrix.MultiplyPoint3x4(id.minP);
        var centerP = localToWorldMatrix.MultiplyPoint3x4(center);
        //var centerP=center*t.localToWorldMatrix;
        var longLine = maxP - centerP;

        longLine = Vector3.Normalize(longLine);
        var shortLine = minP - centerP;
        shortLine = Vector3.Normalize(shortLine);

        Vector3 nor = Vector3.Cross(longLine, shortLine);

        // float angle1=Vector3.Angle(longLine,nor);//90度
        // float angle2=Vector3.Angle(shortLine,nor);//90度
        // Debug.Log($"GetLongShortNormal2 longLine:{longLine},shortLine:{shortLine},nor:({nor.x},{nor.y},{nor.z}),angle1:{angle1},angle2:{angle2}");
        return nor;
    }


    public Vector3 TransformPoint(Matrix4x4 localToWorld, Vector3 pos)
    {
        //   Vector4 v = pos;
        //     v.w = 1;
        //     Vector4 wPos = localToWorld * v;
        //     return v;

        return localToWorld.MultiplyPoint3x4(pos);
    }


    public Vector3 GetCenterP(Transform t)
    {
        // return t.TransformPoint(center);
        //return TransformPoint(t.localToWorldMatrix,center);

        var c1 = t.TransformPoint(center);
        // var c2=t.localToWorldMatrix.MultiplyPoint3x4(center);
        // c1.PrintVector3("c1");
        // c2.PrintVector3("c2");
        return c1;
    }
}


public class ThreePointJobResultList
{
    public static ThreePointJobResultList Instance = new ThreePointJobResultList();

    public void Print()
    {
        foreach (var r in results)
        {
            r.Print();
        }
    }

    public Dictionary<int, ThreePointJobResult> go2Result = new Dictionary<int, ThreePointJobResult>();

    public void InitDict()
    {
        go2Result.Clear();
        foreach (var r in results)
        {
            AddDict(r);
        }
    }

    public ThreePointJobResult[] GetAllResults()
    {
        return results;
    }

    public ThreePointJobResult GetThreePointResult(MeshPoints go1)
    {
        int goId = go1.GetInstanceID();

        if (go2Result.ContainsKey(goId))
        {
            return go2Result[goId];
        }

        //不知道为什么，打印显示有关key是0,就是找不到的那个
        // Debug.LogWarning($"GetThreePoint1 go2Result Not Found Go:{goId},count:{go2Result.Count}");
        // foreach(var key in go2Result.Keys){
        //     Debug.LogWarning($"key:{key}");
        // }

        foreach (var r in results)
        {
            if (r.gId == goId)
            {
                go2Result.Add(goId, r);
                return r;
            }
        }

        //不知道为什么，打印显示有关key是0,就是找不到的那个
        Debug.LogWarning($"GetThreePoint1 go2Result Not Found Go:{goId},count:{go2Result.Count}");
        foreach (var key in go2Result.Keys)
        {
            Debug.LogWarning($"key:{key}");
        }

        Debug.LogError($"GetThreePoint2 results Not Found Go:{goId},count:{results.Length}");
        return new ThreePointJobResult() { IsNull = true };
    }

    public ThreePoint[] GetThreePoints(MeshPoints mf)
    {
        Transform t = mf.transform;
        //ThreePointJobResult resultFrom = ThreePointJobResultList.Instance.GetThreePointResult(mf);
        ThreePointJobResult resultFrom = this.GetThreePointResult(mf);
        resultFrom.localToWorldMatrix = t.localToWorldMatrix;
        var tpsFrom = resultFrom.GetThreePoints(t.localToWorldMatrix);
        if (tpsFrom.Length == 0)
        {
            Debug.LogError($"GetTreePoints tpsFrom.Length == 0 t:{t} mf:{mf}");
        }
        return tpsFrom;
    }

    private ThreePointJobResult[] results;

    private void AddDict(ThreePointJobResult r)
    {
        // if (r == null)
        // {
        //     Debug.LogError("AddDict r==null");
        //     return;
        // }
        int goId = r.gId;
        if (go2Result.ContainsKey(goId))
        {
            go2Result[goId] = r;
        }
        else
        {
            go2Result.Add(goId, r);
        }
    }

    public void SetResult(ThreePointJobResult r, int id, int goId)
    {
        r.gId = goId;
        results[id] = r;
        //   Debug.Log($"SetResult id:{id},gId:{goId},r:{r},count:{go2Result.Count}");
    }

    public void InitResult(int count)
    {
        results = new ThreePointJobResult[count];
    }
}

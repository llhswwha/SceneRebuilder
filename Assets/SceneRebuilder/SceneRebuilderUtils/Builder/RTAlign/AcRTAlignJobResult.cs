using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AcRTAlignJobResult
{
    public static int ID = 0;

    public static int GetId()
    {
        int id = ID;
        ID++;
        return id;
    }

    private static Dictionary<int, IRTResult> Results = new Dictionary<int, IRTResult>();

    public static IRTResult[] rs;

    public static void SetResult(int id, IRTResult r, double t)
    {
        AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);
        arg.Result = r;
        arg.Time = t;
        //  Debug.LogError("AcRTAlignJobResult SetResult:"+id+","+r);


        if (rs == null)
        {
            rs = new IRTResult[10];
        }
        if (r != null)
        {
            r.id = id;
            //r.Time = t;
        }
        // else{
        //     Debug.LogError("SetResult r==null:"+id);
        // }
        //rs.Add(r);


        if (rs.Length - 1 < id)
        {
            Debug.LogError($"AcRTAlignJobResult rs.Length-1 < id id:{id} length:{rs.Length} r:{r}");
            var newRs = new IRTResult[id + 1];
            for (int i = 0; i < newRs.Length && i < rs.Length; i++)
            {
                newRs[i] = rs[i];
            }
            rs = newRs;

            rs[id] = r;
        }
        else
        {
            rs[id] = r;
        }


        //if(Results.ContainsKey(id)){
        //    Results[id]=r;
        //}
        //else{
        //    Results.Add(id,r);
        //}
    }

    public static void InitDict()
    {
        if (Results.Count > 0) return;
        if (rs == null)
        {
            Debug.LogError("AcRTAlignJobResult.InitDict rs==null");
            return;
        }
        for (int i = 0; i < rs.Length; i++)
        {
            IRTResult item = rs[i];
            if (item == null)
            {
                Results.Add(i, null);
                //                    Debug.Log($"InitDict2 {i},null");
                continue;
            }
            Results.Add(item.id, item);
            //Debug.Log($"InitDict1 {item.id},{item}");
        }
    }

    public static void CleanResults()
    {
        //rs.Clear();
        Results.Clear();
    }

    public static IRTResult GetResult(int id)
    {
        InitDict();
        //Debug.LogError("GetResult id:"+id);
        if (Results.ContainsKey(id))
        {
            var r = Results[id];
            // if(r==null){
            //     Debug.LogError("GetResult r==null:"+id);
            // }
            //Debug.LogError("GetResult r:"+r);
            return Results[id];
        }
        Debug.LogError($"GetResult Results.ContainsKey(id)==false, {Results.Count},{rs.Length}");
        //foreach (var item in Results.Keys)
        //{
        //    Debug.LogError($"key:{item}");
        //}
        //foreach (var item in rs)
        //{
        //    Debug.LogError($"item:{item.id}");
        //}
        foreach (var item in rs)
        {
            if (item == null) continue;
            if (item.id == id)
            {
                Results.Add(id, item);
                return item;
            }
        }
        return null;

    }

    public static void InitCount(int count)
    {
        rs = new IRTResult[count];
    }

    //public static float MinDis=0.1f;
}



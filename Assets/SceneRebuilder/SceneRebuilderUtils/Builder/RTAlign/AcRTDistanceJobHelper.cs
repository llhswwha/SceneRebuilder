using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AcRTDistanceJobHelper
{

    public const float zero = 1E-08f;

    public static int ID = 0;

    public static Dictionary<int, AcRTDistanceJobResult> results = new Dictionary<int, AcRTDistanceJobResult>();

    public static void PrintResults()
    {
        foreach (var key in results.Keys)
        {
            var r = results[key];
            Debug.Log(string.Format("PrintResults key:{0},value:{1}", key, r));
        }
    }

    public static void Reset()
    {
        ID = 0;
        results.Clear();
    }

    public static int InitResult()
    {
        AcRTDistanceJobResult result = new AcRTDistanceJobResult();
        int id = ID++;
        results.Add(id, result);
        return id;
    }

    public static void SetResult(int rId, int id, float dis, RTResult rt)
    {
        AcRTDistanceJobResult result = results[rId];
        if (dis < result.min)
        {
            result.min = dis;
            result.minId = id;
            result.rt = rt;

            if (dis <= zero)
            {
                result.IsZero = true;
            }
        }

    }
}

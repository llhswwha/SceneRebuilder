using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeTest
{
    public static Dictionary<string, DateTime> times = new Dictionary<string, DateTime>();
    public static void Start(string tag)
    {
        if (times.ContainsKey(tag))
        {
            DateTime start = times[tag];
            TimeSpan time = DateTime.Now - start;
#if UNITY_EDITOR
            Debug.LogError($"TimeTest.Start[time:{time.TotalMilliseconds:F2}ms] tag:{tag} ");
#endif
            times[tag] = DateTime.Now;
        }
        else
        {
            times.Add(tag,DateTime.Now);
        }
    }

    public static void Stop(string tag,string end)
    {
        if (times.ContainsKey(tag))
        {
            DateTime start = times[tag];
            TimeSpan time = DateTime.Now - start;
#if UNITY_EDITOR
            Debug.LogError($"TimeTest.Stop[time:{time.TotalMilliseconds:F2}ms] tag:{tag} end:{end} ");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"TimeTest.Stop NoStartTime tag:{tag} end:{end}");
#endif
        }
    }
}

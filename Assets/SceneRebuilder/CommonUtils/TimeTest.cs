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
            Debug.LogError($"TimeTest.Start tag:{tag} time:{time.TotalMilliseconds:F2}ms");
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
            Debug.LogError($"TimeTest.Stop tag:{tag} end:{end} time:{time.TotalMilliseconds:F2}ms");
        }
        else
        {
            Debug.LogError($"TimeTest.Stop NoStartTime tag:{tag} end:{end}");
        }
    }
}

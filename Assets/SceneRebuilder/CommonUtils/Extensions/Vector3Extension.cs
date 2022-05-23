using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static void PrintVector3(this Vector3 v, string n)
    {
        Debug.Log($"[{n}]({v.x},{v.y},{v.z})");
    }

    public static string Vector3ToString(this Vector3 v)
    {
        return $"({v.x},{v.y},{v.z})";
    }

    public static string Vector3ToString6(this Vector3 v)
    {
        return $"({Round(v.x, 6):F6},{Round(v.y, 6):F6},{Round(v.z, 6):F6})";
    }

    public static string Vector3ToString5(this Vector3 v)
    {
        return $"({Round(v.x, 5):F5},{Round(v.y, 5):F5},{Round(v.z, 5):F5})";
    }

    public static string Vector3ToString4(this Vector3 v)
    {
        return $"({Round(v.x, 4):F4},{Round(v.y, 4):F4},{Round(v.z, 4):F4})";
    }

    public static string Vector3ToString3(this Vector3 v)
    {
        return $"({Round(v.x, 3):F3},{Round(v.y, 3):F3},{Round(v.z, 3):F3})";
    }

    public static double Round(double input, int power)
    {
        double p = Math.Pow(10, power);
        double output = Math.Round(input * p) / p;
        //Debug.Log($"Round p:{p} input:{input} output:{output}");
        return output;
    }
}

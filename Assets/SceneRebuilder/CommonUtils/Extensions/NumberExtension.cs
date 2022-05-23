using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExtension 
{
    public static float RoundToEx(this float v, int pt)
    {
        float p = Mathf.Pow(10, pt);
        int a = Mathf.RoundToInt(v * p);
        float b = a / p;
        return b;
    }
}

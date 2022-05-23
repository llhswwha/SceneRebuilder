using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
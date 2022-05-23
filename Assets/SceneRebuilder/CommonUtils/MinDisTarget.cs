using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MinDisTarget<T> where T : Component
{
    public float dis = float.MaxValue;
    public float meshDis = float.MaxValue;
    public T target = null;
    public MinDisTarget(float dis, float meshDis, T t)
    {
        this.dis = dis;
        target = t;
        this.meshDis = meshDis;
    }
}

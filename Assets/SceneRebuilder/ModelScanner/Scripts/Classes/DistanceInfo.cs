using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DistanceInfo
{
    public RendererInfo info;
    public Vector3 objPos;
    public Vector3 camPos;

    public void SetPos(Vector3 p1,Vector3 p2)
    {
        objPos = p1;
        camPos = p2;
    }
}
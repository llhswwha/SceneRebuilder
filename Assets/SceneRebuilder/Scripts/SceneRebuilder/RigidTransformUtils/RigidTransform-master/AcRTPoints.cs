using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using Accord;
using System;
using MeshJobs;

[Serializable]
public struct AcRTPoints5
{
    public UnityEngine.Vector3 p01; 
    public UnityEngine.Vector3 p02;
    public UnityEngine.Vector3 p03;

    public UnityEngine.Vector3 p11; 
    public UnityEngine.Vector3 p12;
    public UnityEngine.Vector3 p13;

    public UnityEngine.Vector3 p04;

    public UnityEngine.Vector3 p14;

    public UnityEngine.Vector3 p05;

    public UnityEngine.Vector3 p15;

    public void PrintLog(string tag)
    {
        string txt="";
        txt+="p01:"+p01.Vector3ToString()+"\n";
        txt+="p02:"+p02.Vector3ToString()+"\n";
        txt+="p03:"+p03.Vector3ToString()+"\n";
        txt+="p04:"+p04.Vector3ToString()+"\n";
        txt+="p05:"+p05.Vector3ToString()+"\n";
        txt+="p11:"+p11.Vector3ToString()+"\n";
        txt+="p12:"+p12.Vector3ToString()+"\n";
        txt+="p13:"+p13.Vector3ToString()+"\n";
        txt+="p14:"+p14.Vector3ToString()+"\n";
        txt+="p15:"+p15.Vector3ToString()+"\n";
        Debug.Log($"AcRTPoints[{tag}]:\n{txt}");
    }
}

[Serializable]
public class AcRTPoints
{
    public UnityEngine.Vector3[] ps1;//psFrom
    public UnityEngine.Vector3[] ps2;//psTo

    public AcRTPoints(UnityEngine.Vector3[] ps1, UnityEngine.Vector3[] ps2)
    {
        this.ps1 = ps1;
        this.ps2 = ps2;
    }
}


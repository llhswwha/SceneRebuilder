using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ModelLevelSettingInfo
{
    public float maxDiameter = 0;

    public float updateInterval = 0.2f;

    public float CameraMaxAngle = 65;

    public bool IsStatic=false;

    public ModelLevelSettingInfo()
    {

    }

    public ModelLevelSettingInfo(float d,float interval,float angle,bool isStatic)
    {
        this.maxDiameter = d;
        this.updateInterval = interval;
        this.CameraMaxAngle = angle;
        this.IsStatic=isStatic;
    }

    public ModelLevelSettingInfo(float d,float interval,float angle)
    {
        this.maxDiameter = d;
        this.updateInterval = interval;
        this.CameraMaxAngle = angle;
        this.IsStatic=false;
    }
}

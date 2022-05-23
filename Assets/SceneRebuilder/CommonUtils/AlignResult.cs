using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AlignResult
{

    public int maxPId;

    public int minPId;

    public Vector3 centerP;

    public Vector3 maxP;

    public Vector3 minP;

    public bool IsZero;//����

    public bool IsRelativeZero;//���

    public float Angle1;

    public float Angle2;

    public float ShortAngle;

    public float LongAngle;

    public float Distance;

    public override string ToString()
    {
        return $"��IsZero:{IsZero};IsZero2:{IsRelativeZero}�� Distance:{Distance};ShortAngle:{ShortAngle};LongAngle:{LongAngle};Angle1:{Angle1};Angle2:{Angle2}";
    }
}
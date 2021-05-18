using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    public Vector3 aroundPoint;

    public Vector3 aroundAxis;

    public float angle=0;

    [ContextMenu("DoRoate")]
    public void DoRotate()
    {
        this.transform.RotateAround(aroundPoint,aroundAxis,angle);
    }

    [ContextMenu("ReverseRotate")]
    public void ReverseRotate()
    {
        this.transform.RotateAround(aroundPoint,aroundAxis,-angle);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionRotation : MonoBehaviour
{
    public Vector3 axis = Vector3.forward;

    //public Vector3 angle = Vector3.zero;

    public float angle = 0;

    [ContextMenu("Rotate")]
    public void Rotate()
    {
        Quaternion rotation0 = transform.rotation;
        rotation0 = Quaternion.AngleAxis(angle, axis) * rotation0;
        transform.rotation = rotation0;
        rotation = rotation0;
    }

    void Start()
    {
        Rotate();
    }

    public Quaternion rotation;
}

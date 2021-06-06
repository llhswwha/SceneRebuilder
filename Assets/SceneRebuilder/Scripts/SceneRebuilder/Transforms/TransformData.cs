using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransformData 
{
    public Transform transform;

    //
    // 摘要:
    //     Position of the transform relative to the parent transform.
    public Vector3 localPosition;
    //
    // 摘要:
    //     The rotation as Euler angles in degrees.
    public Vector3 eulerAngles;
    //
    // 摘要:
    //     The rotation as Euler angles in degrees relative to the parent transform's rotation.
    public Vector3 localEulerAngles;
    //
    // 摘要:
    //     The red axis of the transform in world space.
    public Vector3 right;
    //
    // 摘要:
    //     The green axis of the transform in world space.
    public Vector3 up;
    // 摘要:
    //     Returns a normalized vector representing the blue axis of the transform in world
    //     space.
    public Vector3 forward;
    //
    // 摘要:
    //     A Quaternion that stores the rotation of the Transform in world space.
    public Quaternion rotation;
    //
    // 摘要:
    //     The world space position of the Transform.
    public Vector3 position;
    //
    // 摘要:
    //     The rotation of the transform relative to the transform rotation of the parent.
    public Quaternion localRotation;
    //
    // 摘要:
    //     The parent of the transform.
    public Transform parent;
    //
    // 摘要:
    //     Matrix that transforms a point from world space into local space (Read Only).
    public Matrix4x4 worldToLocalMatrix;
    //
    // 摘要:
    //     Matrix that transforms a point from local space into world space (Read Only).
    public Matrix4x4 localToWorldMatrix;
    //
    // 摘要:
    //     Returns the topmost transform in the hierarchy.
    public Transform root;
    //
    // 摘要:
    //     The number of children the parent Transform has.
    public int childCount;
    //
    // 摘要:
    //     The global scale of the object (Read Only).
    public Vector3 lossyScale;
    //
    // 摘要:
    //     The scale of the transform relative to the GameObjects parent.
    public Vector3 localScale { get; set; }
    //
    // 摘要:
    //     The transform capacity of the transform's hierarchy data structure.
    public int hierarchyCapacity { get; set; }
    //
    // 摘要:
    //     The number of transforms in the transform's hierarchy data structure.
    public int hierarchyCount { get; }

    public TransformData(Transform t)
    {
        transform = t;

        this.localPosition = t.localPosition;
        this.eulerAngles = t.eulerAngles;
        this.localEulerAngles = t.localEulerAngles;
        this.right = t.right;
        this.up = t.up;
        this.forward = t.forward;
        this.rotation = t.rotation;
        this.position = t.position;
        this.localRotation = t.localRotation;
        this.parent = t.parent;
        this.worldToLocalMatrix = t.worldToLocalMatrix;
        this.localToWorldMatrix = t.localToWorldMatrix;
        this.root = t.root;
        this.childCount = t.childCount;
        this.lossyScale = t.lossyScale;
        this.localScale = t.localScale;
    }
}

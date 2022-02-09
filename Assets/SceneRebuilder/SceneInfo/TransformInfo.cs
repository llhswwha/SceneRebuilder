using System;
using UnityEngine;

[Serializable]
public struct TransformInfo //: ScriptableObject
{
    [SerializeField]
    public Point3Info pos;
    [SerializeField]
    public Point3Info rotation;
    [SerializeField]
    public Point3Info scale;

    internal TransformInfo Copy(Vector3 offset)
    {
        TransformInfo info = new TransformInfo();
        info.pos = pos.Copy(offset);
        info.rotation = rotation.Copy();
        info.scale = scale.Copy();
        return info;
    }

    public TransformInfo(Transform t)
    {
        pos = new Point3Info(t.position);
        rotation = new Point3Info(t.rotation.eulerAngles);
        scale = new Point3Info(t.lossyScale);
    }

    public void Init()
    {
        pos = new Point3Info();
        rotation = new Point3Info();
        scale = new Point3Info(1,1,1);
    }
}
using System;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public struct Point3Info//:ScriptableObject
{
    [XmlAttribute]
    public float x;
    [XmlAttribute]
    public float y;
    [XmlAttribute]
    public float z;

    public Point3Info(float x,float y,float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point3Info(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }

    //public static float Power = 1000f;

    public static float Power = 1000f;

    public Vector3 ToPos()
    {
        return new Vector3(-x/ Power, z / Power, -y / Power);
    }
    //public Vector3 ToRotation()
    //{
    //    return new Vector3(-x,-z,-y);
    //}
    public Vector3 ToScale()
    {
        return new Vector3(x,z,y);
    }

    internal Point3Info Copy()
    {
        Point3Info info = new Point3Info();
        info.x = this.x;
        info.y = this.y;
        info.z = this.z;
        return info;
    }

    internal Point3Info Copy(Vector3 offset)
    {
        Point3Info info = new Point3Info();
        info.x = this.x + offset.x;
        info.y = this.y + offset.y;
        info.z = this.z + offset.z;
        return info;
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2}", x, y, z);
    }
}

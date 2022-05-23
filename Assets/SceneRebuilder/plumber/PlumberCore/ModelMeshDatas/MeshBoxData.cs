using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public struct MeshBoxData
{
    //public Vector3 center;

    //public Vector3 DirX;

    //public Vector3 DirY;

    //public Vector3 DirZ;

    //public float SizeX;

    //public float SizeY;

    //public float SizeZ;

    public OrientedBoundingBox OBB;

    [XmlAttribute]
    public bool IsGetInfoSuccess;
}

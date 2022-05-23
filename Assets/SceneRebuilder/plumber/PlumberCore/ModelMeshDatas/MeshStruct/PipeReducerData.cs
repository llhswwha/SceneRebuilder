using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public struct PipeReducerData
{
    public Vector4 StartPoint;
    public Vector4 EndPoint;

    [XmlAttribute]
    public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;

    public PipeModelKeyPointData4 KeyPointInfo;

    public override string ToString()
    {
        return $"ReducerData IsSpecial:{IsSpecial} IsGetInfoSuccess:{IsGetInfoSuccess} StartPoint:{StartPoint} EndPoint:{EndPoint}";
    }
}

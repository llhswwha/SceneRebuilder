using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public struct PipeElbowData
{

    public PipeModelKeyPointData4 KeyPointInfo;

    public PipeModelKeyPointData4 InnerKeyPointInfo;

    [XmlAttribute]
    public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;
}

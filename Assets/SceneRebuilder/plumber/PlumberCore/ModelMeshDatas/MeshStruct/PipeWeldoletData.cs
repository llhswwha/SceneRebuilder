using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public struct PipeWeldoletData
{

    public PipeModelKeyPointData4 KeyPointInfo;

    //public PipeElbowKeyPointData InnerKeyPointInfo;

    //public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;

    public override string ToString()
    {
        return $"PipeWeldoletData KeyPointCount:{KeyPointCount} IsGetInfoSuccess:{IsGetInfoSuccess} ";
    }
}

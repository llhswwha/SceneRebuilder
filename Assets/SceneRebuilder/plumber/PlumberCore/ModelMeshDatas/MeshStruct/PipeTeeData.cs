using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public struct PipeTeeData
{


    public PipeModelKeyPointData4 KeyPointInfo;

    public PipeModelKeyPointData4 InnerKeyPointInfo;

    public PipeModelKeyPlaneData4 KeyPlaneInfo;
    public PipeModelKeyPlaneData4 InnerKeyPlaneInfo;

    [XmlAttribute]
    public bool IsSpecial;

    [XmlAttribute]
    public bool IsGetInfoSuccess;

    [XmlAttribute]
    public int KeyPointCount;

    public override string ToString()
    {
        return $"TeeData IsSpecial:{IsSpecial} IsGetInfoSuccess:{IsGetInfoSuccess} KeyPointInfo:{KeyPointInfo} InnerKeyPointInfo:{InnerKeyPointInfo} KeyPlaneInfo:{KeyPlaneInfo} InnerKeyPlaneInfo:{InnerKeyPlaneInfo}";
    }
}

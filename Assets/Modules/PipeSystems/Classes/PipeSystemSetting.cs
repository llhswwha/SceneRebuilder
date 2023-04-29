using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PipeSystemSetting 
{
    [XmlAttribute]
    public float OffsetHeight=595.306f;

    [XmlAttribute]
    public float PipeSizePow=1;

    [XmlAttribute]
    public float OffPosX=0;

    [XmlAttribute]
    public float OffPosY=0;

    [XmlAttribute]
    public float OffPosZ=0;

    [XmlAttribute]
    public float OffScaleX=1;

    [XmlAttribute]
    public float OffScaleY=1;

    [XmlAttribute]
    public float OffScaleZ=1;

    public bool IsOriginal()
    {
        return OffPosX==0 && OffPosY==0 && OffPosZ==0 && OffScaleX==1 && OffScaleY==1 && OffScaleZ==1 && PipeSizePow==1;
    }
    
    public Vector3 GetOffPos()
    {
        return new Vector3(OffPosX,OffPosY,OffPosZ);
    }

    public void SetOffPos(Vector3 pos)
    {
        OffPosX=pos.x;
        OffPosY=pos.y;
        OffPosZ=pos.z;
    }

    public Vector3 GetOffScale()
    {
        return new Vector3(OffScaleX,OffScaleY,OffScaleZ);
    }

    public void SetOffScale(Vector3 scale)
    {
        OffScaleX=scale.x;
        OffScaleY=scale.y;
        OffScaleZ=scale.z;
    }

    public override string ToString()
    {
        return $"[PipeSystemSetting OffsetHeight:{OffsetHeight} pipeSizePow:{PipeSizePow}]";
    }
}

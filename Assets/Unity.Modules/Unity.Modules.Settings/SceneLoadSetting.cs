using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "SceneLoadSetting")]
public class SceneLoadSetting 
{
    [XmlAttribute]
    public bool IsEnable = true;

    [XmlAttribute]
    public double DelayOfLoad = 2;

    [XmlAttribute]
    public double DelayOfUnLoad = 5;

    [XmlAttribute]
    public float WaittingInterval = 1f; 

    [XmlAttribute]
    public float AngleOfVisible = 60;

    [XmlAttribute]
    public float AngleOfLoad = 75;

    [XmlAttribute]
    public float AngleOfHidden = 80;

    [XmlAttribute]
    public float AngleOfUnLoad = 90;

    [XmlAttribute]
    public float DisOfVisible = 2500;//50
    [XmlAttribute]
    public float DisOfLoad = 4900;//70
    [XmlAttribute]
    public float DisOfHidden = 8700;//90
    [XmlAttribute]
    public float DisOfUnLoad = 12100;//110

    [XmlAttribute]
    public bool IsEnableLoad = true;

    [XmlAttribute]
    public bool IsEnableUnload = true;

    [XmlAttribute]
    public bool IsEnableHide = true;

    [XmlAttribute]
    public bool IsEnableShow = true;

    public override string ToString()
    {
        return $"IsEnable:{IsEnable} DelayOfLoad:{DelayOfLoad} DelayOfUnLoad:{DelayOfUnLoad} WaittingInterval:{WaittingInterval}";
    }
}

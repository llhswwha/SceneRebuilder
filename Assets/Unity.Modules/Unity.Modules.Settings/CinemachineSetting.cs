using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "CinemachineSetting")]
public class CinemachineSetting
{
    [XmlAttribute]
    public float CMvcamFollow_X = 0;
    [XmlAttribute]
    public float CMvcamFollow_Y = 20;
    [XmlAttribute]
    public float CMvcamFollow_Z = -10;

}

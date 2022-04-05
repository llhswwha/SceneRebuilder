using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "DeviceSetting")]
public class DeviceSetting {

    /// <summary>
    /// 在进入电厂时加载园区设备
    /// </summary>
    [XmlAttribute]
    public bool LoadParkDevWhenEnter = true;
    /// <summary>
    /// 是否加载记载设备 
    /// </summary>
    [XmlAttribute]
    public bool LoadAnchorDev = true;//todo:完善功能
    /// <summary>
    /// 是否在树中显示设备
    /// </summary>
    [XmlAttribute]
    public bool ShowDevInTree = false;//todo:完善功能

    /// <summary>
    /// 不加载任何设备，测试时用
    /// </summary>
    [XmlAttribute]
    public bool NotLoadAllDev = false;
}

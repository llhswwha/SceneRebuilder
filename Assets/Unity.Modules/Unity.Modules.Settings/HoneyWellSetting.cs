using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 视频监控相关配置
/// </summary>
[XmlType(TypeName = "HoneyWellSetting")]
public class HoneyWellSetting{

    /// <summary>
    /// 是否启用霍尼韦尔视频
    /// </summary>
    [XmlAttribute]
    public bool EnableHoneyWell=false;
    /// <summary>
    /// 视频监控系统的IP设置
    /// </summary>
    [XmlAttribute]
    public string Ip = "192.168.1.3";
    /// <summary>
    /// 视频监控系统的用户名
    /// </summary>
    [XmlAttribute]
    public string UserName = "sdk3D";
    /// <summary>
    /// 视频监控的登录password
    /// </summary>
    [XmlAttribute]
    public string PassWord = "123456";

    [XmlAttribute]
    public string MaxConnectTime = "10";

}

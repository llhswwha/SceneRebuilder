using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "DebugSetting")]
public class DebugSetting  {

    /// <summary>
    /// 是否关闭通信
    /// 关闭通信过程，调试确认是否是通信导致的问题。
    /// </summary>
    [XmlAttribute]
    public bool IsCloseCommunication;

    /// <summary>
    /// 是否远程模式
    /// 远程桌面时漫游的一个调试模式。
    /// </summary>
    [XmlElement]
    public bool IsRemoteMode = false;
}

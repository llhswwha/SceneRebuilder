using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 历史轨迹相关设置
/// </summary>
[XmlType(TypeName = "HistoryPathSetting")]
public class HistoryPathSetting  {

    /// <summary>
    /// 默认开始时间小时
    /// </summary>
    [XmlAttribute]
    public int StartHour = 8;
    /// <summary>
    /// 默认开始时间分钟,一个单位代表10分钟
    /// </summary>
    [XmlAttribute]
    public int StartMinute = 3;
    /// <summary>
    /// 默认播放时长，小时为单位
    /// </summary>
    [XmlAttribute]
    public int Duration = 8;

}

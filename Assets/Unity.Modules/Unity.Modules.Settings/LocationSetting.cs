using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 定位相关设置
/// </summary>
[XmlType(TypeName = "LocationSetting")]
public class LocationSetting  {

    /// <summary>
    /// 人员最大显示时间（小时）
    /// 大于这个时间的人员不在界面中显示出来。
    /// 移除大于locationPersonHideTimeHours小时，没有检测到信号的卡。
    /// 24小时还是72小时合适呢。
    /// </summary>
    [XmlElement]
    public int hideTimeHours = 72;

    [XmlAttribute]
    /// <summary>
    /// 历史轨迹中断时间设置。
    /// 一般两个历史数据点间隔时间超过10秒，认为中间为无历史数据,用虚线表示
    /// </summary>
    public int HistoryIntervalTime = 10;

    /// <summary>
    /// 是否使用NavMesh
    /// </summary>
    [XmlAttribute]
    public bool EnableNavMesh = true;

    /// <summary>
    /// 人员位置设置方式
    /// 0:简单模式，当前的方式
    /// 1:NavMesh，寻路算法，接下来引入
    /// 2:A*,寻路算法，NavMesh后引入研究，暂时不用
    /// </summary>
    public int NavMode = 0;

    /// <summary>
    /// NavMesh统一下调高度，默认1米
    /// </summary>
    [XmlAttribute]
    public float NavMeshHeightOffset = 1f;

}

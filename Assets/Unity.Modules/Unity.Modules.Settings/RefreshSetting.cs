using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
//namespace TModel.Models.Settings

[XmlType(TypeName = "RefreshSetting")]
[Serializable]
public class RefreshSetting
{
    [XmlAttribute]
    /// <summary>
    /// 位置信息刷新间隔
    /// </summary>
    public float TagPos = 0.35f;

    [XmlAttribute]
    /// <summary>
    /// 人员树节点刷新间隔
    /// </summary>
    public int PersonTree = 5;

    [XmlAttribute]
    /// <summary>
    /// 区域统计信息刷新间隔
    /// </summary>
    public int AreaStatistics = 5;


    [XmlAttribute]
    /// <summary>
    /// 部门树节点刷新间隔
    /// </summary>
    public int DepartmentTree = 5;

    [XmlAttribute]
    /// <summary>
    /// 附近摄像头刷新间隔
    /// </summary>
    public int NearCamera = 5;

    [XmlAttribute]
    /// <summary>
    /// 截图保存刷新间隔
    /// </summary>
    public int ScreenShot = 6;

    [XmlAttribute]
    /// <summary>
    /// 机组运行状态刷新间隔
    /// </summary>
    public int GroupPowerState=10;

}


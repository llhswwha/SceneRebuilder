using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// 漫游相关设置
/// </summary>
[XmlType(TypeName = "RoamSetting")]
public class RoamSetting
{
    /// <summary>
    /// 移动速度
    /// </summary>
    [XmlElement]
    public float moveSpeed = 10;
    /// <summary>
    /// 旋转速度
    /// </summary>
    [XmlElement]
    public float rotateSpeed = 0.5f;
    /// <summary>
    /// 滚轮移动速度
    /// </summary>
    [XmlElement]
    public float scrollSpeed = 0.05f;
    /// <summary>
    /// 按住shift，移动加速的倍数
    /// </summary>
    [XmlElement]
    public float shiftSpeed = 2f;
    /// <summary>
    /// 漫游移动速度
    /// </summary>
    [XmlElement]
    public float roamMoveSpeed = 1;
    /// <summary>
    /// 漫游旋转速度
    /// </summary>
    [XmlElement]
    public float roamRotateSpeed = 0.5f;

    [XmlElement]
    public float roamUnLockMoveSpeed = 0.005f;
    [XmlElement]
    public float roamUnLockScrollSpeed = 0.005f;
}
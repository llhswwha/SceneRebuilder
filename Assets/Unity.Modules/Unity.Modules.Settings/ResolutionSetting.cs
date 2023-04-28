using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
//namespace TModel.Models.Settings

[XmlType(TypeName = "ResolutionSetting")]
[Serializable]
public class ResolutionSetting
{
    /// <summary>
    /// 设置系统分辨率的宽
    /// </summary>
    [XmlAttribute]
    public int Width = 1920 ;
    /// <summary>
    /// 设置系统分辨率的高
    /// </summary>
    [XmlAttribute]
    public int High = 1080 ;

    /// <summary>
    /// 是否显示FPS
    /// </summary>
    [XmlAttribute]
    public bool ShowFps = false;

    [XmlAttribute]
    public bool EnableFrameLimt = false;

    [XmlAttribute]
    public int MaxFrameRate=30;

}


using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType(TypeName = "AssetLoadSetting")]
public class AssetLoadSetting {

    [XmlAttribute]
    public bool EnableUnloadFunction = true;

    /// <summary>
    /// 卸载策略 0:按数量 
    /// todo:1:按顶点数 2:按时间 3:按频率
    /// 【不用显示】
    /// </summary>
    [XmlAttribute]
    public int UnloadMode = 0;

    /// <summary>
    /// 模型（建筑）缓存数量，卸载资源
    /// </summary>
    [XmlAttribute]
    public int CacheCount = 2;
    //0:不缓存，加载一个，卸载一个。
    //1:加载A，加载B，卸载A，加载C，卸载B
    //设置为1000也就是只加载，不卸载

    /// <summary>
    /// 设备（主厂房独立设备）缓存数量，卸载资源
    /// </summary>
    [XmlAttribute]
    public int DeviceCacheCount = 15;

    /// <summary>
    /// 最大顶点数量
    /// 【不用显示，不过界面上加一个当前的模型状态的页面，在Debug是True下显示】
    /// </summary>
    [XmlAttribute]
    public int MaxVertex = 1500000;//150万

    /// <summary>
    /// 是否从服务端下载建筑模型和生产设备（静态设备）模型
    /// false的话是从服务端加载模型
    /// </summary>
    [XmlAttribute]
    public bool BuildingFromFile = true;

    /// <summary>
    /// 是否从服务端下载动态设备模型（摄像头、机柜、配电柜等）
    /// false的话是从服务端加载模型
    /// </summary>
    [XmlAttribute]
    public bool DeviceFromFile = true;

    /// <summary>
    /// 是否加载生产设备（静态设备）模型
    /// false的话，就是不加载
    /// </summary>
    [XmlAttribute]
    public bool LoadDeviceAsset = true;

    /// <summary>
    /// 设备模型贴图分辨率
    /// 512，1024，2048，4096，8192。
    /// 【当前功能还没做好,4096,8192】
    /// </summary>
    [XmlAttribute]
    public int DeviceResolution = 2048;

    /// <summary>
    /// 下载模型用的Url
    /// 【暂时没用，从服务端固定的位置上下载的】
    /// </summary>
    [XmlAttribute]
    public string HttpUrl = "";
}

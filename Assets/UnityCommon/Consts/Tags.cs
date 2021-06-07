using UnityEngine;
using System.Collections;

public static class Tags
{
    public const string Prefab = "Prefab";
    public const string CabinetGroup = "CabinetGroup";
    public const string Cabinet = "Cabinet";
    public const string RoomDevice = "RoomDevice";
    public const string Device = "Device";
    public const string Room = "Room";
    public const string Decoration = "Decoration";

    public const string RoomEntity = "RoomEntity";

    /// <summary>
    /// 环境中的装饰物，可移动
    /// </summary>
    public const string Environment = "Environment";
    /// <summary>
    /// 默认的未添加标签
    /// </summary>
    public const string Untagged = "Untagged";
    /// <summary>
    /// 墙壁地板等不可移动的环境元素
    /// </summary>
    public const string Static = "Static";

    /// <summary>
    /// 在地板上的环境设备 桌子椅子之类的
    /// </summary>
    public const string OnFloorEnvDevice = "OnFloorEnvDevice";

    /// <summary>
    /// 墙壁编辑球
    /// </summary>
    public const string EditSphere = "EditSphere";

    /// <summary>
    /// 墙上的装饰，如门窗
    /// </summary>
    public const string WallDecoration = "WallDecoration";

    /// <summary>
    /// 房间的装饰物体
    /// </summary>
    public const string RoomDecoration = "RoomDecoration";
    /// <summary>
    /// 端口的父物体
    /// </summary>
    public const string DevicePortParent = "DevicePortParent";
    /// <summary>
    /// 端口
    /// </summary>
    public const string DevicePort = "DevicePort";
    /// <summary>
    /// 端口闪烁的灯
    /// </summary>
    public const string PortTwinkleLight = "PortTwinkleLight";
    /// <summary>
    /// 端口普通的灯(亮和不亮两种状态)
    /// </summary>
    public const string PortNormalLight = "PortNormalLight";
    /// <summary>
    /// 水浸线编辑物体
    /// </summary>
    public const string WaterSoakLineEditObj = "WaterSoakLineEditObj";
    /// <summary>
    /// 机柜部件
    /// </summary>
    public const string CabinetPart = "CabinetPart";
    /// <summary>
    /// 机房设备部件
    /// </summary>
    public const string RoomDevicePart = "RoomDevicePart";
    /// <summary>
    /// 设备部件
    /// </summary>
    public const string DevicePart = "DevicePart";
    /// <summary>
    /// 三层视图中的节点
    /// </summary>
    public const string PlaneNode = "PlaneNode";
    /// <summary>
    /// 三层视图中的节点连线
    /// </summary>
    public const string PlaneLink = "PlaneLink";
    /// <summary>
    /// 逻辑拓扑节点tag
    /// </summary>
    public const string LogicNodeObj = "LogicNodeObj";

    /// <summary>
    /// 槽位设备（板卡）
    /// </summary>
    public const string SlotDev = "SlotDev";

    /// <summary>
    /// 机柜的前面设备平面
    /// </summary>
    public const string DeviceSurface = "DeviceSurface";
    /// <summary>
    /// 机柜的前面后面设备平面
    /// </summary>
    public const string DeviceSurfaceBack = "DeviceSurfaceBack";

    /// <summary>
    /// Tag是否为设备
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool IsDev(this string tag)
    {
        return tag == RoomDevice || tag == Device || tag == SlotDev;
    }
}

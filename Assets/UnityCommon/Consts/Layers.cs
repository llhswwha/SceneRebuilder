using UnityEngine;
using System.Collections;

public static class Layers
{

    //public const string RoomObject = "RoomObject";
    //public const string Cabinet = "Cabinet";
    ////public const string Decoration = "Decoration";
    //public const string FloorDecoration = "FloorDecoration";
    //public const string WallDecoration = "WallDecoration";
    //public const string Floor = "Floor";
    //public const string CeilingFloor = "CeilingFloor";
    //public const string Device = "Device";
    //public const string RoomDevice = "RoomDevice";
    //public const string DeviceSurface = "DeviceSurface";
    //public const string Wall = "Wall";
    //public const string GUI = "GUI";
    //public const string LogicNodeLayer = "LogicNodeLayer";

    /// <summary>
    /// 监控范围
    /// </summary>
    public const string Default = "Default";
    /// <summary>
    /// 监控范围
    /// </summary>
    public const string Range = "Range";
    /// <summary>
    /// unity自带的忽略射线碰撞层，但能触发Collider碰撞
    /// </summary>
    public const string IgnoreRaycast = "Ignore Raycast";
    /// <summary>
    /// 地板
    /// </summary>
    public const string Floor = "Floor";

    /// <summary>
    /// Wall
    /// </summary>
    public const string Wall = "Wall";

    /// <summary>
    /// Railing
    /// </summary>
    public const string Railing = "Railing";
}

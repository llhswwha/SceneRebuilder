using Location.WCFServiceReferences.LocationServices;

public static class DeviceAlarmHelper
{
    /// <summary>
    /// 获取告警信息字符串
    /// </summary>
    /// <param name="alarm"></param>
    /// <returns></returns>
    public static string TryGetDeviceAlarmInfo(DeviceAlarm alarm)
    {
        string returnTxt = "";

        if (alarm == null)
        {
            returnTxt = "告警信息为空，DeviceAlarm=null!";
        }
        else
        {
            returnTxt = string.Format("   找不到对应设备或区域\n告警标题: {0}\n告警信息: {1}\n告警等级: {2}\n告警代码: {3}\n告警源: {4}\n告警设备类型: {5}\n告警设备ID: {6}\n告警设备名称：{7}",
            alarm.Title, alarm.Message, alarm.LevelName, alarm.Code, GetAlarmSrc(alarm.Src), alarm.DevTypeName, alarm.DevId, alarm.DevName);
        }
        Log.Info("DevAlarmListPanel.TryGetDeviceAlarmInfo", returnTxt);
        return returnTxt;
    }

    /// <summary>
    /// 获取告警设备名称
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private static string GetAlarmSrc(Abutment_DevAlarmSrc src)
    {
        switch (src)
        {
            case Abutment_DevAlarmSrc.SIS:
                return "SIS";
            case Abutment_DevAlarmSrc.人员定位:
                return "人员定位";
            case Abutment_DevAlarmSrc.消防:
                return "消防";
            case Abutment_DevAlarmSrc.视频监控:
                return "视频监控";
            case Abutment_DevAlarmSrc.门禁:
                return "门禁";
            default:
                return "未知";
        }
    }
}

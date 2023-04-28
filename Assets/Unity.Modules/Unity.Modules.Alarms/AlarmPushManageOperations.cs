using Location.WCFServiceReferences.LocationServices;
using System;
using Unity.Modules.Context;

public static class AlarmPushManageOperations
{
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public static void FocusDevByAlarm(DeviceAlarm alarmInfo, string msg)
    {
        var logTag = "AlarmPushManageOperations.FocusDevByAlarm";
        try
        {
           
            if (alarmInfo == null)
            {
                Log.Error(logTag, string.Format("alarmInfo == null"));
                return;
            }
            var depId = alarmInfo.AreaId;

            Log.Info(logTag, string.Format("depId:"+ depId));

            if (depId == 0)
            {
                Log.Info(logTag, string.Format("depId == 0 根据设备查找区域id"));
                //var devInfo = CommunicationObject.Instance.GetDevByid(alarmInfo.DevId);
                var devInfo = Unity.Modules.Context.AppContext.DataClient.GetDevByid(alarmInfo.DevId);
                if (devInfo != null)
                {
                    depId = (int)devInfo.ParentId;
                    Log.Info(logTag, string.Format("devInfo: name={0},type={1}", devInfo.Name, devInfo.TypeName));
                }
                Log.Info(logTag, string.Format("新的数据 depId:{0},devId:{1}", depId, alarmInfo.DevId));
            }

            Log.Info(logTag, string.Format("点击定位设备 depId:{0},devId:{1},title:{2},msg:{3}", depId, alarmInfo.DevId, alarmInfo.Title,alarmInfo.Message));
            Unity.Modules.Context.AppContext.DevManager.FocusDev(alarmInfo.DevId.ToString(), depId, result =>
            {
                Log.Info(logTag, string.Format("After FocusDev result:{0}", result));
                if (!result)//定位失败
                {
                    string msgTitle = DeviceAlarmHelper.TryGetDeviceAlarmInfo(alarmInfo);
                    Unity.Modules.Context.AppContext.MessageBox.ShowMessage(msgTitle);
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error(logTag, ex.ToString());
        }

    }
}
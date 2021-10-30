using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Location.WCFServiceReferences.LocationServices
{
    #region 文档管理（上传下载）
    /// <summary>
    /// 文件传输信息
    /// </summary>
    public class FileTransferInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文件名称（无后缀）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 后缀名 .xml  .txt
        /// </summary>
        public string ExtensionName { get; set; }

        /// <summary>
        /// 上传前名称
        /// </summary>
        public string BeforeUploadPath { get; set; }
        /// <summary>
        /// 文件下载所在目录（例如：http:\\192.168.1.21:8013\Data）
        /// </summary>
        public string FileDownloadDirctoryURL { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 上传人
        /// </summary>
        public string UpLoadPersonName { get; set; }
        /// <summary>
        /// 手册类型
        /// </summary>
        public string ManualType { get; set; }
        /// <summary>
        /// 设备大类
        /// </summary>
        public string DeviceType { get; set; }
        /// <summary>
        /// 信息的唯一标识
        /// </summary>
        public string InfoGuid { get; set; }

        /// <summary>
        /// 设备告警产生时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 告警时间戳
        /// </summary>
        public long UploadTimeStamp { get; set; }
    }
    /// <summary>
    /// 文件上传下载切页信息
    /// </summary>
    public class FileTransferInfomation
    {
        public int Total { get; set; }
        public List<FileTransferInfo> fileInfoList { get; set; }

        public void SetEmpty()
        {
            if (fileInfoList != null && fileInfoList.Count == 0)
            {
                fileInfoList = null;
            }
        }
    }

    public class FileTransferManulDevTypes
    {
        /// <summary>
        /// 手册类型列表
        /// </summary>
        public List<string> manualTypeList;

        /// <summary>
        /// 设备分类
        /// </summary>
        public List<string> devTypeList;
    }

    public class FileTransferSearchArg
    {       
        public string Start { get; set; }       
        public string End { get; set; }        
        public string Keyword { get; set; }
        /// <summary>
        /// 分页
        /// </summary>      
        public PageInfo Page { get; set; }
        /// <summary>
        /// 手册分类
        /// </summary>
        
        public string ManualName { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>        
        public string DevTypeName { get; set; }
    }
    #endregion

    #region 设备分组
    public class DevGroup
    {
        /// <summary>
        ///设备分组Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分组所属父ID （区域||分组）
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 分组父节点类型
        /// </summary>
        public string ParentGroupType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 叶子节点：组下的设备
        /// </summary>
        public List<DevInfo> DevNodes { get; set; }

        /// <summary>
        /// 叶子节点：设备组下的子组
        /// </summary>
        public List<DevGroup> GroupNodes { get; set; }
    }
    public class DevGroupNode
    {
        /// <summary>
        ///设备分组Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分组所属父ID （区域||分组）
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 分组父节点类型
        /// </summary>
        public string ParentGroupType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 叶子节点：组下的设备
        /// </summary>
        public List<DevNode> DevNodes { get; set; }

        /// <summary>
        /// 叶子节点：设备组下的子组
        /// </summary>
        public List<DevGroupNode> GroupNodes { get; set; }
    }
    public class GroupDevs
    {
        //Todo:后续可加上其他信息
        public List<int> SameGroupDevs { get; set; }
        public List<DevInfo> SameGroupDevInfos { get; set; }
    }
    public class DevGroupTransferInfo
    {
        public DevGroup devGroup { get; set; }
        public List<DevInfo> devInfo { get; set; }
        public List<DevGroup> childGroup { get; set; }
        public AreaNode areaNode { get; set; }
    }
    public enum TimeSettingType { 无限制, 时间长度, 时间点范围, 时间长度加范围 }
    public enum RepeatDay
    {
        每天 = 0, 星期一 = 1, 星期二 = 2, 星期三 = 4, 星期四 = 8, 星期五 = 16, 星期六 = 32, 星期天 = 64,
    }
    /// <summary>
    /// 权限范围
    /// </summary>
    public enum AreaRangeType
    {
        WithParent,//从根节点到自身节点
        Single,//只有自身   
        WithChildren,//自身和子节点（递归下去）
        AllRelative,//父节点、自身、子节点（递归下去）
        All,//特殊，全部区域
    }

    /// <summary>
    /// 进出类型
    /// </summary>
    public enum AreaAccessType
    {
        可以进出,
        不能进入,
    }
    /// <summary>
    /// 具体权限分配记录
    /// </summary>
    public class AreaAuthorizationRecord
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 0 表示按时间长度设置权限，1 表示按时间点范围设置权限
        /// </summary>
        public TimeSettingType TimeType { get; set; }

        /// <summary>
        /// 权限起始时间点
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 权限结束时间点
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 权限时长,单位是 分钟
        /// </summary>
        public int TimeSpan { get; set; }

        public void SetTimeSpane()
        {
            if (StartTime == null) return;
            if (EndTime == null) return;
            DateTime start = (DateTime)StartTime;
            DateTime end = (DateTime)EndTime;
            var span = end - start;
            TimeSpan = (int)span.TotalMinutes;
        }

        /// <summary>
        /// 延迟时间，单位是 分钟
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        /// 误差距离，单位是 米
        /// </summary>
        public int ErrorDistance { get; set; }

        /// <summary>
        /// 重复天数
        /// </summary>
        public RepeatDay RepeatDay { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 进出类型，一般使用 AreaAccessType.EnterLeave ，只有特定的危险区域用 AreaAccessType.Leave
        /// </summary>
        public AreaAccessType AccessType { get; set; }


        public AreaRangeType RangeType { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }

        public DateTime? DeleteTime { get; set; }

        public int CardRoleId { get; set; }

        public int AuthorizationId { get; set; }

        public bool SignIn { get; set; }
        public AreaAuthorizationRecord()
        {

        }
    }

    #endregion
    #region 历史轨迹告警
    public class AlarmSearchArgAll
    {
        public int[] personnels { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
    public class AllAlarms
    {
        public List<LocationAlarm> alarmList { get; set; }
        public List<DeviceAlarm> devAlarmList { get; set; }
    }
    #endregion

    #region 模型模糊搜索
    public class ModelInfoSearchValue
    {
        public List<ModelInfoSearchArgs> Args { get; set; }
        public int SelectIndex { get; set; }
        public bool MatchCase { get; set; }
        public bool MatchWidth { get; set; }
        public ModelInfoSearchValue(List<ModelInfoSearchArgs> childArgs,int selectIndexT,bool matchCaseT,bool matchW)
        {
            Args = new List<ModelInfoSearchArgs>();
            Args.AddRange(childArgs);
            SelectIndex = selectIndexT;
            MatchCase = matchCaseT;
            MatchWidth = matchW;
        }
    }

    public class  ModelInfoSearchArgs
    {
        public string Type { get; set; }
        public string Attribute { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
    }

    public class ModelInfoSearchResult
    {
        /// <summary>
        /// ModelId
        /// </summary>
        public List<string> ResultValue { get; set; }

        public int SelectIndex { get; set; }
    }

    #endregion
    #region 信息发送
    public class CardMsgSendInfomation
    {
        public List<string> cardCodes { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string sendInfo { get; set; }
    }
    #endregion
	 #region 区域摄像头、门禁信息

    public class AreaDoorAccessSearchArg
    {
        public int AreaId { get; set; }
        public string Keyword { get; set; }
        /// <summary>
        /// 分页
        /// </summary>
        public PageInfo Page { get; set; }
    }
    public class AreaDoorAccessInfomation
    {
        public int Total;
        public List<DevInfo> doorAccessList;

        public void SetEmpty()
        {
            if (doorAccessList != null && doorAccessList.Count == 0)
            {
                doorAccessList = null;
            }
        }
    }
	public class AreaCameraInfomation
    {
        public int TotalPageNum;
        public int TotalDevNum;
        public List<DevInfo> cameraList;

        public void SetEmpty()
        {
            if (cameraList != null && cameraList.Count == 0)
            {
                cameraList = null;
            }
        }
    }

    public class AreaDevSearchArg
    {
        public int AreaId { get; set; }

        public string Keyword { get; set; }
        /// <summary>
        /// 分页
        /// </summary>
        public PageInfo Page { get; set; }
    }

    public class AreaCameraRelationInfo
    {
        public int AreaId { get; set; }

        public List<AreaCameraRelation> RelationDevList { get; set; }
    }
    public class AreaCameraRelation
    {
        /// <summary>
        ///信息ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///区域id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        ///设备id
        /// </summary>
        public int DevId { get; set; }

        public int Index { get; set; }
        /// <summary>
        /// 摄像头信息
        /// </summary>
        public Dev_CameraInfo CameraInfo { get; set; }

        public DevInfo devInfo { get; set; }
    }
    #endregion
	#region 设备文档关联
	/// <summary>
    /// 获取所有设备pdf文档连接网页的后端(webApi)
    /// </summary>
    /// <param name="callback"></param>
    //类
    public class pdffiledata
    {
        public int state { get; set; }

        public string describe { get; set; }

        public string exception { get; set; }

        public List<CeMainFolder> data { get; set; }
    }
    /// <summary>
    /// 主文件夹连接网页的后端(webApi)
    /// </summary>
    /// <param name="callback"></param>
    //类
    public class CeMainFolder
    {
        public int id { get; set; }
        public int typeId { get; set; }

        public string typeName { get; set; }

        public string createTime { get; set; }
        public string modifyTime { get; set; }
        public string name { get; set; }
        

        public List<CeSubFolderList> ceSubFolderList { get; set; }
    }
    /// <summary>
    /// 子文件夹连接网页的后端(webApi)
    /// </summary>
    /// <param name="callback"></param>
    //类
    public class CeSubFolderList
    {
        public int id { get; set; }

        public int pid { get; set; }
        public string createTime { get; set; }
        public string modifyTime { get; set; }
        public string name { get; set; }

        public List<PdfFile> ceSubFolderFileList;


    }
    /// <summary>
    ///根据子文件夹id获取文件夹列表
    /// </summary>
    /// <param name="callback"></param>
    //类
    public class FileListdata
    {
        public int state { get; set; }

        public string describe { get; set; }

        public string exception { get; set; }

        public List<PdfFile> data { get; set; }
    }
    public class PdfFile
    {
        public int id { get; set; }

        public int pid { get; set; }
        public string createTime { get; set; }
        public string modifyTime { get; set; }
        public string name { get; set; }
        public int nOrder { get; set; }


    }
    /// <summary>
    ///刚刚从后端获取过来的类LocationDev
    /// </summary>
    /// <param name="callback"></param>
    //类
    public class LocationDevdata
    {
        public int state { get; set; }

        public string describe { get; set; }

        public string exception { get; set; }

        public CeLocationDev data { get; set; }
    }
    /// <summary>
    ///location数据库设备
    /// </summary>
    /// <param name="callback"></param>
    ///类
    public class CeLocationDev
    {
        public int? id { get; set; }
        public int devId { get; set; }
      
        public string createTime { get; set; }
        public string modifyTime { get; set; }

        public List<CeLocationDevFile> ceLocationDevFileList { get; set; }
    }


    /// <summary>
    ///设备关联文件信息
    /// </summary>
    /// <param name="callback"></param>
    ///类

    public class CeLocationDevFile
    {
        public int? id { get; set; }
        public int? pid { get; set; }
        public int fileId { get; set; }
        public string createTime { get; set; }
        public string modifyTime { get; set; }
    
    }
    /// <summary>
    /// 添加的时候没有id pid 或者传null
    /// </summary>
    public class CeAddLocationDev
    {
      
        public int devId { get; set; }

        public string createTime { get; set; }
        public string modifyTime { get; set; }

        public List<CeAddLocationDevFile> ceLocationDevFileList { get; set; }
    }

    public class CeAddLocationDevFile
    {
       
        public int fileId { get; set; }
        public string createTime { get; set; }
        public string modifyTime { get; set; }

    }
	/// <summary>
    /// 获取所有设备主资产连接网页的后端(webApi)
    /// </summary>
    /// <param name="callback"></param>
    //类
    ///MasterAsset
    public class MasterAssetdata
    {
        public int state { get; set; }

        public string describe { get; set; }

        public string exception { get; set; }

        public List<MasterAsset> data { get; set; }
    }
    public class MasterAsset
    {
        public int id { get; set; }
        public int pid { get; set; }

        public int devId { get; set; }
        public string devName { get; set; }
        public string specifications { get; set; } 
        public string manufactor { get; set; }
        public string assetCode { get; set; }
        public string locationNumber { get; set; }
        public string accessSerialNumber { get; set; }
        public string unitOfAssets { get; set; }

        public string backgroundHardwareConfiguration { get; set; }
        public string virusLibraryVersion { get; set; }
        public string ruleBaseVersion { get; set; }
        public string assetManager { get; set; }
        public string maintenanceUnit { get; set; }
        public string contactInfo { get; set; }
        public string maintenancePerson { get; set; }

        public string licensePeriod { get; set; }
        public string region { get; set; }
        public string assetStatus { get; set; }
        public string factoryNumber { get; set; }
        public string generationDate { get; set; }
        public string system { get; set; }
        public string operationTime { get; set; }
        public string networkAccessSoftwareVersion { get; set; }
        public string operateSystemConfiguration { get; set; }
        public string remark { get; set; }
        public string createTime { get; set; }
        public string modifyTime { get; set; }
        public List<CeExpansionAssetInfoOne> oneList { get; set; }
        public List<CeExpansionAssetInfoOne> twoList { get; set; }
    }
    public class CeExpansionAssetInfoOne
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string param1  { get; set; }
        public string param2  { get; set; }
        public string param3  { get; set; }
        public string param4  { get; set; }
        public string param5  { get; set; }
        public string param6  { get; set; }
        public string param7  { get; set; }
        public string param8  { get; set; }
        public string param9  { get; set; }
        public string param10  { get; set; }
        public string param11  { get; set; }
        public string param12  { get; set; }
        public string param13  { get; set; }
        public string param14  { get; set; }
        public string param15 { get; set; }
                        

        public string param16 { get; set; }
        public string param17 { get; set; }
        public string param18 { get; set; }
        public string param19 { get; set; }
        public string param20 { get; set; }

        public string createTime { get; set; }


        public string modifyTime { get; set; }



    }
	#endregion
}

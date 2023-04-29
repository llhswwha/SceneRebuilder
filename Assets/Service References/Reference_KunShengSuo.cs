using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Location.Reference_KunShengSuo
{
    #region 企业信息
    /// <summary>
    /// 获取企业信息
    /// </summary>
    [DataContract]
    public class GetCompany
    {
        //企业账号Id
        [DataMember]
        public string accountId { get; set; }

        [DataMember]
        public string accountName { get; set; }
        //企业Id
        [DataMember]
        public string companyId { get; set; }

        [DataMember]
        public string companyName { get; set; }

        [DataMember]
        public string companyCode { get; set; }

        [DataMember]
        public string contactBy { get; set; }

        [DataMember]
        public string contactEmail { get; set; }

        [DataMember]
        public string contactTel { get; set; }
    }
    #endregion

    #region A.分组信息
    /// <summary>
    /// 根据企业Id,获取分组信息列表
    /// </summary>
    [DataContract]
    public class GetBasicGroupInfo
    {
        /// <summary>
        /// 分组Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// 分组描述
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }

        [DataMember]
        public List<GetBasicGroupUserInfo> relationshipGroupUsers { get; set; }
    }
        /// <summary>
        /// 
        /// </summary>
        [DataContract]
    public class GetBasicGroupUserInfo
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string userId { get; set; }

        [DataMember]
        public string groupId { get; set; }

        [DataMember]
        public string realName { get; set; }
    }
    /// <summary>
    /// 添加分组信息
    /// </summary>
    [DataContract]
    public class AddBasicGroupInfo
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// 分组描述
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    /// <summary>
    /// 修改分组信息
    /// </summary>
     [DataContract]
    public class EditBasicGroupInfo
    {
        /// <summary>
        /// 分组Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// 分组描述
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    #endregion

    #region B.菜单信息
    /// <summary>
    /// 根据企业Id,获取菜单树信息
    /// </summary>
    [DataContract]
    public class GetMenuTreeInfo
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string icon { get; set; }

        [DataMember]
        public bool selectable { get; set; }
    }
    #endregion

    #region C.角色信息
    /// <summary>
    /// 根据企业Id,获取角色信息
    /// </summary>
    [DataContract]
    public class GetBasicRoleInfo
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [DataMember]
        public string roleId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember]
        public string roleName { get; set; }
        /// <summary>
        /// 角色说明
        /// </summary>
        [DataMember]
        public string roleDescript { get; set; }

        [DataMember]
        public List<GetBasicRoleGroupRoleInfo> relationshipGroupRoles { get; set; }
    }
    /// <summary>
    /// 角色分组信息
    /// </summary>
    public class GetBasicRoleGroupRoleInfo
    {
        /// <summary>
        /// 角色分组Id
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        [DataMember]
        public string roleId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// 分组说明
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    /// <summary>
    /// 添加角色信息
    /// </summary>
    [DataContract]
    public class AddBasicRoleInfo
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember]
        public string roleName { get; set; }
        /// <summary>
        /// 角色说明
        /// </summary>
        [DataMember]
        public string roleDescript { get; set; }
    }
    /// <summary>
    /// 修改角色信息
    /// </summary>
    [DataContract]
    public class EditBasicRoleInfo
    {
        /// <summary>
        /// 角色Id,uuid
        /// </summary>
        [DataMember]
        public string roleId;
        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember]
        public string roleName;
        /// <summary>
        /// 角色说明
        /// </summary>
        [DataMember]
        public string roleDescript;
    }
    #endregion

    #region D.人员信息
    /// <summary>
    /// 根据企业Id,获取人员列表数据
    /// </summary>
    [DataContract]
    public class GetBasicUserInfo
    {
        /// <summary>
		/// 人员Id
		/// </summary>
		[DataMember]
		public string userId { get; set; }
		/// <summary>
		/// 企业Id
		/// </summary>
		[DataMember]
		public string companyId { get; set; }
		/// <summary>
		/// 账号Id
		/// </summary>
		[DataMember]
		public string accountId { get; set; }
		/// <summary>
		/// 真实名称
		/// </summary>
		[DataMember]
		public string realName { get; set; }
		/// <summary>
		/// 别名
		/// </summary>
		[DataMember]
		public string nickName { get; set; }
		/// <summary>
		/// 是否默认
		/// </summary>
		[DataMember]
		public bool isDefault { get; set; }       
		/// <summary>
		/// 账号名称
		/// </summary>
		[DataMember]
		public string accountName { get; set; }
		/// <summary>
		/// 账号密码
		/// </summary>
		[DataMember]
		public string accountPassword { get; set; }
		[DataMember]
		public List<GetBasicDataFieldInfo> dataFieldCacheSet { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class GetBasicDataFieldInfo
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string value { get; set; }
    }
    /// <summary>
    /// 添加人员信息
    /// </summary>
    [DataContract]
    public class AddBasicUserInfo
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 真实名称
        /// </summary>
        [DataMember]
        public string realName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [DataMember]
        public bool isDefault { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// 账号密码
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    /// <summary>
    /// 编辑人员信息
    /// </summary>
    [DataContract]
    public class EditBasicUserInfo
    {
        /// <summary>
        /// 人员Id,uuid
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// 公司账号Id,uuid
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// 真实人员名称
        /// </summary>
        [DataMember]
        public string realName { get; set; }      
        /// <summary>
        /// 别名
        /// </summary>
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [DataMember]
        public bool isDefault { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// 账号密码
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    #endregion

    #region E.公司账号信息
    /// <summary>
    /// 根据企业Id,获取公司账号信息列表
    /// </summary>
    [DataContract]
    public class GetCompanyAccount
    {
         /// <summary>
        /// 公司账号Id
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 公司账号密码
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
    }
    /// <summary>
    /// 添加公司账号信息
    /// </summary>
    [DataContract]
    public class AddCompanyAccount
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 公司账号名称
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// 公司账号密码
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    /// <summary>
    /// 修改公司账号信息
    /// </summary>
    [DataContract]
    public class EditCompanyAccount
    {
        /// <summary>
        /// 账号Id，uuid
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// 公司账号名称
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// 公司账号密码
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    #endregion

    #region F.公司部门信息
    /// <summary>
    /// 根据企业Id,获取公司部门信息
    /// </summary>
    [DataContract]
    public class GetCompanyDeptInfo
    {
        /// <summary>
        /// 公司部门Id
        /// </summary>
        [DataMember]
        public string deptId { get; set; }
        /// <summary>
        /// 公司上级部门Id
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// 部门说明
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    /// <summary>
    /// 添加公司部门信息
    /// </summary>
    [DataContract]
    public class AddCompanyDeptInfo
    {
        /// <summary>
        /// 公司部门Id,uuid
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 公司部门编号
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// 公司部门名称
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// 公司部门说明
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    /// <summary>
    /// 修改公司部门信息
    /// </summary>
    [DataContract]
    public class EditCompanyDeptInfo
    {
        /// <summary>
        /// 公司部门Id,uuid
        /// </summary>
        [DataMember]
        public string deptId { get; set; }
        /// <summary>
        /// 公司上级部门Id,uuid
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// 公司部门编码
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// 公司部门名称
        /// </summary>
        [DataMember]
        public string deptName { get; set; }       
        /// <summary>
        /// 公司部门说明
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    #endregion

    #region G.设备基础信息
    /// <summary>
    /// 根据企业Id,获取设备基础信息列表
    /// </summary>
    [DataContract]
    public class GetDeviceByCompany
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 上级部门Id
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// 设备类型Id
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }
        /// <summary>
        /// 设备位置信息Id
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// 设备序号
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// 设备说明书
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// 设备厂家名称
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// 设备注册时间
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// 设备由谁注册
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
    }
    /// <summary>
    /// 添加设备基础信息
    /// </summary>
    [DataContract]
    public class AddDevice
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备类型Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// 上级企业部门Id，uuid
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// 企业部门Id,uuid 
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }
        /// <summary>
        /// 设备位置信息Id,uuid
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// 设备序号
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// 设备说明书
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// 设备厂家
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// 设备注册时间
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// 设备由谁注册
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
    }
    /// <summary>
    /// 修改设备基础信息
    /// </summary>
    [DataContract]
    public class EditDevice
    {
        /// <summary>
        /// 设备Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 设备类型Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// 上级企业部门Id,uuid
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// 企业部门Id,uuid 
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }  
        /// <summary>
        /// 设备位置信息Id,uuid
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// 设备序号
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// 设备说明书
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// 设备厂家
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// 设备注册时间
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// 设备由谁注册
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
        /// <summary>
        /// 上级企业部门名称
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// 企业部门名称
        /// </summary>
        [DataMember]
        public string applyDeptName { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// 位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 由谁更新
        /// </summary>
        [DataMember]
        public string updateBy { get; set; }
    }
    /// <summary>
    /// 根据设备Id,获取设备基础信息
    /// </summary>
    [DataContract]
    public class GetDevice
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [DataMember]
        public string companyName { get; set; }
        /// <summary>
        /// 企业部门名称
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// 企业部门名称
        /// </summary>
        [DataMember]
        public string applyDeptName { get; set; }
        /// <summary>
        /// 位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 设备序号
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// 设备说明书
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// 设备厂家名称
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// 设备注册时间
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// 设备由谁注册
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<GetDeviceDataField> dataFieldCacheSet { get; set; }

        [DataMember]
        public List<string> primaryKeyDataFieldCache { get; set; }

        [DataMember]
        public List<string> standardDataFieldCache { get; set; }

        [DataMember]
        public string cachedMappingDataTableName { get; set; }

        [DataMember]
        public int currentEntityStatus { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class GetDeviceDataField
    {
        [DataMember]
        public string key { get; set; }
        [DataMember]
        public string value { get; set; }
    }
    /// <summary>
    /// 根据设备Id,获取设备修改日志信息列表
    /// </summary>
    [DataContract]
    public class GetDeviceUpdateLog
    {
        /// <summary>
        /// 设备修改日志Id
        /// </summary>
        [DataMember]
        public string deviceUpdateId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }   
        /// <summary>
        /// 设备由谁更新
        /// </summary>
        [DataMember]
        public string deviceUpdateBy { get; set; }
        /// <summary>
        /// 设备更新内容
        /// </summary>
        [DataMember]
        public string deviceUpdateContent { get; set; }
        /// <summary>
        /// 设备更新时间
        /// </summary>
        [DataMember]
        public DateTime deviceUpdateTime { get; set; }
    }
    #endregion

    #region H.报警日志信息
    /// <summary>
    /// 根据设备属性Id和告警类型Id,获取告警信息列表
    /// </summary>
    [DataContract]
    public class GetDeviceAlarmLogInfo
    {
        /// <summary>
        /// 告警日志Id
        /// </summary>
        [DataMember]
        public string alarmId { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }  
        /// <summary>
        /// 告警类型Id
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// 告警类型名称
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// 告警时间
        /// </summary>
        [DataMember]
        public DateTime alarmTime { get; set; }
        /// <summary>
        /// 告警状态
        /// </summary>
        [DataMember]
        public int alarmStatus { get; set; }
        /// <summary>
        /// 告警描述
        /// </summary>
        [DataMember]
        public string alarmDescript { get; set; }
        /// <summary>
        /// 告警由谁取消
        /// </summary>
        [DataMember]
        public string alarmCancelBy { get; set; }
        /// <summary>
        /// 告警取消时间
        /// </summary>
        [DataMember]
        public DateTime alarmCancelTime { get; set; }
    }
    /// <summary>
    /// 添加告警日志信息
    /// </summary>
    [DataContract]
    public class AddDeviceAlarmLog
    {
        /// <summary>
        /// 告警类型Id,uuid
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// 告警时间
        /// </summary>
        [DataMember]
        public string alarmTime { get; set; }
        /// <summary>
        /// 告警描述
        /// </summary>
        [DataMember]
        public string alarmDescript { get; set; }
    }
    /// <summary>
    /// 修改告警日志信息
    /// </summary>
    [DataContract]
    public class EditDeviceAlarmLog
    {
        /// <summary>
        /// 告警日志Id，uuid
        /// </summary>
        [DataMember]
        public string alarmId { get; set; }
        /// <summary>
        /// 报警由谁取消
        /// </summary>
        [DataMember]
        public string alarmCancelBy { get; set; }
    }
    #endregion

    #region I.报警类型信息
    /// <summary>
    /// 根据企业Id,获取报警类型信息
    /// </summary>
    [DataContract]
    public class GetDeviceAlarmType
    {
        /// <summary>
        /// 告警类型Id
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }
        /// <summary>
        /// 告警类型编号
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// 告警类型名称
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// 告警类型上限
        /// </summary>
        [DataMember]
        public string alarmTypeUpper { get; set; }
        /// <summary>
        /// 告警类型下限
        /// </summary>
        [DataMember]
        public string alarmTypeLower { get; set; }
        /// <summary>
        /// 告警类型错误信息
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    /// <summary>
    /// 添加报警类型信息
    /// </summary>
    [DataContract]
    public class AddDeviceAlarmType
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备属性Id,uuid
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }
        /// <summary>
        /// 告警类型编码
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// 告警类型名称
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// 告警类型上限值
        /// </summary>
        [DataMember]
        public double alarmTypeUpper { get; set; }
        /// <summary>
        /// 告警类型下限值
        /// </summary>
        [DataMember]
        public double alarmTypeLower { get; set; }
        /// <summary>
        /// 告警类型错误信息
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    /// <summary>
    /// 修改报警类型信息
    /// </summary>
    [DataContract]
    public class EditDeviceAlarmType
    {
        /// <summary>
        /// 告警类型Id,uuid
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// 告警类型编码
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// 告警类型名称
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// 告警类型上限
        /// </summary>
        [DataMember]
        public double alarmTypeUpper { get; set; }
        /// <summary>
        /// 告警类型下限
        /// </summary>
        [DataMember]
        public double alarmTypeLower { get; set; }
        /// <summary>
        /// 告警类型错误信息
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    #endregion

    #region J.设备位置信息
    /// <summary>
    /// 根据公司Id,获取设备位置信息列表
    /// </summary>
    [DataContract]
    public class GetDeviceLocation
    {
        /// <summary>
        /// 设备位置信息Id
        /// </summary>
        [DataMember]
        public string locationId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备位置信息编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 建筑名称
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// 楼层名称
        /// </summary>
        [DataMember]
        public string floorName { get; set; }  
        /// <summary>
        /// 房间名称
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    /// <summary>
    /// 添加设备位置信息
    /// </summary>
    [DataContract]
    public class AddDeviceLocation
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 建筑名称
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// 楼层名称
        /// </summary>
        [DataMember]
        public string floorName { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    [DataContract]
    public class EditDeviceLocation
    {
        /// <summary>
        /// 设备位置信息Id
        /// </summary>
        [DataMember]
        public string locationId { get; set; }
        /// <summary>
        /// 设备位置信息编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 建筑名称
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// 楼层名称
        /// </summary>
        [DataMember]
        public string floorName { get; set; }
        /// <summary>
        /// 房间名称
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    #endregion

    #region K.设备属性信息
    /// <summary>
    /// 根据设备Id,获取设备扩展属性列表
    /// </summary>
    [DataContract]
    public class GetDeviceProperty
    {
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 设备属性名称
        /// </summary>
        [DataMember]
        public string propertyName { get; set; } 
        /// <summary>
        /// 设备属性key值
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// 设备属性value值
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// 设备属性value值单位
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// 设备属性是否动态
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }
        /// <summary>
        /// 设备属性描述
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    /// <summary>
    /// 根据设备Id,获取设备扩展属性列表（该接口有数据源点信息）
    /// </summary>
    [DataContract]
    public class GetDevicePropertyInfo
    {
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 设备属性名称
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }   
        /// <summary>
        /// 设备属性key值
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// 设备属性value值
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// 设备属性value值单位
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// 设备属性是否是动态的
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; } 
        /// <summary>
        /// 设备属性描述
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
        /// <summary>
        /// 数据源点信息
        /// </summary>
        [DataMember]
        public List<GetProtocolSourcePoints> protocolSourcePoints { get; set; }
    }
    /// <summary>
    /// 数据源点信息
    /// </summary>
    [DataContract]
    public class GetProtocolSourcePoints
    {
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string relationshipId { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// 设备属性Id
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// 添加设备扩展属性
    /// </summary>
    [DataContract]
    public class AddDeviceProperty
    {
        /// <summary>
        /// 设备Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 设备属性名称
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }
        /// <summary>
        /// 设备属性key值
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// 设备属性value值
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// 设备属性value值单位
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// 属性是否是动态
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }
        /// <summary>
        /// 属性描述
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    /// <summary>
    /// 修改设备扩展属性
    /// </summary>
    [DataContract]
    public class EditDeviceProperty
    {
        /// <summary>
        /// 设备属性Id,uuid
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }
        /// <summary>
        /// 设备属性key值
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// 设备属性value值
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// 设备属性vlaue值单位
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// 设备属性是否动态
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }  
        /// <summary>
        /// 设备属性描述
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    #endregion

    #region L.设备技术文档
    /// <summary>
    /// 根据设备Id,查询设备技术文档
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class SearchResult<T>
    {
        [DataMember]
        public T context { get; set; }
        [DataMember]
        public string messageText { get; set; }
        [DataMember]
        public string statusText { get; set; }
    }
    /// <summary>
    /// 修改设备技术文档
    /// </summary>
    [DataContract]
    public class EditDeviceTechDoc
    {
        [DataMember]
        public string docId { get; set; }

        [DataMember]
        public string docName { get; set; }

        [DataMember]
        public string revisedBy { get; set; }
    }
    #endregion

    #region M.设备类型信息
    /// <summary>
    /// 根据企业Id，获取设备类型信息列表
    /// </summary>
    [DataContract]
    public class GetDeviceType
    {
        /// <summary>
        /// 设备类型Id
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 上级设备类型Id
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// 设备类型编号
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// 设备类型描述
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    /// <summary>
    /// 添加设备类型信息
    /// </summary>
    [DataContract]
    public class AddDeviceType
    {
        /// <summary>
        /// 上级设备类型Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 设备类型编号
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// 设备类型描述
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    /// <summary>
    /// 修改设备类型信息
    /// </summary>
    [DataContract]
    public class EditDeviceType
    {
        /// <summary>
        /// 设备类型Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// 设备类型编号
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// 上级设备类型Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// 设备类型描述
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    #endregion

    #region N.巡检附件、图片、视频列表(无)
    #endregion

    #region O.巡检日志
    /// <summary>
    /// 根据企业Id,查询巡检树状数据
    /// </summary>
    [DataContract]
    public class GetInspectionLogNode
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string data { get; set; }

        [DataMember]
        public bool selectable { get; set; }

        [DataMember]
        public List<GetInspectionLogNodeChildren> children { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class GetInspectionLogNodeChildren
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string data { get; set; }

        [DataMember]
        public bool selectable { get; set; }

        [DataMember]
        public List<GetInspectionLogNodeChildren> children { get; set; }
    }
    #endregion

    #region P.巡检路径
    /// <summary>
    /// 根据企业Id,获取巡检路径信息
    /// </summary>
    [DataContract]
    public class GetInspectionPath
    {
        /// <summary>
        /// 巡检路径Id
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 企业部门Id
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// 巡检路径名称
        /// </summary>
        [DataMember]
        public string pathName { get; set; }
        /// <summary>
        /// 巡检路径类型
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// 巡检路径描述
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    /// <summary>
    /// 添加巡检路径信息
    /// </summary>
    [DataContract]
    public class AddInspectionPath
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 企业部门Id,uuid
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// 巡检路径名称
        /// </summary>
        [DataMember]
        public string pathName { get; set; }   
        /// <summary>
        /// 询价路径类型
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// 巡检路径描述
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    /// <summary>
    /// 修改巡检路径信息
    /// </summary>
    [DataContract]
    public class EditInspectionPath
    {
        /// <summary>
        /// 巡检路径Id，uuid
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// 企业部门Id，uuid
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// 巡检路径名称
        /// </summary>
        [DataMember]
        public string pathName { get; set; }
        /// <summary>
        /// 巡检路径类型
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// 巡检路径描述
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    #endregion

    #region Q.巡检点
    /// <summary>
    /// 根据路径Id,获取巡检点信息
    /// </summary>
    [DataContract]
    public class GetInspectionPoint
    {
        /// <summary>
        /// 巡检点Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 巡检路径Id
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 巡检点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 巡检点序列
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    /// <summary>
    /// 根据路径Id,获取巡检点信息
    /// </summary>
    [DataContract]
    public class AddInspectionPoint
    {
        /// <summary>
        /// 巡检路径Id,uuid
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// 设备Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 巡检点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }    
        /// <summary>
        /// 巡检点序列
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    /// <summary>
    /// 根据路径Id,获取巡检点信息
    /// </summary>
    [DataContract]
    public class EditInspectionPoint
    {
        /// <summary>
        /// 巡检点Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 设备Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// 巡检点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 巡检点序列
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    #endregion

    #region R.巡检点项目
    /// <summary>
    /// 根据巡检点Id,获取巡检点项目
    /// </summary>
    [DataContract]
    public class GetInspectionPointItem
    {
        /// <summary>
        /// 巡检点位项目Id
        /// </summary>
        [DataMember]
        public string pointItemId { get; set; }
        /// <summary>
        /// 巡检点Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 巡检点位项目编码
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// 巡检点位项目名称
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// 巡检点位项目描述
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    /// <summary>
    /// 添加巡检点项目
    /// </summary>
    [DataContract]
    public class AddInspectionPointItem
    {
        /// <summary>
        /// 巡检点Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 巡检点位项目编码
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// 巡检点位项目名称
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// 巡检点位项目描述
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    /// <summary>
    /// 修改巡检点项目
    /// </summary>
    [DataContract]
    public class EditInspectionPointItem
    {
        /// <summary>
        /// 巡检点位项目Id,uuid
        /// </summary>
        [DataMember]
        public string pointItemId { get; set; }
        /// <summary>
        /// 巡检点位项目编码
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// 巡检点位项目名称
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// 巡检点位项目描述
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    #endregion

    #region S.流程
    /// <summary>
    /// 发起流程，添加流程信息
    /// </summary>
    [DataContract]
    public class AcceptInspectionWorkflow
    {
        /// <summary>
        /// 流程Id,uuid
        /// </summary>
        [DataMember]
        public string workflowId { get; set; }
        /// <summary>
        /// 发送人，uuid
        /// </summary>
        [DataMember]
        public string sender { get; set; }
        /// <summary>
        /// 接收人，uuid
        /// </summary>
        [DataMember]
        public string receiver { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        public string messageContent { get; set; }
        /// <summary>
        /// 是否
        /// </summary>
        [DataMember]
        public bool yesOrNo { get; set; }
    }
    /// <summary>
    /// 流程转移
    /// </summary>
    [DataContract]
    public class TransferInspectionWorkflow
    {
        [DataMember]
        public string workflowId { get; set; }

        [DataMember]
        public string sender { get; set; }

        [DataMember]
        public string receiver { get; set; }

        [DataMember]
        public string messageContent { get; set; }

        [DataMember]
        public bool yesOrNo { get; set; }
    }
    /// <summary>
    /// 巡检异常日志流程派单（生成工单、备品备件出库）
    /// </summary>
    [DataContract]
    public class DealInspectionWorkflow
    {
        [DataMember]
        public string workflowId { get; set; }

        [DataMember]
        public string sender { get; set; }

        [DataMember]
        public string receiver { get; set; }

        [DataMember]
        public string messageContent { get; set; }

        [DataMember]
        public bool yesOrNo { get; set; }
    }
    /// <summary>
    /// 归档流程
    /// </summary>
    [DataContract]
    public class FileInspectionWorkflow
    {
        [DataMember]
        public string workflowId { get; set; }

        [DataMember]
        public string sender { get; set; }

        [DataMember]
        public string receiver { get; set; }

        [DataMember]
        public string messageContent { get; set; }

        [DataMember]
        public bool yesOrNo { get; set; }
    }
    #endregion

    #region T.数据源配置信息
    /// <summary>
    /// 根据企业Id,获取资源配置信息
    /// </summary>
    [DataContract]
    public class GetProtocolSource
    {
        /// <summary>
        /// 数据源配置Id
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 数据源配置名称
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// 数据源配置类型
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// 数据源配置地址
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// 数据源配置描述
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// 添加数据源配置信息
    /// </summary>
    [DataContract]
    public class AddProtocolSource
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 数据源配置名称
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// 数据源配置类型
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// 数据源配置地址
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// 数据源配置描述
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// 修改数据源配置信息
    /// </summary>
    [DataContract]
    public class EditProtocolSource
    {
        /// <summary>
        /// 数据源配置Id,uuid
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// 数据源配置名称
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// 数据源配置类型
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// 数据源配置地址
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// 数据源配置描述
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// 数据源配置类型
    /// </summary>
    [DataContract]
    public class GetSourceTypes
    {
        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        public string text { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [DataMember]
        public string value { get; set; }
    }
    #endregion

    #region U.数据源点信息
    /// <summary>
    /// 根据数据源Id，获取数据源点信息
    /// </summary>
    [DataContract]
    public class GetProtocolSourcePoint
    {
        /// <summary>
        /// 数据源点Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 数据源配置Id
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// 数据源点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 数据源点地址
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// 数据源点描述
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// 添加数据源点信息
    /// </summary>
    [DataContract]
    public class AddProtocolSourcePoint
    {
        /// <summary>
        /// 数据源配置Id，uuid
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// 数据源点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 数据源点地址
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// 数据源点描述
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// 修改数据源点信息
    /// </summary>
    [DataContract]
    public class EditProtocolSourcePoint
    {
        /// <summary>
        /// 数据源点Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 数据源点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 数据源点地址
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// 数据源点描述
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    #endregion

    #region V.设备扩展属性映射数据源点关系
    /// <summary>
    /// 根据设备属性Id,获取关联关系
    /// </summary>
    [DataContract]
    public class GetRelationshipDevicePropertyProtocolSourcePoint
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string mappingId { get; set; }
        /// <summary>
        /// 设备扩展属性Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 数据源点Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// 数据源点名称
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// 数据源点地址
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// 数据源点描述
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// 添加设备扩展属性映射数据源点关系
    /// </summary>
    [DataContract]
    public class AddRelationshipDevicePropertyProtocolSourcePoint
    {
        /// <summary>
        /// 设备属性Id,uuid
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// 数据源点Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
    }
    /// <summary>
    /// 修改设备扩展属性映射数据源点关系
    /// </summary>
    [DataContract]
    public class EditRelationshipDevicePropertyProtocolSourcePoint
    {
        /// <summary>
        /// 设备扩展属性与数据源点关联Id，uuid
        /// </summary>
        [DataMember]
        public string relationshipId { get; set; }   
        /// <summary>
        /// 数据源点Id，uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
    }
    #endregion

    #region W.角色分组
    /// <summary>
    /// 添加角色分组
    /// </summary>
    [DataContract]
    public class AddRelationshipGroupRole
    {
        /// <summary>
        /// 角色Id,uuid
        /// </summary>
        [DataMember]
        public string roleId { get; set; }   
        /// <summary>
        /// 分组Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
    }
    #endregion

    #region X.角色菜单权限
    /// <summary>
    /// 添加菜单权限
    /// </summary>
    [DataContract]
    public class DealRelationshipRolePermission
    {
        [DataMember]
        public List<SingleRelationshipRolePermission> newArr { get; set; }

        [DataMember]
        public List<SingleRelationshipRolePermission> removeArr { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SingleRelationshipRolePermission
    {
        [DataMember]
        public string roleId { get; set; }

        [DataMember]
        public string permissionId { get; set; }
    }
    #endregion

    #region Y.人员分组
    /// <summary>
    /// 根据分组Id,获取人员分组信息
    /// </summary>
    [DataContract]
    public class GetRelationshipUserGroup
    {
        /// <summary>
        /// 分组与人员关联Id
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// 人员Id
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// 分组Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// 真实名称
        /// </summary>
        [DataMember]
        public string realName { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<GetDataField> dataFieldCacheSet { get; set; }

        [DataMember]
        public List<string> primaryKeyDataFieldCache { get; set; }

        [DataMember]
        public List<string> standardDataFieldCache { get; set; }

        [DataMember]
        public string cachedMappingDataTableName { get; set; }

        [DataMember]
        public int currentEntityStatus { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class GetDataField
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string value { get; set; }
    }
    /// <summary>
    /// 添加人员分组
    /// </summary>
    [DataContract]
    public class AddRelationshipUserGroup
    {
        /// <summary>
        /// 人员Id,uuid
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// 分组Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
    }
    #endregion

    #region Z.备品备件信息
    /// <summary>
    /// 根据企业Id,获取备品备件信息
    /// </summary>
    [DataContract]
    public class GetSparePart
    {
        /// <summary>
        /// 企业Id
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 备品备件Id
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// 备品备件编码
        /// </summary>
        [DataMember]
        public string sparePartCode { get; set; }
        /// <summary>
        /// 备品备件描述
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
        /// <summary>
        /// 备品备件名称
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }
        /// <summary>
        /// 备品备件规格
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }   
        /// <summary>
        /// 备品备件类型
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }
        /// <summary>
        /// 设备位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 最少库存
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    /// <summary>
    /// 添加备品备件信息
    /// </summary>
    [DataContract]
    public class AddSparePart
    {
        /// <summary>
        /// 企业Id,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// 备品备件编码
        /// </summary>
        [DataMember]
        public string sparePartCode { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }   
        /// <summary>
        /// 最少库存
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// 备品备件名称
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }
        /// <summary>
        /// 备品备件类型
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// 备品备件规格
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }
        /// <summary>
        ///设备位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 备品备件描述
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
    }
    /// <summary>
    /// 修改备品备件信息
    /// </summary>
    [DataContract]
    public class EditSparePart
    {
        /// <summary>
        /// 备品备件Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// 最少库存
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// 备品备件名称
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }   
        /// <summary>
        /// 备品备件类型
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// 备品备件规格
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }
        /// <summary>
        /// 设备位置编码
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// 备品备件描述
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
        /// <summary>
        /// 当前库存
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    #endregion

    #region AA.出库入库信息
    /// <summary>
    /// 添加入库信息
    /// </summary>
    [DataContract]
    public class AddSparePartInWarehouse
    {
        /// <summary>
        /// 备品备件的Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// 模型信息
        /// </summary>
        [DataMember]
        public string model { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public double price { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int number { get; set; }
        /// <summary>
        /// 由谁入库
        /// </summary>
        [DataMember]
        public string inWarehouseBy { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        [DataMember]
        public string inWarehouseTime { get; set; }
        /// <summary>
        /// 入库描述
        /// </summary>
        [DataMember]
        public string inWarehouseDescript { get; set; }   
        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    /// <summary>
    /// 添加出库信息
    /// </summary>
    [DataContract]
    public class AddSparePartOutWarehouse
    {
        /// <summary>
        /// 备品备件Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        [DataMember]
        public string recipientTime { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        [DataMember]
        public string recipientor { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public int number { get; set; }
        /// <summary>
        /// 由谁出库
        /// </summary>
        [DataMember]
        public string outWarehouseBy { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        [DataMember]
        public string outWarehouseTime { get; set; }
        /// <summary>
        /// 出库描述
        /// </summary>
        [DataMember]
        public string outWarehouseDescript { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    #endregion

    #region AB.天气数据
    /// <summary>
    /// 获取天气数据
    /// </summary>
    [DataContract]
    public class GetWeatherForecast
    {
        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        public string date { get; set; }
        /// <summary>
        /// 摄氏度
        /// </summary>
        [DataMember]
        public int temperatureC { get; set; }
        /// <summary>
        /// 华氏度
        /// </summary>
        [DataMember]
        public int temperatureF { get; set; }
        /// <summary>
        /// 总结
        /// </summary>
        [DataMember]
        public string summary { get; set; }
    }
    #endregion
}

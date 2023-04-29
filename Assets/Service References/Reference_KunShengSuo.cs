using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Location.Reference_KunShengSuo
{
    #region ��ҵ��Ϣ
    /// <summary>
    /// ��ȡ��ҵ��Ϣ
    /// </summary>
    [DataContract]
    public class GetCompany
    {
        //��ҵ�˺�Id
        [DataMember]
        public string accountId { get; set; }

        [DataMember]
        public string accountName { get; set; }
        //��ҵId
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

    #region A.������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ������Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetBasicGroupInfo
    {
        /// <summary>
        /// ����Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// ��������
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
    /// ��ӷ�����Ϣ
    /// </summary>
    [DataContract]
    public class AddBasicGroupInfo
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    /// <summary>
    /// �޸ķ�����Ϣ
    /// </summary>
     [DataContract]
    public class EditBasicGroupInfo
    {
        /// <summary>
        /// ����Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    #endregion

    #region B.�˵���Ϣ
    /// <summary>
    /// ������ҵId,��ȡ�˵�����Ϣ
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

    #region C.��ɫ��Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��ɫ��Ϣ
    /// </summary>
    [DataContract]
    public class GetBasicRoleInfo
    {
        /// <summary>
        /// ��ɫId
        /// </summary>
        [DataMember]
        public string roleId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��ɫ����
        /// </summary>
        [DataMember]
        public string roleName { get; set; }
        /// <summary>
        /// ��ɫ˵��
        /// </summary>
        [DataMember]
        public string roleDescript { get; set; }

        [DataMember]
        public List<GetBasicRoleGroupRoleInfo> relationshipGroupRoles { get; set; }
    }
    /// <summary>
    /// ��ɫ������Ϣ
    /// </summary>
    public class GetBasicRoleGroupRoleInfo
    {
        /// <summary>
        /// ��ɫ����Id
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// ����Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// ��ɫId
        /// </summary>
        [DataMember]
        public string roleId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string groupName { get; set; }
        /// <summary>
        /// ����˵��
        /// </summary>
        [DataMember]
        public string groupDescript { get; set; }
    }
    /// <summary>
    /// ��ӽ�ɫ��Ϣ
    /// </summary>
    [DataContract]
    public class AddBasicRoleInfo
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��ɫ����
        /// </summary>
        [DataMember]
        public string roleName { get; set; }
        /// <summary>
        /// ��ɫ˵��
        /// </summary>
        [DataMember]
        public string roleDescript { get; set; }
    }
    /// <summary>
    /// �޸Ľ�ɫ��Ϣ
    /// </summary>
    [DataContract]
    public class EditBasicRoleInfo
    {
        /// <summary>
        /// ��ɫId,uuid
        /// </summary>
        [DataMember]
        public string roleId;
        /// <summary>
        /// ��ɫ����
        /// </summary>
        [DataMember]
        public string roleName;
        /// <summary>
        /// ��ɫ˵��
        /// </summary>
        [DataMember]
        public string roleDescript;
    }
    #endregion

    #region D.��Ա��Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��Ա�б�����
    /// </summary>
    [DataContract]
    public class GetBasicUserInfo
    {
        /// <summary>
		/// ��ԱId
		/// </summary>
		[DataMember]
		public string userId { get; set; }
		/// <summary>
		/// ��ҵId
		/// </summary>
		[DataMember]
		public string companyId { get; set; }
		/// <summary>
		/// �˺�Id
		/// </summary>
		[DataMember]
		public string accountId { get; set; }
		/// <summary>
		/// ��ʵ����
		/// </summary>
		[DataMember]
		public string realName { get; set; }
		/// <summary>
		/// ����
		/// </summary>
		[DataMember]
		public string nickName { get; set; }
		/// <summary>
		/// �Ƿ�Ĭ��
		/// </summary>
		[DataMember]
		public bool isDefault { get; set; }       
		/// <summary>
		/// �˺�����
		/// </summary>
		[DataMember]
		public string accountName { get; set; }
		/// <summary>
		/// �˺�����
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
    /// �����Ա��Ϣ
    /// </summary>
    [DataContract]
    public class AddBasicUserInfo
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��ʵ����
        /// </summary>
        [DataMember]
        public string realName { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// �Ƿ�Ĭ��
        /// </summary>
        [DataMember]
        public bool isDefault { get; set; }
        /// <summary>
        /// �˺�����
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// �˺�����
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    /// <summary>
    /// �༭��Ա��Ϣ
    /// </summary>
    [DataContract]
    public class EditBasicUserInfo
    {
        /// <summary>
        /// ��ԱId,uuid
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// ��˾�˺�Id,uuid
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// ��ʵ��Ա����
        /// </summary>
        [DataMember]
        public string realName { get; set; }      
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string nickName { get; set; }
        /// <summary>
        /// �Ƿ�Ĭ��
        /// </summary>
        [DataMember]
        public bool isDefault { get; set; }
        /// <summary>
        /// �˺�����
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// �˺�����
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    #endregion

    #region E.��˾�˺���Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��˾�˺���Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetCompanyAccount
    {
         /// <summary>
        /// ��˾�˺�Id
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��˾�˺�����
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
    }
    /// <summary>
    /// ��ӹ�˾�˺���Ϣ
    /// </summary>
    [DataContract]
    public class AddCompanyAccount
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��˾�˺�����
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// ��˾�˺�����
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    /// <summary>
    /// �޸Ĺ�˾�˺���Ϣ
    /// </summary>
    [DataContract]
    public class EditCompanyAccount
    {
        /// <summary>
        /// �˺�Id��uuid
        /// </summary>
        [DataMember]
        public string accountId { get; set; }
        /// <summary>
        /// ��˾�˺�����
        /// </summary>
        [DataMember]
        public string accountName { get; set; }
        /// <summary>
        /// ��˾�˺�����
        /// </summary>
        [DataMember]
        public string accountPassword { get; set; }
    }
    #endregion

    #region F.��˾������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��˾������Ϣ
    /// </summary>
    [DataContract]
    public class GetCompanyDeptInfo
    {
        /// <summary>
        /// ��˾����Id
        /// </summary>
        [DataMember]
        public string deptId { get; set; }
        /// <summary>
        /// ��˾�ϼ�����Id
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ���ű���
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// ����˵��
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    /// <summary>
    /// ��ӹ�˾������Ϣ
    /// </summary>
    [DataContract]
    public class AddCompanyDeptInfo
    {
        /// <summary>
        /// ��˾����Id,uuid
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��˾���ű��
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// ��˾��������
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// ��˾����˵��
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    /// <summary>
    /// �޸Ĺ�˾������Ϣ
    /// </summary>
    [DataContract]
    public class EditCompanyDeptInfo
    {
        /// <summary>
        /// ��˾����Id,uuid
        /// </summary>
        [DataMember]
        public string deptId { get; set; }
        /// <summary>
        /// ��˾�ϼ�����Id,uuid
        /// </summary>
        [DataMember]
        public string deptPId { get; set; }
        /// <summary>
        /// ��˾���ű���
        /// </summary>
        [DataMember]
        public string deptCode { get; set; }
        /// <summary>
        /// ��˾��������
        /// </summary>
        [DataMember]
        public string deptName { get; set; }       
        /// <summary>
        /// ��˾����˵��
        /// </summary>
        [DataMember]
        public string deptDescript { get; set; }
    }
    #endregion

    #region G.�豸������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ�豸������Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetDeviceByCompany
    {
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �ϼ�����Id
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// ����Id
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }
        /// <summary>
        /// �豸λ����ϢId
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// �豸���
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// �豸˵����
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// �豸״̬
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// �豸ע��ʱ��
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// �豸��˭ע��
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
    }
    /// <summary>
    /// ����豸������Ϣ
    /// </summary>
    [DataContract]
    public class AddDevice
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// �ϼ���ҵ����Id��uuid
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// ��ҵ����Id,uuid 
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }
        /// <summary>
        /// �豸λ����ϢId,uuid
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// �豸���
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// �豸˵����
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// �豸ע��ʱ��
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// �豸��˭ע��
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
    }
    /// <summary>
    /// �޸��豸������Ϣ
    /// </summary>
    [DataContract]
    public class EditDevice
    {
        /// <summary>
        /// �豸Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// �ϼ���ҵ����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceDeptId { get; set; }
        /// <summary>
        /// ��ҵ����Id,uuid 
        /// </summary>
        [DataMember]
        public string deviceApplyDeptId { get; set; }  
        /// <summary>
        /// �豸λ����ϢId,uuid
        /// </summary>
        [DataMember]
        public string deviceLocationId { get; set; }
        /// <summary>
        /// �豸���
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// �豸˵����
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// �豸״̬
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// �豸ע��ʱ��
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// �豸��˭ע��
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceDescript { get; set; }
        /// <summary>
        /// �ϼ���ҵ��������
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// ��ҵ��������
        /// </summary>
        [DataMember]
        public string applyDeptName { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��˭����
        /// </summary>
        [DataMember]
        public string updateBy { get; set; }
    }
    /// <summary>
    /// �����豸Id,��ȡ�豸������Ϣ
    /// </summary>
    [DataContract]
    public class GetDevice
    {
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// ��ҵ����
        /// </summary>
        [DataMember]
        public string companyName { get; set; }
        /// <summary>
        /// ��ҵ��������
        /// </summary>
        [DataMember]
        public string deptName { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// ��ҵ��������
        /// </summary>
        [DataMember]
        public string applyDeptName { get; set; }
        /// <summary>
        /// λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// �豸���
        /// </summary>
        [DataMember]
        public string deviceSN { get; set; }
        /// <summary>
        /// �豸����
        /// </summary>
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// �豸˵����
        /// </summary>
        [DataMember]
        public string deviceSpec { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceManufacturer { get; set; }
        /// <summary>
        /// �豸״̬
        /// </summary>
        [DataMember]
        public int deviceStatus { get; set; }
        /// <summary>
        /// �豸ע��ʱ��
        /// </summary>
        [DataMember]
        public DateTime deviceRegisterTime { get; set; }
        /// <summary>
        /// �豸��˭ע��
        /// </summary>
        [DataMember]
        public string deviceRegisterBy { get; set; }
        /// <summary>
        /// �豸����
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
    /// �����豸Id,��ȡ�豸�޸���־��Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetDeviceUpdateLog
    {
        /// <summary>
        /// �豸�޸���־Id
        /// </summary>
        [DataMember]
        public string deviceUpdateId { get; set; }
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }   
        /// <summary>
        /// �豸��˭����
        /// </summary>
        [DataMember]
        public string deviceUpdateBy { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceUpdateContent { get; set; }
        /// <summary>
        /// �豸����ʱ��
        /// </summary>
        [DataMember]
        public DateTime deviceUpdateTime { get; set; }
    }
    #endregion

    #region H.������־��Ϣ
    /// <summary>
    /// �����豸����Id�͸澯����Id,��ȡ�澯��Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetDeviceAlarmLogInfo
    {
        /// <summary>
        /// �澯��־Id
        /// </summary>
        [DataMember]
        public string alarmId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }  
        /// <summary>
        /// �澯����Id
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// �澯ʱ��
        /// </summary>
        [DataMember]
        public DateTime alarmTime { get; set; }
        /// <summary>
        /// �澯״̬
        /// </summary>
        [DataMember]
        public int alarmStatus { get; set; }
        /// <summary>
        /// �澯����
        /// </summary>
        [DataMember]
        public string alarmDescript { get; set; }
        /// <summary>
        /// �澯��˭ȡ��
        /// </summary>
        [DataMember]
        public string alarmCancelBy { get; set; }
        /// <summary>
        /// �澯ȡ��ʱ��
        /// </summary>
        [DataMember]
        public DateTime alarmCancelTime { get; set; }
    }
    /// <summary>
    /// ��Ӹ澯��־��Ϣ
    /// </summary>
    [DataContract]
    public class AddDeviceAlarmLog
    {
        /// <summary>
        /// �澯����Id,uuid
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// �澯ʱ��
        /// </summary>
        [DataMember]
        public string alarmTime { get; set; }
        /// <summary>
        /// �澯����
        /// </summary>
        [DataMember]
        public string alarmDescript { get; set; }
    }
    /// <summary>
    /// �޸ĸ澯��־��Ϣ
    /// </summary>
    [DataContract]
    public class EditDeviceAlarmLog
    {
        /// <summary>
        /// �澯��־Id��uuid
        /// </summary>
        [DataMember]
        public string alarmId { get; set; }
        /// <summary>
        /// ������˭ȡ��
        /// </summary>
        [DataMember]
        public string alarmCancelBy { get; set; }
    }
    #endregion

    #region I.����������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ����������Ϣ
    /// </summary>
    [DataContract]
    public class GetDeviceAlarmType
    {
        /// <summary>
        /// �澯����Id
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }
        /// <summary>
        /// �澯���ͱ��
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeUpper { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeLower { get; set; }
        /// <summary>
        /// �澯���ʹ�����Ϣ
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    /// <summary>
    /// ��ӱ���������Ϣ
    /// </summary>
    [DataContract]
    public class AddDeviceAlarmType
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string devicePropertyId { get; set; }
        /// <summary>
        /// �澯���ͱ���
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// �澯��������ֵ
        /// </summary>
        [DataMember]
        public double alarmTypeUpper { get; set; }
        /// <summary>
        /// �澯��������ֵ
        /// </summary>
        [DataMember]
        public double alarmTypeLower { get; set; }
        /// <summary>
        /// �澯���ʹ�����Ϣ
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    /// <summary>
    /// �޸ı���������Ϣ
    /// </summary>
    [DataContract]
    public class EditDeviceAlarmType
    {
        /// <summary>
        /// �澯����Id,uuid
        /// </summary>
        [DataMember]
        public string alarmTypeId { get; set; }
        /// <summary>
        /// �澯���ͱ���
        /// </summary>
        [DataMember]
        public string alarmTypeCode { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public string alarmTypeName { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public double alarmTypeUpper { get; set; }
        /// <summary>
        /// �澯��������
        /// </summary>
        [DataMember]
        public double alarmTypeLower { get; set; }
        /// <summary>
        /// �澯���ʹ�����Ϣ
        /// </summary>
        [DataMember]
        public string alarmTypeExpertOpinion { get; set; }
    }
    #endregion

    #region J.�豸λ����Ϣ
    /// <summary>
    /// ���ݹ�˾Id,��ȡ�豸λ����Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetDeviceLocation
    {
        /// <summary>
        /// �豸λ����ϢId
        /// </summary>
        [DataMember]
        public string locationId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸λ����Ϣ����
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// ¥������
        /// </summary>
        [DataMember]
        public string floorName { get; set; }  
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    /// <summary>
    /// ����豸λ����Ϣ
    /// </summary>
    [DataContract]
    public class AddDeviceLocation
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// ¥������
        /// </summary>
        [DataMember]
        public string floorName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    /// <summary>
    /// �޸��豸λ����Ϣ
    /// </summary>
    [DataContract]
    public class EditDeviceLocation
    {
        /// <summary>
        /// �豸λ����ϢId
        /// </summary>
        [DataMember]
        public string locationId { get; set; }
        /// <summary>
        /// �豸λ����Ϣ����
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string buildingName { get; set; }
        /// <summary>
        /// ¥������
        /// </summary>
        [DataMember]
        public string floorName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string roomName { get; set; }
    }
    #endregion

    #region K.�豸������Ϣ
    /// <summary>
    /// �����豸Id,��ȡ�豸��չ�����б�
    /// </summary>
    [DataContract]
    public class GetDeviceProperty
    {
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyName { get; set; } 
        /// <summary>
        /// �豸����keyֵ
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// �豸����valueֵ
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// �豸����valueֵ��λ
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// �豸�����Ƿ�̬
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    /// <summary>
    /// �����豸Id,��ȡ�豸��չ�����б��ýӿ�������Դ����Ϣ��
    /// </summary>
    [DataContract]
    public class GetDevicePropertyInfo
    {
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }   
        /// <summary>
        /// �豸����keyֵ
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// �豸����valueֵ
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// �豸����valueֵ��λ
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// �豸�����Ƿ��Ƕ�̬��
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; } 
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
        /// <summary>
        /// ����Դ����Ϣ
        /// </summary>
        [DataMember]
        public List<GetProtocolSourcePoints> protocolSourcePoints { get; set; }
    }
    /// <summary>
    /// ����Դ����Ϣ
    /// </summary>
    [DataContract]
    public class GetProtocolSourcePoints
    {
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string relationshipId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// ����豸��չ����
    /// </summary>
    [DataContract]
    public class AddDeviceProperty
    {
        /// <summary>
        /// �豸Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }
        /// <summary>
        /// �豸����keyֵ
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// �豸����valueֵ
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// �豸����valueֵ��λ
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// �����Ƿ��Ƕ�̬
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    /// <summary>
    /// �޸��豸��չ����
    /// </summary>
    [DataContract]
    public class EditDeviceProperty
    {
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string propertyName { get; set; }
        /// <summary>
        /// �豸����keyֵ
        /// </summary>
        [DataMember]
        public string propertyKey { get; set; }
        /// <summary>
        /// �豸����valueֵ
        /// </summary>
        [DataMember]
        public string propertyValue { get; set; }
        /// <summary>
        /// �豸����vlaueֵ��λ
        /// </summary>
        [DataMember]
        public string propertyValueUnit { get; set; }
        /// <summary>
        /// �豸�����Ƿ�̬
        /// </summary>
        [DataMember]
        public string propertyIsDynamic { get; set; }  
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string propertyDescript { get; set; }
    }
    #endregion

    #region L.�豸�����ĵ�
    /// <summary>
    /// �����豸Id,��ѯ�豸�����ĵ�
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
    /// �޸��豸�����ĵ�
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

    #region M.�豸������Ϣ
    /// <summary>
    /// ������ҵId����ȡ�豸������Ϣ�б�
    /// </summary>
    [DataContract]
    public class GetDeviceType
    {
        /// <summary>
        /// �豸����Id
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �ϼ��豸����Id
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// �豸���ͱ��
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    /// <summary>
    /// ����豸������Ϣ
    /// </summary>
    [DataContract]
    public class AddDeviceType
    {
        /// <summary>
        /// �ϼ��豸����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// �豸���ͱ��
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    /// <summary>
    /// �޸��豸������Ϣ
    /// </summary>
    [DataContract]
    public class EditDeviceType
    {
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypeId { get; set; }
        /// <summary>
        /// �豸���ͱ��
        /// </summary>
        [DataMember]
        public string deviceTypeCode { get; set; }
        /// <summary>
        /// �ϼ��豸����Id,uuid
        /// </summary>
        [DataMember]
        public string deviceTypePId { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeName { get; set; }
        /// <summary>
        /// �豸��������
        /// </summary>
        [DataMember]
        public string deviceTypeDescript { get; set; }
    }
    #endregion

    #region N.Ѳ�츽����ͼƬ����Ƶ�б�(��)
    #endregion

    #region O.Ѳ����־
    /// <summary>
    /// ������ҵId,��ѯѲ����״����
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

    #region P.Ѳ��·��
    /// <summary>
    /// ������ҵId,��ȡѲ��·����Ϣ
    /// </summary>
    [DataContract]
    public class GetInspectionPath
    {
        /// <summary>
        /// Ѳ��·��Id
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��ҵ����Id
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathName { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    /// <summary>
    /// ���Ѳ��·����Ϣ
    /// </summary>
    [DataContract]
    public class AddInspectionPath
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��ҵ����Id,uuid
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathName { get; set; }   
        /// <summary>
        /// ѯ��·������
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    /// <summary>
    /// �޸�Ѳ��·����Ϣ
    /// </summary>
    [DataContract]
    public class EditInspectionPath
    {
        /// <summary>
        /// Ѳ��·��Id��uuid
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// ��ҵ����Id��uuid
        /// </summary>
        [DataMember]
        public string responsibleDeptId { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathName { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public int pathType { get; set; }
        /// <summary>
        /// Ѳ��·������
        /// </summary>
        [DataMember]
        public string pathDescript { get; set; }
    }
    #endregion

    #region Q.Ѳ���
    /// <summary>
    /// ����·��Id,��ȡѲ�����Ϣ
    /// </summary>
    [DataContract]
    public class GetInspectionPoint
    {
        /// <summary>
        /// Ѳ���Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// Ѳ��·��Id
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// �豸Id
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    /// <summary>
    /// ����·��Id,��ȡѲ�����Ϣ
    /// </summary>
    [DataContract]
    public class AddInspectionPoint
    {
        /// <summary>
        /// Ѳ��·��Id,uuid
        /// </summary>
        [DataMember]
        public string pathId { get; set; }
        /// <summary>
        /// �豸Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }    
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    /// <summary>
    /// ����·��Id,��ȡѲ�����Ϣ
    /// </summary>
    [DataContract]
    public class EditInspectionPoint
    {
        /// <summary>
        /// Ѳ���Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// �豸Id,uuid
        /// </summary>
        [DataMember]
        public string deviceId { get; set; }
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// Ѳ�������
        /// </summary>
        [DataMember]
        public int pointSeq { get; set; }
    }
    #endregion

    #region R.Ѳ�����Ŀ
    /// <summary>
    /// ����Ѳ���Id,��ȡѲ�����Ŀ
    /// </summary>
    [DataContract]
    public class GetInspectionPointItem
    {
        /// <summary>
        /// Ѳ���λ��ĿId
        /// </summary>
        [DataMember]
        public string pointItemId { get; set; }
        /// <summary>
        /// Ѳ���Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    /// <summary>
    /// ���Ѳ�����Ŀ
    /// </summary>
    [DataContract]
    public class AddInspectionPointItem
    {
        /// <summary>
        /// Ѳ���Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    /// <summary>
    /// �޸�Ѳ�����Ŀ
    /// </summary>
    [DataContract]
    public class EditInspectionPointItem
    {
        /// <summary>
        /// Ѳ���λ��ĿId,uuid
        /// </summary>
        [DataMember]
        public string pointItemId { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemCode { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemName { get; set; }
        /// <summary>
        /// Ѳ���λ��Ŀ����
        /// </summary>
        [DataMember]
        public string pointItemDescript { get; set; }
    }
    #endregion

    #region S.����
    /// <summary>
    /// �������̣����������Ϣ
    /// </summary>
    [DataContract]
    public class AcceptInspectionWorkflow
    {
        /// <summary>
        /// ����Id,uuid
        /// </summary>
        [DataMember]
        public string workflowId { get; set; }
        /// <summary>
        /// �����ˣ�uuid
        /// </summary>
        [DataMember]
        public string sender { get; set; }
        /// <summary>
        /// �����ˣ�uuid
        /// </summary>
        [DataMember]
        public string receiver { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string messageContent { get; set; }
        /// <summary>
        /// �Ƿ�
        /// </summary>
        [DataMember]
        public bool yesOrNo { get; set; }
    }
    /// <summary>
    /// ����ת��
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
    /// Ѳ���쳣��־�����ɵ������ɹ�������Ʒ�������⣩
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
    /// �鵵����
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

    #region T.����Դ������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��Դ������Ϣ
    /// </summary>
    [DataContract]
    public class GetProtocolSource
    {
        /// <summary>
        /// ����Դ����Id
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// ����Դ���õ�ַ
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// �������Դ������Ϣ
    /// </summary>
    [DataContract]
    public class AddProtocolSource
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// ����Դ���õ�ַ
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// �޸�����Դ������Ϣ
    /// </summary>
    [DataContract]
    public class EditProtocolSource
    {
        /// <summary>
        /// ����Դ����Id,uuid
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceName { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceType { get; set; }
        /// <summary>
        /// ����Դ���õ�ַ
        /// </summary>
        [DataMember]
        public string sourceAddr { get; set; }
        /// <summary>
        /// ����Դ��������
        /// </summary>
        [DataMember]
        public string sourceDescript { get; set; }
    }
    /// <summary>
    /// ����Դ��������
    /// </summary>
    [DataContract]
    public class GetSourceTypes
    {
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string text { get; set; }
        /// <summary>
        /// ֵ
        /// </summary>
        [DataMember]
        public string value { get; set; }
    }
    #endregion

    #region U.����Դ����Ϣ
    /// <summary>
    /// ��������ԴId����ȡ����Դ����Ϣ
    /// </summary>
    [DataContract]
    public class GetProtocolSourcePoint
    {
        /// <summary>
        /// ����Դ��Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// ����Դ����Id
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// ����Դ���ַ
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// �������Դ����Ϣ
    /// </summary>
    [DataContract]
    public class AddProtocolSourcePoint
    {
        /// <summary>
        /// ����Դ����Id��uuid
        /// </summary>
        [DataMember]
        public string sourceId { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// ����Դ���ַ
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// �޸�����Դ����Ϣ
    /// </summary>
    [DataContract]
    public class EditProtocolSourcePoint
    {
        /// <summary>
        /// ����Դ��Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// ����Դ���ַ
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    #endregion

    #region V.�豸��չ����ӳ������Դ���ϵ
    /// <summary>
    /// �����豸����Id,��ȡ������ϵ
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
        /// �豸��չ����Id
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// ����Դ��Id
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointName { get; set; }
        /// <summary>
        /// ����Դ���ַ
        /// </summary>
        [DataMember]
        public string pointAddr { get; set; }
        /// <summary>
        /// ����Դ������
        /// </summary>
        [DataMember]
        public string pointDescript { get; set; }
    }
    /// <summary>
    /// ����豸��չ����ӳ������Դ���ϵ
    /// </summary>
    [DataContract]
    public class AddRelationshipDevicePropertyProtocolSourcePoint
    {
        /// <summary>
        /// �豸����Id,uuid
        /// </summary>
        [DataMember]
        public string propertyId { get; set; }
        /// <summary>
        /// ����Դ��Id,uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
    }
    /// <summary>
    /// �޸��豸��չ����ӳ������Դ���ϵ
    /// </summary>
    [DataContract]
    public class EditRelationshipDevicePropertyProtocolSourcePoint
    {
        /// <summary>
        /// �豸��չ����������Դ�����Id��uuid
        /// </summary>
        [DataMember]
        public string relationshipId { get; set; }   
        /// <summary>
        /// ����Դ��Id��uuid
        /// </summary>
        [DataMember]
        public string pointId { get; set; }
    }
    #endregion

    #region W.��ɫ����
    /// <summary>
    /// ��ӽ�ɫ����
    /// </summary>
    [DataContract]
    public class AddRelationshipGroupRole
    {
        /// <summary>
        /// ��ɫId,uuid
        /// </summary>
        [DataMember]
        public string roleId { get; set; }   
        /// <summary>
        /// ����Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
    }
    #endregion

    #region X.��ɫ�˵�Ȩ��
    /// <summary>
    /// ��Ӳ˵�Ȩ��
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

    #region Y.��Ա����
    /// <summary>
    /// ���ݷ���Id,��ȡ��Ա������Ϣ
    /// </summary>
    [DataContract]
    public class GetRelationshipUserGroup
    {
        /// <summary>
        /// ��������Ա����Id
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// ��ԱId
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// ����Id
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
        /// <summary>
        /// ��ʵ����
        /// </summary>
        [DataMember]
        public string realName { get; set; }
        /// <summary>
        /// ����
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
    /// �����Ա����
    /// </summary>
    [DataContract]
    public class AddRelationshipUserGroup
    {
        /// <summary>
        /// ��ԱId,uuid
        /// </summary>
        [DataMember]
        public string userId { get; set; }
        /// <summary>
        /// ����Id,uuid
        /// </summary>
        [DataMember]
        public string groupId { get; set; }
    }
    #endregion

    #region Z.��Ʒ������Ϣ
    /// <summary>
    /// ������ҵId,��ȡ��Ʒ������Ϣ
    /// </summary>
    [DataContract]
    public class GetSparePart
    {
        /// <summary>
        /// ��ҵId
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��Ʒ����Id
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartCode { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }
        /// <summary>
        /// ��Ʒ�������
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }   
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// ��ǰ���
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }
        /// <summary>
        /// �豸λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ���ٿ��
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// ʱ���
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    /// <summary>
    /// ��ӱ�Ʒ������Ϣ
    /// </summary>
    [DataContract]
    public class AddSparePart
    {
        /// <summary>
        /// ��ҵId,uuid
        /// </summary>
        [DataMember]
        public string companyId { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartCode { get; set; }
        /// <summary>
        /// ��ǰ���
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }   
        /// <summary>
        /// ���ٿ��
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// ��Ʒ�������
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }
        /// <summary>
        ///�豸λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
    }
    /// <summary>
    /// �޸ı�Ʒ������Ϣ
    /// </summary>
    [DataContract]
    public class EditSparePart
    {
        /// <summary>
        /// ��Ʒ����Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// ���ٿ��
        /// </summary>
        [DataMember]
        public int minimumInventory { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartName { get; set; }   
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public int sparePartType { get; set; }
        /// <summary>
        /// ��Ʒ�������
        /// </summary>
        [DataMember]
        public string sparePartSpecs { get; set; }
        /// <summary>
        /// �豸λ�ñ���
        /// </summary>
        [DataMember]
        public string locationCode { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        [DataMember]
        public string sparePartDescript { get; set; }
        /// <summary>
        /// ��ǰ���
        /// </summary>
        [DataMember]
        public int currentInventory { get; set; }
        /// <summary>
        /// ʱ���
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    #endregion

    #region AA.���������Ϣ
    /// <summary>
    /// ��������Ϣ
    /// </summary>
    [DataContract]
    public class AddSparePartInWarehouse
    {
        /// <summary>
        /// ��Ʒ������Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// ģ����Ϣ
        /// </summary>
        [DataMember]
        public string model { get; set; }
        /// <summary>
        /// �۸�
        /// </summary>
        [DataMember]
        public double price { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        [DataMember]
        public int number { get; set; }
        /// <summary>
        /// ��˭���
        /// </summary>
        [DataMember]
        public string inWarehouseBy { get; set; }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        [DataMember]
        public string inWarehouseTime { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        [DataMember]
        public string inWarehouseDescript { get; set; }   
        /// <summary>
        /// ʱ���
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    /// <summary>
    /// ��ӳ�����Ϣ
    /// </summary>
    [DataContract]
    public class AddSparePartOutWarehouse
    {
        /// <summary>
        /// ��Ʒ����Id,uuid
        /// </summary>
        [DataMember]
        public string sparePartId { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public string recipientTime { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string recipientor { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        [DataMember]
        public int number { get; set; }
        /// <summary>
        /// ��˭����
        /// </summary>
        [DataMember]
        public string outWarehouseBy { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public string outWarehouseTime { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember]
        public string outWarehouseDescript { get; set; }
        /// <summary>
        /// ʱ���
        /// </summary>
        [DataMember]
        public string timestamp { get; set; }
    }
    #endregion

    #region AB.��������
    /// <summary>
    /// ��ȡ��������
    /// </summary>
    [DataContract]
    public class GetWeatherForecast
    {
        /// <summary>
        /// ʱ��
        /// </summary>
        [DataMember]
        public string date { get; set; }
        /// <summary>
        /// ���϶�
        /// </summary>
        [DataMember]
        public int temperatureC { get; set; }
        /// <summary>
        /// ���϶�
        /// </summary>
        [DataMember]
        public int temperatureF { get; set; }
        /// <summary>
        /// �ܽ�
        /// </summary>
        [DataMember]
        public string summary { get; set; }
    }
    #endregion
}

using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Location.ReferenceEnitiys
{

    //通用返回信息类
    public class Message<T>
    {

        //结果状态 0:成功 1:失败
        public int state { get; set; }

        public string describe { get; set; }

        public string exception { get; set; }

        public string operation { get; set; }

        public T data { get; set; }

    }


    #region 卡

    /// <summary>
    /// 定位卡查询
    /// </summary>
    public class EmpTagSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 定位卡号
        /// </summary>
        public string code { get; set; }

        public EmpTagSearchArg()
        {
            pageIndex = 1;
            pageSize = 1000;
            code = "";
        }
    }

    /// <summary>
    /// 定位卡信息
    /// </summary>
    [DataContract]
    public class EmpTag
    {
        /// <summary>
        /// 卡id，数据库值
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 卡名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 卡描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 卡关联的人员名称，如果无关联，该值为null
        /// </summary>
        public string personname { get; set; }

        /// <summary>
        /// 卡关联的人员ID，如果无关联，该值为0
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// 卡电量
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// 卡电量状态
        /// </summary>
        public int PowerState { get; set; }

        public string Flag { get; set; }

        /// <summary>
        /// 卡对应的人员角色
        /// </summary>
        public int CardRoleId { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }




    #endregion

    #region 基本传参数类型

    /// <summary>
    /// 附近摄像头
    /// </summary>
    public class NearbyDevArg
    {
        public int Id { get; set; }
        /// <summary>
        /// 多少距离内的设备，毫米为单位，服务端以后都用毫米好了
        /// </summary>
        public float fDis { get; set; }
        public int nFlag { get; set; }
        /// <summary>
        /// 使用Unity尺寸计算
        /// </summary>
        public bool IsUseUnitySize { get; set; }

        //x,y,z转换系数,转换方向
        public float xSca { get; set; }
        public float ySca { get; set; }
        public float zSca { get; set; }

        //Unity中原点坐标xyz
        [DataMember]
        public float oX { get; set; }
        [DataMember]
        public float oY { get; set; }
        [DataMember]
        public float oZ { get; set; }
    }
    #endregion




    #region 基本传参数类型

    /// <summary>
    /// 在webapi Post只传字符串时使用，存在一些字符串无法用Get在路径上传（比如存在以下字符的字符串：#，/）
    /// </summary>
    public class StringArg
    {
        public string Value { get; set; }

        public StringArg(string str)
        {
            Value = str;
        }
    }

    #endregion

    #region 部门EMP获取数据实体类
    /*
     by qclei 2020-09-02
     主要是解决部门获取的新接口，支持emp数据
         
         
         */


    /// <summary>
    /// 部门查询条件
    /// </summary>
    public class EmpDepartmentSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 部门编号/部门名称
        /// </summary>
        public string departmentcode { get; set; }

        public EmpDepartmentSearchArg()
        {
            pageIndex = 1;
            pageSize = 20;
            departmentcode = "";
        }

    }


    /// <summary>
    /// 部门类
    /// </summary>
    public class EmpDepartment
    {

        public int Id { get; set; }

        public string attachsnum { get; set; }

        public string companyid { get; set; }

        public string companyname { get; set; }

        public DateTime createdate { get; set; }

        public string creater { get; set; }

        public string departmentcode { get; set; }

        public string departmentid { get; set; }

        public string departmentname { get; set; }

        public string isvalid { get; set; }//1  是  0  否

        public string lastmodifier { get; set; }

        public DateTime lastmodifydate { get; set; }

        public string officer { get; set; }

        public string parentdepartmentid { get; set; }

        public string responsibilities { get; set; }
    }

    /// <summary>
    /// 分页查询结果集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmpPageInfo<T>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int totalPage { get; set; }
        /// <summary>
        /// 页面条数（一页多少条）
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// list
        /// </summary>
        public List<T> data { get; set; }

        public EmpPageInfo()
        {
            data = new List<T>();
        }
    }

    #endregion


    #region  人员EMP获取数据实体类

    /// <summary>
    /// 人员EMP的查询条件
    /// </summary>
    public class EmpPersonnelSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 人员编号或者姓名
        /// </summary>
        public string personcode { get; set; }
        /// <summary>
        /// 是否有效 0否 1 是  -1 全部
        /// </summary>
        public int isvalid { get; set; }


        /// <summary>
        /// 是否已绑定卡（0：全部，1:绑定，2未绑定）
        /// </summary>
        public int isFilterByTag { get; set; }

        public EmpPersonnelSearchArg()
        {
            pageIndex = 1;
            pageSize = 10;
            personcode = "";
            isvalid = -1;
            isFilterByTag = 0; //由于分页获取，所有该接口一直为false
        }

    }

    /// <summary>
    /// EMP人员类
    /// </summary>
    public class EmpPersonnel
    {

        //[Display(Name = "设备本地Id")]
        public int Id { get; set; }

        //[Display(Name = "人员编号")]
        public string personnelno { get; set; }

        //[Display(Name = "人员姓名")]
        public string personnelname { get; set; }

        // [Display(Name = "所属公司")]
        public string companyname { get; set; }

        // [Display(Name = "所属部门")]
        public string departmentname { get; set; }

        // [Display(Name = "职位")]
        public string positionid { get; set; }

        // [Display(Name = "性别")]
        public string sex { get; set; }//F 女  M 男

        // [Display(Name = "出生日期")]
        public DateTime birthdate { get; set; }

        // [Display(Name = "参加工作日期")]
        public DateTime workdate { get; set; }

        //[Display(Name = "最终院校")]
        public string collage { get; set; }

        //[Display(Name = "最终专业")]
        public string professional { get; set; }

        // [Display(Name = "最终学位")]
        public string degree { get; set; }

        // [Display(Name = "婚姻状况")]
        public string maritalstatusname { get; set; }

        //[Display(Name = "政治面貌")]
        public string politicalfacename { get; set; }

        //[Display(Name = "身份证号")]
        public string idcode { get; set; }

        // [Display(Name = "家庭住址")]
        public string homeaddress { get; set; }

        //[Display(Name = "邮政编码")]
        public string postalcode { get; set; }

        //[Display(Name = "办公电话")]
        public string pfficetel { get; set; }

        //[Display(Name = "手机号码")]
        public string mobile { get; set; }

        //[Display(Name = "电子邮件")]
        public string e_mail { get; set; }

        // [Display(Name = "是否有效")]
        public string isvalid { get; set; }//1 是 0 否

        //ID
        public string personnelid { get; set; }
    }

    /// <summary>
    /// EMP人员详情
    /// </summary>
    public class EmpPersonnelDetail
    {

        // [Display(Name = "设备本地Id")]
        public int Id { get; set; }

        //   [Display(Name = "人员ID")]
        public string personnelid { get; set; }

        //   [Display(Name = "人员编号")]
        public string personnelno { get; set; }

        //    [Display(Name = "人员姓名")]
        public string personnelname { get; set; }

        //   [Display(Name = "性别")]//M 男  F  女
        public string sex { get; set; }

        //    [Display(Name = "出生日期")]
        public string birthdate { get; set; }

        //   [Display(Name = "参加工作日期")]
        public DateTime workdate { get; set; }

        //    [Display(Name = "身份证号")]
        public string idcode { get; set; }

        //   [Display(Name = "社会保障号")]
        public string insurcode { get; set; }

        //    [Display(Name = "家庭住址")]
        public string homeaddress { get; set; }

        //   [Display(Name = "邮政编码")]
        public string postalcode { get; set; }

        //    [Display(Name = "办公电话")]
        public string officetel { get; set; }

        //    [Display(Name = "手机号码")]
        public string mobile { get; set; }

        //    [Display(Name = "电子邮件")]
        public string e_mail { get; set; }

        //    [Display(Name = "是否邮件提醒")]//1  是  0  否
        public string custom3 { get; set; }

        //   [Display(Name = "是否实时提醒")]
        public string custom4 { get; set; }

        //   [Display(Name = "是否巡检员")]
        public string custom6 { get; set; }

        //   [Display(Name = "是否值长")]
        public string custom7 { get; set; }

        //    [Display(Name = "职位名称")]
        public string positionid { get; set; }

        //    [Display(Name = "第一专业")]
        public string firstprofessional { get; set; }

        //   [Display(Name = "第一学历")]
        public string firsteducation { get; set; }

        //   [Display(Name = "第一院校")]
        public string firstcollage { get; set; }

        //   [Display(Name = "最终学历")]
        public string education { get; set; }

        //   [Display(Name = "最终学位")]
        public string degree { get; set; }

        //   [Display(Name = "最终专业")]
        public string professional { get; set; }

        //    [Display(Name = "最终院校")]
        public string collage { get; set; }

        //    [Display(Name = "婚姻状况")]
        public string maritalstatusname { get; set; }

        //    [Display(Name = "政治面貌")]
        public string politicalfacename { get; set; }

        //    [Display(Name = "加入组织日期")]
        public DateTime joindate { get; set; }

        //    [Display(Name = "个人履历")]
        public string workremark { get; set; }

        //    [Display(Name = "是否有效")]
        public string isvalid { get; set; }

        //标签卡号
        public string CardCode { get; set; }

    }

    #endregion







    #region 门禁信息获取

    /// <summary>
    /// 门禁列表数据
    /// </summary>
    public class DataDoor
    {
        public int pageNo { get; set; }
        public int pageSize { get; set; }

        public List<DoorClick> list { get; set; }
        public int total { get; set; }
        public int totalPage { get; set; }

    }

    /// <summary>
    /// 门禁事件历史数据
    /// </summary>

    public class DoorClick
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 事件Id
        /// </summary>
        public string eventId { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string eventName { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime eventTime { get; set; }
        /// <summary>
        /// 发生时间戳
        /// </summary>
        public long eventTimeStmp { get; set; }
        /// <summary>
        /// 人员Id
        /// </summary>
        public string personId { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string personName { get; set; }
        /// <summary>
        /// 组织编码
        /// </summary>
        public string orgIndexCode { get; set; }
        /// <summary>
        /// 门禁名称
        /// </summary>
        public string doorName { get; set; }
        /// <summary>
        /// 门禁标识
        /// </summary>
        public string doorIndexCode { get; set; }
        /// <summary>
        /// 门禁区域标识
        /// </summary>
        public string doorRegionIndexCode { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string picUri { get; set; }
        /// <summary>
        /// 图片存储服务的唯一标识
        /// </summary>
        public string svrIndexCode { get; set; }
        /// <summary>
        /// 事件类型//198914:表示合法卡比对通过
        /// </summary>
        public int eventType { get; set; }
        /// <summary>
        /// 进出类型//1：进，0：出，-1：未知，要求：进门读卡器拨码设置为1，出门读卡器拨码设置为2
        /// </summary>
        public int inAndOutType { get; set; }
    }

    /// <summary>
    /// 门禁查询参数
    /// </summary>
    [DataContract]
    [Serializable]
    public class DoorSearchArgs
    {
        [DataMember]
        public string startTime { get; set; }
        [DataMember]
        public string endTime { get; set; }
        [DataMember]
        public int pageNo { get; set; }
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public string eventType { get; set; }/*必要(事件类型)*/
        [DataMember]
        public string[] personIds { get; set; }
        [DataMember]
        public string[] doorIndexCodes { get; set; }
        [DataMember]
        public string personName { get; set; }
    }

    /// <summary>
    /// Sis数据
    /// </summary>
    public class SisData
    {
        public int Id { get; set; }
        /// <summary>
        /// TagName
        /// </summary>
        public string Name { get; set; }
        public int Type { get; set; }
        public string Desc { get; set; }
        public string Unit { get; set; }
        public string ExDesc { get; set; }
        public int State { get; set; }
        public object ControlSystemName { get; set; }
        public string Value { get; set; }
        public int ROLEType { get; set; }
        public object Key { get; set; }
        public SisData()
        { }
    }
    #endregion

    #region SIS数据获取
    public class DevMonitorArgs
    {
        public string kks { get; set; }
        public bool bFlag { get; set; }
    }

    public class DevMonitorArgs_page : DevMonitorArgs
    {
        /// <summary>
        /// 一页条数
        /// </summary>
        [DataMember]
        public int pageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        [DataMember]
        public int pageIndex { get; set; }

    }

    [DataContract]
    public class Dev_Monitor_page : Dev_Monitor
    {
        /// <summary>
        /// 一页条数
        /// </summary>
        [DataMember]
        public int pageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        [DataMember]
        public int pageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        [DataMember]
        public int pageTotal { get; set; }

    }
    /// <summary>
    /// 测点历史数据
    /// </summary>
    public class SendHisData
    {
        /// <summary>
        /// 点名
        /// </summary>
        public List<string> tagnames { get; set; }

        public SendHisData()
        {
            tagnames = new List<string>();
        }
        /// <summary>
        /// 开始时间，格式:2019-03-19 09:35:02
        /// </summary>
        public string start { get; set; }
        /// <summary>
        /// 结束时间：2019-03-19 09:45:02
        /// </summary>
        public string end { get; set; }
        /// <summary>
        /// 间隔时间，单位秒
        /// </summary>
        public int ts { get; set; }
    }
    public class SisHisDataMH
    {
        public string tagname { get; set; }
        public List<Message> data { get; set; }
    }
    public class Message
    {
        public string status { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
        //值
        public string value { get; set; }
        public Message()
        {

        }
    }


    #endregion

    #region  仿真系统设备/驾驶舱KKS数据获取
    public class SisDataMH
    {
        public string tagname { get; set; }

        public Messages data { get; set; }
    }

    public class SisDataMH_Dev : SisDataMH
    {
        public string devName { get; set; }
        /// <summary>
        /// 设备KKS
        /// </summary>
        public string kks { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
    }

    public class Messages
    {
        public string status { get; set; }

        public string timestamp { get; set; }

        public string type { get; set; }
        //值
        public string value { get; set; }

        public Messages()
        {

        }
    }

    public class DevMonitorNode
    {
        public int Id { get; set; }//id


        public string TagName { get; set; }//标签名


        public string DbTagName { get; set; }//数据库标签名

        public string Describe { get; set; }//描述

        public string Value { get; set; }//值


        public string Unit { get; set; }//单位


        public string DataType { get; set; }//数据类型


        public string KKS { get; set; }


        public string ParentKKS { get; set; }


        public string ParseResult { get; set; }


        public long Time { get; set; }//时间戳


        public int showOrder { get; set; }//显示排序

        /// <summary>
        /// 仿真数据测点
        /// </summary>
        public string simulationTagName { get; set; }

        public string simulationValue { get; set; }//仿真数据
    }
    public class DevMonitorNodeListMH
    {
        [DataMember]
        public List<DevMonitorNode> entityList { get; set; }

        public DevMonitorNodeListMH()
        {
            entityList = new List<DevMonitorNode>();
        }
    }
    /// <summary>
    /// 驾驶舱数据
    /// </summary>
    public class CockpitDataMessage
    {
        public string flg { get; set; }

        public string msg { get; set; }

        public int total { get; set; }

        public List<CockpitData> rows { get; set; }

    }

    public class CockpitData
    {
        /// <summary>
        /// 数据名称
        /// </summary>
        public string nam { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string paraValue { get; set; }
        /// <summary>
        /// 值（此值没有实际意义，以文档单位为准）
        /// </summary>
        public string unit { get; set; }
    }
    /// <summary>
    /// 大屏人员区域统计数据
    /// </summary>
    public class AreaPersonGroup
    {
        public int Id { get; set; }

        public string name { get; set; }

        public int personCount { get; set; }
    }
    #endregion

    #region 设备资产管理

    /// <summary>
    /// 根据设备Id去获取绑定的EMP设备列表,返回类
    /// </summary>
    //public class DevMonitorNodeListMH
    //{
    //    /// <summary>
    //    /// 设备信息ID，devinfo表
    //    /// </summary>
    //    public string LocalDevId { get; set; }
    //    /// <summary>
    //    /// EMP设备kkscode，EmpDev表
    //    /// </summary>
    //    public string oldcode { get; set; }
    //    /// <summary>
    //    /// EMP设备编码，EmpDev表
    //    /// </summary>
    //    public string equipmentcode { get; set; }
    //    /// <summary>
    //    /// EMP设备名称，EmpDev表
    //    /// </summary>
    //    public string equipmentname { get; set; }
    //    /// <summary>
    //    /// EMP设备类别，EmpDev表
    //    /// </summary>
    //    public string classname { get; set; }
    //    /// <summary>
    //    /// EMP设备功能位置名称，EmpDev表
    //    /// </summary>
    //    public string funlocationname { get; set; }

    //}

    /// <summary>
    /// 设备资产
    /// </summary>
    public class EmpDevSearchArg
    {
        [DataMember]
        public int pageIndex { get; set; }
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public string equipmentcode { get; set; }//设备编号/名称/KKS码
        [DataMember]
        public string model { get; set; } //规格型号
        [DataMember]
        public string equipmentlocationcode { get; set; } //设备位号
        [DataMember]
        public string techobjectstatus { get; set; }//技术状况  此处需要传递技术状况编码//02  正常运行//03  带病运行//04   检修中//07   待报废//08   已售卖//09   已报废
        //[DataMember]
        //public string equipmentlevel { get; set; }
        //[DataMember]
        //public string abcleveltype { get; set; }
        [DataMember]
        public string isspecial { get; set; } //是否特种设备 0否 1 是
        /// <summary>
        /// 数据状态
        /// 00 初始创建
        ///02 复核通过
        ///03  复核退回
        ///04  下达使用
        ///05  冻结使用
        ///99 标注删除
        /// </summary>
        [DataMember]
        public string recordstatus { get; set; }
        /// <summary>
        /// 是否为能源设备 1 是2 否
        /// </summary>
        [DataMember]
        public string isEnergySourceEquip { get; set; }
        /// <summary>
        /// 是否为计量设备 1 是  2 否
        /// </summary>
        [DataMember]
        public string isCalCulateEquip { get; set; }
        /// <summary>
        /// ERP同步时间起始时间,格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [DataMember]
        public DateTime lastmodifydate1 { get; set; }
        /// <summary>
        /// ERP同步时间结束时间，格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [DataMember]
        public DateTime lastmodifydate2 { get; set; }






























        /// <summary>        /// 是否关联本地设备（0：全部 1：关联）        /// </summary>        [DataMember]        public int isRelation { get; set; }


        public EmpDevSearchArg()
        {

        }

        public EmpDevSearchArg(int pageIndex, int pageSize, string equipmentcode, string model, string equipmentlocationcode
            , string techobjectstatus, string isspecial, string recordstatus,
            string isEnergySourceEquip, string isCalCulateEquip, DateTime lastmodifydate1, DateTime lastmodifydate2
           )
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.equipmentcode = equipmentcode;
            this.model = model;
            this.equipmentlocationcode = equipmentlocationcode;
            this.techobjectstatus = techobjectstatus;
            this.isspecial = isspecial;
            this.recordstatus = recordstatus;
            this.isEnergySourceEquip = isEnergySourceEquip;
            this.isCalCulateEquip = isCalCulateEquip;
            this.lastmodifydate1 = lastmodifydate1;
            this.lastmodifydate2 = lastmodifydate2;
        }
    }
    /// <summary>
    /// 巡检班组列表
    /// </summary>
    public class CurrentCenter
    {
        public string codeid { get; set; }
        public string codename { get; set; }
    }
    ///// <summary>
    ///// 巡检结果目录查询
    ///// </summary>
    //public class InspectionItemsResult
    //{
    //    [DataMember]
    //    //[Display(Name = "设备本地Id")]
    //    public int Iid { get; set; }
    //    [DataMember]
    //    //[Display(Name = "")]
    //    public string equipname { get; set; }
    //    [DataMember]
    //    //[Display(Name = "")]
    //    public object[] filelist { get; set; }
    //    [DataMember]
    //    //[Display(Name = "")]
    //    public string id { get; set; }
    //    [DataMember]
    //    //[Display(Name = "项目名称")]
    //    public string itemname { get; set; }
    //    [DataMember]
    //    //[Display(Name = "巡检日期")]
    //    public long recodedate { get; set; }
    //    [DataMember]
    //    //[Display(Name = "合理范围")]
    //    public string resultscope { get; set; }
    //    [DataMember]
    //    // [Display(Name = "巡检结果")]
    //    public string resultvalue { get; set; }
    //    [DataMember]
    //    //[Display(Name = "说明")]
    //    public string remark { get; set; }

    /// <summary>
    /// 巡检操作（参数）
    /// </summary>
    public class EmpPatrolOperationSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string taskname { get; set; }
        /// <summary>
        /// 创建日期(yyyy-MM-dd)
        /// </summary>
        public string taskdate { get; set; }
        /// <summary>
        /// 班组ID
        /// </summary>
        public string currentCenterId { get; set; }
        public EmpPatrolOperationSearchArg() { }
        public EmpPatrolOperationSearchArg(int pageIndex, int pageSize, string taskname = null, string taskdate = null, string CurrentCenterId = null)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.taskname = taskname;
            this.taskdate = taskdate;
            currentCenterId = CurrentCenterId;
        }
    }
    public class EmpPatrolOperation
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string companyid { get; set; }
        [DataMember]
        public DateTime createdate { get; set; }
        [DataMember]
        public string creater { get; set; }
        [DataMember]
        public string custom1 { get; set; }
        [DataMember]
        public string Abutment_id { get; set; }
        [DataMember]
        public string ischecked { get; set; }
        [DataMember]
        public string signner { get; set; }
        [DataMember]
        public DateTime taskdate { get; set; }
        [DataMember]
        public string taskname { get; set; }
        [DataMember]
        public string taskstatus { get; set; }
        [DataMember]
        public string tasktempletid { get; set; }
        [DataMember]
        public string workcenterid { get; set; }
        [DataMember]
        public string begintime { get; set; }
        [DataMember]
        public string endtime { get; set; }
    }
    public class PageInfo<T>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int totalPage { get; set; }
        /// <summary>
        /// 页面条数（一页多少条）
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int pageIndex { get; set; }
        /// <summary>
        /// list
        /// </summary>
        public List<T> data { get; set; }
    }

    /// <summary>
    /// 设备资产信息
    /// </summary>
    public class EmpDevMore
    {
        [DataMember]
        public EmpDev empDev;

        /// <summary>
        /// 关联本地模型设备的id
        /// </summary>
        [DataMember]
        public string LocalDevId;
    }

    /// <summary>
    /// 主数据
    /// </summary>
    public class EmpDev
    {
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 设备对象ID
        /// </summary>
        [DataMember]
        public string objectid { get; set; }//
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string companyid { get; set; }//
        /// <summary>
        /// 使用公司
        /// </summary>
        [DataMember]
        public string usecompanyid { get; set; }//
        /// <summary>
        /// 维护工厂
        /// </summary>
        [DataMember]
        public string maintainplantid { get; set; } //
        /// <summary>
        /// 工厂区域
        /// </summary>
        [DataMember]
        public string plantareaid { get; set; }//
        /// <summary>
        /// 功能位置
        /// </summary>
        [DataMember]
        public string funlocationid { get; set; }//
        /// <summary>
        /// 功能位置描述
        /// </summary>
        [DataMember]
        public string funlocationname { get; set; }//
        /// <summary>
        /// 成本中心
        [DataMember]
        /// </summary>
        public string costcenterid { get; set; }//
        /// <summary>
        /// 设备编码
        /// </summary>
        [DataMember]
        public string equipmentcode { get; set; }//
        /// <summary>
        /// 设备名称
        /// </summary>
        [DataMember]
        public string equipmentname { get; set; }//
        /// <summary>
        /// 设备外文名称
        /// </summary>
        [DataMember]
        public string englishname { get; set; }//
        /// <summary>
        /// 设备拼音码
        /// </summary>
        [DataMember]
        public string pinyincode { get; set; }//
        /// <summary>
        /// 上级设备编码
        /// </summary>
        [DataMember]
        public string parentequipmentid { get; set; }//
        /// <summary>
        /// 类别分类种类
        /// </summary>
        [DataMember]
        public string classtype { get; set; }//
        /// <summary>
        /// 类别分类代码
        /// </summary>
        [DataMember]
        public string classcode { get; set; }//
        /// <summary>
        /// 资产来源
        /// </summary>
        [DataMember]
        public string sourcetype { get; set; }//
        /// <summary>
        /// 设备种类
        /// </summary>
        [DataMember]
        public string equipmenttype { get; set; }//
        /// <summary>
        /// 是否为能源设备
        /// </summary>
        [DataMember]
        public string isEnergySourceEquip { get; set; }
        /// <summary>
        /// 是否为能源设备名称
        /// </summary>
        [DataMember]
        public string isEnergySourceEquipname { get; set; }
        /// <summary>
        /// 是否为计量设备
        /// </summary>
        [DataMember]
        public string isCalCulateEquip { get; set; }
        /// <summary>
        /// ABC分类
        /// </summary>
        [DataMember]
        public string abcleveltype { get; set; } //
        /// <summary>
        /// 设备级别
        /// </summary>
        [DataMember]
        public string equipmentlevel { get; set; }//
        /// <summary>
        /// 业务范围
        /// </summary>
        [DataMember]
        public string businessscope { get; set; }
        /// <summary>
        /// 设备位号
        /// </summary>
        [DataMember]
        public string equipmentlocationcode { get; set; }
        /// <summary>
        /// 计划工厂
        /// </summary>
        [DataMember]
        public string planplantid { get; set; }//
        /// <summary>
        /// 计划人员组ID
        /// </summary>
        [DataMember]
        public string plangroupid { get; set; }//
                                               /// <summary>
                                               /// 主工作中心
                                               /// </summary>
        [DataMember]
        public string workcenterid { get; set; }
        /// <summary>
        /// 技术对象类型
        /// </summary>
        [DataMember]
        public string techobjecttype { get; set; }//
        /// <summary>
        /// 是否耗能设备
        /// </summary>
        [DataMember]
        public string isenergyused { get; set; }//
        /// <summary>
        /// 耗能种类
        /// </summary>
        [DataMember]
        public string energytype { get; set; }//
        /// <summary>
        /// 是否强检设备
        /// </summary>
        [DataMember]
        public string isenforcecheck { get; set; }//
        /// <summary>
        /// 强检类型
        /// </summary>
        [DataMember]
        public string checktype { get; set; }//
        /// <summary>
        /// 强检周期
        /// </summary>
        [DataMember]
        public string checkperiod { get; set; }
        /// <summary>
        /// 起始强检日期
        /// </summary>
        [DataMember]
        public string startcheckdate { get; set; }
        /// <summary>
        /// 下次强检日期
        /// </summary>
        [DataMember]
        public string nextcheckdate { get; set; }
        /// <summary>
        /// 强检提示前置天数
        /// </summary>
        [DataMember]
        public string beforealertdays { get; set; }//
        /// <summary>
        /// 检定证书及编号
        /// </summary>
        [DataMember]
        public string certificate { get; set; }//
        /// <summary>
        /// 是否特种设备
        /// </summary>
        [DataMember]
        public string isspecial { get; set; }//
        /// <summary>
        /// 是否危险设备
        /// </summary>
        [DataMember]
        public string isdangerous { get; set; }//
        /// <summary>
        /// 危险等级
        /// </summary>
        [DataMember]
        public string dangerouslevel { get; set; }//
                                                  /// <summary>
                                                  /// 危险分类
                                                  /// </summary>
        [DataMember]
        public string dangeroustype { get; set; }//
        /// <summary>
        /// 技术对象状态
        /// </summary>
        [DataMember]
        public string techobjectstatus { get; set; }//
        /// <summary>
        /// 规格型号
        /// </summary>
        [DataMember]
        public string model { get; set; }//
                                         /// <summary>
                                         /// 大小/尺寸
                                         /// </summary>
        [DataMember]
        public string chipsize { get; set; }//
        /// <summary>
        /// 对象重量
        /// </summary>
        [DataMember]
        public string weight { get; set; }
        /// <summary>
        /// 重量单位
        /// </summary>
        [DataMember]
        public string weightunit { get; set; }//
        /// <summary>
        /// 国家
        /// </summary>
        [DataMember]
        public string country { get; set; }//
        /// <summary>
        /// 制造厂商
        /// </summary>
        [DataMember]
        public string manufacture { get; set; }//
        /// <summary>
        /// 出厂/建造完工日期
        /// </summary>
        [DataMember]
        public string releasedate { get; set; }
        /// <summary>
        /// 出厂序列号
        /// </summary>
        [DataMember]
        public string serialno { get; set; }//
        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        public string vendorid { get; set; }//
        /// <summary>
        /// 购置价值
        /// </summary>
        public string purchasevalue { get; set; }
        /// <summary>
        /// 货币代码
        /// </summary>
        [DataMember]
        public string currency { get; set; }//
        /// <summary>
        /// 保修截止日期
        /// </summary>
        [DataMember]
        public string warrantyenddate { get; set; }
        /// <summary>
        /// 购置日期
        /// </summary>
        [DataMember]
        public string purchasedate { get; set; }
        /// <summary>
        /// 安装日期
        /// </summary>
        [DataMember]
        public string installdate { get; set; }
        /// <summary>
        /// 投用日期
        /// </summary>
        [DataMember]
        public string beginrundate { get; set; }
        /// <summary>
        /// 入资日期
        /// </summary>
        [DataMember]
        public string assertinbookdate { get; set; }
        /// <summary>
        /// 资产编号
        /// </summary>
        [DataMember]
        public string assertcode { get; set; }
        /// <summary>
        /// 资产名称
        /// </summary>
        [DataMember]
        public string assertname { get; set; }
        /// <summary>
        /// 资产原值
        /// </summary>
        [DataMember]
        public string originalvalue { get; set; }//
        /// <summary>
        /// 折旧年限
        /// </summary>
        [DataMember]
        public string depreciationyears { get; set; }
        /// <summary>
        /// 折旧方法
        /// </summary>
        [DataMember]
        public string depreciationmethod { get; set; }
        /// <summary>
        /// 折旧率
        /// </summary>
        [DataMember]
        public string depreciationrate { get; set; }//
        /// <summary>
        /// 当前净值
        /// </summary>
        [DataMember]
        public string netvalue { get; set; }//
        /// <summary>
        /// 评估原值
        /// </summary>
        [DataMember]
        public string assessoriginalvalue { get; set; }
        /// <summary>
        /// 评估净值
        /// </summary>
        [DataMember]
        public string assessnetvalue { get; set; }
        /// <summary>
        /// 是否自动折旧
        /// </summary>
        [DataMember]
        public string isautodepreciation { get; set; }//
        /// <summary>
        /// 记录状态
        /// </summary>
        [DataMember]
        public string recordstatus { get; set; }//数据状态代码
        /// <summary>
        /// 是否从项目设备导入
        /// </summary>
        [DataMember]
        public string isfromproject { get; set; }//
        /// <summary>
        /// 特种设备分类
        /// </summary>
        [DataMember]
        public string specialtype { get; set; }//
        /// <summary>
        /// 复核人
        /// </summary>
        [DataMember]
        public string checker { get; set; }
        /// <summary>
        /// 复核日期
        /// </summary>
        [DataMember]
        public string checkdate { get; set; }
        /// <summary>
        /// 供应商姓名
        /// </summary>
        [DataMember]
        public string vendor { get; set; }
        /// <summary>
        /// 技术状况
        /// </summary>
        [DataMember]
        public string techstates { get; set; }
        /// <summary>
        /// 动作类型
        /// </summary>
        [DataMember]
        public string dotype { get; set; }
        /// <summary>
        /// 公司类型编号
        /// </summary>
        [DataMember]
        public string companycode { get; set; }
        /// <summary>
        /// 公司类型
        /// </summary>
        [DataMember]
        public string companytype { get; set; }
        /// <summary>
        /// 是否增加
        /// </summary>
        [DataMember]
        public string isadd { get; set; }
        /// <summary>
        /// 旧的类别分类代码
        /// </summary>
        [DataMember]
        public string oldclasscode { get; set; }
        /// <summary>
        /// 设备分类名称
        /// </summary>
        [DataMember]
        public string classname { get; set; }//
        /// <summary>
        /// ABC分类名称
        /// </summary>
        [DataMember]
        public string abcleveltypename { get; set; }
        /// <summary>
        /// 设备种类名称
        /// </summary>
        [DataMember]
        public string equipmenttypename { get; set; }
        /// <summary>
        /// 设备级别名称
        /// </summary>
        [DataMember]
        public string equipmentlevelname { get; set; }
        /// <summary>
        /// 技术状况名称
        /// </summary>
        [DataMember]
        public string techobjectstatusname { get; set; }//
        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string vendorname { get; set; }//
        /// <summary>
        /// 维护工厂名称
        /// </summary>
        [DataMember]
        public string maintainplantname { get; set; }//
        /// <summary>
        /// 计划人员组名称
        /// </summary>
        [DataMember]
        public string plangroupname { get; set; }//
        /// <summary>
        /// 主工作中心名称
        /// </summary>
        [DataMember]
        public string workcentername { get; set; }
        /// <summary>
        /// 使用公司名称/使用单位
        /// </summary>
        [DataMember]
        public string usecompanyname { get; set; }//
        /// <summary>
        /// 公司上级ID
        /// </summary>
        [DataMember]
        public string companylevelid { get; set; }//
        /// <summary>
        /// 资产来源名称
        /// </summary>
        [DataMember]
        public string sourcetypename { get; set; }
        /// <summary>
        /// 上级设备名称
        /// </summary>
        [DataMember]
        public string parentequipmentname { get; set; }
        /// <summary>
        /// 上级设备编号
        /// </summary>
        [DataMember]
        public string parentequipmentcode { get; set; }
        /// <summary>
        /// 公司树中各节点param1
        /// </summary>
        [DataMember]
        public string companyparam1 { get; set; }
        /// <summary>
        /// 公司树节点ID
        /// </summary>
        [DataMember]
        public string companynodeid { get; set; }
        /// <summary>
        /// 工厂区域名称
        /// </summary>
        [DataMember]
        public string plantareaname { get; set; }
        /// <summary>
        /// 成本中心名称
        /// </summary>
        [DataMember]
        public string costcentername { get; set; }//
        /// <summary>
        /// 业务范围名称
        /// </summary>
        [DataMember]
        public string businessscopename { get; set; }//
        /// <summary>
        /// 计划工厂名称
        /// </summary>
        [DataMember]
        public string planplantname { get; set; }//
        /// <summary>
        /// 记录状态颜色
        /// </summary>
        [DataMember]
        public string selfdef1 { get; set; }
        /// <summary>
        /// 技术对象状态颜色
        /// </summary>
        [DataMember]
        public string techselfdef1 { get; set; }//
        /// <summary>
        /// 耗能种类名称
        /// </summary>
        [DataMember]
        public string energytypename { get; set; }
        /// <summary>
        /// 耗能系数
        /// </summary>
        [DataMember]
        public string energytyperefer { get; set; }
        /// <summary>
        /// 耗能单位
        /// </summary>
        [DataMember]
        public string energytypeunit { get; set; }
        /// <summary>
        /// 强检类型名称
        /// </summary>
        [DataMember]
        public string checktypename { get; set; }
        /// <summary>
        /// 危险等级名称
        /// </summary>
        [DataMember]
        public string dangerouslevelname { get; set; }
        /// <summary>
        /// 货币代码名称
        /// </summary>
        [DataMember]
        public string currencyname { get; set; }
        /// <summary>
        /// 危险分类名称
        /// </summary>
        [DataMember]
        public string dangeroustypename { get; set; }
        /// <summary>
        /// 重量单位名称
        /// </summary>
        [DataMember]
        public string weightunitname { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        [DataMember]
        public string countryname { get; set; }
        /// <summary>
        /// 折旧方法名称
        /// </summary>
        [DataMember]
        public string depreciationmethodname { get; set; }
        /// <summary>
        /// 记录状态名称
        /// </summary>
        [DataMember]
        public string recordstatusname { get; set; }//
        /// <summary>
        /// 记录状态颜色
        /// </summary>
        [DataMember]
        public string recordstatuscolor { get; set; }//
        /// <summary>
        /// 特种设备分类名称
        /// </summary>
        [DataMember]
        public string specialtypename { get; set; }
        /// <summary>
        /// 强检周期_年
        /// </summary>
        [DataMember]
        public string checkperiod_y { get; set; }
        /// <summary>
        /// 强检周期_月
        /// </summary>
        [DataMember]
        public string checkperiod_m { get; set; }
        /// <summary>
        /// 强检周期_日
        /// </summary>
        [DataMember]
        public string checkperiod_d { get; set; }
        /// <summary>
        /// 是否耗能设备
        /// </summary>
        [DataMember]
        public string isenergyusedname { get; set; }
        /// <summary>
        /// 是否强检设备
        /// </summary>
        [DataMember]
        public string isenforcecheckname { get; set; }
        /// <summary>
        /// 是否危险设备
        /// </summary>
        [DataMember]
        public string isdangerousname { get; set; }
        /// <summary>
        /// 是否特种设备
        /// </summary>
        [DataMember]
        public string isspecialname { get; set; }
        /// <summary>
        /// 是否自动折旧
        /// </summary>
        [DataMember]
        public string isautodepreciationname { get; set; }
        /// <summary>
        /// 是否成套设备
        /// </summary>
        [DataMember]
        public string isgroupequipname { get; set; }
        /// <summary>
        /// 是否帐内设备
        /// </summary>
        [DataMember]
        public string isaccountequipname { get; set; }
        /// <summary>
        /// 起始投用日期
        /// </summary>
        [DataMember]
        public string beginrundate_star { get; set; }
        /// <summary>
        /// 截止投用日期
        /// </summary>
        [DataMember]
        public string beginrundate_end { get; set; }
        /// <summary>
        /// 调拨费用预估
        /// </summary>
        [DataMember]
        public string changecost { get; set; }
        /// <summary>
        /// 处置价值
        /// </summary>
        [DataMember]
        public string sellvalue { get; set; }
        /// <summary>
        /// 是否含有BOM
        /// </summary>
        [DataMember]
        public string havequipbom { get; set; }
        /// <summary>
        /// 是否含有库存BOM
        /// </summary>
        [DataMember]
        public string havestorebom { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        [DataMember]
        public string materielid { get; set; }
        /// <summary>
        /// 已有编码
        /// </summary>
        [DataMember]
        public string oldcode { get; set; }//
        /// <summary>
        /// 重置价值
        /// </summary>
        [DataMember]
        public string resetvalue { get; set; }//
        /// <summary>
        /// 是否成套设备
        /// </summary>
        [DataMember]
        public string isgroupequip { get; set; }//
        /// <summary>
        /// 是否帐内设备
        /// </summary>
        [DataMember]
        public string isaccountequip { get; set; }//
        /// <summary>
        /// 设备异常名称
        /// </summary>
        [DataMember]
        public string reportname { get; set; }
        /// <summary>
        /// 设备异常类型
        /// </summary>
        [DataMember]
        public string reporttype { get; set; }
        /// <summary>
        /// 异常类型
        /// </summary>
        [DataMember]
        public string abnormaltype { get; set; }
        /// <summary>
        /// 异常开始时间1
        /// </summary>
        [DataMember]
        public string reportdate_start { get; set; }
        /// <summary>
        /// 异常开始时间2
        /// </summary>
        [DataMember]
        public string reportdate_end { get; set; }
        /// <summary>
        /// 故障开始时间1
        /// </summary>
        [DataMember]
        public string faultdate_start { get; set; }
        /// <summary>
        /// 故障开始时间2
        /// </summary>
        [DataMember]
        public string faultdate_end { get; set; }
        /// <summary>
        /// 所属公司名称
        /// </summary>
        [DataMember]
        public string companyname { get; set; }//
        /// <summary>
        /// 设备故障分类
        /// </summary>
        [DataMember]
        public string faulttype { get; set; }
        /// <summary>
        /// 过滤掉的设备ID
        /// </summary>
        [DataMember]
        public string notinobjectid { get; set; }
        /// <summary>
        /// 设备年限
        /// </summary>
        [DataMember]
        public string beginrundatenum { get; set; }
        /// <summary>
        /// 设备故障分类名称
        /// </summary>
        [DataMember]
        public string codename { get; set; }
        /// <summary>
        /// 故障次数
        /// </summary>
        [DataMember]
        public string faultnum { get; set; }
        /// <summary>
        /// 停机次数
        /// </summary>
        [DataMember]
        public string stopnum { get; set; }
        /// <summary>
        /// 维修费用
        /// </summary>
        [DataMember]
        public string maintainfee { get; set; }
        /// <summary>
        /// 平均故障时间
        /// </summary>
        [DataMember]
        public string avgfaulttime { get; set; }
        /// <summary>
        /// 平均停机时间
        /// </summary>
        [DataMember]
        public string avgstoptime { get; set; }
        /// <summary>
        /// 平均维修费用
        /// </summary>
        [DataMember]
        public string avgfee { get; set; }
        /// <summary>
        /// 运行专业
        /// </summary>
        [DataMember]
        public string runindustry { get; set; }
        /// <summary>
        /// 机组
        /// </summary>
        [DataMember]
        public string equigroup { get; set; }
        /// <summary>
        /// 运行专业名称
        /// </summary>
        [DataMember]
        public string runindustryname { get; set; }
        /// <summary>
        /// 机组名称
        /// </summary>
        [DataMember]
        public string equigroupname { get; set; }
        /// <summary>
        /// 是否提醒
        /// </summary>
        [DataMember]
        public string ischeck { get; set; }
        /// <summary>
        /// 通知单编号
        /// </summary>
        [DataMember]
        public string noticecode { get; set; }
        /// <summary>
        /// 故障开始时间
        /// </summary>
        [DataMember]
        public string faultstartdate { get; set; }
        /// <summary>
        /// 故障结束时间
        /// </summary>
        [DataMember]
        public string faultenddate { get; set; }
        /// <summary>
        /// 是否引起停机
        /// </summary>
        [DataMember]
        public string isstoped { get; set; }
        /// <summary>
        /// 故障停机时间
        /// </summary>
        [DataMember]
        public string totalstoptime { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        [DataMember]
        public string faultinfo { get; set; }
        /// <summary>
        /// 已计提月份
        /// </summary>
        [DataMember]
        public string depreciabledmonths { get; set; }//
        /// <summary>
        /// 净残值率
        /// </summary>
        [DataMember]
        public string residualrate { get; set; }//
        /// <summary>
        /// 设备残值
        /// </summary>
        [DataMember]
        public string residualvalue { get; set; }//
        /// <summary>
        /// 资产月份
        /// </summary>
        [DataMember]
        public string assetmonth { get; set; }
        /// <summary>
        /// 保管部门
        /// </summary>
        [DataMember]
        public string managedepartment { get; set; }
        /// <summary>
        /// 保管部门名称
        /// </summary>
        [DataMember]
        public string managedepartmentname { get; set; }
        /// <summary>
        /// 入资日期1
        /// </summary>
        [DataMember]
        public string assertinbookdate1 { get; set; }
        /// <summary>
        /// 入资日期2
        /// </summary>
        [DataMember]
        public string assertinbookdate2 { get; set; }
        /// <summary>
        /// 资产原值1
        /// </summary>
        [DataMember]
        public string originalvalue1 { get; set; }
        /// <summary>
        /// 资产原值2
        /// </summary>
        [DataMember]
        public string originalvalue2 { get; set; }
        /// <summary>
        /// 重评估前净值
        /// </summary>
        [DataMember]
        public string equiporiginalvalue { get; set; }
        /// <summary>
        /// 重估值
        /// </summary>
        [DataMember]
        public string estimatedvalue { get; set; }
        /// <summary>
        /// 增减值
        /// </summary>
        [DataMember]
        public string differencevalue { get; set; }
        /// <summary>
        /// 重估日期
        /// </summary>
        [DataMember]
        public string inaccountdate { get; set; }
        /// <summary>
        /// 折旧年限
        /// </summary>
        [DataMember]
        public string equipdepreciationyears { get; set; }
        /// <summary>
        /// 残值率
        /// </summary>
        [DataMember]
        public string equipresidualrate { get; set; }
        /// <summary>
        /// 报废日期
        /// </summary>
        [DataMember]
        public string scrapdate { get; set; }
        /// <summary>
        /// 剩余天数
        /// </summary>
        [DataMember]
        public string lastdays { get; set; }
        /// <summary>
        /// 上级功能位置
        /// </summary>
        [DataMember]
        public string parentfunlocationid { get; set; }//
        /// <summary>
        /// 3=代表设备,1=代表功能位置
        /// </summary>
        [DataMember]
        public string iscomponent { get; set; }//
        /// <summary>
        /// 设备组ID
        /// </summary>
        [DataMember]
        public string equipgroupid { get; set; }
        /// <summary>
        /// 设备原值查询条件: 1有值  0无值  2全部
        /// </summary>
        [DataMember]
        public string isoriginalvalue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime createdate { get; set; }//
        [DataMember]
        public string creater { get; set; }//
        [DataMember]
        public string lastmodifier { get; set; }//
        [DataMember]
        public DateTime lastmodifydate { get; set; }//ERP同步时间
    }
    /// <summary>
    /// 设备特性
    /// </summary>
    [DataContract]
    public class EmpDevCharacteristic
    {
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 设备特性
        /// </summary>
        [DataMember]
        public string attributecode { get; set; }
        /// <summary>
        /// 特性名称
        /// </summary>
        [DataMember]
        public string attributename { get; set; }
        /// <summary>
        /// 特性参数单位
        /// </summary>
        [DataMember]
        public string paramunit { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember]
        public string datatype { get; set; }
        /// <summary>
        /// 特性值长度
        /// </summary>
        [DataMember]
        public string datalength { get; set; }
        /// <summary>
        /// 特性值小数位
        /// </summary>
        [DataMember]
        public string datadecimalnum { get; set; }
        /// <summary>
        /// 输入模板
        /// </summary>
        //public string dataformat { get; set; }
        /// <summary>
        /// 是否允许负值
        /// </summary>
        //public string isnegative { get; set; }

        //是否允许区间值(isinterval =1 的为区间字段)
        [DataMember]
        public string isinterval { get; set; }
        /// <summary>
        /// 是否允许多值
        /// </summary>
        [DataMember]
        public string ismany { get; set; }
        /// <summary>
        /// 助记码
        /// </summary>
        //public string pinyincode { get; set; }
        /// <summary>
        /// 特性状态
        /// </summary>
        //public string attributestatus { get; set; }
        /// <summary>
        /// 设备对象ID
        /// </summary>
        [DataMember]
        public string objectid { get; set; }
        /// <summary>
        /// 多值序号
        /// </summary>
        [DataMember]
        public string serialno { get; set; }
        /// <summary>
        /// 特性值1
        /// </summary>
        [DataMember]
        public string paramvalue1 { get; set; }
        /// <summary>
        /// 特性值2
        /// </summary>
        [DataMember]
        public string paramvalue2 { get; set; }

    }

    public class DevInfoToEmpDevArg
    {
        /// <summary>
        /// 本地设备id
        /// </summary>
        public string LocalDevId { get; set; }

        /// <summary>
        /// EMP设备KKS编码
        /// </summary>
        public string EMPDevKKScode { get; set; }
    }

    [DataContract]
    [Serializable]
    public class DevInfoToEmpDevListArg
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [DataMember]
        public string LocalDevId { get; set; }

        /// <summary>
        /// EMP设备KKS编码
        /// </summary>
        [DataMember]
        public List<string> EMPDevKKScodes { get; set; }
    }

    /// <summary>
    /// 设备BOM
    /// </summary>
    [DataContract]
    [Serializable]
    public class DevBOM
    {
        /// <summary>
        /// 基本单位
        /// </summary>
        public string basicunit { get; set; }
        /// <summary>
        /// 基本单位名称
        /// </summary>
        public string basicunitname { get; set; }
        /// <summary>
        /// 配备数量
        /// </summary>
        public string equipnumber { get; set; }
        /// <summary>
        /// 是否库存物料 1 是  0 否
        /// </summary>
        public string isstockmateriel { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string materielcode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string materielname { get; set; }
        /// <summary>
        /// 设备主数据ID
        /// </summary>
        public string objectid { get; set; }
    }
    /// <summary>
    /// 附件和工具
    /// </summary>
    [DataContract]
    [Serializable]
    public class DevEnclosureAndTool
    {
        /// <summary>
        /// 设备主数据ID
        /// </summary>
        public string objectid { get; set; }
        /// <summary>
        /// 工器具ID
        /// </summary>
        public string toolsid { get; set; }
        /// <summary>
        /// 工器具编码
        /// </summary>
        public string toolscode { get; set; }
        /// <summary>
        /// 工器具名称
        /// </summary>
        public string toolsname { get; set; }
        /// <summary>
        /// 基本单位
        /// </summary>
        public string basicunit { get; set; }
        /// <summary>
        /// 配备数量
        /// </summary>
        public string equipnumber { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public string recordstatus { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string createdate { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string creater { get; set; }
        /// <summary>
        /// 最近修改日期
        /// </summary>
        public DateTime lastmodifydate { get; set; }
        /// <summary>
        /// 最近修改者
        /// </summary>
        public string lastmodifier { get; set; }
        /// <summary>
        /// 基本单位名称
        /// </summary>
        public string basicunitname { get; set; }
        /// <summary>
        /// 记录状态名称
        /// </summary>
        public string datastatusname { get; set; }
        /// <summary>
        /// 记录状态颜色
        /// </summary>
        public string selfdef1 { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string companyid { get; set; }
    }
    /// <summary>
    /// 设备资产其他模块
    /// </summary>
    public class EmpDevOthersSerchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 设备id（对接的ID，不是本地ID）
        /// </summary>
        public string equipmentid { get; set; }
    }
    [DataContract]
    [Serializable]
    public class DevOthers
    {
        [DataMember]
        //[Display(Name = "报警下限")]
        public string alertlowerlimit { get; set; }
        [DataMember]
        //[Display(Name = "报警上限")]
        public string alertupperlimit { get; set; }
        [DataMember]
        //[Display(Name = "计量特性名称")]
        public string attributename { get; set; }
        [DataMember]
        //[Display(Name = "自动创建工单")]//1 是  0 否
        public string isautocreate { get; set; }
        [DataMember]
        //[Display(Name = "操作下限")]
        public int operlowerlimit { get; set; }
        [DataMember]
        //[Display(Name = "操作上限")]
        public int operupperlimit { get; set; }
        [DataMember]
        //[Display(Name = "单位")]
        public string paramunit { get; set; }
        [DataMember]
        //[Display(Name = "计量点名称")]
        public string pointname { get; set; }
        [DataMember]
        //[Display(Name = "计量点位置")]
        public string pointposition { get; set; }
        [DataMember]
        //[Display(Name = "计量点编号")]
        public string pointcode { get; set; }
        [DataMember]
        //[Display(Name = "计量仪器名称")]
        public string toolname { get; set; }
    }
    #endregion

    #region 两票
    /// <summary>
    /// 工作票查询类
    /// </summary>
    public class EmpBusWorkTicketSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 工作票编号
        /// </summary>
        public string ticketcode { get; set; }
        /// <summary>
        /// 工作票类型
        /// </summary>
        public string tickettype { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        public string workcontent { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string workmanagername { get; set; }
        /// <summary>
        /// 签发人
        /// </summary>
        public string issuer { get; set; }
        /// <summary>
        /// 工作许可人
        /// </summary>
        public string licenser { get; set; }
        /// <summary>
        /// 许可人值
        /// </summary>
        public string licensorteam { get; set; }
        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime begindate { get; set; }
        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime enddate { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string ticketstatus { get; set; }
        /// <summary>
        /// 班组ID
        /// </summary>
        public string workcenterid { get; set; }
        //  public string status { get; set; }
        /// <summary>
        /// 工作票编号或工作负责人
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 工作票状态(执行中: 1 (ticketstatus in (00 01 02 03 04 05 06))  完成/关闭: 0( 07 99)  全部: 空字符串 )
        /// </summary>
        public string staus { get; set; }
    }
    /// <summary>
    /// 工作票列表
    /// </summary>
    [DataContract]
    public class BusWorkTicket
    {
        [DataMember]
        //[Display(Name = "设备本地Id")]
        public int Id { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string companyid { get; set; }
        [DataMember]
        //[Display(Name = "创建日期")]
        public DateTime createdate { get; set; }
        [DataMember]
        //[Display(Name = "申请人")]
        public string creater { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string custom6 { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public DateTime lastmodifydate { get; set; }
        [DataMember]
        //[Display(Name = "工作许可时间")]
        public DateTime licensetime { get; set; }
        [DataMember]
        //[Display(Name = "工单编号")]
        public string ordercode { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string orderid { get; set; }
        [DataMember]
        //[Display(Name = "工单名称(标题)")]
        public string ordertitle { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public object[] safetyAllMeaurs { get; set; }
        [DataMember]
        //[Display(Name = "工作票编号")]
        public string ticketcode { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string ticketid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string ticketstatus { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string ticketstatuscolor { get; set; }
        [DataMember]
        //[Display(Name = "流程状态")]
        public string ticketstatusname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string tickettype { get; set; }
        [DataMember]
        //[Display(Name = "工作票类型")]
        public string tickettypename { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string workcenterid { get; set; }
        [DataMember]
        //[Display(Name = "工作内容")]
        public string workcontent { get; set; }
        [DataMember]
        //[Display(Name = "工作负责人")]
        public string workmanagername { get; set; }
        [DataMember]
        //[Display(Name = "工作流状态")]
        public string wfticketstatusname { get; set; }
        [DataMember]
        //[Display(Name = "流程")]
        public string wfinstanceid { get; set; }
        [DataMember]
        //[Display(Name = "签发人")]
        public string issuer { get; set; }
        [DataMember]
        //[Display(Name = "工作许可人")]
        public string licenser { get; set; }
        [DataMember]
        //[Display(Name = "检修班组")]
        public string workcentername { get; set; }
        [DataMember]
        //[Display(Name = "许可人值")]
        public string licensorteam { get; set; }
        [DataMember]
        //[Display(Name = "终结许可人")]
        public string endlicenser { get; set; }
        [DataMember]
        //[Display(Name = "终结许可人值")]
        public string endlicensorteam { get; set; }
    }
    /// <summary>
    /// 操作票查询类
    /// </summary>
    [Serializable]
    public class EmpOperateticketSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 操作票编号/任务
        /// </summary>
        public string ticketcode { get; set; }
        /// <summary>
        /// 机组(机组编号)
        /// </summary>
        public string unitid { get; set; }
        /// <summary>
        /// 设备主数据ID
        /// </summary>
        public string objectid { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string equipmentcode { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operatepersonal { get; set; }
        /// <summary>
        /// 操作票类型
        /// </summary>
        public string tickettype { get; set; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public string ticketstatus { get; set; }
        /// <summary>
        /// 班组编号
        /// </summary>
        public string workcenterid { get; set; }
        /// <summary>
        /// 监护人
        /// </summary>
        public string guarder { get; set; }
        /// <summary>
        /// 创建开始日期（yyyy-MM-dd）
        /// </summary>
        public DateTime begincreatedate { get; set; }
        /// <summary>
        /// 创建结束日期(yyyy-MM-dd)
        /// </summary>
        public DateTime endcreatedate { get; set; }
        /// <summary>
        /// 操作票编号或操作人
        /// </summary>
        public string value { get; set; }
    }
    /// <summary>
    /// 操作票列表
    /// </summary>
    [DataContract]
    public class EmpOperateticket
    {
        [DataMember]
        //[Display(Name = "本地ID")]
        public int Id { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public int attachs { get; set; }
        [DataMember]
        //[Display(Name = "创建日期")]
        public DateTime createdate { get; set; }
        [DataMember]
        //[Display(Name = "创建人")]
        public string creater { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public int custom6 { get; set; }
        [DataMember]
        //[Display(Name = "票ID")]
        public string operateticketid { get; set; }
        [DataMember]
        //[Display(Name = "操作人")]
        public string operatepersonal { get; set; }
        [DataMember]
        //[Display(Name = "操作票编号")]
        public string ticketcode { get; set; }
        [DataMember]
        //[Display(Name = "操作任务")]
        public string ticketname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string ticketstatus { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string ticketstatuscolor { get; set; }
        [DataMember]
        //[Display(Name = "操作票状态")]
        public string ticketstatusname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string tickettype { get; set; }
        [DataMember]
        //[Display(Name = "操作票类型")]
        public string tickettypename { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string unitid { get; set; }
        [DataMember]
        //[Display(Name = "机组")]
        public string unitidname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string workcenterid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string workcenteridname { get; set; }
        [DataMember]
        //[Display(Name = "设备编号")]
        public string equipmentcode { get; set; }
        [DataMember]
        //[Display(Name = "设备名称")]
        public string equipmentname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string guarder { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string objectid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string onduty { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string remark { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string wfinstanceid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string workticketid { get; set; }
        [DataMember]
        //[Display(Name = "工作票编号")]
        public string workticketcode { get; set; }
    }
    ///// <summary>
    ///// 班组列表
    ///// </summary>
    //public class CurrentCenter
    //{
    //    public string codeid { get; set; }
    //    public string codename { get; set; }
    //}
    /// <summary>
    /// 操作票详情数据
    /// </summary>
    public class OperateticketDetail
    {
        [DataMember]
        //[Display(Name = "操作票记录ID")]
        public string operateticketid { get; set; }
        [DataMember]
        //[Display(Name = "设备主数据ID")]
        public string objectid { get; set; }
        [DataMember]
        //[Display(Name = "关联工作票ID")]
        public string workticketid { get; set; }
        [DataMember]
        //[Display(Name = "操作票类型")]
        public string tickettype { get; set; }
        [DataMember]
        //[Display(Name = "操作票编号")]
        public string ticketcode { get; set; }
        [DataMember]
        //[Display(Name = "操作票名称")]
        public string ticketname { get; set; }
        [DataMember]
        //[Display(Name = "所属公司")]
        public string companyid { get; set; }
        [DataMember]
        //[Display(Name = "机组")]
        public string unitid { get; set; }
        [DataMember]
        //[Display(Name = "操作单位 运行部")]
        public string workdepartment { get; set; }
        [DataMember]
        //[Display(Name = "工作中心 运行班组")]
        public string workcenterid { get; set; }
        [DataMember]
        //[Display(Name = "操作人")]
        public string operatepersonal { get; set; }
        [DataMember]
        //[Display(Name = "操作任务")]
        public string operatetask { get; set; }
        [DataMember]
        //[Display(Name = "操作开始日期")]
        public DateTime begindate { get; set; }
        [DataMember]
        //[Display(Name = "操作结束日期")]
        public DateTime enddate { get; set; }
        [DataMember]
        //[Display(Name = "监护人")]
        public string guarder { get; set; }
        [DataMember]
        //[Display(Name = "值长")]
        public string onduty { get; set; }
        [DataMember]
        //[Display(Name = "创建者")]
        public string creater { get; set; }
        [DataMember]
        //[Display(Name = "创建日期")]
        public DateTime createdate { get; set; }
        [DataMember]
        // [Display(Name = "最近修改者")]
        public string lastmodifier { get; set; }
        [DataMember]
        //[Display(Name = "最近修改日期")]
        public DateTime lastmodifydate { get; set; }
        [DataMember]
        //[Display(Name = "操作票状态")]
        public string ticketstatus { get; set; }
        [DataMember]
        // [Display(Name = "备注")]
        public string remark { get; set; }
        [DataMember]
        // [Display(Name = "操作票状态")]
        public string ticketstatusname { get; set; }
        [DataMember]
        //[Display(Name = "设备名称")]
        public string equipmentname { get; set; }
        [DataMember]
        //[Display(Name = "设备编号")]
        public string equipmentcode { get; set; }
        [DataMember]
        //[Display(Name = "操作票类型")]
        public string tickettypename { get; set; }
        [DataMember]
        //[Display(Name = "工作票编号")]
        public string workticketcode { get; set; }
        [DataMember]
        //[Display(Name = "操作票颜色")]
        public string ticketstatuscolor { get; set; }
        [DataMember]
        //[Display(Name = "作废原因")]
        public string voidreason { get; set; }
        [DataMember]
        //[Display(Name = "关闭原因")]
        public string closeremark { get; set; }
        [DataMember]
        //[Display(Name = "操作方式（启停方式）")]
        public string operatmode { get; set; }
        [DataMember]
        //[Display(Name = "NOSA防护用品编号")]
        public string protectarticles { get; set; }
        [DataMember]
        //[Display(Name = "NOSA防护用品名称")]
        public string protectarticlesname { get; set; }
        [DataMember]
        //[Display(Name = "操作人")]
        public string signatureapplicant { get; set; }
        [DataMember]
        //[Display(Name = "操作时间")]
        public DateTime applicantdate { get; set; }
        [DataMember]
        //[Display(Name = "监护人")]
        public string guardiansignature { get; set; }
        [DataMember]
        //[Display(Name = "监护时间")]
        public DateTime guardiandate { get; set; }
        [DataMember]
        //[Display(Name = "值长(控长)")]
        public string executivesignature { get; set; }
        [DataMember]
        //[Display(Name = "值长(控长)时间")]
        public DateTime executordate { get; set; }
        [DataMember]
        //[Display(Name = "操作票操作任务明细")]
        public string itemBeansString1 { get; set; }//json
        [DataMember]
        //[Display(Name = "操作票危险点明细")]
        public string itemBeansString2 { get; set; }//json
        public List<ItemBeans1> itemBeans1 { get; set; }

        public List<ItemBeans2> itemBeans2 { get; set; }
    }
    /// <summary>
    /// 操作票操作任务明细
    /// </summary>
    public class ItemBeans1
    {
        /// <summary>
        /// 操作票ID
        /// </summary>
        public string operateticketid { get; set; }
        /// <summary>
        /// 明细项ID
        /// </summary>
        public string itemid { get; set; }
        /// <summary>
        /// 明细项编号
        /// </summary>
        public string itemcode { get; set; }
        /// <summary>
        /// 操作任务
        /// </summary>
        public string operatetask { get; set; }
        /// <summary>
        /// 地点
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string operater { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string operatetime { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public string iscompleted { get; set; }  //1 是  0 否
        /// <summary>
        /// 所属公司
        /// </summary>
        public string companyid { get; set; }
        /// <summary>
        /// 命令人
        /// </summary>
        public string commander { get; set; }
        /// <summary>
        /// 恢复命令人
        /// </summary>
        public string restorecommander { get; set; }
        /// <summary>
        /// 恢复操作人
        /// </summary>
        public string recoveryoperator { get; set; }
        /// <summary>
        /// 是否需要恢复
        /// </summary>
        public string whether { get; set; } //1 是  0 否
        /// <summary>
        /// 设备id
        /// </summary>
        public string objectid { get; set; }
        /// <summary>
        /// 策略名称
        /// </summary>
        public string taskname { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string equipmentname { get; set; }
    }
    /// <summary>
    /// 操作票危险点明细
    /// </summary>
    public class ItemBeans2
    {
        /// <summary>
        /// 安全措施ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 操作票ID
        /// </summary>
        public string tiketId { get; set; }
        /// <summary>
        /// 措施类型 
        ///1应拉断路器（开关）、隔离开关（刀闸）
        ///2应装接地线、应合接地刀闸（注明确实地点、名称及接地线编号）
        ///3应设遮拦、应挂标示牌及防止二次回路误碰等措施
        ///4工作地点保留带电部分或注意事项    5.危险点分析及控制措施
        ///6.安全措施
        ///7.补充安全措施
        ///8.由检修人员执行的安全措施
        ///9.由运行人员执行的安全措施
        ///10.生产运行人员补充安全措施
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 措施内容
        /// </summary>
        public string measuresContent { get; set; }
        /// <summary>
        /// 危险点
        /// </summary>
        public string dangerPont { get; set; }
        /// <summary>
        /// 执行人签名
        /// </summary>
        public string executorSignStr { get; set; }
        /// <summary>
        /// 解除人签名
        /// </summary>
        public string cancellerSignStr { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public string createDate { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string serialNum { get; set; }
    }
    #endregion

    #region 闵行两票
    public class TicketModifyInfo
    {
        //票id
        public int ticketId { get; set; }

        //票类型  0：工作票  1：操作票  2：辅票
        public int ticketType { get; set; }

        //票操作人id
        public int personId { get; set; }
        //操作人员工号
        public string personWorkNum { get; set; }
        //票操作人员名称
        public string personName { get; set; }

        public CeYesWorkTicket workTicketInfo;

        public void SetWorkTicketInfo(CeYesWorkTicket ticket,string nameT,string workNum)
        {
            workTicketInfo = ticket;
            workTicketInfo.ttkperDsc = nameT;
            workTicketInfo.ttkDscId = workNum;
        }
    }
    /// <summary>
    /// 工作票
    /// </summary>
    public class CeYesWorkTicket
    {

        //主键Id
        public int id { get; set; }

        //工作票类型
        public string tkpSht { get; set; }

        //工作票编号
        public string ttkId { get; set; }

        //工作票主键
        public string ttkNo { get; set; }

        //机组主键
        public string unitNo { get; set; }

        //机组名称
        public string unitNam { get; set; }

        //设备名称
        public string elcNam { get; set; }

        //设备编码
        public string elcId { get; set; }

        //开票时间
        public string firstDtm { get; set; }

        //负责人账号
        public string ttkperId { get; set; }

        //负责人姓名
        public string ttkperNam { get; set; }

        //工作班成员姓名
        public string ttkperDsc { get; set; }

        //工作班成员账号
        public string ttkDscId { get; set; }

        //安全员账号
        public string scratchUsrId { get; set; }

        //安全员姓名
        public string scratchUsrNam { get; set; }

        //部门主键
        public string plaNo { get; set; }

        //部门名称
        public string plaNam { get; set; }

        //班组主键
        public string otcrwNo { get; set; }

        //班组名称
        public string otcrwNam { get; set; }

        //开始时间
        public string planbegDtm { get; set; }

        //结束时间
        public string planenDtm { get; set; }

        ////工作地点
        //public string adrDsc { get; set; }

        public List<CeAdrDscInfo> adrDsc { get; set; }

        //工作内容
        public string ttkAdr { get; set; }

        //状态id
        public string ttkSta { get; set; }

        //状态名称
        public string ttkStaNam { get; set; }

        //评价信息
        public string ttkNot { get; set; }

        //创建时间
        public string createTime { get; set; }

        //修改时间
        public string modifyTime { get; set; }

        public string GetWorkAddress()
        {
            if (adrDsc == null) return "";
            else
            {
                string info = "";
                for(int i=0;i<adrDsc.Count;i++)
                {
                    string endSign = adrDsc[i].ADDRESS_DESC.EndsWith(",") ? "" : (i == adrDsc.Count - 1 ? "" : ",");
                    info += string.Format("{0}{1}", adrDsc[i].ADDRESS_DESC, endSign);
                }
                return info;
            }
        }
    }

    public class CeAdrDscInfo
    {
        public string ADDRESS_DESC { get; set; }

        public string WORK_DESC { get; set; }
    }

    /// <summary>
    /// 操作票
    /// </summary>
    public class CeYesOperateTicket
    {

        //主键Id
        public int id;

        // 操作票编号
        public string opermstId { get; set; }

        // 操作票主键
        public string opermstNo { get; set; }

        // 操作票类型
        public string opermstTyp { get; set; }

        // 机组主键
        public string unitNo { get; set; }

        // 机组名称
        public string unitNam { get; set; }

        // 设备名称
        public string elcNam { get; set; }

        // 设备编码
        public string elcNo { get; set; }

        // 申请时间
        public string fstusrDtm { get; set; }

        // 发令人账号
        public string ordId { get; set; }

        // 发令人姓名
        public string ordNam { get; set; }

        // 受令人账号
        public string revordId { get; set; }

        // 受令人姓名
        public string revordNam { get; set; }

        // 发令时间
        public string ordDtm { get; set; }

        // 操作人姓名
        public string fstusrId { get; set; }

        // 操作人姓名
        public string fstusrNam { get; set; }

        // 操作人账号
        public string keeperId { get; set; }

        // 监护人姓名
        public string keeperNam { get; set; }

        // 部门主键
        public string plaNo { get; set; }

        // 部门名称
        public string plaNam { get; set; }

        // 班组主键
        public string crwNo { get; set; }

        // 班组主键
        public string crwNam { get; set; }

        // 操作内容
        public string opermstSht { get; set; }

        // 状态编号
        public string opermstSta { get; set; }

        // 状态名称
        public string opermstStaNam { get; set; }

        //创建时间
        public string createTime { get; set; }

        //修改时间
        public string modifyTime { get; set; }
    }

    public class CeYesAssistTicket
    {
        //主键Id
        public int id { get; set; }

        // 动火票类型
        public string firTyp { get; set; }

        // 动火票编号
        public string firId { get; set; }

        // 动火票主键
        public string firNo { get; set; }

        // 设备名称
        public string elcNam { get; set; }

        // 设备编码
        public string elcId { get; set; }

        // 开票时间
        public string fstusrDtm { get; set; }

        // 负责人账号
        public string fstusrId { get; set; }

        // 负责人姓名
        public string firperNam { get; set; }

        // 机组主键
        public string unitNo { get; set; }

        // 机组名称
        public string unitNam { get; set; }

        // 成员姓名
        public string ttkperDsc { get; set; }

        // 安全员账号
        public string safusrId { get; set; }

        // 安全员姓名
        public string safusrNam { get; set; }

        // 部门主键
        public string plaNo { get; set; }

        // 部门名称
        public string plaNam { get; set; }

        // 班组主键
        public string crwNo { get; set; }

        // 班组名称
        public string crwNam { get; set; }

        // 开始时间
        public string planbegDtm { get; set; }

        // 结束时间
        public string planendDtm { get; set; }

        // 工作地点
        public string firAdr { get; set; }

        // 工作内容
        public string firDsc { get; set; }

        // 处理状态ID
        public string firSta { get; set; }

        // 处理状态名称
        public string firStaNam { get; set; }

        // 评价信息
        public string qtNot { get; set; }

        //创建时间
        public string createTime { get; set; }

        //修改时间
        public string modifyTime { get; set; }
    }


    /// <summary>
    /// 辅票返回类型
    /// </summary>
    public class CeYesAssistTicketListStatistics 
    {
    public int nSum { get; set; }

    public List<CeYesAssistTicket> entityList { get; set; }

}

    /// <summary>
    /// 操作票返回类型
    /// </summary>
    public class CeYesOperateTicketListStatistics 
    {

    public int nSum { get; set; }

    public  List<CeYesOperateTicket> entityList { get; set; }

}

    /// <summary>
    ///工作票返回类型
    /// </summary>
    public class CeYesWorkTicketListStatistics 
    {

        public int nSum { get; set; }

        public List<CeYesWorkTicket> entityList { get; set; }

}
#endregion

#region EMP移动巡检
/// <summary>
/// 班组巡检情况接口返回参数(任务的巡检区域列表)
/// </summary>
[DataContract]
    public class EmpGroupInspection
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string areacode { get; set; }
        [DataMember]
        public string areaid { get; set; }
        [DataMember]
        public string areaname { get; set; }
        [DataMember]
        public string bASEFLAG_GroupSummaryRow { get; set; }
        [DataMember]
        public string begintime { get; set; }
        [DataMember]
        public string groupobjectid { get; set; }
        [DataMember]
        public string itemcount { get; set; }
        [DataMember]
        public string itemtotalcount { get; set; }
        [DataMember]
        public string signinid { get; set; }
        [DataMember]
        public string signner { get; set; }
        [DataMember]
        public string signnername { get; set; }
        [DataMember]
        public DateTime taskdate { get; set; }
        [DataMember]
        public string taskname { get; set; }
        [DataMember]
        public string taskid { get; set; }

    }

    /// <summary>
    /// 巡检项结果/项目巡检统计
    /// 巡检区域下巡检项目实体 yzl20201201
    /// </summary>
    [DataContract]
    public class InspectionItemsResult
    {
        [DataMember]
        //[Display(Name = "设备本地Id")]
        public int Iid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string equipname { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public object[] filelist { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string id { get; set; }
        [DataMember]
        //[Display(Name = "项目名称")]
        public string itemname { get; set; }
        [DataMember]
        //[Display(Name = "巡检日期")]
        public DateTime recodedate { get; set; }
        [DataMember]
        //[Display(Name = "合理范围")]
        public string resultscope { get; set; }
        [DataMember]
        // [Display(Name = "巡检结果")]
        public string resultvalue { get; set; }
        [DataMember]
        //[Display(Name = "说明")]
        public string remark { get; set; }

    }

    //}
    /// <summary>
    /// 巡检结果目录查询返回参数
    /// </summary>
    public class EmpInspectionItemsResultSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 巡检项目
        /// </summary>
        public string itemname { get; set; }
        /// <summary>
        /// 巡检日期开始日期
        /// </summary>
        public DateTime recodedate1 { get; set; }
        /// <summary>
        /// 巡检日期结束日期
        /// </summary>
        public DateTime recodedate2 { get; set; }
        /// <summary>
        /// 巡检区域ID
        /// </summary>
        public string areaid { get; set; }
        /// <summary>
        /// 按照巡检区域ID进行查询必须携带selectedtype参数 如果 当前节点leaf 为0  说明 当前节点包含子节点 如果当前节点 leaf 为 1 说明 当前节点不包含子节点
        /// </summary>
        public string selectedtype { get; set; }
        public EmpInspectionItemsResultSearchArg(int PageIndex, int PageSize, string StartDate, string EndDate, string ItemName = null, string AreaId = null, string IsLeaf = null)
        {
            this.pageIndex = PageIndex;
            this.pageSize = PageSize;
            recodedate1 = DateTime.Parse(StartDate);
            recodedate2 = DateTime.Parse(EndDate);
            itemname = ItemName;
            areaid = AreaId;
            selectedtype = IsLeaf;
        }
    }

    /// <summary>
    /// 巡检区域
    /// </summary>
    [DataContract]
    public class AreaTree
    {

        [DataMember]
        //[Display(Name = "巡检区域ID")]
        public string id { get; set; }
        [DataMember]
        // [Display(Name = "巡检区域父ID")]
        public string parentid { get; set; }
        [DataMember]
        //[Display(Name = "是否有叶子节点")]//1  没有 0 有
        public int isLeaf { get; set; }
        [DataMember]
        public string icon { get; set; }
        /// <summary>
        /// 区域名称(不明确)
        /// </summary>
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public bool leaf { get; set; }
        [DataMember]
        public Attributes attributes { get; set; }
    }
    [DataContract]
    public class Attributes
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int itemcount { get; set; }
        [DataMember]
        public string parentid { get; set; }

    }

    [DataContract]
    public class TreeList
    {
        [DataMember]
        public List<AreaTree> treeList { get; set; }
    }

    public class EmpCheckresultPageInfoSearchArg
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        /// <summary>
        /// 巡检日期
        /// </summary>
        public DateTime? recodedate1 { get; set; }
        /// <summary>
        /// 巡检人员
        /// </summary>
        public string custom1 { get; set; }
        /// <summary>
        /// 是否超限
        /// </summary>
        public string custom3 { get; set; }
        /// <summary>
        /// 巡检班组
        /// </summary>
        public string custom2 { get; set; }
    }

    /// <summary>
    /// 项目巡检统计
    /// </summary>
    [DataContract]
    public class CheckresultPageInfo
    {
        [DataMember]
        //[Display(Name = "本地Id")]
        public int Id { get; set; }
        [DataMember]
        public string custom1 { get; set; }

        [DataMember]
        //[Display(Name = "区域ID")]
        public string areaid { get; set; }
        [DataMember]
        //[Display(Name = "")]
        public string checkvalue { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [DataMember]
        public string itemname { get; set; }

        [DataMember]
        public List<CheckList> checkList { get; set; }
    }

    public class CheckList
    {
        /// <summary>
        /// 第N巡检人
        /// </summary>
        [DataMember]
        public string checkuser { get; set; }
        /// <summary>
        /// 第N次巡检时间
        /// </summary>
        [DataMember]
        public DateTime? checktime { get; set; }
        /// <summary>
        /// 第N次巡检结果
        /// </summary>
        [DataMember]
        public string checkresult { get; set; }

        public CheckList()
        {

        }
        public CheckList(string checkuser, DateTime? checktime, string checkresult)
        {
            this.checkuser = checkuser;
            this.checktime = checktime;
            this.checkresult = checkresult;
        }
    }
    #endregion

    #region 闵行门禁
    /// <summary>
    /// 门禁历史记录查询参数
    /// </summary>
    [DataContract]
    public class RequestAccessControlHistory
    {
        /*
        字段	        说明	    类型	长度	是否必填	备注
        pageNum	        页码	    int	    8	    Y
        pageSize	    每页大小	int	    8	    Y
        startSwingTime	开始时间	string	--	    N	    yyyy-mm-dd HH:mm:ss
        endSwingTime	结束时间	string	--	    N	    yyyy-mm-dd HH:mm:ss
        category	    卡类型	    string	2	    N	    0:IC卡,1:有源RFID,2:CPU卡
        deptIds	        部门id	    string	--	    Y	    以逗号分隔
        cardNumber	    卡号	    string	32	    N
        personCode	    人员编号	string	30	    N
        enterOrExit	    事件类型	int	    4	    N	    1/进门;2/出门
        openResult	    开门结果	int     4	    N	    1/成功;0/失败
        channelCode	    通道编码	string	64	    N	    查询的时候,通道编码前面需要拼接上ACC_
        openType	    开门方式	int	    4	    N
        41, '胁迫刷卡开门'
        42, '合法密码开门'
        43, '非法密码开门'
        44, '胁迫密码开门'
        45, '合法指纹开门'
        46, '非法指纹开门'
        47, '胁迫指纹开门'
        48, '远程开门'
        49, '按钮开门'
        50, '钥匙开门'
        51, '合法刷卡'
        52, '非法刷卡'
        53, '门磁'
        54, '异常开门'
        55, '异常关门'
        56, '正常关门'
        57, '正常开门'
        58, '巡更超时报警
        59, '报警门禁对讲请求报警'
        60,’人脸开门’
        61,’人脸刷门’
        62,’人脸非法刷门’
        1421,’ RFID有源合法’
        1422,’ RFID无源合法’
        1423,’ RFID有源非法’
        1424,’ RFID无源非法’
        1433,’ 黑名单事件’
        1434,’ 合法二维码开门’
        1435,’ 非法二维码开门’
        1436,’人证合法开门’
        1437,’人证非法开门’
        1438,’人证和身份证合法开门’
        1439,’人证和身份证非法开门’
        1455,’ 先刷卡后密码合法开门’
        1456,’ 先刷卡后密码非法开门’
        1461,’ 刷卡+指纹组合合法开门’
        1462,’ 刷卡+指纹组合非法开门’
        1463,’ 多人合法开门’
        1464,’ 多人非法开门’
        1467,’ 人员编号+密码合法开门’
        1468,’ 人员编号+密码非法开门’
        1469,’ 人脸+密码合法开门’
        1470,’ 人脸+密码非法开门’
        1471,’ 指纹+密码合法开门’
        1472,’ 指纹+密码非法开门’
        1473,’ 指纹+人脸合法开门’
        1474,’ 指纹+人脸非法开门’
        1475,’ 刷卡+人脸合法开门’
        1476,’ 刷卡+人脸非法开门’
        1487,’ 指纹+人脸+密码合法开门’
        1488,’ 指纹+人脸+密码非法开门’
        1489,’ 刷卡+人脸+密码合法开门’
        1490,’ 刷卡+人脸+密码非法开门’
        1491,’ 刷卡+指纹+密码合法开门’
        1492,’ 刷卡+指纹+密码非法开门’
        1493,’ 卡+指纹+人脸组合合法开门’
        1494,’ 卡+指纹+人脸组合非法开门’
        4603,’ 卡+指纹+人脸+密码组合合法开门’
        4604,’ 卡+指纹+人脸+密码组合非法开门’
        4626,’ 人脸安全帽合法开门’
        4627,’ 人脸安全帽非法开门’
        */
        /// <summary>
        /// 页码
        /// </summary>
        [DataMember]
        public int pageNum { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        [DataMember]
        public int pageSize { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public string startSwingTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public string endSwingTime { get; set; }
        //[DataMember]
        //public string category { get; set; }
        //[DataMember]
        //public string deptIds { get; set; }
        //[DataMember]
        //public string cardNumber { get; set; }
        //[DataMember]
        //public string personCode { get; set; }
        //[DataMember]
        //public int enterOrExit { get; set; }
        //[DataMember]
        //public int openResult { get; set; }
        /// <summary>
        /// 通道编码
        /// </summary>
        [DataMember]
        public string channelCode { get; set; }
        //[DataMember]
        //public int openType { get; set; }
    }
    /// <summary>
    ///  门禁历史记录获取数据
    /// </summary>
    [DataContract]
    public class ReturnAccessControlHistory
    {
        /*
      字段	        说明	    类型	    备注
      success	    成功与否	boolean	    true:成功;false:失败
      data	        返回结果	array
      errMsg	    错误原因	string
       */
        [DataMember]
        public bool success { get; set; }
        [DataMember]
        public string errMsg { get; set; }
        [DataMember]
        public AccessControlHistoryData data;

        public ReturnAccessControlHistory()
        {
            data = new AccessControlHistoryData();
        }
    }
    /// <summary>
    ///  门禁历史记录获取数据:data数据
    /// </summary>
    [DataContract]
    public class AccessControlHistoryData
    {
        /*
        字段	        说明	    类型	长度	是否必填	备注
        currentPage	    当前页	    int
        pageSize	    每页条数	int
        totalPage	    总页数	    int
        totalRows	    总条数	    long
        pageData	    分页数据	array
        */
        /// <summary>
        /// 当前页
        /// </summary>
        [DataMember]
        public int currentPage { get; set; }
        /// <summary>
        /// 每页条数(无效字段)
        /// </summary>
        [DataMember]
        public int pageSize { get; set; }
        /// <summary>
        /// 总页数(无效字段)
        /// </summary>
        [DataMember]
        public int totalPage { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        [DataMember]
        public long totalRows { get; set; }
        [DataMember]
        public List<AccessControlHistoryPageData> pageData { get; set; }

        public AccessControlHistoryData()
        {
            pageData = new List<AccessControlHistoryPageData>();
        }
    }
    /// <summary>
    /// 门禁历史记录
    /// </summary>
    [DataContract]
    public class AccessControlHistoryPageData
    {
        /*
       pageData字段
       字段	                    说明	            类型	    长度	是否必填	备注
       id	                    记录id	            long	     20	     Y
       cardNumber	            卡号	            int	         32	     Y	        8位16进制
       cardStatus	            卡状态	            string	     4	     Y	        ACTIVE/激活;BLANK /空白;FROZEN/冻结;WITHDRAWN/注销
       cardType	                卡类型	            int	         4	     Y	        0/普通卡;1/VIP卡;2/来宾卡;3/巡逻卡;6/巡检卡;11/管理员卡
       channelCode	            通道编码	        string	     32	     Y
       channelName	            通道名称	        string	     64	     Y
       deptName	                部门名称	        string	     255	 Y
       deviceCode	            设备编码	        string	     64	     Y
       deviceName	            设备名称	        string	     64	     Y
       enterOrExit	            进出门	            int	         4	     Y	        1/进门;2/出门
       openResult	            开门结果	        int	         4	     Y	        1/成功;0/失败
       openType	                开门类型	        int	         4	     Y
       paperNumber	            证件号码	        string	     64	     Y 
       personCode	            人员编号	        string	     64	     Y
       personId	                人员ID	            long	     20	     Y
       personName	            人员名称	        string	     64	     Y
       swingTime	            刷卡时间	        string	     --	     Y	        yyyy-MM-dd HH:mm:ss
       recordImage	            抓图	            string	     255	 Y
       errorDetail	            失败原因	        string	     64	     N
       */
        [DataMember]
        public long id { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        [DataMember]
        public string cardNumber { get; set; }
        [DataMember]
        public int cardStatus { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        [DataMember]
        public int cardType { get; set; }
        [DataMember]
        public string channelCode { get; set; }
        /// <summary>
        /// 通道名称
        /// </summary>
        [DataMember]
        public string channelName { get; set; }
        [DataMember]
        public string deptName { get; set; }
        [DataMember]
        public string deviceCode { get; set; }
        [DataMember]
        public string deviceName { get; set; }
        /// <summary>
        /// 进出门
        /// </summary>
        [DataMember]
        public int enterOrExit { get; set; }
        [DataMember]
        public int openResult { get; set; }
        [DataMember]
        public int openType { get; set; }
        [DataMember]
        public string paperNumber { get; set; }
        [DataMember]
        public string personCode { get; set; }
        [DataMember]
        public long personId { get; set; }
        /// <summary>
        /// 人员名称
        /// </summary>
        [DataMember]
        public string personName { get; set; }
        /// <summary>
        /// 刷卡时间
        /// </summary>
        [DataMember]
        public string swingTime { get; set; }
        [DataMember]
        public string recordImage { get; set; }
        [DataMember]
        public string errorDetail { get; set; }
    }
    /// <summary>
    /// 远程开关门（接口传入参数）
    /// </summary>
    [DataContract]
    public class RequestAccessControlDoorCtrl
    {
        /// <summary>
        /// 通道编码数组
        /// </summary>
        [DataMember]
        public List<string> channelCodeList { get; set; }
    }
    /// <summary>
    /// 门禁远程开关
    /// </summary>
    [DataContract]
    public class ReturnAccessControlDoorCtrl
    {
        /*
        字段	        说明	    类型	    备注
        success	        成功与否	boolean	    true:成功;false:失败
        errMsg	        错误原因	string
        */
        [DataMember]
        public bool success { get; set; }
        [DataMember]
        public string errMsg { get; set; }
    }
    #endregion

    #region 设备编辑模型增删改查
    public class DevTypeModel
    {
        //设备类型记录Id
        [DataMember]
        public string typeRecordId { get; set; }
        //设备类型名称
        [DataMember]
        public string typeName { get; set; }
        //设备类型编号
        [DataMember]
        public long typeCode { get; set; }
        //前面板
        [DataMember]
        public string frontElevation { get; set; }
        //模型记录Id
        [DataMember]
        public string modelRecordId { get; set; }
        //模型名称
        [DataMember]
        public string modelName { get; set; }
        //模型Id
        [DataMember]
        public string modelId { get; set; }
        //大项名称
        [DataMember]
        public string itemName { get; set; }
        //大类名称
        [DataMember]
        public string className { get; set; }
        //标志位，0表示模型是通过初始化导入的，1表示模型是通过手动添加的
        [DataMember]
        public string flag { get; set; }
        public bool judgeAddParam()
        {
            bool bResult = false;
            if (string.IsNullOrEmpty(this.typeName))
            {
                return bResult;
            }
            if (this.typeCode <= 0)
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.frontElevation))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.modelName))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.modelId))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.itemName))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.className))
            {
                return bResult;
            }
            bResult = true;
            return bResult;
        }
        public bool judgeModifyParam()
        {
            bool bResult = false;

            if (string.IsNullOrEmpty(this.typeRecordId))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.typeName))
            {
                return bResult;
            }
            if (this.typeCode <= 0)
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.frontElevation))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.modelRecordId))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.modelName))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.modelId))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.itemName))
            {
                return bResult;
            }
            if (string.IsNullOrEmpty(this.className))
            {
                return bResult;
            }
            bResult = true;
            return bResult;
        }
    }
    #endregion

    #region 进程管理
    public class AddProcessArg
    {
        /// <summary>
        /// 是否在用
        /// </summary>
        public bool isUsing { get; set; }
        /// <summary>
        /// 登录账户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 程序进程ID
        /// </summary>
        public int pid { get; set; }
        /// <summary>
        /// 用户电脑的IP
        /// </summary>
        public string userIp { get; set; }
        /// <summary>
        /// 客户端exe的连接sessionId
        /// </summary>
        public string sessionId { get; set; }
    }
    #endregion

    #region 报表管理
    /// <summary>
    /// 基站离线统计查询条件
    /// </summary>
    public class ArchorOffLineRateArg
    {
        /// <summary>
        /// 是否按月统计（False: 按天统计）
        /// </summary>
        [DataMember]
        public bool isMonth { get; set; }
        /// <summary>
        /// 基站编号
        /// </summary>
        [DataMember]
        public string code { get; set; }
        /// <summary>
        /// 起始时间(isMonth=false时不能为空)
        /// </summary>
        [DataMember]
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间(isMonth=false时不能为空)
        /// </summary>
        [DataMember]
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 年(isMonth=true时大于0)
        /// </summary>
        [DataMember]
        public int year { get; set; }
        [DataMember]
        public int month { get; set; }
    }
    /// <summary>
    /// 基站汇总统计
    /// </summary>
    public class ArchorOffLineRateByDate
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        [DataMember]
        public DateTime statisticsDate { get; set; }
        /// <summary>
        /// 年
        /// </summary>
        [DataMember]
        public int statisticsYear { get; set; }
        /// <summary>
        /// 月
        /// </summary>
        [DataMember]
        public int statisticsMonth { get; set; }
        /// <summary>
        /// 日
        /// </summary>
        [DataMember]
        public int statisticsDay { get; set; }
        //离线基站数量
        [DataMember]
        public int offlineNum { get; set; }
        /// <summary>
        /// 离线时长
        /// </summary>
        [DataMember]
        public double offlineDate { get; set; }
    }
    /// <summary>
    /// 基站离线率统计
    /// </summary>
    [DataContract]
    public class ArchorOffLineRate
    {
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 基站所属区域id
        /// </summary>
        [DataMember]
        public int areaId { get; set; }
        /// <summary>
        /// 基站所属区域名称
        /// </summary>
        [DataMember]
        public string areaName { get; set; }
        /// <summary>
        /// 基站编号
        /// </summary>
        [DataMember]
        public string code { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        [DataMember]
        public DateTime statisticsDate { get; set; }
        /// <summary>
        /// 年
        /// </summary>
        [DataMember]
        public int statisticsYear { get; set; }
        /// <summary>
        /// 月
        /// </summary>
        [DataMember]
        public int statisticsMonth { get; set; }
        /// <summary>
        /// 日
        /// </summary>
        [DataMember]
        public int statisticsDay { get; set; }
        /// <summary>
        /// 离线时长
        /// </summary>
        [DataMember]
        public double offlineDate { get; set; }
        /// <summary>
        /// 离线率（%）
        /// </summary>
        [DataMember]
        public double offlineRate { get; set; }
        public ArchorOffLineRate()
        {

        }
    }
    #endregion
}


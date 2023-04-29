using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Location.EmpWorkTicket
{
    /// <summary>
    /// 工作票详情数据
    /// </summary>
    public class WorkTicketDetail
    {
        [DataMember]
        //[Display(Name = "设备本地Id")]
        public int Id { get; set; }
        [DataMember]
        //[Display(Name = "工作票记录ID")]
        public string ticketid { get; set; }
        [DataMember]
        //[Display(Name = "维修对象ID 对应工单上的设备，从工单上带入")]
        public string objectid { get; set; }
        [DataMember]
        //[Display(Name = "工作票类型 01-电气第一种工作票 02-电气第二种工作票 03-热力机械工作票 04 - 热控工作票")]
        public string tickettype { get; set; }
        [DataMember]
        //[Display(Name = "工作票类型名称")]
        public string tickettypename { get; set; }
        [DataMember]
        // [Display(Name = "工作票编号")]
        public string ticketcode { get; set; }
        [DataMember]
        //[Display(Name = "工作票名称")]
        public string ticketname { get; set; }
        [DataMember]
        //[Display(Name = "作业单位")]
        public string workdepartment { get; set; }
        [DataMember]
        //[Display(Name = "工作中心")]
        public string workcenterid { get; set; }
        [DataMember]
        //[Display(Name = "工作班组名称")]
        public string workcentername { get; set; }
        [DataMember]
        //[Display(Name = "工作负责人")]
        public string workmanager { get; set; }
        [DataMember]
        //[Display(Name = "工作负责人id")]
        public string workmanagerid { get; set; }
        [DataMember]
        //[Display(Name = "工作负责人名称")]
        public string workmanagername { get; set; }
        [DataMember]
        //[Display(Name = "联系手机号")]
        public string phonecode { get; set; }
        [DataMember]
        //[Display(Name = "工作许可人")]
        public string workpermiter { get; set; }
        [DataMember]
        //[Display(Name = "工作成员名称")]
        public string workmembers { get; set; }
        [DataMember]
        //[Display(Name = "工作成员id")]
        public string workmemberids { get; set; }
        [DataMember]
        //[Display(Name = "工作成员统计")]
        public string workmembercount { get; set; }
        [DataMember]
        //[Display(Name = "外部工作成员")]
        public string outworkmembers { get; set; }
        [DataMember]
        //[Display(Name = "接收人")]
        public string receiver { get; set; }
        [DataMember]
        //[Display(Name = "解票时间")]
        public string receiverTime { get; set; }
        [DataMember]
        //[Display(Name = "隔离锁箱号")]
        public string lockboxcode { get; set; }
        [DataMember]
        //[Display(Name = "有效开始日期")]
        public DateTime beginvaliddate { get; set; }
        [DataMember]
        //[Display(Name = "有效截止日期")]
        public DateTime endvaliddate { get; set; }
        [DataMember]
        //[Display(Name = "工作地点")]
        public string worksite { get; set; }
        [DataMember]
        //[Display(Name = "工作内容")]
        public string workcontent { get; set; }
        [DataMember]
        //[Display(Name = "创建者")]
        public string creater { get; set; }
        [DataMember]
        //[Display(Name = "创建者id")]
        public string createrid { get; set; }
        [DataMember]
        //[Display(Name = "创建日期")]
        public DateTime createdate { get; set; }
        [DataMember]
        //[Display(Name = "最近修改者")]
        public string lastmodifier { get; set; }
        [DataMember]
        //[Display(Name = "最近修改日期")]
        public DateTime lastmodifydate { get; set; }
        [DataMember]
        //[Display(Name = "工作票状态")]
        public string ticketstatus { get; set; }
        [DataMember]
        //[Display(Name = "工单编号")]
        public string ordercode { get; set; }
        [DataMember]
        //[Display(Name = "工单标题")]
        public string ordertitle { get; set; }
        [DataMember]
        //[Display(Name = "数据状态名称")]
        public string ticketstatusname { get; set; }
        [DataMember]
        //[Display(Name = "数据状态颜色")]
        public string ticketstatuscolor { get; set; }
        [DataMember]
        //[Display(Name = "工作票状态")]
        public string status { get; set; }
        [DataMember]
        //[Display(Name = "签发人")]
        public string issuer { get; set; }
        [DataMember]
        //[Display(Name = "签发时间")]
        public DateTime issuerWorkTime { get; set; }
        [DataMember]
        //[Display(Name = "许可人")]
        public string licenser { get; set; }
        [DataMember]
        //[Display(Name = "外包人员")]
        public string outerPerson { get; set; }
        [DataMember]
        //[Display(Name = "是否外包工作票")] //0否 1是
        public string outerTicket { get; set; }
        [DataMember]
        //[Display(Name = "高温")]
        public string permithighTemperature { get; set; }
        [DataMember]
        //[Display(Name = "高压")]
        public string permithighPressure { get; set; }
        [DataMember]
        //[Display(Name = "触电")]
        public string permitelectricShock { get; set; }
        [DataMember]
        //[Display(Name = "转动")]
        public string permitturn { get; set; }
        [DataMember]
        //[Display(Name = "高空坠落")]
        public string permithighFall { get; set; }
        [DataMember]
        //[Display(Name = "室息")]
        public string permitventricularRest { get; set; }
        [DataMember]
        //[Display(Name = "中毒")]
        public string permitpoisoning { get; set; }
        [DataMember]
        //[Display(Name = "辐射")]
        public string Permitradiation { get; set; }
        [DataMember]
        //[Display(Name = "着火")]
        public string permitcatchFire { get; set; }
        [DataMember]
        //[Display(Name = "操作")]
        public string permitoperation { get; set; }
        [DataMember]
        //[Display(Name = "其他")]
        public string permitotherRisk { get; set; }
        [DataMember]
        //[Display(Name = "工作负责人签名")]
        public string permitManager { get; set; }
        [DataMember]
        //[Display(Name = "联系电话")]
        public string permiPhone { get; set; }
        [DataMember]
        //[Display(Name = "运行批准工作时间开始")]
        public string permitRunWorkFromTime { get; set; }
        [DataMember]
        //[Display(Name = "运行批准工作时间结束")]
        public string permitRunWorkToTime { get; set; }
        [DataMember]
        //[Display(Name = "许可开始工作时间")]
        public DateTime permitWorkFromTime { get; set; }
        [DataMember]
        //[Display(Name = "工作许可人签名")]
        public string permitPersSign { get; set; }
        [DataMember]
        //[Display(Name = "控长（值长）签名")]
        public string permitLeaderSign { get; set; }
        [DataMember]
        //[Display(Name = "检修工作确认是否执行")]
        public string examineRepair { get; set; }
        [DataMember]
        //[Display(Name = "工作终结工作结束时间")]
        public DateTime stopWorkTime { get; set; }
        [DataMember]
        //[Display(Name = "工作终结已拆除接地线编号")]
        public string stopRmovGrounLineNum { get; set; }
        [DataMember]
        //[Display(Name = "工作终结已拆除接地线编号 组")]
        public string stopGrounLineCountNum { get; set; }
        [DataMember]
        //[Display(Name = "工作终结已拉开接地刀闸（小车）编号")]
        public string stopRmovTrolleyNum { get; set; }
        [DataMember]
        //[Display(Name = "工作终结已拉开接地刀闸（小车）编号 台")]
        public string stopRmovTrolleyCountNum { get; set; }
        [DataMember]
        //[Display(Name = "终止工作负责人签名")]
        public string stopWorkeManager { get; set; }
        [DataMember]
        //[Display(Name = "终止工作许可人签名")]
        public string stopPermWorkeManager { get; set; }
        [DataMember]
        //[Display(Name = "终止名称")]
        public string stopLineName { get; set; }
        [DataMember]
        //[Display(Name = "终止备注")]
        public string stopRemark { get; set; }
        [DataMember]
        //[Display(Name = "保留安全措施备注")]
        public string persistMeasureRmeark { get; set; }
        [DataMember]
        //[Display(Name = "已拆除接地线编号")]
        public string removeGroundLineNum { get; set; }
        [DataMember]
        //[Display(Name = "已拆除接地线编号共组")]
        public string removeGroundLineCount { get; set; }
        [DataMember]
        //[Display(Name = "已拉开接地刀闸（小车 )")]
        public string removeGroundBladeNum { get; set; }
        [DataMember]
        //[Display(Name = "已拉开接地刀闸（小车 )共         副（台）")]
        public string removeGroundBladeCount { get; set; }
        [DataMember]
        //[Display(Name = "许可时间")]
        public string licensetime { get; set; }
        [DataMember]
        //[Display(Name = "许可人值")]
        public string licensorteam { get; set; }
        [DataMember]
        //[Display(Name = "终结许可人")]
        public string endlicenser { get; set; }
        [DataMember]
        //[Display(Name = "终结许可人值")]
        public string endlicensorteam { get; set; }
        [DataMember]
        //[Display(Name = "关闭人")]
        public string closeer { get; set; }
        [DataMember]
        //[Display(Name = "关闭时间")]
        public string closetime { get; set; }
        [DataMember]
        //[Display(Name = "关闭原因")]
        public string closereason { get; set; }
        [DataMember]
        //[Display(Name = "工作中心编号")]
        public string workcentercode { get; set; }
        [DataMember]
        //[Display(Name = "工作成员总数")]
        public string workMemberscount { get; set; }
        [DataMember]
        //[Display(Name = "设备id")]
        public string equipmentid { get; set; }
        [DataMember]
        //[Display(Name = "设备名称")]
        public string equipmentName { get; set; }
        [DataMember]
        //[Display(Name = "系统设备ID")]
        public string systemEquipmenId { get; set; }
        [DataMember]
        //[Display(Name = "系统设备名称")]
        public string systemEquipmenName { get; set; }
        [DataMember]
        //[Display(Name = "工作地点保留带电部分或注意事项")]
        public string measurescontent { get; set; }
        [DataMember]
        //[Display(Name = "工作票带的副票")]
        public List<AttachItems> attachItems { get; set; }
        [DataMember]
        //[Display(Name = "1应拉断路器（开关）、隔离开关（刀闸）")]
        public List<ItemBeans1> itemBean1 { get; set; }
        [DataMember]
        //[Display(Name = "2应装接地线、应合接地刀闸（注明确实地点、名称及接地线编号）")]
        public List<ItemBeans1> itemBean2 { get; set; }
        [DataMember]
        //[Display(Name = "3应设遮拦、应挂标示牌及防止二次回路误碰等措施")]
        public List<ItemBeans1> itemBean3 { get; set; }
        [DataMember]
        //[Display(Name = "4工作地点保留带电部分或注意事项")]
        public List<ItemBeans1> itemBean4 { get; set; }
        [DataMember]
        //[Display(Name = "5危险点分析及控制措施")]
        public List<ItemBeans1> itemBean5 { get; set; }
        [DataMember]
        //[Display(Name = "6.安全措施")]
        public List<ItemBeans1> itemBean6 { get; set; }
        [DataMember]
        //[Display(Name = "7. 补充安全措施")]
        public List<ItemBeans1> itemBean7 { get; set; }
        [DataMember]
        //[Display(Name = "8.由检修人员执行的安全措施")]
        public List<ItemBeans1> itemBean8 { get; set; }
        [DataMember]
        //[Display(Name = "9.由运行人员执行的安全措施")]
        public List<ItemBeans1> itemBean9 { get; set; }
        /// <summary>
        /// 工作班组人员
        /// </summary>
        public List<WorkGroupItems> workGroupItems { get; set; }
        /// <summary>
        /// 工作票变更
        /// </summary>
        public ChangeBean changeBean { get; set; }
        /// <summary>
        /// 工作票延期
        /// </summary>
        public DelayBean delayBean { get; set; }
        /// <summary>
        /// 工作票挂起
        /// </summary>
        public List<HangUpItems> hangUpItems { get; set; }
        /// <summary>
        /// 专职监护人
        /// </summary>
        public string permitWorkGroupSix { get; set; }
    }
    /// <summary>
    /// 工作票带的副票
    /// </summary>
    public class AttachItems
    {
        //[Display(Name = "副票id")]
        public string id { get; set; }
        //[Display(Name = "主票id")]
        public string ticketid { get; set; }
        //[Display(Name = "1101一级动火工作票、1102二级动火工作票、12气体检测、13外包风险控制单、1401受限空间特种作业许可证、1402 挖掘特种作业许可证、1403 起重特种作业许可证、1404 高处特种作业许可证、15 检修执行安措单")]
        public string ticketItemType { get; set; }
        //[Display(Name = "1101一级动火工作票、1102二级动火工作票、12气体检测、13外包风险控制单、1401受限空间特种作业许可证、1402 挖掘特种作业许可证、1403 起重特种作业许可证、1404 高处特种作业许可证、15 检修执行安措单")]
        public string attachtickettype { get; set; }
        public string attachticketname { get; set; }
        //[Display(Name = "工作票主票编号")]
        public string ticketcode { get; set; }
        //[Display(Name = "附票审核状态  1 新建 2 审核中 3 退回 4 关闭")]
        public string datastatus { get; set; }
        //[Display(Name = "创建人")]
        public string creator { get; set; }
        //[Display(Name = "创建日期")]
        public string createdate { get; set; }
        //[Display(Name = "创建人ID")]
        public string creatorid { get; set; }
    }
    public class ItemBeans1
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
        /// 1应拉断路器（开关）、隔离开关（刀闸）
        /// 2应装接地线、应合接地刀闸（注明确实地点、名称及接地线编号）
        /// 3应设遮拦、应挂标示牌及防止二次回路误碰等措施
        /// 4工作地点保留带电部分或注意事项    5.危险点分析及控制措施
        /// 6.安全措施
        /// 7.补充安全措施
        /// 8.由检修人员执行的安全措施
        /// 9.由运行人员执行的安全措施
        /// 10.生产运行人员补充安全措施
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
        public DateTime createDate { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string serialNum { get; set; }
    }
    /// <summary>
    /// 工作班组人员
    /// </summary>
    public class WorkGroupItems
    {
        /// <summary>
        /// 人员名称
        /// </summary>
        public string personnelName { get; set; }
        /// <summary>
        /// 人员名称签字
        /// </summary>
        public string signInfo { get; set; }
    }
    /// <summary>
    /// 工作票变更
    /// </summary>
    public class ChangeBean
    {
        //本次变更的工作负责人
        public string workmanager { get; set; }
        //工作票签发人员签名
        public string changeissuer { get; set; }
        //工作票签发人员签名时间
        public string changeissurtime { get; set; }
        //工作负责人签名
        public string changeworkmanagername { get; set; }
        //工作负责人签名时间
        public string changeworkmanagertime { get; set; }
        //工作人员变动情况（增添人员姓名、变动日期及时间）
        public string remark { get; set; }
    }
    /// <summary>
    /// 工作票延期
    /// </summary>
    public class DelayBean
    {
        /// <summary>
        /// 工作负责人签名
        /// </summary>
        public string delayjobleadersignature { get; set; }
        /// <summary>
        /// 有效期延长时间
        /// </summary>
        public string delaytime { get; set; }
        /// <summary>
        /// 许可人签名
        /// </summary>
        public string licenser { get; set; }
        /// <summary>
        /// 许可时间
        /// </summary>
        public string licensetime { get; set; }
    }
    /// <summary>
    /// 工作票挂起
    /// </summary>
    public class HangUpItems
    {
        /// <summary>
        /// 工作票收回负责人签名
        /// </summary>
        public string retractJobLeaderPath { get; set; }
        /// <summary>
        /// 工作票收回许可人签名
        /// </summary>
        public string retractLicensorPath { get; set; }
        /// <summary>
        /// 重新开工工作负责人签名
        /// </summary>
        public string restartJobLeaderPath { get; set; }
        /// <summary>
        /// 重新开工许可人签名
        /// </summary>
        public string restartLicensorPath { get; set; }
        /// <summary>
        /// 工作票收回时间
        /// </summary>
        public string retractTime { get; set; }
        /// <summary>
        /// 重新开工时间
        /// </summary>
        public string restartTime { get; set; }
    }

    #region 副票


    /// <summary>
    /// 工作票副票查询传参
    /// </summary>
    public class EmpSideTicketArg
    {
        /// <summary>
        /// 副票ID
        /// </summary>
        public string annexid { get; set; }
        /// <summary>
        /// 副票类型编码
        /// </summary>
        public string ticketItemType { get; set; }
    }

    /// <summary>
    /// 工作票副票(一级动火票、二级动火票)
    /// </summary>
    public class HotWork
    {
        /// <summary>
        /// 动火票编号
        /// </summary>
        public string hotWorkTicketCode { get; set; }
        /// <summary>
        /// 动火单位
        /// </summary>
        public string hotUnit { get; set; }

        public string hotChargePersonId { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string hotChargePerson { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        public string hotTeam { get; set; }
        /// <summary>
        /// 动火内容
        /// </summary>
        public string hotWorkContent { get; set; }
        /// <summary>
        /// 动火地点
        /// </summary>
        public string hotWorkPlace { get; set; }
        /// <summary>
        /// 申请开始时间
        /// </summary>
        public DateTime hotWorkApplicationFromTime { get; set; }
        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime hotWorkApplicationToTime { get; set; }
        /// <summary>
        /// 动火区域示意图
        /// </summary>
        public string hotWorkAreaDiag { get; set; }
        /// <summary>
        /// 应采取的安全措施
        /// </summary>
        public List<ItemBeans1_Side> itemBeans1 { get; set; }
        /// <summary>
        /// 作业前、作业过程中气体检测记录
        /// </summary>
        public List<ItemBeans2_Side> itemBeans2 { get; set; }

        public string fireDepartmentManager { get; set; }

        public string fireExecutorEndSign { get; set; }

        public string fireExecutorStartSign { get; set; }

        public string fireGuardianEndManager { get; set; }

        public string fireGuardianManager { get; set; }

        public DateTime fireLicensorEndTime { get; set; }

        public DateTime fireLicensorStartTime { get; set; }

        public string hotChargeEndSign { get; set; }

        public string hotChargeStartSign { get; set; }

        public string hotWorkTicketId { get; set; }

        public string hotWorkType { get; set; }

        public string is_valid { get; set; }

        public string issueSign { get; set; }

        public string operationLicensorEndSign { get; set; }

        public string operationLicensorStartSign { get; set; }

        public string qDepartmentManager { get; set; }

        public string remark { get; set; }

        public string shiftSupervisorEndSign { get; set; }

        public string shiftSupervisorStartSign { get; set; }

        public string ticketCode { get; set; }

        public string unitsManager { get; set; }
    }

    /// <summary>
    /// 应采取的安全措施
    /// </summary>
    public class ItemBeans1_Side
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int serialNum { get; set; }
        /// <summary>
        /// 操作措施
        /// </summary>
        public string measurecontent { get; set; }
    }

    public class ItemBeans2_Side
    {
        /// <summary>
        /// 取样时间
        /// </summary>
        public string samptime { get; set; }
        /// <summary>
        /// 可燃、有毒气体含量
        /// </summary>
        public string oxicgascontent { get; set; }
        /// <summary>
        /// 氧含量
        /// </summary>
        public string oxygencontent { get; set; }
        /// <summary>
        /// 取样人员
        /// </summary>
        public string samppersonnel { get; set; }
    }

    /// <summary>
    /// 气体检测票
    /// </summary>
    public class GasDetection
    {
        /// <summary>
        /// 检测次序
        /// </summary>
        public string ordernum { get; set; }
        /// <summary>
        /// 取样时间
        /// </summary>
        public DateTime samplingtime { get; set; }
        /// <summary>
        /// 气体检测类别
        /// </summary>
        public string ganame { get; set; }
        /// <summary>
        /// 气体含量
        /// </summary>
        public string gascontent { get; set; }
        /// <summary>
        /// 标准
        /// </summary>
        public string standard { get; set; }
        /// <summary>
        /// 测量单位
        /// </summary>
        public string measureunit { get; set; }
        /// <summary>
        /// 取样人员
        /// </summary>
        public string opreationperson { get; set; }
    }


    /// <summary>
    /// 外包风险控制单
    /// </summary>
    public class WbFxkz_List
    {
        /// <summary>
        /// 外包编号
        /// </summary>
        public string outResourceCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime currenttime { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        public string workcontent { get; set; }
        /// <summary>
        /// 监护人
        /// </summary>
        public string guardiansign { get; set; }
        /// <summary>
        /// 工作许可人
        /// </summary>
        public string licensorsign { get; set; }
        /// <summary>
        /// 值长（控长）
        /// </summary>
        public string supervisorsign { get; set; }
        /// <summary>
        /// 应采取的安全措施
        /// </summary>
        public List<WorkticketAttachProtectmeasureData> workticketAttachProtectmeasureDataDtos { get; set; }
    }

    /// <summary>
    /// 应采取的安全措施
    /// </summary>
    public class WorkticketAttachProtectmeasureData
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string serialNum { get; set; }
        /// <summary>
        /// 操作措施
        /// </summary>
        public string measurecontent { get; set; }
    }

    /// <summary>
    /// 受限空间特种作业许可证  1401
    /// </summary>
    public class TeShuzyLicence_1401
    {
        /// <summary>
        /// 作业许可证编号
        /// </summary>
        public string specialpermitcode { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string workchargeperson { get; set; }
        /// <summary>
        /// 作业单位
        /// </summary>
        public string workunit { get; set; }
        /// <summary>
        /// 作业区域
        /// </summary>
        public string operationregion { get; set; }
        /// <summary>
        /// 作业内容
        /// </summary>
        public string operationcontent { get; set; }
        /// <summary>
        /// 作业人员
        /// </summary>
        public string outoperationperson { get; set; }
        /// <summary>
        /// 作业时间开始时间
        /// </summary>
        public DateTime operationfromtime { get; set; }
        /// <summary>
        /// 作业时间结束时间
        /// </summary>
        public DateTime operationtotime { get; set; }
        /// <summary>
        /// 规范和图纸  1 为选中  0  为未选中
        /// </summary>
        public string attachrange { get; set; }
        /// <summary>
        /// 检测报告 1 为选中  0  为未选中
        /// </summary>
        public string attachtestresult { get; set; }
        /// <summary>
        /// 工作危险性分析（强制） 1 为选中  0  为未选中
        /// </summary>
        public string attachanalysis { get; set; }
        /// <summary>
        /// 其他  1 为选中  0  为未选中
        /// </summary>
        public string attachother { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人
        /// </summary>
        public string chargeperson { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人联系电话
        /// </summary>
        public string chargepersontelephone { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名
        /// </summary>
        public string cpersonname { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名联系电话
        /// </summary>
        public string cpersontelephone { get; set; }
        /// <summary>
        /// 受影响区域/交叉作业单位	1  有  0 无
        /// </summary>
        public string influence { get; set; }
        /// <summary>
        /// 签字人
        /// </summary>
        public string influencemanconfirm { get; set; }
        /// <summary>
        /// 特殊工种作业人员信息
        /// </summary>
        public List<WorkticketAttachSpecmemberData> workticketAttachSpecmemberDataDtos { get; set; }
        /// <summary>
        /// 作业前安全条件检查
        /// </summary>
        public List<WorkticketAttachConditionlistData> workticketAttachConditionlistDataDtos { get; set; }
    }

    /// <summary>
    /// 特殊工种作业人员信息
    /// </summary>
    public class WorkticketAttachSpecmemberData
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string memberName { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string certifiCateType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string certCode { get; set; }
        /// <summary>
        /// 证件有效期
        /// </summary>
        public string certvalidRange { get; set; }
    }

    /// <summary>
    /// 作业前安全条件检查
    /// </summary>
    public class WorkticketAttachConditionlistData
    {
        /// <summary>
        /// 检查项
        /// </summary>
        public string checkedcontent { get; set; }
        /// <summary>
        /// 是否执行 1 选中  0未选中
        /// </summary>
        public string executor { get; set; }
    }

    /// <summary>
    /// 挖掘特种作业许可证 1402
    /// </summary>
    public class TeShuzyLicence_1402
    {
        /// <summary>
        /// 作业许可证编号
        /// </summary>
        public string specialpermitcode { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string workchargeperson { get; set; }
        /// <summary>
        /// 作业单位
        /// </summary>
        public string workunit { get; set; }
        /// <summary>
        /// 作业区域
        /// </summary>
        public string operationregion { get; set; }
        /// <summary>
        /// 作业内容
        /// </summary>
        public string operationcontent { get; set; }
        /// <summary>
        /// 作业人员
        /// </summary>
        public string outoperationperson { get; set; }
        /// <summary>
        /// 作业时间开始时间
        /// </summary>
        public DateTime operationfromtime { get; set; }
        /// <summary>
        /// 作业时间结束时间
        /// </summary>
        public DateTime operationtotime { get; set; }
        /// <summary>
        /// 规范和图纸  1 为选中  0  为未选中
        /// </summary>
        public string attachrange { get; set; }
        /// <summary>
        /// 检测报告 1 为选中  0  为未选中
        /// </summary>
        public string attachtestresult { get; set; }
        /// <summary>
        /// 工作危险性分析（强制） 1 为选中  0  为未选中
        /// </summary>
        public string attachanalysis { get; set; }
        /// <summary>
        /// 其他  1 为选中  0  为未选中
        /// </summary>
        public string attachother { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人
        /// </summary>
        public string chargeperson { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人联系电话
        /// </summary>
        public string chargepersontelephone { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名
        /// </summary>
        public string cpersonname { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名联系电话
        /// </summary>
        public string cpersontelephone { get; set; }
        /// <summary>
        /// 受影响区域/交叉作业单位	1  有  0 无
        /// </summary>
        public string influence { get; set; }
        /// <summary>
        /// 签字人
        /// </summary>
        public string influencemanconfirm { get; set; }
        /// <summary>
        /// 动土作业区域、内容、方式(包括深度、面积，并附简图)：有图纸的可以填写图纸编号   工作负责人
        /// </summary>
        public string breakingchargeperson { get; set; }
        /// <summary>
        /// 动土作业区域、内容、方式(包括深度、面积，并附简图)：有图纸的可以填写图纸编号    日期
        /// </summary>
        public string breakingtime { get; set; }
        /// <summary>
        /// 地下设施的检测情况说明：由项目联系人与施工单位负责人共同确认，施工单位填写  受影响区域
        /// </summary>
        public string undergroundinfarea { get; set; }
        /// <summary>
        /// 地下设施的检测情况说明：由项目联系人与施工单位负责人共同确认，施工单位填写  工作负责人
        /// </summary>
        public string undergroundchargeperson { get; set; }
        /// <summary>
        /// 地下设施的检测情况说明：由项目联系人与施工单位负责人共同确认，施工单位填写   日期
        /// </summary>
        public string undergroundtime { get; set; }


        /// <summary>
        /// 特殊工种作业人员信息
        /// </summary>
        public List<WorkticketAttachSpecmemberData> workticketAttachSpecmemberDataDtos { get; set; }
        /// <summary>
        /// 作业前安全条件检查
        /// </summary>
        public List<WorkticketAttachConditionlistData> workticketAttachConditionlistDataDtos { get; set; }
    }

    /// <summary>
    /// 起重特种作业许可证 1403
    /// </summary>
    public class TeShuzyLicence_1403
    {
        /// <summary>
        /// 作业许可证编号
        /// </summary>
        public string specialpermitcode { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string workchargeperson { get; set; }
        /// <summary>
        /// 作业单位
        /// </summary>
        public string workunit { get; set; }
        /// <summary>
        /// 作业区域
        /// </summary>
        public string operationregion { get; set; }
        /// <summary>
        /// 作业内容
        /// </summary>
        public string operationcontent { get; set; }
        /// <summary>
        /// 作业人员
        /// </summary>
        public string outoperationperson { get; set; }
        /// <summary>
        /// 作业时间开始时间
        /// </summary>
        public DateTime operationfromtime { get; set; }
        /// <summary>
        /// 作业时间结束时间
        /// </summary>
        public DateTime operationtotime { get; set; }
        /// <summary>
        /// 规范和图纸  1 为选中  0  为未选中
        /// </summary>
        public string attachrange { get; set; }
        /// <summary>
        /// 检测报告 1 为选中  0  为未选中
        /// </summary>
        public string attachtestresult { get; set; }
        /// <summary>
        /// 工作危险性分析（强制） 1 为选中  0  为未选中
        /// </summary>
        public string attachanalysis { get; set; }
        /// <summary>
        /// 其他  1 为选中  0  为未选中
        /// </summary>
        public string attachother { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人
        /// </summary>
        public string chargeperson { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人联系电话
        /// </summary>
        public string chargepersontelephone { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名
        /// </summary>
        public string cpersonname { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名联系电话
        /// </summary>
        public string cpersontelephone { get; set; }
        /// <summary>
        /// 受影响区域/交叉作业单位	1  有  0 无
        /// </summary>
        public string influence { get; set; }
        /// <summary>
        /// 签字人
        /// </summary>
        public string influencemanconfirm { get; set; }
        /// <summary>
        /// 起吊重物质量
        /// </summary>
        public string weightliftingobjects { get; set; }
        /// <summary>
        /// 特殊工种作业人员信息
        /// </summary>
        public List<WorkticketAttachSpecmemberData> workticketAttachSpecmemberDataDtos { get; set; }
        /// <summary>
        /// 作业前安全条件检查
        /// </summary>
        public List<WorkticketAttachConditionlistData> workticketAttachConditionlistDataDtos { get; set; }

    }

    /// <summary>
    /// 高处作业许可证 1404
    /// </summary>
    public class TeShuzyLicence_1404
    {
        /// <summary>
        /// 作业许可证编号
        /// </summary>
        public string specialpermitcode { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string workchargeperson { get; set; }
        /// <summary>
        /// 作业单位
        /// </summary>
        public string workunit { get; set; }
        /// <summary>
        /// 作业区域
        /// </summary>
        public string operationregion { get; set; }
        /// <summary>
        /// 作业内容
        /// </summary>
        public string operationcontent { get; set; }
        /// <summary>
        /// 作业人员
        /// </summary>
        public string outoperationperson { get; set; }
        /// <summary>
        /// 作业时间开始时间
        /// </summary>
        public DateTime operationfromtime { get; set; }
        /// <summary>
        /// 作业时间结束时间
        /// </summary>
        public DateTime operationtotime { get; set; }
        /// <summary>
        /// 规范和图纸  1 为选中  0  为未选中
        /// </summary>
        public string attachrange { get; set; }
        /// <summary>
        /// 检测报告 1 为选中  0  为未选中
        /// </summary>
        public string attachtestresult { get; set; }
        /// <summary>
        /// 工作危险性分析（强制） 1 为选中  0  为未选中
        /// </summary>
        public string attachanalysis { get; set; }
        /// <summary>
        /// 其他  1 为选中  0  为未选中
        /// </summary>
        public string attachother { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人
        /// </summary>
        public string chargeperson { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的工作负责人联系电话
        /// </summary>
        public string chargepersontelephone { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名
        /// </summary>
        public string cpersonname { get; set; }
        /// <summary>
        /// 负责人、监护人员姓名及联系方式的指定监护人员姓名联系电话
        /// </summary>
        public string cpersontelephone { get; set; }
        /// <summary>
        /// 受影响区域/交叉作业单位	1  有  0 无
        /// </summary>
        public string influence { get; set; }
        /// <summary>
        /// 签字人
        /// </summary>
        public string influencemanconfirm { get; set; }
        /// <summary>
        /// 作业高度
        /// </summary>
        public string operationheight { get; set; }
        /// <summary>
        /// 作业高度等级
        /// </summary>
        public string operationlevel { get; set; }
        /// <summary>
        /// 强风
        /// </summary>
        public string conditionfiercewind { get; set; }
        /// <summary>
        /// 异温
        /// </summary>
        public string conditiontemperature { get; set; }
        /// <summary>
        /// 雪天
        /// </summary>
        public string conditionsnow { get; set; }
        /// <summary>
        /// 雨天
        /// </summary>
        public string conditionrain { get; set; }
        /// <summary>
        /// 夜间
        /// </summary>
        public string conditionnight { get; set; }
        /// <summary>
        /// 带电
        /// </summary>
        public string conditionelectrified { get; set; }
        /// <summary>
        /// 悬空
        /// </summary>
        public string conditionair { get; set; }
        /// <summary>
        /// 抢救
        /// </summary>
        public string conditionrescue { get; set; }
        /// <summary>
        /// 不需搭设手架
        /// </summary>
        public string conditionscaffolding { get; set; }
        /// <summary>
        /// 高处坠落
        /// </summary>
        public string fallingobjects { get; set; }
        /// <summary>
        /// 触电
        /// </summary>
        public string electricshock { get; set; }

        /// <summary>
        /// 特殊工种作业人员信息
        /// </summary>
        public List<WorkticketAttachSpecmemberData> workticketAttachSpecmemberDataDtos { get; set; }
        /// <summary>
        /// 作业前安全条件检查
        /// </summary>
        public List<WorkticketAttachConditionlistData> workticketAttachConditionlistDataDtos { get; set; }
    }

    /// <summary>
    /// 检修执行按错单  副票类型代码:15	检修执行按错单
    /// </summary>
    public class JXZXAC_List
    {
        /// <summary>
        /// 操作措施
        /// </summary>
        public string safetymeasurecontent { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public string executor_name { get; set; }
        /// <summary>
        /// 接触人
        /// </summary>
        public string terminationp_name { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string measureorder { get; set; }
    }

    #endregion
}


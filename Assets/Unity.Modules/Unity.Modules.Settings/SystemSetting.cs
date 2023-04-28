using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


[XmlType(TypeName = "SystemSetting")]
public class SystemSetting
{
    /// <summary>
    /// 是否调试模式
    /// 调试模式时：
    ///     1.登陆时显示登陆信息调试界面。
    ///     2.在屏幕上显示帧率。
    ///     3.显示电厂CAD。
    ///     4.对焦一个人员时，高亮当前定位信息关联的基站设备。
    ///     5.历史轨迹时显示实际定位点（放个小球在定位点上）。
    /// 这几点都可以在调试模式参数DebugSetting中再细化，当前相当于默认都是true.
    /// IsDebug是true时，【开发调试页面】才显示出来。
    /// </summary>
    [XmlElement]
    public bool IsDebug = false;
    /// <summary>
    /// 蓝色小球：直接显示定位数据的点位小球
    /// 绿色小球：显示定位数据经过navmesh计算优化后的点位小球
    /// </summary>
    [XmlElement]
    public bool IsShowPosSphere = false;
    [XmlElement]
    public bool IsLocalSceneLoad = false;
    /// <summary>
    /// 屏幕分辨率
    /// </summary>
    [XmlElement]
    public  ResolutionSetting ResolutionSetting; 
    /*todo:
     *     1.加一个页面，显示日志列表，IsDebug是true时显示。
     *     2.加一个位置显示当前是访客模式还是管理模式
     */

    /// <summary>
    /// 版本信息，就一个版本号
    /// </summary>
    [XmlElement]
    public VersionSetting VersionSetting;

    /// <summary>
    /// 是否强制全屏
    /// </summary>
    [XmlElement]
    public bool IsFullScreen ;

    /// <summary>
    /// 是否显示首页
    /// </summary>
    [XmlElement]
    public bool IsShowHomePage=false ;

    /// <summary>
    /// 是否显示首页
    /// </summary>
    [XmlIgnore]
    public bool ShowHomePage
    {
        get
        {
            if (CommunicationObject.Instance && CommunicationObject.Instance.IsGuest()) return false;
            else return IsShowHomePage;
        }
        set { IsShowHomePage = value; }
    }
    /// <summary>
    /// 是否使用新的定位历史服务器获取数据，获取定位历史相关数据。YZL:20211118
    /// </summary>
    [XmlElement]
    public bool IsUseNewHistoryServer = true;

    /// <summary>
    /// 定位卡的同时计算数量
    /// </summary>
    [XmlElement]
    public int CalculateCardNum = 10;
    /// <summary>
    /// 实时人员移动，最大间距，设置为0时，没有缓动效果
    /// </summary>
    [XmlElement]
    public int RealtimeMaxDistance = 15;

    /// <summary>
    /// 实时人员移动最大速度
    /// </summary>
    [XmlElement]
    public float RealMaxSpeed = 10f;

    /// <summary>
    /// 实时人员移动加速度
    /// </summary>
    [XmlElement]
    public float Acceleration = 5f;
    /// <summary>
    /// 是否显示左侧拓补图
    /// </summary>
    [XmlElement]
    public bool IsShowLeftTopo=true;

    /// <summary>
    /// 是否显示右下角告警推送
    /// </summary>
    [XmlElement]
    public bool IsShowRightDownAlarm=true;

    /// <summary>
    /// 是否显示右上角统计信息
    /// </summary>
    [XmlElement]
    public bool IsShowRightTopInfo=true;
    /// <summary>
    /// 是否开启基站报表导出
    /// </summary>
    [XmlElement]
    public bool FileTransferBool = false;
    /// <summary>
    /// 报表下载路径
    /// </summary>
    [XmlElement]
    public string FileTransferSaveDirctory = "D:\\MyAddress";   
    /// <summary>
    /// 是否显示大屏动态灯光
    /// </summary>
    [XmlElement]
    public bool ShowBigScreenLight = true;
    /// <summary>
    /// 是否显示大屏动态灯光
    /// </summary>
    [XmlElement]
    public bool ShowBigScreenRotate = true;


    [XmlElement]
    public string ClientProcessManageIP="127.0.0.1";

    [XmlElement]
    public string ClientProcessManagePort="8746";

    /// <summary>
    /// 画面质量0-3
    /// 0:超低质量；1:低质量；2:中质量；3:高质量；
    /// </summary>
    [XmlElement]
    public int GraphicsQualityInt = 1;
    /// <summary>
    /// 是否开启遮挡剔除
    /// </summary>
    [XmlElement]
    public bool IsDynamicCulling = true;
    /// <summary>
    /// 遮挡剔除精度（可以理解为对象计算个数）
    /// </summary>
    [XmlElement]
    public bool IsAddWhenLoaded = false;

    [XmlElement]
    public int DynamicCullingJobsFrame = 500;
    /// <summary>
    /// 遮挡剔除计算时间间隔
    /// </summary>
    [XmlElement]
    public float DynamicCullingTimeInterval = 0.3f;

    /// <summary>
    /// 遮挡剔除物体保留显示时间
    /// </summary>
    [XmlElement]
    public float DynamicCullingObjectsLifeTime = 16f;

    /// <summary>
    /// 摄像头跟随相关参数
    /// 【后续随着功能调整会没用】
    /// </summary>
    [XmlElement]
    public CinemachineSetting CinemachineSetting;

    /// <summary>
    /// 通信相关参数设置
    /// </summary>
    [XmlElement]
    public CommunicationSetting CommunicationSetting;

    /// <summary>
    /// 刷新间隔时间
    /// 1.实时定位信息
    /// 2.区域统计数据
    /// 3.人员树节点
    /// 4.部门树节点刷新
    /// 5.附近摄像头界面刷新
    /// 6.顶视图截图保存【没什么用，先放着，可以设置成】
    /// </summary>
    [XmlElement]
    public RefreshSetting RefreshSetting;

    /// <summary>
    /// 霍尼维尔视频设置
    /// </summary>
    [XmlElement]
    public HoneyWellSetting HoneyWellSetting;

    /// <summary>
    /// 模型动态加载相关设置
    /// </summary>
    [XmlElement]
    public AssetLoadSetting AssetLoadSetting;

    /// <summary>
    /// 历史轨迹相关设置
    /// </summary>
    [XmlElement]
    public HistoryPathSetting HistoryPathSetting; 

    /// <summary>
    /// 定位相关设置
    /// </summary>
    [XmlElement]
    public LocationSetting LocationSetting;

    /// <summary>
    /// 调试模式相关设置
    /// </summary>
    [XmlElement]
    public DebugSetting DebugSetting;

    /// <summary>
    /// 设备加载相关设置
    /// 【临时调试用，不显示】
    /// </summary>
    [XmlElement]
    public DeviceSetting DeviceSetting;
    /// <summary>
    /// 是否显示告警
    /// </summary>
    [XmlElement]
    public AlarmSetting AlarmSetting;

    [XmlElement]
    public ScanSetting ScanSetting;

    [XmlElement]
    public RoamSetting RoamSetting;

    [XmlElement]
    public SceneLoadSetting SceneLoadSetting;

    public SystemSetting()
    {
        ResolutionSetting = new ResolutionSetting();
        CinemachineSetting = new CinemachineSetting();
        CommunicationSetting = new CommunicationSetting();
        VersionSetting = new VersionSetting();
        RefreshSetting = new RefreshSetting();
        AssetLoadSetting = new AssetLoadSetting();
        HoneyWellSetting = new HoneyWellSetting();
        DeviceSetting = new DeviceSetting();
        HistoryPathSetting = new HistoryPathSetting();
        LocationSetting = new LocationSetting();
        DebugSetting = new DebugSetting();
        AlarmSetting = new AlarmSetting();
        ScanSetting = new ScanSetting();
        RoamSetting = new RoamSetting();
        SceneLoadSetting = new SceneLoadSetting();
    }
}


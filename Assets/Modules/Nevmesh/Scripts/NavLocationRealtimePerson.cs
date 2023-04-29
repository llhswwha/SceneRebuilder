using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 实时定位人员控制脚本
/// </summary>
public class NavLocationRealtimePerson : NavAgentControllerBase
{
    /// <summary>
    /// 定位卡信息
    /// </summary>
    public Tag ttag;

    public LocationObject locationObjectT;

    public Transform followTarget;

    public CharacterController characterController;



    void Start()
    {
        base.Start();
        MovePerson();
        InitNavMeshAgent();

        characterController = gameObject.GetComponent<CharacterController>();
        if (characterController)
        {
            characterController.enabled = false;//老的代码需要，新的并不需要，还会导致上下楼梯时有点卡顿。
        }

        //InvokeRepeating("UpdatePosition",0,0.3f);//这里这样写的话，走的路径就有问题，奇怪
    }
    private void OnEnable()
    {
        Log.Info("NavAgentFollowPerson.OnEnable", "" + this.name);
        isOnEnableWrap = true;
        MovePerson();
        if (SystemSettingHelper.systemSetting.IsShowPosSphere)
        {
            if (posSphere != null)
            {
                posSphere.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        Log.Info("NavAgentFollowPerson.OnDisable", "" + this.name);
        isPosInfoSet = false;
        DoRealTimePosEndSet();
        if (SystemSettingHelper.systemSetting.IsShowPosSphere)
        {
            if (posSphere != null)
            {
                posSphere.SetActive(false);
            }
        }
    }

    public void OnDestroy()
    {
        base.OnDestroy();
        if (SystemSettingHelper.systemSetting.IsShowPosSphere)
        {
            if (posSphere != null)
            {
                GameObject.Destroy(posSphere);
            }
        }
    }

//     protected override void InitNavMeshAgent()
//     {
//         if (agent == null)//这个必须加上，不如会一直进来的
//         {
//             base.InitNavMeshAgent();
//         }
//         else
//         {
// #if UNITY_EDITOR
//             base.InitNavMeshAgent();
// #endif
//         }

//     }

    public bool FirstUpdate = true;

    void Update()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        if (FirstUpdate)
        {

            FirstUpdate = false;
        }

        //if(ttag.Code=="556D")
        //{
        //    int I = 0;
        //}
        if (LocationManager.Instance.IsActiveDoWaitTags)
        {
            if (locationObjectT != null)//&& locationObjectT.isDoPositon
            {
                if (isDoRealTimePos)
                {
                    if (isCanSetRealTimePos)
                    {
                        if (this is NavLocationRealtimePerson)
                        {
                            //NavLocationRealtimePerson p = this as NavLocationRealtimePerson;
                            if (locationObjectT.name.Contains("55A4"))
                            {
                                int I = 0;
                            }
                        }
                        isCanSetRealTimePos = false;
                        UpdatePosition(() =>
                        {
                            UpdateSpeed();
                        });
                    }
                }
            }
            else
            {
                DoRealTimePosEndSet();
            }
        }
        else
        {
            UpdatePosition(() =>
            {
                UpdateSpeed();
            });
        }
    }

    //void Update()
    //由Update转为FixedUpdate原因:Update会因为项目卡不卡，影响刷新次数，以及Update下时间增量不固定（Time.deltaTime），而FixedUpdate固定刷新次数和时间增量（0.02）
    void FixedUpdate()
    {
        //if (gameObject.activeInHierarchy == false)
        //{
        //    return;
        //}
        //if (FirstUpdate)
        //{

        //    FirstUpdate = false;
        //}

        //UpdatePosition(() =>
        //{
        //    UpdateSpeed();
        //});
    }

    // protected override void UpdateSpeed()
    // {
    //     base.UpdateSpeed();
    // }

    public void SetFollowTarget(Transform target)
    {
        Debug.Log("SetDestination First");
        followTarget = target;
        gameObject.SetActive(true);
    }

    // protected override void SetDestination(Vector3 pos, Action<bool> callback)
    // {
    //     LocationManager.Instance.processCtestctime(locationObjectT.Tag.Code, "SetDestination01");

    //     //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
    //     if (lastPos == pos)//坐标不变，不用计算处理。
    //     {
    //         //if (locationObjectT.Tag.Code == "6848")
    //         //{
    //         //    LocationManager.Instance.processCtestctime(locationObjectT.Tag.Code, "SetDestination lastPos == pos");
    //         //    Debug.LogError("lastPos == pos!!!");
    //         //}
    //         if (callback != null)
    //         {
    //             callback(false);
    //         }
    //         //LocationManager.Instance.removeCtestctime(locationObjectT.Tag.Code);
    //         DoRealTimePosEndSet();
    //         return;
    //     }
    //     //if (locationObjectT.Tag.Code == "6848")
    //     //{
    //     //    Debug.LogError("SetDestination___0002");
    //     //    Debug.LogError("SetDestination___0002:lastPos1" + lastPos);
    //     //}

    //     lastPos = pos;

    //     //LocationManager.Instance.processCtestctime(locationObjectT.Tag.Code, "SetDestination02");
    //     //Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
    //     if (IsBusyUpdatePosition)
    //     {
    //         //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
    //         if (callback != null)
    //         {
    //             callback(false);
    //         }
    //         DoRealTimePosEndSet();
    //         return;
    //     }
    //     LocationManager.Instance.processCtestctime(locationObjectT.Tag.Code, "SetDestination03");
    //     //DateTime start = DateTime.Now;
    //     IsBusyUpdatePosition = true;
    //     DateTime sta = DateTime.Now;
    //     NavMeshHelper.GetClosetPointAsync(pos, this.name, agent, (destination, p) => //多线程异步方式计算,避免影响性能
    //     {
    //         double a = (DateTime.Now - sta).TotalMilliseconds;
    //         //Debug.Log(this.name + "_时间花费1：" + a);
    //         SetPos(p, destination, pos, callback);
    //         double b = (DateTime.Now - sta).TotalMilliseconds;
    //         //Debug.Log(this.name + "_时间花费2：" + b);
    //         LocationManager.Instance.removeCtestctime(locationObjectT.Tag.Code);
    //         //Debug.LogError(this.name + "总计算时间花费：" + (DateTime.Now - tagposInfo.TagPos.CSdateTime).TotalMilliseconds+ "毫秒");
    //         DoRealTimePosEndSet();
    //         //if (locationObjectT.Tag.Code == "6848")
    //         //{
    //         //    Debug.LogError(this.name + "6848_时间花费2：" + b + ",卡时间：" + tagposInfo.TagPos.DateTime);
    //         //    Debug.LogError(this.name + "卡数据：（" + tagposInfo.TagPos.X + "," + tagposInfo.TagPos.Y + "," + tagposInfo.TagPos.Z + ")");
    //         //}

    //         // 创建高亮测试小球：显示定位数据经过navmesh计算优化后的点位小球
    //         if (SystemSettingHelper.systemSetting.IsShowPosSphere)
    //         {
    //             ShowNavPositionSphereTest(destination);
    //         }
    //     });
    // }


    public GameObject posSphere;
    public HightlightModuleBase posSphereHighlighter;
    public static Transform NavPositionSphereParent;
    /// <summary>
    /// 创建高亮测试小球显示人员计算优化后的真实位置
    /// </summary>
    public void ShowNavPositionSphereTest(Vector3 p)
    {
        if (LocationManager.Instance.testSphere == null) return;
        if (locationObjectT != null && locationObjectT.Tag != null)
        {
            if (NavPositionSphereParent == null)
            {
                GameObject o = new GameObject();
                o.name = "NavPositionSphereParent";
                NavPositionSphereParent = o.transform;
            }
            if (posSphere == null)
            {
                //posSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                posSphere = Instantiate(LocationManager.Instance.testSphere);
                posSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                if (locationObjectT.Tag != null)
                {
                    posSphere.name = locationObjectT.Tag.Code;
                }
                posSphere.transform.SetParent(NavPositionSphereParent);
            }

            if (posSphereHighlighter == null)
            {
                posSphereHighlighter = posSphere.AddMissingComponent<HightlightModuleBase>();
            }
            posSphereHighlighter.ConstantOn(Color.green);
            posSphere.transform.position = p;
            posSphere.GetComponent<Renderer>().material.color = Color.green;
            //if (Tag.Code == "0988")
            //{
            //    Debug.LogError("Code0988:" + p);
            //}
        }
    }

    /// <summary>
    /// 设置Navmesh位置
    /// </summary>
    /// <param name="p"></param>
    /// <param name="destination"></param>
    /// <param name="pos"></param>
    /// <param name="callback"></param>
    private void SetPos(GameObject p,Vector3 destination,Vector3 pos,Action<bool> callback)
    {
        try
        {
            //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}=>{1},{2}", pos,destination, this));
            if (SystemSettingHelper.IsDebug())//调试模式下，查看接下来的移动方向
            {
                // if (ArcManager.Instance != null)
                // {
                //     if (p != null)
                //         ArcManager.Instance.SetLine(gameObject.transform, p.transform);
                // }
            }

            if (agent != null)
            {
                if (agent.gameObject.activeInHierarchy)
                {
                    if (EnableUpdate == false)//代表第一次
                    {
                        Log.Info("NavAgentControllerBase.SetDestination", string.Format("First Wrap!! pos:{0},obj:{1}", pos, this));
                        agent.Warp(destination);
                    }
                    else
                    {
                        bool r = true;
                        //yzl:20201012
                        if (navMeshPath == null)
                        {
                            navMeshPath = new NavMeshPath();
                        }
                        // Vector3 dirTemp = Vector3.zero;
                        // velocityMagnitude = PathFindingManager.Instance.RealMaxSpeed;
                        // r = GetMoveVelocity(destination, ref velocityMagnitude);

                        // if (navMeshPath.corners.Length >= 2)
                        // {
                        //     velocityMagnitude = velocityMagnitude > PathFindingManager.Instance.RealMaxSpeed ? PathFindingManager.Instance.RealMaxSpeed : velocityMagnitude;

                        //     dirTemp = (navMeshPath.corners[1] - navMeshPath.corners[0]).normalized;
                        //     float velocityMagnitudeT = velocityMagnitude * PathFindingManager.Instance.RealSpeedSca;//速度大小
                        //     agent.speed = velocityMagnitudeT;//设置在路线上的最大移动速度
                        //     //Vector3 velocityTemp = velocityMagnitudeT * dirTemp;
                        //     //agent.velocity = velocityTemp;//这里需要设置当前速度agent.velocity，如果只设置agent.speed会从当前速度慢慢加速到目标速度
                        // }
                        //Debug.LogError("agent.SetDestination:" + destination);
                        r = agent.SetDestination(destination);//Agent被关闭或者被销毁，调用这个方法会报错

                        //第一次计算不在navmesh上，通过射线修正一下位置 wk20221122
                        if (!r)
                        {
                            destination = NavMeshHelper.GetSamplePosition(destination);
                            r = agent.SetDestination(destination);
                        }

                        if (r == false)//人物物体不在NavMesh上，立刻跳到目标位置
                        {
                            Log.Info("NavAgentControllerBase.SetDestination", string.Format("Wrap pos:{0},obj:{1}", pos, this));
                            //this.transform.position = destination;
                            agent.Warp(destination);//要用这个,用上面那句话，"不在NavMesh上"的问题会一直出现，要用Warp才会重新计算
                        }
                        else
                        {
                            // HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
                            // if (hisPosInfo != null)
                            // {
                            //     var current = hisPosInfo.CurrentPosInfo;
                            //     var next = current.Next;

                            //     if (next != null)
                            //     {
                            //         TimeSpan t = next.Time - current.Time; //下一个点的时间
                            //                                                //float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

                            //         //Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));
                            //         this.timespan = t.TotalSeconds;//两点时间差，用于计算两点间移动速度
                            //     }

                            // }
                        }
                    }

                    EnableUpdate = true;
                }
                else
                {
                    Log.Error("NavAgentControllerBase.SetDestination", "agent.gameObject.activeInHierarchy==false:" + this);
                }
            }
            else
            {
                Log.Error("NavAgentControllerBase.SetDestination", "agent==null:" + this);
            }
        }
        catch (Exception ex)
        {
            Log.Error("NavAgentControllerBase.SetDestination", "Exception:" + ex);
        }
        finally
        {
            IsBusyUpdatePosition = false;
            if (callback != null)
            {
                callback(false);
            }

        }
    }

    //private bool GetMoveVelocity(Vector3 destination,ref float velocityMagnitudeTT)
    //{
    //    NavMesh.CalculatePath(transform.position, destination, -1, navMeshPath);
    //    //在上一个SetDestination未到达目标点时，又执行SetDestination到下个目标点，会有速度和方向缓慢调整的过程（会导致人员偏离轨迹）
    //    if (navMeshPath.corners.Length >= 2)
    //    {
    //        float disTT = 0;
    //        for (int j = 0; j < navMeshPath.corners.Length; j++)
    //        {
    //            if (j >= 1)
    //            {
    //                disTT += Vector3.Distance(navMeshPath.corners[j - 1], navMeshPath.corners[j]);
    //                if (SystemSettingHelper.IsDebug())
    //                {
    //                    Vector3 startpos = navMeshPath.corners[j - 1];
    //                    Vector3 endpos = navMeshPath.corners[j];
    //                    Debug.DrawLine(startpos, endpos, Color.red, 0.3f);
    //                }
    //            }
    //        }
    //        HisPosInfo hisPosInfoT = posInfo as HisPosInfo;
    //        if (hisPosInfoT.CurrentPosInfo != null && hisPosInfoT.PrePosInfo != null)
    //        {
    //            timeInterval = (float)(hisPosInfoT.CurrentPosInfo.Time - hisPosInfoT.PrePosInfo.Time).TotalSeconds;
    //            if (timeInterval != 0)
    //            {
    //                velocityMagnitudeTT = disTT / timeInterval;
    //            }
    //            else
    //            {
    //                velocityMagnitudeTT = PathFindingManager.Instance.HisMaxSpeed;
    //            }

    //            //velocityMagnitudeTT = velocityMagnitudeTT > PathFindingManager.Instance.HisMaxSpeed ? PathFindingManager.Instance.HisMaxSpeed : velocityMagnitudeTT;
    //            //velocityMagnitudeTT = velocityMagnitudeTT < PathFindingManager.Instance.HisMinSpeed ? PathFindingManager.Instance.HisMinSpeed : velocityMagnitudeTT;
    //        }

    //    }
    //    else
    //    {
    //        return false;
    //    }

    //    return true;
    //}
}

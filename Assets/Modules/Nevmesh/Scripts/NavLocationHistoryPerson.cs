using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 历史轨迹演练人员控制脚本
/// </summary>
public class NavLocationHistoryPerson : NavAgentControllerBase
{
    public Transform followTarget;

    public CharacterController characterController;
    //NavMeshPath navMeshPath;
    protected Quaternion targetQuaternion;//目标转向

    public GameObject posSphere;
    public HightlightModuleBase posSphereHighlighter;
    public static Transform NavPositionSphereParent;

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
        if (SystemSettingHelper.systemSetting.IsDebug)
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
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            if (posSphere != null)
            {
                posSphere.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            if (posSphere != null)
            {
                GameObject.Destroy(posSphere);
            }
        }
    }

    protected override void InitNavMeshAgent()
    {
        if (agent == null)//这个必须加上，不如会一直进来的
        {
            base.InitNavMeshAgent();
        }

    }

    public bool FirstUpdate = true;

    //void Update()
    //由Update转为FixedUpdate原因:Update会因为项目卡不卡，影响刷新次数，以及Update下时间增量不固定（Time.deltaTime），而FixedUpdate固定刷新次数和时间增量（0.02）
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

        UpdatePosition(() =>
        {
            UpdateSpeed();
        });

    }

    protected override void UpdateSpeed()
    {
        base.UpdateSpeed();
    }

    public void SetFollowTarget(Transform target)
    {
        Debug.Log("SetDestination First");
        followTarget = target;
        gameObject.SetActive(true);
    }

    private bool isNewSetDestination = false;//是否开启历史轨迹新的移动方法
    //private Vector3 preDestination = new Vector3(-1, -1, -1);
    protected override void SetDestination(Vector3 pos, Action<bool> callback)
    {
        if (isNewSetDestination)
        {
            SetDestinationNew(pos, callback);
        }
        else
        {
            SetDestinationOld(pos, callback);
        }
    }

    /// <summary>
    /// 直接赋值， 测试高倍数历史轨迹移动（流畅），YZL20210423
    /// 解决了老方法的高倍数轨道偏离，和跟不上问题
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="callback"></param>
    protected void SetDestinationNew(Vector3 pos, Action<bool> callback)
    {
        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
        if (lastPos == followTarget.transform.position)//坐标不变，不用计算处理。
        {
            //StopPerson();
            if (callback != null)
            {
                callback(false);
            }
            return;
        }

        //MovePerson();

        //lastPos = pos;

        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
        if (IsBusyUpdatePosition)
        {
            //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
            if (callback != null)
            {
                callback(false);
            }
            return;
        }
        DateTime start = DateTime.Now;
        EnableUpdate = true;

        SetRotaion(followTarget.transform.position);
        bool isWarp = agent.Warp(followTarget.transform.position);
        if (!isWarp)
        {
            agent.transform.position = (followTarget.transform.position);

        }
        lastPos = followTarget.transform.position;

        //IsBusyUpdatePosition = true;
        //NavMeshHelper.GetClosetPointAsync(followTarget.transform.position, this.name, agent, (destination, p) =>
        //{
        //    SetRotaion(destination);
        //    //bool isWarp2 = agent.Warp(destination);
        //    //if (!isWarp2)
        //    //{
        //    //agent.transform.position = destination;
        //    //}
        //    destinationPos = destination;
        //    IsBusyUpdatePosition = false;
        //    //lastPos = followTarget.transform.position;
        //    lastPos = destination;
        //    //HistoryManController historyManT = gameObject.GetComponentInParent<HistoryManController>();
        //    //if (historyManT != null)
        //    //{
        //    //    historyManT.followUI.GetComponent<UGUIFollowTarget>().LateUpdate();
        //    //}
        //});
    }

    protected void SetDestinationOld(Vector3 pos, Action<bool> callback)
    {
        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
        if (lastPos == pos)//坐标不变，不用计算处理。
        {
            if (callback != null)
            {
                callback(false);
            }
            return;
        }

        lastPos = pos;

        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
        if (IsBusyUpdatePosition)
        {
            //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
            if (callback != null)
            {
                callback(false);
            }
            return;
        }
        DateTime start = DateTime.Now;
        IsBusyUpdatePosition = true;
        NavMeshHelper.GetClosetPointAsync(pos, this.name, agent, (destination, p) => //多线程异步方式计算,避免影响性能
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
                            Vector3 dirTemp = Vector3.zero;
                            r = GetMoveVelocity(destination, ref velocityMagnitude);

                            // if (navMeshPath.corners.Length >= 2)
                            // {
                            //     float hismaxspeed = PathFindingManager.Instance.HisMaxSpeed;
                            //     velocityMagnitude = velocityMagnitude > hismaxspeed ? hismaxspeed : velocityMagnitude;
                            //     velocityMagnitude = velocityMagnitude < PathFindingManager.Instance.HisMinSpeed ? PathFindingManager.Instance.HisMinSpeed : velocityMagnitude;
                            //     dirTemp = (navMeshPath.corners[1] - navMeshPath.corners[0]).normalized;
                            //     //Vector3 velocityTemp = agent.velocity.magnitude * dirTemp;//设置移动方向
                            //     float velocityMagnitudeT = velocityMagnitude * rate * PathFindingManager.Instance.HisSpeedSca;//速度大小
                            //     if (rate > 4)//8倍速的时候速度还是太快，会导致顿卡
                            //     {
                            //         velocityMagnitudeT = velocityMagnitudeT * 0.6f;
                            //     }
                            //     else if (rate > 2)//4倍速优化
                            //     {
                            //         velocityMagnitudeT = velocityMagnitudeT * 0.8f;
                            //     }
                            //     Vector3 velocityTemp = velocityMagnitudeT * dirTemp;
                            //     //agent.speed = velocityMagnitude;//设置在路线上的最大移动速度
                            //     agent.speed = velocityMagnitudeT;//设置在路线上的最大移动速度
                            //     agent.velocity = velocityTemp;//这里需要设置当前速度agent.velocity，如果只设置agent.speed会从当前速度慢慢加速到目标速度
                            //     //agent.
                            // }
                            // else
                            // {
                            //     Debug.LogError("navMeshPath.corners.Length < 2");
                            // }


                            //需要SetDestination一下，不然设置agent.velocity速度，会出现人员在OffMeshLink组件连接附近来回摆动现象（速度太快过了可以传送的点，使其来回纠正）
                            r = agent.SetDestination(destination);

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

                    // 创建高亮测试小球：显示定位数据经过navmesh计算优化后的点位小球
                    if (SystemSettingHelper.systemSetting.IsDebug)
                    {
                        ShowNavPositionSphereTest(destination);
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
                TimeSpan time = DateTime.Now - start;

                if (callback != null)
                {
                    callback(false);
                }

            }

            //Log.Info("NavAgentControllerBase.UpdatePosition", NavMeshHelper.Log);
            //Log.Info("NavAgentControllerBase.SetDestination", "UpdatePosition End time:" +time.TotalMilliseconds+"ms");
        });

    }

    //protected override void SetDestination(Vector3 pos, Action<bool> callback)
    //{
    //    //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
    //    if (lastPos == pos)//坐标不变，不用计算处理。
    //    {
    //        if (callback != null)
    //        {
    //            callback(false);
    //        }
    //        return;
    //    }

    //    lastPos = pos;

    //    //Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
    //    if (IsBusyUpdatePosition)
    //    {
    //        //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
    //        if (callback != null)
    //        {
    //            callback(false);
    //        }
    //        return;
    //    }
    //    DateTime start = DateTime.Now;
    //    IsBusyUpdatePosition = true;
    //    NavMeshHelper.GetClosetPointAsync(pos, this.name, agent, (destination, p) => //多线程异步方式计算,避免影响性能
    //    {
    //        try
    //        {
    //            //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}=>{1},{2}", pos,destination, this));
    //            if (SystemSettingHelper.IsDebug())//调试模式下，查看接下来的移动方向
    //            {
    //                if (ArcManager.Instance != null)
    //                {
    //                    if (p != null)
    //                        ArcManager.Instance.SetLine(gameObject.transform, p.transform);
    //                }
    //            }

    //            if (agent != null)
    //            {
    //                if (agent.gameObject.activeInHierarchy)
    //                {
    //                    if (EnableUpdate == false)//代表第一次
    //                    {
    //                        Log.Info("NavAgentControllerBase.SetDestination", string.Format("First Wrap!! pos:{0},obj:{1}", pos, this));
    //                        agent.Warp(destination);
    //                    }
    //                    else
    //                    {
    //                        bool r = true;
    //                        //yzl:20201012
    //                        if (navMeshPath == null)
    //                        {
    //                            navMeshPath = new NavMeshPath();
    //                        }
    //                        NavMesh.CalculatePath(transform.position, destination, -1, navMeshPath);
    //                        //在上一个SetDestination未到达目标点时，又执行SetDestination到下个目标点，会有速度和方向缓慢调整的过程（会导致人员偏离轨迹）
    //                        Vector3 dirTemp = Vector3.zero;
    //                        if (navMeshPath.corners.Length >= 2)
    //                        {
    //                            dirTemp = (navMeshPath.corners[1] - navMeshPath.corners[0]).normalized;
    //                            float disTT =0 ;
    //                            for (int j = 0; j < navMeshPath.corners.Length; j++)
    //                            {
    //                                if (j >= 1)
    //                                {
    //                                    disTT += Vector3.Distance(navMeshPath.corners[j - 1], navMeshPath.corners[j]);
    //                                    if (SystemSettingHelper.IsDebug())
    //                                    {
    //                                        Vector3 startpos = navMeshPath.corners[j - 1];
    //                                        Vector3 endpos = navMeshPath.corners[j];
    //                                        Debug.DrawLine(startpos, endpos, Color.red, 0.3f);
    //                                    }
    //                                }
    //                            }
    //                            HisPosInfo hisPosInfoT = posInfo as HisPosInfo;
    //                            if (hisPosInfoT.CurrentPosInfo != null && hisPosInfoT.PrePosInfo != null)
    //                            {
    //                                timeInterval = (float)(hisPosInfoT.CurrentPosInfo.Time - hisPosInfoT.PrePosInfo.Time).TotalSeconds;
    //                                if (timeInterval != 0)
    //                                {
    //                                    velocityMagnitude = disTT / timeInterval;
    //                                }
    //                                else
    //                                {
    //                                    velocityMagnitude = PathFindingManager.Instance.HisMaxSpeed;
    //                                }

    //                                velocityMagnitude = velocityMagnitude > PathFindingManager.Instance.HisMaxSpeed ? PathFindingManager.Instance.HisMaxSpeed : velocityMagnitude;
    //                                velocityMagnitude = velocityMagnitude < PathFindingManager.Instance.HisMinSpeed ? PathFindingManager.Instance.HisMinSpeed : velocityMagnitude;
    //                            }

    //                            //Vector3 velocityTemp = agent.velocity.magnitude * dirTemp;//设置移动方向
    //                            float velocityMagnitudeT = velocityMagnitude * rate * speedSca;//速度大小
    //                            Vector3 velocityTemp = velocityMagnitudeT * dirTemp;
    //                            //agent.speed = velocityMagnitude;//设置在路线上的最大移动速度
    //                            agent.speed = velocityMagnitudeT;//设置在路线上的最大移动速度
    //                            agent.velocity = velocityTemp;//这里需要设置当前速度agent.velocity，如果只设置agent.speed会从当前速度慢慢加速到目标速度
    //                        }
    //                        else
    //                        {
    //                            //dirTemp = (destination - agent.transform.position).normalized;//获取移动方向
    //                            r = false;
    //                        }

    //                        //bool r = agent.SetDestination(destination);//Agent被关闭或者被销毁，调用这个方法会报错

    //                        //需要SetDestination一下，不然设置agent.velocity速度，会出现人员在OffMeshLink组件连接附近来回摆动现象（速度太快过了可以传送的点，使其来回纠正）
    //                        r = agent.SetDestination(destination);

    //                        if (r == false)//人物物体不在NavMesh上，立刻跳到目标位置
    //                        {
    //                            Log.Info("NavAgentControllerBase.SetDestination", string.Format("Wrap pos:{0},obj:{1}", pos, this));
    //                            //this.transform.position = destination;
    //                            agent.Warp(destination);//要用这个,用上面那句话，"不在NavMesh上"的问题会一直出现，要用Warp才会重新计算
    //                        }
    //                        else
    //                        {
    //                            HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
    //                            if (hisPosInfo != null)
    //                            {
    //                                var current = hisPosInfo.CurrentPosInfo;
    //                                var next = current.Next;

    //                                if (next != null)
    //                                {
    //                                    TimeSpan t = next.Time - current.Time; //下一个点的时间
    //                                                                           //float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

    //                                    //Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));
    //                                    this.timespan = t.TotalSeconds;//两点时间差，用于计算两点间移动速度
    //                                }

    //                            }
    //                        }
    //                    }

    //                    EnableUpdate = true;
    //                }
    //                else
    //                {
    //                    Log.Error("NavAgentControllerBase.SetDestination", "agent.gameObject.activeInHierarchy==false:" + this);
    //                }
    //            }
    //            else
    //            {
    //                Log.Error("NavAgentControllerBase.SetDestination", "agent==null:" + this);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error("NavAgentControllerBase.SetDestination", "Exception:" + ex);
    //        }
    //        finally
    //        {
    //            IsBusyUpdatePosition = false;
    //            TimeSpan time = DateTime.Now - start;

    //            if (callback != null)
    //            {
    //                callback(false);
    //            }

    //        }

    //        //Log.Info("NavAgentControllerBase.UpdatePosition", NavMeshHelper.Log);
    //        //Log.Info("NavAgentControllerBase.SetDestination", "UpdatePosition End time:" +time.TotalMilliseconds+"ms");
    //    });

    //}

    protected void LateUpdate()
    {
        // if (MultHistoryPanel.Instance.isPlay)
        // {
        //     //缓慢转动到目标点
        //     transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.fixedDeltaTime * 10);
        // }
    }


    /// <summary>
    /// 设置旋转角度
    /// </summary>
    /// <param name="targetPos"></param>
    protected void SetRotaion(Vector3 targetPos)
    {
        //Vector3 dir = targetPos - transform.position;//lastPos
        Vector3 dir = targetPos - lastPos;
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            targetQuaternion = Quaternion.LookRotation(dir, Vector3.up); //targetQuaternion在LateUpdate里面缓慢旋转
            //缓慢转动到目标点
            //transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }
    }

    /// <summary>
    /// 创建高亮测试小球显示人员计算优化后的真实位置
    /// </summary>
    public void ShowNavPositionSphereTest(Vector3 p)
    {
        if (LocationManager.Instance.testSphere == null) return;
        //if (posInfo == null) return;

        //TagPosInfo tp = posInfo as TagPosInfo;
        if (posInfo != null)
        {
            if (NavPositionSphereParent == null)
            {
                GameObject o = new GameObject();
                o.name = "NavHisPositionSphereParent";
                NavPositionSphereParent = o.transform;
            }
            if (posSphere == null)
            {
                //posSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                posSphere = Instantiate(LocationManager.Instance.testSphere);
                posSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //if (tp.TagPos.Tag != null)
                //{
                    posSphere.name = gameObject.name;
                //}
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
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentControllerBase : MonoBehaviour
{

    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;

    private Vector3 personSize;

    public float speedPower = 0.5f;
    public float speedSca = 1F;//速度系数
    public float timeInterval;

    public float distance = 0;
    public bool isDestroy = false;

    public float velocityMagnitude;//速度大小

    protected NavMeshPath navMeshPath;

    protected PersonAnimationController personAnimationController;

    public float MaxDistance
    {
        get
        {
            // if (PathFindingManager.Instance)
            // {
            //     //if (tagposInfo != null && tagposInfo.TagPos != null && tagposInfo.TagPos.IsGps)//考虑GPS定位数据跳动可能比较大；YZL20221024
            //     //{
            //     //    return PathFindingManager.Instance.GPSMaxDistance;
            //     //}
            //     //else
            //     //{
            //         if (this is NavLocationHistoryPerson)
            //         {
            //             return PathFindingManager.Instance.HisMaxDistance;
            //         }
            //         else
            //         {
            //             return PathFindingManager.Instance.MaxDistance;
            //         }
            //     //}
            // }
            // else
            {
                return 3;
            }
        }
    }

    public bool enableJump = true;

    //public bool useWrap = true;

    public double timespan = 1;

    public bool isPosInfoSet;//位置信息是否设置
    public bool isOnEnableWrap;//是否在激活时wrap（处理：人从一楼激活，慢慢走到二楼的情况）

    //private float _maxSpeed;
    //NavMeshAgent移动最大速度
    private float MaxSpeed
    {
        //get
       // {
            //if (PathFindingManager.Instance)
            //{
            //    return PathFindingManager.Instance.MaxSpeed;
            //}
            //else
            //{
            //    return 3;
            //}
        //    if (_maxSpeed == 0) return 3;
        //    return _maxSpeed;
        //}
       // set
       // {
       //     _maxSpeed = value;
       // }
	   
	           get
        {
            // if (PathFindingManager.Instance)
            // {
            //     return PathFindingManager.Instance.HisMaxSpeed;
            // }
            // else
            {
                return 3;
            }
        } 
    }
    //private float _minSpeed;
    //NavMeshAgent移动最大速度
    private float MinSpeed 
     {
        //get
        //{
            //            if (PathFindingManager.Instance)
            //            {
            //                return PathFindingManager.Instance.MinSpeed;
            //}
            //            else
            //            {
            //                return 1f;
            //            }
         //   if (_minSpeed == 0) return 1;
         //   return _minSpeed;
        //}
       // set
        //{
        //    _minSpeed = value;
       // }
	   
	           get
        {
            // if (PathFindingManager.Instance)
            // {
            //     return PathFindingManager.Instance.HisMinSpeed;
            // }
            // else
            {
                return 1f;
            }
        } 
    }
    private bool _useShowPos;

    public void SetMinMaxInfo(float maxDisT,float maxSpeedT,float minSpeedT,bool useShowPosT)
    {
        //_maxDistance = maxDisT;
        //_maxSpeed = maxSpeedT;
        //_minSpeed = minSpeedT;
       // _useShowPos = useShowPosT;
    }

    public void Start()
    {
        personSize = gameObject.GetSize();
    }

    /// <summary>
    /// 根据两点间距离，设置移动速度
    /// </summary>
    /// <param name="dis"></param>
    protected void SetSpeed(float dis)
    {
        if(timespan<0)timespan = Mathf.Abs((float)timespan);//防止出现负数，导致人物停止
        double speed = dis * speedPower * rate / timespan;//根据距离调整速度
        if(speed>MaxSpeed)
        {
            speed = MaxSpeed;
        }
        if (speed < MinSpeed)
        {
            speed = MinSpeed;
        }
        agent.speed = (float)speed;
    }

    protected void SetSpeedByDistance()
    {
        if (targetPos == Vector3.zero) return;//总不会那么巧，目标位置就是原点吧。
        //bool r = agent.SetDestination(followTarget.position);
        if (!isPosInfoSet) return;
        distance = Vector3.Distance(targetPos, transform.position);

        //这里设置速度有点问题,先注释（yzl：20201012）
        //SetSpeed(distance);

        if (enableJump)
        {
            if (distance > MaxDistance||isOnEnableWrap)//距离太远了，直接飞过去
            {

                //if (useWrap)
                //{
                //Vector3 destination = GetDestination(targetPos);
                ////Vector3 destination = NavMeshHelper.GetClosetPointEx(targetPos, agent);
                //if (destination == Vector3.zero)
                //{

                //}
                //else
                //{
                //    var r=agent.Warp(destination);//要用这个
                //    Log.Info("NavAgentControllerBase.SetSpeedByDistance", string.Format("Jump(Warp) distance:{0}|name:{1}|destination:{2}|wrap:{3}|isOnEnableWrap:{4}", 
                //        distance , this.name , destination , r, isOnEnableWrap));
                //}

                // NavMeshHelper.GetClosetPointAsync(targetPos, this.name, agent, (destination, o) =>
                // {
                //     var r = agent.Warp(destination);//要用这个
                //     //Log.Info("NavAgentControllerBase.SetSpeedByDistance", string.Format("Jump(Warp) distance:{0}|name:{1}|destination:{2}|wrap:{3}|isOnEnableWrap:{4}|MaxDistance:{5}",
                //     //    distance, this.name, destination, r, isOnEnableWrap, MaxDistance));
                // });

                isOnEnableWrap = false;
            }
        }
    }

    protected virtual void UpdateSpeed()
    {
        if (isDestroy) return;
        if (gameObject.activeInHierarchy)
        {
            SetSpeedByDistance();
        }
    }

    protected virtual void InitNavMeshAgent()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                //agent.radius = 0.1f;

                //半径小一点，可以避免两个agent移动到同一目标点时，出现相互推开和打转现象，
                //不过半径越小会影响wrap方法，所以要适当，这里测试过0.06可以
                agent.radius = 0.05f;

                agent.height = 0.8f;
            }
            agent.updateRotation = true;
            if (this is NavLocationRealtimePerson)
            {
                //agent.speed = PathFindingManager.Instance.RealSpeed;
                // agent.angularSpeed = PathFindingManager.Instance.angularSpeed;
                // agent.acceleration = PathFindingManager.Instance.Acceleration;
            }
            else
            {
                agent.angularSpeed = 360f;
            }

        }
        else
        {
#if UNITY_EDITOR
            if (this is NavLocationRealtimePerson)
            {
                //agent.speed = PathFindingManager.Instance.RealSpeed;
                // agent.angularSpeed = PathFindingManager.Instance.angularSpeed;
                // agent.acceleration = PathFindingManager.Instance.Acceleration;
            }
#endif
        }

    }

    public Vector3 samplePos;

    public Vector3 edgePos;

    public Vector3 castPos;

    /// <summary>
    /// 获取有效目标点
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected Vector3 GetDestination(Vector3 targetPos)
    {
        Vector3 destination = targetPos;
        InitNavMeshAgent();
        return NavMeshHelper.GetClosetPoint(destination,agent);
    }

    private Dictionary<string,GameObject> posBuffer=new Dictionary<string, GameObject>();

//     private void ShowPoint(string posName,Vector3 pos,Color color)
//     {
//         try
//         {
//             if (!posBuffer.ContainsKey(posName))
//             {
//                 GameObject posObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                 posObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
//                 posObj.GetComponent<Renderer>().material.color = color;
//                 posObj.SetTransparent(0.5f);
//                 posObj.name = posName;
//                 GameObject.DestroyImmediate(posObj.GetComponent<Collider>());
//                 posBuffer.Add(posName, posObj);
//             }

//             GameObject obj = posBuffer[posName];
//             obj.transform.position = pos;
//             obj.HighlightOn();
//         }
//         catch (System.Exception ex)
//         {
//             Log.Error("NavAgentControllerBase.ShowDestination", ex.ToString());
//         }
       
//     }

    public Vector3 targetPos = Vector3.zero;

    public float rate = 1;

    //public virtual void SetDestination(Vector3 targetPos, float rateT)
    //{
    //    if (gameObject.activeInHierarchy == false) return;
    //    this.rate = rateT;
    //    this.targetPos = targetPos;
    //    Vector3 destination = GetDestination(targetPos);
    //    agent.SetDestination(destination);
    //}

    //private DateTime _currentTime;
    //private DateTime _nextTime;
    //private bool isTimeSet;
    public void SetSpanTime(DateTime currentT, DateTime nextT)
    {
        //_currentTime = currentT;
        //_nextTime = nextT;
        //isTimeSet = true;
    }

    protected PosInfo posInfo;

    protected TagPosInfo tagposInfo;//
    protected TagPosInfo pretagposInfo;//上一个点

    public void SetDestination(PosInfo posInfo, float rateT)
    {
        //Log.Info("SetDestination","posInfo:"+posInfo.TargetPos+",rateT:"+rateT);
        if (gameObject.activeInHierarchy == false)
        {
            //Log.Info("SetDestination", "gameObject.activeInHierarchy == false");
            return;
        }
        this.rate = rateT;
        this.posInfo = posInfo;

        //这里设置坐标信息，然后到Update->UpdatePosition 执行移动坐标
    }

    protected bool isDoRealTimePos;//是否开始了实时定位位置计算
    protected bool isCanSetRealTimePos;//是否能设置实时位置计算
    public void SetDestinationRealTime(TagPosInfo tagposInfoT, float rateT)
    {
        LocationManager.Instance.processCtestctime(tagposInfoT.TagPos.Tag, "SetDestinationRealTime01");
        //Log.Debug("NavAgentControllerBase000", string.Format("Start:{0},{1}", this.name, transform.position));
        if (gameObject.activeInHierarchy == false)
        {
            //Log.Info("SetDestination", "gameObject.activeInHierarchy == false");
            DoRealTimePosEndSet();
            return;
        }
        //if (tagposInfoT.TagPos.Tag == "6848")
        //{
        //    Debug.LogError("666");
        //}
        //Log.Debug("NavAgentControllerBase111", string.Format("Start:{0},{1}", this.name, transform.position));
        if (LocationManager.Instance.IsActiveDoWaitTags)
        {
            isDoRealTimePos = true;
            isCanSetRealTimePos = true;

            //if (tagposInfoT.TagPos.Tag == "6848")
            //{
            //    Debug.LogError("isCanSetRealTimePos:true");
            //}

            //Log.Debug("NavAgentControllerBase222", string.Format("Start:{0},{1}", this.name, transform.position));
            if (this is NavLocationRealtimePerson)
            {
                NavLocationRealtimePerson p = this as NavLocationRealtimePerson;
                if (p.name.Contains("55A4"))
                {
                    int I = 0;
                }
            }
        }
        this.rate = rateT;
        //if (posInfo != null)
        //{
        //    TagPosInfo t1 = posInfo as TagPosInfo;
        //    TagPosInfo t2 = tagposInfoT as TagPosInfo;
        //    if (t1.TagPos.DateTime != t2.TagPos.DateTime)
        //    {
        //        Debug.LogError("")
        //    }
        //}
        posInfo = tagposInfoT;
        if (tagposInfoT != tagposInfo)
        {
            if (tagposInfoT != null && tagposInfo != null)
            {
                if (tagposInfoT.TagPos.Time != tagposInfo.TagPos.Time)
                {

                    pretagposInfo = tagposInfo;
                    tagposInfo = tagposInfoT;
                }
                else
                {
                    //if (tagposInfoT.TagPos.Tag == "6848")
                    //{
                    //    Debug.LogError("tagposInfoT6848 111:" + tagposInfoT.TagPos.DateTime);
                    //}
                }
            }
            else
            {
                pretagposInfo = tagposInfo;
                tagposInfo = tagposInfoT;
            }
        }
        else
        {
            int i = 0;

        }
        //LocationManager.Instance.processCtestctime(tagposInfoT.TagPos.Tag, "SetDestinationRealTime");

    }

    public void SetDestination(PosInfo posInfo, float rateT, float velocityMagnitudeT)
    {
        SetDestination(posInfo, rateT);
        this.velocityMagnitude = velocityMagnitudeT;
        //this.timeInterval = timeIntervalT;
    }
    public void DoMovePerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoMove();
    }
    public void DoStopPerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoStop();
    }



    public float minDistanceToTarget = 0.2f;//经测试0.2f差不多

    public bool IsBusyUpdatePosition = false;

    //public bool UseShowPos = false;

    public void UpdatePosition(Action callback)
    {
        if (tagposInfo != null && tagposInfo.TagPos != null) LocationManager.Instance.processCtestctime(tagposInfo.TagPos.Tag, "NavUpdatePosition01");
        if (posInfo == null)
        {
            //if (callback != null)
            //{
            //    callback();
            //}
            DoRealTimePosEndSet();
            return;
        }
        DateTime start = DateTime.Now;

        var posNew = posInfo.TargetPos; //实际的点
        if (LocationManager.Instance.UseShowPos)//宝信用UseShowPos等于true效果好很多，不会偏到上一层，因为原来的代码已经对高度做了一下限制。但是其他场景下不一定适合。
        {
            posNew = posInfo.ShowPos; //原来的算法的人移动的点
        }

        //Log.Info("NavAgentControllerBase.UpdatePosition");
        if (this.targetPos == posNew)//用TargetPos，可以优化性能，用ShowPos，这里就没什么用了
        {
            //Log.Info("NavAgentControllerBase.UpdatePosition", "this.targetPos == posNew : " + targetPos);
            isPosInfoSet = true;
            //StopPerson();
            if (this is NavLocationHistoryPerson) StopPerson();
            DoRealTimePosEndSet();
            return;
        }
        else
        {
            //MovePerson();
            if (this is NavLocationHistoryPerson) MovePerson();
        }

        //NavMeshHelper.ClearTestPoints();
        NavMeshHelper.CreatePoint(posNew, "PinBotarget", 0.1f, Color.yellow, agent);//品铂传过来的真实数据点目标点
        //LocationManager.Instance.processCtestctime(tagposInfo.TagPos.Tag, "UpdatePosition02");
        //Log.Info("UpdatePosition", string.Format("{0},{1},{2},{3}", this.name, posInfo.TargetPos, posInfo.ShowPos, LocationManager.Instance.UseShowPos));

        //this.targetPos = posNew;
        float heightOffset = 0;
        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.ZiBo)
        {
            heightOffset = 1.5f;//针对淄博定位效果，调低高度1.5个单位
        }
        this.targetPos = new Vector3(posNew.x, posNew.y - heightOffset, posNew.z);
        isPosInfoSet = true;
        //1.实际的点，独立往目标点移动的效果。
        //  待机能看出来，人物会在原地不动。
        //  这个确定效果可以的话，原来的人就可以删掉了。
        //  目标点距离较远时移动速度会加快。
        //  结合后面的lastPos == pos会提高性能，不用一直计算。
        //  但是，点与点之间切换时能看出一点“切换”的迹象来。

        //this.targetPos = posInfo.ShowPos;
        //2.原来的算法的人移动的点,跟随原来的人物的效果。
        //  会被柜子挡住，NavMesh无法移动到柜子后面的目的地。

        //targetPos = new Vector3(targetPos.x, targetPos.y - 1.5f, targetPos.z);//人物高度1.7f
        targetPos = new Vector3(targetPos.x, targetPos.y - LocationManager.Instance.heightOffset, targetPos.z);//人物高度1.7f

        InitNavMeshAgent();
        //LocationManager.Instance.processCtestctime(tagposInfo.TagPos.Tag, "UpdatePosition03");
        SetDestination(targetPos, r =>
         {
             if (r)
             {
                 PrintTimeInfo();
             }


             if (callback != null)
             {
                 callback();
             }
         });
    }

    /// <summary>
    /// 实时位置设置结束时，相关操作
    /// </summary>
    protected void DoRealTimePosEndSet()
    {
        if (!LocationManager.Instance.IsActiveDoWaitTags) return;
        //if (isDoRealTimePos)
        //{
        if (this is NavLocationRealtimePerson)
        {
            NavLocationRealtimePerson tt = this as NavLocationRealtimePerson;
            if (tt != null && tt.locationObjectT.Tag != null)
            {
                LocationManager.Instance.RemoveSendedTagsById(tt.locationObjectT.Tag.Id);
                isDoRealTimePos = false;
                tt.locationObjectT.isDoPositon = false;
                //if (tt.locationObjectT.Tag.Code == "6848")
                //{
                //    Debug.LogError("!!!DoRealTimeloPosEndSet:6848");
                //}
            }
        }
        //}
    }

    private void PrintTimeInfo()
    {
        // HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
        // if (hisPosInfo != null)
        // {
        //     var current = hisPosInfo.CurrentPosInfo;
        //     var next = current.Next;

        //     if (next != null)
        //     {
        //         TimeSpan time = next.Time - current.Time; //下一个点的时间
        //         float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

        //         Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));

        //         MultHistoryTimeStamp timeStamp = LocationHistoryUITool.GetTimeStamp();
        //         double timesum = timeStamp.timeSum;
        //         DateTime showPointTime = timeStamp.showPointTime;
        //         float currentSpeedT = timeStamp.currentSpeed;

        //         Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("t1:{0},t2:{1},t3{2}", current.Time.ToString("HH:mm:ss.f"), next.Time.ToString("HH:mm:ss.f"), showPointTime.ToString("HH:mm:ss.f")));
        //     }

        // }
    }

    protected Vector3 lastPos = Vector3.zero;

    protected bool EnableUpdate = false;

    protected virtual void SetDestination(Vector3 pos, Action<bool> callback)
    {
        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
        //if (lastPos == pos)//坐标不变，不用计算处理。
        //{
        //    if (callback != null)
        //    {
        //        callback(false);
        //    }
        //    return;
        //}

        //lastPos = pos;

        ////Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
        //if (IsBusyUpdatePosition)
        //{
        //    //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
        //    if (callback != null)
        //    {
        //        callback(false);
        //    }
        //    return;
        //}
        //DateTime start = DateTime.Now;
        //IsBusyUpdatePosition = true;
        //NavMeshHelper.GetClosetPointAsync(pos,this.name,agent, (destination,p) => //多线程异步方式计算,避免影响性能
        //{
        //    try
        //    {
        //        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}=>{1},{2}", pos,destination, this));
        //        if (SystemSettingHelper.IsDebug())//调试模式下，查看接下来的移动方向
        //        {
        //            if (ArcManager.Instance != null)
        //            {
        //                if (p != null)
        //                    ArcManager.Instance.SetLine(gameObject.transform, p.transform);
        //            }
        //        }

        //        if (agent != null)
        //        {
        //            if (agent.gameObject.activeInHierarchy)
        //            {
        //                if (EnableUpdate == false)//代表第一次
        //                {
        //                    Log.Info("NavAgentControllerBase.SetDestination", string.Format("First Wrap!! pos:{0},obj:{1}", pos, this));
        //                    agent.Warp(destination);
        //                }
        //                else
        //                {
        //                    //yzl:20201012
        //                    //在上一个SetDestination未到达目标点时，又执行SetDestination到下个目标点，会有速度和方向缓慢调整的过程（会导致人员偏离轨迹）
        //                    //Vector3 dirTemp = (destination - agent.transform.position).normalized;//获取移动方向
        //                    //Vector3 velocityTemp = agent.velocity.magnitude * dirTemp;//设置移动方向
        //                    //float velocityMagnitudeT = velocityMagnitude * rate* speedSca;//速度大小
        //                    //velocityTemp = velocityMagnitudeT * velocityTemp.normalized;
        //                    //agent.speed = velocityMagnitudeT;//设置在路线上的最大移动速度
        //                    //agent.velocity = velocityTemp;//这里需要设置当前速度agent.velocity，如果只设置agent.speed会从当前速度慢慢加速到目标速度


        //                    bool r = agent.SetDestination(destination);//Agent被关闭或者被销毁，调用这个方法会报错
        //                    if (r == false)//人物物体不在NavMesh上，立刻跳到目标位置
        //                    {
        //                        Log.Info("NavAgentControllerBase.SetDestination", string.Format("Wrap pos:{0},obj:{1}", pos, this));
        //                        //this.transform.position = destination;
        //                        agent.Warp(destination);//要用这个,用上面那句话，"不在NavMesh上"的问题会一直出现，要用Warp才会重新计算
        //                    }
        //                    else
        //                    {
        //                        HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
        //                        if (hisPosInfo != null)
        //                        {
        //                            var current = hisPosInfo.CurrentPosInfo;
        //                            var next = current.Next;

        //                            if (next != null)
        //                            {
        //                                TimeSpan t = next.Time - current.Time; //下一个点的时间
        //                                                                       //float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

        //                                //Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));
        //                                this.timespan = t.TotalSeconds;//两点时间差，用于计算两点间移动速度
        //                            }

        //                        }
        //                    }
        //                }

        //                EnableUpdate = true;
        //            }
        //            else
        //            {
        //                Log.Error("NavAgentControllerBase.SetDestination", "agent.gameObject.activeInHierarchy==false:" + this);
        //            }
        //        }
        //        else
        //        {
        //            Log.Error("NavAgentControllerBase.SetDestination", "agent==null:" + this);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("NavAgentControllerBase.SetDestination", "Exception:" + ex);
        //    }
        //    finally
        //    {
        //        IsBusyUpdatePosition = false;
        //        TimeSpan time = DateTime.Now - start;

        //        if (callback != null)
        //        {
        //            callback(false);
        //        }

        //    }



        //    //Log.Info("NavAgentControllerBase.UpdatePosition", NavMeshHelper.Log);
        //    //Log.Info("NavAgentControllerBase.SetDestination", "UpdatePosition End time:" +time.TotalMilliseconds+"ms");
        //});

    }

    protected virtual bool GetMoveVelocity(Vector3 destination, ref float velocityMagnitudeTT)
    {

        navMeshPath.ClearCorners();
        NavMesh.CalculatePath(transform.position, destination, -1, navMeshPath);

        //在上一个SetDestination未到达目标点时，又执行SetDestination到下个目标点，会有速度和方向缓慢调整的过程（会导致人员偏离轨迹）
        if (navMeshPath.corners.Length >= 2)
        {
            float disTT = 0;
            for (int j = 0; j < navMeshPath.corners.Length; j++)
            {
                if (j >= 1)
                {
                    disTT += Vector3.Distance(navMeshPath.corners[j - 1], navMeshPath.corners[j]);
                    if (SystemSettingHelper.IsDebug())
                    {
                        Vector3 startpos = navMeshPath.corners[j - 1];
                        Vector3 endpos = navMeshPath.corners[j];
                        Debug.DrawLine(startpos, endpos, Color.red, 0.3f);
                    }
                }
            }

            if (this is NavLocationHistoryPerson)
            {
                // HisPosInfo hisPosInfoT = posInfo as HisPosInfo;
                // if (hisPosInfoT.CurrentPosInfo != null && hisPosInfoT.PrePosInfo != null)
                // {
                //     timeInterval = (float)(hisPosInfoT.CurrentPosInfo.Time - hisPosInfoT.PrePosInfo.Time).TotalSeconds;
                //     if (timeInterval > 0)
                //     {
                //         velocityMagnitudeTT = disTT / timeInterval;
                //     }
                //     else
                //     {
                //         velocityMagnitudeTT = PathFindingManager.Instance.HisMaxSpeed;
                //     }

                //     //velocityMagnitudeTT = velocityMagnitudeTT > PathFindingManager.Instance.HisMaxSpeed ? PathFindingManager.Instance.HisMaxSpeed : velocityMagnitudeTT;
                //     //velocityMagnitudeTT = velocityMagnitudeTT < PathFindingManager.Instance.HisMinSpeed ? PathFindingManager.Instance.HisMinSpeed : velocityMagnitudeTT;
                // }
            }
            else
            {
                if (tagposInfo != null && pretagposInfo != null)
                {
                    TagPosInfo posInfoT = tagposInfo;
                    TagPosInfo preposInfoT = pretagposInfo as TagPosInfo;
                    float D = disTT;
                    //disTT = Vector3.Distance(tagposInfo.TargetPos, pretagposInfo.TargetPos);
                    disTT = Vector3.Distance(destination, transform.position);

                    DateTime ti = LocationManager.GetTimestampToDateTime(posInfoT.TagPos.Time);
                    DateTime preti = LocationManager.GetTimestampToDateTime(preposInfoT.TagPos.Time);
                    timeInterval = (float)(ti - preti).TotalSeconds;
                    // if (timeInterval > 0)
                    // {
                    //     velocityMagnitudeTT = disTT / timeInterval;
                    // }
                    // else
                    // {
                    //     velocityMagnitudeTT = PathFindingManager.Instance.RealMaxSpeed;
                    // }

                    if (velocityMagnitudeTT > 5)
                    {
                        int I = 0;
                    }
                }
            }

        }
        else
        {
            return false;
        }

        return true;
    }

    protected void OnDestroy()
    {
        if (posBuffer == null) return;
        foreach (GameObject o in posBuffer.Values)
        {
            if (o == null) continue;
            GameObject.Destroy(o);
        }
        posBuffer.Clear();
        isDestroy = true;
    }



    public void MovePerson()
    {
        if(personAnimationController ==null) personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        //var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoMove();
    }

    public void StopPerson()
    {
        if (personAnimationController == null) personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        //var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoStop();
    }

}

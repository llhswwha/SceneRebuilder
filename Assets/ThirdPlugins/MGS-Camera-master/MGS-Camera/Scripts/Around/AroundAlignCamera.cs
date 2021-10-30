/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundAlignCamera.cs
 *  Description  :  Camera rotate around and align to target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/9/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using UnityEngine;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Camera rotate around and align to target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/AroundAlignCamera")]
    public class AroundAlignCamera : AroundCamera
    {
        public static AroundAlignCamera Instance;

        void Awake()
        {
            Instance = this;
        }

        #region Field and Property
        /// <summary>
        /// Damper for align.
        /// </summary>
        [Range(0, 10)]
        public float alignDamper = 2;
     
        /// <summary>
        /// Threshold of linear adsorbent.
        /// </summary>
        [Range(0, 1)]
        public float threshold = 0.1f;

        private bool isAliging;

        /// <summary>
        /// Camera is auto aligning.
        /// </summary>
        public bool IsAligning {
            set
            {
                isAliging = value;

#if UNITY_ANDROID || UNITY_IPHONE
                if (orbit)
                {
                    if (isAliging)
                    {
                        orbit.enabled = false;
                    }
                    else
                    {
                        UpdateOrbit();
                        orbit.enabled = true;
                    }
                }
#endif
            }
            get
            {
                return isAliging;
            }
        }

        /// <summary>
        /// Start align event.
        /// </summary>
        public event Action OnAlignStart;

        /// <summary>
        /// End align event.
        /// </summary>
        public event Action OnAlignEnd;

        public Vector2 lastAngles;
        public Vector3 currentDirection, targetDirection, lastDirection;
        public float lastDistance;
        public float anglesSpeed, directionSpeed, distanceSpeed;
        public float anglesOffset, directionOffset, distanceOffset;
        public bool linearAdsorbent;

        public DateTime alignStartTime;
        public bool isCheckAlignTime=true;//是否开启时间检测
        public bool isCheckTimeSet;

        public float AlignUsedTime = 0;//AlignEnd超时时间
        private float AlignTimeout = 2f;//AlignEnd超时时间

        #endregion

        protected override void Start()
        {
            base.Start();
#if UNITY_ANDROID || UNITY_IPHONE
            if (orbit == null)
            {
                orbit = gameObject.GetComponent<TBOrbit>();
            }
#endif
            }

#if UNITY_ANDROID || UNITY_IPHONE

        public bool EnableSelf = true;

        public TBOrbit orbit;

        [ContextMenu("SwithOnTBOrbit")]
        public void SwithOnTBOrbit()
        {
            if(orbit)
            {
                //this.enabled = false;
                orbit.enabled = true;
            }
            else
            {
                Debug.LogError("orbit ==null");
            }
        }

        [ContextMenu("SwithOffTBOrbit")]
        public void SwithOffTBOrbit()
        {
            if (orbit)
            {
                //this.enabled = false;
                orbit.enabled = false;
            }
            else
            {
                Debug.LogError("orbit ==null");
            }
        }

        private void UpdateOrbit()
        {
            orbit.target = this.targetPos.transform;
            orbit.distance = this.targetDistance;
            orbit.idealDistance = this.targetDistance;
            orbit.initialDistance = this.targetDistance;
            orbit.pitch = this.targetAngles.x;
            orbit.idealPitch = this.targetAngles.x;
            orbit.yaw = this.targetAngles.y;
            orbit.idealYaw = this.targetAngles.y;
        }
#endif

        public bool IsTransparentStructureModel;//历史轨迹时透明摄像头和人员之间的墙壁、地板等建筑结构

        #region Protected Method
        protected override void Update()
        {
            if(IsTransparentStructureModel){
                var pos1=GetTargetPosition();
                
            }
            //if (CameraSceneManager.Instance.theThirdPersonCamera.enabled == true) return;
            if (IsAligning)
            {                
                AutoAlignView();
            }
            else
            {
#if UNITY_ANDROID || UNITY_IPHONE
                if (EnableSelf == false) return;

                if (orbit == null)
                {
                    orbit = TBOrbit.Instance;
                }
                if (orbit)
                {
                    if (orbit.enabled == false)
                    {
                        UpdateOrbit();//将数值同步到Orbit

                        AroundByMouseInput();
                    }
                    else
                    {
                        this.targetDistance = orbit.idealDistance;
                        this.targetAngles.x = orbit.pitch;
                        this.targetAngles.y = orbit.yaw;
                    }
                }
#else
                AroundByMouseInput();
#endif
                //#if UNITY_ANDROID || UNITY_IPHONE
                //                AroundByMobileInput();
                //#else
                //                AroundByMouseInput();
                //#endif
            }

        }

        /// <summary>
        /// Check Offset Value
        /// </summary>
        /// <param name="anglesOffset"></param>
        /// <param name="directionOffset"></param>
        /// <param name="distanceOffset"></param>
        private void CheckValue(ref float anglesOffset,ref float directionOffset,ref float distanceOffset)
        {
            anglesOffset = Math.Round(anglesOffset, 4) == 0 ? 0 : anglesOffset;
            directionOffset = Math.Round(directionOffset, 4) == 0 ? 0 : directionOffset;
            distanceOffset = Math.Round(distanceOffset, 4) == 0 ? 0 : distanceOffset;
        }
        public int cameraMoveSpeed = 1;
        /// <summary>
        /// Auto align camera veiw to target.
        /// </summary>
        protected void AutoAlignView()
        {
            //Calculate current offset.
            var currentAnglesOffset = (targetAngles - CurrentAngles).magnitude;
            var currentDirectionOffset = (targetDirection - currentDirection).magnitude;
            var currentDistanceOffset = Mathf.Abs(targetDistance - CurrentDistance);
            CheckValue(ref currentAnglesOffset,ref currentDirectionOffset,ref currentDistanceOffset);
            if (AlignEndTimeOut()) 
            {
                Debug.LogError("移动卡住超时了,自动触发结束回调并退出!");
                return;//如果移动卡住超时了,自动触发结束回调并退出
            }
            //Check align finish.
            if (currentAnglesOffset < Vector3.kEpsilon && currentDirectionOffset < Vector3.kEpsilon &&
                currentDistanceOffset < Vector3.kEpsilon*100)//修改过YZL:有时候始终无法currentDistanceOffset< Vector3.kEpsilon；
            {
                IsAligning = false;
                isCheckTimeSet = false;//正常结束，把处理超时的标志位设为false
                if (OnAlignEnd != null)
                    OnAlignEnd.Invoke();
            }
            else
            {
                if (linearAdsorbent)
                {
                    //MoveTowards to linear adsorbent align.
                    CurrentAngles = Vector2.MoveTowards(CurrentAngles, targetAngles, anglesSpeed * Time.deltaTime* cameraMoveSpeed);
                    currentDirection = Vector3.MoveTowards(currentDirection, targetDirection, directionSpeed * Time.deltaTime* cameraMoveSpeed);
                    CurrentDistance = Mathf.MoveTowards(CurrentDistance, targetDistance, distanceSpeed * Time.deltaTime* cameraMoveSpeed);
                }
                else
                {
                    //Record last.
                    lastAngles = CurrentAngles;
                    lastDirection = currentDirection;
                    lastDistance = CurrentDistance;

                    //Lerp to align.
                    CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, alignDamper * Time.deltaTime * cameraMoveSpeed);
                    currentDirection = Vector3.Lerp(currentDirection, targetDirection, alignDamper * Time.deltaTime * cameraMoveSpeed);
                    CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, alignDamper * Time.deltaTime * cameraMoveSpeed);

                    //Check into linear adsorbent.
                    if (currentAnglesOffset / anglesOffset < threshold && currentDirectionOffset / directionOffset < threshold &&
                        currentDistanceOffset / distanceOffset < threshold)
                    {
                        anglesSpeed = (CurrentAngles - lastAngles).magnitude / Time.deltaTime;
                        directionSpeed = (currentDirection - lastDirection).magnitude / Time.deltaTime;
                        distanceSpeed = Mathf.Abs(CurrentDistance - lastDistance) / Time.deltaTime;
                        linearAdsorbent = true;
                    }
                }

                //Update position and rotation.
                transform.position = GetTargetPosition() + currentDirection.normalized * CurrentDistance;
                transform.rotation = Quaternion.Euler(CurrentAngles);
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// SetCurrentAngle
        /// </summary>
        /// <param name="angle"></param>
        public void SetCurrentAngle(Vector2 angle)
        {
            CurrentAngles = angle;
        }

        /// <summary>
        /// Align camera veiw to target.
        /// </summary>
        /// <param name="center">Around center.</param>
        /// <param name="angles">Rotate angles.</param>
        /// <param name="distance">Distance from camera to target.</param>
        public void AlignVeiwToTarget(Transform center, Vector2 angles, float distance)
        {
            //target = center;
            targetPos = new TransformPos(center);
            targetAngles = angles;
            targetDistance = distance;

            AlignVeiwToTarget();
            //cww:动画有问题

            //SetTargetEx(center.position,angles,distance);
        }

        public void AlignVeiwToTarget(Transform center,  float distance)
        {
            //target = center;
            targetPos = new TransformPos(center);
            targetDistance = distance;
            AlignVeiwToTarget();
            //cww:动画有问题

            //SetTargetEx(center.position,angles,distance);
        }

        public void SetTargetEx(Vector3 centerPos, Vector2 angles, float distance)
        {
            //target = center;
            targetPos.SetPosition(centerPos);
            targetAngles = angles;
            targetDistance = distance;

            CurrentAngles=targetAngles;
            CurrentDistance=targetDistance;

            //AlignVeiwToTarget();
        }

        /// <summary>
        /// YZL20210426:直接移动到对应角度和位置，原始目标物体不变
        /// </summary>
        /// <param name="angles"></param>
        /// <param name="distance"></param>
        public void SetTargetEx(Vector3 targetCameraPos, Vector2 angles)
        {
            float distance = Vector3.Distance(targetCameraPos, target.position);
            targetAngles = angles;
            targetDistance = distance;

            CurrentAngles = targetAngles;
            CurrentDistance = targetDistance;
            transform.position = targetCameraPos;
            //AlignVeiwToTarget();
        }

        public void AlignVeiwToTarget(Transform center)
        {
            targetPos = new TransformPos(center);
            AlignVeiwToTarget();
        }

        private void AlignVeiwToTarget()
        {
            //Optimal angles.
            while (targetAngles.y - CurrentAngles.y > 180)
                targetAngles.y -= 360;
            while (targetAngles.y - CurrentAngles.y < -180)
                targetAngles.y += 360;

            //Calculate lerp parameter.
            currentDirection = (transform.position - GetTargetPosition()).normalized;
            targetDirection = (Quaternion.Euler(targetAngles) * Vector3.back).normalized;
            CurrentDistance = Vector3.Distance(transform.position, GetTargetPosition());

            //Calculate offset.
            anglesOffset = Mathf.Max((targetAngles - CurrentAngles).magnitude, Vector3.kEpsilon);
            directionOffset = Mathf.Max((targetDirection - currentDirection).magnitude, Vector3.kEpsilon);
            distanceOffset = Mathf.Max(Mathf.Abs(targetDistance - CurrentDistance), Vector3.kEpsilon);

            //Start align.
            linearAdsorbent = false;
            IsAligning = true;
            SetAlignStartTime();
            if (OnAlignStart != null)
                OnAlignStart.Invoke();
        }
        /// <summary>
        /// 记录视角移动开始时间
        /// </summary>
        private void SetAlignStartTime()
        {
            if(isCheckAlignTime)
            {
                isCheckTimeSet = true;
                alignStartTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 处理移动超时情况
        /// </summary>
        private bool AlignEndTimeOut()
        {
            if(isCheckAlignTime&&isCheckTimeSet)
            {
                float maxTimeout = alignDamper + AlignTimeout;
                AlignUsedTime=(float)((DateTime.Now-alignStartTime).TotalSeconds);
                if (AlignUsedTime > maxTimeout)
                {
                    Debug.LogFormat("AroundAlignCamera.AlignEndTimeOut->targetName:{0}",target.name);
                    //移动意外卡住，超时自动结束
                    IsAligning = false;
                    isCheckTimeSet = false;//执行完之后，等待下次的记录时间
                    if (OnAlignEnd != null)
                        OnAlignEnd.Invoke();
                    return true; 
                }
            }
            return false;
        }

        public AlignTarget lastAlignTarget;

        public bool IsFirst = true;

        /// <summary>
        /// Align camera veiw to target.
        /// </summary>
        /// <param name="alignTarget">Target of camera align.</param>
        public void AlignVeiwToTarget(AlignTarget alignTarget,int speed=1)
        {
            //if (alignTarget.enableAngles==false && IsFirst==false)
            //{
            //    alignTarget.angles = lastAlignTarget.angles;
            //}

            //if (alignTarget.enableAngles == false && IsFirst == false)
            //{
            //    alignTarget.angles = lastAlignTarget.angles;
            //}

            if (IsFirst == false)
            {
                if (lastAlignTarget.center == alignTarget.center)
                {
                    Debug.LogError(string.Format("AlignVeiwToTarget [lastAlignTarget.center == alignTarget.center] center:{0},angles:{1},distance:{2} go:{3}", alignTarget.center.position, alignTarget.angles, alignTarget.distance, alignTarget.center));
                    return;
                }
            }

            IsFirst = false;

            lastAlignTarget = alignTarget;
            cameraMoveSpeed = speed;
            Debug.LogError(string.Format("AlignVeiwToTarget center:{0},angles:{1},distance:{2} go:{3}",alignTarget.center.position,alignTarget.angles,alignTarget.distance, alignTarget.center));
            AlignVeiwToTarget(alignTarget.center, alignTarget.angles, alignTarget.distance);
            
            //Override range.
            angleRange = alignTarget.angleRange;
            distanceRange = alignTarget.distanceRange;           
        }

        #endregion
    }
}
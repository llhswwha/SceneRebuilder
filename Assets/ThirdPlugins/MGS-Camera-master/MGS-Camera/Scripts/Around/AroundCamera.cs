/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundCamera.cs
 *  Description  :  Camera rotate around target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Camera rotate around target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/AroundCamera")]
    [RequireComponent(typeof(Camera))]
    public class AroundCamera : MonoBehaviour
    {
        #region Field and Property
        ///// <summary>
        ///// Around center.
        ///// </summary>
        public Transform target
        {
            get
            {
                return GetTarget();
            }
        }

        /// <summary>
        /// Around center.
        /// </summary>
        public TransformPos targetPos;

        /// <summary>
        /// Settings of mouse button, pointer and scrollwheel.
        /// </summary>
        public MouseSettings mouseSettings = new MouseSettings(1, 10, 10);

        /// <summary>
        /// Range limit of angle.
        /// </summary>
        public Range angleRange = new Range(-90, 90);

        /// <summary>
        /// Range limit of distance.
        /// </summary>
        public Range distanceRange = new Range(1, 10);

        //正交摄像机范围
        public Range OrthographicRange = new Range(10, 155);
        /// <summary>
        /// Damper for move and rotate.
        /// </summary>
        [Range(0, 10)]
        public float damper = 5;

        /// <summary>
        /// Camera current angls.
        /// </summary>
        public Vector2 CurrentAngles { protected set; get; }

        /// <summary>
        /// Current distance from camera to target.
        /// </summary>
        public float CurrentDistance { protected set; get; }

        /// <summary>
        /// Camera target angls.
        /// </summary>
        public Vector2 targetAngles;

        /// <summary>
        /// Target distance from camera to target.
        /// </summary>
        public float targetDistance;

        /// <summary>
        /// 是否关闭鼠标旋转
        /// </summary>
        private bool IsDisableMouseInput;

        /// <summary>
        /// 鼠标原始位置（Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动）
        /// </summary>
        private Vector3 mousePositionOri;

        private Camera mainCamera;


        #endregion

        #region Protected Method
        protected virtual void Start()
        {
            CurrentAngles = targetAngles = transform.eulerAngles;
            CurrentDistance = targetDistance = Vector3.Distance(transform.position, GetTargetPosition());
            mainCamera = transform.GetComponent<Camera>();
        }

        protected virtual void Update()
        {
            AroundByMouseInput();
        }
        /// <summary>
        /// 打开/关闭 鼠标旋转功能
        /// </summary>
        /// <param name="isEnableMouse">是否启用鼠标旋转</param>
        public virtual void SetMouseInputState(bool isEnableMouse)
        {
            IsDisableMouseInput = !isEnableMouse;
        }
        //记录上一次手机触摸位置判断用户是在左放大还是缩小手势  
        private Vector2 oldPosition1;
        private Vector2 oldPosition2;

        private bool m_IsSingleFinger;
        public bool canRotation_X = true;
        public bool canRotation_Y = true;
        public bool canScale = true;

        public float GetWheelSensitivity()
        {
            float ws = mouseSettings.wheelSensitivity;
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ws *= 0.1f;
            }
            return ws;
        }

        protected void AroundByMobileInput()
        {
            if (!IsEnableAround()) return;           
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == UnityEngine.TouchPhase.Moved)
                {
                    targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                    targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                    //Range.
                    targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
                }
                //Mouse pointer.
                m_IsSingleFinger = true;
            }
            //Mouse scrollwheel.
            if (canScale)
            {
                if (Input.touchCount > 1)
                {
                    //计算出当前两点触摸点的位置  
                    if (m_IsSingleFinger)
                    {
                        oldPosition1 = Input.GetTouch(0).position;
                        oldPosition2 = Input.GetTouch(1).position;
                    }
                    if (Input.touches[0].phase == UnityEngine.TouchPhase.Moved && Input.touches[1].phase == UnityEngine.TouchPhase.Moved)
                    {
                        var tempPosition1 = Input.GetTouch(0).position;
                        var tempPosition2 = Input.GetTouch(1).position;

                        float currentTouchDistance = Vector3.Distance(tempPosition1, tempPosition2);
                        float lastTouchDistance = Vector3.Distance(oldPosition1, oldPosition2);

                        //计算上次和这次双指触摸之间的距离差距  
                        //然后去更改摄像机的距离  
                        targetDistance -= (currentTouchDistance - lastTouchDistance) * Time.deltaTime * GetWheelSensitivity();
                        //  m_debugTip.text = ( currentTouchDistance - lastTouchDistance ).ToString() + " + " + targetDistance.ToString();

                        //把距离限制住在min和max之间  

                        //备份上一次触摸点的位置，用于对比  
                        oldPosition1 = tempPosition1;
                        oldPosition2 = tempPosition2;
                        m_IsSingleFinger = false;
                    }
                }
            }
            targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

            //Lerp.
            CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

            if (!canRotation_X) targetAngles.y = 0;
            if (!canRotation_Y) targetAngles.x = 0;

            //Update transform position and rotation.
            transform.rotation = Quaternion.Euler(CurrentAngles);
            transform.position = target.position - transform.forward * CurrentDistance;
            // transform.position = target.position - Vector3.forward * CurrentDistance;
        }

        

        /// <summary>
        /// Camera around target by mouse input.
        /// </summary>
        protected void AroundByMouseInput()
        {
            //if (RoomFactory.Instance && RoomFactory.Instance.RemoteMode == RemoteMode.RenderStreaming)
            //{
            //    AroundByMouseInputSystem();
            //}
            //else
            {
                if (!IsEnableAround()) return;
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    mousePositionOri = Input.mousePosition;
                }
                if (Input.GetMouseButton(mouseSettings.mouseButtonID))
                {
                    Vector2 mouseOffset = GetMouseOffset();
                    targetAngles.y += mouseOffset.x * mouseSettings.pointerSensitivity;
                    targetAngles.x -= mouseOffset.y * mouseSettings.pointerSensitivity;

                    //Mouse pointer.
                    //targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                    //targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                    //Range.
                    targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
                }

                //Mouse scrollwheel.
                if (mainCamera == null) mainCamera = transform.GetComponent<Camera>();
                if (mainCamera.orthographic)
                {
                    if (!Input.GetMouseButton(2))
                    {
                        mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * GetWheelSensitivity();
                        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, OrthographicRange.min, OrthographicRange.max);
                    }
                }
                if (!Input.GetMouseButton(2))
                {
                    targetDistance -= Input.GetAxis("Mouse ScrollWheel") * GetWheelSensitivity();
                }
                targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

                //Lerp.
                CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
                CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

                //Update transform position and rotation.
                transform.rotation = Quaternion.Euler(CurrentAngles);
                //transform.position = GetTargetPosition()- transform.forward * CurrentDistance;

                if (isCameraCollider)
                {
                    Vector3 currentpos = GetTargetPosition() - transform.forward * CurrentDistance;
                    SetPosition(currentpos);

                }
                else
                {
                    transform.position = GetTargetPosition() - transform.forward * CurrentDistance;
                }
            }
        }

        /// <summary>
        /// 通过新输入系统控制
        /// </summary>
        private void AroundByMouseInputSystem()
        {
            if (!IsEnableAround()) return;
            //if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            //{
            //    //mousePositionOri = Input.mousePosition;
            //    //Debug.LogError("Mouse.current.leftButton.wasPressedThisFrame:"+ Mouse.current.position.ReadValue());
            //    mousePositionOri = Mouse.current.position.ReadValue();
            //}
            if (GetMouseButton(mouseSettings.mouseButtonID))
            {
                //Debug.LogError("Delta:"+ Mouse.current.delta.ReadValue());
                Vector2 mouseOffset = Mouse.current.delta.ReadValue();// MouseOffsetByInputSystem();
                targetAngles.y += mouseOffset.x * mouseSettings.pointerSensitivity;
                targetAngles.x -= mouseOffset.y * mouseSettings.pointerSensitivity;

                //Mouse pointer.
                //targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                //targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //Range.
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }

            //Mouse scrollwheel.
            if (mainCamera == null) mainCamera = transform.GetComponent<Camera>();
            if (mainCamera.orthographic)
            {
                if (!Mouse.current.middleButton.isPressed)//(!Input.GetMouseButton(2))
                {
                    mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * GetWheelSensitivity();
                    mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, OrthographicRange.min, OrthographicRange.max);
                    //mainCamera.orthographicSize -= Mouse.current.scroll.ReadValue() * GetWheelSensitivity();
                    //mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, OrthographicRange.min, OrthographicRange.max);
                }
            }
            if (!Mouse.current.middleButton.isPressed)//(!Input.GetMouseButton(2))
            {
                //targetDistance -= Input.GetAxis("Mouse ScrollWheel") * GetWheelSensitivity();                                
                //if (Mouse.current.scroll.ReadValue().y!=0) Debug.LogError("MouseMoveY:"+Mouse.current.scroll.ReadValue().y  +" default:"+ Mouse.current.scroll.ReadDefaultValue().y);
                float y = Mouse.current.scroll.ReadValue().y/300f;
                targetDistance -= y * GetWheelSensitivity();
            }
            targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

            //Lerp.
            CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

            //Update transform position and rotation.
            transform.rotation = Quaternion.Euler(CurrentAngles);
            //transform.position = GetTargetPosition()- transform.forward * CurrentDistance;

            if (isCameraCollider)
            {
                Vector3 currentpos = GetTargetPosition() - transform.forward * CurrentDistance;
                SetPosition(currentpos);

            }
            else
            {
                transform.position = GetTargetPosition() - transform.forward * CurrentDistance;
            }
        }

        private bool GetMouseButton(int mouseId)
        {
            if(mouseId==0&&Mouse.current.leftButton.isPressed)
            {
                return true;
            }
            else if (mouseId == 1 && Mouse.current.rightButton.isPressed)
            {
                return true;
            }else if (mouseId == 2 && Mouse.current.middleButton.isPressed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否能够旋转
        /// </summary>
        /// <returns></returns>
        private bool IsEnableAround()
        {
            if (IsDisableMouseInput) return false;
            //if (IsClickUGUIorNGUI.Instance)
            //{
            //    if (IsClickUGUIorNGUI.Instance.isClickedUI) return false;
            //}
            //if (BigScreenManager.IsFullView) return false;//大屏不能旋转建筑
            //if (FPSMode.Instance != null && FPSMode.Instance.FPSController != null)
            //{
            //    if (FPSMode.Instance.FPSController.IsLoadBuildAndDev) return false;
            //}
            return true;
        }

        #region 摄像机遮挡相关功能

        public bool isCanCameraCollider;
        /// <summary>
        /// 是否开启通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
        /// </summary>
        [HideInInspector]
        public bool isCameraCollider;
        public MCinemachineCollider mcinemachineCollider;
        Camera cam;

        public Transform GetTarget()
        {
            if (targetPos != null)
            {
                return targetPos.transform;
            }
            return null;
            //return target.transform;
        }

        public void SetTarget(Transform t)
        {
            targetPos = new TransformPos(t);
        }

        public TransformPos GetTargetPos()
        {
            return targetPos;
        }

        public Vector3 GetTargetPosition()
        {
            if (targetPos != null)
            {
                return targetPos.getPositon();
            }
            return Vector3.zero;
            //return target.position;
        }

        /// <summary>
        /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
        /// </summary>
        public void SetPosition(Vector3 currentpos)
        {
            if (mcinemachineCollider != null)
            {
                Vector3 adjustPos;
                bool isbool=  mcinemachineCollider.GetPos(currentpos, GetTargetPosition(), out adjustPos);

                //targetDistance = Vector3.Distance(adjustPos, target.transform.position);
                //CurrentDistance = targetDistance;
                if (isbool)
                {
                    Vector3 targetToCamdir = currentpos - GetTargetPosition();
                    float offset = targetToCamdir.magnitude - adjustPos.magnitude;

                    Vector3 tpos = GetTargetPosition();
                    //if (offset > distanceRange.min)
                    //{
                        tpos = targetToCamdir.normalized * offset + GetTargetPosition();
                    //}
                    //else
                    //{
                    //    tpos = targetToCamdir.normalized * distanceRange.min + GetTargetPosition();
                    //}

                    //Debug.LogError("currentpos:" + currentpos + "||currentpos:" + adjustPos);
                    //Vector3 p = Vector3.Lerp(transform.position, tpos, damper * Time.deltaTime);
                    transform.position = tpos;
                    //Debug.LogErrorFormat("offset:{0}", offset);
                }
                else
                {
                    transform.position = GetTargetPosition() - transform.forward * CurrentDistance;
                }
            }
        }

        /// <summary>
        /// 是否开启通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
        /// </summary>
        public void SetisCameraCollider(bool isbool)
        {
            if (!isCanCameraCollider) return;
            isCameraCollider = isbool;
        }

        [ContextMenu("SetTest")]
        public void SetTest()
        {
            targetDistance = Vector3.Distance(transform.position + transform.forward, GetTargetPosition());
            CurrentDistance = targetDistance;
            transform.position = GetTargetPosition() - transform.forward * CurrentDistance;
        }

        #endregion
        /// <summary>
        /// 获取鼠标移动的距离，Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动
        /// </summary>
        /// <returns></returns>
        private Vector2 GetMouseOffset()
        {
            //float x = Input.GetAxis("Mouse X");
            //float y = Input.GetAxis("Mouse Y");
            //Vector2 lastPos = new Vector2(x,y);
            Vector3 mouseOffset = Input.mousePosition - mousePositionOri;
            //Debug.Log("MouseOffset:(" + mouseOffset.x + "," + mouseOffset.y + ")");
            mousePositionOri = Input.mousePosition;
            float x = mouseOffset.x / 30f;
            float y = mouseOffset.y / 30f;   //12
            //if (x != 0 && y != 0)
            //{
            //    Debug.Log(string.Format("Around GetAxis,system:({0},{1}) caculate:({2},{3})", lastPos.x, lastPos.y, x, y));
            //}
            return new Vector2(x, y);
        }
        private Vector2 MouseOffsetByInputSystem()
        {
            Debug.LogError("MouseOffsetByInputSystem:" + Mouse.current.position.ReadValue()+" Ori:"+mousePositionOri);
            Vector2 mouseOffset = Mouse.current.position.ReadValue() - new Vector2(mousePositionOri.x,mousePositionOri.y);
            //Debug.Log("MouseOffset:(" + mouseOffset.x + "," + mouseOffset.y + ")");
            mousePositionOri = Mouse.current.position.ReadValue();
            float x = mouseOffset.x / 30f;
            float y = mouseOffset.y / 30f;   //12
            //if (x != 0 && y != 0)
            //{
            //    Debug.Log(string.Format("Around GetAxis,system:({0},{1}) caculate:({2},{3})", lastPos.x, lastPos.y, x, y));
            //}
            return new Vector2(x, y);
        }
        #endregion
    }
}
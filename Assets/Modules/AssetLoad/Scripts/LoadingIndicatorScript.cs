using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Jacovone.AssetBundleMagic
{
    public class LoadingIndicatorScript : MonoBehaviour
    {
        public static LoadingIndicatorScript Instance;
        public Image RotatingLoading;
        public Image ColoredCircle;
        public GameObject Bg;
        private ChunkManager _chunkManager;
        public FirstPersonController FirstPerson;
        public Image MaskBg;//摄像机自动旋转时，遮罩
        Vector2 mousePos = new Vector2(960f, 560f);
        void Start()
        {
            GameObject go = GameObject.Find("ChunkManager");
            if (go != null)
            {
                _chunkManager = go.GetComponent<ChunkManager>();
            }
        }
        /// <summary>
        /// 判断是否在加载设备或者建筑，在漫游情况下
        /// </summary>
      public void IsBuildingAndIsDev()
        {
            if (IsBuilding || SetByCode)
                return;
        }
        void Awake()
        {
            Instance = this;
        }
        public bool IsBuilding;

        public float waitTime = 0.5f;

        /// <summary>
        /// 是否等待时间
        /// </summary>
        public bool IsWait = true;

        /// <summary>
        /// 是否有遮罩
        /// </summary>
        public bool HaveShade = true;

        /// <summary>
        /// 是否显示进度条
        /// </summary>
        public bool Enabled = true;

        // Update is called once per frame
        void Update()
        {
            if (Enabled == false) return;
            if (SetByCode) return;//代码设置
            if (IsInvoking("ShowBuildingLoadingProcess")) return;
            if (_chunkManager != null)
            {
                //if (_chunkManager.currentProgress != null) {
                if (_chunkManager.isBusy)
                {
                    IsBuilding = true;
                   Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    if(HaveShade)
                        Bg.gameObject.SetActive(true);
                    RotatingLoading.enabled = true;
                    ColoredCircle.enabled = true;
                    var progress = _chunkManager.GetProgress();
                    //Debug.Log("progress:"+progress);
                    ColoredCircle.fillAmount = progress;
                    
                    FirstPerson.IsLoadBuildAndDev = true;


                }
                else
                {
                    if (!RotatingLoading.enabled) return;
                    ShowBuildingLoadingProcessEx();
                }
            }
            else
            {
                if (!RotatingLoading.enabled) return;
                ShowBuildingLoadingProcessEx();
            }
        }
        /// <summary>
        /// 最大遮罩时间
        /// </summary>
        private float maxMaskTime = 5f;
        /// <summary>
        /// 设置遮罩
        /// </summary>
        /// <param name="isShow"></param>
        public void SetMaskImangeState(bool isShow)
        {
            if (IsInvoking("HideMaskAuto")) CancelInvoke("HideMaskAuto");
            if(isShow)
            {
                if (!IsInvoking("HideMaskAuto")) Invoke("HideMaskAuto",maxMaskTime);
            }
            if (MaskBg != null)
            {
                MaskBg.enabled = isShow;
            }
        }
        /// <summary>
        /// 自动关闭遮罩
        /// </summary>
        private void HideMaskAuto()
        {
            if (MaskBg != null) MaskBg.enabled = false;
        }
        public void ShowBuildingLoadingProcessEx()
        {
            if (waitTime > 0 && IsWait)
            {
                Invoke("ShowBuildingLoadingProcess", waitTime);
            }
            else
            {
                ShowBuildingLoadingProcess();
            }
        }

            public void ShowBuildingLoadingProcess()
        {
            RotatingLoading.enabled = false;
            ColoredCircle.enabled = false;
            Bg.gameObject.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            IsBuilding = false;
            FirstPerson.IsLoadBuildAndDev = false;
        }
        public bool SetByCode = false;
        public void SetDevLoadingProcess()
        {

            RotatingLoading.enabled = false;
            ColoredCircle.enabled = false;
            Bg.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
         
            SetByCode = false;
            FirstPerson.IsLoadBuildAndDev = false;

        }
        public void SetProgress(float progress)
        {
            if (IsInvoking("SetDevLoadingProcess")) return;
            if (progress > 0 && progress < 1)
            {
                SetByCode = true;
                //Debug.Log("SetProgress:"+ progress);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                if(HaveShade)
                    Bg.gameObject.SetActive(true);
                RotatingLoading.enabled = true;
                ColoredCircle.enabled = true;             
                ColoredCircle.fillAmount = progress;
                FirstPerson.IsLoadBuildAndDev = true;
            }
            else
            {
                if (!RotatingLoading.enabled) return;
                Invoke("SetDevLoadingProcess", waitTime);
                //Debug.Log("HideProgress:" + progress);
                //RotatingLoading.enabled = false;
                //ColoredCircle.enabled = false;
                //SetByCode = false;
            }
        }
    }

}

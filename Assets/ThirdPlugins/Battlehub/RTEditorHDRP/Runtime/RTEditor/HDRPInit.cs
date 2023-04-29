using Battlehub.RTCommon;
using Battlehub.RTHandles.HDRP;
using Battlehub.UIControls;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTEditor.HDRP
{
    public class HDRPInit : EditorExtension
    {
        private GameObject m_foregroundOutput;
        private IRenderPipelineCameraUtility m_cameraUtility;
        private OutlineManagerHDRP m_outlineManager;

        [SerializeField]
        private Material m_selectionMaterial = null;

        //private void Awake(){
        //     base.Awake();
        //     if(m_outlineManager==null){
        //         m_outlineManager= gameObject.AddComponent<OutlineManagerHDRP>();
        //     }
        // }

        // private void Start(){
        //     //base.Start();
        //     if(m_outlineManager==null){
        //         m_outlineManager= gameObject.AddComponent<OutlineManagerHDRP>();
        //     }
        // }

        // protected override void Awake()
        // {
        //     Debug.LogError("HDRPInit.Awake:"+this);
        //     m_rteState = IOC.Resolve<IRTEState>();
        //     Debug.Log("m_rteState:"+m_rteState);
        //     if (m_rteState != null)
        //     {
        //         Debug.Log("IsCreated:"+m_rteState.IsCreated);
        //         if (m_rteState.IsCreated)
        //         {
        //             Debug.Log("OnEditorExist()");
        //             OnEditorExist();
        //         }
        //         else
        //         {
        //              Debug.Log("m_rteState.Created += OnEditorCreated");
                    
        //             m_rteState.Created += OnEditorCreated;

        //              OnEditorExist();
        //         }
        //     }
        //     else
        //     {
        //          Debug.Log("OnEditorExist()2");
        //         OnEditorExist();
        //     }

            
        // }

        public void FireEditExist(){
            OnEditorExist();
        }

        public void EnableOutline(){
            OnEditorExist();
        }

        protected override void OnEditorExist()
        {
            base.OnEditorExist();
            if (RenderPipelineInfo.Type != RPType.HDRP || RenderPipelineInfo.ForceUseRenderTextures)
            {
                return;
            }

            m_cameraUtility = GetComponent<IRenderPipelineCameraUtility>();
            m_outlineManager = gameObject.AddComponent<OutlineManagerHDRP>();
            m_outlineManager.SelectionMaterial = m_selectionMaterial;
            
            IRTE rte = IOC.Resolve<IRTE>();
            IRTEAppearance appearance = IOC.Resolve<IRTEAppearance>();
            if(appearance != null)
            {
                Canvas foregroundCanvas = appearance.UIForegroundScaler.GetComponent<Canvas>();
                Camera foregroundCamera = foregroundCanvas.worldCamera;
                if (foregroundCamera != null)
                {
                    if (m_cameraUtility != null)
                    {
                        m_cameraUtility.EnablePostProcessing(foregroundCamera, false);
                        m_cameraUtility.SetBackgroundColor(foregroundCamera, new Color(0, 0, 0, 0));
                    }

                    GameObject foregroundLayer = new GameObject("ForegroundLayer");
                    foregroundLayer.transform.SetParent(rte.Root, false);
                    foregroundCanvas = foregroundLayer.AddComponent<Canvas>();
                    foregroundCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    foregroundCamera.gameObject.SetActive(false);
                    m_foregroundOutput = new GameObject("Output");
                    m_foregroundOutput.transform.SetParent(foregroundCanvas.transform, false);
                    m_foregroundOutput.AddComponent<RectTransform>().Stretch();

                    RenderTextureCamera renderTextureCamera = foregroundCamera.gameObject.AddComponent<RenderTextureCamera>();
                    renderTextureCamera.OutputRoot = foregroundCanvas.gameObject.GetComponent<RectTransform>();
                    renderTextureCamera.Output = m_foregroundOutput.AddComponent<RawImage>();
                    renderTextureCamera.OverlayMaterial = new Material(Shader.Find("Battlehub/HDRP/RTEditor/UIForeground"));
                    foregroundCamera.gameObject.SetActive(true);

                    foregroundCanvas.sortingOrder = -1;
                }

                Canvas backgroundCanvas = IOC.Resolve<IRTEAppearance>().UIBackgroundScaler.GetComponent<Canvas>();
                if (backgroundCanvas != null)
                {
                    Camera backgroundCamera = backgroundCanvas.worldCamera;
                    if (m_cameraUtility != null)
                    {
                        m_cameraUtility.EnablePostProcessing(backgroundCamera, false);
                        m_cameraUtility.SetBackgroundColor(backgroundCamera, new Color(0, 0, 0, 0));
                    }
                }
            } 
        }
        protected override void OnEditorClosed()
        {
            Debug.LogError("HDRPInit.OnEditorClosed");
            base.OnEditorClosed();
            if(m_outlineManager != null)
            {
                Destroy(m_outlineManager);
                m_outlineManager = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_outlineManager != null)
            {
                Destroy(m_outlineManager);
                m_outlineManager = null;
            }
        }
    }
}


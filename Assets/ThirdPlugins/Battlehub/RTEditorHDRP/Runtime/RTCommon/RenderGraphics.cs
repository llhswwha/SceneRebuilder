using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine;
using System.Collections.Generic;

namespace Battlehub.RTCommon.HDRP
{
    public class RenderGraphics : CustomPass
    {
        protected Dictionary<Camera, List<IRTECamera>> m_cameras;

        [SerializeField]
        private bool m_afterImageEffects = false;

        [SerializeField]
        private bool m_clearDepth = true;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            base.Setup(renderContext, cmd);
            m_cameras = new Dictionary<Camera, List<IRTECamera>>();

            RTECamera[] cameras = Object.FindObjectsOfType<RTECamera>();
            foreach(RTECamera camera in cameras)
            {
                AddCamera(camera);
            }

            RTECamera.Created += OnRTECameraCreated;
            RTECamera.Destroyed += OnRTECameraDestroyed;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            RTECamera.Created -= OnRTECameraCreated;
            RTECamera.Destroyed -= OnRTECameraDestroyed;

            m_cameras.Clear();
        }

        private void OnRTECameraCreated(IRTECamera camera)
        {
            AddCamera(camera);
        }

        private void OnRTECameraDestroyed(IRTECamera camera)
        {
            RemoveCamera(camera);
        }

        protected virtual void AddCamera(IRTECamera camera)
        {
            switch (injectionPoint)
            {
                case CustomPassInjectionPoint.BeforeTransparent:
                    if (camera.Event == CameraEvent.BeforeForwardAlpha ||
                        camera.Event == CameraEvent.AfterForwardAlpha)
                    {
                        List<IRTECamera> cameras = GetList(camera);
                        cameras.Add(camera);
                    }
                    break;
                case CustomPassInjectionPoint.BeforePostProcess:
                    if(!m_afterImageEffects)
                    {
                        if (camera.Event == CameraEvent.BeforeImageEffects ||
                            camera.Event == CameraEvent.BeforeImageEffectsOpaque)
                        {
                            List<IRTECamera> cameras = GetList(camera);
                            cameras.Add(camera);
                        }
                    }
                    else
                    {
                        if (camera.Event == CameraEvent.AfterImageEffects ||
                            camera.Event == CameraEvent.AfterImageEffectsOpaque)
                        {
                            List<IRTECamera> cameras = GetList(camera);
                            cameras.Add(camera);
                        }
                    }

                    break;
                //case CustomPassInjectionPoint.AfterPostProcess:
                //    if (camera.Event == CameraEvent.AfterImageEffects ||
                //       camera.Event == CameraEvent.AfterImageEffectsOpaque)
                //    {
                //        List<IRTECamera> cameras = GetList(camera);
                //        cameras.Add(camera);
                //    }
                //    break;
            }
        }

        protected virtual void RemoveCamera(IRTECamera camera)
        {
            List<IRTECamera> list;
            if (m_cameras.TryGetValue(camera.Camera, out list))
            {
                list.Remove(camera);
                if (list.Count == 0)
                {
                    m_cameras.Remove(camera.Camera);
                }
            }
        }

        protected List<IRTECamera> GetList(IRTECamera camera)
        {
            List<IRTECamera> list;
            if (!m_cameras.TryGetValue(camera.Camera, out list))
            {
                list = new List<IRTECamera>();
                m_cameras.Add(camera.Camera, list);
            }
            return list;
        }

        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
        {
            List<IRTECamera> list;
            if(m_cameras.TryGetValue(hdCamera.camera, out list))
            {
                cmd.ClearRenderTarget(m_clearDepth, false, Color.black);

                for (int i = 0; i < list.Count; ++i)
                {
                    IRTECamera rteCamera = list[i];
                    rteCamera.CommandBufferOverride = cmd;
                    rteCamera.RefreshCommandBuffer();
                }
            }   
        }
    }
}
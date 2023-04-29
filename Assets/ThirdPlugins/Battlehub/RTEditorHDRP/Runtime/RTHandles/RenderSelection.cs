using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

namespace Battlehub.RTCommon.HDRP
{
    public class RenderSelection : RenderGraphics
    {
        protected override void AddCamera(IRTECamera camera)
        {
            if (camera.Event == CameraEvent.AfterEverything)
            {
                List<IRTECamera> cameras = GetList(camera);
                cameras.Add(camera);
            }
        }

        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
        {
            List<IRTECamera> list;
            if (m_cameras.TryGetValue(hdCamera.camera, out list))
            {
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
using Battlehub.ProBuilderIntegration;
using Battlehub.RTCommon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Battlehub.RTBuilder.HDRP
{
    public class PBSelectionPickerRendererHDRP : PBSelectionPickerRenderer
    {
        private IRTECamera m_rteCamera;
        private List<Renderer[]> m_renderers = new List<Renderer[]>();

        protected override void PrepareCamera(Camera renderCamera)
        {
            base.PrepareCamera(renderCamera);

            IRenderPipelineCameraUtility cameraUtil = IOC.Resolve<IRenderPipelineCameraUtility>();
            cameraUtil.EnablePostProcessing(renderCamera, false);
            cameraUtil.SetBackgroundColor(renderCamera, Color.white);
        }

        protected override void Render(Shader shader, string tag, Camera renderCamera)
        {
            IRTEGraphics graphics = IOC.Resolve<IRTEGraphics>();
            m_rteCamera = graphics.CreateCamera(renderCamera, UnityEngine.Rendering.CameraEvent.AfterForwardAlpha, false, true);

            for (int i = 0; i < m_renderers.Count; ++i)
            {
                Renderer[] renderers = m_renderers[i];
                for (int j = 0; j < renderers.Length; ++j)
                {
                    m_rteCamera.RenderersCache.Add(renderers[j]);
                }
            }

            //Material material = new Material(shader);
            //m_rteCamera.RenderersCache.MaterialOverride = material;

            bool invertCulling = GL.invertCulling;

            GL.invertCulling = true;
            renderCamera.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
            renderCamera.Render();
            GL.invertCulling = invertCulling;

            //Object.Destroy(material);
            m_rteCamera.Destroy();

            m_renderers.Clear();
        }

        protected override void GenerateEdgePickingObjects(IList<ProBuilderMesh> selection, bool doDepthTest, out Dictionary<uint, SimpleTuple<ProBuilderMesh, Edge>> map, out GameObject[] depthObjects, out GameObject[] pickerObjects)
        {
            base.GenerateEdgePickingObjects(selection, doDepthTest, out map, out depthObjects, out pickerObjects);
            if (depthObjects != null)
            {
                m_renderers.Add(depthObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
            if (pickerObjects != null)
            {
                m_renderers.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
        }

        protected override GameObject[] GenerateFacePickingObjects(IList<ProBuilderMesh> selection, out Dictionary<uint, SimpleTuple<ProBuilderMesh, Face>> map)
        {
            GameObject[] pickerObjects = base.GenerateFacePickingObjects(selection, out map);
            m_renderers.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            return pickerObjects;
        }

        protected override void GenerateVertexPickingObjects(IList<ProBuilderMesh> selection, bool doDepthTest, out Dictionary<uint, SimpleTuple<ProBuilderMesh, int>> map, out GameObject[] depthObjects, out GameObject[] pickerObjects)
        {
            base.GenerateVertexPickingObjects(selection, doDepthTest, out map, out depthObjects, out pickerObjects);
            if (depthObjects != null)
            {
                m_renderers.Add(depthObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
            if (pickerObjects != null)
            {
                m_renderers.Add(pickerObjects.SelectMany(go => go.GetComponentsInChildren<Renderer>()).ToArray());
            }
        }
    }

}

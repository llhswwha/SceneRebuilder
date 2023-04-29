using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Unity.RenderStreaming
{
    [RequireComponent(typeof(Camera))]
    public class CameraStreamer : MonoBehaviour
    {
        [SerializeField, Tooltip("Streaming size should match display aspect ratio")]
        private Vector2Int streamingSize = new Vector2Int(1280, 720);

        private Camera m_camera;
        private VideoStreamTrack m_track;

        void Awake()
        {
            m_camera = GetComponent<Camera>();
        }
        public VideoStreamTrack GetTrack()
        {
            return m_track;
        }
        
        //Unity.WebRTC.RenderTextureDepth
        public int textureDepth=1000000;

        //public Unity.WebRTC.RenderTextureDepth textureDepth;
        void OnEnable()
        {
            // todo(kazuki): remove bitrate parameter because it is not supported
            if (RenderStreaming.Instance && RenderStreaming.Instance.StartOnAwake)
            {
                m_track = m_camera.CaptureStreamTrack(streamingSize.x, streamingSize.y, textureDepth);
                RenderStreaming.Instance?.AddVideoStreamTrack(m_track);
            }
            else
            {
                RenderStreaming.Instance?.AddWaitStream(this);
            }
        }
        [ContextMenu("ChangeTrack")]
        public void ChangeTrack()
        {
            m_track = m_camera.CaptureStreamTrack(streamingSize.x, streamingSize.y, textureDepth);
            RenderStreaming.Instance?.ChangeResolution(m_track);
        }

        public VideoStreamTrack ChangeTrackSize(int width,int height)
        {
            streamingSize = new Vector2Int(width,height);
            m_track = m_camera.CaptureStreamTrack(streamingSize.x, streamingSize.y, textureDepth);
            RenderStreaming.Instance?.ChangeResolution(m_track);
            return m_track;
        }

        public void CaptureStreamTrack()
        {
            m_track = m_camera.CaptureStreamTrack(streamingSize.x, streamingSize.y, textureDepth);
            RenderStreaming.Instance?.AddVideoStreamTrack(m_track);
        }

        public VideoStreamTrack setResolution(int x, int y)
        {
            streamingSize.x = x;
            streamingSize.y = y;
            m_track = CreateTrack();
            return m_track;
        }

        protected VideoStreamTrack CreateTrack()
        {
            RenderTexture rt;
            if (m_camera.targetTexture != null && m_camera.targetTexture.width == streamingSize.x && m_camera.targetTexture.height == streamingSize.y)
            {
                rt = m_camera.targetTexture;
                RenderTextureFormat supportFormat = WebRTC.WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
                GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(supportFormat, RenderTextureReadWrite.Default);
                GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, FormatUsage.Render);
                GraphicsFormat format = graphicsFormat == compatibleFormat ? graphicsFormat : compatibleFormat;

                if (rt.graphicsFormat != format)
                {
                    Debug.LogWarning(
                        $"This color format:{rt.graphicsFormat} not support in unity.webrtc. Change to supported color format:{format}.");
                    rt.Release();
                    rt.graphicsFormat = format;
                    rt.Create();
                }

                m_camera.targetTexture = rt;
            }
            else
            {
                var format = WebRTC.WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
                rt = new RenderTexture(streamingSize.x, streamingSize.y, 0, format)
                {
                    antiAliasing = 1
                };
                rt.Create();


                RenderTexture temp_target_texture = null;
                if (m_camera.targetTexture != null)
                    temp_target_texture = m_camera.targetTexture;

                m_camera.targetTexture = rt;

                if (temp_target_texture != null)
                    temp_target_texture.Release();
            }

            return new VideoStreamTrack(rt);
        }

        void OnDisable()
        {
            RenderStreaming.Instance?.RemoveVideoStreamTrack(m_track);
            RenderStreaming.Instance?.RemoveWatiStream(this);
        }
    }
}

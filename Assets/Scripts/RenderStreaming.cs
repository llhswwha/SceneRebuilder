using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.WebRTC;
using System.Text.RegularExpressions;
using Unity.RenderStreaming.Signaling;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Unity.RenderStreaming
{
    using DataChannelDictionary = Dictionary<int, RTCDataChannel>;

    [Serializable]
    public class ButtonClickEvent : UnityEngine.Events.UnityEvent<int> { }

    [Serializable]
    public class ButtonClickElement
    {
        [Tooltip("Specifies the ID on the HTML")]
        public int elementId;
        public ButtonClickEvent click;
    }

    public class RenderStreaming : MonoBehaviour
    {
        public static void ConnectRenderStreaming()
        {
            RenderStreaming instance = RenderStreaming.Instance;
            if (instance && !instance.StartOnAwake)
            {
                SystemSettingHelper.GetSystemSetting();
                var commSetting = SystemSettingHelper.communicationSetting;
                Debug.Log($"RenderStreaming.ConnectRenderStreaming url:{commSetting.RenderStreamingUrl}");
                instance.StartConnect(commSetting.RenderStreamingType, commSetting.RenderStreamingUrl,
                    commSetting.RenderStreamingStartBirate, commSetting.RenderStreamingsMinBirate, commSetting.RenderStreamingMaxBirate,commSetting.Interval);
                instance.SetAutoCloseInfo(SystemSettingHelper.communicationSetting.AutoCloseInUserQuit);
            }
            else
            {
                Debug.LogError("RenderStreaming.Instance is null...");
            }
        }
        
#pragma warning disable 0649
        [SerializeField, Tooltip("Address for signaling server")]
        private string urlSignaling = "http://localhost";

        [SerializeField, Tooltip("Array to set your own STUN/TURN servers")]
        private RTCIceServer[] iceServers = new RTCIceServer[]
        {
            new RTCIceServer()
            {
                urls = new string[] { "stun:stun.l.google.com:19302" }
            }
        };

        [SerializeField, Tooltip("Time interval for polling from signaling server")]
        private float interval = 5.0f;

        [SerializeField, Tooltip("Enable or disable hardware encoder")]
        private bool hardwareEncoderSupport = true;

        [SerializeField, Tooltip("Enable or disable wss offfer")]
        private bool enableWssOffer = true;

        [SerializeField, Tooltip("Array to set your own click event")]
        private ButtonClickElement[] arrayButtonClickEvent;
#pragma warning restore 0649

        private ISignaling m_signaling;
        private WebSocketSignaling wss_signaling;
        private readonly Dictionary<string, RTCPeerConnection> m_mapConnectionIdAndPeer = new Dictionary<string, RTCPeerConnection>();
        private readonly Dictionary<RTCPeerConnection, DataChannelDictionary> m_mapPeerAndChannelDictionary = new Dictionary<RTCPeerConnection, DataChannelDictionary>();
        private readonly Dictionary<RemoteInput, SimpleCameraController> m_remoteInputAndCameraController = new Dictionary<RemoteInput, SimpleCameraController>();
        private readonly Dictionary<RTCDataChannel, RemoteInput> m_mapChannelAndRemoteInput = new Dictionary<RTCDataChannel, RemoteInput>();
        private readonly List<SimpleCameraController> m_listController = new List<SimpleCameraController>();
        private readonly List<VideoStreamTrack> m_listVideoStreamTrack = new List<VideoStreamTrack>();
        //private MediaStream m_audioStream;
        private DefaultInput m_defaultInput;
        private RTCConfiguration m_conf;

        public static RenderStreaming Instance { get; private set; }

        public Action<string> MsgRecieveAction;
        private List<CameraStreamer> videoStreamTrackWaitlist = new List<CameraStreamer>();

        [SerializeField, Tooltip("Enable or disable hardware encoder")]
        private bool startOnAwake = true;

        private float startBitrate = 9000;
        private float minBirtate = 8000;
        private float maxBirtate = 10000;
        [SerializeField]
        public bool isReadXML = true;
        public string m_sessionId = "";
        public bool StartOnAwake
        {
            get { return startOnAwake; }
            set { startOnAwake = value; }
        }
        enum UnityEventType
        {
            SwitchVideo = 0
        }


        public void Awake()
        {
            Instance = this;
            Debug.Log("RenderStreaming.Awake isReadXML:" + isReadXML);
            if (isReadXML)
            {
                CommunicationSetting communicationSetting = null;
                SystemSettingHelper.GetSystemSetting(() =>
                {
                    communicationSetting = SystemSettingHelper.communicationSetting;
                });

                if (communicationSetting == null)
                {
                    Debug.LogError("communicationSetting == null");
                    return;
                }
                int typeT = communicationSetting.RenderStreamingType;
                string urlT = communicationSetting.RenderStreamingUrl;
                float startBirateT = communicationSetting.RenderStreamingStartBirate;
                float minBitrateT = communicationSetting.RenderStreamingsMinBirate;
                float maxBirateT = communicationSetting.RenderStreamingMaxBirate;

#if !UNITY_EDITOR
                        hardwareEncoderSupport = typeT==0?false:true;
                        urlSignaling = urlT;
                        startBitrate = startBirateT;
                        minBirtate = minBitrateT;
                        maxBirtate = maxBirateT;
#endif
            }

            if (startOnAwake)
            {
                //var encoderType = hardwareEncoderSupport ? EncoderType.Hardware : EncoderType.Software;
                //WebRTC.WebRTC.Initialize(encoderType);
                WebRTC.WebRTC.Initialize();
                m_defaultInput = new DefaultInput();
                EnhancedTouchSupport.Enable();
            }
        }

        public void OnDestroy()
        {
            Instance = null;
            EnhancedTouchSupport.Disable();
            WebRTC.WebRTC.Dispose();
            RemoteInputReceiver.Dispose();
            //Unity.WebRTC.Audio.Stop();

            StopSingaling();
        }
        public void Start()
        {
            if (startOnAwake)
            {
                //m_audioStream = Unity.WebRTC.Audio.CaptureStream();
                m_conf = default;
                m_conf.iceServers = iceServers;
                StartCoroutine(WebRTC.WebRTC.Update());
            }

        }

        public void StartConnect(int typeT, string urlT, float startBirateT, float minBitrateT, float maxBirateT, float intervalT)
        {
            if (startOnAwake) return;
            //打包后才使用systemSetting.xml中配置，未打包使用面板上设置参数
#if !UNITY_EDITOR
            hardwareEncoderSupport = typeT==0?false:true;
            urlSignaling = urlT;
            startBitrate = startBirateT;
            minBirtate = minBitrateT;
            maxBirtate = maxBirateT;
            interval = intervalT;
#endif
            Debug.LogFormat("RenderStreaming.StartConnect Type:{0} URL:{1} startBirateT{2} maxBirateT:{3}", typeT, urlT, startBirateT, maxBirateT);

            //var encoderType = hardwareEncoderSupport ? EncoderType.Hardware : EncoderType.Software;
            //WebRTC.WebRTC.Initialize(encoderType);
            WebRTC.WebRTC.Initialize();
            m_defaultInput = new DefaultInput();
            EnhancedTouchSupport.Enable();

            foreach (var watiStream in videoStreamTrackWaitlist)
            {
                if (watiStream != null) watiStream.CaptureStreamTrack();
            }
            videoStreamTrackWaitlist.Clear();

            if (this.m_signaling == null)
            {
                Debug.Log($"RenderStreaming.StartConnect HttpSignaling(Create) urlSignaling:{urlSignaling} interval:{interval}");
                // if (urlSignaling.StartsWith("ws"))
                //{
                //   this.m_signaling = new WebSocketSignaling(urlSignaling, interval);
                // }
                // else
                // {
                this.m_signaling = new HttpSignaling(urlSignaling, interval);
                HttpSignaling singalT = this.m_signaling as HttpSignaling;
                singalT.EnableWssOffer = enableWssOffer;
                // }

                this.m_signaling.OnOffer += OnOffer;
                this.m_signaling.OnIceCandidate += OnIceCandidate;
                Debug.Log($"RenderStreaming.StartConnect HttpSignaling(Create2) enableWssOffer:{enableWssOffer}");

            }

            Debug.Log($"RenderStreaming.StartConnect HttpSignaling(Start) urlSignaling:{urlSignaling} interval:{interval}");

            this.m_signaling.Start();
            //m_audioStream = Unity.WebRTC.Audio.CaptureStream();
            m_conf = default;
            m_conf.iceServers = iceServers;
            StartCoroutine(WebRTC.WebRTC.Update());
        }

        public void enableWss()
        {
            wss_signaling = new WebSocketSignaling(urlSignaling.Replace("https", "wss"));
        }


        void OnEnable()
        {
            if (!startOnAwake) return;
            if (this.m_signaling == null)
            {
                //if (urlSignaling.StartsWith("ws"))
                // {

                // }
                //else
                // {
                this.m_signaling = new HttpSignaling(urlSignaling, interval);
                HttpSignaling singalT = this.m_signaling as HttpSignaling;
                singalT.EnableWssOffer = enableWssOffer;
                // }

                this.m_signaling.OnOffer += OnOffer;
                this.m_signaling.OnIceCandidate += OnIceCandidate;


            }
            this.m_signaling.Start();


        }

        public void AddController(SimpleCameraController controller)
        {
            m_listController.Add(controller);
            controller.SetInput(m_defaultInput);
        }

        public void RemoveController(SimpleCameraController controller)
        {
            m_listController.Remove(controller);
        }

        public void AddVideoStreamTrack(VideoStreamTrack track)
        {
            m_listVideoStreamTrack.Add(track);
        }

        public void AddWaitStream(CameraStreamer track)
        {
            if (!videoStreamTrackWaitlist.Contains(track)) videoStreamTrackWaitlist.Add(track);
        }
        public void RemoveWatiStream(CameraStreamer track)
        {
            if (videoStreamTrackWaitlist.Contains(track)) videoStreamTrackWaitlist.Remove(track);
        }
        public void RemoveVideoStreamTrack(VideoStreamTrack track)
        {
            if (m_listVideoStreamTrack.Contains(track)) m_listVideoStreamTrack.Remove(track);
        }

        void OnDisable()
        {
            StopSingaling();
        }

        private void StopSingaling()
        {
            if (this.m_signaling != null)
            {
                this.m_signaling.Stop();
                this.m_signaling = null;
            }
            if (wss_signaling != null)
            {
                wss_signaling.Stop();
                wss_signaling = null;
            }
        }

        void OnOffer(ISignaling signaling, DescData e)
        {
            Debug.Log($"RenderStreaming.OnOffer e:{e}");
            RTCSessionDescription _desc;
            _desc.type = RTCSdpType.Offer;
            _desc.sdp = e.sdp;
            var connectionId = e.connectionId;
            if (m_mapConnectionIdAndPeer.ContainsKey(connectionId))
            {
                return;
            }
            var pc = new RTCPeerConnection();
            m_mapConnectionIdAndPeer.Add(e.connectionId, pc);

            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(pc, channel); });
            pc.SetConfiguration(ref m_conf);
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                if (candidate != null) Debug.LogFormat("OnIceCandidate  Address:{0} Port:{1}", candidate.Address, candidate.Port);
                m_signaling.SendCandidate(e.connectionId, candidate);
            });
            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if (state == RTCIceConnectionState.Disconnected)
                {
                    pc.Close();
                    m_mapConnectionIdAndPeer.Remove(e.connectionId);
                }
            });
            //make video bit rate starts at 16000kbits, and 160000kbits at max.
            string pattern = @"(a=fmtp:\d+ .*level-asymmetry-allowed=.*)\r\n";
            //_desc.sdp = Regex.Replace(_desc.sdp, pattern, "$1;x-google-start-bitrate=16000;x-google-max-bitrate=160000\r\n");
            _desc.sdp = Regex.Replace(_desc.sdp, pattern, string.Format("$1;x-google-start-bitrate={0};x-google-min-bitrate={1};x-google-max-bitrate={2}\r\n", startBitrate, minBirtate, maxBirtate));//16000, 80000, 160000;x-google-min-bitrate=12000  A:20000, 25000, 30000
            pc.SetRemoteDescription(ref _desc);
            foreach (var track in m_listVideoStreamTrack)
            {
                pc.AddTrack(track);
            }
            //foreach (var track in m_audioStream.GetTracks())
            //{
            //    pc.AddTrack(track);
            //}

            RTCOfferAnswerOptions options = default;
            var op = pc.CreateAnswer(ref options);
            while (op.MoveNext())
            {
            }
            if (op.IsError)
            {
                Debug.LogError($"Network Error: {op.Error}");
                return;
            }

            var desc = op.Desc;
            var opLocalDesc = pc.SetLocalDescription(ref desc);
            while (opLocalDesc.MoveNext())
            {
            }
            if (opLocalDesc.IsError)
            {
                Debug.LogError($"Network Error: {opLocalDesc.Error}");
                return;
            }
            Debug.LogError("OnOffer.SendAnswer:" + connectionId + " desc:\n" + desc.sdp);
            m_signaling.SendAnswer(connectionId, desc);
        }

        public void wssOnOffer(DescData e)
        {
            Debug.Log($"RenderStreaming.wssOnOffer");
            //改为推送模式
            RTCSessionDescription _desc;
            _desc.type = RTCSdpType.Offer;
            _desc.sdp = e.sdp;
            var connectionId = e.connectionId;
            if (m_mapConnectionIdAndPeer.ContainsKey(connectionId))
            {
                return;
            }
            var pc = new RTCPeerConnection();
            m_mapConnectionIdAndPeer.Add(e.connectionId, pc);

            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(pc, channel); });
            pc.SetConfiguration(ref m_conf);
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                if (candidate != null) Debug.LogFormat("OnIceCandidate  Address:{0} Type:{1} Port:{2}", candidate.Address,candidate.Protocol, candidate.Port);
                m_signaling.SendCandidate(e.connectionId, candidate);
            });
            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if (state == RTCIceConnectionState.Disconnected)
                {
                    pc.Close();
                    m_mapConnectionIdAndPeer.Remove(e.connectionId);
                }
            });
            //Debug.LogError("SetRemoteDescription sdp before:" + _desc.sdp);
            //make video bit rate starts at 16000kbits, and 160000kbits at max.
            string pattern = @"(a=fmtp:\d+ .*level-asymmetry-allowed=.*)\r\n";
            //_desc.sdp = Regex.Replace(_desc.sdp, pattern, "$1;x-google-start-bitrate=16000;x-google-max-bitrate=160000\r\n");
            _desc.sdp = Regex.Replace(_desc.sdp, pattern, string.Format("$1;x-google-start-bitrate={0};x-google-min-bitrate={1};x-google-max-bitrate={2}\r\n", startBitrate, minBirtate, maxBirtate));//16000, 80000, 160000;x-google-min-bitrate=12000  A:20000, 25000, 30000

            if (SystemSettingHelper.systemSetting.IsDebug)
            {
                //要手动修改sdp,这里的SetRemoteDescription要注释掉
                pc.SetRemoteDescription(ref _desc);
            }
            else
            {
                string dpInfo = _desc.sdp;
                //SetCodecPreferences改GPU编码无效，先手动处理sdp信息 wk
                _desc.sdp = ReplaceInfo(_desc.sdp);
                try
                {
                    pc.SetRemoteDescription(ref _desc);
                }
                catch (Exception ex)
                {
                    Debug.LogError("SetRemoteDescription.Exception:" + ex.ToString());
                    _desc.sdp = dpInfo;
                    pc.SetRemoteDescription(ref _desc);
                }
            }
            Debug.LogErrorFormat("HardwareEncoder:{0}\n SetRemoteDescription sdp after:{1}",hardwareEncoderSupport,_desc.sdp);


            //// Get all available video codecs.
            //var codecs = RTCRtpSender.GetCapabilities(TrackKind.Video).codecs;
            //// Filter codecs.
            //var h264Codecs = codecs.Where(codec => codec.mimeType == "video/H264");

            //foreach (var item in h264Codecs)
            //{
            //    Debug.LogErrorFormat("H264Info: channlel-{0}|clockRate-{1}|mimeType-{2}|sdpFmtpline-{3}", item.channels, item.clockRate, item.mimeType, item.sdpFmtpLine);
            //}           

            foreach (var track in m_listVideoStreamTrack)
            {
                pc.AddTrack(track);
            }
            //foreach (var track in m_audioStream.GetTracks())
            //{
            //    pc.AddTrack(track);
            //}
            //var transceiver = pc.GetTransceivers();
            //foreach(var item in transceiver)
            //{
            //    RTCErrorType error = item.SetCodecPreferences(h264Codecs.ToArray());
            //    if (error != RTCErrorType.None)
            //    {
            //        Debug.LogErrorFormat("RTCRtpTransceiver.SetCodecPreferences failed. {0}", error);

            //    }
            //    else
            //    {
            //        Debug.LogError(item.Mid+"SetCodecPreferences success:" + RTCRtpSender.GetCapabilities(TrackKind.Video));
            //    }
            //}           
            //Debug.LogError("SetRemoteDescription sdp2:" + pc.RemoteDescription.sdp);

            RTCOfferAnswerOptions options = default;
            var op = pc.CreateAnswer(ref options);
            while (op.MoveNext())
            {
            }
            if (op.IsError)
            {
                Debug.LogError($"Network Error: {op.Error}");
                return;
            }

            var desc = op.Desc;
            var opLocalDesc = pc.SetLocalDescription(ref desc);
            while (opLocalDesc.MoveNext())
            {
            }
            if (opLocalDesc.IsError)
            {
                Debug.LogError($"Network Error: {opLocalDesc.Error}");
                return;
            }
            Debug.LogError("OnOffer.SendAnswer:" + connectionId + " desc:\n" + desc.sdp);
            m_signaling.SendAnswer(connectionId, desc);
        }
        private string ReplaceInfo(string sdp)
        {
            if (!hardwareEncoderSupport) return sdp;

            string encoderType = hardwareEncoderSupport ? "H264" : "VP8";

            string[] infos = sdp.Split('\n');
            string newInfo = "";

            bool isOtherCodec = false;
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].StartsWith("a=rtpmap"))
                {
                    if (infos[i].Contains(encoderType))
                    {
                        isOtherCodec = false;
                        newInfo += infos[i] + "\n";
                    }
                    else
                    {
                        isOtherCodec = true;
                    }
                }
                else
                {
                    if (!isOtherCodec)
                    {
                        newInfo += infos[i] + (infos[i] == "" || infos[i].EndsWith("\n") ? "" : "\n");
                    }
                    else
                    {
                        if (infos[i].Contains("a=fmtp") || infos[i].Contains("a=rtcp"))
                        {
                            continue;
                        }
                        else
                        {
                            isOtherCodec = false;
                            newInfo += infos[i] + (infos[i] == "" || infos[i].EndsWith("\n") ? "" : "\n");
                        }
                    }
                }
            }
            //Debug.LogError("New SDP:\n"+newInfo);
            return newInfo;
        }
        /// <summary>
        /// 是否连接上云渲染服务器
        /// </summary>
        /// <returns></returns>
        public bool IsConnectServer()
        {
            if (m_signaling is HttpSignaling)
            {
                var httpSig = m_signaling as HttpSignaling;
                return !string.IsNullOrEmpty(httpSig.SessionId);
            }
            return true;
        }

        public override string ToString()
        {
            string sessionId = "null session";
            if (m_signaling is HttpSignaling)
            {
                var httpSig = m_signaling as HttpSignaling;
                sessionId = $"urlSignaling:{urlSignaling} sessionId:{httpSig.SessionId}"; 
            }
            return sessionId;
        }

        void OnIceCandidate(ISignaling signaling, CandidateData e)
        {
            if (!m_mapConnectionIdAndPeer.TryGetValue(e.connectionId, out var pc))
            {
                return;
            }

            //RTCIceCandidate​ _candidate = default;
            //_candidate.Candidate = e.candidate;
            //_candidate.sdpMLineIndex = e.sdpMLineIndex;
            //_candidate.sdpMid = e.sdpMid;

            //pc.AddIceCandidate(ref _candidate);
            RTCIceCandidateInit option = new RTCIceCandidateInit
            {
                candidate = e.candidate,
                sdpMLineIndex = e.sdpMLineIndex,
                sdpMid = e.sdpMid
            };

            if (!pc.AddIceCandidate(new RTCIceCandidate(option)))
            {
                Debug.LogWarning($"{pc} this candidate can't accept current signaling state {pc.SignalingState}.");
            }
        }

        void OnDataChannel(RTCPeerConnection pc, RTCDataChannel channel)
        {
            Debug.Log($"RenderStreaming.OnDataChannel_01");

            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }
            channels.Add(channel.Id, channel);

            List<RTCPeerConnection> deletrtcpRTCP = new List<RTCPeerConnection>();            //删除连接为空的字典
            foreach (var itemchannel in m_mapPeerAndChannelDictionary)
            {
                if (itemchannel.Value.Count == 0)
                {
                    deletrtcpRTCP.Add(itemchannel.Key);
                    itemchannel.Key.Dispose();
                    Log.Error("没连接");
                }
                else
                {
                    Log.Error("连接了" + itemchannel.Value.Count + "个");
                    wss_signaling.WSConnectedState("true");
                    if (RenderSProcessHeartbeat.Instance != null) RenderSProcessHeartbeat.Instance.SetRenderStreamingDoing(true);
                }

            }
            if (deletrtcpRTCP.Count > 0)
            {
                foreach (var itemdeletchannel in deletrtcpRTCP)
                {
                    m_mapPeerAndChannelDictionary.Remove(itemdeletchannel);
                }
                deletrtcpRTCP = new List<RTCPeerConnection>();
            }
            if (channel.Label != "data")
            {
                return;
            }

            Debug.Log($"RenderStreaming.OnDataChannel_02 RemoteInputReceiver.Create");
            RemoteInput input = RemoteInputReceiver.Create();
            input.ActionButtonClick = OnButtonClick;
            input.MsgRecieveAction = MsgRecieveAction;
            // device.current must be changed after creating devices
            m_defaultInput.MakeCurrent();

            m_mapChannelAndRemoteInput.Add(channel, input);
            channel.OnMessage = bytes => m_mapChannelAndRemoteInput[channel].ProcessInput(bytes);
            channel.OnClose = () => OnCloseChannel(channel);

            Debug.Log($"RenderStreaming.OnDataChannel_03 SimpleCameraController MsgRecieveAction:{MsgRecieveAction!=null}");
            // find controller that not assigned remote input
            SimpleCameraController controller = m_listController
                .FirstOrDefault(_controller => !m_remoteInputAndCameraController.ContainsValue(_controller));

            if (controller != null)
            {
                Debug.Log($"RenderStreaming.OnDataChannel_04");
                controller.SetInput(input);
                m_remoteInputAndCameraController.Add(input, controller);

                byte index = (byte)m_listController.IndexOf(controller);
                byte[] bytes = { (byte)UnityEventType.SwitchVideo, index };
                channel.Send(bytes);
                Debug.Log($"RenderStreaming.OnDataChannel_05");
            }
        }

        public void SendMsg(string msg)
        {
            foreach (var peers in m_mapPeerAndChannelDictionary.Values)
            {
                if (peers == null) continue;
                foreach (var item in peers.Values)
                {
                    if (item == null || item.ReadyState != RTCDataChannelState.Open) continue;
                    item.Send(msg);
                }
            }
        }
        public ulong bitrateInfo;
        public uint framerateInfo;
        public double scaleResoluton;
        [ContextMenu("ChangeBirate")]
        public void ChangeBirate()
        {
            ChangeFrameBitrate(bitrateInfo, framerateInfo);
        }

        public void ChangeFrameBitrate(ulong bitrateT, uint frameT)
        {
            if (m_mapConnectionIdAndPeer == null) return;
            Debug.Log("m_mapConnectionIdAndPeer:" + m_mapConnectionIdAndPeer.Values.Count);
            foreach (var item in m_mapConnectionIdAndPeer.Values)
            {
                var sendersT = item.GetSenders();
                foreach (var s in sendersT)
                {
                    var paramGroups = s.GetParameters();
                    //var encodings = paramGroups.encodings.ToList().GetRange(0, 1);
                    //paramGroups.encodings = encodings.ToArray();
                    //s.SetParameters(paramGroups);

                    foreach (var encoding in paramGroups.encodings)
                    {
                        if (bitrateT != 0) encoding.maxBitrate = bitrateT * 1024 * 1024;
                        if (frameT != 0) encoding.maxFramerate = frameT;
                        if (scaleResoluton != 0) encoding.scaleResolutionDownBy = scaleResoluton;
                    }
                    var errorInfo = s.SetParameters(paramGroups);
                    if (errorInfo.errorType != RTCErrorType.None) Debug.LogError("ErrorInfo:" + errorInfo.errorType + " msg:" + errorInfo.message);
                }
            }
        }

        public void ChangeResolution(MediaStreamTrack trackT)
        {
            if (m_mapConnectionIdAndPeer == null) return;
            //Debug.Log("m_mapConnectionIdAndPeer:" + m_mapConnectionIdAndPeer.Values.Count);
            foreach (var item in m_mapConnectionIdAndPeer.Values)
            {
                var sendersT = item.GetSenders();
                foreach (var s in sendersT)
                {
                    if (s.Track.Kind == trackT.Kind)
                    {
                        //s.ReplaceTrack(null);
                        s.ReplaceTrack(trackT);
                        //Debug.Log("Replace track1:" + s.Track.Kind);
                    }
                    else
                    {
                        Debug.Log("Replace track2:" + s.Track.Kind);
                    }
                }
            }
        }


        [ContextMenu("ChangeCamera0")]
        public void ChangeCamera0()
        {
            TestChangeCamera(0);
        }
        [ContextMenu("ChangeCamera1")]
        public void ChangeCamera1()
        {
            TestChangeCamera(1);
        }
        public void TestChangeCamera(int index)
        {
            Debug.LogError("m_mapPeerAndChannelDictionary.count:" + m_mapPeerAndChannelDictionary.Count);
            foreach (var item in m_mapPeerAndChannelDictionary.Values)
            {
                Debug.LogError("dataChannelDictionary.count:" + item.Count);
                var items = item.Values.ToList();
                byte indexT = (byte)index;
                byte[] bytes = { (byte)UnityEventType.SwitchVideo, indexT };
                items[0].Send(bytes);
            }
        }

        void OnCloseChannel(RTCDataChannel channel)
        {

            RemoteInput input = m_mapChannelAndRemoteInput[channel];
            RemoteInputReceiver.Delete(input);

            // device.current must be changed after removing devices
            m_defaultInput.MakeCurrent();

            // reassign remote input to controller
            if (m_remoteInputAndCameraController.TryGetValue(input, out var controller))
            {
                RemoteInput newInput = FindPrioritizedInput();
                if (newInput == null)
                {
                    controller.SetInput(m_defaultInput);
                }
                else
                {
                    controller.SetInput(newInput);
                    m_remoteInputAndCameraController.Add(newInput, controller);
                }
            }
            m_remoteInputAndCameraController.Remove(input);

            m_mapChannelAndRemoteInput.Remove(channel);

            foreach (var item in m_mapPeerAndChannelDictionary.Values)
            {
                if (item.ContainsKey(channel.Id))
                {
                    bool result = item.Remove(channel.Id);
                    Debug.Log("网页退出，result:" + result);
                    wss_signaling.WSConnectedState("false");
                    if (RenderSProcessHeartbeat.Instance != null) RenderSProcessHeartbeat.Instance.SetRenderStreamingDoing(false);
                    AutoCloseApp();
                    break;
                }
            }
        }

        private bool AutoCloseInUserQuit;

        public void SetAutoCloseInfo(bool isAutoQuit)
        {
            AutoCloseInUserQuit = isAutoQuit;
        }

        private void AutoCloseApp()
        {
            if (AutoCloseInUserQuit)
            {
                Debug.Log("UserQuit,Application auto quit!");
                Application.Quit();
            }
        }

        RemoteInput FindPrioritizedInput()
        {
            var list = RemoteInputReceiver.All();

            // filter here
            // return null if not found the input
            return list.Except(m_remoteInputAndCameraController.Keys).FirstOrDefault();
        }

        void OnButtonClick(int elementId)
        {
            foreach (var element in arrayButtonClickEvent)
            {
                if (element.elementId == elementId)
                {
                    element.click.Invoke(elementId);
                }
            }
        }
    }
}

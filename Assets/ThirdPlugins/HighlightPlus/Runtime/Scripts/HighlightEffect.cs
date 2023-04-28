/// <summary>
/// Highlight Plus - (c) 2018-2020 Ramiro Oliva (Kronnect)
/// </summary>

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HighlightPlus {

    public delegate bool OnObjectHighlightStartEvent(GameObject obj);
    public delegate void OnObjectHighlightEndEvent(GameObject obj);
    public delegate bool OnRendererHighlightEvent(Renderer renderer);


    public enum SeeThroughMode {
        WhenHighlighted = 0,
        AlwaysWhenOccluded = 1,
        Never = 2
    }

    public enum QualityLevel {
        Fastest = 0,
        High = 1,
        Highest = 2
    }

    public enum TargetOptions {
        Children,
        OnlyThisObject,
        RootToChildren,
        LayerInScene,
        LayerInChildren
    }

    public enum Visibility {
        Normal,
        AlwaysOnTop,
        OnlyWhenOccluded
    }

    [Serializable]
    public struct GlowPassData {
        public float offset;
        public float alpha;
        public Color color;
    }

    [ExecuteInEditMode]
    [HelpURL("https://kronnect.freshdesk.com/support/solutions/42000065090")]
    public partial class HighlightEffect : MonoBehaviour {

        /// <summary>
        /// Gets or sets the current profile. To load a profile and apply its settings at runtime, please use ProfileLoad() method.
        /// </summary>
        public HighlightProfile profile;

        /// <summary>
        /// Sets if changes to the original profile should propagate to this effect.
        /// </summary>
        [Tooltip("If enabled, settings will be synced with profile.")]
        public bool profileSync;

        /// <summary>
        /// Makes the effects visible in the SceneView.
        /// </summary>
        public bool previewInEditor = true;

        /// <summary>
        /// Specifies which objects are affected by this effect.
        /// </summary>
        public TargetOptions effectGroup = TargetOptions.Children;

        /// <summary>
        /// The layer that contains the affected objects by this effect when effectGroup is set to LayerMask.
        /// </summary>
        public LayerMask effectGroupLayer = -1;

        /// <summary>
        /// The alpha threshold for transparent cutout objects. Pixels with alpha below this value will be discarded.
        /// </summary>
        [Range(0, 1)]
        public float alphaCutOff = 0;

        /// <summary>
        /// If back facing triangles are ignored. Backfaces triangles are not visible but you may set this property to false to force highlight effects to act on those triangles as well.
        /// </summary>
        public bool cullBackFaces = true;

        /// <summary>
        /// Show highlight effects even if the object is currently not visible. This option is useful if the affected objects are rendered using GPU instancing tools which render directly to the GPU without creating real game object geometry in CPU.
        /// </summary>
        [Tooltip("Show highlight effects even if the object is not visible. If this object or its children use GPU Instancing tools, the MeshRenderer can be disabled although the object is visible. In this case, this option is useful to enable highlighting.")]
        public bool ignoreObjectVisibility;

        /// <summary>
        /// Enable to support reflection probes
        /// </summary>
        [Tooltip("Support reflection probes. Enable only if you want the effects to be visible in reflections.")]
        public bool reflectionProbes;

        /// <summary>
        /// Enable to smooth normals
        /// </summary>
        [Tooltip("Smooth mesh normals to improve outline or glow effect when using Fast or High quality levels (not used in Highest mode). Original mesh or gameobject is not modified.")]
        public bool smoothNormals = true;

        /// <summary>
        /// Enable to support reflection probes
        /// </summary>
        [Tooltip("Enables GPU instancing. Reduces draw calls in outline and outer glow effects on platforms that support GPU instancing. Should be enabled by default.")]
        public bool GPUInstancing = true;

        /// <summary>
        /// Ignores highlight effects on this object.
        /// </summary>
        [Tooltip("Ignore highlighting on this object.")]
        public bool ignore;

        [SerializeField]
        bool _highlighted;

        public bool highlighted { get { return _highlighted; } set { SetHighlighted(value); } }

        public float fadeInDuration;
        public float fadeOutDuration;

#if UNITY_2019_OR_NEWER
        public bool flipY = true;

#else
        public bool flipY;
#endif

        public bool constantWidth = true;

        [Range(0, 1)]
        public float overlay = 0.5f;
        [ColorUsage(true, true)] public Color overlayColor = Color.yellow;
        public float overlayAnimationSpeed = 1f;
        [Range(0, 1)]
        public float overlayMinIntensity = 0.5f;
        [Range(0, 1)]
        public float overlayBlending = 1.0f;

        [Range(0, 1)]
        public float outline = 1f;
        [ColorUsage(true, true)] public Color outlineColor = Color.black;
        public float outlineWidth = 0.45f;
        public QualityLevel outlineQuality = QualityLevel.High;
        [Range(1, 8)]
        public int outlineDownsampling = 2;
        public Visibility outlineVisibility = Visibility.Normal;
        public bool outlineBlitDebug;
        public bool outlineIndependent;

        [Range(0, 5)]
        public float glow = 1f;
        public float glowWidth = 0.4f;
        public QualityLevel glowQuality = QualityLevel.High;
        [Range(1, 8)]
        public int glowDownsampling = 2;
        [ColorUsage(true, true)] public Color glowHQColor = new Color(0.64f, 1f, 0f, 1f);
        public bool glowDithering = true;
        public float glowMagicNumber1 = 0.75f;
        public float glowMagicNumber2 = 0.5f;
        public float glowAnimationSpeed = 1f;
        public Visibility glowVisibility = Visibility.Normal;
        public bool glowBlitDebug;
        public GlowPassData[] glowPasses;

        [Range(0, 5f)]
        public float innerGlow;
        [Range(0, 2)]
        public float innerGlowWidth = 1f;
        [ColorUsage(true, true)] public Color innerGlowColor = Color.white;
        public Visibility innerGlowVisibility = Visibility.Normal;

        public bool targetFX;
        public Texture2D targetFXTexture;
        [ColorUsage(true, true)] public Color targetFXColor = Color.white;
        public Transform targetFXCenter;
        public float targetFXRotationSpeed = 50f;
        public float targetFXInitialScale = 4f;
        public float targetFXEndScale = 1.5f;
        public float targetFXTransitionDuration = 0.5f;
        public float targetFXStayDuration = 1.5f;

        public event OnObjectHighlightStartEvent OnObjectHighlightStart;
        public event OnObjectHighlightEndEvent OnObjectHighlightEnd;
        public event OnRendererHighlightEvent OnRendererHighlightStart;

        public SeeThroughMode seeThrough;
        [Range(0, 5f)]
        public float seeThroughIntensity = 0.8f;
        [Range(0, 1)]
        public float seeThroughTintAlpha = 0.5f;
        [ColorUsage(true, true)] public Color seeThroughTintColor = Color.red;
        [Range(0, 1)]
        public float seeThroughNoise = 1f;


        struct ModelMaterials {
            public bool render; // if this object can render this frame
            public Transform transform;
            public bool renderWasVisibleDuringSetup;
            public Mesh mesh, originalMesh;
            public Renderer renderer;
            public bool skinnedMesh;
            public Material[] fxMatMask, fxMatSolidColor, fxMatSeeThrough, fxMatOverlay, fxMatInnerGlow;
        }

        enum FadingState {
            FadingOut = -1,
            NoFading = 0,
            FadingIn = 1
        }

        [SerializeField, HideInInspector]
        ModelMaterials[] rms;
        [SerializeField, HideInInspector]
        int rmsCount;

#if UNITY_EDITOR
        /// <summary>
        /// True if there's some static children
        /// </summary>
        [NonSerialized]
        public bool staticChildren;
#endif

        [NonSerialized]
        public Transform target;

        // Time in which the highlight started
        [NonSerialized]
        public float highlightStartTime;

        const string SKW_ALPHACLIP = "HP_ALPHACLIP";
        const string SKW_DEPTHCLIP = "HP_DEPTHCLIP";
        const string UNIFORM_CUTOFF = "_CutOff";
        const float TAU = 0.70711f;

        // Reference materials. These are instanced per object (rms).
        static Material fxMatMask, fxMatSolidColor, fxMatSeeThrough, fxMatOverlay, fxMatClearStencil;

        // Per-object materials
        Material fxMatGlowTemplate, fxMatInnerGlow, fxMatOutlineTemplate, fxMatTarget;
        Material fxMatComposeGlow, fxMatComposeOutline, fxMatBlurGlow, fxMatBlurOutline;
        Material[] fxMatOutline, fxMatGlow;

        static Vector3[] offsets;

        float fadeStartTime;
        FadingState fading = FadingState.NoFading;
        CommandBuffer cbHighlight, cbSeeThrough, cbOverlay, cbInnerGlow;
        int[] mipGlowBuffers, mipOutlineBuffers;
        int glowRT, outlineRT;
        static Mesh quadMesh;
        int sourceRT;
        Matrix4x4 quadGlowMatrix, quadOutlineMatrix;
        Vector3[] corners;
        RenderTextureDescriptor sourceDesc;
        Color debugColor, blackColor;
        Visibility lastOutlineVisibility;
        bool requireUpdateMaterial;

        [NonSerialized]
        public static List<HighlightEffect> instances = new List<HighlightEffect>();

        //bool usingPipeline;
        bool useSmoothGlow, useSmoothOutline, useSmoothBlend;
        bool useGPUInstancing;

        MaterialPropertyBlock glowPropertyBlock, outlinePropertyBlock;
        static List<Vector4> matDataDirection = new List<Vector4>();
        static List<Vector4> matDataGlow = new List<Vector4>();
        static List<Vector4> matDataColor = new List<Vector4>();
        static Matrix4x4[] matrices;

        void OnEnable() {
            lastOutlineVisibility = outlineVisibility;
            debugColor = new Color(1f, 0f, 0f, 0.5f);
            blackColor = new Color(0, 0, 0, 0);
            if (offsets == null || offsets.Length != 8) {
                offsets = new Vector3[] {
                    Vector3.up,
                    Vector3.right,
                    Vector3.down,
                    Vector3.left,
                    new Vector3 (-TAU, TAU, 0),
                    new Vector3 (TAU, TAU, 0),
                    new Vector3 (TAU, -TAU, 0),
                    new Vector3 (-TAU, -TAU, 0)
                };
            }
            if (corners == null || corners.Length != 8) {
                corners = new Vector3[8];
            }
            if (quadMesh == null) {
                BuildQuad();
            }
            if (target == null) {
                target = transform;
            }
            if (profileSync && profile != null) {
                profile.Load(this);
            }
            if (glowPasses == null || glowPasses.Length == 0) {
                glowPasses = new GlowPassData[4];
                glowPasses[0] = new GlowPassData() { offset = 4, alpha = 0.1f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[1] = new GlowPassData() { offset = 3, alpha = 0.2f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[2] = new GlowPassData() { offset = 2, alpha = 0.3f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[3] = new GlowPassData() { offset = 1, alpha = 0.4f, color = new Color(0.64f, 1f, 0f, 1f) };
            }
            sourceRT = Shader.PropertyToID("_HPSourceRT");
            useGPUInstancing = GPUInstancing && SystemInfo.supportsInstancing;
            if (useGPUInstancing) {
                if (glowPropertyBlock == null) {
                    glowPropertyBlock = new MaterialPropertyBlock();
                }
                if (outlinePropertyBlock == null) {
                    outlinePropertyBlock = new MaterialPropertyBlock();
                }
            }

            CheckGeometrySupportDependencies();
            SetupMaterial();

            instances.Add(this);
        }

        void OnDisable() {
            UpdateMaterialProperties();
            if (instances != null) {
                int k = instances.IndexOf(this);
                if (k >= 0) {
                    instances.RemoveAt(k);
                }
            }
        }

        void Reset() {
            SetupMaterial();
        }


        /// <summary>
        /// Loads a profile into this effect
        /// </summary>
        public void ProfileLoad(HighlightProfile profile) {
            if (profile != null) {
                profile.Load(this);
            }
        }

        /// <summary>
        /// Reloads currently assigned profile
        /// </summary>
        public void ProfileReload() {
            if (profile != null) {
                profile.Load(this);
            }
        }


        /// <summary>
        /// Save current settings into given profile
        /// </summary>
        public void ProfileSaveChanges(HighlightProfile profile) {
            if (profile != null) {
                profile.Save(this);
            }
        }

        /// <summary>
        /// Save current settings into current profile
        /// </summary>
        public void ProfileSaveChanges() {
            if (profile != null) {
                profile.Save(this);
            }
        }


        public void Refresh() {
            if (enabled) {
                SetupMaterial();
            }
        }

        RenderTargetIdentifier colorAttachmentBuffer, depthAttachmentBuffer;

        public CommandBuffer GetCommandBuffer(Camera cam, RenderTargetIdentifier colorAttachmentBuffer, RenderTargetIdentifier depthAttachmentBuffer) {
            //usingPipeline = true;
            this.colorAttachmentBuffer = colorAttachmentBuffer;
            this.depthAttachmentBuffer = depthAttachmentBuffer;
            BuildCommandBuffer(cam);
            return cbHighlight;
        }

        void BuildCommandBuffer(Camera cam) {

            if (colorAttachmentBuffer == 0) {
                colorAttachmentBuffer = BuiltinRenderTextureType.CameraTarget;
            }
            if (depthAttachmentBuffer == 0) {
                depthAttachmentBuffer = BuiltinRenderTextureType.CameraTarget;
            }

            if (cam == null || cbHighlight == null) return;

            cbHighlight.Clear();

#if UNITY_EDITOR
            if (!previewInEditor && !Application.isPlaying) {
                return;
            }
#endif
            if (requireUpdateMaterial) {
                requireUpdateMaterial = false;
                UpdateMaterialProperties();
            }

            bool seeThroughReal = seeThroughIntensity > 0 && (this.seeThrough == SeeThroughMode.AlwaysWhenOccluded || (this.seeThrough == SeeThroughMode.WhenHighlighted && _highlighted));
            if (seeThroughReal) {
                RenderOccluders(cbHighlight, cam);
            }
            if (cancelSeeThroughThisFrame) {
                cancelSeeThroughThisFrame = false;
                seeThroughReal = false;
            }
            if (!_highlighted && !seeThroughReal && !hitActive) {
                return;
            }

            // Check camera culling mask
            int cullingMask = cam.cullingMask;

            // Ensure renderers are valid and visible (in case LODgroup has changed active renderer)
            if (!ignoreObjectVisibility) {
                for (int k = 0; k < rmsCount; k++) {
                    if (rms[k].renderer != null && rms[k].renderer.isVisible != rms[k].renderWasVisibleDuringSetup) {
                        SetupMaterial();
                        break;
                    }
                }
            }

            // Apply effect
            float glowReal = _highlighted ? this.glow : 0;
            int layer = gameObject.layer;

            if (fxMatMask == null)
                return;

            // Check smooth blend ztesting capability
            Visibility smoothGlowVisibility = glowVisibility;
            Visibility smoothOutlineVisibility = outlineVisibility;

            float aspect = cam.aspect;
            bool somePartVisible = false;

            // First create masks
            bool independentFullScreenNotExecuted = true;
            for (int k = 0; k < rmsCount; k++) {
                rms[k].render = false;
                Transform t = rms[k].transform;
                if (t == null)
                    continue;
                Mesh mesh = rms[k].mesh;
                if (mesh == null)
                    continue;
                if (((1 << t.gameObject.layer) & cullingMask) == 0)
                    continue;
                if (!rms[k].renderer.isVisible)
                    continue;
                if (!reflectionProbes && cam.cameraType == CameraType.Reflection)
                    continue;
                rms[k].render = true;
                somePartVisible = true;

                if (outlineIndependent) {
                    if (useSmoothBlend && independentFullScreenNotExecuted) {
                        independentFullScreenNotExecuted = false;
                        cbHighlight.DrawMesh(quadMesh, Matrix4x4.identity, fxMatClearStencil);
                    } else if (outline > 0) {
                        for (int l = 0; l < mesh.subMeshCount; l++) {
                            if (outlineQuality == QualityLevel.High) {
                                for (int o = 0; o < offsets.Length; o++) {
                                    Vector3 direction = offsets[o] * (outlineWidth / 100f);
                                    direction.y *= aspect;
                                    fxMatOutline[o].SetVector("_OutlineDirection", direction);
                                    cbHighlight.DrawRenderer(rms[k].renderer, fxMatOutline[o], l, 1);
                                }
                            } else {
                                fxMatOutline[0].SetVector("_OutlineDirection", Vector3.zero);
                                cbHighlight.DrawRenderer(rms[k].renderer, fxMatOutline[0], l, 1);
                            }
                        }
                    }
                }


                for (int l = 0; l < mesh.subMeshCount; l++) {
                    if (_highlighted && ((outline > 0 && smoothOutlineVisibility != Visibility.Normal) || (glow > 0 && smoothGlowVisibility != Visibility.Normal) || (innerGlow > 0 && innerGlowVisibility != Visibility.Normal))) {
                        rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                    } else {
                        rms[k].fxMatMask[l].SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                    }
                    cbHighlight.DrawRenderer(rms[k].renderer, rms[k].fxMatMask[l], l);
                }
            }

            // Compute tweening
            float fade = 1f;
            if (fading != FadingState.NoFading) {
                if (fading == FadingState.FadingIn) {
                    if (fadeInDuration > 0) {
                        fade = (Time.time - fadeStartTime) / fadeInDuration;
                        if (fade > 1f) {
                            fade = 1f;
                            fading = FadingState.NoFading;
                        }
                    }
                } else if (fadeOutDuration > 0) {
                    fade = 1f - (Time.time - fadeStartTime) / fadeOutDuration;
                    if (fade < 0f) {
                        fade = 0f;
                        fading = FadingState.NoFading;
                        _highlighted = false;
                        if (OnObjectHighlightEnd != null) {
                            OnObjectHighlightEnd(gameObject);
                        }
                        SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }

            if (glowQuality == QualityLevel.High) {
                glowReal *= 0.25f;
            }

            // Add mesh effects
            for (int k = 0; k < rmsCount; k++) {
                if (!rms[k].render)
                    continue;
                Mesh mesh = rms[k].mesh;

                // See-Through
                if (seeThroughReal) {
                    for (int l = 0; l < mesh.subMeshCount; l++) {
                        if (l < rms[k].fxMatSeeThrough.Length && rms[k].fxMatSeeThrough[l] != null) {
                            cbHighlight.DrawRenderer(rms[k].renderer, rms[k].fxMatSeeThrough[l], l);
                        }
                    }
                }

                if (_highlighted || hitActive) {
                    // Overlay
                    Color overlayColor = this.overlayColor;
                    float overlayMinIntensity = this.overlayMinIntensity;
                    float overlayBlending = this.overlayBlending;
                    if (hitActive) {
                        float t = hitFadeOutDuration > 0 ? (Time.time - hitStartTime) / hitFadeOutDuration : 1f;
                        if (t >= 1f) {
                            hitActive = false;
                            overlayColor.a = _highlighted ? overlay : 0;
                        } else {
                            bool lerpToCurrentOverlay = _highlighted && overlay > 0;
                            overlayColor = lerpToCurrentOverlay ? Color.Lerp(hitColor, overlayColor, t) : hitColor;
                            overlayColor.a = lerpToCurrentOverlay ? Mathf.Lerp(1f - t, overlay, t) : 1f - t;
                            overlayColor.a *= hitInitialIntensity;
                            overlayMinIntensity = 1f;
                            overlayBlending = 0;
                        }
                    } else {
                        overlayColor.a = overlay * fade;
                    }

                    if (overlay > 0) {
                        for (int l = 0; l < mesh.subMeshCount; l++) {
                            if (l < rms[k].fxMatOverlay.Length && rms[k].fxMatOverlay[l] != null) {
                                rms[k].fxMatOverlay[l].color = overlayColor;
                                rms[k].fxMatOverlay[l].SetVector("_OverlayData", new Vector3(overlayAnimationSpeed, overlayMinIntensity, overlayBlending));
                                cbHighlight.DrawRenderer(rms[k].renderer, rms[k].fxMatOverlay[l], l);
                            }
                        }
                    }
                }

                if (!_highlighted)
                    continue;

                bool allowGPUInstancing = useGPUInstancing && !rms[k].skinnedMesh;

                for (int l = 0; l < mesh.subMeshCount; l++) {
                    // Glow
                    if (glow > 0 && glowQuality != QualityLevel.Highest) {
                        matDataGlow.Clear();
                        matDataColor.Clear();
                        matDataDirection.Clear();
                        for (int glowPass = 0; glowPass < glowPasses.Length; glowPass++) {
                            if (glowQuality == QualityLevel.High) {
                                for (int o = 0; o < offsets.Length; o++) {
                                    Vector3 direction = offsets[o];
                                    direction.y *= aspect;
                                    Color dataColor = glowPasses[glowPass].color;
                                    Vector4 dataGlow = new Vector4(fade * glowReal * glowPasses[glowPass].alpha, glowPasses[glowPass].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2);
                                    if (allowGPUInstancing) {
                                        matDataDirection.Add(direction);
                                        matDataGlow.Add(dataGlow);
                                        matDataColor.Add(new Color(dataColor.r, dataColor.g, dataColor.b, dataColor.a));
                                    } else {
                                        int matIndex = glowPass * 8 + o;
                                        fxMatGlow[matIndex].SetVector("_GlowDirection", direction);
                                        fxMatGlow[matIndex].SetColor("_GlowColor", dataColor);
                                        fxMatGlow[matIndex].SetVector("_Glow", dataGlow);
                                        cbHighlight.DrawRenderer(rms[k].renderer, fxMatGlow[matIndex], l);
                                    }
                                }
                            } else {
                                Vector4 dataGlow = new Vector4(fade * glowReal * glowPasses[glowPass].alpha, glowPasses[glowPass].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2);
                                Color dataColor = glowPasses[glowPass].color;
                                if (allowGPUInstancing) {
                                    matDataDirection.Add(Vector4.zero);
                                    matDataGlow.Add(dataGlow);
                                    matDataColor.Add(new Color(dataColor.r, dataColor.g, dataColor.b, dataColor.a));
                                } else {
                                    int matIndex = glowPass * 8;
                                    fxMatGlow[matIndex].SetColor("_GlowColor", dataColor);
                                    fxMatGlow[matIndex].SetVector("_Glow", dataGlow);
                                    fxMatGlow[matIndex].SetVector("_GlowDirection", Vector3.zero);
                                    cbHighlight.DrawRenderer(rms[k].renderer, fxMatGlow[matIndex], l);
                                }
                            }
                        }
                        if (allowGPUInstancing) {
                            int instanceCount = matDataDirection.Count;
                            if (instanceCount > 0) {
                                glowPropertyBlock.SetVectorArray("_GlowDirection", matDataDirection);
                                glowPropertyBlock.SetVectorArray("_GlowColor", matDataColor);
                                glowPropertyBlock.SetVectorArray("_Glow", matDataGlow);
                                if (matrices == null || matrices.Length < instanceCount) {
                                    matrices = new Matrix4x4[instanceCount];
                                }
                                Matrix4x4 objectToWorld = rms[k].transform.localToWorldMatrix;
                                for (int m = 0; m < instanceCount; m++) {
                                    matrices[m] = objectToWorld;
                                }
                                cbHighlight.DrawMeshInstanced(mesh, l, fxMatGlow[0], 0, matrices, instanceCount, glowPropertyBlock);
                            }
                        }
                    }

                    // Outline
                    if (outline > 0 && outlineQuality != QualityLevel.Highest) {
                        Color outlineColor = this.outlineColor;
                        outlineColor.a = outline * fade;
                        if (outlineQuality == QualityLevel.High) {
                            matDataDirection.Clear();
                            for (int o = 0; o < offsets.Length; o++) {
                                fxMatOutline[o].SetColor("_OutlineColor", outlineColor);
                                Vector3 direction = offsets[o] * (outlineWidth / 100f);
                                direction.y *= aspect;
                                if (allowGPUInstancing) {
                                    matDataDirection.Add(direction);
                                } else {
                                    fxMatOutline[o].SetVector("_OutlineDirection", direction);
                                    cbHighlight.DrawRenderer(rms[k].renderer, fxMatOutline[o], l, 0);
                                }
                            }
                            if (allowGPUInstancing) {
                                int instanceCount = matDataDirection.Count;
                                if (instanceCount > 0) {
                                    outlinePropertyBlock.SetVectorArray("_OutlineDirection", matDataDirection);
                                    if (matrices == null || matrices.Length < instanceCount) {
                                        matrices = new Matrix4x4[instanceCount];
                                    }
                                    Matrix4x4 objectToWorld = rms[k].transform.localToWorldMatrix;
                                    for (int m = 0; m < instanceCount; m++) {
                                        matrices[m] = objectToWorld;
                                    }
                                    cbHighlight.DrawMeshInstanced(mesh, l, fxMatOutline[0], 0, matrices, instanceCount, outlinePropertyBlock);
                                }
                            }
                        } else {
                            fxMatOutline[0].SetColor("_OutlineColor", outlineColor);
                            fxMatOutline[0].SetVector("_OutlineDirection", Vector3.zero);
                            cbHighlight.DrawRenderer(rms[k].renderer, fxMatOutline[0], l, 0);
                        }
                    }

                    // Inner Glow
                    if (innerGlow > 0 && innerGlowWidth > 0) {
                        if (l < rms[k].fxMatInnerGlow.Length && rms[k].fxMatInnerGlow[l] != null) {
                            Color innerGlowColorA = innerGlowColor;
                            innerGlowColorA.a = innerGlow * fade;
                            rms[k].fxMatInnerGlow[l].SetColor("_Color", innerGlowColorA);
                            cbHighlight.DrawRenderer(rms[k].renderer, rms[k].fxMatInnerGlow[l], l);
                        }
                    }
                }

                // Target
                if (targetFX) {
                    float fadeOut = 1f;
                    if (Application.isPlaying) {
                        fadeOut = (Time.time - highlightStartTime);
                        if (fadeOut >= targetFXStayDuration) {
                            fadeOut -= targetFXStayDuration;
                            fadeOut = 1f - fadeOut;
                        }
                        if (fadeOut > 1f) {
                            fadeOut = 1f;
                        }
                    }
                    if (fadeOut > 0) {
                        float scaleT = 1f;
                        float time;
                        if (Application.isPlaying) {
                            scaleT = (Time.time - highlightStartTime) / targetFXTransitionDuration;
                            if (scaleT > 1f) {
                                scaleT = 1f;
                            }
                            scaleT = Mathf.Sin(scaleT * Mathf.PI * 0.5f);
                            time = Time.time;
                        } else {
                            time = (float)DateTime.Now.Subtract(DateTime.Today).TotalSeconds;
                        }
                        Bounds bounds = rms[k].renderer.bounds;
                        Vector3 size = bounds.size;
                        float minSize = size.x;
                        if (size.y < minSize) {
                            minSize = size.y;
                        }
                        if (size.z < minSize) {
                            minSize = size.z;
                        }
                        size.x = size.y = size.z = minSize;
                        size = Vector3.Lerp(size * targetFXInitialScale, size * targetFXEndScale, scaleT);
                        Quaternion camRot = Quaternion.LookRotation(cam.transform.position - rms[k].transform.position);
                        Quaternion rotation = Quaternion.Euler(0, 0, time * targetFXRotationSpeed);
                        camRot *= rotation;
                        Vector3 center = targetFXCenter != null ? targetFXCenter.transform.position : bounds.center;
                        Matrix4x4 m = Matrix4x4.TRS(center, camRot, size);
                        Color color = targetFXColor;
                        color.a *= fade * fadeOut;
                        fxMatTarget.color = color;
                        cbHighlight.DrawMesh(quadMesh, m, fxMatTarget, 0);
                    }
                }
            }

            if (useSmoothBlend && _highlighted && somePartVisible) {

                int smoothRTWidth = 0;
                int smoothRTHeight = 0;
                Bounds smoothBounds = new Bounds();

                // Prepare smooth outer glow / outline target
                smoothRTWidth = cam.pixelWidth;
                smoothRTHeight = cam.pixelHeight;
                if (smoothRTHeight <= 0) {
                    smoothRTHeight = 1;
                }
                if (UnityEngine.XR.XRSettings.enabled && Application.isPlaying) {
                    sourceDesc = UnityEngine.XR.XRSettings.eyeTextureDesc;
                } else {
                    sourceDesc = new RenderTextureDescriptor(smoothRTWidth, smoothRTHeight, Application.isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);
                    sourceDesc.volumeDepth = 1;
                }
                sourceDesc.msaaSamples = 1;
                sourceDesc.useMipMap = false;
                sourceDesc.depthBufferBits = 0;

                cbHighlight.GetTemporaryRT(sourceRT, sourceDesc, FilterMode.Bilinear);
                cbHighlight.SetRenderTarget(sourceRT);
                cbHighlight.ClearRenderTarget(false, true, new Color(0, 0, 0, 0));

                for (int k = 0; k < rmsCount; k++) {
                    if (!rms[k].render)
                        continue;
                    if (k == 0) {
                        smoothBounds = rms[k].renderer.bounds;
                    } else {
                        smoothBounds.Encapsulate(rms[k].renderer.bounds);
                    }
                    Mesh mesh = rms[k].mesh;

                    // Render object body for glow/outline highest quality
                    for (int l = 0; l < mesh.subMeshCount; l++) {
                        if (l < rms[k].fxMatSolidColor.Length) {
                            cbHighlight.DrawRenderer(rms[k].renderer, rms[k].fxMatSolidColor[l], l);
                        }
                    }
                }

                if (ComputeSmoothQuadMatrix(cam, smoothBounds)) {
                    // Smooth Glow
                    if (useSmoothGlow) {
                        float intensity = glow * fade;
                        fxMatComposeGlow.color = new Color(glowHQColor.r * intensity, glowHQColor.g * intensity, glowHQColor.b * intensity, glowHQColor.a * intensity);
                        SmoothGlow(smoothRTWidth / glowDownsampling, smoothRTHeight / glowDownsampling);
                    }

                    // Smooth Outline
                    if (useSmoothOutline) {
                        float intensity = outline * fade;
                        fxMatComposeOutline.color = new Color(outlineColor.r * intensity, outlineColor.g * intensity, outlineColor.b * intensity, outlineColor.a * intensity * 10f);
                        SmoothOutline(smoothRTWidth / outlineDownsampling, smoothRTHeight / outlineDownsampling);
                    }

                    // Bit result
                    ComposeSmoothBlend(smoothGlowVisibility, smoothOutlineVisibility);
                }
            }
        }

        bool ComputeSmoothQuadMatrix(Camera cam, Bounds bounds) {
            // Compute bounds in screen space and enlarge for glow space
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            corners[0] = new Vector3(min.x, min.y, min.z);
            corners[1] = new Vector3(min.x, min.y, max.z);
            corners[2] = new Vector3(max.x, min.y, min.z);
            corners[3] = new Vector3(max.x, min.y, max.z);
            corners[4] = new Vector3(min.x, max.y, min.z);
            corners[5] = new Vector3(min.x, max.y, max.z);
            corners[6] = new Vector3(max.x, max.y, min.z);
            corners[7] = new Vector3(max.x, max.y, max.z);
            Vector3 scrMin = new Vector3(float.MaxValue, float.MaxValue, 0);
            Vector3 scrMax = new Vector3(float.MinValue, float.MinValue, 0);
            float distance = float.MaxValue;
            for (int k = 0; k < corners.Length; k++) {
                corners[k] = cam.WorldToScreenPoint(corners[k]);
                if (corners[k].x < scrMin.x) {
                    scrMin.x = corners[k].x;
                }
                if (corners[k].y < scrMin.y) {
                    scrMin.y = corners[k].y;
                }
                if (corners[k].x > scrMax.x) {
                    scrMax.x = corners[k].x;
                }
                if (corners[k].y > scrMax.y) {
                    scrMax.y = corners[k].y;
                }
                if (corners[k].z < distance) {
                    distance = corners[k].z;
                    if (distance < cam.nearClipPlane) {
                        scrMin.x = scrMin.y = 0;
                        scrMax.x = cam.pixelWidth;
                        scrMax.y = cam.pixelHeight;
                        break;
                    }
                }
            }
            if (scrMax.y == scrMin.y)
                return false;

            if (distance < cam.nearClipPlane) {
                distance = cam.nearClipPlane + 0.01f;
            }
            scrMin.z = scrMax.z = distance;
            if (outline > 0) {
                BuildMatrix(cam, scrMin, scrMax, (int)(10 + 20 * outlineWidth + 5 * outlineDownsampling), ref quadOutlineMatrix);
            }
            if (glow > 0) {
                BuildMatrix(cam, scrMin, scrMax, (int)(20 + 30 * glowWidth + 10 * glowDownsampling), ref quadGlowMatrix);
            }
            return true;
        }

        void BuildMatrix(Camera cam, Vector3 scrMin, Vector3 scrMax, int border, ref Matrix4x4 quadMatrix) {

            // Insert padding to make room for effects
            scrMin.x -= border;
            scrMin.y -= border;
            scrMax.x += border;
            scrMax.y += border;

            // Back to world space
            Vector3 third = new Vector3(scrMax.x, scrMin.y, scrMin.z);
            scrMin = cam.ScreenToWorldPoint(scrMin);
            scrMax = cam.ScreenToWorldPoint(scrMax);
            third = cam.ScreenToWorldPoint(third);

            float width = Vector3.Distance(scrMin, third);
            float height = Vector3.Distance(scrMax, third);

            quadMatrix = Matrix4x4.TRS((scrMin + scrMax) * 0.5f, cam.transform.rotation, new Vector3(width, height, 1f));
        }

        void SmoothGlow(int rtWidth, int rtHeight) {
            const int blurPasses = 4;

            // Blur buffers
            int bufferCount = blurPasses * 2;
            if (mipGlowBuffers == null || mipGlowBuffers.Length != bufferCount) {
                mipGlowBuffers = new int[bufferCount];
                for (int k = 0; k < bufferCount; k++) {
                    mipGlowBuffers[k] = Shader.PropertyToID("_HPSmoothGlowTemp" + k);
                }
                glowRT = Shader.PropertyToID("_HPComposeGlowFinal");
                mipGlowBuffers[bufferCount - 2] = glowRT;
            }
            RenderTextureDescriptor glowDesc = sourceDesc;
            glowDesc.depthBufferBits = 0;
            if (glowDesc.vrUsage == VRTextureUsage.TwoEyes) {
                glowDesc.vrUsage = VRTextureUsage.None;
                fxMatBlurGlow.SetFloat("_StereoRendering", 0.5f);
                fxMatComposeGlow.SetFloat("_StereoRendering", 0.5f);
            } else {
                fxMatBlurGlow.SetFloat("_StereoRendering", 1f);
                fxMatComposeGlow.SetFloat("_StereoRendering", 1f);
            }

            for (int k = 0; k < bufferCount; k++) {
                float reduction = k / 2 + 2;
                int reducedWidth = (int)(rtWidth / reduction);
                int reducedHeight = (int)(rtHeight / reduction);
                if (reducedWidth <= 0) {
                    reducedWidth = 1;
                }
                if (reducedHeight <= 0) {
                    reducedHeight = 1;
                }
                glowDesc.width = reducedWidth;
                glowDesc.height = reducedHeight;
                cbHighlight.GetTemporaryRT(mipGlowBuffers[k], glowDesc, FilterMode.Bilinear);
            }

            for (int k = 0; k < bufferCount - 1; k += 2) {
                if (k == 0) {
                    cbHighlight.Blit(sourceRT, mipGlowBuffers[k + 1], fxMatBlurGlow, 0);
                } else {
                    cbHighlight.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 1], fxMatBlurGlow, 0);
                }
                cbHighlight.Blit(mipGlowBuffers[k + 1], mipGlowBuffers[k], fxMatBlurGlow, 1);

                if (k < bufferCount - 2) {
                    cbHighlight.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 2], fxMatBlurGlow, 2);
                }
            }
        }

        void SmoothOutline(int rtWidth, int rtHeight) {
            const int blurPasses = 2;

            // Blur buffers
            int bufferCount = blurPasses * 2;
            if (mipOutlineBuffers == null || mipOutlineBuffers.Length != bufferCount) {
                mipOutlineBuffers = new int[bufferCount];
                for (int k = 0; k < bufferCount; k++) {
                    mipOutlineBuffers[k] = Shader.PropertyToID("_HPSmoothOutlineTemp" + k);
                }
                outlineRT = Shader.PropertyToID("_HPComposeOutlineFinal");
                mipOutlineBuffers[bufferCount - 2] = outlineRT;
            }
            RenderTextureDescriptor outlineDesc = sourceDesc;
            outlineDesc.depthBufferBits = 0;
            if (outlineDesc.vrUsage == VRTextureUsage.TwoEyes) {
                outlineDesc.vrUsage = VRTextureUsage.None;
                fxMatBlurOutline.SetFloat("_StereoRendering", 0.5f);
                fxMatComposeOutline.SetFloat("_StereoRendering", 0.5f);
            } else {
                fxMatBlurOutline.SetFloat("_StereoRendering", 1f);
                fxMatComposeOutline.SetFloat("_StereoRendering", 1f);
            }

            for (int k = 0; k < bufferCount; k++) {
                float reduction = k / 2 + 2;
                int reducedWidth = (int)(rtWidth / reduction);
                int reducedHeight = (int)(rtHeight / reduction);
                if (reducedWidth <= 0) {
                    reducedWidth = 1;
                }
                if (reducedHeight <= 0) {
                    reducedHeight = 1;
                }
                outlineDesc.width = reducedWidth;
                outlineDesc.height = reducedHeight;
                cbHighlight.GetTemporaryRT(mipOutlineBuffers[k], outlineDesc, FilterMode.Bilinear);
            }

            for (int k = 0; k < bufferCount - 1; k += 2) {
                if (k == 0) {
                    cbHighlight.Blit(sourceRT, mipOutlineBuffers[k + 1], fxMatBlurOutline, 0);
                } else {
                    cbHighlight.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 1], fxMatBlurOutline, 0);
                }
                cbHighlight.Blit(mipOutlineBuffers[k + 1], mipOutlineBuffers[k], fxMatBlurOutline, 1);

                if (k < bufferCount - 2) {
                    cbHighlight.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 2], fxMatBlurOutline, 2);
                }
            }
        }


        void ComposeSmoothBlend(Visibility smoothGlowVisibility, Visibility smoothOutlineVisibility) {

            // Render mask on target surface
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.IPhonePlayer) {
                cbHighlight.SetRenderTarget(colorAttachmentBuffer, depthAttachmentBuffer);
            } else {
                cbHighlight.SetRenderTarget(colorAttachmentBuffer);
            }

            bool renderSmoothGlow = glow > 0 && glowQuality == QualityLevel.Highest;
            if (renderSmoothGlow) {
                fxMatComposeGlow.SetVector("_Flip", (UnityEngine.XR.XRSettings.enabled && flipY) ? new Vector3(1, -1, 0) : new Vector3(0, 1, 0));
                fxMatComposeGlow.SetInt("_ZTest", GetZTestValue(smoothGlowVisibility));
                fxMatComposeGlow.SetColor("_Debug", glowBlitDebug ? debugColor : blackColor);
                cbHighlight.DrawMesh(quadMesh, quadGlowMatrix, fxMatComposeGlow, 0, 0);
            }
            bool renderSmoothOutline = outline > 0 && outlineQuality == QualityLevel.Highest;
            if (renderSmoothOutline) {
                fxMatComposeOutline.SetVector("_Flip", (UnityEngine.XR.XRSettings.enabled && flipY) ? new Vector3(1, -1, 0) : new Vector3(0, 1, 0));
                fxMatComposeOutline.SetInt("_ZTest", GetZTestValue(smoothOutlineVisibility));
                fxMatComposeOutline.SetColor("_Debug", outlineBlitDebug ? debugColor : blackColor);
                cbHighlight.DrawMesh(quadMesh, quadOutlineMatrix, fxMatComposeOutline, 0, 0);
            }
            // Release render textures
            if (renderSmoothGlow) {
                for (int k = 0; k < mipGlowBuffers.Length; k++) {
                    cbHighlight.ReleaseTemporaryRT(mipGlowBuffers[k]);
                }
            }
            if (renderSmoothOutline) {
                for (int k = 0; k < mipOutlineBuffers.Length; k++) {
                    cbHighlight.ReleaseTemporaryRT(mipOutlineBuffers[k]);
                }
            }

            cbHighlight.ReleaseTemporaryRT(sourceRT);
        }

        void InitMaterial(ref Material material, string shaderName) {
            if (material == null) {
                Shader shaderFX = Shader.Find(shaderName);
                if (shaderFX == null) {
                    Debug.LogError("Shader " + shaderName + " not found.");
                    enabled = false;
                    return;
                }
                material = new Material(shaderFX);
            }
        }

        void ForkMaterial(Material material, ref Material[] array, int count) {
            if (array == null || array.Length != count) {
                array = new Material[count];
            }
            for (int k = 0; k < count; k++) {
                if (array[k] == null) {
                    array[k] = Instantiate<Material>(material);
                }
            }
        }

        /// <summary>
        /// Sets target for highlight effects
        /// </summary>
        public void SetTarget(Transform transform) {
            if (transform == target || transform == null)
                return;

            if (_highlighted) {
                SetHighlighted(false);
            }

            target = transform;
            SetupMaterial();
        }



        /// <summary>
        /// Sets target for highlight effects and also specify a list of renderers to be included as well
        /// </summary>
        public void SetTargets(Transform transform, Renderer[] renderers) {
            if (transform == target || transform == null)
                return;

            if (_highlighted) {
                SetHighlighted(false);
            }

            target = transform;
            SetupMaterial(renderers);
        }


        /// <summary>
        /// Start or finish highlight on the object
        /// </summary>
        public void SetHighlighted(bool state) {

            if (!Application.isPlaying) {
                _highlighted = state;
                return;
            }

            if (fading == FadingState.NoFading) {
                fadeStartTime = Time.time;
            }

            if (state && !ignore) {
                if (_highlighted && fading == FadingState.NoFading) {
                    return;
                }
                if (OnObjectHighlightStart != null) {
                    if (!OnObjectHighlightStart(gameObject)) {
                        return;
                    }
                }
                SendMessage("HighlightStart", null, SendMessageOptions.DontRequireReceiver);
                highlightStartTime = Time.time;
                if (fadeInDuration > 0) {
                    if (fading == FadingState.FadingOut) {
                        float remaining = fadeOutDuration - (Time.time - fadeStartTime);
                        fadeStartTime = Time.time - remaining;
                    }
                    fading = FadingState.FadingIn;
                } else {
                    fading = FadingState.NoFading;
                }
                _highlighted = true;
                requireUpdateMaterial = true;
            } else if (_highlighted) {
                if (fadeOutDuration > 0) {
                    if (fading == FadingState.FadingIn) {
                        float elapsed = Time.time - fadeStartTime;
                        fadeStartTime = Time.time + elapsed - fadeInDuration;
                    }
                    fading = FadingState.FadingOut; // when fade out ends, highlighted will be set to false in OnRenderObject
                } else {
                    fading = FadingState.NoFading;
                    _highlighted = false;
                    if (OnObjectHighlightEnd != null) {
                        OnObjectHighlightEnd(gameObject);
                    }
                    SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
                    requireUpdateMaterial = true;
                }
            }
        }

        void SetupMaterial() {

#if UNITY_EDITOR
            staticChildren = false;
#endif

            if (target == null || fxMatMask == null)
                return;

            ForkMaterial(fxMatOutlineTemplate, ref fxMatOutline, 8);
            ForkMaterial(fxMatGlowTemplate, ref fxMatGlow, 8 * glowPasses.Length);

            Renderer[] rr = null;
            switch (effectGroup) {
                case TargetOptions.OnlyThisObject:
                    Renderer renderer = target.GetComponent<Renderer>();
                    if (renderer != null) {
                        rr = new Renderer[1];
                        rr[0] = renderer;
                    }
                    break;
                case TargetOptions.RootToChildren:
                    Transform root = target;
                    while (root.parent != null) {
                        root = root.parent;
                    }
                    rr = root.GetComponentsInChildren<Renderer>();
                    break;
                case TargetOptions.LayerInScene: {
                        HighlightEffect eg = this;
                        if (target != transform) {
                            HighlightEffect targetEffect = target.GetComponent<HighlightEffect>();
                            if (targetEffect != null) {
                                eg = targetEffect;
                            }
                        }
                        rr = FindRenderersWithLayerInScene(eg.effectGroupLayer);
                    }
                    break;
                case TargetOptions.LayerInChildren: {
                        HighlightEffect eg = this;
                        if (target != transform) {
                            HighlightEffect targetEffect = target.GetComponent<HighlightEffect>();
                            if (targetEffect != null) {
                                eg = targetEffect;
                            }
                        }
                        rr = FindRenderersWithLayerInChildren(eg.effectGroupLayer);
                    }
                    break;
                default:
                    rr = target.GetComponentsInChildren<Renderer>();
                    break;
            }

            SetupMaterial(rr);
        }

        void SetupMaterial(Renderer[] rr) { 

            if (rr == null) {
                rr = new Renderer[0];
            }
            if (rms == null || rms.Length < rr.Length) {
                rms = new ModelMaterials[rr.Length];
            }

            rmsCount = 0;
            for (int k = 0; k < rr.Length; k++) {
                rms[rmsCount] = new ModelMaterials();
                Renderer renderer = rr[k];
                rms[rmsCount].renderer = renderer;
                rms[rmsCount].renderWasVisibleDuringSetup = renderer.isVisible;


                if (renderer.transform != target) {
                    HighlightEffect otherEffect = renderer.GetComponent<HighlightEffect>();
                    if (otherEffect != null && otherEffect.enabled) {
                        otherEffect.highlighted = highlighted;
                        continue; // independent subobject
                    }
                }

                if (OnRendererHighlightStart != null) {
                    if (!OnRendererHighlightStart(renderer)) {
                        rmsCount++;
                        continue;
                    }
                }

                CheckCommandBuffers();
                if (renderer is SkinnedMeshRenderer) {
                    // ignore cloth skinned renderers
                    rms[rmsCount].skinnedMesh = true;
                    rms[rmsCount].mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
                } else if (Application.isPlaying && renderer.isPartOfStaticBatch) {
                    // static batched objects need to have a mesh collider in order to use its original mesh
                    MeshCollider mc = renderer.GetComponent<MeshCollider>();
                    if (mc != null) {
                        rms[rmsCount].mesh = mc.sharedMesh;
                    }
                } else {
                    MeshFilter mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null) {
                        rms[rmsCount].mesh = mf.sharedMesh;

#if UNITY_EDITOR
                        if (renderer.gameObject.isStatic && renderer.GetComponent<MeshCollider>() == null) {
                            staticChildren = true;
                        }
#endif

                    }
                }

                if (rms[rmsCount].mesh == null) {
                    continue;
                }

                rms[rmsCount].transform = renderer.transform;
                rms[rmsCount].fxMatMask = Fork(fxMatMask, rms[rmsCount].mesh);
                rms[rmsCount].fxMatSeeThrough = Fork(fxMatSeeThrough, rms[rmsCount].mesh);
                rms[rmsCount].fxMatOverlay = Fork(fxMatOverlay, rms[rmsCount].mesh);
                rms[rmsCount].fxMatInnerGlow = Fork(fxMatInnerGlow, rms[rmsCount].mesh);
                rms[rmsCount].fxMatSolidColor = Fork(fxMatSolidColor, rms[rmsCount].mesh);
                rms[rmsCount].originalMesh = rms[rmsCount].mesh;
                if (smoothNormals && !rms[rmsCount].skinnedMesh) {
                    AverageNormals(rmsCount);
                }
                rmsCount++;
            }

#if UNITY_EDITOR
            // Avoids command buffer issue when refreshing asset inside the Editor
            if (!Application.isPlaying) {
                mipGlowBuffers = null;
                mipOutlineBuffers = null;
            }
#endif

            UpdateMaterialProperties();
        }

        List<Renderer> tempRR;

        Renderer[] FindRenderersWithLayerInScene(LayerMask layer) {
            Renderer[] rr = FindObjectsOfType<Renderer>();
            if (tempRR == null) {
                tempRR = new List<Renderer>();
            } else {
                tempRR.Clear();
            }
            for (var i = 0; i < rr.Length; i++) {
                Renderer r = rr[i];
                if (((1 << r.gameObject.layer) & layer) != 0) {
                    tempRR.Add(r);
                }
            }
            return tempRR.ToArray();
        }

        Renderer[] FindRenderersWithLayerInChildren(LayerMask layer) {
            Renderer[] rr = target.GetComponentsInChildren<Renderer>();
            if (tempRR == null) {
                tempRR = new List<Renderer>();
            } else {
                tempRR.Clear();
            }
            for (var i = 0; i < rr.Length; i++) {
                Renderer r = rr[i];
                if (((1 << r.gameObject.layer) & layer) != 0) {
                    tempRR.Add(r);
                }
            }
            return tempRR.ToArray();
        }

        void CheckGeometrySupportDependencies() {
            InitMaterial(ref fxMatMask, "HighlightPlus/Geometry/Mask");
            InitMaterial(ref fxMatGlowTemplate, "HighlightPlus/Geometry/Glow");
            if (fxMatGlowTemplate != null && useGPUInstancing) fxMatGlowTemplate.enableInstancing = true;
            InitMaterial(ref fxMatInnerGlow, "HighlightPlus/Geometry/InnerGlow");
            InitMaterial(ref fxMatOutlineTemplate, "HighlightPlus/Geometry/Outline");
            if (fxMatOutlineTemplate != null && useGPUInstancing) fxMatOutlineTemplate.enableInstancing = true;
            InitMaterial(ref fxMatOverlay, "HighlightPlus/Geometry/Overlay");
            InitMaterial(ref fxMatSeeThrough, "HighlightPlus/Geometry/SeeThrough");
            InitMaterial(ref fxMatTarget, "HighlightPlus/Geometry/Target");
            InitMaterial(ref fxMatComposeGlow, "HighlightPlus/Geometry/ComposeGlow");
            InitMaterial(ref fxMatComposeOutline, "HighlightPlus/Geometry/ComposeOutline");
            InitMaterial(ref fxMatSolidColor, "HighlightPlus/Geometry/SolidColor");
            InitMaterial(ref fxMatBlurGlow, "HighlightPlus/Geometry/BlurGlow");
            InitMaterial(ref fxMatBlurOutline, "HighlightPlus/Geometry/BlurOutline");
            InitMaterial(ref fxMatClearStencil, "HighlightPlus/ClearStencil");
        }

        void CheckCommandBuffers() {
            if (cbHighlight == null) {
                cbHighlight = new CommandBuffer();
                cbHighlight.name = "Highlight Plus for " + name;
            }
            if (cbSeeThrough == null) {
                cbSeeThrough = new CommandBuffer();
                cbSeeThrough.name = "See Through";
            }
            if (cbOverlay == null) {
                cbOverlay = new CommandBuffer();
                cbOverlay.name = "Overlay";
            }
            if (cbInnerGlow == null) {
                cbInnerGlow = new CommandBuffer();
                cbInnerGlow.name = "Inner Glow";
            }
        }

        Material[] Fork(Material mat, Mesh mesh) {
            if (mesh == null)
                return null;
            int count = mesh.subMeshCount;
            Material[] mm = new Material[count];
            for (int k = 0; k < count; k++) {
                mm[k] = Instantiate<Material>(mat);
            }
            return mm;
        }


        public void UpdateMaterialProperties() {

            if (rms == null)
                return;

            if (ignore) {
                _highlighted = false;
            }

            Color seeThroughTintColor = this.seeThroughTintColor;
            seeThroughTintColor.a = this.seeThroughTintAlpha;

            if (lastOutlineVisibility != outlineVisibility) {
                // change by scripting?
                if (glowQuality == QualityLevel.Highest && outlineQuality == QualityLevel.Highest) {
                    glowVisibility = outlineVisibility;
                }
                lastOutlineVisibility = outlineVisibility;
            }
            if (outlineWidth < 0) {
                outlineWidth = 0;
            }
            if (glowWidth < 0) {
                glowWidth = 0;
            }
            if (overlay < overlayMinIntensity) {
                overlay = overlayMinIntensity;
            }
            if (targetFXTransitionDuration <= 0) {
                targetFXTransitionDuration = 0.0001f;
            }
            if (targetFXStayDuration <= 0) {
                targetFXStayDuration = 0.0001f;
            }

            useSmoothGlow = glow > 0 && glowQuality == QualityLevel.Highest;
            useSmoothOutline = outline > 0 && outlineQuality == QualityLevel.Highest;
            useSmoothBlend = useSmoothGlow || useSmoothOutline;
            if (useSmoothBlend) {
                if (useSmoothGlow && useSmoothOutline) {
                    outlineVisibility = glowVisibility;
                }
            }
            if (useSmoothGlow) {
                fxMatComposeGlow.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                fxMatBlurGlow.SetFloat("_BlurScale", glowWidth / glowDownsampling);
                fxMatBlurGlow.SetFloat("_Speed", glowAnimationSpeed);
            }

            if (useSmoothOutline) {
                fxMatComposeOutline.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                fxMatBlurOutline.SetFloat("_BlurScale", outlineWidth / outlineDownsampling);
            }

            // Setup materials
            for (int k = 0; k < rmsCount; k++) {
                if (rms[k].mesh != null) {
                    // Outline
                    float scaledOutlineWidth = outlineQuality == QualityLevel.High ? 0f : outlineWidth / 100f;
                    for (int m = 0; m < fxMatOutline.Length; m++) {
                        fxMatOutline[m].SetFloat("_OutlineWidth", scaledOutlineWidth);
                        fxMatOutline[m].SetVector("_OutlineDirection", Vector3.zero);
                        fxMatOutline[m].SetInt("_OutlineZTest", GetZTestValue(outlineVisibility));
                        fxMatOutline[m].SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                        fxMatOutline[m].SetFloat("_ConstantWidth", constantWidth ? 1.0f : 0);
                    }

                    // Glow
                    for (int m = 0; m < fxMatGlow.Length; m++) {
                        fxMatGlow[m].SetVector("_Glow2", new Vector3(outlineWidth / 100f, glowAnimationSpeed, glowDithering ? 0 : 1));
                        fxMatGlow[m].SetInt("_GlowZTest", GetZTestValue(glowVisibility));
                        fxMatGlow[m].SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                        fxMatGlow[m].SetFloat("_ConstantWidth", constantWidth ? 1.0f : 0);
                    }

                    // Target
                    if (targetFX) {
                        if (targetFXTexture == null) {
                            targetFXTexture = Resources.Load<Texture2D>("HighlightPlus/target");
                        }
                        fxMatTarget.mainTexture = targetFXTexture;
                    }

                    // Mask, See-through & Overlay per submesh
                    for (int l = 0; l < rms[k].mesh.subMeshCount; l++) {
                        Renderer renderer = rms[k].renderer;
                        if (renderer == null)
                            continue;

                        Material mat = null;
                        if (renderer.sharedMaterials != null && l < renderer.sharedMaterials.Length) {
                            mat = renderer.sharedMaterials[l];
                        }
                        if (mat == null)
                            continue;

                        bool hasTexture = mat.HasProperty("_MainTex");
                        bool useAlphaTest = alphaCutOff > 0 && hasTexture;

                        // Mask
                        if (rms[k].fxMatMask != null && rms[k].fxMatMask.Length > l) {
                            Material fxMat = rms[k].fxMatMask[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                                fxMat.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                            }
                        }

                        // See-through
                        if (rms[k].fxMatSeeThrough != null && rms[k].fxMatSeeThrough.Length > l) {
                            Material fxMat = rms[k].fxMatSeeThrough[l];
                            if (fxMat != null) {
                                fxMat.SetFloat("_SeeThrough", seeThroughIntensity);
                                fxMat.SetFloat("_SeeThroughNoise", seeThroughNoise);
                                fxMat.SetColor("_SeeThroughTintColor", seeThroughTintColor);
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Overlay
                        if (rms[k].fxMatOverlay != null && rms[k].fxMatOverlay.Length > l) {
                            Material fxMat = rms[k].fxMatOverlay[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                if (mat.HasProperty("_Color")) {
                                    fxMat.SetColor("_OverlayBackColor", mat.GetColor("_Color"));
                                }
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Inner Glow
                        if (rms[k].fxMatInnerGlow != null && rms[k].fxMatInnerGlow.Length > l) {
                            Material fxMat = rms[k].fxMatInnerGlow[l];
                            if (fxMat != null) {
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                fxMat.SetFloat("_Width", innerGlowWidth);
                                fxMat.SetInt("_InnerGlowZTest", GetZTestValue(innerGlowVisibility));
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }

                        // Solid Color for smooth glow
                        if (rms[k].fxMatSolidColor != null && rms[k].fxMatSolidColor.Length > l) {
                            Material fxMat = rms[k].fxMatSolidColor[l];
                            if (fxMat != null) {
                                fxMat.color = glowHQColor;
                                fxMat.SetInt("_Cull", cullBackFaces ? (int)UnityEngine.Rendering.CullMode.Back : (int)UnityEngine.Rendering.CullMode.Off);
                                //fxMat.SetInt("_ZTest", GetZTestValue(useSmoothGlow ? glowVisibility : outlineVisibility));
                                if (hasTexture) {
                                    Texture texture = mat.mainTexture;
                                    fxMat.mainTexture = texture;
                                    fxMat.mainTextureOffset = mat.mainTextureOffset;
                                    fxMat.mainTextureScale = mat.mainTextureScale;
                                }
                                //                                if (!Application.isMobilePlatform) { // TODO: currently this does not work with URP on Android
                                if ((glow > 0 && glowQuality == QualityLevel.Highest && glowVisibility == Visibility.Normal) || (outline > 0 && outlineQuality == QualityLevel.Highest && outlineVisibility == Visibility.Normal)) {
                                    fxMat.EnableKeyword(SKW_DEPTHCLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_DEPTHCLIP);
                                }
                                //}
                                if (useAlphaTest) {
                                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                                    fxMat.EnableKeyword(SKW_ALPHACLIP);
                                } else {
                                    fxMat.DisableKeyword(SKW_ALPHACLIP);
                                }
                            }
                        }
                    }
                }
            }
        }

        int GetZTestValue(Visibility param) {
            switch (param) {
                case Visibility.AlwaysOnTop: return (int)UnityEngine.Rendering.CompareFunction.Always;
                case Visibility.OnlyWhenOccluded: return (int)UnityEngine.Rendering.CompareFunction.Greater;
                default:
                    return (int)UnityEngine.Rendering.CompareFunction.LessEqual;
            }
        }

        void BuildQuad() {
            quadMesh = new Mesh();

            // Setup vertices
            Vector3[] newVertices = new Vector3[4];
            float halfHeight = 0.5f;
            float halfWidth = 0.5f;
            newVertices[0] = new Vector3(-halfWidth, -halfHeight, 0);
            newVertices[1] = new Vector3(-halfWidth, halfHeight, 0);
            newVertices[2] = new Vector3(halfWidth, -halfHeight, 0);
            newVertices[3] = new Vector3(halfWidth, halfHeight, 0);

            // Setup UVs
            Vector2[] newUVs = new Vector2[newVertices.Length];
            newUVs[0] = new Vector2(0, 0);
            newUVs[1] = new Vector2(0, 1);
            newUVs[2] = new Vector2(1, 0);
            newUVs[3] = new Vector2(1, 1);

            // Setup triangles
            int[] newTriangles = { 0, 1, 2, 3, 2, 1 };

            // Setup normals
            Vector3[] newNormals = new Vector3[newVertices.Length];
            for (int i = 0; i < newNormals.Length; i++) {
                newNormals[i] = Vector3.forward;
            }

            // Create quad
            quadMesh.vertices = newVertices;
            quadMesh.uv = newUVs;
            quadMesh.triangles = newTriangles;
            quadMesh.normals = newNormals;

            quadMesh.RecalculateBounds();
        }

        #region Normals handling

        static Vector3[] newNormals;
        static int[] matches;
        static Dictionary<Vector3, int> vv;

        void AverageNormals(int objIndex) {
            if (rms == null || objIndex >= rms.Length) return;
            Mesh mesh = rms[objIndex].mesh;
            if (!mesh.isReadable) return;
            Vector3[] normals = mesh.normals;
            if (normals == null || normals.Length == 0)
                return;
            Vector3[] vertices = mesh.vertices;
            int vertexCount = vertices.Length;
            if (normals.Length < vertexCount) {
                vertexCount = normals.Length;
            }
            if (newNormals == null || newNormals.Length < vertexCount) {
                newNormals = new Vector3[vertexCount];
            } else {
                Vector3 zero = Vector3.zero;
                for (int k = 0; k < vertexCount; k++) {
                    newNormals[k] = zero;
                }
            }
            if (matches == null || matches.Length < vertexCount) {
                matches = new int[vertexCount];
            }
            if (vv == null) {
                vv = new Dictionary<Vector3, int>(vertexCount);
            } else {
                vv.Clear();
            }
            // Locate overlapping vertices
            for (int k = 0; k < vertexCount; k++) {
                int i;
                if (!vv.TryGetValue(vertices[k], out i)) {
                    vv[vertices[k]] = i = k;
                }
                matches[k] = i;
            }
            // Average normals
            for (int k = 0; k < vertexCount; k++) {
                int match = matches[k];
                newNormals[match] += normals[k];
            }
            for (int k = 0; k < vertexCount; k++) {
                int match = matches[k];
                normals[k] = newNormals[match].normalized;
            }
            // Reassign normals
            Mesh newMesh = Instantiate(mesh);
            newMesh.hideFlags = HideFlags.DontSave;
            newMesh.normals = normals;
            rms[objIndex].mesh = newMesh;
        }

        #endregion

    }
}



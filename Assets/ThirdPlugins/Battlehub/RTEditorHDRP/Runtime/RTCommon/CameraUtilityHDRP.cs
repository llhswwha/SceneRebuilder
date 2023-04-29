using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Battlehub.RTCommon.HDRP
{
    public class CameraUtilityHDRP : MonoBehaviour, IRenderPipelineCameraUtility
    {
        public event Action<Camera, bool> PostProcessingEnabled;

        private void Awake()
        {
            if (RenderPipelineInfo.Type != RPType.HDRP)
            {
                Destroy(this);
                return;
            }

            IOC.RegisterFallback<IRenderPipelineCameraUtility>(this);
        }

        private void OnDestroy()
        {
            IOC.UnregisterFallback<IRenderPipelineCameraUtility>(this);
        }

        public void EnablePostProcessing(Camera camera, bool value)
        {
            HDAdditionalCameraData cameraData = camera.GetComponent<HDAdditionalCameraData>();
            if (cameraData == null)
            {
                cameraData = camera.gameObject.AddComponent<HDAdditionalCameraData>();
            }

            cameraData.customRenderingSettings = true;
            FrameSettingsOverrideMask settingsOverride = cameraData.renderingPathCustomFrameSettingsOverrideMask;
            FrameSettings settings = cameraData.renderingPathCustomFrameSettings;
            
            settings.SetEnabled(FrameSettingsField.Postprocess, value);
            settings.SetEnabled(FrameSettingsField.AfterPostprocess, value);
            settings.SetEnabled(FrameSettingsField.TransparentPostpass, value);
            settings.SetEnabled(FrameSettingsField.TransparentPostpass, value);
            settings.SetEnabled(FrameSettingsField.LowResTransparent, value);
            settings.SetEnabled(FrameSettingsField.ShadowMaps, value);
            settings.SetEnabled(FrameSettingsField.ContactShadows, value);
            settings.SetEnabled(FrameSettingsField.ScreenSpaceShadows, value);
            settings.SetEnabled(FrameSettingsField.Transmission, value);
            settings.SetEnabled(FrameSettingsField.ExposureControl, value);
            settings.SetEnabled(FrameSettingsField.ReflectionProbe, value);
            settings.SetEnabled(FrameSettingsField.PlanarProbe, value);
            settings.SetEnabled(FrameSettingsField.ReplaceDiffuseForIndirect, value);
            settings.SetEnabled(FrameSettingsField.SkyReflection, value);
            settings.SetEnabled(FrameSettingsField.DirectSpecularLighting, value);
            settings.SetEnabled(FrameSettingsField.Volumetrics, value);
            settings.SetEnabled(FrameSettingsField.ReprojectionForVolumetrics, value);
            settings.SetEnabled(FrameSettingsField.CustomPostProcess, value);

            settingsOverride.mask[(uint)FrameSettingsField.Postprocess] = true;
            settingsOverride.mask[(uint)FrameSettingsField.AfterPostprocess] = true;
            settingsOverride.mask[(uint)FrameSettingsField.TransparentPostpass] = true;
            settingsOverride.mask[(uint)FrameSettingsField.TransparentPostpass] = true;
            settingsOverride.mask[(uint)FrameSettingsField.LowResTransparent] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ShadowMaps] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ContactShadows] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ScreenSpaceShadows] = true;
            settingsOverride.mask[(uint)FrameSettingsField.SubsurfaceScattering] = true;
            settingsOverride.mask[(uint)FrameSettingsField.Transmission] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ExposureControl] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ReflectionProbe] = true;
            settingsOverride.mask[(uint)FrameSettingsField.PlanarProbe] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ReplaceDiffuseForIndirect] = true;
            settingsOverride.mask[(uint)FrameSettingsField.SkyReflection] = true;
            settingsOverride.mask[(uint)FrameSettingsField.DirectSpecularLighting] = true;
            settingsOverride.mask[(uint)FrameSettingsField.Volumetrics] = true;
            settingsOverride.mask[(uint)FrameSettingsField.ReprojectionForVolumetrics] = true;
            settingsOverride.mask[(uint)FrameSettingsField.CustomPostProcess] = true;
            
            cameraData.renderingPathCustomFrameSettingsOverrideMask = settingsOverride;
            cameraData.renderingPathCustomFrameSettings = settings;
            
            if (PostProcessingEnabled != null)
            {
                PostProcessingEnabled(camera, value);
            }
        }

        public bool IsPostProcessingEnabled(Camera camera)
        {
            HDAdditionalCameraData cameraData = camera.GetComponent<HDAdditionalCameraData>();
            if (cameraData == null)
            {
                Debug.Log("IsPostProcessingEnabled 1:"+camera);
                return false;
            }

            FrameSettings settings = cameraData.renderingPathCustomFrameSettings;
            bool isEnabled= settings.IsEnabled(FrameSettingsField.Postprocess);
            Debug.Log("IsPostProcessingEnabled isEnabled:"+isEnabled);
            return isEnabled;
        }

        public void RequiresDepthTexture(Camera camera, bool value)
        {
            
        }

        public void Stack(Camera baseCamera, Camera overlayCamera)
        {
            
        }

        public void SetBackgroundColor(Camera camera, Color color)
        {
            HDAdditionalCameraData data = camera.GetComponent<HDAdditionalCameraData>();
            if(data == null)
            {
                data = camera.gameObject.AddComponent<HDAdditionalCameraData>();
            }
            data.backgroundColorHDR = color;
            data.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
        }
    }

}


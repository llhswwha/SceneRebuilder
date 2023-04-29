using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CameraFrameSettings : MonoBehaviour
{
    public bool Postprocess;

    public bool MSAA;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Camera camera;

    [ContextMenu("GetSettings")]
    public void GetSettings()
    {
        Debug.Log("GetSettings");
        camera=gameObject.GetComponent<Camera>();
        HDAdditionalCameraData cameraData = camera.GetComponent<HDAdditionalCameraData>();
        FrameSettings settings = cameraData.renderingPathCustomFrameSettings;
        Postprocess = settings.IsEnabled(FrameSettingsField.Postprocess);
        MSAA = settings.IsEnabled(FrameSettingsField.Postprocess);
    }

    [ContextMenu("SetSettings")]
    public void SetSettings()
    {
        Debug.Log("SetSettings");
        camera=gameObject.GetComponent<Camera>();
        HDAdditionalCameraData cameraData = camera.GetComponent<HDAdditionalCameraData>();
        FrameSettings settings = cameraData.renderingPathCustomFrameSettings;
        settings.SetEnabled(FrameSettingsField.Postprocess,Postprocess);
        settings.SetEnabled(FrameSettingsField.MSAA,MSAA);
    }
}

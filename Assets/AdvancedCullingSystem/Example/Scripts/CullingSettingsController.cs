using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdvancedCullingSystem.DynamicCullingCore;

namespace AdvancedCullingSystem.Examples
{
    public class CullingSettingsController : MonoBehaviour
    {
        public void EnableCulling()
        {
            DynamicCulling.Instance.Enable();
        }

        public void DisableCulling()
        {
            DynamicCulling.Instance.Disable();
        }

        public void OnSliderChanged(float value)
        {
            DynamicCulling.Instance.SetJobsPerFrame((int)value);
        }
    }
}

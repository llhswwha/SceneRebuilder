using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDebug : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDestroy()
    {
        Debug.LogError("EnableDebug.OnDestroy:" + this.name);
    }

    private void OnDisable()
    {
        Debug.LogError("EnableDebug.OnDisable:" + this.name);
    }

    private void OnEnable()
    {
        Debug.LogError("EnableDebug.OnEnable:" + this.name);
    }
#endif
}

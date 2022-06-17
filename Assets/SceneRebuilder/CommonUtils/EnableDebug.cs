using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDebug : MonoBehaviour
{
    public bool DebugDestroy = true;

    public bool DebugDisable = true;

    public bool DebugEnable = true;

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if(DebugDestroy)
            Debug.LogError($"EnableDebug.OnDestroy:{this.name} path:{transform.GetPath()}");
    }

    private void OnDisable()
    {
        if(DebugDisable)
            Debug.LogError($"EnableDebug.OnDisable:{this.name} path:{transform.GetPath()}");
    }

    private void OnEnable()
    {
        if(DebugEnable)
            Debug.LogError($"EnableDebug.OnEnable:{this.name} path:{transform.GetPath()}");
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneDebug : MonoBehaviour
{
    private void Start()
    {
        SubScene_Base scene = this.GetComponent<SubScene_Base>();
        if (scene != null)
        {
            scene.LoadFinished += Scene_LoadFinished;
        }
        else
        {
            Debug.LogError($"SubSceneDebug scene != null {this.name} path:{transform.GetPath()}");
        }
    }

    private void Scene_LoadFinished(SubScene_Base obj)
    {
        Debug.LogError($"Scene_LoadFinished ! {this.name} path:{transform.GetPath()}");
    }
}

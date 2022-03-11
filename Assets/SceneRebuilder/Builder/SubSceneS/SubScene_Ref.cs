using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_Ref : SubScene_Base
{
    public string RefId = "";

    public static void Init(GameObject go)
    {
        var lodSubScenes = go.GetComponentsInChildren<SubScene_Base>(true);
        foreach (var ss in lodSubScenes)
        {
            var ss2 = go.AddComponent<SubScene_Ref>();
            ss2.sceneArg = ss.sceneArg;
            ss2.RefId = RendererId.GetId(ss.gameObject);
        }
    }

    public bool IsRefSceneLoaded = false;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_Ref //: MonoBehaviour
: SubScene_Base
{
    //public SubSceneArg sceneArg; 

    //public SceneLoadArg GetSceneArg()
    //{
    //    SceneLoadArg arg = new SceneLoadArg();
    //    arg.name = GetSceneName();
    //    arg.path = sceneArg.path;
    //    arg.index = sceneArg.index;
    //    return arg;
    //}

    //public string sceneName = "";

    //public string GetSceneName()
    //{
    //    if (string.IsNullOrEmpty(sceneName))
    //    {
    //        try
    //        {
    //            if (sceneArg == null) return "";
    //            if (string.IsNullOrEmpty(sceneArg.path)) return "";
    //            string[] parts = sceneArg.path.Split(new char[] { '.', '\\', '/' });
    //            if (parts.Length < 2) return "";
    //            sceneName = parts[parts.Length - 2];
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError($"GetSceneName  obj:{this},path:{sceneArg.path},Exception:{ex}");
    //        }

    //    }
    //    return sceneName;
    //}

    //public SceneContentType contentType;

    public string RefId = "";

    public static void BeforeCreateScene(GameObject root)
    {
        var lodSubScenes = root.GetComponentsInChildren<SubScene_Base>(true);
        foreach (var ss in lodSubScenes)
        {
            var ss2 = root.AddComponent<SubScene_Ref>();
            ss2.sceneArg = ss.sceneArg;
            ss2.contentType = SceneContentType.Ref;
            ss2.RefId = RendererId.GetId(ss.gameObject);
            RendererId[] rids = ss.gameObject.GetComponentsInChildren<RendererId>(true);
            foreach(var rid in rids)
            {
                ss2.SceneIds.Add(rid.Id);
            }
        }
    }

    public static void BeforeUnloadScene(GameObject root)
    {
        SubScene_Ref[] refScenes = root.GetComponentsInChildren<SubScene_Ref>(true);
        foreach (var refS in refScenes)
        {
            refS.SceneIds.Clear();
            refS.contentType = SceneContentType.Ref;
            RendererId[] rids = refS.gameObject.GetComponentsInChildren<RendererId>(true);
            foreach (var rid in rids)
            {
                refS.SceneIds.Add(rid.Id);
            }
        }
    }

    public static void ClearRefs(GameObject root)
    {
        SubScene_Ref[] refScenes = root.GetComponentsInChildren<SubScene_Ref>(true);
        foreach (var refS in refScenes)
        {
            GameObject.DestroyImmediate(refS);
        }
    }

    public static void AfterLoadScene(GameObject root)
    {
        SubScene_Ref[] refScenes = root.GetComponentsInChildren<SubScene_Ref>(true);
        foreach (var refS in refScenes)
        {
            GameObject go = IdDictionary.GetGo(refS.RefId);
            if (go == null)
            {
                Debug.LogError($"AfterLoadScene go==null RefId:{refS.RefId}");
                continue;
            }
            SubScene_Base ss = go.GetComponent<SubScene_Base>();
            var arg = refS.sceneArg;
            int idOld = ss.sceneArg.index;
            ss.sceneArg.index = arg.index;
            //Debug.LogError($"AfterLoadScene old:{idOld} new:{ss.sceneArg.index} name:{go.name} path:{arg.path}");
            //GameObject.DestroyImmediate(refS);
            refS.enabled = false;
            refS.IsRefSceneLoaded = true;
        }
    }

    public bool IsRefSceneLoaded = false;

    public List<string> SceneIds = new List<string>();
}

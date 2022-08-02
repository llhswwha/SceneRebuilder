using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_Ref
: SubSceneArgComponent
//: SubScene_Base
{
    

    public string RefId = ""; 

    public static void BeforeCreateScene(GameObject root)
    {
        SubScene_Ref[] refScenes = root.GetComponentsInChildren<SubScene_Ref>(true);

        foreach(var refScene in refScenes)
        {
            GameObject.DestroyImmediate(refScene);
        }

        var lodSubScenes = root.GetComponentsInChildren<SubScene_Base>(true);
        foreach (var ss in lodSubScenes)
        {
            if (ss is SubScene_LODs) continue;
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
        SubScene_Base[] sceneScenes= root.GetComponentsInChildren<SubScene_Base>(true);
        Dictionary<string, SubScene_Base> name2Scenes = new Dictionary<string, SubScene_Base>();
        foreach(var scene in sceneScenes)
        {
            string sName = scene.GetSceneName();
            if (name2Scenes.ContainsKey(sName))
            {
                Debug.LogError($"AfterLoadScene0 name2Scenes.ContainsKey(sName) refScenes:{refScenes.Length} sceneScenes:{sceneScenes.Length} sName:{sName} scene:{scene} path={scene.transform.GetPath()}");
            }
            else
            {
                name2Scenes.Add(scene.GetSceneName(), scene);
            }
           
        }
        foreach (var refS in refScenes)
        {
            GameObject go = IdDictionary.GetGo(refS.RefId);
            SubScene_Base ss = null;
            if (go == null)
            {
                if (name2Scenes.ContainsKey(refS.sceneName))
                {
                    SubScene_Base scene = name2Scenes[refS.sceneName];
                    ss = scene;
                    Debug.LogError($"AfterLoadScene2 go==null RefId:{refS.RefId} Name:{refS.sceneName} refScenes:{refScenes.Length} sceneScenes:{sceneScenes.Length}");
                    continue;
                }
                else
                {
                    Debug.LogError($"AfterLoadScene1 go==null RefId:{refS.RefId} Name:{refS.sceneName} refScenes:{refScenes.Length} sceneScenes:{sceneScenes.Length}");
                    continue;
                }
            }
            if (ss == null)
            {
                ss = go.GetComponent<SubScene_Base>();
            }
            
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectId : MonoBehaviour
{
    public string Id;

    public int insId;

    public string parentId;

    private void Awake()
    {
        Init();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Id = Guid.NewGuid().ToString();
        insId = this.gameObject.GetInstanceID();
    }


}

// public static class ObjectIdHelper
// {
//     public static ObjectId InitObjectId(GameObject go)
//     {
//         if(go==null)return null;
//         MeshRenderer meshRenderer=go.GetComponent<MeshRenderer>();
//         if(meshRenderer){

//         }
//         else{
//             RendererId rId=go.GetComponent<RendererId>();
//             if(rId!=null){
//                 rId=go.AddComponent<RendererId>();
//                 rId.Init(meshRenderer);
//             }
//             else{

//             }
//         }
//     }

//     public static string GetId(GameObject r)
//     {
//         if (r == null) return "";
//         RendererId id = r.GetComponent<RendererId>();
//         if (id == null)
//         {
//             id = r.gameObject.AddComponent<RendererId>();
//             id.Init(r);
//         }
//         return id.Id;
//     }

//     public static RendererId GetId(MeshRenderer r)
//     {
//         RendererId id = r.GetComponent<RendererId>();
//         if (id == null)
//         {
//             id = r.gameObject.AddComponent<RendererId>();
//             id.Init(r);
//         }
//         return id;
//     }
// }

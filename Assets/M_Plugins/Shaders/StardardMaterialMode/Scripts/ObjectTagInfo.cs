using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTagInfo : MonoBehaviour
{
    public List<string> items=new List<string>();

    [ContextMenu("SetAll")]
    public void SetAll(){
        var ts=gameObject.GetComponentsInChildren<Transform>(true);
        foreach(var t in ts){
            ObjectTagInfo info=t.GetComponent<ObjectTagInfo>();
            if(info==null){
                info=t.gameObject.AddComponent<ObjectTagInfo>();
            }
            info.items=items;
        }
    }
}

public class ObjectTags{
    public const string Structure="Structure";
}

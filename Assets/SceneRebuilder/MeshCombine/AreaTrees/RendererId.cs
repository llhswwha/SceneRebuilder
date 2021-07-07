using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererId
    : MonoBehaviour
    //: Behaviour
{
    public string Id;

    public string parentId;

    public int insId;

    public MeshRenderer mr;

    internal void Init(MeshRenderer r)
    {
        this.mr = r;
        Id = Guid.NewGuid().ToString();
        insId = r.gameObject.GetInstanceID();

        parentId=GetId(this.transform.parent);
    }

    internal void Init(GameObject go)
    {
        if (go == null) return;
        this.mr = go.GetComponent<MeshRenderer>();
        Id = Guid.NewGuid().ToString();
        insId = go.GetInstanceID();
        //parentId=GetId(this.transform.parent);
    }

    [ContextMenu("SetParent")]
    public void SetParent()
    {
        if(string.IsNullOrEmpty(parentId)){
            return;//
        }
        GameObject pGo=IdDictionay.GetGo(parentId);
        if(pGo!=null){
            this.transform.SetParent(pGo.transform);
        }
        else{
            Debug.LogError($"RendererId.SetParent pGo==null name:{this.name} Id:{this.Id} parentId:{this.parentId}");
        }
    }

    [ContextMenu("GetParent")]
    public void GetParent()
    {
        GameObject pGo=IdDictionay.GetGo(parentId);
        Debug.LogError($"RendererId.GetParent name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
    }

    [ContextMenu("GetParentEx")]
    public void GetParentEx()
    {
        GameObject pGo=IdDictionay.GetGoEx(parentId);
        Debug.LogError($"RendererId.GetParentEx name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
    }

    public static string GetId(GameObject r)
    {
        if (r == null) return "";
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        return id.Id;
    }

    public static string GetId(Transform r)
    {
        if (r == null) return "";
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r.gameObject);
        }
        return id.Id;
    }

    public static RendererId GetId(MeshRenderer r)
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        return id;
    }

    private void OnDestroy()
    {
        // Debug.Log("RendererId.OnDestroy:"+this.name);
    }
}

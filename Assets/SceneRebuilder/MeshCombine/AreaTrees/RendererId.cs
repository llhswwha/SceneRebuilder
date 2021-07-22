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

    [ContextMenu("Init")]
    public void Init()
    {
        Init(this.gameObject, 0);
    }

    internal void Init(MeshRenderer r)
    {
        this.mr = r;
        //int instanceId= r.gameObject.GetInstanceID();
        //if(instanceId!= insId)
        //{
        //    //insId = instanceId;
        //    //Id = Guid.NewGuid().ToString();
        //    UpdateId(instanceId, r.gameObject.transform);
        //}
        //SetParentId();
        Init(r.gameObject, 0);
    }

    internal void Init(GameObject go, int level)
    {
        if (go == null) return;
        if (this.mr == null)
        {
            this.mr = go.GetComponent<MeshRenderer>();
        }
        
        //Id = Guid.NewGuid().ToString();
        //insId = go.GetInstanceID();
        //parentId=GetId(this.transform.parent,level+1);

        int instanceId = go.GetInstanceID();
        if (instanceId != insId)
        {
            UpdateId(instanceId, go.transform);
        }
        parentId = GetId(this.transform.parent, level + 1);
    }

    private void UpdateId(int instanceId,Transform t)
    {
        insId = instanceId;
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.NewGuid().ToString();
        }
        else
        {
            Id = Guid.NewGuid().ToString();
            //update children 
            for (int i = 0; i < t.childCount; i++)
            {
                RendererId rId = t.GetChild(i).GetComponent<RendererId>();
                rId.Init();
            }
        }
    }



    public void SetParentId()
    {
        parentId = GetId(this.transform.parent,0);
    }

    public bool IsParentChanged()
    {
        var newParentId=GetId(this.transform.parent,0);
        bool isChanged=newParentId != parentId;
        if(isChanged){
            Debug.Log($"ParentChanged {this.name} parentId:{parentId} newParentId:{newParentId} parent:{GetParent()} newParent:{this.transform.parent}");
        }
        return isChanged;
    }
    

    [ContextMenu("SetParent")]
    public void SetParent()
    {
        if(string.IsNullOrEmpty(parentId)){
            return;//
        }
        GameObject pGo=IdDictionary.GetGo(parentId);
        if(pGo!=null){
            this.transform.SetParent(pGo.transform);
        }
        else{
            Debug.LogError($"RendererId.SetParent pGo==null name:{this.name} Id:{this.Id} parentId:{this.parentId}");
        }
    }

    [ContextMenu("GetParent")]
    public GameObject GetParent()
    {
        if(string.IsNullOrEmpty(parentId))
        {
            parentId=GetId(this.transform.parent,0);
        }
        GameObject pGo=IdDictionary.GetGo(parentId);
        // Debug.LogError($"RendererId.GetParent name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
        return pGo;
    }

    [ContextMenu("GetParentEx")]
    public void GetParentEx()
    {
        GameObject pGo=IdDictionary.GetGoEx(parentId);
        Debug.LogError($"RendererId.GetParentEx name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
    }

    public static string GetId(GameObject go,int level)
    {
        if (go == null) return "";
        RendererId id = go.GetComponent<RendererId>();
        if (id == null)
        {
            id = go.gameObject.AddComponent<RendererId>();
            id.Init(go, level+1);
        }
        return id.Id;
    }

    public static string GetId(Transform t,int level)
    {
        if (t == null || level >= 2) return "";
        RendererId id = t.GetComponent<RendererId>();
        if (id == null )
        {
            id = t.gameObject.AddComponent<RendererId>();
            id.Init(t.gameObject,level+1);
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

    public static RendererId InitId(MeshRenderer r)
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        id.SetParentId();
        return id;
    }

    public static void InitIds(GameObject rootObj)
    {
        MeshRenderer[] renderers=rootObj.GetComponentsInChildren<MeshRenderer>();
        InitIds(renderers);
    }

    public static void InitIds(MeshRenderer[] renderers)
    {
        // DateTime start = DateTime.Now;
        int count = renderers.Length;
        for (int i = 0; i < count; i++)
        {
            MeshRenderer r = renderers[i];
            RendererId id = RendererId.InitId(r);
            // float progress = (float)i / count;
            // float percents = progress * 100;
            // if (ProgressBarHelper.DisplayCancelableProgressBar("InitIds", $"Progress1 {i}/{count} {percents:F1}% {r.name}", progress))
            // {
            //     break;
            // }

        }

        // Count = allRenderers.Length;
        // ProgressBarHelper.ClearProgressBar();

        // Debug.Log($"InitIds count:{renderers.Length} time:{(DateTime.Now - start)}");
    }

    private void OnDestroy()
    {
        // Debug.Log("RendererId.OnDestroy:"+this.name);
    }
}

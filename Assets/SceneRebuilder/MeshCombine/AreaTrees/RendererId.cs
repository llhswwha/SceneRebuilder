using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObjectId
/// </summary>
public class RendererId
    : MonoBehaviour
    //: Behaviour
{

    public string Id;

    public string GetId()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Init();
        }
        return Id;
    }

    public string parentId;

    public int insId;

    public MeshRenderer mr;

    public List<string> childrenIds = new List<string>();

    public override string ToString()
    {
        return $"RendererId Id:{Id} parentId:{parentId} insId:{insId} mr:{mr}";
    }

    public string GetText()
    {
        return $"[insId:{insId} Id:{Id} ]";
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Init(this.gameObject, 0);
    }

    [ContextMenu("Clear")]
    public void ClearIds()
    {
        var ids = this.GetComponentsInChildren<RendererId>(true);
        foreach(var id in ids)
        {
            GameObject.DestroyImmediate(id);
        }
    }

    [ContextMenu("ClearScripts")]
    public void ClearScripts()
    {
        var ids = this.GetComponentsInChildren<MonoBehaviour>(true);
        int count = 0;
        foreach (var id in ids)
        {
            if (id.gameObject == this.gameObject) continue;
            count++;
            GameObject.DestroyImmediate(id);
        }
        Debug.Log("ClearScripts "+count);
    }

    [ContextMenu("ClearLODs")]
    public void ClearLODs()
    {
        var ids = this.GetComponentsInChildren<LODGroup>(true);
        int count = 0;
        foreach (var id in ids)
        {
            if (id.gameObject == this.gameObject) continue;
            count++;
            GameObject.DestroyImmediate(id);
        }
        Debug.Log("ClearScripts " + count);
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        MeshHelper.ShowAllRenderers(renderers, 5,this.name);
    }

    public void ResetTransform()
    {
        //transform.rotation = Quaternion.identity;
        //transform.localScale = Vector3.one;
        //transform.position = Vector3.zero;

        var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var renderer in renderers)
        {
            renderer.transform.rotation = Quaternion.identity;
            renderer.transform.localScale = Vector3.one;
            renderer.transform.position = Vector3.zero;
        }
    }

    public void ResetPos()
    {
        //transform.rotation = Quaternion.identity;
        //transform.localScale = Vector3.one;
        //transform.position = Vector3.zero;

        var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            renderer.transform.position= new Vector3(0.1f,0,0)*i;
        }
    }

    //[ContextMenu("ClearComponents")]
    //public void ClearComponents()
    //{
    //    var ids = this.GetComponentsInChildren<MonoBehaviour>(true);
    //    foreach (var id in ids)
    //    {
    //        if (id.gameObject == this.gameObject) continue;
    //        GameObject.DestroyImmediate(id);
    //    }
    //}

    internal void Init<T>(T r) where T :Component
    {
        if(r is MeshRenderer)
        {
            this.mr = r as MeshRenderer;
        }
        else
        {
            this.mr = r.GetComponent<MeshRenderer>();
        }
        
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
        insId = go.GetInstanceID();
        //parentId=GetId(this.transform.parent,level+1);

        //int instanceId = go.GetInstanceID();
        //if (instanceId != insId)
        //{
        //    UpdateId(instanceId, go.transform);
        //}

        if (string.IsNullOrEmpty(Id))
        {
            NewId();
        }
        var pid= GetId(this.transform.parent, level + 1);
        SetPid(pid, this.transform.parent);
    }

    public void NewId()
    {
        Id = Guid.NewGuid().ToString();
        UpdateChildrenId(false);
    }

    public void UpdateChildrenId(bool isAll)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var rid = child.GetComponent<RendererId>();
            if (rid)
            {
                rid.SetPid(Id, this.transform);

                if (isAll)
                {
                    rid.UpdateChildrenId(isAll);
                }
            }
        }
    }

    public void Refresh()
    {
        insId = 0;
        Id = "";
        SetPid("",null);
        Init();
    }

    //private void UpdateId(int instanceId,Transform t)
    //{
    //    insId = instanceId;
    //    if (string.IsNullOrEmpty(Id))
    //    {
    //        Id = Guid.NewGuid().ToString();
    //    }
    //    else
    //    {
    //        Id = Guid.NewGuid().ToString();
    //        //update children 
    //        for (int i = 0; i < t.childCount; i++)
    //        {
    //            //RendererId rId = t.GetChild(i).GetComponent<RendererId>();
    //            //rId.Init();

    //            RendererId rId = t.GetChild(i).GetComponent<RendererId>();
    //            if (rId == null)
    //            {
    //                rId = t.GetChild(i).gameObject.AddComponent<RendererId>();
    //                rId.Init();
    //            }
    //            else
    //            {
    //                rId.Init();
    //            }
    //        }
    //    }
    //}

    public void RefreshParentId()
    {
        if (string.IsNullOrEmpty(parentId))
        {
            SetParentId(false);
        }
    }

    public void SetParentId(bool isShowLog=false)
    {
        var newP= GetId(this.transform.parent, 0);
        if (string.IsNullOrEmpty(newP))
        {
            Debug.LogError($"SetParentId string.IsNullOrEmpty(newP) parent:{this.transform.parent} oldP:{parentId} Id:{Id} name:{this.name}");
        }
        if(string.IsNullOrEmpty(parentId))
        {
            //Debug.Log($"SetParentId oldP:NULL newP:{newP} Id:{Id} name:{this.name} 12");
        }
        else if(newP != parentId)
        {
            if (isShowLog)
            {
                Debug.LogError($"SetParentId oldP:{parentId} newP:{newP} Id:{Id} name:{this.name}");
            }
            
        }
        SetPid(newP, this.transform.parent);
    }

    public void SetPid(string pid,Transform p)
    {
        if (p != null)
        {
            if (p.name.Contains("Node_") && !this.name.Contains("Node_"))
            {
                //Debug.LogError($"RendererId SetPid Not Set Parent old:{parentId} new:{pid} gameObject:{this.name} parent:{p}");
                return;
            }
        }
        if(string.IsNullOrEmpty(pid))
        {
            //Debug.LogError($"RendererId SetPid old:{parentId} new:{pid} gameObject:{this.name} parent:{p}");
        }
        
        parentId = pid;
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

    public void UpdateParent()
    {
        this.SetParentId(false);
    }
    

    [ContextMenu("SetParent")]
    public GameObject SetParent()
    {
        if(string.IsNullOrEmpty(parentId)){
            return null;//
        }
        GameObject pGo=IdDictionary.GetParentGo(this);
        if(pGo!=null){
            this.transform.SetParent(pGo.transform);
            //Debug.LogError($"RendererId.SetParent1 pGo!=null name:{this.name} Id:{this.Id} parentId:{this.parentId}");
        }
        else{
            Debug.LogError($"RendererId.SetParent2 pGo==null name:{this.name} Id:{this.Id} parentId:{this.parentId}");
        }
        return pGo;
    }

    [ContextMenu("GetParent")]
    public GameObject GetParent()
    {
        if(string.IsNullOrEmpty(parentId))
        {
            SetPid(GetId(this.transform.parent, 0), this.transform.parent);
        }
        GameObject pGo= IdDictionary.GetParentGo(this);
        // Debug.LogError($"RendererId.GetParent name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
        return pGo;
    }

    public void RecoverParent()
    {
        GameObject p = GetParent();
        if (p != null)
        {
            this.transform.SetParent(p.transform);
        }
        else
        {
            Debug.LogError($"RecoverParent parentId:{parentId} this:{this} currentParent:{this.transform.parent}");
        }
    }

    [ContextMenu("GetCurrentParent")]
    public GameObject GetCurrentParent()
    {
        if (this.transform.parent == null) return null;
        var pId_current= GetId(this.transform.parent, 0);
        if(pId_current== parentId)
        {
            return this.transform.parent.gameObject;
        }
        else
        {
            return null;
        }
    }

    [ContextMenu("FindParent")]
    public void FindParent()
    {
        GameObject pGo=IdDictionary.GetGoEx(parentId);
        Debug.LogError($"RendererId.GetParentEx name:{this.name} Id:{this.Id} parentId:{this.parentId} pGo:{pGo}");
    }

    public static string GetId(GameObject go,int level=0)
    {
        if (go == null) return "";
        RendererId id = go.GetComponent<RendererId>();
        if (id == null)
        {
            id = go.gameObject.AddComponent<RendererId>();
            id.Init(go, level+1);
        }
        return id.GetId();
    }

    public static int GetInsId(GameObject go, int level = 0)
    {
        if (go == null) return 0;
        RendererId id = go.GetComponent<RendererId>();
        if (id == null)
        {
            id = go.gameObject.AddComponent<RendererId>();
            id.Init(go, level + 1);
        }
        return id.insId;
    }

    //public static string GetId(Component go, int level = 0)
    //{
    //    return GetId(go.gameObject, level);
    //}

    //public static string GetId(Transform go, int level = 0)
    //{
    //    return GetId(go.gameObject, level);
    //}

    public static RendererId GetRId(GameObject go, int level=0)
    {
        if (go == null) return null;
        RendererId id = go.GetComponent<RendererId>();
        if (id == null)
        {
            id = go.gameObject.AddComponent<RendererId>();
            id.Init(go, level + 1);
        }
        return id;
    }

    public static string GetId<T>(T t,int level) where T : Component
    {
        if (t == null || level >= 2) return "";
        RendererId id = t.GetComponent<RendererId>();
        if (id == null )
        {
            id = t.gameObject.AddComponent<RendererId>();
            id.Init(t.gameObject,level+1);
        }
        return id.GetId();
    }
    public static string GetId<T>(T r) where T : Component
    {
        if (r == null) return "";
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        return id.GetId();
    }

    public static RendererId GetRId<T>(T t, int level=0) where T : Component
    {
        if (t == null || level >= 2) return null;
        RendererId id = t.GetComponent<RendererId>();
        if (id == null)
        {
            id = t.gameObject.AddComponent<RendererId>();
            id.Init(t.gameObject, level + 1);
        }
        return id;
    }



    public static Transform MoveTargetsParent(List<GameObject> targets,Transform newParent,string newParentName="NewP")
    {
        if (newParent == null)
        {
            newParent = new GameObject(newParentName).transform;
        }
        foreach (GameObject item in targets)
        {
            if (item == newParent.gameObject) continue;
            RendererId.InitId(item);
            item.transform.SetParent(newParent);
        }
        return newParent;
    }

    public static Transform MoveTargetsParent(List<Transform> targets, Transform newParent, string newParentName = "NewP")
    {
        if (newParent == null)
        {
            newParent = new GameObject(newParentName).transform;
        }
        foreach (Transform item in targets)
        {
            if (item == newParent) continue;
            RendererId.InitId(item);
            item.transform.SetParent(newParent);
        }
        return newParent;
    }

    public static void RecoverTargetsParent(List<GameObject> targets, Transform newParent)
    {
        IdDictionary.InitInfos();
        foreach (var item in targets)
        {
            if (newParent != null && item == newParent.gameObject)
            {
                Debug.LogError($"RecoverTargetsParent({item}) newParent != null && item == newParent.gameObject");
                continue;
            }
            RendererId rId = RendererId.GetRId(item);
            if (rId == null)
            {
                Debug.LogError($"RecoverTargetsParent({item}) rId == null");
                continue;
            }
            rId.RecoverParent();
        }
    }

    public static void RecoverTargetsParent(List<Transform> targets, Transform newParent)
    {
        IdDictionary.InitInfos();
        foreach (Transform item in targets)
        {
            if (item == newParent) continue;
            RendererId rId = RendererId.GetRId(item);
            rId.RecoverParent();
        }
    }

    public static RendererId InitId(GameObject r)
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init();
        }
        id.SetParentId(true);
        return id;
    }

    public static RendererId InitId<T>(T r) where T :Component
    {
        if (r == null) return null;
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
            id.Init(r);
        }
        id.SetParentId(true);
        return id;
    }

    public static RendererId[] NewIds(GameObject go)
    {
        RendererId[] ids = go.GetComponentsInChildren<RendererId>(true);
        foreach(var id in ids)
        {
            id.NewId();
        }
        return ids;
    }

    public static RendererId[] NewIds<T>(T go) where T : Component
    {
        RendererId[] ids = go.GetComponentsInChildren<RendererId>(true);
        foreach (var id in ids)
        {
            id.NewId();
        }
        return ids;
    }

    public static RendererId UpdateId(GameObject r,bool isForce=false) 
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
        }
        if (isForce)
        {
            id.NewId();
        }
        id.Init(r,0);
        return id;
    }

    public static RendererId UpdateId<T>(T r, bool isForce = false) where T : Component
    {
        RendererId id = r.GetComponent<RendererId>();
        if (id == null)
        {
            id = r.gameObject.AddComponent<RendererId>();
        }
        if (isForce)
        {
            id.NewId();
        }
        id.Init(r);
        return id;
    }

    public static void InitIds(GameObject rootObj,bool showLog=false)
    {
        if (rootObj == null)
        {
            Debug.LogError("InitIds rootObj == null");
            return;
        }
        MeshRenderer[] renderers=rootObj.GetComponentsInChildren<MeshRenderer>(true);
        InitIds(renderers);
        if (showLog)
        {
            Debug.Log($"InitIds rootObj:{rootObj} renderers:{renderers.Length}");
        }
    }

    public static void UpdateIds(GameObject rootObj, bool showLog = false)
    {
        if (rootObj == null)
        {
            Debug.LogError("InitIds rootObj == null");
            return;
        }
        MeshRenderer[] renderers = rootObj.GetComponentsInChildren<MeshRenderer>(true);
        UpdateIds(renderers);
        if (showLog)
        {
            Debug.Log($"InitIds rootObj:{rootObj} renderers:{renderers.Length}");
        }
    }

    public static void UpdateIds<T>(T[] renderers) where T : Component
    {
        // DateTime start = DateTime.Now;
        int count = renderers.Length;
        for (int i = 0; i < count; i++)
        {
            T r = renderers[i];
            RendererId id = RendererId.UpdateId(r);
        }
    }

    public static void InitIds<T>(T[] renderers) where T : Component
    {
        // DateTime start = DateTime.Now;
        int count = renderers.Length;
        for (int i = 0; i < count; i++)
        {
            T r = renderers[i];
            RendererId id = RendererId.InitId(r);
        }
    }

    public static void ChangeChildrenParent(Transform root,Transform newParent)
    {
        List<Transform> childrens = new List<Transform>();
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            childrens.Add(child);
        }
        foreach (var child in childrens)
        {
            child.SetParent(newParent);

            UpdateId(child);
        }
    }

    private void OnDestroy()
    {
        // Debug.Log("RendererId.OnDestroy:"+this.name);
    }

    public bool IsDebug = false;

    private void OnDisable()
    {
        if(IsDebug)
            Debug.LogError("RendererId.OnDisable:" + this.name);
    }

    private void OnEnable()
    {
        if (IsDebug)
            Debug.LogError("RendererId.OnEnable:" + this.name);
    }
}

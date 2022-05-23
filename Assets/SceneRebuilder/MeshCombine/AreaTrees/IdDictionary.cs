using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class IdDictionary
{
    public static Dictionary<string, MeshRenderer> RendererDict = new Dictionary<string, MeshRenderer>();

    public static Dictionary<string, RendererId> IdDict = new Dictionary<string, RendererId>();

    public static void Check()
    {

    }

    public static List<string> GetIds()
    {
        return IdDict.Keys.ToList();
    }

    public static List<RendererId> GetRIds()
    {
        return IdDict.Values.ToList();
    }

    public static List<MeshRenderer> GetRenderers()
    {
        return RendererDict.Values.ToList();
    }

    //public static List<RendererId> GetIds()
    //{
    //    return IdDict.Values.ToList();
    //}

    public static void SetId(GameObject go)
    {
        RendererId id = go.GetComponent<RendererId>();
        SetId(id);
    }

    public static void SetId(RendererId id, bool isUpdate = false)
    {
        SetRendererId(id, id.mr, isUpdate);
    }

    private static void SetRendererId(RendererId id, MeshRenderer renderer, bool isUpdate = false, bool isShowError = false)
    {
        try
        {
            if (id == null)
            {
                Debug.LogError($"RendererDictionay.SetRendererId id == null");
            }
            else
            {
                if (string.IsNullOrEmpty(id.Id))
                {
                    if (renderer == null)
                    {
                        id.NewId();
                        Debug.LogWarning($"RendererDictionay.SetRendererId string.IsNullOrEmpty(id.Id)  newId:{id.Id} path:{id.transform.GetPath()}");
                        return;
                    }
                    else
                    {
                        Debug.LogError($"RendererDictionay.SetRendererId string.IsNullOrEmpty(id.Id) path:{id.transform.GetPath()}");
                        return;
                    }
                    
                }
                if (renderer != null)
                {
                    if (isUpdate)
                    {
                        if (IdDict.ContainsKey(id.Id))
                        {
                            RendererId rid1 = IdDict[id.Id];
                            Debug.LogWarning($"SetRendererId(RendererId) id:{id.Id} [rid1:{rid1}] [rid2:{id.name}] [path1:{rid1.transform.GetPath()}] [path2:{id.transform.GetPath()}]");
                            id.NewId();
                        }
                    }

                    if (!RendererDict.ContainsKey(id.Id))
                    {
                        RendererDict.Add(id.Id, renderer);
                    }
                    else
                    {
                        MeshRenderer renderer1 = RendererDict[id.Id];
                        if (renderer1 == null)
                        {
                            RendererDict[id.Id] = renderer;
                        }
                        else if (renderer1 != renderer)
                        {
                            Debug.LogError($"SetRendererId(MeshRenderer) id:{id.Id} [renderer1:{renderer1}] [renderer2:{renderer}] [path1:{renderer1.transform.GetPath()}] [path2:{id.transform.GetPath()}]");
                            RendererDict[id.Id] = renderer;
                        }
                        
                    }

                    if (!IdDict.ContainsKey(id.Id))
                    {
                        IdDict.Add(id.Id, id);
                    }
                    else
                    {
                        RendererId rid1 = IdDict[id.Id];
                        //Debug.LogError($"SetRendererId(RendererId) id:{id.Id} rid1:{rid1.name} rid2:{id.name}");
                        IdDict[id.Id] = id;
                    }
                }
                else
                {
                    if (!IdDict.ContainsKey(id.Id))
                    {
                        IdDict.Add(id.Id, id);
                    }
                    else
                    {
                        RendererId rid1 = IdDict[id.Id];
                        if (rid1 == null)
                        {
                            IdDict[id.Id] = id;
                        }
                        else if (rid1 != id)
                        {
                            Debug.LogError($"SetRendererId(RendererId) id:{id.Id} [rid1:{rid1}] [rid2:{id.name}] [path1:{rid1.transform.GetPath()}] [path2:{id.transform.GetPath()}]");
                            IdDict[id.Id] = id;
                        }
                        
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"RendererDictionay.SetRendererId Renderer:{id},Exception:{ex}");
        }
    }

    public static void InitInfos(GameObject root = null, bool includeInactive = true, bool isUpdate = false)
    {
        DateTime start = DateTime.Now;
        RendererDict.Clear();
        IdDict.Clear();

        //InitRenderers(root, includeInactive);

        InitIds(root, includeInactive, isUpdate);
        Debug.Log($"IdDictionay.InitInfos root:[{root}] idCount:{IdDict.Count},RendererCount:{RendererDict.Count} time:{(DateTime.Now - start)}");
    }

    private static void InitRenderers(GameObject root = null,bool includeInactive=true)
    {
        DateTime start = DateTime.Now;
        MeshRenderer[] renderers = null;
        if (root == null)
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>(includeInactive);
        }
        else
        {
            renderers = root.GetComponentsInChildren<MeshRenderer>(includeInactive);
        }
        InitRenderers(renderers);
        //Debug.Log($"IdDictionay.InitRenderers count:{renderers.Length},Dict:{RendererDict.Count} time:{(DateTime.Now - start)}");
    }

    private static RendererId[] InitIds(GameObject root=null, bool includeInactive = true, bool isUpdate=false)
    {
        DateTime start = DateTime.Now;

        //RendererId.InitIds(root);

        RendererId[] ids = null;
        if (root == null)
        {
            ids = GameObject.FindObjectsOfType<RendererId>(includeInactive);
        }
        else
        {
            ids = root.GetComponentsInChildren<RendererId>(includeInactive);
        }
        foreach (RendererId id in ids)
        {
            SetId(id, isUpdate);
            //id.childrenIds.Clear();
        }
        //Debug.Log($"IdDictionay.InitIds count:{ids.Length},Dict:{IdDict.Count} time:{(DateTime.Now - start)}");
        return ids;
    }

    public static void SaveChildrenIds(List<RendererId> idsAll, GameObject root)
    {
        IdDictionary.InitIds(root);
        Dictionary<string, RendererId> dict = new Dictionary<string, RendererId>();
        foreach (var id in idsAll)
        {
            if (!dict.ContainsKey(id.parentId))
            {
                RendererId parent = IdDictionary.GetId(id.parentId);
                if (parent != null)
                {
                    dict.Add(id.parentId, parent);
                    //if (parent.childrenIds.Count > 0)
                    //{
                    //    Debug.LogError($"SaveChildrenIds parent.childrenIds.Count > 0 childrens:{parent.childrenIds.Count} id:{id} parentId:{id.parentId} parent:{parent}");
                    //}
                    parent.childrenIds.Clear();
                    parent.childrenIds.Add(id.Id);
                }
                else
                {
                    Debug.LogError($"SaveChildrenIds parent==null!!!! id:{id} parentId:{id.parentId} parent:NULL");
                }
            }
            else
            {
                RendererId parent = dict[id.parentId];
                parent.childrenIds.Add(id.Id);
            }
        }

    }

    public static void InitRenderers(IEnumerable<MeshRenderer> renderers)
    {
        foreach (var renderer in renderers)
        {
            if(renderer==null)
            {
                Debug.LogError($"renderer==null");
                continue;
            }
            RendererId id = RendererId.GetRId(renderer);
            SetRendererId(id, renderer);
        }
    }


    public static void InitGos(IEnumerable<GameObject> objs,string tag)
    {
        DateTime start = DateTime.Now;
        int count1 = RendererDict.Count;
        int idCount = 0;
        foreach(var obj in objs)
        {
            if (obj == null) continue;
            //var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            //InitRenderers(renderers);

            var ids = obj.GetComponentsInChildren<RendererId>(true);
            foreach(var id in ids)
            {
                SetId(id);
            }
            idCount += ids.Length;
        }
        int count2 = RendererDict.Count;
        //Debug.Log($"RendererDictionay.InitRenderers tag:{tag} objs:{objs.Count()},idCount:{idCount} count1:{count1} count2:{count2} add:{count2-count1} time:{(DateTime.Now - start)}");
    }

    public static GameObject GetParentGo(RendererId rId)
    {
        return GetGo(rId.parentId, rId.transform);
    }

     public static GameObject GetGo(string id, Transform t = null, bool showLog = true)
    {
        RendererId rId = GetId(id, t, showLog);
        if (rId != null)
        {
            return rId.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static RendererId GetId(string id, Transform t = null,bool showLog=true,bool isError=true)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"RendererDictionay.GetGo IsNullOrEmpty !!! ,Dict:{IdDict.Count}");
            return null;
        }
        if (IdDict.ContainsKey(id))
        {
            var go = IdDict[id];
            if (go == null)
            {
                Debug.LogError($"RendererDictionay.GetGo go == null :{id},Dict:{IdDict.Count}");
                return null;
            }
            return go;
        }
        if(showLog)
        {
            if (isError)
            {
                Debug.LogError($"RendererDictionay.GetGo go not found id:{id},\tTransform:{t},\tPath:{t.GetPath()}\tDict:{IdDict.Count}");
            }
            else
            {
                Debug.LogWarning($"RendererDictionay.GetGo go not found id:{id},\tTransform:{t},\tPath:{t.GetPath()}\tDict:{IdDict.Count}");
            }
        }
            
        return null;
    }

    public static GameObject GetGo(string id, string name, bool showLog = true)
    {
        RendererId rId = GetId(id, name, showLog);
        if (rId != null)
        {
            return rId.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static RendererId GetPId(IdInfo rid, bool showLog)
    {
        if (string.IsNullOrEmpty(rid.parentId))
        {
            Debug.LogError($"RendererDictionay.GetPId IsNullOrEmpty !!! ,Dict:{IdDict.Count} rid:{rid.GetFullString()} ");
            return null;
        }
        if (IdDict.ContainsKey(rid.parentId))
        {
            var go = IdDict[rid.parentId];
            if (go == null)
            {
                Debug.LogError($"RendererDictionay.GetPId go == null :{rid.parentId},Dict:{IdDict.Count}");
                return null;
            }
            return go;
        }
        if (showLog)
            Debug.LogError($"RendererDictionay.GetPId go not found id:{rid.parentId},\tDict:{IdDict.Count}");
        return null;
    }

    public static RendererId GetId(IdInfo idI, bool showLog)
    {
        if (string.IsNullOrEmpty(idI.Id))
        {
            Debug.LogError($"RendererDictionay.GetId IsNullOrEmpty !!! ,Dict:{IdDict.Count} IdInfo:{idI.GetFullString()}");
            return null;
        }
        if (IdDict.ContainsKey(idI.Id))
        {
            var go = IdDict[idI.Id];
            if (go == null)
            {
                Debug.LogError($"RendererDictionay.GetId go == null :{idI.Id},Dict:{IdDict.Count}");
                return null;
            }
            return go;
        }
        if (showLog)
            Debug.LogError($"RendererDictionay.GetId go not found id:{idI.Id},\tDict:{IdDict.Count}");
        return null;
    }

    public static RendererId GetId(string id, bool showLog)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"RendererDictionay.GetId IsNullOrEmpty !!! ,Dict:{IdDict.Count}");
            return null;
        }
        if (IdDict.ContainsKey(id))
        {
            var go = IdDict[id];
            if (go == null)
            {
                Debug.LogError($"RendererDictionay.GetId go == null :{id},Dict:{IdDict.Count}");
                return null;
            }
            return go;
        }
        if (showLog)
            Debug.LogError($"RendererDictionay.GetId go not found id:{id},\tDict:{IdDict.Count}");
        return null;
    }

    public static RendererId GetId(string id, string name, bool showLog = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError($"RendererDictionay.GetGo IsNullOrEmpty !!! ,Dict:{IdDict.Count}");
            return null;
        }
        if (IdDict.ContainsKey(id))
        {
            var go = IdDict[id];
            if (go == null)
            {
                Debug.LogError($"RendererDictionay.GetGo go == null :{id},Dict:{IdDict.Count}");
                return null;
            }
            return go;
        }
        if (showLog)
            Debug.LogError($"RendererDictionay.GetGo go not found id:{id},\name:{name},\tDict:{IdDict.Count}");
        return null;
    }

    public static GameObject GetGoEx(string id)
    {
        if (!IdDict.ContainsKey(id))
        {
           InitIds();
        }
        return GetGo(id);
    }

    public static MeshRenderer GetRenderer(string id,bool isShowLog)
    {
        //if (!RendererDict.ContainsKey(id))
        //{
        //    InitInfos();
        //}

        if (RendererDict.ContainsKey(id))
        {
            var renderer = RendererDict[id];
            if (renderer == null)
            {
                Debug.LogError($"RendererDictionay.GetRenderer renderer == null :Dict:{RendererDict.Count} id:{id}");
            }
            return renderer;
        }
       
        if (isShowLog)
        {
            GameObject go = null;
            if (IdDict.ContainsKey(id))
            {
                go = IdDict[id].gameObject;
            }
            Debug.LogError($"RendererDictionay.GetRenderer not found Dict:{RendererDict.Count} go:{go} id:{id}");
        }
        return null;
    }

    internal static List<MeshRenderer> GetRenderers(List<string> renderersId,Transform root)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        int rCount = renderersId.Count;
        int foundCount = 0;
        int notFoundCount = 0;
        StringBuilder notFoundIds = new StringBuilder();
        for (int i = 0; i < renderersId.Count; i++)
        {
            string id = renderersId[i];
            var renderer = GetRenderer(id,false);
            if (renderer != null)
            {
                renderers.Add(renderer);
                foundCount++;
            }
            else
            {
                GameObject go = null;
                if (IdDict.ContainsKey(id))
                {
                    go = IdDict[id].gameObject;
                }
                //Debug.LogError($"RendererDictionay.GetRenderers[{i}] not found Dict:{RendererDict.Count} go:{go} id:{id}");
                notFoundCount++;
                if (notFoundCount < 5)
                {
                    notFoundIds.AppendLine(id);
                }
            }
        }
        if (notFoundCount > 0)
        {
            Debug.LogError($"RendererDictionay.GetRenderers renderersId:{renderersId.Count} foundCount:{foundCount} notFoundCount:{notFoundCount} root:{root.GetPath()} notFoundIds:{notFoundIds}");
        }
        
        return renderers;
    }

}

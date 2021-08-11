using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IdDictionary
{
    public static Dictionary<string, MeshRenderer> RendererDict = new Dictionary<string, MeshRenderer>();

    public static Dictionary<string, RendererId> IdDict = new Dictionary<string, RendererId>();

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

    public static void SetId(RendererId id)
    {
        //try
        //{

        //    if (!IdDict.ContainsKey(id.Id))
        //    {
        //        IdDict.Add(id.Id, id);
        //    }
        //    else
        //    {
        //        IdDict[id.Id] = id;//�ɵĿ��ܱ�ж�ء�ɾ����
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"RendererDictionay.SetId Renderer:{id},Exception:{ex}");
        //}

        SetRendererId(id, id.mr);
    }

    private static void SetRendererId(RendererId id,MeshRenderer renderer)
    {
        try
        {
            if (id == null)
            {
                Debug.LogError($"RendererDictionay.SetRendererId id == null");
            }
            else
            {
                if (renderer != null)
                {
                    if (!RendererDict.ContainsKey(id.Id))
                    {
                        RendererDict.Add(id.Id, renderer);
                    }
                    else
                    {
                        RendererDict[id.Id] = renderer;
                    }
                }

                if (!IdDict.ContainsKey(id.Id))
                {
                    IdDict.Add(id.Id, id);
                }
                else
                {
                    IdDict[id.Id] = id;//�ɵĿ��ܱ�ж�ء�ɾ����
                }
            }
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"RendererDictionay.SetRendererId Renderer:{id},Exception:{ex}");
        }
    }

    public static void InitInfos(GameObject root = null)
    {
        DateTime start = DateTime.Now;
        RendererDict.Clear();
        IdDict.Clear();
        InitRenderers(root);
        InitIds(root);
        Debug.Log($"IdDictionay.InitInfos idCount:{IdDict.Count},RendererCount:{RendererDict.Count} time:{(DateTime.Now - start)}");
    }

    public static void InitRenderers(GameObject root = null)
    {
        DateTime start = DateTime.Now;
        MeshRenderer[] renderers = null;
        if (root == null)
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        }
        else
        {
            renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        }
        InitRenderers(renderers);
        Debug.Log($"IdDictionay.InitRenderers count:{renderers.Length},Dict:{RendererDict.Count} time:{(DateTime.Now - start)}");
    }

    public static RendererId[] InitIds(GameObject root=null)
    {
        DateTime start = DateTime.Now;
        RendererId[] ids = null;
        if (root == null)
        {
            ids = GameObject.FindObjectsOfType<RendererId>(true);
        }
        else
        {
            ids = root.GetComponentsInChildren<RendererId>(true);
        }
        foreach (var id in ids)
        {
            SetId(id);
            //id.childrenIds.Clear();
        }
        Debug.Log($"IdDictionay.InitIds count:{ids.Length},Dict:{IdDict.Count} time:{(DateTime.Now - start)}");
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

    public static void InitRenderers(MeshRenderer[] renderers)
    {
        foreach (var renderer in renderers)
        {
            if(renderer==null)
            {
                Debug.LogError($"renderer==null");
                continue;
            }
            RendererId id = RendererId.GetId(renderer);
            SetRendererId(id, renderer);
        }
    }


    internal static void InitGos(IEnumerable<GameObject> objs,string tag)
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

    public static GameObject GetGo(string id,string goName="")
    {
        //if(string.IsNullOrEmpty(id)){
        //    Debug.LogError($"RendererDictionay.GetGo IsNullOrEmpty !!! ,Dict:{IdDict.Count}");
        //    return null;
        //}
        //if (IdDict.ContainsKey(id))
        //{
        //    var go = IdDict[id];
        //    if (go == null)
        //    {
        //        Debug.LogError($"RendererDictionay.GetGo go == null :{id},Dict:{IdDict.Count}");
        //        return null;
        //    }
        //    return go.gameObject;
        //}
        //Debug.LogError($"RendererDictionay.GetGo go not found id:{id},\tgoName:{goName},\tDict:{IdDict.Count}");
        //return null;
        RendererId rId = GetId(id, goName);
        if (rId != null)
        {
            return rId.gameObject;
        }
        else
        {
            return null;
        }
    }

    public static RendererId GetId(string id, string goName = "")
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
        Debug.LogError($"RendererDictionay.GetGo go not found id:{id},\tgoName:{goName},\tDict:{IdDict.Count}");
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

    public static MeshRenderer GetRenderer(string id)
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
        GameObject go = null;
        if (IdDict.ContainsKey(id))
        {
            go = IdDict[id].gameObject;
        }
        Debug.LogError($"RendererDictionay.GetRenderer not found Dict:{RendererDict.Count} go:{go} id:{id}");
        return null;
    }

    internal static List<MeshRenderer> GetRenderers(List<string> renderersId)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach(var id in renderersId)
        {
            var renderer = GetRenderer(id);
            if (renderer != null)
            {
                renderers.Add(renderer);
            }
        }
        return renderers;
    }

}

using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Base.Common;
using System.Text;

[Serializable]
public class ModelItemInfoDictionary
{
    [NonSerialized]
    public Dictionary<string, List<ModelItemInfo>> repeatModelsByRendererId = new Dictionary<string, List<ModelItemInfo>>();

    [NonSerialized]
    public Dictionary<string, List<ModelItemInfo>> repeatModelsByUId = new Dictionary<string, List<ModelItemInfo>>();

    [NonSerialized]
    public Dictionary<string, List<ModelItemInfo>> renderId2Model = new Dictionary<string, List<ModelItemInfo>>();

    [NonSerialized]
    public Dictionary<string, List<ModelItemInfo>> uid2Model = new Dictionary<string, List<ModelItemInfo>>();

    public int rendererIdCount = 0;

    public int UidCount = 0;

    public ModelItemInfoDictionary()
    {
    }

        public ModelItemInfoDictionary(List<ModelItemInfo> all, ProgressArgEx p0)
    {
        InitDict(all, p0);
        GetRepeatedModels();
    }

    public List<ModelItemInfo> all = new List<ModelItemInfo>();

    public int Count
    {
        get
        {
            return all.Count;
        }
    }

    private void InitDict(List<ModelItemInfo> all, ProgressArgEx p0)
    {
        this.all = all;
        renderId2Model = new Dictionary<string, List<ModelItemInfo>>();
        uid2Model = new Dictionary<string, List<ModelItemInfo>>();

        for (int i = 0; i < all.Count; i++)
        {
            ModelItemInfo item = all[i];

            var p1 = ProgressArg.New("ModelItemInfoDictionary", i, all.Count, item.Name, p0);
            //InitNavisFileInfoByModel.Instance.progressArg = p1;
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

            if (!string.IsNullOrEmpty(item.RenderId))
            {
                rendererIdCount++;

                if (!renderId2Model.ContainsKey(item.RenderId))
                {
                    renderId2Model.Add(item.RenderId, new List<ModelItemInfo>());
                }

                renderId2Model[item.RenderId].Add(item);
            }

            if (!string.IsNullOrEmpty(item.UId))
            {
                UidCount++;

                if (!uid2Model.ContainsKey(item.UId))
                {
                    uid2Model.Add(item.UId, new List<ModelItemInfo>());
                }

                uid2Model[item.UId].Add(item);
            }

            var pos = item.GetPosition();
            positionDictionaryList.Add(pos, item);
        }

        positionDictionaryList.ShowCount("ModelItemDictionary");

        if(p0==null)
            ProgressBarHelper.ClearProgressBar();
    }

    public ModelItemInfo GetModelByUID(string uid)
    {
        if (uid2Model.ContainsKey(uid))
        {
            var lst = uid2Model[uid];
            if (lst.Count == 0)
            {
                return lst[0];
            }
            else
            {
                return null;
            }
            
        }
        return null;
    }

    //public ModelItemInfo GetModelByRendererId()
    //{

    //}

    public PositionDictionaryList<ModelItemInfo> positionDictionaryList = new PositionDictionaryList<ModelItemInfo>();

    public void GetRepeatedModels()
    {
        repeatModelsByRendererId = new Dictionary<string, List<ModelItemInfo>>();
        foreach (var r in renderId2Model.Keys)
        {
            var ms = renderId2Model[r];
            if (ms.Count > 1)
            {
                //if (rendererId2Bim.ContainsKey(r))
                //{
                //    var bim = rendererId2Bim[r];
                //    Debug.LogError($"repeatModels rendererId:{r} list:{ms.Count} bim:{bim}");
                //}
                //else
                //{
                //    Debug.LogError($"repeatModels rendererId:{r} list:{ms.Count} bim:NULL");
                //}
                //repeatModels.AddRange(ms);
                repeatModelsByRendererId.Add(r, ms);
            }
        }

        repeatModelsByUId = new Dictionary<string, List<ModelItemInfo>>();
        foreach (var r in uid2Model.Keys)
        {
            var ms = uid2Model[r];
            if (ms.Count > 1)
            {
                //if (guid2Bim.ContainsKey(r))
                //{
                //    var bim = guid2Bim[r];
                //    Debug.LogError($"repeatModels UID:{r} list:{ms.Count} bim:{bim}");
                //}
                //else
                //{
                //    Debug.LogError($"repeatModels UID:{r} list:{ms.Count} bim:NULL");
                //}
                //repeatModels.AddRange(ms);
                repeatModelsByUId.Add(r, ms);
            }
        }
    }

    public void ClearRendererId()
    {
        int count = 0;
        for (int i = 0; i < all.Count; i++)
        {
            ModelItemInfo item = all[i];
            if(!string.IsNullOrEmpty(item.RenderId))
            {
                count++;
            }
            item.RenderId = null;
            item.RenderName = null;
            item.AreaName = null;
        }
        Debug.LogError($"ClearRendererId count:{count}");
    }

    public int RemoveRepeatedModelInfo(BIMModelInfoDictionary bimDict)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var rendererId in repeatModelsByRendererId.Keys)
        {
            var models = repeatModelsByRendererId[rendererId];
            var bim = bimDict.GetBIMModelByRendererId(rendererId);
            if (bim != null)
            {
                var closedModel = bim.FindClosedModel(models);
                bim.SetModelInfo(closedModel);
                models.Remove(closedModel);
            }
            for (int i = 0; i < models.Count; i++)
            {
                ModelItemInfo model = models[i];
                model.RenderId = "";
                model.RenderName = "";
                model.AreaName = "";
                sb.AppendLine($"id:{rendererId} model:{model.Name} uid:{model.UId}");

                if (i > 0 && i % 100 == 0)
                {
                    Debug.LogError($"RemoveRepeated: " + sb);
                    sb = new StringBuilder();
                }
            }
        }
        return repeatModelsByRendererId.Keys.Count;
        //SaveXml();
    }

    internal ModelItemInfo FindModelByPosAndName(Transform transform)
    {
        var ms = this.FindModelsByPos(transform);
        if (ms.Count == 1)
        {
            return ms[0];
        }
        else
        {
            int sameNameCount = 0;
            ModelItemInfo sameNameModel = null;
            foreach(var m in ms)
            {
                if (m.IsSameName(transform))
                {
                    sameNameModel = m;
                    sameNameCount++;
                }
            }
            if (sameNameCount == 1)
            {
                return sameNameModel;
            }
            else
            {
                return null;
            }
        }
    }

    internal List<ModelItemInfo> FindModelsByPosAndName(Transform transform)
    {
        var ms = this.FindModelsByPos(transform);
        if (ms.Count == 1)
        {
            return ms;
        }
        else
        {
            List<ModelItemInfo> sameNameModels = new List<ModelItemInfo>();
            foreach (var m in ms)
            {
                if (m.IsSameName(transform))
                {
                    sameNameModels.Add(m);
                }
            }
            if (sameNameModels.Count == 0)
            {
                return ms;
            }
            else if (sameNameModels.Count == 1)
            {
                return sameNameModels;
            }
            else
            {
                //return ms;
                return sameNameModels;
            }
        }
    }

    internal ModelItemInfo FindModelByPos(Transform t)
    {
        Vector3 pos = t.position;
        int listId = 0;
        var model = positionDictionaryList.GetItem(pos, out listId);

        //if (model != null)
        //{
        //    if (listId == 0)
        //    {

        //    }
        //    else if (listId == 1 || listId == 2)
        //    {
        //        Debug.Log($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else if (listId == 3 || listId == 4 || listId == 5)
        //    {
        //        Debug.LogWarning($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else
        //    {
        //        Debug.LogError($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //}
        return model;
    }

    internal List<ModelItemInfo> FindModelsByPos(Transform t)
    {
        Vector3 pos = t.position;
        int listId = 0;
        var models = positionDictionaryList.GetItems(pos, out listId);
        if (models == null)
        {
            models = new List<ModelItemInfo>();
        }
        //if (model != null)
        //{
        //    if (listId == 0)
        //    {

        //    }
        //    else if (listId == 1 || listId == 2)
        //    {
        //        Debug.Log($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else if (listId == 3 || listId == 4 || listId == 5)
        //    {
        //        Debug.LogWarning($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else
        //    {
        //        Debug.LogError($"[FindModelByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //}
        return models;
    }
}
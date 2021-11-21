using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Base.Common;
using System.Text;

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

        public ModelItemInfoDictionary(List<ModelItemInfo> all)
    {
        InitDict(all);
        GetRepeatedModels();
    }

    private void InitDict(List<ModelItemInfo> all)
    {
        renderId2Model = new Dictionary<string, List<ModelItemInfo>>();
        uid2Model = new Dictionary<string, List<ModelItemInfo>>();

        for (int i = 0; i < all.Count; i++)
        {
            ModelItemInfo item = all[i];

            ProgressArg p1 = new ProgressArg("LoadModels", i, all.Count, item.Name);
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
        }
    }

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

    public void RemoveRepeatedModelInfo(BIMModelInfoDictionary bimDict)
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
        //SaveXml();
    }
}
using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BIMModelInfoDictionary 
{
    public Dictionary<string, List<BIMModelInfo>> bimAreas = new Dictionary<string, List<BIMModelInfo>>();

    private Dictionary<string, BIMModelInfo> rendererId2Bim = new Dictionary<string, BIMModelInfo>();

    private Dictionary<string, BIMModelInfo> guid2Bim = new Dictionary<string, BIMModelInfo>();

    public List<BIMModelInfo> errorBims = new List<BIMModelInfo>();

    public List<BIMModelInfo> bimInfos = new List<BIMModelInfo>();

    public BIMModelInfoDictionary(BIMModelInfo[] bims)
    {
        InitDict(bims);
    }

    public BIMModelInfoDictionary()
    {

    }

    private void InitDict(BIMModelInfo[] bims)
    {
        bimInfos = new List<BIMModelInfo>();
        //bimInfos.AddRange(this.GetComponentsInChildren<BIMModelInfo>(true));
        bimInfos.AddRange(bims);
        bimInfos.Sort();

        errorBims.Clear();
        bimAreas.Clear();
        rendererId2Bim.Clear();
        guid2Bim.Clear();
        for (int i = 0; i < bimInfos.Count; i++)
        {
            BIMModelInfo bim = bimInfos[i];
            ProgressBarHelper.DisplayCancelableProgressBar("GetBims", i, bimInfos.Count, bim);

            var area = bim.GetArea();

            if (!string.IsNullOrEmpty(area))
            {
                if (!bimAreas.ContainsKey(area))
                {
                    bimAreas.Add(area, new List<BIMModelInfo>());
                }
                bimAreas[area].Add(bim);
            }

            if (!string.IsNullOrEmpty(bim.RenderId))
            {
                if (!rendererId2Bim.ContainsKey(bim.RenderId))
                {
                    rendererId2Bim.Add(bim.RenderId, bim);
                }
                else
                {
                    Debug.LogError($"GetBims[{i}/{bimInfos.Count}] rendererId2Bim.ContainsKey(bim.RenderId) bim:{bim} rendererID:{bim.RenderId}");
                }
            }
            else
            {
                Debug.LogError($"GetBims[{i}/{bimInfos.Count}] string.IsNullOrEmpty(bim.RenderId) bim:{bim}");
            }

            if (!string.IsNullOrEmpty(bim.Guid))
            {
                if (!guid2Bim.ContainsKey(bim.Guid))
                {
                    guid2Bim.Add(bim.Guid, bim);
                }
                else
                {
                    Debug.LogError($"GetBims[{i}/{bimInfos.Count}] guid2Bim.ContainsKey(bim.Guid) bim:{bim} UID:{bim.Guid} Name:{bim.name} MName:{bim.MName} Distance:{bim.Distance}");
                    errorBims.Add(bim);
                }
            }
            else
            {
                Debug.LogError($"GetBims[{i}/{bimInfos.Count}] string.IsNullOrEmpty(bim.RenderId) bim:{bim}");
            }

            //ModelItemInfo model= file.GetModelBy
        }
    }

    public void CheckDict(List<ModelItemInfo> models)
    {
        var bimsList2 = new List<BIMModelInfo>(bimInfos);
        //var models = file.GetAllItems();
        int foundCount = 0;

        foreach (var model in models)
        {
            //if (string.IsNullOrEmpty(model.RenderId)) continue;

            var bim = GetBIMModel(model);
            if (bim != null)
            {
                bim.Model = model;
                //bim.SetModelInfo(model);

                bimsList2.Remove(bim);

                foundCount++;
            }
            else
            {

            }
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bimsList2.Count; i++)
        {
            BIMModelInfo bim = bimsList2[i];
            //Debug.LogError($"Not Found Model[{i}/{bimsList2.Count}]:{bim}");

            sb.AppendLine($"Not Found Model[{i}/{bimsList2.Count}]:{bim}");
        }

        Debug.LogError($"GetBims bimCount:{bimInfos.Count} rendererId2Bim:{rendererId2Bim.Count} guid2Bim:{guid2Bim.Count}  foundCount:{foundCount} notFoundCount:{bimsList2.Count}\n{sb}");

        ProgressBarHelper.ClearProgressBar();

        foreach (string key in bimAreas.Keys)
        {
            Debug.LogError($"BimAreas area:{key} bims:{bimAreas[key].Count}");
        }

        Debug.LogError($"GetBims infos:{bimInfos.Count}");
    }

    public BIMModelInfo GetBIMModelByRendererId(string r)
    {
        //if (bimInfos.Count == 0)
        //{
        //    GetBims(null);
        //}
        if (rendererId2Bim.ContainsKey(r))
        {
            var bim = rendererId2Bim[r];
            return bim;
        }
        else
        {
            return null;
        }
    }

    public BIMModelInfo GetBIMModel(ModelItemInfo model)
    {
        if (model == null) return null;
        if (!string.IsNullOrEmpty(model.UId) && guid2Bim.ContainsKey(model.UId))
        {
            var bim = guid2Bim[model.UId];
            return bim;
        }
        if (!string.IsNullOrEmpty(model.RenderId) && rendererId2Bim.ContainsKey(model.RenderId))
        {
            var bim = rendererId2Bim[model.RenderId];
            return bim;
        }

        return null;
    }
}

using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class BIMModelInfoDictionary 
{
    [NonSerialized]
    public Dictionary<string, List<BIMModelInfo>> bimAreas = new Dictionary<string, List<BIMModelInfo>>();

    [NonSerialized]
    private Dictionary<string, BIMModelInfo> rendererId2Bim = new Dictionary<string, BIMModelInfo>();
    [NonSerialized]
    private Dictionary<string, BIMModelInfo> guid2Bim = new Dictionary<string, BIMModelInfo>();

    public BIMModelInfoList errorBims = new BIMModelInfoList();

    public BIMModelInfoList bimInfos = new BIMModelInfoList();

    public BIMModelInfoDictionary()
    {

    }

    public BIMModelInfoDictionary(BIMModelInfo[] bims, ProgressArgEx p0)
    {
        InitDict(bims, p0);
    }

    public BIMModelInfoDictionary(List<BIMModelInfo> bims, ProgressArgEx p0)
    {
        InitDict(bims.ToArray(), p0);
    }

    private void InitDict(BIMModelInfo[] bims,ProgressArgEx p0)
    {
        bimInfos = new BIMModelInfoList();
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
            var p1 = ProgressArg.New("GetBims", i, bimInfos.Count, bim, p0);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

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
                    //Debug.LogError($"GetBims[{i}/{bimInfos.Count}] rendererId2Bim.ContainsKey(bim.RenderId) bim:{bim} rendererID:{bim.RenderId}");
                }
            }
            else
            {
                //Debug.LogError($"GetBims[{i}/{bimInfos.Count}] string.IsNullOrEmpty(bim.RenderId) bim:{bim}");
            }

            if (!string.IsNullOrEmpty(bim.Guid))
            {
                if (!guid2Bim.ContainsKey(bim.Guid))
                {
                    guid2Bim.Add(bim.Guid, bim);
                }
                else
                {
                    //var bim2 = guid2Bim[bim.Guid];
                    //Debug.LogError($"GetBims[{i}/{bimInfos.Count}] guid2Bim.ContainsKey(bim.Guid) id:{bim.Guid} bimNew:??{bim}?? bimOld:??{bim2}??");
                    errorBims.Add(bim);
                }
            }
            else
            {
                //Debug.LogError($"GetBims[{i}/{bimInfos.Count}] string.IsNullOrEmpty(bim.RenderId) bim:{bim}");
                errorBims.Add(bim);
            }

            //ModelItemInfo model= file.GetModelBy
        }

        StringBuilder errorInfo = new StringBuilder();
        for (int i = 0; i < errorBims.Count; i++)
        {
            BIMModelInfo bim = errorBims[i];
            if (string.IsNullOrEmpty(bim.Guid))
            {
                errorInfo.AppendLine($"GetBims[{i}/{errorBims.Count}] string.IsNullOrEmpty(bim.RenderId) bim:{bim}");
            }
            else
            {
                var bim2 = guid2Bim[bim.Guid];
                errorInfo.AppendLine($"GetBims[{i}/{errorBims.Count}] guid2Bim.ContainsKey(bim.Guid) id:{bim.Guid} bimNew:??{bim}?? bimOld:??{bim2}??");
            }
        }
        string errorInfoTxt = errorInfo.ToString();
        if (!string.IsNullOrEmpty(errorInfoTxt))
        {
            Debug.LogError(errorInfoTxt);
        }
        else
        {
            Debug.Log("GetBims No Error");
        }

        //foreach(var rendererId in rendererId2Bim.Keys)
        //{
        //    var list = rendererId2Bim[rendererId];
        //    if(list.)
        //}
    }

    public void CheckDict(List<ModelItemInfo> checkModelList)
    {
        var bimsList2 = new List<BIMModelInfo>(bimInfos);//SubModelList In A Building
        //var models = file.GetAllItems();
        int foundCount = 0;

        if(checkModelList!=null)
            foreach (var model in checkModelList)
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

        Debug.LogWarning($"[CheckDict] GetBims bimCount:{bimInfos.Count} rendererId2Bim:{rendererId2Bim.Count} guid2Bim:{guid2Bim.Count}  foundCount:{foundCount} notFoundCount:{bimsList2.Count}\n{sb}");

        //ProgressBarHelper.ClearProgressBar();
        StringBuilder txt = new StringBuilder();
        foreach (string key in bimAreas.Keys)
        {
            txt.AppendLine($"area:{key} bims:{bimAreas[key].Count}");
        }
        Debug.Log($"[CheckDict] BimAreas({bimAreas.Keys.Count}) :\n{txt}");

        //Debug.LogError($"[CheckDict] GetBims infos:{bimInfos.Count}");
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
        BIMModelInfo result = null;
        if (model == null) return result;
        result = GetBIMModelByGuid(model.UId);
        if (result != null) return result;
        if (!string.IsNullOrEmpty(model.RenderId) && rendererId2Bim.ContainsKey(model.RenderId))
        {
            var bim = rendererId2Bim[model.RenderId];
            return bim;
        }
        return null;
    }

    public BIMModelInfo GetBIMModelByGuid(string guid)
    {
        if (!string.IsNullOrEmpty(guid) && guid2Bim.ContainsKey(guid))
        {
            var bim = guid2Bim[guid];
            return bim;
        }
        return null;
    }
}

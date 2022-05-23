using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelComponent : BaseMeshModel
{
    public virtual string GetDictKey()
    {
        return "";
    }

    public virtual string GetSortKey()
    {
        return "";
    }

    public virtual GameObject CreateModelByPrefab()
    {
        return null;
    }
    public virtual GameObject CreateModelByPrefabMesh()
    {
        return null;
    }

    public PipeGenerateArg generateArg = new PipeGenerateArg();





    public virtual void InitSaveData(MeshModelSaveData data)
    {
        //data.Name = this.name;
        //data.Id = RendererId.GetId(this.gameObject);
        ////data.Path = GetPath();
        //data.PId = RendererId.GetId(this.gameObject.transform.parent);
        ////data.Transform = new TransformInfo(this.transform);
        //data.GetTransformInfo(this.transform);
        ////this.LineInfo = data.Info;

        ////data.PipeWelds = GetWeldsSaveData();

        //data.Init(this.gameObject);
        if (this.ResultGo == null)
        {
            Debug.LogWarning($"GetWeldsSaveData this.ResultGo == null type:{this.GetType()} gameObject:{this.gameObject}");
            return;
        }
        //data.Init(this.ResultGo);
        InitData(data, this.ResultGo);
        //data.InitPrefabInfo(this.ResultGo);
        InitPrefabInfo(data, this.ResultGo);
    }

    public void InitData(MeshModelSaveData data,GameObject go)
    {
        if (go == null)
        {
            Debug.LogError($"MeshModelSaveData.Init go==null");
        }
        data.Name = go.name;
        data.Id = InnerRendererId.GetId(go);
        //data.Path = GetPath();
        data.PId = InnerRendererId.GetId(go.transform.parent);
        //data.Transform = new TransformInfo(this.transform);
        data.GetTransformInfo(go.transform);
    }

    public void InitPrefabInfo(MeshModelSaveData data, GameObject go)
    {
        if (go == null)
        {
            Debug.LogError($"MeshModelSaveData.InitPrefabInfo go==null Name:{data.Name}");
            return;
        }
        MeshPrefabInstance ins = go.GetComponent<MeshPrefabInstance>();
        if (ins == null)
        {
            Debug.LogError($"MeshModelSaveData.InitPrefabInfo ins==null go:{go}");
        }
        else
        {
            if (ins.PrefabGo == null)
            {
                Debug.LogError($"MeshModelSaveData.InitPrefabInfo ins.PrefabGo==null go:{go}");
                return;
            }
            data.prefabId = InnerRendererId.GetId(ins.PrefabGo);
            if (string.IsNullOrEmpty(data.prefabId))
            {
                Debug.LogError($"MeshModelSaveData.InitPrefabInfo string.IsNullOrEmpty(prefabId) go:{go}");
                return;
            }
            data.isPrefab = ins.IsPrefab;
        }
    }

    //public List<PipeWeldSaveData> GetWeldsSaveData()
    //{
    //    List<PipeWeldSaveData> welds = new List<PipeWeldSaveData>();

    //    if (IsGetInfoSuccess == false)
    //    {
    //        Debug.LogWarning($"GetWeldsSaveData IsGetInfoSuccess == false gameObject:{this.gameObject}");
    //        return null;
    //    }
    //    if (ResultGo==null)
    //    {
    //        Debug.LogError($"GetWeldsSaveData ResultGo==null gameObject:{this.gameObject}");
    //        return null;
    //    }
    //    PipeWeldModel[] weldModels = ResultGo.GetComponentsInChildren<PipeWeldModel>();
    //    foreach (var m in weldModels)
    //    {
    //        welds.Add(m.GetSaveData());
    //    }
    //    if (welds.Count == 0)
    //    {
    //        welds = null;
    //    }
    //    return welds;
    //}

    public virtual List<PipeWeldSaveData> GetWeldsSaveData()
    {
        List<PipeWeldSaveData> welds = new List<PipeWeldSaveData>();
        return welds;
    }

    //public virtual void GetModelInfo()
    //{

    //}

    //public virtual void GetModelInfo_Job()
    //{

    //}


    public override bool RendererModel()
    {
        //RendererModel(GetArg(), "_New");
        GameObject go = RendererModel(generateArg, "_New");
        return go != null;
    }

    //public PipeGenerateArg GetArg()
    //{
    //    MeshRenderer r = this.GetComponent<MeshRenderer>();
    //    MeshFilter mf = this.GetComponent<MeshFilter>();
    //    if (r)
    //    {
    //        if (generateArg.pipeMaterial == null)
    //        {
    //            generateArg.pipeMaterial = r.sharedMaterial;
    //        }
    //        if (generateArg.weldMaterial == null)
    //        {
    //            generateArg.weldMaterial = r.sharedMaterial;
    //        }
    //    }
    //    else
    //    {
    //        if (generateArg.pipeMaterial == null)
    //        {
    //            generateArg.pipeMaterial = PipeFactory.Instance.generateArg.pipeMaterial;
    //        }
    //        if (generateArg.weldMaterial == null)
    //        {
    //            generateArg.weldMaterial = PipeFactory.Instance.generateArg.weldMaterial;
    //        }
    //    }

    //    if (generateArg.pipeMaterial == null)
    //    {
    //        generateArg.pipeMaterial = PipeFactory.Instance.generateArg.pipeMaterial;
    //    }
    //    if (generateArg.weldMaterial == null)
    //    {
    //        generateArg.weldMaterial = PipeFactory.Instance.generateArg.weldMaterial;
    //    }

    //    return generateArg;
    //}


    public virtual GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return null;
    }
}

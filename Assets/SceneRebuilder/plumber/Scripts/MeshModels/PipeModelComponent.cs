using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelComponent : BaseMeshModel
{


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
        data.Init(this.ResultGo);
        data.InitPrefabInfo(this.ResultGo);
    }

    public List<PipeWeldSaveData> GetWeldsSaveData()
    {
        List<PipeWeldSaveData> welds = new List<PipeWeldSaveData>();

        if (IsGetInfoSuccess == false)
        {
            Debug.LogWarning($"GetWeldsSaveData IsGetInfoSuccess == false gameObject:{this.gameObject}");
            return null;
        }
        if (ResultGo==null)
        {
            Debug.LogError($"GetWeldsSaveData ResultGo==null gameObject:{this.gameObject}");
            return null;
        }
        PipeWeldModel[] weldModels = ResultGo.GetComponentsInChildren<PipeWeldModel>();
        foreach (var m in weldModels)
        {
            welds.Add(m.GetSaveData());
        }
        if (welds.Count == 0)
        {
            welds = null;
        }
        return welds;
    }

    //public virtual void GetModelInfo()
    //{

    //}

    //public virtual void GetModelInfo_Job()
    //{

    //}


    public virtual void RendererModel()
    {
        MeshRenderer r = this.GetComponent<MeshRenderer>();
        if (r)
        {
            if (generateArg.pipeMaterial == null)
            {
                generateArg.pipeMaterial = r.sharedMaterial;
            }
            if (generateArg.weldMaterial == null)
            {
                generateArg.weldMaterial = r.sharedMaterial;
            }
        }
        else
        {
            if (generateArg.pipeMaterial == null)
            {
                generateArg.pipeMaterial = PipeFactory.Instance.generateArg.pipeMaterial;
            }
            if (generateArg.weldMaterial == null)
            {
                generateArg.weldMaterial = PipeFactory.Instance.generateArg.weldMaterial;
            }
        }

        if (generateArg.pipeMaterial == null)
        {
            generateArg.pipeMaterial = PipeFactory.Instance.generateArg.pipeMaterial;
        }
        if (generateArg.weldMaterial == null)
        {
            generateArg.weldMaterial = PipeFactory.Instance.generateArg.weldMaterial;
        }

        RendererModel(generateArg, "_New");
    }


    public virtual GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return null;
    }
}

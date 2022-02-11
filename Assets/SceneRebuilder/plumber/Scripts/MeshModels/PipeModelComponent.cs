using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelComponent : MonoBehaviour
{
    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public GameObject ResultGo = null;

    [ContextMenu("ClearGo")]
    public void ClearGo()
    {
        if (ResultGo != null)
        {
            if (ResultGo != this.gameObject)
            {
                GameObject.DestroyImmediate(ResultGo);
            }
            else
            {

                //DestroyMeshComponent();
                PipeMeshGeneratorBase generators = ResultGo.GetComponent<PipeMeshGeneratorBase>();
                if (generators != null)
                {
                    generators.ClearResult();
                    GameObject.DestroyImmediate(generators);
                }
                //ResultGo = null;
            }
            ResultGo = null;
        }
    }

    public virtual void InitSaveData(PipeModelSaveData data)
    {
        data.Name = this.name;
        data.Id = RendererId.GetId(this.gameObject);
        //data.Path = GetPath();
        data.PId = RendererId.GetId(this.gameObject.transform.parent);
        //data.Transform = new TransformInfo(this.transform);
        data.GetTransformInfo(this.transform);
        //this.LineInfo = data.Info;

        //data.PipeWelds = GetWeldsSaveData();
    }

    public List<PipeWeldSaveData> GetWeldsSaveData()
    {
        List<PipeWeldSaveData> welds = new List<PipeWeldSaveData>();
        if (ResultGo==null)
        {
            Debug.LogError($"GetWeldsSaveData ResultGo==null gameObject:{this.gameObject}");
            return welds;
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

    public virtual void GetModelInfo()
    {

    }


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

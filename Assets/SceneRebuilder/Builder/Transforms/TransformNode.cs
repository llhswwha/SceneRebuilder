using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformNode : MonoBehaviour
{
    public TransformData Data;
    //void Start()
    //{
    //    Init();
    //}

    [ContextMenu("Init")]
    public void Init()
    {
        Data = new TransformData(this.transform);
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        if(Data==null || Data.transform==null)
        {
            Data = new TransformData(this.transform);
        }
        Data.Reset();
    }

    [ContextMenu("Recover")]
    public void Recover()
    {
        if (Data == null || Data.transform == null)
        {
            Data = new TransformData(this.transform);
        }
        Data.Recover();
    }

    [ContextMenu("SplitByMaterials")]
    public void SplitByMaterials()
    {
        MeshCombineHelper.SplitByMaterials(this.gameObject);
    }
}

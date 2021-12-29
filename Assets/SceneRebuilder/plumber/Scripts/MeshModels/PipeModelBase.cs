using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelBase : MonoBehaviour,IComparable<PipeModelBase>
{
    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public float PipeRadius = 0;

    public float PipeRadius1 = 0;
    public float PipeRadius2 = 0;

    public bool IsObbSuccess = true;

    public int VertexCount = 0;

    public int CompareTo(PipeModelBase other)
    {
        if (this.IsObbSuccess == false) return -1;
        if (other.IsObbSuccess == false) return 1;

        if (other.PipeRadius == 0) return 1;
        if (this.PipeRadius == 0) return -1;

        int r = other.PipeRadius.ToString("F5").CompareTo(this.PipeRadius.ToString("F5"));
        if (r == 0)
        {
            r = this.GetType().Name.CompareTo(other.GetType().Name);
        }
        if (r == 0)
        {
            r = other.VertexCount.CompareTo(this.VertexCount);
        }
        return r;
    }

    public override string ToString()
    {
        return $"type:{this.GetType().Name} radius:{PipeRadius:F5}| v:{VertexCount}|obb:{IsObbSuccess}";
    }

    public virtual void GetModelInfo()
    {

    }

    public virtual GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return null;
    }


    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
    }
}

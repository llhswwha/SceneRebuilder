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

    public bool IsGetInfoSuccess = true;

    public int VertexCount = 0;

    public float ObbDistance = 0;

    public float MeshDistance = 0;

    public float SizeDistance = 0;

    public float RTDistance = 0;

    private string GetCompareString()
    {
        string a = "1";
        if (IsGetInfoSuccess)
        {
            a = "0";
        }
        else
        {
            a = "1";
        }
        string radius = PipeRadius.ToString("F5");
        if (PipeRadius == 0)
        {
            radius = "9.99999";
        }
        float dis = SizeDistance+ObbDistance + MeshDistance;

        //return $"{ObbDistance:F5}_{MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
        return $"{SizeDistance:F5}_{dis:F5}_{ObbDistance:F5}_{RTDistance:F5}_{MeshDistance:F5}_{a}_{radius:00000}_{this.GetType().Name}_{this.VertexCount}";
    }

    public int CompareTo(PipeModelBase other)
    {
        return other.GetCompareString().CompareTo(this.GetCompareString());
        //if (this.IsGetInfoSuccess == false) return -1;
        //if (other.IsGetInfoSuccess == false) return 1;

        //if (other.PipeRadius == 0) return 1;
        //if (this.PipeRadius == 0) return -1;

        //int r = other.PipeRadius.ToString("F5").CompareTo(this.PipeRadius.ToString("F5"));
        //if (r == 0)
        //{
        //    r = this.GetType().Name.CompareTo(other.GetType().Name);
        //}
        //if (r == 0)
        //{
        //    r = other.VertexCount.CompareTo(this.VertexCount);
        //}
        //return r;
    }

    public override string ToString()
    {
        //return $"T:[{this.GetType().Name}] [Size:{SizeDistance:F5} Mesh:{MeshDistance:F5} Obb:{ObbDistance:F5} ObbRT:{RTDistance:F5}] R:{PipeRadius:F5}| V:{VertexCount:00000}|{IsGetInfoSuccess} ";
        return GetCompareString();
    }

    public virtual void GetModelInfo()
    {

    }

    public virtual GameObject RendererModel(PipeGenerateArg arg, string afterName)
    {
        return null;
    }

    public void CheckResult()
    {
        this.ObbDistance = OBBCollider.GetObbDistance(ResultGo,this.gameObject);
        
        this.MeshDistance = MeshHelper.GetVertexDistanceEx(ResultGo,this.gameObject);
        this.SizeDistance = MeshHelper.GetSizeDistance(ResultGo, this.gameObject);
        this.RTDistance= OBBCollider.GetObbRTDistance(ResultGo, this.gameObject);

        if (SizeDistance > 0)
        {
            IsGetInfoSuccess = false;
        }
    }

    public void ClearCheckDistance()
    {
        ObbDistance = 0;

        MeshDistance = 0;

        SizeDistance = 0;

        RTDistance = 0;
    }

    //public void GetSizeDistance()
    //{

    //}


    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
        ClearCheckDistance();
    }

    public T GetGenerator<T>(PipeGenerateArg arg, string afterName) where T : PipeMeshGeneratorBase
    {
        GameObject pipeNew = new GameObject(this.name + afterName);
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        T pipe = pipeNew.GetComponent<T>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<T>();
        }
        pipe.Target = this.gameObject;
        ResultGo = pipe.gameObject;
        return pipe;
    }

    public GameObject ResultGo = null;
}

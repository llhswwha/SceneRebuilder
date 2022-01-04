using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelBase : MonoBehaviour,IComparable<PipeModelBase>
{
    public List<PipeModelBase> ConnectedModels = new List<PipeModelBase>();

    public void AddConnectedModel(PipeModelBase other)
    {
        if (!ConnectedModels.Contains(other))
        {
            ConnectedModels.Add(other);
        }
        if (ConnectedModels.Count > 2)
        {
            if (this.GetType() != typeof(PipeTeeModel))
            {
                Debug.LogError($"AddConnectedModel ConnectedModels.Count > 3 model:{this}");
            }
        }
    }

    public static int IsConnectedEx(PipeElbowModel model1, PipeModelBase model2, float minPointDis,bool isUniformRaidus,float minRadiusDis=0.0001f)
    {
        int ConnectedCount = 0;
        float dis11 = Vector3.Distance(model1.GetEndPointIn1(), model2.GetModelStartPoint());
        float dis12 = Vector3.Distance(model1.GetEndPointIn1(), model2.GetModelEndPoint());
        float dis21 = Vector3.Distance(model1.GetEndPointIn2(), model2.GetModelStartPoint());
        float dis22 = Vector3.Distance(model1.GetEndPointIn2(), model2.GetModelEndPoint());

        //Debug.LogError($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");

        if (dis11 < minPointDis && dis12 < minPointDis)
        {
            //Error
            Debug.LogError($"IsConnected Error1 dis11 < minDis && dis12 < minDis dis11:{dis11} dis12:{dis12} minDis:{minPointDis}");
            return -1;
        }
        if (dis11 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelStartPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelStartPoint.w} radius2:{model2.ModelStartPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelStartPoint.w = model2.ModelStartPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
                
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);

           
            ConnectedCount++;
        }
        if (dis12 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelEndPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelStartPoint.w} radius2:{model2.ModelEndPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelStartPoint.w = model2.ModelEndPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        if (dis21 < minPointDis && dis22 < minPointDis)
        {
            //Error
            Debug.LogError($"IsConnected Error2 dis21 < minDis && dis22 < minDis dis21:{dis21} dis22:{dis22} minDis:{minPointDis}");
            return -1;
        }
        if (dis21 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelStartPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelEndPoint.w} radius2:{model2.ModelStartPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelEndPoint.w = model2.ModelStartPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }
        if (dis22 < minPointDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelEndPoint.w) > minRadiusDis)
            {
                Debug.LogWarning($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} radius1:{model1.ModelEndPoint.w} radius2:{model2.ModelEndPoint.w} ");
                if (isUniformRaidus)
                {
                    model1.ModelEndPoint.w = model2.ModelEndPoint.w;
                    model1.PipeRadius = model2.PipeRadius;
                }
                else
                {
                    model1.generateArg.generateEndCaps = true;
                }
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        return ConnectedCount;
    }


    public static int IsConnected(PipeModelBase model1, PipeModelBase model2, float minDis)
    {
        int ConnectedCount = 0;
        float dis11 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelStartPoint());
        float dis12 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelEndPoint());
        float dis21 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelStartPoint());
        float dis22 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelEndPoint());
        //Debug.LogError($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");

        if (dis11 < minDis && dis12 < minDis)
        {
            //Error
            Debug.LogError($"IsConnected Error1 dis11 < minDis && dis12 < minDis dis11:{dis11} dis12:{dis12} minDis:{minDis}");
            return -1;
        }
        if (dis11 < minDis)
        {
            if(Mathf.Abs(model1.ModelStartPoint.w- model2.ModelStartPoint.w)>0.00001)
            {
                Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }
        if (dis12 < minDis)
        {
            if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelEndPoint.w) > 0.00001)
            {
                Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        if (dis21 < minDis && dis22 < minDis)
        {
            //Error
            Debug.LogError($"IsConnected Error2 dis21 < minDis && dis22 < minDis dis21:{dis21} dis22:{dis22} minDis:{minDis}");
            return -1;
        }
        if (dis21 < minDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelStartPoint.w) > 0.00001)
            {
                Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }
        if (dis22 < minDis)
        {
            if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelEndPoint.w) > 0.00001)
            {
                Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
            }
            model1.AddConnectedModel(model2);
            model2.AddConnectedModel(model1);
            ConnectedCount++;
        }

        return ConnectedCount;
    }

    public Vector4 ModelStartPoint;

    public Vector3 GetModelStartPoint()
    {
        return transform.TransformPoint(ModelStartPoint);
    }

    public Vector4 ModelEndPoint;

    public Vector3 GetModelEndPoint()
    {
        return transform.TransformPoint(ModelEndPoint);
    }

    public List<Vector4> ModelKeyPoints = new List<Vector4>();

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
        GameObject pipeNew = GetPipeNewGo(arg, afterName);

        T pipe = pipeNew.GetComponent<T>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<T>();
        }
        pipe.Target = this.gameObject;
        ResultGo = pipe.gameObject;
        return pipe;
    }

    public GameObject GetPipeNewGo(PipeGenerateArg arg, string afterName)
    {
        GameObject pipeNew = new GameObject(this.name + afterName);
        pipeNew.transform.position = this.transform.position + arg.Offset;
        pipeNew.transform.SetParent(this.transform.parent);
        return pipeNew;
    }

    public GameObject ResultGo = null;
}

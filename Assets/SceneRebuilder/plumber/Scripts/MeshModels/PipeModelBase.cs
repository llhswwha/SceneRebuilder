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

    public virtual bool IsConnected(Vector3 keyPoint)
    {
        return false;
    }

    public virtual void SetUniformRadius(bool isUniformRaidus, Vector4 p2, int p1Id)
    {
        //if (isUniformRaidus)
        //{
        //    //p1.w = p2.w;//这个怎么保存回去呢？
        //    this.SetRadius(p1Id, p2.w);
        //    //this.PipeRadius = model2.PipeRadius;
        //    this.generateArg.generateEndCaps = false;
        //}
        //else
        //{
        //    this.generateArg.generateEndCaps = true;
        //}
    }

    public virtual int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog, bool isUniformRaidus, float minRadiusDis)
    {
        PipeModelBase model1 = this;
        int ConnectedCount = 0;
        var points1 = model1.GetModelKeyPoints();
        var points2 = model2.GetModelKeyPoints();
        for (int i = 0; i < points1.Count; i++)
        {
            Vector4 p1 = points1[i];
            int cCount = 0;
            for (int i1 = 0; i1 < points2.Count; i1++)
            {
                Vector4 p2 = points2[i1];
                float dis12= Vector3.Distance(p1, p2);

                if (isShowLog)
                {
                    Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1:{p1} p2:{p2}");
                }

                if (dis12 < minPointDis)
                {
                    if (Mathf.Abs(p1.w - p2.w) > minRadiusDis)
                    {
                        Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} [{i},{i1}]  dis:{dis12} p1:{p1} p2:{p2}");
                    }
                    model1.AddConnectedModel(model2);
                    model2.AddConnectedModel(model1);
                    cCount++;
                    SetUniformRadius(isUniformRaidus, p2, i);

                }
            }
            if(cCount== points2.Count)
            {
                Debug.LogError($"IsConnected Error1 cCount== points2.Count model1:{model1.name} model2:{model2.name} p1:{p1} points2:{points2.Count} cCount:{cCount}");
                //return -1;
            }
            ConnectedCount += cCount;
        }

        //float dis11 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelStartPoint());
        //float dis12 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelEndPoint());
        //float dis21 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelStartPoint());
        //float dis22 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelEndPoint());
        //if (isShowLog)
        //{
        //    Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
        //}

        return ConnectedCount;
    }

    //public virtual int ConnectedModel(PipeModelBase model2, float minPointDis, bool isShowLog)
    //{
    //    PipeModelBase model1 = this;
    //    int ConnectedCount = 0;
    //    float dis11 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelStartPoint());
    //    float dis12 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelEndPoint());
    //    float dis21 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelStartPoint());
    //    float dis22 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelEndPoint());
    //    if (isShowLog)
    //    {
    //        Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //    }


    //    if (dis11 < minPointDis && dis12 < minPointDis)
    //    {
    //        //Error
    //        Debug.LogError($"IsConnected Error1 dis11 < minDis && dis12 < minDis dis11:{dis11} dis12:{dis12} minDis:{minPointDis}");
    //        return -1;
    //    }
    //    if (dis11 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelStartPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }
    //    if (dis12 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelEndPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }

    //    if (dis21 < minPointDis && dis22 < minPointDis)
    //    {
    //        //Error
    //        Debug.LogError($"IsConnected Error2 dis21 < minDis && dis22 < minDis dis21:{dis21} dis22:{dis22} minDis:{minPointDis}");
    //        return -1;
    //    }
    //    if (dis21 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelStartPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }
    //    if (dis22 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelEndPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }

    //    return ConnectedCount;
    //}

    public static int ConnectedModelEx(PipeElbowModel model1, PipeModelBase model2, float minPointDis, bool isUniformRaidus, float minRadiusDis = 0.0001f)
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


    //public static int ConnectedModel(PipeModelBase model1, PipeModelBase model2, float minPointDis,bool isShowLog)
    //{
    //    int ConnectedCount = 0;
    //    float dis11 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelStartPoint());
    //    float dis12 = Vector3.Distance(model1.GetModelStartPoint(), model2.GetModelEndPoint());
    //    float dis21 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelStartPoint());
    //    float dis22 = Vector3.Distance(model1.GetModelEndPoint(), model2.GetModelEndPoint());
    //    if (isShowLog)
    //    {
    //        Debug.Log($"IsConnected model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //    }


    //    if (dis11 < minPointDis && dis12 < minPointDis)
    //    {
    //        //Error
    //        Debug.LogError($"IsConnected Error1 dis11 < minDis && dis12 < minDis dis11:{dis11} dis12:{dis12} minDis:{minPointDis}");
    //        return -1;
    //    }
    //    if (dis11 < minPointDis)
    //    {
    //        if(Mathf.Abs(model1.ModelStartPoint.w- model2.ModelStartPoint.w)>0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }
    //    if (dis12 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelStartPoint.w - model2.ModelEndPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }

    //    if (dis21 < minPointDis && dis22 < minPointDis)
    //    {
    //        //Error
    //        Debug.LogError($"IsConnected Error2 dis21 < minDis && dis22 < minDis dis21:{dis21} dis22:{dis22} minDis:{minPointDis}");
    //        return -1;
    //    }
    //    if (dis21 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelStartPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }
    //    if (dis22 < minPointDis)
    //    {
    //        if (Mathf.Abs(model1.ModelEndPoint.w - model2.ModelEndPoint.w) > 0.00001)
    //        {
    //            Debug.LogError($"Radius IsNot Equal model1:{model1.name} model2:{model2.name} dis11:{dis11} dis12:{dis12} dis21:{dis21} dis22:{dis22}");
    //        }
    //        model1.AddConnectedModel(model2);
    //        model2.AddConnectedModel(model1);
    //        ConnectedCount++;
    //    }

    //    return ConnectedCount;
    //}

    public Vector4 ModelStartPoint;

    public Vector4 TransformPoint(Vector4 p1)
    {
        Vector4 p = transform.TransformPoint(p1);
        p.w = p1.w;
        return p;
    }

    public Vector4 GetModelStartPoint()
    {
        Vector4 p= this.TransformPoint(ModelStartPoint);
        return p;
    }

    public Vector4 ModelEndPoint;

    public Vector4 GetModelEndPoint()
    {
        Vector4 p = this.TransformPoint(ModelEndPoint);
        return p;
    }

    public virtual List<Vector4> GetModelKeyPoints()
    {
        List<Vector4> list = new List<Vector4>();
        list.Add(GetModelStartPoint());
        list.Add(GetModelEndPoint());
        return list;
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

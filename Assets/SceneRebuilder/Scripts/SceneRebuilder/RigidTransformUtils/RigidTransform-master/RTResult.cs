using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using Accord;
using System;
using MeshJobs;

public interface IRTResult
{
    int id {get;set;}
    float Distance {get;set;}
    void ApplyMatrix(Transform t);
}

public class RTResultList:List<RTResult>,IRTResult
{
    public int id {get;set;}
    public float Distance {get;set;}
    public void ApplyMatrix(Transform t)
    {
        for(int i=0;i<this.Count;i++)
        {
            //Debug.LogError($"RTResultList.ApplyMatrix[{t.name}][{i}]");
            var r=this[i];
            r.ApplyMatrix(t);
        }
    }
}

[Serializable]
public class RTResult:IRTResult
{
    public int Mode=0;//0:原本的，1:新的角度旋转,2:比例调整
    public int id {get;set;}
    public UnityEngine.Matrix4x4 TransformationMatrix;
    public UnityEngine.Vector3 Translation;

    public UnityEngine.Vector3 Scale=UnityEngine.Vector3.one;

    public Matrix3x3 R;

    public bool IsZero;

    public bool IsReflection;

    public float Distance {get;set;}

    // private List<RTResult> SubResults=null;

    // public void AddSub(RTResult subR){
    //     if(SubResults==null){
    //         SubResults=new List<RTResult>();
    //         SubResults.Add(this);
    //     }
    //     SubResults.Add(subR);
    // }

     public void SetRT(Matrix3x3 r,UnityEngine.Vector3 t)
    {
        this.R=r;
        this.Translation=t;
        UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        TransformationMatrix = AcRigidTransformHelper.AccordToUnityMatrix(matrix, R, Translation);
    }

    public void SetRT(Matrix3x3 r,Accord.Math.Vector3 t)
    {
        this.R=r;
        this.Translation=new UnityEngine.Vector3(t.X,t.Y,t.Z);
        UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        TransformationMatrix = AcRigidTransformHelper.AccordToUnityMatrix(matrix, R, Translation);
    }

    public void SetRT(Quaternion q,Accord.Math.Vector3 t)
    {
        // this.R=r;
        // this.Translation=t;
        // UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        // TransformationMatrix = AcRigidTransformHelper.AccordToUnityMatrix(matrix, R, Translation);
    }

    internal UnityEngine.Vector3[] ApplyPoints(UnityEngine.Vector3[] vs2)
    {
        // if(SubResults!=null)
        // {
        //     UnityEngine.Vector3[] vs4=vs2;
        //     for(int i=0;i<SubResults.Count;i++)
        //     {
        //         var r=SubResults[i];
        //         vs4=r.ApplyPoints(vs4);
        //     }
        //     return vs4;
        // }
        UnityEngine.Vector3[] vs3=new UnityEngine.Vector3[vs2.Length];
        for(int i=0;i<vs2.Length;i++)
        {
            vs3[i]=TransformationMatrix.MultiplyPoint(vs2[i]);
        }
        return vs3;
    }

    internal Unity.Mathematics.float3[] ApplyPoints(Unity.Mathematics.float3[] vs2)
    {
        // if(SubResults!=null)
        // {
        //     Unity.Mathematics.float3[] vs4=vs2;
        //     for(int i=0;i<SubResults.Count;i++)
        //     {
        //         var r=SubResults[i];
        //         vs4=r.ApplyPoints(vs4);
        //     }
        //     return vs4;
        // }
        Unity.Mathematics.float3[] vs3=new Unity.Mathematics.float3[vs2.Length];
        for(int i=0;i<vs2.Length;i++)
        {
            vs3[i]=TransformationMatrix.MultiplyPoint(vs2[i]);
        }
        return vs3;
    }

    public void ApplyMatrix(Transform t)
    {
        // if(SubResults!=null)
        // {
        //     Debug.LogError("ApplyMatrix SubResults:"+SubResults.Count);
        //     for(int i=0;i<SubResults.Count;i++)
        //     {
        //         var r=SubResults[i];
        //         r.ApplyMatrix(t);
        //     }
        // }
        var pos=t.position;
        var qt=t.rotation;

        if(Mode==0){
            t.position = TransformationMatrix.MultiplyPoint(pos);
            t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1))* qt;
        }
        else if(Mode==1){
            t.position = pos+Translation;
            t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2))* qt;
        }
        else if(Mode==2){
            t.position = pos+Translation;
            t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2))* qt;
            t.localScale=Scale;
        }
    }

    private float lastDistance1;

    public void TestApplyPoint(UnityEngine.Vector3 pFrom,UnityEngine.Vector3 pTo)
    {
        IsZero=true;
        //pFrom.PrintVector3("pFrom");
        //pTo.PrintVector3("pTo");
        var pTo2=TransformationMatrix.MultiplyPoint(pFrom);
        //pTo2.PrintVector3("pTo2");
        float dis1=UnityEngine.Vector3.Distance(pTo,pTo2);
        float dd1=Mathf.Abs(lastDistance1-dis1);
        lastDistance1=dis1;
        //Debug.Log("dis1:"+dis1+"|dd:"+dd1);
        if(dis1>0.00001)
        {
            //Debug.LogWarning("TestApplyPoint dis>0.000001"+ " dis1:" + dis1 + "|dd:" + dd1);
            IsZero=false;
        }

        // RTPoints.p05.PrintVector3("p05");
        // RTPoints.p15.PrintVector3("p15");
        // var p152=TransformationMatrix.MultiplyPoint(RTPoints.p05);
        // p152.PrintVector3("p152");
        // float dis2=UnityEngine.Vector3.Distance(RTPoints.p15,p152);
        // float dd2=Mathf.Abs(lastDistance2-dis2);
        // lastDistance2=dis2;
        // Debug.Log("dis2:"+dis2+"|dd:"+dd2);
        // if(dis2>0.000001)
        // {
        //     Debug.LogWarning("TestApplyPoint dis>0.000001");
        //     IsZero=false;
        // }

        //Debug.LogWarning("IsZero:"+IsZero);
    }

}
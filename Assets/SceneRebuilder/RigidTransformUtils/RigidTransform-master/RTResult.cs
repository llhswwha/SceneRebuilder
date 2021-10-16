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
    void ApplyMatrix(Transform tFrom,Transform tTo);
}

public class RTResultList:List<RTResult>,IRTResult
{
    public int id {get;set;}
    public float Distance {get;set;}
    public void ApplyMatrix(Transform tFrom, Transform tTo)
    {
        for(int i=0;i<this.Count;i++)
        {
            //Debug.LogError($"RTResultList.ApplyMatrix[{t.name}][{i}]");
            var r=this[i];
            r.ApplyMatrix(tFrom, tTo);
        }
    }
}

public enum AlignMode
{
    RT,Rotate,Scale,ICP
}

[Serializable]
public class RTResult:IRTResult
{
    public AlignMode Mode =0;//0:原本的，1:新的角度旋转,2:比例调整
    public int id {get;set;}
    public UnityEngine.Matrix4x4 TransformationMatrix;
    public UnityEngine.Vector3 Translation;

    public UnityEngine.Vector3[] NewVecties = null;

    public UnityEngine.Vector3 Scale=UnityEngine.Vector3.one;

    public Matrix3x3 R;

    public bool IsZero;

    public bool IsReflection;

    private float _dis = -1;

    public float Distance
    {
        get
        {
            return _dis;
        }
        set
        {
            _dis = value;
        }
    }

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

    public static RTResult ApplyAngleMatrix(UnityEngine.Vector3 trans, 
        UnityEngine.Matrix4x4 matrix4World, 
        UnityEngine.Vector3[] newVs3,Transform t)
    {
        RTResult rt = new RTResult();
        rt.Mode = AlignMode.Rotate;
        rt.NewVecties = newVs3;
        rt.Translation = trans;
        rt.TransformationMatrix = matrix4World;
        rt.ApplyMatrix(t, null);
        return rt;
    }

    public void ApplyMatrix(Transform tFrom, Transform tTo)
    {

        //Debug.LogError($"ApplyMatrix Mode:{Mode},");
        var pos=tFrom.position;
        var qt=tFrom.rotation;

        if (Mode == AlignMode.RT)
        {
            tFrom.position = TransformationMatrix.MultiplyPoint(pos);
            tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1)) * qt;
        }
        else if (Mode == AlignMode.Rotate)
        {
            if (tFrom.transform.parent != null && tFrom.transform.parent.name!="ZeroPoint")
            {
                Transform oldP = tFrom.transform.parent;
                tFrom.transform.parent = null;
                
                Debug.LogError($"ApplyMatrix01 tFrom.transform.parent != null from:{tFrom.name} parent:{oldP}");
            }

            tFrom.rotation = Quaternion.Euler(0, 0, 0);
            qt = tFrom.rotation;

            tFrom.position = pos + Translation;
            //tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2)) * qt;//测试钢架时这个是正确的
            tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1)) * qt;//测试teaports时这个是正确的
            var vsNew = MeshHelper.GetWorldVertexes(tFrom.gameObject);
            var disNew = DistanceUtil.GetDistance(vsNew, NewVecties);
            if (disNew == 0)
            {
            }
            else
            {
                
                Debug.LogError($"ApplyMatrix01 zero:{DistanceSetting.zeroM:F5},Distance:{this.Distance} newDis:{disNew},\tMode:{Mode},\tfrom:{tFrom.name}");
                tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2)) * qt;//测试钢架时这个是正确的
                //tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1)) * qt;//测试teaports时这个是正确的

                var vsNew2 = MeshHelper.GetWorldVertexes(tFrom.gameObject);
                var disNew2 = DistanceUtil.GetDistance(vsNew2, NewVecties);

                if (disNew2 == 0)
                {
                    //ok
                }
                else
                {
                    Debug.LogError($"ApplyMatrix02 zero:{DistanceSetting.zeroM:F5},Distance:{this.Distance} newDis:{disNew},newDis2:{disNew2},\tMode:{Mode},\tfrom:{tFrom.name}");
                }
            }
        }
        else if (Mode == AlignMode.Scale)
        {
            //tFrom.position = pos + Translation;
            //tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2)) * qt;
            //tFrom.localScale = Scale;

            if (tFrom.transform.parent != null && tFrom.transform.parent.name != "ZeroPoint")
            {
                Transform oldP = tFrom.transform.parent;
                tFrom.transform.parent = null;

                Debug.LogError($"ApplyMatrix11 tFrom.transform.parent != null from:{tFrom.name} parent:{oldP}");
            }

            tFrom.rotation = Quaternion.Euler(0, 0, 0);
            qt = tFrom.rotation;

            tFrom.position = pos + Translation;
            //tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2)) * qt;//测试钢架时这个是正确的
            tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1)) * qt;//测试teaports时这个是正确的
            tFrom.localScale = Scale;

            var vsNew = MeshHelper.GetWorldVertexes(tFrom.gameObject);
            var disNew = DistanceUtil.GetDistance(vsNew, NewVecties);
            if (disNew == 0)
            {
            }
            else
            {

                Debug.LogError($"ApplyMatrix11 zero:{DistanceSetting.zeroM:F5},Distance:{this.Distance} newDis:{disNew},\tMode:{Mode},\tfrom:{tFrom.name}");
                tFrom.position = pos + Translation;
                tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2)) * qt;//测试钢架时这个是正确的
                tFrom.localScale = Scale;
                //tFrom.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1)) * qt;//测试teaports时这个是正确的

                var vsNew2 = MeshHelper.GetWorldVertexes(tFrom.gameObject);
                var disNew2 = DistanceUtil.GetDistance(vsNew2, NewVecties);

                if (disNew2 == 0)
                {
                    //ok
                }
                else
                {
                    Debug.LogError($"ApplyMatrix12 zero:{DistanceSetting.zeroM:F5},Distance:{this.Distance} newDis:{disNew},newDis2:{disNew2},\tMode:{Mode},\tfrom:{tFrom.name}");
                }
            }
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
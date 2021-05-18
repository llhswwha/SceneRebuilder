using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RTTEst : MonoBehaviour
{
    public Transform tFrom;

    public Transform tTo;

    public List<Transform> tFormPoints=new List<Transform>();

    [ContextMenu("DoTrans")]
    public void DoTrans(){
        Debug.Log("DoTrans");
        Debug.Log(tFrom);
        Debug.Log(tFrom.localToWorldMatrix);
        Debug.Log(tFrom.worldToLocalMatrix);
        Debug.Log(tTo);
         Debug.Log(tTo.localToWorldMatrix);
         Debug.Log(tTo.worldToLocalMatrix);

        Matrix4x4 matrix=Matrix4x4.TRS(tTo.position,tTo.rotation,tTo.lossyScale);
        Debug.Log(matrix);
       
        Matrix4x4 m1 = Matrix4x4.identity;
        //Position沿y轴增加5个单位，绕y轴旋转45度，缩放2倍
        m1.SetTRS(tTo.position,tTo.rotation,tTo.lossyScale);
        Debug.Log(m1);

        Matrix4x4 v=tTo.localToWorldMatrix*Matrix4x4.Inverse(tFrom.localToWorldMatrix);
        Debug.Log(v);

         Matrix4x4 v1=tFrom.localToWorldMatrix*Matrix4x4.Inverse(tTo.localToWorldMatrix);

        foreach(var t in tFormPoints){
            var tCopy=MeshHelper.CopyT(t);
            var p2=v1.MultiplyPoint3x4(tCopy.position);
            tCopy.position=p2;
        }
    }

    //     var point1onA: float3 = float3(0,0,0)
    // var point2onA: float3 = float3(0,0,0)
    // var point3onA: float3 = float3(0,0,0)
    
    // var point1onB: float3 = float3(0,0,0)
    // var point2onB: float3 = float3(0,0,0)
    // var point3onB: float3 = float3(0,0,0)
    
    // var m_ratio: Float = 0.0 // 比例参数λ
    // var m_Translation: float3 = float3(0,0,0) // λx平移向量Δ，3个参数
    // var m_Rotation: matrix_float3x3！ // λx旋转矩阵R ，3个参数abc，计算时解算的是abc

    public Transform a1;

    public Transform a2;

    public Transform a3;

    public Transform b1;

    public Transform b2;

    public Transform b3;

    public float3 point1onA;

    public float3 point2onA;

    public float3 point3onA;

    public float3 point1onB;

    public float3 point2onB;

    public float3 point3onB;

    public float m_ratio;

    public float3 m_Translation;

    public float3x3 m_Rotation;

    public float distance(float3 p1,float3 p2)
    {
        return Vector3.Distance(p1,p2);
    }
    public void caliratio(){
        float[] ratiotemp=new float[3];
        float temp1 = 0.0f;
        float temp2 = 0.0f;
        
        temp1 = distance(point1onA, point2onA);
        temp2 = distance(point1onB, point2onB);
        ratiotemp[0] = temp1/temp2;
        
        temp1 = distance(point2onA, point3onA);
        temp2 = distance(point2onB, point3onB);
        ratiotemp[1] = temp1/temp2;
        
        temp1 = distance(point1onA, point3onA);
        temp2 = distance(point1onB, point3onB);
        ratiotemp[2] = temp1/temp2;
        
        m_ratio = (ratiotemp[0]+ratiotemp[1]+ratiotemp[2])/3;
    }

    public void caliRotation(){ 
        //2点减1点
        var m12 = (point2onA.z - point1onA.z) + m_ratio*(point2onB.z - point1onB.z);
        
        var m32 = (point2onA.x - point1onA.x) + m_ratio*(point2onB.x - point1onB.x);

        var m31 = (point2onA.y - point1onA.y) + m_ratio*(point2onB.y - point1onB.y);
    
        var m21 = m12;
        
        //3点减1点
        var m23 = (point3onA.x - point1onA.x) + m_ratio*(point3onB.x - point1onB.x);
        
        var m13 = (point3onA.y - point1onA.y) + m_ratio*(point3onB.y - point1onB.y);
        
        
        float3x3 J = new float3x3(new float3(0,-m12,m13),new float3(-m21,0,m23),new float3(-m31,m32,0));//
        // J.m00=0;
        // J.m01=-m12;
        // J.m02=m13;

        // J.m10=-m21;
        // J.m11=0;
        // J.m12=m23;

        // J.m20=-m31;
        // J.m21=m32;
        // J.m22=0;

        var K = inverse(J);
      
        
        float3 xyz = new float3(0,0,0);
        
        xyz.x = -1 * m_ratio*(point2onA.x - point1onA.x) + (point2onB.x - point1onB.x);
        
        xyz.y = -1 * m_ratio*(point2onA.y - point1onA.y) + (point2onB.y - point1onB.y);
        
        xyz.z = -1 * m_ratio*(point3onA.z - point1onA.z) + (point3onB.z - point1onB.z);
        
        //var abc = K * xyz;
        var abc=multi(K,xyz);

        float3x3 S = new float3x3(new float3(0, abc.z, abc.y), new float3(-1*abc.z, 0, abc.x), new float3(-1*abc.y, -1*abc.x, 0));
        
        float3x3 I = new float3x3(new float3(1,0,0), new float3(0,1,0), new float3(0,0,1));
        
        
         m_Rotation = (I + S) * (inverse(I - S)) * m_ratio;
    }

    public float3 multi(float3x3 k ,float3 xyz)
    {
        float3 r=new float3();
        // r.x=k.c0.x*xyz.x+k.c0.y*xyz.y+k.c0.z*xyz.z;
        // r.y=k.c1.x*xyz.x+k.c1.y*xyz.y+k.c1.z*xyz.z;
        // r.z=k.c2.x*xyz.x+k.c2.y*xyz.y+k.c2.z*xyz.z;

        r.x=k.c0.x*xyz.x+k.c1.x*xyz.y+k.c2.x*xyz.z;
        r.y=k.c0.y*xyz.x+k.c1.y*xyz.y+k.c2.y*xyz.z;
        r.z=k.c0.z*xyz.x+k.c1.z*xyz.y+k.c2.z*xyz.z;

        Debug.Log("multi 1");
        Debug.Log(k);
        Debug.Log(xyz);
        Debug.Log(r);
         Debug.Log("multi 2");
        return r;
    }

//     public float3 transform( float3x3 m, float3 v )

// {

//    float3 res;

//    res.x = m.m00*v.x + m.m01*v.y + m.m02*v.z;

//    res.y = m.m10*v.x + m.m11*v.y + m.m12*v.z;

//    res.z = m.m20*v.x + m.m21*v.y + m.m22*v.z;

//    return res;

// }

     public static float3x3 inverse(float3x3 m)
        {
            float3 c0 = m.c0;
            float3 c1 = m.c1;
            float3 c2 = m.c2;

            float3 t0 = new float3(c1.x, c2.x, c0.x);
            float3 t1 = new float3(c1.y, c2.y, c0.y);
            float3 t2 = new float3(c1.z, c2.z, c0.z);

            float3 m0 = t1 * t2.yzx - t1.yzx * t2;
            float3 m1 = t0.yzx * t2 - t0 * t2.yzx;
            float3 m2 = t0 * t1.yzx - t0.yzx * t1;

            float rcpDet = 1.0f / math.csum(t0.zxy * m0);
            return new float3x3(m0, m1, m2) * rcpDet;
        }

    public void caliTranslation(){
       m_Translation = point3onA - multi(m_Rotation , point3onB);
    }

    public Transform t;

    [ContextMenu("test")]
    public void test(){
    
     point1onA = new float3(1,0,0);
     point2onA = new float3(0,1,0);
     point3onA = new float3(0,0,1);
    
    //  var node = t;
    //  node.position = new Vector3(0.1f, 0.2f, 0.3f);
    //  node.eulerAngles = new Vector3(0.1f, 0.2f, 0.3f);
    //  node.localScale = new Vector3(1.2f, 1.2f, 1.2f);

     Matrix4x4 matrix=new Matrix4x4();
     matrix.SetTRS(new Vector3(0.1f, 0.2f, 0.3f),Quaternion.Euler(0.1f, 0.2f, 0.3f),new Vector3(1.2f, 1.2f, 1.2f));

     var newA = matrix.MultiplyPoint3x4(point1onA);
     var newB = matrix.MultiplyPoint3x4(point2onA);
     var newC = matrix.MultiplyPoint3x4(point3onA);
        
     point1onB = new float3(newA.x, newA.y, newA.z);
     point2onB = new float3(newB.x, newB.y, newB.z);
     point3onB = new float3(newC.x, newC.y, newC.z);
        
     caliratio();
     caliRotation();
     caliTranslation();
        
     print($"比例参数λ:{m_ratio}");
     print($"平移向量Δ:{m_Translation}");
     print($"旋转矩阵R:{m_Rotation}");
     print($"变换矩阵真值:{matrix}");

    }

    [ContextMenu("test2")]
    public void test2(){
    
     point1onA = a1.position;
     point2onA = a2.position;
     point3onA = a3.position;
    
    point1onB = b1.position;
     point2onB = b2.position;
     point3onB = b3.position;
        
     caliratio();
     caliRotation();
     caliTranslation();
    Vector3 c0=m_Rotation.c0 * m_ratio;
    Vector3 c1=m_Rotation.c1 * m_ratio;
    Vector3 c2=m_Rotation.c2 * m_ratio;
    Vector3 t=m_Translation * m_ratio;
    Vector4 tt=t;
    tt.w=1;
    //Matrix4x4 matrix=new Matrix4x4(c0,c1,c2,tt);

     Matrix4x4 matrix=new Matrix4x4(new Vector4(c0.x,c1.x,c2.x,0),new Vector4(c0.y,c1.y,c2.y,0),new Vector4(c0.z,c1.z,c2.z,0),new Vector4(t.x,t.y,t.z,1));

    // Debug.Log(matrix);
    //  matrix.rotation=m_Rotation;
    //  matrix.SetTRS(m_Translation,Quaternion.Euler(0.1f, 0.2f, 0.3f),new Vector3(1.2f, 1.2f, 1.2f));

    

    var p=matrix.MultiplyPoint3x4(point1onA);

    var p2=matrix.MultiplyPoint(point1onA);
        
     print($"比例参数λ:\n{m_ratio}");
     print($"平移向量Δ:\n{m_Translation}");
     print($"旋转矩阵R:\n{m_Rotation}");
     print($"变换矩阵真值:\n{matrix}");

     print($"point1onA:{point1onA}");
    p.PrintVector3("p");
    p2.PrintVector3("p2");
    print($"point1onB:{point1onB}");
    }
}

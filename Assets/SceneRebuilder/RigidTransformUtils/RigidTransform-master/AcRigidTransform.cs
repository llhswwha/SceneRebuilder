
// Rigid Transform implementation using explanation in http://nghiaho.com/?page_id=671
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using Accord;
using System;
using MeshJobs;

public class AcRigidTransform : MonoBehaviour {
    
    public static AcRigidTransform Instance;

    public void Awake()
    {
        Instance=this;
    }

    // Initial objects   
    public GameObject P01GO;
    public GameObject P02GO;
    public GameObject P03GO;

    public GameObject P04GO;

    // Transformed objects
    public GameObject P11GO;
    public GameObject P12GO;
    public GameObject P13GO;

    public GameObject P14GO;
    
    // object used as a 4th point to test Transformation Matrix
    public GameObject TestObject;

    public GameObject TestObject2;

    public AcRTPoints5 RTPoints;
    
    // variables used for RigidTransform
    // private Matrix3x3 H;
    // private Matrix3x3 U;
    // private Accord.Math.Vector3 E;
    // private Matrix3x3 V;
    // private Matrix3x3 R;
    // Accord.Math.Vector3 centroidA;
    // Accord.Math.Vector3 centroidB;

    UnityEngine.Matrix4x4 TransformationMatrix;
    // private Accord.Math.Vector3 Translation;

    // Saving initial position and rotation to apply transformation in Update()
    private UnityEngine.Vector3 InitPosition;
    private Quaternion InitQT;

    void Start () {
        InitPosition = TestObject.transform.position;
        InitQT = TestObject.transform.rotation;        
    }
   

    [ContextMenu("InitRTPoints")]
    public void InitRTPoints()
    {
        if(P01GO==null||P02GO==null||P03GO==null||P11GO==null||P12GO==null||P13GO==null)
        {
            return;
        }
        RTPoints.p01=P01GO.transform.position;
        RTPoints.p02=P02GO.transform.position;
        RTPoints.p03=P03GO.transform.position;
        RTPoints.p04=P04GO.transform.position;
        if(TestObject!=null)
        {
            RTPoints.p04=TestObject.transform.position;
            InitPosition = TestObject.transform.position;
            InitQT = TestObject.transform.rotation; 
        }
        RTPoints.p11=P11GO.transform.position;
        RTPoints.p12=P12GO.transform.position;
        RTPoints.p13=P13GO.transform.position;
        RTPoints.p14=P14GO.transform.position;
        if(TestObject2!=null)
            RTPoints.p14=TestObject2.transform.position;

    }

    [ContextMenu("CleanObjects")]
    public void CleanObjects()
    {
        P01GO=null;
        P02GO=null;
        P03GO=null;
        P11GO=null;
        P12GO=null;
        P13GO=null;
    }

    public bool IsReflection=false;

    // [ContextMenu("ApplyTransformation4")]
    // public void ApplyTransformation4()
    // {   
    //     InitRTPoints();

    //     //改成基于4个点进行的变换。4个点一个是中心，一个是法线，2个是法平面上的两点

    //     //centroid:质心
    //     //Calculating Centroids from both coordinate system
    //     centroidA = (UnitytoAccord(p01) + UnitytoAccord(p02) + UnitytoAccord(p03) + UnitytoAccord(p04)) / 4;
    //     centroidB = (UnitytoAccord(p11) + UnitytoAccord(p12) + UnitytoAccord(p13) + UnitytoAccord(p14)) / 4;
    //     //Debug.Log("Centroid A is" + centroidA + " Centroid B is" + centroidB);

    //     // Calculating Covariance Matrix
    //     H = CovarianceMatrixStep(UnitytoAccord(p01) - centroidA, UnitytoAccord(p11) - centroidB)
    //         + CovarianceMatrixStep(UnitytoAccord(p02) - centroidA, UnitytoAccord(p12) - centroidB)
    //         + CovarianceMatrixStep(UnitytoAccord(p03) - centroidA, UnitytoAccord(p13) - centroidB)
    //         + CovarianceMatrixStep(UnitytoAccord(p04) - centroidA, UnitytoAccord(p14) - centroidB);

    //     H.SVD(out U, out E, out V);
    //     R = V * U.Transpose();      

    //     Debug.Log("R.Determinant is" + R.Determinant);
    //     //special reflection case
    //     if(R.Determinant<0)
    //         {
    //             IsReflection=true;
    //             V.V02 = (-V.V02);
    //             V.V12 = (-V.V12);
    //             V.V22 = (-V.V22);
    //             R = V * U.Transpose();
    //             Debug.LogWarning("Reflection case");
    //             Debug.Log("R.Determinant is" + R.Determinant);
    //         }
    //         else{
    //             IsReflection=false;
    //         }
        
    //     // InitPosition.PrintVector3("P01");
    //     // TestObject2.transform.position.PrintVector3("P02");

    //     Translation = NegativeMatrix(R) * centroidA + centroidB;
    //     TransformationMatrix = AccordToUnityMatrix(TransformationMatrix, R, Translation);
    //     Debug.Log(TransformationMatrix);
        
    //     // // Transformaiton Matrix for Unity
    //     // TransformationMatrix.SetTRS(AccordtoUnity(Translation), Quaternion.LookRotation(TransformationMatrix.GetColumn(1),
    //     //      TransformationMatrix.GetColumn(2)), UnityEngine.Vector3.one);
    //     // Debug.Log(TransformationMatrix);

    //     // // Applying Translation and rotation to 4th point/object
    //     // TestObject.transform.position = TransformationMatrix.MultiplyPoint(InitPosition);
    //     // TestObject.transform.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2))* InitQT;
    // }

    [ContextMenu("ApplyTransformation4")]
    public RTResult ApplyTransformation4()
    {   
        InitRTPoints();
        var result=ApplyTransformation4(RTPoints);
        this.IsReflection=result.IsReflection;
        this.TransformationMatrix=result.TransformationMatrix;
        return result;
    }

    [ContextMenu("ApplyTransformation3")]
    public RTResult ApplyTransformation3()
    {   
        InitRTPoints();
        var result=ApplyTransformation3(RTPoints);
        this.IsReflection=result.IsReflection;
        this.TransformationMatrix=result.TransformationMatrix;
        return result;
    }

    public static RTResult ApplyTransformationN(UnityEngine.Vector3[] ps1, UnityEngine.Vector3[] ps2)
    {
        AcRTPoints points = new AcRTPoints(ps1, ps2);
        return ApplyTransformationN(points);
    }
    public static RTResult ApplyTransformationN(AcRTPoints points)
    {
        //points.PrintLog("RT");

        RTResult result = new RTResult();

        //改成基于4个点进行的变换。4个点一个是中心，一个是法线，2个是法平面上的两点

        //centroid:质心
        //Calculating Centroids from both coordinate system

        var ps1a = AcRigidTransformHelper.UnitytoAccord(points.ps1);//from
        var ps2a = AcRigidTransformHelper.UnitytoAccord(points.ps2);//to

        var centroidA = AcRigidTransformHelper.GetCentroid(ps1a);
        var centroidB = AcRigidTransformHelper.GetCentroid(ps2a);
        //Debug.Log("Centroid A is" + centroidA + " Centroid B is" + centroidB);

        // Calculating Covariance Matrix
        var H = AcRigidTransformHelper.GetCovarianceMatrix(ps1a, centroidA, ps2a, centroidB);
        // Debug.Log("H:"+H);

        // Debug.Log("H.Determinant is" + H.Determinant);

        // var Qx=Math.Sqrt(Distance2(p01a,centroidA)+Distance2(p02a,centroidA)+Distance2(p03a,centroidA)+Distance2(p04a,centroidA)/4);
        // Debug.Log("Qx:"+Qx);
        // var Qy=Math.Sqrt(Distance2(p11a,centroidB)+Distance2(p12a,centroidB)+Distance2(p13a,centroidB)+Distance2(p14a,centroidB)/4);
        // Debug.Log("Qy:"+Qy);
        // var qq=(Qx*Qy);
        // Debug.Log("qq:"+qq);
        // var qq2=((int)Qx*(int)Qy);
        // Debug.Log("qq2:"+qq2);
        // var dd=H.Determinant;
        // Debug.Log("dd:"+dd);
        // var p=dd/qq;
        // Debug.Log("p:"+p);

        Matrix3x3 U;
        Accord.Math.Vector3 E;
        Matrix3x3 V;

        H.SVD(out U, out E, out V);
        var R = V * U.Transpose();

        // Debug.Log("ApplyTransformation4 R.Determinant is" + R.Determinant);
        //special reflection case
        if (R.Determinant < 0)
        {
            result.IsReflection = true;
            V.V02 = (-V.V02);
            V.V12 = (-V.V12);
            V.V22 = (-V.V22);
            R = V * U.Transpose();
            //Debug.LogWarning("ApplyTransformation4 Reflection case");
            //Debug.Log("ApplyTransformation4 R.Determinant is" + R.Determinant);
        }
        else
        {
            result.IsReflection = false;
        }
        // InitPosition.PrintVector3("P01");
        // TestObject2.transform.position.PrintVector3("P02");

        var T = AcRigidTransformHelper.NegativeMatrix(R) * centroidA + centroidB;
        // UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        // TransformationMatrix = AccordToUnityMatrix(matrix, R, Translation);
        // Debug.Log(TransformationMatrix);

        result.SetRT(R, T);
        //Debug.Log(result.TransformationMatrix);

        //result.TestApplyPoint(points.p04,points.p14);//确认转换后的坐标是否正确

        return result;
    }

    public static RTResult ApplyTransformation4(AcRTPoints5 points)
    {   
        //points.PrintLog("RT");

        RTResult result=new RTResult();

        //改成基于4个点进行的变换。4个点一个是中心，一个是法线，2个是法平面上的两点

        //centroid:质心
        //Calculating Centroids from both coordinate system
        var p01a=AcRigidTransformHelper.UnitytoAccord(points.p01);
        var p02a=AcRigidTransformHelper.UnitytoAccord(points.p02);
        var p03a=AcRigidTransformHelper.UnitytoAccord(points.p03);
        var p04a=AcRigidTransformHelper.UnitytoAccord(points.p04);
        var p11a=AcRigidTransformHelper.UnitytoAccord(points.p11);
        var p12a=AcRigidTransformHelper.UnitytoAccord(points.p12);
        var p13a=AcRigidTransformHelper.UnitytoAccord(points.p13);
        var p14a=AcRigidTransformHelper.UnitytoAccord(points.p14);

        var centroidA = (p01a + p02a + p03a + p04a) / 4;
        var centroidB = (p11a + p12a + p13a + p14a) / 4;
        //Debug.Log("Centroid A is" + centroidA + " Centroid B is" + centroidB);

        // Calculating Covariance Matrix
        var H = AcRigidTransformHelper.CovarianceMatrixStep(p01a - centroidA, p11a - centroidB)
            + AcRigidTransformHelper.CovarianceMatrixStep(p02a - centroidA, p12a - centroidB)
            + AcRigidTransformHelper.CovarianceMatrixStep(p03a - centroidA, p13a - centroidB)
            + AcRigidTransformHelper.CovarianceMatrixStep(p04a - centroidA, p14a - centroidB);
        // Debug.Log("H:"+H);

        // Debug.Log("H.Determinant is" + H.Determinant);

        // var Qx=Math.Sqrt(Distance2(p01a,centroidA)+Distance2(p02a,centroidA)+Distance2(p03a,centroidA)+Distance2(p04a,centroidA)/4);
        // Debug.Log("Qx:"+Qx);
        // var Qy=Math.Sqrt(Distance2(p11a,centroidB)+Distance2(p12a,centroidB)+Distance2(p13a,centroidB)+Distance2(p14a,centroidB)/4);
        // Debug.Log("Qy:"+Qy);
        // var qq=(Qx*Qy);
        // Debug.Log("qq:"+qq);
        // var qq2=((int)Qx*(int)Qy);
        // Debug.Log("qq2:"+qq2);
        // var dd=H.Determinant;
        // Debug.Log("dd:"+dd);
        // var p=dd/qq;
        // Debug.Log("p:"+p);

        Matrix3x3 U;
        Accord.Math.Vector3 E;
        Matrix3x3 V;

        H.SVD(out U, out E, out V);
        var R = V * U.Transpose();      

        // Debug.Log("ApplyTransformation4 R.Determinant is" + R.Determinant);
        //special reflection case
        if(R.Determinant<0)
            {
                result.IsReflection=true;
                V.V02 = (-V.V02);
                V.V12 = (-V.V12);
                V.V22 = (-V.V22);
                R = V * U.Transpose();
                //Debug.LogWarning("ApplyTransformation4 Reflection case");
                //Debug.Log("ApplyTransformation4 R.Determinant is" + R.Determinant);
            }
            else{
                result.IsReflection=false;
            }
        // InitPosition.PrintVector3("P01");
        // TestObject2.transform.position.PrintVector3("P02");
        
        var T = AcRigidTransformHelper.NegativeMatrix(R) * centroidA + centroidB;
        // UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        // TransformationMatrix = AccordToUnityMatrix(matrix, R, Translation);
        // Debug.Log(TransformationMatrix);
        
        result.SetRT(R,T);
        //Debug.Log(result.TransformationMatrix);

        //result.TestApplyPoint(points.p04,points.p14);//确认转换后的坐标是否正确

        return result;
    }

    public static RTResult ApplyTransformation3(AcRTPoints5 points)
    {   
        RTResult result=new RTResult();

        //改成基于4个点进行的变换。4个点一个是中心，一个是法线，2个是法平面上的两点

        //centroid:质心
        //Calculating Centroids from both coordinate system
        var centroidA = (AcRigidTransformHelper.UnitytoAccord(points.p01) + AcRigidTransformHelper.UnitytoAccord(points.p02) + AcRigidTransformHelper.UnitytoAccord(points.p03) ) / 3;
        var centroidB = (AcRigidTransformHelper.UnitytoAccord(points.p11) + AcRigidTransformHelper.UnitytoAccord(points.p12) + AcRigidTransformHelper.UnitytoAccord(points.p13) ) / 3;
        //Debug.Log("Centroid A is" + centroidA + " Centroid B is" + centroidB);

        // Calculating Covariance Matrix
        var H = AcRigidTransformHelper.CovarianceMatrixStep(AcRigidTransformHelper.UnitytoAccord(points.p01) - centroidA, AcRigidTransformHelper.UnitytoAccord(points.p11) - centroidB)
            + AcRigidTransformHelper.CovarianceMatrixStep(AcRigidTransformHelper.UnitytoAccord(points.p02) - centroidA, AcRigidTransformHelper.UnitytoAccord(points.p12) - centroidB)
            + AcRigidTransformHelper.CovarianceMatrixStep(AcRigidTransformHelper.UnitytoAccord(points.p03) - centroidA, AcRigidTransformHelper.UnitytoAccord(points.p13) - centroidB);

        Matrix3x3 U;
        Accord.Math.Vector3 E;
        Matrix3x3 V;

        H.SVD(out U, out E, out V);
        var R = V * U.Transpose();      

        Debug.Log("R.Determinant is" + R.Determinant);
        //special reflection case
        if(R.Determinant<0)
            {
                result.IsReflection=true;
                V.V02 = (-V.V02);
                V.V12 = (-V.V12);
                V.V22 = (-V.V22);
                R = V * U.Transpose();
                Debug.LogWarning("Reflection case");
                Debug.Log("R.Determinant is" + R.Determinant);
            }
            else{
                result.IsReflection=false;
            }
        // InitPosition.PrintVector3("P01");
        // TestObject2.transform.position.PrintVector3("P02");
        
        var T = AcRigidTransformHelper.NegativeMatrix(R) * centroidA + centroidB;
        // UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        // TransformationMatrix = AccordToUnityMatrix(matrix, R, Translation);
        // Debug.Log(TransformationMatrix);
        
        result.SetRT(R,T);
        Debug.Log(result.TransformationMatrix);

        result.TestApplyPoint(points.p04,points.p14);//确认转换后的坐标是否正确

        return result;
    }

    public void ApplyTransform(Transform t)
    {
        Debug.LogError("ApplyTransform:"+t);
        Debug.LogError(TransformationMatrix);
        Debug.LogError(t.position);
        Debug.LogError(t.rotation);
        var pos=t.position;
        var qt=t.rotation;

        //TransformationMatrix.SetTRS(AccordtoUnity(Translation), Quaternion.LookRotation(TransformationMatrix.GetColumn(1),TransformationMatrix.GetColumn(2)), UnityEngine.Vector3.one);
        t.position = TransformationMatrix.MultiplyPoint(pos);
        //t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(2))* qt;
        t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(1))* qt;

        // t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(0), TransformationMatrix.GetColumn(1))* qt;
        // t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(1), TransformationMatrix.GetColumn(0))* qt;

        //t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(0), TransformationMatrix.GetColumn(2))* qt;
        //t.rotation = Quaternion.LookRotation(TransformationMatrix.GetColumn(2), TransformationMatrix.GetColumn(0))* qt;

        // UnityEngine.Matrix4x4 matrix=UnityEngine.Matrix4x4.identity;
        // matrix.SetTRS(AccordtoUnity(Translation), Quaternion.LookRotation(TransformationMatrix.GetColumn(1),TransformationMatrix.GetColumn(2)), UnityEngine.Vector3.one);
        // t.position = matrix.MultiplyPoint(t.position);
        // //t.rotation = Quaternion.LookRotation(matrix.GetColumn(1), matrix.GetColumn(2))* qt;

        Debug.LogError(t.position);
        Debug.LogError(t.rotation);
    }

    public static AcRTPoints5 GetRTPoints(ThreePoint tpFrom,ThreePoint tpTo)
    {
        AcRTPoints5 points=new AcRTPoints5();
        points.p01=tpFrom.GetMaxP();
        points.p02=tpFrom.GetMinP();
        points.p03=tpFrom.GetCenterP();
        points.p04=tpFrom.GetNormalP();//法线
        points.p05=tpFrom.GetMaxP();

        points.p11=tpTo.GetMaxP();
        points.p12=tpTo.GetMinP();
        points.p13=tpTo.GetCenterP();
        points.p14=tpTo.GetNormalP();//法线
        points.p15=tpTo.GetMaxP();

        //points.PrintLog($"Init");
        return points;
    }

    public RTResult GetRTMatrix(ThreePoint tpFrom,ThreePoint tpTo)
    {
        this.RTPoints=GetRTPoints(tpFrom,tpTo);
        return this.GetRTMatrix();//脚本内的
    }

    public static double totalTime1;
    public static double totalTime2;

    public static RTResult GetRTMatrixS(ThreePoint tpFrom,ThreePoint tpTo)
    {
        DateTime  tmpT = DateTime.Now;
        var RTPoints=GetRTPoints(tpFrom,tpTo);
        totalTime1+=(DateTime.Now-tmpT).TotalMilliseconds;
        
        tmpT = DateTime.Now;
        var result=ApplyTransformation4(RTPoints);
        totalTime2+=(DateTime.Now-tmpT).TotalMilliseconds;
        //return result.TransformationMatrix;
        return result;
    }

    public static void ApplyMatrix(UnityEngine.Matrix4x4 matrix,Transform t)
    {
        var pos=t.position;
        var qt=t.rotation;
        t.position = matrix.MultiplyPoint(pos);
        t.rotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1))* qt;
    }


    [ContextMenu("ResetTransformationMatrix")]
    public void ResetTransformationMatrix()
    {
        TransformationMatrix=UnityEngine.Matrix4x4.zero;
    }

    // [ContextMenu("TestApplyTransformation")]
    // public void TestApplyTransformation()
    // {
    //     ResetTransformationMatrix();
    //     ApplyTransformation();
    //     TestApplyPoint();
    // }

    [ContextMenu("GetRTMatrix4")]
    public RTResult GetRTMatrix()
    {
        ResetTransformationMatrix();
        RTResult result=ApplyTransformation4();
        TestApplyPoint();
        return result;
    }

    [ContextMenu("GetRTMatrix3")]
    public RTResult GetRTMatrix3()
    {
        ResetTransformationMatrix();
        RTResult result=ApplyTransformation3();
        TestApplyPoint();
        return result;
    }

    private float lastDistance1;

    private float lastDistance2;

    public bool IsZero=true;

    [ContextMenu("TestApplyPoint")]
    public void TestApplyPoint()
    {
        IsZero=true;
        RTPoints.p04.PrintVector3("p04");
        RTPoints.p14.PrintVector3("p14");
        var p142=TransformationMatrix.MultiplyPoint(RTPoints.p04);
        p142.PrintVector3("p142");
        float dis1=UnityEngine.Vector3.Distance(RTPoints.p14,p142);
        float dd1=Mathf.Abs(lastDistance1-dis1);
        lastDistance1=dis1;
        Debug.Log("dis1:"+dis1+"|dd:"+dd1);
        if(dis1>0.000001)
        {
            Debug.LogWarning("TestApplyPoint dis>0.000001");
            IsZero=false;
        }

        RTPoints.p05.PrintVector3("p05");
        RTPoints.p15.PrintVector3("p15");
        var p152=TransformationMatrix.MultiplyPoint(RTPoints.p05);
        p152.PrintVector3("p152");
        float dis2=UnityEngine.Vector3.Distance(RTPoints.p15,p152);
        float dd2=Mathf.Abs(lastDistance2-dis2);
        lastDistance2=dis2;
        Debug.Log("dis2:"+dis2+"|dd:"+dd2);
        if(dis2>0.000001)
        {
            Debug.LogWarning("TestApplyPoint dis>0.000001");
            IsZero=false;
        }

        //Debug.LogWarning("IsZero:"+IsZero);
    }

    public UnityEngine.Vector3 ApplyPoint(UnityEngine.Vector3 p1)
    {
        p1.PrintVector3("P1");
        var p2=TransformationMatrix.MultiplyPoint(p1);
        p2.PrintVector3("P2");
        return p2;
    }

    // // Update is called once per frame
    // void Update () {           
    //        ApplyTransformation();        
    // }

    // public static void RTAlign(MeshFilter mfFrom,MeshFilter mfTo)
    // {
    //     DateTime start=DateTime.Now;

    //     MeshJobHelper.NewThreePointJobs(new MeshFilter[]{mfFrom,mfTo},10000);
    //     // MeshJobs.NewMeshAlignJob(go1, go2Copy,true);
    //     RTAlignOneCore(mfFrom,mfTo);
    // }

    public static void RTAlign(MeshPoints mfFrom, MeshPoints mfTo)
    {
        DateTime start=DateTime.Now;

        Transform tFrom=mfFrom.transform;
        Transform tTo=mfTo.transform;
        // ThreePointJobResult resultFrom=ThreePointJobResultList.Instance.GetThreePoint(mfFrom);
        // resultFrom.localToWorldMatrix=tFrom.localToWorldMatrix;

        // ThreePointJobResult resultTo=ThreePointJobResultList.Instance.GetThreePoint(mfTo);
        // resultTo.localToWorldMatrix=tTo.localToWorldMatrix;

        var tpsFrom = ThreePointJobResultList.Instance.GetThreePoints(mfFrom);
        // for (int i = 0; i < tpsFrom.Length; i++)
        // {
        //     ThreePoint tp = tpsFrom[i];
        //     tp.PrintLog("From"+i);
        // }
        var tpsTo = ThreePointJobResultList.Instance.GetThreePoints(mfTo);
        // for (int i = 0; i < tpsTo.Length; i++)
        // {
        //     ThreePoint tp = tpsTo[i];
        //     tp.PrintLog("To"+i);
        // }

        MeshHelper.ClearChildren(tFrom);
        MeshHelper.ClearChildren(tTo);

        AcRigidTransform acRigidTransform=GetAcRigidTransform();
        int count=0;

        float minDis=float.MaxValue;
        RTResult minRT=new RTResult();

        var vsFrom=MeshHelper.GetWorldVertexes(mfFrom);
        var vsTo=MeshHelper.GetWorldVertexes(mfTo);
        bool isFound=false;
        for(int l=0;l<tpsFrom.Length;l++)
        {
            var tpFrom=tpsFrom[l];
            for(int k=0;k<tpsTo.Length;k++)
            {
                var tpTo=tpsTo[k];
                count++;
                var rt=acRigidTransform.GetRTMatrix(tpFrom,tpTo);//核心,基于脚本的
                //var rt=GetRTMatrixS(tpFrom,tpTo);//核心，静态函数的
                MeshHelper.ClearChildren(tFrom);
                var vsNew=rt.ApplyPoints(vsFrom);
                var dis=DistanceUtil.GetDistance(vsTo,vsNew);
                if(dis<minDis){
                    minDis=dis;
                    minRT=rt;
                }
                Debug.LogError($">>>RTAlignOneCore [{l},{k}]\tIsZero:{rt.IsZero},\tIsReflection:{rt.IsReflection},\tdis:{dis}");
                if(dis==0)
                {
                    Debug.LogError($"RTAlignOneCore2 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
                    //acRigidTransform.ApplyTransform(t1);
                    
                    //AcRigidTransform.ApplyMatrix(matrix,tFrom);
                    rt.ApplyMatrix(tFrom, tTo);
                    var gos=MeshHelper.ShowVertexes(vsNew,0.005f,tFrom);//0.1,0.005
                    isFound=true;
                    return;
                }
            }
            //break;
        }

        if(isFound==false){
            var vsNew=minRT.ApplyPoints(vsFrom);
            minRT.ApplyMatrix(tFrom, tTo);
            var gos=MeshHelper.ShowVertexes(vsNew,0.005f,tFrom);//0.1,0.005
        }

        //if(minDis<AcRTAlignJobResult.MinDis)
        // {
        //     minRT.ApplyMatrix(tFrom);
        // }

         Debug.LogError($"RTAlignOneCore Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms,minDis:{minDis}");
    }

    public static AcRigidTransform acRigidTransform;

    private static AcRigidTransform GetAcRigidTransform()
    {
        if(acRigidTransform==null){
            acRigidTransform=AcRigidTransform.Instance;
        }
        if(acRigidTransform==null){
            acRigidTransform=GameObject.FindObjectOfType<AcRigidTransform>();
        }
        if(acRigidTransform==null){
            GameObject tmp=new GameObject("AcRigidTransform");
            acRigidTransform=tmp.AddComponent<AcRigidTransform>();
        }
        acRigidTransform.CleanObjects();
        return acRigidTransform;
    }
}



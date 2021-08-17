using System;
using System.Collections;
using System.Collections.Generic;
using MeshJobs;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MeshComparer : SingletonBehaviour<MeshComparer>
{
    public AlignDebugStep step;

    public AlignRotateMode mode;

    //public float zeroP= 0.0001f;//0.0004f


    //public double zeroPMaxDis = 5;

    //public float zeroM=0.0004f;//PointZero

    public DisSetting disSetting;

    public bool IsWorld=false;

    public GameObject goTo;

    public GameObject goFrom;

    public GameObject goFromCopy;

    public void SetGos(GameObject from,GameObject to)
    {
        goFrom = from;
        goTo = to;
        if (goFromCopy != null)
        {
            GameObject.DestroyImmediate(goFromCopy);
        }
    }

    public MeshNode node1;

    public MeshNode node2;

    public int TryRotateAngleCount=20;

    public bool ShowDebugDetail=true;

    public Vector3 centerOffset=Vector3.zero;

    private MeshNode CreateMeshNode(GameObject go)
    {
        MeshNode node=MeshNode.CreateNew(go);
        node.IsWorld=this.IsWorld;
        return node;
    }

    public void InitMeshNodeInfo()
    {
        SetDistanceSetting();

        DateTime start=DateTime.Now;
        
        node1=CreateMeshNode(goTo);

        // if(go2Copy==null){
        //     go2Copy=go2;
        // }

        var node21=CreateMeshNode(goFrom);
        node21.GetVertexCenterInfo(ShowDebugDetail,true,centerOffset);

        node2=CreateMeshNode(goFromCopy);

        node1.GetVertexCenterInfo(ShowDebugDetail,true,centerOffset);
        node2.GetVertexCenterInfo(ShowDebugDetail,true,centerOffset);

        Debug.LogWarning($"InitMeshNodeInfo Time:{(DateTime.Now-start).TotalMilliseconds}ms");
    }

    public bool CreateNewScaleGo(GameObject goNew,Vector3 scale)
    {
        var goNew1 = MeshHelper.CopyGO(goNew);
        goNew1.name+="_"+scale.Vector3ToString();
        goNew1.transform.SetParent(goNew.transform.parent);
        goNew1.transform.localScale=scale;
        var dis2=MeshHelper.GetVertexDistanceEx(goNew1.transform,goTo.transform);
        if (dis2 < 0.01f)
        {
            Debug.LogError($"\tCreateNewScaleGo1 dis2:{dis2},scale:{scale.Vector3ToString()}");
            goNew1.name += "_Zero_" + dis2;
            return true;
        }
        if (dis2<0.2f){
            Debug.LogError($"\tCreateNewScaleGo2 dis2:{dis2},scale:{scale.Vector3ToString()}");
            goNew1.name+="_Zero2_"+dis2;
            return false;
        }
        else if(dis2<0.5f){
            Debug.LogWarning($"\tCreateNewScaleGo3 dis2:{dis2},scale:{scale.Vector3ToString()}");
            GameObject.DestroyImmediate(goNew1);
            return false;
        }
        else{
            GameObject.DestroyImmediate(goNew1);
            return false;
        }
    }

    public void TryScales()
    {
        var goNew = MeshHelper.CopyGO(goFrom);

        var scale1=goNew.transform.localScale;

        var vs1=MeshHelper.GetWorldVertexes(goNew);
        var mm1=MeshHelper.GetMinMax(vs1);
        var vs2=MeshHelper.GetWorldVertexes(goTo);
        var mm2=MeshHelper.GetMinMax(vs2);
        var scaleNew=new Vector3(mm2[2].x/mm1[2].x,mm2[2].y/mm1[2].y,mm2[2].z/mm1[2].z);

        var dis=DistanceUtil.GetDistance(vs1,vs2);

        string log=$"Distance:{dis},\tsize1:{mm1[2].Vector3ToString()},\tsize2:{mm2[2].Vector3ToString()},\tscale:{scaleNew.Vector3ToString()}";

        if(dis==0){
            Debug.LogError(log);
        }
        else if(dis<0.5f){
            Debug.LogWarning(log);

            // var goNew1 = MeshHelper.CopyGO(goNew);
            // goNew1.transform.SetParent(goNew.transform);
            // goNew1.transform.localScale=new Vector3(scale1.x*scaleNew.x,scale1.y*scaleNew.y,scale1.z*scaleNew.z);
            // var dis2=MeshHelper.GetVertexDistanceEx(goNew1.transform,goTo.transform);
            // Debug.LogWarning($"dis2:{dis2}");
            Vector3 scaleNN=new Vector3(scale1.x*scaleNew.x,scale1.y*scaleNew.y,scale1.z*scaleNew.z);
            CreateNewScaleGo(goNew,new Vector3(scaleNN.x,scaleNN.y,scaleNN.z));
            CreateNewScaleGo(goNew,new Vector3(scaleNN.y,scaleNN.x,scaleNN.z));
            CreateNewScaleGo(goNew,new Vector3(scaleNN.z,scaleNN.y,scaleNN.x));
            CreateNewScaleGo(goNew,new Vector3(scaleNN.z,scaleNN.x,scaleNN.y));
            CreateNewScaleGo(goNew,new Vector3(scaleNN.x,scaleNN.z,scaleNN.y));
            CreateNewScaleGo(goNew,new Vector3(scaleNN.y,scaleNN.z,scaleNN.x));//6种情况
        }
        else{
            Debug.Log(log);
            GameObject.DestroyImmediate(goNew);
        }
    }

    public void TryAngles()
    {
        SetDistanceSetting();
        GameObject gameObject=new GameObject("TryAngles");
        Quaternion rotation=goFrom.transform.rotation;
        int count=0;
        for(int i=0;i<3;i++){
            for(int j=0;j<3;j++)
            {
                for(int k=0;k<3;k++)
                {
                    Vector3 angle=new Vector3(i*90,j*90,k*90);
                    count++;
                    var goNew = MeshHelper.CopyGO(goFrom);

                    var scale1=goNew.transform.localScale;

                    goNew.transform.position=goTo.transform.position;
                    goNew.transform.rotation=Quaternion.Euler(angle)*rotation;
                    goNew.transform.SetParent(gameObject.transform);

                    var vs1=MeshHelper.GetWorldVertexes(goNew);
                    var mm1=MeshHelper.GetMinMax(vs1);
                    var vs2=MeshHelper.GetWorldVertexes(goTo);
                    var mm2=MeshHelper.GetMinMax(vs2);
                    var scaleNew=new Vector3(mm2[2].x/mm1[2].x,mm2[2].y/mm1[2].y,mm2[2].z/mm1[2].z);

                    var dis=DistanceUtil.GetDistance(vs1,vs2);

                    string log=$"angle[{count}]:\t{angle},\tDistance:{dis},\tsize1:{mm1[2].Vector3ToString()},\tsize2:{mm2[2].Vector3ToString()},\tscale:{scaleNew.Vector3ToString()}";

                    if(dis==0){
                        Debug.LogError(log);
                    }
                    else if(dis<0.5f){
                        Debug.LogWarning(log);
                        goNew.name+="_"+angle+"_"+ dis;

                        //旋转之后的比例

                        // var goNew1 = MeshHelper.CopyGO(goNew);
                        // goNew1.transform.SetParent(goNew.transform);
                        // goNew1.transform.localScale=new Vector3(scale1.x*scaleNew.x,scale1.y*scaleNew.y,scale1.z*scaleNew.z);
                        // var dis2=MeshHelper.GetVertexDistanceEx(goNew1.transform,goTo.transform);
                        // Debug.LogWarning($"dis2:{dis2}");

                        Vector3 scaleNN = new Vector3(scale1.x * scaleNew.x, scale1.y * scaleNew.y, scale1.z * scaleNew.z);
                        var r1  = CreateNewScaleGo(goNew, new Vector3(scaleNN.x, scaleNN.y, scaleNN.z));
                        var r2 = CreateNewScaleGo(goNew, new Vector3(scaleNN.y, scaleNN.x, scaleNN.z));
                        var r3 = CreateNewScaleGo(goNew, new Vector3(scaleNN.z, scaleNN.y, scaleNN.x));
                        var r4 = CreateNewScaleGo(goNew, new Vector3(scaleNN.z, scaleNN.x, scaleNN.y));
                        var r5 = CreateNewScaleGo(goNew, new Vector3(scaleNN.x, scaleNN.z, scaleNN.y));
                        var r6 = CreateNewScaleGo(goNew, new Vector3(scaleNN.y, scaleNN.z, scaleNN.x));//6种情况

                        if (r1 == true || r2 == true || r3 == true || r4 == true || r5 == true || r6 == true)
                        {
                            GameObject.DestroyImmediate(goNew);
                        }

                        if(r1==false&&r2==false && r3 == false && r4 == false && r5 == false && r6 == false)
                        {
                            GameObject.DestroyImmediate(goNew);
                        }
                    }
                    else{
                        //Debug.Log(log);
                        GameObject.DestroyImmediate(goNew);
                    }
                    
                }
            }
        }
    }

    public void TryMatrixAngle0_0_0()
    {
        TryMatrixAngle(new Vector3(0, 0, 0));
    }

    public void TryMatrixAngle90_0_0()
    {
        TryMatrixAngle(new Vector3(90, 0, 0));
    }

    public void TryMatrixAngle180_0_0()
    {
        TryMatrixAngle(new Vector3(180, 0, 0));
    }

    public void TryMatrixAngle0_90_0()
    {
        TryMatrixAngle(new Vector3(0, 90, 0));
    }

    public void TryMatrixAngle0_0_90()
    {
        TryMatrixAngle(new Vector3(0, 0, 90));
    }

    public void TryMatrixAngle(Vector3 angle)
    {


        var mfFrom = goFrom.GetComponent<MeshFilter>();
        //var vsFrom=MeshHelper.GetWorldVertexes(mfFrom);

        var vsFrom = mfFrom.sharedMesh.vertices;
        var vsFromWorld = MeshHelper.GetWorldVertexes(vsFrom, mfFrom.transform);

        GameObject gameObject = new GameObject("TryAngle");
        MeshHelper.ShowVertexes(vsFromWorld, pScale, gameObject.transform);

        Matrix4x4 localMatrix = mfFrom.transform.localToWorldMatrix;
        Debug.LogError("localMatrix:\n" + localMatrix);

        Vector3 trans = goTo.transform.position - goFrom.transform.position;
        Debug.LogError("trans:\n" + trans);

        //Vector3 angle = new Vector3(90, 0, 0);
        Quaternion R = Quaternion.Euler(angle);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(angle), Vector3.one);

        List<Vector3> newVs = new List<Vector3>();
        List<Vector3> newVs2 = new List<Vector3>();
        List<Vector3> newVs3 = new List<Vector3>();

        Matrix4x4 matrix4World = localMatrix * matrix;

        for (int i = 0; i < vsFrom.Length; i++)
        {
            //Vector3 vN=matrix.MultiplyPoint(vsFrom[i]);
            Vector3 vN = matrix.MultiplyPoint3x4(vsFrom[i]);
            Vector3 v2N = localMatrix.MultiplyPoint3x4(vN) + trans;
            newVs.Add(vN);
            newVs2.Add(v2N);

            Vector3 v3N = matrix4World.MultiplyPoint3x4(vsFrom[i]) + trans;
            newVs3.Add(v3N);
        }

        var goNew = MeshHelper.CopyGO(goFrom);
        var mfNew = goNew.GetComponent<MeshFilter>();
        //mfNew.mesh.vertices=newVs.ToArray();

        RTResult rt = new RTResult();
        rt.Mode = AlignMode.Rotate;
        rt.Translation = trans;
        rt.TransformationMatrix = matrix4World;
        rt.ApplyMatrix(goNew.transform,null);

        GameObject gameObject2 = new GameObject("TryAngle1" + angle);
        MeshHelper.ShowVertexes(newVs2.ToArray(), pScale, gameObject2.transform);

        GameObject gameObject3 = new GameObject("TryAngle2" + angle);
        MeshHelper.ShowVertexes(newVs3.ToArray(), pScale, gameObject3.transform);
    }

    public Vector3[] ApplyMatrix(Vector3[] vs1,Matrix4x4 matrix4World,Vector3 trans)
    {
         Vector3[] newVs2=new Vector3[vs1.Length];
         for(int l=0;l<vs1.Length;l++){
            Vector3 v2N2=matrix4World.MultiplyPoint3x4(vs1[l])+trans;
            newVs2[l]=v2N2;
        }
        return newVs2;
    }

    public void ApplyMatrix(Vector3[] vs1,Matrix4x4 matrix4World,Vector3 trans,Vector3[] newVs2)
    {
         //Vector3[] newVs2=new Vector3[vs1.Length];
         for(int l=0;l<vs1.Length;l++){
            Vector3 v2N2=matrix4World.MultiplyPoint3x4(vs1[l])+trans;
            newVs2[l]=v2N2;
        }
        //return newVs2;
    }

    public void CreateNewScaleGoMatrix(Matrix4x4 localMatrix,Vector3[] vsFromLocal,Vector3[] vsToWorld,Vector3 angle,Vector3 scale,Vector3 trans)
    {
        Matrix4x4 matrix2=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),scale);
        Matrix4x4 matrix4World2=localMatrix*matrix2;
        Vector3[] newVs3=ApplyMatrix(vsFromLocal,matrix4World2,trans);
        var dis2=DistanceUtil.GetDistance(newVs3,vsToWorld);
        if(dis2<=0.000005){
            GameObject gameObject3=new GameObject("TryScale"+scale);
            MeshHelper.ShowVertexes(newVs3,pScale,gameObject3.transform);
            Debug.LogWarning($"dis2:{dis2},matrix4World:\n{matrix4World2}");
        }
    }

    public float pScale=0.005f;
    
    public void TryAnglesMatrix()
    {
        var mfFrom=goFrom.GetComponent<MeshFilter>();
        var mfTo=goTo.GetComponent<MeshFilter>();

        var vsToWorld=MeshHelper.GetWorldVertexes(mfTo);

        Matrix4x4 localMatrix=mfFrom.transform.localToWorldMatrix;
        Vector3 trans=goTo.transform.position-goFrom.transform.position;

        var vsFromLocal=mfFrom.sharedMesh.vertices;
        var vsFromWorld=MeshHelper.GetWorldVertexes(vsFromLocal,mfFrom.transform);

        SetDistanceSetting();
        GameObject gameObject=new GameObject("TryAngles");
        Quaternion rotation=goFrom.transform.rotation;
        for(int i=0;i<3;i++){
            for(int j=0;j<3;j++)
            {
                for(int k=0;k<3;k++)
                {
                    Vector3 angle=new Vector3(i*90,j*90,k*90);

                    Matrix4x4 matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),Vector3.one);

                    
                    Matrix4x4 matrix4World=localMatrix*matrix;

                    var scale1=goTo.transform.localScale;

                    // Vector3[] newVs=new Vector3[vsFromLocal.Length];
                    // Vector3[] newVs2=new Vector3[vsFromLocal.Length];
                    // for(int l=0;l<vsFromLocal.Length;l++){
                    //     //Vector3 vN=matrix.MultiplyPoint(vsFrom[i]);
                    //     Vector3 vN=matrix.MultiplyPoint3x4(vsFromLocal[l]);
                    //     Vector3 v2N1=localMatrix.MultiplyPoint3x4(matrix.MultiplyPoint3x4(vsFromLocal[l]))+trans;
                    //     Vector3 v2N2=matrix4World.MultiplyPoint3x4(vsFromLocal[l])+trans;
                    //     if(v2N2!=v2N1){
                    //         Debug.LogError("v2N2!=v2N1");
                    //     }
                    //     newVs[l]=vN;
                    //     newVs2[l]=v2N2;
                    // }

                    //Vector3[] newVs2=ApplyMatrix(vsFromLocal,matrix4World,trans);
                    Vector3[] newVs2=new Vector3[vsFromLocal.Length];
                    ApplyMatrix(vsFromLocal,matrix4World,trans,newVs2);

                    //GameObject gameObject2=new GameObject("TryAngle"+angle);
                    //MeshHelper.ShowVertexes(newVs2,0.01f,gameObject2.transform);
                    
                   
                    // goNew.transform.position=goTo.transform.position;
                    // goNew.transform.rotation=Quaternion.Euler(angle)*rotation;
                    // goNew.transform.SetParent(gameObject.transform);
                    var dis=DistanceUtil.GetDistance(newVs2,vsToWorld);
                    if(dis<0.5f){
                         var mm1=MeshHelper.GetMinMax(newVs2);
                        var mm2=MeshHelper.GetMinMax(vsToWorld);
                        var scaleNew=new Vector3(mm2[2].x/mm1[2].x,mm2[2].y/mm1[2].y,mm2[2].z/mm1[2].z);
                        Vector3 scaleNN=new Vector3(scale1.x*scaleNew.x,scale1.y*scaleNew.y,scale1.z*scaleNew.z);

                        Debug.LogWarning($"angle:{angle},Distance:{dis},scaleNew:{scaleNew.Vector3ToString()},matrix4World:\n{matrix4World}");

                        var goNew = MeshHelper.CopyGO(goFrom);
                        // Matrix4x4 matrix2=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),Vector3.one);

                        goNew.transform.position=goTo.transform.position;
                        // goNew.transform.rotation=Quaternion.Euler(angle)*rotation;

                        // goNew.transform.rotation = Quaternion.LookRotation(matrix4World.GetColumn(2), matrix4World.GetColumn(1))* rotation;
                        goNew.transform.rotation = Quaternion.LookRotation(matrix4World.GetColumn(1), matrix4World.GetColumn(2))* rotation;

                        GameObject gameObject2=new GameObject("TryAngle"+angle);
                        MeshHelper.ShowVertexes(newVs2,pScale,gameObject2.transform);

                        // goNew.transform.localToWorldMatrix=matrix4World;

                        // GameObject gameObject2=new GameObject("TryAngle"+angle);
                        // MeshHelper.ShowVertexes(newVs2,0.01f,gameObject2.transform);
                        // return;

                        //-----------------------------------------------------------
                        // Matrix4x4 matrix2=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),scaleNN);
                        // Matrix4x4 matrix4World2=localMatrix*matrix2;
                        // Vector3[] newVs3=ApplyMatrix(vsFromLocal,matrix4World2,trans);
                        // GameObject gameObject3=new GameObject("TryScale"+scaleNN);
                        // MeshHelper.ShowVertexes(newVs3,pScale,gameObject3.transform);
                        // var dis2=DistanceUtil.GetDistance(newVs3,vsToWorld);
                        // Debug.LogWarning($"dis2:{dis2},matrix4World:\n{matrix4World2}");

                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.x,scaleNN.y,scaleNN.z));
                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.y,scaleNN.x,scaleNN.z));
                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.z,scaleNN.y,scaleNN.x));
                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.z,scaleNN.x,scaleNN.y));
                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.x,scaleNN.z,scaleNN.y));
                        // CreateNewScaleGo(goNew,new Vector3(scaleNN.y,scaleNN.z,scaleNN.x));//6种情况

                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.x,scaleNN.y,scaleNN.z),trans);
                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.y,scaleNN.x,scaleNN.z),trans);
                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.z,scaleNN.y,scaleNN.x),trans);
                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.z,scaleNN.x,scaleNN.y),trans);
                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.x,scaleNN.z,scaleNN.y),trans);
                        CreateNewScaleGoMatrix(localMatrix,vsFromLocal,vsToWorld,angle,new Vector3(scaleNN.y,scaleNN.z,scaleNN.x),trans);
                    }
                    else{
                        Debug.Log($"angle:{angle},Distance:{dis}");
                        //GameObject.DestroyImmediate(goNew);
                    }
                    
                }
            }
        }
    }

    public float distance = 0;

    public bool ShowLog = false;

    public void GetDistance12()
    {
        DateTime start = DateTime.Now;
        SetDistanceSetting();
        distance=MeshHelper.GetVertexDistanceEx(goTo.transform,goFrom.transform,"GetDistance12", ShowLog);
        Debug.Log($"GetDistance12 distance:{distance} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms log:{DistanceUtil.DisLog}");
    }

    private void SetDistanceSetting()
    {
        //DistanceSetting.zeroP=this.zeroP;
        //DistanceSetting.zeroM = this.zeroM;
        //DistanceSetting.zeroMMaxDis = this.zeroPMaxDis;

        DistanceSetting.Set(this.disSetting);
    }

    public void GetDistance12C()
    {
        DateTime start = DateTime.Now;

        if (goFromCopy == null)
        {
            DestroyGO2Copy();
            goFromCopy = MeshHelper.CopyGO(goFrom);
        }
        

        SetDistanceSetting();
        distance = MeshHelper.GetVertexDistanceEx(goTo.transform,goFromCopy.transform,"GetDistance12C",true);
        Debug.Log($"GetDistance12 distance:{distance} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms log:{DistanceUtil.DisLog}");
    }

    public void GetDistance22C()
    {
        DateTime start = DateTime.Now;
        DestroyGO2Copy();
        goFromCopy = MeshHelper.CopyGO(goFrom);
        SetDistanceSetting();
        distance = MeshHelper.GetVertexDistanceEx(goFrom.transform,goFromCopy.transform,"GetDistance22C",true);
        Debug.Log($"GetDistance12 distance:{distance} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms log:{DistanceUtil.DisLog}");
    }

    public void SwitchGO()
    {
        GameObject temp=goTo;
        goTo=goFrom;
        goFrom=temp;
    }

    private void DestroyGO2Copy()
    {
        if (goFromCopy && goFromCopy!=goFrom)
        {
            if (goFromCopy.transform.parent != null && goFromCopy.transform.parent.name == goFromCopy.name + "_TempCenter")
            {
                GameObject.DestroyImmediate(goFromCopy.transform.parent.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(goFromCopy);
            }
        }
    }

    public void CopyGo2()
    {
        // go2 = GameObject.Instantiate(go2);
        DestroyGO2Copy();
        goFromCopy = MeshHelper.CopyGO(goFrom);
        goFromCopy.SetActive(true);
    }

    public bool IsSetParentNull=true;

    public void CopyTest()
    {
        Transform t2=MeshHelper.CopyT(goTo.transform);
        Debug.Log(t2);
    }

    public void CreatePlaneByThreePoint()
    {
        CopyGo2();

        MeshHelper.SetParentZero(goTo);
        MeshHelper.SetParentZero(goFromCopy);

        Transform t1=goTo.transform;
        Transform t2=goFromCopy.transform;
        var mf1=new MeshPoints(goTo);
        var mf2=new MeshPoints(goFromCopy);

        MeshJobHelper.NewThreePointJobs(new MeshPoints[]{mf1,mf2},10000);
        // MeshJobs.NewMeshAlignJob(go1, go2Copy,true);

        ThreePointJobResult r1=ThreePointJobResultList.Instance.GetThreePointResult(mf1);
        r1.localToWorldMatrix=t1.localToWorldMatrix;

        ThreePointJobResult r2=ThreePointJobResultList.Instance.GetThreePointResult(mf2);
        r2.localToWorldMatrix=t2.localToWorldMatrix;

        var ps1 = r1.GetThreePoints(t1.localToWorldMatrix);
        var ps2 = r2.GetThreePoints(t2.localToWorldMatrix); 

        MeshHelper.ClearChildren(t1);
        MeshHelper.ClearChildren(t2);
        CreatePlaneByThreePoints(ps1,t1);
        CreatePlaneByThreePoints(ps2,t2);
    }



    public AcRigidTransform acRigidTransform;

    public int RTTestMode=0;





    public int maxTestCount=1;

    public void AcRTAlign()
    {

        CopyGo2();
        MeshAlignHelper.AcRTAlign(goFromCopy, goTo);

    }

    public void AcRTAlignJob()
    {
        CopyGo2();
        MeshAlignHelper.AcRTAlignJob(goFromCopy, goTo);
    }

    public void AcRTAlignJob(GameObject from, GameObject to)
    {
        SetDistanceSetting();
        SetGos(from, to);

        //goFromCopy = from;

        //AcRTAlignJob();   

        //CopyGo2();
        MeshAlignHelper.AcRTAlignJob(from, goTo);
    }



    private void CreatePlaneByThreePoints(ThreePoint[] list,Transform parent)
    {
        for(int l=0;l<list.Length;l++)
        {
            var id1=list[l];

            var rt=ThreePointHelper.GetRT(ref id1,parent);
            //rt.SetParent(parent);

            var plane=ThreePointHelper.CreateNormalPlane(ref id1,parent,true);
            plane.name+=$"{l}({id1.maxId},{id1.minId})";
        }
    }

    public void TestRTAlignJob()
    {
        SetDistanceSetting();
        CopyGo2();

        MeshHelper.SetParentZero(goTo);
        MeshHelper.SetParentZero(goFromCopy);

        Transform t1=goTo.transform;
        Transform t2=goFromCopy.transform;
        var mf1=new MeshPoints(goTo);
        var mf2=new MeshPoints(goFromCopy);

        MeshJobHelper.NewThreePointJobs(new MeshPoints[]{mf1,mf2},JobSize);
        MeshJobHelper.NewRTAlignJobs(mf1, mf2,JobSize);
    }
    

    public int JobSize=10000;


    public void TestMeshAlignJob()
    {
        CopyGo2();

        MeshHelper.SetParentZero(goTo);
        MeshHelper.SetParentZero(goFromCopy);

        MeshJobHelper.NewThreePointJobs(new GameObject[]{goTo,goFromCopy},JobSize);
        MeshJobHelper.NewMeshAlignJob(goTo, goFromCopy,true);
    }

    public void AlignModels()
    {
        InitMeshNodeInfoEx();
        MeshHelper.AlignMeshNode(node1, node2, TryRotateAngleCount, ShowDebugDetail,true);
    }

    private void InitMeshNodeInfoEx()
    {
        SetDistanceSetting();
        MeshHelper.step = this.step;
        MeshHelper.mode = this.mode;
        //DistanceSetting.zeroP=this.zeroP;
        DistanceSetting.Set(this.disSetting);

        if (IsSetParentNull)
        {
            // go1.transform.SetParent(null);
            // go2.transform.SetParent(null);

            MeshHelper.SetParentZero(goTo);
            MeshHelper.SetParentZero(goFrom);
        }

        CopyGo2();
        InitMeshNodeInfo();
    }

    // [ContextMenu("AlignModels2")]
    // public void AlignModels2()
    // {
    //     InitMeshNodeInfo();
    //     AlignMeshNode(node1,node2,TryRotateAngleCount);
    // }

    public int MaxICPCount=5;

    public void TestICP()
    {
        SetDistanceSetting();

        DateTime start=DateTime.Now;
         CopyGo2();

        var pTo = goTo.transform.parent;
        var pFrom = goFromCopy.transform.parent;

        MeshHelper.SetParentZero(goTo);
        MeshHelper.SetParentZero(goFromCopy);

        //goFromCopy.transform.position = goTo.transform.position;//这个加不加差不多

        var vsFrom = MeshHelper.GetWorldVertexes(goFromCopy);
        //MeshHelper.ShowVertexes(vsFrom, pScale, "vsFrom");
        var vsTo = MeshHelper.GetWorldVertexes(goTo);
        //MeshHelper.ShowVertexes(vsTo, pScale, "vsTo");
        var dis1 = DistanceUtil.GetDistance(vsFrom, vsTo);
        Debug.LogError("TestICP2 dis1:"+dis1);

        GameObject goOld=goFromCopy;
        RTResultList rList=new RTResultList();
        for(int i=0;i<MaxICPCount;i++)
        {
            DateTime start1=DateTime.Now;
            float progress = (float)i / MaxICPCount;
            float percents = progress * 100;
            if(ProgressBarHelper.DisplayCancelableProgressBar("TestICP2", $"{i}/{MaxICPCount} {percents:F2}% of 100%", progress))
            {
                break;
            }

            //DateTime start21=DateTime.Now;
            //var psList=vsFrom.ToList();
            //Debug.LogError($"Time0:{(DateTime.Now-start21).TotalMilliseconds}ms");

            //DateTime start2=DateTime.Now;
            var vsFromCP1 = DistanceUtil.GetClosedPoints(vsTo, vsFrom.ToList());
            //Debug.LogError($"Time1:{(DateTime.Now-start2).TotalMilliseconds}ms");

            //DateTime start3=DateTime.Now;
            //MeshHelper.ShowVertexes(vsFromCP1, pScale, "vsFromCP_"+(i+1));
            var r1 = AcRigidTransform.ApplyTransformationN(vsFromCP1, vsTo);
            rList.Add(r1);
            //Debug.LogError($"Time2:{(DateTime.Now-start3).TotalMilliseconds}ms");

            var vsFromNew1 = r1.ApplyPoints(vsFrom);

            //MeshHelper.ShowVertexes(vsFromNew1, pScale, "vsFromNew_"+(i+1));

            // var goNew=MeshHelper.CopyGO(goOld);
            // goNew.name="vsFromNew_"+(i+1);
            // r1.ApplyMatrix(goNew.transform);
            // goOld=goNew;
            // MeshHelper.ShowVertexes(vsFromNew1, pScale, goNew.transform);

            var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo,i==MaxICPCount-1);
            //Debug.LogError($"TestICP1 dis{i+1}:" + dis2);
            Debug.LogError($"dis[{i+1}] dis:{dis2}, Time:{(DateTime.Now-start1).TotalMilliseconds}ms");
            vsFrom=vsFromNew1;
            if(dis2<=0.000001)
            {
                break;
            }
            
        }

        rList.ApplyMatrix(goOld.transform, goTo.transform);

        goTo.transform.parent = pTo;
        goFromCopy.transform.parent = pFrom;

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"TestICP2 End Time:{(DateTime.Now-start).TotalMilliseconds}ms");
    }

    public void ReplacePrefab()
    {
        InitMeshNodeInfo();
        node1.ClearChildren();
        node2.ClearChildren();
        MeshHelper.ReplaceToPrefab(node1.gameObject, node2.gameObject, TryRotateAngleCount, centerOffset, ShowDebugDetail);
    }

    public void AlignMeshNode(MeshNode node1,MeshNode node2,int tryCount)
    {
        SetDistanceSetting();
        DateTime start=DateTime.Now;
        Debug.Log("AlignMeshNode Start");
        Vector3 longLine1 = node1.GetLongLine();
        Vector3 longLine2 = node2.GetLongLine();

        Quaternion qua1 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());
        node2.transform.rotation = qua1 * node2.transform.rotation;

        //Quaternion qua2 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());
        //node2.transform.rotation = qua2 * node2.transform.rotation;

        //Quaternion qua3 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());
        //node2.transform.rotation = qua3 * node2.transform.rotation;

        //Quaternion qua4 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());
        //node2.transform.rotation = qua4 * node2.transform.rotation;

        Vector3 offset = node1.GetCenterP() - node2.GetCenterP();
        node2.transform.position += offset;


        int j=0;
        float angle=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
        for(;j<tryCount;j++)
        {
            //Debug.Log($"[{j}]angle:{angle}");
            node2.transform.RotateAround(node1.GetCenterP(), node2.GetLongLine(), -angle);
            angle = Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
            if (angle <= 0) {
                Debug.Log($"[{j}]angle:{angle}");
                break;
            }
        }
        if(j>=tryCount)
        {
            Debug.LogError($"[j>=tryCount]angle:{angle}");
        }
        Debug.Log($"AlignMeshNode End Time:{(DateTime.Now-start).TotalMilliseconds}ms");
    }

    public void Rotate2()
    {
        float angle = Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
        Debug.Log($"[Rotate2]angle:{angle}");
        node2.transform.RotateAround(node1.GetCenterP(), node2.GetLongLine(), -angle);
        angle = Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
        Debug.Log($"[Rotate2]angle:{angle}");
    }
}

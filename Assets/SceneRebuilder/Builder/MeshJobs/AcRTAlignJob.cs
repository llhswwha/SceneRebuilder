using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
using static MeshJobs.MeshJobHelper;
using System.Linq;

namespace MeshJobs
{
    public static class AcRTAlignJobResult
    {
        public static int ID = 0;

        public static int GetId()
        {
            int id = ID;
            ID++;
            return id;
        }

        private static Dictionary<int, IRTResult> Results = new Dictionary<int, IRTResult>();

        public static IRTResult[] rs;

        public static void SetResult(int id, IRTResult r)
        {
            //  Debug.LogError("AcRTAlignJobResult SetResult:"+id+","+r);

            if(rs==null){
                rs=new IRTResult[10];
            }
            if (r != null)
            {
                r.id = id;
            }
            // else{
            //     Debug.LogError("SetResult r==null:"+id);
            // }
            //rs.Add(r);
            

            if (rs.Length-1 < id)
            {
                Debug.LogError($"AcRTAlignJobResult rs.Length-1 < id id:{id} length:{rs.Length} r:{r}");
                var newRs=new IRTResult[id+1];
                for(int i=0;i< newRs.Length && i<rs.Length;i++)
                {
                    newRs[i] = rs[i];
                }
                rs = newRs;

                rs[id] = r;
            }
            else
            {
                rs[id] = r;
            }
           

            //if(Results.ContainsKey(id)){
            //    Results[id]=r;
            //}
            //else{
            //    Results.Add(id,r);
            //}
        }

        public static void InitDict()
        {
            if (Results.Count > 0) return;
            if (rs == null)
            {
                Debug.LogError("AcRTAlignJobResult.InitDict rs==null");
                return;
            }
            for (int i = 0; i < rs.Length; i++)
            {
                IRTResult item = rs[i];
                if (item == null)
                {
                    Results.Add(i, null);
//                    Debug.Log($"InitDict2 {i},null");
                    continue;
                }
                Results.Add(item.id, item);
                //Debug.Log($"InitDict1 {item.id},{item}");
            }
        }

        public static void CleanResults()
        {
            //rs.Clear();
            Results.Clear();
        }

        public static IRTResult GetResult(int id)
        {
            InitDict();
            //Debug.LogError("GetResult id:"+id);
            if (Results.ContainsKey(id))
            {
                var r=Results[id];
                // if(r==null){
                //     Debug.LogError("GetResult r==null:"+id);
                // }
                //Debug.LogError("GetResult r:"+r);
                return Results[id];
            }
            Debug.LogError($"GetResult Results.ContainsKey(id)==false, {Results.Count},{rs.Length}");
            //foreach (var item in Results.Keys)
            //{
            //    Debug.LogError($"key:{item}");
            //}
            //foreach (var item in rs)
            //{
            //    Debug.LogError($"item:{item.id}");
            //}
            foreach (var item in rs)
            {
                if (item == null) continue;
                if (item.id == id)
                {
                    Results.Add(id, item);
                    return item;
                }
            }
            return null;

        }

        internal static void InitCount(int count)
        {
            rs = new IRTResult[count];
        }

        //public static float MinDis=0.1f;
    }

    public class AcRtAlignJobArg
    {
        public int id;

        public MeshPoints mfFrom;

        public MeshPoints mfTo;

        public AcRtAlignJobArg(int i, MeshPoints from, MeshPoints to)
        {
            this.id = i;
            this.mfFrom = from;
            this.mfTo = to;
        }

        public static Dictionary<int, AcRtAlignJobArg> Dicts = new Dictionary<int, AcRtAlignJobArg>();

        public static AcRtAlignJobArg SaveArg(int i, MeshPoints from, MeshPoints to)
        {
            AcRtAlignJobArg arg = new AcRtAlignJobArg(i, from, to);
            Dicts.Add(i, arg);
            return arg;
        }

        public static AcRtAlignJobArg LoadArg(int id)
        {
            if (Dicts.ContainsKey(id))
            {
                return Dicts[id];
            }
            Debug.LogError("AcRtAlignJobArg.LoadArg NotFound:");
            return null;
        }

        public static void CleanArgs()
        {
            Dicts.Clear();
        }
    }

    public class AcRTAlignJobPrefab
    {
        public int Id;

        public PrefabInfo PrefabInfo;

        public int VerticesCount;

        public MeshFilterList List;

        public void RemoveMeshFilter(MeshPoints mf)
        {
            List.Remove(mf);
        }

        public void AddInstance(GameObject instance)
        {
            PrefabInfo.AddInstance(instance);
        }

        public static Dictionary<int, AcRTAlignJobPrefab> Items = new Dictionary<int, AcRTAlignJobPrefab>();

        public static void Clear()
        {
            Items.Clear();
        }

        public static void AddItem(int id, PrefabInfo preInfo, MeshFilterList mfl)
        {
            //Debug.Log($"AcRTAlignJobPrefab.AddItem jobId:{jobId},meshFilter:{mfl.Count}");
            AcRTAlignJobPrefab item = new AcRTAlignJobPrefab();
            item.Id = id;
            item.PrefabInfo = preInfo;
            item.List = mfl;
            Items.Add(id, item);
        }

        public static AcRTAlignJobPrefab GetItem(int id)
        {
            if (Items.ContainsKey(id))
            {
                return Items[id];
            }
            Debug.LogError("AcRTAlignJobPrefab.GetItem Not Found jobId:" + id);
            return null;
        }
    }


    public struct AcRTAlignJob : IJob
    {
        public static float MaxAngle = 10;

        public int Id;
        // [ReadOnly]public NativeArray<ThreePoint> tpsFrom;

        // [ReadOnly] public NativeArray<ThreePoint> tpsTo;

        public int vsFromId;

        //[ReadOnly] public NativeArray<Vector3> vsFrom;

        public int vsToId;

        //[ReadOnly] public NativeArray<Vector3> vsTo;

        public static double totalTime1 = 0;
        public static double totalTime2 = 0;
        public static double totalTime3 = 0;

        public static int totalFoundCount = 0;
        public static int totalNoFoundCount = 0;

        public static double totalFoundTime = 0;
        public static double totalNoFoundTime = 0;

        public static int AngleCount=0;

        public static int ScaleCount=0;

        public static int RTCount=0;

        public static int ICPCount=0;

        public static bool IsTryAngles= true;

        public static bool IsTryAngles_Scale=true;

        public void Dispose()
        {
            //vsFrom.Dispose();
            //vsTo.Dispose();
            // tpsFrom.Dispose();
            // tpsTo.Dispose();
        }

        public static void ResetTotalData()
        {
            totalTime1 = 0;
            totalTime2 = 0;
            totalTime3 = 0;

            totalFoundCount = 0;
            totalNoFoundCount = 0;

            totalFoundTime = 0;
            totalNoFoundTime = 0;

            AngleCount=0;
            ScaleCount=0;
            RTCount=0;
            ICPCount=0;
        }

        // public Vector3[] ApplyMatrix(Vector3[] vs1,Matrix4x4 matrix4World,Vector3 trans)
        // {
        //     Vector3[] newVs2=new Vector3[vs1.Length];
        //     for(int l=0;l<vs1.Length;l++){
        //         Vector3 v2N2=matrix4World.MultiplyPoint3x4(vs1[l])+trans;
        //         newVs2[l]=v2N2;
        //     }
        //     return newVs2;
        // }
        public void ApplyMatrix(Vector3[] vs1,Matrix4x4 matrix4World,Vector3 trans,Vector3[] newVs2)
        {
            //Vector3[] newVs2=new Vector3[vs1.Length];
            for(int l=0;l<vs1.Length;l++){
                Vector3 v2N2=matrix4World.MultiplyPoint3x4(vs1[l])+trans;
                newVs2[l]=v2N2;
            }
            //return newVs2;
        }

        //private NativeArray<Vector3> newVs2;

        //private Vector3[] newVs2;

        public RTResult CreateNewScaleGoMatrix(Matrix4x4 localMatrix,Vector3[] vsFromLocal,Vector3[] vsToWorld,Vector3 angle,Vector3 scale,Vector3 trans,Vector3[] newVs2)
        {
            Matrix4x4 matrix2=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),scale);
            Matrix4x4 matrix4World2=localMatrix*matrix2;
            ApplyMatrix(vsFromLocal,matrix4World2,trans,newVs2);
            var dis2=DistanceUtil.GetDistance(newVs2,vsToWorld);
            if(dis2<=DistanceSetting.zeroM){//0.000005
                //GameObject gameObject3=new GameObject("TryScale"+scale);
                //MeshHelper.ShowVertexes(newVs3,pScale,gameObject3.transform);
                //Debug.LogWarning($"dis2:{dis2},matrix4World:\n{matrix4World2}");
                //return true;
                RTResult result=new RTResult();
                result.Mode= AlignMode.Scale;
                result.Distance=dis2;
                result.IsZero=true;
                result.TransformationMatrix=matrix4World2;
                result.Translation=trans;
                result.Scale=scale;
                //AcRTAlignJobResult.SetResult(Id, result);
                return result;
            }
            else{
                RTResult result=new RTResult();
                result.IsZero=false;
                return result;
            }
        }

        public static void SetStatisticsInfo(double t,bool isFoundZero,int foundType)
        {
            //double t = (DateTime.Now - start).TotalMilliseconds;
            if (isFoundZero)
            {
                totalFoundCount++;
                totalFoundTime += t;

                if(foundType==1){
                    AngleCount++;
                }
                else if(foundType==2){
                    ScaleCount++;
                }
                else if(foundType==3){
                    RTCount++;
                }
                else if(foundType==4){
                    ICPCount++;
                }
            }
            else
            {
                totalNoFoundCount++;
                totalNoFoundTime += t;
            }
        }

        public void Execute()
        {
            //DateTime start = DateTime.Now;

            int count = 0;

            float minDis = float.MaxValue;
            RTResult minRT = null;
            Vector3[] minVS=null;
            bool isFoundZero = false;

            var tpsFrom=AcRTAlignJobHelper.tpDict[vsFromId];
            var tpsTo=AcRTAlignJobHelper.tpDict[vsToId];

            var vsFromW = AcRTAlignJobHelper.vsDictWorld[vsFromId];
            var vsFromL=AcRTAlignJobHelper.vsDictLocal[vsFromId];
            var vsToW = AcRTAlignJobHelper.vsDictWorld[vsToId];

            var scale1=AcRTAlignJobHelper.scaleDict[vsToId];

            if(IsTryAngles){
                var trans=AcRTAlignJobHelper.posDict[vsToId]-AcRTAlignJobHelper.posDict[vsFromId];
                Matrix4x4 localMatrix=tpsFrom[0].localToWorldMatrix;
                int aCount=0;

                Vector3[] newVs2=new Vector3[vsFromL.Length];
                //NativeArray<Vector3> newVs2=new NativeArray<Vector3>(vsFromL.Length,Allocator.Temp);

                for(int i=0;i<4;i++)
                {
                    for(int j=0;j<4;j++)
                    {
                        for(int k=0;k<4;k++)
                        {
                            aCount++;
                            Vector3 angle=new Vector3(i*90,j*90,k*90);
                            Matrix4x4 matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(angle),Vector3.one);
                            Matrix4x4 matrix4World=localMatrix*matrix;
                            // for(int l=0;l<vsFromL.Length;l++){
                            //     //Vector3 vN=matrix.MultiplyPoint(vsFrom[i]);
                            //     //Vector3 vN=matrix.MultiplyPoint3x4(vsFromL[l]);
                            //     //Vector3 v2N1=localMatrix.MultiplyPoint3x4(vN)+trans;
                            //     Vector3 v2N2=matrix4World.MultiplyPoint3x4(vsFromL[l])+trans;
                            //     // if(v2N2!=v2N1){
                            //     //     var vDis=Vector3.Distance(v2N1,v2N2);
                            //     //     Debug.LogError($"v2N2!=v2N1 {vN},{v2N1.Vector3ToString()},{v2N2.Vector3ToString()},{vDis}");
                            //     //     //v2N2!=v2N1 (-0.2, 0.0, 0.0),(3242.288,28.68081,3207.155),(3242.288,28.68081,3207.155),0.0002441406
                            //     // }
                            //     //newVs[l]=vN;
                            //     newVs2[l]=v2N2;
                            // }

                            //Vector3[] newVs2=ApplyMatrix(vsFromL,matrix4World,trans);
                            ApplyMatrix(vsFromL,matrix4World,trans,newVs2);

                            var dis=DistanceUtil.GetDistance(newVs2,vsToW);
                            if(dis==0){
                                //Debug.LogWarning($"angle[{aCount}]:{angle},Distance:{dis},Id:{Id},trans:{trans}\nmatrix:\n{matrix}\nlocalMatrix:\n{localMatrix}\nmatrix4World:\n{matrix4World}");
                                RTResult result=new RTResult();
                                result.Mode= AlignMode.Rotate;
                                result.Distance=dis;
                                result.IsZero=true;
                                result.TransformationMatrix=matrix4World;
                                result.NewVecties = newVs2;
                                result.Translation=trans;

                                //RTResult rt = RTResult.ApplyAngleMatrix(trans, matrix4World, newVs2.ToArray(), goNew.transform);

                                AcRTAlignJobResult.SetResult(Id, result);

                                //AngleCount++;
                                //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,1);
                                return;
                            }
                            else if(dis<0.5f && IsTryAngles_Scale)//比例
                            {
                                var mm1=MeshHelper.GetMinMax(newVs2);
                                var mm2=MeshHelper.GetMinMax(vsToW);
                                var scaleNew=new Vector3(mm2[2].x/mm1[2].x,mm2[2].y/mm1[2].y,mm2[2].z/mm1[2].z);
                                Vector3 scaleNN=new Vector3(scale1.x*scaleNew.x,scale1.y*scaleNew.y,scale1.z*scaleNew.z);
                                RTResult r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.x,scaleNN.y,scaleNN.z),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //ScaleCount++;
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                                r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.y,scaleNN.x,scaleNN.z),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                                r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.z,scaleNN.y,scaleNN.x),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                                r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.z,scaleNN.x,scaleNN.y),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                                r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.x,scaleNN.z,scaleNN.y),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                                r2 = CreateNewScaleGoMatrix(localMatrix,vsFromL,vsToW,angle,new Vector3(scaleNN.y,scaleNN.z,scaleNN.x),trans,newVs2);
                                if(r2.IsZero)
                                {
                                    AcRTAlignJobResult.SetResult(Id, r2);
                                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,true,2);
                                    return;
                                } 
                            }
                            else{
                                //Debug.Log($"angle:{angle},Distance:{dis}");
                                //GameObject.DestroyImmediate(goNew);
                            }
                        }
                    }
                }
            }
            

            //长短轴
            for (int l = 0; l < tpsFrom.Length; l++)
            {
                var tpFrom = tpsFrom[l];
                for (int k = 0; k < tpsTo.Length; k++)
                {
                    var tpTo = tpsTo[k];
                    count++;
                    //var matrix=GetRTMatrix(tpFrom,tpTo);//核心,基于脚本的

                    var angle1 = tpFrom.GetLongShortAngle();
                    var angle2 = tpTo.GetLongShortAngle();
                    var angle22 = math.abs(angle1 - angle2);

                    if (angle22 > MaxAngle)
                    {
                        continue;//下一个了，角度都不对其
                    }

                    //DateTime tmpT = DateTime.Now;
                    var rt = AcRigidTransform.GetRTMatrixS(tpFrom, tpTo);//核心，静态函数的
                    //totalTime1 += (DateTime.Now - tmpT).TotalMilliseconds;

                    //tmpT = DateTime.Now;
                   

                    var vsNew = rt.ApplyPoints(vsFromW);
                    //totalTime2 += (DateTime.Now - tmpT).TotalMilliseconds;

                    //tmpT = DateTime.Now;
                    
                    var dis = DistanceUtil.GetDistance(vsToW, vsNew, false);

                    //Debug.Log(string.Format("AcRTAlignJob[{6}][{4}][{5}] angle1:{0}\tangle2:{1}\tangle1_2:{2}\tdistance:{3}",angle1,angle2,angle22,dis,l,k,count));

                    //float4x4 float4X4=new float4x4();

                    //var dis= MathematicsDistance.GetDistance(vsTo.ToArray(),vsNew);

                    //totalTime3 += (DateTime.Now - tmpT).TotalMilliseconds;

                    //7420,2777,15732
                    //8427,3447,16070

                    rt.Distance = dis;
                    if (dis < minDis)
                    {
                        minDis = dis;
                        minRT = rt;
                        minVS=vsNew;
                    }

                    //Debug.LogError($">>>AcRTAlignJob [{l},{k}]\tIsZero:{rt.IsZero},\tIsReflection:{rt.IsReflection},\tdis:{dis}");
                    if (dis == 0)
                    {
                        isFoundZero = true;
                        //break;
                    }

                    AcRTAlignJobExResult.disJobCount++;
                }
                if (isFoundZero)
                {
                    //break;
                }
                //break;
            }

            if(minDis<DistanceSetting.ICPMinDis && minDis>DistanceSetting.zeroM){
                // if(minDis<DistanceSetting.ICPMinDis)
                {
                    //Debug.LogError("minDis>DistanceSetting.zeroDis:"+minDis+">"+DistanceSetting.zeroDis+" and <"+DistanceSetting.ICPMinDis);
                    RTResultList rList=new RTResultList();
                    rList.Add(minRT);
                    var vsFromStart=minVS;
                    string log="";

                    //int equalCount = 0;
                     for(int i=0;i<DistanceSetting.ICPMaxCount;i++)
                    {
                        //DateTime start1=DateTime.Now;
                        var vsFromCP1 = DistanceUtil.GetClosedPoints(vsToW, vsFromStart.ToList());
                        var r1 = AcRigidTransform.ApplyTransformationN(vsFromCP1, vsToW);
                        rList.Add(r1);
                        var vsFromNew1 = r1.ApplyPoints(vsFromStart);
                        //var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsToW,i==DistanceSetting.ICPMaxCount-1);
                        var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsToW);
                        //Debug.LogError($"icp[{i+1}] dis:{dis2}, Time:{(DateTime.Now-start1).TotalMilliseconds}ms");
                        log+=string.Format("{0:F5}; ",dis2);
                        vsFromStart=vsFromNew1;
                        r1.Distance=dis2;
                        rList.Distance=dis2;
                        if(dis2<=DistanceSetting.zeroM)
                        {
                            isFoundZero=true;
                            break;
                        }
                    }

                    Debug.LogError($"ICP({DistanceSetting.zeroM:F4},{DistanceSetting.ICPMinDis:F2}) IsFoundZero:[{isFoundZero}],\tstartDis:{minDis},\tendDis:{rList.Distance},\tlog:{log}");
                    
                    if(rList.Distance<minRT.Distance){ //ICP算法处理后更加匹配了
                        //ICPCount++;
                        AcRTAlignJobResult.SetResult(Id, rList);

                        //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,isFoundZero,4);

                        // double t = (DateTime.Now - start).TotalMilliseconds;
                        // if (isFoundZero)
                        // {
                        //     totalFoundCount++;
                        //     totalFoundTime += t;
                        // }
                        // else
                        // {
                        //     totalNoFoundCount++;
                        //     totalNoFoundTime += t;
                        // }
                        return;
                    }
                }
            }

            {
                //double t = (DateTime.Now - start).TotalMilliseconds;
                //if(minDis<AcRTAlignJobResult.MinDis)
                {
                    // RTCount++;
                    //SetStatisticsInfo((DateTime.Now - start).TotalMilliseconds,isFoundZero,3);
                    AcRTAlignJobResult.SetResult(Id, minRT);
                    //Debug.LogWarning(string.Format("IsFoundZero:{0},Distance:{1}",isFoundZero,minDis));
                    // if (isFoundZero)
                    // {
                    //     totalFoundCount++;
                    //     totalFoundTime += t;
                    // }
                    // else
                    // {
                    //     totalNoFoundCount++;
                    //     totalNoFoundTime += t;
                    // }
                }
            }

            //Debug.LogError($"Return AcRTAlignJob Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");

            //tpsFrom.Dispose();
            //tpsTo.Dispose();
        }


        
    }
}

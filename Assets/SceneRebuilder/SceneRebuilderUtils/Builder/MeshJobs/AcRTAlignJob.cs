using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
//using static MeshJobs.MeshJobHelper;
using System.Linq;
using System.Text;
using CommonUtils;
using CommonExtension;

namespace MeshJobs
{




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

        public static int MirrorCount = 0;

        public static int RTCount=0;

        public static int ICPCount=0;

        public static bool IsTrySameAngle = true;

        public static bool IsTryAngles= true;

        public static bool IsTryAngles_Scale=true;

        public static float TryScaleMaxDis = 2f;

        public static bool IsTryAngles_Mirror = true;

        public static bool IsTryRT = true;

        public static bool IsTryICP = true;

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

            jobCount = 0;
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
                result.NewVecties = newVs2;
                result.Distance=dis2;
                result.IsZero=true;
                result.TransformationMatrix=matrix4World2;
                result.Translation=trans;
                result.Scale=scale;
                result.Angle = angle;
                //AcRTAlignJobResult.SetResult(Id, result);
                return result;
            }
            else{
                RTResult result=new RTResult();
                result.IsZero=false;
                return result;
            }
        }

        public static void ResetLoopData()
        {
            loopStartTime = DateTime.Now;
            loopJobCount = 0;
        }

        public static DateTime loopStartTime;

        public static int loopJobCount = 0;

        public static int AlignJobCount = 0;

        public static int jobCount = 0;

        //public static ProgressArg progressArg;



        public static void SetStatisticsInfo(int id,double t,bool isFoundZero, AlignMode foundType,float dis,string info="")
        {
            jobCount++;
            loopJobCount++;

            //ProgressArg subProgress = new ProgressArg("LoopJob", loopJobCount, AlignJobCount);
            //progressArg.AddSubProgress(subProgress);
            ////ProgressBarHelper.DisplayCancelableProgressBar(progressArg);

            
            //Debug.Log($"SetStatisticsInfo[{id}][{jobCount}][{loopJobCount}/{AlignJobCount}][{(DateTime.Now - loopStartTime).TotalMilliseconds:F1}] isFoundZero:{isFoundZero} foundType:{foundType} t:{t:F1}ms dis:{dis} info:{info}");
            
            //double t = (DateTime.Now - start).TotalMilliseconds;
            if (isFoundZero)
            {
                totalFoundCount++;
                totalFoundTime += t;

                if(foundType== AlignMode.Rotate)
                {
                    AngleCount++;
                }
                else if(foundType== AlignMode.Scale)
                {
                    ScaleCount++;
                }
                else if(foundType== AlignMode.RT)
                {
                    RTCount++;
                }
                else if(foundType== AlignMode.ICP)
                {
                    ICPCount++;
                }
                else if (foundType == AlignMode.Mirror)
                {
                    MirrorCount++;
                }
            }
            else
            {
                totalNoFoundCount++;
                totalNoFoundTime += t;
            }
        }

        public static int ProgressIndex = 0;

        public static int ProgressMax = 1;

        public static bool IsCancel = false;

        public void Execute()
        {
            ProgressIndex++;

            //Debug.LogError($"！！！！！！ IsTryAngles:{IsTryAngles} IsTryAngles_Scale:{IsTryAngles_Scale}");

            DateTime start = DateTime.Now;

            int count = 0;

            float minDis = float.MaxValue;
            IRTResult minRT = null;
            Vector3[] minVS=null;
            bool isFoundZero = false;

            var tpsFrom=AcRTAlignJobHelper.tpDict[vsFromId];
            if (tpsFrom.Length == 0)
            {
                Debug.LogError($"tpsFrom.Length == 0 vsFromId:{vsFromId} vsToId:{vsToId}");
                return;
            }
            var tpsTo=AcRTAlignJobHelper.tpDict[vsToId];

            var vsFromW = AcRTAlignJobHelper.vsDictWorld[vsFromId];
            var vsFromL=AcRTAlignJobHelper.vsDictLocal[vsFromId];
            var vsToW = AcRTAlignJobHelper.vsDictWorld[vsToId];
            var vsToL = AcRTAlignJobHelper.vsDictLocal[vsToId];

            if (IsTrySameAngle)
            {
                
                var disLocal = DistanceUtil.GetDistance(vsFromL, vsToL);

                if (disLocal <= DistanceSetting.zeroM)
                {
                    //var disWorld = DistanceUtil.GetDistance(vsFromW, vsToW);
                    //Debug.Log($"AcRTAlignJob SameAngleResult disLocal:{disLocal} disWorld:{disWorld} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms");

                    double t = (DateTime.Now - start).TotalMilliseconds;
                    minRT = new SameAngleResult();
                    AcRTAlignJobResult.SetResult(Id, minRT, t);
                    return;
                }
            }            

            var scale1=AcRTAlignJobHelper.scaleDict[vsToId];

            //一.角度测试，Angle，Scale
            if(IsTryAngles){
                var trans=AcRTAlignJobHelper.posDict[vsToId]-AcRTAlignJobHelper.posDict[vsFromId];
                Matrix4x4 localMatrix=tpsFrom[0].localToWorldMatrix;
                int aCount=0;

                Vector3[] newVs2=new Vector3[vsFromL.Length];
                //NativeArray<Vector3> newVs2=new NativeArray<Vector3>(vsFromL.Length,Allocator.Temp);

                if (IsTryAngles_Scale == false)
                {
                    //StringBuilder sbScaleLog = new StringBuilder();
                    //List<float> sizeFrom=MeshHelper.GetSizeList(vsFromW,vsFromId);
                    //List<float> sizeTo = MeshHelper.GetSizeList(vsToW, vsToId);
                    //List<float> scaleList = new List<float>();
                    //for(int i=0;i<3;i++)
                    //{
                    //    float sizeScale = sizeTo[i] / sizeFrom[i];
                    //    if(scaleList.Contains(sizeScale))
                    //    {
                    //        continue;
                    //    }
                    //    sbScaleLog.Append($"sizeScale[{i}]:{sizeScale}({sizeTo[i]}/{sizeFrom[i]});\t");
                    //    scaleList.Add(sizeScale);
                    //}

                    //if (scaleList.Count == 1)
                    //{
                    //    //
                    //}

                    //scaleList.Sort();

                    //for (int i = 0; i < scaleList.Count; i++)
                    //{
                    //    float scale = scaleList[i];
                    //    sbScaleLog.Append($"scale[{i}]:{scale};\t");
                    //}
                    //var scaleMin = scaleList.First();
                    //var scaleMax = scaleList.Last();
                    //float scale_scale = scaleMax / scaleMin;
                    //sbScaleLog.AppendLine($"scale_scale:{scale_scale}");

                    //Debug.LogError($"sbScaleLog:{sbScaleLog}");
                }

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

                                double t = (DateTime.Now - start).TotalMilliseconds;
                                AcRTAlignJobResult.SetResult(Id, result, t);

                                //AngleCount++;
                                SetStatisticsInfo(Id, t, true, result.Mode, dis);
                                return;
                            }
                            else if(IsTryAngles_Scale)//比例
                            {
                                if(dis < TryScaleMaxDis)
                                {
                                    var mm1 = VertexHelper.GetMinMax(newVs2);
                                    var mm2 = VertexHelper.GetMinMax(vsToW);
                                    var scaleNew = new Vector3(mm2[2].x / mm1[2].x, mm2[2].y / mm1[2].y, mm2[2].z / mm1[2].z);
                                    Vector3 scaleNN = new Vector3(scale1.x * scaleNew.x, scale1.y * scaleNew.y, scale1.z * scaleNew.z);
                                    RTResult r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.x, scaleNN.y, scaleNN.z), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                    r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.y, scaleNN.x, scaleNN.z), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                    r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.z, scaleNN.y, scaleNN.x), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                    r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.z, scaleNN.x, scaleNN.y), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                    r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.x, scaleNN.z, scaleNN.y), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                    r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(scaleNN.y, scaleNN.z, scaleNN.x), trans, newVs2);
                                    if (r2.IsZero)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        AcRTAlignJobResult.SetResult(Id, r2, t);
                                        SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                        return;
                                    }
                                }
                                else
                                {
                                    //Debug.LogError($"AcRTAlignJob_Scale angle:{angle},Distance:{dis}");
                                }
                            }
                            else if (IsTryAngles_Mirror)//镜像
                            {
                                //var mm1 = VertexHelper.GetMinMax(newVs2);
                                //var mm2 = VertexHelper.GetMinMax(vsToW);
                                //var scaleNew = new Vector3(mm2[2].x / mm1[2].x, mm2[2].y / mm1[2].y, mm2[2].z / mm1[2].z);
                                //Vector3 scaleNN = new Vector3(scale1.x * scaleNew.x, scale1.y * scaleNew.y, scale1.z * scaleNew.z);
                                RTResult r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(-1,1,1), trans, newVs2);
                                r2.Mode = AlignMode.Mirror;
                                if (r2.IsZero)
                                {
                                    double t = (DateTime.Now - start).TotalMilliseconds;
                                    AcRTAlignJobResult.SetResult(Id, r2, t);
                                    SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                    return;
                                }
                                //r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(1, -1, 1), trans, newVs2);
                                //r2.Mode = AlignMode.Mirror;
                                //if (r2.IsZero)
                                //{
                                //    double t = (DateTime.Now - start).TotalMilliseconds;
                                //    AcRTAlignJobResult.SetResult(Id, r2, t);
                                //    SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                //    return;
                                //}
                                //r2 = CreateNewScaleGoMatrix(localMatrix, vsFromL, vsToW, angle, new Vector3(1, 1, -1), trans, newVs2);
                                //r2.Mode = AlignMode.Mirror;
                                //if (r2.IsZero)
                                //{
                                //    double t = (DateTime.Now - start).TotalMilliseconds;
                                //    AcRTAlignJobResult.SetResult(Id, r2, t);
                                //    SetStatisticsInfo(Id, t, true, r2.Mode, dis);
                                //    return;
                                //}
                            }
                            else
                            {
                                //Debug.Log($"angle:{angle},Distance:{dis}");
                                //GameObject.DestroyImmediate(goNew);
                            }
                        }
                    }
                }
            }

            if(IsTryRT)
            {
                //二、长短轴，RT
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
                        if (angle22 > MaxAngle) continue;//下一个了，角度都不对其
                        var rt = AcRigidTransform.GetRTMatrixS(tpFrom, tpTo);//核心，静态函数的
                        var vsNew = rt.ApplyPoints(vsFromW);
                        var dis = DistanceUtil.GetDistance(vsToW, vsNew, false);
                        //Debug.Log(string.Format("AcRTAlignJob[{6}][{4}][{5}] angle1:{0}\tangle2:{1}\tangle1_2:{2}\tdistance:{3}",angle1,angle2,angle22,dis,l,k,count));
                        rt.Distance = dis;
                        if (dis < minDis)
                        {
                            minDis = dis;
                            minRT = rt;
                            minVS = vsNew;
                        }
                        //Debug.LogError($">>>AcRTAlignJob [{l},{k}]\tIsZero:{rt.IsZero},\tIsReflection:{rt.IsReflection},\tdis:{dis}");
                        if (dis == 0)
                        {
                            isFoundZero = true;//break;
                        }
                        AcRTAlignJobExResult.disJobCount++;
                    }
                    //if (isFoundZero) break;
                }
            }
            
            if(IsTryICP)
            {
                //三、ICP
                if (minDis < DistanceSetting.ICPMinDis && minDis > DistanceSetting.zeroM)
                {
                    // if(minDis<DistanceSetting.ICPMinDis)
                    {
                        //Debug.LogError("minDis>DistanceSetting.zeroDis:"+minDis+">"+DistanceSetting.zeroDis+" and <"+DistanceSetting.ICPMinDis);
                        RTResultList rList = new RTResultList();
                        rList.Add(minRT as RTResult);
                        var vsFromStart = minVS;
                        string log = "";
                        for (int i = 0; i < DistanceSetting.ICPMaxCount; i++)
                        {
                            var vsFromCP1 = DistanceUtil.GetClosedPoints(vsToW, vsFromStart.ToList());
                            var r1 = AcRigidTransform.ApplyTransformationN(vsFromCP1, vsToW);
                            rList.Add(r1);
                            var vsFromNew1 = r1.ApplyPoints(vsFromStart);
                            var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsToW);
                            log += string.Format("{0:F5}; ", dis2);
                            vsFromStart = vsFromNew1;
                            r1.Distance = dis2;
                            rList.Distance = dis2;
                            if (dis2 <= DistanceSetting.zeroM)
                            {
                                isFoundZero = true;
                                break;
                            }
                        }

                        Debug.LogError($"ICP({DistanceSetting.zeroM:F4},{DistanceSetting.ICPMinDis:F2}) IsFoundZero:[{isFoundZero}],\tstartDis:{minDis},\tendDis:{rList.Distance},\tlog:{log}");
                        if (rList.Distance < minRT.Distance)
                        { 
                            //ICP算法处理后更加匹配了
                            double t = (DateTime.Now - start).TotalMilliseconds;
                            //ICPCount++;
                            AcRTAlignJobResult.SetResult(Id, rList, t);
                            SetStatisticsInfo(Id, t, isFoundZero, AlignMode.ICP, rList.Distance);
                            return;
                        }
                    }
                }
            }

            {
                //double t = (DateTime.Now - start).TotalMilliseconds;
                //if(minDis<AcRTAlignJobResult.MinDis)
                {
                    double t = (DateTime.Now - start).TotalMilliseconds;
                    // RTCount++;
                    if (minRT != null)
                    {
                        SetStatisticsInfo(Id, t, isFoundZero, AlignMode.RT, minRT.Distance);
                    }
                    else
                    {
                        //RT里面所有的点的GetLongShortAngle相差比较大
                        //string info = $"([From Id:{vsFromId} tps:{tpsFrom.Length} go:{MeshPoints.dictId2Go[vsFromId]} angle:{tpsFrom[0].GetLongShortAngle()}] [To Id:{vsToId} tps:{tpsTo.Length} go:{MeshPoints.dictId2Go[vsToId]} angle:{tpsTo[0].GetLongShortAngle()}] MaxAngle:{MaxAngle})";
                        string info = "";
                        SetStatisticsInfo(Id, t, isFoundZero, AlignMode.RT, minDis
                            , info);
                    }
                    
                    AcRTAlignJobResult.SetResult(Id, minRT, t);
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

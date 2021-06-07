using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;

namespace MeshJobs
{

  [BurstCompile]
public struct MeshAlignJob : IJobParallelForTransform
{
    // public int count;
    public NativeArray<ThreePoint> ps1;
    public NativeArray<ThreePoint> ps2;

    public void Init(int count)
    {
        // this.count=count;
        ps1=new NativeArray<ThreePoint>(count,Allocator.TempJob);
        ps2=new NativeArray<ThreePoint>(count,Allocator.TempJob);
    }

    public void SetIds(int i,ThreePoint p1,ThreePoint p2)
    {
        ps1[i]=p1;
        ps2[i]=p2;
    }

    public const bool IsDebug=false;

    public void Execute(int i, TransformAccess transform)
    {
        ThreePoint p1=ps1[i];
        ThreePoint p2=ps2[i];

        p2.localToWorldMatrix=transform.localToWorldMatrix;

        if(IsDebug) Debug.Log($"MeshAlignJob {i},p1:{p1},p2:{p2}");
        // Debug.Log($"MeshAlignJob {i},p1:{p1},p2:{p2}");

        // //坐标对齐
        // {
        //     Vector3 pos1=p1.GetCenterP();
        //     Vector3 pos2=p2.GetCenterP();
        //     Vector3 offset = pos1 - pos2;
        //     transform.position += offset;//动作1，移动，也可以。
        //     p2.localToWorldMatrix=transform.localToWorldMatrix;//旋转后更新变换矩阵
        // }


        //法线对齐
        //{
            var normal1=p1.GetLongShortNormal();
            var normal2=p2.GetLongShortNormal();
            var normalAngle=Vector3.Angle(normal2,normal1);

            if(IsDebug){
                Debug.Log(string.Format("{0} Before normal1:{1},normal2:{2},normalAngle:{3}",i,normal1,normal2,normalAngle));
                normal1.PrintVector3("normal1");
                normal2.PrintVector3("normal2");
            }


            Quaternion qua1 = Quaternion.FromToRotation(normal2, normal1);//两条法线之间的角度变化
            transform.rotation = qua1 * transform.rotation;//旋转tempCenter，对齐两条法线
            p2.localToWorldMatrix=transform.localToWorldMatrix;//旋转后更新变换矩阵

            if(IsDebug){
                normal1=p1.GetLongShortNormal();
                normal2=p2.GetLongShortNormal();
                normalAngle=Vector3.Angle(normal2,normal1);
                Debug.Log(string.Format("{0} After normal1:{1},normal2:{2},normalAngle:{3}",i,normal1,normal2,normalAngle));
            }
        //}

        // //坐标对齐
        // {
        //     Vector3 pos1=p1.GetCenterP();
        //     Vector3 pos2=p2.GetCenterP();
        //     Vector3 offset = pos1 - pos2;
        //     transform.position += offset;//动作1，移动，也可以。
        //     p2.localToWorldMatrix=transform.localToWorldMatrix;//旋转后更新变换矩阵
        // }

        //长短轴对齐（对应特征点）
        {
            //两个模型的长轴和短轴之间的角度
            float angle1=Vector3.Angle(p2.GetShortLine(), p1.GetShortLine());
            float angle2=Vector3.Angle(p2.GetLongLine(), p1.GetLongLine());
            var avgAngle = (angle1 + angle2) / 2f;
            if(IsDebug){
                Debug.Log($"Before ShortLineAngle:{angle1}, LongLineAngle:{angle2}, rotateAngle:{avgAngle} ");
            }

            //4.旋转tempCenter，对齐两条短轴
            Quaternion qua2 = Quaternion.FromToRotation(p2.GetShortLine(), p1.GetShortLine());//两条短轴之间的角度变化
            transform.rotation = qua2 * transform.rotation;//旋转tempCenter，对齐两条短轴
            p2.localToWorldMatrix=transform.localToWorldMatrix;//旋转后更新变换矩阵
            //这个旋转居然没影响到法线？之前加个临时物体调整中心就是好像这个会影响啊。转玩了，坐标对齐就好了？

            //transform.Rotate(tempCenter.transform.up, -angle1);

            //transform.rotation*=Quaternion.AngleAxis(-angle1,normal2);

             if(IsDebug){
                angle1=Vector3.Angle(p2.GetShortLine(), p1.GetShortLine());
                angle2=Vector3.Angle(p2.GetLongLine(), p1.GetLongLine());
                avgAngle = (angle1 + angle2) / 2f;
                Debug.Log($"After1 ShortLineAngle:{angle1}, LongLineAngle:{angle2}, rotateAngle:{avgAngle} ");

                normal1=p1.GetLongShortNormal();
                normal2=p2.GetLongShortNormal();
                normalAngle=Vector3.Angle(normal2,normal1);
                Debug.Log($"After2 normal1:{normal1},normal2:{normal2},normalAngle:{normalAngle}");
            }
        }

        //坐标对齐
        {
            Vector3 pos1=p1.GetCenterP();
            Vector3 pos2=p2.GetCenterP();
            Vector3 offset = pos1 - pos2;
            transform.position += offset;//动作1，移动，也可以。
            p2.localToWorldMatrix=transform.localToWorldMatrix;//旋转后更新变换矩阵
        }


        // normal1.PrintVector3("normal1");
        // normal2.PrintVector3("normal2");

        //if(isShowDetail) node2.ShowDebugDetail(true);//旋转后更新调试用的模型位置，法线变了。
        //node2.CreateNormalLineAndPlane("_Update1");//转正法平面，直观的比较法线方向

        // // //旋转后法线比较
        // // Vector3 normal12 = node1.GetLongShortNormalNew();
        // // Vector3 normal22 = node2.GetLongShortNormalNew();
        // // if(showLog)Debug.Log($"[After RotateNormal]normal12:({normal12.x},{normal12.y},{normal12.z}) |"+$"normal22:({normal22.x},{normal22.y},{normal22.z}) |"+$"normalAngle:{Vector3.Angle(normal12,normal22)}");

    }
}
}
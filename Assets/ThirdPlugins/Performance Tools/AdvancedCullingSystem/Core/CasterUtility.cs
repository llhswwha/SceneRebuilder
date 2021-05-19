using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
//using UnityEditor;
using AdvancedCullingSystem.StaticCullingCore;

namespace AdvancedCullingSystem.Core
{
/// <summary>
    /// 投射器工具类
    /// </summary>
    public static class CasterUtility
    {
        public const int CastersBatch = 10;

        /// <summary>
        /// 获取采样单元球
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static NativeList<float3> SphereUniform(int count)
        {
            NativeList<float3> directions = new NativeList<float3>(Allocator.TempJob);

            //黄金比例:黄金分割是指将整体一分为二，较大部分与整体部分的比值等于较小部分与较大部分的比值，其比值约为0.618。这个比例被公认为是最能引起美感的比例，因此被称为黄金分割。
            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            //角度增量
            float angleIncrement = Mathf.PI * 2 * goldenRatio; //3.1415926*2*0.618=3.8830084536

            for (int i = 0; i < count; i++)
            {
                float t = (float)i / count;//1/count * i,t:[0,1]
                //倾斜度
                float inclination = Mathf.Acos(1 - 2 * t); // t:[0,1],1-2*t:[1,-1] inclination([1,-1]):
                //Math.acos()用于计算反余弦,返回值的单位为弧度,对于[-1,1]之间的元素,函数值域为[0,pi],如果要想将角度转成余弦的值可以Math.cos()
                //cos(0)==1,acos(1)==0;cos(PI)=-1

                //地平经度，方位角(用以找出恒星、行星等的方位)
                //方位角，又称地平经度(Azimuth angle，缩写为Az)，是在平面上量度物体之间的角度差的方法之一。是从某点的指北方向线起，依顺时针方向到目标方向线之间的水平夹角。
                float azimuth = angleIncrement * i;

                //计算角度
                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                float3 dir = new float3(x, y, z);
                directions.Add(dir);
            }
            return directions;
        }

        /// <summary>
        /// Halton序列是一种为数值方法（如蒙特卡洛模拟算法）产生顶点的系列生成算法。虽然这些序列是以确定的方法算出来的，但它们的偏差很小。也就是说，在大多数情况下这些序列可以看成是随机的。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="b">base</param>
        /// <returns></returns>
        public static float HaltonSequence(int index, int b)
        {
            float res = 0f;
            float f = 1f / b;

            int i = index;

            while (i > 0)
            {
                res = res + f * (i % b);
                i = Mathf.FloorToInt(i / b);
                f = f / b;
            }

            return res;
        }

        /// <summary>
        /// 创建射线方向，基于Halton序列
        /// </summary>
        public static NativeArray<float3> CreateRayDirsArray(int jobsPerFrame, int unitSize = 4, int fieldOfView = 61)
        {
            Debug.Log($"CreateRayDirsArray jobsPerFrame:{jobsPerFrame},unitSize:{unitSize},fieldOfView:{fieldOfView}");
            int dirsCount = Mathf.RoundToInt(((Screen.width * Screen.height) / unitSize) / jobsPerFrame) * jobsPerFrame;

            NativeArray<float3> rayDirs = new NativeArray<float3>(dirsCount, Allocator.Persistent);

            Camera camera = new GameObject().AddComponent<Camera>();
            camera.fieldOfView = fieldOfView;

            for (int i = 0; i < rayDirs.Length; i++)
            {
                Vector2 screenPoint = new Vector2(HaltonSequence(i, 2), HaltonSequence(i, 3));//Halton序列

                Ray ray = camera.ViewportPointToRay(new Vector3(screenPoint.x, screenPoint.y, 0));

                rayDirs[i] = ray.direction;
            }
            GameObject.Destroy(camera.gameObject);
            return rayDirs;
        }

        /// <summary>
        /// 创建射线方向，基于Halton序列
        /// </summary>
        public static NativeList<float3> CreateRayDirsByJobsPerFrame(int jobsPerFrame, int unitSize = 4, int fieldOfView = 61)
        {
            int dirsCount = Mathf.RoundToInt(((Screen.width * Screen.height) / unitSize) / jobsPerFrame) * jobsPerFrame;
            Debug.Log($"CreateRayDirs Screen:({Screen.width},{Screen.height},{Screen.width * Screen.height}) jobsPerFrame:{jobsPerFrame},dirsCount:{dirsCount}");
            return CreateRayDirsListByCount(dirsCount, fieldOfView);
        }

        /// <summary>
        /// 创建射线方向，基于Halton序列
        /// </summary>
        public static NativeList<float3> CreateRayDirsListByCount(int dirsCount, int fieldOfView)
        {
            NativeList<float3> rayDirs = new NativeList<float3>(Allocator.TempJob);

            Camera camera = new GameObject().AddComponent<Camera>();
            camera.fieldOfView = fieldOfView;//摄像头的广角

            for (int i = 0; i < dirsCount; i++)
            {
                Vector2 screenPoint = new Vector2(HaltonSequence(i, 2), HaltonSequence(i, 3));//Halton序列

                Ray ray = camera.ViewportPointToRay(new Vector3(screenPoint.x, screenPoint.y, 0));

                rayDirs.Add(ray.direction);
            }
            GameObject.Destroy(camera.gameObject);
            return rayDirs;
        }

        public static void InitializeCasters(List<Caster> casters, CasterDataContainer container)
        {
            for (int i = 0; i < casters.Count; i++)
                casters[i].Initialize(container);
        }

        public static bool ProcessCasters(List<Caster> casters, NativeList<float3> directions,
            string barTitle = null)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.TempJob);

            int processed = 0;
            while (processed < casters.Count)
            {
                float progress = (float)System.Math.Round((float)processed / casters.Count, 2);
                float percents = progress * 100;

                if (barTitle != null && ProgressBarHelper.DisplayCancelableProgressBar(barTitle, percents + "% of 100%", progress))
                {
                    handles.Dispose();

                    ProgressBarHelper.ClearProgressBar();

                    return false;
                }

                int count = Mathf.Min(CastersBatch, casters.Count - processed);

                //1.CreateRayCommands
                for (int i = processed; i < processed + count; i++)
                    casters[i].CreateRayCommands(directions);

                //2.CastRays
                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].CastRays());
                JobHandle.CompleteAll(handles);

                //3.ComputeHitsResult
                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].ComputeHitsResult());
                JobHandle.CompleteAll(handles);

                //4.PushData
                for (int i = processed; i < processed + count; i++)
                    casters[i].PushData();

                processed += count;
            }

            handles.Dispose();

            if(barTitle != null)
                ProgressBarHelper.ClearProgressBar();

            return true;
        }

        public static bool ProcessCasters(List<Caster> casters, NativeList<int> targets, 
            NativeList<int2> pointers, NativeList<float3> points, string barTitle = null)
        {
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.TempJob);

            int processed = 0;
            while (processed < casters.Count)
            {
                float progress = (float)System.Math.Round((float)processed / casters.Count, 2);
                float percents = progress * 100;

                if (barTitle != null && ProgressBarHelper.DisplayCancelableProgressBar(barTitle, percents + "% of 100%", progress))
                {
                    handles.Dispose();

                    ProgressBarHelper.ClearProgressBar();

                    return false;
                }

                int count = Mathf.Min(CastersBatch, casters.Count - processed);

                //1.CreateRayCommands
                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                {
                    JobHandle handle=casters[i].CreateRayCommands(targets, points, pointers);
                    
                    handles.Add(handle);
                }
                JobHandle.CompleteAll(handles);

                //2.CastRays
                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].CastRays());
                JobHandle.CompleteAll(handles);

                //3.ComputeHitsResult
                handles.Clear();
                for (int i = processed; i < processed + count; i++)
                    handles.Add(casters[i].ComputeHitsResult());
                JobHandle.CompleteAll(handles);

                //4.PushData
                for (int i = processed; i < processed + count; i++)
                    casters[i].PushData();

                processed += count;
            }

            handles.Dispose();

            if(barTitle != null)
                ProgressBarHelper.ClearProgressBar();

            return true;
        }

        public static void Dispose(List<Caster> casters, CasterDataContainer dataContainer = null)
        {
            for (int i = 0; i < casters.Count; i++)
                casters[i].Dispose();

            if (dataContainer != null)
                dataContainer.Dispose();
        }

        #region StaticCullingMaster
        /// <summary>
        /// 获取多个物体的包围盒
        /// </summary>
        /// <param name="renderers"></param>
        /// <returns></returns>

        public static Bounds CalculateBoundingBox(IEnumerable<MeshRenderer> renderers)
        {
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = -min;

            foreach (var renderer in renderers)
            {
                Bounds rendererBounds = renderer.bounds;

                min.x = Mathf.Min(rendererBounds.min.x, min.x);
                min.y = Mathf.Min(rendererBounds.min.y, min.y);
                min.z = Mathf.Min(rendererBounds.min.z, min.z);

                max.x = Mathf.Max(rendererBounds.max.x, max.x);
                max.y = Mathf.Max(rendererBounds.max.y, max.y);
                max.z = Mathf.Max(rendererBounds.max.z, max.z);
            }

            return new Bounds((min + max) / 2, max - min);
        }

        /// <summary>
        /// 根据CellSize将一个Area的Boundes拆分为多个Cell的Boundes
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="cellSize"></param>
        /// <returns></returns>
        public static List<Bounds> CalculateCellsBounds(Bounds boundingBox, float cellSize)
        {
            Vector3 min = boundingBox.min;

            List<Bounds> bounds = new List<Bounds>();

            int countX = Mathf.CeilToInt(boundingBox.size.x / cellSize);
            int countY = Mathf.CeilToInt(boundingBox.size.y / cellSize);
            int countZ = Mathf.CeilToInt(boundingBox.size.z / cellSize);

            for (int i = 0; i < countX; i++)
            {
                for (int c = 0; c < countY; c++)
                {
                    for (int j = 0; j < countZ; j++)
                    {
                        Vector3 center = min;

                        center.x += (i * cellSize) + cellSize / 2;
                        center.y += (c * cellSize) + cellSize / 2;
                        center.z += (j * cellSize) + cellSize / 2;

                        bounds.Add(new Bounds(center, Vector3.one * cellSize));
                    }
                }
            }

            return bounds;
        }

        public static int CalculateCastersCount(List<Bounds> areas, float cellSize)
        {
            return CalculateCastersCount(areas, cellSize, out int x, out int y, out int z);
        }

        public static int CalculateCastersCount(List<Bounds> areas, float cellSize,
            out int countX, out int countY, out int countZ)
        {
            countX = 0;
            countY = 0;
            countZ = 0;

            for (int i = 0; i < areas.Count; i++)
            {
                countX += Mathf.CeilToInt((areas[i].max.x - areas[i].min.x) / cellSize) + 1;
                countY += Mathf.CeilToInt((areas[i].max.y - areas[i].min.y) / cellSize) + 1;
                countZ += Mathf.CeilToInt((areas[i].max.z - areas[i].min.z) / cellSize) + 1;
            }

            return countX * countY * countZ;
        }
        #endregion
    }
}

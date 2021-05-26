using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using UnityEditor;

using Random = UnityEngine.Random;
using Stopwatch = System.Diagnostics.Stopwatch;
using AdvancedCullingSystem.Core;
using System;
using Object = UnityEngine.Object;

namespace AdvancedCullingSystem.StaticCullingCore
{
    [Serializable]
    public class StaticCullingMaster
    {
        public const int StandardInitJobsCount = 2000;
        public const int FastInitJobsCount = 6000;
        public const int RaysBatch = 100000;//10w

        [SerializeField]
        private Camera[] _cameras;
        [SerializeField]
        private MeshRenderer[] _renderers;
        [SerializeField]
        private Collider[] _occluders;
        [SerializeField]
        private MeshFilter[] _filters;
        [SerializeField]
        private Bounds[] _areas;

        //[SerializeField]
        //private bool _fastBake;
        //[SerializeField]
        //private int _maxRaysPerObject;
        //[SerializeField]
        //private float _cellSize;
        //[SerializeField]
        //private int _layer;
        //[SerializeField]
        //private int _directionCount=2000;//cww_add
        //[SerializeField]
        //private bool _isOptimaizeTree=true;//cww_add

        [SerializeField]
        public CullingSetting _cullingSetting = new CullingSetting();

        [SerializeField]
        private List<Caster> _casters;
        [SerializeField]
        private List<Caster[,,]> _castersUnit;
        [SerializeField]
        private CasterDataContainer _dataContainer;
        [SerializeField]
        private MeshCollider[] _renderersColliders;
        [SerializeField]
        private List<Collider> _disabledColliders;
        [SerializeField]
        private BinaryTree[] _trees;


        //public StaticCullingMaster(Camera[] cameras, MeshRenderer[] renderers, Collider[] occluders, Bounds[] areas,
        //    bool fastBake, int maxJobs, float cellSize, int layer,int sphereCount,bool isOptimizeTree)
        //{
        //    _renderers = renderers;
        //    _occluders = occluders;
        //    _filters = _renderers.Select(r => r.GetComponent<MeshFilter>()).ToArray();
        //    _areas = areas;

        //    _fastBake = fastBake;
        //    _maxRaysPerObject = maxJobs;
        //    _cellSize = cellSize;
        //    _layer = layer;
        //    _cameras = cameras;

        //    _directionCount=sphereCount;
        //    _isOptimaizeTree=isOptimizeTree;
        //}
        //    public StaticCullingMaster(Camera[] cameras, MeshRenderer[] renderers, Collider[] occluders, Bounds[] areas,
        //CullingSetting cullingSetting)
        //    {
        //        _renderers = renderers;
        //        _occluders = occluders;
        //        _filters = _renderers.Select(r => r.GetComponent<MeshFilter>()).ToArray();
        //        _areas = areas;

        //        _cameras = cameras;

        //        _fastBake = cullingSetting._fastBake;
        //        _maxRaysPerObject = cullingSetting._jobsPerObject;
        //        _cellSize = cullingSetting._cellSize;
        //        _layer = cullingSetting.layer;
        //        _directionCount = cullingSetting._directionCount;
        //        _isOptimaizeTree = cullingSetting._isOptimaizeTree;
        //    }

        public StaticCullingMaster(Camera[] cameras, MeshRenderer[] renderers, Collider[] occluders, Bounds[] areas,
            CullingSetting cullingSetting)
        {
            _renderers = renderers;
            //_renderers = GameObject.FindObjectsOfType<MeshRenderer>();
            _occluders = occluders;
            _filters = _renderers.Select(r => r.GetComponent<MeshFilter>()).ToArray();
            _areas = areas;

            _cameras = cameras;
            _cullingSetting = cullingSetting;
        }

        public float CalculateComputingTime()
        {
            float totalSeconds = 0;

            try
            {
                int totalCasters = CasterUtility.CalculateCastersCount(_areas.ToList(), _cullingSetting.CellSize);
                int castersCount = Mathf.Min(500, totalCasters);
                float ratio = (float)totalCasters / castersCount;

                InstantiateColliders();

                Stopwatch mainSW = new Stopwatch();
                mainSW.Start();

                InstantiateCastersRandom(castersCount);

                if (InitializeCasters())
                {
                    mainSW.Stop();

                    if (_cullingSetting.FastBake)
                    {
                        totalSeconds = mainSW.Elapsed.Seconds * ratio;
                    }
                    else
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        if (ComputeVisibilityWithoutCasting())
                        {
                            sw.Stop();

                            mainSW.Start();
                            if (ComputeVisibility())
                            {
                                mainSW.Stop();

                                totalSeconds = (mainSW.Elapsed.Seconds - sw.Elapsed.Seconds) * ratio + sw.Elapsed.Seconds;
                            }
                        }

                        sw.Stop();
                    }
                }

                mainSW.Stop();
            }
            catch(System.Exception ex)
            {
                ProgressBarHelper.ClearProgressBar();

                Debug.Log("Can't calculate baking time");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                totalSeconds = 0;
            }

            Clear();

            return (float)System.Math.Round(totalSeconds / 60, 2);
        }        

        public void Compute()
        {
            System.DateTime start = System.DateTime.Now;
            try
            {
                ProgressBarHelper.DisplayProgressBar("Start...", "0 of 1", 0);

                

                InstantiateCasters();//创建Casters Areas->AreaBounds->Cells(Bounds)->Casters

                InstantiateColliders();//初始化Colliders，做什么的？

                ProgressBarHelper.ClearProgressBar();
;
                if (InitializeCasters()) //射线碰撞检测，计算可见性1：  CasterUtility.ProcessCasters1()，根据采样单元球来计算物体可见性
                {
                    if (_cullingSetting.FastBake || ComputeVisibility())//计算可见性2： CasterUtility.ProcessCasters2() ，根据物体的Mesh的顶点、三角面进行射线碰撞检测，并获取可见的物体，提高了采样的精度和烘培的时间
                    {
                        CreateBinaryTrees();//构建二分树
                        CreateStaticCullingObject();//创建StaticCulling，运行时执行遮挡剔除操作
                    }

                    ////等效于：
                    //if (_fastBake) //简单采样
                    //{
                    //    CreateBinaryTrees();
                    //    CreateStaticCullingObject();
                    //}
                    //else //(_fastBake==false) //复杂采样
                    //{
                    //    bool isCompleted = ComputeVisibility();//多了这个步骤，所有模型的Mesh的顶点，提高了采样的精度和烘培的时间
                    //    if (isCompleted)
                    //    {
                    //        CreateBinaryTrees();
                    //        CreateStaticCullingObject();
                    //    }
                    //}
                }
            }
            catch (System.Exception ex)
            {
                ProgressBarHelper.ClearProgressBar();

                Debug.Log("Can't compute visibility");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");
            }
            finally
            {
                Clear();
            }

            System.TimeSpan timeSpan = System.DateTime.Now - start;
            _cullingSetting.BakingTime = (float)timeSpan.TotalSeconds;


            StaticCullingTestData testData = StaticCullingTestReporter.Current;
            testData.Time = timeSpan.TotalSeconds;
            testData.FastBake = _cullingSetting.FastBake;
            testData.CellSize = _cullingSetting.CellSize;
            testData.JobsPerObject = _cullingSetting.JobsPerObject;
            testData.FastBake = _cullingSetting.FastBake;
            testData.RayPointsCountAll = rayPointsCountAll;
            testData.RayPointsCount = rayPointsCount;
            testData.AvgRayPointsCount = rayPointsCount / _renderers.Length;

            Debug.LogError($"Compute FastBake:{_cullingSetting.FastBake} CellSize:{_cullingSetting.CellSize} MaxRaysPerObject:{_cullingSetting.JobsPerObject} Time:{timeSpan.TotalSeconds:F2}s rayPointsCount:{rayPointsCountAll} avgRayPoints:{rayPointsCountAll/_renderers.Length}");
        }


        private void InstantiateCastersRandom(int count)
        {
            _casters = new List<Caster>();

            int created = 0;
            while (created < count)
            {
                int idx = Random.Range(0, _areas.Length);

                float posX = Random.Range(_areas[idx].min.x, _areas[idx].max.x);
                float posY = Random.Range(_areas[idx].min.y, _areas[idx].max.y);
                float posZ = Random.Range(_areas[idx].min.z, _areas[idx].max.z);

                Caster caster = new GameObject("Caster").AddComponent<Caster>();

                caster.transform.position = new Vector3(posX, posY, posZ);

                _casters.Add(caster);

                created++;
            }
        }

        private int rayPointsCountAll = 0;

        private int rayPointsCount = 0;
        

        private bool ComputeVisibilityWithoutCasting()
        {
            rayPointsCountAll=0;
            rayPointsCount = 0;
            NativeList<int> targets = new NativeList<int>(Allocator.TempJob);
            NativeList<int2> pointers = new NativeList<int2>(Allocator.TempJob);
            NativeList<float3> points = new NativeList<float3>(Allocator.TempJob);

            bool canceled = false;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (canceled)
                    break;

                float progress = (float)System.Math.Round((float)i / _renderers.Length, 2);
                float percents = progress * 100;

                if (ProgressBarHelper.DisplayCancelableProgressBar("Calculating Visibility...", percents + "% of 100%", progress))
                {
                    targets.Dispose();
                    pointers.Dispose();
                    points.Dispose();

                    ProgressBarHelper.ClearProgressBar();

                    return false;
                }

                targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                int startIndex = points.Length;

                Mesh mesh = _filters[i].sharedMesh;

                Vector3[] vertices = mesh.vertices.Select(v => _filters[i].transform.TransformPoint(v)).ToArray();
                List<int> triangles = new List<int>();

                for (int c = 0; c < mesh.subMeshCount; c++)
                    triangles.AddRange(mesh.GetTriangles(c));

                List<Vector3> rayPoints = new List<Vector3>(vertices.Distinct());

                for (int c = 0; c < triangles.Count; c += 3)
                {
                    Vector3 vec1 = vertices[triangles[c]];
                    Vector3 vec2 = vertices[triangles[c + 1]];
                    Vector3 vec3 = vertices[triangles[c + 2]];

                    rayPoints.Add((vec1 + vec2 + vec3) / 3);
                }

                rayPointsCountAll+=rayPoints.Count;

                int step = _cullingSetting.GetStep(rayPoints.Count);

                for (int c = 0; c < rayPoints.Count; c += step)
                {
                    points.Add(rayPoints[c]);
                    

                    if (points.Length >= RaysBatch)
                    {
                        rayPointsCount+= points.Length;
                        pointers.Add(new int2(startIndex, points.Length));

                        targets.Clear();
                        points.Clear();
                        pointers.Clear();

                        targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                        startIndex = 0;
                    }
                }

                pointers.Add(new int2(startIndex, points.Length));
            }



            targets.Dispose();
            points.Dispose();
            pointers.Dispose();

            ProgressBarHelper.ClearProgressBar();

            return !canceled;
        }


        private void InstantiateColliders()
        {
            Debug.LogError("InstantiateColliders:"+_renderers.Length);
            _disabledColliders = new List<Collider>();

            _disabledColliders = Object.FindObjectsOfType<Collider>()
                .Where(col => col.gameObject.layer == _cullingSetting.layer)
                .ToList();

            _disabledColliders.ForEach(col => col.enabled = false);


            _renderersColliders = new MeshCollider[_renderers.Length];



            for(int i = 0; i < _renderersColliders.Length; i++)
            {
                float progress = i * 1f / _renderersColliders.Length;
                ProgressBarHelper.DisplayProgressBar("Start...InstantiateColliders", $"{i} of {_renderersColliders.Length}", progress);

                if(IsUseMask)
                {
                GameObject go = new GameObject("Mesh Collider");

                go.transform.parent = _renderers[i].transform;

                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                go.layer = _cullingSetting.layer;

                MeshCollider col = go.AddComponent<MeshCollider>();

                 col.sharedMesh = _renderers[i].GetComponent<MeshFilter>().sharedMesh;

                _renderersColliders[i] = col;
                }
                else{
                  MeshCollider col=_renderers[i].gameObject.GetComponent<MeshCollider>();
                  if(col==null){
                     col=_renderers[i].gameObject.AddComponent<MeshCollider>();
                  _renderersColliders[i] = col;
                 }
                }
            }

            for (int i = 0; i < _occluders.Length; i++)
            {
                _occluders[i].gameObject.layer = _cullingSetting.layer;
                _occluders[i].enabled = true;
            }

            Debug.LogError("InstantiateColliders:"+_renderers.Length+"|"+_disabledColliders);
        }

        private void InstantiateCasters()
        {
            _casters = new List<Caster>();
            _castersUnit = new List<Caster[,,]>();

            ProgressBarHelper.DisplayProgressBar("Start...InstantiateCasters", "0 of 1", 0);

            //Debug.Log("InstantiateCasters _areas:" + _areas.Length);
            for (int i = 0; i < _areas.Length; i++)
            {
                Bounds bounds = _areas[i];

                int countX = Mathf.CeilToInt((bounds.max.x - bounds.min.x) / _cullingSetting.CellSize);
                int countY = Mathf.CeilToInt((bounds.max.y - bounds.min.y) / _cullingSetting.CellSize);
                int countZ = Mathf.CeilToInt((bounds.max.z - bounds.min.z) / _cullingSetting.CellSize);

                _castersUnit.Add(new Caster[countX + 1, countY + 1, countZ + 1]);

                for (int x = 0; x <= countX; x++)
                {
                    float progress = x * 1f / countX;
                    ProgressBarHelper.DisplayProgressBar("Start...InstantiateCasters", $"{i} of {_areas.Length} {x}/{countX}", progress);

                    for (int y = 0; y <= countY; y++)
                    {
                        for (int z = 0; z <= countZ; z++)
                        {
                            Caster caster = new GameObject("Caster").AddComponent<Caster>();

                            caster.transform.position = bounds.min + new Vector3(x, y, z) * _cullingSetting.CellSize;
                            caster.transform.localScale = Vector3.one * _cullingSetting.CellSize;

                            _casters.Add(caster);
                            _castersUnit[i][x, y, z] = caster;
                        }
                    }
                }
            }

            StaticCullingTestReporter.Current.CastersCount = _casters.Count;
        }

        private bool InitializeCasters()
        {
            Debug.Log($"InitializeCasters {_renderers.Length},{_renderersColliders.Length},{LayerMask.LayerToName(_cullingSetting.layer)}");
            _dataContainer = new CasterDataContainer(_renderers, _renderersColliders, _occluders, LayerMask.LayerToName(_cullingSetting.layer));
            
            int count= _cullingSetting.FastBake ? FastInitJobsCount : StandardInitJobsCount;
            count= _cullingSetting.DirectionCount;
            //采样单元球
            NativeList<float3> sphereUniformDirections = CasterUtility.SphereUniform(count);
            Debug.Log("sphereUniformDirections:"+ sphereUniformDirections.Length);

            StaticCullingTestReporter.Current.DirectionCount = sphereUniformDirections.Length;
            //StandardInitJobsCount = 2000;FastInitJobsCount = 6000;

            CasterUtility.InitializeCasters(_casters, _dataContainer);

            bool isCompleted = CasterUtility.ProcessCasters(_casters, sphereUniformDirections, "InitializeCasters...");

            sphereUniformDirections.Dispose();

            return isCompleted;
        }

        /// <summary>
        /// 根据物体的Mesh的顶点、三角面进行射线碰撞检测，并获取可见的物体。
        /// </summary>
        /// <returns></returns>
        private bool ComputeVisibility()
        {
            rayPointsCountAll = 0;
            rayPointsCount = 0;
            Debug.LogWarning("ComputeVisibility");
            NativeList<int> targets = new NativeList<int>(Allocator.TempJob);
            NativeList<int2> pointers = new NativeList<int2>(Allocator.TempJob);
            NativeList<float3> points = new NativeList<float3>(Allocator.TempJob);

            bool canceled = false;
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (canceled)
                    break;

                float progress = (float)System.Math.Round((float) i / _renderers.Length, 2);
                float percents = progress * 100;

                if (ProgressBarHelper.DisplayCancelableProgressBar("Calculating Visibility...", percents + "% of 100%", progress))
                {
                    targets.Dispose();
                    pointers.Dispose();
                    points.Dispose();

                    ProgressBarHelper.ClearProgressBar();

                    return false;
                }

                targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                int startIndex = points.Length;

                Mesh mesh = _filters[i].sharedMesh;

                Vector3[] vertices = mesh.vertices.Select(v => _filters[i].transform.TransformPoint(v)).ToArray();//获取对象的所有顶点的世界坐标

                List<int> triangles = new List<int>();

                for (int c = 0; c < mesh.subMeshCount; c++)
                    triangles.AddRange(mesh.GetTriangles(c));

                List<Vector3> rayPoints = new List<Vector3>(vertices.Distinct());//顶点增加到rayPoints列表中

                for (int c = 0; c < triangles.Count; c += 3)
                {
                    Vector3 vec1 = vertices[triangles[c]];
                    Vector3 vec2 = vertices[triangles[c + 1]];
                    Vector3 vec3 = vertices[triangles[c + 2]];

                    rayPoints.Add((vec1 + vec2 + vec3) / 3);//三角形的中点增加到rayPoints列表中
                }
                rayPointsCountAll+=rayPoints.Count;

                int step = _cullingSetting.GetStep(rayPoints.Count);//并不是所有的rayPoints都要采样的，根据步长step采样，默认是上面的全部的/20
                //Debug.LogError("step:"+step);

                for (int c = 0; c < rayPoints.Count; c += step)
                {
                    points.Add(rayPoints[c]);

                    if (points.Length >= RaysBatch)//RaysBatch=10w，每10w个点一个包
                    {
                        rayPointsCount += points.Length;
                        pointers.Add(new int2(startIndex, points.Length));

                        if (!CasterUtility.ProcessCasters(_casters, targets, pointers, points, "StepComputeVisibility..."))
                        {
                            canceled = true;
                            break;
                        }

                        targets.Clear();
                        points.Clear();
                        pointers.Clear();

                        targets.Add(_dataContainer.iDByRenderer[_renderers[i]]);

                        startIndex = 0;
                    }
                }
                pointers.Add(new int2(startIndex, points.Length));
            }

            if (!canceled)
            {
                rayPointsCount += points.Length;
                canceled = !CasterUtility.ProcessCasters(_casters, targets, pointers, points, "StepComputeVisibility...");
            }

            CasterUtility.Dispose(_casters, _dataContainer);

            targets.Dispose();
            points.Dispose();
            pointers.Dispose();

            ProgressBarHelper.ClearProgressBar();

            return !canceled;
        }

        private int CreateBinaryTrees()
        {

            String log = "";
            //Debug.Log("CreateBinaryTrees:"+ _areas.Length);
            _trees = new BinaryTree[_areas.Length];

            int treeNodeCount = 0;
            for (int i = 0; i < _areas.Length; i++)
            {
                ProgressBarHelper.DisplayProgressBar("Creating Binary Trees...", i + " of " + _areas.Length,
                    (float)i / _areas.Length);

                _trees[i] = new GameObject("Binary Tree").AddComponent<BinaryTree>();//一个Area,创建一个BinaryTree
                _trees[i].CreateTree(_castersUnit[i]);

                if (_cullingSetting.IsOptimaizeTree)
                {
                    ProgressBarHelper.DisplayProgressBar("Optimize Binary Trees...", i + " of " + _areas.Length,
                    (float)i / _areas.Length);
                    BinaryTreeUtil.OptimizeTree(_trees[i]);
                }

                treeNodeCount += _trees[i].GetAllChildrenNode().Count;

                log+=($"Tree{i}_NodeCount:{treeNodeCount}\t");//cww_add
            }



            _cullingSetting.TreeNodeCount = treeNodeCount;

            log=$"CreateBinaryTrees AllTreeNodeCount:{_cullingSetting.TreeNodeCount}\t"+log;//cww_add
            Debug.Log(log);

            ProgressBarHelper.ClearProgressBar();
            return treeNodeCount;
        }

        private void CreateStaticCullingObject()
        {
            StaticCulling staticCulling = StaticCulling.Create(_renderers, _trees, _cameras);
          
            for (int i = 0; i < _trees.Length; i++)
                _trees[i].transform.parent = staticCulling.transform;

            staticCulling.cullingMaster = this;
        }

        public const bool IsUseMask=true;

        private void Clear()
        {
            if (_disabledColliders != null)
                _disabledColliders.ForEach(col => col.enabled = true);

            if(IsUseMask)
            {
                if (_renderersColliders != null)
                {
                    for (int i = 0; i < _renderersColliders.Length; i++)
                        if (_renderersColliders[i] != null)
                            Object.DestroyImmediate(_renderersColliders[i].gameObject);
                }
            }


            if (_casters != null)
            {
                for (int i = 0; i < _casters.Count; i++)
                    if (_casters[i] != null)
                    {
                        _casters[i].Dispose();

                        Object.DestroyImmediate(_casters[i].gameObject);
                    }
            }

            _dataContainer?.Dispose();
        }
    }

    
}
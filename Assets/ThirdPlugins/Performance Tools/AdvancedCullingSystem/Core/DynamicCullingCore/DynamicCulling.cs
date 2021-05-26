using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System;
using AdvancedCullingSystem.StaticCullingCore;

namespace AdvancedCullingSystem.DynamicCullingCore
{
    public class DynamicCulling : MonoBehaviour
    {
        public static DynamicCulling Instance { get; private set; }

        [SerializeField] private List<Camera> _cameras = new List<Camera>();
        [SerializeField] private List<MeshRenderer> _startRenderers = new List<MeshRenderer>();
        [SerializeField] private int _jobsPerFrame = 500;
        [SerializeField] private float _objectsLifetime = 1.5f;
        [SerializeField] private int UnitSize = 4;
        [SerializeField] private int FieldOfView = 61;

        //[SerializeField]
        //[HideInInspector]
        //private List<Mesh> _startMeshes = new List<Mesh>();//这个没什么用，而且物体多了还乱了

        private Dictionary<MeshRenderer, Mesh> _startMeshes = new Dictionary<MeshRenderer, Mesh>();

        [SerializeField]
        [HideInInspector]
        private List<Collider> _occluders = new List<Collider>();


        private Dictionary<int, MeshRenderer> _indexToRenderer = new Dictionary<int, MeshRenderer>();

        private List<int> HideRenders = new List<int>();

        private List<int> ShowRenders = new List<int>();//这两个是否要改成Dictionary？

        public int SelectedCount
        {
            get
            {
                return _indexToRenderer.Count;
            }
        }

        public int VisibleCount;

        public int HideCount;

        private Dictionary<Collider, int> _colliderToIndex = new Dictionary<Collider, int>();
        private List<Collider> _occludeColliders = new List<Collider>();

        private List<Camera> _camerasForRemove = new List<Camera>();
        private List<int> _renderersForRemoveIDs = new List<int>();

        [ReadOnly] private NativeArray<float3> _rayDirs;
        private NativeList<int> _visibleObjects;
        private NativeList<int> _hittedObjects;
        private NativeList<float> _timers;
        private NativeList<JobHandle> _handles;
        private List<NativeArray<RaycastCommand>> _rayCommands;
        private List<NativeArray<RaycastHit>> _hitResults;

        private int _mask;
        private int _layer;
        public int _dirsOffsetIndex;
        private int _newJobsCount;
        private bool _onUpdateJobsPerFrame;

        private void InitCameras()
        {
            for (int i = 0; i < _cameras.Count; i++)
            {
                if (_cameras[i] == null || _cameras[i].enabled == false || _cameras[i].gameObject.activeInHierarchy == false)
                {
                    Debug.Log("DynamicCulling::Looks like camera was destroyed");
                    _cameras.RemoveAt(i);
                    i--;
                }
            }

            if (_cameras.Count == 0)
            {
                _cameras.AddRange(GameObject.FindObjectsOfType<Camera>());
            }

        }

        private void FindDestroyedCameras()
        {
            for (int i = 0; i < _cameras.Count; i++)
            {
                if (_cameras[i] == null)
                {
                    Debug.Log("DynamicCulling::Looks like camera was destroyed");
                    _camerasForRemove.Add(_cameras[i]);
                }
            }
        }

        private bool CheckCameras()
        {
            if (_cameras.Count == 0)
            {
                Debug.Log("DynamicCulling::no cameras assigned");
                return false;
            }

            return true;
        }

        public StaticCulling staticCulling;

        private void Awake()
        {
            Instance = this;

            _rayDirs= Core.CasterUtility.CreateRayDirsArray(_jobsPerFrame, UnitSize, FieldOfView); //创建射线方向，基于Halton序列

             _layer = ACSInfo.CullingLayer;
            _mask = ACSInfo.CullingMask;

            _visibleObjects = new NativeList<int>(Allocator.Persistent);
            _hittedObjects = new NativeList<int>(Allocator.Persistent);
            _timers = new NativeList<float>(Allocator.Persistent);
            _handles = new NativeList<JobHandle>(Allocator.Persistent);
            _rayCommands = new List<NativeArray<RaycastCommand>>();
            _hitResults = new List<NativeArray<RaycastHit>>();

            InitCameras();
            var camerasCopy = _cameras.Where(c => c != null && c.enabled == true || c.gameObject.activeInHierarchy == true).ToList();
            
            DateTime start=DateTime.Now;
            if(_startRenderers.Count==0){
                _startRenderers=GameObject.FindObjectsOfType<MeshRenderer>().ToList();
            }
            Debug.LogError("DynamicCulling.Awake FindObjectsOfType:"+(DateTime.Now-start).ToString());
            
            MeshRenderer[] renderersCopy = _startRenderers.Where(r => r != null).ToArray();

            _cameras.Clear();

            AddCameras(camerasCopy.ToArray());
            AddOccluders(_occluders.ToArray());
            AddObjectsForCulling(renderersCopy);
        }

        public void AddAllCamera()
        {

        }

        public void AddAllRenderers()
        {

        }

        private void Start()
        {
            staticCulling = StaticCulling.Instance;//Add StaticCulling Link
        }

        private void LogException(System.Exception ex)
        {
            Debug.Log("Dynamic Culling will be disabled");
            Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
            Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
            Debug.Log("-----------------------------------");
        }

        private DateTime startUpdateTime;

        public double UpdateTimeTotal;

        public double UpdateTime1;

        public double UpdateTime2;

        [ContextMenu("UpdateInfo1")]
        public void UpdateInfo1()
        {
            try
            {
                startUpdateTime = DateTime.Now;
                FindDestroyedCameras();
                OnRemoveCamerasFromList();
                if (!CheckCameras())
                {
                    Disable(); return;
                }
                _hittedObjects.Clear();//清理

                _handles.Clear();
                for (int i = 0; i < _cameras.Count; i++)
                {
                    _handles.Add(new CreateRayCommandsJob()
                    {
                        position = _cameras[i].transform.position,
                        rotation = _cameras[i].transform.rotation,
                        dirsOffsetIdx = _dirsOffsetIndex,
                        rayDirs = _rayDirs,
                        mask = _mask,
                        rayCommands = _rayCommands[i]

                    }.Schedule(_jobsPerFrame, 64, default));//创建射线检测
                }
                if ((_dirsOffsetIndex += _jobsPerFrame) >= (_rayDirs.Length - _jobsPerFrame))
                    _dirsOffsetIndex = 0;

                JobHandle.CompleteAll(_handles);//完成射线检测

                _handles.Clear();
                for (int i = 0; i < _cameras.Count; i++)
                    _handles.Add(RaycastCommand.ScheduleBatch(_rayCommands[i], _hitResults[i], 1, default));
                _handles.Add(new UpdateTimersJob()
                {
                    timers = _timers,
                    deltaTime = Time.deltaTime
                }.Schedule());
            }
            catch (System.Exception ex)
            {
                LogException(ex);
                Disable();
            }

            UpdateTime1 = (DateTime.Now - startUpdateTime).TotalMilliseconds;
        }

        private void CollectHitObjects()
        {
            int c1 = 0;
            int c2 = 0;
            for (int i = 0; i < _hitResults.Count; i++)
            {
                NativeArray<RaycastHit> hits = _hitResults[i];
                for (int j = 0; j < hits.Length; j++)
                {
                    c1++;
                    
                    Collider collider = hits[j].collider;

                    //if (collider != null && !_occludeColliders.Contains(collider))
                    //    _hittedObjects.Add(_colliderToIndex[collider]);

                    if (collider != null && !_occludeColliders.Contains(collider))
                    {
                        int objId = _colliderToIndex[collider];
                        if(!_hittedObjects.Contains(objId))
                        {
                            _hittedObjects.Add(objId);
                            c2++;
                        }
                        
                    }
                }
            }
            //Debug.LogError("CollectHitObjects:"+c2+"/"+ c1);
        }

        public List<Renderer> visibleRenderers = new List<Renderer>();

        public bool IsSetVisible = true;

        public double RaycastTime = 0;

        [ContextMenu("UpdateInfo2")]
        public void UpdateInfo2()
        {
            DateTime start = DateTime.Now;
            try
            {
                visibleRenderers.Clear();
                HideRenders.Clear();
                ShowRenders.Clear();

                JobHandle.CompleteAll(_handles);//完成UpdateTimersJob和RaycastCommand？ 5000用时5ms

                RaycastTime = (DateTime.Now - start).TotalMilliseconds;

                CollectHitObjects();//收集碰撞检测的结果：从_hitResults将碰撞物体放到_hittedObjects
                new ComputeResultsJob()
                {
                    visibleObjects = _visibleObjects,
                    hittedObjects = _hittedObjects,
                    timers = _timers
                }.Schedule().Complete();
                int c = 0;
                while (c < _visibleObjects.Length)//根据碰撞检测和时间，显示或者隐藏物体
                {
                    int id = _visibleObjects[c];
                    try
                    {
                        if (_timers[c] > _objectsLifetime)
                        {
                            if(IsSetVisible)
                                RendererHelper.HideRenderer(_indexToRenderer[id]);
                            _visibleObjects.RemoveAtSwapBack(c);
                            _timers.RemoveAtSwapBack(c);

                            if (!HideRenders.Contains(id))
                                HideRenders.Add(id);
                        }
                        else
                        {
                            visibleRenderers.Add(_indexToRenderer[id]);
                            if (IsSetVisible)
                                RendererHelper.ShowRenderer(_indexToRenderer[id]);
                            c++;
                            if (!ShowRenders.Contains(id))
                                ShowRenders.Add(id);
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        _renderersForRemoveIDs.Add(id);
                        c++;
                    }
                }
                OnRemoveRenderersFromList();
                if (_onUpdateJobsPerFrame)
                    OnUpdateJobsPerFrame();
            }
            catch (System.Exception ex)
            {
                LogException(ex);
                Disable();
            }

            VisibleCount = ShowRenders.Count;
            HideCount = HideRenders.Count;
            UpdateTimeTotal = (DateTime.Now - startUpdateTime).TotalMilliseconds;
            UpdateTime2= (DateTime.Now - start).TotalMilliseconds;
        }

        public bool EnableUpdateInfo = true;

        public void SetCullingEnable(bool value)
        {
            IsSetVisible = value;
            EnableUpdateInfo = value;
        }


        private void Update()
        {
            if(EnableUpdateInfo)
                UpdateInfo1();
        }

        private void LateUpdate()//接上面的Update()
        {
            if(EnableUpdateInfo)
                UpdateInfo2();
        }

        private void OnRemoveCamerasFromList()
        {
            if (_camerasForRemove.Count == 0)
                return;

            for (int i = 0; i < _camerasForRemove.Count; i++)
                RemoveCameraFromList(_camerasForRemove[i]);

            _camerasForRemove.Clear();
        }

        private void OnRemoveRenderersFromList()
        {
            if (_renderersForRemoveIDs.Count == 0)
                return;

            for (int i = 0; i < _renderersForRemoveIDs.Count; i++)
                RemoveRendererFromList(_renderersForRemoveIDs[i]);

            _renderersForRemoveIDs.Clear();

            RemoveEmptyCollidersRefs();
        }

        private void OnUpdateJobsPerFrame()
        {
            _jobsPerFrame = _newJobsCount;

            for (int i = 0; i < _rayCommands.Count; i++)
            {
                _rayCommands[i].Dispose();
                _hitResults[i].Dispose();

                _rayCommands[i] = new NativeArray<RaycastCommand>(_jobsPerFrame, Allocator.Persistent);
                _hitResults[i] = new NativeArray<RaycastHit>(_jobsPerFrame, Allocator.Persistent);
            }

            _onUpdateJobsPerFrame = false;
        }

        private void OnDestroy()
        {
            if (_handles.IsCreated && _handles.Length > 0)
            {
                JobHandle.CompleteAll(_handles);
                _handles.Dispose();
            }

            if (_rayDirs.IsCreated)
                _rayDirs.Dispose();

            if (_visibleObjects.IsCreated)
                _visibleObjects.Dispose();

            if (_hittedObjects.IsCreated)
                _hittedObjects.Dispose();

            if (_timers.IsCreated)
                _timers.Dispose();

            if(_rayCommands!=null)
                for (int i = 0; i < _rayCommands.Count; i++)
                {
                    _rayCommands[i].Dispose();
                    _hitResults[i].Dispose();
                }
        }


        private void RemoveCameraFromList(Camera camera)
        {
            int index = _cameras.IndexOf(camera);

            if (index < 0)
                return;

            _cameras.RemoveAt(index);

            _rayCommands[index].Dispose();
            _rayCommands.RemoveAt(index);

            _hitResults[index].Dispose();
            _hitResults.RemoveAt(index);
        }

        private void RemoveRendererFromList(int id)
        {
            if (!_indexToRenderer.ContainsKey(id))
                return;

            Renderer renderer = _indexToRenderer[id];

            if (renderer != null)
            {
                renderer.ShowRenderer();

                Collider collider = _colliderToIndex.First(dic => dic.Value == id).Key;

                Destroy(collider.gameObject);
            }

            _indexToRenderer.Remove(id);

            int idx = _visibleObjects.IndexOf(id);

            if (idx < 0)
                return;

            _visibleObjects.RemoveAtSwapBack(idx);
            _timers.RemoveAtSwapBack(idx);
        }

        private void RemoveEmptyCollidersRefs()
        {
            int i = 0;
            while (i < _colliderToIndex.Count)
            {
                Collider key = _colliderToIndex.Keys.ElementAt(i);
                
                if (key == null)
                    _colliderToIndex.Remove(key);

                else
                    i++;
            }
        }

        internal List<Renderer> ComputeVisibility()
        {
            UpdateInfo1();
            UpdateInfo2();
            return visibleRenderers;
        }

        public void Enable()
        {
            for (int i = 0; i < _indexToRenderer.Count; i++)
            {
                int id = _indexToRenderer.Keys.ElementAt(i);

                try
                {
                    _indexToRenderer[id].HideRenderer();
                }
                catch (MissingReferenceException)
                {
                    _renderersForRemoveIDs.Add(id);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Dynamic Culling has errors");
                    Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                    Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                    Debug.Log("-----------------------------------");
                }
            }

            FindDestroyedCameras();

            OnRemoveCamerasFromList();
            OnRemoveRenderersFromList();

            if (!CheckCameras())
            {
                Disable();
                return;
            }

            enabled = true;
        }

        public void Disable()
        {
            for (int i = 0; i < _indexToRenderer.Count; i++)
            {
                int id = _indexToRenderer.Keys.ElementAt(i);

                try
                {
                    _indexToRenderer[id].ShowRenderer();
                }
                catch (MissingReferenceException)
                {
                    _renderersForRemoveIDs.Add(id);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Dynamic Culling has errors");
                    Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                    Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                    Debug.Log("-----------------------------------");
                }
            }

            OnRemoveRenderersFromList();

            enabled = false;
        }

        public void SetObjectsLifetime(int value)
        {
            _objectsLifetime = Mathf.Max(0.25f, value);
        }

        public void SetJobsPerFrame(int value)
        {
            if (_jobsPerFrame == value)
                return;

            _newJobsCount = Mathf.Max(1, value);

            _onUpdateJobsPerFrame = true;
        }


        public void AddCamera(Camera camera)
        {
            if (_cameras.Contains(camera))
                return;

            _cameras.Add(camera);

            _rayCommands.Add(new NativeArray<RaycastCommand>(_jobsPerFrame, Allocator.Persistent));
            _hitResults.Add(new NativeArray<RaycastHit>(_jobsPerFrame, Allocator.Persistent));

            if (!enabled)
                Enable();
        }

        public void AddCameras(Camera[] cameras)
        {
            if (cameras == null)
                return;

            for (int i = 0; i < cameras.Length; i++)
                if (cameras[i] != null)
                    AddCamera(cameras[i]);
        }


        public void RemoveCamera(Camera camera)
        {
            if (!_cameras.Contains(camera))
                return;

            _camerasForRemove.Add(camera);
        }

        public void RemoveObject(MeshRenderer renderer)
        {
            if (!_indexToRenderer.ContainsValue(renderer))
                return;

            int id = _indexToRenderer.First(dic => dic.Value == renderer).Key;

            _renderersForRemoveIDs.Add(id);
        }
        

        public void AddObjectsForCulling(MeshRenderer[] renderers)
        {
            if (renderers == null)
                return;

            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] != null)
                    AddObjectForCulling(renderers[i],i);
        }

        public void AddObjectForCulling(MeshRenderer renderer,int i)
        {
            if (renderer == null )
                return;

            // if (!renderer.enabled)
            //     return;

            int id = renderer.GetInstanceID();

            if (_indexToRenderer.ContainsKey(id))
                return;
            Collider collider=null;
            collider=renderer.GetComponent<Collider>();
            if(collider==null){
                collider = CreateMeshCollider(renderer, i);
            }
            //Collider collider = CreateSphereCollider(renderer, i);
            //Collider collider = CreateBoxCollider(renderer, i,_layer);
            if (collider == null) return;

            _indexToRenderer.Add(id, renderer);
            _colliderToIndex.Add(collider, id);//collider->Index->Renderer

            if (enabled)
                renderer.HideRenderer();
        }

        private Collider CreateSphereCollider(MeshRenderer renderer, int i)
        {
            MeshFilter filter = renderer.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return null;

            SphereCollider collider = renderer.gameObject.AddComponent<SphereCollider>();
            collider.radius *= 1.5f;

            collider.gameObject.layer = _layer;

            //renderer.bounds.ex

            //collider.transform.parent = renderer.transform;

            //collider.transform.localPosition = Vector3.zero;
            //collider.transform.localRotation = Quaternion.identity;
            //collider.transform.localScale = Vector3.one;

            //collider.gameObject.layer = _layer;

            return collider;
        }

        private static Collider CreateBoxCollider(MeshRenderer renderer, int i,int layer)
        {
            MeshFilter filter = renderer.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return null;

            BoxCollider collider=AddBoxCollider(renderer.transform, renderer.bounds);
            collider.gameObject.layer = layer;
            return collider;
        }

        public static BoxCollider AddBoxCollider(Transform parent, Bounds bounds)
        {
            BoxCollider box1 = parent.gameObject.AddComponent<BoxCollider>();
            GameObject newGo = new GameObject("Culling Collider");
            newGo.transform.position = parent.position;
            newGo.transform.rotation = parent.rotation;
            newGo.transform.localScale = parent.lossyScale;
            BoxCollider box2 = newGo.AddComponent<BoxCollider>();
            box2.center = box1.center;
            box2.size = box1.size;
            newGo.transform.SetParent(parent);
            GameObject.Destroy(box1);
            //newCollider = box2;
            return box2;
        }

        private Collider CreateMeshCollider(MeshRenderer renderer, int i)
        {
            MeshFilter filter = renderer.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return null;

            MeshCollider collider = new GameObject("Culling Collider" + i).AddComponent<MeshCollider>();

            collider.transform.parent = renderer.transform;

            collider.transform.localPosition = Vector3.zero;
            collider.transform.localRotation = Quaternion.identity;
            collider.transform.localScale = Vector3.one;

            collider.gameObject.layer = _layer;

            //int idx = _startRenderers.IndexOf(renderer);
            //if (idx > 0 && _startMeshes.Count > idx && _startMeshes[idx] != null)
            //    collider.sharedMesh = _startMeshes[idx];
            //else
            //    collider.sharedMesh = filter.sharedMesh;
            //这部分代码导致碰撞体错乱，一个碰撞体的Mesh是另一条物体的。

            if (_startMeshes.ContainsKey(renderer))
            {
                collider.sharedMesh = _startMeshes[renderer];
            }
            else
            {
                collider.sharedMesh = filter.sharedMesh;//cww
            }
            return collider;
        }

        public void AddOccluders(Collider[] colliders)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    AddOccluder(colliders[i]);
            }
        }

        public void AddOccluder(Collider collider)
        {
            Collider occluder = Instantiate(collider);

            foreach(Component comp in occluder.GetComponents<Component>())
            {
                if (!(comp is Transform) && !(comp is Collider))
                    Destroy(comp);
            }

            occluder.transform.position = collider.transform.position;
            occluder.transform.rotation = collider.transform.rotation;
            occluder.transform.localScale = collider.transform.lossyScale;

            occluder.transform.parent = collider.transform;
            occluder.gameObject.layer = _layer;

            _occludeColliders.Add(occluder);
        }

#if UNITY_EDITOR

        public void OnEditorAddSelectedStartRenderers()
        {
            foreach (var selected in UnityEditor.Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    //if (renderer.enabled)
                        OnEditorAddStartRenderer(renderer);
            }
        }

        public void OnEditorRemoveSelectedStartRenderers()
        {
            foreach (var selected in UnityEditor.Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    if (_startRenderers.Contains(renderer))
                        OnEditorRemoveRenderer(renderer);
            }
        }

        public void OnEditorAddAllStartRenderers()
        {
            // foreach (var renderer in FindObjectsOfType<MeshRenderer>().Where(r => r.enabled))
            //     OnEditorAddStartRenderer(renderer);
            foreach (var renderer in FindObjectsOfType<MeshRenderer>())
                OnEditorAddStartRenderer(renderer);
        }

        public void OnEditorRemoveAllStartRenderers()
        {
            _startMeshes.Clear();
            _startRenderers.Clear();
        }

        private void OnEditorAddStartRenderer(MeshRenderer renderer)
        {
            if (_startRenderers.Contains(renderer))
                return;

            Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;

            if (mesh == null)
                return;

            _startRenderers.Add(renderer);
            _startMeshes.Add(renderer,mesh);
        }

        private void OnEditorRemoveRenderer(MeshRenderer renderer)
        {
            int index = _startRenderers.IndexOf(renderer);

            if (index < 0)
                return;

            _startMeshes.Remove(renderer);
            _startRenderers.RemoveAt(index);
        }


        public void OnEditorAddSelectedOccluders()
        {
            foreach (var selected in UnityEditor.Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null)
                    _occluders.Add(collider);
            }

            _occluders = _occluders.Distinct().Where(c => c != null).ToList();
        }

        public void OnEditorRemoveSelectedOccluders()
        {
            foreach (var selected in UnityEditor.Selection.gameObjects)
            {
                Collider collider = selected.GetComponent<Collider>();

                if (collider != null && _occluders.Contains(collider))
                    _occluders.Remove(collider);
            }

            _occluders = _occluders.Distinct().Where(c => c != null).ToList();
        }

        public void OnEditorRemoveAllOccluders()
        {
            _occluders.Clear();
        }

#endif
    }

    [BurstCompile]
    public struct UpdateTimersJob : IJob
    {
        public NativeList<float> timers;

        [ReadOnly]
        public float deltaTime;

        public void Execute()
        {
            for (int i = 0; i < timers.Length; i++)
                timers[i] += deltaTime;
        }
    }

    [BurstCompile]
    public struct CreateRayCommandsJob : IJobParallelFor
    {
        [ReadOnly] public float3 position;
        [ReadOnly] public quaternion rotation;

        [ReadOnly] public int dirsOffsetIdx;

        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<float3> rayDirs;

        [ReadOnly] public int mask;
        [WriteOnly] public NativeArray<RaycastCommand> rayCommands;

        public void Execute(int index)
        {
            int id=dirsOffsetIdx + index;
            float3 dir=rayDirs[id];
            //IndexOutOfRangeException: Index 141820 is out of restricted IJobParallelFor range [320...383] in ReadWriteBuffer.
            //ReadWriteBuffers are restricted to only read & write the element at the job index. You can use double buffering strategies to avoid race conditions due to reading & writing in parallel to the same elements from a job.
            float3 direction = math.mul(rotation, dir);

            // float3 end = position + direction * 100;
            // Debug.DrawLine(position, end, Color.red,0.1f);//测试
            if(StaticCullingMaster.IsUseMask)
            {
                RaycastCommand command = new RaycastCommand(position, direction, layerMask : mask);
                rayCommands[index] = command;
            }
            else
            {
                RaycastCommand command = new RaycastCommand(position, direction);
                rayCommands[index] = command;
            }
        }
    }

    [BurstCompile]
    public struct ComputeResultsJob : IJob
    {
        public NativeList<int> visibleObjects;

        [ReadOnly]
        public NativeList<int> hittedObjects;

        [WriteOnly]
        public NativeList<float> timers;

        public void Execute()
        {
            for (int i = 0; i < hittedObjects.Length; i++)
            {
                int id = hittedObjects[i];
                int index = visibleObjects.IndexOf(id);

                if (index < 0)
                {
                    visibleObjects.Add(id);
                    timers.Add(0);
                }
                else
                    timers[index] = 0;
            }
        }
    }
}
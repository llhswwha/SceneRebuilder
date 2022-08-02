using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections;
using System;

namespace AdvancedCullingSystem.DynamicCullingCore
{
    public class DynamicCulling : SingletonBehaviour<DynamicCulling>
    {
        //public static DynamicCulling Instance { get; private set; }

        [SerializeField] private List<Camera> _cameras = new List<Camera>();
        [SerializeField] private List<MeshRenderer> _startRenderers = new List<MeshRenderer>();
        [SerializeField] private int _jobsPerFrame = 500;
        [SerializeField] private float _objectsLifetime = 1.5f;

        [SerializeField]
        [Range(2, 179)]
        private int _fieldOfView = 60;

        public void Clear()
        {
            _startRenderers.Clear();
            _startMeshes.Clear();
            _occluders.Clear();
            _indexToRenderer.Clear();
            _occludeColliders.Clear();
            _renderersForRemoveIDs.Clear();
        }

        public List<MeshRenderer> GetStartRenderers()
        {
            return _startRenderers;
        }

        [SerializeField]
        [HideInInspector]
        private List<Mesh> _startMeshes = new List<Mesh>();

        [SerializeField]
        [HideInInspector]
        private List<Collider> _occluders = new List<Collider>();


        private Dictionary<int, MeshRenderer> _indexToRenderer = new Dictionary<int, MeshRenderer>();
        private Dictionary<Collider, int> _colliderToIndex = new Dictionary<Collider, int>();
        private List<Collider> _occludeColliders = new List<Collider>();

        public int GetRendererCount()
        {
            return _indexToRenderer.Count;
        }

        public int GetVisibleCount()
        {
            return _visibleObjects.Length;
        }

        public int GetHittedCount()
        {
            return _hittedObjects.Length;
        }

        private List<Camera> _camerasForRemove = new List<Camera>();
        private List<int> _renderersForRemoveIDs = new List<int>();

        private NativeArray<float3> _rayDirs;
        private NativeList<int> _visibleObjects;
        private NativeList<int> _hittedObjects;
        private NativeList<float> _timers;
        private NativeList<JobHandle> _handles;
        private List<NativeArray<RaycastCommand>> _rayCommands;
        private List<NativeArray<RaycastHit>> _hitResults;

        private int _mask;
        private int _layer;
        private int _dirsOffsetIndex;
        private int _newJobsCount;
        private bool _onUpdateJobsPerFrame;

        [ContextMenu("InitCameras")]
        public void InitCameras()
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
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    _cameras.Add(mainCamera);
                }
            }

            if (_cameras.Count == 0)
            {
                var cams = GameObject.FindObjectsOfType<Camera>();
                _cameras.AddRange(cams);
            }

            //if (_cameras.Count == 0)
            //{
            //    _cameras.AddRange(GameObject.FindObjectsOfType<Camera>());
            //}
        }


        private float HaltonSequence(int index, int b)
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

        private void CreateRayDirs()
        {
            int dirsCount = Mathf.RoundToInt(((Screen.width * Screen.height) / 4) / _jobsPerFrame) * _jobsPerFrame;

            _rayDirs = new NativeArray<float3>(dirsCount, Allocator.Persistent);

            Camera camera = new GameObject().AddComponent<Camera>();
            camera.fieldOfView = _fieldOfView + 1;

            for (int i = 0; i < _rayDirs.Length; i++)
            {
                Vector2 screenPoint = new Vector2(HaltonSequence(i, 2), HaltonSequence(i, 3));

                Ray ray = camera.ViewportPointToRay(new Vector3(screenPoint.x, screenPoint.y, 0));

                _rayDirs[i] = ray.direction;
            }

            Destroy(camera.gameObject);
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



        private void Awake()
        {
            Debug.Log($"DynamicCulling.Awake");
            //Instance = this;

            CreateRayDirs();

            _layer = ACSInfo.CullingLayer;
            _mask = ACSInfo.CullingMask;

            _visibleObjects = new NativeList<int>(Allocator.Persistent);
            _hittedObjects = new NativeList<int>(Allocator.Persistent);
            _timers = new NativeList<float>(Allocator.Persistent);
            _handles = new NativeList<JobHandle>(Allocator.Persistent);
            _rayCommands = new List<NativeArray<RaycastCommand>>();
            _hitResults = new List<NativeArray<RaycastHit>>();

            InitCameras();
            Camera[] camerasCopy = _cameras.Where(c => c != null).ToArray();
            MeshRenderer[] renderersCopy = _startRenderers.Where(r => r != null).ToArray();

            _cameras.Clear();

            AddCameras(camerasCopy);
            AddOccluders(_occluders.ToArray());
            AddObjectsForCulling(renderersCopy);
        }

        private float timecount=0;
        private bool isCanLateUpdate = false;
        public float DynamicCullingTimeInterval = 0;

        public bool IsEnableCulling = true;

        private void Update()
        {
            try
            {
                if (IsEnableCulling == false)
                {
                    return;
                }
                //if (SystemSettingHelper.systemSetting != null && timecount < SystemSettingHelper.systemSetting.DynamicCullingTimeInterval)//0.5s执行计算1次
                float timecountT = 0;
                if (DynamicCullingTimeInterval > 0 && timecount < DynamicCullingTimeInterval) 
                {
                    timecount += Time.deltaTime;
                    return;
                }
                else
                {
                    timecountT = timecount;
                    timecount = 0;
                    isCanLateUpdate = true;
                }
                FindDestroyedCameras();

                OnRemoveCamerasFromList();

                if (!CheckCameras())
                {
                    Disable();
                    return;
                }

                _hittedObjects.Clear();
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

                    }.Schedule(_jobsPerFrame, 64, default));//.Schedule(_jobsPerFrame, 64, default));
                }

                if ((_dirsOffsetIndex += _jobsPerFrame) >= (_rayDirs.Length - _jobsPerFrame))
                    _dirsOffsetIndex = 0;

                JobHandle.CompleteAll(_handles);
                //Debug.Log($"DynamicCulling.Update1 _handles:{_handles.Length} ");

                _handles.Clear();
                for (int i = 0; i < _cameras.Count; i++)
                    _handles.Add(RaycastCommand.ScheduleBatch(_rayCommands[i], _hitResults[i], 1, default));

                _handles.Add(new UpdateTimersJob()
                {

                    timers = _timers,
                    deltaTime = Time.deltaTime + timecountT

                }.Schedule());

                //Debug.Log($"DynamicCulling.Update2 _handles:{_handles.Length} ");
            }
            catch (System.Exception ex)
            {
                Debug.Log("Dynamic Culling will be disabled");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }
        }

        public void HideAll()
        {

        }

        public void ShowAll()
        {

        }

        private void LateUpdate()
        {
            try
            {
                if (IsEnableCulling == false)
                {
                    return;
                }

                if (!isCanLateUpdate) return;
                isCanLateUpdate = false;
                //Debug.Log($"DynamicCulling.LateUpdate1 _hitResults:{_hitResults.Count} ");

                JobHandle.CompleteAll(_handles);

                //Debug.Log($"DynamicCulling.LateUpdate2 _hitResults:{_hitResults.Count} ");
                for (int i = 0; i < _hitResults.Count; i++)
                {
                    NativeArray<RaycastHit> hits = _hitResults[i];

                    for (int j = 0; j < hits.Length; j++)
                    {
                        Collider collider = hits[j].collider;

                        if (collider != null && !_occludeColliders.Contains(collider))
                            _hittedObjects.Add(_colliderToIndex[collider]);
                    }
                }


                //Debug.Log($"DynamicCulling.LateUpdate3 _hittedObjects:{_hittedObjects.Length} ");

                new ComputeResultsJob()
                {

                    visibleObjects = _visibleObjects,
                    hittedObjects = _hittedObjects,
                    timers = _timers

                }.Schedule().Complete();

                int c = 0;
                while (c < _visibleObjects.Length)
                {
                    int id = _visibleObjects[c];
                    if(_indexToRenderer.ContainsKey(id))
                    {
                        try
                        {
                            if (_timers[c] > _objectsLifetime)
                            {
                                _indexToRenderer[id].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                                _visibleObjects.RemoveAtSwapBack(c);
                                _timers.RemoveAtSwapBack(c);
                            }
                            else
                            {
                                _indexToRenderer[id].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                                c++;
                            }
                        }
                        catch (MissingReferenceException ex)
                        {
                            _renderersForRemoveIDs.Add(id);
                            c++;
                        }
                    }
                    else
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
                Debug.Log("Dynamic Culling will be disabled");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }
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

            if (_rayCommands != null)
            {
                for (int i = 0; i < _rayCommands.Count; i++)
                {
                    _rayCommands[i].Dispose();
                    _hitResults[i].Dispose();
                }
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
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

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
        List<Collider> empty;
        private void RemoveEmptyCollidersRefs()
        {
            //Add by wk,处理while+ElementAt导致的，数据过万条，程序卡死问题 20220308
            //DateTime r = DateTime.Now;
            //string log = "";
            if(empty==null)empty = new List<Collider>();
            foreach(KeyValuePair<Collider,int> item in _colliderToIndex)
            {
                if (item.Key == null)
                {
                    empty.Add(item.Key);
                }
            }
            for(int m=0;m<empty.Count;m++)
            {
                _colliderToIndex.Remove(empty[m]);
            }

            //log += string.Format("1.costTime:{0}ms",(DateTime.Now-r).TotalMilliseconds);
            //r = DateTime.Now;

            //var result2 = _colliderToIndex.Where(k=>k.Key==null);
            //foreach(var item in result2)
            //{
            //    _colliderToIndex.Remove(item.Key);
            //}
            //log += string.Format("2.costTime:{0}ms", (DateTime.Now - r).TotalMilliseconds);
            //r = DateTime.Now;

            //int i = 0;
            //while (i < _colliderToIndex.Count)
            //{
            //    Collider key = _colliderToIndex.Keys.ElementAt(i);

            //    if (key == null)
            //        _colliderToIndex.Remove(key);

            //    else
            //        i++;
            //}
            //log += string.Format("3.costTime:{0}ms", (DateTime.Now - r).TotalMilliseconds);
            //r = DateTime.Now;
        }



        public void Enable()
        {
            InitCameras();

            for (int i = 0; i < _indexToRenderer.Count; i++)
            {
                int id = _indexToRenderer.Keys.ElementAt(i);

                try
                {
                    _indexToRenderer[id].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
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
            Debug.Log("DynamicCulling Disable");
            for (int i = 0; i < _indexToRenderer.Count; i++)
            {
                int id = _indexToRenderer.Keys.ElementAt(i);

                try
                {
                    _indexToRenderer[id].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
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

        public float GetObjectsLifetime()
        {
            return _objectsLifetime;
        }

        public void SetObjectsLifetime(float value)
        {
            _objectsLifetime = Mathf.Max(0.25f, value);
        }

        public int GetJobsPerFrame()
        {
            return _newJobsCount;
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
            if (renderer == null) return;
            if (!_indexToRenderer.ContainsValue(renderer))
                return;

            int id = _indexToRenderer.First(dic => dic.Value == renderer).Key;

            _renderersForRemoveIDs.Add(id);
        }


        public int AddObjectsForCulling(MeshRenderer[] renderers)
        {
            if (renderers == null)
                return 0;

            int count = 0;
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i] != null)
                {
                    if (AddObjectForCulling(renderers[i]))
                    {
                        count++;
                    }
                }
            return count;
        }

        public IEnumerator AddObjectsForCullingCoroutine(MeshRenderer[] renderers)
        {
            int count = 0;
            if(renderers!=null&& renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                    if (renderers[i] != null)
                    {
                        if (AddObjectForCulling(renderers[i]))
                        {
                            count++;
                        }
                        yield return null;
                    }
            }
            yield return null;
        }

        public bool isIncludeStatic = true;

        private string IgnoreCullingTag = "IgnoreDynamicCulling";
        public bool AddObjectForCulling(MeshRenderer renderer)
        {
            if (renderer == null)
                return false;

            // if (!renderer.enabled)
            //     return;

            if (isIncludeStatic==false && renderer.gameObject.isStatic)//add_cww
            {
                return false;
            }
            //Tag标记为不参与遮挡剔除的 add_wk
            if (renderer.gameObject.tag == IgnoreCullingTag) return false;
            if (renderer.transform.GetComponent<BoundsBox>()) return false;
            int id = renderer.GetInstanceID();

            if (_indexToRenderer.ContainsKey(id))
                return false;

            MeshFilter filter = renderer.GetComponent<MeshFilter>();

            if (filter == null || filter.sharedMesh == null)
                return false;

            MeshCollider collider = new GameObject("Culling Collider").AddComponent<MeshCollider>();

            collider.transform.parent = renderer.transform;

            collider.transform.localPosition = Vector3.zero;
            collider.transform.localRotation = Quaternion.identity;
            collider.transform.localScale = Vector3.one;

            collider.gameObject.layer = _layer;

            int idx = _startRenderers.IndexOf(renderer);

            if (idx > 0 && _startMeshes.Count > idx && _startMeshes[idx] != null)
                collider.sharedMesh = _startMeshes[idx];
            else
                collider.sharedMesh = filter.sharedMesh;

            _indexToRenderer.Add(id, renderer);
            _colliderToIndex.Add(collider, id);

            if (enabled)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            return true;
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

            foreach (Component comp in occluder.GetComponents<Component>())
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

        /// <summary>
        /// 添加遮挡剔除Renderers
        /// </summary>
        /// <param name="renderers"></param>
        public int AddStartRenderers(MeshRenderer[] renderers)
        {
            RemoveStartRenderers();
            int count = 0;
            for (int i = 0; i < renderers.Length; i++)
            {
                //if (i > 30000) return;
                if (AddStartRenderer(renderers[i]))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 添加遮挡剔除Renderer
        /// </summary>
        /// <param name="renderer"></param>
        private bool AddStartRenderer(MeshRenderer renderer)
        {
            if (_startRenderers.Contains(renderer))
                return false;

            MeshFilter meshFilterT = renderer.GetComponent<MeshFilter>();
            if (meshFilterT == null) return false;

            Mesh mesh = meshFilterT.sharedMesh;

            if (mesh == null)
                return false;

            _startRenderers.Add(renderer);
            _startMeshes.Add(mesh);
            return true;
        }

        /// <summary>
        /// 移除所有遮挡剔除Renderer
        /// </summary>
        public void RemoveStartRenderers()
        {
            _startMeshes.Clear();
            _startRenderers.Clear();
        }

#if UNITY_EDITOR

        public void OnEditorAddSelectedStartRenderers()
        {
            foreach (var selected in UnityEditor.Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                    if (renderer.enabled)
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
            foreach (var renderer in FindObjectsOfType<MeshRenderer>().Where(r => r.enabled))
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
            MeshFilter mf=renderer.GetComponent<MeshFilter>();
            if(mf==null){
                Debug.LogError($"OnEditorAddStartRenderer MeshFilter==null renderer:{renderer}");
                return;
            }
            Mesh mesh = mf.sharedMesh;

            if (mesh == null)
                return;

            _startRenderers.Add(renderer);
            _startMeshes.Add(mesh);
        }

        private void OnEditorRemoveRenderer(MeshRenderer renderer)
        {
            int index = _startRenderers.IndexOf(renderer);

            if (index < 0)
                return;

            _startMeshes.RemoveAt(index);
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

        [ReadOnly]
        public int dirsOffsetIdx;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<float3> rayDirs;

        [ReadOnly]
        public int mask;

        [WriteOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<RaycastCommand> rayCommands;

        public void Execute(int index)
        {
            float3 direction = math.mul(rotation, rayDirs[dirsOffsetIdx + index]);

            RaycastCommand command = new RaycastCommand(position, direction, layerMask: mask);

            rayCommands[index] = command;
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
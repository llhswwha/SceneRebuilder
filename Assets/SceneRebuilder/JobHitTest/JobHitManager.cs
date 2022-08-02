using AdvancedCullingSystem;
using JobHitManagers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobHitManager : MonoBehaviour
{
    public List<GameObject> startRendererRoots = new List<GameObject>();
    public List<MeshRenderer> startRenderers = new List<MeshRenderer>();
    public List<Camera> cameras = new List<Camera>();

    [SerializeField] private int _jobsPerFrame = 500;
    [SerializeField] private float _objectsLifetime = 1.5f;

    [SerializeField]
    [Range(2, 179)]
    private int _fieldOfView = 60;

    [SerializeField]
    [HideInInspector]
    private List<Mesh> _startMeshes = new List<Mesh>();

    [SerializeField]
    [HideInInspector]
    private List<Collider> _occluders = new List<Collider>();

    private Dictionary<int, MeshRenderer> _indexToRenderer = new Dictionary<int, MeshRenderer>();
    private Dictionary<Collider, int> _colliderToIndex = new Dictionary<Collider, int>();
    private List<Collider> _occludeColliders = new List<Collider>();

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

    public string IgnoreCullingTag = "IgnoreDynamicCulling";

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        if (cameras.Count == 0)
        {
            cameras.Add(Camera.main);
        }

        CreateRayDirs(false);

        _layer = ACSInfo.CullingLayer;
        _mask = ACSInfo.CullingMask;

        _visibleObjects = new NativeList<int>(Allocator.Persistent);
        _hittedObjects = new NativeList<int>(Allocator.Persistent);
        _timers = new NativeList<float>(Allocator.Persistent);
        _handles = new NativeList<JobHandle>(Allocator.Persistent);
        _rayCommands = new List<NativeArray<RaycastCommand>>();
        _hitResults = new List<NativeArray<RaycastHit>>();

        Camera[] camerasCopy = cameras.Where(c => c != null).ToArray();
        cameras.Clear();
        AddCameras(camerasCopy);

        InitStartRenderersForCulling();
    }

    private void InitStartRenderersForCulling()
    {
        foreach (var root in startRendererRoots)
        {
            var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            startRenderers.AddRange(renderers);
        }
        MeshRenderer[] renderersCopy = startRenderers.Where(i => i != null).ToArray();
        AddObjectsForCulling(renderersCopy);
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

    public bool AddObjectForCulling(MeshRenderer renderer)
    {
        if (renderer == null) return false;
        if (renderer.gameObject.tag == IgnoreCullingTag) return false;
        int id = renderer.GetInstanceID();
        if (_indexToRenderer.ContainsKey(id)) return false;

        MeshFilter filter = renderer.GetComponent<MeshFilter>();
        if (filter == null || filter.sharedMesh == null) return false;
        MeshCollider collider = new GameObject("Culling Collider").AddComponent<MeshCollider>();
        collider.transform.parent = renderer.transform;
        collider.transform.localPosition = Vector3.zero;
        collider.transform.localRotation = Quaternion.identity;
        collider.transform.localScale = Vector3.one;
        collider.gameObject.layer = _layer;

        int idx = startRenderers.IndexOf(renderer);
        if(idx>0 && _startMeshes.Count>idx && _startMeshes[idx] != null)
        {
            collider.sharedMesh = _startMeshes[idx];
        }
        else
        {
            collider.sharedMesh = filter.sharedMesh;
        }
        _indexToRenderer.Add(id, renderer);
        _colliderToIndex.Add(collider, id);
        if (enabled)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        return true;

    }

    private void AddCameras(Camera[] cams)
    {
        if (cams == null) return;
        foreach (var item in cams)
        {
            if (item == null) continue;
            AddCamera(item);
        }
    }
    public void AddCamera(Camera camera)
    {
        if (cameras.Contains(camera))
            return;

        cameras.Add(camera);

        _rayCommands.Add(new NativeArray<RaycastCommand>(_jobsPerFrame, Allocator.Persistent));
        _hitResults.Add(new NativeArray<RaycastHit>(_jobsPerFrame, Allocator.Persistent));

        //if (!enabled)
        //    Enable();
    }

    public bool IsDebug = false;

    [ContextMenu("TestCreateRayDirs")]
    private void TestCreateRayDirs()
    {
        CreateRayDirs(true);
    }

    private void CreateRayDirs(bool isDebug)
    {
        if (isDebug)
        {
            MeshHelper.ClearChildren(this.transform);
        }
        int dirsCount = Mathf.RoundToInt((Screen.width * Screen.height / 4) / _jobsPerFrame) * _jobsPerFrame;
        _rayDirs = new NativeArray<float3>(dirsCount, Allocator.Persistent);

        Camera camera = new GameObject().AddComponent<Camera>();
        camera.fieldOfView = _fieldOfView + 1;
        for (int i = 0; i < _rayDirs.Length; i++)
        {
            Vector2 screePoint = new Vector2(HaltonSequence(i, 2), HaltonSequence(i, 3));
            Ray ray = camera.ViewportPointToRay(new Vector3(screePoint.x, screePoint.y, 0));
            _rayDirs[i] = ray.direction;
            if (isDebug)
            {
                PointHelper.CreatePoint(screePoint, $"screePoint[{i}]", this.transform, 0.01f);
            }
        }
        Destroy(camera.gameObject);

        Debug.Log($"JobHitManager.CreateRayDirs width:{Screen.width} height:{Screen.height} _jobsPerFrame:{_jobsPerFrame} dirsCount:{dirsCount}");
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

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            CreateHitCommandsJob job = new CreateHitCommandsJob();
            job.positin = cameras[i].transform.position;
            job.rotation = cameras[i].transform.rotation;
            job.dirsOffsetIdx = _dirsOffsetIndex;
            job.rayDirs = _rayDirs;
            job.mask = _mask;
            job.rayCommands = _rayCommands[i];
            _handles.Add(job.Schedule(_jobsPerFrame, 64, default));
        }

        //if ((_dirsOffsetIndex += _jobsPerFrame) >= (_rayDirs.Length - _jobsPerFrame))
        //    _dirsOffsetIndex = 0;
        _dirsOffsetIndex += _jobsPerFrame;
        if(_dirsOffsetIndex>=_rayDirs.Length- _jobsPerFrame)
        {
            _dirsOffsetIndex = 0;
        }
        JobHandle.CompleteAll(_handles);
        _handles.Clear();

        for(int i = 0; i < cameras.Count; i++)
        {
            _handles.Add(RaycastCommand.ScheduleBatch(_rayCommands[i],_hitResults[i],1,default));
        }
        _handles.Add(new UpdateTimerJob()
        {
            timers = _timers,
            deltaTime=Time.deltaTime
        }.Schedule()); ;
    }

    private void LateUpdate()
    {
        JobHandle.CompleteAll(_handles);
        for(int i = 0; i < _hitResults.Count; i++)
        {
            NativeArray<RaycastHit> hitts = _hitResults[i];
            for(int j = 0; j < hitts.Length; j++)
            {
                Collider collider = hitts[j].collider;
                if(collider!=null && _occludeColliders.Contains(collider))
                {
                    _hittedObjects.Add(_colliderToIndex[collider]);
                }
            }
        }

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
            if (_indexToRenderer.ContainsKey(id))
            {
                try
                {
                    if(_timers[c]>_objectsLifetime)
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
    }

    private void OnRemoveRenderersFromList()
    {
        if (_renderersForRemoveIDs.Count == 0)
        {
            return;
        }

        for(int i = 0; i < _renderersForRemoveIDs.Count; i++)
        {
            RemoveRendererFromList(_renderersForRemoveIDs[i]);
        }
        _renderersForRemoveIDs.Clear();
        RemoveEmptyCollidersRefs();

    }

    private void RemoveRendererFromList(int id)
    {
        if (!_indexToRenderer.ContainsKey(id))
        {
            return;
        }
        Renderer renderer = _indexToRenderer[id];
        if (renderer != null)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Collider collider = _colliderToIndex.First(dic => dic.Value == id).Key;
            Destroy(collider.gameObject);
        }

        _indexToRenderer.Remove(id);
        int idx = _visibleObjects.IndexOf(id);
        if (idx < 0) return;
        _visibleObjects.RemoveAtSwapBack(idx);
        _timers.RemoveAtSwapBack(idx);
    }

    List<Collider> empty;
    private void RemoveEmptyCollidersRefs()
    {
        if (empty == null) empty = new List<Collider>();
        foreach(var item in _colliderToIndex)
        {
            if (item.Key == null)
            {
                empty.Add(item.Key);
            }
        }
        foreach (var item in empty)
        {
            _colliderToIndex.Remove(item);
        }
    }
}

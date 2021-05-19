using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace NGS.AdvancedRenderSystem
{
    public class AdvancedRenderSystem : MonoBehaviour
    {
        public int UpdatesPerFrame
        {
            get { return _updatesPerFrame; }
            set { _updatesPerFrame = Mathf.Max(1, value); }
        }
        public float ReplaceDistance
        {
            get { return _replaceDistance; }
            set { _replaceDistance = Mathf.Max(0, value); }
        }
        public float UpdateAngle
        {
            get { return _updateAngle; }
            set { _updateAngle = Mathf.Max(-1, value); }
        }

        [SerializeField]
        private List<Camera> _cameras = new List<Camera>();

        [SerializeField]
        private Material _billboardsMaterial;

        [SerializeField]
        private TextureResolution _maxTextureResolution = TextureResolution._512;

        [SerializeField]
        [HideInInspector]
        private int _updatesPerFrame = 15;

        [SerializeField]
        [HideInInspector]
        private float _replaceDistance = 30;

        [SerializeField]
        [HideInInspector]
        private float _updateAngle = 2.5f;

        [SerializeField]
        [HideInInspector]
        public List<MeshRenderer> _renderers = new List<MeshRenderer>();

        [SerializeField]
        [HideInInspector]
        private List<BillboardOfGroupList> _renderersGroups = new List<BillboardOfGroupList>();

        private int _layer;
        private string _layerName;
        private NativeList<JobHandle> _handles;

        private Camera _renderCamera;
        private List<GameObject> _billboardsParents = new List<GameObject>();
        private List<List<BaseBillboard>> _billboards = new List<List<BaseBillboard>>();
        private List<BillboardsUpdateData> _billboardsToUpdate = new List<BillboardsUpdateData>();
        private List<CameraPreset> _presets = new List<CameraPreset>();
        private TexturesManager _texturesManager;


        private void Awake()
        {
            if (!CheckComponents())
            {
                Debug.Log("AdvancedRenderSystem will be disabled");
                enabled = false;
                return;
            }

            UpdateAssignedData();

            _layer = ARSInfo.Layer;
            _layerName = ARSInfo.LayerName;
            _handles = new NativeList<JobHandle>(Allocator.Persistent);

            _renderCamera = CreateRenderCamera();//创建一个隐藏着的摄像头
            _texturesManager = new TexturesManager(_maxTextureResolution);

            CreateBillboards();

            CreatePresets();

            #if UNITY_2018

            Camera.onPreCull += OnCameraPreCull;
            Camera.onPostRender += OnCameraPostRender;

            #else

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRender;
            RenderPipelineManager.endCameraRendering += OnEndCameraRender;

            #endif
        }

        private bool CheckComponents(bool printInfo = true)
        {
            if (_cameras.Count == 0)
            {
                if (printInfo)
                    Debug.Log("AdvancedRenderSystem::Cameras not assigned");

                return false;
            }

            if (_renderers.Count == 0 && _renderersGroups.Count == 0)
            {
                if (printInfo)
                    Debug.Log("AdvancedRenderSystem::Renderers not assigned");

                return false;
            }

            return true;
        }

        private Camera CreateRenderCamera()
        {
            Camera renderCamera = new GameObject("ARS_RenderCamera").AddComponent<Camera>();

            renderCamera.cullingMask = LayerMask.GetMask(_layerName);

            renderCamera.clearFlags = CameraClearFlags.SolidColor;

            renderCamera.backgroundColor = new Color(0, 0, 0, 0);

            renderCamera.allowMSAA = false;

            renderCamera.allowHDR = false;

            renderCamera.useOcclusionCulling = false;

            renderCamera.enabled = false;

            return renderCamera;
        }

        private void CreateBillboards()
        {
            for (int c = 0; c < _cameras.Count; c++)
            {
                Transform parent = new GameObject("ARS_Parent_" + _cameras[c].name).transform;
                List<BaseBillboard> billboards = new List<BaseBillboard>();
                
                for (int i = 0; i < _renderers.Count; i++)
                {
                    BaseBillboard billboard = Billboard.Create(_renderers[i], _billboardsMaterial, _layer);

                    billboard.ToBillboard();

                    billboard.transform.SetParent(parent);

                    billboards.Add(billboard);
                }

                for (int i = 0; i < _renderersGroups.Count; i++)
                {
                    BaseBillboard billboard = BillboardOfGroup.Create(_renderersGroups[i].renderers.ToArray(), _billboardsMaterial, _layer);

                    billboard.ToBillboard();

                    billboard.transform.SetParent(parent);

                    billboards.Add(billboard);
                }

                _billboards.Add(billboards);
                _billboardsParents.Add(parent.gameObject);
            }
        }

        private void CreatePresets()
        {
            for (int i = 0; i < _cameras.Count; i++)
            {
                CameraPreset preset = new CameraPreset(_cameras[i], _billboards[i]);

                _presets.Add(preset);
            }
        }


        private void Update()
        {
            try
            {
                for (int i = 0; i < _presets.Count; i++)
                    _handles.Add(_presets[i].UpdateBillboardsData(Time.deltaTime));

                JobHandle.CompleteAll(_handles);

                _handles.Clear();

                for (int i = 0; i < _presets.Count; i++)
                {
                    _handles.Add(_presets[i].SortBillboardsByTime());
                    _handles.Add(_presets[i].SortBillboardsByDistance());
                }
            }
            catch (Exception ex)
            {
                Debug.Log("AdvancedRenderSystem will be disabled");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }
        }

        private void LateUpdate()
        {
            try
            {
                JobHandle.CompleteAll(_handles);
                _handles.Clear();

                for (int i = 0; i < _presets.Count; i++)
                {
                    _billboardsToUpdate.Clear();

                    CameraPreset preset = _presets[i];
                    Camera camera = preset.Camera;

                    NativeArray<BillboardData> distanceOrdered = preset.GetDistanceOrderedBillboardDatas();
                    NativeArray<BillboardData> timeOrdered = preset.GetTimeOrderedBillboardDatas();

                    FindBillboardsToUpdate(camera, distanceOrdered, i);
                    FindBillboardsToUpdate(camera, timeOrdered, i);

                    preset.ReplaceToMeshNearestObjects(distanceOrdered, _replaceDistance);
                    preset.UpdateBillboards(_renderCamera, _texturesManager, _billboardsToUpdate);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("AdvancedRenderSystem will be disabled");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }
        }

        private void FindBillboardsToUpdate(Camera camera, NativeArray<BillboardData> datas, int presetIdx)
        {
            int halfUpdates = Mathf.RoundToInt(_updatesPerFrame / 2f);
            int updates = Mathf.Min(UpdatesPerFrame, datas.Length);
            int updated = 0;

            for (int i = 0; i < updates; i++)
            {
                if (updated >= halfUpdates)
                    break;

                if (datas[i].lastUpdateTime < 0.05f)
                    continue;

                BaseBillboard billboard = _billboards[presetIdx][datas[i].billboardIndex];

                float angle = Vector3.Angle(-billboard.transform.forward, camera.transform.position - billboard.transform.position);

                if (angle > _updateAngle || billboard.Texture == null)
                {
                    _billboardsToUpdate.Add(new BillboardsUpdateData(billboard, datas[i].distance, datas[i].billboardIndex));

                    updated++;
                }
            }
        }


        #if !UNITY_2018

        private void OnBeginCameraRender(ScriptableRenderContext context, Camera camera)
        {
            OnCameraPreCull(camera);
        }

        private void OnEndCameraRender(ScriptableRenderContext context, Camera camera)
        {
            OnCameraPostRender(camera);
        }

        #endif

        private void OnCameraPreCull(Camera camera)
        {
            if (!enabled)
                return;

            if (camera.name == "SceneCamera" || camera.name == "Preview Camera")
            {
                foreach (var parent in _billboardsParents)
                    parent.SetActive(true);

                return;
            }

            int idx = _cameras.IndexOf(camera);
         
            if (idx < 0)
                return;

            _billboardsParents[idx].SetActive(true);
        }

        private void OnCameraPostRender(Camera camera)
        {
            if (!enabled)
                return;

            if (camera.name == "SceneCamera" || camera.name == "Preview Camera")
            {
                foreach (var parent in _billboardsParents)
                    parent.SetActive(false);

                return;
            }

            int idx = _cameras.IndexOf(camera);

            if (idx < 0)
                return;

            _billboardsParents[idx].SetActive(false);
        }

        private void OnDestroy()
        {
            #if UNITY_2018

            Camera.onPreCull -= OnCameraPreCull;
            Camera.onPostRender -= OnCameraPostRender;

            #else

            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRender;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRender;

            #endif

            foreach (var preset in _presets)
                preset.Dispose();

            _handles.Dispose();
        }


        public void UpdateAssignedData()
        {
            _cameras = _cameras.Distinct().Where(c => c != null).ToList();

            int idx = 0;
            while (idx < _renderersGroups.Count)
            {
                _renderersGroups[idx].renderers = _renderersGroups[idx].renderers
                    .Where(r => IsValidRenderer(r))
                    .ToList();

                if (_renderersGroups[idx].renderers.Count == 0)
                {
                    _renderersGroups.RemoveAt(idx);
                    continue;
                }

                if (_renderersGroups[idx].renderers.Count == 1)
                {
                    _renderers.Add(_renderersGroups[idx].renderers[0]);
                    _renderersGroups.RemoveAt(idx);
                    continue;
                }

                idx++;
            }

            _renderers = _renderers.Distinct().Where(r => IsValidRenderer(r)).ToList();
        }

        private bool IsRendererAlreadyAssigned(MeshRenderer renderer)
        {
            if (_renderers.Contains(renderer))
                return true;

            for (int i = 0; i < _renderersGroups.Count; i++)
            {
                if (_renderersGroups[i].Contains(renderer))
                    return true;
            }

            return false;
        }

        private bool IsValidRenderer(MeshRenderer renderer, bool printInfo = false)
        {
            if (renderer == null)
            {
                if(printInfo)
                    Debug.Log("AdvancedRenderSystem::You trying to add a null object");

                return false;
            }

            if (renderer.GetComponent<MeshFilter>() == null)
            {
                if (printInfo)
                    Debug.Log("AdvancedRenderSystem::Can't add object " + renderer.gameObject.name + ". MeshFilter not found");

                return false;
            }

            if (Application.isEditor && !renderer.gameObject.isStatic)
            {
                if (printInfo)
                    Debug.Log("AdvancedRenderSystem::Can't add object " + renderer.gameObject.name + ". Not marked as static");

                return false;
            }

            return true;
        }


        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            for (int i = 0; i < _billboards.Count; i++)
            {
                for (int c = 0; c < _billboards[i].Count; c++)
                    _billboards[i][c].ToMesh();
            }

            enabled = false;
        }


#region EditorOnly

#if UNITY_EDITOR

        public List<MeshRenderer> EditorGetRenderers()
        {
            return _renderers;
        }

        public void EditorAddAllStaticRenderers()
        {
            int count = _renderers.Count;

            foreach (var renderer in FindObjectsOfType<MeshRenderer>())
            {
                if (IsValidRenderer(renderer) && !IsRendererAlreadyAssigned(renderer))
                    _renderers.Add(renderer);
            }

            UpdateAssignedData();

            Debug.Log("Assigned " + (_renderers.Count - count) + " new objects");
        }

        public void EditorAddSelectedRenderers()
        {
            int count = _renderers.Count;

            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                {
                    if (IsValidRenderer(renderer, true) && !IsRendererAlreadyAssigned(renderer))
                        _renderers.Add(renderer);
                }
            }

            UpdateAssignedData();

            Debug.Log("Assigned " + (_renderers.Count - count) + " new objects");
        }


        public List<BillboardOfGroupList> EditorGetGroupedRenderers()
        {
            return _renderersGroups;
        }

        public void EditorAddSelectedAsGroup()
        {
            List<MeshRenderer> renderersGroup = new List<MeshRenderer>();

            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                {
                    if (_renderers.Contains(renderer))
                        continue;

                    bool skipRenderer = false;

                    for(int i = 0; i < _renderersGroups.Count; i++)
                    {
                        if (_renderersGroups[i].Contains(renderer))
                        {
                            skipRenderer = true;
                            break;
                        }
                    }

                    if (skipRenderer)
                        continue;

                    if (IsValidRenderer(renderer, true) && !IsRendererAlreadyAssigned(renderer))
                        renderersGroup.Add(renderer);
                }
            }

            if (renderersGroup.Count == 0)
            {
                Debug.Log("Renderers not added");
            }
            else if (renderersGroup.Count == 1)
            {
                _renderers.Add(renderersGroup[0]);
            }
            else
            {
                _renderersGroups.Add(new BillboardOfGroupList(renderersGroup));
            }

            UpdateAssignedData();
        }


        public void EditorRemoveSelectedRenderers()
        {
            int removedObjects = 0;

            foreach (var selected in Selection.gameObjects)
            {
                foreach (var renderer in selected.GetComponentsInChildren<MeshRenderer>())
                {
                    if (_renderers.Contains(renderer))
                    {
                        _renderers.Remove(renderer);

                        removedObjects++;

                        continue;
                    }

                    for (int i = 0; i < _renderersGroups.Count; i++)
                    {
                        if (_renderersGroups[i].Contains(renderer))
                        {
                            _renderersGroups[i].renderers.Remove(renderer);

                            removedObjects++;

                            break;
                        }
                    }
                }
            }

            Debug.Log("Removed " + removedObjects + " objects");

            UpdateAssignedData();
        }

        public void EditorRemoveAllRenderers()
        {
            _renderers.Clear();
            _renderersGroups.Clear();

            Debug.Log("All Renderers Removed From List");
        }
#endif
        
#endregion
    }


    //摄像机预置
    public class CameraPreset
    {
        public Camera Camera { get; private set; }

        private Transform _cameraTransform;
        private List<BaseBillboard> _billboards;

        private NativeArray<BillboardData> _billboardDatas;
        private NativeArray<BillboardData> _distanceOrderedBillboardDatas;
        private NativeArray<BillboardData> _timeOrderedBillboardDatas;

        public CameraPreset(Camera camera, List<BaseBillboard> billboards)
        {
            Camera = camera;

            _cameraTransform = camera.transform;
            _billboards = billboards;

            _billboardDatas = new NativeArray<BillboardData>(billboards.Count, Allocator.Persistent);

            for (int i = 0; i < billboards.Count; i++)
            {
                _billboardDatas[i] = new BillboardData(i, billboards[i].SourcesBounds.center)
                {
                    lastUpdateTime = 5f
                };
            }

            _distanceOrderedBillboardDatas = new NativeArray<BillboardData>(billboards.Count, Allocator.Persistent);
            _timeOrderedBillboardDatas = new NativeArray<BillboardData>(billboards.Count, Allocator.Persistent);
        }

        public JobHandle UpdateBillboardsData(float deltaTime)
        {
            return new UpdateBillboardDataJob()
            {
                billboardDatas = _billboardDatas,
                cameraPosition = _cameraTransform.position,
                deltaTime = deltaTime,

            }.Schedule(_billboardDatas.Length, 64);
        }

        public JobHandle SortBillboardsByDistance()
        {
            return new QuickSortJob<BillboardData, BillboardDataDistanceComarator>()
            {

                sourceArray = _billboardDatas,
                sortedArray = _distanceOrderedBillboardDatas,
                comparer = new BillboardDataDistanceComarator(),

            }.Schedule();
        }

        public JobHandle SortBillboardsByTime()
        {
            return new QuickSortJob<BillboardData, BillboardDataTimeComarator>()
            {

                sourceArray = _billboardDatas,
                sortedArray = _timeOrderedBillboardDatas,
                comparer = new BillboardDataTimeComarator(),

            }.Schedule();
        }

        public void ReplaceToMeshNearestObjects(NativeArray<BillboardData> distanceOrderedBillboards, float replaceDistance)
        {
            int i = 0;
            for (; i < distanceOrderedBillboards.Length; i++)
            {
                BillboardData data = distanceOrderedBillboards[i];

                if (data.distance > replaceDistance)
                    break;

                _billboards[data.billboardIndex].ToMesh();
            }

            for (; i < distanceOrderedBillboards.Length; i++)
            {
                BillboardData data = distanceOrderedBillboards[i];

                _billboards[data.billboardIndex].ToBillboard();
            }
        }

        public void UpdateBillboards(Camera renderCamera, TexturesManager texturesManager, List<BillboardsUpdateData> datas)
        {
            Camera camera = Camera;

            renderCamera.transform.position = camera.transform.position;
            renderCamera.transform.rotation = camera.transform.rotation;

            float fov = camera.fieldOfView;
            for (int i = 0; i < datas.Count; i++)
            {
                BaseBillboard billboard = datas[i].billboard;
                float distance = datas[i].distance;


                billboard.UpdateTexture(renderCamera, distance, fov, texturesManager);
                billboard.transform.rotation = Quaternion.LookRotation(billboard.SourcesBounds.center - camera.transform.position);
                //billboard.ToBillboard();


                BillboardData updatedData = _billboardDatas[datas[i].idx];
                updatedData.lastUpdateTime = 0;
                _billboardDatas[datas[i].idx] = updatedData;
            }
        }

        public void Dispose()
        {
            _billboardDatas.Dispose();
            _distanceOrderedBillboardDatas.Dispose();
            _timeOrderedBillboardDatas.Dispose();
        }


        public NativeArray<BillboardData> GetBillboardDatas()
        {
            return _billboardDatas;
        }

        public NativeArray<BillboardData> GetDistanceOrderedBillboardDatas()
        {
            return _distanceOrderedBillboardDatas;
        }

        public NativeArray<BillboardData> GetTimeOrderedBillboardDatas()
        {
            return _timeOrderedBillboardDatas;
        }
    }
}

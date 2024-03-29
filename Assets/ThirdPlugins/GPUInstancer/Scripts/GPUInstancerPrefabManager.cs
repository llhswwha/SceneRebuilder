﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer
{
    [ExecuteInEditMode]
    public class GPUInstancerPrefabManager : GPUInstancerManager
    {
        public static GPUInstancerPrefabManager Instance;

        [SerializeField]
        public RegisteredPrefabsDataList registeredPrefabs = new RegisteredPrefabsDataList();
        [SerializeField]
        public List<GameObject> prefabList;
        public bool enableMROnManagerDisable = true;
        public bool enableMROnRemoveInstance = true;

        protected List<GPUInstancerModificationCollider> _modificationColliders;
        protected GPUInstancerPrototypeDict _registeredPrefabsRuntimeData;
        protected List<IPrefabVariationData> _variationDataList;
        protected bool _addRemoveInProgress;

        public bool IsAddRemoveInProgress()
        {
            return _addRemoveInProgress;
        }

        #region MonoBehavior Methods

        public override void Awake()
        {
            base.Awake();

            Instance = this;

            if (prefabList == null)
                prefabList = new List<GameObject>();
        }

        public override void Reset()
        {
            base.Reset();

            RegisterPrefabsInScene();
        }

       

        public override void Update()
        {
            if (GetIsEnableUpdateEx()==false) return;
            base.Update();

            if (runtimeDataList != null && Application.isPlaying)
            {
                foreach (GPUInstancerRuntimeData runtimeData in runtimeDataList)
                {
                    if (runtimeData.prototype.autoUpdateTransformData
                        //#if UNITY_EDITOR
                        //                        || EditorApplication.isPaused
                        //#endif
                        )
                    {
                        //List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[runtimeData.prototype];
                        ////Transform instanceTransform;
                        ////foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
                        ////{
                        ////    instanceTransform = prefabInstance.GetInstanceTransform();
                        ////    if (instanceTransform.hasChanged && prefabInstance.state == PrefabInstancingState.Instanced)
                        ////    {
                        ////        instanceTransform.hasChanged = false;
                        ////        runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = instanceTransform.localToWorldMatrix;
                        ////        runtimeData.transformDataModified = true;
                        ////    }
                        ////}
                        //runtimeData.UpdateTransform(prefabInstanceList);

                        _registeredPrefabsRuntimeData.UpdateTransform(runtimeData);
                    }

                    if (runtimeData.transformDataModified)
                    {
                        runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataArray);
                        runtimeData.transformDataModified = false;
                    }
                }
            }
        }
        #endregion MonoBehavior Methods

        public override void ClearInstancingData()
        {
            base.ClearInstancingData();

            if (Application.isPlaying && _registeredPrefabsRuntimeData != null && enableMROnManagerDisable)
            {
//                foreach (GPUInstancerPrefabPrototype p in _registeredPrefabsRuntimeData.Keys)
//                {
//                    if (p.meshRenderersDisabled)
//                        continue;
//                    foreach (GPUInstancerPrefab prefabInstance in _registeredPrefabsRuntimeData[p])
//                    {
//                        if (!prefabInstance)
//                            continue;
//#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
//                        if (playModeState != PlayModeStateChange.EnteredEditMode && playModeState != PlayModeStateChange.ExitingPlayMode)
//#endif
//                        //SetRenderersEnabled(prefabInstance, true);
//                        prefabInstance.SetRenderersEnabled(true, layerMask);
//                    }
//                }
                bool isSetRenderersEnabled=true;
#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
                isSetRenderersEnabled= (playModeState != PlayModeStateChange.EnteredEditMode && playModeState != PlayModeStateChange.ExitingPlayMode);
#endif
                _registeredPrefabsRuntimeData.ClearInstancingData(true, layerMask, isSetRenderersEnabled);
            }

            if (_variationDataList != null)
            {
                foreach (IPrefabVariationData pvd in _variationDataList)
                    pvd.ReleaseBuffer();
            }
        }

        public override void GeneratePrototypes(bool forceNew = false)
        {
            base.GeneratePrototypes();

            GPUInstancerUtility.SetPrefabInstancePrototypes(gameObject, prototypeList, prefabList, forceNew);
        }
        [ContextMenu("NewPrototypes")]
        public void NewPrototypes()
        {
            base.GeneratePrototypes(true);
            GPUInstancerUtility.SetPrefabInstancePrototypes(gameObject, prototypeList, prefabList, true);
        }

#if UNITY_EDITOR
        public override void CheckPrototypeChanges()
        {
            base.CheckPrototypeChanges();

            if (prefabList == null)
                prefabList = new List<GameObject>();

            prefabList.RemoveAll(p => p == null);
            prefabList.RemoveAll(p => p.GetComponent<GPUInstancerPrefab>() == null);

            //Debug.LogError($"CheckPrototypeChanges1 prefabList:{prefabList.Count} prototypeList:{prototypeList.Count}");
            prototypeList.RemoveAll(p => p == null);
            //Debug.LogError($"CheckPrototypeChanges2 prefabList:{prefabList.Count} prototypeList:{prototypeList.Count}");
            prototypeList.RemoveAll(p => !prefabList.Contains(p.prefabObject));

            if (prefabList.Count != prototypeList.Count)
            {
                if (prefabList.Count > 0)
                {
                    Debug.Log($"CheckPrototypeChanges3 prefabList:{prefabList.Count} prefab0:{prefabList[0]} prototypeList:{prototypeList.Count}");
                }
                
                GeneratePrototypes();
            }

            //registeredPrefabs.RemoveAll(rpd => !prototypeList.Contains(rpd.prefabPrototype));
            //foreach (GPUInstancerPrefabPrototype prototype in prototypeList)
            //{
            //    if (!registeredPrefabs.Exists(rpd => rpd.prefabPrototype == prototype))
            //        registeredPrefabs.Add(new RegisteredPrefabsData(prototype));
            //}

            registeredPrefabs.RemovePrototypeList(prototypeList);
        }
#endif

        [ContextMenu("ClearPrefabList")]
        public void ClearPrefabList()
        {
            Debug.Log($"ClearPrefabList prefabList:{prefabList.Count} prototypeList:{prototypeList.Count}");
            prefabList.Clear();
            prototypeList.Clear();
        }

        public void ClearPrefabsAndPrototypes()
        {
            ClearPrefabList();
            NewPrototypes();
        }

        public override void InitializeRuntimeDataAndBuffers(bool forceNew = true)
        {
            base.InitializeRuntimeDataAndBuffers(forceNew);

            if (!forceNew && isInitialized)
                return;

            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new GPUInstancerPrototypeDict();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                //if (registeredPrefabs != null && registeredPrefabs.Count > 0)
                //{
                //    foreach (RegisteredPrefabsData rpd in registeredPrefabs)
                //    {
                //        if (!_registeredPrefabsRuntimeData.ContainsKey(rpd.prefabPrototype))
                //            _registeredPrefabsRuntimeData.Add(rpd.prefabPrototype, rpd.registeredPrefabs);
                //        else
                //        {
                //            _registeredPrefabsRuntimeData[rpd.prefabPrototype].AddRange(rpd.registeredPrefabs);
                //            _registeredPrefabsRuntimeData[rpd.prefabPrototype] = new List<GPUInstancerPrefab>(_registeredPrefabsRuntimeData[rpd.prefabPrototype].Distinct());
                //        }
                //    }
                //    registeredPrefabs.Clear();
                //}

                _registeredPrefabsRuntimeData.AddPrefabDatas(registeredPrefabs);

                //if (_registeredPrefabsRuntimeData.Count != prototypeList.Count)
                //{
                //    foreach (GPUInstancerPrototype p in prototypeList)
                //    {
                //        if (!_registeredPrefabsRuntimeData.ContainsKey(p))
                //            _registeredPrefabsRuntimeData.Add(p, new List<GPUInstancerPrefab>());
                //    }
                //}

                _registeredPrefabsRuntimeData.AddPrototypeList(prototypeList);
#if UNITY_EDITOR
            }
#endif

            InitializeRuntimeDataRegisteredPrefabs();
            GPUInstancerUtility.InitializeGPUBuffers(runtimeDataList);
            isInitial = true;
            isInitialized = true;
        }

        public override void DeletePrototype(GPUInstancerPrototype prototype, bool removeSO = true)
        {
            base.DeletePrototype(prototype, removeSO);

            prefabList.Remove(prototype.prefabObject);
            if (removeSO)
            {
#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
                GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefab>(prototype.prefabObject);
                GPUInstancerUtility.RemoveComponentFromPrefab<GPUInstancerPrefabRuntimeHandler>(prototype.prefabObject);
#else
                DestroyImmediate(prototype.prefabObject.GetComponent<GPUInstancerPrefab>(), true);
                if (prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>() != null)
                    DestroyImmediate(prototype.prefabObject.GetComponent<GPUInstancerPrefabRuntimeHandler>(), true);
#endif
#if UNITY_EDITOR
                EditorUtility.SetDirty(prototype.prefabObject);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prototype));
#endif
            }
            GeneratePrototypes(false);
        }

        public virtual void InitializeRuntimeDataRegisteredPrefabs(int additionalBufferSize = 0)
        {
            //if (runtimeDataList == null)
            //    runtimeDataList = new List<GPUInstancerRuntimeData>();
            //else
            //    GPUInstancerUtility.ClearInstanceData(runtimeDataList);
            //if (runtimeDataDictionary == null)
            //    runtimeDataDictionary = new Dictionary<GPUInstancerPrototype, GPUInstancerRuntimeData>();

            runtimeDataDictList.InitData2();

            foreach (GPUInstancerPrefabPrototype p in prototypeList)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && !p.isTransformsSerialized && !p.meshRenderersDisabled)
                    continue;
#endif
                InitializeRuntimeDataForPrefabPrototype(p, additionalBufferSize);
            }
        }

        public virtual GPUInstancerRuntimeData InitializeRuntimeDataForPrefabPrototype(GPUInstancerPrefabPrototype p, int additionalBufferSize = 0)
        {
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                p.useOriginalShaderForShadow = true;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(p);
            if (runtimeData == null)
            {
                //runtimeData = new GPUInstancerRuntimeData(p);
                //if (!runtimeData.CreateRenderersFromGameObject(p))
                //    return null;
                //runtimeDataList.Add(runtimeData);
                //runtimeDataDictionary.Add(p, runtimeData);

                runtimeData = runtimeDataDictList.AddPrototype(p);
                if (runtimeData == null) return null;

                if (p.isShadowCasting)
                {
                    runtimeData.hasShadowCasterBuffer = true;

                    if (!p.useOriginalShaderForShadow)
                    {
                        runtimeData.shadowCasterMaterial = new Material(Shader.Find(GPUInstancerConstants.SHADER_GPUI_SHADOWS_ONLY));
                    }
                }

                GPUInstancerUtility.AddBillboardToRuntimeData(runtimeData);

                if (p.treeType == GPUInstancerTreeType.SpeedTree || p.treeType == GPUInstancerTreeType.SpeedTree8 || p.treeType == GPUInstancerTreeType.TreeCreatorTree)
                    AddTreeProxy(p, runtimeData);
            }

            int instanceCount = 0;
            List<GPUInstancerPrefab> registeredPrefabsList = null;
            if (p.isTransformsSerialized)
            {
                string matrixStr;
                System.IO.StringReader strReader = new System.IO.StringReader(p.serializedTransformData.text);
                List<Matrix4x4> matrixData = new List<Matrix4x4>();
                while (true)
                {
                    matrixStr = strReader.ReadLine();
                    if (!string.IsNullOrEmpty(matrixStr))
                    {
                        matrixData.Add(GPUInstancerUtility.Matrix4x4FromString(matrixStr));
                    }
                    else
                        break;
                }
                runtimeData.instanceDataArray = matrixData.ToArray();
                runtimeData.bufferSize = runtimeData.instanceDataArray.Length + (p.enableRuntimeModifications && p.addRemoveInstancesAtRuntime ? p.extraBufferSize : 0) + additionalBufferSize;
                instanceCount = runtimeData.instanceDataArray.Length;
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying && p.meshRenderersDisabled)
            {
                List<GPUInstancerPrefab> prefabInstances = registeredPrefabs.GetInstances(p);

                runtimeData.instanceDataArray = new Matrix4x4[prefabInstances.Count];
                runtimeData.bufferSize = prefabInstances.Count;
                instanceCount = prefabInstances.Count;
                for (int i = 0; i < runtimeData.instanceDataArray.Length; i++)
                {
                    runtimeData.instanceDataArray[i] = prefabInstances[i].transform.localToWorldMatrix;
                }
            }
#endif
            else
            {
                if (_registeredPrefabsRuntimeData.TryGetValue(p, out registeredPrefabsList))
                {
                    runtimeData.instanceDataArray = new Matrix4x4[registeredPrefabsList.Count + (p.enableRuntimeModifications && p.addRemoveInstancesAtRuntime ? p.extraBufferSize : 0) + additionalBufferSize];
                    runtimeData.bufferSize = runtimeData.instanceDataArray.Length;

                    Matrix4x4 instanceData;
                    foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsList)
                    {
                        if (!prefabInstance)
                            continue;

                        instanceData = prefabInstance.GetInstanceTransform().localToWorldMatrix;
                        prefabInstance.GetInstanceTransform().hasChanged = false;
                        prefabInstance.state = PrefabInstancingState.Instanced;

                        bool disableRenderers = true;

                        if (prefabInstance.prefabPrototype.enableRuntimeModifications)
                        {
                            if (_modificationColliders != null && _modificationColliders.Count > 0)
                            {
                                bool isInsideCollider = false;
                                foreach (GPUInstancerModificationCollider mc in _modificationColliders)
                                {
                                    if (mc.IsInsideCollider(prefabInstance))
                                    {
                                        isInsideCollider = true;
                                        mc.AddEnteredInstance(prefabInstance);
                                        instanceData = GPUInstancerConstants.zeroMatrix;
                                        prefabInstance.state = PrefabInstancingState.Disabled;
                                        disableRenderers = false;
                                        break;
                                    }
                                }
                                if (!isInsideCollider)
                                {
                                    if (prefabInstance.prefabPrototype.startWithRigidBody && prefabInstance.GetComponent<Rigidbody>() != null)
                                    {
                                        isInsideCollider = true;
                                        _modificationColliders[0].AddEnteredInstance(prefabInstance);
                                        instanceData = GPUInstancerConstants.zeroMatrix;
                                        prefabInstance.state = PrefabInstancingState.Disabled;
                                        disableRenderers = false;
                                    }
                                }
                            }
                        }

                        if (disableRenderers && !prefabInstance.prefabPrototype.meshRenderersDisabled)
                            SetRenderersEnabled(prefabInstance, false);

                        runtimeData.instanceDataArray[instanceCount] = instanceData;
                        instanceCount++;
                        prefabInstance.gpuInstancerID = instanceCount;
                    }
                }
            }

            // set instanceCount
            runtimeData.instanceCount = instanceCount;

            // variations
            if (_variationDataList != null)
            {
                foreach (IPrefabVariationData pvd in _variationDataList)
                {
                    if (pvd.GetPrototype() == p)
                    {
                        pvd.InitializeBufferAndArray(runtimeData.bufferSize);
                        if (registeredPrefabsList != null)
                        {
                            foreach (GPUInstancerPrefab prefabInstance in registeredPrefabsList)
                            {
                                pvd.SetInstanceData(prefabInstance);
                            }
                        }
                        pvd.SetBufferData(0, 0, runtimeData.bufferSize);

                        for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                        {
                            for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                            {
                                pvd.SetVariation(runtimeData.instanceLODs[i].renderers[j].mpb);
                            }
                        }
                    }
                }
            }
            return runtimeData;
        }

        public virtual void SetRenderersEnabled(GPUInstancerPrefab prefabInstance, bool enabled, bool isThreading=false)
        {
            if (!prefabInstance) return;
            if (isThreading) return;
            prefabInstance.SetRenderersEnabled(enabled, layerMask);//将Render，LODGroup等去掉。

            //if (!prefabInstance || !prefabInstance.prefabPrototype || !prefabInstance.prefabPrototype.prefabObject)
            //    return;

            //MeshRenderer[] meshRenderers = prefabInstance.GetComponentsInChildren<MeshRenderer>(true);
            //if (meshRenderers != null && meshRenderers.Length > 0)
            //    for (int mr = 0; mr < meshRenderers.Length; mr++)
            //        if (GPUInstancerUtility.IsInLayer(layerMask, meshRenderers[mr].gameObject.layer))
            //            meshRenderers[mr].enabled = enabled;

            //BillboardRenderer[] billboardRenderers = prefabInstance.GetComponentsInChildren<BillboardRenderer>(true);
            //if (billboardRenderers != null && billboardRenderers.Length > 0)
            //    for (int mr = 0; mr < billboardRenderers.Length; mr++)
            //        if (GPUInstancerUtility.IsInLayer(layerMask, billboardRenderers[mr].gameObject.layer))
            //            billboardRenderers[mr].enabled = enabled;

            //LODGroup lodGroup = prefabInstance.GetComponent<LODGroup>();
            //if (lodGroup != null)
            //    lodGroup.enabled = enabled;

            //Rigidbody rigidbody = prefabInstance.GetComponent<Rigidbody>();

            //if (enabled)
            //{
            //    if (rigidbody == null)
            //    {
            //        rigidbody = prefabInstance.CreateRigidbody();
            //        //GPUInstancerPrefabPrototype.RigidbodyData rigidbodyData = prefabInstance.prefabPrototype.rigidbodyData;
            //        //if (rigidbodyData != null && prefabInstance.prefabPrototype.hasRigidBody)
            //        //{
            //        //    rigidbody = prefabInstance.gameObject.AddComponent<Rigidbody>();
            //        //    rigidbody.useGravity = rigidbodyData.useGravity;
            //        //    rigidbody.angularDrag = rigidbodyData.angularDrag;
            //        //    rigidbody.mass = rigidbodyData.mass;
            //        //    rigidbody.constraints = rigidbodyData.constraints;
            //        //    rigidbody.detectCollisions = true;
            //        //    rigidbody.drag = rigidbodyData.drag;
            //        //    rigidbody.isKinematic = rigidbodyData.isKinematic;
            //        //    rigidbody.interpolation = rigidbodyData.interpolation;
            //        //}
            //    }
            //}
            //else if (rigidbody != null && !prefabInstance.prefabPrototype.autoUpdateTransformData)
            //    Destroy(rigidbody);
        }

        #region API Methods

        public virtual void DisableIntancingForInstance(GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.instanceDataArray.Length)
            {
                prefabInstance.state = PrefabInstancingState.Disabled;
                runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = GPUInstancerConstants.zeroMatrix;

                runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (setRenderersEnabled)
                    SetRenderersEnabled(prefabInstance, true);
            }
            else
            {
                Debug.LogWarning("Can not disable instancing for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void EnableInstancingForInstance(GPUInstancerPrefab prefabInstance, bool setRenderersDisabled = true)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.instanceDataArray.Length)
            {
                prefabInstance.state = PrefabInstancingState.Instanced;
                runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = prefabInstance.GetInstanceTransform().localToWorldMatrix;

                runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (setRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);
            }
            else
            {
                Debug.LogWarning("Can not enable instancing for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void UpdateTransformDataForInstance(GPUInstancerPrefab prefabInstance)
        {
            if (!prefabInstance)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null && prefabInstance.gpuInstancerID > 0 && prefabInstance.gpuInstancerID <= runtimeData.instanceDataArray.Length)
            {
                runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = prefabInstance.GetInstanceTransform().localToWorldMatrix;

                // automatically set in Update method
                runtimeData.transformDataModified = true;
                //runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
            }
            else
            {
                Debug.LogWarning("Can not update transform for instance with id: " + prefabInstance.gpuInstancerID);
            }
        }

        public virtual void AddPrefabInstance(GPUInstancerPrefab prefabInstance, bool automaticallyIncreaseBufferSize = false)
        {
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerPrefabManager.AddPrefabInstance");
//#endif
            if (!prefabInstance || prefabInstance.state == PrefabInstancingState.Instanced)
                return;

            if (runtimeDataList == null)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype, true);
            if (runtimeData != null)
            {
                if (runtimeData.instanceDataArray.Length == runtimeData.instanceCount)
                {
                    if (automaticallyIncreaseBufferSize)
                    {
                        runtimeData.bufferSize += 1024;
                        Matrix4x4[] oldInstanceDataArray = runtimeData.instanceDataArray;
                        runtimeData.instanceDataArray = new Matrix4x4[runtimeData.bufferSize];
                        Array.Copy(oldInstanceDataArray, runtimeData.instanceDataArray, oldInstanceDataArray.Length);
                        runtimeData.instanceDataArray[runtimeData.instanceCount] = prefabInstance.GetInstanceTransform().localToWorldMatrix;
                        runtimeData.instanceCount++;
                        prefabInstance.gpuInstancerID = runtimeData.instanceCount;
                        _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);
                        if (!prefabInstance.prefabPrototype.meshRenderersDisabled)
                            SetRenderersEnabled(prefabInstance, false);
                        prefabInstance.GetInstanceTransform().hasChanged = false;
                        prefabInstance.state = PrefabInstancingState.Instanced;
                        GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                        prefabInstance.SetupPrefabInstance(runtimeData, true);

                        // variations
                        if (_variationDataList != null)
                        {
                            foreach (IPrefabVariationData pvd in _variationDataList)
                            {
                                if (pvd.GetPrototype() == prefabInstance.prefabPrototype)
                                {
                                    pvd.SetNewBufferSize(runtimeData.bufferSize);
                                    pvd.SetInstanceData(prefabInstance);
                                    pvd.SetBufferData(prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);

                                    for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                                    {
                                        for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                                        {
                                            pvd.SetVariation(runtimeData.instanceLODs[i].renderers[j].mpb);
                                        }
                                    }
                                }
                            }
                        }

                        return;
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogWarning("Can not add instance. Buffer is full.");
                        return;
#endif
                    }
                }
                prefabInstance.state = PrefabInstancingState.Instanced;
                runtimeData.instanceDataArray[runtimeData.instanceCount] = prefabInstance.GetInstanceTransform().localToWorldMatrix;
                runtimeData.instanceCount++;
                prefabInstance.gpuInstancerID = runtimeData.instanceCount;

                runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                if (!prefabInstance.prefabPrototype.meshRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);

                //if (!_registeredPrefabsRuntimeData.ContainsKey(prefabInstance.prefabPrototype))
                //    _registeredPrefabsRuntimeData.Add(prefabInstance.prefabPrototype, new List<GPUInstancerPrefab>());
                //_registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);

                _registeredPrefabsRuntimeData.AddInstance(prefabInstance);

                prefabInstance.GetInstanceTransform().hasChanged = false;

                // variations
                if (_variationDataList != null)
                {
                    foreach (IPrefabVariationData pvd in _variationDataList)
                    {
                        if (pvd.GetPrototype() == prefabInstance.prefabPrototype)
                        {
                            pvd.SetInstanceData(prefabInstance);
                            pvd.SetBufferData(prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                        }
                    }
                }

                prefabInstance.SetupPrefabInstance(runtimeData, true);
            }
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.EndSample();
//#endif
        }

        /// <summary>
        /// Adds prefab instances for multiple prototypes
        /// </summary>
        public virtual void AddPrefabInstances(IEnumerable<GPUInstancerPrefab> prefabInstances, bool isThreading = false)
        {
            while (isThreading && _addRemoveInProgress)
                Thread.Sleep(100);
            _addRemoveInProgress = true;
            List<GPUInstancerPrefab>[] instanceLists = new List<GPUInstancerPrefab>[prototypeList.Count];
            Dictionary<GPUInstancerPrototype, int> indexDict = new Dictionary<GPUInstancerPrototype, int>();
            for (int i = 0; i < instanceLists.Length; i++)
            {
                instanceLists[i] = new List<GPUInstancerPrefab>();
                indexDict.Add(prototypeList[i], i);
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstances)
            {
                instanceLists[indexDict[prefabInstance.prefabPrototype]].Add(prefabInstance);
            }

            for (int i = 0; i < instanceLists.Length; i++)
            {
                AddPrefabInstances((GPUInstancerPrefabPrototype)prototypeList[i], instanceLists[i], isThreading);
            }
            if (isThreading)
                threadQueue.Enqueue(() => _addRemoveInProgress = false);
            else
                _addRemoveInProgress = false;
        }

        public Dictionary<GPUInstancerPrefab, bool> isAddedDict = new Dictionary<GPUInstancerPrefab, bool>();

        public List<GPUInstancerPrefab> GetPrefabInstances(IEnumerable<GPUInstancerPrefab> prefabInstances,bool defaultValue,string tag)
        {
            int count1 = 0;
            
            List<GPUInstancerPrefab> prefabs2 = new List<GPUInstancerPrefab>();
            foreach (GPUInstancerPrefab pi in prefabInstances)
            {
                count1++;
                if (isAddedDict.ContainsKey(pi))
                {
                    bool isAdd = isAddedDict[pi];
                    if (isAdd == defaultValue)
                    {

                    }
                    else
                    {
                        prefabs2.Add(pi);
                        isAddedDict[pi] = defaultValue;
                    }
                }
                else
                {
                    prefabs2.Add(pi);
                    isAddedDict.Add(pi, defaultValue);
                }
            }

            //Debug.Log($"GetPrefabInstances({tag}) count1:{count1} prefabs2:{prefabs2.Count} defaultValue:{defaultValue} isAddedDict:{isAddedDict.Count}");
            return prefabs2;
        }

        /// <summary>
        /// Adds prefab instances for single prototye
        /// </summary>
        public virtual void AddPrefabInstances(GPUInstancerPrefabPrototype prototype, IEnumerable<GPUInstancerPrefab> prefabInstances, bool isThreading = false)
        {
            if (prefabInstances == null || prefabInstances.Count() == 0)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype, true);
            if (runtimeData == null)
                return;

            prefabInstances = GetPrefabInstances(prefabInstances,true, "AddPrefabInstances");

            int count = prefabInstances.Count();

            GPUInstancerPrefab prefabInstance;
            if (runtimeData.instanceCount + count > runtimeData.bufferSize)
            {
                runtimeData.bufferSize = runtimeData.instanceCount + count;
                Matrix4x4[] oldInstanceDataArray = runtimeData.instanceDataArray;
                runtimeData.instanceDataArray = new Matrix4x4[runtimeData.bufferSize];
                Array.Copy(oldInstanceDataArray, runtimeData.instanceDataArray, oldInstanceDataArray.Length);

                for (int i = 0; i < count; i++)
                {
                    prefabInstance = prefabInstances.ElementAt(i);
                    runtimeData.instanceDataArray[runtimeData.instanceCount + i] = prefabInstance.GetLocalToWorldMatrix();
                    prefabInstance.gpuInstancerID = runtimeData.instanceCount + i + 1;
                    if (!prototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, false, isThreading);
                    prefabInstance.state = PrefabInstancingState.Instanced;
                }
                _registeredPrefabsRuntimeData[prototype].AddRange(prefabInstances);
                runtimeData.instanceCount = runtimeData.bufferSize;

                if (isThreading)
                    threadQueue.Enqueue(() => GPUInstancerUtility.InitializeGPUBuffer(runtimeData));
                else
                    GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                prefabInstance = prefabInstances.ElementAt(i);
                runtimeData.instanceDataArray[runtimeData.instanceCount + i] = prefabInstance.GetLocalToWorldMatrix();
                prefabInstance.gpuInstancerID = runtimeData.instanceCount + i + 1;
                if (!prototype.meshRenderersDisabled)
                    SetRenderersEnabled(prefabInstance, false);
                prefabInstance.state = PrefabInstancingState.Instanced;
            }
            _registeredPrefabsRuntimeData[prototype].AddRange(prefabInstances);
            if (isThreading)
                threadQueue.Enqueue(() => runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataArray));
            else
                runtimeData.transformationMatrixVisibilityBuffer.SetData(runtimeData.instanceDataArray);
            runtimeData.instanceCount += count;
        }

        public virtual void UpdateInstanceDataArray(GPUInstancerRuntimeData runtimeData, List<GPUInstancerPrefab> prefabList, bool isThreading = false)
        {
            if (runtimeData.instanceDataArray.Length != prefabList.Count)
                runtimeData.instanceDataArray = new Matrix4x4[prefabList.Count];

            for (int i = 0; i < prefabList.Count;)
            {
                runtimeData.instanceDataArray[i] = prefabList[i].GetLocalToWorldMatrix();
                prefabList[i].gpuInstancerID = ++i;
            }
            int instanceCount = prefabList.Count;
            int bufferSize = instanceCount + ((GPUInstancerPrefabPrototype)runtimeData.prototype).extraBufferSize;
            if (isThreading)
                threadQueue.Enqueue(() =>
                {
                    runtimeData.instanceCount = instanceCount;
                    runtimeData.bufferSize = bufferSize;
                    GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
                });
            else
            {
                runtimeData.instanceCount = instanceCount;
                runtimeData.bufferSize = bufferSize;
                GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
            }
            return;
        }

        public virtual void RemovePrefabInstance(GPUInstancerPrefab prefabInstance, bool setRenderersEnabled = true)
        {
//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.BeginSample("GPUInstancerPrefabManager.RemovePrefabInstance");
//#endif
            if (!prefabInstance || prefabInstance.state == PrefabInstancingState.None)
                return;

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabInstance.prefabPrototype);
            if (runtimeData != null)
            {
                if (prefabInstance.gpuInstancerID > runtimeData.instanceDataArray.Length)
                {
                    Debug.LogWarning("Instance can not be removed.");
                    return;
                }

                List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype];

                if (prefabInstance.gpuInstancerID == runtimeData.instanceCount)
                {
                    prefabInstance.state = PrefabInstancingState.None;
                    runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = GPUInstancerConstants.zeroMatrix;
                    runtimeData.instanceCount--;
                    prefabInstanceList.RemoveAt(prefabInstance.gpuInstancerID - 1);
                    if (setRenderersEnabled && enableMROnRemoveInstance && !prefabInstance.prefabPrototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, true);
                }
                else
                {
                    GPUInstancerPrefab lastIndexPrefabInstance = null;
                    for (int i = prefabInstanceList.Count - 1; i >= 0; i--)
                    {
                        GPUInstancerPrefab loopPI = prefabInstanceList[i];
                        if (loopPI == null)
                        {
                            prefabInstanceList.RemoveAt(i);
                            if (i < prefabInstanceList.Count - 1)
                                i++;
                        }
                        else if (loopPI.gpuInstancerID == runtimeData.instanceCount)
                        {
                            lastIndexPrefabInstance = loopPI;
                            break;
                        }
                    }
                    if (!lastIndexPrefabInstance)
                    {
                        prefabInstanceList.RemoveAll(pi => pi == null);
                        Debug.LogWarning("Prefab instance was destoyed before being removed from instance list in GPUI Prefab Manager!");
                        return;
                    }

                    prefabInstance.state = PrefabInstancingState.None;

                    // exchange last index with this one
                    runtimeData.instanceDataArray[prefabInstance.gpuInstancerID - 1] = runtimeData.instanceDataArray[lastIndexPrefabInstance.gpuInstancerID - 1];
                    // set last index data to Matrix4x4.zero
                    runtimeData.instanceDataArray[lastIndexPrefabInstance.gpuInstancerID - 1] = GPUInstancerConstants.zeroMatrix;
                    runtimeData.instanceCount--;

                    runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, prefabInstance.gpuInstancerID - 1, prefabInstance.gpuInstancerID - 1, 1);
                    //runtimeData.transformationMatrixVisibilityBuffer.SetDataPartial(runtimeData.instanceDataArray, lastIndexPrefabInstance.gpuInstancerID - 1, lastIndexPrefabInstance.gpuInstancerID - 1, 1);

                    prefabInstanceList.RemoveAt(lastIndexPrefabInstance.gpuInstancerID - 1);
                    lastIndexPrefabInstance.gpuInstancerID = prefabInstance.gpuInstancerID;
                    prefabInstanceList[lastIndexPrefabInstance.gpuInstancerID - 1] = lastIndexPrefabInstance;

                    if (setRenderersEnabled && enableMROnRemoveInstance && !prefabInstance.prefabPrototype.meshRenderersDisabled)
                        SetRenderersEnabled(prefabInstance, true);
                    //Destroy(prefabInstance);

                    // variations
                    if (_variationDataList != null)
                    {
                        foreach (IPrefabVariationData pvd in _variationDataList)
                        {
                            if (pvd.GetPrototype() == lastIndexPrefabInstance.prefabPrototype)
                            {
                                pvd.SetInstanceData(lastIndexPrefabInstance);
                                pvd.SetBufferData(lastIndexPrefabInstance.gpuInstancerID - 1, lastIndexPrefabInstance.gpuInstancerID - 1, 1);
                            }
                        }
                    }

                    lastIndexPrefabInstance.SetupPrefabInstance(runtimeData);
                }
            }

//#if UNITY_EDITOR
//            UnityEngine.Profiling.Profiler.EndSample();
//#endif
        }

        /// <summary>
        /// Removes prefab instances for multiple prototypes
        /// </summary>
        public virtual void RemovePrefabInstances(IEnumerable<GPUInstancerPrefab> prefabInstances, bool isThreading = false)
        {
            DateTime startT = DateTime.Now;
            //Debug.LogError($"GPUInstancerPrefaManager.RemovePrefabInstances1 prefabInstances:{prefabInstances.Count()} isThreading:{isThreading} _addRemoveInProgress:{_addRemoveInProgress}");

            int count = 0;
            while (isThreading && _addRemoveInProgress)
            {
                Debug.LogError($"GPUInstancerPrefaManager.RemovePrefabInstances2 [while (isThreading && _addRemoveInProgress)][{count++}] prefabInstances:{prefabInstances.Count()} isThreading:{isThreading} _addRemoveInProgress:{_addRemoveInProgress}");
                Thread.Sleep(100);
            }
            _addRemoveInProgress = true;
            List<GPUInstancerPrefab>[] instanceLists = new List<GPUInstancerPrefab>[prototypeList.Count];
            Dictionary<GPUInstancerPrototype, int> indexDict = new Dictionary<GPUInstancerPrototype, int>();
            for (int i = 0; i < instanceLists.Length; i++)
            {
                instanceLists[i] = new List<GPUInstancerPrefab>();
                indexDict.Add(prototypeList[i], i);
            }

            int c = 0;
            foreach (GPUInstancerPrefab prefabInstance in prefabInstances)
            {
                c++;
                if (prefabInstance == null)
                {
                    Debug.LogError($"RemovePrefabInstances[{c}] prefabInstance == null");
                    continue;
                }
                if(prefabInstance.prefabPrototype==null)
                {
                    Debug.LogError($"RemovePrefabInstances[{c}] prefabInstance.prefabPrototype==null");
                    continue;
                }
                int index = indexDict[prefabInstance.prefabPrototype];
                instanceLists[index].Add(prefabInstance);
            }
            //Debug.LogError($"GPUInstancerPrefaManager.RemovePrefabInstances3 time:{DateTime.Now- startT} prefabInstances:{prefabInstances.Count()} isThreading:{isThreading} _addRemoveInProgress:{_addRemoveInProgress}");
            for (int i = 0; i < instanceLists.Length; i++)
            {
                if (instanceLists[i] == null || instanceLists[i].Count == 0) continue;
                RemovePrefabInstances((GPUInstancerPrefabPrototype)prototypeList[i], instanceLists[i],i+1, isThreading);
            }
            //Debug.LogError($"GPUInstancerPrefaManager.RemovePrefabInstances4 time:{DateTime.Now - startT} prefabInstances:{prefabInstances.Count()} isThreading:{isThreading} _addRemoveInProgress:{_addRemoveInProgress}");

            if (isThreading)
                threadQueue.Enqueue(() => _addRemoveInProgress = false);
            else
                _addRemoveInProgress = false;

            //Debug.LogError($"GPUInstancerPrefaManager.RemovePrefabInstances5 time:{DateTime.Now - startT} prefabInstances:{prefabInstances.Count()} isThreading:{isThreading} _addRemoveInProgress:{_addRemoveInProgress}");
        }

        /// <summary>
        /// Removes prefab instances for single prototye
        /// </summary>
        public virtual void RemovePrefabInstances(GPUInstancerPrefabPrototype prototype, IEnumerable<GPUInstancerPrefab> prefabInstances,int index, bool isThreading = false)
        {
            DateTime startT = DateTime.Now;
            if (prefabInstances == null)
            {
                Debug.LogWarning($"RemovePrefabInstances11[{index}] prefabInstances == null ");
                return;
            }
            if (prefabInstances.Count() == 0)
            {
                Debug.LogWarning($"RemovePrefabInstances12[{index}] prefabInstances.Count() == 0");
                return;
            }
            int count1 = prefabInstances.Count();

            //Debug.Log($"RemovePrefabInstances[{index}] 1[{DateTime.Now- startT}] ");

            prefabInstances = GetPrefabInstances(prefabInstances,false, $"RemovePrefabInstances[{index}]");

            if (prefabInstances == null || prefabInstances.Count() == 0)
            {
                Debug.LogError($"RemovePrefabInstances2[{index}] prefabInstances == null || prefabInstances.Count() == 0 origCount:{count1}");
                return;
            }


            int count = prefabInstances.Count();

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype, true);
            if (runtimeData == null)
            {
                Debug.LogError($"RemovePrefabInstances3[{index}] runtimeData == null count:{count}");
                return;
            }
            //Debug.Log($"RemovePrefabInstances[{index}] 2[{DateTime.Now - startT}] ");

            List<GPUInstancerPrefab> prefabInstanceList = _registeredPrefabsRuntimeData[prototype];
            Dictionary<GPUInstancerPrefab, GPUInstancerPrefab> dict = new Dictionary<GPUInstancerPrefab, GPUInstancerPrefab>();
            foreach(var item in prefabInstanceList)
            {
                dict.Add(item, item);
            }
            //Debug.Log($"RemovePrefabInstances[{index}] 21[{DateTime.Now - startT}]");

            int listCount = prefabInstanceList.Count;
            if (listCount == 0)
            {
                Debug.LogError($"RemovePrefabInstances4[{index}] listCount == 0 isThreading:{isThreading} count:{count} listCount:{listCount}");
                return;
            }
            GPUInstancerPrefab element0 = prefabInstances.ElementAt(0);
            int startIndex = element0.gpuInstancerID - 1;
            if (startIndex < 0)
            {
                Debug.LogError($"RemovePrefabInstances5[{index}] startIndex < 0 isThreading:{isThreading} startIndex:{startIndex} count:{count} listCount:{listCount}");
                return;
            }

            //Debug.Log($"RemovePrefabInstances[{index}] 3[{DateTime.Now - startT}] listCount:{listCount}");
            try
            {
                if(count== listCount && startIndex!=0)
                {
                    //Debug.LogWarning($"RemovePrefabInstances6[{index}] startIndex:{startIndex} count:{count} listCount:{listCount}");
                    prefabInstanceList.Clear();
                }
                else
                {
                    foreach (var prefab in prefabInstances)
                    {
                        //prefabInstanceList.Remove(prefab);//慢
                        dict.Remove(prefab);//快
                    }
                    //Debug.Log($"RemovePrefabInstances[{index}] 41[{DateTime.Now - startT}]");
                    prefabInstanceList = dict.Keys.ToList();
                    _registeredPrefabsRuntimeData[prototype] = prefabInstanceList;//不能忘了
                    //Debug.Log($"RemovePrefabInstances[{index}] 42[{DateTime.Now - startT}]");
                }
                //prefabInstanceList.RemoveRange(startIndex, count);//Original Code
                int count2 = prefabInstanceList.Count;

                //Debug.Log($"RemovePrefabInstances[{index}] 4[{DateTime.Now - startT}] listCount:{listCount} count:{count} > {count2}");

                //Debug.Log($"RemovePrefabInstances7[{index}] isThreading:{isThreading} startIndex:{startIndex} count:{count} listCount:{listCount} listCount2:{prefabInstanceList.Count} ");

                foreach (GPUInstancerPrefab pi in prefabInstances)
                {
                    if (enableMROnRemoveInstance && !prototype.meshRenderersDisabled)
                    {
                        SetRenderersEnabled(pi, true, isThreading);
                    }

                    pi.state = PrefabInstancingState.None;
                    pi.gpuInstancerID = 0;
                }
                //Debug.Log($"RemovePrefabInstances[{index}] 5[{DateTime.Now - startT}] listCount:{listCount}");
                UpdateInstanceDataArray(runtimeData, prefabInstanceList, isThreading);
                //Debug.Log($"RemovePrefabInstances[{index}] 6[{DateTime.Now - startT}] listCount:{listCount}");
            }
            catch (Exception ex)
            {
                if (isThreading)
                {
                    Debug.LogError($"RemovePrefabInstances[{index}]8 startIndex:{startIndex} count:{count} listCount:{listCount} Exception:{ex}");
                }
                else
                {
                    Debug.LogError($"RemovePrefabInstances[{index}]9 element0:{element0.name} startIndex:{startIndex} count:{count} listCount:{listCount} Exception:{ex}");
                }
            }
        }

        public virtual void RegisterPrefabsInScene()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Registered prefabs changed");
#endif
            //registeredPrefabs.Clear();
            //foreach (GPUInstancerPrefabPrototype pp in prototypeList)
            //    registeredPrefabs.Add(new RegisteredPrefabsData(pp));

            registeredPrefabs.AddPrototypeList(prototypeList);

            GPUInstancerPrefab[] scenePrefabInstances = FindObjectsOfType<GPUInstancerPrefab>();
            foreach (GPUInstancerPrefab prefabInstance in scenePrefabInstances)
                AddRegisteredPrefab(prefabInstance);
        }

        [ContextMenu("ShowTestInfo")]
        public void ShowTestInfo()
        {
            Debug.LogError($"RegisterPrefabInstanceList prefabList:{prefabList.Count()},prototypeList: {prototypeList.Count}");
        }

        public virtual void RegisterPrefabInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            //Debug.Log($"RegisterPrefabInstanceList prefabInstanceList:{prefabInstanceList.Count()}, prefabList:{prefabList.Count()}, prototypeList: {prototypeList.Count} ");

            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new GPUInstancerPrototypeDict(prototypeList);
            //if (_registeredPrefabsRuntimeData.Keys.Count != prototypeList.Count)
            //{
            //    foreach (GPUInstancerPrototype prototype in prototypeList)
            //        if (!_registeredPrefabsRuntimeData.ContainsKey(prototype))
            //            _registeredPrefabsRuntimeData.Add(prototype, new List<GPUInstancerPrefab>());
            //}

            _registeredPrefabsRuntimeData.RegisterInstanceList(prefabInstanceList);

            //foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            //{
            //    if (prefabInstance == null)
            //    {
            //        Debug.LogError("prefabInstance == null");
            //        continue;
            //    }
            //    if (prefabInstance.prefabPrototype == null)
            //    {
            //        Debug.LogError("prefabInstance.prefabPrototype == null");
            //        continue;
            //    }
            //    if(_registeredPrefabsRuntimeData.ContainsKey(prefabInstance.prefabPrototype)==false)
            //    {
            //        Debug.LogError("_registeredPrefabsRuntimeData.ContainsKey(prefabInstance.prefabPrototype)==false："+ prefabInstance.prefabPrototype);
            //    }
            //    else
            //    {
            //        _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Add(prefabInstance);
            //    }

            //}
        }

        public virtual void UnregisterPrefabInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            if (_registeredPrefabsRuntimeData == null)
                _registeredPrefabsRuntimeData = new GPUInstancerPrototypeDict(prototypeList);

            //if (_registeredPrefabsRuntimeData.Keys.Count != prototypeList.Count)
            //{
            //    foreach (GPUInstancerPrototype prototype in prototypeList)
            //        if (!_registeredPrefabsRuntimeData.ContainsKey(prototype))
            //            _registeredPrefabsRuntimeData.Add(prototype, new List<GPUInstancerPrefab>());
            //}

            //foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            //{
            //    _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype].Remove(prefabInstance);
            //}

            _registeredPrefabsRuntimeData.UnregisterInstanceList(prefabInstanceList);
        }

        public virtual void ClearRegisteredPrefabInstances()
        {
            //foreach (GPUInstancerPrototype p in _registeredPrefabsRuntimeData.Keys)
            //{
            //    _registeredPrefabsRuntimeData[p].Clear();
            //}

            _registeredPrefabsRuntimeData.ClearInstances();
        }

        public void ClearRegisteredPrefabInstances(GPUInstancerPrototype p)
        {
            //if (_registeredPrefabsRuntimeData.ContainsKey(p))
            //    _registeredPrefabsRuntimeData[p].Clear();
            _registeredPrefabsRuntimeData.ClearInstances(p);
        }

        public virtual PrefabVariationData<T> DefinePrototypeVariationBuffer<T>(GPUInstancerPrefabPrototype prototype, string bufferName)
        {
            if (_variationDataList == null)
                _variationDataList = new List<IPrefabVariationData>();
            if (GPUInstancerUtility.matrixHandlingType != GPUIMatrixHandlingType.Default)
            {
                Debug.LogError("GPUI can not define material variations in this platform and/or with this rendering settings.");
                return null;
            }
            PrefabVariationData<T> result = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    result = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (result == null)
            {
                result = new PrefabVariationData<T>(prototype, bufferName);
                _variationDataList.Add(result);

                if (isInitialized)
                {
                    GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
                    result.InitializeBufferAndArray(runtimeData.bufferSize);
                    if (_registeredPrefabsRuntimeData != null && _registeredPrefabsRuntimeData.ContainsKey(prototype))
                    {
                        foreach (GPUInstancerPrefab prefabInstance in _registeredPrefabsRuntimeData[prototype])
                        {
                            result.SetInstanceData(prefabInstance);
                        }
                    }
                    result.SetBufferData(0, 0, runtimeData.bufferSize);

                    for (int i = 0; i < runtimeData.instanceLODs.Count; i++)
                    {
                        for (int j = 0; j < runtimeData.instanceLODs[i].renderers.Count; j++)
                        {
                            result.SetVariation(runtimeData.instanceLODs[i].renderers[j].mpb);
                        }
                    }
                }
            }
            return result;
        }

        public virtual void UpdateVariationData<T>(GPUInstancerPrefab prefabInstance, string bufferName, T value)
        {
            if (!prefabInstance || !prefabInstance.prefabPrototype)
                return;
            PrefabVariationData<T> variationData = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prefabInstance.prefabPrototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    variationData = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (variationData != null && variationData.dataArray != null)
            {
                int index = prefabInstance.gpuInstancerID - 1;
                if (index >= 0 && index < variationData.dataArray.Length)
                {
                    variationData.dataArray[index] = value;
#if UNITY_2017_1_OR_NEWER
                    variationData.variationBuffer.SetData(variationData.dataArray, index, index, 1);
#else
                    variationData.variationBuffer.SetData(variationData.dataArray);
#endif
                }
            }
        }

        public virtual PrefabVariationData<T> DefineAndAddVariationFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray)
        {
            PrefabVariationData<T> variationData = DefinePrototypeVariationBuffer<T>(prototype, bufferName);
            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
            if (runtimeData != null && variationData != null)
            {
                variationData.SetArrayAndInitializeBuffer(variationArray);
                variationData.SetBufferData(0, 0, Math.Min(runtimeData.bufferSize, variationArray.Length));
                for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                {
                    for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                    {
                        variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r].mpb);
                    }
                }
            }

            return variationData;
        }

        public virtual PrefabVariationData<T> UpdateVariationsFromArray<T>(GPUInstancerPrefabPrototype prototype, string bufferName, T[] variationArray,
            int arrayStartIndex = 0, int bufferStartIndex = 0, int count = 0)
        {
            PrefabVariationData<T> variationData = null;
            foreach (IPrefabVariationData item in _variationDataList)
            {
                if (item.GetPrototype() == prototype && item.GetBufferName() == bufferName && item is PrefabVariationData<T>)
                {
                    variationData = (PrefabVariationData<T>)item;
                    break;
                }
            }
            if (variationData != null)
            {
                GPUInstancerRuntimeData runtimeData = GetRuntimeData(prototype);
                if (runtimeData != null)
                {
                    variationData.dataArray = variationArray;
                    if (count > 0)
                        variationData.SetBufferData(arrayStartIndex, bufferStartIndex, count);
                    else
                        variationData.SetBufferData(0, 0, runtimeData.bufferSize);
                    for (int l = 0; l < runtimeData.instanceLODs.Count; l++)
                    {
                        for (int r = 0; r < runtimeData.instanceLODs[l].renderers.Count; r++)
                        {
                            variationData.SetVariation(runtimeData.instanceLODs[l].renderers[r].mpb);
                        }
                    }
                }
            }

            return variationData;
        }

        public virtual GPUInstancerPrefabPrototype DefineGameObjectAsPrefabPrototypeAtRuntime(GameObject prototypeGameObject, bool attachScript = true)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("DefineGameObjectAsPrefabPrototypeAtRuntime method is designed to use at runtime. Prototype generation canceled.");
                return null;
            }

           return DefineGameObjectAsPrefabPrototype(prototypeGameObject,attachScript);
        }

        public GPUInstancerPrefabPrototype DefineGameObjectAsPrefabPrototype(GameObject prototypeGameObject, bool attachScript = true)
        {
            if (prefabList == null)
                prefabList = new List<GameObject>();
            GPUInstancerPrefabPrototype prefabPrototype = GPUInstancerUtility.GeneratePrefabPrototype(prototypeGameObject, false, attachScript);
            if (!prototypeList.Contains(prefabPrototype))
                prototypeList.Add(prefabPrototype);
            if (!prefabList.Contains(prototypeGameObject))
                prefabList.Add(prototypeGameObject);
            if (prefabPrototype.minCullingDistance < minCullingDistance)
                prefabPrototype.minCullingDistance = minCullingDistance;

            //Debug.LogError("AddPrefab:"+prefabPrototype);
            return prefabPrototype;
        }

        public void AddPrefabObject(GameObject obj)
        {
            if (!this.prefabList.Contains(obj))
            {
                this.prefabList.Add(obj);
                this.GeneratePrototypes();
            }
        }

        [ContextMenu("InitPrefabs")]
        public void InitPrefabs(List<GPUInstancerPrefab> list)
        {
            //if (isClear)
            //{
            //    ClearPrefabList();
            //}
            list.RemoveAll(i => i == null);
            for (int i = 0; i < list.Count; i++)
            {
                float progress = (float)i / list.Count;
                float percents = progress * 100;
#if UNITY_EDITOR
                if (EditorUtility.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{list.Count} {percents:F1}%", progress))
                {
                    break;
                }
#endif
                if (list[i] == null)
                {
                    continue;
                }

                GameObject item = list[i].gameObject;
                Debug.Log($"InitPrefabs {i + 1}/{list.Count} item:{item}");
                GPUInstancerPrefab prefab = list[i];

                if (prefab == null)
                {
#if UNITY_EDITOR
                    prefab = GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefab>(item);
#else
                    prefab = item.AddComponent<GPUInstancerPrefab>();
#endif
                }

                AddPrefabObject(item);
            }
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            Debug.Log($"InitPrefabs list:{list.Count}"); 
        }

        [ContextMenu("InitPrefabs")]
        public void InitPrefabs(List<GameObject> list)
        {

            ClearPrefabList();
            for (int i = 0; i < list.Count; i++)
            {
                float progress = (float)i / list.Count;
                float percents = progress * 100;
#if UNITY_EDITOR
                if (EditorUtility.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{list.Count} {percents:F1}%", progress))
                {
                    break;
                }
#endif

                GameObject item = list[i];
                Debug.Log($"InitPrefabs {i+1}/{list.Count} item:{item}");
                GPUInstancerPrefab prefab = item.GetComponent<GPUInstancerPrefab>();

                if (prefab == null)
                {
#if UNITY_EDITOR
                    prefab = GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefab>(item);
#else
                    prefab = item.AddComponent<GPUInstancerPrefab>();
#endif
                }

                AddPrefabObject(item);
            }
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            Debug.Log($"InitPrefabs list:{list.Count}");
        }

        public virtual void AddInstancesToPrefabPrototypeAtRuntime(GPUInstancerPrefabPrototype prefabPrototype, IEnumerable<GameObject> instances)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("AddInstancesToPrefabPrototypeAtRuntime method is designed to use at runtime. Adding instances canceled.");
                return;
            }

            if (isActiveAndEnabled)
            {
                //List<GPUInstancerPrefab> instanceList;
                //if (!_registeredPrefabsRuntimeData.TryGetValue(prefabPrototype, out instanceList))
                //{
                //    instanceList = new List<GPUInstancerPrefab>();
                //    _registeredPrefabsRuntimeData.Add(prefabPrototype, instanceList);
                //}

                List<GPUInstancerPrefab> instanceList = _registeredPrefabsRuntimeData.GetInstanceList(prefabPrototype);

                GPUInstancerPrefab prefabInstance;
                foreach (GameObject instance in instances)
                {
                    prefabInstance = instance.GetComponent<GPUInstancerPrefab>();
                    if (prefabInstance == null)
                    {
                        prefabInstance = instance.AddComponent<GPUInstancerPrefab>();
                        prefabInstance.prefabPrototype = prefabPrototype;
                    }
                    if (prefabInstance != null && !instanceList.Contains(prefabInstance))
                        instanceList.Add(prefabInstance);
                }

                GPUInstancerRuntimeData runtimeData = InitializeRuntimeDataForPrefabPrototype(prefabPrototype, 0);
                GPUInstancerUtility.ReleaseInstanceBuffers(runtimeData);
                GPUInstancerUtility.InitializeGPUBuffer(runtimeData);
            }
            else
            {
                if (registeredPrefabs == null)
                    registeredPrefabs = new RegisteredPrefabsDataList();

                //RegisteredPrefabsData data = null;
                //foreach (RegisteredPrefabsData item in registeredPrefabs)
                //{
                //    if (item.prefabPrototype == prefabPrototype)
                //    {
                //        data = item;
                //        break;
                //    }
                //}
                //if (data == null)
                //{
                //    data = new RegisteredPrefabsData(prefabPrototype);
                //    registeredPrefabs.Add(data);
                //}

                RegisteredPrefabsData data = registeredPrefabs.GetData(prefabPrototype);

                GPUInstancerPrefab prefabInstance;
                foreach (GameObject instance in instances)
                {
                    prefabInstance = instance.GetComponent<GPUInstancerPrefab>();
                    if (prefabInstance == null)
                    {
                        prefabInstance = instance.AddComponent<GPUInstancerPrefab>();
                        prefabInstance.prefabPrototype = prefabPrototype;
                    }
                    if (prefabInstance != null && !data.registeredPrefabs.Contains(prefabInstance))
                        data.registeredPrefabs.Add(prefabInstance);
                }
            }
        }

        public virtual void RemovePrototypeAtRuntime(GPUInstancerPrefabPrototype prefabPrototype)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("RemovePrototypeAtRuntime method is designed to use at runtime. Prototype removal canceled.");
                return;
            }

            GPUInstancerRuntimeData runtimeData = GetRuntimeData(prefabPrototype);
            //if (runtimeData != null)
            //{
            //    GPUInstancerUtility.ReleaseInstanceBuffers(runtimeData);
            //    if (runtimeDataList != null)
            //        runtimeDataList.Remove(runtimeData);
            //    if (runtimeDataDictionary != null)
            //        runtimeDataDictionary.Remove(runtimeData.prototype);
            //}
            runtimeDataDictList.RemovePrototype(runtimeData);

            if (isActiveAndEnabled)
            {
                _registeredPrefabsRuntimeData.Remove(prefabPrototype);
            }
            else if (registeredPrefabs != null)
            {
                //RegisteredPrefabsData data = null;
                //foreach (RegisteredPrefabsData item in registeredPrefabs)
                //{
                //    if (item.prefabPrototype == prefabPrototype)
                //    {
                //        data = item;
                //        break;
                //    }
                //}
                //if (data != null)
                //    registeredPrefabs.Remove(data);

                registeredPrefabs.RemovePrototype(prefabPrototype);
            }
            if (prototypeList.Contains(prefabPrototype))
                prototypeList.Remove(prefabPrototype);
            if (prefabPrototype.prefabObject && prefabList.Contains(prefabPrototype.prefabObject))
                prefabList.Remove(prefabPrototype.prefabObject);
        }
        #endregion API Methods

        public virtual void AddRegisteredPrefab(GPUInstancerPrefab prefabInstance)
        {
            //RegisteredPrefabsData data = null;
            //foreach (RegisteredPrefabsData item in registeredPrefabs)
            //{
            //    if (item.prefabPrototype == prefabInstance.prefabPrototype)
            //    {
            //        data = item;
            //        break;
            //    }
            //}
            //if (data != null)
            //    data.registeredPrefabs.Add(prefabInstance);

            registeredPrefabs.AddRegisteredPrefab(prefabInstance);
        }

        public virtual void AddRuntimeRegisteredPrefab(GPUInstancerPrefab prefabInstance)
        {
            //List<GPUInstancerPrefab> list;
            //if (_registeredPrefabsRuntimeData.ContainsKey(prefabInstance.prefabPrototype))
            //    list = _registeredPrefabsRuntimeData[prefabInstance.prefabPrototype];
            //else
            //{
            //    list = new List<GPUInstancerPrefab>();
            //    _registeredPrefabsRuntimeData.Add(prefabInstance.prefabPrototype, list);
            //}

            //if (!list.Contains(prefabInstance))
            //    list.Add(prefabInstance);

            _registeredPrefabsRuntimeData.AddInstance(prefabInstance);
        }

        public virtual void AddModificationCollider(GPUInstancerModificationCollider modificationCollider)
        {
            if (_modificationColliders == null)
                _modificationColliders = new List<GPUInstancerModificationCollider>();

            _modificationColliders.Add(modificationCollider);
        }

        public virtual int GetEnabledPrefabCount()
        {
            int sum = 0;
            if (_modificationColliders != null)
            {
                for (int i = 0; i < _modificationColliders.Count; i++)
                    sum += _modificationColliders[i].GetEnteredInstanceCount();
            }
            return sum;
        }

        public virtual Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>> GetRegisteredPrefabsRuntimeData()
        {
            return _registeredPrefabsRuntimeData;
        }
    }

    [Serializable]
    public class RegisteredPrefabsData
    {
        public GPUInstancerPrefabPrototype prefabPrototype;
        public List<GPUInstancerPrefab> registeredPrefabs;

        public RegisteredPrefabsData(GPUInstancerPrefabPrototype prefabPrototype)
        {
            this.prefabPrototype = prefabPrototype;
            registeredPrefabs = new List<GPUInstancerPrefab>();
        }
    }

    public class RegisteredPrefabsDataList : List<RegisteredPrefabsData>
    {
        public List<GPUInstancerPrefab> GetInstances(GPUInstancerPrefabPrototype p)
        {
            List<GPUInstancerPrefab> prefabInstances = this.Find(rpd => rpd.prefabPrototype == p).registeredPrefabs;
            return prefabInstances;
        }
        public void AddRegisteredPrefab(GPUInstancerPrefab prefabInstance)
        {
            RegisteredPrefabsData data = null;
            foreach (RegisteredPrefabsData item in this)
            {
                if (item.prefabPrototype == prefabInstance.prefabPrototype)
                {
                    data = item;
                    break;
                }
            }
            if (data != null)
                data.registeredPrefabs.Add(prefabInstance);
        }

        public RegisteredPrefabsData GetData(GPUInstancerPrefabPrototype prefabPrototype)
        {
            RegisteredPrefabsData data = null;
            foreach (RegisteredPrefabsData item in this)
            {
                if (item.prefabPrototype == prefabPrototype)
                {
                    data = item;
                    break;
                }
            }
            if (data == null)
            {
                data = new RegisteredPrefabsData(prefabPrototype);
                this.Add(data);
            }
            return data;
        }

        public void RemovePrototype(GPUInstancerPrefabPrototype prefabPrototype)
        {
            RegisteredPrefabsData data = null;
            foreach (RegisteredPrefabsData item in this)
            {
                if (item.prefabPrototype == prefabPrototype)
                {
                    data = item;
                    break;
                }
            }
            if (data != null)
                this.Remove(data);
        }

        public void RemovePrototypeList(List<GPUInstancerPrototype> prototypeList)
        {
            this.RemoveAll(rpd => !prototypeList.Contains(rpd.prefabPrototype));
            foreach (GPUInstancerPrefabPrototype prototype in prototypeList)
            {
                if (!this.Exists(rpd => rpd.prefabPrototype == prototype))
                    this.Add(new RegisteredPrefabsData(prototype));
            }
        }

        public void AddPrototypeList(List<GPUInstancerPrototype> prototypeList)
        {
            this.Clear();
            foreach (GPUInstancerPrefabPrototype pp in prototypeList)
                this.Add(new RegisteredPrefabsData(pp));
        }

        public void RemovePrototypeList(GPUInstancerPrototypeList prototypeList)
        {
            this.RemoveAll(rpd => !prototypeList.Contains(rpd.prefabPrototype));
            foreach (GPUInstancerPrefabPrototype prototype in prototypeList)
            {
                if (!this.Exists(rpd => rpd.prefabPrototype == prototype))
                    this.Add(new RegisteredPrefabsData(prototype));
            }
        }

        public void AddPrototypeList(GPUInstancerPrototypeList prototypeList)
        {
            this.Clear();
            foreach (GPUInstancerPrefabPrototype pp in prototypeList)
                this.Add(new RegisteredPrefabsData(pp));
        }
    }

    public interface IPrefabVariationData
    {
        void InitializeBufferAndArray(int count, bool setDefaults = true);
        void SetInstanceData(GPUInstancerPrefab prefabInstance);
        void SetBufferData(int managedBufferStartIndex, int computeBufferStartIndex, int count);
        void SetVariation(MaterialPropertyBlock mpb);
        void SetNewBufferSize(int newCount);
        GPUInstancerPrefabPrototype GetPrototype();
        string GetBufferName();
        void ReleaseBuffer();
    }

    public class PrefabVariationData<T> : IPrefabVariationData
    {
        public GPUInstancerPrefabPrototype prototype;
        public string bufferName;
        public ComputeBuffer variationBuffer;
        public T[] dataArray;
        public T defaultValue;

        public PrefabVariationData(GPUInstancerPrefabPrototype prototype, string bufferName, T defaultValue = default(T))
        {
            this.prototype = prototype;
            this.bufferName = bufferName;
            this.defaultValue = defaultValue;
        }

        public void InitializeBufferAndArray(int count, bool setDefaults = true)
        {
            if (count == 0)
                return;
            dataArray = new T[count];
            if (setDefaults)
            {
                for (int i = 0; i < count; i++)
                {
                    dataArray[i] = defaultValue;
                }
            }
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = new ComputeBuffer(count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
        }

        public void SetArrayAndInitializeBuffer(T[] dataArray)
        {
            if (dataArray == null || dataArray.Length == 0)
                return;
            this.dataArray = dataArray;
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = new ComputeBuffer(dataArray.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
        }

        public void SetInstanceData(GPUInstancerPrefab prefabInstance)
        {
            if (prefabInstance.variationDataList != null && dataArray != null && prefabInstance.variationDataList.ContainsKey(bufferName) && dataArray.Length > prefabInstance.gpuInstancerID - 1)
                dataArray[prefabInstance.gpuInstancerID - 1] = (T)prefabInstance.variationDataList[bufferName];
        }

        public void SetBufferData(int managedBufferStartIndex, int computeBufferStartIndex, int count)
        {
            if (variationBuffer != null && count > 0)
            {
#if UNITY_2017_1_OR_NEWER
                variationBuffer.SetData(dataArray, managedBufferStartIndex, computeBufferStartIndex, count);
#else
                variationBuffer.SetData(dataArray);
#endif
            }
        }

        public void SetVariation(MaterialPropertyBlock mpb)
        {
            if (variationBuffer != null)
                mpb.SetBuffer(bufferName, variationBuffer);
        }

        public void SetNewBufferSize(int newCount)
        {
            if (newCount <= 0)
                return;
            int count = 0;
            if (dataArray != null)
            {
                count = dataArray.Length;
                if (count < newCount)
                    Array.Resize<T>(ref dataArray, newCount);
            }
            else
                dataArray = new T[newCount];

            if (count < newCount)
            {
                if (variationBuffer != null)
                    variationBuffer.Release();
                variationBuffer = new ComputeBuffer(newCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
                Array.Resize<T>(ref dataArray, newCount);
                for (int i = count; i < newCount; i++)
                {
                    dataArray[i] = defaultValue;
                }
                variationBuffer.SetData(dataArray);
            }
        }

        public GPUInstancerPrefabPrototype GetPrototype()
        {
            return prototype;
        }

        public string GetBufferName()
        {
            return bufferName;
        }

        public void ReleaseBuffer()
        {
            if (variationBuffer != null)
                variationBuffer.Release();
            variationBuffer = null;
            dataArray = null;
        }
    }
}
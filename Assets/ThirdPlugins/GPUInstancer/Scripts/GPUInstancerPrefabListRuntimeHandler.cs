using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace GPUInstancer
{
    public class GPUInstancerPrefabListRuntimeHandler : MonoBehaviour
    {
        public GPUInstancerPrefabManager prefabManager;
        private IEnumerable<GPUInstancerPrefab> _gpuiPrefabs;
        private bool _isIntancesAdded;
        public bool runInThreads = true;

        //private void OnEnable()
        //{
        //    if (prefabManager == null)
        //        return;
        //    if (!prefabManager.prototypeList.All(p => ((GPUInstancerPrefabPrototype)p).meshRenderersDisabled))
        //    {
        //        Debug.LogWarning("GPUInstancerPrefabListRuntimeHandler can not run in Threads while Mesh Renderers are enabled on the prefabs. Disabling threading...");
        //        runInThreads = false;
        //    }
        //    _gpuiPrefabs = gameObject.GetComponentsInChildren<GPUInstancerPrefab>(true);
        //    AddPrefabInstancesAsync(_gpuiPrefabs);
        //}

        public bool AddPrefabInstancesAsync(IEnumerable<GPUInstancerPrefab> prefabs)
        {
            if (prefabManager.IsAddRemoveInProgress() == true)
            {
                Debug.LogWarning($"AddPrefabInstancesAsync IsAddRemoveInProgress == true");
                return false;
            }
            //isThreadBusy = true;
            if (prefabs != null && prefabs.Count() > 0)
            {
                Debug.Log($"AddPrefabInstancesAsync prefabs :{prefabs.Count()} runInThreads:{runInThreads}");
                _isIntancesAdded = true;
                if (runInThreads)
                {
                    foreach (GPUInstancerPrefab pi in prefabs)
                    {
                        // save transform data before threading
                        pi.GetLocalToWorldMatrix(true);
                    }
                    ParameterizedThreadStart addPrefabInstancesAsync = new ParameterizedThreadStart(AddPrefabInstancesAsyncOfThread);
                    Thread addPrefabInstancesAsyncThread = new Thread(addPrefabInstancesAsync);
                    addPrefabInstancesAsyncThread.IsBackground = true;
                    prefabManager.threadStartQueue.Enqueue(new GPUInstancerManager.GPUIThreadData() { thread = addPrefabInstancesAsyncThread, parameter = prefabs });
                }
                else
                    AddPrefabInstancesAsyncOfThread(prefabs);
            }
            else
            {
                Debug.LogError($"AddPrefabInstancesAsync prefabs == null || prefabs.Count() == 0");
            }
            return true;
        }

        private void OnDisable()
        {
            _isIntancesAdded = false;
            if (prefabManager == null)
                return;
            RemovePrefabInstancesAsync(_gpuiPrefabs);
            _gpuiPrefabs = null;
        }

        //public bool isThreadBusy = false;

        public bool RemovePrefabInstancesAsync(IEnumerable<GPUInstancerPrefab> prefabs)
        {
            if (prefabManager.IsAddRemoveInProgress() == true)
            {
                Debug.LogWarning($"RemovePrefabInstancesAsync IsAddRemoveInProgress == true");
                return false;
            }
            //isThreadBusy = true;
            if (prefabs != null && prefabs.Count() > 0)
            {
                Debug.Log($"RemovePrefabInstancesAsync prefabs :{prefabs.Count()} runInThreads:{runInThreads}");
                if (runInThreads)
                {
                    bool isError = false;
                    int c = 0;
                    foreach (GPUInstancerPrefab prefabInstance in prefabs)
                    {
                        c++;
                        if (prefabInstance == null)
                        {
                            Debug.LogError($"RemovePrefabInstancesAsync[{c}] prefabInstance == null ");
                            continue;
                        }
                        if (prefabInstance.prefabPrototype == null)
                        {
                            isError = true;
                            Debug.LogError($"RemovePrefabInstancesAsync[{c}] prefabInstance.prefabPrototype==null prefabInstance:{prefabInstance}");
                            continue;
                        }
                    }
                    if (isError)
                    {
                        return false;
                    }

                    ParameterizedThreadStart removePrefabInstancesAsync = new ParameterizedThreadStart(RemovePrefabInstancesAsyncOfThread);
                    Thread removePrefabInstancesAsyncThread = new Thread(removePrefabInstancesAsync);
                    removePrefabInstancesAsyncThread.IsBackground = true;
                    prefabManager.threadStartQueue.Enqueue(new GPUInstancerManager.GPUIThreadData() { thread = removePrefabInstancesAsyncThread, parameter = prefabs });
                }
                else
                    RemovePrefabInstancesAsyncOfThread(prefabs);
            }
            else
            {
                Debug.LogError($"RemovePrefabInstancesAsync prefabs == null || prefabs.Count() == 0");
            }
            return true;
        }

        public void AddPrefabInstancesAsyncOfThread(object param)
        {
            try
            {
                prefabManager.AddPrefabInstances((IEnumerable<GPUInstancerPrefab>)param, runInThreads);
            }
            catch (Exception e)
            {
                if (runInThreads)
                {
                    prefabManager.threadException = e;
                    prefabManager.threadQueue.Enqueue(prefabManager.LogThreadException);
                }
                else
                    Debug.LogException(e);
            }
            //isThreadBusy = false;
        }

        public void RemovePrefabInstancesAsyncOfThread(object param)
        {
            try
            {
                prefabManager.RemovePrefabInstances((IEnumerable<GPUInstancerPrefab>)param, runInThreads);
            }
            catch (Exception e)
            {
                if (runInThreads)
                {
                    prefabManager.threadException = e;
                    prefabManager.threadQueue.Enqueue(prefabManager.LogThreadException);
                }
                else
                    Debug.LogException(e);
            }
            //isThreadBusy = false;
        }

        //public void SetManager(GPUInstancerPrefabManager prefabManager)
        //{
        //    if (_isIntancesAdded)
        //        OnDisable();
        //    this.prefabManager = prefabManager;
        //    if (isActiveAndEnabled)
        //        OnEnable();
        //}
    }
}

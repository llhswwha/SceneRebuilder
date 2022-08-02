using System;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer
{
    /// <summary>
    /// Add this to the prefabs of GameObjects you want to GPU Instance at runtime.
    /// </summary>
    public class GPUInstancerPrefab : MonoBehaviour
    {
        [HideInInspector]
        public GPUInstancerPrefabPrototype prefabPrototype;
        [NonSerialized]
        public int gpuInstancerID;
        [NonSerialized]
        public PrefabInstancingState state = PrefabInstancingState.None;
        public Dictionary<string, object> variationDataList;

        protected bool _isTransformSet;
        protected Transform _instanceTransform;

        protected bool _isMatrixSet;
        protected Matrix4x4 _localToWorldMatrix;

        [ContextMenu("ShowInfo")]
        public void ShowInfo()
        {
            Debug.LogError("GPUInstancerPrefab.prefabPrototype isNull:"+(prefabPrototype==null));
        }

        [ContextMenu("GeneratePrototype")]
        public void GeneratePrototype()
        {
            prefabPrototype=GPUInstancerUtility.GeneratePrefabPrototype(this.gameObject,true);
             ShowInfo();
        }

        public virtual void AddVariation<T>(string bufferName, T value)
        {
            if (variationDataList == null)
                variationDataList = new Dictionary<string, object>();
            if (variationDataList.ContainsKey(bufferName))
                variationDataList[bufferName] = value;
            else
                variationDataList.Add(bufferName, value);
        }

        public virtual Transform GetInstanceTransform(bool forceNew = false)
        {
            if (!_isTransformSet || forceNew)
            {
                _instanceTransform = transform;
                _instanceTransform.hasChanged = false;
                _isTransformSet = true;
            }
            return _instanceTransform;
        }

        public virtual Matrix4x4 GetLocalToWorldMatrix(bool forceNew = false)
        {
            if (!_isMatrixSet || forceNew)
            {
                _localToWorldMatrix = GetInstanceTransform(forceNew).localToWorldMatrix;
                _isMatrixSet = true;
            }
            return _localToWorldMatrix;
        }

        public virtual void SetupPrefabInstance(GPUInstancerRuntimeData runtimeData, bool forceNew = false)
        {

        }

        public Rigidbody CreateRigidbody()
        {
            GPUInstancerPrefabPrototype.RigidbodyData rigidbodyData = this.prefabPrototype.rigidbodyData;
            if (rigidbodyData != null && this.prefabPrototype.hasRigidBody)
            {
                Rigidbody rigidbody = this.gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = rigidbodyData.useGravity;
                rigidbody.angularDrag = rigidbodyData.angularDrag;
                rigidbody.mass = rigidbodyData.mass;
                rigidbody.constraints = rigidbodyData.constraints;
                rigidbody.detectCollisions = true;
                rigidbody.drag = rigidbodyData.drag;
                rigidbody.isKinematic = rigidbodyData.isKinematic;
                rigidbody.interpolation = rigidbodyData.interpolation;
                return rigidbody;
            }
            return null;
        }

        public static bool AlwaysGUPI = false;

        public void SetRenderersEnabled(bool enabled, int layerMask)
        {
            //Debug.Log($"SetRenderersEnabled prefabInstance:{this} enabled:{enabled} layerMask:{layerMask}");

            if (!this.prefabPrototype || !this.prefabPrototype.prefabObject)
                return;

            if (enabled && AlwaysGUPI)
            {
                return;
            }

            MeshRenderer[] meshRenderers = this.GetComponentsInChildren<MeshRenderer>(true);
            if (meshRenderers != null && meshRenderers.Length > 0)
                for (int mr = 0; mr < meshRenderers.Length; mr++)
                    if (GPUInstancerUtility.IsInLayer(layerMask, meshRenderers[mr].gameObject.layer))
                        meshRenderers[mr].enabled = enabled;

            BillboardRenderer[] billboardRenderers = this.GetComponentsInChildren<BillboardRenderer>(true);
            if (billboardRenderers != null && billboardRenderers.Length > 0)
                for (int mr = 0; mr < billboardRenderers.Length; mr++)
                    if (GPUInstancerUtility.IsInLayer(layerMask, billboardRenderers[mr].gameObject.layer))
                        billboardRenderers[mr].enabled = enabled;

            LODGroup lodGroup = this.GetComponent<LODGroup>();
            if (lodGroup != null)
                lodGroup.enabled = enabled;

            //gameObject.SetActive(enabled);

            //LODGroup lodGroup = this.GetComponent<>();
            //if (lodGroup != null)
            //    lodGroup.enabled = enabled;

            Rigidbody rigidbody = this.GetComponent<Rigidbody>();

            if (enabled)
            {
                if (rigidbody == null)
                {
                    rigidbody = this.CreateRigidbody();
                    //GPUInstancerPrefabPrototype.RigidbodyData rigidbodyData = prefabInstance.prefabPrototype.rigidbodyData;
                    //if (rigidbodyData != null && prefabInstance.prefabPrototype.hasRigidBody)
                    //{
                    //    rigidbody = prefabInstance.gameObject.AddComponent<Rigidbody>();
                    //    rigidbody.useGravity = rigidbodyData.useGravity;
                    //    rigidbody.angularDrag = rigidbodyData.angularDrag;
                    //    rigidbody.mass = rigidbodyData.mass;
                    //    rigidbody.constraints = rigidbodyData.constraints;
                    //    rigidbody.detectCollisions = true;
                    //    rigidbody.drag = rigidbodyData.drag;
                    //    rigidbody.isKinematic = rigidbodyData.isKinematic;
                    //    rigidbody.interpolation = rigidbodyData.interpolation;
                    //}
                }
            }
            else if (rigidbody != null && !this.prefabPrototype.autoUpdateTransformData)
                Destroy(rigidbody);
        }

        //public void OnDisable()
        //{
        //    Debug.LogError("OnDisable");
        //}
    }

    public enum PrefabInstancingState
    {
        None,
        Disabled,
        Instanced
    }
}

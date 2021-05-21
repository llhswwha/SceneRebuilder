using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPUInstancer
{
    [Serializable]
    public abstract class GPUInstancerPrototype : ScriptableObject
    {
        public GameObject prefabObject;

        // Shadows
        public bool isShadowCasting = true;
        public bool useCustomShadowDistance = false;
        public float shadowDistance = 0;
        public float[] shadowLODMap = new float[] {
            0, 4, 0, 0,
            1, 5, 0, 0,
            2, 6, 0, 0,
            3, 7, 0, 0};
        public bool useOriginalShaderForShadow = false;
        public bool cullShadows = false;

        // Culling
        public float minDistance = 0;
        public float maxDistance = 10000;//cww:原来的500太小了
        public bool isFrustumCulling = true;
        public bool isOcclusionCulling = true;
        public float frustumOffset = 0.2f;
        public float minCullingDistance = 0;
        public float occlusionOffset = 0;
        public int occlusionAccuracy = 1;

        // Bounds
        public Vector3 boundsOffset;

        // LOD
        public bool isLODCrossFade = false;
        public bool isLODCrossFadeAnimate = true;
        [Range(0.01f, 1.0f)]
        public float lodFadeTransitionWidth = 0.1f;
        public float lodBiasAdjustment = 1;

        // Billboard
        public GPUInstancerBillboard billboard;
        public bool isBillboardDisabled;
        // Set to true if the object should not have a billboard option
        public bool useGeneratedBillboard = false;
        public bool checkedForBillboardExtensions;

        // Other
        public bool autoUpdateTransformData;
        public GPUInstancerTreeType treeType;
        public string warningText;

        public override string ToString()
        {
            if (prefabObject != null)
                return prefabObject.name;
            return base.ToString();
        }
    }

    [Serializable]
    public class GPUInstancerBillboard
    {
        public BillboardQuality billboardQuality = BillboardQuality.Mid;
        public int atlasResolution = 2048;
        public int frameCount = 8;
        public bool replaceLODCullWithBillboard = true;
        public bool isOverridingOriginalCutoff = false;
        public float cutoffOverride = -1f;
        [Range(0.0f, 1.0f)]
        public float billboardBrightness = 0.5f;
        [Range(0.01f, 1.0f)]
        public float billboardDistance = 0.8f;

        public float quadSize;
        public float yPivotOffset;

        public Texture2D albedoAtlasTexture;
        public Texture2D normalAtlasTexture;

        // true if LOD group already has a billboard
        public bool customBillboardInLODGroup;
        // Custom billboard mesh-material options
        public bool useCustomBillboard;
        public Mesh customBillboardMesh;
        public Material customBillboardMaterial;

        public bool isBillboardShadowCasting = false;

        public bool billboardFaceCamPos = false;
    }

    public enum BillboardQuality
    {
        Low = 0,
        Mid = 1,
        High = 2,
        VeryHigh = 3
    }

    [Serializable]
    public class GPUInstancerPrototypeList//:List<GPUInstancerPrototype>
    {
        public List<GPUInstancerPrototype> List=new List<GPUInstancerPrototype>();

        public GPUInstancerPrototype this[int index] 
        {
            get{
                return List[index];
            }   
            set
            {
                List[index]=value;
            }
        }

        public void Add(GPUInstancerPrototype item)
        {
            //Debug.LogError("GPUInstancerPrototypeList.Add:"+item);
            List.Add(item);
        }

        public void Remove(GPUInstancerPrototype item)
        {
            Debug.LogError("GPUInstancerPrototypeList.Add:"+item);
            List.Remove(item);
        }

        public int IndexOf(GPUInstancerPrototype item){
            return List.IndexOf(item);
        }

        public int Count{
            get{
                return List.Count;
            }
        }

        public void RemoveAll(Predicate<GPUInstancerPrototype> match)
        {
            int count1=this.Count;
            
            List.RemoveAll(match);
            //Debug.LogError("GPUInstancerPrototypeList.RemoveAll:"+count1+">"+this.Count);
        }

        public void Clear()
        {
            Debug.LogError("GPUInstancerPrototypeList.Clear");
            List.Clear();
        }

        public bool Contains(GPUInstancerPrototype item){
            return List.Contains(item);
        }

        public List<GPUInstancerPrototype> ToList()
        {
            return List;
        }

        public IEnumerator<GPUInstancerPrototype> GetEnumerator(){
            return List.GetEnumerator();
        }

        public bool All(Func<GPUInstancerPrototype, bool> predicate)
        {
            return List.All(predicate);
            //public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
        }

        public bool Exists(Predicate<GPUInstancerPrototype> match)
        {
            return List.Exists(match);
        }
    }

    public class GPUInstancerPrototypeDict: Dictionary<GPUInstancerPrototype, List<GPUInstancerPrefab>>
    {
        public GPUInstancerPrototypeDict()
        {
        }
        public GPUInstancerPrototypeDict(List<GPUInstancerPrototype> prototypeList)
        {
            Debug.LogError($"GPUInstancerPrototypeDict prototypeList: {prototypeList.Count}");

            if (this.Keys.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype prototype in prototypeList)
                    if (!this.ContainsKey(prototype))
                        this.Add(prototype, new List<GPUInstancerPrefab>());
            }
        }

        public GPUInstancerPrototypeDict(GPUInstancerPrototypeList prototypeList)
        {
            Debug.LogError($"GPUInstancerPrototypeDict prototypeList: {prototypeList.Count}");

            if (this.Keys.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype prototype in prototypeList)
                    if (!this.ContainsKey(prototype))
                        this.Add(prototype, new List<GPUInstancerPrefab>());
            }
        }

        public void UnregisterInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            {
                this[prefabInstance.prefabPrototype].Remove(prefabInstance);
            }
        }

        public void RegisterInstanceList(IEnumerable<GPUInstancerPrefab> prefabInstanceList)
        {
            Debug.LogError("GPUInstancerPrototypeDict.RegisterInstanceList Dict Count: " + this.Count+"|"+this.Keys.Count);
            foreach(var key in this.Keys)
            {
                Debug.LogError("GPUInstancerPrototypeDict.key:"+key);
            }

            foreach (GPUInstancerPrefab prefabInstance in prefabInstanceList)
            {
                if (prefabInstance == null)
                {
                    Debug.LogWarning("prefabInstance == null");
                    continue;
                }
                if (prefabInstance.prefabPrototype == null)
                {
                    Debug.LogWarning("prefabInstance.prefabPrototype == null");
                    continue;
                }
                if(this.ContainsKey(prefabInstance.prefabPrototype)==false)
                {
                    Debug.LogWarning("GPUInstancerPrototypeDict.ContainsKey(prefabInstance.prefabPrototype)==false："+ prefabInstance.prefabPrototype);
                }
                else
                {
                    this[prefabInstance.prefabPrototype].Add(prefabInstance);
                }
                
            }
        }

        public void AddPrototypeList(List<GPUInstancerPrototype> prototypeList)
        {
            if (this.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype p in prototypeList)
                {
                    if (!this.ContainsKey(p))
                        this.Add(p, new List<GPUInstancerPrefab>());
                }
            }
        }

        public void AddPrototypeList(GPUInstancerPrototypeList prototypeList)
        {
            if (this.Count != prototypeList.Count)
            {
                foreach (GPUInstancerPrototype p in prototypeList)
                {
                    if (!this.ContainsKey(p))
                        this.Add(p, new List<GPUInstancerPrefab>());
                }
            }
        }

        public void AddPrefabDatas(List<RegisteredPrefabsData> registeredPrefabs)
        {
            if (registeredPrefabs != null && registeredPrefabs.Count > 0)
            {
                foreach (RegisteredPrefabsData rpd in registeredPrefabs)
                {
                    var prototype=rpd.prefabPrototype;
                    var prefabs=rpd.registeredPrefabs;
                    if (!this.ContainsKey(prototype))
                        this.Add(prototype, prefabs);
                    else
                    {
                        this[prototype].AddRange(prefabs);
                        this[prototype] = new List<GPUInstancerPrefab>(this[prototype].Distinct());
                    }
                }
                registeredPrefabs.Clear();
            }
        }

        public void AddInstance(GPUInstancerPrefab prefabInstance)
        {
            var prototype=prefabInstance.prefabPrototype;
            if (!this.ContainsKey(prototype))
                this.Add(prototype, new List<GPUInstancerPrefab>());
            this[prototype].Add(prefabInstance);
        }

        public List<GPUInstancerPrefab> GetInstanceList(GPUInstancerPrefabPrototype prefabPrototype)
        {
            List<GPUInstancerPrefab> instanceList;
            if (!this.TryGetValue(prefabPrototype, out instanceList))
            {
                instanceList = new List<GPUInstancerPrefab>();
                this.Add(prefabPrototype, instanceList);
            }
            return instanceList;
        }

        public void ClearInstances()
        {
            foreach (GPUInstancerPrototype p in this.Keys)
            {
                this[p].Clear();
            }
        }

        public void ClearInstances(GPUInstancerPrototype p)
        {
            if (this.ContainsKey(p))
                this[p].Clear();
        }

        internal void UpdateTransform(GPUInstancerRuntimeData runtimeData)
        {
            List<GPUInstancerPrefab> prefabInstanceList = this[runtimeData.prototype];
            runtimeData.UpdateTransform(prefabInstanceList);
        }

        public void ClearInstancingData(bool enabled, int layerMask, bool isSetRenderersEnabled)
        {
            foreach (GPUInstancerPrefabPrototype p in this.Keys)
            {
                if (p.meshRenderersDisabled)
                    continue;
                foreach (GPUInstancerPrefab prefabInstance in this[p])
                {
                    if (!prefabInstance)
                        continue;
                    if (isSetRenderersEnabled)
                        prefabInstance.SetRenderersEnabled(enabled, layerMask);
                }
            }
        }
    }
}

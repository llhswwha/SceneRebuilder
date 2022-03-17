using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPUInstancer
{
    public class AstroidGenerator : MonoBehaviour
    {
        public static AstroidGenerator Instance;

        [Range(0, 1000000)]
        public int count = 100000;
        
        public List<GPUInstancerPrefab> asteroidObjects = new List<GPUInstancerPrefab>();
        public GPUInstancerPrefabManager prefabManager;
        public Transform centerTransform;

        private List<GPUInstancerPrefab> asteroidInstances = new List<GPUInstancerPrefab>();
        private int instantiatedCount;
        private Vector3 center;
        private Vector3 allocatedPos;
        private Quaternion allocatedRot;
        private Vector3 allocatedLocalEulerRot;
        private Vector3 allocatedLocalScale;
        private GPUInstancerPrefab allocatedGO;
        private GameObject goParent;
        private float allocatedLocalScaleFactor;
        private int columnSize;
        private int columnSpace = 3;

        public bool IsGenerateWhenAwake = true;

        private void Awake()
        {
            Instance = this;
            if (IsGenerateWhenAwake)
                GenerateGos();
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            var prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();

            for (int i = 0; i < prefabs.Length; i++)
            {
                GPUInstancerPrefab item = prefabs[i];
                if (asteroidObjects.Contains(item)) continue;

                GameObject.DestroyImmediate(item.gameObject);

                float progress = (float)i / prefabs.Length;
                float percents = progress * 100;



#if UNITY_EDITOR
                if (UnityEditor.EditorUtility.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{prefabs.Length} {percents:F1}%", progress))
                {
                    break;
                }
#endif
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        }

        [ContextMenu("GenerateGos")]
        public void GenerateGos()
        {
            Clear();

            instantiatedCount = 0;
            center = centerTransform.position;
            allocatedPos = Vector3.zero;
            allocatedRot = Quaternion.identity;
            allocatedLocalEulerRot = Vector3.zero;
            allocatedLocalScale = Vector3.one;
            allocatedLocalScaleFactor = 1f;

            goParent = new GameObject("Asteroids");
            goParent.transform.position = center;
            goParent.transform.parent = gameObject.transform;

            columnSize = count < 5000 ? 1 : count / 2500;

            int firstPassColumnSize = count % columnSize > 0 ? columnSize - 1 : columnSize;

            asteroidInstances.Clear();

            for (int i = 0; i < asteroidObjects.Count; i++)
            {
                GPUInstancerPrefab obj = asteroidObjects[i];
                if (obj == null)
                {
                    Debug.LogError($"GenerateGos obj == null :{i}/{asteroidObjects.Count}");
                    asteroidObjects.RemoveAt(0);
                    i--;
                }
            }

            if (asteroidObjects.Count == 0)
            {
                return;
            }

            for (int h = 0; h < firstPassColumnSize; h++)
            {
                for (int i = 0; i < Mathf.FloorToInt((float)count / columnSize); i++)
                {
                    var instance = InstantiateInCircle(center, h);
                    if (instance == null) continue;
                    asteroidInstances.Add(instance);
                }
            }

            if (firstPassColumnSize != columnSize)
            {
                for (int i = 0; i < count - (Mathf.FloorToInt((float)count / columnSize) * firstPassColumnSize); i++)
                {
                    var instance = InstantiateInCircle(center, columnSize);
                    if (instance == null) continue;
                    asteroidInstances.Add(instance);
                }
            }
            Debug.Log("Instantiated " + instantiatedCount + " objects.");
        }

        public bool IsUseGPU = true;

        private void Start()
        {
            if (IsUseGPU)
            {
                StartGPUInstance();
            }
        }

        public void StartGPUInstance()
        {
            if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
            {
                if (asteroidInstances == null || asteroidInstances.Count == 0)
                {
                    asteroidInstances = GameObject.FindObjectsOfType<GPUInstancerPrefab>().ToList();
                }
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, asteroidInstances);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);

                var prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
                Debug.LogError("prefabs:"+ prefabs.Length);
            }
        }

        private void SetRandomPosInCircle(Vector3 center, int column, float radius)
        {
            float ang = Random.value * 360;

            allocatedPos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            allocatedPos.y = center.y - (column * (float)columnSpace / 2) + (column * columnSpace) + Random.Range(HeightRange.x, HeightRange.y);
            allocatedPos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        }

        public Vector2 ScaleRange = new Vector2(0.3f, 1.2f);

        public Vector2 RadiusRange = new Vector2(80.0f, 150f);

        public Vector2 HeightRange = new Vector2(0, 1);

        private GPUInstancerPrefab InstantiateInCircle(Vector3 center, int column)
        {
            SetRandomPosInCircle(center, column - Mathf.FloorToInt(columnSize / 2f), Random.Range(RadiusRange.x, RadiusRange.y));
            allocatedRot = Quaternion.FromToRotation(Vector3.forward, center - allocatedPos);
            int id = Random.Range(0, asteroidObjects.Count);
            var prefab = asteroidObjects[id];
            
            if (prefab == null)
            {
                Debug.LogError($"InstantiateInCircle asteroidObjects:{asteroidObjects.Count} prefab:{prefab} id:{id}");
                return null;
            }
            allocatedGO = Instantiate(prefab, allocatedPos, allocatedRot);
            allocatedGO.transform.parent = goParent.transform;

            allocatedLocalEulerRot.x = Random.Range(-180f, 180f);
            allocatedLocalEulerRot.y = Random.Range(-180f, 180f);
            allocatedLocalEulerRot.z = Random.Range(-180f, 180f);
            allocatedGO.transform.localRotation = Quaternion.Euler(allocatedLocalEulerRot);

            allocatedLocalScaleFactor = Random.Range(ScaleRange.x, ScaleRange.y);
            allocatedLocalScale.x = allocatedLocalScaleFactor;
            allocatedLocalScale.y = allocatedLocalScaleFactor;
            allocatedLocalScale.z = allocatedLocalScaleFactor;
            allocatedGO.transform.localScale = allocatedLocalScale;

            instantiatedCount++;

            return allocatedGO;
        }
    }
}

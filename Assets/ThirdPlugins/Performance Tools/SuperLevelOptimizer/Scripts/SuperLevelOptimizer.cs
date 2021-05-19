using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace NGS.SuperLevelOptimizer
{
    public enum BakeType { FullScene, Zonal }
    public enum SearchState { Automatical, User, ByTag }
    public enum CombineState { CombineToScene, CombineToPrefab }

    public class SuperLevelOptimizer : MonoBehaviour
    {

#if UNITY_EDITOR

#if UNITY_2017
        public const int UVsCount = 4;
#else
        public const int UVsCount = 8;
#endif
        public readonly CoefficientTable coefficientTable = new CoefficientTable();

        private static string _dataFolder = "Assets/SLO_Data/";
        public static string DataFolder
        {
            get
            {
                return _dataFolder;
            }

            set
            {
                if (!value.StartsWith("Assets/"))
                    value = "Assets/" + value;

                if (!value.EndsWith("/"))
                    value += "/";

                _dataFolder = value;
            }
        }

        public BakeType bakeType = BakeType.FullScene;
        public CombineState combineState = CombineState.CombineToScene;
        public SearchState searchState = SearchState.Automatical;

        public string searchTag = "Untagged";
        public bool saveColliders = false;

        [SerializeField]
        private Vector3 _zoneCount = Vector3.one;
        public Vector3 zoneCount
        {
            get
            {
                return _zoneCount;
            }

            set
            {
                _zoneCount.x = Mathf.Max(1, value.x);
                _zoneCount.y = Mathf.Max(1, value.y);
                _zoneCount.z = Mathf.Max(1, value.z);
            }
        }

        [SerializeField]
        private string _prefabsFolder = DataFolder + "Prefabs/";
        public string prefabsFolder
        {
            get
            {
                return _prefabsFolder;
            }

            set
            {
                if (!value.StartsWith("Assets/"))
                    value = "Assets/" + value;

                if (!value.EndsWith("/"))
                    value += "/";

                _prefabsFolder = value;
            }
        }

        [SerializeField]
        private List<Renderer> _objectsForCombine = new List<Renderer>();
        public List<Renderer> objectsForCombine
        {
            get
            {
                return _objectsForCombine;
            }
        }


        public bool CanCombineRenderer(Renderer renderer)
        {
            if (renderer == null)
                return false;

            if (!GameObjectUtility.AreStaticEditorFlagsSet(renderer.gameObject, StaticEditorFlags.BatchingStatic))
                return false;

            if (renderer.GetComponent<MeshFilter>() == null)
                return false;

            if (renderer.GetComponent<MeshFilter>().sharedMesh == null)
                return false;

            if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
                return false;

            return true;
        }

        public Renderer[] GetRenderersForCombine()
        {
            Renderer[] renderers = null;

            if (searchState == SearchState.User)
                renderers = objectsForCombine.ToArray();
            
            else if (searchState == SearchState.ByTag)
            {
                renderers = GameObject.FindGameObjectsWithTag(searchTag)
                        .Where(go => go.GetComponent<Renderer>() != null)
                        .Select(go => go.GetComponent<Renderer>())
                        .Where(r => CanCombineRenderer(r))
                        .ToArray();
            }
            else renderers = FindObjectsOfType<Renderer>()
                    .Where(r => CanCombineRenderer(r))
                    .ToArray();

            if (renderers == null || renderers.Length == 0)
            {
                string log = "No objects found";

                if (searchState == SearchState.User)
                    log += ", add objects in Manager Window";

                else if (searchState == SearchState.ByTag)
                    log += ", add tag(" + searchTag + ") to objects";

                log += ", mark objects as static";

                Debug.Log(log);
            }

            return renderers;
        }


        public void AddObjectsForCombine(IEnumerable<Renderer> renderers)
        {
            foreach (var renderer in renderers)
                AddObjectForCombine(renderer);
        }

        public void AddObjectForCombine(Renderer renderer)
        {
            if (!CanCombineRenderer(renderer) || _objectsForCombine.Contains(renderer))
                return;

            _objectsForCombine.Add(renderer);
        }


        public void DeleteObjectsForCombine(IEnumerable<Renderer> renderers)
        {
            foreach (var renderer in renderers)
                DeleteObjectForCombine(renderer);
        }

        public void DeleteObjectForCombine(Renderer renderer)
        {
            if (_objectsForCombine.Contains(renderer))
                _objectsForCombine.Remove(renderer);
        }


        public void ClearObjectsForCombine()
        {
            _objectsForCombine.Clear();
        }

#endif
    }

    [Serializable]
    public class CoefficientTable
    {
        [SerializeField]
        private float _floatThreshold = 0.1f;
        public float floatThreshold
        {
            get
            {
                return _floatThreshold;
            }

            set
            {
                _floatThreshold = Mathf.Max(value, 0);
            }
        }

        [SerializeField]
        private Vector4 _vectorThreshold = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        public Vector4 vectorThreshold
        {
            get
            {
                return _vectorThreshold;
            }

            set
            {
                _vectorThreshold.x = Mathf.Max(0, value.x);
                _vectorThreshold.y = Mathf.Max(0, value.y);
                _vectorThreshold.z = Mathf.Max(0, value.z);
                _vectorThreshold.w = Mathf.Max(0, value.w);
            }
        }

        [SerializeField]
        private Color _colorThreshold = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        public Color colorThreshold
        {
            get
            {
                return _colorThreshold;
            }

            set
            {
                _colorThreshold.r = Mathf.Max(0, value.r);
                _colorThreshold.g = Mathf.Max(0, value.g);
                _colorThreshold.b = Mathf.Max(0, value.b);
                _colorThreshold.a = Mathf.Max(0, value.a);
            }
        }


        public CoefficientTable() { }

        public CoefficientTable(float floatThreshold, Vector4 vectorThreshold, Color colorThreshold)
        {
            this.floatThreshold = floatThreshold;

            this.vectorThreshold = vectorThreshold;

            this.colorThreshold = colorThreshold;
        }


        public bool IsEqual(float value1, float value2, float range)
        {
            range = Mathf.Max(range, 0.001f);

            return Mathf.Abs(value1 - value2) <= range;
        }

        public bool IsEqual(float value1, float value2)
        {
            return IsEqual(value1, value2, _floatThreshold);
        }

        public bool IsEqual(Vector4 vec1, Vector4 vec2)
        {
            if (!IsEqual(vec1.x, vec2.x, _vectorThreshold.x)) return false;
            if (!IsEqual(vec1.y, vec2.y, _vectorThreshold.y)) return false;
            if (!IsEqual(vec1.z, vec2.z, _vectorThreshold.z)) return false;
            if (!IsEqual(vec1.w, vec2.w, _vectorThreshold.w)) return false;

            return true;
        }

        public bool IsEqual(Color color1, Color color2)
        {
            if (!IsEqual(color1.r, color2.r, _colorThreshold.r)) return false;
            if (!IsEqual(color1.g, color2.g, _colorThreshold.g)) return false;
            if (!IsEqual(color1.b, color2.b, _colorThreshold.b)) return false;
            if (!IsEqual(color1.a, color2.a, _colorThreshold.a)) return false;

            return true;
        }
    }
}

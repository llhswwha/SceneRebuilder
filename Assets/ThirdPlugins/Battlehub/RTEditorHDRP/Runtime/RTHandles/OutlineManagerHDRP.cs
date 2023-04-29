using Battlehub.RTCommon;
using Battlehub.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Battlehub.RTHandles.HDRP
{
    public class OutlineManagerHDRP : MonoBehaviour, IOutlineManager
    {
        private IRenderersCache m_cache;
        private IRTE m_editor;
        private IRTEGraphics m_graphics;

        [SerializeField]
        private Material m_selectionMaterial = null;
        public Material SelectionMaterial
        {
            get { return m_selectionMaterial; }
            set { m_selectionMaterial = value; }
        }

        private IRuntimeSelection m_selectionOverride;
        public IRuntimeSelection Selection
        {
            get
            {
                if (m_selectionOverride != null)
                {
                    return m_selectionOverride;
                }

                return m_editor.Selection;
            }
            set
            {
                if (m_selectionOverride != value)
                {
                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged -= OnSelectionChanged;
                    }

                    m_selectionOverride = value;
                    if (m_selectionOverride == m_editor.Selection)
                    {
                        m_selectionOverride = null;
                    }

                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged += OnSelectionChanged;
                    }
                }
            }
        }

        private void Awake(){
            //Debug.Log("OutlineManagerHDRP.Awake");
        }

        private void Start()
        {
            m_graphics = IOC.Resolve<IRTEGraphics>();
            m_graphics.SetCameraId("Outline");
            m_cache = m_graphics.CreateSharedRenderersCache(CameraEvent.AfterEverything);
            m_cache.MaterialOverride = m_selectionMaterial;
            m_editor = IOC.Resolve<IRTE>();

            TryToAddRenderers(m_editor.Selection);
            m_editor.Selection.SelectionChanged += OnRuntimeEditorSelectionChanged;
            m_editor.Object.Enabled += OnObjectEnabled;
            m_editor.Object.Disabled += OnObjectDisabled;

            IOC.RegisterFallback<IOutlineManager>(this);

            StartCoroutine(EnableSelectionFullScreenPass());
        }

        private void OnDestroy()
        {
            Debug.LogError("OutlineManagerHDRP.OnDestroy:"+this);
            if (m_editor != null)
            {
                if(m_editor.Selection != null)
                {
                    m_editor.Selection.SelectionChanged -= OnRuntimeEditorSelectionChanged;
                }
                
                if(m_editor.Object != null)
                {
                    m_editor.Object.Enabled -= OnObjectEnabled;
                    m_editor.Object.Disabled -= OnObjectDisabled;
                }
            }

            if (m_selectionOverride != null)
            {
                m_selectionOverride.SelectionChanged -= OnSelectionChanged;
            }

            if(m_graphics != null)
            {
                m_graphics.DestroySharedRenderersCache(m_cache);
            }
            
            IOC.UnregisterFallback<IOutlineManager>(this);
        }

        private IEnumerator EnableSelectionFullScreenPass()
        {
            yield return new WaitForEndOfFrame();
            CustomPassVolume[] customPassVolume = GetComponents<CustomPassVolume>();
            foreach (CustomPassVolume volume in customPassVolume)
            {
                foreach (CustomPass pass in volume.customPasses)
                {
                    if (pass.name == "SelectionFullScreenPass")
                    {
                        pass.enabled = true;
                    }
                }
            }
        }

        private void OnObjectEnabled(ExposeToEditor obj)
        {
            if (m_selectionOverride != null)
            {
                OnSelectionChanged(m_selectionOverride.objects);
            }
            else
            {
                OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
            }
        }

        private void OnObjectDisabled(ExposeToEditor obj)
        {
            if (m_selectionOverride != null)
            {
                OnSelectionChanged(m_selectionOverride.objects);
            }
            else
            {
                OnRuntimeEditorSelectionChanged(m_editor.Selection.objects);
            }
        }

        private void OnRuntimeEditorSelectionChanged(Object[] unselectedObject)
        {
            OnSelectionChanged(m_editor.Selection, unselectedObject);
        }

        private void OnSelectionChanged(Object[] unselectedObjects)
        {
            OnSelectionChanged(m_selectionOverride, unselectedObjects);
        }

        private void OnSelectionChanged(IRuntimeSelection selection, Object[] unselectedObjects)
        {
            TryToRemoveRenderers(unselectedObjects);
            TryToAddRenderers(selection);
        }

        private void TryToRemoveRenderers(Object[] unselectedObjects)
        {
            if (unselectedObjects != null)
            {
                Renderer[] renderers = unselectedObjects.Select(go => go as GameObject).Where(go => go != null).SelectMany(go => go.GetComponentsInChildren<Renderer>(true)).ToArray();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    Renderer renderer = renderers[i];
                    m_cache.Remove(renderer);
                }
            }
        }

        private void TryToAddRenderers(IRuntimeSelection selection)
        {
            if (selection.gameObjects != null)
            {
                IList<Renderer> renderers = GetRenderers(selection.gameObjects);
                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer renderer = renderers[i];
                    m_cache.Add(renderer);
                }
            }
        }

        private IList<GameObject> FilterSelection(IList<GameObject> gameObjects)
        {
            IList<GameObject> result = new List<GameObject>();

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];
                if (go == null || go.IsPrefab() || (go.hideFlags & HideFlags.HideInHierarchy) != 0)
                {
                    continue;
                }

                ExposeToEditor exposed = go.GetComponent<ExposeToEditor>();
                if (exposed == null || exposed.ShowSelectionGizmo)
                {
                    result.Add(go);
                }
            }
            return result;
        }

        private IList<Renderer> GetRenderers(IList<GameObject> gameObjects)
        {
            List<Renderer> result = new List<Renderer>();

            gameObjects = FilterSelection(gameObjects);

            for (int i = 0; i < gameObjects.Count; ++i)
            {
                GameObject go = gameObjects[i];

                foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.gameObject.activeInHierarchy && (renderer.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        result.Add(renderer);
                    }
                }
            }

            return result;
        }

        public void AddRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                m_cache.Add(renderer);
            }
        }

        public void RemoveRenderers(Renderer[] renderers)
        {
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];
                m_cache.Remove(renderer);
            }
        }

        public void RecreateCommandBuffer()
        {

        }

        public bool ContainsRenderer(Renderer renderer)
        {
            return m_cache.Renderers.Contains(renderer);
        }

    }
}


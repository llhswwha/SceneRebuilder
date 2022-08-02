using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class StaticCulling : SingletonBehaviour<StaticCulling>
    {
        public static float _cellSizeS = 0;

        public static float _cellSplitCountS = 0;

        //public static StaticCulling Instance { get; private set; }

        [SerializeField]
        //[HideInInspector]
        private MeshRenderer[] _renderers;

        [SerializeField]
        private string[] _renderersIds;

        [SerializeField]
        //[HideInInspector]
        private BinaryTree[] _trees;

        [ContextMenu("GetRendererIds")]
        private void GetRendererIds()
        {
            if (_renderers != null)
            {
                _renderersIds = new string[_renderers.Length];
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderersIds[i] = RendererId.GetId(_renderers[i]);
                }
            }
        }

        internal List<BinaryTreeNode> FindBinaryTreeNodes(string id)
        {
            List<BinaryTreeNode> nodes = new List<BinaryTreeNode>();
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                List<BinaryTreeNode> ns = tree.FindBinaryTreeNodes(id);
                nodes.AddRange(ns);
            }
            _drawSlectedNodes = true;
            _drawCullingArea = false;

            SelectedNodes = nodes;
            return nodes;
        }

        public List<BinaryTreeNode> SelectedNodes = new List<BinaryTreeNode>();

        internal List<BinaryTreeNode> GetAllLeafNodes()
        {
            List<BinaryTreeNode> nodes = new List<BinaryTreeNode>();
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                List<BinaryTreeNode> ns = tree.GetAllLeafNodes();
                nodes.AddRange(ns);
            }
            return nodes;
        }

        [ContextMenu("GetAllIds")]
        private void GetAllIds()
        {
            //GetRendererIds();

            DictList<string> ids = new DictList<string>();
            System.DateTime start = System.DateTime.Now;
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                ids.AddRange(tree.GetAllIds());
            }
            //_activeRendererIds = ids.NewList();
            Debug.LogError($"StaticCulling.GetRendererIds _trees:{_trees.Length} ids:{ids.Count} time:{System.DateTime.Now - start}");
        }

        public bool isShowLog = false;

        [ContextMenu("CheckRenderers")]
        public List<MeshRenderer> CheckRenderers()
        {
            var treeRenderers = GetTreeRenderers();
            int count1 = treeRenderers.Count;
            DictList<MeshRenderer> rendererDict = new DictList<MeshRenderer>(_renderers);
            int count2 = rendererDict.Count;
            List<MeshRenderer> list = new List<MeshRenderer>();
            foreach (var renderer in treeRenderers)
            {
                rendererDict.Remove(renderer);
            }
            int count3 = rendererDict.Count;
            if (count3 > 0)
            {
                string nameList = "";
                list = rendererDict.NewList();
                foreach (var renderer in list)
                {
                    nameList += renderer.name + ";";
                }
                Debug.LogError($"CheckRenderers _renderers:{count2} treeRenderers:{count1} NoVisibleRenderers:{count3} nameList:{nameList}");
            }
            else
            {
                //Debug.Log($"CheckRenderers _renderers:{count2} treeRenderers:{count1} NoVisibleRenderers:{count3}");
            }
            return list;
        }

        public List<MeshRenderer> GetTreeRenderers()
        {
            DictList<MeshRenderer> renderers = new DictList<MeshRenderer>();
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                renderers.AddRange(tree.GetAllRenderers());
            }
            return renderers.NewList();
        }

        [ContextMenu("GetRenderers")]
        public void GetRenderers()
        {
            System.DateTime start = System.DateTime.Now;
            try
            {
                IdDictionary.InitInfos();

                int rendererCount = 0;
                if (_renderers != null && _renderersIds != null)
                {
                    if (_renderers.Length == _renderersIds.Length)
                    {
                        for (int i = 0; i < _renderersIds.Length; i++)
                        {
                            if (_renderers[i] == null)
                            {
                                _renderers[i] = IdDictionary.GetRenderer(_renderersIds[i], isShowLog);

                                if (_renderers[i] != null)
                                {
                                    _activeRenderers.Add(_renderers[i]);
                                }
                            }

                            if (_renderers[i] != null)
                            {
                                rendererCount++;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"StaticCulling.GetRenderers _renderers.Length({_renderers.Length}) != _renderersIds.Length({_renderersIds.Length}) _trees:{_trees.Length} time:{System.DateTime.Now - start}");
                    }

                    //_renderersIds = new string[_renderers.Length];
                    //for (int i = 0; i < _renderers.Length; i++)
                    //{
                    //    _renderersIds[i] = RendererId.GetId(_renderers[i]);
                    //}
                }

                if (_trees != null)
                {
                    foreach (var tree in _trees)
                    {
                        if (tree == null) continue;
                        tree.GetRenderers();
                    }
                }

                Debug.Log($"StaticCulling.GetRenderers rendererCount:{rendererCount} time:{System.DateTime.Now - start}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"StaticCulling.GetRenderers time:{System.DateTime.Now - start} Exception:{ex}");
            }
            
        }

        [ContextMenu("RemoveEmptyRenderers")]
        public void RemoveEmptyRenderers()
        {
            System.DateTime start = System.DateTime.Now;
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                tree.RemoveEmptyRenderers();
            }
            Debug.LogError($"StaticCulling.RemoveEmptyRenderers _trees:{_trees.Length} time:{System.DateTime.Now - start}");
        }

        [SerializeField]
        private List<Camera> _cameras;

        [Header("Editor only")]

        [SerializeField]
        private bool _enableFrustum;

        [SerializeField]
        private bool _drawCullingArea;

        [SerializeField]
        private bool _drawSlectedNodes;

        private DictList<MeshRenderer> _activeRenderers = new DictList<MeshRenderer>();

        private DictList<string> _activeRendererIds = new DictList<string>();

        [SerializeField]
        private DictList<string> visibleRendererIds = new DictList<string>();

        public static StaticCulling Create(MeshRenderer[] renderers, BinaryTree[] trees, Camera[] cameras = null)
        {
            StaticCulling staticCulling = new GameObject("Static Culling").AddComponent<StaticCulling>();

            staticCulling._renderers = renderers;
            staticCulling.GetRendererIds();
            staticCulling._trees = trees;

            if (cameras != null)
            {
                staticCulling._cameras = new List<Camera>();

                for (int i = 0; i < cameras.Length; i++)
                    staticCulling.AddCamera(cameras[i]);
            }

            return staticCulling;
        }


        private void Awake()
        {
            //Instance = this;
            GetRenderers();
            
            _activeRenderers.AddRange(_renderers);
            _activeRendererIds.AddRange(_renderersIds);
        }

        private void Start()
        {
            CheckCameras();
        }

        public bool IsInsideCullingArea = false;

        public double UpdateTime = 0;

        public BinaryTreeNode currentNode;

        private void EnableTreeNodeRenderers(Camera camera, BinaryTreeNode node)
        {
            EnableRenderers(camera, node.visibleRenderers);
            visibleRendererIds.AddRange(node.visibleRendererIds);
            if (isShowLog)
            {
                Debug.Log($"EnableTreeNodeRenderers visibleRenderers:{node.visibleRenderers.Count}");
            }
        }

        private void Update()
        {
            System.DateTime start = System.DateTime.Now;
            try
            {
                DisableRenderers();
                int i = 0;
                while (i < _cameras.Count)
                {
                    if (_cameras[i] == null)
                    {
                        Debug.Log("StaticCulling::looks like camera was destroyed");
                        _cameras.RemoveAt(i);
                        if (!CheckCameras())
                            return;
                        continue;
                    }
                    bool insideArea = false;
                    Vector3 pos = _cameras[i].transform.position;
                    for (int c = 0; c < _trees.Length; c++)
                    {
                        if (new Bounds(_trees[c].rootNode.center, _trees[c].rootNode.size).Contains(pos))
                        {
                            BinaryTreeNode node = _trees[c].GetNode(pos);
                            EnableTreeNodeRenderers(_cameras[i], node);
                            insideArea = true;
                            currentNode = node;
                            break;
                        }
                    }
                    IsInsideCullingArea = insideArea;

                    if (!insideArea)
                    {
                        EnableRenderers();
                        return;
                    }

                    i++;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("StaticCulling::get exception");
                Debug.Log("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.Log("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.Log("-----------------------------------");

                Disable();
            }
            UpdateTime = (System.DateTime.Now - start).TotalMilliseconds;
        }

        internal void SetToSelected(BinaryTreeNode binaryTreeNode)
        {
            SelectedNodes = binaryTreeNode.GetAllLeafNodes();
        }

        private void OnDrawGizmos()
        {
            if (!_drawCullingArea || _trees == null || _trees.Length == 0)
                return;

            foreach (var tree in _trees)
                BinaryTreeUtil.DrawGizmos(tree, Color.blue);
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawCullingArea || _trees == null || _trees.Length == 0)
            {
                DrawSlectedNodes();
                return;
            }

            Gizmos.color = Color.blue;
            foreach (var tree in _trees)
                Gizmos.DrawWireCube(tree.rootNode.center, tree.rootNode.size);

            DrawSlectedNodes();
        }

        private void DrawSlectedNodes()
        {
            if (_drawSlectedNodes)
            {
                Gizmos.color = Color.red;
                foreach (var node in SelectedNodes)
                    Gizmos.DrawWireCube(node.center, node.size);
            }
        }


        private bool CheckCameras()
        {
            if (_cameras == null || _cameras.Count == 0)
            {
                Debug.Log("StaticCulling::no cameras assigned");
                //Disable();
                Camera cam = Camera.main;
                if (cam != null)
                {
                    _cameras = new List<Camera>() { cam };
                }
            }
            if (_cameras == null || _cameras.Count == 0)
            {
                Debug.LogError("StaticCulling::no cameras assigned");

                Disable();
                return false;
            }
            return true;
        }

        private void DisableRenderers()
        {
            //if (_activeRenderers.Count > 0)
            //{
            //    Debug.Log($"StaticCulling.DisableRenderers _activeRenderers:{_activeRenderers.Count}");
            //}

            for (int i = 0; i < _activeRenderers.Count; i++)
                HideRenderer(_activeRenderers[i]);

            _activeRenderers.Clear();

            visibleRendererIds.Clear();
        }

        public bool EnableChangeRenderer = true;

        private void HideRenderer(MeshRenderer renderer)
        {
            if (renderer==null) return;
            if (EnableChangeRenderer)
            {
                //if (renderer.name == TestName)
                //{
                //    Debug.LogError($"HideRenderer {TestName}");
                //}
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        private void ShowRenderer(MeshRenderer renderer)
        {
            if (renderer == null) return;
            if (EnableChangeRenderer)
            {
                //if(renderer.name== TestName)
                //{
                //    Debug.LogError($"ShowRenderer {TestName}");
                //}
                renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }

        //public string TestName = "对象076";

        private void EnableRenderers()
        {
            for (int i = 0; i < _renderers.Length; i++)
                ShowRenderer(_renderers[i]);

            _activeRenderers.AddRange(_renderers);
            visibleRendererIds.AddRange(_renderersIds);
        }

        private void EnableRenderers(Camera camera, List<MeshRenderer> renderers)
        {
#if UNITY_EDITOR

            Plane[] planes = null;

            if (_enableFrustum)
                planes = GeometryUtility.CalculateFrustumPlanes(camera);

            for (int i = 0; i < renderers.Count; i++)
            {
                if (!_enableFrustum || GeometryUtility.TestPlanesAABB(planes, renderers[i].bounds))
                {
                    ShowRenderer(renderers[i]);

                    _activeRenderers.Add(renderers[i]);
                }
            }

#else

            for (int i = 0; i < renderers.Count; i++)
                ShowRenderer(renderers[i]);

            _activeRenderers.AddRange(renderers);

#endif
        }


        public void AddCamera(Camera camera)
        {
            if (!_cameras.Contains(camera))
                _cameras.Add(camera);

            if (!enabled)
                Enable();
        }

        public void RemoveCamera(Camera camera)
        {
            if (_cameras.Contains(camera))
                _cameras.Remove(camera);

            CheckCameras();
        }

        public void Enable()
        {
            enabled = true;

            CheckCameras();
        }

        public void Disable()
        {
            Debug.LogError("StaticCulling.Disable");

            EnableRenderers();

            enabled = false;
        }

        private Dictionary<string, bool> isVisibleDict = new Dictionary<string, bool>();

        internal bool IsRenderersVisible(List<string> ids, string tag, string sceneId,string path)
        {
            if (this.gameObject.activeInHierarchy == false) return true;
            if (this.enabled == false) return true;
            if (IsInsideCullingArea == false) return false;
            if (currentNode == null) return false;

            string key = RendererId.GetId(currentNode) + sceneId;
            if (isVisibleDict.ContainsKey(key))
            {
                return isVisibleDict[key];
            }

            //if (visibleRendererIds.Count == 0) return true;
            bool isVisible = false;
            foreach(var id in ids)
            {
                if (visibleRendererIds.Contains(id))
                {
                    isVisible=true;
                    break;
                }
            }
            if (isVisible)
            {
                Debug.LogError($"IsRenderersVisible({tag}) currentNode:{currentNode.name} sceneId:{sceneId} isVisible:{isVisible} ids:{ids.Count} visibleIds:{visibleRendererIds.Count} tag:{path} ");
            }

            if (isVisibleDict.ContainsKey(key))
            {
                Debug.LogError($"IsRenderersVisible({tag}) isVisibleDict.ContainsKey(key) currentNode:{currentNode.name} sceneId:{sceneId} isVisible:{isVisible} ids:{ids.Count} visibleIds:{visibleRendererIds.Count} tag:{path} ");
            }
            else
            {
                isVisibleDict.Add(key, isVisible);
            }

            //Debug.LogError($"IsRenderersVisible isVisible:{isVisible} IsInsideCullingArea:{IsInsideCullingArea} ids:{ids.Count} visibleRendererIds:{visibleRendererIds.Count} tag:{tag} ");
            return isVisible;
        }
    }
}
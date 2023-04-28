using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class BinaryTreeNode : MonoBehaviour
    {
        public Bounds GetBounds()
        {
            return new Bounds(center, size);
        }    

        [SerializeField] private BinaryTreeNode _left;
        [SerializeField] private BinaryTreeNode _right;
        [SerializeField] private List<MeshRenderer> _visibleRenderers = new List<MeshRenderer>();
        [SerializeField] private List<string> _visibleRendererIds = new List<string>();

        public List<string> visibleRendererIds
        {
            get
            {
                return _visibleRendererIds;
            }
        }

        public List<string> GetAllIds()
        {
            DictList<string> ids = new DictList<string>();
            foreach(var id in _visibleRendererIds)
            {
                ids.Add(id);
            }
            if (_left)
            {
                var idsLeft=_left.GetAllIds();
                ids.AddRange(idsLeft);
            }
            if (_right)
            {
                var idsRight = _right.GetAllIds();
                ids.AddRange(idsRight);
            }
            return ids.NewList();
        }

        public List<MeshRenderer> GetAllRenderers()
        {
            DictList<MeshRenderer> ids = new DictList<MeshRenderer>();
            foreach (var id in _visibleRenderers)
            {
                ids.Add(id);
            }
            if (_left)
            {
                var idsLeft = _left.GetAllRenderers();
                ids.AddRange(idsLeft);
            }
            if (_right)
            {
                var idsRight = _right.GetAllRenderers();
                ids.AddRange(idsRight);
            }
            return ids.NewList();
        }

        public void GetRendererIds()
        {
            if (_visibleRenderers.Count > 0)
            {
                _visibleRendererIds.Clear();
                foreach (var renderer in _visibleRenderers)
                {
                    string id = RendererId.GetId(renderer.gameObject);
                    _visibleRendererIds.Add(id);
                }
                if (_visibleRendererIds.Count == 0)
                {
                    Debug.LogError($"BinaryTreeNode.GetRendererIds _visibleRendererIds.Count == 0 _visibleRenderers:{_visibleRenderers.Count} name:{this.name}");
                }
            }
            
            if (_left)
            {
                _left.GetRendererIds();
            }
            if (_right)
            {
                _right.GetRendererIds();
            }
        }

        internal List<BinaryTreeNode> FindBinaryTreeNodes(string id)
        {
            List<BinaryTreeNode> nodes = new List<BinaryTreeNode>();
            if (_visibleRendererIds.Contains(id))
            {
                nodes.Add(this);
            }
            if (_left)
            {
                var nodes1=_left.FindBinaryTreeNodes(id);
                nodes.AddRange(nodes1);
            }
            if (_right)
            {
                var nodes2 = _right.FindBinaryTreeNodes(id);
                nodes.AddRange(nodes2);
            }
            return nodes;
        }

        internal List<BinaryTreeNode> GetAllLeafNodes()
        {
            List<BinaryTreeNode> nodes = new List<BinaryTreeNode>();
            if (_left == null && _right == null)
            {
                nodes.Add(this);
            }
            if (_left)
            {
                var nodes1 = _left.GetAllLeafNodes();
                nodes.AddRange(nodes1);
            }
            if (_right)
            {
                var nodes2 = _right.GetAllLeafNodes();
                nodes.AddRange(nodes2);
            }
            return nodes;
        }

        public void ClearRenderers()
        {
            _visibleRenderers.Clear();
            if (_left)
            {
                _left.ClearRenderers();
            }
            if (_right)
            {
                _right.ClearRenderers();
            }
        }

        public void RemoveEmptyRenderers()
        {
            _visibleRenderers.RemoveAll(i => i == null);

            if (_left)
            {
                _left.RemoveEmptyRenderers();
            }
            if (_right)
            {
                _right.RemoveEmptyRenderers();
            }
        }

        public void GetRenderers()
        {
            if (_visibleRendererIds.Count > 0)
            {
                _visibleRenderers.Clear();
                foreach (var id in _visibleRendererIds)
                {
                    var renderer = IdDictionary.GetRenderer(id, false);
                    if (renderer != null)
                    {
                        _visibleRenderers.Add(renderer);
                    }
                }
            }

            if (_left == null && _right == null)
            {

            }

            if (_left)
            {
                _left.GetRenderers();
            }
            if (_right)
            {
                _right.GetRenderers();
            }
        }


        public Vector3 center
        {
            get
            {
                return transform.position;
            }
        }
        public Vector3 size
        {
            get
            {
                return transform.lossyScale;
            }
        }

        public BinaryTreeNode left
        {
            get
            {
                return _left;
            }
        }
        public BinaryTreeNode right
        {
            get
            {
                return _right;
            }
        }

        public List<MeshRenderer> visibleRenderers
        {
            get
            {
                return _visibleRenderers;
            }
        }
        public bool isLeaf
        {
            get
            {
                return left == null && right == null;
            }
        }

        public static int nodeCount = 0;


        public static BinaryTreeNode Create(Vector3 center, Vector3 size, string name = "TreeNode")
        {
            nodeCount++;
            //Debug.Log($"BinaryTreeNode.Create nodeCount:{nodeCount}");
            GameObject go = new GameObject(name+"_"+ nodeCount);

            go.transform.position = center;
            go.transform.localScale = size;

            return go.AddComponent<BinaryTreeNode>();
        }


        public void SetChilds(BinaryTreeNode left, BinaryTreeNode right)
        {
            _left = left;
            _right = right;

            if(left != null)
                _left.transform.parent = transform;

            if(right != null)
                _right.transform.parent = transform;
        }

        public void SetVisibleRenderers(List<MeshRenderer> renderers)
        {
            _visibleRenderers = renderers;
        }


        public void AddVisibleRenderers(Caster caster)
        {
            _visibleRenderers.AddRange(caster.visibleRenderers);
        }

        public void DistinctVisibleRenderersList()
        {
            _visibleRenderers = _visibleRenderers.Distinct().ToList();
        }

        [ContextMenu("SetToSelected")]
        private void SetToSelected()
        {
            IsDrawGizmos = true;
            StaticCulling.Instance.SetToSelected(this);
        }

        [ContextMenu("NotDrawGizmos")]
        private void NotDrawGizmos()
        {
            IsDrawGizmos = false;
        }

        [ContextMenu("EnableDrawGizmos")]
        private void EnableDrawGizmos()
        {
            IsDrawGizmos = true;
        }

        //private void OnDrawGizmos()
        //{
        //     BinaryTreeUtil.DrawGizmos(tree, Color.blue);
        //}

        public static bool IsDrawGizmos = false;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (IsDrawGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(this.center, this.size);

                Handles.color = Color.blue;
                foreach (var obj in visibleRenderers)
                {
                    if (obj == null) continue;
                    Bounds bounds = obj.GetComponent<MeshRenderer>().bounds;
                    Handles.DrawWireCube(bounds.center, bounds.size);
                }
            }
        }
#endif

        [ContextMenu("ShowCasters")]
        public void ShowCasters()
        {
            this.transform.ClearChildren();

            BinaryTreeNode node = this;
            Bounds bounds = new Bounds(node.center, node.size);
            var _cellSize = StaticCulling._cellSizeS;
            //_cellSize /= 2f;
            //_cellSize /= 3f;
            int countX = Mathf.CeilToInt((bounds.max.x - bounds.min.x) / _cellSize) ;
            int countY = Mathf.CeilToInt((bounds.max.y - bounds.min.y) / _cellSize) ;
            int countZ = Mathf.CeilToInt((bounds.max.z - bounds.min.z) / _cellSize) ;

            Debug.Log($"_cellSize:{_cellSize} x:{(bounds.max.x - bounds.min.x) / _cellSize} countX:{countX} countY:{countY} countZ:{countZ}");
            //_castersUnit.Add(new Caster[countX + 1, countY + 1, countZ + 1]);
            int count = 0;
            for (int x = 0; x <= countX; x++)
                for (int y = 0; y <= countY; y++)
                    for (int z = 0; z <= countZ; z++)
                    {
                        count++;
                        //GameObject go = new GameObject($"Caster[{x},{y},{z}]");
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        go.name = $"Caster[{count:000}][{x},{y},{z}]";
                        Caster caster = go.AddComponent<Caster>();

                        caster.transform.position = bounds.min + new Vector3(x, y, z) * _cellSize;
                        caster.transform.localScale = Vector3.one * _cellSize /10f;

                        go.transform.SetParent(this.transform);

                        //_casters.Add(caster);
                        //_castersUnit[i][x, y, z] = caster;
                    }
        }
    }
}

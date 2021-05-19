using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class BinaryTreeNode : MonoBehaviour
    {
        [SerializeField] private BinaryTreeNode _left;
        [SerializeField] private BinaryTreeNode _right;
        [SerializeField] private List<Renderer> _visibleRenderers = new List<Renderer>();

        //[SerializeField]
        //private BinaryTreeNodeData Data = new BinaryTreeNodeData();

        public bool IsLeft = true;

        public BinaryTreeNodeData GetData()
        {
            BinaryTreeNodeData Data = new BinaryTreeNodeData();

            Data.Id = this.name;
            Data.ParentId = this.transform.parent.name;

            Data.center = new V3(this.center);
            Data.size = new V3(this.size);
            Data.isLeft = this.IsLeft;

            foreach (var item in this._visibleRenderers)
            {
                //Data.AddId(item.GetInstanceID());
                Data.AddId(item.GetRendererID());
            }

            return Data;
        }

        public void SetData(BinaryTreeNodeData data)
        {
            //this.Data = data;
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
            set
            {
                _left = value;
            }
        }
        public BinaryTreeNode right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }

        public List<Renderer> visibleRenderers
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

        public static int AllNodeCount = 0;


        public static BinaryTreeNode Create(Vector3 center, Vector3 size, string name = "TreeNode")
        {
            AllNodeCount++;
            //if (name == "TreeNode")
            //{
            //    name = "TreeNode" + AllNodeCount;
            //}
            GameObject go = new GameObject(name);

            go.transform.position = center;
            go.transform.localScale = size;

            return go.AddComponent<BinaryTreeNode>();
        }


        public void SetChilds(BinaryTreeNode left, BinaryTreeNode right)
        {
            _left = left;
            _right = right;

            if (left != null)
            {
                _left.IsLeft = true;
                _left.transform.parent = transform;
            }

            if (right != null)

            {
                _left.IsLeft = false;
                _right.transform.parent = transform;
            }
        }

        public void SetVisibleRenderers(List<Renderer> renderers)
        {
            _visibleRenderers = renderers;
        }

        public List<GameObject> casters = new List<GameObject>();

        public static bool ShowNodeCasters=false;

        public void AddVisibleRenderer(Renderer render)
        {
            _visibleRenderers.Add(render);
        }

        public void AddVisibleRenderers(Caster caster)
        {
            if(ShowNodeCasters)
            {
                // //ShowCaster，显示投射器位置
                // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // sphere.transform.position = caster.transform.position;
                // //sphere.transform.localScale = new Vector3(1, 1, 1);
                // sphere.transform.SetParent(this.transform);
                // casters.Add(sphere);
                // Renderer render = sphere.GetComponent<Renderer>();
                // render.enabled = false;

                GameObject goNew=GameObject.Instantiate(caster.gameObject);
                goNew.transform.SetParent(this.transform);
                casters.Add(goNew);
                Caster casterNew=goNew.GetComponent<Caster>();
                casterNew.WireSphereScale = 0.25f;
            }


            _visibleRenderers.AddRange(caster.visibleRenderers);
            //Data.visibleRenderers.AddRange(caster.visibleRenderersIDs);
            //Data.AddRendererIds(caster.visibleRenderersIDs);
        }

        private string originalName = "";

        /// <summary>
        /// 去掉重复的Renderer
        /// </summary>
        public void DistinctVisibleRenderersList()
        {
            if(string.IsNullOrEmpty(originalName))
            {
                originalName = this.name;
            }
            _visibleRenderers = _visibleRenderers.Distinct().ToList();
            //Data.visibleRenderers = Data.visibleRenderers.Distinct().ToList();
            this.gameObject.name = $"{originalName}|{_visibleRenderers.Count}|{this.transform.lossyScale}|{this.transform.localScale}";
        }

        //-------------------------------cww add

        [ContextMenu("HideBounds")]
        public void HideBounds()
        {
            if(cube!=null){
                GameObject.DestroyImmediate(cube);
            }

            if(left)
                left.HideBounds();
            if(right)
                right.HideBounds();
        }

        GameObject cube;

        [ContextMenu("ShowBounds")]
        public void ShowBounds()
        {
            if(cube)
            {
                GameObject.DestroyImmediate(cube);
            }
            cube=GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position=this.transform.position;
            cube.transform.localScale=this.transform.lossyScale;
            cube.transform.SetParent(this.transform);
            MeshRenderer renderer=cube.GetComponent<MeshRenderer>();
            renderer.enabled=false;

            if(left)
                left.ShowBounds();
            if(right)
                right.ShowBounds();
        }

        public void GetAllChildrenNode(List<BinaryTreeNode> nodes)
        {
            nodes.Add(this);
            if (left != null)
            {
                left.GetAllChildrenNode(nodes);
            }
            if (right != null)
            {
                right.GetAllChildrenNode(nodes);
            }
        }

#if UNITY_EDITOR
        //private void OnDrawGizmosSelected()
        //{
        //    if (_visibleRenderers == null || _visibleRenderers.Count == 0)
        //        return;
        //    //Gizmos.DrawMesh()
        //    UnityEditor.Handles.Label(this.transform.position,_visibleRenderers.Count.ToString());
        //    // foreach(var render in _visibleRenderers)
        //    // {
        //    //     //Debug.DrawLine(this.transform.position)
        //    //     Gizmos.DrawLine(this.transform.position,render.transform.position);
        //    // }
        //}
#endif
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class BinaryTree : MonoBehaviour
    {
        [SerializeField]
        private BinaryTreeNode _rootNode;
        public BinaryTreeNode rootNode
        {
            get
            {
                return _rootNode;
            }
            set
            {
                _rootNode = value;
            }
        }

        public void CreateTree(Caster[,,] casters)
        {
            int countX = casters.GetLength(0);
            int countY = casters.GetLength(1);
            int countZ = casters.GetLength(2);

            Vector3 min = casters[0, 0, 0].transform.position;
            Vector3 max = casters[countX - 1, countY - 1, countZ - 1].transform.position;

            BinaryTreeNode.AllNodeCount = 0;//cww_add
            _rootNode = BinaryTreeNode.Create((min + max) / 2, max - min,"RootNode");
            _rootNode.transform.parent = transform;

            CreateChilds(_rootNode, casters);
        }

        private void CreateChilds(BinaryTreeNode parent, Caster[,,] casters, int depth = 1)
        {
            int countX = casters.GetLength(0);
            int countY = casters.GetLength(1);
            int countZ = casters.GetLength(2);

            //一个Cell(TreeNode)内显示的模型是这个Cell的Cube的8个角上的Caster采样的结果
            if (countX <= 2 && countY <= 2 && countZ <= 2) 
            {
                for (int i = 0; i < countX; i++)
                    for (int c = 0; c < countY; c++)
                        for (int j = 0; j < countZ; j++)
                            if (casters[i, c, j].visibleRenderers != null)
                                parent.AddVisibleRenderers(casters[i, c, j]);//将可见的物体(Renders)加到TreeNode里面
                parent.DistinctVisibleRenderersList();
                return;//结束递归
            }

            string pName = parent.name;
            if (depth == 1)
            {
                pName = "";
            }

            //关键：将casters分到left和right两份
            ComputeChildsData(casters, out BinaryTreeNode leftNode, out BinaryTreeNode rightNode,
                out Caster[,,] leftNodeCasters, out Caster[,,] rightNodeCasters, pName);

            parent.SetChilds(leftNode, rightNode);

            //递归
            CreateChilds(leftNode, leftNodeCasters, depth + 1);
            CreateChilds(rightNode, rightNodeCasters, depth + 1);
        }

        private void ComputeChildsData(Caster[,,] casters, out BinaryTreeNode leftNode, out BinaryTreeNode rightNode,
            out Caster[,,] leftNodeCasters, out Caster[,,] rightNodeCasters,string pName)
        {
            int lastX = casters.GetLength(0) - 1;
            int lastY = casters.GetLength(1) - 1;
            int lastZ = casters.GetLength(2) - 1;

            Vector3 lMin = Vector3.zero, lMax = Vector3.zero, rMin = Vector3.zero, rMax = Vector3.zero;

            if (lastX >= lastY && lastX >= lastZ && lastX > 1)
            {
                int middle = lastX / 2;

                leftNodeCasters = new Caster[middle + 1, lastY + 1, lastZ + 1];
                rightNodeCasters = new Caster[lastX - middle + 1, lastY + 1, lastZ + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[middle, lastY, lastZ].transform.position;

                rMin = casters[middle, 0, 0].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (i <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (i >= middle)
                                rightNodeCasters[i - middle, c, j] = casters[i, c, j];
                        }
            }
            else if (lastY >= lastX && lastY >= lastZ && lastY > 1)
            {
                int middle = lastY / 2;

                leftNodeCasters = new Caster[lastX + 1, middle + 1, lastZ + 1];
                rightNodeCasters = new Caster[lastX + 1, lastY - middle + 1, lastZ + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[lastX, middle, lastZ].transform.position;

                rMin = casters[0, middle, 0].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (c <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (c >= middle)
                                rightNodeCasters[i, c - middle, j] = casters[i, c, j];
                        }
            }
            else if (lastZ >= lastX && lastZ >= lastY && lastZ > 1)
            {
                int middle = lastZ / 2;

                leftNodeCasters = new Caster[lastX + 1, lastY + 1, middle + 1];
                rightNodeCasters = new Caster[lastX + 1, lastY + 1, lastZ - middle + 1];

                lMin = casters[0, 0, 0].transform.position;
                lMax = casters[lastX, lastY, middle].transform.position;

                rMin = casters[0, 0, middle].transform.position;
                rMax = casters[lastX, lastY, lastZ].transform.position;

                for (int i = 0; i <= lastX; i++)
                    for (int c = 0; c <= lastY; c++)
                        for (int j = 0; j <= lastZ; j++)
                        {
                            if (j <= middle)
                                leftNodeCasters[i, c, j] = casters[i, c, j];

                            if (j >= middle)
                                rightNodeCasters[i, c, j - middle] = casters[i, c, j];
                        }
            }
            else
                throw new System.Exception("Ex");

            leftNode = BinaryTreeNode.Create((lMin + lMax) / 2, (lMax - lMin),pName+"L");
            rightNode = BinaryTreeNode.Create((rMin + rMax) / 2, (rMax - rMin), pName + "R");
        }


        /// <summary>
        /// 根据坐标找到TreeNode
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public BinaryTreeNode GetNode(Vector3 point)
        {
            return GetNode(_rootNode, point);
        }

        /// <summary>
        /// 根据坐标找到TreeNode
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public BinaryTreeNode GetNode(BinaryTreeNode parent, Vector3 point)
        {
            if (parent.isLeaf)
                return parent;//递归结束

            if (parent.left != null)
            {
                if (new Bounds(parent.left.center, parent.left.size).Contains(point))
                    return GetNode(parent.left, point);//递归
            }

            return GetNode(parent.right, point);//递归
        }

        // ------------------------------------------cww add
        [ContextMenu("ShowBounds")]
        public void ShowBounds()
        {
            rootNode.ShowBounds();
        }

        [ContextMenu("HideBounds")]
        public void HideBounds()
        {
            rootNode.HideBounds();
        }

        public List<BinaryTreeNode> GetAllChildrenNode()
        {
            List<BinaryTreeNode> nodes = new List<BinaryTreeNode>();
            rootNode.GetAllChildrenNode(nodes);
            return nodes;
        }

        //private string DataPath = "Assets/Performance Tools/AdvancedCullingSystem/Resources/";

        //public string DataFileName = "";

        //public int TreeCount = 0;



        public BinaryTreeData GetData()
        {
            ProgressBarHelper.DisplayProgressBar("GetData","GetData",0);
            Data = new BinaryTreeData();
            var nodes = GetAllChildrenNode();
            int count=nodes.Count;
            for (int i = 0; i < count; i++)
            {
                float p=i*1f/count;
                ProgressBarHelper.DisplayProgressBar("GetData",$"{i}/{count}",p);
                BinaryTreeNode item = nodes[i];
                Data.Add(item.GetData());
            }
            ProgressBarHelper.ClearProgressBar();
            return Data;
        }

        public BinaryTreeData Data;

        //[ContextMenu("LoadData")]
        //public void LoadData()
        //{
        //    if (DataFileName == "")
        //    {
        //        DataFileName = "BinaryTree" + TreeCount;
        //    }
        //    Data = Resources.Load<BinaryTreeData>(DataFileName + "");
        //    if (Data == null)
        //    {
        //        Debug.LogError("Data == null:"+ DataFileName + ".asset");
        //        return;
        //    }
        //}


        [ContextMenu("CreateTree")]
        public void CreateTree()
        {
            Dictionary<string, BinaryTreeNodeData> nodeDict = new Dictionary<string, BinaryTreeNodeData>();
            foreach (var item in Data.NodeDatas)
            {
                nodeDict.Add(item.Id, item);
            }

            BinaryTreeNodeData root = null;
            foreach (var item in Data.NodeDatas)
            {
                if (nodeDict.ContainsKey(item.ParentId))
                {
                    BinaryTreeNodeData parent = nodeDict[item.ParentId];
                    if (item.isLeft)
                    {
                        parent.left = item;
                    }
                    else
                    {
                        parent.right = item;
                    }
                }
                else
                {
                    root = item;
                }
            }

            for (int i = 0; i < this.transform.childCount; i++)
            {
                GameObject.DestroyImmediate(this.transform.GetChild(i).gameObject);
            }

            _rootNode=CreateTreeNodes(root, this.transform);
        }

        private BinaryTreeNode CreateTreeNodes(BinaryTreeNodeData root,Transform tParent)
        {
            if (root == null) return null;
            GameObject go = new GameObject(root.Id);
            Transform t = go.transform;
            t.position = root.center.GetVector3();
            t.localScale = root.size.GetVector3();
            t.SetParent(tParent);
            BinaryTreeNode node=go.AddComponent<BinaryTreeNode>();
            node.SetData(root);

            if (root.Ids != null)
            {
                foreach (var id in root.Ids)
                {
                    Renderer renderer = RendererHelper.GetRendererById(id);
                    if (renderer != null)
                    {
                        node.AddVisibleRenderer(renderer);
                    }
                    else
                    {
                        Debug.LogError("Render not Found:" + id);
                    }
                }
            }


            node.left = CreateTreeNodes(root.left, t);
            node.right = CreateTreeNodes(root.right, t);

            return node;
        }


        public static void SaveAsset<T>(T asset,string path,string name) where T : ScriptableObject
        {
            Debug.Log("SaveAsset:"+ path + name + ".asset");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(asset, path + name + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}

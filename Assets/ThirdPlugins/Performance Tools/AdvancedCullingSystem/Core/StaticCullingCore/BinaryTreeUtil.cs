using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public static class BinaryTreeUtil
    {
        /// <summary>
        /// 优化二分树
        /// </summary>
        /// <param name="binaryTree"></param>
        public static void OptimizeTree(BinaryTree binaryTree)
        {
            int depth1 = 0;

            var list1 = binaryTree.GetAllChildrenNode();//cww_add
            GetMaxDepth(binaryTree.rootNode, ref depth1);
            

            for (int i = 0; i <= (depth1 + 1); i++) //执行次数为最深的depth，合并一次再合并一次
                LinkNodesIfEquals(binaryTree, binaryTree.rootNode);

            var list2 = binaryTree.GetAllChildrenNode();//cww_add
            int depth2 = 0;

            int totalVisibleCount = 0;
            int nodeCount = 0;
            
            for(int i=0;i<list2.Count;i++)
            {
                var item = list2[i];
                if (item.visibleRenderers.Count > 0)
                {
                    nodeCount++;
                    totalVisibleCount += item.visibleRenderers.Count;
                }
            }

            GetMaxDepth(binaryTree.rootNode, ref depth2);

            StaticCullingTestData testData = StaticCullingTestReporter.Current;
            testData.TreeDepth = depth1;
            testData.TreeNodeCount = list2.Count;
            testData.OptimizeCount = list1.Count - list2.Count;
            testData.TotalVisible = totalVisibleCount;
            testData.VisibleLeaf = nodeCount;
            if(nodeCount!=0)
                testData.AvgVisible = totalVisibleCount / nodeCount;
            testData.OptimizeTree = true;
            Debug.Log($"OptimizeTree depth={depth1},count={list1.Count}-{list2.Count}={list1.Count-list2.Count},visibleCount={totalVisibleCount}/{nodeCount}={testData.AvgVisible}");//cww_add
            //depth:11,list1:2159,list2:325
        }

        /// <summary>
        /// 如果left和right的可见物体一样，则合并
        /// </summary>
        /// <param name="binaryTree"></param>
        /// <param name="node"></param>
        public static void LinkNodesIfEquals(BinaryTree binaryTree, BinaryTreeNode node)
        {
            if ((node.left != null && node.left.isLeaf) && (node.right != null && node.right.isLeaf))
            {
                if (node.left.visibleRenderers.Count != node.right.visibleRenderers.Count)//数量不同
                    return;

                for (int i = 0; i < node.left.visibleRenderers.Count; i++)
                    if (!node.right.visibleRenderers.Contains(node.left.visibleRenderers[i]))//可见的物体不同
                        return;

                //如果left和right显示的模型都一样，那就合并(删除left和right，将可见的物体(Renderers)设置给parent的node）
                node.SetVisibleRenderers(node.left.visibleRenderers);

                Object.DestroyImmediate(node.left.gameObject);
                Object.DestroyImmediate(node.right.gameObject);

                node.SetChilds(null, null);

                return;
            }

            //递归
            if (node.left != null)
                if (node.left.left != null || node.left.right != null)
                    LinkNodesIfEquals(binaryTree, node.left);

            if (node.right != null)
                if (node.right.left != null || node.right.right != null)
                    LinkNodesIfEquals(binaryTree, node.right);
        }


        public static void GetMaxDepth(BinaryTreeNode node, ref int depth, int currentDepth = 0)
        {
            if (node.isLeaf)
            {
                if (currentDepth > depth)
                    depth = currentDepth;

                return;
            }

            if (node.left != null)
                GetMaxDepth(node.left, ref depth, currentDepth + 1);

            if (node.right != null)
                GetMaxDepth(node.right, ref depth, currentDepth + 1);
        }


        public static void DrawGizmos(BinaryTree binaryTree, Color color)
        {
            if (binaryTree.rootNode == null)
                return;

            DrawGizmosRecursively(binaryTree.rootNode, color);
        }

        public static void DrawGizmosRecursively(BinaryTreeNode parent, Color color)
        {
            if (parent.isLeaf)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(parent.center, parent.size);

                return;
            }

            if (parent.left != null)
                DrawGizmosRecursively(parent.left, color);

            if (parent.right != null)
                DrawGizmosRecursively(parent.right, color);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public static class AreaTreeHelper
{
    public static AreaTreeNode GetNodeByRenderer(MeshRenderer renderer)
    {
        if (render2NodeDict.ContainsKey(renderer))
        {
            return render2NodeDict[renderer];
        }
        return null;
    }

    public static AreaTreeNode GetNodeByCombined(MeshRenderer renderer)
    {
        if (combined2NodeDict.ContainsKey(renderer))
        {
            return combined2NodeDict[renderer];
        }
        return null;
    }

    public static AreaTreeNode GetNodeById(string rendererId)
    {
        if (renderId2NodeDict.ContainsKey(rendererId))
        {
            return renderId2NodeDict[rendererId];
        }
        return null;
    }

    public static List<AreaTreeNode> GetNodesByChildrens(params RendererId[] rIds)
    {
        List<AreaTreeNode> list = new List<AreaTreeNode>();
        foreach (var rId in rIds)
        {
            foreach (var id in rId.childrenIds)
            {
                if (renderId2NodeDict.ContainsKey(id))
                {
                    AreaTreeNode node = renderId2NodeDict[id];
                    if (!list.Contains(node))
                    {
                        list.Add(node);
                    }
                }
                else
                {
                    Debug.LogError($"GetNodesByIds not found node ! id:{id}");
                }
            }
        }
        return list;
    }

    public static List<AreaTreeNode> GetNodesByIds(List<string> ids)
    {
        List<AreaTreeNode> list = new List<AreaTreeNode>();
        foreach(var id in ids)
        {
            if (renderId2NodeDict.ContainsKey(id))
            {
                AreaTreeNode node= renderId2NodeDict[id];
                if(!list.Contains(node))
                {
                    list.Add(node);
                }
            }
            else
            {
                Debug.LogError($"GetNodesByIds not found node ! id:{id}");
            }
        }
        return list;
    }

    public static Dictionary<MeshRenderer,AreaTreeNode> render2NodeDict=new Dictionary<MeshRenderer, AreaTreeNode>();

    public static Dictionary<string, AreaTreeNode> renderId2NodeDict = new Dictionary<string, AreaTreeNode>();

    public static Dictionary<MeshRenderer, AreaTreeNode> combined2NodeDict = new Dictionary<MeshRenderer, AreaTreeNode>();

    public static void ClearDict()
    {
        render2NodeDict.Clear();
        combined2NodeDict.Clear();
        renderId2NodeDict.Clear();
        Debug.LogWarning($"ClearDict render2NodeDict:{AreaTreeHelper.render2NodeDict.Count}");
    }

    public static void RegisterRenderer(MeshRenderer render, AreaTreeNode newNode)
    {
        if (render == null) return;
        if (render2NodeDict.ContainsKey(render))
        {
            var node = render2NodeDict[render];
            if (node == null)
            {
                //Debug.LogWarning($"Node1被删除了 render:{render},node1:{AreaTreeHelper.render2NodeDict[render]},node2:{this}");
                render2NodeDict[render] = newNode;
            }
            else if (node == newNode)
            {

            }
            else
            {
                Debug.LogError($"RegisterRenderer 模型重复在不同的Node里 render:{render.name},node1:{render2NodeDict[render].name},node2:{newNode.name}");
                render2NodeDict[render] = newNode;
            }
        }
        else
        {
            render2NodeDict.Add(render, newNode);
        }
    }

    public static bool RegisterRendererId(string rendererId,int id, AreaTreeNode newNode)
    {
        if (rendererId == null) return false;
        if (renderId2NodeDict.ContainsKey(rendererId))
        {
            var node = renderId2NodeDict[rendererId];
            if (node == null)
            {
                //Debug.LogWarning($"Node1被删除了 render:{render},node1:{AreaTreeHelper.render2NodeDict[render]},node2:{this}");
                renderId2NodeDict[rendererId] = newNode;
            }
            else if (node == newNode)
            {

            }
            else
            {
                Debug.LogError($"RegisterRendererId 模型重复在不同的Node里 tree:{newNode.tree.name} renderId:{rendererId},index:{id} node1:{renderId2NodeDict[rendererId].name},node2:{newNode.name}");
                renderId2NodeDict[rendererId] = newNode;
                return false;
            }
        }
        else
        {
            renderId2NodeDict.Add(rendererId, newNode);
        }
        return true;
    }

    public static void AddNodeDictItem_Renderers(IEnumerable<MeshRenderer> renderers,AreaTreeNode node)
    {
        foreach (var render in renderers)
        {
            //renderer.gameObject.AddComponent<MeshCollider>();
            if (render2NodeDict.ContainsKey(render))
            {
                
            }
            else
            {
                render2NodeDict.Add(render, node);
            }
        }
    }

    public static void AddNodeDictItem_Combined(IEnumerable<MeshRenderer> renderers, AreaTreeNode node)
    {
        foreach (var render in renderers)
        {
            //renderer.gameObject.AddComponent<MeshCollider>();
            if (combined2NodeDict.ContainsKey(render))
            {
                
            }
            else
            {
                combined2NodeDict.Add(render, node);
            }
        }
    }

    public static List<GameObject> CubePrefabs = new List<GameObject>();

    public static AreaTreeManager InitCubePrefab()
    {
        AreaTreeManager areaTreeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (areaTreeManager)
        {
            AreaTreeHelper.CubePrefabs = areaTreeManager.CubePrefabs;
        }
        return areaTreeManager;
    }

    public static GameObject CreateBoundsCube(Bounds bounds,string n,Transform parent,int prefabId)
    {
        // Debug.Log($"CreateBoundsCube bounds:{bounds} name:{n} parent:{parent}");
        InitCubePrefab();

        if(CubePrefabs==null||CubePrefabs.Count==0){
            var cubePrefab=GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubePrefab.SetActive(false);
        }
        GameObject cube=GameObject.Instantiate(CubePrefabs[prefabId]);
        cube.AddComponent<BoundsBox>();
        cube.SetActive(true);
        cube.name=n;
        cube.transform.position=bounds.center;
        cube.transform.localScale=bounds.size;
        cube.transform.SetParent(parent);
        return cube;
    }

    public static List<Transform> GetAllTransforms(this Transform transform)
    {
        List<Transform> result = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            result.Add(child);
            result.AddRange(GetAllTransforms(child));
        }
        return result;
    }
}
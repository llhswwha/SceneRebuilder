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
        Debug.LogError($"AreaTreeHelper GetNodeById ==null rendererId:{rendererId} renderId2NodeDict:{renderId2NodeDict.Count} ");
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
                    Debug.LogError($"AreaTreeHelper GetNodesByIds not found node ! id:{id}");
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
                Debug.LogError($"AreaTreeHelper GetNodesByIds not found node ! id:{id}");
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
        Debug.LogWarning($"AreaTreeHelper ClearDict render2NodeDict:{AreaTreeHelper.render2NodeDict.Count}");
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
                Debug.LogError($"AreaTreeHelper RegisterRenderer 模型重复在不同的Node里 render:{render.name},node1:{render2NodeDict[render].name},node2:{newNode.name}");
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
        //Debug.LogError($"AreaTreeHelper RegisterRendererId rendererId:{rendererId} id:{id} node:{newNode} tree:{newNode.tree} path:{TransformHelper.GetPath(newNode.transform)} dict:{renderId2NodeDict.Count}");
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
                Debug.LogError($"AreaTreeHelper RegisterRendererId 模型重复在不同的Node里 tree:{newNode.tree.name} renderId:{rendererId},index:{id} node1:{renderId2NodeDict[rendererId].name},node2:{newNode.name}");
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
        AreaTreeManager areaTreeManager = AreaTreeManager.Instance;
        CubePrefabs = areaTreeManager.CubePrefabs;
        return areaTreeManager;

        //CubePrefabs = AreaTreeManager.Instance.CubePrefabs;
        //return AreaTreeManager.Instance;
    }

    public static GameObject CreateBoundsCube(Bounds bounds,string n,Transform parent,int prefabId)
    {
        InitCubePrefab();

        //if (CubePrefabs.Count > 0)
        //{
        //    Debug.Log($"CreateBoundsCube1 bounds:{bounds} name:{n} parent:{parent} prefabId:{prefabId} CubePrefabs:{CubePrefabs.Count} CubePrefabs0:{CubePrefabs[0]}");
        //}
        //else
        //{
        //    Debug.Log($"CreateBoundsCube2 bounds:{bounds} name:{n} parent:{parent} prefabId:{prefabId} CubePrefabs:{CubePrefabs.Count}");
        //}

        GameObject prefab = null;
        if (CubePrefabs==null||CubePrefabs.Count==0 || prefabId>= CubePrefabs.Count || prefabId<0)
        {
            prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.name = "BoundsCube_"+prefabId;
            prefab.SetActive(false);
            CubePrefabs.Add(prefab);
        }
        else
        {
            prefab = CubePrefabs[prefabId];

            if (prefab == null)
            {
                prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                prefab.name = "BoundsCube_" + prefabId;
                prefab.SetActive(false);
                CubePrefabs[prefabId] = prefab;
            }
        }

        //Debug.Log($"CubePrefabs:{CubePrefabs.Count}");
        //for (int i = 0; i < CubePrefabs.Count; i++)
        //{
        //    GameObject cprefab = CubePrefabs[i];
        //    Debug.Log($"[{i}]cprefab:{cprefab}");
        //}

        if (prefab == null)
        {
            Debug.LogError($"AreaTreeHelper CreateBoundsCube prefab == null bounds:{bounds} name:{n} parent:{parent} prefabId:{prefabId} CubePrefabs:{CubePrefabs.Count}");
            return null;
        }
        

        GameObject cube=GameObject.Instantiate(prefab);

        if (cube == null)
        {
            Debug.LogError($"AreaTreeHelper CreateBoundsCube cube == null bounds:{bounds} name:{n} parent:{parent} prefabId:{prefabId} CubePrefabs:{CubePrefabs.Count}");
            return null;
        }

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
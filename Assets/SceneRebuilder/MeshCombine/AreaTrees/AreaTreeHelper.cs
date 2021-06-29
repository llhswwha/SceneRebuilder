using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public static class AreaTreeHelper
{
    
    public static Dictionary<MeshRenderer,AreaTreeNode> render2NodeDict=new Dictionary<MeshRenderer, AreaTreeNode>();

    public static Dictionary<MeshRenderer, AreaTreeNode> combined2NodeDict = new Dictionary<MeshRenderer, AreaTreeNode>();

    public static void ClearDict()
    {
        render2NodeDict.Clear();
        combined2NodeDict.Clear();
    }

    public static void AddNodeDictItem_Renderers(IEnumerable<MeshRenderer> renderers,AreaTreeNode node)
    {
        foreach (var render in renderers)
        {
            //renderer.gameObject.AddComponent<MeshCollider>();
            if (AreaTreeHelper.render2NodeDict.ContainsKey(render))
            {
                
            }
            else
            {
                AreaTreeHelper.render2NodeDict.Add(render, node);
            }
        }
    }

    public static void AddNodeDictItem_Combined(IEnumerable<MeshRenderer> renderers, AreaTreeNode node)
    {
        foreach (var render in renderers)
        {
            //renderer.gameObject.AddComponent<MeshCollider>();
            if (AreaTreeHelper.combined2NodeDict.ContainsKey(render))
            {
                
            }
            else
            {
                AreaTreeHelper.combined2NodeDict.Add(render, node);
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
        Debug.LogError($"CreateBoundsCube bounds:{bounds} name:{n} parent:{parent}");
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
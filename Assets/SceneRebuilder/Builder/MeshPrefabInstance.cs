using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPrefabInstance : MonoBehaviour, IGameObject
{
    public static Dictionary<GameObject, List<MeshPrefabInstance>> InstancesDict = new Dictionary<GameObject, List<MeshPrefabInstance>>();

    public static void InitInstancesDict()
    {
        InstancesDict = new Dictionary<GameObject, List<MeshPrefabInstance>>();
        var insList = GameObject.FindObjectsOfType<MeshPrefabInstance>();
        foreach (var item in insList)
        {
            if (item == null) continue;
            GameObject pre = item.PrefabGo;
            if (pre == null) continue;
            if (!InstancesDict.ContainsKey(pre))
            {
                InstancesDict.Add(pre, new List<MeshPrefabInstance>());
            }
            InstancesDict[pre].Add(item);
        }
        Debug.Log($"InitInstancesDict InstancesDict:{InstancesDict.Count} insList:{insList.Length}");
    }

    public static List<MeshPrefabInstance> FindInstances(MeshPrefabInstance go)
    {
        if (go.PrefabGo == null)
        {
            Debug.LogError($"MeshPrefabInstance.FindInstances this.PrefabGo == null go:{go}");
        }

        if(!InstancesDict.ContainsKey(go.PrefabGo))
            InitInstancesDict();

        List<MeshPrefabInstance> instances = new List<MeshPrefabInstance>();

        if (InstancesDict.ContainsKey(go.PrefabGo))
        {
            var insList = InstancesDict[go.PrefabGo];
            foreach (var item in insList)
            {
                if (item == go) continue;
                if (item.PrefabGo == go.PrefabGo)
                {
                    instances.Add(item);
                }
            }
            Debug.Log($"FindInstances instances:{instances.Count} insList:{insList.Count}");
        }
        else
        {
            var insList = GameObject.FindObjectsOfType<MeshPrefabInstance>();
            foreach (var item in insList)
            {
                if (item == go) continue;
                if (item.PrefabGo == go.PrefabGo)
                {
                    instances.Add(item);
                }
            }
            Debug.Log($"FindInstances instances:{instances.Count} insList:{insList.Length}");
        }
       
        return instances;
    }

    public bool IsPrefab = false;

    public GameObject PrefabGo = null;

    public List<MeshPrefabInstance> FindInstances()
    {
        return FindInstances(this);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetName()
    {
        return name;
    }

    public void LoadMesh()
    {
        string id = RendererId.GetId(PrefabGo);
        var mesh=EditorHelper.LoadResoucesMesh(id);
        LoadMesh(mesh);
    }

    public void LoadMesh(Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError($"MeshPrefabInstance.LoadMesh mesh == null:{this.gameObject}");
            return;
        }
        MeshFilter mf = gameObject.AddMissingComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer mr = gameObject.AddMissingComponent<MeshRenderer>();
    }

    internal void RemomveMesh()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.sharedMesh = null;

        //GameObject.DestroyImmediate(mf);
        //MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        //GameObject.DestroyImmediate(mr);
    }

    public Mesh GetMesh()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        return mf.sharedMesh;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPrefabInstance : MonoBehaviour, IGameObject
{
    public bool IsPrefab = false;

    public GameObject PrefabGo = null;

    public List<MeshPrefabInstance> FindInstances()
    {
        if (this.PrefabGo == null)
        {
            Debug.LogError("MeshPrefabInstance.FindInstances this.PrefabGo == null");
        }
        List<MeshPrefabInstance> instances = new List<MeshPrefabInstance>();
        var insList = GameObject.FindObjectsOfType<MeshPrefabInstance>();
        foreach(var item in insList)
        {
            if (item == this) continue;
            if (item.PrefabGo == this.PrefabGo)
            {
                instances.Add(item);
            }
        }
        Debug.Log($"FindInstances instances:{instances.Count} insList:{insList.Length}");
        return instances;
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

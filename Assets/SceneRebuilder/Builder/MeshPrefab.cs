using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPrefab : MonoBehaviour
{
    public GameObject Prefab;

    public GameObject Instance;

    public GameObject TargetScene;

    public List<MeshNode> TargetItems = new List<MeshNode>();

    public List<MeshNode> TypeItems = new List<MeshNode>();

    public MeshNode meshNode;

    public string MeshTypeName;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (Prefab == null)
        {
            Prefab = this.gameObject;
        }

        Instance = MeshHelper.CopyGO(Prefab);
        Instance.SetActive(false);
        Instance.transform.parent = this.transform;

        meshNode = Instance.GetComponent<MeshNode>();
        if (meshNode == null)
        {
            meshNode = Instance.AddComponent<MeshNode>();
        }
        meshNode.Init();

        if (TargetScene != null)
        {
            MeshNode meshNode2 = TargetScene.GetComponent<MeshNode>();
            if (meshNode2 == null)
            {
                meshNode2 = TargetScene.AddComponent<MeshNode>();
            }
        }
        MeshTypeName = MeshType.GetTypeName(Instance.name);
    }

    public MeshNode[] nodes;

    [ContextMenu("Search")]
    public void Search()
    {
        TargetItems.Clear();
        TypeItems.Clear();
        nodes = TargetScene.GetComponentsInChildren<MeshNode>();
        foreach (var node in nodes)
        {
            if (meshNode.IsSameMesh(node))
            {
                TargetItems.Add(node);
            }
            if(node.MeshTypeName == MeshTypeName)
            {
                TypeItems.Add(node);
            }
        }

        TargetCount = TargetItems.Count;
    }

    public int TargetCount = 0;

    //[ContextMenu("ReplaceTargets")]
    //public void ReplaceTargets()
    //{
    //    Search();
    //    //ReplaceList(TargetItems);
    //    StartCoroutine(StartReplaceList(TargetItems));
    //}

    //[ContextMenu("ReplaceTypes")]
    //public void ReplaceTypes()
    //{
    //    Search();
    //    //ReplaceList(TypeItems);
    //    StartCoroutine(StartReplaceList(TypeItems));
    //}

    private void ReplaceList(List<MeshNode> list)
    {
        foreach (var item in list)
        {
            MeshHelper.ReplaceByPrefab(item.gameObject, Prefab);
        }
        //StartCoroutine(MeshHelper.ReplaceByPrefabEx(oldObj, Prefab));
    }

    //private IEnumerator StartReplaceList(List<MeshNode> list)
    //{
    //    DateTime start = DateTime.Now;
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        MeshNode item = list[i];
    //        yield return MeshHelper.ReplaceByPrefabEx(item.gameObject, Prefab, meshNode.GetMeshKey1(), item.GetMeshKey2());
    //        //yield return null;
    //    }
    //    Debug.LogWarning(string.Format("StartReplaceList 类型:{0},数量:{1},用时:{2:F1}s", meshNode.MeshTypeName, list.Count, (DateTime.Now - start).TotalSeconds));
    //}

    //public IEnumerator StartReplaceTargets()
    //{
    //    Search();
    //    yield return StartReplaceList(TargetItems);
    //}

    //public IEnumerator StartReplaceTypes()
    //{
    //    Search();
    //    yield return StartReplaceList(TypeItems);
    //}
}

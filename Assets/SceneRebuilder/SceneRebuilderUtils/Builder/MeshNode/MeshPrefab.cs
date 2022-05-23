using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPrefab : MonoBehaviour
{
    public GameObject Prefab;

    public GameObject Instance;

    public GameObject TargetScene;

    public List<InnerMeshNode> TargetItems = new List<InnerMeshNode>();

    public List<InnerMeshNode> TypeItems = new List<InnerMeshNode>();

    public InnerMeshNode meshNode;

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

        Instance = InnerMeshHelper.CopyGO(Prefab);
        Instance.SetActive(false);
        Instance.transform.parent = this.transform;

        meshNode = Instance.GetComponent<InnerMeshNode>();
        if (meshNode == null)
        {
            meshNode = Instance.AddComponent<InnerMeshNode>();
        }
        meshNode.Init();

        if (TargetScene != null)
        {
            InnerMeshNode meshNode2 = TargetScene.GetComponent<InnerMeshNode>();
            if (meshNode2 == null)
            {
                meshNode2 = TargetScene.AddComponent<InnerMeshNode>();
            }
        }
        MeshTypeName = MeshType.GetTypeName(Instance.name);
    }

    public InnerMeshNode[] nodes;

    [ContextMenu("Search")]
    public void Search()
    {
        TargetItems.Clear();
        TypeItems.Clear();
        nodes = TargetScene.GetComponentsInChildren<InnerMeshNode>();
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

    private void ReplaceList(List<InnerMeshNode> list)
    {
        foreach (var item in list)
        {
            InnerMeshHelper.ReplaceByPrefab(item.gameObject, Prefab);
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

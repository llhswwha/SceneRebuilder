using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcRTAlignJobPrefab
{
    public int Id;

    public PrefabInfo PrefabInfo;

    public int VerticesCount;

    public MeshFilterList List;

    public void RemoveMeshFilter(MeshPoints mf)
    {
        List.Remove(mf);
    }

    public void AddInstance(GameObject instance)
    {
        PrefabInfo.AddInstance(instance);
    }

    public static Dictionary<int, AcRTAlignJobPrefab> Items = new Dictionary<int, AcRTAlignJobPrefab>();

    public static void Clear()
    {
        Items.Clear();
    }

    public static void AddItem(int id, PrefabInfo preInfo, MeshFilterList mfl)
    {
        //Debug.Log($"AcRTAlignJobPrefab.AddItem jobId:{jobId},meshFilter:{mfl.Count}");
        AcRTAlignJobPrefab item = new AcRTAlignJobPrefab();
        item.Id = id;
        item.PrefabInfo = preInfo;
        item.List = mfl;
        Items.Add(id, item);
    }

    public static AcRTAlignJobPrefab GetItem(int id)
    {
        if (Items.ContainsKey(id))
        {
            return Items[id];
        }
        Debug.LogError("AcRTAlignJobPrefab.GetItem Not Found jobId:" + id);
        return null;
    }
}


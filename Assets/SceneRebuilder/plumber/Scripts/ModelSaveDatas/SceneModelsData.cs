using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneSaveData 
{
    public PipeGenerateArg Arg;

    public List<PipeLineSaveData> PipeLines = new List<PipeLineSaveData>();

    public List<PipeElbowSaveData> PipeElbows = new List<PipeElbowSaveData>();

    public List<PipeTeeSaveData> PipeTees = new List<PipeTeeSaveData>();

    public List<PipeReducerSaveData> PipeReducers = new List<PipeReducerSaveData>();

    public List<PipeFlangeSaveData> PipeFlanges = new List<PipeFlangeSaveData>();

    public List<PipeWeldoletSaveData> PipeWeldolets = new List<PipeWeldoletSaveData>();

    public List<MeshPrefabSaveData> MeshPrefabs = new List<MeshPrefabSaveData>();

    internal List<PipeWeldSaveData> GetWelds()
    {
        List<PipeWeldSaveData> welds = new List<PipeWeldSaveData>();
        foreach (var item in PipeLines)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        foreach (var item in PipeElbows)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        foreach (var item in PipeTees)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        foreach (var item in PipeReducers)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        foreach (var item in PipeFlanges)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        foreach (var item in PipeWeldolets)
        {
            if (item.PipeWelds == null) continue;
            welds.AddRange(item.PipeWelds);
        }
        return welds;
    }

    public int GetPipeModelCount()
    {
        return PipeLines.Count + PipeElbows.Count + PipeTees.Count + PipeReducers.Count + PipeFlanges.Count + PipeWeldolets.Count;
    }

    private Dictionary<GameObject,MeshPrefabSaveData> MeshPrefabDict = new Dictionary<GameObject, MeshPrefabSaveData>();

    public string GetCountString()
    {
        return $" Prefabs:{MeshPrefabs.Count} Line:{PipeLines.Count} Elbow:{PipeElbows.Count} Tee:{PipeTees.Count} Reducer:{PipeReducers.Count} Flange:{PipeFlanges.Count} Weldolet:{PipeWeldolets.Count}";
    }

    internal void AddData_PipeLine(PipeLineSaveData pipeLineSaveData)
    {
        PipeLines.Add(pipeLineSaveData);
        //if(pipeLineSaveData.PipeWelds!=null)
        //    foreach(var item in pipeLineSaveData.PipeWelds)
        //    {
        //        if(item.Name== "HH-J-0GMA-11-006 9")
        //        {
        //            Debug.LogError($"AddData_PipeLine item:{item}");
        //        }
        //    }
    }

    internal void AddData_PipeElbow(PipeElbowSaveData pipeLineSaveData)
    {
        PipeElbows.Add(pipeLineSaveData);
    }

    internal void AddData_PipeTee(PipeTeeSaveData pipeLineSaveData)
    {
        PipeTees.Add(pipeLineSaveData);
    }

    internal void AddData_PipeReducer(PipeReducerSaveData pipeLineSaveData)
    {
        PipeReducers.Add(pipeLineSaveData);
    }

    internal void AddData_PipeFlange(PipeFlangeSaveData pipeLineSaveData)
    {
        PipeFlanges.Add(pipeLineSaveData);
    }

    internal void AddData_PipeWeldolet(PipeWeldoletSaveData pipeLineSaveData)
    {
        PipeWeldolets.Add(pipeLineSaveData);
    }

    internal void AddData_MeshInstance(MeshPrefabInstance instance)
    {
        //PipeWeldolets.Add(pipeLineSaveData);
        if (!MeshPrefabDict.ContainsKey(instance.PrefabGo))
        {
            MeshPrefabSaveData data = new MeshPrefabSaveData(instance);
            MeshPrefabDict.Add(instance.PrefabGo, data);
            MeshPrefabs.Add(data);

            var mesh = EditorHelper.LoadResoucesMesh(data.prefabId);
            if (mesh == null)
            {
#if UNITY_EDITOR
                EditorHelper.SaveMeshAssetResource(instance.PrefabGo);
#endif
                Debug.LogWarning($"AddData_MeshInstance id:{data.prefabId} obj:{instance.gameObject} prefab:{instance.PrefabGo}");
            }
        }
        MeshPrefabDict[instance.PrefabGo].AddInstance(instance);
    }

    public override string ToString()
    {
        return $"SaveData Lines:{PipeLines.Count} Elbows:{PipeElbows.Count}";
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneSaveData 
{
    public List<PipeLineSaveData> PipeLines = new List<PipeLineSaveData>();

    public List<PipeElbowSaveData> PipeElbows = new List<PipeElbowSaveData>(); 

    internal void AddData(PipeLineSaveData pipeLineSaveData)
    {
        PipeLines.Add(pipeLineSaveData);
    }

    internal void AddData(PipeElbowSaveData pipeLineSaveData)
    {
        PipeElbows.Add(pipeLineSaveData);
    }

    public override string ToString()
    {
        return $"SaveData Lines:{PipeLines.Count} Elbows:{PipeElbows.Count}";
    }
}

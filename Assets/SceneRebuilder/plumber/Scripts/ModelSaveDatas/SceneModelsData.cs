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

    internal void AddData_PipeLine(PipeLineSaveData pipeLineSaveData)
    {
        PipeLines.Add(pipeLineSaveData);
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

    public override string ToString()
    {
        return $"SaveData Lines:{PipeLines.Count} Elbows:{PipeElbows.Count}";
    }
}

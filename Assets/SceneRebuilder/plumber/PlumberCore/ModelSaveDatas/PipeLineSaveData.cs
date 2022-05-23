using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class PipeLineSaveData : MeshModelSaveData
{
    public PipeLineData Data;

    public override bool IsSuccess()
    {
        return Data.IsGetInfoSuccess;
    }
}

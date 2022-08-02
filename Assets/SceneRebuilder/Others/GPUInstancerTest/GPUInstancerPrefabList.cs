using GPUInstancer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GPUInstancerPrefabList 
{
    public GPUInstancerPrefab Prefab;
    public List<string> InstanceIds = new List<string>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class AreaTreeNodeSetting
{
    public int MinLevel = 10;

    public int MaxLevel = 20;

    public int MaxRenderCount = 1000;

    public int MinRenderCount = 100;

    public int MaxVertexCount = 100;
}
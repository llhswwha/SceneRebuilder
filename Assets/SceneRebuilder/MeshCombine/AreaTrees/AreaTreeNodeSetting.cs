using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class AreaTreeNodeSetting
{
    public int MinLevel = 2;

    public int MaxLevel = 20;

    public int MaxRenderCount = 2000;

    public int MinRenderCount = 100;

    public int MaxVertexCount = 100;
}
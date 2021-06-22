using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererId : Behaviour
{
    public string Id;

    public MeshRenderer renderer;

    internal void Init()
    {
        Id = Guid.NewGuid().ToString();
    }
}

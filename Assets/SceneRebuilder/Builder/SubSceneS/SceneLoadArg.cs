using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneLoadArg 
{
    public string name;

    public string path;

    public int index;

    public override string ToString()
    {
        return $"name:{name} index:{index} path:{path}";
    }
}

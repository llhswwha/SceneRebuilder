using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneLoadArg 
{
    public string name;

    public string path;

    public int index=-1;

    public override string ToString()
    {
        return $"name:{name} index:{index} path:{path}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_LOD : SubScene_Part
{
    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_Out0_" + contentType;
        }
    }
}


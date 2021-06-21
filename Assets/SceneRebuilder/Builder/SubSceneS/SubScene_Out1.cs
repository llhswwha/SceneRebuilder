using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_Out1 : SubScene_Part
{
    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_Out1";
        }
    }
}

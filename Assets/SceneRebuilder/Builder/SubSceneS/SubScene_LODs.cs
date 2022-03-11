using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_LODs : SubScene_Part
{
    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_Out0_" + contentType;
        }
    }

    public override int UnLoadGosM()
    {
        SubScene_Ref.BeforeUnloadScene(this.gameObject);
        int r= base.UnLoadGosM();
        return r;
    }

    public override void DestroyScene()
    {
        SubScene_Ref.ClearRefs(this.gameObject);
        base.DestroyScene();
    }
}


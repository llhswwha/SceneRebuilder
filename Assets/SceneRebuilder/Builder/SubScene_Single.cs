using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class SubScene_Single : SubScene_Base
{
    public void DestroyOldBounds()
    {
        var components = this.GetComponentsInChildren<BoundsBox>();//In Out0 Out1
        foreach (var c in components)
        {
            GameObject.DestroyImmediate(c.gameObject);//重新创建，把之前的删除
        }
    }

    public override void DestroyBoundsBox()
    {
        base.DestroyBoundsBox();
        DestroyOldBounds();
    }
}

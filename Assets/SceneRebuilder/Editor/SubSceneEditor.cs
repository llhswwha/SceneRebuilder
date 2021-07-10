using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(SubScene_Base))]
public class SubSceneEditor : BaseEditor<SubScene_Base>
{
    public override void OnToolLayout(SubScene_Base item)
    {
        base.OnToolLayout(item);
    }
}

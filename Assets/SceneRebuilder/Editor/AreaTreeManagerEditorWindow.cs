using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaTreeManager))]
public class AreaTreeManagerEditorWindow : BaseFoldoutEditor<AreaTreeManager>
{
    public override void OnToolLayout(AreaTreeManager item)
    {
        base.OnToolLayout(item);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaTreeNode))]
public class AreaTreeNodeEditor : BaseEditor<AreaTreeNode>
{
    public override void OnToolLayout(AreaTreeNode item)
    {
        base.OnToolLayout(item);
    }
}

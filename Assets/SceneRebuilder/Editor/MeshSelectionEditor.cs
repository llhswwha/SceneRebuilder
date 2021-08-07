using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshSelection))]
public class MeshSelectionEditor : BaseFoldoutEditor<MeshSelection>
{

    public override void OnToolLayout(MeshSelection item)
    {
        //item.
        EditorGUILayout.ObjectField(item.testRId, typeof(RendererId))/* as RendererId*/;
        EditorGUILayout.TextField(item.testRIdText);
        if (GUILayout.Button("TestSelectObjectByRId"))
        {
            item.TestSelectObjectByRId();
        }
        base.OnToolLayout(item);
    }
}

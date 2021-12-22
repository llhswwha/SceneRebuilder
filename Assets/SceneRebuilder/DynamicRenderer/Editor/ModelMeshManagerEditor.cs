using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModelMeshManager))]
public class ModelMeshManagerEditor : BaseFoldoutEditor<ModelMeshManager>
{
    public override void OnEnable()
    {
        base.OnEnable();

        targetT.InitPrefixNames();
    }

    public override void OnToolLayout(ModelMeshManager item)
    {
        if(GUILayout.Button("GetAllRenderers"))
        {
            item.GetAllRenderers();
        }
        if (GUILayout.Button("GetPrefixNames"))
        {
            item.GetPrefixNames();
        }
    }

    private void DrawModelClassDict(ModelClassDict<MeshRenderer> Dict)
    {

    }
}

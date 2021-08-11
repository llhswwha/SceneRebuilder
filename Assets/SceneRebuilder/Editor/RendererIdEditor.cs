using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RendererId))]
public class RendererIdEditor : BaseEditor<RendererId>
{
    private GameObject parent;
    public override void OnEnable()
    {
        base.OnEnable();
        IdDictionary.InitInfos();
        parent = targetT.GetParent();
    }
    public override void OnToolLayout(RendererId item)
    {
        base.OnToolLayout(item);

        GUILayout.Label("Id:"+item.Id);
        GUILayout.Label("Children:"+item.childrenIds.Count);
        if (parent != null)
        {
            if (GUILayout.Button(parent.name))
            {
                EditorHelper.SelectObject(parent);
            }
        }
        else
        {
            if (GUILayout.Button(targetT.parentId))
            {
                //EditorHelper.SelectObject(parent);
            }
        }

        if (GUILayout.Button("Clear"))
        {
            item.Clear();
        }

        if (GUILayout.Button("RemoveEmpty"))
        {
            MeshHelper.RemoveEmptyObjects(item.gameObject);
        }

        if (GUILayout.Button("DecreaseEmpty"))
        {
            MeshHelper.DecreaseEmptyGroup(item.gameObject);
        }
    }
}

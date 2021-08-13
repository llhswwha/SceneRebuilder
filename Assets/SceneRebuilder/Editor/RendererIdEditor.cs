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
        if (parent == null)
        {
            IdDictionary.InitInfos();
            parent = targetT.GetParent();
        }
        
    }
    public override void OnToolLayout(RendererId item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
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
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InitIds"))
        {
            IdDictionary.InitInfos();
        }
        if (GUILayout.Button("Clear"))
        {
            item.Clear();
        }
        if (GUILayout.Button("Unpack"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
        }
        if (GUILayout.Button("Center"))
        {
            MeshHelper.CenterPivot(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("RemoveEmpty"))
        {
            MeshHelper.RemoveEmptyObjects(item.gameObject);
        }

        if (GUILayout.Button("DecreaseEmpty"))
        {
            MeshHelper.DecreaseEmptyGroup(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy_Split"))
        {
            MeshCombineHelper.SplitByMaterials(item.gameObject);
        }
        if (GUILayout.Button("Split"))
        {
            MeshCombineHelper.SplitByMaterials(item.gameObject);
        }
        if (GUILayout.Button("Combine"))
        {
            //MeshCombineHelper.CombineEx(new MeshCombineArg(item.gameObject), MeshCombineMode.OneMesh);
            MeshCombiner.Instance.CombineToOne(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetLOD"))
        {
            LODHelper.CreateLODs(item.gameObject);
        }
        if (GUILayout.Button("DoorLOD"))
        {
            LODHelper.SetDoorLOD(item.gameObject);
        }
        if (GUILayout.Button("CopyDoorA1"))
        {
            DoorHelper.CopyDoorA(item.gameObject,false);
        }
        if (GUILayout.Button("CopyDoorA2"))
        {
            DoorHelper.CopyDoorA(item.gameObject,true);
        }
        EditorGUILayout.EndHorizontal();
    }

}

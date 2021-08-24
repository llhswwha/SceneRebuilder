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
        //if (parent == null)
        //{
        //    IdDictionary.InitInfos();
        //    parent = targetT.GetParent();
        //}
        
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
        if (GUILayout.Button("ResetT"))
        {
            item.ResetTransform();
        }
        if (GUILayout.Button("ResetPos"))
        {
            item.ResetPos();
        }
        if (GUILayout.Button("CenterParent"))
        {
            MeshHelper.CenterPivot(item.gameObject);
        }
        if (GUILayout.Button("CenterMesh"))
        {
            //MeshHelper.CenterMesh(item.gameObject);
        }
        if (GUILayout.Button("ShowRenderers"))
        {
            item.ShowRenderers();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("NewId"))
        {
            item.NewId();
        }
        if (GUILayout.Button("Init"))
        {
            item.Init();
        }
        if (GUILayout.Button("UpdateIds"))
        {
            //item.NewId();
            RendererId.InitIds(item.gameObject,true);
        }
        if (GUILayout.Button("InitIdDict"))
        {
            IdDictionary.InitInfos();
        }
        if (GUILayout.Button("ClearIds"))
        {
            item.ClearIds();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ClearScripts"))
        {
            item.ClearScripts();
        }
        if (GUILayout.Button("ClearLODs"))
        {
            item.ClearLODs();
        }
        
        if (GUILayout.Button("Unpack"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("RmEmpty"))
        {
            MeshHelper.RemoveEmptyObjects(item.gameObject);
        }

        if (GUILayout.Button("DcsEmpty"))
        {
            MeshHelper.DecreaseEmptyGroup(item.gameObject);
        }

        if (GUILayout.Button("InitNodes"))
        {
            MeshNode.InitNodes(item.gameObject);
        }
        if (GUILayout.Button("Align"))
        {
            PrefabInstanceBuilder.Instance.AcRTAlignJobs(item.gameObject, true);
        }
        if (GUILayout.Button("AlignEx"))
        {
            PrefabInstanceBuilder.Instance.AcRTAlignJobsEx(item.gameObject, true);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy_Split"))
        {
            var newGo1 = MeshHelper.CopyGO(item.gameObject);
           
            var newGo1Split=MeshCombineHelper.SplitByMaterials(newGo1,true);
            newGo1Split.name += "_Center";
            float dis1 = MeshHelper.GetVertexDistanceEx(item.gameObject.transform, newGo1Split.transform);

            var newGo2 = MeshHelper.CopyGO(item.gameObject);
            var newGo2Split = MeshCombineHelper.SplitByMaterials(newGo2, false);
            float dis2 = MeshHelper.GetVertexDistanceEx(item.gameObject.transform, newGo2Split.transform);

            float dis3 = MeshHelper.GetVertexDistanceEx(newGo1Split.transform, newGo2Split.transform);

            Debug.Log($"dis1:{dis1},dis2:{dis2},dis3:{dis3}");
        }
        if (GUILayout.Button("Split"))
        {
            MeshCombineHelper.SplitByMaterials(item.gameObject,false);
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
            var obj=LODHelper.SetDoorLOD(item.gameObject);
            EditorHelper.SelectObject(obj);

        }
        if (GUILayout.Button("CpDoor1"))
        {
            DoorHelper.CopyDoorA(item.gameObject,false, false);
        }
        if (GUILayout.Button("CpDoor2"))
        {
            DoorHelper.CopyDoorA(item.gameObject,true,false);
        }

        if (GUILayout.Button("Prepare"))
        {
            DoorHelper.Prepare(item.gameObject);
        }
        if (GUILayout.Button("PivotPart"))
        {
            DoorHelper.SetDoorPartPivot(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();
    }

}

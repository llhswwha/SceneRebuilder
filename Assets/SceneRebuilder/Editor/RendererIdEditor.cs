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
        if (GUILayout.Button("CopyDoorA"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
            var childCount = item.transform.childCount;
            if (childCount == 2)
            {
                var door1 = item.transform.GetChild(0);
                var door2 = item.transform.GetChild(1);

                var scale1 = door1.localScale;
                var scale2 = door2.localScale;
                if(scale1==Vector3.one || scale2 == Vector3.one)
                {
                    if (scale2 == Vector3.one)
                    {
                        var tmp = door1;
                        door1 = door2;
                        door2 = tmp;
                    }

                    GameObject newDoor2=MeshHelper.CopyGO(door1.gameObject);
                    newDoor2.transform.localScale = new Vector3(-1, 1, 1);
                    newDoor2.transform.position = door2.transform.position;
                    float distance1 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor1", false);
                    //door2.gameObject.SetActive(false);
                    //MeshAlignHelper.AcRTAlignJob(newDoor2, door2.gameObject);

                    MeshComparer.Instance.AcRTAlignJob(newDoor2, door2.gameObject);

                    float distance2 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor2", false);
                    Debug.Log($"distance1:{distance1} distance2:{distance2}");

                    newDoor2.name = door2.name + "_New";
                    GameObject.DestroyImmediate(door2.gameObject);
                }
                else
                {
                    Debug.LogError($"RendererIdEditor.CopyDoorA scale1!=Vector3.one && scale2 != Vector3.one scale1:{scale1} scale2:{scale2}");
                }
            }
            else
            {
                Debug.LogError("RendererIdEditor.CopyDoorA childCount =!= 2");
            }
        }
        EditorGUILayout.EndHorizontal();
    }

}

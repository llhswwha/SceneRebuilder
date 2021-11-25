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
        parent = targetT.GetCurrentParent();
    }



    public override void OnToolLayout(RendererId item)
    {
        base.OnToolLayout(item);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Id:" + item.Id);
        GUILayout.Label("Children:" + item.childrenIds.Count);
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
        if(GUILayout.Button("ZeroParent"))
        {
            MeshHelper.ZeroParent(item.gameObject);
        }
        //if (GUILayout.Button("CenterMesh"))
        //{
        //    //MeshHelper.CenterMesh(item.gameObject);
        //}

        if (GUILayout.Button("RootParent"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
            item.gameObject.transform.SetParent(null);
        }
        if (GUILayout.Button("UpParent"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
            if(item.gameObject.transform.parent!=null)
                item.gameObject.transform.SetParent(item.gameObject.transform.parent.parent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowRenderers"))
        {
            item.ShowRenderers();
        }
        if (GUILayout.Button("ShowAll"))
        {
            TransformHelper.ShowAll(item.gameObject);
        }
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
            RendererId.InitIds(item.gameObject, true);
        }
        if (GUILayout.Button("InitIdDict"))
        {
            IdDictionary.InitInfos();
        }
        if (GUILayout.Button("ClearIds"))
        {
            item.ClearIds();
        }
         if (GUILayout.Button("ClearUps"))
        {
            //item.ClearIds();
             var ids = item.gameObject.GetComponentsInChildren<RendererUpdateInfo>(true);
            foreach(var id in ids)
            {
                GameObject.DestroyImmediate(id);
            }
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
        if (GUILayout.Button("GetSize"))
        {
            var minMax = MeshHelper.GetMinMax(item.gameObject);
            Debug.Log("size:" + minMax[2]);
        }
        if (GUILayout.Button("RemoveNew"))
        {
            MeshHelper.RemoveNew(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("IsGroup"))
        {
            bool result = MeshHelper.IsEmptyGroup(item.transform,true);
            Debug.Log($"IsEmptyGroup {result}");
        }
        if (GUILayout.Button("IsSameName"))
        {
            bool result = MeshHelper.IsSameNameGroup(item.transform);
            Debug.Log($"IsSame {result}");
        }
        if (GUILayout.Button("IsEmpty"))
        {
            bool result = MeshHelper.IsEmptyObject(item.transform,true);
            Debug.Log($"IsEmpty {result}");
        }
        if (GUILayout.Button("IsEmptyChild"))
        {
            bool result = MeshHelper.IsEmptyChildObject(item.transform);
            Debug.Log($"IsEmptyChild {result}");
        }
        if (GUILayout.Button("IsDetail"))
        {
            bool result1 = RendererManager.Instance.IsDetail(item.gameObject);
            bool result2 = RendererManager.Instance.IsDetail2(item.gameObject);
            Debug.Log($"IsDetail result1:{result1},result2:{result2}");
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

            var newGo1Split = MeshCombineHelper.SplitByMaterials(newGo1, true);
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
            MeshCombineHelper.SplitByMaterials(item.gameObject, false);
        }
        if (GUILayout.Button("Combine"))
        {
            //MeshCombineHelper.CombineEx(new MeshCombineArg(item.gameObject), MeshCombineMode.OneMesh);
            MeshCombiner.Instance.CombineToOne(item.gameObject);
        }
        if (GUILayout.Button("PivotPart1"))
        {
            DoorHelper.SetDoorPartPivot(item.gameObject, false);
        }
        if (GUILayout.Button("PivotPart2"))
        {
            DoorHelper.SetDoorPartPivot(item.gameObject, true);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("MoveLODs"))
        {
            //LODHelper.SetChildrenLODName(item.gameObject);
            Transform lodRoot = LODHelper.GetFloorLODsRoot(item.transform);
            item.transform.SetParent(lodRoot);
        }
        if (GUILayout.Button("SetLODName"))
        {
            LODHelper.SetChildrenLODName(item.gameObject);
        }
        if (GUILayout.Button("SetLOD"))
        {
            LODHelper.CreateLODs(item.gameObject);
        }
        if (GUILayout.Button("DoorLOD"))
        {
            var obj = LODHelper.SetDoorLOD(item.gameObject);
            EditorHelper.SelectObject(obj);

        }
        if (GUILayout.Button("CpDoor1"))
        {
            DoorHelper.CopyDoorA(item.gameObject, false, false);
        }
        if (GUILayout.Button("CpDoor2"))
        {
            DoorHelper.CopyDoorA(item.gameObject, true, false);
        }

        if (GUILayout.Button("Prepare"))
        {
            DoorHelper.Prepare(item.gameObject);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetAsDetails"))
        {
            var meshRendererInfos = MeshRendererInfo.InitRenderers(item.gameObject);
            foreach (MeshRendererInfo info in meshRendererInfos)
            {
                info.AddType(MeshRendererType.Detail);
            }
            Debug.Log($"SetAsDetails renderers:{meshRendererInfos.Length}");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HideWalls"))
        {
            SetObjectsActive(item.gameObject, "Walls", false);
        }
        if (GUILayout.Button("ShowWalls"))
        {
            SetObjectsActive(item.gameObject, "Walls", true);
        }
        if (GUILayout.Button("RemovePipe"))
        {
            DestroyObject(item.gameObject, new List<string>() { "HH_","CC_","JQ_","JG_","SG_"});
        }
        if (GUILayout.Button("UpdateCollider"))
        {
            BoxCollider boxCollider = item.gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                GameObject.DestroyImmediate(boxCollider);
            }
            ColliderHelper.CreateBoxCollider(item.gameObject, false);
        }
        

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("AddPreName"))
        {


            TransformHelper.AddPreName(item.transform);
        }
        if (GUILayout.Button("AddPreNames"))
        {
            //string pName = item.gameObject.name + "_";
            TransformHelper.AddPreNames(item.transform);
        }
        if (GUILayout.Button("RemoveChildren"))
        {
            TransformHelper.RemoveChildren(item.transform);
        }
        //if (GUILayout.Button("ReGroup1_After"))
        //{
        //    TransformHelper.ReGroup1_After(item.transform);
        //}

        if (GUILayout.Button("ReGroup_Before"))
        {

            TransformHelper.ReGroupBefore(item.transform);
        }
        if (GUILayout.Button("ReGroup_After"))
        {

            TransformHelper.ReGroupAfter(item.transform);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("FindDwfGo"))
        {
            GameObject goDwf = FindDwfGo(item.gameObject);
        }
        if (GUILayout.Button("AlignDwf"))
        {
            GameObject goDwf = FindDwfGo(item.gameObject);
            if (goDwf == null) return;

            EditorHelper.UnpackPrefab(item.gameObject);
            EditorHelper.UnpackPrefab(goDwf.gameObject);
            MeshHelper.CenterPivot(item.gameObject);
            MeshHelper.CenterPivot(goDwf.gameObject);

            goDwf.transform.rotation = Quaternion.Euler(-90, 0, -90);
            MeshAlignmentManager.Instance.DoAlign(goDwf, item.gameObject);

            
        }
        if (GUILayout.Button("FindGeometry"))
        {
            GameObject goDwf = FindDwfGo(item.gameObject);
            if (goDwf == null) return;

            List<Transform> geoList = FindGeometries(item.gameObject);

           var ts2 = goDwf.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < geoList.Count; i++)
            {
                Transform geo = geoList[i];
                List<Transform> result = FindListByName(ts2, geo.name);
                if (result.Count == 1)
                {
                    //result[0].SetParent(geo.parent);
                }
            }
        }
        if (GUILayout.Button("SetParent"))
        {
            GameObject goDwf = FindDwfGo(item.gameObject);
            if (goDwf == null) return;

            EditorHelper.UnpackPrefab(item.gameObject);
            EditorHelper.UnpackPrefab(goDwf.gameObject);

            List<Transform> geoList = FindGeometries(item.gameObject);

            var ts2 = goDwf.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < geoList.Count; i++)
            {
                Transform geo = geoList[i];
                List<Transform> result = FindListByName(ts2, geo.name);
                if (result.Count == 1)
                {
                    result[0].SetParent(geo.parent);
                    GameObject.DestroyImmediate(geo.gameObject);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public static List<Transform> FindListByName(Transform[] ts2,string name)
    {
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            if (t2.name == name)
            {
                result.Add(t2);
            }
        }
        if (result.Count == 1)
        {
            Debug.Log($"FindListByName name:{name} result:{result[0]}");
        }
        else
        {
            Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
        }
        return result;
    }

    public static List<Transform> FindGeometries(GameObject go)
    {
        List<Transform> geoList = new List<Transform>();
        var ts1 = go.GetComponentsInChildren<Transform>(true);
        foreach (var t in ts1)
        {
            if (t.name.StartsWith("Geometry"))
            {
                var p = t.parent;
                if (p.childCount == 1)
                {
                    geoList.Add(p);
                }
                else
                {
                    Debug.LogWarning($"p.childCount != 1 p:{p.name}");
                    t.name = p.name;
                }
            }
        }

        Debug.Log($"geoList:{geoList.Count}");
        return geoList;
    }

    public static GameObject FindDwfGo(GameObject go)
    {
        string dwfName = go.name + "_dwf";
        GameObject goDwf = GameObject.Find(dwfName);
        Debug.Log($"FindDwfGo dwfName:{dwfName} goDwf:{goDwf}");
        return goDwf;
    }


    public static void SetObjectsActive(GameObject go,string key,bool isActive)
    {
        Transform[] ts = go.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.name.Contains(key))
            {
                t.gameObject.SetActive(isActive);
            }
        }
    }

    public static void DestroyObject(GameObject go, List<string> keys)
    {
        Transform[] ts = go.GetComponentsInChildren<Transform>(true);
        int count = 0;
        foreach (Transform t in ts)
        {
            if (t == null) continue;
            if (IsStartWith(t.name, keys))
            {
                GameObject.DestroyImmediate(t.gameObject);
                count++;
            }
        }
        Debug.LogError($"DestroyObject count:{count}");
    }

    private static bool IsStartWith(string name, List<string> keys)
    {
        foreach (var key in keys)
        {
            if (name.StartsWith(key))
            {
                return true;
            }
        }
        return false;
    }
}

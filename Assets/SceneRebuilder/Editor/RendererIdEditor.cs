using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        //parent = targetT.GetCurrentParent();
    }



    public override void OnToolLayout(RendererId item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select"))
        {
            EditorHelper.SelectObject(item.gameObject);
        }
        if (GUILayout.Button("SelectParent"))
        {
            EditorHelper.SelectObject(item.transform.parent);
        }
        if (GUILayout.Button("SelectParent2"))
        {
            var p = item.transform.parent;
            if (p == null)
            {
                EditorHelper.SelectObject(p);
            }
            else
            {
                EditorHelper.SelectObject(p.parent);
            }
            
        }
        EditorGUILayout.EndHorizontal();

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
                Debug.LogError(targetT.parentId);
                IdDictionary.InitInfos();
                var p = IdDictionary.GetGo(targetT.parentId);
                Debug.LogError(TransformHelper.GetPath(p.transform));
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
            EditorHelper.SelectObject(item.gameObject);
        }
        if (GUILayout.Button("UpParent"))
        {
            EditorHelper.UnpackPrefab(item.gameObject);
            if(item.gameObject.transform.parent!=null)
                item.gameObject.transform.SetParent(item.gameObject.transform.parent.parent);
            EditorHelper.SelectObject(item.gameObject);
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
            AcRTAlignJobSetting.Instance.SetDefault();
            PrefabInstanceBuilder.Instance.AcRTAlignJobs(item.gameObject, true);
        }
        if (GUILayout.Button("AlignEx"))
        {
            AcRTAlignJobSetting.Instance.SetDefault();
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
            MeshCombiner.Instance.CombineToOne(item.gameObject,true,true);
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
            FindGeometriesAndReplace(item.gameObject, false);
        }
        if (GUILayout.Button("SetParent"))
        {
            //GameObject goDwf = FindDwfGo(item.gameObject);
            //if (goDwf == null) return;

            //EditorHelper.UnpackPrefab(item.gameObject);
            //EditorHelper.UnpackPrefab(goDwf.gameObject);

            //List<Transform> geoList = FindGeometriesParents(item.gameObject);

            //var ts2 = goDwf.GetComponentsInChildren<Transform>(true);
            //for (int i = 0; i < geoList.Count; i++)
            //{
            //    Transform geo = geoList[i];
            //    List<Transform> result = FindListByName(ts2, geo.name);
            //    if (result.Count == 1)
            //    {
            //        result[0].SetParent(geo.parent);
            //        GameObject.DestroyImmediate(geo.gameObject);
            //    }
            //}

            FindGeometriesAndReplace(item.gameObject, true);
        }
        EditorGUILayout.EndHorizontal();
    }





    private static void FindGeometriesAndReplace(GameObject fbxObj, bool isReplace)
    {
        GameObject goDwf = FindDwfGo(fbxObj);
        if (goDwf == null) return;

        List<Transform> geoParentList0 = FindGeometriesParents(fbxObj);
        List<Transform> curParentList = FindCurvesParents(fbxObj);
        List<Transform> geoParentList1 = curParentList;

        List<Transform> geoList2 = FindGeometriesParents(goDwf);
        Debug.LogError($"geoList0:{geoParentList0.Count},geoList1:{geoParentList1.Count}geoList2:{geoList2.Count} curParentList:{curParentList.Count}");

        GameObject geoRoot3 = null;
        GameObject geoRoot4 = null;
        if (isReplace == false)
        {
            List<Transform> geoParentList01 = new List<Transform>();

            GameObject geoRoot01 = new GameObject("Geometry01");
            for (int i = 0; i < geoParentList0.Count; i++)
            {
                Transform t1 = geoParentList0[i];
                if (!geoParentList1.Contains(t1))
                {
                    var t1Clone = MeshHelper.CopyGO(t1);
                    t1Clone.SetParent(geoRoot01.transform);
                }
            }

            GameObject geoRoot10 = new GameObject("Geometry10");
            for (int i = 0; i < geoParentList1.Count; i++)
            {
                Transform t1 = geoParentList1[i];
                if (!geoParentList0.Contains(t1))
                {
                    var t1Clone = MeshHelper.CopyGO(t1);
                    t1Clone.SetParent(geoRoot10.transform);
                }
            }

            GameObject geoRoot1 = new GameObject("Geometry1");
            for (int i = 0; i < geoParentList1.Count; i++)
            {
                Transform t1 = geoParentList1[i];
                var t1Clone = MeshHelper.CopyGO(t1);
                t1Clone.SetParent(geoRoot1.transform);
            }

            GameObject geoRoot2 = new GameObject("Geometry2");
            for (int i = 0; i < geoList2.Count; i++)
            {
                Transform t1 = geoList2[i];
                Transform t1Clone = MeshHelper.CopyGO(t1);
                t1Clone.SetParent(geoRoot2.transform);
            }

            geoRoot3 = new GameObject("Geometry3");
            geoRoot4 = new GameObject("Geometry4");
        }



        for (int i = 0; i < geoParentList1.Count; i++)
        {
            Transform geoParent = geoParentList1[i];
            Transform geo1 = FindChild(geoParent, "geometry");
            Transform t2 = TransformHelper.FindByNameAndPosition(geoList2, geoParent.name, geo1.position);
            if (t2 != null)
            {
                geoParentList1.RemoveAt(i); i--;
                geoList2.Remove(t2);

                Transform geo2 = FindChild(t2, "geometry");
                Transform t1Clone = MeshHelper.CopyGO(geo2);

                float dis = Vector3.Distance(geo2.position, geo1.position);
                Debug.Log($"Find1 dis:{dis}\tgeo1:{geo1} geo2:{geo2}"); 

                

                if (isReplace == false)
                {
                    t1Clone.SetParent(geoRoot3.transform);
                }
                else
                {
                    if (geoParent.childCount == 1)
                    {
                        t1Clone.name = geoParent.name + "_New";
                        t1Clone.SetParent(geoParent.parent);
                        GameObject.DestroyImmediate(geoParent.gameObject);
                    }
                    else
                    {
                        t1Clone.name = geo1.name + "_New";
                        //Transform geo1 = FindChild(geoParent, "geometry");
                        t1Clone.SetParent(geoParent);
                        GameObject.DestroyImmediate(geo1.gameObject);
                    }

                }
            }
        }

        var ts2 = goDwf.GetComponentsInChildren<Transform>(true).ToList(); ;
        for (int i = 0; i < geoParentList1.Count; i++)
        {
            Transform geoParent = geoParentList1[i];
            Transform geo1 = FindChild(geoParent, "geometry");
            Transform t2 = TransformHelper.FindByNameAndPosition(ts2, geoParent.name, geo1.position);
            if (t2 != null)
            {
                Transform t1Clone = MeshHelper.CopyGO(t2);

                float dis = Vector3.Distance(t2.position, geo1.position);
                Debug.Log($"Find1 dis:{dis}\tgeo1:{geo1} geo2:{t2}");

                if (isReplace == false)
                {
                    t1Clone.SetParent(geoRoot4.transform);
                }
                else
                {
                    //t1Clone.SetParent(geoParent.parent);
                    //GameObject.DestroyImmediate(geoParent.gameObject);

                    if (geoParent.childCount == 1)
                    {
                        t1Clone.name = geoParent.name + "_New";
                        t1Clone.SetParent(geoParent.parent);
                        EditorHelper.Destroy(geoParent.gameObject);
                    }
                    else
                    {
                        t1Clone.name = geo1.name + "_New";
                        t1Clone.SetParent(geoParent);
                        GameObject.DestroyImmediate(geo1.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogError($"No Found 2 name:{geoParent.name} pos:{geo1.position}");
            }
        }
    }

    public static List<Transform> FindGeometriesParents(GameObject go)
    {
        List<Transform> geoList = FindTransforms(go, "Geometry");
        List<Transform> parents = new List<Transform>();
        foreach (var item in geoList)
        {
            var parent = item.parent;
            if (!parents.Contains(parent))
            {
                parents.Add(parent);
            }
        }
        return parents;
    }

    //public static List<Transform> FindCurve(GameObject go)
    //{
    //    List<Transform> geoList = FindTransforms(go, "curve");
    //    return geoList;
    //}

    public static List<Transform> FindCurvesParents(GameObject go)
    {
        List<Transform> geoList = FindTransforms(go, "curve");

        List<Transform> parents = new List<Transform>();
        foreach(var item in geoList)
        {
            var parent = item.parent;
            if (parent.name.Contains("Geometry"))
            {
                parent = parent.parent;
            }
            if (!parents.Contains(parent))
            {
                parents.Add(parent);
            }
        }
        return parents;
    }

    public static Transform FindChild(Transform t,string key)
    {
        List<Transform> list = FindChildren(t, key);
        return list[0];
    }

    public static List<Transform> FindChildren(Transform t, string key)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).name.ToLower().StartsWith(key))
            {
                //return t.GetChild(i);
                children.Add(t.GetChild(i));
            }
        }
        return children;
    }

    public static List<Transform> FindTransforms(GameObject go,string key)
    {
        List<Transform> list1 = new List<Transform>();
        List<Transform> list2 = new List<Transform>();
        var ts1 = go.GetComponentsInChildren<Transform>(true);
        foreach (var t in ts1)
        {
            string n = t.name;
            if (n.StartsWith(key))
            //if (t.name.ToLower().StartsWith(key))
            {
                //var p = t.parent;

                list1.Add(t);

                //if (p.childCount == 1)
                //{
                //    geoList.Add(p);
                //}
                //else
                //{
                //    Debug.LogWarning($"p.childCount != 1 p:{p.name}");
                //    t.name = p.name;
                //}
            }
            else
            {
                list2.Add(t);
            }
        }

        Debug.Log($"FindTransforms key:{key} count:{list1.Count} count2:{list2.Count} all:{ts1.Length}");
        return list1;
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

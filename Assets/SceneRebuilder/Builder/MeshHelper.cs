using CommonUtils;
using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.ComnLib.Utils;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class MeshHelper
{


    //public static GameObject CreatePoint(Vector3 p, string n, Transform pT, float size)
    //{
    //    GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    //    //g1.transform.SetParent(this.transform);
    //    //g1.transform.localPosition=p;
    //    g1.transform.position = p;
    //    g1.transform.localScale = new Vector3(size, size, size);
    //    g1.name = n;

    //    g1.transform.SetParent(pT);
    //    return g1;
    //}

    //public static GameObject CreateLocalPoint(Vector3 p, string n, Transform pT, float scale)
    //{
    //    GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    //    g1.transform.SetParent(pT);
    //    g1.transform.localPosition = p;
    //    //g1.transform.position = p;
    //    g1.transform.localScale = new Vector3(scale, scale, scale);
    //    g1.name = n;

    //    //g1.transform.SetParent(this.transform);

    //    g1.AddComponent<DebugInfoRoot>();
    //    return g1;
    //}

    public static void RemoveMeshComponents(GameObject go,bool isRemoveCollider=true)
    {
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf)
            GameObject.DestroyImmediate(mf);

        if (isRemoveCollider)
        {
            Collider mc = go.GetComponent<Collider>();
            if (mc)
                GameObject.DestroyImmediate(mc);
        }
     
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        if (mr)
            GameObject.DestroyImmediate(mr);
    }

    public static void SetNewMesh(GameObject go,Mesh mesh)
    {
        MeshFilter mf = go.AddMissingComponent<MeshFilter>();
        if (mf)
        {
            mf.sharedMesh = mesh;
        }
        MeshCollider mc = go.GetComponent<MeshCollider>();
        if (mc)
        {
            mc.sharedMesh = mesh;
        }
    }

    public static void RefreshCollderMesh()
    {
        DateTime start = DateTime.Now;
        int count1 = 0;
        int count2 = 0;
        MeshCollider[] collders = GameObject.FindObjectsOfType<MeshCollider>(true);
        for (int i = 0; i < collders.Length; i++)
        {
            MeshCollider collider = collders[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("RefreshCollderMesh", i, collders.Length, collider))
            {
                break;
            }
            MeshFilter mf = collider.GetComponent<MeshFilter>();
            if (mf == null)
            {
                count2++;
                Debug.Log($"RefreshCollderMesh Error MeshFilter==null [{i}] count2:{count2} collder:{collider}");
                MeshRenderer mr = collider.GetComponent<MeshRenderer>();
                if (mr)
                {
                    GameObject.DestroyImmediate(mr);
                }
                GameObject.DestroyImmediate(collider);
            }
            else if (collider.sharedMesh != mf.sharedMesh)
            {
                count1++;
                collider.sharedMesh = mf.sharedMesh;
                if (count1 < 10)
                {
                    Debug.Log($"RefreshCollderMesh Error collider.sharedMesh!= mf.sharedMesh [{i}] count2:{count2} collder:{collider}");
                }

            }
        }
        Debug.Log($"RefreshCollderMesh collders:{collders.Length} count:{count1} count2:{count2} time:{DateTime.Now - start}");
        ProgressBarHelper.ClearProgressBar();
    }

    //public static GameObject CopyMeshObject(GameObject source)
    //{
    //    return CopyMeshObject(source, source.name);
    //}


    //public static GameObject CopyMeshObject(GameObject source, string newName)
    //{
    //    GameObject newGo = new GameObject(newName);
    //    bool r1 = MeshHelper.CopyTransformMesh(source.gameObject, newGo);
    //    return newGo;
    //}

    //    public static bool CopyTransformMesh(GameObject source, GameObject target)
    //{
    //    bool r=CopyMeshComponents(source, target);
    //    if (r == false) return r;
    //    CopyTransfrom(source.transform, target.transform);
    //    return true;
    //}

    //public static void CopyTransfrom(Transform source, Transform target)
    //{
    //    if (target.parent != source.parent)
    //    {
    //        EditorHelper.UnpackPrefab(target.gameObject);
    //        target.SetParent(source.parent);
    //    }
    //    target.localPosition = source.localPosition;
    //    target.localRotation = source.localRotation;
    //    target.localScale = source.localScale;
    //}

    //public static MeshFilter CreateMeshComponents(GameObject go)
    //{
    //    //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    //MeshHelper.CopyMeshComponents(go, this.gameObject);
    //    //GameObject.DestroyImmediate(go);
    //    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
    //    if (meshRenderer == null)
    //    {
    //        meshRenderer = go.AddComponent<MeshRenderer>();
    //        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
    //        meshRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));
    //    }
    //    MeshFilter meshFilter = go.AddMissingComponent<MeshFilter>();
    //    return meshFilter;
    //}

    //public static bool CopyMeshComponents(GameObject source, GameObject target)
    //{
    //    if (source == null)
    //    {
    //        Debug.LogError($"CopyMeshComponents source == null target:{target}");
    //        return false;
    //    }
    //    if (source == null)
    //    {
    //        Debug.LogError($"CopyMeshComponents target == null source:{source}");
    //        return false;
    //    }
    //    MeshRenderer meshRenderer1 = source.GetComponent<MeshRenderer>();
    //    MeshFilter meshFilter1 = source.GetComponent<MeshFilter>();
    //    if (meshRenderer1 == null)
    //    {
    //        Debug.LogError($"CopyMeshComponents meshRenderer1 == null source:{source} target:{target}");
    //        return false;
    //    }
    //    if (meshFilter1 == null)
    //    {
    //        Debug.LogError($"CopyMeshComponents meshFilter1 == null source:{source} target:{target}");
    //        return false;
    //    }

    //    MeshRenderer meshRenderer2 = target.GetComponent<MeshRenderer>();
    //    if (meshRenderer2 == null)
    //    {
    //        meshRenderer2 = target.AddMissingComponent<MeshRenderer>();
    //    }

    //    //Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials1  :{meshRenderer2.sharedMaterial} source:{source} target:{target}");

    //    if (meshRenderer2.sharedMaterial == null)
    //    {
    //        meshRenderer2.sharedMaterials = meshRenderer1.sharedMaterials;
    //    }

    //    //Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials2  :{meshRenderer2.sharedMaterial} source:{source} target:{target}");

    //    if (meshRenderer2.sharedMaterial == null)
    //    {
    //        Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials  == null source:{source} target:{target}");
    //    }

       
    //    MeshFilter meshFilter2 = target.AddMissingComponent<MeshFilter>();
    //    meshFilter2.sharedMesh = meshFilter1.sharedMesh;

    //    MeshCollider meshCollider2 = target.GetComponent<MeshCollider>();
    //    if (meshCollider2)
    //    {
    //        meshCollider2.sharedMesh= meshFilter1.sharedMesh;
    //    }
    //    return true;
    //}


    public static Vector3 GetCenterOfList(List<Vector3> list)
    {
        Vector3 center = Vector3.zero;
        foreach (var item in list)
        {
            center += item;
        }
        center /= list.Count;
        return center;
    }

    public static Vector3 GetCenterOfList(MeshPointList list)
    {
        return list.GetCenter();
    }

    public static Vector3 GetCenterOfList(SharedMeshTrianglesList list)
    {
        return list.GetCenter();
    }

    public static float3 FindClosedPoint(float3 p, NativeArray<float3> list)
    {
        float minDis = float.MaxValue;
        float3 minP = float3.zero;
        foreach (float3 item in list)
        {
            bool3 b3 = item == p;
            if (b3.x && b3.y && b3.z) continue;
            float dis = math.distance(item, p);
            if (dis < minDis)
            {
                minDis = dis;
                minP = item;
            }
        }
        return minP;
    }

    public static Vector3 FindClosedPoint(Vector3 p, NativeArray<Vector3> list)
    {
        float minDis = float.MaxValue;
        Vector3 minP = Vector3.zero;
        foreach (var item in list)
        {
            if (item == p) continue;
            float dis = Vector3.Distance(item, p);
            if (dis < minDis)
            {
                minDis = dis;
                minP = item;
            }
        }
        return minP;
    }

    public static Vector3 FindClosedPoint(Vector3 p, List<Vector3> list)
    {
        float minDis = float.MaxValue;
        Vector3 minP = Vector3.zero;
        foreach (var item in list)
        {
            if (item == p) continue;
            float dis = Vector3.Distance(item, p);
            if (dis < minDis)
            {
                minDis = dis;
                minP = item;
            }
        }
        return minP;
    }

    //public static Vector3 FindClosedPoint(Vector3 p, SharedMeshTrianglesList list)
    //{
    //    float minDis = float.MaxValue;
    //    Vector3 minP = Vector3.zero;
    //    foreach (var item in list)
    //    {
    //        float dis = Vector3.Distance(item.GetCenter(), p);
    //        if (dis < minDis)
    //        {
    //            minDis = dis;
    //            minP = item.GetCenter();
    //        }
    //    }
    //    return minP;
    //}

    //public static SharedMeshTriangles FindClosedPlane(Vector3 p, SharedMeshTrianglesList list)
    //{
    //    float minDis = float.MaxValue;
    //    SharedMeshTriangles minP = null;
    //    foreach (var item in list)
    //    {
    //        float dis = Vector3.Distance(item.GetCenter(), p);
    //        if (dis < minDis)
    //        {
    //            minDis = dis;
    //            minP = item;
    //        }
    //    }
    //    return minP;
    //}

    //public static GameObject InstantiatePrefabFromSceneGo()
    //{

    //}
    [ContextMenu("RemoveEmptyObjects")]
    public static void RemoveEmptyObjects(GameObject root)
    {
        var ts = root.GetComponentsInChildren<Transform>(true);
        List<Transform> emptyList = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            var t = ts[i];
            if (t.childCount == 0)
            {
                if (IsEmptyObject(t))
                {
                    emptyList.Add(t);
                    Debug.Log($"empty:{t.name}");
                }
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            EditorHelper.UnpackPrefab(emptyList[i].gameObject);
            GameObject.DestroyImmediate(emptyList[i].gameObject);
        }

        Debug.Log($"empty:{emptyList.Count},all:{ts.Length}");
    }

    public static List<Type> typesOfEmptyObject = new List<Type>() {typeof(Transform),typeof(RendererId),typeof(MeshNode), typeof(InnerMeshNode), typeof(MeshRendererInfo), typeof(MeshRendererInfoEx), typeof(SubScene_Single), typeof(BoxCollider), typeof(BIMModelInfo), typeof(NavisModelRoot) };

    public static List<Type> typesOfEmptyChildObject = new List<Type>() { typeof(Transform), typeof(RendererId), typeof(MeshNode), typeof(InnerMeshNode), typeof(MeshRendererInfo), typeof(MeshRendererInfoEx), typeof(LODGroup), typeof(LODGroupInfo), typeof(SubScene_Single), typeof(BoxCollider)/*, typeof(BIMModelInfo), typeof(NavisModelRoot)*/ };

    public static bool IsSameNameGroup(Transform t)
    {
        if (t.childCount == 0) return false;
        if (IsEmptyObject(t) == false) return false;
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            if (!child.name.StartsWith(t.name)) return false;
        }
        return true;
    }

    public static bool IsEmptyLODSubGroup(Transform t)
    {
        if (t.childCount == 0) return false;
        if (IsEmptyObject(t) == false) return false;
        //bool isAllNoMeshRenderInfo = true;
        MeshRendererInfo[] rs = t.GetComponentsInChildren<MeshRendererInfo>(true);
        if(rs.Length==0)
        {
            return false;
        }
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            //MeshRendererInfo info = child.GetComponent<MeshRendererInfo>();
            MeshRendererInfo info = MeshRendererInfo.GetInfo(child.gameObject, false);
            if (info != null)
            {
                if (info.IsRendererType(MeshRendererType.LOD)==false)
                {
                    return false;
                }
            }
        }
        //Debug.Log($"IsEmptyLODSubGroup {t}");
        return true;
    }

    public static bool IsEmptyGroup(Transform t,bool isDebug)
    {
        if(isDebug)Debug.Log($"IsChildrenAllEmpty {t}");

        if (t.childCount == 0)
        {
            if (isDebug) Debug.Log($"t.childCount == 0");
            return false;
        }
        if (IsEmptyObject(t) == false)
        {
            if (isDebug) Debug.Log($"IsEmptyObject(t) == false");
            return false;
        }
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            if (IsEmptyChildObject(child) == false)
            {
                if (isDebug) Debug.Log($"IsEmptyChildObject(child) == false child:{child} i:{i}");
                return false;
            }
        }
        return true;
    }

    public static bool IsEmptyObjectEx(Transform t,bool isLog=false)
    {
        var components = t.GetComponents<Component>();
        if (components.Length == 1) return true;
        if (t.GetComponent<MeshRenderer>() != null && t.GetComponent<MeshFilter>() != null)
        {
            return true;
        }

        bool r = true;
        foreach(var c in components)
        {
            var type = c.GetType();
            if (typesOfEmptyObject.Contains(type) == false)
            {
                if (isLog)
                {
                    Debug.LogError($"IsEmptyObject type:{type}");
                }
                return false;
            }
        }
        return r;
    }

    public static bool IsEmptyObject(Transform t, bool isLog = false)
    {
        var components = t.GetComponents<Component>();
        if (components.Length == 1) return true;
        if (t.GetComponent<MeshRenderer>() != null && t.GetComponent<MeshFilter>() == null)
        {
            return true;
        }

        bool r = true;
        foreach (var c in components)
        {
            var type = c.GetType();
            if (typesOfEmptyObject.Contains(type) == false)
            {
                if (isLog)
                {
                    Debug.LogError($"IsEmptyObject type:{type}");
                }
                return false;
            }
        }
        return r;
    }

    public static bool IsEmptyChildObject(Transform t)
    {
        var components = t.GetComponents<Component>();
        if (components.Length == 1) return true;
        bool r = true;
        foreach (var c in components)
        {
            var type = c.GetType();
            if (typesOfEmptyChildObject.Contains(type) == false) return false;
        }
        return r;
    }

    public static void DecreaseEmptyGroupEx(GameObject root)
    {
        var ts = root.GetComponentsInChildren<Transform>(true);
        List<Transform> emptyList = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            var t = ts[i];
            if (t.childCount == 1)
            {
                if (IsEmptyObjectEx(t))
                {
                    emptyList.Add(t);
                    Debug.Log($"empty:{t.name}");
                }
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            var t = emptyList[i];
#if UNITY_EDITOR
            EditorHelper.UnpackPrefab(t.gameObject);
#endif
            var child = t.GetChild(0);
            child.SetParent(t.parent);
            child.name = t.name;
            GameObject.DestroyImmediate(t.gameObject);
        }

        Debug.Log($"empty:{emptyList.Count},all:{ts.Length}");
    }

    public static void DecreaseEmptyGroup(GameObject root)
    {
        var ts = root.GetComponentsInChildren<Transform>(true);
        List<Transform> emptyList = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            var t = ts[i];
            if (t.childCount == 1)
            {
                if (IsEmptyObject(t))
                {
                    emptyList.Add(t);
                    Debug.Log($"empty:{t.name}");
                }
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            var t = emptyList[i];
#if UNITY_EDITOR
            EditorHelper.UnpackPrefab(t.gameObject);
#endif
            var child = t.GetChild(0);
            child.SetParent(t.parent);
            child.name = t.name;
            GameObject.DestroyImmediate(t.gameObject);
        }

        Debug.Log($"empty:{emptyList.Count},all:{ts.Length}");
    }

    internal static void ShowAllRenderers(MeshRenderer[] allRenderers,int lv,string name)
    {
        DateTime start = DateTime.Now;
        //var allRenderers = GetRenderers();
        foreach (var renderer in allRenderers)
        {
            renderer.enabled = true;
            GameObject go = renderer.gameObject;

            for (int i = 0; i < lv && go != null && go.activeInHierarchy == false; i++)
            {
                go.SetActive(true);
                if (go.transform.parent != null)
                    go = go.transform.parent.gameObject;
            }
        }
        //Debug.Log($"ShowAllRenderers[{name}] count:{allRenderers.Length} time:{(DateTime.Now - start)} ");
    }

    public static string GetVertexCountS(float vertexCount)
    {
        //float f = vertexCount / 10000f;
        //if (f >= 100)
        //{
        //    return $"{f:F0}";
        //}
        //else if (f>=10)
        //{
        //    return $"{f:F1}";
        //}
        //else if (f >= 1)
        //{
        //    return $"{f:F2}";
        //}
        //else if (vertexCount >= 1000)
        //{
        //    return $"{f:F3}";
        //}
        ////else if (vertexCount >= 100)
        ////{
        ////    return $"{f:F3}";
        ////}
        ////else if (vertexCount >= 10)
        ////{
        ////    return $"{f:F3}";
        ////}
        //else if (vertexCount == 0)
        //{
        //    return "0";
        //}
        //else
        //{
        //    return $"{f:F3}";
        //}
        return VertexHelper.GetVertexCountS(vertexCount);
    }

//    public static GameObject EditorCopyGo(GameObject sourceGo)
//    {
//#if UNITY_EDITOR
//        GameObject newObj = null;
//        GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(sourceGo);
//        //Debug.Log($"root:[{root}]");
//        if (root != null)
//        {
//            string prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
//            //Debug.Log($"prefabPath:[{prefabPath}]");
//            var prefabAsset = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
//            //Debug.Log($"prefabAsset:[{prefabAsset}]");
//            var prefab = PrefabUtility.InstantiatePrefab(prefabAsset);
//            //Debug.Log($"prefab:[{prefab}]");
//            newObj = prefab as GameObject;
//        }
//        else
//        {
//            newObj = CopyGO(sourceGo);
//        }
//#else
//        GameObject newObj = CopyGO(sourceGo);
//#endif
//        return newObj;
//    }

    public static Vector3 GetNewPos(Vector3 pos,Vector3 fromP,Vector3 toP, TransfromAlignSetting transfromAlignSetting)
    {
        //var dis = toP - fromP;
        //if (transfromAlignSetting.SetPosX)
        //{
        //    pos.x += dis.x;
        //}
        //if (transfromAlignSetting.SetPosY)
        //{
        //    pos.y += dis.y;
        //}
        //if (transfromAlignSetting.SetPosZ)
        //{
        //    pos.z += dis.z;
        //}
        //return pos;

        Vector3 offset = GetPosOffset(fromP, toP, transfromAlignSetting);
        return pos + offset;
    }

    public static Vector3 GetPosOffset(Vector3 fromP, Vector3 toP, TransfromAlignSetting transfromAlignSetting)
    {
        Vector3 offset = Vector3.zero;
        var dis = toP - fromP;
        if (transfromAlignSetting.SetPosX)
        {
            offset.x = dis.x;
        }
        if (transfromAlignSetting.SetPosY)
        {
            offset.y = dis.y;
        }
        if (transfromAlignSetting.SetPosZ)
        {
            offset.z = dis.z;
        }
        return offset;
    }

    public static void Align(GameObject source, GameObject target,  TransfromAlignSetting transfromAlignSetting)
    {
        var minMax_From = MeshRendererInfo.GetMinMax(source);
        var minMax_TO = MeshRendererInfo.GetMinMax(target);

        var pos = source.transform.position;
        var offset = Vector3.zero;
        if (transfromAlignSetting.Align == TransfromAlignMode.Pivot)
        {
            offset= GetPosOffset(source.transform.position, target.transform.position, transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Min)
        {
            offset = GetPosOffset(minMax_From[0], minMax_TO[0], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Max)
        {
            offset = GetPosOffset(minMax_From[1], minMax_TO[1], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Center)
        {
            offset = GetPosOffset(minMax_From[3], minMax_TO[3], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.MinMax)
        {
            offset = GetPosOffset(minMax_From[0], minMax_TO[1], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.MaxMin)
        {
            offset = GetPosOffset(minMax_From[1], minMax_TO[0], transfromAlignSetting);
        }
        source.transform.position += offset;
    }

    public static void AlignRoot(GameObject source, GameObject target, GameObject sourceRoot, TransfromAlignSetting transfromAlignSetting)
    {
        var minMax_From = MeshRendererInfo.GetMinMax(source);
        var minMax_TO = MeshRendererInfo.GetMinMax(target);

        //var pos = source.transform.position;
        var offset = Vector3.zero;
        if (transfromAlignSetting.Align == TransfromAlignMode.Pivot)
        {
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x = target.transform.position.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y = target.transform.position.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z = target.transform.position.z;
            //}

            //pos = GetNewPos(pos, source.transform.position, target.transform.position, transfromAlignSetting);

            offset = GetPosOffset(source.transform.position, target.transform.position, transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Min)
        {
            //var dis = minMax_TO[0] - minMax_From[0];
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x += dis.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y += dis.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z += dis.z;
            //}

            //pos = GetNewPos(pos, minMax_From[0], minMax_TO[0], transfromAlignSetting);

            offset = GetPosOffset(minMax_From[0], minMax_TO[0], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Max)
        {
            //var dis = minMax_TO[1] - minMax_From[1];
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x += dis.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y += dis.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z += dis.z;
            //}

            //pos = GetNewPos(pos, minMax_From[1], minMax_TO[1], transfromAlignSetting);

            offset = GetPosOffset(minMax_From[1], minMax_TO[1], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.Center)
        {
            //var dis = minMax_TO[3] - minMax_From[3];
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x += dis.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y += dis.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z += dis.z;
            //}

            //pos = GetNewPos(pos, minMax_From[3], minMax_TO[3], transfromAlignSetting);

            offset = GetPosOffset(minMax_From[3], minMax_TO[3], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.MinMax)
        {
            //var dis = minMax_TO[1] - minMax_From[0];
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x += dis.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y += dis.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z += dis.z;
            //}

            //pos = GetNewPos(pos, minMax_From[0], minMax_TO[1], transfromAlignSetting);

            offset = GetPosOffset(minMax_From[0], minMax_TO[1], transfromAlignSetting);
        }
        else if (transfromAlignSetting.Align == TransfromAlignMode.MaxMin)
        {
            //var dis = minMax_TO[0] - minMax_From[1];
            //if (transfromAlignSetting.SetPosX)
            //{
            //    pos.x += dis.x;
            //}
            //if (transfromAlignSetting.SetPosY)
            //{
            //    pos.y += dis.y;
            //}
            //if (transfromAlignSetting.SetPosZ)
            //{
            //    pos.z += dis.z;
            //}

            //pos = GetNewPos(pos, minMax_From[1], minMax_TO[0], transfromAlignSetting);

            offset = GetPosOffset(minMax_From[1], minMax_TO[0], transfromAlignSetting);
        }
        //source.transform.position = pos + offset;

        sourceRoot.transform.position += offset;
    }

//    public static GameObject ReplaceGameObject(GameObject oldObj, GameObject prefab,bool isDestoryOriginal, TransfromAlignSetting transfromAlignSetting)
//    {
//        if (prefab == null) return null;
//#if UNITY_EDITOR
//        EditorHelper.UnpackPrefab(oldObj);
//        EditorHelper.UnpackPrefab(prefab);
//#endif

//        Transform parentOldObj = oldObj.transform.parent;
//        Transform parentPrefab = prefab.transform.parent;
//        oldObj.transform.parent = null;
//        prefab.transform.parent = null;

//        GameObject newObj = EditorCopyGo(prefab);

//        newObj.SetActive(true);

//        //newObj.transform.position = oldObj.transform.position;
//        //newObj.transform.eulerAngles = oldObj.transform.eulerAngles;
//        //newObj.transform.parent = oldObj.transform;

//        newObj.transform.position = prefab.transform.position;

//        newObj.transform.parent = oldObj.transform.parent;
//        if (transfromAlignSetting == null)
//        {
//            newObj.transform.localPosition = oldObj.transform.localPosition;
//        }
//        else
//        {
//            var minMax_TO = MeshRendererInfo.GetMinMax(oldObj);
//            var minMax_From = MeshRendererInfo.GetMinMax(prefab);

//            var pos = newObj.transform.localPosition;
//            if(transfromAlignSetting.Align== TransfromAlignMode.Pivot)
//            {
//                if (transfromAlignSetting.SetPosX)
//                {
//                    pos.x = oldObj.transform.localPosition.x;
//                }
//                if (transfromAlignSetting.SetPosY)
//                {
//                    pos.y = oldObj.transform.localPosition.y;
//                }
//                if (transfromAlignSetting.SetPosZ)
//                {
//                    pos.z = oldObj.transform.localPosition.z;
//                }
//            }
//            else if (transfromAlignSetting.Align == TransfromAlignMode.Min)
//            {
//                var dis = minMax_TO[0] - minMax_From[0];
//                if (transfromAlignSetting.SetPosX)
//                {
//                    pos.x += dis.x;
//                }
//                if (transfromAlignSetting.SetPosY)
//                {
//                    pos.y += dis.y;
//                }
//                if (transfromAlignSetting.SetPosZ)
//                {
//                    pos.z += dis.z;
//                }
//            }
//            else if (transfromAlignSetting.Align == TransfromAlignMode.Max)
//            {
//                var dis = minMax_TO[1] - minMax_From[1];
//                if (transfromAlignSetting.SetPosX)
//                {
//                    pos.x += dis.x;
//                }
//                if (transfromAlignSetting.SetPosY)
//                {
//                    pos.y += dis.y;
//                }
//                if (transfromAlignSetting.SetPosZ)
//                {
//                    pos.z += dis.z;
//                }
//            }
//            else if (transfromAlignSetting.Align == TransfromAlignMode.Center)
//            {
//                var dis = minMax_TO[3] - minMax_From[3];
//                if (transfromAlignSetting.SetPosX)
//                {
//                    pos.x += dis.x;
//                }
//                if (transfromAlignSetting.SetPosY)
//                {
//                    pos.y += dis.y;
//                }
//                if (transfromAlignSetting.SetPosZ)
//                {
//                    pos.z += dis.z;
//                }
//            }

//            if (transfromAlignSetting.SetPosition)
//            {
//                newObj.transform.localPosition = pos;
//            }
//        }

//        if (transfromAlignSetting.SetScale)
//        {
//            newObj.transform.localScale = oldObj.transform.localScale;
//        }
//        if (transfromAlignSetting.SetRotation)
//        {
//            newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles;
//        }
//        //newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles + new Vector3(0, 90, 90);

//        if (isDestoryOriginal)
//        {
//            newObj.name = oldObj.name;
//            GameObject.DestroyImmediate(oldObj);
//        }
//        else
//        {
//            newObj.name = oldObj.name + "_New";
//        }

//        newObj.transform.parent = parentOldObj;
//        oldObj.transform.parent = parentOldObj;
//        prefab.transform.parent = parentPrefab;

//        return newObj;
//    }

//    /// <summary>
//    /// ????????????????????????????????????????????????????????????
//    /// </summary>
//    /// <param name="oldObj"></param>
//    /// <param name="prefab"></param>
//    /// <returns></returns>
//    public static GameObject ReplaceByPrefab(GameObject oldObj, GameObject prefab)
//    {
//        if (prefab == null) return null;
//        GameObject newObj=ReplaceGameObject(oldObj, prefab,false,null);
//        newObj.transform.localEulerAngles = oldObj.transform.localEulerAngles + new Vector3(0, 90, 90);

//        //GameObject.Destroy(item.gameObject.transform);
//#if UNITY_EDITOR
//        if (oldObj.GetComponent<MeshNode>() == null)
//        {
//            oldObj.AddComponent<MeshNode>();
//        }
//        if (newObj.GetComponent<MeshNode>() == null)
//        {
//            newObj.AddComponent<MeshNode>();
//        }
//#endif
//        return newObj;
//    }

    //public static IEnumerator ReplaceByPrefabEx(GameObject oldObj, GameObject prefab, string bufferKey1 = "", string bufferKey2 = "", bool isDestoryOriginal = true)
    //{
    //    if (prefab == null)
    //    {
    //        yield break;
    //    }
    //    GameObject newObj = ReplaceByPrefab(oldObj, prefab);
    //    yield return RotateUntilMinDistanceEx(oldObj.transform, newObj.transform, bufferKey1, bufferKey2);
    //    if(isDestoryOriginal)
    //        GameObject.Destroy(oldObj.gameObject);
    //}

    public static MeshRotateBuffer buffer = new MeshRotateBuffer();

    public static bool showLog = true;
    private static void DebugLog(string msg)
    {
        if(showLog)
            Debug.Log(msg);
    }

    private static void DebugWarning(string msg)
    {
        if(showLog)
            Debug.LogWarning(msg);
    }

    //public static IEnumerator RotateUntilMinDistanceEx(Transform oldObj, Transform newObj, string bufferKey1 = "",string bufferKey2="", YieldInstruction yieldInstruction = null)
    //{
    //    //int rx = 0;
    //    //int ry = 0;
    //    //int rz = 0;

    //    DateTime start = DateTime.Now;
    //    Transform target = oldObj;
    //    double min = double.MaxValue;
    //    Vector3 v = Vector3.zero;
    //    int i = 0;
    //    {
    //        if (!string.IsNullOrEmpty(bufferKey1))
    //        {
    //            MeshRotateInfo info = buffer.Get(bufferKey1, bufferKey2, oldObj.localEulerAngles);
    //            if (info != null)
    //            {
    //                newObj.localEulerAngles = info.V2;

    //                DebugLog(string.Format("RotateUntilMinDistance ??????????????????:{0}", info));

    //                //yield break;
    //            }
    //        }
    //    }

    //    {
    //        var distance = MeshHelper.GetVertexDistanceEx(target, newObj);
    //        if (DistanceSetting.minDistance > 0 && distance < DistanceSetting.minDistance)
    //        {
    //            DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????1:{3},??????2:{4}",
    //                i, (DateTime.Now - start).TotalSeconds, distance, oldObj.localEulerAngles, newObj.localEulerAngles));
    //            //??????1:??????

    //            yield break;
    //        }
    //    }

    //    for (int rx = 0; rx < 4; rx++)
    //    {
    //        for (int ry = 0; ry < 4; ry++)
    //        {
    //            for (int rz = 0; rz < 4; rz++)
    //            {
    //                i++;
    //                newObj.localEulerAngles = new Vector3(90 * rx, 90 * ry, 90 * rz);
    //                var distance = MeshHelper.GetVertexDistanceEx(target, newObj,string.Format("{0}/{1}",i,64));
    //                if (distance > 0)
    //                {
    //                    if (DistanceSetting.minDistance > 0 && distance < DistanceSetting.minDistance)
    //                    {
    //                        DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????1:{3},??????2:{4}",
    //                            i, (DateTime.Now - start).TotalSeconds, distance, oldObj.localEulerAngles, newObj.localEulerAngles));
    //                        //??????1:??????
    //                        if (!string.IsNullOrEmpty(bufferKey1))
    //                        {
    //                            buffer.Set(bufferKey1,bufferKey2, oldObj.localEulerAngles, newObj.localEulerAngles);
    //                        }
    //                        yield break;
    //                    }

    //                    if (distance < min)
    //                    {
    //                        min = distance;
    //                        v = newObj.localEulerAngles;
    //                    }
    //                    //??????
    //                    //DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????:{3}???????????????:{4}",i, (DateTime.Now - start).TotalSeconds, distance, v, min));
    //                    //yield return new WaitForSeconds(0.5f);
    //                    //yield return new WaitForEndOfFrame();//17.2s / 64 = 0.269
    //                    //yield return null;//13.2s / 64 = 0.206s
    //                    //yield return new WaitForFixedUpdate();//4.8s / 64 = 0.075
    //                    yield return yieldInstruction;
    //                }
    //                else
    //                {
    //                    //??????2:??????1
    //                    yield break;
    //                }
    //            }
    //        }
    //    }

    //    {
    //        //??????3:??????2???????????????1????????????????????????????????????????????????????????????
    //        DebugLog(string.Format("RotateUntilMinDistance(??????) ??????:{0:F1}s?????????:{1},??????:{2}", (DateTime.Now - start).TotalSeconds, min, v));
    //        newObj.localEulerAngles = v;
    //    }
    //    //??????1:??????
    //    yield break;

    //}


    public static IEnumerator RotateUntilMinDistance(Transform oldObj, Transform newObj, string bufferKey = "",float minDistance = 0.01f, YieldInstruction yieldInstruction=null)
    {
        //int rx = 0;
        //int ry = 0;
        //int rz = 0;

        DateTime start = DateTime.Now;
        Transform target = oldObj;
        float min = float.MaxValue;
        Vector3 v = Vector3.zero;
        int i = 0;
        {
            if (!string.IsNullOrEmpty(bufferKey))
            {
                MeshRotateInfo info=buffer.Get(bufferKey,"", oldObj.localEulerAngles);
                if (info != null)
                {
                    newObj.localEulerAngles = info.V2;

                    DebugLog(string.Format("RotateUntilMinDistance ??????????????????:{0}", info));
                }
            }
        }

        {
            float distance = MeshHelper.GetVertexDistance(target, newObj);
            if (minDistance > 0 && distance < minDistance)
            {
                DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????1:{3},??????2:{4}",
                    i, (DateTime.Now - start).TotalSeconds, distance, oldObj.localEulerAngles, newObj.localEulerAngles));
                //??????1:??????
               
                yield break;
            }
        }

        for(int rx = 0; rx < 4; rx++)
        {
            for (int ry = 0; ry < 4; ry++)
            {
                for (int rz = 0; rz < 4; rz++)
                {
                    i++;
                    newObj.localEulerAngles = new Vector3(90 * rx, 90 * ry, 90 * rz);
                    float distance = MeshHelper.GetVertexDistance(target, newObj);
                    if (distance > 0)
                    {
                        if (minDistance>0 && distance < minDistance)
                        {
                            DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????1:{3},??????2:{4}", 
                                i,(DateTime.Now - start).TotalSeconds, distance, oldObj.localEulerAngles,newObj.localEulerAngles));
                            //??????1:??????
                            if (!string.IsNullOrEmpty(bufferKey))
                            {
                                buffer.Set(bufferKey,"", oldObj.localEulerAngles, newObj.localEulerAngles);
                            }
                                yield break;
                        }

                        if (distance < min)
                        {
                            min = distance;
                            v = newObj.localEulerAngles;
                        }
                        //??????
                        //DebugLog(string.Format("[{0}]RotateUntilMinDistance(??????) ??????:{1:F1}s?????????:{2},??????:{3}???????????????:{4}",i, (DateTime.Now - start).TotalSeconds, distance, v, min));
                        //yield return new WaitForSeconds(0.5f);
                        //yield return new WaitForEndOfFrame();//17.2s / 64 = 0.269
                        //yield return null;//13.2s / 64 = 0.206s
                        //yield return new WaitForFixedUpdate();//4.8s / 64 = 0.075
                        yield return yieldInstruction;
                    }
                    else
                    {
                        //??????2:??????1
                        yield break;
                    }
                }
            }
        }

        {
            //??????3:??????2???????????????1????????????????????????????????????????????????????????????
            DebugLog(string.Format("RotateUntilMinDistance(??????) ??????:{0:F1}s?????????:{1},??????:{2}", (DateTime.Now - start).TotalSeconds, min, v));
            newObj.localEulerAngles =v;
        }
        //??????1:??????
        yield break;

    }

    /// <summary>
    /// ????????????????????????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    private static float GetVertexDistance(Transform t1, Transform t2,bool isFroce=false)
    {
        MeshFilter mf1 = t1.GetComponent<MeshFilter>();
        MeshFilter mf2 = t2.GetComponent<MeshFilter>();
        if (mf1 == null || mf2 == null)
        {
            return -1;
        }
        else
        {
            //return GetVertexDistance(mf1.sharedMesh, mf2.sharedMesh);
            Mesh mesh1 = mf1.sharedMesh;
            Mesh mesh2 = mf2.sharedMesh;
            if (mesh1.vertexCount != mesh2.vertexCount && isFroce==false)
            {
                return float.MaxValue;
            }
            else
            {
                float distance = 0;
                var vsCount=mesh1.vertexCount;
                var vs1=mesh1.vertices;
                var vs2=mesh2.vertices;
                for (int i = 0; i < mesh1.vertexCount; i++)
                {
                    Vector3 p1 = vs1[i];
                    Vector3 p11 = t1.TransformPoint(p1);
                    Vector3 p2 = vs2[i];
                    Vector3 p22 = t2.TransformPoint(p2);

                    float d = Vector3.Distance(p11, p22);
                    distance += d;
                }
                return distance;
            }
        }
    }

    // public static double zero = 0.00001f;//1E-05
    // public static int zeroMax = 100;

    // public static double zeroDis=0.0001f;//1E-04

    // public static int maxDistance = 5;
    // public static int minDistance = 5;

    //public class DistanceArgs
    //{
    //    public Transform t1;
    //    public Transform t2;
    //    public bool isResetParent=false;
    //    public bool isResetPos=false;
    //    public bool isResetRotation=false;
    //    public string progress = "";

    //    public bool isLocal = false;//????????????

    //    public bool showLog=false;

    //    public DistanceArgs(Transform t1,Transform t2,string progress,bool showLog,bool local=false)
    //    {
    //        this.t1=t1;
    //        this.t2=t2;
    //        this.progress=progress;
    //        this.showLog=showLog;
    //        this.isLocal = local;
    //    }
    //}


    public static float GetVertexDistanceEx(Transform t1,Transform t2,string progress="",bool showLog=false, bool local = false)
    {
        //if (t1 == null)
        //{
        //    Debug.LogError($"GetVertexDistanceEx t1==null");
        //    return float.MaxValue;
        //}
        //if (t2 == null)
        //{
        //    Debug.LogError($"GetVertexDistanceEx t2==null");
        //    return float.MaxValue;
        //}
        //return GetVertexDistanceEx(new DistanceArgs(t1,t2,progress,showLog, local));
        return VertexHelper.GetVertexDistanceEx(t1, t2, progress, showLog, local);
    }

    public static float GetVertexDistanceEx(GameObject g1, GameObject g2, string progress = "", bool showLog = false,bool local=false)
    {
        //if (g1 == null)
        //{
        //    Debug.LogError($"GetVertexDistanceEx g1==null");
        //    return float.MaxValue ;
        //}
        //if (g2 == null)
        //{
        //    Debug.LogError($"GetVertexDistanceEx g2==null");
        //    return float.MaxValue;
        //}
        //return GetVertexDistanceEx(new DistanceArgs(g1.transform, g2.transform, progress, showLog, local));

        return VertexHelper.GetVertexDistanceEx(g1, g2, progress, showLog, local);
    }

    public static float GetSizeDistance(GameObject g1, GameObject g2)
    {
        MeshRendererInfo r1 = MeshRendererInfo.GetInfo(g1);
        MeshRendererInfo r2 = MeshRendererInfo.GetInfo(g2);
        var vs1=r1.GetBoxCornerPonts();
        var vs2 = r2.GetBoxCornerPonts();
        return DistanceUtil.GetDistance(vs1, vs2);
    }

    //public static float GetAvgVertexDistanceEx(Transform t1, Transform t2, string progress = "", bool showLog = false)
    //{
    //    if (t1 == null)
    //    {
    //        Debug.LogError($"GetVertexDistanceEx t1==null");
    //        return float.MaxValue;
    //    }
    //    if (t2 == null)
    //    {
    //        Debug.LogError($"GetVertexDistanceEx t2==null");
    //        return float.MaxValue;
    //    }
    //    return GetAvgVertexDistanceEx(new DistanceArgs(t1, t2, progress, showLog));
    //}

    //public static float GetAvgVertexDistanceEx(DistanceArgs arg)
    //{
    //    if (arg.showLog) Debug.Log($"GetVertexDistanceEx {arg.t1.name}|{arg.t2.name}");
    //    Transform t1 = arg.t1;
    //    Transform t2 = arg.t2;


    //    Vector3 p01 = t1.position;
    //    Vector3 p02 = t2.position;

    //    if (arg.isResetPos)
    //    {
    //        t1.position = Vector3.zero;
    //        t2.position = Vector3.zero;
    //    }

    //    Quaternion q1 = t1.rotation;
    //    Quaternion q2 = t2.rotation;
    //    if (arg.isResetRotation)
    //    {
    //        t1.rotation = Quaternion.identity;
    //        t2.rotation = Quaternion.identity;
    //    }

    //    DateTime start = DateTime.Now;

    //    MeshFilter mf1 = t1.GetComponent<MeshFilter>();
    //    MeshFilter mf2 = t2.GetComponent<MeshFilter>();
    //    float dis = -1;
    //    if (mf1 == null || mf2 == null)
    //    {
    //        Debug.LogError($"GetAvgVertexDistanceEx mf1 == null || mf2 == null mf1:{mf1} mf2:{mf2}");
    //        return float.MaxValue;
    //    }
    //    else
    //    {
    //        Mesh mesh1 = mf1.sharedMesh;
    //        Mesh mesh2 = mf2.sharedMesh;
    //        if (mesh1 == null || mesh2 == null)
    //        {
    //            Debug.LogError($"GetAvgVertexDistanceEx mesh1 == null || mesh2 == null mesh1:{mesh1} mesh2:{mesh2}");
    //            return float.MaxValue;
    //        }
    //        else
    //        {
    //            float distance = 0;
    //            Vector3[] points1 = VertexHelper.GetWorldVertexes(mesh1, t1);
    //            Vector3[] points2 = VertexHelper.GetWorldVertexes(mesh2, t2);
    //            int count = points1.Length;
    //            if (points2.Length < count)
    //            {
    //                count = points2.Length;
    //            }
    //            dis = DistanceUtil.GetDistance(points1, points2, arg.showLog) / count;
    //        }
    //    }

    //    if (arg.isResetPos)
    //    {
    //        t1.position = p01;
    //        t2.position = p02;
    //    }

    //    if (arg.isResetRotation)
    //    {
    //        t1.rotation = q1;
    //        t2.rotation = q2;
    //    }

    //    // if(isResetParent){
    //    //     t1.SetParent(parent1,true);
    //    //     t2.SetParent(parent2,true);
    //    // }

    //    InvokeGetVertexDistanceExCount++;

    //    return dis;
    //}

    //[Serializable]
    //public class MinDisTarget<T> where T : Component
    //{
    //    public float dis = float.MaxValue;
    //    public float meshDis = float.MaxValue;
    //    public T target = null;
    //    public MinDisTarget(float dis, float meshDis, T t)
    //    {
    //        this.dis = dis;
    //        target = t;
    //        this.meshDis = meshDis;
    //    }
    //}

    public static float GetCenterDistance(GameObject go1, GameObject go2)
    {
        MeshRendererInfo info0 = MeshRendererInfo.GetInfo(go1,false);
        MeshRendererInfo info1 = MeshRendererInfo.GetInfo(go2, false);
        float distance = Vector3.Distance(info0.center, info1.center);
        return distance;
    }
    public static float GetMinDistance(GameObject go1, GameObject go2)
    {
        MeshRendererInfo info0 = MeshRendererInfo.GetInfo(go1, false);
        MeshRendererInfo info1 = MeshRendererInfo.GetInfo(go2, false);
        float distance = Vector3.Distance(info0.minMax[0], info1.minMax[0]);
        return distance;
    }

    public static float GetMaxDistance(GameObject go1, GameObject go2)
    {
        MeshRendererInfo info0 = MeshRendererInfo.GetInfo(go1, false);
        MeshRendererInfo info1 = MeshRendererInfo.GetInfo(go2, false);
        float distance = Vector3.Distance(info0.minMax[1], info1.minMax[1]);
        return distance;
    }

    public static float GetBoundsDistance(GameObject go1, GameObject go2)
    {
        MeshRendererInfo info0 = MeshRendererInfo.GetInfo(go1, false);
        MeshRendererInfo info1 = MeshRendererInfo.GetInfo(go2, false);
        float distance1 = Vector3.Distance(info0.center, info1.center);
        float distance2 = Vector3.Distance(info0.minMax[0], info1.minMax[0]);
        float distance3 = Vector3.Distance(info0.minMax[1], info1.minMax[1]);
        float distance = distance1 + distance2 + distance3;
        return distance;
    }

    public enum LODCompareMode
    {
        Name,NameWithPos, NameWithCenter, NameWithMin, NameWithMax, NameWithBounds, NameWithMesh,NameWithSharedMesh, Pos, Center, Min, Max,Bounds, Mesh,SharedMesh
    }

    public static float GetDistance<T>(T item, Transform t, LODCompareMode mode) where T : Component
    {
        if (mode == LODCompareMode.NameWithPos || mode == LODCompareMode.Pos)
        {
            float distance = Vector3.Distance(item.transform.position, t.position);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithCenter || mode == LODCompareMode.Center)
        {
            float distance = GetCenterDistance(item.gameObject, t.gameObject);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithMin || mode == LODCompareMode.Min)
        {
            float distance = GetMinDistance(item.gameObject, t.gameObject);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithMax || mode == LODCompareMode.Max)
        {
            float distance = GetMaxDistance(item.gameObject, t.gameObject);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithBounds || mode == LODCompareMode.Bounds)
        {
            float distance = GetBoundsDistance(item.gameObject, t.gameObject);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithMesh || mode == LODCompareMode.Mesh)
        {
            float distance = VertexHelper.GetAvgVertexDistanceEx(item.transform, t);
            return distance;
        }
        else if (mode == LODCompareMode.NameWithSharedMesh || mode == LODCompareMode.SharedMesh)
        {
            MeshFilter mf1=t.GetComponent<MeshFilter>();
            MeshFilter mf2=item.transform.GetComponent<MeshFilter>();
            if(mf1==null||mf2==null){
                return 100;
            }
            if(mf1.sharedMesh==mf2.sharedMesh){
                return 0;
            }
            else{
                return 50;
            }
        }
        else
        {
            float distance = GetCenterDistance(item.gameObject, t.gameObject);
            return distance;
        }
    }

    public static int InvokeGetVertexDistanceExCount=0;



    //public static float GetVertexDistanceEx(DistanceArgs arg)
    //{
    //    if(arg.showLog)Debug.Log($"GetVertexDistanceEx {arg.t1.name}|{arg.t2.name}");
    //    Transform t1=arg.t1;
    //    Transform t2=arg.t2;


    //    Vector3 p01=t1.position;
    //    Vector3 p02=t2.position;

    //    if(arg.isResetPos)
    //    {
    //        t1.position=Vector3.zero;
    //        t2.position=Vector3.zero;
    //    }

    //    Quaternion q1=t1.rotation;
    //    Quaternion q2=t2.rotation;
    //    if(arg.isResetRotation)
    //    {
    //        t1.rotation=Quaternion.identity;
    //        t2.rotation=Quaternion.identity;
    //    }

    //    DateTime start = DateTime.Now;

    //    MeshFilter mf1 = t1.GetComponent<MeshFilter>();
    //    MeshFilter mf2 = t2.GetComponent<MeshFilter>();
    //    float dis=-1;
    //    if (mf1 == null || mf2 == null)
    //    {
    //        //return -1;
    //        //Debug.LogWarning("mf1 == null || mf2 == null");
    //        Vector3[] points1 = VertexHelper.GetChildrenWorldVertexes(t1.gameObject);
    //        Vector3[] points2 = VertexHelper.GetChildrenWorldVertexes(t2.gameObject);
    //        dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
    //    }
    //    else
    //    {
    //        Mesh mesh1 = mf1.sharedMesh;
    //        if (mesh1 == null)
    //        {
    //            Debug.LogError("mf1.sharedMesh==null:"+mf1);
    //        }
    //        Mesh mesh2 = mf2.sharedMesh;
    //        if (mesh2 == null)
    //        {
    //            Debug.LogError("mf2.sharedMesh==null:" + mf2);
    //        }

    //        if (mesh1 == null || mesh2 == null)
    //        {
    //            Vector3[] points1 = VertexHelper.GetChildrenWorldVertexes(t1.gameObject);
    //            Vector3[] points2 = VertexHelper.GetChildrenWorldVertexes(t2.gameObject);
    //            dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
    //        }
    //        else
    //        {
    //            //Vector3[] points1 = GetWorldVertexes(mesh1, t1);
    //            //Vector3[] points2 = GetWorldVertexes(mesh2, t2);

    //            Vector3[] points1 = mesh1.vertices;
    //            if (arg.isLocal == false)
    //            {
    //                points1 = VertexHelper.GetWorldVertexes(points1, t1);
    //            }
    //            Vector3[] points2 = mesh2.vertices;
    //            if (arg.isLocal == false)
    //            {
    //                points2 = VertexHelper.GetWorldVertexes(points2, t2);
    //            }
    //            dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
    //        }
    //    }

    //    if(arg.isResetPos)
    //    {
    //        t1.position=p01;
    //        t2.position=p02;
    //    }

    //    if(arg.isResetRotation)
    //    {
    //        t1.rotation=q1;
    //        t2.rotation=q2;
    //    }

    //    // if(isResetParent){
    //    //     t1.SetParent(parent1,true);
    //    //     t2.SetParent(parent2,true);
    //    // }

    //    InvokeGetVertexDistanceExCount++;

    //    return dis;
    //}

    // public static Vector3 GetMinDistancePoint(Vector3 p1, Vector3[] points)
    // {
    //     //DateTime start = DateTime.Now;

    //     float distance = float.MaxValue;
    //     Vector3 result = Vector3.zero;
    //     int index = 0;
    //     for (int i = 0; i < points.Length; i++)
    //     {
    //         Vector3 p2 = points[i];
    //         float dis = Vector3.Distance(p1, p2);
    //         if (dis < distance)
    //         {
    //             distance = dis;
    //             result = p2;
    //             index = i;
    //         }
    //     }
    //     //if(distance<zero)
    //     //    DebugLog(string.Format("GetMinDistancePoint ??????:{0}s?????????:{1}?????????:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
    //     return result;
    // }

    // public static Bounds GetBounds(MeshFilter meshFilter)
    // {
    //     Vector3[] vs=GetWorldVertexes(meshFilter);
    //     Bounds bounds=new Bounds();
    //     bounds.center=vs[3];
    //     bounds.size=vs[2];
    //     return bounds;
    // }

       

    public static void CenterPivot(Transform t,Vector3 center)
    {
        //List<Transform> children=new List<Transform>();
        //for(int i=0;i<t.childCount;i++)
        //{
        //    children.Add(t.GetChild(i));
        //}
        //foreach(var child in children){
        //    child.SetParent(null);
        //}
        //t.position=center;

        //foreach(var child in children){
        //    child.SetParent(t);
        //}
        SetParentTransfrom(t, () =>
        {
            t.position = center;
        });
    }
    public static void SetParentTransfrom(Transform t,Action setTranfromActoin)
    {
#if UNITY_EDITOR
        EditorHelper.UnpackPrefab(t.gameObject);
#endif
        Transform p = t.parent;

        List<Transform> children=new List<Transform>();
        for(int i=0;i<t.childCount;i++)
        {
            children.Add(t.GetChild(i));
        }
        foreach(var child in children){
            child.SetParent(null);
        }

        t.SetParent(null);
        t.position = Vector3.zero;
        t.localScale = Vector3.one;
        t.rotation = Quaternion.identity;

        if (setTranfromActoin != null)
        {
            setTranfromActoin();
        }

        foreach(var child in children){
            child.SetParent(t);
        }

        t.SetParent(p);
    }

    public static Vector3[] CenterPivot(Transform t,IEnumerable<MeshFilter> meshFilters)
    {
        var minMax= VertexHelper.GetMinMax(meshFilters);
        CenterPivot(t,minMax[3]);
        return minMax;
    }

    public static void CenterPivotAll(GameObject root)
    {
        DateTime startT = DateTime.Now;
        Transform[] ts = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < ts.Length; i++)
        {
            Transform t = ts[i];
            //MeshRenderer mr = t.GetComponent<MeshRenderer>();
            //if (mr != null)
            //{
            //    continue;
            //}
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("", i, ts.Length, t)))
            {
                break;
            }
            MeshHelper.CenterPivot(t.gameObject,true);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"CenterPivotAll root:{root} ts:{ts.Length} time:{DateTime.Now - startT}");
    }

    public static Vector3[] CenterPivot(GameObject go,bool isForce=false)
    {
        if (go == null) return null;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
            var minMax = VertexHelper.GetMinMax(mfs);
            CenterPivot(go.transform, minMax[3]);
            return minMax;
        }
        else
        {
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
            var minMax = VertexHelper.GetMinMax(mfs);
            if (isForce)
            {
                CenterPivot(go.transform, minMax[3]);
            }
            return minMax;
        }
    }



    public static void ZeroParent(GameObject go)
    {
        SetParentTransfrom(go.transform, () =>
        {
            go.transform.position = Vector3.zero;
        });
    }

    //public static Vector3[] GetMinMax(MeshFilter meshFilter)
    //{
    //    return GetMinMax(new MeshFilter[] { meshFilter });
    //}





    public static void RemoveNew(GameObject go)
    {
        int count = 0;
        var renderers = go.GetComponentsInChildren<Transform>(true);
        foreach (var renderer in renderers)
        {
            if (renderer.name.EndsWith("_New"))
            {
                renderer.name = renderer.name.Replace("_New", "");
                count++;
            }
            if (renderer.name.EndsWith("_New1"))
            {
                renderer.name = renderer.name.Replace("_New1", "");
                count++;
            }
            if (renderer.name.EndsWith("_New2"))
            {
                renderer.name = renderer.name.Replace("_New2", "");
                count++;
            }
            if (renderer.name.EndsWith("_New3"))
            {
                renderer.name = renderer.name.Replace("_New3", "");
                count++;
            }
        }
        Debug.Log($"RemoveNew count:{count} renderers:{renderers.Length}");
    }

    public static void DestroyError(GameObject go)
    {
        int count = 0;
        var renderers = go.GetComponentsInChildren<Transform>(true);
        foreach (var renderer in renderers)
        {
            if (renderer.name.EndsWith("_Error") || renderer.name.EndsWith("_AlignError"))
            {
                GameObject.DestroyImmediate(renderer.gameObject);
                count++;
            }
        }
        Debug.Log($"RemoveNew count:{count} renderers:{renderers.Length}");
    }

 

    public static float CompareSize(MeshPoints ps1, MeshPoints ps2)
    {
        StringBuilder sbScaleLog = new StringBuilder();
        List<float> sizeFrom = ps1.GetSizeList();
        List<float> sizeTo = ps2.GetSizeList();
        List<float> scaleList = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            float sizeScale = sizeTo[i] / sizeFrom[i];
            if (scaleList.Contains(sizeScale))
            {
                continue;
            }
            sbScaleLog.Append($"sizeScale[{i}]:{sizeScale}({sizeTo[i]}/{sizeFrom[i]});\t");
            scaleList.Add(sizeScale);
        }

        if (scaleList.Count == 1)
        {
            //
        }

        scaleList.Sort();

        float sum = 0;
        float avg = 0;
        for (int i = 0; i < scaleList.Count; i++)
        {
            float scale = scaleList[i];
            sbScaleLog.Append($"scale[{i}]:{scale};\t");
            sum += scale;
        }
        avg = sum / scaleList.Count;

        var scaleMin = scaleList.First();
        var scaleMax = scaleList.Last();
        float scale_scale = scaleMax / scaleMin;
        sbScaleLog.AppendLine($"scale_scale:{scale_scale}");
        //bool isSame = scale_scale < 1.01;

        //Debug.LogError($"IsSameSize ps1:{ps1.name} ps2:{ps2.name} result:{isSame} ss:{scale_scale} log:{sbScaleLog}");
        return scale_scale;
    }

    public static string CompareSize_Debug(MeshPoints ps1, MeshPoints ps2)
    {
        StringBuilder sbScaleLog = new StringBuilder();
        List<float> sizeFrom = ps1.GetSizeList();
        List<float> sizeTo = ps2.GetSizeList();
        List<float> scaleList = new List<float>();
        for (int i = 0; i < 3; i++)
        {
            float sizeScale = sizeTo[i] / sizeFrom[i];
            if (scaleList.Contains(sizeScale))
            {
                continue;
            }
            sbScaleLog.Append($"sizeScale[{i}]:{sizeScale}({sizeTo[i]}/{sizeFrom[i]});\t");
            scaleList.Add(sizeScale);
        }

        if (scaleList.Count == 1)
        {
            //
        }

        scaleList.Sort();

        string txt = "";
        float sum = 0;
        float avg = 0;
        for (int i = 0; i < scaleList.Count; i++)
        {
            float scale = scaleList[i];
            sbScaleLog.Append($"scale[{i}]:{scale};\t");
            sum += scale;
            txt += scale + ";";
        }
        avg = sum / scaleList.Count;

        var scaleMin = scaleList.First();
        var scaleMax = scaleList.Last();
        float scale_scale = scaleMax / scaleMin;
        sbScaleLog.AppendLine($"scale_scale:{scale_scale}");
        //bool isSame = scale_scale < 1.01;

        //Debug.LogError($"IsSameSize ps1:{ps1.name} ps2:{ps2.name} result:{isSame} ss:{scale_scale} log:{sbScaleLog}");
        return $"avg:{avg} max/min:{scale_scale} list:{txt}";
    }

    public static bool IsSameSize(MeshPoints ps1,MeshPoints ps2)
    {
        float ss = CompareSize(ps1, ps2);
        return ss < 1.05;
    }

  


    public static Vector3[] GetWorldVertexes(MeshPoints go)
    {
        //return go.vertices;
        return go.GetWorldVertexes();
    }


    public static Unity.Mathematics.float3[] GetWorldVertexesF3(MeshFilter meshFilter){
        var vs2=meshFilter.sharedMesh.vertices;
        var vs22=MeshHelper.GetWorldVertexesF3(vs2,meshFilter.transform);
        return vs22;
    }

    public static Unity.Mathematics.float3[] GetWorldVertexesF3(Vector3[] vs, Transform t1){
        var vCount=vs.Length;
        Unity.Mathematics.float3[] points1 = new Unity.Mathematics.float3[vCount];
        // var vs=mesh1.vertices;
        for (int i = 0; i < vCount; i++)
        {
            Unity.Mathematics.float3 p1 = vs[i];
            Unity.Mathematics.float3 p11 = t1.TransformPoint(p1);
            //points1.Add(p11);
            points1[i]=p11;
        }
        return points1;
    }





    public static float GetVertexDistance(Mesh mesh1, Mesh mesh2)
    {
        if (mesh1.vertexCount != mesh2.vertexCount)
        {
            return float.MaxValue;
        }
        else
        {
            float distance = 0;
            var vs1=mesh1.vertices;
            var vs2=mesh2.vertices;
            var vCount=mesh1.vertexCount;
            for (int i = 0; i < vCount; i++)
            {
                Vector3 p1 = vs1[i];
                Vector3 p2 = vs2[i];
                float d = Vector3.Distance(p1, p2);
                distance += d;
            }
            return distance;
        }
    }

    public static bool AlignMeshNode_OLD1(MeshNode node1,MeshNode node2,int tryCount)
    {
        Debug.Log($"AlignMeshNode,Node1:{node1.name},Node2:{node2.name}");

        DateTime start=DateTime.Now;
        //Debug.Log("AlignMeshNode Start");
        Vector3 longLine1 = node1.GetLongLine();
        Vector3 longLine2 = node2.GetLongLine();

        Quaternion qua1 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());
        node2.transform.rotation = qua1 * node2.transform.rotation;

        //Quaternion qua2 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());
        //node2.transform.rotation = qua2 * node2.transform.rotation;

        Vector3 offset = node1.GetCenterP() - node2.GetCenterP();
        node2.transform.position += offset;

        int j=0;
        float angle=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
        for(;j<tryCount;j++)
        {
            Debug.Log($"[{j}]angle:{angle}");
            //node2.transform.RotateAround(node1.GetCenterP(), node2.GetLongLine(), -angle);
            node2.transform.RotateAround(node2.GetCenterP(), node2.GetLongLine(), -angle);
            angle = Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
            if (angle <= 0) {
                Debug.Log($"[{j}]angle:{angle}");
                break;
            }
        }
        if(j>=tryCount)
        {
            Debug.LogError($"[j>=tryCount]tryCount:{tryCount},angle:{angle},Node1:{node1.name},Node2:{node2.name}");
            return false;
        }
        Debug.Log($"AlignMeshNode Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        return true;
    }

    public static void ShowNodesNormalInfo(MeshNode node1,MeshNode node2)
    {
        
        Vector3 longLine1 = node1.GetLongLine();
        Vector3 longLine2 = node2.GetLongLine();
        Debug.Log($"[ShowNodesNormalInfo] longLine1:({longLine1.x},{longLine1.y},{longLine1.z}) |" + $"longLine2:({longLine2.x},{longLine2.y},{longLine2.z}) |" );

        //?????????????????????
        Vector3 normal1 = node1.GetLongShortNormal();
        Vector3 normal2 = node2.GetLongShortNormal();
        Debug.Log($"[GetLongShortNormal] normal1:({normal1.x},{normal1.y},{normal1.z}) |" + $"normal2:({normal2.x},{normal2.y},{normal2.z}) |" + $"normalAngle:{Vector3.Angle(normal1, normal2)}");

        // //??????????????????
        // Vector3 normal12 = node1.GetLongShortNormalNew();
        // Vector3 normal22 = node2.GetLongShortNormalNew();
        // Debug.Log($"[GetLongShortNormalNew] normal12:({normal12.x},{normal12.y},{normal12.z}) |" + $"normal22:({normal22.x},{normal22.y},{normal22.z}) |" + $"normalAngle:{Vector3.Angle(normal12, normal22)}");
    }

    public static int testId = 0;

    // public static bool AlignMeshNode(MeshNode node1,MeshNode node2,int tryCount,bool isShowDetail)
    // {
    //     DateTime start=DateTime.Now;
    //     double minDis=double.MaxValue;
    //     int maxPId=0;
    //     int minPId=0;
    //     AlignResult minResult=null;
    //     //node1?????????node2??????node1???
    //     Debug.Log($">> AlignMeshNode,Node1:{node1.name},Node2:{node2.name}");

    //     var meshData=node2.meshData;

    //     List<IJ> ids1 = node1.GetAllMinMaxIds();
    //     List<IJ> ids2 = node2.GetAllMinMaxIds();

    //     for(int k= 0; k<ids2.Count;k++)
    //     {
    //         int i = ids2[k].i;
    //         int j = ids2[k].j;
    //         meshData.maxPId = i;
    //         meshData.minPId = j;
    //         Debug.LogWarning($"----------------maxPId:{meshData.maxPId},minPId:{meshData.minPId}");

    //         MeshNode node22 = CopyGO(node2);//?????????????????????????????????
    //         AlignResult result = AlignMeshNode_Core(node1, node22, tryCount, isShowDetail, true);
    //         if (result == null)
    //         {
    //             node2.gameObject.name += "_" + testId;
    //             testId++;
    //             continue;
    //             //return false;
    //         }
    //         if (result.Distance < minDis)
    //         {
    //             minDis = result.Distance;
    //             minResult = result;
    //             maxPId = i;
    //             minPId = j;
    //         }
    //     }

    //     //for (int i= 0; i<meshData.maxPList.Count;i++)
    //     //{
    //     //    var maxP=meshData.GetMaxP(i);
    //     //    for(int j= 0; j<meshData.minPList.Count;j++)
    //     //    {
    //     //        var minP=meshData.GetMinP(j);
    //     //        meshData.maxPId=i;
    //     //        meshData.minPId=j;
    //     //        Debug.LogWarning($"----------------maxPId:{meshData.maxPId},minPId:{meshData.minPId}");
    //     //        AlignResult result=AlignMeshNode_Core(node1,node2,tryCount,isShowDetail,true);
    //     //        if(result==null){
    //     //            return false;
    //     //        }
    //     //        if(result.Distance<minDis){
    //     //            minDis=result.Distance;
    //     //            minResult=result;
    //     //            maxPId=i;
    //     //            minPId=j;
    //     //        }
    //     //    }
    //     //}

    //     meshData.maxPId=maxPId;
    //     meshData.minPId=minPId;
    //     AlignResult result2=AlignMeshNode_Core(node1,node2,tryCount,isShowDetail,true);
    //     Debug.LogError($"AlignMeshNode Time:{(DateTime.Now-start).TotalMilliseconds}ms,minResult:{minResult}");
    //     if(minResult==null){
    //         return false;
    //     }
        
    //     return minResult.IsZero;
    // }

    // public static void TryAngles(MeshNode node1,MeshNode node2)
    // {
    //     Quaternion quaternion=node2.transform.rotation;
    //     for(int x=0;x<)
    // }

    public static AlignResult AlignMeshNode(MeshNode node1,MeshNode node2,int maxTryCount,bool isShowDetail,bool isShowLog)
    {
        //Debug.LogError($"AlignMeshNode isShowDetail:{isShowDetail},isShowLog:{isShowLog}");

        DateTime start=DateTime.Now;

        // node2.transform.position=node1.transform.position;
        // var dis1 = GetVertexDistanceEx(node1.transform, node2.transform,"AlignMeshNode1");//??????????????????true,true(isResetPos),false
        // Debug.Log($"dis1:{dis1}");
        // if(dis1<=0)//??????????????????????????????????????????????????????????????????????????????
        // {
        //     Debug.LogWarning($"AlignMeshNode[NoAlign] Time:{(DateTime.Now-start).TotalMilliseconds}ms,Node1:{node1.name},Node2:{node2.name},dis:{dis1}");
        //     NoAlignCount++;
                    
        //     AlignResult result0=new AlignResult();
        //     result0.Distance=dis1;
        //     result0.IsZero=true;
        //     return true;
        // }
        //////////////////////////////////////////////////////////////////////

        if(node1.VertexCount!=node2.VertexCount){
            return new AlignResult();
        }

        double minDis=double.MaxValue;
        MinMaxId? minMaxId1=null;
        MinMaxId? minMaxId2=null;
        AlignResult minResult=null;
        //node1?????????node2??????node1???
        if(isShowLog)Debug.Log($">> AlignMeshNode,Node1:{node1.name},Node2:{node2.name}");

        List<MinMaxId> ids1 = node1.GetAllMinMaxIds();
        List<MinMaxId> ids2 = node2.GetAllMinMaxIds();

        var meshData1=node1.meshData;
        var meshData2=node2.meshData;
        
        bool isTest=step!=AlignDebugStep.None;
        int tryCount=0;
        bool IsFoundZero=false;
        for(int l=0;l<ids1.Count;l++)
        {
            meshData1.SetMinMaxId(ids1[l]);
            var angle1=meshData1.GetLongShortAngle();
            // ids2.Sort((a,b)=>{
            //     return (a.)
            // })
            for(int k= 0; k<ids2.Count;k++)
            {
                meshData2.SetMinMaxId(ids2[k]);
                tryCount++;

                if(isShowLog)Debug.LogWarning($"[{l},{k}]----------------maxPId1:{meshData1.maxPId},minPId1:{meshData1.minPId}|maxPId2:{meshData2.maxPId},minPId2:{meshData2.minPId}");

                MeshNode node22 =node2;
                if(isTest) node22 = CopyGO(node2);//?????????????????????????????????
                AlignResult result = AlignMeshNode_Core(node1, node22, maxTryCount, isShowDetail, isShowLog);
                if (result == null)
                {
                    node22.gameObject.name += "_" + testId;
                    testId++;
                    continue;
                    //return false;
                }
                if (result.Distance < minDis )
                {
                    minDis = result.Distance;
                    minResult = result;
                    
                    minMaxId1=ids1[l];
                    minMaxId2=ids2[k];

                    // if(minDis<=zero && isTest==false){ //????????????????????????
                    //     IsFoundZero=true;
                    //     break;
                    // }

                    if(result.IsZero && isTest==false){ //????????????????????????????????????????????????
                        IsFoundZero=true;
                        break;
                    }
                }
            }
            if(IsFoundZero){
                break;
            }
        }

        if(IsFoundZero==false || isTest==true)
        {
            if(minMaxId1!=null&&minMaxId2!=null)
            {
                //?????????????????????????????????
                meshData1.SetMinMaxId((MinMaxId)minMaxId1);
                node1.ShowLongShortDebugDetail(true);
                meshData2.SetMinMaxId((MinMaxId)minMaxId2);
                node2.ShowLongShortDebugDetail(true);
                AlignResult result2=AlignMeshNode_Core(node1,node2,tryCount,isShowDetail,isShowLog);
                result2.IsZero=result2.IsRelativeZero;//????????????????????????????????????
                //Debug.LogError($"AlignMeshNode2 [IsFoundZero:{IsFoundZero}] try:{tryCount};Time:{(DateTime.Now-start).TotalMilliseconds}ms,Result:{result2}");
                if(isShowLog)
                {
                    Debug.LogError($"AlignMeshNode2 [IsFoundZero:{IsFoundZero}] try:{tryCount};Time:{(DateTime.Now-start).TotalMilliseconds}ms,Result:{result2}");
                }
                else{
                    Debug.Log($"AlignMeshNode2 [IsFoundZero:{IsFoundZero}] try:{tryCount};Time:{(DateTime.Now-start).TotalMilliseconds}ms,Result:{result2}");
                }
                return result2;
            }
            else{
                return new AlignResult();
            }
        }
        if(isShowLog)
        {
            Debug.LogError($"AlignMeshNode1 [IsFoundZero:{IsFoundZero}] try:{tryCount};Time:{(DateTime.Now-start).TotalMilliseconds}ms,Result:{minResult}");
        }
        else{
            Debug.Log($"AlignMeshNode1 [IsFoundZero:{IsFoundZero}] try:{tryCount};Time:{(DateTime.Now-start).TotalMilliseconds}ms,Result:{minResult}");
        }
        return minResult;
    }


    public static int TryAlignMeshNodeCount=0;

    public static int NoAlignCount=0;

    public static AlignResult AlignMeshNode_Core(MeshNode node1,MeshNode node2,int tryCount,bool isShowDetail,bool showLog)
    {
        // MeshJobs.NewMeshAlignJob(node1,node2);

        // return null;
        //node1?????????node2??????node1???
        // Debug.Log($">> AlignMeshNode,Node1:{node1.name},Node2:{node2.name}");

        DateTime start=DateTime.Now;
        //Debug.Log("AlignMeshNode Start");

        TryAlignMeshNodeCount++;

        var meshData1=node1.meshData;
        var meshData2=node2.meshData;

        // node1.transform.SetParent(null);
        // node2.transform.SetParent(null);

        //node1.transform.position.PrintVector3("AlignMeshNode_Core_Start");

        if(showLog)ShowNodesNormalInfo(node1,node2);

        //????????????????????????????????????????????????
        if(showLog)Debug.Log($"node1 angle:{node1.GetLongShortAngle()},long:{node1.LongLineDistance},short:{node1.ShortLineDistance},rate:{node1.LongShortRate}, |" + $"node2 angle:{node2.GetLongShortAngle()},long:{node2.LongLineDistance},short:{node2.ShortLineDistance},rate:{node2.LongShortRate}");

        var angle22 = Math.Abs(node1.GetLongShortAngle() - node2.GetLongShortAngle());
        var long22 = Math.Abs(node1.LongLineDistance - node2.LongLineDistance);
        var short22= Math.Abs(node1.ShortLineDistance - node2.ShortLineDistance);
        var rate22 = Math.Abs(node1.LongShortRate - node2.LongShortRate);
        if(showLog)Debug.Log($"node1-node2 angle:{angle22},long:{long22},short:{short22},rate:{rate22} |");
        if (step == AlignDebugStep.Start)
        {
            Debug.LogError("DebugTest Step:" + step);
            return null;//?????????0??????????????????
        }

        //1.???????????????????????????????????????node2??????????????????
        Transform parentOld=node2.transform.parent;
        GameObject tempCenter=node2.CreateTempGo(meshData1.maxPId + "_" + meshData1.minPId + "__" + meshData2.maxPId + "_" + meshData2.minPId);

        if (step == AlignDebugStep.TempGO)
        {
            Debug.LogError("DebugTest Step:" + step);
            return null;//?????????1
        }

        //2.????????????????????????
        Vector3 offset = node1.GetCenterP() - tempCenter.transform.position;
        tempCenter.transform.position += offset;//??????1?????????

        if (step == AlignDebugStep.Offset)
        {
            Debug.Log($"offset:({offset.x},{offset.y},{offset.z})");
            Debug.LogError("DebugTest Step:" + step);
            return null;//?????????2
        }

        //3.??????tempCenter?????????????????????
        Quaternion qua1 = Quaternion.FromToRotation(node2.GetLongShortNormalNew(), node1.GetLongShortNormalNew());//?????????????????????????????????
        tempCenter.transform.rotation = qua1 * tempCenter.transform.rotation;//??????tempCenter?????????????????????
        //if(isShowDetail) node2.ShowDebugDetail(true);//?????????????????????????????????????????????????????????
        node2.CreateNormalLineAndPlane("_Update1");//?????????????????????????????????????????????

        //?????????????????????
        Vector3 normal12 = node1.GetLongShortNormalNew();
        Vector3 normal22 = node2.GetLongShortNormalNew();
        if(showLog)Debug.Log($"[After RotateNormal]normal12:({normal12.x},{normal12.y},{normal12.z}) |"+$"normal22:({normal22.x},{normal22.y},{normal22.z}) |"+$"normalAngle:{Vector3.Angle(normal12,normal22)}");

        if (step == AlignDebugStep.Normal)
        {
            Debug.LogError("DebugTest Step:" + step);
            return null;//?????????3
        }

        //?????????????????????????????????????????????
        float angle1=Vector3.Angle(node2.GetShortLineNew(), node1.GetShortLineNew());
        float angle2=Vector3.Angle(node2.GetLongLineNew(), node1.GetLongLineNew());
        var avgAngle = (angle1 + angle2) / 2f;
        if(showLog)Debug.Log($"Before ShortLineAngle:{angle1}, LongLineAngle:{angle2}, rotateAngle:{avgAngle} ");

        //return true;

        if(mode==AlignRotateMode.ShortQuat)
        {
            //4.??????tempCenter?????????????????????
            Quaternion qua2 = Quaternion.FromToRotation(node2.GetShortLineNew(), node1.GetShortLineNew());//?????????????????????????????????
            tempCenter.transform.rotation = qua2 * tempCenter.transform.rotation;//??????tempCenter?????????????????????
        }
        if (mode == AlignRotateMode.LongQuat)
        {
            //4.??????tempCenter?????????????????????
            Quaternion qua2 = Quaternion.FromToRotation(node2.GetLongLineNew(), node1.GetLongLineNew());//?????????????????????????????????
            tempCenter.transform.rotation = qua2 * tempCenter.transform.rotation;//??????tempCenter?????????????????????
        }
        if (mode == AlignRotateMode.ShortAngle)
        {
            tempCenter.transform.Rotate(tempCenter.transform.up, -angle1);
        }
        if (mode == AlignRotateMode.LongAngle)
        {
            tempCenter.transform.Rotate(tempCenter.transform.up, -angle2);
        }
        if (mode == AlignRotateMode.AvgAngle)
        {
            tempCenter.transform.Rotate(tempCenter.transform.up, -avgAngle);
        }

        node2.CreateNormalLineAndPlane("_Update2");//?????????????????????????????????????????????

        ////if(isShowDetail) node2.ShowDebugDetail(true);//????????????????????????????????????????????????????????? 


        //??????????????????
        angle1 =Vector3.Angle(node2.GetShortLineNew(), node1.GetShortLineNew());
        angle2=Vector3.Angle(node2.GetLongLineNew(), node1.GetLongLineNew());

        //node1.transform.position.PrintVector3("AlignMeshNode_Core_GetVertexDistanceEx");

        var dis2 = GetVertexDistanceEx(node1.transform, node2.transform);
        if(showLog)Debug.Log($"After ShortLine angle1:{angle1} | LongLine angle2:{angle2} | dis:{dis2}");
        
        if (step == AlignDebugStep.Rotate)
        {
            Debug.LogError("DebugTest Step:" + step);
            return null;//?????????4
        }


        
        AlignResult result=new AlignResult();
        result.ShortAngle=angle1;
        result.LongAngle=angle2;
        result.Distance=dis2;
        result.maxPId = node2.meshData.maxPId;
        result.minPId = node2.meshData.minPId;
        result.Angle1 = node1.GetLongShortAngle();
        result.Angle2 = node2.GetLongShortAngle();
        result.centerP = node2.meshData.GetCenterP();
        result.minP = node2.meshData.GetMinP();
        result.maxP = node2.meshData.GetMaxP();

        if(step==AlignDebugStep.None){ //????????????
            node2.transform.SetParent(parentOld);
            GameObject.DestroyImmediate(tempCenter);

            AlignResultComponent component=node2.gameObject.GetComponent<AlignResultComponent>();
            if(component==null){
                component=node2.gameObject.AddComponent<AlignResultComponent>();
            }
            component.Result = result;
        }
        else{
            AlignResultComponent component=tempCenter.AddComponent<AlignResultComponent>();
            component.Result = result;
            tempCenter.name+="|"+result;
        }

        if(showLog)Debug.LogWarning($"AlignMeshNode Time:{(DateTime.Now-start).TotalMilliseconds}ms,result:{result}");


        //node1.transform.position.PrintVector3("AlignMeshNode_Core_End");

        //if(isShowDetail) node2.ShowDebugDetail(true);//?????????????????????????????????????????????????????????

        // var angle22 = node1.GetLongShortAngle() - node2.GetLongShortAngle();
        // var long22 = node1.LongLineDistance - node2.LongLineDistance;
        // var short22= node1.ShortLineDistance - node2.ShortLineDistance;
        // var rate22 = node1.LongShortRate - node2.LongShortRate;
        // if(showLog)Debug.Log($"node1-node2 angle:{angle22},long:{long22},short:{short22},rate:{rate22} |");
        // node1-node2 angle:0.06484985,long:5.444884E-05,short:1.757592E-05,rate:0.0007190704 |
        //tryCount:64;Time:149.9519ms,minResult:IsZero:False;Distance:0.01115181;ShortAngle:0;LongAngle:0.0656106;Angle1:152.9654;Angle2:152.9005
        //AlignMeshNode Error Node1:r000000000000000_0_10_00100D3725A5B6E06,Node2:r000000000000000_0_11_00100D3725A5B6E06(Clone) | ShortLine angle1:0,LongLine angle2:0.0656106???dis2:0.01115237


        result.IsZero=dis2==0;//??????

        if(showLog)Debug.LogError($"IsRelativeZero: {long22<0.0001}|{short22<0.0001}|{angle1<=0.09f}|{angle2<=0.09f}|{dis2<=0.02f}");

        if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f&&dis2<=0.02f)//?????????????????????????????????????????????????????????????????????ICP??????
        {
            result.IsRelativeZero=true;
        }
        else if(angle1>0 || angle2>0 || dis2> DistanceSetting.zeroM)
        {
            result.IsRelativeZero=false;
        }
        else
        {
            result.IsRelativeZero=true;
        }

        if (step == AlignDebugStep.Finish)
        {
            Debug.LogError("DebugTest Step:" + step);
            return result;//?????????4
        }

        if(result.IsZero==false || result.IsRelativeZero==false){
             if(showLog)Debug.LogError($"AlignMeshNode Error Node1:{node1.name},Node2:{node2.name} | ShortLine angle1:{angle1},LongLine angle2:{angle2}???dis2:{dis2}");
        }

        return result;
    }

    // public static bool AlignMeshNode(MeshNode node1,MeshNode node2,int tryCount,bool isShowDetail)
    // {
    //     //node1?????????node2??????node1???
    //     Debug.Log($"AlignMeshNode,Node1:{node1.name},Node2:{node2.name}");

    //     DateTime start=DateTime.Now;
    //     //Debug.Log("AlignMeshNode Start");
    //     Vector3 longLine1 = node1.GetLongLine();
    //     Vector3 longLine2 = node2.GetLongLine();

    //     //????????????????????????
    //     Vector3 offset = node1.GetCenterP() - node2.GetCenterP();
    //     node2.transform.position += offset;

    //     //???????????????????????????????????????node2??????????????????
    //     GameObject tempCenter=GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //     tempCenter.name=node2.name+"_TempCenter";
    //     tempCenter.transform.position=node2.GetCenterP();
    //     Transform parentOld=node2.transform.parent;
    //     node2.transform.SetParent(tempCenter.transform);

    //     //???????????????????????????????????????
    //     float angle02=node2.GetLongShortAngle();
    //     float angle01=node1.GetLongShortAngle();
    //     Debug.Log($"node1 angle1:{angle01} |"+$"node2 angle1:{angle02}");

    //     //?????????????????????????????????????????????
    //     float angle1=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
    //     float angle2=Vector3.Angle(node2.GetLongLine(), node1.GetLongLine());
    //     Debug.Log($"Before ShortLine angle1:{angle1}"+$"LongLine angle2:{angle2}");


    //     Quaternion qua1 = Quaternion.FromToRotation(node2.GetLongLine(), node1.GetLongLine());//?????????????????????????????????
    //     tempCenter.transform.rotation = qua1 * tempCenter.transform.rotation;//??????tempCenter???????????????
    //     if(isShowDetail) node2.ShowDebugDetail(true);//?????????????????????????????????????????????????????????


    //     // Quaternion qua2 = Quaternion.FromToRotation(node2.GetShortLine(), node1.GetShortLine());//?????????????????????????????????
    //     // tempCenter.transform.rotation = qua2* tempCenter.transform.rotation;//??????tempCenter???????????????

    //     //??????????????????
    //     angle1=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
    //     angle2=Vector3.Angle(node2.GetLongLine(), node1.GetLongLine());
    //     Debug.Log($"After ShortLine angle1:{angle1}"+$"LongLine angle2:{angle2}");

    //     var shortNormal=Vector3.Cross(node2.GetShortLine(), node1.GetShortLine());//?????????????????????????????????
    //     tempCenter.transform.RotateAround(tempCenter.transform.position, shortNormal, -angle1);

    //     angle1=Vector3.Angle(node2.GetShortLine(), node1.GetShortLine());
    //     angle2=Vector3.Angle(node2.GetLongLine(), node1.GetLongLine());
    //     Debug.Log($"After ShortLine angle1:{angle1}"+$"LongLine angle2:{angle2}");

    //     // node2.transform.SetParent(parentOld);
    //     // GameObject.DestroyImmediate(tempCenter);

    //     // Debug.Log($"AlignMeshNode Time:{(DateTime.Now-start).TotalMilliseconds}ms");

    //     // if(angle1>0 || angle2>0)
    //     // {
    //     //     Debug.LogError($"Error Node1:{node1.name},Node2:{node2.name}|"+$"ShortLine angle1:{angle1}"+$"LongLine angle2:{angle2}");
    //     //     return false;
    //     // }

    //     return true;
    // }

    public static bool ReplaceToPrefab(GameObject newGo,GameObject model,int tryCount,Vector3 off,bool isShowDetail)
    {
        try
        {
            //Debug.LogError($"ReplaceToPrefab isShowDetail:{isShowDetail}");

            GameObject go1=model;//?????????
            GameObject go2=newGo;//?????????????????????????????????????????????????????????????????????????????????

            //1.InitMeshNodeInfo
            DateTime start1=DateTime.Now;
            MeshNode node1=go1.GetComponent<MeshNode>();
            if(node1==null){
                node1=go1.AddComponent<MeshNode>();
            }

            MeshNode node2=go2.GetComponent<MeshNode>();
            if(node2==null){
                node2=go2.AddComponent<MeshNode>();
            }

            node1.GetVertexCenterInfo(isShowDetail,false,off);
            node2.GetVertexCenterInfo(isShowDetail,false,off);

            //Debug.Log($"InitMeshNodeInfo Time:{(DateTime.Now-start1).TotalMilliseconds}ms");

            //2.AlignMeshNode
            var r=MeshHelper.AlignMeshNode(node1,node2,tryCount,isShowDetail,isShowDetail);//?????????????????????

            if(r!=null && r.IsZero){
                //3.????????????
                newGo.transform.SetParent(model.transform.parent);

                newGo.name=model.name+"_New";//New?????????????????????????????????
                //todo:????????????????????????????????????????????????????????????????????????????????????Renderer??????????????????????????????

                model.SetActive(false);//Destroy

#if UNITY_EDITOR
                GameObject root=PrefabUtility.GetOutermostPrefabInstanceRoot(model);
                if(root!=null){
                    PrefabUtility.UnpackPrefabInstance(root,PrefabUnpackMode.OutermostRoot,InteractionMode.UserAction);
                }
                GameObject.DestroyImmediate(model);//???????????????
#endif

                node1.ClearVertexes();
                GameObject.DestroyImmediate(node1);
                node2.ClearVertexes();
                GameObject.DestroyImmediate(node2);
            }
            else{

            }
            //return r;
            bool isSuccess=!(r==null || r.IsZero==false);
            return isSuccess;
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    public static MeshNode CopyGO(this MeshNode go2, bool isDebug = false)
    {
        GameObject copy = CopyGO(go2.gameObject);
        MeshNode node = copy.GetComponent<MeshNode>();
        return node;
    }

    public static double TotalCopyTime=0;

    public static int TotalCopyCount=0;

    public static GameObject CopyRenderer(this GameObject go2)
    {
        DateTime start = DateTime.Now;

        if (go2 == null) return null;
        Transform t1 = go2.transform;

        Transform parent1 = t1.parent;
        //go2.transform.SetParent(null);

        //List<Transform> children = new List<Transform>();
        //for(int i=0;)

        GameObject go2Copy = new GameObject(go2.name);

        //GameObject go2Copy=new GameObject(go2.name);

        Transform t2 = go2Copy.transform;
        t2.SetParent(parent1);//???????????????????????????null???
        t2.localPosition = t1.localPosition;
        t2.localRotation = t1.localRotation; //?????????Instantiate????????????????????????????????????(0,0,0)
        t2.localScale = t1.localScale; //?????????Instantiate????????????????????????????????????(1,1,1)
        //????????????????????????parent???localxxx??????????????????????????????!?

        double t = (DateTime.Now - start).TotalMilliseconds;
        TotalCopyTime += t;
        TotalCopyCount++;
        MeshRenderer renderer1 = go2.GetComponent<MeshRenderer>();
        MeshFilter filter1 = go2.GetComponent<MeshFilter>();


        MeshFilter filter2 = go2Copy.AddComponent<MeshFilter>();
        filter2.sharedMesh = filter1.sharedMesh;

        MeshRenderer renderer2 = go2Copy.AddComponent<MeshRenderer>();
        renderer2.sharedMaterials = renderer1.sharedMaterials;

        return go2Copy;
    }
    public static Transform CopyGO(this Transform go2, bool isDebug = false)
    {
        var goNew = CopyGO(go2.gameObject);
        return goNew.transform;
    }
    public static GameObject CopyGO(this GameObject go2,bool isDebug=false)
    {
        DateTime start=DateTime.Now;

        if (go2 == null)
        {
            Debug.LogError("CopyGO go2==null");
            return null;
        }
        Transform t1=go2.transform;

        Transform parent1=t1.parent;
        //go2.transform.SetParent(null);

        //List<Transform> children = new List<Transform>();
        //for(int i=0;)

        GameObject go2Copy = GameObject.Instantiate(go2);

        //EditorHelper.RemoveAllComponents(go2Copy);

        //GameObject go2Copy=new GameObject(go2.name);
        
        Transform t2=go2Copy.transform;
        t2.SetParent(parent1);//???????????????????????????null???
        t2.localPosition = t1.localPosition;
        t2.localRotation = t1.localRotation; //?????????Instantiate????????????????????????????????????(0,0,0)
        t2.localScale = t1.localScale; //?????????Instantiate????????????????????????????????????(1,1,1)
        //????????????????????????parent???localxxx??????????????????????????????!?

        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalCopyTime+=t;
        TotalCopyCount++;

        //var ids = go2Copy.GetComponentsInChildren<RendererId>();
        RendererId.NewIds(go2Copy);

        return go2Copy;
    }

    public static Transform CopyT(this Transform t){
        Transform t2=GameObject.Instantiate(t);
        return t2;
    }

    public static AlignDebugStep step;

    public static AlignRotateMode mode;


    public static GameObject ZeroPointGo;

    public static Transform SetParentZero(GameObject go)
    {
        if(ZeroPointGo==null){
            ZeroPointGo=GameObject.Find("ZeroPoint");
        }
        if(ZeroPointGo==null){
            ZeroPointGo=new GameObject("ZeroPoint");
        }
        //SaveParent(go.transform);
        var parent = go.transform.parent;
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(ZeroPointGo.transform);
        return parent;
    }

    public static void SaveParent(Transform t)
    {
        var parent = t.parent;
        if (parent == ZeroPointGo.transform) return;
        if (parents.ContainsKey(t))
        {
            var parentOld = parents[t];
            Debug.LogError($"MeshHelper.SaveParent t:{t} old:{parentOld},parent:{parent}");
        }
        else
        {
            parents.Add(t, parent);
        }
    }

    public static void LoadParent(Transform t)
    {
        var parent = t.parent;
        if (parent == ZeroPointGo.transform) return;
        if (parents.ContainsKey(t))
        {
            var parentOld = parents[t];
            t.SetParent(parentOld);
        }
        else
        {
            Debug.LogError($"MeshHelper.LoadParent parents.ContainsKey(t) == false t:{t}");
        }
    }

    private static Dictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();

    public static void ClearChildren(Transform t)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            children.Add(child);
        }
        foreach (var child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
    public static GameObject ShowVertexes(Vector3[] vs, float scale, string name)
    {
        GameObject go = new GameObject(name);
        ShowVertexes(vs, scale, go.transform);
        return go;
    }

        public static List<GameObject> ShowVertexes(Vector3[] vs,float scale,Transform parent){
        List<GameObject> games=new List<GameObject>();
        var vcount=vs.Length;
        for (int i = 0; i < vcount; i++)
        {
            Vector3 p = vs[i];
            GameObject go = VertexHelper.CreateWorldPoint(p, string.Format("[{0}]{1}", i, p),scale);
            go.transform.SetParent(parent);
            games.Add(go);
        }
        return games;
    }   

    //private static GameObject CreateWorldPoint(Vector3 p,string n,float scale)
    //{
    //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    go.transform.localScale = new Vector3(scale, scale, scale);
    //    go.transform.position = p;
    //    go.name = n;
    //    return go;
    //}


    //public static int GetInstanceID(MeshFilter mf)
    //{
    //    //return mf.sharedMesh.GetInstanceID();
    //    return mf.GetInstanceID();
    //}

    public static int GetInstanceID(MeshPoints mf)
    {
        //return mf.sharedMesh.GetInstanceID();
        return mf.InstanceId;
    }

    public static RTResult GetRTResult(Vector3[] vsFrom, Vector3[] vsTo)
    {
        var r1 = AcRigidTransform.ApplyTransformationN(vsFrom, vsTo);
        var vsFromNew1 = r1.ApplyPoints(vsFrom);
        var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo, false);
        return r1;
    }

    public static float GetRTDistance(Vector3[] vsFrom, Vector3[] vsTo)
    {
        var r1 = AcRigidTransform.ApplyTransformationN(vsFrom, vsTo);
        var vsFromNew1 = r1.ApplyPoints(vsFrom);
        var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo, false);
        return dis2;
    }


    public static RTResultList GetRTResultList(Vector3[] vsFrom, Vector3[] vsTo, int maxCount,float minDis)
    {
        //MeshHelper.ShowVertexes(vsTo, pScale, "vsTo");
        var dis1 = DistanceUtil.GetDistance(vsFrom, vsTo);
        Debug.LogError("TestICP2 dis1:" + dis1);

        RTResultList rList = new RTResultList();
        for (int i = 0; i < maxCount; i++)
        {
            DateTime start1 = DateTime.Now;
            float progress = (float)i / maxCount;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("TestICP2", $"{i}/{maxCount} {percents:F2}% of 100%", progress))
            {
                break;
            }
            //DateTime start21=DateTime.Now;
            //var psList=vsFrom.ToList();
            //Debug.LogError($"Time0:{(DateTime.Now-start21).TotalMilliseconds}ms");

            //DateTime start2=DateTime.Now;
            var vsFromCP1 = DistanceUtil.GetClosedPoints(vsTo, vsFrom.ToList());
            //Debug.LogError($"Time1:{(DateTime.Now-start2).TotalMilliseconds}ms");

            //DateTime start3=DateTime.Now;
            //MeshHelper.ShowVertexes(vsFromCP1, pScale, "vsFromCP_"+(i+1));
            var r1 = AcRigidTransform.ApplyTransformationN(vsFromCP1, vsTo);
            rList.Add(r1);
            //Debug.LogError($"Time2:{(DateTime.Now-start3).TotalMilliseconds}ms");

            var vsFromNew1 = r1.ApplyPoints(vsFrom);

            //MeshHelper.ShowVertexes(vsFromNew1, pScale, "vsFromNew_"+(i+1));

            // var goNew=MeshHelper.CopyGO(goOld);
            // goNew.name="vsFromNew_"+(i+1);
            // r1.ApplyMatrix(goNew.transform);
            // goOld=goNew;
            // MeshHelper.ShowVertexes(vsFromNew1, pScale, goNew.transform);

            var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo, i == maxCount - 1);
            //Debug.LogError($"TestICP1 dis{i+1}:" + dis2);
            Debug.LogError($"dis[{i + 1}] dis:{dis2}, Time:{(DateTime.Now - start1).TotalMilliseconds}ms");
            vsFrom = vsFromNew1;
            //if (dis2 <= 0.000001)
            if (dis2 <= minDis)
            {
                break;
            }
        }
        return rList;
    }
}

//[Serializable]
//public class AlignResult{

//    public int maxPId;

//    public int minPId;

//    public Vector3 centerP;

//    public Vector3 maxP;

//    public Vector3 minP;

//    public bool IsZero;//??????

//    public bool IsRelativeZero;//??????

//    public float Angle1;

//    public float Angle2;

//    public float ShortAngle;

//    public float LongAngle;

//    public float Distance;

//  public override string ToString()
//  {
//    return $"???IsZero:{IsZero};IsZero2:{IsRelativeZero}??? Distance:{Distance};ShortAngle:{ShortAngle};LongAngle:{LongAngle};Angle1:{Angle1};Angle2:{Angle2}";
//  }
//}

//public static class DistanceUtil
//{
//    public static float GetDistance(Unity.Collections.NativeArray<Vector3> points1, Vector3[] points2, bool showLog = false)
//    {
//        //Debug.Log("GetDistance:"+showLog);
//        DateTime start = DateTime.Now;
//        //float dis=-1;
//        float disSum = 0;

//        float distance = 0;
//        // var points1 = vertices1;
//        // var points2 = vertices2World;
//        //List<float> disList = new List<float>();
//        int zeroCount = 0;
//        int i = 0;

//        int maxCount = points1.Length;
//        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
//        int step = 1;
//        if(maxCount > zeroPMaxCount)
//        {
//            step = maxCount / zeroPMaxCount;
//        }

//        //for (; i < points1.Length & i<points2.Length; i++)
//        int count = 0;
//        for (; i < points1.Length ; i+= step, count++)
//        {
//            Vector3 p1 = points1[i];
//            Vector3 p2 = GetMinDistancePoint(p1, points2);
//            float d = Vector3.Distance(p1, p2);
//            disSum += d;//???????????????????????????
//            if (d <= DistanceSetting.zeroP)
//            {
//                if (showLog) Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
//                zeroCount++;
//                if (zeroCount > zeroPMaxCount)//???????????????????????????100????????????????????????????????????????????????
//                {
//                    //return distance;//????????????0???
//                    break;
//                }
//                else
//                {

//                }
//                d = 0;//????????????????????????zero??????0???????????????10???E-06???E-05???100?????????E-04???1000?????????E-03??????0.001??????????????????????????????????????????????????????
//            }
//            else
//            {
//                if (showLog) Debug.Log($"GetDistance2[{i}]d2:{d}|{distance}");
//            }
//            //disList.Add(d);
//            distance += d;

//            if (distance > DistanceSetting.zeroMMaxDis)//???????????????????????????????????????????????????????????????????????????
//            {
//                break;
//            }
//        }
//        if (showLog) Debug.Log($"GetVertexDistanceEx ??????:{(DateTime.Now - start).TotalMilliseconds:F2}ms?????????:{disSum:F7},??????:{distance:F7},??????:{i}/{points1.Length} count:{count}");
//        return distance;
//    }

//    // public static Vector3[] GetClosedPoints(Vector3[] points1, Vector3[] points2, bool showLog = false)
//    // {
//    //     //points1??????????????????vsTo,
//    //     //points2??????????????????vsFrom??????points2??????????????????points1??????????????????????????????P
//    //     Vector3[] points3 = new Vector3[points2.Length];//????????????,Pi<=P,???points2???????????????points1???????????????????????????????????????
//    //     //???????????????points1???????????????point2??????????????????????????????????????????
//    //     DateTime start = DateTime.Now;
//    //     //float disSum = 0;
//    //     int i = 0;
//    //     for (; i < points1.Length & i < points2.Length; i++)
//    //     {
//    //         Vector3 p1 = points1[i];
//    //         Vector3 p2 = GetMinDistancePoint(p1, points2);
//    //         points3[i] = p2;
//    //         //float d = Vector3.Distance(p1, p2);
//    //         //disSum += d;//???????????????????????????
//    //     }
//    //     if (showLog) Debug.Log($"GetClosedPoints ??????:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
//    //     return points3;
//    // }

//    public static Vector3[] GetClosedPoints(Vector3[] points1, List<Vector3> points2, bool showLog = false)
//    {
//        //points1??????????????????vsTo,
//        //points2??????????????????vsFrom??????points2??????????????????points1??????????????????????????????P
//        Vector3[] points3 = new Vector3[points2.Count];//????????????,Pi<=P,???points2???????????????points1???????????????????????????????????????
//        //???????????????points1???????????????point2??????????????????????????????????????????
//        DateTime start = DateTime.Now;
//        //float disSum = 0;
//        int i = 0;
//        int count1 = points1.Length;
//        int count2 = points2.Count;
//        for (; i < count1 & i < count2; i++)
//        {
//            Vector3 p1 = points1[i];
//            Vector3 p2 = GetMinDistancePoint(p1, points2,true);
//            points3[i] = p2;
//            //float d = Vector3.Distance(p1, p2);
//            //disSum += d;//???????????????????????????
//        }
//        if (showLog) Debug.Log($"GetClosedPoints ??????:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
//        return points3;
//    }

//    public static float GetDistance(List<Vector3> points1, List<Vector3> points2, bool showLog = false)
//    {
//        //Debug.Log("GetDistance:"+showLog);
//        DateTime start = DateTime.Now;
//        //float dis=-1;
//        float disSum = 0;

//        float distance = 0;
//        // var points1 = vertices1;
//        // var points2 = vertices2World;
//        //List<float> disList = new List<float>();
//        int zeroCount = 0;
//        int i = 0;

//        int maxCount = points1.Count;
//        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
//        int step = 1;
//        if (maxCount > zeroPMaxCount)
//        {
//            step = maxCount / zeroPMaxCount;
//        }
//        int count = 0;

//        for (; i < points1.Count ; i+=step,count++)
//        {
//            Vector3 p1 = points1[i];
//            Vector3 p2 = GetMinDistancePoint(p1, points2);
//            float d = Vector3.Distance(p1, p2);
//            disSum += d;//???????????????????????????
//            if (d <= DistanceSetting.zeroP)
//            {
//                if (showLog)
//                {
//                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
//                }
//                zeroCount++;
//                if (zeroCount > zeroPMaxCount)//???????????????????????????100????????????????????????????????????????????????
//                {
//                    //return distance;//????????????0???
//                    break;
//                }
//                else
//                {

//                }
//                d = 0;//????????????????????????zero??????0???????????????10???E-06???E-05???100?????????E-04???1000?????????E-03??????0.001??????????????????????????????????????????????????????
//            }
//            else
//            {
//                if (showLog)
//                {
//                    Debug.LogWarning($"GetDistance2[{i}]d2:{d}|{distance}");
//                }
//            }
//            //disList.Add(d);
//            distance += d;

//            if (distance > DistanceSetting.zeroMMaxDis)//???????????????????????????????????????????????????????????????????????????
//            {
//                break;
//            }
//        }

//        DisLog = $"GetVertexDistanceEx points1:{points1.Count} points2:{points2.Count} ??????:{(DateTime.Now - start).TotalMilliseconds:F2}ms?????????:{disSum:F7},??????:{distance:F7},??????:{i}/{points1.Count} count:{count}";
//        if (showLog) Debug.Log(DisLog);
//        return distance;
//    }

//    public static float GetDistance(Vector3[] points1,Vector3[] points2,bool showLog=false)
//    {
//        //Debug.Log("GetDistance:"+showLog);
//        DateTime start=DateTime.Now;
//        //float dis=-1;
//        float disSum=0;

//        float distance = 0;
//        // var points1 = vertices1;
//        // var points2 = vertices2World;
//        //List<float> disList = new List<float>();
//        int zeroCount = 0;
//        int i = 0;

//        int maxCount = points1.Length;
//        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
//        int step = 1;
//        if (maxCount > zeroPMaxCount)
//        {
//            step = maxCount / zeroPMaxCount;
//        }
//        int count = 0;

//        for (; i < points1.Length; i+=step, count++)
//        {
//            Vector3 p1 = points1[i];
//            Vector3 p2 = GetMinDistancePoint(p1, points2);
//            float d = Vector3.Distance(p1, p2);
//            disSum+=d;//???????????????????????????
//            if (d <= DistanceSetting.zeroP)
//            {
//                if (showLog)
//                {
//                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
//                }
//                zeroCount++;
//                if (zeroCount > zeroPMaxCount)//???????????????????????????100????????????????????????????????????????????????
//                {
//                    //return distance;//????????????0???
//                    break;
//                }
//                else
//                {

//                }
//                d=0;//????????????????????????zero??????0???????????????10???E-06???E-05???100?????????E-04???1000?????????E-03??????0.001??????????????????????????????????????????????????????
//            }
//            else{
//                if (showLog)
//                {
//                    Debug.LogWarning($"GetDistance2[{i}]d2:{d}|{distance}");
//                }
//            }
//            //disList.Add(d);
//            distance += d;

//            if (distance > DistanceSetting.zeroMMaxDis)//???????????????????????????????????????????????????????????????????????????
//            {
//                break;
//            }
//        }

//        DisLog = $"GetVertexDistanceEx points1:{points1.Length} points2:{points2.Length} ??????:{(DateTime.Now - start).TotalMilliseconds:F2}ms?????????:{disSum:F7},??????:{distance:F7},??????:{i}/{points1.Length} count:{count}";
//        if (showLog)Debug.Log(DisLog);
//        return distance;
//    }

//    public static string DisLog = "";

//    public static Vector3 GetMinDistancePoint(Vector3 p1, Vector3[] points)
//    {
//        //DateTime start = DateTime.Now;

//        float distance = float.MaxValue;
//        Vector3 result = Vector3.zero;
//        int index = 0;
//        for (int i = 0; i < points.Length; i++)
//        {
//            Vector3 p2 = points[i];
//            float dis = Vector3.Distance(p1, p2);
//            if (dis < distance)
//            {
//                distance = dis;
//                result = p2;
//                index = i;
//            }
//        }
//        //if(distance<zero)
//        //    DebugLog(string.Format("GetMinDistancePoint ??????:{0}s?????????:{1}?????????:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
//        return result;
//    }

//    public static float GetMinDistance(Vector3 p1, Vector3[] points)
//    {
//        float distance = float.MaxValue;
//        Vector3 result = Vector3.zero;
//        int index = 0;
//        for (int i = 0; i < points.Length; i++)
//        {
//            Vector3 p2 = points[i];
//            float dis = Vector3.Distance(p1, p2);
//            if (dis < distance)
//            {
//                distance = dis;
//                result = p2;
//                index = i;
//            }
//        }
//        return distance;
//    }

//    public static float GetMinDistance(Vector3 p1, List<Vector3> points)
//    {
//        float distance = float.MaxValue;
//        Vector3 result = Vector3.zero;
//        int index = 0;
//        for (int i = 0; i < points.Count; i++)
//        {
//            Vector3 p2 = points[i];
//            float dis = Vector3.Distance(p1, p2);
//            if (dis < distance)
//            {
//                distance = dis;
//                result = p2;
//                index = i;
//            }
//        }
//        return distance;
//    }

//    public static Vector3 GetMinDistancePoint(Vector3 p1, List<Vector3> points,bool isRemove=false)
//    {
//        //DateTime start = DateTime.Now;

//        float distance = float.MaxValue;
//        Vector3 result = Vector3.zero;
//        int index = 0;
//        for (int i = 0; i < points.Count; i++)
//        {
//            Vector3 p2 = points[i];
//            float dis = Vector3.Distance(p1, p2);
//            if (dis < distance)
//            {
//                distance = dis;
//                result = p2;
//                index = i;
//            }
//        }
//        if (isRemove)
//        {
//            points.RemoveAt(index);
//        }
//        //if(distance<zero)
//        //    DebugLog(string.Format("GetMinDistancePoint ??????:{0}s?????????:{1}?????????:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
//        return result;
//    }

//}

public enum AlignDebugStep
{
    None,
    Start,
    TempGO,
    Offset,
    Normal,
    Rotate,
    Finish
}

public enum AlignRotateMode
{
    ShortQuat,LongQuat,ShortAngle,LongAngle,AvgAngle
}

//public interface IGameObject
//{
//    public GameObject GetGameObject();

//    public string GetName();
//}

[System.Serializable]
public class MeshReplaceTarget: IGameObject,IComparable<MeshReplaceTarget>
{
    public GameObject gameObject;

    public int vertexCount = 0;

    public void DestroyImmediate()
    {
#if UNITY_EDITOR
        EditorHelper.UnpackPrefab(gameObject);
        GameObject.DestroyImmediate(gameObject);
#endif
    }

    public float dis;

    public GameObject newGo;

    public string name
    {
        get
        {
            if (gameObject == null) return "";
            return gameObject.name;
        }
    }

    public Transform transform
    {
        get
        {
            if (gameObject == null) return null;
            return gameObject.transform;
        }
    }

    public MeshReplaceTarget(GameObject go)
    {
        this.gameObject = go;

        MeshPoints ps = new MeshPoints(go);

        this.vertexCount = ps.vertexCount;
    }

    public void SetActive(bool isActive)
    {
        if (gameObject)
        {
            gameObject.SetActive(isActive);
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetName()
    {
        return $"{name}[{vertexCount}]";
    }

    public int CompareTo(MeshReplaceTarget other)
    {
        int r = other.vertexCount.CompareTo(this.vertexCount);
        if (r == 0)
        {
            r = this.name.CompareTo(other.name);
        }
        return r;
    }
}

[System.Serializable]
public class MeshReplaceItem
{
    public GameObject prefab;

    public int prefabVertexCount = 0;
    //public List<float> targetDistance = new List<float>();
    //public List<GameObject> targetList = new List<GameObject>();
    //public List<GameObject> targetListNew = new List<GameObject>();

    public List<MeshReplaceTarget> targetList = new List<MeshReplaceTarget>();

    public void AddTarget(GameObject go)
    {
        foreach(var target in targetList)
        {
            if (target.gameObject == go)
            {
                return;
            }
        }
        targetList.Add(new MeshReplaceTarget(go));
    }

    public void Compare(ProgressArg p1)
    {
        if (prefab == null)
        {
            Debug.LogError($"MeshReplaceItem.Compare prefab == null");
            return;
        }
        //targetListNew.Clear();
        //targetDistance.Clear();
        for (int i = 0; i < targetList.Count; i++)
        {
            var target = targetList[i];
            var p2 = new ProgressArg("ReplaceItem", i, targetList.Count, target);
            p1.AddSubProgress(p2);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                return;
            }
            if (target == null) continue;
            if (target.gameObject == prefab) continue;
            //Debug.Log($"Compare prefab:{prefab} target:{target}");
            float dis = MeshHelper.GetVertexDistanceEx(prefab.transform, target.transform,"",false,true);
            if (dis > 0.0001f)
            {
                Debug.LogError($"Replace[{i}][{p1.progress:P2}] prefab:{prefab.name} target:{target.name} dis:{dis}");
            }
            else
            {
                Debug.Log($"Replace[{i}][{p1.progress:P2}] prefab:{prefab.name} target:{target.name} dis:{dis}");
            }
            //targetDistance.Add(dis);
            target.dis = dis;
        }
    }

    public int Count
    {
        get
        {
            return targetList.Count;
        }
    }

    public bool Replace(bool isDestoryOriginal,bool isHidden, TransfromAlignSetting transfromReplaceSetting,ProgressArg p1)
    {
        if (prefab == null)
        {
            Debug.LogError($"MeshReplaceItem.Replace prefab == null");
            return true;
        }
        //MeshHelper.ReplaceByPrefab(target, prefab);
        //StartCoroutine(MeshHelper.ReplaceByPrefabEx(target, prefab,"", "",isDestoryOriginal));

        //targetListNew.Clear();
        //targetDistance.Clear();

        for (int i = 0; i < targetList.Count; i++)
        {
            var target = targetList[i];
            var p2 = new ProgressArg("ReplaceItem", i, targetList.Count, target);
            p1.AddSubProgress(p2);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                return false;
            }
            if (target == null) continue;
            if (target.gameObject == prefab) continue;
            GameObject newGo = InnerMeshHelper.ReplaceGameObject(target.gameObject, prefab, isDestoryOriginal, transfromReplaceSetting);
            float dis = MeshHelper.GetVertexDistanceEx(newGo, target.gameObject);
            if (dis > 0.0001f)
            {
                Debug.LogError($"Replace[{i}][{p1.progress:P2}] prefab:{prefab.name} target:{target.name} dis:{dis} transfromAlignSetting:{transfromReplaceSetting}");
            }
            else
            {
                Debug.Log($"Replace[{i}][{p1.progress:P2}] prefab:{prefab.name} target:{target.name} dis:{dis}  transfromAlignSetting:{transfromReplaceSetting}");
            }
            //targetDistance.Add(dis);
            target.dis = dis;
            target.newGo = newGo;

            //targetListNew.Add(newGo);
            if(isHidden)
                target.SetActive(false);
        }
        return true;
    }

    public void ClearNewGos()
    {
        //foreach (var go in targetListNew)
        //{
        //    if (go == null) continue;
        //    GameObject.DestroyImmediate(go);
        //}
        //targetListNew.Clear();

        foreach (var target in targetList)
        {
            if (target == null) continue;
            target.SetActive(true);
        }
        //targetDistance.Clear();
    }

    public void ApplyNewGos()
    {

        foreach (var target in targetList)
        {
            GameObject targetNew = target.newGo;
            if (targetNew == null) continue;
            targetNew.name = targetNew.name.Replace("_New", "");

            MeshRendererInfo info = targetNew.GetComponent<MeshRendererInfo>();
            RendererId id1 = target.gameObject.GetComponent<RendererId>();
            RendererId id2 = targetNew.GetComponent<RendererId>();
            if (id1 != null && id2 != null)
            {
                id2.Id = id1.Id;
            }
            if (info)
            {
                GameObject.DestroyImmediate(info);
            }
        }

        foreach (var target in targetList)
        {
            if (target == null) continue;
            target.DestroyImmediate();
        }

        targetList.Clear();
        //targetList.AddRange(targetListNew);
        //targetListNew.Clear();
    }
}

//[System.Serializable]
//public class TransfromAlignSetting
//{
    
//    public bool SetPosition = true;
//    public bool SetScale = true;
//    public bool SetRotation = true;
//    public TransfromAlignMode Align = TransfromAlignMode.Max;
//    public bool SetPosX = true;
//    public bool SetPosY = true;
//    public bool SetPosZ = true;

//    //public bool SetPosByMinX = true;
//    //public bool SetPosByMinY = true;
//    //public bool SetPosByMinZ = true;
//}

//public enum TransfromAlignMode
//{
//    Pivot,Min,Max,Center, MinMax,MaxMin
//}

public static class MeshAlignHelper
{
    public static void AcRTAlign(GameObject from, GameObject to,bool isShowLog=true)
    {
        DateTime start = DateTime.Now;

        if (from == null)
        {
            Debug.LogError($"AcRTAlign from == null");
            return;
        }
        if (to == null)
        {
            Debug.LogError($"AcRTAlign to == null");
            return;
        }

#if UNITY_EDITOR
        GameObject root1 = PrefabUtility.GetOutermostPrefabInstanceRoot(to);
        if (root1 != null)
        {
            PrefabUtility.UnpackPrefabInstance(root1, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        }
        GameObject root2 = PrefabUtility.GetOutermostPrefabInstanceRoot(from);
        if (root2 != null)
        {
            PrefabUtility.UnpackPrefabInstance(root2, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
        }
#endif

        var pTo=MeshHelper.SetParentZero(to);
        var pFrom=MeshHelper.SetParentZero(from);

        var mfFrom = new MeshPoints(from);
        var mfTo = new MeshPoints(to);

        MeshJobHelper.NewThreePointJobs(new MeshPoints[] { mfFrom, mfTo }, 10000);
        AcRigidTransform.RTAlign(mfFrom, mfTo, isShowLog);

        to.transform.SetParent(pTo);
        from.transform.SetParent(pFrom);

        //Debug.Log($"AcRTAlign End Time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }

    public static bool AcRTAlignJob(GameObject from, GameObject to)
    {
        DateTime start = DateTime.Now;
        var pTo = MeshHelper.SetParentZero(to);
        var pFrom = MeshHelper.SetParentZero(from);

        var mfFrom = new MeshPoints(from);
        var mfTo = new MeshPoints(to);

        AcRTAlignJobResult.CleanResults();
        AcRtAlignJobArg.CleanArgs();
        MeshJobHelper.NewThreePointJobs(new MeshPoints[] { mfFrom, mfTo }, 10000);
        bool r=MeshJobHelper.DoAcRTAlignJob(mfFrom, mfTo, 2);

        to.transform.SetParent(pTo);
        from.transform.SetParent(pFrom);

        Debug.Log($"AcRTAlignJob End Time:{(DateTime.Now - start).TotalMilliseconds}ms");
        return r;
    }
}
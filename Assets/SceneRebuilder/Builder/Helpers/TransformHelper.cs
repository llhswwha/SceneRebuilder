using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformHelper
{
    public static GameObject CreateBoundsCube(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError($"CreateBoundsCube go==null go:{go}");
            return null;
        }
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogError($"CreateBoundsCube MeshFilter==null go:{go}");
            return null;
        }
        if (mf.sharedMesh == null)
        {
            Debug.LogError($"CreateBoundsCube sharedMesh==null go:{go}");
            return null; 
        }
        Bounds b = GetBounds(go, new Vector3(1, 1, 1));
        return CreateBoundsCube(b, go.name + "_Box", go.transform.parent);
    }

    public static Bounds GetBounds(GameObject go,Vector3 scale)
    {
        //ShowRenderers();

        List<Renderer> renderers = new List<Renderer>();
        //foreach (var go in BoundsGos)
        {
            renderers.AddRange(go.GetComponentsInChildren<Renderer>(true));
        }
        Bounds bounds = ColliderHelper.CaculateBounds(renderers);
        var size = bounds.size;
        bounds.size = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        return bounds;
    }

    public static GameObject CreateBoundsCube(Bounds bounds, string n, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoundsBox>();
        cube.SetActive(true);
        cube.name = n;
        cube.transform.position = bounds.center;
        cube.transform.localScale = bounds.size;
        cube.transform.SetParent(parent);
        return cube;
    }

    public static Dictionary<Transform, string> PathDict = new Dictionary<Transform, string>();

    public static string GetPath<T>(T t, Transform root = null, string split = ">") where T :MonoBehaviour
    {
        return GetPath(t.transform, root, split);
    }

    public static string GetPath(Transform t, Transform root = null, string split = ">")
    {
        //string path = t.name;
        //t = t.parent;
        //while (t != null)
        //{
        //    path = t.name + split + path;
        //    if (t == root)
        //    {
        //        break;
        //    }
        //    t = t.parent;
        //}
        //return path;

        List<Transform> ancestors = GetAncestors(t, root);

        string path = "";
        for (int i = 0; i < ancestors.Count; i++)
        {
            Transform a = ancestors[i];
            path += a.name;
            if (i < ancestors.Count - 1)
            {
                path += split;
            }
        }
        return path;
    }

    public static List<Transform> GetAncestors(Transform t, Transform root = null)
    {
        List<Transform> ancestors = new List<Transform>();
        ancestors.Add(t);
        string path = t.name;
        t = t.parent;
        while (t != null)
        {
            ancestors.Add(t);
            if (t == root)
            {
                break;
            }
            t = t.parent;
        }
        ancestors.Reverse();
        return ancestors;
    }

    public static void MoveToNewRoot(GameObject target,string newRootName)
    {
        GameObject newRoot = GameObject.Find(newRootName);
        if (newRoot == null)
            newRoot = new GameObject(newRootName);
        MeshFilter[] gos = target.GetComponentsInChildren<MeshFilter>(true);
        foreach (MeshFilter go in gos)
        {
            TransformHelper.MoveGameObject(go.transform, newRoot.transform);
        }
        Debug.LogError($"MoveToZero gos:{gos.Length}");
    }

    public static void MoveGameObject(Transform target, Transform newRoot)
    {
        List<Transform> path = TransformHelper.GetAncestors(target, null);
        Transform newP = TransformHelper.FindOrCreatePath(newRoot, path, false);
        target.SetParent(newP.transform);
    }

    public static Transform FindOrCreatePath(Transform root,List<Transform> path,bool isDebug)
    {
        Transform parent = root;
        for(int j = 0; j < path.Count-1; j++)
        {
            Transform node = path[j];
            Transform child = FindOrCreate(parent, node.name);
            if (isDebug)
            {
                Debug.LogError($"FindOrCreatePath[{j}/{path.Count}] root:{root.name} node:{node.name} parent:{parent.name} ");
            }
            parent = child;
        }
        return parent;
    }

    public static Transform FindOrCreate(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == name)
            {
                return child;
            }
        }
        GameObject go = new GameObject(name);
        go.transform.SetParent(root);
        return go.transform;
    }
    public static string GetPathWithActive<T>(T t) where T : MonoBehaviour
    {
        if (t == null) return "NULL";
        return GetPathWithActive(t.transform);
    }

    public static string GetPathWithActive(Transform t)
    {
        string path = "";
        while (t != null)
        {
            path = $"{t.name}({t.gameObject.activeSelf})>{path}";
            t = t.parent;
        }
        return path;
    }

    public static void ClearComponentsAll<T>()
    where T : Component
    {
        T[] cs = GameObject.FindObjectsOfType<T>(true);
        TransformHelper.ClearComponents(cs);
    }

    public static void ClearComponentsAllGo<T>()
    where T : Component
    {
        T[] cs = GameObject.FindObjectsOfType<T>(true);
        TransformHelper.ClearComponentGos(cs);
    }

    public static void ClearComponentGos<T>(GameObject obj) where T : Component
    {
        var ids = obj.GetComponentsInChildren<T>(true);
        foreach (var id in ids)
        {
            if (id == null) continue;
            if (id.gameObject == null) continue;
            GameObject.DestroyImmediate(id.gameObject);
        }
        Debug.Log($"ClearComponentGos[{typeof(T)}] ids:{ids.Length}");
    }

    public static void ClearNotComponentGos<T>(GameObject obj) where T : Component
    {
        var ids = obj.GetComponentsInChildren<MeshRenderer>(true);
        int count1 = 0;
        int count2 = 0;
        foreach (var id in ids)
        {
            T t = id.GetComponent<T>();
            if (t != null)
            {
                count1++;
                continue;
            }
            count2++;
            GameObject.DestroyImmediate(id.gameObject);
        }
        Debug.Log($"ClearNotComponentGos[{typeof(T)}] ids:{ids.Length} count1:{count1} count2:{count2}");
    }

    public static void ClearComponents<T>(GameObject obj) where T : Component
    {
        var ids = obj.GetComponentsInChildren<T>(true);
        foreach (var id in ids)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] ids:{ids.Length}");
    }

    public static void ClearComponents<T>(GameObject[] objs) where T : Component
    {
        foreach (var obj in objs)
        {
            ClearComponents<T>(obj);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] objs:{objs.Length}");
    }

    public static void ClearComponents<T>(T[] objs) where T : Component
    {
        foreach (var obj in objs)
        {
            GameObject.DestroyImmediate(obj);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] objs:{objs.Length}");
    }

    public static void ClearComponentGos<T>(T[] objs) where T : Component
    {
        foreach (var obj in objs)
        {
            GameObject.DestroyImmediate(obj.gameObject);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] objs:{objs.Length}");
    }

    public static void SetCollidersEnabled<T>(GameObject[] objs,bool enbled) where T : Collider
    {
        foreach (var obj in objs)
        {
            //ClearComponents<T>(obj);
            T com = obj.GetComponent<T>();
            com.enabled = enbled;
        }
    }

    //public static void ClearComponents<T>(GameObject root) where T : Component
    //{
    //    T[] obbs = root.GetComponentsInChildren<T>(true);
    //    foreach (var obb in obbs)
    //    {
    //        if (obb == null) continue;
    //        GameObject.DestroyImmediate(obb);
    //    }
    //}


    public static GameObject CreateBoxLine(Vector3 p1, Vector3 p2, float size, string n, Transform pt)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=(p1+p2)/2;
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward = p2 - p1;
        Vector3 scale = new Vector3(size, size, Vector3.Distance(p2, p1));
        g1.transform.localScale = scale;
        g1.name = n;
        //g1.transform.SetParent(this.transform);
        if (pt != null)
            g1.transform.SetParent(pt);
        return g1;
    }

    public static GameObject CreateSubTestObj(string objName, Transform parent)
    {
        GameObject objTriangles = new GameObject(objName);
        objTriangles.AddComponent<DebugInfoRoot>();
        objTriangles.transform.SetParent(parent);
        objTriangles.transform.localPosition = Vector3.zero;
        objTriangles.transform.localRotation = Quaternion.identity;
        return objTriangles;
    }

    public static List<Transform> GetChildrenNoLOD(GameObject root)
    {
        List<Transform> list = new List<Transform>();
        GetMeshPointsNoLOD(root.transform, list);
        return list;
    }

    public static void GetMeshPointsNoLOD(Transform root, List<Transform> list)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            var lod = child.GetComponent<LODGroup>();
            if (lod != null)
            {
                continue;
            }
            var render = child.GetComponent<MeshRenderer>();
            if(render!=null)
                list.Add(child);
            var filter = child.GetComponent<MeshFilter>();
            if (filter == null)
            {

            }
            GetMeshPointsNoLOD(child, list);
        }
    }

    public static Transform FindClosedTransform(IEnumerable<Transform> ts, Vector3 pos, bool isUseCenter=false)
    {
        float minDis = float.MaxValue;
        Transform minModel = null;
        foreach (var t in ts)
        {
            //float dis = Vector3.Distance(t.position, pos);
            Vector3 posT = t.transform.position;
            if (isUseCenter)
            {
                MeshRendererInfo info0 = MeshRendererInfo.GetInfo(t.gameObject, false);
                posT = info0.center;
            }
            float dis = Vector3.Distance(posT, pos);
            if (minDis > dis)
            {
                minDis = dis;
                minModel = t;
            }
        }
        return minModel;
    }

    public static T FindClosedComponent<T>(IEnumerable<T> ts, Vector3 pos,bool isUseCenter = false) where T :Component
    {
        float minDis = float.MaxValue;
        T minModel = null;
        foreach (var t in ts)
        {
            Vector3 posT = t.transform.position;
            if (isUseCenter)
            {
                MeshRendererInfo info0 = MeshRendererInfo.GetInfo(t.gameObject, false);
                posT = info0.center;
            }
            float dis = Vector3.Distance(posT, pos);
            if (minDis > dis)
            {
                minDis = dis;
                minModel = t;
            }
        }
        return minModel;
    }

    public class ClosedTransform
    {
        public Transform t;

        public float dis;

        public ClosedTransform(Transform t,float d)
        {
            this.t = t;
            this.dis = d;
        }

        public override string ToString()
        {
            if (t == null)
            {
                return $"t:NULL dis:{dis}";
            }
            return $"t:{t.name} dis:{dis}";
        }
    }

    public static ClosedTransform FindClosedComponentEx<T>(IEnumerable<T> ts, Transform target, bool isUseCenter = false) where T : Component
    {
        float minDis = float.MaxValue;
        Transform minModel = null;
        var pos = target.position;
        foreach (var t in ts)
        {
            if (t == null) continue;
            if (t.transform == target) continue;
            Vector3 posT = t.transform.position;
            if (isUseCenter)
            {
                MeshRendererInfo info0 = MeshRendererInfo.GetInfo(t.gameObject, false);
                posT = info0.center;
            }
            float dis = Vector3.Distance(posT, pos);
            if (minDis > dis)
            {
                minDis = dis;
                minModel = t.transform;
            }
        }
        return new ClosedTransform(minModel,minDis);
    }

    public static string GetPrefix(string n)
    {
        return GetPrefix(n, new char[] { ' ', '_' });
    }

    public static string GetPrefix(string n, char[] dividers)
    {
        //var n = item.name;
        int id = 0;
        foreach(var c in dividers)
        {
            int id1 = n.LastIndexOf(c);
            if (id1 > id)
            {
                id = id1;
            }
        }
        //int id1 = n.LastIndexOf(' ');
        ////int id2 = n.LastIndexOf('-');
        //int id3 = n.LastIndexOf('_');
        ////12-3 2
        //if (id1 > id)
        //{
        //    id = id1;
        //}
        ////if (id2 > id)
        ////{
        ////    id = id2;
        ////}
        //if (id3 > id)
        //{
        //    id = id3;
        //}
        ////if (id == 0 && n.Length>9)
        ////{
        ////    id = 9;//？？？
        ////}
        ////最后一个是数字或者英文字母的情况，最后是多个数字的情况
        string pre = n;
        string after = "";
        if (id > 0)
        {
            pre = n.Substring(0, id);
            after = n.Substring(id + 1);
            //ModelClassDict_Auto.AddModel(pre, item);
        }
        //Debug.LogError($"GetPrefix name:{n} id1:{id1} id2:{id2} id3:{id3} id:{id} pre:{pre} after:{after}");
        return pre;
    }

    internal static GameObject ShowLocalPoint(Vector3 point, float pointScale, Transform transform1, Transform transform2)
    {
        GameObject objPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        objPoint.transform.SetParent(transform1);
        if (point.x == float.NaN)
        {
            Debug.LogError($"ShowLocalPoint NaN transform1:{transform1} transform2:{transform2} point:{point}");
            return objPoint;
        }
        try
        {
            //objPoint.name = $"Point[{i + 1}][{j + 1}]({p.Point})";
            //objPoint.name = $"Point[{j + 1}]({p.Point})";
            objPoint.name = $"Point({point.x},{point.y},{point.z})";
             objPoint.transform.localPosition = point;
            objPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            if (transform2 != null)
                objPoint.transform.SetParent(transform2);
            return objPoint;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ShowLocalPoint transform1:{transform1} transform2:{transform2} point:{point} Exception:{ex}");
            return objPoint;
        }
        
    }

    internal static GameObject ShowPoint(Vector3 point, float pointScale, Transform transform1)
    {
        GameObject objPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //objPoint.name = $"Point[{i + 1}][{j + 1}]({p.Point})";
        //objPoint.name = $"Point[{j + 1}]({p.Point})";
        objPoint.name = $"Point({point.x},{point.y},{point.z})";
        objPoint.transform.position = point;
        objPoint.transform.SetParent(transform1);
        objPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);
        return objPoint;
    }

    public static bool IsSameName(string name,string name2)
    {
        string pre1 = GetPrefix(name);
        string pre2 = GetPrefix(name2);
        return pre1 == pre2;
    }

    //public static List<Transform> FindClosedTransform(List<Transform> ts2, string name)
    //{
    //    List<Transform> result = new List<Transform>();
    //    foreach (var t2 in ts2)
    //    {
    //        if (IsSameName(t2.name, name))
    //        {
    //            result.Add(t2);
    //        }
    //    }
    //    if (result.Count == 1)
    //    {
    //        Debug.Log($"FindListByName name:{name} result:{result[0]}");
    //    }
    //    else if (result.Count == 0)
    //    {
    //        Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
    //    }
    //    return result;
    //}

    public static Transform FindOneByName(string name)
    {
        var ts2 = GameObject.FindObjectsOfType<Transform>();
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            //if (IsSameName(t2.name, name))
            if (t2.name == name)
            {
                result.Add(t2);
            }
        }

        Transform t = null;
        if (result.Count == 1)
        {
            t = result[0];
        }
        else if (result.Count == 0)
        {
            Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
        }
        else
        {
            Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
        }
        return t;
    }

    public static Dictionary<IdInfo, Transform> idInfoDict = new Dictionary<IdInfo, Transform>();

    public static Transform FindOneByNameAndPos(TransformDictionary ts2, IdInfo idInfo, float minDis, bool isFindByName)
    {
        if (idInfoDict.ContainsKey(idInfo))
        {
            return idInfoDict[idInfo];
        }
        Vector3 pos = idInfo.GetPosition();
        Vector3 center = idInfo.GetCenter();
        string name = idInfo.name;
        //var ts2 = GameObject.FindObjectsOfType<Transform>();
        List<Transform> result = ts2.FindModelsByPosAndName(idInfo, isFindByName);
        //foreach (var t2 in ts2)
        //{
        //    //if (IsSameName(t2.name, name))
        //    if (t2.name == name)
        //    {
        //        result.Add(t2);
        //    }
        //}

        Transform t = null;
        if (result == null)
        {
            Debug.LogError($"FindOneByName Error[{errorCount++}] result == null name:{name} pos:{pos} center:{center} ts2:{ts2.Count} 【{idInfo}】");
        }
        else if (result.Count == 0)
        {
            Debug.LogError($"FindOneByName Error[{errorCount++}] result.Count == 0 result:{result.Count} name:{name} pos:{pos} ts2:{ts2.Count} 【{idInfo}】");
        }
        else if (result.Count == 1)
        {
            t = result[0];
        }
        else
        {
            //Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
            t = FindClosedTransform(result, pos);
        }
        if (t != null)
        {
            if(idInfo.HasMesh)
            {
                float disOfPos = Vector3.Distance(t.position, pos);
                float disOfCenter = Vector3.Distance(MeshRendererInfo.GetCenterPos(t.gameObject), center);
                if (t.name == name || t.name == (idInfo.parent + "_" + name))
                {
                    if (disOfPos < minDis)
                    {

                    }
                    else if (disOfCenter < minDis)
                    {

                    }
                    else
                    {
                        Debug.LogError($"FindOneByName Error1[{errorCount++}] dis>=minDis result:{result.Count} name:{name} pos:{pos} dis:{disOfPos} disOfCenter:{disOfCenter} minDis:{minDis} t:{t}  {idInfo.GetFullString()}  path:{TransformHelper.GetPath(t)}");
                    }
                }
                else
                {
                    Debug.LogError($"FindOneByName Error2[{errorCount++}] t.name != name result:{result.Count} name:{name} pos:{pos} dis:{disOfPos} disOfCenter:{disOfCenter} minDis:{minDis} t:{t}  {idInfo.GetFullString()}  path:{TransformHelper.GetPath(t)}");
                    return null;
                }
            }
            else
            {
                float disOfPos = Vector3.Distance(t.position, pos);
                if (t.name == name || t.name == (idInfo.parent + "_" + name))
                {
                    if (disOfPos < minDis)
                    {

                    }
                    else
                    {
                        Debug.LogError($"FindOneByName Error3[{errorCount++}] dis>=minDis result:{result.Count} name:{name} pos:{pos} dis:{disOfPos} minDis:{minDis} t:{t}  {idInfo.GetFullString()}  path:{TransformHelper.GetPath(t)}");
                    }
                }
                else
                {
                    Debug.LogError($"FindOneByName Error4[{errorCount++}] t.name != name result:{result.Count} name:{name} pos:{pos} dis:{disOfPos} minDis:{minDis} t:{t}  {idInfo.GetFullString()}  path:{TransformHelper.GetPath(t)}");
                    return null;
                }
            }
            
        }
        if (t != null)
        {
            idInfoDict.Add(idInfo, t);
        }
        
        return t;
    }

    public static int errorCount = 0;

    //public static Transform FindOneByNameAndPos(TransformDictionary ts2,string name, Vector3 pos, Vector3 center, float minDis, bool isFindByName)
    //{
    //    //var ts2 = GameObject.FindObjectsOfType<Transform>();
    //    List<Transform> result = ts2.FindModelsByPosAndName(pos, center, name, isFindByName);
    //    //foreach (var t2 in ts2)
    //    //{
    //    //    //if (IsSameName(t2.name, name))
    //    //    if (t2.name == name)
    //    //    {
    //    //        result.Add(t2);
    //    //    }
    //    //}

    //    Transform t = null;
    //    if (result == null)
    //    {
    //        Debug.LogError($"FindOneByName result == null name:{name} pos:{pos} center:{center} ts2:{ts2.Count}");
    //    }
    //    else if (result.Count == 0)
    //    {
    //        Debug.LogError($"FindOneByName result.Count == 0 result:{result.Count} name:{name} pos:{pos} ts2:{ts2.Count}");
    //    }
    //    else if (result.Count == 1)
    //    {
    //        t = result[0];
    //    }
    //    else
    //    {
    //        //Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
    //        t = FindClosedTransform(result, pos);
    //    }
    //    if (t != null)
    //    {
    //        float dis = Vector3.Distance(t.position, pos);
    //        if (dis < minDis)
    //        {

    //        }
    //        else
    //        {
    //            Debug.LogError($"FindOneByName result.Count != 1 result:{result.Count} name:{name} pos:{pos} dis:{dis} minDis:{minDis} t:{t}");
    //        }
    //    }
        
    //    return t;
    //}

    public static List<Transform> FindListByName(string name)
    {
        var ts2 = GameObject.FindObjectsOfType<Transform>();
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            //if (IsSameName(t2.name, name))
            if(t2.name==name)
            {
                result.Add(t2);
            }
        }
        if (result.Count == 1)
        {
            //Debug.Log($"FindListByName name:{name} result:{result[0]}");
        }
        else if (result.Count == 0)
        {
            Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
        }
        else
        {
            Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
        }
        return result;
    }

    public static List<Transform> FindListByName(List<Transform> ts2, string name)
    {
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            if (IsSameName(t2.name,name))
            {
                result.Add(t2);
            }
        }
        if (result.Count == 1)
        {
            Debug.Log($"FindListByName name:{name} result:{result[0]}");
        }
        else if (result.Count == 0)
        {
            Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
        }
        else
        {
            Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
        }
        return result;
    }

    public static Transform FindByNameAndPosition(List<Transform> ts2, string name,Vector3 pos)
    {
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            if (IsSameName(t2.name, name))
            {
                result.Add(t2);
            }
        }
        Transform closedTransform = FindClosedTransform(result, pos);
        //float dis1 = Vector3.Distance(closedTransform.position, pos);
        return closedTransform;
    }

    public static List<Transform> FindListByName(Transform[] ts2, string name)
    {
        List<Transform> result = new List<Transform>();
        foreach (var t2 in ts2)
        {
            if (IsSameName(t2.name, name))
            {
                result.Add(t2);
            }
        }
        if (result.Count == 1)
        {
            Debug.Log($"FindListByName name:{name} result:{result[0]}");
        }
        else if (result.Count == 0)
        {
            if (!name.StartsWith("0028-240050-"))
            {
                Debug.LogError($"FindListByName result.Count != 1 result:{result.Count} name:{name} ");
            }
        }
        else
        {
            Debug.LogWarning($"FindListByName name:{name} result:{result.Count}_{result[0]}");
        }
        return result;
    }


    public static List<Transform> FindTransforms(Transform root,string key)
    {
        List<Transform> list = new List<Transform>();
        var childrens = root.GetComponentsInChildren<Transform>(true);
        foreach(var child in childrens)
        {
            if (child.name.Contains(key))
            {
                list.Add(child);
            }
        }
        return list;
    }

    public static List<Transform> FindAllTransforms(Transform root, string key)
    {
        Dictionary<Transform, Transform> dict = new Dictionary<Transform, Transform>();
        List<Transform> list = FindTransforms(root, key);
        foreach(var item in list)
        {
            var ts = item.GetComponentsInChildren<Transform>(true);
            foreach(var t in ts)
            {
                if (!dict.ContainsKey(t))
                {
                    dict.Add(t, t);
                }
            }
        }
        return dict.Keys.ToList();
    }

    public static List<GameObject> FindGameObjects(Transform root, string key)
    {
        List<GameObject> list = new List<GameObject>();
        var childrens = root.GetComponentsInChildren<Transform>(true);
        foreach (var child in childrens)
        {
            if (child.name.Contains(key))
            {
                list.Add(child.gameObject);
            }
        }
        return list;
    }



    public static List<GameObject> FindGameObjects(Transform root, List<string> keys)
    {
        List<GameObject> list = new List<GameObject>();
        var childrens = root.GetComponentsInChildren<Transform>(true);
        foreach (var child in childrens)
        {
            if (IsContainsKeys(child.name,keys))
            {
                list.Add(child.gameObject);
            }
        }
        return list;
    }

    public static bool IsContainsKeys(string name, List<string> keys)
    {
        foreach(var key in keys)
        {
            if (name.Contains(key))
            {
                return true;
            }
        }
        return false;
    }

    public static List<Transform> FindSameNameList(List<Transform> modelDicT, string n)
    {
        List<Transform> sameNameList = new List<Transform>();
        string n2 = n + " ";
        string n3 = n + "_";
        foreach (Transform item in modelDicT)
        {
            if (item == null) continue;
            //if (item.name.StartsWith(n))
            //{
            //    sameNameList.Add(item);
            //}
            if (item.name == n || item.name.StartsWith(n2) || item.name.StartsWith(n3))
            {
                sameNameList.Add(item);
            }
        }
        return sameNameList;
    }

    public static void AddPreName(Transform p)
    {
        string pName = p.name + "_";
        for (int i = 0; i < p.childCount; i++)
        {
            var child = p.GetChild(i);
            if (child.name.StartsWith(pName)) continue;
            child.name = $"{pName}{child.name}";
        }
    }

    public static void AddPreNames(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            TransformHelper.AddPreName(child);
        }
    }

    public static void RemoveChildren(Transform root)
    {
        List<Transform> list1 = new List<Transform>();
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            list1.Add(child);

        }

        for (int i = 0; i < list1.Count; i++)
        {
            var child = list1[i];
            EditorHelper.UnpackPrefab(child.gameObject);

            List<Transform> list2 = new List<Transform>();
            for (int j = 0; j < child.childCount; j++)
            {
                list2.Add(child.GetChild(j));
            }
            foreach (var t in list2)
            {
                t.SetParent(root);
            }
            GameObject.DestroyImmediate(child.gameObject);
            //i--;
        }
    }

    public static void ReGroup1_After(Transform root)
    {
        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            string cName = child.name;
            if (cName.Contains("_") == false)
            {

                continue;
            }
            string[] parts = cName.Split('_');
            string n1 = parts[0];
            string n2 = parts[1];
            if (!beforeNames.ContainsKey(n1))
            {
                beforeNames.Add(n1, new List<Transform>());
            }
            beforeNames[n1].Add(child);

            if (!afterNames.ContainsKey(n2))
            {
                afterNames.Add(n2, new List<Transform>());
            }
            afterNames[n2].Add(child);
        }
        foreach (var n in afterNames.Keys)
        {
            GameObject go = new GameObject(n);
            var list = afterNames[n];
            go.transform.SetParent(root);
            foreach (var child in list)
            {
                child.SetParent(go.transform);
            }
        }
    }

    public static void ReGroupBefore(Transform root)
    {
        EditorHelper.UnpackPrefab(root.gameObject);

        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        GetBeforeAfterList(root, beforeNames, afterNames);

        ReGroupByDict(root, beforeNames);
    }

    public static void ShowAll(GameObject root)
    {
        var ts = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in ts)
        {
            t.gameObject.SetActive(true);
        }
    }

    public static void ReGroupByDict(Transform root,Dictionary<string, List<Transform>> dict)
    {
        List<Transform> orgChildren = new List<Transform>();
        for(int i=0;i<root.childCount;i++)
        {
            orgChildren.Add(root.GetChild(i));
        }

        foreach (var n in dict.Keys)
        {
            GameObject go = new GameObject(n);
            var list = dict[n];
            go.transform.SetParent(root);
            foreach (var child in list)
            {
                child.SetParent(go.transform);
                Debug.Log($"parent:{go.name} child:{child}");
            }
        }

        foreach(var c in orgChildren)
        {
            GameObject.DestroyImmediate(c.gameObject);
        }
    }

    public static void ReGroupAfter(Transform root)
    {
        EditorHelper.UnpackPrefab(root.gameObject);

        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        GetBeforeAfterList(root, beforeNames, afterNames);

        ReGroupByDict(root, afterNames);
    }

    private static void GetBeforeAfterList(Transform root, Dictionary<string, List<Transform>> beforeNames, Dictionary<string, List<Transform>> afterNames)
    {
        for (int j = 0; j < root.childCount; j++)
        {
            var child0 = root.GetChild(j);
            for (int i = 0; i < child0.childCount; i++)
            {
                var child = child0.GetChild(i);
                string cName = child.name;
                if (cName.Contains("_") == false)
                {

                    continue;
                }
                string[] parts = cName.Split('_');
                string n1 = parts[0];
                string n2 = parts[1];
                if (!beforeNames.ContainsKey(n1))
                {
                    beforeNames.Add(n1, new List<Transform>());
                }
                beforeNames[n1].Add(child);

                if (!afterNames.ContainsKey(n2))
                {
                    afterNames.Add(n2, new List<Transform>());
                }
                afterNames[n2].Add(child);
            }
        }
    }

    internal static void ClearChildren(GameObject root)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child == null) continue;
            if (child.gameObject == root) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}

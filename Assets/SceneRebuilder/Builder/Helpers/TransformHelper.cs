using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformHelper
{
    public static List<Transform> GetChildrenNoLOD(GameObject root)
    {
        List<Transform> list = new List<Transform>();
        GetMeshPointsNoLOD(root.transform, list);
        return list;
    }

    private static void GetMeshPointsNoLOD(Transform root, List<Transform> list)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            var lod = child.GetComponent<LODGroup>();
            if (lod != null)
            {
                continue;
            }
            list.Add(child);

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
        //objPoint.name = $"Point[{i + 1}][{j + 1}]({p.Point})";
        //objPoint.name = $"Point[{j + 1}]({p.Point})";
        objPoint.name = $"Point({point.x},{point.y},{point.z})";
        objPoint.transform.SetParent(transform1);
        objPoint.transform.localPosition = point;
        objPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

        if(transform2!=null)
            objPoint.transform.SetParent(transform2);
        return objPoint;
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

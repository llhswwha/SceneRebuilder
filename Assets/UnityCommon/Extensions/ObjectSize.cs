using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 专门计算物体的体积用
/// </summary>
public static class ObjectSize
{

    /// <summary>
    /// （主要）
    ///（在用） 获得物体在local的坐标系下计算的大小(就是相对于父物体的大小)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetLocalSize(this GameObject obj)
    {
        Quaternion rotation = obj.transform.localRotation;
        Transform parent = obj.transform.parent;

        //obj.transform.parent = null;
        obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        obj.transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector3 result = obj.GetSizeByCollider();
        Vector3 scaleT = new Vector3(obj.transform.lossyScale.x / obj.transform.localScale.x, obj.transform.lossyScale.y / obj.transform.localScale.y, obj.transform.lossyScale.z / obj.transform.localScale.z);
        //因为要获取LocalSize所以要除以缩放比例
        result = new Vector3(result.x / scaleT.x, result.y / scaleT.y, result.z / scaleT.z);

        obj.transform.parent = parent;
        obj.transform.localRotation = rotation;

        return result;
    }

    /// <summary>
    /// （主要）
    /// （在用）获得物体的Size，旋转角度为0,经过缩放后的真实大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetGlobalSize(this GameObject obj)
    {
        Quaternion rotation = obj.transform.localRotation;
        Transform parent = obj.transform.parent;

        //obj.transform.parent = null;
        obj.transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector3 result = obj.GetSizeByCollider();
        ////Vector3 scaleT = new Vector3(obj.transform.lossyScale.x / obj.transform.localScale.x, obj.transform.lossyScale.y / obj.transform.localScale.y, obj.transform.lossyScale.z / obj.transform.localScale.z);
        //////result = new Vector3(result.x * obj.transform.lossyScale.x, result.y * obj.transform.lossyScale.y, result.z * obj.transform.lossyScale.z);
        ////result = new Vector3(result.x * scaleT.x, result.y * scaleT.y, result.z * scaleT.z);

        obj.transform.parent = parent;
        obj.transform.localRotation = rotation;

        return result;
    }

    /// <summary>
    /// 经过缩放后的真实大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetSizeByCollider(this GameObject obj)
    {
        Vector3 result = Vector3.zero;
        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider != null)
        {
            result = GetSizeByCollider(obj, collider);
        }
        else
        {
            result = GetSizeOfNoCollider(obj);
            //Vector3 scaleT = new Vector3(obj.transform.lossyScale.x / obj.transform.localScale.x, obj.transform.lossyScale.y / obj.transform.localScale.y, obj.transform.lossyScale.z / obj.transform.localScale.z);
            ////因为要获取LocalSize所以要除以缩放比例
            //result = new Vector3(result.x / scaleT.x, result.y / scaleT.y, result.z / scaleT.z);
        }
        return result;
    }

    /// <summary>
    /// 获取物体的当前大小，通过BoxCollider
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    private static Vector3 GetSizeByCollider(GameObject obj, BoxCollider collider)
    {
        Vector3 result;
        Vector3 size = collider.size;
        //Vector3 scale = obj.transform.localScale;
        Vector3 scale = obj.transform.lossyScale;
        result = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        return result;
    }


    /// <summary>
    /// （主要）
    /// 获得物体的Size，经过缩放后的真实大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetSize(this GameObject obj)
    {
        //return obj.GetCurrentSize();
        return obj.GetGlobalSize();
    }

    /// <summary>
    /// 获取没有Collider物体的大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static Vector3 GetSizeOfNoCollider(GameObject obj)
    {
        Vector3 result;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render != null)
        {
            result = render.bounds.size;
        }
        else
        {
            result = GetSizeFromChildrenMeshRender(obj);
        }
        return result;
    }

    /// <summary>
    /// 获得模型的原始大小,受旋转角度和缩放比例影响
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetOriginalSizeByCollider(this GameObject obj)
    {
        Vector3 result = Vector3.zero;
        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider != null)
        {
            result = collider.size;
        }
        else
        {
            result = GetSizeOfNoCollider(obj);
        }
        return result;
    }

    /// <summary>
    /// 获得模型的原始大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetOriginalSizeByMeshFilter(this GameObject obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            return meshFilter.mesh.bounds.size;
        }
        else
        {
            return GetSizeFromChildrenMeshFilter(obj);
        }
    }

    private static Vector3 GetSizeFromChildrenMeshFilter(GameObject obj)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            MeshFilter childMeshFilter = child.GetComponent<MeshFilter>();
            if (childMeshFilter == null) continue;
            Bounds bounds = childMeshFilter.mesh.bounds;
            Vector3 size = bounds.size;

            if (size.x > x)
            {
                x = size.x;
            }
            if (size.y > y)
            {
                y = size.y;
            }
            if (size.z > z)
            {
                z = size.z;
            }
            //print("bounds"+i+":"+x+","+y+","+z);
        }
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 计算物体的体积，根据MeshRenderer的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static Vector3 GetSizeFromChildrenMeshRender(GameObject obj)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;

        //List<Transform> transforms = obj.GetAllTransforms();
        MeshRenderer[] ChildRenders = obj.GetComponentsInChildren<MeshRenderer>();
        //Transform[] transforms = obj.GetComponentsInChildren<Transform>();
        if(ChildRenders.Length!=0)
        {
            foreach (MeshRenderer childRender in ChildRenders)
            {
                //MeshRenderer childRender = child.GetComponent<MeshRenderer>();
                //if (childRender == null) continue;
                Bounds bounds = childRender.bounds;
                //bounds.max
                Vector3 max = bounds.max;
                if (max.x > maxX)
                {
                    maxX = max.x;
                }
                if (max.y > maxY)
                {
                    maxY = max.y;
                }
                if (max.z > maxZ)
                {
                    maxZ = max.z;
                }

                Vector3 min = bounds.min;
                if (min.x < minX)
                {
                    minX = min.x;
                }
                if (min.y < minY)
                {
                    minY = min.y;
                }
                if (min.z < minZ)
                {
                    minZ = min.z;
                }
                //print("bounds"+i+":"+x+","+y+","+z);
            }
            x = maxX - minX;
            y = maxY - minY;
            z = maxZ - minZ;
        }       
        return new Vector3(x, y, z);
    }

    public static Dictionary<string, Vector3> sizebuffer = new Dictionary<string, Vector3>();

    /// <summary>
    /// 获得模型的原始大小，并缓存起来
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Vector3 GetOriginalSizeByPrefab(this GameObject obj, string prefabName)
    {
        if (sizebuffer.ContainsKey(prefabName))
        {
            return sizebuffer[prefabName];
        }

        Vector3 size = GetSize(obj);
        //Vector3 scale = obj.transform.localScale;
        Vector3 scale = obj.transform.lossyScale;
        Vector3 size2 = new Vector3(size.x / scale.x, size.y / scale.y, size.z / scale.z);
        //MonoBehaviour.print("GetOriginalSizeEx：" + obj + "|" + size2);

        if (!string.IsNullOrEmpty(prefabName))
        {
            sizebuffer.Add(prefabName, size2);
        }

        return size2;
    }
}

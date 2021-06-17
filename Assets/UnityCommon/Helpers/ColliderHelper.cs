using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 自动创建碰撞体
/// </summary>
public static class ColliderHelper  {

    /// <summary>
    /// 添加碰撞器，用于碰撞检测
    /// </summary>
    /// <param name="obj"></param>
    public static BoxCollider AddCollider(this GameObject obj, bool isRemoveChildCollider = true)
    {
        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider=CreateBoxCollider(obj.transform, isRemoveChildCollider);
        }
        return collider;

    }

    public static BoxCollider CreateBoxCollider(this GameObject obj, bool isRemoveChildCollider = true)
    {
        return CreateBoxCollider(obj.transform, isRemoveChildCollider);
    }

    public static BoxCollider CreateBoxColliderOld(Transform parent)
    {
        //MonoBehaviour.print("AddBoxCollider:" + parent.childCount);
        if (parent.childCount == 0)
        {
            return parent.gameObject.AddComponent<BoxCollider>();
        }

        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;

        RemoveColliders(parent);
        Bounds bounds = CaculateBounds(parent,true);
        BoxCollider boxCollider = AddBoxColliderOld(parent, bounds);

        parent.position = postion;
        parent.rotation = rotation;
        parent.localScale = scale;

        return boxCollider;
    }

    public static BoxCollider RecreateBoxCollider(Transform parent, bool isRemoveChildCollider = true, bool isMultiBox = false)
    {
        BoxCollider collider = parent.GetComponent<BoxCollider>();
        if (collider != null)
        {
            GameObject.DestroyImmediate(collider);
        }
        return CreateBoxCollider(parent, isRemoveChildCollider, isMultiBox);
    }

        public static BoxCollider CreateBoxCollider(Transform parent,bool isRemoveChildCollider = true,bool isMultiBox=false)
    {
        if (isMultiBox == false)
        {
            BoxCollider collider = parent.GetComponent<BoxCollider>();
            if (collider != null)
            {
                return collider;
            }
        }
        //MonoBehaviour.print("AddBoxCollider:" + parent.childCount);
        if (parent.childCount == 0)
        {
            return parent.gameObject.AddComponent<BoxCollider>();
        }

        Vector3 postion = parent.localPosition;
        Vector3 scale = parent.localScale;
        Quaternion rotation = parent.localRotation;

        if (isRemoveChildCollider)
        {
            RemoveColliders(parent);
        }
        else
        {
            if (isMultiBox == false)
            {
                BoxCollider B = parent.gameObject.GetComponent<BoxCollider>();
                Object.DestroyImmediate(B);
            }
        }

        Bounds bounds = CaculateBounds(parent,true);
        BoxCollider boxCollider = AddBoxCollider(parent, bounds);

        parent.localPosition = postion;
        parent.localScale = scale;
        parent.localRotation = rotation;

        return boxCollider;
    }

    public static SphereCollider CreateSphereCollider(Transform parent, bool isRemoveChildCollider = true, bool isMultiBox = false)
    {
        if (isMultiBox == false)
        {
            SphereCollider collider = parent.GetComponent<SphereCollider>();
            if (collider != null)
            {
                return collider;
            }
        }

        Vector3 postion = parent.localPosition;
        Vector3 scale = parent.localScale;
        Quaternion rotation = parent.localRotation;

        if (isRemoveChildCollider)
        {
            RemoveColliders(parent);
        }
        else
        {
            if (isMultiBox == false)
            {
                BoxCollider B = parent.gameObject.GetComponent<BoxCollider>();
                Object.DestroyImmediate(B);
            }
        }

        Bounds bounds = CaculateBounds(parent, true);
        SphereCollider sphereCollider = AddSphereCollider(parent, bounds);

        parent.localPosition = postion;
        parent.localScale = scale;
        parent.localRotation = rotation;

        return sphereCollider;
    }

    private static void Reset(Transform parent)
    {
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;
    }

    private static void RemoveColliders(Transform parent)
    {
        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider child in colliders)
        {
            Object.DestroyImmediate(child);
        } //避免子节点中有残留的Collider，生成前先把所有子节点的Collider删除。
    }

    public static BoxCollider AddBoxCollider(Transform parent, Bounds bounds)
    {
        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        if (boxCollider == null)
        {
            LogError("无法添加BoxCollider:" + parent + "|" + bounds);
        }
        else
        {
            try
            {
                Vector3 center= bounds.center - parent.position;
                Vector3 size= bounds.size;
                Vector3 scale = parent.lossyScale;

                boxCollider.center = new Vector3(center.x / scale.x, center.y / scale.y, center.z / scale.z);
                boxCollider.size = new Vector3(size.x / scale.x, size.y / scale.y, size.z / scale.z);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
        }
        return boxCollider;
    }

    public static SphereCollider AddSphereCollider(Transform parent, Bounds bounds)
    {
        SphereCollider boxCollider = parent.gameObject.AddComponent<SphereCollider>();
        if (boxCollider == null)
        {
            LogError("无法添加BoxCollider:" + parent + "|" + bounds);
        }
        else
        {
            try
            {
                Vector3 center = bounds.center - parent.position;
                Vector3 size = bounds.size;
                Vector3 scale = parent.lossyScale;

                boxCollider.center = new Vector3(center.x / scale.x, center.y / scale.y, center.z / scale.z);

                Vector3 colliderSize = new Vector3(size.x / scale.x, size.y / scale.y, size.z / scale.z);
                boxCollider.radius = GetRadius(colliderSize);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
        }
        return boxCollider;
    }

    public static float GetRadius(Transform parent, Bounds bounds)
    {
        Vector3 center = bounds.center - parent.position;
        Vector3 size = bounds.size;
        Vector3 scale = parent.lossyScale;
        Vector3 colliderSize = new Vector3(size.x / scale.x, size.y / scale.y, size.z / scale.z);
        float x = colliderSize.x;
        float y = colliderSize.y;
        float z = colliderSize.z;
        float r = (float)Math.Pow(x * x + y * y + z * z, 0.5f);
        return r;
    }

    public static float GetRadius(Vector3 size)
    {
        float x = size.x;
        float y = size.y;
        float z = size.z;
        float r = (float)Math.Pow(x * x + y * y + z * z, 0.5f)/2.0f;
        return r;
    }

    private static BoxCollider AddBoxColliderOld(Transform parent, Bounds bounds)
    {
        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        if (boxCollider == null)
        {
            LogError("无法添加BoxCollider:" + parent+"|"+bounds);
        }
        else
        {
            try
            {
                boxCollider.center = bounds.center - parent.position;
                boxCollider.size = bounds.size;
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
        }
        return boxCollider;
    }

    private static void LogError(string msg)
    {
        LogInfo.Error(msg);
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    /// <returns></returns>
    public static Bounds CaculateBounds(Transform parent,bool isReset)
    {
        if(isReset)
            Reset(parent);
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        List<Renderer> rendersFilter = FilterRenderers(renders);
        return CaculateBounds(rendersFilter);
    }

    public static List<string> filterNames = new List<string>() { "rail", "柱" };//楼梯围栏会突出到上一层楼

    private static bool IsContainsFilterName(string name)
    {
        foreach (string filterName in filterNames)
        {
            if (name.Contains(filterName))
            {
                return true;
            }
        }
        return false;
    }

    private static List<Renderer> FilterRenderers(Renderer[] renders)
    {
        List<Renderer> rendersFilter = new List<Renderer>();
        foreach (var render in renders)
        {
            var name = render.gameObject.name.ToLower();
            
            if (IsContainsFilterName(name))//楼梯围栏会突出到上一层楼
            {

            }
            else
            {
                rendersFilter.Add(render);
            }
        }
        return rendersFilter;
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    /// <returns></returns>
    public static Bounds CaculateBounds(this GameObject parent)
    {
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        List<Renderer> rendersFilter = FilterRenderers(renders);
        return CaculateBounds(rendersFilter);
    }

    /// <summary>
    /// 获取对象包围盒的最大和最小值
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static List<Vector3> GetBoundsMaxMinPoints(this GameObject go)
    {
        Bounds bounds = go.CaculateBounds();
        List<Vector3> points = new List<Vector3>();
        points.Add(bounds.max);
        points.Add(bounds.min);
        return points;
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    /// <param name="renders"></param>
    /// <returns></returns>
    public static Bounds CaculateBounds(IEnumerable<Renderer> renders)
    {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (Renderer child in renders)
        {
            if (child == null) continue;
            if (!child.enabled) continue;
            center += child.bounds.center;
            count++;
        }

        if (count > 0)
        {
            center /= count;
        }
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            if (!child.enabled) continue;
            bounds.Encapsulate(child.bounds);
        }
        return bounds;
    }
}

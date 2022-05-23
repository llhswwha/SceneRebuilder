using System;
using System.Collections.Generic;
using System.Linq;
//using Unity.ComnLib;
using UnityEngine;


namespace CommonUtils
{
    /// <summary>
    /// 游戏物体扩展函数
    /// </summary>
    public static class ObjectExtension
    {
        public static T FindComponent<T>(this Transform t) where T : Component
        {
            T component = t.FindComponentInParent<T>();
            if (component == null)
            {
                component = GameObject.FindObjectOfType<T>();
            }
            return component;
        }

        public static Camera FindCamera(this Transform t, Camera c)
        {
            if (c == null)
            {
                c = t.FindComponentInParent<Camera>();
            }
            if (c == null)
            {
                c = Camera.main;
            }
            if (c == null)
            {
                c = GameObject.FindObjectOfType<Camera>();
            }
            return c;
        }

        public static void HideMeshRender(this GameObject go)
        {
            MeshRenderer render = go.GetComponent<MeshRenderer>();
            if (render)
            {
                render.enabled = false;
            }
        }

        /// <summary>
        /// 在子物体中查找，若是没有的话就创建一个
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static GameObject FindOrCreateChild(this GameObject parent, string goName)
        {
            Transform tran = parent.transform.Find(goName);
            GameObject go;
            if (!tran)
            {
                go = new GameObject(goName);
                go.transform.parent = parent.transform;
                go.Reset();
            }
            else
            {
                go = tran.gameObject;
            }
            return go;
        }


        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="obj"></param>
        public static void Reset(this GameObject obj)
        {
            obj.transform.Reset();
        }

        /// <summary>
        /// 重置位置
        /// </summary>
        /// <param name="t"></param>
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.Euler(Vector3.zero);
        }

        //public static bool IsChild(this Transform parent, Transform child)
        //{
        //    Transform p = child.parent;
        //    if (p == null) return false;
        //    while (p!= parent)
        //    {
        //        p = p.parent;
        //        if (p == null) return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 将屏幕坐标转换成NGUI坐标
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public static Vector3 GetNGUIPos(this Vector3 screenPos)
        {
            return new Vector3(screenPos.x - Screen.width / 2f, screenPos.y - Screen.height / 2f);
        }

        /// <summary>
        /// 获取机柜游戏物体的中心位置
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static Vector3 GetCenterPosition(this List<GameObject> objs)
        {
            Vector3 pos = Vector3.zero;
            if (objs == null) return pos;
            foreach (GameObject o in objs)
            {
                pos += o.transform.position;
            }
            pos = pos / objs.Count;
            return pos;
        }

        /// <summary>
        /// 得到（没有的话创建）一个游戏物体的BoxCollider
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BoxCollider GetBoxCollider(this GameObject obj)
        {
            BoxCollider collider = obj.GetComponent<BoxCollider>();
            if (collider == null)
            {
                if (obj.transform.childCount == 0)
                {
                    collider = obj.AddComponentEx<BoxCollider>();
                }
                else if (obj.transform.childCount == 1)
                {
                    Component[] components = obj.GetComponents<Component>();
                    if (components.Length == 0)//必须是空物体
                    {
                        collider = obj.transform.GetChild(0).gameObject.GetBoxCollider();
                    }
                    else
                    {
                        collider = ColliderExtension.CreateBoxCollider(obj.transform);
                    }
                }
                else
                {
                    collider = ColliderExtension.CreateBoxCollider(obj.transform);
                }
            }
            return collider;
        }

        /// <summary>
        /// 获取游戏物体的基本信息：路径，子物体，脚本，位置
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetObjectInfo(this GameObject obj)
        {
            string txt = "";
            txt += "路径:" + obj.transform.GetPathToRoot() + "\n";
            txt += "子物体:" + obj.transform.GetChildList() + "\n";
            txt += "脚本:" + obj.GetScriptList() + "\n";
            txt += "位置:" + obj.transform.localPosition;
            return txt;
        }

        /// <summary>
        /// 获取一个物体上绑定的脚本的列表的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetScriptList(this GameObject obj)
        {
            string txt = "";
            MonoBehaviour[] behaviours = obj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                txt += behaviour.GetType() + " ";
            }
            return txt.Trim();
        }

        /// <summary>
        /// 获取物体的子物体的列表的字符串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetChildList(this Transform t)
        {
            string txt = "";
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                txt += child.name + ",";
            }
            return txt;
        }

        /// <summary>
        /// 获取游戏物体到根对象的路径
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetPathToRoot(this Transform t)
        {
            if (t.parent == null)
            {
                return t.name;
            }
            else
            {
                return GetPathToRoot(t.parent) + "->" + t.name;
            }
        }

        public static void SetParentEx(this GameObject go, GameObject pGo)
        {
            go.transform.SetParentEx(pGo.transform);
        }

        /// <summary>
        /// 设置父对象，并修改localPosition。
        /// </summary>
        /// <param name="t"></param>
        public static void SetParentEx(this Transform t, Transform p)
        {
            if (t.transform.parent != p)
            {
                Vector3 scale = t.localScale;
                t.SetParent(p);
                t.localPosition = t.position;
                t.localScale = scale;
            }
        }

        /// <summary>
        /// 删除一个物体的所有子物体
        /// </summary>
        /// <param name="t"></param>
        public static void ClearChildren(this Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                if (child != null)
                    GameObject.DestroyImmediate(child.gameObject); //这里不能用Destroy
            }
        }

        /// <summary>
        /// 获取一个物体的第一个子物体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GameObject FirstChild(this GameObject obj)
        {
            Transform t = obj.transform;
            if (t.childCount == 0) return null;
            return t.GetChild(0).gameObject;
        }

        /// <summary>
        /// 隐藏游戏物体集合
        /// </summary>
        /// <param name="list"></param>
        public static void Hide(this IEnumerable<GameObject> list)
        {
            SetActive(list, false);
        }

        /// <summary>
        /// 显示游戏物体集合
        /// </summary>
        /// <param name="list"></param>
        public static void Show(this IEnumerable<GameObject> list)
        {
            SetActive(list, true);
        }

        /// <summary>
        /// 设置包含某个脚本的子物体的激活状态(包括父物体)
        /// </summary>
        /// <param name="go"></param>
        /// <param name="isActive"></param>
        public static T[] SetChildrenActive<T>(this GameObject go, bool isActive) where T : Component
        {
            T[] components = go.GetComponentsInChildren<T>();
            components.SetActive(false);
            return components;
        }

        /// <summary>
        /// 设置子物体的激活状态(不包括父物体)
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="isActive"></param>
        public static void SetChildrenActive(this Transform tf, bool isActive)
        {
            for (int i = 0; i < tf.childCount; i++)
            {
                Transform child = tf.GetChild(i);
                child.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 设置脚本游戏物体的激活状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isActive"></param>
        public static void SetActive(this IEnumerable<GameObject> list, bool isActive)
        {
            if (list == null) return;
            foreach (GameObject item in list)
            {
                if (item != null)
                {
                    item.SetActive(isActive);
                }
            }
        }

        /// <summary>
        /// 设置脚本游戏物体的激活状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isActive"></param>
        public static void SetActive(this IEnumerable<Transform> list, bool isActive)
        {
            if (list == null) return;
            foreach (Transform item in list)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(isActive);
                }
            }
        }

        /// <summary>
        /// 设置脚本（所在的游戏物体）的激活状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isActive"></param>
        public static void SetActive(this IEnumerable<Component> list, bool isActive)
        {
            if (list == null) return;
            foreach (Component item in list)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(isActive);
                }
            }
        }

        /// <summary>
        /// 设置脚本的状态
        /// </summary>
        /// <param name="list"></param>
        /// <param name="enabled"></param>
        public static void SetEnable(this IEnumerable<MonoBehaviour> list, bool enabled)
        {
            if (list == null) return;
            foreach (MonoBehaviour item in list)
            {
                if (item != null)
                {
                    item.enabled = enabled;
                }
            }
        }

        public static T SetEnable<T>(this Transform t, bool enabled) where T : MonoBehaviour
        {
            T component = t.GetComponent<T>();
            if (component)
            {
                component.enabled = enabled;
            }
            return component;
        }

        public static T SetColliderEnable<T>(this Transform t, bool enabled) where T : Collider
        {
            T component = t.GetComponent<T>();
            if (component)
            {
                component.enabled = enabled;
            }
            return component;
        }

        public static T SetEnable<T>(this GameObject go, bool enabled) where T : MonoBehaviour
        {
            T component = go.GetComponent<T>();
            if (component)
            {
                component.enabled = enabled;
            }
            return component;
        }

        /// <summary>
        /// Transform转换成GameObject
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<GameObject> ToGameObjects(this IEnumerable<Transform> items)
        {
            return items.Select(item => item.gameObject).ToList();
        }

        public static T AddComponentEx<T>(this GameObject obj) where T : Component
        {
            //T component = obj.GetComponent<T>() ?? obj.AddComponent<T>();//这个写法会有问题
            //return component;
            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }
            return comp;
        }

        /// <summary>
        /// 获取物体上的材质
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Material[] GetMaterials(this GameObject go)
        {
            Renderer render = go.GetComponent<Renderer>();
            //List<Material> ms = new List<Material>(render.materials);
            //render.materials.Select(t => new Material(t)).ToArray();
            return render.materials.Select(t => new Material(t)).ToArray();
            //return ms.ToArray();
        }

        /// <summary>
        /// 根据名称获取子物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform GetChildByName(this Transform transform, string name)
        {
            List<Transform> children = transform.GetChildrenTransform();
            foreach (Transform child in children)
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据名称获取子物体,name不用和名称相同，包含就可以
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform transform, string name)
        {
            List<Transform> children = transform.GetChildrenTransform();
            foreach (Transform child in children)
            {
                if (child.name.Contains(name))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据名称获取子物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<Transform> FindChildrenByName(this Transform transform, string name)
        {
            List<Transform> result = new List<Transform>();
            List<Transform> children = transform.GetChildrenTransform();
            foreach (Transform child in children)
            {
                if (child.name.Contains(name))
                {
                    result.Add(child);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据标签获取子物体（多个）
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenByTag(this Transform transform, params string[] tags)
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.CompareTag(tags))
                {
                    result.Add(child);
                }
                result.AddRange(GetChildrenByTag(child, tags));
            }
            return result;
        }

        public static List<Transform> GetChildrenByLayer(this Transform transform, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.layer == layer)
                {
                    result.Add(child);
                }
                result.AddRange(GetChildrenByLayer(child, layerName));
            }
            return result;
        }

        ///// <summary>
        ///// 根据标签获取子物体（多个）
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="tags"></param>
        ///// <returns></returns>
        //public static List<GameObject> GetChildrenObjByTagOP(this GameObject obj, params string[] tags)
        //{
        //    List<GameObject> result = new List<GameObject>();
        //    for (int i = 0; i < obj.transform.childCount; i++)
        //    {
        //        Transform child = obj.transform.GetChild(i);
        //        if (child.CompareTag(tags))
        //        {
        //            result.Add(child.gameObject);
        //        }
        //        result.AddRange(GetChildrenObjByTagOP(child.gameObject, tags));
        //    }
        //    return result;
        //}

        /// <summary>
        /// 根据标签获取子物体（多个）(返回List GameObject)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<GameObject> GetChildrenOBJByTag(this GameObject obj, params string[] tags)
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                if (child.CompareTag(tags))
                {
                    result.Add(child.gameObject);
                }
                result.AddRange(GetChildrenOBJByTag(child.gameObject, tags));
            }
            return result;
        }

        /// <summary>
        /// 比较标签
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool CompareTag(this Transform transform, string[] tags)
        {
            return tags.Any(transform.CompareTag);
        }

        /// <summary>
        /// 根据标签获取子物体（多个）
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenByTag(this GameObject go, params string[] tags)
        {
            return go.transform.GetChildrenByTag(tags);
        }
        ///// <summary>
        ///// 根据标签获取子物体（多个）
        ///// </summary>
        ///// <param name="go"></param>
        ///// <param name="tags"></param>
        ///// <returns></returns>
        //public static List<GameObject> GetChildrenObjByTag(this GameObject go, params string[] tags)
        //{
        //    return go.GetChildrenObjByTagOP(tags);
        //}


        /// <summary>
        /// 获取所有绑定T类型脚本的子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenTransform<T>(this Transform transform) where T : Component
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    result.Add(child);
                }
                result.AddRange(GetChildrenTransform<T>(child));
            }
            return result;
        }

        /// <summary>
        /// 获取所有绑定T类型脚本的子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<GameObject> GetChildrenGameObject<T>(this Transform transform) where T : Component
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<T>() != null)
                {
                    result.Add(child.gameObject);
                }
                result.AddRange(GetChildrenGameObject<T>(child));
            }
            return result;
        }

        public static List<T> FindComponentsInChildren<T>(this Transform transform, bool isContainSelf = true) where T : Component
        {
            List<T> result = new List<T>();
            if (isContainSelf)
            {
                T component0 = transform.GetComponent<T>();
                if (component0 != null)
                {
                    result.Add(component0);
                }
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                T component = child.GetComponent<T>();
                if (component != null)
                {
                    result.Add(component);
                }
                result.AddRange(FindComponentsInChildren<T>(child, false));
            }
            return result;
        }

        /// <summary>
        /// 获取所有绑定T类型脚本的子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenTransform<T>(this GameObject go) where T : Component
        {
            return go.transform.GetChildrenTransform<T>();
        }

        /// <summary>
        /// 获取所有绑定T类型脚本的子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<GameObject> GetChildrenGameObject<T>(this GameObject go) where T : Component
        {
            return go.transform.GetChildrenGameObject<T>();
        }

        public static T FindComponentInChildren<T>(this GameObject go) where T : Component
        {
            Transform transform = go.transform;
            return FindComponentInChildren<T>(transform);
        }

        public static T FindComponentInChildren<T>(this Transform transform) where T : Component
        {
            List<Transform> children = transform.GetChildrenTransform();
            foreach (Transform child in children)
            {
                T component = child.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public static T FindComponentInParent<T>(this Camera go) where T : Component
        {
            return go.transform.FindComponentInParent<T>();
        }

        public static T FindComponentInParent<T>(this GameObject go) where T : Component
        {
            return go.transform.FindComponentInParent<T>();
        }

        public static T FindComponentInParent<T>(this Transform transform) where T : Component
        {
            Transform parent = transform.parent;
            if (parent == null)
            {
                return transform.GetComponentInChildren<T>();
            }
            T component = parent.GetComponentInChildren<T>();
            while (component == null)
            {
                parent = parent.parent;
                if (parent == null) break;
                component = parent.GetComponentInChildren<T>();
            }
            return component;
        }

        /// <summary>
        /// 获取所有绑定T类型脚本的子物体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<T> FindComponentsInChildren<T>(this GameObject go) where T : Component
        {
            //List<T> result = null;
            //TimeSpan time = TimeCounter.Run(() =>
            //{
            //    result = go.transform.GetChildrenComponents<T>();
            //});
            //LogInfo.Error("GetChildrenComponents", typeof(T)+":"+time.TotalMilliseconds + "ms");
            //return result;

            return go.transform.FindComponentsInChildren<T>();
        }

        /// <summary>
        /// 获取所有子物体（递归）
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenTransform(this Transform transform)
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                result.Add(child);
                result.AddRange(GetChildrenTransform(child));
            }
            return result;
        }

        /// <summary>
        /// 获取所有子物体（递归）
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static List<Transform> GetChildrenTransform(this GameObject go, bool isContainParent = false)
        {
            List<Transform> children = go.transform.GetChildrenTransform();
            if (isContainParent)
            {
                children.Add(go.transform);
            }
            return children;
        }

        /// <summary>
        /// 设置物体及子物体的Layer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        public static void SetLayerAll(this GameObject obj, string layer)
        {
            obj.layer = LayerMask.NameToLayer(layer);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                child.gameObject.SetLayerAll(layer);
            }
        }

        /// <summary>
        /// 设置物体及子物体的Tag
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tag"></param>
        public static void SetTagAll(this GameObject obj, string tag, bool isAll = true)
        {
            if (isAll)//全部设置
            {
                obj.tag = tag;
            }
            else
            {
                if (!obj.CompareTag("Untagged"))//只设置不是Untagged的物体，即已经设置过的不覆盖掉
                {
                    obj.tag = tag;
                }
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                child.gameObject.SetTagAll(tag, isAll);
            }
        }

        /// <summary>
        /// 复制游戏物体，并设置名称
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject InstantiateSelf(this GameObject prefab, string name = null)
        {
            GameObject instance = GameObject.Instantiate(prefab) as GameObject;
            instance.name = string.IsNullOrEmpty(name) ? prefab.name : name;

            //result.SetActive(true);
            return instance;
        }

        /// <summary>
        /// 获取物体的角度Y
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static float GetObjectAngleY(this GameObject go)
        {
            return go == null ? -1 : go.transform.eulerAngles.y;
        }

        /// <summary>
        /// 获取物体的幅度Y
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static float GetObjectRadianY(this GameObject go)
        {
            float angle = go.GetObjectAngleY();
            return Mathf.PI / 180 * angle;
        }

        /// <summary>
        /// 删除游戏物体上的组件（脚本）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void RemoveComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return;
            T component = obj.GetComponent<T>();
            if (component != null)
            {
                GameObject.DestroyImmediate(component);
            }
        }

        /// <summary>
        /// 删除游戏物体上的组件（脚本）
        /// </summary>
        public static void RemoveComponent<T>(this IList<GameObject> allObjs) where T : Component
        {
            if (allObjs == null) return;
            foreach (GameObject go in allObjs)
            {
                if (go == null) continue;
                go.RemoveComponent<T>();//不删除Highlighter的话隐身会有问题
            }
        }

        public static void Destroy(this IList<GameObject> allObjs)
        {
            foreach (GameObject go in allObjs)
            {
                GameObject.DestroyImmediate(go);
            }
        }


        public static void RemoveComponentAll<T>(this GameObject obj) where T : Component
        {
            obj.RemoveComponent<T>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                child.gameObject.RemoveComponentAll<T>();
            }
        }

        /// <summary>
        /// 关闭某对象下面的所有Collider
        /// </summary>
        /// <param name="go"></param>
        public static void DisableCollider(this GameObject go)
        {
            go.transform.DisableCollider();
        }

        /// <summary>
        /// 关闭某对象下面的所有Collider
        /// </summary>
        /// <param name="tran"></param>
        public static void DisableCollider(this Transform tran)
        {
            Collider[] colliders = tran.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }
        }

        /// <summary>
        /// 判断两个物体是否相交
        /// </summary>
        /// <returns></returns>
        public static bool CheckIntersectsByBounds(this GameObject go1, GameObject go2)
        {
            if (go2 == null)
            {
                return false;
            }
            BoxCollider bc1 = go1.GetComponent<BoxCollider>();
            if (bc1 == null)
            {
                bc1 = go1.CreateBoxCollider(false);
                bc1.enabled = false;
            }
            Bounds colliderBounds = bc1.bounds;
            //print("rendererBounds：" + colliderBounds);
            var rendererIsInsideBox = CheckIntersectsByBounds(go2, colliderBounds);
            return rendererIsInsideBox;
        }

        /// <summary>
        /// 判断两个物体是否相交
        /// </summary>
        /// <param name="go2"></param>
        /// <param name="colliderBounds"></param>
        /// <returns></returns>
        public static bool CheckIntersectsByBounds(GameObject go2, Bounds colliderBounds)
        {
            bool rendererIsInsideBox = false;
            Renderer render = go2.GetComponent<Renderer>();
            if (render != null)
            {
                //LogInfo.Alarm("StaticRoom.IsIntersects","render==null:"+go);
                //return false;
                Bounds rendererBounds = render.bounds;
                rendererIsInsideBox = colliderBounds.Intersects(rendererBounds);
                //print("rendererBounds：" + rendererBounds);
                //LogInfo(rendererBounds);
            }
            else
            {
                Bounds bounds = go2.CaculateBounds();
                rendererIsInsideBox = colliderBounds.Intersects(bounds);
                //BoxCollider collider1 = go2.GetComponent<BoxCollider>();
                //if (collider1 != null && go2.activeInHierarchy)
                ////只有enabled是true时才有效，不然bounds的Size是0；
                ////游戏物体的active也必须是true；所以这个的使用比较窄
                //{
                //    bool originalFalse = false;
                //    if (collider1.enabled == false)
                //    {
                //        collider1.enabled = true;
                //        originalFalse = true;
                //    }
                //    Bounds collider1Bounds = collider1.bounds;
                //    //print("collider1Bounds：" + collider1Bounds);
                //    //LogInfo(collider1Bounds);
                //    rendererIsInsideBox = colliderBounds.Intersects(collider1Bounds);
                //    if (originalFalse)
                //    {
                //        collider1.enabled = false;
                //    }
                //}
                //else
                //{
                //    Renderer[] subRenders = go2.GetComponentsInChildren<Renderer>();
                //    foreach (Renderer subRender in subRenders)
                //    {
                //        if (colliderBounds.Intersects(subRender.bounds))
                //        {
                //            rendererIsInsideBox = true;
                //            break;
                //        }
                //    }
                //}
            }
            return rendererIsInsideBox;
        }

        //public static bool SetColor(this GameObject obj,Color color)
        //{
        //    Renderer render = obj.GetComponent<Renderer>();
        //    if (render == null) return false;
        //    render.material.color = color;
        //    return true;
        //}
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MyTools
{
    public static void ClearComponents<T>(GameObject obj) where T :Component
    {
        var ids = obj.GetComponentsInChildren<T>(true);
        foreach (var id in ids)
        {
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"ClearComponents[{typeof(T)}] ids:{ids.Length}");
    }

    public static void ClearComponents<T>() where T : Component
    {
        foreach (var obj in Selection.gameObjects)
        {
            ClearComponents<T>(obj);
        }
    }

    [MenuItem("Tools/Clear/ClearRId")]
    public static void ClearRendererId()
    {
        ClearComponents<RendererId>();
    }

    [MenuItem("Tools/Clear/ClearMeshNode")]
    public static void ClearMeshNode()
    {
        ClearComponents<MeshNode>();
    }

    [MenuItem("Tools/Clear/ClearRendererInfo")]
    public static void ClearRendererInfo()
    {
        ClearComponents<MeshRendererInfo>();
    }

    [MenuItem("Tools/Clear/ClearGenerators")]
    public static void ClearGenerators()
    {
        ClearComponents<PipeMeshGeneratorBase>();
    }

    [MenuItem("Tools/Clear/ClearGeneratorArgs")]
    public static void ClearGeneratorArgs()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var ids = obj.GetComponentsInChildren<PipeModelComponent>(true);
            foreach (var id in ids)
            {
                //GameObject.DestroyImmediate(id);
                id.generateArg = null;
            }
            Debug.Log($"ClearGeneratorArgs ids:{ids.Length}");
        }
    }

    [MenuItem("Tools/Clear/ClearPipeModels")]
    public static void ClearPipeModels()
    {
        ClearComponents<PipeModelComponent>();
    }

    [MenuItem("Tools/Clear/ClearMeshComponents")]
    public static void ClearMeshComponents()
    {
        ClearComponents<MeshFilter>();
        ClearComponents<MeshRenderer>();
    }


    [MenuItem("Tools/Clear/ClearMesh")]
    public static void ClearMesh()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var ids = obj.GetComponentsInChildren<MeshFilter>(true);
            foreach (var id in ids)
            {
                id.sharedMesh = null;
            }
            Debug.Log($"ClearMesh       ids:{ids.Length}");
        }
    }


    [MenuItem("Tools/Others/ClearScripts")]
    public static void ClearScripts()
    {
        ClearComponents<MonoBehaviour>();
        //foreach (var obj in Selection.gameObjects)
        //{
        //    var ids = obj.GetComponentsInChildren<MonoBehaviour>(true);
        //    foreach (var id in ids)
        //    {
        //        GameObject.DestroyImmediate(id);
        //    }
        //    Debug.Log($"ClearScripts       ids:{ids.Length}");
        //}
    }

    [MenuItem("Tools/Transform/X10")]
    public static void TransformX10()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position *= 10;
            obj.transform.localScale *= 10;
        }
    }

    [MenuItem("Tools/Transform/D100Ex_P")]
    public static void TransformD100Ex_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                children.Add(child);
            }
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(null);
            }
            obj.transform.localScale *= 100;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(obj.transform);
            }
            obj.transform.localScale /= 100;
        }
    }

    [MenuItem("Tools/Transform/X10C")]
    public static void TransformX10C()
    {
        foreach (var obj in Selection.gameObjects)
        {
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale *= 10;
            }
        }
    }

    [MenuItem("Tools/Transform/D10C_S")]
    public static void TransformD10C_S()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("Tools/Transform/D10C_PS")]
    public static void TransformD10C_PS()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.position /= 10;
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("Tools/Transform/D10C_P")]
    public static void TransformD10C_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position /= 10;
            }
        }
    }

    [MenuItem("Tools/Transform/D10_P")]
    public static void TransformD10_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 10;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("Tools/Transform/D100_P")]
    public static void TransformD100_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("Tools/Transform/APP")]
    public static void TransformAPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var parent = obj.transform.parent;
            obj.transform.position += parent.position;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("Tools/Transform/CSPP")]
    public static void TransformCSPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var p1 = obj.transform.position;

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position -= p1;
            }
        }
    }

    [MenuItem("Tools/Transform/GetPositionOffset")]
    public static void GetPositionOffset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        foreach (var obj in Selection.gameObjects)
        {
            string name = obj.name;
            foreach(var i in allT)
            {
                if (i.name == name && i.gameObject != obj.gameObject)
                {
                    var posOff = i.position - obj.transform.position;
                    Debug.Log("posOffset:" + posOff + "|" + name);
                }
            }
        }
    }

    [MenuItem("Tools/Transform/SetParentNull")]
    public static void SetParentNull()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.SetParent(null);
        }
    }

    [MenuItem("Tools/Transform/Reset")]
    public static void Reset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        foreach (var obj in Selection.gameObjects)
        {
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero;
        }
    }

    [MenuItem("Tools/Transform/LayoutX10")]
    public static void LayoutX10()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero+i*Vector3.forward*1f;
        }
    }
    [MenuItem("Tools/Transform/LayoutX05")]
    public static void LayoutX05()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.5f;
        }
    }
    [MenuItem("Tools/Transform/LayoutX01")]
    public static void LayoutX01()
    {
        var allT = GameObject.FindObjectsOfType<Transform>();
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.1f;
        }
    }

    [MenuItem("Tools/Renderers/ShowSelection")]
    public static void ShowSelectionRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("Tools/Renderers/HideSelection")]
    public static void HideSelectionRenders()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    [MenuItem("Tools/Renderers/ShowAll")]
    public static void ShowAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("Tools/Renderers/HideAll")]
    public static void HideAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("Tools/Collider/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("Tools/Collider/AddBoxCollider_IsTrigger")]
    public static void AddBoxCollider_IsTrigger()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("Tools/Collider/AddBoxCollider_IsTrigger_NotRemoveChild")]
    public static void AddBoxCollider_IsTrigger_NotRemoveChild()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform, false);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 添加所有的MeshCollider
    /// </summary>
    [MenuItem("Tools/Collider/AddAllMeshCollider")]
    public static void AddAllMeshCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        AddAllMeshCollider(parent);
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        var meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }
}

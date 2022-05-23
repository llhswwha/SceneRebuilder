using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CommonExtension
{
    public static class GameObjectExtension
    {

        public static void CenterPivot(Transform t, Vector3 center)
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
        public static void SetParentTransfrom(Transform t, Action setTranfromActoin)
        {
#if UNITY_EDITOR
            InnerEditorHelper.UnpackPrefab(t.gameObject);
#endif
            Transform p = t.parent;

            List<Transform> children = new List<Transform>();
            for (int i = 0; i < t.childCount; i++)
            {
                children.Add(t.GetChild(i));
            }
            foreach (var child in children)
            {
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

            foreach (var child in children)
            {
                child.SetParent(t);
            }

            t.SetParent(p);
        }

        public static Vector3[] CenterPivot(Transform t, IEnumerable<MeshFilter> meshFilters)
        {
            var minMax = VertexHelper.GetMinMax(meshFilters);
            CenterPivot(t, minMax[3]);
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
                CenterPivot(t.gameObject);
            }
            ProgressBarHelper.ClearProgressBar();
            Debug.LogError($"CenterPivotAll root:{root} ts:{ts.Length} time:{DateTime.Now - startT}");
        }

        public static Vector3[] CenterPivot(GameObject go)
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
                return minMax;
            }
        }


        public static void ClearChildren(this GameObject root)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == null) continue;
                if (child.gameObject == root) continue;
                GameObject.DestroyImmediate(child.gameObject);
            }
        }



        public static GameObject CopyMeshObject(this GameObject source)
        {
            return CopyMeshObject(source, source.name);
        }


        public static GameObject CopyMeshObject(this GameObject source, string newName)
        {
            GameObject newGo = new GameObject(newName);
            bool r1 = CopyTransformMesh(source.gameObject, newGo);
            return newGo;
        }

        public static bool CopyTransformMesh(this GameObject source, GameObject target)
        {
            bool r = CopyMeshComponents(source, target);
            if (r == false) return r;
            CopyTransfrom(source.transform, target.transform);
            return true;
        }

        public static void CopyTransfrom(this Transform source, Transform target)
        {
            if (target.parent != source.parent)
            {
                //InnerEditorHelper.UnpackPrefab(target.gameObject);
                target.SetParent(source.parent);
            }
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        public static MeshFilter CreateMeshComponents(this GameObject go)
        {
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //MeshHelper.CopyMeshComponents(go, this.gameObject);
            //GameObject.DestroyImmediate(go);
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = go.AddComponent<MeshRenderer>();
                //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                meshRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));
            }
            MeshFilter meshFilter = go.AddMissingComponent<MeshFilter>();
            return meshFilter;
        }

        public static bool CopyMeshComponents(this GameObject source, GameObject target)
        {
            if (source == null)
            {
                Debug.LogError($"CopyMeshComponents source == null target:{target}");
                return false;
            }
            if (source == null)
            {
                Debug.LogError($"CopyMeshComponents target == null source:{source}");
                return false;
            }
            MeshRenderer meshRenderer1 = source.GetComponent<MeshRenderer>();
            MeshFilter meshFilter1 = source.GetComponent<MeshFilter>();
            if (meshRenderer1 == null)
            {
                Debug.LogError($"CopyMeshComponents meshRenderer1 == null source:{source} target:{target}");
                return false;
            }
            if (meshFilter1 == null)
            {
                Debug.LogError($"CopyMeshComponents meshFilter1 == null source:{source} target:{target}");
                return false;
            }

            MeshRenderer meshRenderer2 = target.GetComponent<MeshRenderer>();
            if (meshRenderer2 == null)
            {
                meshRenderer2 = target.AddMissingComponent<MeshRenderer>();
            }

            //Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials1  :{meshRenderer2.sharedMaterial} source:{source} target:{target}");

            if (meshRenderer2.sharedMaterial == null)
            {
                meshRenderer2.sharedMaterials = meshRenderer1.sharedMaterials;
            }

            //Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials2  :{meshRenderer2.sharedMaterial} source:{source} target:{target}");

            if (meshRenderer2.sharedMaterial == null)
            {
                Debug.LogError($"CopyMeshComponents meshRenderer2.sharedMaterials  == null source:{source} target:{target}");
            }


            MeshFilter meshFilter2 = target.AddMissingComponent<MeshFilter>();
            meshFilter2.sharedMesh = meshFilter1.sharedMesh;

            MeshCollider meshCollider2 = target.GetComponent<MeshCollider>();
            if (meshCollider2)
            {
                meshCollider2.sharedMesh = meshFilter1.sharedMesh;
            }
            return true;
        }

        static public T AddMissingComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }

        public static void RemoveMeshComponents(this GameObject go, bool isRemoveCollider = true)
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
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
    public static class LODUtility
    {
        //LODGroup下，不同层级的mesh上添加脚本。可通过BecameVisible知道LOD哪个等级被启用
        //void OnBecameVisible()
        //{
        //    Debug.Log("LOD Level " + LODLevel + " became visible");
        //}

        //void OnBecameInvisible()
        //{
        //    Debug.Log("LOD Level " + LODLevel + " became invisible");
        //}

        //Return the LODGroup component with a renderer pointing to a specific GameObject. If the GameObject is not part of a LODGroup, returns null 
        static public LODGroup GetParentLODGroupComponent(GameObject GO)
        {
            LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
            if (LODGroupParent == null)
                return null;
            LOD[] LODs = LODGroupParent.GetLODs();

            var FoundLOD = LODs.Where(lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0).ToArray();
            if (FoundLOD != null && FoundLOD.Count() > 0)
                return (LODGroupParent);

            return null;
        }


        //Return the GameObject of the LODGroup component with a renderer pointing to a specific GameObject. If the GameObject is not part of a LODGroup, returns null.
        static public GameObject GetParentLODGroupGameObject(GameObject GO)
        {
            var LODGroup = GetParentLODGroupComponent(GO);

            return LODGroup == null ? null : LODGroup.gameObject;
        }

        //Get the LOD # of a selected GameObject. If the GameObject is not part of any LODGroup returns -1.
        static public int GetLODid(GameObject GO)
        {
            LODGroup LODGroupParent = GO.GetComponentInParent<LODGroup>();
            if (LODGroupParent == null)
                return -1;
            LOD[] LODs = LODGroupParent.GetLODs();

            var index = Array.FindIndex(LODs, lod => lod.renderers.Where(renderer => renderer == GO.GetComponent<Renderer>()).ToArray().Count() > 0);
            return index;
        }


        //returns the currently visible LOD level of a specific LODGroup, from a specific camera. If no camera is define, uses the Camera.current.
        public static LODValueInfo GetVisibleLOD(LODGroup lodGroup, Camera camera = null)
        {
            LODValueInfo info = new LODValueInfo();
            var lods = lodGroup.GetLODs();
            var relativeHeight = GetRelativeHeight(lodGroup, camera ?? Camera.current);


            int lodIndex = GetMaxLOD(lodGroup);
            bool inNormalRange = false;
            for (var i = 0; i < lods.Length; i++)
            {
                var lod = lods[i];
                //Debug.Log(relativeHeight + " -" + lod.screenRelativeTransitionHeight);
                if (relativeHeight >= lod.screenRelativeTransitionHeight)
                {
                    lodIndex = i;
                    inNormalRange = true;
                    break;
                }
            }
            //GetLODs获取不到cullOFF这个百分比，当占比不在lods范围内，则处于cullOFF状态
            if (!inNormalRange && lods.Length > 0) lodIndex = lods.Length;
            info.currentLevel = lodIndex;
            info.currentPercentage = relativeHeight;
            return info;
        }

        //returns the currently visible LOD level of a specific LODGroup, from a the SceneView Camera.
        public static LODValueInfo GetVisibleLODSceneView(LODGroup lodGroup)
        {
#if UNITY_EDITOR
            Camera camera = SceneView.lastActiveSceneView.camera;
            return GetVisibleLOD(lodGroup, camera);
#endif
            return null;
        }

        static float GetRelativeHeight(LODGroup lodGroup, Camera camera)
        {
            var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
            return DistanceToRelativeHeight(camera, (distance / QualitySettings.lodBias), GetWorldSpaceSize(lodGroup));
        }

        static float DistanceToRelativeHeight(Camera camera, float distance, float size)
        {
            if (camera.orthographic)
                return size * 0.5F / camera.orthographicSize;

            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
            var relativeHeight = size * 0.5F / (distance * halfAngle);
            return relativeHeight;
        }
        public static int GetMaxLOD(LODGroup lodGroup)
        {
            return lodGroup.lodCount - 1;
        }
        public static float GetWorldSpaceSize(LODGroup lodGroup)
        {
            return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
        }
        static float GetWorldSpaceScale(Transform t)
        {
            var scale = t.lossyScale;
            float largestAxis = Mathf.Abs(scale.x);
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
            return largestAxis;
        }
    }
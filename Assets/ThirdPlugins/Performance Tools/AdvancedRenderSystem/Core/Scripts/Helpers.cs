using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace NGS.AdvancedRenderSystem
{
    [Serializable]
    public class BillboardOfGroupList
    {
        public List<MeshRenderer> renderers;

        public BillboardOfGroupList(IEnumerable<MeshRenderer> renderers)
        {
            this.renderers = new List<MeshRenderer>(renderers);
        }

        public MeshRenderer this[int index]
        {
            get
            {
                return renderers[index];
            }

            set
            {
                renderers[index] = value;
            }
        }

        public bool Contains(MeshRenderer renderer)
        {
            return renderers.Contains(renderer);
        }

        public Bounds GetBounds()
        {
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = Vector3.one * float.MinValue;

            for (int i = 0; i < renderers.Count; i++)
            {
                Vector3 boundsMin = renderers[i].bounds.min;
                Vector3 boundsMax = renderers[i].bounds.max;

                min.x = Mathf.Min(min.x, boundsMin.x);
                min.y = Mathf.Min(min.y, boundsMin.y);
                min.z = Mathf.Min(min.z, boundsMin.z);

                max.x = Mathf.Max(max.x, boundsMax.x);
                max.y = Mathf.Max(max.y, boundsMax.y);
                max.z = Mathf.Max(max.z, boundsMax.z);
            }

            return new Bounds((min + max) / 2, max - min);
        }
    }

    public static class LayersHelper
    {
#if UNITY_EDITOR

        public static int CreateLayer(string layerName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            int emptyLayerIdx = -1;

            SerializedProperty layerSP;
            for (int i = 0; i < layers.arraySize; i++)
            {
                layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == layerName)
                    return i;

                if ((layerSP.stringValue == null || layerSP.stringValue == "") && (emptyLayerIdx == -1 && i >= 8))
                    emptyLayerIdx = i;
            }

            if (emptyLayerIdx < 0)
            {
                Debug.Log("Advanced Render System require needs to take one layer. " +
                    "Please open 'Project Settins/Tags and Layers' and clear one layer field." +
                    "Then open this tool again");

                return -1;
            }

            layerSP = layers.GetArrayElementAtIndex(emptyLayerIdx);
            layerSP.stringValue = layerName;

            tagManager.ApplyModifiedProperties();

            for (int i = 0; i < layers.arraySize; i++)
            {
                layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == null || layerSP.stringValue == "")
                    continue;

                Physics.IgnoreLayerCollision(LayerMask.NameToLayer(layerName), LayerMask.NameToLayer(layerSP.stringValue));
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(layerName), LayerMask.NameToLayer(layerSP.stringValue));
            }

            tagManager.ApplyModifiedProperties();

            return emptyLayerIdx;
        }

#else
        public static int CreateLayer(string layerName)
        {
            return -1;
        }
#endif
    }
}
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Battlehub.RTEditor.HDRP
{
    public static class RTEditorMenu
    {
        [MenuItem("Tools/Runtime Editor/Create Editor HDRP", priority = 100)]
        public static void CreateRuntimeEditor()
        {
            GameObject hdrpSupport = InstantiateHDRPSupport();
            if (hdrpSupport != null)
            {
                Undo.RegisterCreatedObjectUndo(hdrpSupport, "Battlehub.RTEditor.HDRPSupport");
            }
        }

        public static GameObject InstantiateHDRPSupport()
        {
            UnityObject prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/RTEditorInitHDRP.prefab", typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}
#endif
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;


namespace Battlehub.RTEditor.HDRP
{
    public static class RTExtensionsMenu
    {
        [MenuItem("Tools/Runtime Editor/Create Extensions HDRP", priority = 101)]
        public static void CreateExtensionsHDRP()
        {
            GameObject hdrpSupport = InstantiateHDRPSupport();
            Undo.RegisterCreatedObjectUndo(hdrpSupport, "Battlehub.RTExtensions.CreateHDRP");
        }

        public static GameObject InstantiateHDRPSupport()
        {
            UnityObject prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/EditorExtensionsHDRP.prefab", typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}
#endif
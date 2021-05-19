using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace NGS.SuperLevelOptimizer
{
    public static class AssetsManager 
    {
        public static T CreateAsset<T>(T source, string folderPath, string fileName) where T : Object
        {
            if (source == null)
                return null;

            folderPath = folderPath.EndsWith("/") ? folderPath : folderPath + "/";
            fileName = fileName.EndsWith(".asset") ? fileName : fileName + ".asset";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = folderPath + fileName;

            AssetDatabase.CreateAsset(source, fullPath);

            return (T) AssetDatabase.LoadAssetAtPath(fullPath, typeof(T));
        }

        public static GameObject CreatePrefab(GameObject go, string folderPath, string fileName)
        {
            if (go == null)
                return null;

            folderPath = folderPath.EndsWith("/") ? folderPath : folderPath + "/";
            fileName = fileName.EndsWith(".prefab") ? fileName : fileName + ".prefab";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = folderPath + fileName;

            return PrefabUtility.CreatePrefab(fullPath, go);
        }

        public static void DeleteAsset(Object source)
        {
            if (source == null)
                return;

            string path = AssetDatabase.GetAssetPath(source);

            if (path == null || path == "")
                return;

            AssetDatabase.DeleteAsset(path);
        }

        public static bool ContainsAsset(Object asset)
        {
            if (asset == null)
                return false;

            return AssetDatabase.Contains(asset);
        }

        public static void Refresh()
        {
            AssetDatabase.Refresh();
        }
    }
}

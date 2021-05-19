using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace NGS.SuperLevelOptimizer
{
    [CustomEditor(typeof(SuperLevelOptimizer))]
    public class SuperLevelOptimizerEditor : Editor
    {
        private new SuperLevelOptimizer target { get { return base.target as SuperLevelOptimizer; } }


        [MenuItem("Tools/NGSTools/SuperLevelOptimizer/Create Optimizer")]
        private static void CreateOptimizer()
        {
            Selection.activeGameObject = new GameObject("SuperLevelOptimizer", typeof(SuperLevelOptimizer));
        }

        [MenuItem("Tools/NGSTools/SuperLevelOptimizer/Create Prefab")]
        private static void CreatePrefab()
        {
            string folderPath = SuperLevelOptimizer.DataFolder + "Prefabs/";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();

                Debug.Log("Create folder for prefabs at " + folderPath);
            }

            if (Selection.activeGameObject == null)
            {
                Debug.Log("No object selected");
                return;
            }

            GameObject go = Selection.activeGameObject;
            string fullPath = SuperLevelOptimizer.DataFolder + "Prefabs/" + go.name + "(" + go.GetInstanceID() + ").prefab";

            PrefabUtility.CreatePrefab(fullPath, go);
            AssetDatabase.Refresh();

            Debug.Log("Prefab created : " + fullPath);
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Coefficient Table"))
            {
                var window = EditorWindow.GetWindow<CoefficientTableEditorWindow>();
                window.table = target.coefficientTable;
            }

            EditorGUILayout.Space();

            SuperLevelOptimizer.DataFolder = EditorGUILayout.TextField("Data Save Folder : ", SuperLevelOptimizer.DataFolder);

            target.bakeType = (BakeType)EditorGUILayout.EnumPopup("Bake Type : ", target.bakeType);

            if (target.bakeType == BakeType.Zonal)
            {
                Vector3 zoneCount = target.zoneCount;

                zoneCount.x = EditorGUILayout.IntField("Count X : ", (int)target.zoneCount.x);
                zoneCount.y = EditorGUILayout.IntField("Count Y : ", (int)target.zoneCount.y);
                zoneCount.z = EditorGUILayout.IntField("Count Z : ", (int)target.zoneCount.z);

                target.zoneCount = zoneCount;

                EditorGUILayout.Space();
            }

            target.searchState = (SearchState)EditorGUILayout.EnumPopup("Search State : ", target.searchState);

            if (target.searchState == SearchState.User)
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Open Manager Window"))
                {
                    var window = EditorWindow.GetWindow<ManagerWindow>();
                    window.optimizer = target;
                }

                #region RenderersDraw

                SerializedProperty s_prop = serializedObject.FindProperty("_objectsForCombine");

                serializedObject.Update();

                if (EditorGUILayout.PropertyField(s_prop))
                {
                    for (int i = 0; i < s_prop.arraySize; i++)
                        EditorGUILayout.PropertyField(s_prop.GetArrayElementAtIndex(i));
                }

                serializedObject.ApplyModifiedProperties();

                #endregion

                EditorGUILayout.Space();
            }
            else if (target.searchState == SearchState.ByTag)
            {
                target.searchTag = EditorGUILayout.TextField("Tag : ", target.searchTag);

                EditorGUILayout.Space();
            }

            target.combineState = (CombineState)EditorGUILayout.EnumPopup("Combine State : ", target.combineState);

            if (target.combineState == CombineState.CombineToPrefab)
            {
                target.prefabsFolder = EditorGUILayout.TextField("Folder Path : ", target.prefabsFolder);

                EditorGUILayout.Space();
            }

            target.saveColliders = EditorGUILayout.Toggle("Save Colliders : ", target.saveColliders);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Atlases"))
                OnCreateAtlases();

            if (GUILayout.Button("Combine Meshes"))
                OnCombineMeshes();

            OnDestroySourcesOption();

            OnDisplayUndoOption();
        }

        private void OnSceneGUI()
        {
            if (target.bakeType != BakeType.Zonal)
                return;

            Renderer[] renderers = target.GetRenderersForCombine();

            if (renderers == null || renderers.Length == 0)
                return;

            Bounds[] bounds = GetZones(renderers);

            UnityEditorHelper.SetFunction(DrawFunction.OnSceneGUI);
            for (int i = 0; i < bounds.Length; i++)
                UnityEditorHelper.DrawWireCube(bounds[i].center, bounds[i].size, Color.blue);
        }

        private Bounds[] GetZones(Renderer[] renderers)
        {
            Vector3 min = Vector3.one * Mathf.Infinity;
            Vector3 max = Vector3.one * (-Mathf.Infinity);

            for (int i = 0; i < renderers.Length; i++)
            {
                Bounds bounds = renderers[i].bounds;

                min.x = Mathf.Min(min.x, bounds.min.x);
                min.y = Mathf.Min(min.y, bounds.min.y);
                min.z = Mathf.Min(min.z, bounds.min.z);

                max.x = Mathf.Max(max.x, bounds.max.x);
                max.y = Mathf.Max(max.y, bounds.max.y);
                max.z = Mathf.Max(max.z, bounds.max.z);
            }


            List<Bounds> zones = new List<Bounds>();
            Vector3 size = new Vector3(
                (max.x - min.x) / target.zoneCount.x,
                (max.y - min.y) / target.zoneCount.y,
                (max.z - min.z) / target.zoneCount.z);

            for (int i = 0; i < target.zoneCount.x; i++)
                for (int c = 0; c < target.zoneCount.y; c++)
                    for (int j = 0; j < target.zoneCount.z; j++)
                    {
                        Vector3 center = new Vector3(
                            min.x + size.x / 2 + size.x * i,
                            min.y + size.y / 2 + size.y * c,
                            min.z + size.z / 2 + size.z * j);

                        zones.Add(new Bounds(center, size));
                    }

            return zones.ToArray();
        }


        private void OnCombineMeshes()
        {
            Renderer[] renderers = target.GetRenderersForCombine();

            Transform combinedRoot = null;

            if (target.bakeType == BakeType.FullScene)
                combinedRoot = MeshCombiner.CombineMeshes(renderers, target.saveColliders);
            
            else
            {
                foreach (var zone in GetZones(renderers))
                {
                    Renderer[] renderersForCombine = renderers.Where(r => zone.Contains(r.transform.position)).ToArray();

                    if (renderersForCombine.Length != 0)
                        combinedRoot = MeshCombiner.CombineMeshes(renderersForCombine, target.saveColliders);
                }
            }

            if (target.combineState == CombineState.CombineToPrefab && combinedRoot != null)
            {
                string combineName = "Combined_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_" + combinedRoot.GetInstanceID();

                AssetsManager.CreatePrefab(combinedRoot.gameObject, target.prefabsFolder, combineName);

                DestroyImmediate(combinedRoot.gameObject);

                foreach (var mark in (SLO_AtlasMark[]) FindObjectsOfTypeAll(typeof(SLO_AtlasMark)))
                    DestroyImmediate(mark.gameObject);

                foreach (var mark in (SLO_SourceMark[]) FindObjectsOfTypeAll(typeof(SLO_SourceMark)))
                {
                    mark.gameObject.SetActive(true);
                    DestroyImmediate(mark);
                }
            }
        }

        private void OnCreateAtlases()
        {
            Renderer[] renderers = target.GetRenderersForCombine();

            TexturePacker.CombineTextures(renderers, target.coefficientTable, AtlasSize._8192, SuperLevelOptimizer.DataFolder + "Atlases/");
        }

        private void OnDestroySourcesOption()
        {
            SLO_SourceMark[] marks = (SLO_SourceMark[]) FindObjectsOfTypeAll(typeof(SLO_SourceMark));

            if (marks == null || marks.Length == 0 || !GUILayout.Button("Destroy Sources"))
                return;

            foreach (var mark in marks)
            {
                if (mark == null)
                    continue;

                foreach (var child in mark.gameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject == mark.gameObject)
                        continue;

                    child.SetParent(mark.transform.parent);
                }

                DestroyImmediate(mark.gameObject);
            }

            foreach (var mark in FindObjectsOfType<SLO_CombinedMark>())
                DestroyImmediate(mark);
        }

        private void OnDisplayUndoOption()
        {
            SLO_SourceMark[] sourceMarks = (SLO_SourceMark[]) FindObjectsOfTypeAll(typeof(SLO_SourceMark));

            if (sourceMarks == null || sourceMarks.Length == 0)
                return;

            if (GUILayout.Button("Undo"))
                Undo();
        }

        private void Undo()
        {
            SLO_SourceMark[] sourceMarks = (SLO_SourceMark[])FindObjectsOfTypeAll(typeof(SLO_SourceMark));

            foreach (var mark in sourceMarks)
            {
                mark.gameObject.SetActive(true);
                DestroyImmediate(mark);
            }


            SLO_AtlasMark[] atlasMarks = FindObjectsOfType<SLO_AtlasMark>();
            for (int i = 0; i < atlasMarks.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Undo(Combined Textures)...", i + " of " + atlasMarks.Length, (float)i / atlasMarks.Length);

                foreach (var mat in atlasMarks[i].GetComponent<Renderer>().sharedMaterials)
                    MaterialUtil.DeleteMultimaterialData(mat);
            }


            SLO_CombinedMark[] combinedMarks = FindObjectsOfType<SLO_CombinedMark>();
            for (int i = 0; i < combinedMarks.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Undo(Combined Meshes)...", i + " of " + combinedMarks.Length, (float)i / combinedMarks.Length);

                foreach (var filter in combinedMarks[i].GetComponentsInChildren<MeshFilter>())
                    if (filter.sharedMesh != null && filter.sharedMesh.name.StartsWith("combined_"))
                        AssetsManager.DeleteAsset(filter.sharedMesh);

                DestroyImmediate(combinedMarks[i].gameObject);
            }


            EditorUtility.ClearProgressBar();

            AssetsManager.Refresh();
        }
    }
}

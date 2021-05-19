using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace NGS.SuperLevelOptimizer
{
    public static class MeshCombiner
    {
        public static Transform CombineMeshes(Renderer[] renderers, bool saveColliders)
        {
            if (renderers == null || renderers.Length == 0)
                return null;

            Dictionary<string, List<MeshFilter>> groupedMeshes = GroupMeshesByMaterials(renderers);

            if (groupedMeshes.Keys.Count == 0)
                return null;

            Transform root = new GameObject("CombinedMeshes", typeof(SLO_CombinedMark)).transform;

            int idx = 0, count = groupedMeshes.Keys.Count;
            foreach (var key in new List<string>(groupedMeshes.Keys))
            {
                if (EditorUtility.DisplayCancelableProgressBar("Combining meshes", "Ready : " + idx + " of " + count, (float)idx / count))
                    break;

                idx++;

                if (groupedMeshes[key].Count < 2)
                    continue;

                List<MeshFilter> filters = groupedMeshes[key];
                MeshGroup currentGroup = new MeshGroup(filters[0].GetComponent<Renderer>().sharedMaterials);

                foreach (var filter in filters)
                {
                    if (currentGroup.CanMergeMesh(filter))
                        currentGroup.MergeMesh(filter);

                    else
                    {
                        CombineGroup(currentGroup, root, saveColliders);

                        currentGroup = new MeshGroup(currentGroup.Materials);
                        currentGroup.MergeMesh(filter);
                    }

                    filter.gameObject.AddComponent<SLO_SourceMark>();
                    filter.gameObject.SetActive(false);
                }

                CombineGroup(currentGroup, root, saveColliders);
            }

            EditorUtility.ClearProgressBar();

            AssetsManager.Refresh();

            return root;
        }

        private static void SaveColliders(Transform parent, Renderer[] renderers)
        {
            foreach (var renderer in renderers)
            {
                foreach (var collider in renderer.gameObject.GetComponents<Collider>())
                {
                    GameObject obj = new GameObject("Collider");

                    obj.transform.position = collider.transform.position;
                    obj.transform.rotation = collider.transform.rotation;
                    obj.transform.localScale = collider.transform.lossyScale;

                    obj.transform.SetParent(parent);

                    ComponentUtility.CopyComponent(collider);
                    ComponentUtility.PasteComponentAsNew(obj);
                }
            }
        }


        private static Dictionary<string, List<MeshFilter>> GroupMeshesByMaterials(Renderer[] renderers)
        {
            Dictionary<string, List<MeshFilter>> groupedMeshes = new Dictionary<string, List<MeshFilter>>();

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshFilter filter = renderers[i].GetComponent<MeshFilter>();

                if (filter == null)
                    continue;

                string key = "";
                foreach (var material in renderers[i].sharedMaterials)
                    if(material != null)
                        key += material.GetHashCode();

                if (!groupedMeshes.ContainsKey(key))
                    groupedMeshes.Add(key, new List<MeshFilter>());

                groupedMeshes[key].Add(filter);
            }

            return groupedMeshes;
        }

        private static void CombineGroup(MeshGroup group, Transform parent, bool saveColliders)
        {
            try
            {
                GameObject combined = group.Combine();

                if (combined == null)
                    return;

                MeshFilter filter = combined.GetComponent<MeshFilter>();

                filter.sharedMesh = AssetsManager.CreateAsset(filter.sharedMesh, SuperLevelOptimizer.DataFolder + "Meshes/", "combined_" + filter.sharedMesh.GetHashCode());
          
                if (saveColliders)
                    SaveColliders(combined.transform, group.MeshFilters.Select(f => f.GetComponent<Renderer>()).ToArray());

                combined.transform.SetParent(parent);
            }
            catch (Exception ex)
            {
                Debug.Log("At runtime exception occurred. Don't worry, it didn't lead to critical consequences. Please inform the developer(e-mail: andre-orsk@yandex.ru)");
                Debug.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
   
    public class MeshGroup
    {
        public List<MeshFilter> MeshFilters { get; private set; }
        public Material[] Materials { get; private set; }
        public int VertexCount { get; private set; }

        private int _uvsCode = -1;


        public MeshGroup(Material[] materials)
        {
            MeshFilters = new List<MeshFilter>();
            Materials = materials;
        }

        public bool CanMergeMesh(MeshFilter filter)
        {
            if (_uvsCode != -1 && _uvsCode != CalculateUVsCode(filter.sharedMesh))
                return false;

            return VertexCount + filter.sharedMesh.vertexCount < 63999;
        }

        public void MergeMesh(MeshFilter filter)
        {
            if (MeshFilters.Contains(filter))
                return;

            if (_uvsCode == -1)
                _uvsCode = CalculateUVsCode(filter.sharedMesh);

            MeshFilters.Add(filter);

            VertexCount += filter.sharedMesh.vertexCount;
        }

        public GameObject Combine(Transform parent = null)
        {
            GameObject combined = new GameObject(MeshFilters[0].name + "_combined");

            Renderer renderer = combined.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = Materials;

            MeshFilter filter = combined.AddComponent<MeshFilter>();
            filter.sharedMesh = CreateMesh();

            Unwrapping.GenerateSecondaryUVSet(filter.sharedMesh);
            
            combined.isStatic = true;
            combined.transform.SetParent(parent);

            return combined;
        }


        private int CalculateUVsCode(Mesh mesh)
        {
            int code = 0;

            for (int i = 0; i < SuperLevelOptimizer.UVsCount; i++)
            {
                List<Vector2> uv = new List<Vector2>();

                mesh.GetUVs(i, uv);

                if (uv.Count == mesh.vertexCount)
                    code += (int)Mathf.Pow(10, i + 1);
            }

            return code;
        }
      
        private Mesh CreateMesh()
        {
            Mesh[] meshes = MeshFilters.Select(f => f.sharedMesh).ToArray();
            Matrix4x4[] matricies = MeshFilters.Select(filter => filter.transform.localToWorldMatrix).ToArray();

            Vector3[] vertices = new Vector3[VertexCount];
            Vector3[] normals = new Vector3[VertexCount];
            Vector4[] tangents = new Vector4[VertexCount];
            
            List<int[]> triangles = new List<int[]>();
            List<List<Vector2>> uvs = new List<List<Vector2>>();
            List<int> uvsChannels = new List<int>();

            #region vertices

            int offset = 0;

            for (int i = 0; i < meshes.Length; i++)
                GetVertices(meshes[i].vertexCount, meshes[i].vertices, vertices, ref offset, matricies[i]);

            #endregion

            #region normals

            offset = 0;

            for (int i = 0; i < meshes.Length; i++)
                GetNormal(meshes[i].vertexCount, meshes[i].normals, normals, ref offset, matricies[i]);

            #endregion

            #region tangents

            offset = 0;

            for (int i = 0; i < meshes.Length; i++)
                GetTangents(meshes[i].vertexCount, meshes[i].tangents, tangents, ref offset, matricies[i]);

            #endregion

            #region triangles

            for (int i = 0; i < meshes[0].subMeshCount; i++)
            {
                int curTrianglesCount = 0;

                for (int c = 0; c < meshes.Length; c++)
                    curTrianglesCount = curTrianglesCount + meshes[c].GetTriangles(i).Length;

                int[] curTriangles = new int[curTrianglesCount];

                int triangleOffset = 0;
                int vertexOffset = 0;

                for (int c = 0; c < meshes.Length; c++)
                {
                    int[] inputTriangles = meshes[c].GetTriangles(i);

                    for (int p = 0; p < inputTriangles.Length; p += 3)
                    {
                        Vector3 scale = MeshFilters[c].transform.lossyScale;

                        if (scale.x < 0 || scale.y < 0 || scale.z < 0)
                        {
                            curTriangles[p + triangleOffset] = inputTriangles[p + 2] + vertexOffset;
                            curTriangles[p + 1 + triangleOffset] = inputTriangles[p + 1] + vertexOffset;
                            curTriangles[p + 2 + triangleOffset] = inputTriangles[p] + vertexOffset;
                        }
                        else
                        {
                            curTriangles[p + triangleOffset] = inputTriangles[p] + vertexOffset;
                            curTriangles[p + 1 + triangleOffset] = inputTriangles[p + 1] + vertexOffset;
                            curTriangles[p + 2 + triangleOffset] = inputTriangles[p + 2] + vertexOffset;
                        }
                    }

                    triangleOffset += inputTriangles.Length;
                    vertexOffset += meshes[c].vertexCount;
                }

                triangles.Add(curTriangles);
            }

            #endregion

            #region UVs

            for (int i = 0; i < SuperLevelOptimizer.UVsCount; i++)
            {
                List<Vector2> uv = new List<Vector2>();
                meshes[0].GetUVs(i, uv);

                if (uv.Count != meshes[0].vertexCount)
                    continue;

                uv.Clear();

                for (int c = 0; c < meshes.Length; c++)
                {
                    List<Vector2> meshUV = new List<Vector2>();

                    meshes[c].GetUVs(i, meshUV);

                    uv.AddRange(meshUV);
                }

                uvs.Add(uv);
                uvsChannels.Add(i);
            }

            #endregion

            Mesh mesh = new Mesh
            {
                name = "CombineMesh",

                vertices = vertices,
                normals = normals,
                tangents = tangents,
               
                subMeshCount = meshes[0].subMeshCount
            };

            for (int i = 0; i < mesh.subMeshCount; i++)
                mesh.SetTriangles(triangles[i], i);

            for (int i = 0; i < uvsChannels.Count; i++)
                mesh.SetUVs(uvsChannels[i], uvs[i]);

            return mesh;
        }

        private void GetVertices(int vertexcount, Vector3[] sources, Vector3[] main, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < sources.Length; i++)
                main[i + offset] = transform.MultiplyPoint(sources[i]);

            offset += vertexcount;
        }

        private void GetNormal(int vertexcount, Vector3[] sources, Vector3[] main, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < sources.Length; i++)
                main[i + offset] = transform.MultiplyVector(sources[i]).normalized;

            offset += vertexcount;
        }

        private void GetTangents(int vertexcount, Vector4[] sources, Vector4[] main, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < sources.Length; i++)
            {
                Vector4 p4 = sources[i];
                Vector3 p = new Vector3(p4.x, p4.y, p4.z);
                p = transform.MultiplyVector(p).normalized;
                main[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
            }

            offset += vertexcount;
        }
    }
}

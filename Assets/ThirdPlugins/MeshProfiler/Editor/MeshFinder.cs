using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using System.Text;

namespace MeshProfilerNS
{
    public class MeshElement
    {
        public string name;
        public string assetPath;
        public bool isGroup = false;
        public List<MeshValues> childList;
        public MeshValues rootMeshValue;
        public List<GameObject> refList;
        public GameObject rootObj;
        public int AllVertsNum;
        public bool isAsset=false;
        public bool isSkin=false;

        public MeshElement(GameObject _rootObj, bool _isAsset,bool _isSkin=false)
        {
            rootObj = _rootObj;
            isAsset = _isAsset;
            isSkin = _isSkin;
            refList = new List<GameObject>();
            RefleshProps();

        }

        public void RefleshProps()
        {
            childList = new List<MeshValues>();
            rootMeshValue = new MeshValues();

            if (!isSkin)
            {
                List<MeshFilter> filters = new List<MeshFilter>();
                filters.AddRange(rootObj.GetComponentsInChildren<MeshFilter>());
                for (int i = filters.Count - 1; i >= 0; i--)
                {
                    if (filters[i].sharedMesh == null)
                    {
                        filters.RemoveAt(i);
                    }
                }
                for (int i = 1; i < filters.Count; i++)
                    for (int j = 0; j < filters.Count - i; j++)
                    {
                        if (filters[j].sharedMesh.vertexCount < filters[j + 1].sharedMesh.vertexCount)
                        {
                            MeshFilter temp = filters[j];
                            filters[j] = filters[j + 1];
                            filters[j + 1] = temp;
                        }
                    }

                if (isAsset)
                {
                    assetPath = AssetDatabase.GetAssetPath(filters[0].sharedMesh);
                    name = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
                }
                else
                {
                    name = filters[0].sharedMesh.name + "_InsID=" + filters[0].sharedMesh.GetInstanceID();
                }
                rootMeshValue.parentName = name;
                for (int i = 0; i < filters.Count; i++)
                {
                    if (filters[i].sharedMesh != null)
                    {
                        MeshValues value = new MeshValues(filters[i].sharedMesh);
                        value.parentName = name;
                        childList.Add(value);
                        rootMeshValue += value;
                    }
                }
                if (childList.Count == 1)
                {
                    rootMeshValue.mesh = filters[0].sharedMesh;
                }
                isGroup = childList.Count > 1;
            }
            else
            {
                List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>();
                skins.AddRange(rootObj.GetComponentsInChildren<SkinnedMeshRenderer>());
                for (int i = skins.Count - 1; i >= 0; i--)
                {
                    if (skins[i].sharedMesh == null)
                    {
                        skins.RemoveAt(i);
                    }
                }
                for (int i = 1; i < skins.Count; i++)
                    for (int j = 0; j < skins.Count - i; j++)
                    {
                        if (skins[j].sharedMesh.vertexCount < skins[j + 1].sharedMesh.vertexCount)
                        {
                            SkinnedMeshRenderer temp = skins[j];
                            skins[j] = skins[j + 1];
                            skins[j + 1] = temp;
                        }
                    }

                if (isAsset)
                {
                    assetPath = AssetDatabase.GetAssetPath(skins[0].sharedMesh);
                    name = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
                }
                else
                {
                    name = skins[0].sharedMesh.name + "_InsID=" + skins[0].sharedMesh.GetInstanceID();
                }
                rootMeshValue.parentName = name;
                for (int i = 0; i < skins.Count; i++)
                {
                    if (skins[i].sharedMesh != null)
                    {
                        MeshValues value = new MeshValues(skins[i].sharedMesh);
                        value.parentName = name;
                        childList.Add(value);
                        rootMeshValue += value;
                    }
                }
                if (childList.Count == 1)
                {
                    rootMeshValue.mesh = skins[0].sharedMesh;
                }
                isGroup = childList.Count > 1;
            }
        }
    }


    public class MeshValues
    {
        public string parentName = "";
        public Mesh mesh = null;
        public int vertCount = 0;
        public int triangles = 0;
        public float memory = 0;
        public bool exist_normals;
        public bool exist_tangents;
        public bool exist_colors;
        public bool[] exist_uv = new bool[4];
        public bool isRead;

        public List<StringBuilder> GetVerticsStr()
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count = 0;
            int sum = mesh.vertexCount;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < sum; i ++)
            {
                if (i % 500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }

                list[count - 1].Append("[" + i + "]——" + vertices[i].ToString("F3") + "\n");
            }
            return list;
        }
        public List<StringBuilder> GetTrianglesStr()
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count=0;
            int sum = mesh.triangles.Length;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < sum; i +=3)
            {
                if (i % 1500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }
                list[count - 1].Append("[" + i/3 + "]——" + triangles[i] + " " + triangles[i + 1] + " " + triangles[i + 2] + "\n");
            }
            return list;
        }

        public List<StringBuilder> GetColorsStr()
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count = 0;
            int sum = mesh.colors.Length;
            Color[] colors = mesh.colors;
            for (int i = 0; i < sum; i++)
            {
                if (i % 500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }

                list[count - 1].Append("[" + i + "]——" + colors[i].ToString("F3") + "\n");
            }
            return list;
        }
        public List<StringBuilder> GetTangentsStr()
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count = 0;
            int sum = mesh.tangents.Length;
            Vector4 []tangents = mesh.tangents;
            for (int i = 0; i < sum; i++)
            {
                if (i % 500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }

                list[count - 1].Append("[" + i + "]——" +tangents[i].ToString("F3") + "\n");
            }
            return list;
        }
        public List<StringBuilder> GetNormalsStr()
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count = 0;
            int sum = mesh.normals.Length;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < sum; i++)
            {
                if (i % 500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }

                list[count - 1].Append("[" + i + "]——" + normals[i].ToString("F3")+"\n");
            }
            return list;
        }
        public List<StringBuilder> GetUVStr(int index)
        {
            List<StringBuilder> list = new List<StringBuilder>();
            int count = 0;
            List<Vector2> uvList = new List<Vector2>();
            mesh.GetUVs(index, uvList);
            int sum = uvList.Count;
            for (int i = 0; i < sum; i++)
            {
                if (i % 500 == 0)
                {
                    list.Add(new StringBuilder());
                    count++;
                }

                list[count - 1].Append("[" + i + "]——" + uvList[i].ToString("F3") + "\n");
            }
            return list;
        }

        public MeshValues(Mesh mesh = null)
        {
            this.mesh = mesh;
            if (mesh == null)
                return;
            vertCount = mesh.vertexCount;
            triangles = mesh.triangles.Length / 3;
            exist_normals = mesh.normals != null && mesh.normals.Length != 0;
            exist_tangents = mesh.tangents != null && mesh.tangents.Length != 0;
            exist_colors = mesh.colors != null && mesh.colors.Length != 0;
            exist_uv[0] = mesh.uv != null && mesh.uv.Length != 0;
            exist_uv[1] = mesh.uv2 != null && mesh.uv2.Length != 0;
            exist_uv[2] = mesh.uv3 != null && mesh.uv3.Length != 0;
            exist_uv[3] = mesh.uv4 != null && mesh.uv4.Length != 0;
            memory = Profiler.GetRuntimeMemorySizeLong(mesh) / 1024f;
            isRead = mesh.isReadable;
        }


        public static MeshValues operator +(MeshValues a, MeshValues b)
        {
            MeshValues value = new MeshValues();
            value.vertCount = a.vertCount + b.vertCount;
            value.triangles = a.triangles + b.triangles;
            value.memory = a.memory + b.memory;
            value.exist_normals = a.exist_normals || b.exist_normals;
            value.exist_tangents = a.exist_tangents || b.exist_tangents;
            value.exist_colors = a.exist_colors || b.exist_colors;
            value.exist_uv[0] = a.exist_uv[0] || b.exist_uv[0];
            value.exist_uv[1] = a.exist_uv[1] || b.exist_uv[1];
            value.exist_uv[2] = a.exist_uv[2] || b.exist_uv[2];
            value.exist_uv[3] = a.exist_uv[3] || b.exist_uv[3];
            value.isRead = a.isRead || b.isRead;
            return value;
        }

    }
    public class MeshFinder
    {
        public static List<MeshElement> GetMeshElementList()
        {
            Dictionary<string, MeshElement> meshDataDict = new Dictionary<string, MeshElement>();
            MeshFilter[] filters = GameObject.FindObjectsOfType<MeshFilter>();
            
            int oldInstanceID = -1;
            for (int i = 0; i < filters.Length; i++)
            {
                if (filters[i].sharedMesh == null)
                    continue;
                string assetPath = AssetDatabase.GetAssetPath(filters[i].sharedMesh);
                if ((!string.IsNullOrEmpty(assetPath) && !assetPath.Contains("unity default resources")))//is Asset
                {
#if UNITY_2018_3_OR_NEWER
                    GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(filters[i].gameObject);
#else
                    GameObject root = PrefabUtility.FindPrefabRoot(filters[i].gameObject);
#endif
                    if (root == null)
                    {
                        if (!assetPath.EndsWith(".asset"))
                            continue;
                        else
                        {
                            root = filters[i].gameObject;
                        }
                    }

                    
                    if (!meshDataDict.ContainsKey(assetPath))
                    {
                        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                        if (obj == null)
                        {
                            obj = root;
                        }
                        MeshElement element = new MeshElement(obj, true);
                        meshDataDict.Add(assetPath, element);

                    }

                    int newInstanceID = root.GetInstanceID();
                    if (newInstanceID != oldInstanceID)
                    {
                        meshDataDict[assetPath].refList.Add(root);
                        oldInstanceID = newInstanceID;
                    }
                }
                else
                {
                    string id = filters[i].sharedMesh.GetInstanceID().ToString();
                    if (!meshDataDict.ContainsKey(id))
                    {
                        MeshElement element = new MeshElement(filters[i].gameObject, false);
                        meshDataDict.Add(id, element);
                        element.refList.Add(filters[i].gameObject);
                        element.AllVertsNum += filters[i].sharedMesh.vertexCount;
                    }

                    else
                    {
                        MeshElement element = meshDataDict[id];
                        element.refList.Add(filters[i].gameObject);
                        element.AllVertsNum += filters[i].sharedMesh.vertexCount;

                    }

                }
            }
            //search skins
            SkinnedMeshRenderer[] skins = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();            for (int i = 0; i < skins.Length; i++)
            {
                if (skins[i].sharedMesh == null)
                    continue;
                string assetPath = AssetDatabase.GetAssetPath(skins[i].sharedMesh);
                if ((!string.IsNullOrEmpty(assetPath) && !assetPath.Contains("unity default resources")))//is Asset
                {
#if UNITY_2018_3_OR_NEWER
                    GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(skins[i].gameObject);
#else
                    GameObject root = PrefabUtility.FindPrefabRoot(skins[i].gameObject);
#endif
                    if (root == null)
                    {
                        continue;
                    }
                    int newInstanceID = root.GetInstanceID();
                    if (!meshDataDict.ContainsKey(assetPath))
                    {
                        GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                        if (obj == null)
                        {
                            obj = root;
                        }
                        MeshElement element = new MeshElement(obj, true,true);
                        meshDataDict.Add(assetPath, element);

                    }
                    if (newInstanceID != oldInstanceID)
                    {
                        meshDataDict[assetPath].refList.Add(root);
                        oldInstanceID = newInstanceID;
                    }

                }
                else
                {
                    string id = skins[i].sharedMesh.GetInstanceID().ToString();
                    if (!meshDataDict.ContainsKey(id))
                    {
                        MeshElement element = new MeshElement(skins[i].gameObject, false,true);
                        meshDataDict.Add(id, element);
                        element.refList.Add(skins[i].gameObject);
                        element.AllVertsNum += skins[i].sharedMesh.vertexCount;
                    }

                    else
                    {
                        MeshElement element = meshDataDict[id];
                        element.refList.Add(skins[i].gameObject);
                        element.AllVertsNum += skins[i].sharedMesh.vertexCount;

                    }

                }
            }

            List<MeshElement> meshDataList = new List<MeshElement>();
            foreach (string key in meshDataDict.Keys)
            {
                meshDataDict[key].AllVertsNum = meshDataDict[key].rootMeshValue.vertCount * meshDataDict[key].refList.Count;
                meshDataList.Add(meshDataDict[key]);
            }
            return meshDataList;
        }
    }
}

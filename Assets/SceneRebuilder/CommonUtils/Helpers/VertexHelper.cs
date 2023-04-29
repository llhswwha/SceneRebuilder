using CommonExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace CommonUtils
{
    public static class VertexHelper
    {
        public static int GetVertexCount(GameObject go)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf == null) return 0;
            if (mf.sharedMesh == null) return 0;
            return mf.sharedMesh.vertexCount;
        }

        public static string GetVertexCountS(float vertexCount)
        {
            float f = vertexCount / 10000f;
            if (f >= 100)
            {
                return $"{f:F0}";
            }
            else if (f >= 10)
            {
                return $"{f:F1}";
            }
            else if (f >= 1)
            {
                return $"{f:F2}";
            }
            else if (vertexCount >= 1000)
            {
                return $"{f:F3}";
            }
            //else if (vertexCount >= 100)
            //{
            //    return $"{f:F3}";
            //}
            //else if (vertexCount >= 10)
            //{
            //    return $"{f:F3}";
            //}
            else if (vertexCount == 0)
            {
                return "0";
            }
            else
            {
                return $"{f:F3}";
            }
        }

        public static Vector3 GetCenterOfList(List<Vector3> list)
        {
            Vector3 center = Vector3.zero;
            foreach (var item in list)
            {
                center += item;
            }
            center /= list.Count;
            return center;
        }

        //public static Vector3 GetCenterOfList(MeshPointList list)
        //{
        //    return list.GetCenter();
        //}

        //public static Vector3 GetCenterOfList(SharedMeshTrianglesList list)
        //{
        //    return list.GetCenter();
        //}

        //public static float3 FindClosedPoint(float3 p, NativeArray<float3> list)
        //{
        //    float minDis = float.MaxValue;
        //    float3 minP = float3.zero;
        //    foreach (float3 item in list)
        //    {
        //        bool3 b3 = item == p;
        //        if (b3.x && b3.y && b3.z) continue;
        //        float dis = math.distance(item, p);
        //        if (dis < minDis)
        //        {
        //            minDis = dis;
        //            minP = item;
        //        }
        //    }
        //    return minP;
        //}

        public static Vector3 FindClosedPoint(Vector3 p, NativeArray<Vector3> list)
        {
            float minDis = float.MaxValue;
            Vector3 minP = Vector3.zero;
            foreach (var item in list)
            {
                if (item == p) continue;
                float dis = Vector3.Distance(item, p);
                if (dis < minDis)
                {
                    minDis = dis;
                    minP = item;
                }
            }
            return minP;
        }

        public static Vector3 FindClosedPoint(Vector3 p, List<Vector3> list)
        {
            float minDis = float.MaxValue;
            Vector3 minP = Vector3.zero;
            foreach (var item in list)
            {
                if (item == p) continue;
                float dis = Vector3.Distance(item, p);
                if (dis < minDis)
                {
                    minDis = dis;
                    minP = item;
                }
            }
            return minP;
        }

        public static Vector3 GetClosedPointToPlane(Vector3 planeNormal, Vector3 planePoint, List<Vector3> list, bool isShowDebug = false, Transform t = null,string tag="")
        {
            if (list.Count == 0)
            {
                return planeNormal;
            }
            try
            {
                //var list = Plane1Points;
                float minD = float.MaxValue;
                int minI = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    Vector3 point = list[i];

                    //float dis = Math3D.DistanceToLine(linePoint, lineVec, point);

                    //Vector3 lineP = Math3D.ProjectPointOnLine(planeNormal, planePoint, point);
                    //float dis = Vector3.Distance(lineP, point);

                    float dis = Math.Abs(Math3D.SignedDistancePlanePoint(planeNormal, planePoint, point));
                    
                    //float dis = Vector3.Distance(list[i], linePoint);
                    if (dis < minD)
                    {
                        minD = dis;
                        minI = i;
                    }

                    if (isShowDebug)
                    {
                        //PointHelper.CreateLocalPoint(lineP, $"GetClosedPointToLine[{i}/{list.Count}] point:{point.Vector3ToString3()} dis:{dis} minD:{minD}", t, 0.01f);
                        Debug.Log($"GetClosedPointToPlane({tag})[{i}/{list.Count}] point:{point.Vector3ToString3()} dis:{dis} minD:{minD}");
                    }
                }
                return list[minI];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetClosedPointToPlane({tag}) linePoint:{planeNormal} lineVec:{planePoint} list:{list.Count} ex:{ex}");
                return Vector3.zero;
            }

        }

        public static Vector3 GetClosedPointToLine(Vector3 linePoint, Vector3 lineVec, List<Vector3> list, bool isShowDebug = false, Transform t = null)
        {
            if (list.Count == 0)
            {
                return linePoint;
            }
            try
            {
                //var list = Plane1Points;
                float minD = float.MaxValue;
                int minI = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    Vector3 point = list[i];

                    //float dis = Math3D.DistanceToLine(linePoint, lineVec, point);

                    Vector3 lineP = Math3D.ProjectPointOnLine(linePoint, lineVec, point);
                    float dis = Vector3.Distance(lineP, point);

                    //float dis = Vector3.Distance(list[i], linePoint);
                    if (dis < minD)
                    {
                        minD = dis;
                        minI = i;
                    }

                    if (isShowDebug)
                    {
                        //PointHelper.CreateLocalPoint(lineP, $"GetClosedPointToLine[{i}/{list.Count}] point:{point.Vector3ToString3()} dis:{dis} minD:{minD}", t, 0.01f);
                        Debug.Log($"GetClosedPointToLine[{i}/{list.Count}] point:{point.Vector3ToString3()} dis:{dis} minD:{minD}");
                    }
                }
                return list[minI];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetClosedPointToLine linePoint:{linePoint} lineVec:{lineVec} list:{list.Count} ex:{ex}");
                return Vector3.zero;
            }

        }

        public static Vector3 GetClosedPoint(Vector3 p, List<Vector3> list)
        {
            try
            {
                //var list = Plane1Points;
                float minD = float.MaxValue;
                int minI = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    float dis = Vector3.Distance(list[i], p);
                    if (dis < minD)
                    {
                        minD = dis;
                        minI = i;
                    }
                }
                return list[minI];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetClosedPlanePoint1 p:{p} list:{list.Count} ex:{ex}");
                return Vector3.zero;
            }

        }

        //public static int GetInstanceID(MeshPoints mf)
        //{
        //    //return mf.sharedMesh.GetInstanceID();
        //    return mf.InstanceId;
        //}

        public static GameObject CreateWorldPoint(Vector3 p, string n, float scale)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(scale, scale, scale);
            go.transform.position = p;
            go.name = n;
            return go;
        }



        public static GameObject ShowVertexes(Vector3[] vs, float scale, string name)
        {
            GameObject go = new GameObject(name);
            ShowVertexes(vs, scale, go.transform);
            return go;
        }

        public static List<GameObject> ShowVertexes(Transform target,float pScale)
        {
            MeshFilter mf = target.GetComponent<MeshFilter>();
            var vs = mf.sharedMesh.vertices;
            var vs2 = VertexHelper.GetWorldVertexes(vs, target);
            var gos = VertexHelper.ShowVertexes(vs2, pScale, target);
            return gos;
        }

        public static List<GameObject> ShowVertexes(Vector3[] vs, float scale, Transform parent)
        {
            List<GameObject> games = new List<GameObject>();
            var vcount = vs.Length;
            for (int i = 0; i < vcount; i++)
            {
                Vector3 p = vs[i];
                GameObject go = CreateWorldPoint(p, string.Format("[{0}]{1}", i, p), scale);
                go.transform.SetParent(parent);
                games.Add(go);
            }
            return games;
        }

        public static void ClearChildren(Transform t)
        {
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                children.Add(child);
            }
            foreach (var child in children)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        public static Vector3 GetCenter(GameObject go)
        {
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
            return GetMinMax(meshFilters)[3];
        }

        public static Vector3[] GetMinMax(GameObject go)
        {
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
            return GetMinMax(meshFilters);
        }

        public static Vector3[] GetMinMax(IEnumerable<MeshFilter> meshFilters)
        {
            if (meshFilters == null) return null;
            List<Vector3> allVs = new List<Vector3>();
            foreach (var mf in meshFilters)
            {
                if (mf == null) continue;
                Vector3[] vs = GetWorldVertexes(mf);
                if (vs == null) continue;
                allVs.AddRange(vs);
            }
            return GetMinMax(allVs.ToArray());
        }

        public static Vector3[] GetMinMax(IEnumerable<MeshRenderer> meshRenderers)
        {
            if (meshRenderers == null) return null;
            List<Vector3> allVs = new List<Vector3>();
            foreach (var mf in meshRenderers)
            {
                if (mf == null) continue;
                Vector3[] vs = GetWorldVertexes(mf);
                if (vs == null) continue;
                allVs.AddRange(vs);
            }
            return GetMinMax(allVs.ToArray());
        }

        public static Vector3[] GetMinMax<T>(T t) where T : Component
        {
            MeshFilter[] meshFilters = t.GetComponentsInChildren<MeshFilter>(true);
            return GetMinMax(meshFilters);
        }

        public static Vector3[] GetMinMax(MeshFilter meshFilter)
        {
            if (meshFilter == null) return null;
            Vector3[] vs = GetWorldVertexes(meshFilter);
            return GetMinMax(vs);
        }

        //public static Dictionary<int, List<float>> SizeListBuffer = new Dictionary<int, List<float>>();

        public static List<float> GetSizeList(Vector3[] vs, int id)
        {
            //if (SizeListBuffer.ContainsKey(id))
            //{
            //    return SizeListBuffer[id];
            //}
            var minMax = GetMinMax(vs);
            var size = minMax[2];
            List<float> sizeList = new List<float>() { size.x, size.y, size.z };
            sizeList.Sort();

            //SizeListBuffer.Add(id, sizeList);

            return sizeList;
        }

        public static Vector3[] GetMinMax(Vector3[] vs)
        {
            if (vs == null) return null;
            
            Vector3[] minMax = new Vector3[5];
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < vs.Length; i++)
            {
                var p = vs[i];
                sum += p;
                if (p.x < minX)
                {
                    minX = p.x;
                }
                if (p.y < minY)
                {
                    minY = p.y;
                }
                if (p.z < minZ)
                {
                    minZ = p.z;
                }
                if (p.x > maxX)
                {
                    maxX = p.x;
                }
                if (p.y > maxY)
                {
                    maxY = p.y;
                }
                if (p.z > maxZ)
                {
                    maxZ = p.z;
                }
            }
            minMax[0] = new Vector3(minX, minY, minZ);
            minMax[1] = new Vector3(maxX, maxY, maxZ);
            minMax[2] = minMax[1] - minMax[0];//size
            minMax[3] = (minMax[1] + minMax[0]) / 2;//center
            minMax[4] = sum / vs.Length;//weight

            //Debug.Log($"GetMinMax vs:{vs.Length} min:{minMax[0]} max:{minMax[1]}");
            return minMax;
        }

        public static Vector3[] GetChildrenWorldVertexes(GameObject t1)
        {
            List<Vector3> vertexes = new List<Vector3>();
            var meshFilters = t1.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh == null) continue;
                var vs = GetWorldVertexes(mf.sharedMesh, mf.transform);
                vertexes.AddRange(vs);
            }
            return vertexes.ToArray();
        }

        public static Vector3[] GetChildrenVertexes(GameObject t1)
        {
            List<Vector3> vertexes = new List<Vector3>();
            var meshFilters = t1.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh == null) continue;
                vertexes.AddRange(mf.sharedMesh.vertices);
            }
            return vertexes.ToArray();
        }

        public static Vector3[] GetWorldVertexes(Vector3[] vs, Transform t1)
        {
            var vCount = vs.Length;
            Vector3[] points1 = new Vector3[vCount];
            // var vs=mesh1.vertices;
            for (int i = 0; i < vCount; i++)
            {
                Vector3 p1 = vs[i];
                Vector3 p11 = t1.TransformPoint(p1);
                //points1.Add(p11);
                points1[i] = p11;
            }
            return points1;
        }

        public static Vector3[] GetWorldVertexes(Mesh mesh1, Transform t1)
        {
            if (mesh1 == null)
            {
                Debug.LogError($"GetWorldVertexes mesh1 == null t1:{t1}");
                return null;
            }
            if (t1 == null)
            {
                Debug.LogError($"GetWorldVertexes t1 == null mesh1:{mesh1}");
                return null;
            }
            var vs = mesh1.vertices;
            return GetWorldVertexes(vs, t1);
        }

        public static Vector3[] GetChildrenWorldVertexes<T>(T t1) where T : Component
        {
            List<Vector3> vertexes = new List<Vector3>();
            var meshFilters = t1.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh == null) continue;
                var vs = GetWorldVertexes(mf.sharedMesh, mf.transform);
                vertexes.AddRange(vs);
            }
            return vertexes.ToArray();
        }

        public static Vector3[] GetWorldVertexes<T>(T go) where T : Component
        {
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                return GetWorldVertexes(meshFilter.sharedMesh, go.transform);
            }
            else
            {
                return GetChildrenWorldVertexes(go);
            }
        }

        public static Vector3[] GetWorldVertexes(GameObject go)
        {
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                return GetWorldVertexes(meshFilter.sharedMesh, go.transform);
            }
            else
            {
                return GetChildrenWorldVertexes(go);
            }
        }

        public static Vector3[] GetWorldVertexes(MeshFilter meshFilter)
        {
            if (meshFilter == null) return null;
            if (meshFilter.sharedMesh == null) return null;
            try
            {
                if (Application.isPlaying)
                {
                    var vs2 = meshFilter.mesh.vertices;
                    var vs22 = GetWorldVertexes(vs2, meshFilter.transform);
                    return vs22;
                }
                else
                {
                    var vs2 = meshFilter.sharedMesh.vertices;
                    var vs22 = GetWorldVertexes(vs2, meshFilter.transform);
                    return vs22;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetWorldVertexes path:{meshFilter.transform.GetPath()} Exception:{ex} ");
                return new Vector3[0];
            }
        }


        public static int InvokeGetVertexDistanceExCount = 0;

        public static float GetAvgVertexDistanceEx(Transform t1, Transform t2, string progress = "", bool showLog = false)
        {
            if (t1 == null)
            {
                Debug.LogError($"GetVertexDistanceEx t1==null");
                return float.MaxValue;
            }
            if (t2 == null)
            {
                Debug.LogError($"GetVertexDistanceEx t2==null");
                return float.MaxValue;
            }
            return GetAvgVertexDistanceEx(new DistanceArgs(t1, t2, progress, showLog));
        }

        public static float GetAvgVertexDistanceEx(DistanceArgs arg)
        {
            if (arg.showLog) Debug.Log($"GetVertexDistanceEx {arg.t1.name}|{arg.t2.name}");
            Transform t1 = arg.t1;
            Transform t2 = arg.t2;


            Vector3 p01 = t1.position;
            Vector3 p02 = t2.position;

            if (arg.isResetPos)
            {
                t1.position = Vector3.zero;
                t2.position = Vector3.zero;
            }

            Quaternion q1 = t1.rotation;
            Quaternion q2 = t2.rotation;
            if (arg.isResetRotation)
            {
                t1.rotation = Quaternion.identity;
                t2.rotation = Quaternion.identity;
            }

            DateTime start = DateTime.Now;

            MeshFilter mf1 = t1.GetComponent<MeshFilter>();
            MeshFilter mf2 = t2.GetComponent<MeshFilter>();
            float dis = -1;
            if (mf1 == null || mf2 == null)
            {
                Debug.LogError($"GetAvgVertexDistanceEx mf1 == null || mf2 == null mf1:{mf1} mf2:{mf2}");
                return float.MaxValue;
            }
            else
            {
                Mesh mesh1 = mf1.sharedMesh;
                Mesh mesh2 = mf2.sharedMesh;
                if (mesh1 == null || mesh2 == null)
                {
                    Debug.LogError($"GetAvgVertexDistanceEx mesh1 == null || mesh2 == null mesh1:{mesh1} mesh2:{mesh2}");
                    return float.MaxValue;
                }
                else
                {
                    float distance = 0;
                    Vector3[] points1 = VertexHelper.GetWorldVertexes(mesh1, t1);
                    Vector3[] points2 = VertexHelper.GetWorldVertexes(mesh2, t2);
                    int count = points1.Length;
                    if (points2.Length < count)
                    {
                        count = points2.Length;
                    }
                    dis = DistanceUtil.GetDistance(points1, points2, arg.showLog) / count;
                }
            }

            if (arg.isResetPos)
            {
                t1.position = p01;
                t2.position = p02;
            }

            if (arg.isResetRotation)
            {
                t1.rotation = q1;
                t2.rotation = q2;
            }

            // if(isResetParent){
            //     t1.SetParent(parent1,true);
            //     t2.SetParent(parent2,true);
            // }

            InvokeGetVertexDistanceExCount++;

            return dis;
        }

        public static float GetVertexDistanceEx(Transform t1, Transform t2, string progress = "", bool showLog = false, bool local = false)
        {
            if (t1 == null)
            {
                Debug.LogError($"GetVertexDistanceEx t1==null");
                return float.MaxValue;
            }
            if (t2 == null)
            {
                Debug.LogError($"GetVertexDistanceEx t2==null");
                return float.MaxValue;
            }
            return GetVertexDistanceEx(new DistanceArgs(t1, t2, progress, showLog, local));
        }

        public static float GetVertexDistanceEx(GameObject g1, GameObject g2, string progress = "", bool showLog = false, bool local = false)
        {
            if (g1 == null)
            {
                Debug.LogError($"GetVertexDistanceEx g1==null");
                return float.MaxValue;
            }
            if (g2 == null)
            {
                Debug.LogError($"GetVertexDistanceEx g2==null");
                return float.MaxValue;
            }
            return GetVertexDistanceEx(new DistanceArgs(g1.transform, g2.transform, progress, showLog, local));
        }


        public static float GetVertexDistanceEx(DistanceArgs arg)
        {
            if (arg.showLog) Debug.Log($"GetVertexDistanceEx {arg.t1.name}|{arg.t2.name}");
            Transform t1 = arg.t1;
            Transform t2 = arg.t2;


            Vector3 p01 = t1.position;
            Vector3 p02 = t2.position;

            if (arg.isResetPos)
            {
                t1.position = Vector3.zero;
                t2.position = Vector3.zero;
            }

            Quaternion q1 = t1.rotation;
            Quaternion q2 = t2.rotation;
            if (arg.isResetRotation)
            {
                t1.rotation = Quaternion.identity;
                t2.rotation = Quaternion.identity;
            }

            DateTime start = DateTime.Now;

            MeshFilter mf1 = t1.GetComponent<MeshFilter>();
            MeshFilter mf2 = t2.GetComponent<MeshFilter>();
            float dis = -1;
            if (mf1 == null || mf2 == null)
            {
                //return -1;
                //Debug.LogWarning("mf1 == null || mf2 == null");
                Vector3[] points1 = VertexHelper.GetChildrenWorldVertexes(t1.gameObject);
                Vector3[] points2 = VertexHelper.GetChildrenWorldVertexes(t2.gameObject);
                dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
            }
            else
            {
                Mesh mesh1 = mf1.sharedMesh;
                if (mesh1 == null)
                {
                    Debug.LogError("mf1.sharedMesh==null:" + mf1);
                }
                Mesh mesh2 = mf2.sharedMesh;
                if (mesh2 == null)
                {
                    Debug.LogError("mf2.sharedMesh==null:" + mf2);
                }

                if (mesh1 == null || mesh2 == null)
                {
                    Vector3[] points1 = VertexHelper.GetChildrenWorldVertexes(t1.gameObject);
                    Vector3[] points2 = VertexHelper.GetChildrenWorldVertexes(t2.gameObject);
                    dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
                }
                else
                {
                    //Vector3[] points1 = GetWorldVertexes(mesh1, t1);
                    //Vector3[] points2 = GetWorldVertexes(mesh2, t2);

                    Vector3[] points1 = mesh1.vertices;
                    if (arg.isLocal == false)
                    {
                        points1 = VertexHelper.GetWorldVertexes(points1, t1);
                    }
                    Vector3[] points2 = mesh2.vertices;
                    if (arg.isLocal == false)
                    {
                        points2 = VertexHelper.GetWorldVertexes(points2, t2);
                    }
                    dis = DistanceUtil.GetDistance(points1, points2, arg.showLog);
                }
            }

            if (arg.isResetPos)
            {
                t1.position = p01;
                t2.position = p02;
            }

            if (arg.isResetRotation)
            {
                t1.rotation = q1;
                t2.rotation = q2;
            }

            // if(isResetParent){
            //     t1.SetParent(parent1,true);
            //     t2.SetParent(parent2,true);
            // }

            InvokeGetVertexDistanceExCount++;

            return dis;
        }

        public static GameObject CopyGameObjectMesh(MeshFilter mf)
        {
            Vector3 center = VertexHelper.GetCenter(mf.gameObject);

            MeshRenderer oldRender = mf.GetComponent<MeshRenderer>();
            GameObject newGo = new GameObject(mf.name + "_NewMesh");
            newGo.transform.position = center;
            MeshFilter newMf = newGo.AddMissingComponent<MeshFilter>();
            MeshRenderer newRender = newGo.AddMissingComponent<MeshRenderer>();
            newRender.sharedMaterial = oldRender.sharedMaterial;
            newMf.sharedMesh = VertexHelper.CopyMesh(mf);
            newMf.sharedMesh.name = mf.name;
            newGo.transform.SetParent(mf.transform.parent);
            return newGo;
        }

        public static Mesh CopyMesh(MeshFilter mf)
        {
            Vector3 center = VertexHelper.GetCenter(mf.gameObject);
            var sharedMesh = mf.sharedMesh;
            Vector3[] worldVertexes = VertexHelper.GetWorldVertexes(mf);
            for (int i = 0; i < worldVertexes.Length; i++)
            {
                worldVertexes[i] = worldVertexes[i] - center;
            }
            Mesh mesh = new Mesh();
            mesh.vertices = worldVertexes;
            mesh.triangles = sharedMesh.triangles;

            //mesh.normals = sharedMesh.normals;
            //mesh.uv = sharedMesh.uv;
            //mesh.indexFormat = sharedMesh.indexFormat;
            //mesh.tangents = sharedMesh.tangents;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mesh.name = sharedMesh.name + "_Copy";

            float dis = Vector3.Distance(center, mf.transform.position);
            //Debug.LogError($"CopyMesh1 mf:{mf} center:{center} position:{mf.transform.position} dis:{dis} vertices:{sharedMesh.vertices.Length} worldVertexes:{worldVertexes.Length} triangles:{sharedMesh.triangles.Length} normals:{sharedMesh.normals.Length} uv:{sharedMesh.uv.Length}");
            //Debug.LogError($"CopyMesh2 mf:{mf} center:{center} position:{mf.transform.position} dis:{dis} vertices:{mesh.vertices.Length} worldVertexes:{worldVertexes.Length} triangles:{mesh.triangles.Length} normals:{mesh.normals.Length} uv:{mesh.uv.Length}");
            return mesh;
        }
    }
}
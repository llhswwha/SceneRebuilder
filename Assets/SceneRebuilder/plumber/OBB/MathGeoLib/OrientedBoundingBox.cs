using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
// #if UNITY || UNITY_EDITOR
using UnityEngine;

// #endif

#pragma warning disable IDE1006 // naming rules blah blah blah

// ReSharper disable once CheckNamespace
namespace MathGeoLib
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3S
    {
        public readonly float X;

        public readonly float Y;

        public readonly float Z;

        public static Vector3S Right { get; } = new Vector3S(1, 0, 0);

        public static Vector3S Up { get; } = new Vector3S(0, 1, 0);

        public static Vector3S Forward { get; } = new Vector3S(0, 0, 1);

        public static Vector3S Zero { get; } = new Vector3S(0, 0, 0);

        public static Vector3S One { get; } = new Vector3S(1, 1, 1);

        public Vector3S(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3S(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public override string ToString()
        {
            //return $"({nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z})";

            return $"({X},{Y},{Z})";
        }

        public float GetDistance(Vector3S vector2)
        {
            float xx = this.X - vector2.X;
            float yy = this.Y - vector2.Y;
            float zz = this.Z - vector2.Z;
            float dis = xx * xx + yy * yy + zz * zz;
            dis = Mathf.Pow(dis, 0.5f);
            return dis;
        }

        public static Vector3S operator *(Vector3S vector3, float scale)
        {
            return new Vector3S(vector3.X * scale, vector3.Y * scale, vector3.Z * scale);
        }

        public static Vector3S operator /(Vector3S vector3, float scale)
        {
            return new Vector3S(vector3.X / scale, vector3.Y / scale, vector3.Z / scale);
        }

        public static Vector3S operator +(Vector3S vector1, Vector3S vector2)
        {
            return new Vector3S(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }
        public static Vector3S operator -(Vector3S vector1, Vector3S vector2)
        {
            return new Vector3S(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        public Vector3 GetVector3()
        {
            return new Vector3(X,Y,Z);
        }
    }

    [PublicAPI]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct OrientedBoundingBox
    {
        #region Native

        public static class MathGeoLibNativeMethods
        {
#if UNITY || UNITY_EDITOR
            private const string DllName = "MathGeoLib.Exports";
#else
            private const string DllName = "MathGeoLib.Exports.dll";
#endif

            [DllImport(DllName)]
            public static extern void obb_optimal_enclosing(
                Vector3S[] points, int numPoints, out Vector3S center, out Vector3S extent, [In] [Out] Vector3S[] axis
            );

            [DllImport(DllName)]
            public static extern void obb_brute_enclosing(
                Vector3S[] points, int numPoints, out Vector3S center, out Vector3S extent, [In] [Out] Vector3S[] axis
            );

            [DllImport(DllName)]
            public static extern void obb_enclose(
                [In] [Out] OrientedBoundingBox box,
                Vector3S point
            );

            [DllImport(DllName)]
            public static extern void obb_point_inside(
                [In] [Out] OrientedBoundingBox box,
                float x, float y, float z, out Vector3S point
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_contains_point(
                [In] [Out] OrientedBoundingBox box,
                Vector3S other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_contains_line_segment(
                [In] [Out] OrientedBoundingBox box,
                Line other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_contains_obb(
                [In] [Out] OrientedBoundingBox box,
                OrientedBoundingBox other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_intersects_obb(
                [In] [Out] OrientedBoundingBox box,
                OrientedBoundingBox other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_intersects_line_segment(
                [In] [Out] OrientedBoundingBox box,
                Line other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_intersects_ray(
                [In] [Out] OrientedBoundingBox box,
                Ray other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_intersects_plane(
                [In] [Out] OrientedBoundingBox box,
                Plane other
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_is_finite(
                [In] [Out] OrientedBoundingBox box
            );

            [DllImport(DllName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool obb_is_degenerate(
                [In] [Out] OrientedBoundingBox box
            );

            [DllImport(DllName)]
            public static extern void obb_corner_point(
                [In] [Out] OrientedBoundingBox box,
                int index, out Vector3S point
            );

            [DllImport(DllName)]
            public static extern void obb_face_point(
                [In] [Out] OrientedBoundingBox box,
                int index, float u, float v, out Vector3S point
            );

            [DllImport(DllName)]
            public static extern int obb_num_faces();

            [DllImport(DllName)]
            public static extern int obb_num_edges();

            [DllImport(DllName)]
            public static extern int obb_num_vertices();

            [DllImport(DllName)]
            public static extern void obb_scale(
                [In] [Out] OrientedBoundingBox box,
                Vector3S center, Vector3S factor
            );

            [DllImport(DllName)]
            public static extern void obb_translate(
                [In] [Out] OrientedBoundingBox box,
                Vector3S offset
            );

            [DllImport(DllName)]
            public static extern float obb_distance(
                [In] [Out] OrientedBoundingBox box,
                Vector3S point
            );

            [DllImport(DllName)]
            public static extern void obb_point_on_edge(
                [In] [Out] OrientedBoundingBox box,
                int index, float u, out Vector3S point
            );

            [DllImport(DllName)]
            public static extern void obb_edge(
                [In] [Out] OrientedBoundingBox box,
                int index, out Line segment
            );

            [DllImport(DllName)]
            public static extern void obb_world_to_local(
                [In] [Out] OrientedBoundingBox box,
                out Matrix3X4 local
            );

            [DllImport(DllName)]
            public static extern void obb_local_to_world(
                [In] [Out] OrientedBoundingBox box,
                out Matrix3X4 world
            );

            [DllImport(DllName)]
            public static extern void obb_face_plane(
                [In] [Out] OrientedBoundingBox box,
                int index, out Plane plane
            );

            [DllImport(DllName)]
            public static extern void obb_random_point_on_surface(
                [In] OrientedBoundingBox box,
                LCG rng,
                out Vector3S outVec
            );
        }

        #endregion

        #region Fields

        // NOTE region prevents re-ordering on cleanup

        public Vector3 Center;
        public Vector3 Extent;
        public Vector3 Right;
        public Vector3 Up;
        public Vector3 Forward;

        public bool IsInfinity()
        {
            return this.Extent == Vector3.positiveInfinity || this.Extent == Vector3.negativeInfinity || float.IsInfinity(this.Extent.x);
        }
        
        #endregion

        #region Constructors

        //[PublicAPI]
        //public OrientedBoundingBox()
        //{
        //    // for serialization

        //    Center = Vector3.zero;
        //    Extent =  Vector3.zero;
        //    Right =Vector3.zero;
        //    Up = Vector3.zero;
        //    Forward = Vector3.zero;
        //}

        public static OrientedBoundingBox Create(Vector3[] vs, bool isGetObbEx,string name)
        {
            //IsBreakProgress = false;

            DateTime start = DateTime.Now;
            List<Vector3> ps1 = new List<Vector3>();
            List<Vector3S> ps2 = new List<Vector3S>();
            if (vs == null || vs.Length == 0)
            {
                //MeshFilter meshFilter = this.GetComponent<MeshFilter>();
                //if (meshFilter.sharedMesh == null)
                //{
                //    Debug.LogError("OBBCollider.GetObb meshFilter.sharedMesh == null");
                //    IsObbError = true;
                //    return new OrientedBoundingBox();
                //}
                //vs = meshFilter.sharedMesh.vertices;

                Debug.LogError("OrientedBoundingBox.Create vs == null || vs.Length == 0");
                return new OrientedBoundingBox();
            }

            var count = vs.Length;
            for (int i = 0; i < count; i++)
            {
                Vector3 p = vs[i];
                ps2.Add(new Vector3S(p.x, p.y, p.z));
                ps1.Add(p);
            }
            //Debug.Log("ps:"+ps.Count);
            OrientedBoundingBox OBB = OrientedBoundingBox.BruteEnclosing(ps2.ToArray());
            //Debug.Log($"GetObb ps:{ps2.Count} go:{gameObject.name} time:{(DateTime.Now - start).TotalMilliseconds}ms OBB:{OBB} Center:{OBB.Center} Extent:{OBB.Extent}");
            if (OBB.IsInfinity())
            {
                Debug.LogError($"GetObb Error gameObject:{name} Extent:{OBB.Extent} ps_Last:{ps2.Last()}");
                var errorP = ps1.Last();
                //CreateLocalPoint(errorP, $"ObbErrorPoint({errorP.x},{errorP.y},{errorP.z})");
                //OBB = null;
                if (isGetObbEx)
                {
                    if (GetObbEx(vs,name) == false)
                    {
                        return new OrientedBoundingBox();
                    }
                }
                //IsObbError = true;
            }
            return OBB;
        }

        public static bool GetObbEx(Vector3[] vs,string name)
        {
            DateTime start = DateTime.Now;
            List<Vector3> ps1 = new List<Vector3>();
            List<Vector3S> ps21 = new List<Vector3S>();
            List<Vector3S> ps22 = new List<Vector3S>();

            if (vs == null || vs.Length == 0)
            {
                Debug.LogError("OrientedBoundingBox.GetObbEx vs == null || vs.Length == 0");
                return false;
            }

            var count = vs.Length;

            StringBuilder sb = new StringBuilder();

            int TestObbPointCount = int.MaxValue;

            OrientedBoundingBox OBB;

            for (int i = 0; i < count && i < TestObbPointCount; i++)
            {
                Vector3 p = vs[i];

                if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetObbEx", i, count, p)))
                {
                    Debug.LogError("GetObbEx BreakProgress");
                    //IsBreakProgress = true;
                    ProgressBarHelper.ClearProgressBar();
                    return false;
                }

                ps21.Add(new Vector3S(p.x, p.y, p.z));
                
                if (i > 2)
                {
                    OBB = OrientedBoundingBox.BruteEnclosing(ps21.ToArray());
                    if (float.IsInfinity(OBB.Extent.x))
                    {
                        //Debug.LogWarning($"GetObb Error[{i}/{count},{TestObbPointCount}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{OBB.Extent} ps_Last:{ps21.Last()}");
                        sb.AppendLine($"GetObb go:{name} Error[{i}/{count},{TestObbPointCount}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{OBB.Extent} ps_Last:{ps21.Last()}");
                        ps21 = new List<Vector3S>(ps22);
                    }
                    else
                    {
                        ps22.Add(new Vector3S(p.x, p.y, p.z));
                    }
                    ps1.Add(p);
                }
                else
                {
                    ps22.Add(new Vector3S(p.x, p.y, p.z));
                }

            }
            //Debug.Log("ps:"+ps.Count);
            OBB = OrientedBoundingBox.BruteEnclosing(ps22.ToArray());
            if (sb.Length > 0)
            {
                Debug.LogWarning(sb.ToString());
            }
            Debug.Log($"GetObbEx go:{name} ps:{ps22.Count}/{vs.Length}  time:{(DateTime.Now - start).TotalMilliseconds}ms OBB:{OBB} Center:{OBB.Center} Extent:{OBB.Extent}");
            if (OBB.IsInfinity())
            {
                Debug.LogError($"GetObbEx Error Extent:{OBB.Extent} ps_Last:{ps22.Last()}");
                var errorP = ps1.Last();
                //CreateLocalPoint(errorP, $"ObbErrorPoint({errorP.x},{errorP.y},{errorP.z})");
            }
            ProgressBarHelper.ClearProgressBar();
            return true;
        }

        public OrientedBoundingBox(Vector3 center, Vector3 extent, Vector3 right, Vector3 up, Vector3 forward)
        {
            Center = center;
            Extent = extent;
            Right = right;
            Up = up;
            Forward = forward;
        }

        public OrientedBoundingBox(Vector3S center, Vector3S extent, Vector3S right, Vector3S up, Vector3S forward)
        {
            Center = center.GetVector3();
            Extent = extent.GetVector3();
            Right = right.GetVector3();
            Up = up.GetVector3();
            Forward = forward.GetVector3();
        }

        #endregion

        #region Static

        public static int NumEdges => MathGeoLibNativeMethods.obb_num_edges();

        public static int NumFaces => MathGeoLibNativeMethods.obb_num_faces();

        public static int NumVertices => MathGeoLibNativeMethods.obb_num_vertices();

        public static OrientedBoundingBox OptimalEnclosing(Vector3S[] points)
        {
            var axis = new Vector3S[3];

            MathGeoLibNativeMethods.obb_optimal_enclosing(points, points.Length, out var center, out var extent, axis);

            var box = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);

            return box;
        }

        public static OrientedBoundingBox BruteEnclosing(Vector3S[] points)
        {
            var axis = new Vector3S[3];

            MathGeoLibNativeMethods.obb_brute_enclosing(points, points.Length, out var center, out var extent, axis);

            var box = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);

            return box;
        }

        public static List<Vector3S> GetVerticesS(GameObject go)
        {
            List<Vector3S> ps2 = new List<Vector3S>();
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) return ps2;
            var vs = meshFilter.sharedMesh.vertices;

            //var count = vs.Length;
            //for (int i = 0; i < count; i++)
            //{
            //    Vector3 p = vs[i];
            //    ps2.Add(new Vector3S(p.x, p.y, p.z));
            //}
            //return ps2;
            return GetVerticesS(vs);
        }

        public static Vector3[] GetVertices(GameObject go)
        {
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) return null;
            var vs = meshFilter.sharedMesh.vertices;

            //var count = vs.Length;
            //for (int i = 0; i < count; i++)
            //{
            //    Vector3 p = vs[i];
            //    ps2.Add(new Vector3S(p.x, p.y, p.z));
            //}
            //return ps2;
            return vs;
        }

        public static List<Vector3S> GetVerticesS(Vector3[] vs)
        {
            List<Vector3S> ps2 = new List<Vector3S>();
            var count = vs.Length;
            for (int i = 0; i < count; i++)
            {
                Vector3 p = vs[i];
                ps2.Add(new Vector3S(p.x, p.y, p.z));
            }
            return ps2;
        }

        private static OrientedBoundingBox GetObbEx(Vector3[] vs, object name)
        {
            DateTime start = DateTime.Now;
            List<Vector3> ps1 = new List<Vector3>();
            List<Vector3S> ps21 = new List<Vector3S>();
            List<Vector3S> ps22 = new List<Vector3S>();

            //var vs = ps.ToArray();
            var count = vs.Length;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                Vector3 p = vs[i];

                //if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetObbEx", i, count, p)))
                //{
                //    ProgressBarHelper.ClearProgressBar();
                //    return false;
                //}

                ps21.Add(new Vector3S(p.x, p.y, p.z));

                if (i > 2)
                {
                    var obb = OrientedBoundingBox.BruteEnclosing(ps21.ToArray());
                    if (float.IsInfinity(obb.Extent.x))
                    {
                        //Debug.LogWarning($"GetObb Error[{i}/{count},{TestObbPointCount}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{OBB.Extent} ps_Last:{ps21.Last()}");
                        sb.AppendLine($"GetObb go:{name} Error[{i}/{count}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{obb.Extent} ps_Last:{ps21.Last()}");
                        ps21 = new List<Vector3S>(ps22);
                    }
                    else
                    {
                        ps22.Add(new Vector3S(p.x, p.y, p.z));
                    }
                    ps1.Add(p);
                }
                else
                {
                    ps22.Add(new Vector3S(p.x, p.y, p.z));
                }

            }
            //Debug.Log("ps:"+ps.Count);
            var OBB = OrientedBoundingBox.BruteEnclosing(ps22.ToArray());
            if (sb.Length > 0)
            {
                Debug.LogWarning(sb.ToString());
            }
            Debug.Log($"GetObbEx go:{name} ps:{ps22.Count}  time:{(DateTime.Now - start).TotalMilliseconds}ms OBB:{OBB} Center:{OBB.Center} Extent:{OBB.Extent}");
            if (OBB.IsInfinity())
            {
                Debug.LogError($"GetObbEx Error Extent:{OBB.Extent} ps_Last:{ps22.Last()}");
                //var errorP = ps1.Last();
                //CreateLocalPoint(errorP, $"ErrorPoint({errorP.x},{errorP.y},{errorP.z})");
            }
            //ProgressBarHelper.ClearProgressBar();
            return OBB;
        }

        public static OrientedBoundingBox GetObb(Vector3[] vs, object name,bool isGetObbEx)
        {
            List<Vector3S> ps2 = OrientedBoundingBox.GetVerticesS(vs);
            //var axis = new Vector3S[3];
            //MathGeoLibNativeMethods.obb_optimal_enclosing(ps2.ToArray(), ps2.Count, out var center, out var extent, axis);
            //OrientedBoundingBox OBB = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);
            var OBB = OrientedBoundingBox.BruteEnclosing(ps2.ToArray());
            if (OBB.IsInfinity())
            {
                Debug.LogError($"GetModelInfo GetObb Error gameObject:{name} Extent:{OBB.Extent} ps_Last:{ps2.Last()}");
                //var errorP = ps1.Last();
                //CreateLocalPoint(errorP, $"ErrorPoint({errorP.x},{errorP.y},{errorP.z})");
                //OBB = null;
                if (isGetObbEx)
                {
                    OBB = GetObbEx(vs, name);
                }
                
                //lineData.IsObbError = true;
            }
            return OBB;
        }

        #endregion

        #region Instance

        public bool IsDegenerate => MathGeoLibNativeMethods.obb_is_degenerate(this);

        public bool IsFinite => MathGeoLibNativeMethods.obb_is_finite(this);

        public bool Contains(Vector3S other)
        {
            return MathGeoLibNativeMethods.obb_contains_point(this, other);
        }

        public bool Contains(Line other)
        {
            return MathGeoLibNativeMethods.obb_contains_line_segment(this, other);
        }

        public bool Contains(OrientedBoundingBox other)
        {
            return MathGeoLibNativeMethods.obb_contains_obb(this, other);
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            return MathGeoLibNativeMethods.obb_intersects_obb(this, other);
        }

        public bool Intersects(Ray other)
        {
            return MathGeoLibNativeMethods.obb_intersects_ray(this, other);
        }

        public bool Intersects(Plane other)
        {
            return MathGeoLibNativeMethods.obb_intersects_plane(this, other);
        }

        public bool Intersects(Line other)
        {
            return MathGeoLibNativeMethods.obb_intersects_line_segment(this, other);
        }

        public Vector3S CornerPoint(int index)
        {
            MathGeoLibNativeMethods.obb_corner_point(this, index, out var point);
            return point;
        }

        public Vector3S[] CornerBoxPoints()
        {
            var p0 = this.CornerPoint(0);
            var p1 = this.CornerPoint(1);
            var p2 = this.CornerPoint(2);
            var p3 = this.CornerPoint(3);
            var p4 = this.CornerPoint(4);
            var p5 = this.CornerPoint(5);
            var p6 = this.CornerPoint(6);
            var p7 = this.CornerPoint(7);

            var points = new[]
            {
                p0, p2, p6, p4, p0,
                p1, p3, p7, p5, p1
            };
            return points;
        }

        //public Vector3S[] BoxPlaneCenterPoints()
        //{
        //    var p0 = this.CornerPoint(0);
        //    var p1 = this.CornerPoint(1);
        //    var p2 = this.CornerPoint(2);
        //    var p3 = this.CornerPoint(3);
        //    var p4 = this.CornerPoint(4);
        //    var p5 = this.CornerPoint(5);
        //    var p6 = this.CornerPoint(6);
        //    var p7 = this.CornerPoint(7);

        //    var pc1 = (p0 + p1 + p2 + p3) / 4;
        //    var pc2 = (p4 + p5 + p6 + p7) / 4;
        //    var pc3 = (p0 + p1 + p4 + p5) / 4;
        //    var pc4 = (p2 + p3 + p6 + p7) / 4;
        //    var pc5 = (p0 + p2 + p4 + p6) / 4;
        //    var pc6 = (p1 + p3 + p5 + p7) / 4;

        //    var points = new[]
        //    {
        //        pc1,pc2,pc3,pc4,pc5,pc6
        //    };
        //    return points;
        //}

        public PlaneInfo[] GetPlaneInfos()
        {
            var p0 = this.CornerPoint(0);
            var p1 = this.CornerPoint(1);
            var p2 = this.CornerPoint(2);
            var p3 = this.CornerPoint(3);
            var p4 = this.CornerPoint(4);
            var p5 = this.CornerPoint(5);
            var p6 = this.CornerPoint(6);
            var p7 = this.CornerPoint(7);

            //var pc1 = (p0 + p1 + p2 + p3) / 4;
            //var pc2 = (p4 + p5 + p6 + p7) / 4;
            //var pc3 = (p0 + p1 + p4 + p5) / 4;
            //var pc4 = (p2 + p3 + p6 + p7) / 4;
            //var pc5 = (p0 + p4 + p3 + p7) / 4;
            //var pc6 = (p1 + p5 + p2 + p6) / 4;

            var pc1 = (p0 + p1 + p2 + p3) / 4;
            var pc2 = (p4 + p5 + p6 + p7) / 4;
            var pc3 = (p0 + p1 + p4 + p5) / 4;
            var pc4 = (p2 + p3 + p6 + p7) / 4;
            var pc5 = (p0 + p2 + p4 + p6) / 4;
            var pc6 = (p1 + p3 + p5 + p7) / 4;

            var points = new PlaneInfo[]
            {
                //pc1,pc2,pc3,pc4,pc5,pc6
                new PlaneInfo(p0,p1,p3,p2),
                //new PlaneInfo(p4,p5,p7,p6),
                new PlaneInfo(p6,p7,p5,p4),

                //new PlaneInfo(p0,p1,p5,p4),
                new PlaneInfo(p4,p5,p1,p0),
                new PlaneInfo(p2,p3,p7,p6),

                new PlaneInfo(p0,p2,p6,p4),
                //new PlaneInfo(p1,p3,p7,p5)
                new PlaneInfo(p5,p7,p3,p1)
            };
            return points;

            //List<PlaneInfo> planes = new List<PlaneInfo>();
            //for(int i = 0; i < 6; i++)
            //{
            //    for(int v = 0; v < 2; v++)
            //    {
            //        for(int u = 0; u < 2; u++)
            //        {
            //            var p1=FacePoint(i, u, v);
            //        }
            //    }
            //}
        }


        public Vector3S[] CornerPoints()
        {
            var p0 = this.CornerPoint(0);
            var p1 = this.CornerPoint(1);
            var p2 = this.CornerPoint(2);
            var p3 = this.CornerPoint(3);
            var p4 = this.CornerPoint(4);
            var p5 = this.CornerPoint(5);
            var p6 = this.CornerPoint(6);
            var p7 = this.CornerPoint(7);

            //var points = new[]
            //{
            //    p0, p1, p2, p3,
            //    p4, p5, p6, p7
            //};
            var points = new[]
           {
                p0, p1, p3, p2,
                p4, p5, p7, p6
            };
            return points;
        }

        public Vector3[] CornerPointsVector3()
        {
            var p0 = this.CornerPoint(0);
            var p1 = this.CornerPoint(1);
            var p2 = this.CornerPoint(2);
            var p3 = this.CornerPoint(3);
            var p4 = this.CornerPoint(4);
            var p5 = this.CornerPoint(5);
            var p6 = this.CornerPoint(6);
            var p7 = this.CornerPoint(7);

            //var points = new[]
            //{
            //    p0.GetVector3(), p1.GetVector3(), p2.GetVector3(), p3.GetVector3(),
            //    p4.GetVector3(), p5.GetVector3(), p6.GetVector3(), p7.GetVector3()
            //};
            var points = new[]
            {
                p0.GetVector3(), p1.GetVector3(), p3.GetVector3(), p2.GetVector3(),
                p4.GetVector3(), p5.GetVector3(), p7.GetVector3(), p6.GetVector3()
            };
            return points;
        }

        public void Enclose(Vector3 point)
        {
            MathGeoLibNativeMethods.obb_enclose(this, new Vector3S(point));
        }

        public void Enclose(Vector3S point)
        {
            MathGeoLibNativeMethods.obb_enclose(this, point);
        }

        public Vector3S FacePoint(int index, float u, float v)
        {
            MathGeoLibNativeMethods.obb_face_point(this, index, u, v, out var point);
            return point;
        }

        public Vector3S PointInside(float x, float y, float z)
        {
            MathGeoLibNativeMethods.obb_point_inside(this, x, y, z, out var point);
            return point;
        }

        public void Scale(Vector3S center, Vector3S factor)
        {
            MathGeoLibNativeMethods.obb_scale(this, center, factor);
        }

        public void Translate(Vector3S offset)
        {
            MathGeoLibNativeMethods.obb_translate(this, offset);
        }

        public float Distance(Vector3S point)
        {
            return MathGeoLibNativeMethods.obb_distance(this, point);
        }

        public Vector3S PointOnEdge(int index, float u)
        {
            MathGeoLibNativeMethods.obb_point_on_edge(this, index, u, out var point);
            return point;
        }


        public Line Edge(int index)
        {
            MathGeoLibNativeMethods.obb_edge(this, index, out var segment);
            return segment;
        }

        public Matrix3X4 WorldToLocal()
        {
            MathGeoLibNativeMethods.obb_world_to_local(this, out var local);
            return local;
        }

        public Matrix3X4 LocalToWorld()
        {
            MathGeoLibNativeMethods.obb_local_to_world(this, out var world);
            return world;
        }

        public Plane FacePlane(int index)
        {
            MathGeoLibNativeMethods.obb_face_plane(this, index, out var plane);
            return plane;
        }

        public override string ToString()
        {
            return $"{nameof(Center)}: {Center}, {nameof(Extent)}: {Extent} Forward:{Forward}";
        }

        public Vector3S RandomPointOnSurface(LCG rng)
        {
            MathGeoLibNativeMethods.obb_random_point_on_surface(this, rng, out var point);
            return point;
        }

        #endregion
    }
}

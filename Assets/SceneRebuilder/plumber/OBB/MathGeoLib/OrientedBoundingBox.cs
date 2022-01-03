using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
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
    public sealed class OrientedBoundingBox
    {
        #region Native

        private static class NativeMethods
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
        
        #endregion

        #region Constructors

        [PublicAPI]
        public OrientedBoundingBox()
        {
            // for serialization
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

        public static int NumEdges => NativeMethods.obb_num_edges();

        public static int NumFaces => NativeMethods.obb_num_faces();

        public static int NumVertices => NativeMethods.obb_num_vertices();

        public static OrientedBoundingBox OptimalEnclosing(Vector3S[] points)
        {
            var axis = new Vector3S[3];

            NativeMethods.obb_optimal_enclosing(points, points.Length, out var center, out var extent, axis);

            var box = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);

            return box;
        }

        public static OrientedBoundingBox BruteEnclosing(Vector3S[] points)
        {
            var axis = new Vector3S[3];

            NativeMethods.obb_brute_enclosing(points, points.Length, out var center, out var extent, axis);

            var box = new OrientedBoundingBox(center, extent, axis[0], axis[1], axis[2]);

            return box;
        }

        #endregion

        #region Instance
        
        public bool IsDegenerate => NativeMethods.obb_is_degenerate(this);

        public bool IsFinite => NativeMethods.obb_is_finite(this);

        public bool Contains(Vector3S other)
        {
            return NativeMethods.obb_contains_point(this, other);
        }

        public bool Contains(Line other)
        {
            return NativeMethods.obb_contains_line_segment(this, other);
        }

        public bool Contains(OrientedBoundingBox other)
        {
            return NativeMethods.obb_contains_obb(this, other);
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            return NativeMethods.obb_intersects_obb(this, other);
        }

        public bool Intersects(Ray other)
        {
            return NativeMethods.obb_intersects_ray(this, other);
        }

        public bool Intersects(Plane other)
        {
            return NativeMethods.obb_intersects_plane(this, other);
        }

        public bool Intersects(Line other)
        {
            return NativeMethods.obb_intersects_line_segment(this, other);
        }

        public Vector3S CornerPoint(int index)
        {
            NativeMethods.obb_corner_point(this, index, out var point);
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

        public void Enclose(Vector3S point)
        {
            NativeMethods.obb_enclose(this, point);
        }

        public Vector3S FacePoint(int index, float u, float v)
        {
            NativeMethods.obb_face_point(this, index, u, v, out var point);
            return point;
        }

        public Vector3S PointInside(float x, float y, float z)
        {
            NativeMethods.obb_point_inside(this, x, y, z, out var point);
            return point;
        }

        public void Scale(Vector3S center, Vector3S factor)
        {
            NativeMethods.obb_scale(this, center, factor);
        }

        public void Translate(Vector3S offset)
        {
            NativeMethods.obb_translate(this, offset);
        }

        public float Distance(Vector3S point)
        {
            return NativeMethods.obb_distance(this, point);
        }

        public Vector3S PointOnEdge(int index, float u)
        {
            NativeMethods.obb_point_on_edge(this, index, u, out var point);
            return point;
        }


        public Line Edge(int index)
        {
            NativeMethods.obb_edge(this, index, out var segment);
            return segment;
        }

        public Matrix3X4 WorldToLocal()
        {
            NativeMethods.obb_world_to_local(this, out var local);
            return local;
        }

        public Matrix3X4 LocalToWorld()
        {
            NativeMethods.obb_local_to_world(this, out var world);
            return world;
        }

        public Plane FacePlane(int index)
        {
            NativeMethods.obb_face_plane(this, index, out var plane);
            return plane;
        }

        public override string ToString()
        {
            return $"{nameof(Center)}: {Center}, {nameof(Extent)}: {Extent}";
        }

        public Vector3S RandomPointOnSurface(LCG rng)
        {
            NativeMethods.obb_random_point_on_surface(this, rng, out var point);
            return point;
        }

        #endregion
    }
}

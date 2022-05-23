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
            return new Vector3(X, Y, Z);
        }
    }
}

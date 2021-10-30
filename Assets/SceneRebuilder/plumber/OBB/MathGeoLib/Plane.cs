﻿#if UNITY || UNITY_EDITOR
// using Unity type
#else

using System.Runtime.InteropServices;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace MathGeoLib
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    public struct Plane
    {
        public readonly Vector3S Normal;

        public readonly float Distance;

        public Plane(Vector3S normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"{nameof(Normal)}: {Normal}, {nameof(Distance)}: {Distance}";
        }
    }
}

#endif // !UNITY
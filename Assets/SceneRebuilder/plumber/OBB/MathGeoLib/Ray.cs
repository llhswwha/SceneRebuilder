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
    public struct Ray
    {
        public readonly Vector3S Origin;

        public readonly Vector3S Direction;

        public Ray(Vector3S point1, Vector3S point2)
        {
            Origin = point1;
            Direction = point2;
        }

        public override string ToString()
        {
            return $"{nameof(Origin)}: {Origin}, {nameof(Direction)}: {Direction}";
        }
    }
}

#endif
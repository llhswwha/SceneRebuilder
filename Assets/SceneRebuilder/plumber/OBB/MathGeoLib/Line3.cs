using System.Runtime.InteropServices;
using JetBrains.Annotations;

// #if UNITY || UNITY_EDITOR
// using Vector3S = UnityEngine.Vector3S;
// #endif

// ReSharper disable once CheckNamespace
namespace MathGeoLib
{
    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    public struct Line3
    {
        public readonly Vector3S Point1;

        public readonly Vector3S Point2;

        public Line3(Vector3S point1, Vector3S point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public override string ToString()
        {
            return $"{nameof(Point1)}: {Point1}, {nameof(Point2)}: {Point2}";
        }
    }
}
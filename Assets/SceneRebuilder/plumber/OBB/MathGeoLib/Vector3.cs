// #if UNITY || UNITY_EDITOR
// // using Unity type
// #else

// using System.Runtime.InteropServices;
// using JetBrains.Annotations;

// // ReSharper disable once CheckNamespace
// namespace MathGeoLib
// {
//     [PublicAPI]
//     [StructLayout(LayoutKind.Sequential)]
//     public struct Vector3S
//     {
//         public readonly float X;

//         public readonly float Y;

//         public readonly float Z;

//         public static Vector3S Right { get; } = new Vector3S(1, 0, 0);

//         public static Vector3S Up { get; } = new Vector3S(0, 1, 0);

//         public static Vector3S Forward { get; } = new Vector3S(0, 0, 1);

//         public static Vector3S Zero { get; } = new Vector3S(0, 0, 0);

//         public static Vector3S One { get; } = new Vector3S(1, 1, 1);

//         public Vector3S(float x, float y, float z)
//         {
//             X = x;
//             Y = y;
//             Z = z;
//         }

//         public override string ToString()
//         {
//             return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
//         }

//         public static Vector3S operator *(Vector3S vector3, float scale)
//         {
//             return new Vector3S(vector3.X * scale, vector3.Y * scale, vector3.Z * scale);
//         }
//     }
// }

// #endif // !UNITY
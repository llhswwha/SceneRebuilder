using UnityEngine;


namespace MeshJobs
{
    public static class Vector3Extensions
    {
        public static bool Approximately(this Vector3 l, Vector3 r, float epsilon=1e-5f)
        {
            if (Mathf.Abs(l.x - r.x) > epsilon) return false;
            if (Mathf.Abs(l.y - r.y) > epsilon) return false;
            if (Mathf.Abs(l.z - r.z) > epsilon) return false;
            return true;
        }
    }
}

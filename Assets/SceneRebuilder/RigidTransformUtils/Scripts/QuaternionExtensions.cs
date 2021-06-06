using UnityEngine;


namespace MeshJobs
{
    public static class QuaternionExtensions
    {
        public static bool Approximately(this Quaternion l, Quaternion r, float epsilon=1e-5f)
        {
            if (Mathf.Abs(l.x - r.x) > epsilon) return false;
            if (Mathf.Abs(l.y - r.y) > epsilon) return false;
            if (Mathf.Abs(l.z - r.z) > epsilon) return false;
            if (Mathf.Abs(l.w - r.w) > epsilon) return false;
            return true;
        }
    }
}

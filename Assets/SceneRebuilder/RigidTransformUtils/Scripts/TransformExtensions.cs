using UnityEngine;


namespace MeshJobs
{
    public static class TransformExtensions
    {
        public static RTTransform LocalRigidTransform(this Transform transform)
        {
            return new RTTransform(transform.localRotation, transform.localPosition);

        }
        public static RTTransform WorldRigidTransform(this Transform transform)
        {
            return new RTTransform(transform.rotation, transform.position);
        }
    }
}


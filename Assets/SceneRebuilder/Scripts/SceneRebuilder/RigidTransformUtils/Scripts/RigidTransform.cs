using System;
using System.Runtime.Serialization;
using UnityEngine;


namespace MeshJobs
{
    [Serializable, DataContract]
    public struct RTTransform //原RigidTransform
    {
        public readonly Quaternion Rotation;

        public readonly Vector3 Translation;

        public RTTransform(Quaternion r) : this(r, Vector3.zero)
        { }

        public RTTransform(Vector3 t) : this(Quaternion.identity, t)
        { }

        public RTTransform(Quaternion r, Vector3 t)
        {
            Rotation = r;
            Translation = t;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}]", Rotation, Translation);
        }

        public static RTTransform Identity {
            get
            {
                return  new RTTransform(Quaternion.identity, Vector3.zero);
            }
        }

        public static RTTransform operator *(RTTransform c1, RTTransform c2)
        {
            return new RTTransform(
                c1.Rotation * c2.Rotation
                , c1.Rotation * c2.Translation + c1.Translation);
        }

        public RTTransform Inverse()
        {
            var inverse = Quaternion.Inverse(Rotation);
            return new RTTransform(inverse, inverse * (-Translation));
        }

        public RTTransform LocalFrom(RTTransform rt)
        {
            return rt.Inverse() * this;
        }

        public RTTransform LocalTo(RTTransform rt)
        {
            return this * rt.Inverse() ;
        }

        public static RTTransform FormTo(RTTransform tFrom,RTTransform tTo)
        {
            var rtTransform=tTo.LocalTo(tFrom);//可以
            return rtTransform;
        }

        public Vector3 Apply(Vector3 v)
        {
            return (Rotation * v) + Translation;
        }

        public void ApplyWorld(Transform target)
        {
            target.position = Translation;
            target.rotation = Rotation;
        }

        public void ApplyLocal(Transform target)
        {
            target.localPosition = Translation;
            target.localRotation = Rotation;
        }

        public bool Approximately(RTTransform r)
        {
            if (!Rotation.Approximately(r.Rotation)) return false;
            if (!Translation.Approximately(r.Translation)) return false;
            return true;
        }
    }
}

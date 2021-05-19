using System;
using System.Xml.Serialization;
using UnityEngine;

namespace AdvancedCullingSystem.StaticCullingCore
{
    /// <summary>
    ///     三维向量
    /// </summary>
    [Serializable]
    public class V3
    {
        [XmlAttribute]
        public float X { get; set; }
        [XmlAttribute]
        public float Y { get; set; }
        [XmlAttribute]
        public float Z { get; set; }

        public V3()
        {

        }

        public V3(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
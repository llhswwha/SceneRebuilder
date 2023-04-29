using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Battlehub.RTTerrain.HDRP
{
    public class TerrainProjectorHDRP : TerrainProjectorBase
    {
        private DecalProjector m_decal;

        public override Texture2D Brush
        {
            get { return (Texture2D)m_decal.material.GetTexture("_BaseColorMap"); }
            set
            {
                m_decal.material.SetTexture("_BaseColorMap", value);
                TerrainBrush.Texture = value;
            }
        }

        public override float Size
        {
            get { return transform.localScale.x; }
            set
            {
                Vector3 size = m_decal.size;
                size.x = value;
                size.y = value;
                m_decal.size = size;
                TerrainBrush.Radius = value * 0.5f;
            }
        }


        protected override void Awake()
        {
            m_decal = GetComponent<DecalProjector>();
            base.Awake();
        }

        public override void Enable(bool value)
        {
            if(m_decal.enabled != value)
            {
                m_decal.enabled = value;
            }
        }
    }
}

using Battlehub.RTCommon;
using Battlehub.RTEditor;
using UnityEngine;

namespace Battlehub.RTTerrain.HDRP
{
    public class TerrainInitHDRP : EditorExtension
    {
        [SerializeField]
        private TerrainProjectorBase m_projector = null;

        private TerrainProjectorBase InstantiateTerrainProjector()
        {
            return Instantiate(m_projector);
        }

        protected override void Awake()
        {
            base.Awake();
            IOC.Register(InstantiateTerrainProjector);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            IOC.Unregister(InstantiateTerrainProjector);
        }
    }
}


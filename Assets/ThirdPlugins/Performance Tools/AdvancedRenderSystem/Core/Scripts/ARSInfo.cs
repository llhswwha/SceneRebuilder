using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem
{
    public static class ARSInfo
    {
        public const string LayerName = "ARS_Billboard";
        public static int Layer
        {
            get
            {
                int layer = LayerMask.NameToLayer(LayerName);

                if (layer < 0)
                    return LayersHelper.CreateLayer(LayerName);

                return layer;
            }
        }
    }
}

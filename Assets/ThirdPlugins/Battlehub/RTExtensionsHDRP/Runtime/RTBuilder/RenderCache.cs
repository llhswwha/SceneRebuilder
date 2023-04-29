using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using Battlehub.RTCommon;
using System.Collections.Generic;

namespace Battlehub.RTBuilder.HDRP
{
    public class RenderCache : CustomPass
    {
        public IRenderersCache Cache
        {
            get;
            set;
        }
        
        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
        {
            if (Cache != null)
            {
                IList<Renderer> renderers = Cache.Renderers;
                for (int i = 0; i < renderers.Count; ++i)
                {
                    Renderer renderer = renderers[i];
                    Material[] materials = renderer.sharedMaterials;
                    for (int j = 0; j < materials.Length; ++j)
                    {
                        if (Cache.MaterialOverride != null)
                        {
                            cmd.DrawRenderer(renderer, Cache.MaterialOverride, j, -1);
                        }
                        else
                        {
                            Material material = materials[j];
                            cmd.DrawRenderer(renderer, material, j, -1);
                        }
                    }
                }
            }
        }
    }
}

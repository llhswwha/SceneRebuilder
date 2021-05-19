using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem
{
    public class BillboardOfGroup : BaseBillboard
    {
        [SerializeField]
        private MeshRenderer[] _sources;
        public MeshRenderer[] Sources
        {
            get
            {
                return _sources;
            }
        }


        public static BillboardOfGroup Create(MeshRenderer[] sources, Material material, int layer)
        {
            BillboardOfGroup billboard = GameObject.CreatePrimitive(PrimitiveType.Quad)
               .AddComponent<BillboardOfGroup>();

            MeshRenderer renderer = billboard.GetComponent<MeshRenderer>();

            Destroy(billboard.GetComponent<Collider>());

            renderer.material = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;


            billboard._sources = sources;

            billboard.Initialize(layer);

            return billboard;
        }


        public override void ToMesh()
        {
            if (!IsReplaced)
                return;

            for (int i = 0; i < _sources.Length; i++)
                _sources[i].enabled = true;

            _billboardRenderer.enabled = false;

            IsReplaced = false;
        }

        public override void ToBillboard()
        {
            if (IsReplaced)
                return;

            for (int i = 0; i < _sources.Length; i++)
                _sources[i].enabled = false;

            _billboardRenderer.enabled = true;

            IsReplaced = true;

        }

        public override void UpdateTexture(Camera renderCamera, float distance, float cameraFov, TexturesManager texturesManager)
        {
            int textureSize = texturesManager.GetTextureSize(_billboardSize, distance, cameraFov);

            if (Texture == null)
            {
                Texture = texturesManager.GetTexture(textureSize);
            }
            else if (textureSize != Texture.width)
            {
                texturesManager.FreeTexture(Texture);
                Texture = texturesManager.GetTexture(textureSize);
            }

            int cachedLayer = _sources[0].gameObject.layer;

            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i].enabled = true;
                _sources[i].gameObject.layer = _layer;
            }

            CentralizeCamera(renderCamera, distance);

            renderCamera.targetTexture = Texture;
            renderCamera.Render();

            for (int i = 0; i < _sources.Length; i++)
            {
                _sources[i].enabled = false;
                _sources[i].gameObject.layer = cachedLayer;
            }

            _billboardRenderer.material.mainTexture = Texture;
        }


        protected override Bounds CalculateSourcesBounds()
        {
            Vector3 min = Vector3.one * Mathf.Infinity;
            Vector3 max = Vector3.one * Mathf.NegativeInfinity;

            foreach (var renderer in _sources)
            {
                Bounds bounds = renderer.bounds;

                min.x = Mathf.Min(min.x, bounds.min.x);
                min.y = Mathf.Min(min.y, bounds.min.y);
                min.z = Mathf.Min(min.z, bounds.min.z);

                max.x = Mathf.Max(max.x, bounds.max.x);
                max.y = Mathf.Max(max.y, bounds.max.y);
                max.z = Mathf.Max(max.z, bounds.max.z);
            }

            return new Bounds((min + max) / 2, max - min);
        }
    }
}

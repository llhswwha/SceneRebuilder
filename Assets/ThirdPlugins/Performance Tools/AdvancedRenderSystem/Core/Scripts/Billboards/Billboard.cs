using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem
{
    public class Billboard : BaseBillboard
    {
        [SerializeField]
        private MeshRenderer _source;
        public MeshRenderer source
        {
            get { return _source; }
        }


        public static Billboard Create(MeshRenderer source, Material material, int layer)
        {
            Billboard billboard = GameObject.CreatePrimitive(PrimitiveType.Quad)
                .AddComponent<Billboard>();

            MeshRenderer renderer = billboard.GetComponent<MeshRenderer>();

            Destroy(billboard.GetComponent<Collider>());

            renderer.material = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            billboard._source = source;

            billboard.Initialize(layer);

            return billboard;
        }


        public override void ToMesh()
        {
            IsReplaced = false;

            _billboardRenderer.enabled = false;

            _source.enabled = true;
        }

        public override void ToBillboard()
        {
            IsReplaced = true;

            _billboardRenderer.enabled = true;

            _source.enabled = false;
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

            bool enabled = _source.enabled;
            int cachedLayer = _source.gameObject.layer;

            _source.enabled = true;//显示物体
            _source.gameObject.layer = _layer;

            CentralizeCamera(renderCamera, distance);

            renderCamera.targetTexture = Texture;
            renderCamera.Render();//核心：将摄像头的镜头显示在Texture中

            _source.gameObject.layer = cachedLayer;
            _source.enabled = enabled;//隐藏物体

            _billboardRenderer.material.mainTexture = Texture;//将Texture显示在广告牌上
        }

        protected override Bounds CalculateSourcesBounds()
        {
            return _source.bounds;
        }
    }
}

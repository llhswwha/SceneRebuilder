using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem
{
    public enum TextureResolution { _128 = 128, _256 = 256, _512 = 512, _1024 = 1024 }

    public class TexturesManager 
    {
        private int _maxTextureSize;
        private Dictionary<int, Stack<RenderTexture>> _textures;


        public TexturesManager(TextureResolution maxTextureResolution)
        {
            _textures = new Dictionary<int, Stack<RenderTexture>>();

            _maxTextureSize = (int) maxTextureResolution;

            for (int i = 1; i <= _maxTextureSize; i *= 2)
                _textures.Add(i, new Stack<RenderTexture>());
        }

        public void Dispose()
        {
            foreach (var texStack in _textures.Values)
                foreach (var texture in texStack)
                    Object.Destroy(texture);
        }


        public RenderTexture GetTexture(int size)
        {
            if (_textures[size].Count == 0)
            {
                RenderTexture renderTexture = new RenderTexture(size, size, 16);

                renderTexture.Create();
                renderTexture.MarkRestoreExpected();
                renderTexture.DiscardContents();

                return renderTexture;
            }

            return _textures[size].Pop();
        }

        public void FreeTexture(RenderTexture texture)
        {
            _textures[texture.width].Push(texture);
        }


        public int GetTextureSize(float billboardSize, float billboardDistance, float cameraFov)
        {
            return Mathf.Max(Mathf.Min(ClosestPowerOfTwo((((billboardSize / billboardDistance) * Mathf.Rad2Deg) * Screen.height) / cameraFov), _maxTextureSize), 1);
        }

        private int ClosestPowerOfTwo(float x)
        {
            int y = 1;

            while (y < x)
                y = y * 2;

            return y;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGS.AdvancedRenderSystem
{
    public abstract class BaseBillboard : MonoBehaviour
    {
        public virtual bool IsReplaced { get; protected set; }
        public RenderTexture Texture { get; protected set; }

        [SerializeField]
        protected MeshRenderer _billboardRenderer;
        public MeshRenderer BillboardRenderer
        {
            get {  return _billboardRenderer; }
        }

        [SerializeField]
        protected float _billboardSize;
        public float BillboardSize
        {
            get { return _billboardSize; }
        }

        [SerializeField]
        protected Bounds _sourcesBounds;
        public Bounds SourcesBounds
        {
            get { return _sourcesBounds; }
        }

        [SerializeField]
        protected int _layer;
        public int Layer
        {
            get { return _layer; }
        }

        public abstract void ToMesh();

        public abstract void ToBillboard();

        public virtual void RotateTo(Vector3 position)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - position);
        }

        public abstract void UpdateTexture(Camera renderCamera, float distance, float cameraFov, TexturesManager texturesManager);

        protected virtual void Initialize(int layer)
        {
            _sourcesBounds = CalculateSourcesBounds();
            _layer = layer;
            _billboardRenderer = GetComponent<MeshRenderer>();
            _billboardRenderer.transform.position = SourcesBounds.center;
            _billboardRenderer.transform.localScale = Vector3.one * SourcesBounds.extents.magnitude * 2f;
            _billboardSize = _billboardRenderer.bounds.extents.magnitude * 2.0f;
        }

        protected void CentralizeCamera(Camera camera, float distance)
        {
            float size = _sourcesBounds.extents.magnitude * 2f;
            Vector3 cameraDirection = camera.transform.position - _sourcesBounds.center;
            camera.transform.rotation = Quaternion.LookRotation(-cameraDirection);
            camera.fieldOfView = 2.0f * Mathf.Atan(size / (2.0f * distance)) * (180 / Mathf.PI);
            camera.nearClipPlane = Mathf.Max(cameraDirection.magnitude - size, 0.01f);
            camera.farClipPlane = cameraDirection.magnitude + size;
        }

        protected abstract Bounds CalculateSourcesBounds();
    }
}

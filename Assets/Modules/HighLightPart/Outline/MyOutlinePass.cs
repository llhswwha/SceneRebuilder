using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class MyOutlinePass : CustomPass
{
    public LayerMask outlineLayer=0;

    [ColorUsage(false,true)]
    public Color outlineColor=Color.black;

    public float threshold=1;

    [SerializeField,HideInInspector]
    Shader outlineShader;

    Material fullscreenOutline;

    MaterialPropertyBlock outlineProperties;

    ShaderTagId[] shaderTags;

    RTHandle outlineBuffer;
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        name="Outline";
        
        //Debug.Log("Outline.Setup");
        outlineShader=Shader.Find("Hidden/Outline");
        if(outlineShader==null){
            Debug.LogError("Outline.Setup outlineShader==null");
            return;
        }
        fullscreenOutline=CoreUtils.CreateEngineMaterial(outlineShader);
        outlineProperties=new MaterialPropertyBlock();
        shaderTags=new ShaderTagId[]
        {
            new ShaderTagId("Forward"),
            new ShaderTagId("ForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
        };
        outlineBuffer=RTHandles.Alloc(
            Vector2.one,TextureXR.slices,dimension:TextureXR.dimension,
            colorFormat:GraphicsFormat.B10G11R11_UFloatPack32,
            useDynamicScale:true,name:"Outline Buffer"
        );
    }

    void DrawOutlineMeshes(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
    {
        var result=new RendererListDesc(shaderTags,cullingResult,hdCamera.camera)
        {
            rendererConfiguration=PerObjectData.LightProbe | PerObjectData.LightProbeProxyVolume | PerObjectData.Lightmaps,
            renderQueueRange=RenderQueueRange.all,
            sortingCriteria=SortingCriteria.BackToFront,
            excludeObjectMotionVectors=false,
            layerMask=outlineLayer,
        };
        CoreUtils.SetRenderTarget(cmd,outlineBuffer,ClearFlag.Color);
        HDUtils.DrawRendererList(renderContext,cmd,RendererList.Create(result));
    }

    protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
    {
        if(fullscreenOutline!=null){
            DrawOutlineMeshes(renderContext,cmd,hdCamera,cullingResult);
            SetCameraRenderTarget(cmd);
            outlineProperties.SetColor("_OutlineColor",outlineColor);
            outlineProperties.SetTexture("_OutlineBuffer",outlineBuffer);
            outlineProperties.SetFloat("_Threshold",threshold);
            CoreUtils.DrawFullScreen(cmd,fullscreenOutline,outlineProperties,shaderPassId:0);
        }
        else{
            Debug.LogError("fullscreenOutline(Material) == mull");
        }
    }

    protected override void Cleanup()
    {
        Debug.Log("MyOutLinePass.Outline.Cleanup");
        if(fullscreenOutline!=null){
            CoreUtils.Destroy(fullscreenOutline);
        }
        if(outlineBuffer!=null){
            outlineBuffer.Release();
        }
        
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NightVisionPass : ScriptableRenderPass
{
    private Material material;
    private NightVisionPostProcess nightVisionPostProcess;
    private RenderTargetIdentifier src;
    private RenderTargetHandle dest;
    private int texID;

    public NightVisionPass()
    {
        if (!material)
        {
            material = CoreUtils.CreateEngineMaterial("Custom Post-Processing/NightVision");
        }
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        nightVisionPostProcess = VolumeManager.instance.stack.GetComponent<NightVisionPostProcess>();

        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

        src = renderingData.cameraData.renderer.cameraColorTarget;

        base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if (nightVisionPostProcess == null || !nightVisionPostProcess.IsActive())
            return;
        

        texID = Shader.PropertyToID("_MainTex");
        dest = new RenderTargetHandle();
        dest.id = texID;
        
        cmd.GetTemporaryRT(texID, cameraTextureDescriptor);

        base.Configure(cmd, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (nightVisionPostProcess == null || !nightVisionPostProcess.IsActive())
            return;
        
        if(!renderingData.postProcessingEnabled) return;
        CommandBuffer cmd = CommandBufferPool.Get("Custom Post-Processing/NightVision");
        material.SetFloat("_LuminosityMidpoint", nightVisionPostProcess.luminosityMidpoint.value);
        material.SetFloat("_LuminosityIntensity", nightVisionPostProcess.luminosityIntensity.value);
        material.SetColor("_NightVisionTint", nightVisionPostProcess.nightVisionTint.value);
        material.SetFloat("_NightVisionIntensity", nightVisionPostProcess.nightVisionIntensity.value);
        material.SetFloat("_blend", nightVisionPostProcess.blend.value);
        material.SetInt("_pixelize", nightVisionPostProcess.pixelize.value);

        cmd.Blit(src, texID, material, 0);
        cmd.Blit(texID, src, material);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}

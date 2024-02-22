using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ToonShaderPass : ScriptableRenderPass
{
    private Material material;
    private ToonShaderPostProcess toonPostProcess;
    private RenderTargetIdentifier src;
    private RenderTargetHandle dest;
    private int texID;

    public ToonShaderPass()
    {
        if(!material)
        {
            material = CoreUtils.CreateEngineMaterial("Custom Post-Processing/Toon Shader");
        }
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(toonPostProcess == null || !toonPostProcess.IsActive()) return;
        if(!renderingData.postProcessingEnabled) return;

        CommandBuffer cmd = CommandBufferPool.Get("Custom Post-Processing/Toon Shader");
        material.SetFloat("_Blend",toonPostProcess.blend.value);

        cmd.Blit(src, texID, material, 0);
        cmd.Blit(texID, src, material);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        toonPostProcess = VolumeManager.instance.stack.GetComponent<ToonShaderPostProcess>();

        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

        src = renderingData.cameraData.renderer.cameraColorTarget;
        //base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if(toonPostProcess == null || !toonPostProcess.IsActive()) return;

        texID = Shader.PropertyToID("_MainTex");
        dest = new RenderTargetHandle();
        dest.id = texID;

        cmd.GetTemporaryRT(texID, cameraTextureDescriptor);

        base.Configure(cmd, cameraTextureDescriptor);
    }
}

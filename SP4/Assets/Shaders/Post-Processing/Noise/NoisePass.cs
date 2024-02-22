using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NoisePass : ScriptableRenderPass
{
    private Material material;
    private NoisePostProcess noisePP;
    private RenderTargetIdentifier src;
    private RenderTargetHandle dest;
    private int texID;

    public NoisePass()
    {
        if(!material)
        {
            material = CoreUtils.CreateEngineMaterial("Shader Graphs/Noise");
        }
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(noisePP == null || !noisePP.IsActive()) return;
        if(!renderingData.postProcessingEnabled) return;

        CommandBuffer cmd = CommandBufferPool.Get("Custom Post-Processing/Noise");
        material.SetFloat("_Blend",noisePP.blend.value);
        material.SetFloat("_Brightness",noisePP.brightness.value);

        cmd.Blit(src, texID, material, 0);
        cmd.Blit(texID, src, material);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        noisePP = VolumeManager.instance.stack.GetComponent<NoisePostProcess>();

        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

        src = renderingData.cameraData.renderer.cameraColorTarget;
        //base.OnCameraSetup(cmd, ref renderingData);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        if(noisePP == null || !noisePP.IsActive()) return;

        texID = Shader.PropertyToID("_MainTex");
        dest = new RenderTargetHandle();
        dest.id = texID;

        cmd.GetTemporaryRT(texID, cameraTextureDescriptor);

        base.Configure(cmd, cameraTextureDescriptor);
    }
}

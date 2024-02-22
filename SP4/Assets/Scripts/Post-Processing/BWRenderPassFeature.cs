using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class BWRenderPassFeature : ScriptableRendererFeature
{
    private BWPass bwpass;
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(bwpass);
    }

    public override void Create()
    {
        bwpass = new BWPass();
    }

    class BWPass : ScriptableRenderPass
    {
        Material _mat;
        int bwId = Shader.PropertyToID("_Temp");
        RenderTargetIdentifier src, bw;

        public BWPass() { 
            if(!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("Custom/BlackAndWhite");
            }
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;

            src = renderingData.cameraData.renderer.cameraColorTarget;

            cmd.GetTemporaryRT(bwId, desc, FilterMode.Bilinear);
            bw = new RenderTargetIdentifier(bwId);
            //base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(bwId);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("BWRenderPassFeature");
            VolumeStack volumes = VolumeManager.instance.stack;
            BlackAndWhitePostProcess bwPP = volumes.GetComponent<BlackAndWhitePostProcess>();


            if(bwPP.IsActive())
            {
                _mat.SetFloat("_blend", (float)bwPP.blendIntensity);

                //apply black and white
                Blit(commandBuffer, src, bw, _mat, 0);

                //back to source
                Blit(commandBuffer, bw, src);
            }

            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NoiseRenderFeature : ScriptableRendererFeature
{
    private NoisePass noisePass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(noisePass);
    }

    public override void Create()
    {
        noisePass = new NoisePass();
        name = "Noise";
    }
}

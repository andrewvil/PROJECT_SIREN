using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToonShaderRenderFeature : ScriptableRendererFeature
{
    private ToonShaderPass toonShaderPass;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(toonShaderPass);
    }

    public override void Create()
    {
        toonShaderPass = new ToonShaderPass();
        name = "Toon Shader";
    }
}

using UnityEngine.Rendering.Universal;

public class NightVisionRenderFeature : ScriptableRendererFeature
{
private NightVisionPass nightVisionPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(nightVisionPass);
    }

    public override void Create()
    {
        nightVisionPass = new NightVisionPass();
        name = "NightVision";
    }
}

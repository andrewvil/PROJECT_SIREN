using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[SerializeField, VolumeComponentMenuForRenderPipeline("Custom Post-Processing/NightVision", typeof(UniversalRenderPipeline))]
public class NightVisionPostProcess : VolumeComponent, IPostProcessComponent
{
    
    public FloatParameter luminosityMidpoint = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
    public FloatParameter luminosityIntensity = new ClampedFloatParameter(0.0f, 0.0f, 50.0f);
    public ColorParameter nightVisionTint = new ColorParameter(new Color());
    public FloatParameter nightVisionIntensity = new ClampedFloatParameter(0.0f, 0.0f, 20.0f);
    public FloatParameter blend = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

    public IntParameter pixelize = new IntParameter(1);

    public bool IsActive()
    {
        return (nightVisionIntensity.value > 0.0f) && active;
    }

    public bool IsTileCompatible()
    {
        return true;
    }
}

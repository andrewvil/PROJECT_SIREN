using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[Serializable, VolumeComponentMenuForRenderPipeline("Custom Post-Processing/Noise",typeof(UniversalRenderPipeline))]
public class NoisePostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blend = new ClampedFloatParameter(0.0f,0.0f,1f);
    public FloatParameter brightness = new ClampedFloatParameter(0.0f, 0.0f, 1f);
    public bool IsActive()
    {
        return (blend.value > 0.0f || brightness.value > 0.0f) && active;
    }

    public bool IsTileCompatible() =>true;
}

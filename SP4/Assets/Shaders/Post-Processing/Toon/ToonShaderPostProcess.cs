using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[Serializable, VolumeComponentMenuForRenderPipeline("Custom Post-Processing/Toon Shader",typeof(UniversalRenderPipeline))]
public class ToonShaderPostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blend = new ClampedFloatParameter(0.0f,0.0f,1f);
    public bool IsActive()
    {
        return (blend.value > 0.0f) && active;
    }

    public bool IsTileCompatible() =>true;
}

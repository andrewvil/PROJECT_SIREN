using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/BlackAndWhite",typeof(UniversalRenderPipeline))]
public class BlackAndWhitePostProcess : VolumeComponent, IPostProcessComponent
{
    public FloatParameter blendIntensity = new FloatParameter(1f);
    public bool IsActive() => true;

    public bool IsTileCompatible() => true;
}

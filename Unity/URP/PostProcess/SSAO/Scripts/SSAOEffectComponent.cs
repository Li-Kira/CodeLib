using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
[VolumeComponentMenuForRenderPipeline("Custom/SSAO", typeof(UniversalRenderPipeline))]
public class SSAOEffectComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter radius = new ClampedFloatParameter(0.5f, 0f, 0.8f,  true);
    public NoInterpColorParameter color = new NoInterpColorParameter(Color.black);
    public ClampedIntParameter sampleCount = new ClampedIntParameter(22, 1, 128);
    public ClampedFloatParameter rangeCheck = new ClampedFloatParameter(0f, 0f, 10f);
    public ClampedFloatParameter aoInt  = new ClampedFloatParameter(1f, 0f, 3f);
    
    public ClampedFloatParameter blurSize = new ClampedFloatParameter(1f, 0f, 10f);
    
    // public ClampedFloatParameter blurRadius = new ClampedFloatParameter(1f, 0f, 3f);
    // public ClampedFloatParameter bilaterFilterFactor = new ClampedFloatParameter(0.1f, 0f, 1f);
    
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}

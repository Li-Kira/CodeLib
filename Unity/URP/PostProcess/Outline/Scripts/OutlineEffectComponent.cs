using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
[VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class OutlineEffectComponent : VolumeComponent, IPostProcessComponent
{
    [Header("Edge Detection Settings")]
    public ClampedFloatParameter scale = new ClampedFloatParameter(value: 0, min: 0, max: 5);
    public ClampedFloatParameter depthThreshold = new ClampedFloatParameter(value: 20f, min: 0, max: 30, true);
    public ClampedFloatParameter normalThreshold = new ClampedFloatParameter(value: 0.7f, min: 0, max: 2, true);
    public ClampedFloatParameter depthNormalThreshold = new ClampedFloatParameter(value: 0.5f, min: 0, max: 1, true);
    public ClampedFloatParameter depthNormalThresholdScale = new ClampedFloatParameter (value: 7, min: 0, max: 10, true);

    [Header("Line Settings")]
    public NoInterpColorParameter edgeColor = new NoInterpColorParameter(Color.white, true);
    
    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}

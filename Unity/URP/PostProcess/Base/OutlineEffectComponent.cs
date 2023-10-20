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
    public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, true);
    public NoInterpColorParameter overlayColor = new NoInterpColorParameter(Color.white);

    public FloatParameter thickness = new FloatParameter(1.0f, true);
    public FloatParameter depthMin = new FloatParameter(0);
    public FloatParameter depthMax = new FloatParameter(1.0f);
    public NoInterpColorParameter edgeColor = new NoInterpColorParameter(Color.white);
    
    public bool IsActive() => true;
    public bool IsTileCompatible() => true;
}

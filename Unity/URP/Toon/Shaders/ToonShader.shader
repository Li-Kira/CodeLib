Shader "Custom/ToonShader"
{
    Properties
    {
        [Header(Texture)]
        _MainTex ("Base Texture", 2D) = "white" {}
        _RampTex ("Ramp Texture", 2D) = "white" {}
        _LightMap ("LightMap", 2D) = "white" {}
        _SDFTex ("Face SDF Texture", 2D) = "white" {}
        _EmissionTex ("Emission Texture", 2D) = "white" {}
        
        [Header(Setting)]
        [Toggle(SHADOW_RAMP_ON)] _isRampOn ("isRampOn", Float) = 0.0
        [Toggle(SHADOW_FACE_ON)] _isFace ("isFace", Float) = 0.0
        [Toggle(EMISSION_ON)] _isEmission ("isEmission", Float) = 0.0
        [Toggle(BLINK_ON)] _isBlink ("isBlink", Float) = 0.0
        [Toggle(REFLECTION_ON)] _isReflection ("isReflection", Float) = 0.0
        
        [Header(Shadow)]
        _ShadowRange("Shadow Range", Float) = 0
        _ShadowSmooth ("Shadow Smooth", Float) = 0.01
        
        [Header(Ramp)]
        _ShadowRampNum ("Ramp Num", Float) = 16
        _ShadowRampWidth ("Ramp Width", Float) = 256
        
        [Header(Aniso)]
        _AnisoFresnelPow ("AnisoFresnelPow", Float) = 20
        _AnisoFresnelIntensity ("AnisoFresnelIntensity", Float) = 0.01
        
        [Header(RimLight)]
        _OffsetMul ("RimLight Offset", Float) = 0.0012
        _Threshold ("RimLight Threshold", Float) = 0.07
        _FresnelMask ("RimLight Threshold", Float) = 0.7
        
        [Header(Emission)]
        _EmissionIntensity("Emission Intensity", Float) = 1.0
        
        [Header(Outline)]
        _OutlineWidth("Width", Range(0,4)) = 1
        _OutlineColor("Color", Color) = (0.5,0.5,0.5,1)
        
        [Header(Outline ZOffset)]
        _OutlineZOffset("ZOffset (View Space)", Range(0,1)) = 0.0001
        [NoScaleOffset]_OutlineZOffsetMaskTex("    Mask (black is apply ZOffset)", 2D) = "black" {}
        _OutlineZOffsetMaskRemapStart("    RemapStart", Range(0,1)) = 0
        _OutlineZOffsetMaskRemapEnd("    RemapEnd", Range(0,1)) = 1
        
        [Header(Transparent)]
        _Alpha("Alpha", Range(0,1)) = 1
        
        [Header(Reflection)]
        _ReflectionIntensity("Reflection Intensity", Float) = 0.2
        
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"  
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "Toon"
            Tags
            { 
                //"LightMode" = "UniversalForward" 
                "LightMode" = "UniversalForwardOnly"
            }

            Blend One Zero
            ZWrite On
            Cull Off
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma shader_feature_local_fragment SHADOW_FACE_ON
            #pragma shader_feature_local_fragment SHADOW_RAMP_ON
            #pragma shader_feature_local_fragment EMISSION_ON
            #pragma shader_feature_local_fragment REFLECTION_ON
            #pragma shader_feature_local_fragment BLINK_ON

            #pragma vertex Toon_Vert
            #pragma fragment Toon_Frag
            #include "Toon.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags{  }
            
            Blend One Zero
            ZWrite On
            Cull Front
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex Outline_Vert
            #pragma fragment Outline_Frag
            #include "Outline.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            
            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #pragma multi_compile_instancing
            
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma target 2.0


            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
            ENDHLSL
        }

        
    }
}

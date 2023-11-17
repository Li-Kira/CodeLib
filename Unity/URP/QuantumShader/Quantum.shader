Shader "Custom/Quantum"
{
    Properties
    {
        [Header(Quantum)]
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenTex ("Screen Texture", 2D) = "white" {}
        _ScreenColor ("ScreenColor", Color) = (0.02254656, 0.0255132, 0.1886792, 0)
        _ScreenTexScale ("ScreenTex Scale", Float) = 0.4
        _FresnelPow ("Fresnel Pow", Float) = 1
        _FresnelIntensity ("Fresnel Intensity", Float) = 1
        
        
        [Header(Mask)]
        _FresnelMaskColor ("FresnelMask Color", Color) = (0.1530562, 0.09781132, 0.9056604, 0)
        _FresnelMaskOffset ("FresnelMask Offset", Float) = 0.15
        _FresnelMaskPow ("FresnelMask Pow", Float) = 1
        _FresnelMaskIntensity ("FresnelMask Intensity", Float) = 0.68
        _FresnelMaskSmoothMin ("FresnelMask Smooth Min", Float) = 0
        _FresnelMaskSmoothMax ("FresnelMask Smooth Max", Float) = 1
         
        [Header(RimLight)]
        _RimLightColor ("FresnelMask Color", Color) = (1, 1, 1, 0)
        _RimLightScale ("RimLight Scale", Float) = 0.85
        _RimLightSmoothMin ("RimLight Smooth Min", Float) = 0.6
        _RimLightSmoothMax ("RimLight Smooth Max", Float) = 0.85
        
        [Header(Alpha)]
        _AlphaPow ("Alpha Pow", Float) = 1.2
        _AlphaScale ("Alpha Scale", Float) = 0.8
        
        
        [Header(Outline)]
        _OutlineOffset("Outline Offset", Float) = 0.0002
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"  
        }
        
        LOD 100

        Pass
        {
            Zwrite On
            ColorMask 0
        }
        
        
        Pass
        {
            Tags
            { 
                "LightMode" = "UniversalForwardOnly"
            }
            
            Name "Quantum"
            
            Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
            ZWrite Off
			ZTest LEqual

            
            HLSLPROGRAM
            #pragma vertex Quantum_Vert
            #pragma fragment Quantum_Frag
            #include "Quantum.hlsl"
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

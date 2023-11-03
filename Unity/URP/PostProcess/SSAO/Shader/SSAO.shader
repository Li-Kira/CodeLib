Shader "Hidden/SSAO"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _aoColor("aoColor", Color) = (1,1,1,1)
    }
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            Name "SSAO"
            HLSLPROGRAM
            #pragma vertex SSAO_Vert
            #pragma fragment SSAO_Frag
            #include "SSAOPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Vertical Blur"
            HLSLPROGRAM
            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur
            #include "BlurPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "Horizental Blur"
            HLSLPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur
            #include "BlurPass.hlsl"
            ENDHLSL
        }
        
        

//        Pass
//        {
//            HLSLPROGRAM
//            #pragma vertex vert_h
//            #pragma fragment frag_Blur
//
//            #include "Blur.hlsl"
//
//            ENDHLSL
//        }

//        Pass
//        {
//            HLSLPROGRAM
//            #pragma vertex vert_v
//            #pragma fragment frag_Blur
//
//            #include "Blur.hlsl"
//
//            ENDHLSL
//        }
        
        

        Pass
        {
            Name "Final SSAO"
            HLSLPROGRAM
            #pragma vertex SSAO_Vert
            #pragma fragment Final_Frag
            #include "SSAOPass.hlsl"
            ENDHLSL
        }
        
    }
}

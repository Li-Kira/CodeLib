Shader "Custom/QuantumOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        
        
        _OutlineOffset("Outline Offset", Float) = 1
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"  
        }
        
        LOD 100


        Pass
        {
            Tags
            { 
                "RenderType"="Transparent" 
                "RenderPipeline" = "UniversalPipeline"  
            }
            
            Name "Outline"
            
            Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
            ZWrite Off
			ZTest LEqual

            
            HLSLPROGRAM
            #pragma vertex Outline_Vert
            #pragma fragment Outline_Frag
            #include "Quantum.hlsl"
            ENDHLSL
        }
        

        
    }
}

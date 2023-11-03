#ifndef BLUR_INCLUDED
#define BLUR_INCLUDED

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    float4  _AOTex_TexelSize;
    float _BlurSize;
    CBUFFER_END

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    TEXTURE2D(_AOTex);
    SAMPLER(sampler_AOTex);

    struct Attribute
    {
        float4 position : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 position : SV_POSITION;
        float2 uv[5]  : TEXCOORD0;
    };

    Varyings vertBlurHorizontal(Attribute v)
    {
        Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.position.xyz);
        o.position = PositionInputs.positionCS;
        
        half2 uv = v.uv;
        
        o.uv[0] = uv;
        o.uv[1] = uv + float2(_AOTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
        o.uv[2] = uv - float2(_AOTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
        o.uv[3] = uv + float2(_AOTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
        o.uv[4] = uv - float2(_AOTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
					 
        return o;     
    }

    Varyings vertBlurVertical(Attribute v)
    {
        Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.position.xyz);
        o.position = PositionInputs.positionCS;

        half2 uv = v.uv;
			
        o.uv[0] = uv;
        o.uv[1] = uv + float2(0.0, _AOTex_TexelSize.y * 1.0) * _BlurSize;
        o.uv[2] = uv - float2(0.0, _AOTex_TexelSize.y * 1.0) * _BlurSize;
        o.uv[3] = uv + float2(0.0, _AOTex_TexelSize.y * 2.0) * _BlurSize;
        o.uv[4] = uv - float2(0.0, _AOTex_TexelSize.y * 2.0) * _BlurSize;
					 
        return o;
    }


    half4 fragBlur(Varyings i) : SV_Target {
        float weight[3] = {0.4026, 0.2442, 0.0545};
        

        float3 sum = SAMPLE_TEXTURE2D(_AOTex, sampler_AOTex, i.uv[0]) * weight[0];
        
        for (int it = 1; it < 3; it++) {
            sum += SAMPLE_TEXTURE2D(_AOTex, sampler_AOTex, i.uv[it*2-1]) * weight[it];
            sum += SAMPLE_TEXTURE2D(_AOTex, sampler_AOTex, i.uv[it*2]) * weight[it];
        }
			    
        return float4(sum, 1.0);
    }


    

#endif //BLUR_INCLUDED
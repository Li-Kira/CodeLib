#ifndef SSAO_INCLUDED
#define SSAO_INCLUDED

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    float4 _Color;

    float4x4 _VPMatrix_invers;
    float _SampleCount;
    float _Radius;
    float4x4 _VMatrix;
    float4x4 _PMatrix;
    float _AOInt;
    float _RangeCheck;
    CBUFFER_END
    
    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
	TEXTURE2D(_CameraNormalsTexture);
    SAMPLER(sampler_CameraNormalsTexture);
    TEXTURE2D(_AOTex);
    SAMPLER(sampler_AOTex);
	//TEXTURE2D(_CameraDepthTexture);
    //SAMPLER(sampler_CameraDepthTexture);

    struct Attribute
    {
        float4 vertex : POSITION;
    	float2 texcoord : TEXCOORD0;
        float3 normal : TEXCOORD1;
    };

    struct Varyings
	{
    	float4 vertex : SV_POSITION;
    	float2 texcoord  : TEXCOORD0;
        float3 normal : TEXCOORD1;
	};

    
    float4 GetWorldPos(float2 uv)
    {
        float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv)).r;
    #if defined(UNITY_REVERSED_Z)
        rawDepth = 1 - rawDepth;
    #endif
        float4 ndc = float4(uv.xy * 2 - 1, rawDepth * 2 - 1, 1);
        float4 wPos = mul(_VPMatrix_invers, ndc);
        wPos /= wPos.w;
        return wPos;
    }

    float3 GetWorldNormal(float2 uv)
    {
        float3 wNor = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).xyz; //world normal
        return wNor;
    }

    float GetEyeDepth(float2 uv)
    {
        float rawDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv)).r;
        return LinearEyeDepth(rawDepth, _ZBufferParams);
    }
    
    // 生成随机随机向量
    float Hash(float2 p)
    {
        return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
    }
    
    float3 GetRandomVec(float2 p)
    {
        float3 vec = float3(0, 0, 0);
        vec.x = Hash(p) * 2 - 1;
        vec.y = Hash(p * p) * 2 - 1;
        vec.z = Hash(p * p * p) * 2 - 1;
        return normalize(vec);
    }

    float3 GetRandomVecHalf(float2 p)
    {
        float3 vec = float3(0, 0, 0);
        vec.x = Hash(p) * 2 - 1;
        vec.y = Hash(p * p) * 2 - 1;
        vec.z = saturate(Hash(p * p * p) + 0.2);
        return normalize(vec);
    }

    Varyings SSAO_Vert(Attribute v)
    {
	    Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.vertex.xyz);
        o.vertex = PositionInputs.positionCS;
        o.texcoord = v.texcoord;

        return o;
    }


    half4 SSAO_Frag(Varyings i) : SV_Target
    {
        float3 worldPos = GetWorldPos(i.texcoord);
        float3 worldNormal = GetWorldNormal(i.texcoord);
        float3 randomVec  = GetRandomVec(i.texcoord);
        float depth = GetEyeDepth(i.texcoord);
        
        float3 tangent = normalize(randomVec  - worldNormal * dot(randomVec , worldNormal));
        float3 bitangent = cross(worldNormal, randomVec);
        float3x3 TBN = float3x3(tangent, bitangent, worldNormal);
        
        
        float ao = 0;
        int sampleCount = (int)_SampleCount;
        
        //采样核
        [unroll(128)]
        for (int s = 0; s < sampleCount; s++)
        {
            float3 sample = GetRandomVecHalf(s * i.texcoord);
            float scale = s / _SampleCount;
            scale = lerp(0.01f, 1.0f, scale * scale);
            
            sample *= scale * _Radius;
            float weight = smoothstep(0, 0.2, length(sample));
            sample = mul(sample, TBN);
            sample += worldPos;
            
            float4 offset = float4(sample, 1.0);
            offset = mul(_VMatrix, offset);
            offset = mul(_PMatrix, offset);
            offset.xy /= offset.w; 
            offset.xy = offset.xy * 0.5 + 0.5;
            
            float sampleDepth = SampleSceneDepth(offset.xy);
            sampleDepth = LinearEyeDepth(sampleDepth, _ZBufferParams);
            
            float sampleZ = offset.w;
            float rangeCheck = smoothstep(0, 1.0, _Radius / abs(sampleZ - sampleDepth) * _RangeCheck * 0.1);
            float selfCheck = (sampleDepth < depth - 0.08) ?  1 : 0;
            ao += (sampleDepth < sampleZ) ?  1 * rangeCheck * selfCheck * _AOInt * weight : 0;
            
        }

        ao = 1 - saturate((ao / sampleCount));
        return ao;
        
    }


    half4 Final_Frag(Varyings i) : SV_Target
    {
        half4 scrTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        half4 aoTex = SAMPLE_TEXTURE2D(_AOTex, sampler_AOTex, i.texcoord);

        half4 finalCol = lerp(scrTex * _Color, scrTex, aoTex.r);
        return finalCol;
    }


#endif //SSAO_INCLUDED
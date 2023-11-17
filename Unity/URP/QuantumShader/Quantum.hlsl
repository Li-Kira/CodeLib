#ifndef QUANTUM_INCLUDED
#define QUANTUM_INCLUDED

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
    half4 _ScreenColor;
    float _FresnelPow;
    float _FresnelIntensity;
    float _ScreenTexScale;

    half4 _FresnelMaskColor;
    float _FresnelMaskOffset;
    float _FresnelMaskPow;
    float _FresnelMaskIntensity;

    float _FresnelMaskSmoothMin;
    float _FresnelMaskSmoothMax;

    half4 _RimLightColor;
    float _RimLightScale;
    float _RimLightSmoothMin;
    float _RimLightSmoothMax;

    float _AlphaPow;
    float _AlphaScale;

    float _OutlineOffset;
    half4 _OutlineColor;
    CBUFFER_END


    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
   

    TEXTURE2D(_ScreenTex);
    SAMPLER(sampler_ScreenTex);
    float4 _ScreenTex_ST;


    struct Attribute
    {
        float4 position : POSITION;
        float2 uv : TEXCOORD0;
        float3 normal : NORMAL;
    };

    struct Varyings
    {
        float4 position : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 worldPos : TEXCOORD1;
        float3 worldNormal : TEXCOORD2;
        float3 worldViewDir : TEXCOORD3;
        float4 positionSS : TEXCOORD4;
    };

    inline float4 ComputeGrabScreenPos(float4 pos)
    {
        #if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
        #else
        float scale = 1.0;
        #endif
        float4 o = pos;
        o.y = pos.w * 0.5f;
        o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
        return o;
    }

    float2 GetComputedUV(float2 uv, float speed, float2 time, float4 name_ST)
    {
        float2 output = float2(uv.x + time.x * speed, uv.y + time.y * speed);
        output = output.xy * name_ST.xy + name_ST.zw;
        return output;
    }

    float GetFresnel(Varyings i)
    {
        float NdotV = dot(i.worldNormal, i.worldViewDir);
        float fresnel = 1 - saturate(NdotV);
        return fresnel;
    }

    Varyings Quantum_Vert(Attribute v)
    {
        Varyings o;

        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.position.xyz);
        VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normal);
        
        o.position = PositionInputs.positionCS;
        o.worldPos =  TransformObjectToWorld(v.position.xyz);
        o.positionSS = ComputeGrabScreenPos(PositionInputs.positionCS);
        o.worldNormal = normalize(vertexNormalInput.normalWS); 
        o.worldViewDir = normalize(GetCameraPositionWS() - o.worldPos);
        
        o.uv = TRANSFORM_TEX(v.uv, _ScreenTex);
        return o;
    }

    half4 Quantum_Frag(Varyings i) : SV_Target
    {
        float fresnel = GetFresnel(i);
        float fresnelMask = GetFresnel(i);
        
        fresnel = pow(fresnel, _FresnelPow);
        fresnel *= _FresnelIntensity;

        fresnelMask = pow(fresnelMask, _FresnelMaskPow);
        fresnelMask *= _FresnelMaskIntensity;
        
        float time = _Time.y;
        float2 uv = i.positionSS;
        uv *= _ScreenTexScale;
        
        uv = GetComputedUV(uv, 0.1, float2(0, time), _ScreenTex_ST);
        half3 screeColor = SAMPLE_TEXTURE2D(_ScreenTex, sampler_ScreenTex, uv);
        screeColor = lerp(screeColor, _ScreenColor, fresnel);
        
        float fresnelMaskPos = -i.worldPos.y;
        fresnelMaskPos += _FresnelMaskOffset;
        fresnelMaskPos = smoothstep(_FresnelMaskSmoothMin, _FresnelMaskSmoothMax, fresnelMaskPos);
        half3 fresnelMaskColor = _FresnelMaskColor * fresnelMask * fresnelMaskPos;

        float rimlight = GetFresnel(i);
        rimlight = smoothstep(_RimLightSmoothMin, _RimLightSmoothMax, rimlight);
        rimlight *= _RimLightScale * _RimLightColor;

        
        half3 color = screeColor + fresnelMaskColor + rimlight;

        float NdotV = dot(i.worldNormal, i.worldViewDir);
        float alpha = pow(NdotV, _AlphaPow);
        alpha *= _AlphaScale;
        alpha += fresnelMask;
        
        
        return half4(color, alpha);
    }








    Varyings Outline_Vert(Attribute v)
    {
        Varyings o;
        v.position.xyz += v.normal * _OutlineOffset;
        o.position = TransformObjectToHClip(v.position);
        return o;
    }

    half4 Outline_Frag(Varyings i) : SV_Target
    {
        float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half4 color = _OutlineColor;
        return color;
    }




#endif //QUANTUM_INCLUDED
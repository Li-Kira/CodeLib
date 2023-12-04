#ifndef TOON_INCLUDED
#define TOON_INCLUDED


    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

    CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;

    float _ShadowRange;
    float _ShadowSmooth;
    float _ShadowRampWidth;
    float _ShadowRampNum;

    float _AnisoFresnelPow;
    float _AnisoFresnelIntensity;

    float _EmissionIntensity;

    float _OffsetMul;
    float _Threshold;
    float _FresnelMask;

    float _Alpha;
    float _ReflectionIntensity;
    CBUFFER_END

    float4 _MainTex_TexelSize;

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    TEXTURE2D(_RampTex);
    SAMPLER(sampler_RampTex);

    TEXTURE2D(_LightMap);
    SAMPLER(sampler_LightMap);

    TEXTURE2D(_SDFTex);
    SAMPLER(sampler_SDFTex);

    TEXTURE2D(_EmissionTex);
    SAMPLER(sampler_EmissionTex);

    TEXTURE2D(_DepthTexture);
    SAMPLER(sampler_DepthTexture);

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
        float3 normal : TEXCOORD1;
        float3 worldPos : TEXCOORD2;
        float3 worldNormal : TEXCOORD3;
        float3 worldViewDir : TEXCOORD4;

        float3 positionVS : TEXCOORD5;
        float3 positionWS : TEXCOORD6;
        float4 positionNDC : TEXCOORD7;
    };

    float adjustAO(float ao) {
        ao += 0.25;
        ao =  clamp(ao, 0.0, 1.0);
        return ao;
    }

    float GetAO(Varyings i, float3 lightDirWS)
    {
        float NdotL = dot(i.worldNormal, lightDirWS);
        float halfLambert = 0.5 + 0.5 * NdotL;

        half3 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, i.uv);
        float ao = lightMap.r;
        
        return ao;
        
        // // -0.1 能让ao不是那么暗
        // float shadow = halfLambert * smoothstep(-0.1, 0.2, ao);
        // float noAOMask = step(0.9, ao);
        // shadow = lerp(shadow, 1.0, noAOMask);
        //
        // // 修正眼球错误AO
        // #ifdef SHADOW_FACE_ON
        //     shadow = 1;
        // #endif
        
        //return adjustAO(shadow);
    }

    float GetFaceShadow(Varyings i, float3 lightDirWS)
    {
        float3 forwardDirWS = normalize(TransformObjectToWorldDir(float3(0.0,0.0,1.0)));
        float3 rightDirWS = normalize(TransformObjectToWorldDir(float3(1.0,0.0,0.0)));

        float2 faceShadowUV = float2(1 - i.uv.x, i.uv.y);
        float faceShadow_left = SAMPLE_TEXTURE2D(_SDFTex, sampler_SDFTex, i.uv).r;
        float faceShadow_right = SAMPLE_TEXTURE2D(_SDFTex, sampler_SDFTex, faceShadowUV).r;
        
        float FdotL = dot(forwardDirWS, lightDirWS);
        float RdotL = dot(rightDirWS, lightDirWS);

        // 通过RdotL决定用哪张阴影图,当主光角度大于180选择右侧阴影图。
        float shadowTex = RdotL > 0 ? faceShadow_right : faceShadow_left;

        // 阈值
        float faceShadowThreshold = RdotL > 0 ? (1 - acos(RdotL) / PI * 2) : (acos(RdotL) / PI * 2 - 1);
        
        float shadowBehind = step(0, FdotL);
        float shadowFront = step(faceShadowThreshold, shadowTex);

        // 如果光线在背后，则全是阴影，如果光线在前面，则按光线位置来决定阴影
        float shadow = mul(shadowBehind, shadowFront);

        // 矫正眼部阴影
        float eyeShadowMask = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, faceShadowUV).g;
        float eyeMask = step(0.5, eyeShadowMask);
        shadow = lerp(shadow, 1.0, eyeMask);
        
        return shadow;
    }

    float GetShadow(Varyings i, float3 lightDirWS)
    {
        float NdotL = dot(i.worldNormal, lightDirWS);
        float shadow = smoothstep(_ShadowRange, _ShadowRange, NdotL);
        //float shadow = smoothstep(_ShadowRange, _ShadowRange + _ShadowSmooth, NdotL);
        float ao = GetAO(i, lightDirWS);
        ao = smoothstep(-0.1, 0.5, ao);
        shadow *= ao;
        
        
        #ifdef SHADOW_FACE_ON
            shadow = GetFaceShadow(i, lightDirWS);
        #endif
        
        return shadow;
    }

    float2 GetRampUV(float shadow, float RampMap)
    {
        // 白天的情况，夜晚需要在rampV上面再加一个0.5
        float rampU = shadow;
        float rampV = RampMap * 0.45;

        #ifdef SHADOW_FACE_ON
            rampV = 0.1;
        #endif
        
        return float2(rampU, rampV);
    }

    // half3 GetRampColor(Varyings i, float3 lightDirWS)
    // {
    //     float shadow = GetShadow(i, lightDirWS);
    //     float2 rampUV = GetRampUV(shadow, _ShadowRampNum);
    //     float lightValue = step(_ShadowRampWidth, rampUV.x);
    //
    //     half3 rampColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV);
    //     rampColor = lerp(rampColor, 1.0, lightValue);
    //
    //     return rampColor;
    // }

    half3 GetRampColor(Varyings i, float3 lightDirWS)
    {
        float shadow = GetShadow(i, lightDirWS);
        float RampMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, i.uv).a;
        
        float2 rampUV = GetRampUV(shadow, RampMap);
        half3 rampColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, rampUV);
        
        return rampColor;
    }

    half3 GetSpecular(Varyings i, float3 lightDirWS)
    {
        float3 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, i.uv);
        
        // 各向异性高光
        float NdotV = dot(i.worldNormal, i.worldViewDir);
        float anisoFresnel = pow((1.0 - saturate(NdotV)), _AnisoFresnelPow) * _AnisoFresnelIntensity;
        float NdotL = dot(i.worldNormal, lightDirWS);
        float halfLambert = 0.5 + 0.5 * NdotL;
        float aniso = saturate(1 - anisoFresnel) * lightMap.b * halfLambert * 0.3;
        
        #ifdef SHADOW_FACE_ON
            aniso = 0;
        #endif

        return aniso;
    }

    float3 GetEmission(Varyings i, half3 baseColor)
    {
        float emissionMask = SAMPLE_TEXTURE2D(_EmissionTex, sampler_EmissionTex, i.uv).a;
        float3 emission = baseColor * emissionMask * _EmissionIntensity;
        #ifdef BLINK_ON
            float time = _CosTime.w * 0.5 + 0.5;
            emission = baseColor * emissionMask * lerp(0, _EmissionIntensity, time);
        #endif
        
        return emission;
    }
   
    float4 TransformHClipToViewPortPos(float4 positionCS)
    {
        float4 o = positionCS * 0.5f;
        o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
        o.zw = positionCS.zw;
        return o / o.w;
    }

    half3 GetRimLight(Varyings i)
    {   
        float3 worldNormal = i.worldNormal;
        float3 normalVS = TransformWorldToViewDir(worldNormal, true);
        float3 positionVS = i.positionVS;
            
        float3 samplePositionVS = float3(positionVS.xy + normalVS.xy * _OffsetMul, positionVS.z); // 保持z不变（CS.w = -VS.z）
        float4 samplePositionCS = TransformWViewToHClip(samplePositionVS); // input.positionCS不是真正的CS 而是SV_Position屏幕坐标
        float4 samplePositionVP = TransformHClipToViewPortPos(samplePositionCS);

        float depth = i.positionNDC.z / i.positionNDC.w;
        float linearEyeDepth = LinearEyeDepth(depth, _ZBufferParams); // 离相机越近越小
        float offsetDepth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, samplePositionVP).r; // _CameraDepthTexture.r = input.positionNDC.z / input.positionNDC.w
        float linearEyeOffsetDepth = LinearEyeDepth(offsetDepth, _ZBufferParams);
        float depthDiff = linearEyeOffsetDepth - linearEyeDepth;
        float rimIntensity = step(_Threshold, depthDiff);
        
        float3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - i.positionWS);
        float rimRatio = 1 - saturate(dot(viewDirectionWS, worldNormal));
        rimRatio = pow(rimRatio, exp2(lerp(4.0, 0.0, _FresnelMask)));
        rimIntensity = lerp(0, rimIntensity, rimRatio);
        
        half3 rimlight = lerp(float3(0, 0, 0), float3(1, 1, 1), rimIntensity);
        
        float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        rimlight *= baseColor.rgb;
        
        return rimlight;
    }

    float3 GetFresnelSchlickReflection(Varyings i,  float3 lightDirWS)
    {
        float3 F0 = float3(0.04, 0.04, 0.04);
        float3 halfDir = normalize(lightDirWS + i.worldViewDir);
        float3 fresnel = F0 + (1 - F0) * pow(1.0 - dot(i.worldNormal, halfDir), 5.0);
        fresnel *= _ReflectionIntensity;
        
        return fresnel;
    }

    half3 AdjustColor(half3 color)
    {
        float Brightness = 1;
        float Saturation = 1.2;
        float Contrast = 1.03;

        // Apply brightness
        half3 finalColor = color.rgb * Brightness;

        // Apply saturation
        float luminance = 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
        half3 luminanceColor = half3(luminance, luminance, luminance);
        finalColor = lerp(luminanceColor, finalColor, Saturation);

        // Apply contrast
        half3 avgColor = half3(0.5, 0.5, 0.5);
        finalColor = lerp(avgColor, finalColor, Contrast);
                
        return finalColor;
    }

    Varyings Toon_Vert(Attribute v)
    {
        Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.position.xyz);
        VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normal);
        
        o.position = PositionInputs.positionCS;
        o.uv = v.uv;
        o.normal = v.normal;
        o.worldPos =  TransformObjectToWorld(v.position.xyz);
        o.worldNormal = normalize(vertexNormalInput.normalWS); 
        o.worldViewDir = normalize(GetCameraPositionWS() - o.worldPos);
        
        o.positionVS = PositionInputs.positionVS;
        o.positionWS = PositionInputs.positionWS;
        o.positionNDC = PositionInputs.positionNDC;
        
        return o;
    }

    half4 Toon_Frag(Varyings i) : SV_Target
    {
        Light mainLight = GetMainLight(); 
        float3 lightDir = mainLight.direction;
        float4 mainLightColor = float4(mainLight.color, 1);

        // lightMap.r: AO | lightMap.g: Specular | lightMap.b: SpecularIntensity | lightMap.a: Ramp
        half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half3 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, i.uv);
        
        // AO
        float ao =  GetAO(i, lightDir);
        float shadow = GetShadow(i, lightDir);
        //return shadow;
        
        // Ramp        
        half3 rampColor = GetRampColor(i, lightDir);
        //half3 diffuse = rampColor * baseColor.rgb * ao;
        half3 diffuse = rampColor * baseColor.rgb;
        half3 specular = GetSpecular(i, lightDir);

        float3 emission = float3(0, 0, 0);
        // Emission
        #ifdef EMISSION_ON
            emission = GetEmission(i, baseColor.rgb);
            specular += emission;
        #endif
        
        // RimLight
        half3 rimlight = GetRimLight(i);

        // Reflection
        #ifdef REFLECTION_ON
            float3 fresnel = GetFresnelSchlickReflection(i, lightDir);
            specular += fresnel;
        #endif
        
        half3 finalColor = diffuse + specular + rimlight;
        finalColor = AdjustColor(finalColor);
        
        return half4(finalColor, _Alpha);
    }


#endif //TOON_INCLUDED
Shader "Hidden/Outline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    CBUFFER_END

	float4 _MainTex_TexelSize;
    
    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
	TEXTURE2D(_CameraNormalsTexture);
    SAMPLER(sampler_CameraNormalsTexture);
	TEXTURE2D(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    //用来控制识别梯度的阈值
    float _Scale;
	//设置判断深度和法线梯度的阈值
    float _DepthThreshold;
    float _NormalThreshold;
    //得到视图空间的矩阵，用来计算视图方向
    float4x4 _ClipToView;

    //
    float _DepthNormalThreshold;
    float _DepthNormalThresholdScale;

    float _EdgeThickness;
    float4 _EdgeColor;
    
    struct Attribute
    {
        float4 vertex : POSITION;
    	float2 texcoord : TEXCOORD0;
    };

    struct Varyings
	{
    	float4 vertex : SV_POSITION;
    	float2 texcoord  : TEXCOORD0;
    	
		float2 texcoordStereo : TEXCOORD1;
#if STEREO_INSTANCING_ENABLED
	uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
#endif

    	float3 viewSpaceDir : TEXCOORD2;
	};

    
    float2 TransformTriangleVertexToUV(float2 vertex)
	{
	    float2 uv = (vertex + 1.0) * 0.5;
	    return uv;
	}
 
    float2 TransformStereoScreenSpaceTex(float2 uv, float w)
	{
	    float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
	    return uv.xy * scaleOffset.xy + scaleOffset.zw * w;
	}

    float4 alphaBlend(float4 top, float4 bottom)
	{
		float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
		float alpha = top.a + bottom.a * (1 - top.a);

		return float4(color, alpha);
	}
    
    Varyings vert (Attribute v)
    {
        Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.vertex.xyz);
        o.vertex = PositionInputs.positionCS;
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

    	o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);
    	o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
        return o;
    }
    
    Varyings OutlineVert(Attribute v)
	{
		Varyings o;
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.vertex.xyz);
        o.vertex = PositionInputs.positionCS;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

	// #if UNITY_UV_STARTS_AT_TOP
	// 	o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
	// #endif

		o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);
    	float4 vertex = float4(o.vertex.xy, 0.0, -1.0);
    	o.viewSpaceDir = mul(_ClipToView, vertex).xyz;
		return o;
	}


    half4 OutlineFrag (Varyings  i) : SV_Target
    {
        float halfScaleFloor = floor(_Scale * 0.5);
		float halfScaleCeil = ceil(_Scale * 0.5);
    	
		//获取深度梯度
		float2 bottomLeftUV  = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
		float2 topRightUV    = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;
		float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
		float2 topLeftUV     = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);
    	
        float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
		float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
		float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
		float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;

    	//根据深度判断梯度，得到边
    	float depthFiniteDifference0 = depth1 - depth0;
		float depthFiniteDifference1 = depth3 - depth2;
    	
    	//获取法线梯度
		float3 normal0 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomLeftUV).rgb;
		float3 normal1 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topRightUV).rgb;
		float3 normal2 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomRightUV).rgb;
		float3 normal3 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topLeftUV).rgb;

    	float3 normalFiniteDifference0 = normal1 - normal0;
		float3 normalFiniteDifference1 = normal3 - normal2;

    	//去除边缘被识别成一整面的情况
		float3 viewNormal = normal0 * 2 - 1;
		float NdotV = 1 - dot(viewNormal, -i.viewSpaceDir);
		float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
		float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;

		float depthThreshold = _DepthThreshold * depth0 * normalThreshold;
		float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
		edgeDepth = edgeDepth > depthThreshold ? 1 : 0;
    	
		float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
		edgeNormal = edgeNormal > _NormalThreshold ? 1 : 0;
    	
    	//根据法线和深度的结果以及视角放向来得到最终的边缘
    	float edge = max(edgeDepth, edgeNormal);
		float4 edgeColor = float4(_EdgeColor.rgb, _EdgeColor.a * edge);
    	float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
    	
		return alphaBlend(edgeColor, color);


    	
    }
            
    
    ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "Outline"
            
            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            ENDHLSL
        }
    }
}

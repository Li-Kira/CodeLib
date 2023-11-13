#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    
	sampler2D _BaseMap; 
	sampler2D _EmissionMap;
	sampler2D _OcclusionMap;
	sampler2D _OutlineZOffsetMaskTex;


	CBUFFER_START(UnityPerMaterial)

	float4  _BaseMap_ST;
	half4   _BaseColor;

	float _isFace;

	// Outline
	float _OutlineWidth;
	half3 _OutlineColor;

    float _OutlineZOffset;
	float _OutlineZOffsetMaskRemapStart;
	float _OutlineZOffsetMaskRemapEnd;
	CBUFFER_END

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
    TEXTURE2D(_CameraNormalsTexture);
    SAMPLER(sampler_CameraNormalsTexture);

        
    struct Attributes
    {
        float4 positionOS : POSITION;
    	half3 normalOS : NORMAL;
    	half4 tangentOS : TANGENT;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv  : TEXCOORD0;
    	half3 normalWS : TEXCOORD1;
    };

	half invLerp(half from, half to, half value) 
	{
		return (value - from) / (to - from);
	}
	half invLerpClamp(half from, half to, half value)
	{
		return saturate(invLerp(from,to,value));
	}
	// full control remap, but slower
	half remap(half origFrom, half origTo, half targetFrom, half targetTo, half value)
	{
		half rel = invLerp(origFrom, origTo, value);
		return lerp(targetFrom, targetTo, rel);
	}

	float GetCameraFOV()
	{
		float t = unity_CameraProjection._m11;
		float Rad2Deg = 180 / 3.1415;
		float fov = atan(1.0f / t) * 2.0 * Rad2Deg;
		return fov;
	}

	float ApplyOutlineDistanceFadeOut(float inputMulFix)
	{
		//make outline "fadeout" if character is too small in camera's view
		return saturate(inputMulFix);
	}

	float GetOutlineCameraFovAndDistanceFixMultiplier(float positionVS_Z)
	{
		float cameraMulFix;
		if(unity_OrthoParams.w == 0)
		{
			////////////////////////////////
			// Perspective camera case
			////////////////////////////////
			cameraMulFix = abs(positionVS_Z);
			cameraMulFix = ApplyOutlineDistanceFadeOut(cameraMulFix);
			cameraMulFix *= GetCameraFOV();       
		}
		else
		{
			////////////////////////////////
			// Orthographic camera case
			////////////////////////////////
			float orthoSize = abs(unity_OrthoParams.y);
			orthoSize = ApplyOutlineDistanceFadeOut(orthoSize);
			cameraMulFix = orthoSize * 50; 
		}

		return cameraMulFix * 0.00005; 
	}

	float4 NiloGetNewClipPosWithZOffset(float4 originalPositionCS, float viewSpaceZOffsetAmount)
	{
		if(unity_OrthoParams.w == 0)
		{
			////////////////////////////////
			//Perspective camera case
			////////////////////////////////
			float2 ProjM_ZRow_ZW = UNITY_MATRIX_P[2].zw;
			float modifiedPositionVS_Z = -originalPositionCS.w + -viewSpaceZOffsetAmount; // push imaginary vertex
			float modifiedPositionCS_Z = modifiedPositionVS_Z * ProjM_ZRow_ZW[0] + ProjM_ZRow_ZW[1];
			originalPositionCS.z = modifiedPositionCS_Z * originalPositionCS.w / (-modifiedPositionVS_Z); // overwrite positionCS.z
			return originalPositionCS;    
		}
		else
		{
			////////////////////////////////
			//Orthographic camera case
			////////////////////////////////
			originalPositionCS.z += -viewSpaceZOffsetAmount / _ProjectionParams.z; // push imaginary vertex and overwrite positionCS.z
			return originalPositionCS;
		}
	}

	float3 TransformPositionWSToOutlinePositionWS(float3 positionWS, float positionVS_Z, float3 normalWS)
	{
		//you can replace it to your own method! Here we will write a simple world space method for tutorial reason, it is not the best method!
		float outlineExpandAmount = _OutlineWidth * GetOutlineCameraFovAndDistanceFixMultiplier(positionVS_Z);

		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(UNITY_STEREO_DOUBLE_WIDE_ENABLED)
		outlineExpandAmount *= 0.5;
		#endif
	    
		return positionWS + normalWS * outlineExpandAmount; 
	}

    Varyings Outline_Vert(Attributes v)
	{
		Varyings o;

		UNITY_SETUP_INSTANCE_ID(v);                 // will turn into this in non OpenGL and non PSSL -> UnitySetupInstanceID(input.instanceID);
		UNITY_TRANSFER_INSTANCE_ID(v, o);      // will turn into this in non OpenGL and non PSSL -> output.instanceID = input.instanceID;
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);  // will turn into this in non OpenGL and non PSSL -> output.stereoTargetEyeIndexAsRTArrayIdx = unity_StereoEyeIndex;
		
        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.positionOS.xyz);
		VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);
		
    	float3 positionWS = TransformPositionWSToOutlinePositionWS(PositionInputs.positionWS, PositionInputs.positionVS.z, vertexNormalInput.normalWS);
		o.uv = v.uv;

		o.normalWS = vertexNormalInput.normalWS;
		o.positionCS = TransformWorldToHClip(positionWS);
		
		float outlineZOffsetMaskTexExplictMipLevel = 0;
		float outlineZOffsetMask = tex2Dlod(_OutlineZOffsetMaskTex, float4(v.uv,0,outlineZOffsetMaskTexExplictMipLevel)).r; 
		outlineZOffsetMask = 1-outlineZOffsetMask;
		outlineZOffsetMask = invLerpClamp(_OutlineZOffsetMaskRemapStart,_OutlineZOffsetMaskRemapEnd,outlineZOffsetMask);// allow user to flip value or remap
		
		o.positionCS = NiloGetNewClipPosWithZOffset(o.positionCS, _OutlineZOffset * outlineZOffsetMask + 0.03 * _isFace);
		
		return o;
	}


    half4 Outline_Frag (Varyings  i) : SV_Target
    {
    	float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        half3 color =  baseColor.rgb * _OutlineColor;
    	
    	return half4(color, 1);
    }


#endif //OUTLINE_INCLUDED
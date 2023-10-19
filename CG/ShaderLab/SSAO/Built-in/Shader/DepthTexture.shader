Shader "Unlit/SSAO"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            float4x4 _FrustumCornersRay;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            sampler2D _CameraDepthTexture;
	        float4x4 _CurrentViewProjectionInverseMatrix;
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half2 uv_depth : TEXCOORD1;
                float4 interpolatedRay : TEXCOORD2;
            };
 
            v2f vert(appdata_img v)
            {
            	v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv_depth = v.texcoord;
        	
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				{
					o.uv_depth.y = 1 - o.uv_depth.y;
				}
				#endif

        		int index = 0;
				if (v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
				{
					index = 0;
				}
				else if(v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
				{
					index = 1;
				}else if(v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
				{
					index = 2;
				}
				else
				{
					index = 3;
				}

        		#if UNITY_UV_STARTS_AT_TOP
        		if (_MainTex_TexelSize.y < 0)
				{
					index = 3 - index;
				}
        		#endif

				o.interpolatedRay = _FrustumCornersRay[index];
				return o;
            }
 

 
            fixed4 frag(v2f i):SV_Target
            {
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				float linearDepth = Linear01Depth(depth);
				float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.interpolatedRay.xyz;

				return fixed4(linearDepth, linearDepth, linearDepth, 1.0);
            }
            ENDCG
        }
    }
	FallBack Off

}

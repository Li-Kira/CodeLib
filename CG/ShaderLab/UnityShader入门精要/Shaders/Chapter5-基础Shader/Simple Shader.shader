// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Learning/Chapter5/Simple Shader"
{
   Properties {
       //声明一个Color类型的属性
		_Color ("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;
            };
            
            
            v2f vert (a2v v) : POSITION{
                v2f o;
                o.pos  = UnityObjectToClipPos(v.vertex);
                o.color = v.normal * 0.5 + fixed3(0.5, 0.5, 0.5);
                return  o;
            }
            
            
             fixed4 frag(v2f i) : SV_Target {
                 fixed3 c = i.color;
                 c *= _Color.rgb;
                 return fixed4(c, 1.0);
             }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            
            // struct appdata {
            //     float4 vertex : POSITION;
            //     float3 normal : NORMAL;
            //     float4 texcoord : TEXCOORD0;
            // };
            //
            // struct v2f
            // {
            //     float4 pos : SV_POSITION;
            //     fixed3 color : COLOR0;
            // };
            //
            //
            // float4 vert (appdata v) : POSITION{
            //     return UnityObjectToClipPos(v.vertex);
            // }
            //
            //
            //  fixed4 frag(v2f i) : SV_Target {
            //      return fixed4(1.0,1.0,1.0,1.0);
            //  }


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            // struct v2f
            // {
            //     float4 pos : SV_POSITION;
            //     fixed3 color : COLOR0;
            // };
            //
            //
            //
            //  float4 vert (float4 v : POSITION) : SV_POSITION{
            //      return UnityObjectToClipPos(v);
            // }
            //
            //
            // fixed4 frag(v2f i) : SV_Target {
            //     return fixed4(1.0,1.0,1.0,1.0);
            // }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



            

            ENDCG
        }
    }
}

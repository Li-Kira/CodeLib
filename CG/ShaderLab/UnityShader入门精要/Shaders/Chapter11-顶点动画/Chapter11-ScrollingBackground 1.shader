Shader "Shader Learning/Chapter11/Chapter11-ScrollingBackground"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DetailTex ("2nd Layer", 2D) = "white" {}
        _ScollX ("Base layer scroll speed", Float) = 1.0
        _Scoll2X ("2nd layer scroll speed", Float) = 1.0
        _Multiplier ("Layer Multiplier", Float) =1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DetailTex;
            float4 _DetailTex_ST;
            float _ScollX;
            float _Scoll2X;
            float _Multiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex) + frac(float2(_ScollX, 0.0) * _Time.y);
                o.uv.zw = TRANSFORM_TEX(v.uv.zw, _DetailTex) + frac(float2(_Scoll2X, 0.0) * _Time.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 firstLayer = tex2D(_MainTex, i.uv);
                fixed4 secondLayer = tex2D(_DetailTex, i.uv);

                fixed4 col = lerp(firstLayer, secondLayer, secondLayer.a);
                col.rgb *= _Multiplier; 

                
                return col;
            }
            ENDCG
        }
    }
}

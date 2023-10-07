Shader "Shader Learning/Chapter6/Chapter6-DiffuseVertexLevel"
{
    Properties
    {
        _Diffuse ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 color : COLOR;
            };
            
            fixed4 _Diffuse;

            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 获取环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

                // 将法线方向从物体空间转为剪裁空间
                fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                // 获取世界空间下的光线方向
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                
                // 计算漫反射
                fixed3 diffuse =  _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLight));
                o.color = ambient + diffuse;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // return col;

                return fixed4(i.color, 1.0);
                
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}

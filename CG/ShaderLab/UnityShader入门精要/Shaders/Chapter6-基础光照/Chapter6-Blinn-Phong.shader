Shader "Shader Learning/Chapter6/Chapter6-Blinn-Phong"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1.0, 1.0, 1.0, 1.0)
        _Specular ("Specular", Color) = (1.0, 1.0, 1.0, 1.0)
        _Gloss ("", Range(8.0, 256)) = 20
        //高光区域大小
    }
    SubShader
    {
        Tags { "LightMode" = "ForwardBase" }
        LOD 100

        Pass
        {
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
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.worldNomal = mul(v.normal, (float3x3)unity_WorldToObject);
                //使用内置函数来计算世界空间中的法线向量
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                 // 获取环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                
                // 将法线方向从物体空间转为剪裁空间
                fixed3 worldNormal = normalize(i.worldNormal);
                // 获取世界空间下的光线方向
                fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                //fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                //计算漫反射系数
                fixed3 diffuse =  _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLight));

                //从世界空间获取反射方向
                fixed3 reflectDir = normalize(reflect(-worldLight, worldNormal));
                //从世界空间获取视角方向
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                //fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                //从世界空间获取半方向
                fixed3 halfDir = normalize(worldLight + viewDir);
                //计算高光系数
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal, halfDir)),_Gloss);
                
                fixed3 color = ambient + diffuse + specular;

                
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    
    FallBack "Specular"
}

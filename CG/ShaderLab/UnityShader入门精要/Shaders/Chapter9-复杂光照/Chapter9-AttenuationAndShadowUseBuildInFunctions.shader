Shader "Shader Learning/Chapter9/Chapter9-AttenuationAndShadowUseBuildInFunctions"
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
        Tags { "RenderType"="Opaque" }
        
        LOD 100

        Pass
        {
            // Base Shader
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Apparently need to add this declaration 
            #pragma multi_compile_fwdbase	
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //使用内置函数来计算世界空间中的法线向量
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                // Pass shadow coordinates to pixel shader
			 	TRANSFER_SHADOW(o);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                 // 获取环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                
                // 将法线方向从物体空间转为剪裁空间
                fixed3 worldNormal = normalize(i.worldNormal);
                // 获取世界空间下的光线方向
                // fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                //计算漫反射系数
                fixed3 diffuse =  _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal,worldLight));

                //从世界空间获取反射方向
                fixed3 reflectDir = normalize(reflect(-worldLight, worldNormal));
                //从世界空间获取视角方向
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                //fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                //从世界空间获取半方向
                fixed3 halfDir = normalize(worldLight + viewDir);
                //计算高光系数
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal, halfDir)),_Gloss);

                
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                fixed3 color = ambient + (diffuse + specular) * atten ;
                
                
                return fixed4(color, 1.0);
            }
            ENDCG
        }
        
        
        Pass
        {
            // Additional Shader
            Tags { "LightMode" = "ForwardAdd" }
            
            Blend One One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Apparently need to add this declaration
			#pragma multi_compile_fwdadd
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //使用内置函数来计算世界空间中的法线向量
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                 // 获取环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                
                // 将法线方向从物体空间转为剪裁空间
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                
                
                
                //计算漫反射
                fixed3 diffuse =  _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal,worldLightDir));
                
                //从世界空间获取视角方向
                //fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                //从世界空间获取半方向
                fixed3 halfDir = normalize(worldLightDir + viewDir);
                //计算高光系数
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0,dot(worldNormal, halfDir)),_Gloss);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                
                fixed3 color = ambient + (diffuse + specular) * atten;
                return fixed4(color, 1.0);
            }
            ENDCG
        }
        
    }
    
    FallBack "Specular"
}

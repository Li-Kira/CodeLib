#ifndef TEMPLATE_INCLUDED
#define TEMPLATE_INCLUDED


    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


    CBUFFER_START(UnityPerMaterial)
    CBUFFER_END

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);


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
    };

    Varyings Vert(Attribute v)
    {
        Varyings o;

        VertexPositionInputs  PositionInputs = GetVertexPositionInputs(v.position.xyz);
        VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normal);

        o.position = PositionInputs.positionCS;
        o.uv = v.uv;

        return o;
    }

    half4 Frag(Varyings i) : SV_Target
    {
        half4 color = half4(1, 1, 1, 1);
        return color;
    }


#endif //TEMPLATE_INCLUDED
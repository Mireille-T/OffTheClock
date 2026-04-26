Shader "Custom/Toon"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _ShadeColor ("Shade Color", Color) = (0.2,0.2,0.2,1)
        _Threshold ("Shade Threshold", Range(0,1)) = 0.5
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0,0.05)) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ToonPass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 normalWS     : TEXCOORD0;
                float2 uv           : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float4 _ShadeColor;
            float _Threshold;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 normal = normalize(i.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);

                float NdotL = dot(normal, lightDir);

                float shadeStep = step(_Threshold, NdotL);

                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                float4 color = lerp(_ShadeColor, _BaseColor, shadeStep) * tex;

                return color;
            }
            ENDHLSL
        }

        // Outline Pass
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite On
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 normal = normalize(TransformObjectToWorldNormal(v.normalOS));
                float3 pos = TransformObjectToWorld(v.positionOS.xyz);

                pos += normal * _OutlineThickness;
                o.positionHCS = TransformWorldToHClip(pos);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
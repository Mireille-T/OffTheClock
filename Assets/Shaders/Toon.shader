Shader "Custom/Toon"
{
    Properties
    {
        _ColorHighlight("Color Highlight", Color) = (1,1,1,1)
        _RangeHighlight("Range Highlight", Range(0,1)) = 0.8
        _Color("Color", Color) = (0.9, 0.9, 0.9, 1)
        _ColorShaded("Color Shaded", Color) = (0.6, 0.6, 0.6, 1)
        _RangeShaded("Range Shaded", Range(0,1)) = 0.5
        _ColorReflected("Color Reflected", Color) = (0.7, 0.7, 0.7, 1)
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _ColorBackLight("Color BackLight", Color) = (0.8, 0.8, 0.8, 1)
        _RangeBackLight("Range BackLight", Range(0,1)) = 0.8

        [Toggle(USE_CUSTOM_LIGHT_DIR)] _UseCustomLightDir("Use CustomLightDireciton", Int) = 1
        _CustomLightDir("CustomLightDirection", Vector) = (5,2,-3,0)

        _EdgeSmooth("Edge Smoothness", Range(0,0.5)) = 0.1

        _SDFTex("SDF Texture", 2D) = "gray" {}
        [Toggle] _InvertSDF("Invert SDF?", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _ USE_CUSTOM_LIGHT_DIR

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
            };

            struct Varying
            {
                float4 positionCS : SV_POSITION;
                float3 values : TEXCOORD1; // r: nl, g: nv, b: dist
                float4 screenPos : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_SDFTex);
            SAMPLER(sampler_SDFTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _SDFTex_ST;

            half4 _ColorHighlight;
            half4 _Color;
            half4 _ColorShaded;
            half4 _ColorReflected;
            half4 _ColorBackLight;
            half4 _ColorRimLight;

            half _RangeHighlight;
            half _RangeShaded;
            half _RangeBackLight;
            half _RangeRimLight;

            half3 _CustomLightDir;

            half _Smoothness;

            half _EdgeSmooth;

            half _InvertSDF;
            CBUFFER_END

            Varying vert (Attributes IN)
            {
                Varying OUT = (Varying) 0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = vertexInput.positionCS;

                half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);

                float3 normalWS = mul((float3x3)unity_ObjectToWorld, IN.normal).xyz;
                half3 lightDir = _MainLightPosition.xyz;
                #if defined(USE_CUSTOM_LIGHT_DIR)
                    // to calculate custom light direction in view direction space.
                    half3 viewRightDir = cross(viewDirWS, half3(0,1,0));
                    half3 viewUpDir = cross(viewRightDir, viewDirWS);
                    half3x3 viewToWorld = half3x3(viewRightDir, viewUpDir, viewDirWS);

                    lightDir = mul(viewToWorld, _CustomLightDir);
                    lightDir = normalize(lightDir);
                #endif
                OUT.values.r = dot(normalWS, lightDir); // nl
                OUT.values.g = dot(normalWS, viewDirWS); // nv
                // Camera to Object Distance to keep sdf pattern size the same.
                OUT.values.b = distance(_WorldSpaceCameraPos.xyz, float3(UNITY_MATRIX_M._14, UNITY_MATRIX_M._24, UNITY_MATRIX_M._34));

                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag (Varying IN) : SV_Target
            {
                float nl = IN.values.r;
                float rim = 1 - max(0, IN.values.g);
                float edge = nl * (rim+0.3); // avoid highlight/backlight appears too much.

                // Pattern
                float2 screenUV = (IN.screenPos.xy / IN.screenPos.w) * IN.values.b; // Keep pattern size multiplied by the distance.(cam to object)
                screenUV.x *= _ScreenParams.x/_ScreenParams.y; // Apply screen ratio to keep the pattern ratio.
                half sdfTexture = SAMPLE_TEXTURE2D(_SDFTex, sampler_SDFTex, screenUV * _SDFTex_ST.xy).r;
                if(!_InvertSDF)
                {
                    sdfTexture = 1-sdfTexture;
                }

                // Highlight
                half highlight = smoothstep(_RangeHighlight, _RangeHighlight + _EdgeSmooth, edge);
                highlight = step(sdfTexture, highlight);

                // Shaded
                half shaded = smoothstep(_RangeShaded, _RangeShaded - _EdgeSmooth, nl);
                shaded = step(sdfTexture, shaded);

                // Reflected Light
                half rangeReflected = _Smoothness - 1;
                half reflected = smoothstep(rangeReflected, rangeReflected - _EdgeSmooth, nl);
                reflected = step(sdfTexture, reflected);

                // BackLight
                half rangeBackLight = (_RangeBackLight - 1);
                half backlight = smoothstep(rangeBackLight, rangeBackLight - _EdgeSmooth, edge);
                backlight = step(sdfTexture, backlight);

                // Mix
                half4 color = _Color;
                color = lerp(lerp(lerp(lerp(color, _ColorHighlight, highlight), _ColorShaded, shaded), _ColorReflected, reflected), _ColorBackLight, backlight);

                return color;
            }
            ENDHLSL
        }
    }
}
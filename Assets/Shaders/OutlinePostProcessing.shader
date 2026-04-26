Shader "Custom/OutlinePostProcessing"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 10)) = 1
        _DepthSensitivity("Depth Sensitivity", Range(0, 50)) = 10
        _NormalSensitivity("Normal Sensitivity", Range(0, 10)) = 1
        _EdgeThreshold("Edge Threshold", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "OutlinePass"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _OutlineColor;
            float _OutlineThickness;
            float _DepthSensitivity;
            float _NormalSensitivity;
            float _EdgeThreshold;

            float3 ReconstructWorldPos(float2 uv, float depth)
            {
                float4 clipPos = float4(uv * 2.0 - 1.0, depth, 1.0);
                #if UNITY_UV_STARTS_AT_TOP
                    clipPos.y = -clipPos.y;
                #endif
                float4 worldPos = mul(UNITY_MATRIX_I_VP, clipPos);
                return worldPos.xyz / worldPos.w;
            }

            float3 ReconstructNormalFromDepth(float2 uv, float2 texelSize)
            {
                float depthC = SampleSceneDepth(uv);
                float depthL = SampleSceneDepth(uv + float2(-texelSize.x, 0));
                float depthR = SampleSceneDepth(uv + float2(texelSize.x, 0));
                float depthU = SampleSceneDepth(uv + float2(0, texelSize.y));
                float depthD = SampleSceneDepth(uv + float2(0, -texelSize.y));
                float3 posC = ReconstructWorldPos(uv, depthC);
                float3 posL = ReconstructWorldPos(uv + float2(-texelSize.x, 0), depthL);
                float3 posR = ReconstructWorldPos(uv + float2(texelSize.x, 0), depthR);
                float3 posU = ReconstructWorldPos(uv + float2(0, texelSize.y), depthU);
                float3 posD = ReconstructWorldPos(uv + float2(0, -texelSize.y), depthD);
                float3 dx = (posR - posL) * 0.5;
                float3 dy = (posU - posD) * 0.5;
                return normalize(cross(dy, dx));
            }

            float DetectDepthEdge(float2 uv, float2 texelSize)
            {
                float sobelX = 0.0;
                float sobelY = 0.0;
                float sobelXWeights[9] = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
                float sobelYWeights[9] = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
                int index = 0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        float depth = LinearEyeDepth(SampleSceneDepth(uv + offset), _ZBufferParams);
                        sobelX += depth * sobelXWeights[index];
                        sobelY += depth * sobelYWeights[index];
                        index++;
                    }
                }
                return sqrt(sobelX * sobelX + sobelY * sobelY);
            }

            float DetectNormalEdge(float2 uv, float2 texelSize)
            {
                float3 normalC = ReconstructNormalFromDepth(uv, texelSize);
                float3 normalL = ReconstructNormalFromDepth(uv + float2(-texelSize.x, 0), texelSize);
                float3 normalR = ReconstructNormalFromDepth(uv + float2(texelSize.x, 0), texelSize);
                float3 normalU = ReconstructNormalFromDepth(uv + float2(0, texelSize.y), texelSize);
                float3 normalD = ReconstructNormalFromDepth(uv + float2(0, -texelSize.y), texelSize);
                float edgeX = length(normalR - normalL);
                float edgeY = length(normalU - normalD);
                return (edgeX + edgeY) * 0.5;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 originalColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float2 texelSize = _OutlineThickness * float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float centerDepth = SampleSceneDepth(input.texcoord);
                if (centerDepth >= 0.99999)
                    return originalColor;
                float depthEdge = DetectDepthEdge(input.texcoord, texelSize) * _DepthSensitivity;
                float normalEdge = DetectNormalEdge(input.texcoord, texelSize) * _NormalSensitivity;
                float combinedEdge = max(depthEdge, normalEdge);
                combinedEdge = smoothstep(_EdgeThreshold, _EdgeThreshold + 0.05, combinedEdge);
                combinedEdge = saturate(combinedEdge * 2.0);
                float3 finalColor = lerp(originalColor.rgb, _OutlineColor.rgb, combinedEdge);
                return float4(finalColor, originalColor.a);
            }

            ENDHLSL
        }
    }
}
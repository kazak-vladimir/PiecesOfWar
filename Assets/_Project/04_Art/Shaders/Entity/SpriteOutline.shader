Shader "Custom/URP/SpriteOutlineUnlit"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 20)) = 1.0
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "SpriteOutline"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4 _Color;
                half4 _OutlineColor;
                half _OutlineWidth;
                half _AlphaThreshold;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                
                return output;
            }

            // Optimized outline detection using 4-way sampling
            half GetOutlineAlpha(float2 uv)
            {
                half2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
                
                // Sample in 4 cardinal directions
                half alpha = 0.0;
                alpha = max(alpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(texelSize.x, 0)).a);
                alpha = max(alpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-texelSize.x, 0)).a);
                alpha = max(alpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, texelSize.y)).a);
                alpha = max(alpha, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, -texelSize.y)).a);
                
                return alpha;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample main texture
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                mainColor *= input.color;
                
                // If pixel is already opaque enough, just return the main color
                if (mainColor.a > _AlphaThreshold)
                {
                    return mainColor;
                }
                
                // Check neighboring pixels for outline
                half outlineAlpha = GetOutlineAlpha(input.uv);
                
                // Only draw outline where main pixel is transparent but neighbors are opaque
                half isOutline = step(_AlphaThreshold, outlineAlpha) * (1.0 - step(_AlphaThreshold, mainColor.a));
                
                // Blend between main color and outline
                half4 finalColor = lerp(mainColor, _OutlineColor, isOutline);
                finalColor.a = max(mainColor.a, outlineAlpha * isOutline * _OutlineColor.a);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Unlit"
}

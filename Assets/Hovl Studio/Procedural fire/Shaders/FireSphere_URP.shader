Shader "URP/Particles/FireSphere"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Emission("Emission", Float) = 2
        _StartFrequency("Start Frequency", Float) = 4
        _Frequency("Frequency", Float) = 10
        _Amplitude("Amplitude", Float) = 1
        [Toggle]_Usedepth("Use depth?", Float) = 0
        _Depthpower("Depth power", Float) = 1
        [Toggle]_Useblack("Use black", Float) = 0
        _Opacity("Opacity", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent"  
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType"="Plane"
        }
        
        LOD 100
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0; // xy = uv, z = custom data, w = unused
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float3 uv : TEXCOORD0; // xyz for custom UV handling
                float4 screenPos : TEXCOORD1;
                float fogCoord : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Emission;
                float _StartFrequency;
                float _Frequency;
                float _Amplitude;
                float _Usedepth;
                float _Depthpower;
                float _Useblack;
                float _Opacity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);
                
                output.color = input.color;
                output.uv = float3(input.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw, input.uv.z);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            // Procedural noise function
            float ProceduralNoise2D(float2 uv, float frequency)
            {
                float2 animatedUV = (float2(0.2, 0) * _Time.y + uv + uv.z) * frequency;
                float2 floorUV = floor(animatedUV);
                float2 fractUV = frac(animatedUV);
                
                // Smoothstep interpolation
                float2 smoothUV = fractUV * fractUV * (3.0 - 2.0 * fractUV);
                
                // Hash function for noise
                float cellBase = floorUV.x + floorUV.y * 57.0;
                
                float n0 = frac(473.5 * sin(cellBase));
                float n1 = frac(473.5 * sin(cellBase + 1.0));
                float n2 = frac(473.5 * sin(cellBase + 57.0));
                float n3 = frac(473.5 * sin(cellBase + 58.0));
                
                float nx0 = lerp(n0, n1, smoothUV.x);
                float nx1 = lerp(n2, n3, smoothUV.x);
                
                return lerp(nx0, nx1, smoothUV.y);
            }

            float ProceduralNoise3D(float3 uv, float frequency)
            {
                float3 animatedUV = (float3(float2(0.5, 0.5) * _Time.y, 0.0) + uv + uv.z) * frequency;
                float3 floorUV = floor(animatedUV);
                float3 fractUV = frac(animatedUV);
                
                // Smoothstep interpolation
                float3 smoothUV = fractUV * fractUV * (3.0 - 2.0 * fractUV);
                
                // Simplified 3D noise
                float cellBase = floorUV.x + floorUV.y * 57.0;
                
                float n0 = frac(473.5 * sin(cellBase));
                float n1 = frac(473.5 * sin(cellBase + 1.0));
                float n2 = frac(473.5 * sin(cellBase + 57.0));
                float n3 = frac(473.5 * sin(cellBase + 58.0));
                
                float nx0 = lerp(n0, n1, smoothUV.x);
                float nx1 = lerp(n2, n3, smoothUV.x);
                float result = lerp(nx0, nx1, smoothUV.y);
                
                return result;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Generate first noise for distortion
                float noise1 = ProceduralNoise2D(input.uv.xy, _StartFrequency);
                
                // Distort UVs with first noise
                float3 distortedUV = input.uv + (noise1 * _Amplitude);
                
                // Generate second noise
                float noise2 = ProceduralNoise3D(distortedUV, _Frequency);
                
                // Sample main texture with distorted UVs
                float2 finalUV = input.uv.xy + (0.2 * noise2 * _Amplitude);
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, finalUV);
                
                // Calculate emission color
                half4 emissionColor = _Emission * _Color * input.color;
                
                // Apply "use black" toggle
                half4 finalEmission = lerp(emissionColor, emissionColor * mainTex, _Useblack);
                
                // Calculate alpha
                half4 alphaBase = input.color.a * mainTex * _Opacity;
                alphaBase = saturate(alphaBase);
                
                // Depth fade
                half depthFade = 1.0;
                if (_Usedepth > 0.5)
                {
                    float2 screenUV = input.screenPos.xy / input.screenPos.w;
                    
                    #if UNITY_REVERSED_Z
                        real depth = SampleSceneDepth(screenUV);
                    #else
                        real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(screenUV));
                    #endif
                    
                    float sceneZ = LinearEyeDepth(depth, _ZBufferParams);
                    float thisZ = LinearEyeDepth(input.screenPos.z / input.screenPos.w, _ZBufferParams);
                    float fadeDistance = abs(sceneZ - thisZ) / _Depthpower;
                    depthFade = saturate(fadeDistance);
                }
                
                half4 finalColor;
                finalColor.rgb = finalEmission.rgb;
                finalColor.a = alphaBase.r * depthFade;
                
                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Particles/Unlit"
}


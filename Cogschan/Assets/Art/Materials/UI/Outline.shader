Shader "Custom/UI/Outline"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _HorizontalOffset("HorizontalOffset", Float) = 0.05
        _VerticalOffset("VerticalOffset", Float) = 0.05
        _OutlineColor("OutlineColor", Color) = (0.1882353, 0.2579783, 0.8117647, 1)
        _AlphaCutoff("AlphaCutoff", Float) = 0.2
        _FadeDistance("FadeDistance", Float) = 0.5
        _FadeStart("FadeStart", Float) = 0.3
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _HorizontalOffset;
        float _VerticalOffset;
        float4 _OutlineColor;
        float _AlphaCutoff;
        float _FadeDistance;
        float _FadeStart;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        struct Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float
        {
        };
        
        void SG_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float(float _Cutoff, float4 _Color, Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float IN, out float4 Color_1)
        {
        float4 _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0 = _Color;
        float _Split_a4128434860a4993bc158e98857412dd_R_1 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[0];
        float _Split_a4128434860a4993bc158e98857412dd_G_2 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[1];
        float _Split_a4128434860a4993bc158e98857412dd_B_3 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[2];
        float _Split_a4128434860a4993bc158e98857412dd_A_4 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[3];
        float _Property_ebe0bb210a9c4d2b9b5acd3b03b62b2a_Out_0 = _Cutoff;
        float _Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2;
        Unity_Comparison_LessOrEqual_float(_Split_a4128434860a4993bc158e98857412dd_A_4, _Property_ebe0bb210a9c4d2b9b5acd3b03b62b2a_Out_0, _Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2);
        float4 _Property_b58206dd93ee403580c2d12bc424a1e2_Out_0 = _Color;
        float4 _Branch_659071cf245c46849c6019a9c4f45061_Out_3;
        Unity_Branch_float4(_Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2, float4(0, 0, 0, 0), _Property_b58206dd93ee403580c2d12bc424a1e2_Out_0, _Branch_659071cf245c46849c6019a9c4f45061_Out_3);
        Color_1 = _Branch_659071cf245c46849c6019a9c4f45061_Out_3;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_70ea0b010cc24275900fbdb6184e8d6e_Out_0 = _AlphaCutoff;
            UnityTexture2D _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.tex, _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.samplerstate, _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_R_4 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.r;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_G_5 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.g;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_B_6 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.b;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_A_7 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.a;
            Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0;
            float4 _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1;
            SG_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float(_Property_70ea0b010cc24275900fbdb6184e8d6e_Out_0, _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0, _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0, _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1);
            float4 _Property_f86c0ba631f14d17adcbfbafb720e7f4_Out_0 = _OutlineColor;
            UnityTexture2D _Property_1d440f30c2334771b839197b81b3c52b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_42591f27abfc4275809892617caef2e0_Out_0 = _HorizontalOffset;
            float2 _Vector2_789d01e3223346a996c6f70d5f86836e_Out_0 = float2(_Property_42591f27abfc4275809892617caef2e0_Out_0, 0);
            float2 _TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_789d01e3223346a996c6f70d5f86836e_Out_0, _TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3);
            float4 _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1d440f30c2334771b839197b81b3c52b_Out_0.tex, _Property_1d440f30c2334771b839197b81b3c52b_Out_0.samplerstate, _Property_1d440f30c2334771b839197b81b3c52b_Out_0.GetTransformedUV(_TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3));
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_R_4 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.r;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_G_5 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.g;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_B_6 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.b;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_A_7 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.a;
            UnityTexture2D _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2;
            Unity_Multiply_float_float(_Property_42591f27abfc4275809892617caef2e0_Out_0, -1, _Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2);
            float2 _Vector2_e4be7ab435694e8ba713d886ea3f4e30_Out_0 = float2(_Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2, 0);
            float2 _TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_e4be7ab435694e8ba713d886ea3f4e30_Out_0, _TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3);
            float4 _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.tex, _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.samplerstate, _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.GetTransformedUV(_TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3));
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_R_4 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.r;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_G_5 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.g;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_B_6 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.b;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_A_7 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.a;
            float _Add_c576055925134cb882b963b36fef2a81_Out_2;
            Unity_Add_float(_SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_A_7, _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_A_7, _Add_c576055925134cb882b963b36fef2a81_Out_2);
            UnityTexture2D _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0 = _VerticalOffset;
            float2 _Vector2_d61d1978ebf3421c98888d0ac5562bd5_Out_0 = float2(0, _Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0);
            float2 _TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_d61d1978ebf3421c98888d0ac5562bd5_Out_0, _TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3);
            float4 _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0 = SAMPLE_TEXTURE2D(_Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.tex, _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.samplerstate, _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.GetTransformedUV(_TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3));
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_R_4 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.r;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_G_5 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.g;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_B_6 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.b;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_A_7 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.a;
            UnityTexture2D _Property_1001754cdaea4a4980d0320834184dd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2;
            Unity_Multiply_float_float(_Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0, -1, _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2);
            float2 _Vector2_c955c897eabb4b2fa7f05f703146e63c_Out_0 = float2(0, _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2);
            float2 _TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_c955c897eabb4b2fa7f05f703146e63c_Out_0, _TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3);
            float4 _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1001754cdaea4a4980d0320834184dd3_Out_0.tex, _Property_1001754cdaea4a4980d0320834184dd3_Out_0.samplerstate, _Property_1001754cdaea4a4980d0320834184dd3_Out_0.GetTransformedUV(_TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3));
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_R_4 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.r;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_G_5 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.g;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_B_6 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.b;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_A_7 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.a;
            float _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2;
            Unity_Add_float(_SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_A_7, _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_A_7, _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2);
            float _Add_5b06ab53361147e8af16b4944f257265_Out_2;
            Unity_Add_float(_Add_c576055925134cb882b963b36fef2a81_Out_2, _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2, _Add_5b06ab53361147e8af16b4944f257265_Out_2);
            float _Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3;
            Unity_Clamp_float(_Add_5b06ab53361147e8af16b4944f257265_Out_2, 0, 1, _Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3);
            float _Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2;
            Unity_Subtract_float(_Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3, _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_A_7, _Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2);
            float4 _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2;
            Unity_Multiply_float4_float4(_Property_f86c0ba631f14d17adcbfbafb720e7f4_Out_0, (_Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2.xxxx), _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2);
            float4 _Add_9116b7bce65146f986e2b06935575f59_Out_2;
            Unity_Add_float4(_AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1, _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2, _Add_9116b7bce65146f986e2b06935575f59_Out_2);
            float4 _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0 = IN.uv0;
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_R_1 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[0];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_G_2 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[1];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_B_3 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[2];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_A_4 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[3];
            float _Property_b26ce27c0339484da6790c3200679d87_Out_0 = _FadeStart;
            float _Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2;
            Unity_Subtract_float(_Split_5f6d87a9d9744e0597c7bab17886ece6_G_2, _Property_b26ce27c0339484da6790c3200679d87_Out_0, _Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2);
            float _Property_d593dcab2e4948529746991ff757e358_Out_0 = _FadeDistance;
            float _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2;
            Unity_Divide_float(1, _Property_d593dcab2e4948529746991ff757e358_Out_0, _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2);
            float2 _Vector2_b427f764e55b4fa5924cf2471dc1788e_Out_0 = float2(0, _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2);
            float _Remap_790a3e84ac13451b982aac1161d74b74_Out_3;
            Unity_Remap_float(_Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2, float2 (0, 1), _Vector2_b427f764e55b4fa5924cf2471dc1788e_Out_0, _Remap_790a3e84ac13451b982aac1161d74b74_Out_3);
            float _Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3;
            Unity_Clamp_float(_Remap_790a3e84ac13451b982aac1161d74b74_Out_3, 0, 1, _Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3);
            float _Split_09c0887e23634a2c9e14f156c406020b_R_1 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[0];
            float _Split_09c0887e23634a2c9e14f156c406020b_G_2 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[1];
            float _Split_09c0887e23634a2c9e14f156c406020b_B_3 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[2];
            float _Split_09c0887e23634a2c9e14f156c406020b_A_4 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[3];
            float _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2;
            Unity_Multiply_float_float(_Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3, _Split_09c0887e23634a2c9e14f156c406020b_A_4, _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2);
            surface.BaseColor = (_Add_9116b7bce65146f986e2b06935575f59_Out_2.xyz);
            surface.Alpha = _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _HorizontalOffset;
        float _VerticalOffset;
        float4 _OutlineColor;
        float _AlphaCutoff;
        float _FadeDistance;
        float _FadeStart;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }
        
        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }
        
        struct Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float
        {
        };
        
        void SG_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float(float _Cutoff, float4 _Color, Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float IN, out float4 Color_1)
        {
        float4 _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0 = _Color;
        float _Split_a4128434860a4993bc158e98857412dd_R_1 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[0];
        float _Split_a4128434860a4993bc158e98857412dd_G_2 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[1];
        float _Split_a4128434860a4993bc158e98857412dd_B_3 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[2];
        float _Split_a4128434860a4993bc158e98857412dd_A_4 = _Property_09d70bf8d3254e90970bac56d19fb0c3_Out_0[3];
        float _Property_ebe0bb210a9c4d2b9b5acd3b03b62b2a_Out_0 = _Cutoff;
        float _Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2;
        Unity_Comparison_LessOrEqual_float(_Split_a4128434860a4993bc158e98857412dd_A_4, _Property_ebe0bb210a9c4d2b9b5acd3b03b62b2a_Out_0, _Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2);
        float4 _Property_b58206dd93ee403580c2d12bc424a1e2_Out_0 = _Color;
        float4 _Branch_659071cf245c46849c6019a9c4f45061_Out_3;
        Unity_Branch_float4(_Comparison_6ff7d1c7a35243d290a924f86cdc987b_Out_2, float4(0, 0, 0, 0), _Property_b58206dd93ee403580c2d12bc424a1e2_Out_0, _Branch_659071cf245c46849c6019a9c4f45061_Out_3);
        Color_1 = _Branch_659071cf245c46849c6019a9c4f45061_Out_3;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_70ea0b010cc24275900fbdb6184e8d6e_Out_0 = _AlphaCutoff;
            UnityTexture2D _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.tex, _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.samplerstate, _Property_c3e1794e8b8745e9b091f0bbbdee4881_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_R_4 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.r;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_G_5 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.g;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_B_6 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.b;
            float _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_A_7 = _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0.a;
            Bindings_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0;
            float4 _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1;
            SG_AlphaCleanup_823c8844e3661dc4eaa1b111b42016ae_float(_Property_70ea0b010cc24275900fbdb6184e8d6e_Out_0, _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_RGBA_0, _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0, _AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1);
            float4 _Property_f86c0ba631f14d17adcbfbafb720e7f4_Out_0 = _OutlineColor;
            UnityTexture2D _Property_1d440f30c2334771b839197b81b3c52b_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_42591f27abfc4275809892617caef2e0_Out_0 = _HorizontalOffset;
            float2 _Vector2_789d01e3223346a996c6f70d5f86836e_Out_0 = float2(_Property_42591f27abfc4275809892617caef2e0_Out_0, 0);
            float2 _TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_789d01e3223346a996c6f70d5f86836e_Out_0, _TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3);
            float4 _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1d440f30c2334771b839197b81b3c52b_Out_0.tex, _Property_1d440f30c2334771b839197b81b3c52b_Out_0.samplerstate, _Property_1d440f30c2334771b839197b81b3c52b_Out_0.GetTransformedUV(_TilingAndOffset_fa508ac7c13445be82fd8d6ab2231265_Out_3));
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_R_4 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.r;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_G_5 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.g;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_B_6 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.b;
            float _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_A_7 = _SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_RGBA_0.a;
            UnityTexture2D _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2;
            Unity_Multiply_float_float(_Property_42591f27abfc4275809892617caef2e0_Out_0, -1, _Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2);
            float2 _Vector2_e4be7ab435694e8ba713d886ea3f4e30_Out_0 = float2(_Multiply_505c16ff5c154fff86762e8a1c7acb92_Out_2, 0);
            float2 _TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_e4be7ab435694e8ba713d886ea3f4e30_Out_0, _TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3);
            float4 _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0 = SAMPLE_TEXTURE2D(_Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.tex, _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.samplerstate, _Property_d6d6ffb2590e46b0823ffe920c924259_Out_0.GetTransformedUV(_TilingAndOffset_4ac15aaaf97c4832a381d2b6a3e1e3a4_Out_3));
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_R_4 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.r;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_G_5 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.g;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_B_6 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.b;
            float _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_A_7 = _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_RGBA_0.a;
            float _Add_c576055925134cb882b963b36fef2a81_Out_2;
            Unity_Add_float(_SampleTexture2D_12d465c03b284d0783aae2d7d98c6f31_A_7, _SampleTexture2D_6d1b240cbc154c7788bac5c2fae90917_A_7, _Add_c576055925134cb882b963b36fef2a81_Out_2);
            UnityTexture2D _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0 = _VerticalOffset;
            float2 _Vector2_d61d1978ebf3421c98888d0ac5562bd5_Out_0 = float2(0, _Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0);
            float2 _TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_d61d1978ebf3421c98888d0ac5562bd5_Out_0, _TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3);
            float4 _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0 = SAMPLE_TEXTURE2D(_Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.tex, _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.samplerstate, _Property_fc0f60b739ab4a20ba908cc226f0bb24_Out_0.GetTransformedUV(_TilingAndOffset_411c63cecfa547afa29d6bc35717286e_Out_3));
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_R_4 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.r;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_G_5 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.g;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_B_6 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.b;
            float _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_A_7 = _SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_RGBA_0.a;
            UnityTexture2D _Property_1001754cdaea4a4980d0320834184dd3_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2;
            Unity_Multiply_float_float(_Property_ad306cf88dd44c35aa4f770f6baa2a91_Out_0, -1, _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2);
            float2 _Vector2_c955c897eabb4b2fa7f05f703146e63c_Out_0 = float2(0, _Multiply_9d4dc9a746524e84a53dac4b9b99c35e_Out_2);
            float2 _TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_c955c897eabb4b2fa7f05f703146e63c_Out_0, _TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3);
            float4 _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1001754cdaea4a4980d0320834184dd3_Out_0.tex, _Property_1001754cdaea4a4980d0320834184dd3_Out_0.samplerstate, _Property_1001754cdaea4a4980d0320834184dd3_Out_0.GetTransformedUV(_TilingAndOffset_faddbdbee3b9414fbffaf989c730f57a_Out_3));
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_R_4 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.r;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_G_5 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.g;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_B_6 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.b;
            float _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_A_7 = _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_RGBA_0.a;
            float _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2;
            Unity_Add_float(_SampleTexture2D_8f900170d0374f59ac4dd8b2f6cb7d32_A_7, _SampleTexture2D_745e3c1735814c12a781e3eaf0584d1d_A_7, _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2);
            float _Add_5b06ab53361147e8af16b4944f257265_Out_2;
            Unity_Add_float(_Add_c576055925134cb882b963b36fef2a81_Out_2, _Add_ce4493dacd0e4b159935870fa6b4ef4f_Out_2, _Add_5b06ab53361147e8af16b4944f257265_Out_2);
            float _Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3;
            Unity_Clamp_float(_Add_5b06ab53361147e8af16b4944f257265_Out_2, 0, 1, _Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3);
            float _Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2;
            Unity_Subtract_float(_Clamp_dba0adebd67a4974a7c322c5e17175d9_Out_3, _SampleTexture2D_8625b0bf5c214e85884a65ee9eb61ab3_A_7, _Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2);
            float4 _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2;
            Unity_Multiply_float4_float4(_Property_f86c0ba631f14d17adcbfbafb720e7f4_Out_0, (_Subtract_ccafd416b2574b44b112fa4afc80538a_Out_2.xxxx), _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2);
            float4 _Add_9116b7bce65146f986e2b06935575f59_Out_2;
            Unity_Add_float4(_AlphaCleanup_f28b7ef0573e4a038c5b12ec680db1b0_Color_1, _Multiply_b4762b14087e46eebda5d7c45b1b2361_Out_2, _Add_9116b7bce65146f986e2b06935575f59_Out_2);
            float4 _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0 = IN.uv0;
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_R_1 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[0];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_G_2 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[1];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_B_3 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[2];
            float _Split_5f6d87a9d9744e0597c7bab17886ece6_A_4 = _UV_f107f55fe13a46b6a163d00b00d38c82_Out_0[3];
            float _Property_b26ce27c0339484da6790c3200679d87_Out_0 = _FadeStart;
            float _Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2;
            Unity_Subtract_float(_Split_5f6d87a9d9744e0597c7bab17886ece6_G_2, _Property_b26ce27c0339484da6790c3200679d87_Out_0, _Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2);
            float _Property_d593dcab2e4948529746991ff757e358_Out_0 = _FadeDistance;
            float _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2;
            Unity_Divide_float(1, _Property_d593dcab2e4948529746991ff757e358_Out_0, _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2);
            float2 _Vector2_b427f764e55b4fa5924cf2471dc1788e_Out_0 = float2(0, _Divide_659acd80d76b4e38b245ecbc72c801be_Out_2);
            float _Remap_790a3e84ac13451b982aac1161d74b74_Out_3;
            Unity_Remap_float(_Subtract_76c9a3608632490fb9dbc3383dbe2fca_Out_2, float2 (0, 1), _Vector2_b427f764e55b4fa5924cf2471dc1788e_Out_0, _Remap_790a3e84ac13451b982aac1161d74b74_Out_3);
            float _Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3;
            Unity_Clamp_float(_Remap_790a3e84ac13451b982aac1161d74b74_Out_3, 0, 1, _Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3);
            float _Split_09c0887e23634a2c9e14f156c406020b_R_1 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[0];
            float _Split_09c0887e23634a2c9e14f156c406020b_G_2 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[1];
            float _Split_09c0887e23634a2c9e14f156c406020b_B_3 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[2];
            float _Split_09c0887e23634a2c9e14f156c406020b_A_4 = _Add_9116b7bce65146f986e2b06935575f59_Out_2[3];
            float _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2;
            Unity_Multiply_float_float(_Clamp_3b9e925de82943dd8bfb812eeb9717d6_Out_3, _Split_09c0887e23634a2c9e14f156c406020b_A_4, _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2);
            surface.BaseColor = (_Add_9116b7bce65146f986e2b06935575f59_Out_2.xyz);
            surface.Alpha = _Multiply_f8c1e1ab1e20493f991459dcdf68eb91_Out_2;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
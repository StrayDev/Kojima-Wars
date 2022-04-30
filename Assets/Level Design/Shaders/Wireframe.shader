Shader "Custom/Wireframe"
{
    Properties
    {
        [MainColor] _MainColor("Surface Color", Color) = (1,1,1,1)
        _Alpha("Transparency", Range(0, 1)) = 1.0
        _WireFrameColor("WireFrame Color", Color) = (1,1,1,1)
        [Toggle] _NoSurf("Toggle surface Color", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        LOD 200

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

         // Mainly used for SRP
        CBUFFER_START(UnityPerMaterial)
            float4 _MainColor;
            float _Alpha;
            float4 _WireFrameColor;
            float _NoSurf;
        CBUFFER_END

        ENDHLSL

         Pass
         {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Cull off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha


            HLSLPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo

            // For general use in most surface shaders
            struct standard_vertex_data						// Also known as Attributes by Unity
            {
                float4 positionOS   : POSITION;
                half3 normal        : NORMAL;
                float2 uv : TEXCOORD1;
                float4 tangent : TEXCOORD2;
                float3 world_position : TEXCOORD3;
            };
            // For general use in most surface shaders
            struct standard_vertex_output					// Also known as Varying by Unity
            {
                float4 positionHCS  : SV_POSITION;
                half3 normal        : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 tangent : TEXCOORD2;
                float3 world_position : TEXCOORD3;
            };

            standard_vertex_output vert(standard_vertex_data IN)
            {
                VertexPositionInputs position_inputs = GetVertexPositionInputs(IN.positionOS.xyz);

                standard_vertex_output output;
                output.positionHCS = position_inputs.positionCS;
                output.normal = TransformObjectToWorldNormal(IN.normal);
                output.uv = IN.uv;
                output.tangent = float4(TransformObjectToWorldDir(IN.tangent.xyz), IN.tangent.w);
                output.world_position = position_inputs.positionWS;
                return output;
            }

            struct geo_data
            {
                standard_vertex_output data;
                float2 baryCentricCoords : TEXCOORD4;
            };

            [maxvertexcount(3)]
            void geo(triangle standard_vertex_output i[3], inout TriangleStream<geo_data> stream)
            {
                // Get world position of the three vertices
                const float3 p0 = i[0].world_position.xyz;
                const float3 p1 = i[1].world_position.xyz;
                const float3 p2 = i[2].world_position.xyz;

                // Calculate the triangle normal 
                const float3 triangle_normal = normalize(cross(p1 - p0, p2 - p0));

                geo_data g0, g1, g2;
                g0.data = i[0];
                g1.data = i[1];
                g2.data = i[2];

                // Give each vertex a barycentric coord
                g0.baryCentricCoords = float2(1.0f, 0.0f);
                g1.baryCentricCoords = float2(0.0f, 1.0f);
                g2.baryCentricCoords = float2(0.0f, 0.0f);

                // Replace vertex normals with triangle normals
                i[0].normal = triangle_normal;
                i[1].normal = triangle_normal;
                i[2].normal = triangle_normal;

                // Append vertex data to stream
                stream.Append(g0);
                stream.Append(g1);
                stream.Append(g2);
            }

            float4 frag(geo_data input) : SV_Target
            {
                float3 barys;
                barys.xy = input.baryCentricCoords;
                barys.z = 1.0f - barys.x - barys.y;

                float min_bary = min(barys.x, min(barys.y, barys.z));
                float detla = fwidth(min_bary);
                min_bary = smoothstep(0.0f, detla, min_bary);

                float4 output_with_surf = float4(lerp(_WireFrameColor.rgb, _MainColor.rgb, min_bary), _Alpha);
                float4 output_without_surf = float4(_WireFrameColor.rgb, 1.0f - min_bary);

                return lerp(output_without_surf, output_with_surf, _NoSurf);
            }
            ENDHLSL
         }
    }
    FallBack "Diffuse"
}

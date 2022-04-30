Shader "Custom/unitShader"
{
 	Properties 
 	{
 		[HideInInspector][MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
 		[HideInInspector][MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        
	    [HideInInspector][Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping", Float) = 0
		[HideInInspector]_Cutoff ("Alpha Cutoff", Float) = 0.5
 		
	    _MainTex ("Texture", 2D) = "white" {}
 		_BackColor ("Background colour", Color) = (0.5, 0.5, 0.5, 1.0)
 		_FrontColor ("Line Colour", Color) = (1, 1, 1, 1)
 		_Scale	("General scale", float)	= 10.0 // 1 meter in unit is around 10 in hlsl. In Unreal its 100
 		[Toggle] _xyToggle ("Toggle xy size", float) = 1.0
 		[Toggle] _yzToggle ("Toggle yz size", float) = 1.0
 		[Toggle] _zxToggle ("Toggle zx size", float) = 1.0
	    _Blending ("Axis blending amount", Range(0, 1)) = 1.0
 		_Roughness ("Roughness", Range(0, 1)) = 0.5
 		_SpecIntensity ("Specular Intensity", Range(0, 1)) = 0.5

    }
 	SubShader 
 	{
 		Tags 
 		{ 
 			"RenderPipeline" = "UniversalPipeline"
 			"RenderType" = "Geometry"
 			"Queue" = "Geometry"
 		}
 		LOD 200
 		
 		HLSLINCLUDE
 		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 		#include "Assets/Environment/Kojima Island/Materials/Shaders/Includes/BRDF.cginc"

		TEXTURE2D(_MainTex);
 		SAMPLER(sampler_MainTex);

 		// Mainly used for SRP
 		CBUFFER_START(UnityPerMaterial)
 		float4 _MainTex_ST;
 		float4 _BackColor;
 		float4 _FrontColor;

 		float4 _BaseMap_ST;
 		float4 _BaseColor;
 		float _AlphaTestToggle;
 		float _Cutoff;
 		
 		float _Scale;
 		float _xyToggle;
 		float _yzToggle;
 		float _zxToggle;
 		float _Blending;
 		float _Roughness;
 		float _SpecIntensity;
 		CBUFFER_END
 		
 		ENDHLSL
 		
 		Pass
 		{
 			Tags
 			{
 				"LightMode"="UniversalForward"
 			}
 			CULL BACK
 				
 			HLSLPROGRAM
 			#pragma vertex vert
 			#pragma fragment frag

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
 				output.positionHCS		= position_inputs.positionCS;
 				output.normal			= TransformObjectToWorldNormal(IN.normal);
 				output.uv				= TRANSFORM_TEX(IN.uv, _MainTex);
 				output.tangent			= float4(TransformObjectToWorldDir(IN.tangent.xyz), IN.tangent.w);
 				output.world_position	= position_inputs.positionWS;
 				return output;
 			}
 
 			half4 frag(standard_vertex_output input) : SV_Target
 			{
 				// Get lighting information from Unity
 				LightData light_data;
 				light_data.position = _MainLightPosition;
 				light_data.color = _MainLightColor;
 				
				float3 uvw = input.world_position.xyz / _Scale;
 				float3 blending = saturate(abs(input.world_position.xyz) - 0.3f);
 				blending /= dot(blending, float3(1.0f, 1.0f, 1.0f));

 				const half side_zy = lerp(0.0h, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvw.yz).r, blending.x);
 				const half side_zx = lerp(0.0h, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvw.zx).r, blending.y);
				const half side_xy = lerp(0.0h, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvw.xy).r, blending.z);

 				const half toggle_zy = lerp(0.0h, side_zy, _yzToggle);
 				const half toggle_zx = lerp(0.0h, side_zx, _zxToggle);
				const half toggle_xy = lerp(0.0h, side_xy, _xyToggle);

 				const half combine = toggle_zy + toggle_zx + toggle_xy;

 				const half4 blendColour = lerp(_BackColor, _FrontColor, combine);

				const half NdotL = saturate(dot(input.normal, light_data.position));
 				
 				half4 diffuse = blendColour * light_data.color;

 				// Create base specular using Cook-torrance BRDF
 				const half3 view_dir = GetWorldSpaceNormalizeViewDir(input.world_position);
 				const half3 specular = evaluateBRDF(view_dir.xyz, light_data.position.xyz, input.normal, _SpecIntensity, _Roughness);
				const half3 outgoingRadiance = (diffuse.rgb + specular) * NdotL;
 				
				const half3 ambient = blendColour * half3(0.4h, 0.4h, 0.4h);
 				const half3 colour = ambient + outgoingRadiance;
 				
 				return half4(colour, 1.0h);
 			}
 			ENDHLSL
 		}

 		Pass
 		{
 			Name "ShadowCaster"
		    Tags { "LightMode"="ShadowCaster" }
		 
		    ZWrite On
		    ZTest LEqual
 			
 			HLSLPROGRAM
			#pragma vertex ShadowPassVertex
 			#pragma fragment ShadowPassFragment

 			#pragma shader_feature _ALPHATEST_ON
 			#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			    // GPU Instancing
		    #pragma multi_compile_instancing
		    #pragma multi_compile _ DOTS_INSTANCING_ON
		             
		    #pragma vertex ShadowPassVertex
		    #pragma fragment ShadowPassFragment
		     
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
 			ENDHLSL
        }
 	}
 	FallBack "Diffuse"
}

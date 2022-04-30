Shader "Custom/Water"
{
 	Properties 
 	{
 		[Header(Water Appearance)]
 		[Space(10)]
 		[HDR][MainColor] _Color ("Color", Color) = (1, 1, 1, 1)
 		_Specular	("Specular intensity", Range(0,1))	= 0.5
 		_Glossy		("Water Glossyness", Range(0,1))	= 1.0
 
 		[Space(10)]
     	[Header(Fog appearance)]
     	[Space(10)]
 		[HDR] _FogColor	("Fog Color", Color)			= (0,0,0,0)
 		_FogThreshold	("Fog threshold", Range(0,100)) = 1.0
 		
 		[Space(10)]
     	[Header(Form)]
     	[Space(10)]
 		[HDR] _FormColor	("Form Color", Color)				= (1,1,1,1)
 		_IntersectThreshold	("Form threshold", Range(1,10))		= 1.0
 		_IntersectPower		("Form power", Range(1,4))			= 1.0
 		
 		[Space(10)]
     	[Header((XY direction) (Z steepness) (W wavelength))]
 		[Space(10)]
 		_Wave1 ("Wave 1", Vector) = (1.0, 1.0, 1.0, 1.25)
     	_Wave2 ("Wave 2", Vector) = (0.0, 1.0, 1.0, 1.25)
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
		Cull Off
 		
 		HLSLINCLUDE
 		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
		#include "Assets/Environment/Kojima Island/Materials/Shaders/Includes/BRDF.cginc"
 		
 		// Mainly used for SRP
 		CBUFFER_START(UnityPerMaterial)
 		float4 _Color;
 		float _Specular;
 		float _Glossy;
 		
 		float4 _FogColor;
 		float _FogThreshold;
 		
 		float4 _FormColor;
 		float _IntersectThreshold;
 		float _IntersectPower;
 		
 		float4 _Wave1;
 		float4 _Wave2;
 		
 		CBUFFER_END
 		
 		ENDHLSL
 		
 		Pass
 		{
 			Tags
 			{
 				"LightMode"="UniversalForward"
 			}
 			Cull Off
 				
 			HLSLPROGRAM
 			#pragma vertex vert
 			#pragma fragment frag

 			float3 gerstner_wave(float4 wave_data, float3 vertex)
 			{
 				// Unpack Wave properties
 				const float steepness = wave_data.z;
 				const float wavelength = wave_data.w;
 				float2 direction = normalize(wave_data.xy);
 
 				const float num_wave = 2.0f * PI / wavelength;
 				const float phase_speed = sqrt(9.8f / num_wave);
 
 				const float f = num_wave * (dot(wave_data, float4(vertex.x, 0.0f, vertex.z, 0.0f)) - phase_speed * _Time.y);
 				float amplitude = steepness / num_wave;
 				const float anchor = amplitude * cos(f);
 
 				return float3(direction.x * anchor, amplitude * sin(f), direction.y * anchor);
 			}
 
 			float3 generate_low_poly_normal(const float3 world_position)
 			{
 				const float3 world_pos_ddx = ddx(world_position);
 	    		const float3 world_pos_ddy = ddy(world_position);
 				// For some reason, ddy has to be inverted. I have no idea why it has to be like this
 				// but it works
 	    		return normalize(cross(world_pos_ddx, -world_pos_ddy));
 			}
 
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
 				float4 screenPos : TEXCOORD4;
 			};
 			
 			standard_vertex_output vert(standard_vertex_data IN)
 			{
 				// Assign vertex position object space xyz to temp p for displacing
 				float3 p = IN.positionOS.xyz;
 				p += gerstner_wave(_Wave1, IN.positionOS.xyz);
 				p += gerstner_wave(_Wave2, IN.positionOS.xyz);
 				IN.positionOS.xyz = p;
 				
 				VertexPositionInputs position_inputs = GetVertexPositionInputs(IN.positionOS.xyz);
 			
 				standard_vertex_output output;
 				output.positionHCS		= position_inputs.positionCS;
 				output.normal			= TransformObjectToWorldNormal(IN.normal);
 				output.uv				= IN.uv;
 				output.tangent			= float4(TransformObjectToWorldDir(IN.tangent.xyz), IN.tangent.w);
 				output.world_position	= position_inputs.positionWS;
 				output.screenPos		= ComputeScreenPos(position_inputs.positionCS);
 				return output;
 			}
 
 			float4 frag(standard_vertex_output input) : SV_Target
 			{
 				// invert Glossy to make more sense for artist when using it in editor
 				_Glossy = 1.0f - _Glossy;
 
 				// Get lighting information from Unity
 				LightData light_data;
 				light_data.position = _MainLightPosition;
 				light_data.color = _MainLightColor;
 
 				// Generate low-poly normals from the vertex world position using DDX and DDY
 	    		const float3 lowPoly_normal = generate_low_poly_normal(input.world_position);

 				const half3 view_dir = GetWorldSpaceNormalizeViewDir(input.world_position);
				const half n_dot_v = saturate(dot(lowPoly_normal, view_dir));
 				
				// Create base diffuse
				float3 diffuse = _Color.rgb * light_data.color.rgb;

 				// Create base specular using Cook-torrance BRDF
 				const float3 specular = evaluateBRDF(view_dir.xyz, light_data.position.xyz, lowPoly_normal, _Specular, _Glossy);

 				// Create a fresnel specular for applying with base specular
 				const float3 fresnel = fresnel_schlick(n_dot_v, float3(0.04f, 0.04f, 0.04f), 1.0f);
 				
 				// Sample camera depth
 				// For more info about depth in URP:
 				// https://www.cyanilux.com/tutorials/depth/#sample-depth-texture
				const float rawDepth = SampleSceneDepth(input.screenPos.xy / input.screenPos.w);
 				const float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);

 				// Underwater fog
 				const float depthDiff = saturate((sceneEyeDepth - input.screenPos.w) / _FogThreshold);
				diffuse = lerp(diffuse, _FogColor.rgb, depthDiff);

				// Generate form
				const float intersect = saturate((sceneEyeDepth - input.screenPos.w) / _IntersectThreshold);
				const float3 form = _FormColor.rgb * pow(1.0f - intersect, 4) * _IntersectPower;
				diffuse += form;
 				
 				return float4(diffuse + (fresnel + specular), 1.0f);
 			}
 			ENDHLSL
 		}
 	}
 	FallBack "Diffuse"
}

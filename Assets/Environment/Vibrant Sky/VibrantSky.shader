// Adapted from https://kelvinvanhoorn.com/2022/03/17/skybox-tutorial-part-1

Shader "olihewi/Vibrant Sky"
{
  Properties
  {
    [Header(This shader requires an active VibrantSkies component in the scene.)] [Space(10)]
    [Header(Sun)] [Space(5)]
    _SunRadius ("Sun Size", Range(0,1)) = 0.05

    [Header(Moon)] [Space(5)]
    _MoonRadius ("Moon Size", Range(0,1)) = 0.03
    _MoonExposure ("Moon Exposure", Range(-16,16)) = 0

    [Header(Sky)] [Space(5)]
    [NoScaleOffset] _SunZenithGrad ("Sky Colour Gradient", 2D) = "white" {}
    [NoScaleOffset] _ViewZenithGrad ("Horizon Haze Gradient", 2D) = "white" {}
    [NoScaleOffset] _SunViewGrad ("Sun Bloom Gradient", 2D) = "white" {}
    [NoScaleOffset] _StarsTexture ("Stars Texture", Cube) = "black" {}

    [Header(Stars)] [Space(5)]
    _StarExposure ("Star Exposure", Range(-16,16)) = 0
    _StarPower ("Star Power", Range(1,5)) = 1
    _Latitude ("Planet Latitude", Range(-90, 90)) = 0
    _StarSpeed ("Planet Rotation Speed", Float) = 0.001

    [Header(Clouds)] [Space(5)]
    [NoScaleOffset] _CloudTexture ("Cloud Texture", 2D) = "black" {}
    _CloudHeight ("Cloud Height", Range(0,1)) = 1.0
    _CloudOpacity ("Cloud Opacity", Range(0,1)) = 0.5
    _CloudMovement ("Cloud Movement", Vector) = (1.0,0.0,0.0,0.0)
  }
    SubShader
    {
      Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
      Cull Off ZWrite Off
      
      Pass
      {
        HLSLPROGRAM
        #pragma vertex Vertex
        #pragma fragment Fragment

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      
        struct Attributes
        {
          float4 posOS : POSITION;
        };
      
        struct v2f
        {
          float4 posCS : SV_POSITION;
          float3 viewDirWS : TEXCOORD0;
        };
      
        sampler2D _MainTex;
        float4 _MainTex_ST;
      
        v2f Vertex (Attributes i)
        {
          v2f o = (v2f)0;
          VertexPositionInputs vertexInput = GetVertexPositionInputs(i.posOS.xyz);
          o.posCS = vertexInput.positionCS;
          o.viewDirWS = vertexInput.positionWS;
          return o;
        }

        // Gradients
        TEXTURE2D(_SunZenithGrad);  SAMPLER(sampler_SunZenithGrad);
        TEXTURE2D(_ViewZenithGrad); SAMPLER(sampler_ViewZenithGrad);
        TEXTURE2D(_SunViewGrad);    SAMPLER(sampler_SunViewGrad);
        // Sun, Moon & Stars
        float3 _SunDir, _MoonDir;
        float _SunRadius, _MoonRadius;
        float _MoonExposure, _StarExposure;
        float _StarPower;
        float _Latitude, _StarSpeed;
        // Cubemaps
        TEXTURECUBE(_StarsTexture); SAMPLER(sampler_StarsTexture);
        // Clouds
        TEXTURE2D(_CloudTexture); SAMPLER(sampler_CloudTexture);
        float _CloudOpacity, _CloudHeight;
        float2 _CloudMovement;

        float SphereRayIntersect(float3 rayDir, float3 spherePos, float radius)
        {
          float3 oc = -spherePos;
          float b = dot(oc, rayDir);
          float c = dot(oc, oc) - radius * radius;
          float h = b * b - c;
          if (h < 0.0) return -1.0;
          h = sqrt(h);
          return -b-h;
        }
        float3x3 AngleAxis3x3(float angle, float3 axis)
        {
          float c, s;
          sincos(angle, s, c);

          float t = 1 - c;
          float x = axis.x;
          float y = axis.y;
          float z = axis.z;

          return float3x3(
              t * x * x + c, t * x * y - s * z, t * x * z + s * y,
              t * x * y + s * z, t * y * y + c, t * y * z - s * x,
              t * x * z - s * y, t * y * z + s * x, t * z * z + c
              );
        }
      
        float4 Fragment (v2f i) : SV_Target
        {
          float3 viewDir = normalize(i.viewDirWS);

          float sunViewDot = dot(_SunDir, viewDir);
          float sunZenithDot = _SunDir.y;
          float viewZenithDot = viewDir.y;
          float sunMoonDot = dot(_SunDir, _MoonDir);

          float sunViewDot01 = (sunViewDot + 1.0) * 0.5;
          float sunZenithDot01 = (sunZenithDot + 1.0) * 0.5;

          // Base Sky Colour
          float3 sunZenithColour = SAMPLE_TEXTURE2D(_SunZenithGrad, sampler_SunZenithGrad, float2(sunZenithDot01, 0.5)).rgb;
          
          // The Moon
          float moonIntersect = SphereRayIntersect(viewDir, _MoonDir, _MoonRadius);
          float moonMask = moonIntersect > -1 ? 1 : 0;
          float3 moonNormal = normalize(_MoonDir - viewDir * moonIntersect);
          float3 moonNdotL = saturate(dot(moonNormal, -_SunDir));
          float3 moonColour = moonMask * moonNdotL * exp2(_MoonExposure);

          // Horizon Haze
          float3 viewZenithColour = SAMPLE_TEXTURE2D(_ViewZenithGrad, sampler_ViewZenithGrad, float2(sunZenithDot01, 0.5F)).rgb;
          float viewZenithMask = pow(saturate(1.0 - viewZenithDot), 4);

          // Sun Bloom
          float3 sunViewColour = SAMPLE_TEXTURE2D(_SunViewGrad, sampler_SunViewGrad, float2(sunZenithDot01, 0.5F)).rgb;
          float sunViewMask = pow(saturate(sunViewDot), 10);
          
          // Clouds
          float2 cloudUV = (viewDir.xz / viewDir.y + _CloudMovement * _Time.y) * _CloudHeight;
          float cloudAlpha = SAMPLE_TEXTURE2D(_CloudTexture, sampler_CloudTexture, cloudUV).r * _CloudOpacity * max(viewZenithDot,0);
          sunViewMask *= 1.0F + cloudAlpha * 10.0F;

          // Final Sky Colour
          float3 skyColour = sunZenithColour + (viewZenithMask * viewZenithColour) + (sunViewMask * sunViewColour);

          // The Sun
          float sunMask = step(1 - _SunRadius * _SunRadius, sunViewDot) * (1 - moonMask);
          sunMask *= 1.0F - cloudAlpha;

          // Star Rotation
          float3x3 tiltRotation = AngleAxis3x3(PI * (_Latitude-90) / 180, float3(1,0,0));
          float3x3 spinRotation = AngleAxis3x3((0.75 - (_Time.y * fmod(_StarSpeed,1))) * 2 * PI, float3(0,1,0));
          float3 starUVW = mul(mul(spinRotation,tiltRotation), viewDir);
          
          
          // Stars
          float starStrength = (1 - sunViewDot01) * saturate(-sunZenithDot);
          float3 starColour = SAMPLE_TEXTURECUBE(_StarsTexture, sampler_StarsTexture, starUVW).rgb;
          starColour = pow(abs(starColour), _StarPower);
          starColour *= (1 - sunMask) * (1 - moonMask) * exp2(_StarExposure) * starStrength;
          starColour = max(starColour - cloudAlpha, 0);
          
          
          float4 o = float4(skyColour + sunMask + moonColour + starColour + cloudAlpha,1);
          return o;
        }
        ENDHLSL
    }
  }
}

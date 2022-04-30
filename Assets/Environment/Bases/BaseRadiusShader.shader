Shader "olihewi/BaseRadiusShader"
{
  Properties
  {
    _Colour ("Colour", Color) = (1,1,1,1)
    _Size ("Size", Float) = 1
  }
  SubShader
  {
    Tags
    {
      "Queue"="Transparent"
      "RenderType"="Transparent"
      "IgnoreProjector"="True"
    }
    LOD 100
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off

    Pass
    {
      CGPROGRAM
      #pragma vertex Vertex
      #pragma fragment Fragment

      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
      };

      struct v2f
      {
        float4 vertex : SV_POSITION;
        float4 screenPos : TEXCOORD1;
      };

      float4 _Colour;
      float _Size;
      UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

      v2f Vertex(appdata v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.screenPos = ComputeScreenPos(o.vertex);
        COMPUTE_EYEDEPTH(o.screenPos.z);
        return o;
      }

      float4 Fragment(v2f i) : SV_Target
      {
        // sample the texture
        float4 col = _Colour;
        float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
        float depth = sceneZ - i.screenPos.z;
        float intersect = saturate(abs(depth) / _Size);
        if (intersect > 0.1F) discard;
        //col.a *= pow(1 - intersect, 2) * 2;
        //col.a = intersect;
        return col;
      }
      ENDCG
    }
  }
}
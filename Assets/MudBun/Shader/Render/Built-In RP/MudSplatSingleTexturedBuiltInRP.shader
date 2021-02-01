/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

Shader "MudBun/Mud Splat Single-Textured (Built-In RP)"
{
  Properties
  {
    _MainTex("Albedo", 2D) = "white" {}
    _AlphaCutoutThreshold("Alpha Cutout Threshold", Range(0.0, 1.0)) = 0.5
    _Dithering("Dithering", Range(0.0, 1.0)) = 0.0
    _DitherTexture("Dither Texture", 2D) = "black"
    _DitherTextureSize("Dither TextureSize", Int) = 256
    [Toggle] _RandomDither("Random Dither", Int) = 0
  }
  SubShader
  {
    ZWrite On
    Cull Back
    Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }

    CGPROGRAM

    #define MUDBUN_BUILT_IN_RP
    #pragma multi_compile_instancing
    #pragma multi_compile _ MUDBUN_PROCEDURAL
    #pragma multi_compile _ MUDBUN_QUAD_SPLATS
    #pragma surface surf Standard vertex:vert addshadow fullforwardshadows
    #pragma target 3.5

    #include "UnityCG.cginc"

    #include "../../../Shader/Render/ShaderCommon.cginc"

    #if MUDBUN_VALID
      #include "../../../Shader/Render/SplatCommon.cginc"
    #endif

    void vert(inout Vertex i, out Input o)
    {
      UNITY_INITIALIZE_OUTPUT(Input, o);

      #if MUDBUN_VALID
        float3 tangentLs;
        float3 centerWs;
        float3 centerLs;
        float sdfValue;
        float3 normal2dLs;
        float3 normal2dWs;
        mudbun_splat_vert(i.id, i.vertex, o.localPos, i.normal, o.localNorm, i.tangent, tangentLs, centerWs, centerLs, i.color, o.emissionHash, o.metallicSmoothness, o.tex, o.texWeight, sdfValue, normal2dLs, normal2dWs);
      #endif
    }

    void surf(Input i, inout SurfaceOutputStandard o)
    {
      #ifndef MUDBUN_QUAD_SPLATS
        clip(0.5f - length(i.tex));
      #endif

      float2 uv = i.tex + 0.5f;
      i.texWeight.x += 0.01f;

      float4 color = tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex));

      float3 albedo = i.color.rgb * _Color.rgb * color.rgb;
      float alpha = i.color.a * _Color.a * color.a;
      float alphaThreshold;
      float2 screenPos = i.screenPos.xy * _ScreenParams.xy / (i.screenPos.w + kEpsilon);
      computeOpaqueTransparency(screenPos, i.localPos, i.emissionHash.a, _DitherTexture, _DitherTextureSize, _RandomDither > 0, _AlphaCutoutThreshold, _Dithering, alpha, alphaThreshold);
      clip(alpha - alphaThreshold);

      o.Albedo = albedo;
      o.Emission = float4(i.emissionHash.rgb, 1.0f) * _Emission;
      o.Metallic = i.metallicSmoothness.x * _Metallic;
      o.Smoothness = i.metallicSmoothness.y * _Smoothness;
    }

    ENDCG
  }

  CustomEditor "MudBun.MudSplatSingleTexturedMaterialEditor"
}

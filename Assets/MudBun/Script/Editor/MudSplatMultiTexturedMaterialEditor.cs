/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEditor;

namespace MudBun
{
  public class MudSplatMultiTexturedMaterialEditor : ShaderGUI
  {
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] aProp)
    {
      var _MainTex = FindProperty("_MainTex", aProp);
      editor.ShaderProperty(_MainTex, _MainTex.displayName);

      var _AlphaCutoutThreshold = FindProperty("_AlphaCutoutThreshold", aProp);
      editor.ShaderProperty(_AlphaCutoutThreshold, _AlphaCutoutThreshold.displayName);

      var _Dithering = FindProperty("_Dithering", aProp);
      editor.ShaderProperty(_Dithering, _Dithering.displayName);

      var _DitherTexture = FindProperty("_DitherTexture", aProp);
      editor.ShaderProperty(_DitherTexture, _DitherTexture.displayName);

      var _DitherTextureSize = FindProperty("_DitherTextureSize", aProp);
      editor.ShaderProperty(_DitherTextureSize, _DitherTextureSize.displayName);

      var _RandomDither = FindProperty("_RandomDither", aProp);
      editor.ShaderProperty(_RandomDither, _RandomDither.displayName);

      EditorGUILayout.Space();

      var _UseTex1 = FindProperty("_UseTex1", aProp);
      editor.ShaderProperty(_UseTex1, _UseTex1.displayName);
      if (_UseTex1.floatValue > 0.0f)
      {
        var _Tex1 = FindProperty("_Tex1", aProp);
        editor.ShaderProperty(_Tex1, _Tex1.displayName);
        EditorGUILayout.Space();
      }

      var _UseTex2 = FindProperty("_UseTex2", aProp);
      editor.ShaderProperty(_UseTex2, _UseTex2.displayName);
      if (_UseTex2.floatValue > 0.0f)
      {
        var _Tex2 = FindProperty("_Tex2", aProp);
        editor.ShaderProperty(_Tex2, _Tex2.displayName);
        EditorGUILayout.Space();
      }

      var _UseTex3 = FindProperty("_UseTex3", aProp);
      editor.ShaderProperty(_UseTex3, _UseTex3.displayName);
      if (_UseTex3.floatValue > 0.0f)
      {
        var _Tex3 = FindProperty("_Tex3", aProp);
        editor.ShaderProperty(_Tex3, _Tex3.displayName);
      }

      EditorGUILayout.Space();

      editor.RenderQueueField();
      editor.DoubleSidedGIField();
    }
  }
}


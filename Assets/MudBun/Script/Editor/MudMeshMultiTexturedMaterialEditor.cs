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
  public class MudMeshMultiTexturedMaterialEditor : ShaderGUI
  {
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] aProp)
    {
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

      var _UseTex0 = FindProperty("_UseTex0", aProp);
      editor.ShaderProperty(_UseTex0, _UseTex0.displayName);
      if (_UseTex0.floatValue > 0.0f)
      {
        var _MainTex = FindProperty("_MainTex", aProp);
        var _MainTexX = FindProperty("_MainTexX", aProp);
        var _MainTexY = FindProperty("_MainTexY", aProp);
        var _MainTexZ = FindProperty("_MainTexZ", aProp);
        editor.ShaderProperty(_MainTex, _MainTex.displayName);
        editor.ShaderProperty(_MainTexX, _MainTexX.displayName);
        editor.ShaderProperty(_MainTexY, _MainTexY.displayName);
        editor.ShaderProperty(_MainTexZ, _MainTexZ.displayName);
        EditorGUILayout.Space();
      }

      var _UseTex1 = FindProperty("_UseTex1", aProp);
      editor.ShaderProperty(_UseTex1, _UseTex1.displayName);
      if (_UseTex1.floatValue > 0.0f)
      {
        var _Tex1 = FindProperty("_Tex1", aProp);
        var _Tex1X = FindProperty("_Tex1X", aProp);
        var _Tex1Y = FindProperty("_Tex1Y", aProp);
        var _Tex1Z = FindProperty("_Tex1Z", aProp);
        editor.ShaderProperty(_Tex1, _Tex1.displayName);
        editor.ShaderProperty(_Tex1X, _Tex1X.displayName);
        editor.ShaderProperty(_Tex1Y, _Tex1Y.displayName);
        editor.ShaderProperty(_Tex1Z, _Tex1Z.displayName);
        EditorGUILayout.Space();
      }

      var _UseTex2 = FindProperty("_UseTex2", aProp);
      editor.ShaderProperty(_UseTex2, _UseTex2.displayName);
      if (_UseTex2.floatValue > 0.0f)
      {
        var _Tex2 = FindProperty("_Tex2", aProp);
        var _Tex2X = FindProperty("_Tex2X", aProp);
        var _Tex2Y = FindProperty("_Tex2Y", aProp);
        var _Tex2Z = FindProperty("_Tex2Z", aProp);
        editor.ShaderProperty(_Tex2, _Tex2.displayName);
        editor.ShaderProperty(_Tex2X, _Tex2X.displayName);
        editor.ShaderProperty(_Tex2Y, _Tex2Y.displayName);
        editor.ShaderProperty(_Tex2Z, _Tex2Z.displayName);
        EditorGUILayout.Space();
      }

      var _UseTex3 = FindProperty("_UseTex3", aProp);
      editor.ShaderProperty(_UseTex3, _UseTex3.displayName);
      if (_UseTex3.floatValue > 0.0f)
      {
        var _Tex3 = FindProperty("_Tex3", aProp);
        var _Tex3X = FindProperty("_Tex3X", aProp);
        var _Tex3Y = FindProperty("_Tex3Y", aProp);
        var _Tex3Z = FindProperty("_Tex3Z", aProp);
        editor.ShaderProperty(_Tex3, _Tex3.displayName);
        editor.ShaderProperty(_Tex3X, _Tex3X.displayName);
        editor.ShaderProperty(_Tex3Y, _Tex3Y.displayName);
        editor.ShaderProperty(_Tex3Z, _Tex3Z.displayName);
      }

      EditorGUILayout.Space();

      editor.RenderQueueField();
      editor.DoubleSidedGIField();
    }
  }
}




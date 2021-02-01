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
  public class MudSplatSingleTexturedMaterialEditor : ShaderGUI
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

      editor.RenderQueueField();
      editor.DoubleSidedGIField();
    }
  }
}


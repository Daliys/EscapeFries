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
  public class MudMeshSingleTexturedMaterialEditor : ShaderGUI
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

      EditorGUILayout.Space();

      editor.RenderQueueField();
      editor.DoubleSidedGIField();
    }
  }
}




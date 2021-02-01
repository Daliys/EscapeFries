/*****************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace MudBun
{
  public class ResourcesUtilEditor
  {
#if UNITY_EDITOR
    // default init/fallback materials
    public static Material DefaultMeshMaterial => DefaultMeshSingleTexturedMaterial;
    public static Material DefaultSplatMaterial => DefaultSplatSingleTexturedMaterial;

    // default materials
    public static Material DefaultMeshSingleTexturedMaterial => GetMaterial(PathUtil.DefaultMeshSingleTexturedMaterial);
    public static Material DefaultSplatSingleTexturedMaterial => GetMaterial(PathUtil.DefaultSplatSingleTexturedMaterial);
    public static Material DefaultMeshMultiTexturedMaterial => GetMaterial(PathUtil.DefaultMeshMultiTexturedMaterial);
    public static Material DefaultSplatMultiTexturedMaterial => GetMaterial(PathUtil.DefaultSplatMultiTexturedMaterial);

    // preset mesh materials
    public static Material StopmotionMeshMaterial => GetMaterial(PathUtil.StopmotionMeshMaterial);

    // preset splat materials
    public static Material BrushStrokesSplatMaterial => GetMaterial(PathUtil.BrushStrokesSplatMaterial);
    public static Material FloaterSplatMaterial => GetMaterial(PathUtil.FloaterSplatMaterial);
    public static Material FloofSplatMaterial => GetMaterial(PathUtil.FloofSplatMaterial);
    public static Material LeafSplatMaterial => GetMaterial(PathUtil.LeafSplatMaterial);
    public static Material StopmotionSplatMaterial => GetMaterial(PathUtil.StopmotionSplatMaterial);

    public static Material GetMaterial(string path)
    {
      var mat = AssetDatabase.LoadAssetAtPath<Material>($"Assets/{path}.mat");
      if (mat == null)
        Debug.LogError($"MudBun: Cannot load renderer material at \"{path}.mat\". Did you forget to import \"{PathUtil.RenderPipelineFull}\"?");
      return mat;
    }
#endif
  }
}


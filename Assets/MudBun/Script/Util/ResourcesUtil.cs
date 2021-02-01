/*****************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace MudBun
{
  public class ResourcesUtil
  {
    public static ComputeShader VoxelGen => GetComputeResource(PathUtil.VoxelGen);
    public static ComputeShader MarchingCubes => GetComputeResource(PathUtil.MarchingCubes);
    public static ComputeShader DualMeshing => GetComputeResource(PathUtil.DualMeshing);
    public static ComputeShader SurfaceNets => GetComputeResource(PathUtil.SurfaceNets);
    public static ComputeShader DualContouring => GetComputeResource(PathUtil.DualContouring);
    public static ComputeShader NoiseCache => GetComputeResource(PathUtil.NoiseCache);
    public static ComputeShader MeshLock => GetComputeResource(PathUtil.MeshLock);

    public static ComputeShader GetComputeResource(string resourcePath)
    {
      var compute = Resources.Load<ComputeShader>(resourcePath);
      if (compute == null)
        Debug.LogError($"MudBun: Compute shader \"{PathUtil.ResourceRoot}/{resourcePath}.compute\" not found.");
      return compute;
    }

    // this needs to be available at run-time
    public static Material DefaultLockedMeshMaterial => GetMaterialResource(PathUtil.DefaultLockedMeshMaterial);

    public static Material GetMaterialResource(string resourcePath)
    {
      var compute = Resources.Load<Material>(resourcePath);
      if (compute == null)
        Debug.LogError($"MudBun: Material \"{PathUtil.ResourceRoot}/{resourcePath}.mat\" not found.");
      return compute;
    }
  }
}


/******************************************************************************/
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
  [ExecuteInEditMode]
  public class MudStandardMeshRenderer : MonoBehaviour
  {
    public void OnEnable()
    {
      var meshRenderer = GetComponent<MeshRenderer>();
      if (meshRenderer != null)
      {
        var material = meshRenderer.sharedMaterial;
        meshRenderer.sharedMaterial = material;
      }

      var skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
      if (skinnedMeshRenderer != null)
      {
        var material = skinnedMeshRenderer.sharedMaterial;
        skinnedMeshRenderer.sharedMaterial = material;
      }
    }
  }
}


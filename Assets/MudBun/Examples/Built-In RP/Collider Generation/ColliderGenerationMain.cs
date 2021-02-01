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
  public class ColliderGenerationMain : MonoBehaviour
  {
    private Mesh m_colliderMesh;

    private void Update()
    {
      var renderer = GetComponent<MudRenderer>();
      if (renderer == null)
        return;

      if (!renderer.IsMeshGenerationPending(m_colliderMesh))
        m_colliderMesh = renderer.AddCollider(renderer.gameObject, true, m_colliderMesh);
    }
  }
}


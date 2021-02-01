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
  public class Rotator : MonoBehaviour
  {
    public Vector3 Speed = Vector3.zero;

    private void Update()
    {
      Quaternion q = Quaternion.Euler(Speed * Time.deltaTime);
      transform.rotation = q * transform.rotation;
    }
  }
}


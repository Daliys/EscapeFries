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
  public class MudNoiseScroller : MonoBehaviour
  {
    public Vector3 Speed = Vector3.zero;

    private void Update()
    {
      var noise = GetComponent<MudNoiseVolume>();
      if (noise != null)
      {
        noise.Offset += Speed * Time.deltaTime;
      }

      var curveSimple = GetComponent<MudCurveSimple>();
      if (curveSimple != null)
      {
        curveSimple.NoiseOffset += Speed.x * Time.deltaTime;
      }
    }
  }
}


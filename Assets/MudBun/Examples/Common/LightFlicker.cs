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
  [RequireComponent(typeof(Light))]
  public class LightFlicker : MonoBehaviour
  {
    public float MinIntensity = 0.9f;
    public float MaxIntensity = 1.1f;

    // Update is called once per frame
    private void Update()
    {
      var light = GetComponent<Light>();
      light.intensity = Random.Range(MinIntensity, MaxIntensity);
    }
  }
}


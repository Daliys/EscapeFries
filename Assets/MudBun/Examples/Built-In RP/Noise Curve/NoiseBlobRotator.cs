/******** **********************************************************************/
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
  public class NoiseBlobRotator : MonoBehaviour
  {
    public Vector3 Axis = Vector3.right;
    public float RotateSpeed = 0.1f;
    public float NoiseSpeed = 0.1f;
    public MudCurveSimple Curve;

    private void Update()
    {
      transform.rotation = Quaternion.AngleAxis(RotateSpeed * Time.deltaTime, Axis) * transform.rotation;

      if (Curve != null)
        Curve.NoiseOffset += NoiseSpeed * Time.deltaTime;
    }
  }
}


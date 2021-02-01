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
  public class TwisterMain : MonoBehaviour
  {
    public Transform CubeSolid;
    public Transform CubeSpace;
    public MudTwist Twist;

    public float Period = 1.0f;

    public float MaxRotateAngle = 45.0f;
    public float MaxTwistAngle = 180.0f;
    public float PhaseOffset = 90.0f;

    public void Update()
    {
      float phase = MathUtil.TwoPi * Time.time / Period;
      CubeSolid.rotation = Quaternion.AngleAxis(Mathf.Sin(phase) * MaxRotateAngle, Vector3.up);
      CubeSpace.rotation = Quaternion.AngleAxis(Mathf.Sin(phase) * MaxRotateAngle, Vector3.up);
      Twist.Angle = Mathf.Sin(phase + PhaseOffset * MathUtil.Deg2Rad) * MaxTwistAngle;
    }
  }
}


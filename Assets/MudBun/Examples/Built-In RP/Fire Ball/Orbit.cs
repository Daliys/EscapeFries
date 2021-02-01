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
  public class Orbit : MonoBehaviour
  {
    public float Radius = 5.0f;
    public float Speed = 1.0f;

    private void Update()
    {
      float theta = Speed * Time.time / Radius;
      float sin = Mathf.Sin(theta);
      float cos = Mathf.Cos(theta);
      transform.position = new Vector3(Radius * cos, 0.0f, Radius * sin);
      transform.rotation = Quaternion.LookRotation(new Vector3(-sin, 0.0f, cos));
    }
  }
}


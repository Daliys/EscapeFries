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
  public class BouncePuffMain : MonoBehaviour
  {
    public Transform Ball;
    public MudTorus Puff;

    public float Period = 1.0f;

    public float BounceHeight = 2.0f;

    public float MaxPuffRadius = 3.0f;
    public float MaxPuffSize = 1.0f;

    private float m_timer = 0.0f;

    public void Update()
    {
      m_timer += Time.deltaTime;
      m_timer = Mathf.Repeat(m_timer, Period);

      float t = m_timer / Period;

      float s = 2.0f * (t - 0.5f);
      Ball.position = ((1.0f - s * s) * BounceHeight + 0.5f) * Vector3.up;

      Puff.transform.localScale = new Vector3(MaxPuffRadius * (t + 0.2f) / 1.2f, 1.0f, MaxPuffRadius * (t + 0.2f) / 1.2f);

      if (t < 0.5f)
      {
        Puff.Radius = MaxPuffSize * ((t + 0.4f) / 0.9f);
      }
      else
      {
        Puff.Radius = MaxPuffSize * (1.0f - t) * 2.0f;
      }
    }
  }
}


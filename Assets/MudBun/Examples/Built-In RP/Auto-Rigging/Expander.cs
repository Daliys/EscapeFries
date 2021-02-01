/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;

using UnityEngine;

namespace MudBun
{
  public class Expander : MonoBehaviour
  {
    public float Period = 2.0f;
    public float Distance = 1.0f;

    private struct Mover
    {
      public Transform Transform;
      public Vector3 Origin;
      public Vector3 Direction;
    }
    private List<Mover> m_aMover;

    private void Start()
    {
      m_aMover = new List<Mover>();
      for (int i = 0; i < transform.childCount; ++i)
      {
        var child = transform.GetChild(i);
        if (child.transform.localPosition.sqrMagnitude < MathUtil.Epsilon)
          continue;

        m_aMover.Add
        (
          new Mover ()
          {
            Transform = child, 
            Origin = child.localPosition, 
            Direction = child.localPosition.normalized
          }
        );
      }
    }

    private float m_timer = 0.0f;
    private void Update()
    {
      m_timer = Mathf.Repeat(m_timer + Time.deltaTime, Period);
      float c = Mathf.Cos(MathUtil.TwoPi * m_timer / Period);
      float h = 0.5f * Distance;
      foreach (var mover in m_aMover)
      {
        mover.Transform.localPosition = mover.Origin + (h - h * c) * mover.Direction;
      }
    }
  }
}


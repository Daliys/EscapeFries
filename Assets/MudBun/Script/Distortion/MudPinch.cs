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
  public class MudPinch : MudDistortion
  {
    [SerializeField] private float m_radius = 0.5f;
    public float Radius { get => m_radius; set { m_radius = value; MarkDirty(); } }

    [SerializeField] private float m_depth = 1.0f;
    public float Depth { get => m_depth; set { m_depth = value; MarkDirty(); } }

    [SerializeField] [Range(0.0f, 1.0f)] private float m_amount = 1.0f;
    public float Amount { get => m_amount; set { m_amount = value; MarkDirty(); } }

    [SerializeField] [Range(1.0f, 10.0f)] private float m_strength = 2.0f;
    public float Strength { get => m_strength; set { m_strength = value; MarkDirty(); } }

    public override float MaxDistortion => Depth;

    public override Aabb Bounds
    {
      get
      {
        Vector3 posRs = PointRs(transform.position);
        Vector3 r = new Vector3(m_radius, Depth, m_radius);
        Aabb bounds = new Aabb(-r, new Vector3(r.x, 0.0f, r.z));
        bounds.Rotate(RotationRs(transform.rotation));
        bounds.Min += posRs;
        bounds.Max += posRs;
        return bounds;
      }
    }

    public override void SanitizeParameters()
    {
      base.SanitizeParameters();

      Validate.NonNegative(ref m_radius);
      Validate.Saturate(ref m_amount);
    }

    public override int FillComputeData(SdfBrush [] aBrush, int iStart, List<Transform> aBone)
    {
      SdfBrush brush = SdfBrush.New;
      brush.Type = (int) SdfBrush.TypeEnum.Pinch;
      brush.Radius = m_radius;
      brush.Data0.x = m_depth;
      brush.Data0.y = m_amount;
      brush.Data0.z = m_strength;
      aBrush[iStart] = brush;

      return 1;
    }

    public override void DrawSelectionGizmosRs()
    {
      base.DrawSelectionGizmosRs();

      GizmosUtil.DrawInvisibleCone(PointRs(transform.position) + VectorRs(-m_depth * transform.up), m_radius, m_depth, RotationRs(transform.rotation));
    }

    public override void DrawOutlineGizmosRs()
    {
      base.DrawOutlineGizmosRs();

      GizmosUtil.DrawWireCone(PointRs(transform.position) + VectorRs(-m_depth * transform.up), m_radius, m_depth, RotationRs(transform.rotation));
    }
  }
}

